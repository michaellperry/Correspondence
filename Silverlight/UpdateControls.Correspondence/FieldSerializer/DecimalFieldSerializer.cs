using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Strategy;
using System.IO;

namespace UpdateControls.Correspondence.FieldSerializer
{
    public class DecimalFieldSerializer : IFieldSerializer
    {
        public object ReadData(BinaryReader input)
        {
            throw new NotImplementedException();
        }

        public void WriteData(BinaryWriter output, object value)
        {
            throw new NotImplementedException();
        }
    }
}
