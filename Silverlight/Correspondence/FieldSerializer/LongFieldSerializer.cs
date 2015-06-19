using System.IO;
using Correspondence.Strategy;

namespace Correspondence.FieldSerializer
{
	public class LongFieldSerializer : IFieldSerializer
	{
		public object ReadData(BinaryReader input)
		{
			return BinaryHelper.ReadLong(input);
		}

		public void WriteData(BinaryWriter output, object value)
		{
            BinaryHelper.WriteLong((long)value, output);
		}
	}
}
