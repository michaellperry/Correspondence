using System.IO;
using Correspondence.Strategy;

namespace Correspondence.FieldSerializer
{
	public class ByteFieldSerializer : IFieldSerializer
	{
		public object ReadData(BinaryReader input)
		{
			return input.ReadByte();
		}

		public void WriteData(BinaryWriter output, object value)
		{
			output.Write((byte)value);
		}
	}
}
