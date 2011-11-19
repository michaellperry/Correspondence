using System;
using System.Collections.Generic;
using System.IO;
using UpdateControls.Correspondence.FieldSerializer;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.BinaryHTTPClient
{
    public abstract class BinaryResponse
    {
        public static byte Version = 1;

        public static BinaryResponse Read(BinaryReader responseReader)
        {
            byte version = BinaryHelper.ReadByte(responseReader);
            if (version != BinaryRequest.Version)
                throw new CorrespondenceException(String.Format("This application cannot read version {0} responses.", version));

            byte token = BinaryHelper.ReadByte(responseReader);
            if (token == GetManyResponse.Token)
                return GetManyResponse.ReadInternal(responseReader);
            else if (token == PostResponse.Token)
                return new PostResponse();
            else if (token == SubscribeResponse.Token)
                return new SubscribeResponse();
            else if (token == UnsubscribeResponse.Token)
                return new UnsubscribeResponse();
            else
                throw new CorrespondenceException(String.Format("Unknown token {0}.", token));
        }
    }
    public class GetManyResponse : BinaryResponse
    {
        public static byte Token = 1;

        public FactTreeMemento FactTree { get; set; }
        public List<FactTimestamp> PivotIds { get; set; }

        public static GetManyResponse ReadInternal(BinaryReader responseReader)
        {
            GetManyResponse response = new GetManyResponse();
            response.FactTree = new FactTreeSerlializer().DeserializeFactTree(responseReader);
            short pivotCount = BinaryHelper.ReadShort(responseReader);
            response.PivotIds = new List<FactTimestamp>(pivotCount);
            for (short i = 0; i < pivotCount; i++)
            {
                FactTimestamp factTimestamp = new FactTimestamp();
                factTimestamp.FactId = BinaryHelper.ReadLong(responseReader);
                factTimestamp.TimestampId = BinaryHelper.ReadLong(responseReader);
                response.PivotIds.Add(factTimestamp);
            }
            return response;
        }
    }
    public class PostResponse : BinaryResponse
    {
        public static byte Token = 2;
    }
    public class SubscribeResponse : BinaryResponse
    {
        public static byte Token = 3;
    }
    public class UnsubscribeResponse : BinaryResponse
    {
        public static byte Token = 4;
    }
}
