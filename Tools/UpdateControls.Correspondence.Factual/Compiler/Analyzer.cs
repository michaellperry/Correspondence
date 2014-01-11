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
            { Source.NativeType.Time,   Target.NativeType.Time },
            { Source.NativeType.Byte,   Target.NativeType.Byte },
            { Source.NativeType.Binary, Target.NativeType.Binary }
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

        public Target.Analyzed Analyze()
        {
            Target.Analyzed result = new Target.Analyzed(_root.Identifier);

            foreach (Source.Fact fact in _root.Facts)
            {
                try
                {
                    EmbelishAnalyzedFromFact(result, fact);
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

        private void EmbelishAnalyzedFromFact(Target.Analyzed result, Source.Fact fact)
        {
            if (_root.Facts.Any(f => f != fact && f.Name == fact.Name))
                throw new CompilerException(string.Format("The fact \"{0}\" is defined more than once.", fact.Name), fact.LineNumber);
            if (fact.Principal && string.IsNullOrEmpty(_root.Strength))
                throw new CompilerException(String.Format("The fact \"{0}\" is a principal. The model must have a declared strength.", fact.Name), fact.LineNumber);
            if (fact.Principal && !fact.Unique)
                throw new CompilerException(String.Format("The fact \"{0}\" is a principal. It must also be unique.", fact.Name), fact.LineNumber);
            if (fact.Principal && fact.Lock)
                throw new CompilerException(string.Format("The fact \"{0}\" is a principal. It cannot also be locked.", fact.Name), fact.LineNumber);
            if (fact.Lock && !fact.Unique)
                throw new CompilerException(string.Format("The fact \"{0}\" is locked. It must also be unique.", fact.Name), fact.LineNumber);

            Target.Class factClass = new Target.Class(fact.Name);
            result.AddClass(factClass);

            factClass.Unique = fact.Unique;

            foreach (Source.FactMember member in fact.Members)
            {
                AnalyzeMember(result, fact, factClass, member);
            }

            factClass.HasPublicKey = fact.Principal;
            factClass.HasSharedKey = fact.Lock;

            ComputeVersionOfFact(factClass);
        }

        private void AnalyzeMember(Target.Analyzed result, Source.Fact fact, Target.Class factClass, Source.FactMember member)
        {
            try
            {
                if (fact.Members.Any(m => m != member && m.Name == member.Name))
                    throw new CompilerException(string.Format("The member \"{0}.{1}\" is defined more than once.", fact.Name, member.Name), member.LineNumber);
                var field = member as Source.Field;
                if (field != null)
                    AnalyzeField(factClass, fact, field);
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
                        else
                        {
                            var property = member as Source.Property;
                            if (property != null)
                                AnalyzeProperty(result, factClass, fact, property);
                        }
                    }
                }
            }
            catch (CompilerException ex)
            {
                _errors.Add(new Error(ex.Message, ex.LineNumber));
            }
        }

        private void AnalyzeField(Target.Class factClass, Source.Fact fact, Source.Field field)
        {
            Source.DataTypeNative dataTypeNative = field.Type as Source.DataTypeNative;
            if (dataTypeNative != null)
                AnalyzeFieldNative(field, dataTypeNative, factClass);
            else
            {
                Source.DataTypeFact dataTypeFact = field.Type as Source.DataTypeFact;
                if (dataTypeFact != null)
                    AnalyzeFieldFact(field, fact, dataTypeFact, factClass);
            }
        }

        private void AnalyzeFieldNative(Source.Field field, Source.DataTypeNative dataTypeNative, Target.Class factClass)
        {
            factClass.AddField(new Target.Field(
                field.Name,
                _cardinalityMap[dataTypeNative.Cardinality],
                _nativeTypeMap[dataTypeNative.NativeType]));
        }

        private void AnalyzeFieldFact(Source.Field field, Source.Fact fact, Source.DataTypeFact dataTypeFact, Target.Class factClass)
        {
            if (!_root.Facts.Any(f => f.Name == dataTypeFact.FactName))
                throw new CompilerException(string.Format("The fact type \"{0}\" is not defined.", dataTypeFact.FactName), field.LineNumber);

            Target.Predecessor predecessor = new Target.Predecessor(field.Name, _cardinalityMap[dataTypeFact.Cardinality], dataTypeFact.FactName, field.Publish);
            factClass.AddPredecessor(predecessor);

            if (field.Publish && field.PublishCondition != null && field.PublishCondition.Clauses.Any())
            {
                foreach (Source.Clause clause in field.PublishCondition.Clauses)
                {
                    Source.FactMember member = fact.GetMemberByName(clause.PredicateName);
                    if (member == null)
                        throw new CompilerException(string.Format("The member \"{0}.{1}\" is not defined.", fact.Name, clause.PredicateName), clause.LineNumber);
                    Source.Predicate predicate = member as Source.Predicate;
                    if (predicate == null)
                        throw new CompilerException(string.Format("The member \"{0}.{1}\" is not a predicate.", fact.Name, clause.PredicateName), clause.LineNumber);

                    Source.ConditionModifier modifier = clause.Existence;
                    if (predicate.Existence == Source.ConditionModifier.Negative)
                    {
                        // Invert the modifier if the predicate itself is negative.
                        modifier = modifier == Source.ConditionModifier.Positive ?
                            Source.ConditionModifier.Negative :
                            Source.ConditionModifier.Positive;
                    }

                    predecessor.AddPublishCondition(new Target.Condition(
                        modifier == Source.ConditionModifier.Positive ?
                            Target.ConditionModifier.Positive :
                            Target.ConditionModifier.Negative,
                        clause.PredicateName,
                        fact.Name));
                }
            }
        }

        private void AnalyzeQuery(Target.Class factClass, Source.Fact fact, Source.Query sourceQuery)
        {
            if (!_root.Facts.Any(f => f.Name == sourceQuery.FactName))
                throw new CompilerException(string.Format("The fact type \"{0}\" is not defined.", sourceQuery.FactName), sourceQuery.LineNumber);
            Source.Fact resultType;
            Target.Query targetQuery = GenerateTargetQuery(factClass, fact, sourceQuery.Name, sourceQuery.Sets, sourceQuery.LineNumber, out resultType);
            if (sourceQuery.FactName != resultType.Name)
                throw new CompilerException(
                    String.Format("The query results in \"{0}\", not \"{1}\".",
                        resultType.Name,
                        sourceQuery.FactName),
                    sourceQuery.LineNumber);
            factClass.AddResult(new Target.ResultDirect(sourceQuery.FactName, targetQuery));
        }

        private void AnalyzePredicate(Target.Class factClass, Source.Fact fact, Source.Predicate predicate)
        {
            Source.Fact resultType;
            Target.Query targetQuery = GenerateTargetQuery(factClass, fact, predicate.Name, predicate.Sets, predicate.LineNumber, out resultType);
            factClass.AddPredicate(new Target.Predicate(
                predicate.Existence == Source.ConditionModifier.Negative ?
                    Target.ConditionModifier.Negative :
                    Target.ConditionModifier.Positive,
                targetQuery));
        }

        private void AnalyzeProperty(Target.Analyzed result, Target.Class factClass, Source.Fact fact, Source.Property property)
        {
            Target.Class childClass = new Target.Class(String.Format("{0}__{1}", fact.Name, property.Name));
            result.AddClass(childClass);

            childClass.AddPredecessor(new Target.Predecessor(fact.Name.ToCamelCase(), Target.Cardinality.One, fact.Name, property.Publish));
            childClass.AddPredecessor(new Target.Predecessor("prior", Target.Cardinality.Many, childClass.Name, false));

            Target.Query query = new Target.Query("isCurrent")
                .AddJoin(new Target.Join(Target.Direction.Successors, childClass.Name, "prior"));
            childClass.AddQuery(query);
            childClass.AddPredicate(new Target.Predicate(Target.ConditionModifier.Negative, query));

            Target.Query valueQuery = new Target.Query(property.Name)
                .AddJoin(new Target.Join(Target.Direction.Successors, childClass.Name, fact.Name.ToCamelCase())
                    .AddCondition(new Target.Condition(Target.ConditionModifier.Negative, "isCurrent", childClass.Name)));
            factClass.AddQuery(valueQuery);

            Source.DataTypeNative dataTypeNative = property.Type as Source.DataTypeNative;
            if (dataTypeNative != null)
            {
                Target.Cardinality cardinality = _cardinalityMap[dataTypeNative.Cardinality];
                Target.NativeType nativeType = _nativeTypeMap[dataTypeNative.NativeType];
                childClass.AddField(new Target.Field(
                    "value",
                    cardinality,
                    nativeType));
                factClass.AddResult(new Target.ResultValueNative(
                    childClass.Name,
                    valueQuery,
                    cardinality,
                    nativeType));
            }
            else
            {
                Source.DataTypeFact dataTypeFact = property.Type as Source.DataTypeFact;
                if (dataTypeFact != null)
                {
                    Target.Cardinality cardinality = _cardinalityMap[dataTypeFact.Cardinality];
                    childClass.AddPredecessor(new Target.Predecessor("value", cardinality, dataTypeFact.FactName, false));
                    factClass.AddResult(new Target.ResultValueFact(
                        childClass.Name,
                        valueQuery,
                        cardinality,
                        dataTypeFact.FactName));
                }
            }

            ComputeVersionOfFact(childClass);
        }

        private Target.Query GenerateTargetQuery(Target.Class factClass, Source.Fact fact, string queryName, IEnumerable<Source.Set> sets, int lineNumber, out Source.Fact resultType)
        {
            Target.Query targetQuery = new Target.Query(queryName);

            if (!sets.Any())
                throw new CompilerException("Define at least one set within a query.", lineNumber);

            // Follow the chain of predecessors on both sides of each set.
            Source.Set firstSet = sets.First();
            Source.Fact priorType = JoinFirstSet(fact, targetQuery, firstSet);
            string priorName = firstSet.Name;
            foreach (Source.Set subsequentSet in sets.Skip(1))
            {
                priorType = JoinSubsequentSet(priorType, targetQuery, priorName, subsequentSet);
                priorName = subsequentSet.Name;
            }

            resultType = priorType;
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

            Target.Join lastJoin = null;
            foreach (PredecessorInfo parentPredecessor in parentPredecessors)
            {
                lastJoin = new Target.Join(Target.Direction.Predecessors, parentPredecessor.Fact.Name, parentPredecessor.Field.Name);
                targetQuery.AddJoin(lastJoin);
                ApplyCondition("this", sourceSet.LineNumber, parentPredecessor.Fact, lastJoin, parentPredecessor.Fact.PurgeCondition, inverse: true);
            }
            childPredecessors.Reverse();
            foreach (PredecessorInfo childPredecessor in childPredecessors)
            {
                lastJoin = new Target.Join(Target.Direction.Successors, childPredecessor.Fact.Name, childPredecessor.Field.Name);
                targetQuery.AddJoin(lastJoin);
                ApplyCondition("this", sourceSet.LineNumber, childPredecessor.Fact, lastJoin, childPredecessor.Fact.PurgeCondition, inverse: true);
            }

            ApplyCondition(sourceSet.Name, sourceSet.LineNumber, setType, lastJoin, sourceSet.Condition, false);
            return setType;
        }

        private static void ApplyCondition(string sourceSetName, int lineNumber, Source.Fact setType, Target.Join join, Source.Condition sourceCondition, bool inverse)
        {
            if (sourceCondition != null && sourceCondition.Clauses.Any())
            {
                if (join == null)
                    throw new CompilerException("The query must specify at least one join in order to have a condition.", lineNumber);

                foreach (Source.Clause clause in sourceCondition.Clauses)
                {
                    if (clause.Name != sourceSetName)
                        throw new CompilerException(string.Format("The condition must relate to {0}.", sourceSetName), clause.LineNumber);
                    Source.FactMember member = setType.GetMemberByName(clause.PredicateName);
                    if (member == null)
                        throw new CompilerException(string.Format("The member \"{0}.{1}\" is not defined.", setType.Name, clause.PredicateName), clause.LineNumber);
                    Source.Predicate predicate = member as Source.Predicate;
                    if (predicate == null)
                        throw new CompilerException(string.Format("The member \"{0}.{1}\" is not a predicate.", setType.Name, clause.PredicateName), clause.LineNumber);

                    Source.ConditionModifier modifier = clause.Existence;
                    if (predicate.Existence == Source.ConditionModifier.Negative ^ inverse)
                    {
                        // Invert the modifier if the predicate itself is negative.
                        modifier = modifier == Source.ConditionModifier.Positive ?
                            Source.ConditionModifier.Negative :
                            Source.ConditionModifier.Positive;
                    }

                    join.AddCondition(new Target.Condition(
                        modifier == Source.ConditionModifier.Positive ?
                            Target.ConditionModifier.Positive :
                            Target.ConditionModifier.Negative,
                        clause.PredicateName,
                        setType.Name));
                }
            }
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

        private void ComputeVersionOfFact(Target.Class factClass)
        {
            if (_root.Version == "legacy")
                factClass.Version = 1;
            else
                unchecked
                {
                    int hash = 0;
                    foreach (var predecessor in factClass.Predecessors)
                        hash = hash * 37 + predecessor.ComputeHash();
                    foreach (var field in factClass.Fields)
                        hash = hash * 37 + field.ComputeHash();
                    hash = hash * 2 + (factClass.Unique ? 1 : 0);
                    hash = hash * 2 + (factClass.HasPublicKey ? 1 : 0);
                    factClass.Version = hash;
                }
        }
    }
}
