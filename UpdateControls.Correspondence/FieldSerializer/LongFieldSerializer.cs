using System.IO;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.FieldSerializer
{
	public class LongFieldSerializer : IFieldSerializer
	{
		public object ReadData(BinaryReader input)
		{
			return input.ReadInt64();
		}

		public void WriteData(BinaryWriter output, object value)
		{
			output.Write((long)value);
		}
	}
}
