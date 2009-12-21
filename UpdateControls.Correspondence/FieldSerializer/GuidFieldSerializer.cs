using System;
using System.IO;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.FieldSerializer
{
    public class GuidFieldSerializer : IFieldSerializer
    {
        public object ReadData(BinaryReader input)
        {
            return new Guid(input.ReadBytes(16));
        }

        public void WriteData(BinaryWriter output, object value)
        {
            output.Write(((Guid)value).ToByteArray());
        }
    }
}
