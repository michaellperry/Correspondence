using Correspondence.Strategy;
using System.IO;

namespace Correspondence.FieldSerializer
{
	class FloatFieldSerializer : IFieldSerializer
	{
		public object ReadData(BinaryReader input)
		{
			return input.ReadSingle();
		}

		public void WriteData(BinaryWriter output, object value)
		{
			output.Write((float)value);
		}
	}
}
