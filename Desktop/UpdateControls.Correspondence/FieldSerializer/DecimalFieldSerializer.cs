using System.IO;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.FieldSerializer
{
	class DecimalFieldSerializer : IFieldSerializer
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
