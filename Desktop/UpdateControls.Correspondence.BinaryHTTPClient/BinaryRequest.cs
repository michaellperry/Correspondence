using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;
using System;
using System.IO;
using UpdateControls.Correspondence.FieldSerializer;

namespace UpdateControls.Correspondence.BinaryHTTPClient
{
    public abstract class BinaryRequest
    {
        public static byte Version = 2;

        public string Domain { get; set; }

        public void Write(BinaryWriter requestWriter)
        {
            BinaryHelper.WriteByte(Version, requestWriter);
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
            BinaryHelper.WriteByte(Token, requestWriter);
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
        public List<UnpublishMemento> UnpublishedMessages { get; set; }

        protected override void WriteInternal(BinaryWriter requestWriter)
        {
            BinaryHelper.WriteByte(Token, requestWriter);
            FactTreeSerlializer factTreeSerlializer = new FactTreeSerlializer();
            foreach (var unpublishedMessage in UnpublishedMessages)
            {
                factTreeSerlializer.AddFactType(unpublishedMessage.Role.DeclaringType);
                factTreeSerlializer.AddRole(unpublishedMessage.Role);
            }
            factTreeSerlializer.SerlializeFactTree(MessageBody, requestWriter);
            BinaryHelper.WriteString(ClientGuid, requestWriter);
            BinaryHelper.WriteShort((short)UnpublishedMessages.Count, requestWriter);
            foreach (var unpublishedMessage in UnpublishedMessages)
            {
                BinaryHelper.WriteLong(unpublishedMessage.MessageId.key, requestWriter);
                BinaryHelper.WriteShort(factTreeSerlializer.GetRoleId(unpublishedMessage.Role), requestWriter);
            }
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
            BinaryHelper.WriteByte(Token, requestWriter);
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
            BinaryHelper.WriteByte(Token, requestWriter);
            new FactTreeSerlializer().SerlializeFactTree(PivotTree, requestWriter);
            BinaryHelper.WriteLong(PivotId, requestWriter);
            BinaryHelper.WriteString(DeviceUri, requestWriter);
        }
    }
    public class InterruptRequest : BinaryRequest
    {
        public static byte Token = 5;

        public string ClientGuid { get; set; }

        protected override void WriteInternal(BinaryWriter requestWriter)
        {
            BinaryHelper.WriteByte(Token, requestWriter);
            BinaryHelper.WriteString(ClientGuid, requestWriter);
        }
    }
}
