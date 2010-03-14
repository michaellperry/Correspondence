using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Source = UpdateControls.Correspondence.Factual.AST;
using Target = UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class Analyzer
    {
        private Source.Namespace _root;

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

        public Target.Namespace Analyze()
        {
            Target.Namespace result = new Target.Namespace(_root.Identifier);
            foreach (Source.Fact fact in _root.Facts)
            {
                Target.Class factClass = new Target.Class(fact.Name);
                result.AddClass(factClass);
                foreach (Source.FactMember member in fact.Members)
                {
                    var field = member as Source.Field;
                    if (field != null)
                    {
                        Source.DataTypeNative sourceDataTypeNative = field.Type as Source.DataTypeNative;
                        if (sourceDataTypeNative != null)
                        {
                            factClass.AddField(new Target.Field(
                                field.Name,
                                _cardinalityMap[sourceDataTypeNative.Cardinality],
                                _nativeTypeMap[sourceDataTypeNative.NativeType]));
                        }
                    }
                }
            }
            return result;
        }
    }
}
