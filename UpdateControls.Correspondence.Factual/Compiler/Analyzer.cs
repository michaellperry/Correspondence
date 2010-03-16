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

        private static Dictionary<Source.NativeType, Target.NativeType> _nativeTypeMap = new Dictionary<Source.NativeType,Target.NativeType>
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
                AnalyzeFact(fact, result);

            if (_errors.Any())
                return null;
            else
                return result;
        }

        private void AnalyzeFact(Source.Fact fact, Target.Namespace result)
        {
            if (_root.Facts.Any(f => f != fact && f.Name == fact.Name))
                _errors.Add(new Error(string.Format("The fact \"{0}\" is defined more than once.", fact.Name), fact.LineNumber));
            else
            {
                Target.Class factClass = new Target.Class(fact.Name);
                result.AddClass(factClass);

                foreach (Source.FactMember member in fact.Members)
                {
                    if (fact.Members.Any(m => m != member && m.Name == member.Name))
                        _errors.Add(new Error(string.Format("The member \"{0}.{1}\" is defined more than once.", fact.Name, member.Name), member.LineNumber));
                    var field = member as Source.Field;
                    if (field != null)
                        AnalyzeField(factClass, field);
                }
            }
        }

        private void AnalyzeField(Target.Class factClass, UpdateControls.Correspondence.Factual.AST.Field field)
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
                _errors.Add(new Error(string.Format("The fact type \"{0}\" is not defined.", dataTypeFact.FactName), field.LineNumber));
            else
            {
                factClass.AddPredecessor(new Target.Predecessor(
                    field.Name,
                    _cardinalityMap[dataTypeFact.Cardinality],
                    dataTypeFact.FactName
                ));
            }
        }
    }
}
