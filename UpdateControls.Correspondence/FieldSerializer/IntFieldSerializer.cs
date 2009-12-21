using System;
using System.IO;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.FieldSerializer
{
    class IntFieldSerializer : IFieldSerializer
    {
        public object ReadData(BinaryReader input)
        {
            return input.ReadInt32();
        }

        public void WriteData(BinaryWriter output, object value)
        {
            output.Write((int)value);
        }
    }
}
