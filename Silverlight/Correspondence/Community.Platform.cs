using System;
using Correspondence.FieldSerializer;

namespace Correspondence
{
	public partial class Community
	{
		partial void RegisterDefaultTypes()
		{
			this
				.AddFieldSerializer<string>(new StringFieldSerializer())
				//.AddFieldSerializer<decimal>(new DecimalFieldSerializer())
				.AddFieldSerializer<DateTime>(new DateTimeFieldSerializer())
				.AddFieldSerializer<Guid>(new GuidFieldSerializer())
				.AddFieldSerializer<int>(new IntFieldSerializer())
				.AddFieldSerializer<float>(new FloatFieldSerializer())
				.AddFieldSerializer<double>(new DoubleFieldSerializer())
				.AddFieldSerializer<long>(new LongFieldSerializer())
				.AddFieldSerializer<byte>(new ByteFieldSerializer())
				.AddFieldSerializer<char>(new CharFieldSerializer())
                .AddFieldSerializer<byte[]>(new BinaryFieldSerializer());
		}
	}
}