using System.IO;
using UpdateControls.Correspondence.Strategy;
using System.Text;
using System;

namespace UpdateControls.Correspondence.FieldSerializer
{
    public class StringFieldSerializer : IFieldSerializer
    {
        public object ReadData(BinaryReader input)
        {
            short length = BinaryHelper.ReadShort(input);
            if (length > Model.MaxDataLength)
                throw new CorrespondenceException(string.Format("String length {0} exceeded the maximum bytes allowable ({1}).", length, Model.MaxDataLength));
            if (length == 0)
                return string.Empty;

            byte[] data = input.ReadBytes(length);
            string value = BinaryHelper.Encoding.GetString(data, 0, length);
            return value;
        }

        public void WriteData(BinaryWriter output, object value)
        {
            string str = (string)value;
            if (string.IsNullOrEmpty(str))
                BinaryHelper.WriteShort(0, output);
            else
            {
                byte[] data = BinaryHelper.Encoding.GetBytes(str);
                int length = data.Length;
                if (length > Model.MaxDataLength)
                    throw new CorrespondenceException(string.Format("String length {0} exceeded the maximum bytes allowable ({1}).", length, Model.MaxDataLength));
                BinaryHelper.WriteShort((short)length, output);
                output.Write(data);
            }
        }
    }
}
