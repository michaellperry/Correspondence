using System.IO;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.FieldSerializer
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
