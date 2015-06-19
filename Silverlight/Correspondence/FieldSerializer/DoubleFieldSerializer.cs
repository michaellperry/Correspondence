using System.IO;
using Correspondence.Strategy;

namespace Correspondence.FieldSerializer
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
