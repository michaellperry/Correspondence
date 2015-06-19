using System.IO;
using Correspondence.Strategy;

namespace Correspondence.FieldSerializer
{
	public class CharFieldSerializer : IFieldSerializer
	{
		public object ReadData(BinaryReader input)
		{
			return BinaryHelper.ReadChar(input);
		}

		public void WriteData(BinaryWriter output, object value)
		{
			BinaryHelper.WriteChar((char)value, output);
		}
	}
}
