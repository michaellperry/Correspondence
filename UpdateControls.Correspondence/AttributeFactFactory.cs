using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence
{
    class AttributeFactFactory : ICorrespondenceFactFactory
    {
        class FieldDefinition
        {
            public FieldInfo FieldInfo;
            public int MinVersion;
            public IFieldSerializer FieldSerializer;
        }

        private Type _type;
        private ConstructorInfo _mementoConstructor;
        private List<FieldDefinition> _fields = new List<FieldDefinition>();
        private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

        public AttributeFactFactory(Type type, ConstructorInfo mementoConstructor, IDictionary<Type, IFieldSerializer> fieldSerializerByType)
        {
            _type = type;
            _mementoConstructor = mementoConstructor;
            _fieldSerializerByType = fieldSerializerByType;
            _fields =
                (
                    from f in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    from a in f.GetCustomAttributes(typeof(CorrespondenceFieldAttribute), false)
                    orderby f.Name
                    select new FieldDefinition()
                    {
                        FieldInfo = f,
                        MinVersion = ((CorrespondenceFieldAttribute)a).MinVersion,
                        FieldSerializer = GetFieldSerializer(f.FieldType)
                    }
                )
                .ToList();
        }

        public CorrespondenceFact CreateFact(Memento memento)
        {
            CorrespondenceFact newFact = (CorrespondenceFact)_mementoConstructor.Invoke(new object[] { memento });

            // Create a memory stream from the memento data.
            using (MemoryStream data = new MemoryStream(memento.Data))
            {
                using (BinaryReader output = new BinaryReader(data))
                {
                    // Read all of the fields from the memory stream.
                    foreach (FieldDefinition fieldDefinition in _fields)
                    {
                        object value;

                        // Write the field value from the stream.
                        value = fieldDefinition.FieldSerializer.ReadData(output);

                        // Put it in the object.
                        fieldDefinition.FieldInfo.SetValue(newFact, value);
                    }
                }
            }

            return newFact;
        }

        public void WriteFactData(CorrespondenceFact fact, BinaryWriter output)
        {
            // Write all of the fields to the memory stream.
            foreach (FieldDefinition fieldDefinition in _fields)
            {
                // Get the field value.
                object value = fieldDefinition.FieldInfo.GetValue(fact);

                // Write it to the stream.
                fieldDefinition.FieldSerializer.WriteData(output, value);
            }
        }

        private IFieldSerializer GetFieldSerializer(Type type)
        {
            IFieldSerializer fieldSerializer;
            if (!_fieldSerializerByType.TryGetValue(type, out fieldSerializer))
                throw new CorrespondenceException(string.Format("Please register a field serializer for the type {0}.", type));

            return fieldSerializer;
        }
    }
}
