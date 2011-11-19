using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;
using System;
using System.IO;
using UpdateControls.Correspondence.FieldSerializer;

namespace UpdateControls.Correspondence.BinaryHTTPClient
{
    public abstract class BinaryRequest
    {
        public static byte Version = 1;

        public string Domain { get; set; }

        public void Write(BinaryWriter requestWriter)
        {
            BinaryHelper.WriteByte(BinaryRequest.Version, requestWriter);
            BinaryHelper.WriteString(Domain, requestWriter);
            WriteInternal(requestWriter);
        }

        protected abstract void WriteInternal(BinaryWriter requestWriter);
    }
    public class GetManyRequest : BinaryRequest
    {
        public static byte Token = 1;

        public FactTreeMemento PivotTree { get; set; }
        public List<FactTimestamp> PivotIds { get; set; }
        public string ClientGuid { get; set; }
        public int TimeoutSeconds { get; set; }

        protected override void WriteInternal(BinaryWriter requestWriter)
        {
            BinaryHelper.WriteByte(GetManyRequest.Token, requestWriter);
            new FactTreeSerlializer().SerlializeFactTree(PivotTree, requestWriter);
            BinaryHelper.WriteShort((short)PivotIds.Count, requestWriter);
            foreach (FactTimestamp factTimestamp in PivotIds)
            {
                BinaryHelper.WriteLong(factTimestamp.FactId, requestWriter);
                BinaryHelper.WriteLong(factTimestamp.TimestampId, requestWriter);
            }
            BinaryHelper.WriteString(ClientGuid, requestWriter);
            BinaryHelper.WriteInt(TimeoutSeconds, requestWriter);
        }
    }
    public class PostRequest : BinaryRequest
    {
        public static byte Token = 2;

        public FactTreeMemento MessageBody { get; set; }
        public string ClientGuid { get; set; }

        protected override void WriteInternal(BinaryWriter requestWriter)
        {
            BinaryHelper.WriteByte(PostRequest.Token, requestWriter);
            new FactTreeSerlializer().SerlializeFactTree(MessageBody, requestWriter);
            BinaryHelper.WriteString(ClientGuid, requestWriter);
        }
    }
    public class SubscribeRequest : BinaryRequest
    {
        public static byte Token = 3;

        public FactTreeMemento PivotTree { get; set; }
        public long PivotId { get; set; }
        public string DeviceUri { get; set; }
        public string ClientGuid { get; set; }

        protected override void WriteInternal(BinaryWriter requestWriter)
        {
            BinaryHelper.WriteByte(SubscribeRequest.Token, requestWriter);
            new FactTreeSerlializer().SerlializeFactTree(PivotTree, requestWriter);
            BinaryHelper.WriteLong(PivotId, requestWriter);
            BinaryHelper.WriteString(DeviceUri, requestWriter);
            BinaryHelper.WriteString(ClientGuid, requestWriter);
        }
    }
    public class UnsubscribeRequest : BinaryRequest
    {
        public static byte Token = 4;

        public FactTreeMemento PivotTree { get; set; }
        public long PivotId { get; set; }
        public string DeviceUri { get; set; }

        protected override void WriteInternal(BinaryWriter requestWriter)
        {
            BinaryHelper.WriteByte(UnsubscribeRequest.Token, requestWriter);
            new FactTreeSerlializer().SerlializeFactTree(PivotTree, requestWriter);
            BinaryHelper.WriteLong(PivotId, requestWriter);
            BinaryHelper.WriteString(DeviceUri, requestWriter);
        }
    }
}
