using System;
using System.Collections.Generic;
using System.Linq;
using Source = UpdateControls.Correspondence.Factual.AST;
using Target = UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class Analyzer
    {
        private Source.Namespace _root;
        private List<Error> _errors = new List<Error>();

        private static Dictionary<Source.NativeType, Target.NativeType> _nativeTypeMap = new Dictionary<Source.NativeType, Target.NativeType>
        {
            { Source.NativeType.String, Target.NativeType.String },
            { Source.NativeType.Int,    Target.NativeType.Int },
            { Source.NativeType.Float,  Target.NativeType.Float },
            { Source.NativeType.Char,   Target.NativeType.Char },
            { Source.NativeType.Date,   Target.NativeType.Date },
            { Source.NativeType.Time,   Target.NativeType.Time }
        };

        private static Dictionary<Source.Cardinality, Target.Cardinality> _cardinalityMap = new Dictionary<Source.Cardinality, Target.Cardinality>()
        {
            { Source.Cardinality.Optional, Target.Cardinality.Optional },
            { Source.Cardinality.Many,     Target.Cardinality.Many },
            { Source.Cardinality.One,      Target.Cardinality.One }
            };

        public Analyzer(Source.Namespace root)
        {
            _root = root;
        }

        public List<Error> Errors
        {
            get { return _errors; }
        }

        public Target.Namespace Analyze()
        {
            Target.Namespace result = new Target.Namespace(_root.Identifier);

            foreach (Source.Fact fact in _root.Facts)
            {
                try
                {
                    AnalyzeFact(fact, result);
                }
                catch (CompilerException ex)
                {
                    _errors.Add(new Error(ex.Message, ex.LineNumber));
                }
            }

            if (_errors.Any())
                return null;
            else
                return result;
        }

        private void AnalyzeFact(Source.Fact fact, Target.Namespace result)
        {
            if (_root.Facts.Any(f => f != fact && f.Name == fact.Name))
                throw new CompilerException(string.Format("The fact \"{0}\" is defined more than once.", fact.Name), fact.LineNumber);

            Target.Class factClass = new Target.Class(fact.Name);
            result.AddClass(factClass);

            foreach (Source.FactMember member in fact.Members)
            {
                try
                {
                    if (fact.Members.Any(m => m != member && m.Name == member.Name))
                        throw new CompilerException(string.Format("The member \"{0}.{1}\" is defined more than once.", fact.Name, member.Name), member.LineNumber);
                    var field = member as Source.Field;
                    if (field != null)
                        AnalyzeField(factClass, field);
                    else
                    {
                        var query = member as Source.Query;
                        if (query != null)
                            AnalyzeQuery(factClass, fact, query);
                        else
                        {
                            var predicate = member as Source.Predicate;
                            if (predicate != null)
                                AnalyzePredicate(factClass, fact, predicate);
                        }
                    }
                }
                catch (CompilerException ex)
                {
                    _errors.Add(new Error(ex.Message, ex.LineNumber));
                }
            }
        }

        private void AnalyzeField(Target.Class factClass, Source.Field field)
        {
            Source.DataTypeNative dataTypeNative = field.Type as Source.DataTypeNative;
            if (dataTypeNative != null)
                AnalyzeFieldNative(field, dataTypeNative, factClass);
            else
            {
                Source.DataTypeFact dataTypeFact = field.Type as Source.DataTypeFact;
                if (dataTypeFact != null)
                    AnalyzeFieldFact(field, dataTypeFact, factClass);
            }
        }

        private void AnalyzeFieldNative(Source.Field field, Source.DataTypeNative dataTypeNative, Target.Class factClass)
        {
            factClass.AddField(new Target.Field(
                field.Name,
                _cardinalityMap[dataTypeNative.Cardinality],
                _nativeTypeMap[dataTypeNative.NativeType]));
        }

        private void AnalyzeFieldFact(Source.Field field, Source.DataTypeFact dataTypeFact, Target.Class factClass)
        {
            if (!_root.Facts.Any(f => f.Name == dataTypeFact.FactName))
                throw new CompilerException(string.Format("The fact type \"{0}\" is not defined.", dataTypeFact.FactName), field.LineNumber);

            factClass.AddPredecessor(new Target.Predecessor(
                field.Name,
                _cardinalityMap[dataTypeFact.Cardinality],
                dataTypeFact.FactName
            ));
        }

        private void AnalyzeQuery(Target.Class factClass, Source.Fact fact, Source.Query sourceQuery)
        {
            Target.Query targetQuery = GenerateTargetQuery(factClass, fact, sourceQuery.Name, sourceQuery.Sets);
            factClass.AddResult(new Target.Result(sourceQuery.FactName, targetQuery));
        }

        private void AnalyzePredicate(Target.Class factClass, Source.Fact fact, Source.Predicate predicate)
        {
            Source.Clause clause = predicate.Clauses.First();
            AnalyzeClause(factClass, fact, predicate, clause);
        }

        private void AnalyzeClause(Target.Class factClass, Source.Fact fact, Source.Predicate predicate, Source.Clause clause)
        {
            Target.Query targetQuery = GenerateTargetQuery(factClass, fact, predicate.Name, clause.Sets);
            factClass.AddCondition(new Target.Condition(
                clause.Existence == Source.ConditionModifier.Negative ?
                    Target.ConditionModifier.Negative :
                    Target.ConditionModifier.Positive,
                targetQuery));
        }
        private Target.Query GenerateTargetQuery(Target.Class factClass, Source.Fact fact, string queryName, IEnumerable<Source.Set> sets)
        {
            Target.Query targetQuery = new Target.Query(queryName);

            // Follow the chain of predecessors on both sides of each set.
            Source.Set firstSet = sets.First();
            Source.Fact priorType = JoinFirstSet(fact, targetQuery, firstSet);
            string priorName = firstSet.Name;
            foreach (Source.Set subsequentSet in sets.Skip(1))
            {
                priorType = JoinSubsequentSet(priorType, targetQuery, priorName, subsequentSet);
                priorName = subsequentSet.Name;
            }

            factClass.AddQuery(targetQuery);
            return targetQuery;
        }

        private Source.Fact JoinFirstSet(Source.Fact fact, Target.Query targetQuery, Source.Set sourceSet)
        {
            Source.Path parentPath = sourceSet.RightPath;
            Source.Path childPath = sourceSet.LeftPath;
            if (!parentPath.Absolute)
            {
                parentPath = sourceSet.LeftPath;
                childPath = sourceSet.RightPath;
            }
            int lineNumber = sourceSet.LineNumber;
            if (!parentPath.Absolute)
                throw new CompilerException("The query set needs to relate to \"this\".", lineNumber);
            if (childPath.Absolute)
                throw new CompilerException("Only one side of the equation can relate to \"this\".", lineNumber);
            if (childPath.RelativeTo != sourceSet.Name)
                throw new CompilerException(string.Format("The query set needs to relate to \"{0}\".", sourceSet.Name), lineNumber);

            return JoinPaths(fact, targetQuery, sourceSet, parentPath, childPath, lineNumber);
        }

        private Source.Fact JoinSubsequentSet(Source.Fact fact, Target.Query targetQuery, string priorName, Source.Set sourceSet)
        {
            Source.Path parentPath = sourceSet.RightPath;
            Source.Path childPath = sourceSet.LeftPath;
            if (parentPath.RelativeTo != priorName)
            {
                parentPath = sourceSet.LeftPath;
                childPath = sourceSet.RightPath;
            }
            int lineNumber = sourceSet.LineNumber;
            if (parentPath.Absolute || childPath.Absolute)
                throw new CompilerException("Subsequent sets cannot relate to \"this\".", lineNumber);
            if (parentPath.RelativeTo != priorName)
                throw new CompilerException(string.Format("The query set needs to relate to \"{0}\".", priorName), lineNumber);
            if (childPath.RelativeTo != sourceSet.Name)
                throw new CompilerException(string.Format("The query set needs to relate to \"{0}\".", sourceSet.Name), lineNumber);

            return JoinPaths(fact, targetQuery, sourceSet, parentPath, childPath, lineNumber);
        }

        private Source.Fact JoinPaths(Source.Fact fact, Target.Query targetQuery, Source.Set sourceSet, Source.Path parentPath, Source.Path childPath, int lineNumber)
        {
            Source.Fact parentEnd = fact;
            IEnumerable<string> parentSegments = parentPath.Segments;
            List<PredecessorInfo> parentPredecessors = WalkSegments(ref parentEnd, parentSegments, lineNumber);
            Source.Fact setType = GetFactByName(sourceSet.FactName, lineNumber);
            Source.Fact childEnd = setType;
            if (childEnd == null)
                throw new CompilerException(string.Format("The fact \"{0}\" is not defined.", sourceSet.FactName), lineNumber);
            List<PredecessorInfo> childPredecessors = WalkSegments(ref childEnd, childPath.Segments, lineNumber);
            if (parentEnd != childEnd)
                throw new CompilerException(string.Format("A query cannot join \"{0}\" to \"{1}\".", childEnd.Name, parentEnd.Name), lineNumber);

            foreach (PredecessorInfo parentPredecessor in parentPredecessors)
            {
                targetQuery.AddJoin(
                    new Target.Join(
                        Target.Direction.Predecessors,
                        parentPredecessor.Fact.Name,
                        parentPredecessor.Field.Name));
            }
            childPredecessors.Reverse();
            foreach (PredecessorInfo childPredecessor in childPredecessors)
            {
                targetQuery.AddJoin(
                    new Target.Join(
                        Target.Direction.Successors,
                        childPredecessor.Fact.Name,
                        childPredecessor.Field.Name));
            }
            return setType;
        }

        private List<PredecessorInfo> WalkSegments(ref Source.Fact fact, IEnumerable<string> segments, int lineNumber)
        {
            List<PredecessorInfo> predecessors = new List<PredecessorInfo>();
            foreach (string segment in segments)
            {
                Source.FactMember member = fact.GetMemberByName(segment);
                if (member == null)
                    throw new CompilerException(string.Format("The member \"{0}.{1}\" is not defined.", fact.Name, segment), lineNumber);
                Source.Field field = member as Source.Field;
                if (field == null)
                    throw new CompilerException(string.Format("The member \"{0}.{1}\" is not a field.", fact.Name, segment), lineNumber);
                Source.DataTypeFact predecessor = field.Type as Source.DataTypeFact;
                if (predecessor == null)
                    throw new CompilerException(string.Format("The member \"{0}.{1}\" is not a fact.", fact.Name, segment), lineNumber);
                predecessors.Add(new PredecessorInfo
                {
                    Fact = fact,
                    Field = field
                });
                fact = GetFactByName(predecessor.FactName, lineNumber);
            }
            return predecessors;
        }

        private Source.Fact GetFactByName(string factName, int lineNumber)
        {
            Source.Fact fact = _root.Facts.FirstOrDefault(f => f.Name == factName);
            if (fact == null)
                throw new CompilerException(string.Format("The fact type \"{0}\" is not defined.", factName), lineNumber);
            return fact;
        }
    }
}
