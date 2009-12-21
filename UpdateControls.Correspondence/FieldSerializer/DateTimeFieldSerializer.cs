using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Strategy;
using System.IO;

namespace UpdateControls.Correspondence.FieldSerializer
{
	class DateTimeFieldSerializer : IFieldSerializer
	{
		public object ReadData(BinaryReader input)
		{
			long ticks = input.ReadInt64();
			byte kind = input.ReadByte();
			return new DateTime(ticks, (DateTimeKind)kind);
		}

		public void WriteData(BinaryWriter output, object value)
		{
			long ticks = ((DateTime)value).Ticks;
			byte kind = (byte)((DateTime)value).Kind;
			output.Write(ticks);
			output.Write(kind);
		}
	}
}
