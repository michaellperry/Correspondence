using System.IO;
using Correspondence.Strategy;

namespace Correspondence.FieldSerializer
{
	public class DecimalFieldSerializer : IFieldSerializer
	{
		public object ReadData(BinaryReader input)
		{
			return input.ReadDecimal();
		}

		public void WriteData(BinaryWriter output, object value)
		{
			output.Write((decimal)value);
		}
	}
}
