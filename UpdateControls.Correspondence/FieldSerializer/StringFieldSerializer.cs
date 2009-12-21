using System.IO;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.FieldSerializer
{
    class StringFieldSerializer : IFieldSerializer
    {
        public object ReadData(BinaryReader input)
        {
            return input.ReadString();
        }

        public void WriteData(BinaryWriter output, object value)
        {
            output.Write((string)value);
        }
    }
}
