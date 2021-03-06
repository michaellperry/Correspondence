﻿using System;
using System.IO;
using Correspondence.Strategy;

namespace Correspondence.FieldSerializer
{
    public class IntFieldSerializer : IFieldSerializer
    {
        public object ReadData(BinaryReader input)
        {
            return BinaryHelper.ReadInt(input);
        }

        public void WriteData(BinaryWriter output, object value)
        {
            BinaryHelper.WriteInt((int)value, output);
        }
    }
}
