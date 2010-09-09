using System.IO;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.FieldSerializer
{
	public class CharFieldSerializer : IFieldSerializer
	{
		public object ReadData(BinaryReader input)
		{
			return input.ReadChar();
		}

		public void WriteData(BinaryWriter output, object value)
		{
			output.Write((char)value);
		}
	}
}
