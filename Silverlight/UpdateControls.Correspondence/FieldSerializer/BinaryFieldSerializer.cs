using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateControls.Correspondence.Strategy;
using System.IO;

namespace UpdateControls.Correspondence.FieldSerializer
{
    public class BinaryFieldSerializer : IFieldSerializer
    {
        public object ReadData(BinaryReader input)
        {
            short length = BinaryHelper.ReadShort(input);

            if (length > Model.MaxDataLength - 2)
                throw new CorrespondenceException(string.Format("Binary length {0} exceeded the maximum bytes allowable ({1}).", length, Model.MaxDataLength - 2));
            if (length < 0)
                throw new CorrespondenceException(string.Format("Binary length {0} is negative.", length));
            if (length == 0)
                return new byte[0];

            byte[] result = input.ReadBytes(length);
            return result;
        }

        public void WriteData(BinaryWriter output, object value)
        {
            if (value == null)
                BinaryHelper.WriteShort(0, output);
            else
            {
                byte[] data = (byte[])value;
                int length = data.Length;
                if (length > Model.MaxDataLength - 2)
                    throw new CorrespondenceException(string.Format("Binary length {0} exceeded the maximum bytes allowable ({1}).", length, Model.MaxDataLength - 2));
                BinaryHelper.WriteShort((short)length, output);
                output.Write(data);
            }
        }
    }
}
