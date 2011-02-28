using System.IO;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.FieldSerializer
{
	public class DoubleFieldSerializer : IFieldSerializer
	{
		public object ReadData(BinaryReader input)
		{
			return input.ReadDouble();
		}

		public void WriteData(BinaryWriter output, object value)
		{
			output.Write((double)value);
		}
	}
}
