using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Correspondence.Strategy;
using System.IO;

namespace Correspondence.FieldSerializer
{
	class DateTimeFieldSerializer : IFieldSerializer
	{
		public object ReadData(BinaryReader input)
		{
			long ticks = input.ReadInt64();
			return new DateTime(ticks, DateTimeKind.Utc);
		}

		public void WriteData(BinaryWriter output, object value)
		{
			long ticks = ((DateTime)value).ToUniversalTime().Ticks;
			output.Write(ticks);
		}
	}
}
