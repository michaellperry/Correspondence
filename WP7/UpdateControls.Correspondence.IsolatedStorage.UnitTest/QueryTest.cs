using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;
using System;
using Microsoft.Silverlight.Testing;

namespace UpdateControls.Correspondence.IsolatedStorage.UnitTest
{
    [TestClass]
    public class QueryTest : SilverlightTest
    {
        private IStorageStrategy _storage;

        private CorrespondenceFactType _userType;
        private CorrespondenceFactType _messageType;
        private CorrespondenceFactType _acknowledgeType;
        private CorrespondenceFactType _conversationType;

        private RoleMemento _fromRole;
        private RoleMemento _toRole;
        private RoleMemento _messageRole;
        private RoleMemento _userRole;
        private RoleMemento _participantRole;

        [TestInitialize]
        public void Initialize()
        {
            _userType = new CorrespondenceFactType("IM.Model.User", 1);
            _messageType = new CorrespondenceFactType("IM.Model.Message", 1);
            _acknowledgeType = new CorrespondenceFactType("IM.Model.Acknowledge", 1);
            _conversationType = new CorrespondenceFactType("IM.Model.Conversation", 1);
            _fromRole = new RoleMemento(_messageType, "from", _userType, true);
            _toRole = new RoleMemento(_messageType, "to", _userType, true);
            _messageRole = new RoleMemento(_acknowledgeType, "message", _messageType, false);
            _userRole = new RoleMemento(_acknowledgeType, "user", _userType, false);
            _participantRole = new RoleMemento(_conversationType, "participants", _userType, true);
            
            IsolatedStorageStorageStrategy.DeleteAll();
            _storage = IsolatedStorageStorageStrategy.Load();

            // Load up some test data.
            FactID michael = SaveUser("Michael");
            FactID jenny = SaveUser("Jenny");
            FactID russell = SaveUser("Russell");
            FactID message1 = SaveMessage(michael, jenny, "Hello");
            Acknowledge(message1, jenny);
            FactID message2 = SaveMessage(jenny, michael, "Hi, love");
            Acknowledge(message2, michael);
            FactID message3 = SaveMessage(jenny, michael, "How are you?");
            SaveMessage(michael, russell, "On line tonight?");

            SaveConversation(michael, russell);
            SaveConversation(michael, jenny);

            _storage = IsolatedStorageStorageStrategy.Load();
        }

        [TestMethod]
        public void ShouldFindUser()
        {
            FactID michael;
            bool saved = _storage.Save(CreateUser("Michael"), 0, out michael);

            Assert.IsFalse(saved);
        }

        [TestMethod]
        public void ShouldFindMessagesFromMichael()
        {
            FactID michael = SaveUser("Michael");
            QueryDefinition messagesFromUser = new QueryDefinition();
            messagesFromUser.AddJoin(true, _fromRole, null);
            var messages = _storage.QueryForFacts(messagesFromUser, michael, null);

            Assert.AreEqual(2, messages.Count());
            Assert.AreEqual(_messageType, messages.ElementAt(0).Memento.FactType);
            Assert.AreEqual("On line tonight?", Decode(messages.ElementAt(0).Memento.Data));
            Assert.AreEqual(_messageType, messages.ElementAt(1).Memento.FactType);
            Assert.AreEqual("Hello", Decode(messages.ElementAt(1).Memento.Data));
        }

        [TestMethod]
        public void ShouldFindMessagesFromJenny()
        {
            FactID jenny = SaveUser("Jenny");
            QueryDefinition messagesFromUser = new QueryDefinition();
            messagesFromUser.AddJoin(true, _fromRole, null);
            var messages = _storage.QueryForFacts(messagesFromUser, jenny, null);

            Assert.AreEqual(2, messages.Count());
            Assert.AreEqual(_messageType, messages.ElementAt(0).Memento.FactType);
            Assert.AreEqual("How are you?", Decode(messages.ElementAt(0).Memento.Data));
            Assert.AreEqual(_messageType, messages.ElementAt(1).Memento.FactType);
            Assert.AreEqual("Hi, love", Decode(messages.ElementAt(1).Memento.Data));
        }

        [TestMethod]
        public void ShouldFindUnacknowledgedMessagesToMichael()
        {
            FactID michael = SaveUser("Michael");
            QueryDefinition acknowledges = new QueryDefinition();
            acknowledges.AddJoin(true, _messageRole, null);
            QueryDefinition unacknowledgedMessagesToUser = new QueryDefinition();
            unacknowledgedMessagesToUser.AddJoin(true, _toRole, new WhereIsEmptyCondition(acknowledges));
            var messages = _storage.QueryForFacts(unacknowledgedMessagesToUser, michael, null);

            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual(_messageType, messages.Single().Memento.FactType);
            Assert.AreEqual("How are you?", Decode(messages.Single().Memento.Data));
        }

        [TestMethod]
        public void ShouldFindAcknowledgedMessagesToMichael()
        {
            FactID michael = SaveUser("Michael");
            QueryDefinition acknowledges = new QueryDefinition();
            acknowledges.AddJoin(true, _messageRole, null);
            QueryDefinition unacknowledgedMessagesToUser = new QueryDefinition();
            unacknowledgedMessagesToUser.AddJoin(true, _toRole, new WhereIsNotEmptyCondition(acknowledges));
            var messages = _storage.QueryForFacts(unacknowledgedMessagesToUser, michael, null);

            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual(_messageType, messages.Single().Memento.FactType);
            Assert.AreEqual("Hi, love", Decode(messages.Single().Memento.Data));
        }

        [TestMethod]
        public void ShouldFindFriends()
        {
            FactID michael = SaveUser("Michael");
            QueryDefinition friendsQuery = new QueryDefinition();
            friendsQuery.AddJoin(true, _fromRole, null);
            friendsQuery.AddJoin(false, _toRole, null);
            var friends = _storage.QueryForFacts(friendsQuery, michael, null);

            Assert.AreEqual(2, friends.Count());
            Assert.AreEqual(_userType, friends.ElementAt(0).Memento.FactType);
            Assert.AreEqual("Russell", Decode(friends.ElementAt(0).Memento.Data));
            Assert.AreEqual(_userType, friends.ElementAt(1).Memento.FactType);
            Assert.AreEqual("Jenny", Decode(friends.ElementAt(1).Memento.Data));
        }

        [TestMethod]
        public void ShouldFindConversations()
        {
            FactID michael = SaveUser("Michael");
            QueryDefinition conversationsQuery = new QueryDefinition();
            conversationsQuery.AddJoin(true, _participantRole, null);
            var conversations = _storage.QueryForIds(conversationsQuery, michael);

            Assert.AreEqual(2, conversations.Count());
        }

        private FactID SaveUser(string userName)
        {
            FactID userId;
            _storage.Save(CreateUser(userName), 0, out userId);
            return userId;
        }

        private FactMemento CreateUser(string userName)
        {
            return new FactMemento(_userType) { Data = Encode(userName) };
        }

        private FactID SaveMessage(FactID from, FactID to, string body)
        {
            FactID messageId;
            FactMemento message = new FactMemento(_messageType) { Data = Encode(body) };
            message.AddPredecessor(_fromRole, from);
            message.AddPredecessor(_toRole, to);
            _storage.Save(message, 0, out messageId);
            return messageId;
        }

        private void Acknowledge(FactID message, FactID user)
        {
            FactID acknowledgeId;
            FactMemento acknowledge = new FactMemento(_acknowledgeType) { Data = new byte[0] };
            acknowledge.AddPredecessor(_messageRole, message);
            acknowledge.AddPredecessor(_userRole, user);
            _storage.Save(acknowledge, 0, out acknowledgeId);
        }

        private FactID SaveConversation(FactID userA, FactID userB)
        {
            FactMemento conversation = new FactMemento(_conversationType)
            {
                Data = new byte[0]
            };
            conversation.AddPredecessor(_participantRole, userA);
            conversation.AddPredecessor(_participantRole, userB);

            FactID id;
            _storage.Save(conversation, 0, out id);
            return id;
        }

        private byte[] Encode(string value)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(value);
            return ms.ToArray();
        }

        private string Decode(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(ms);
            return reader.ReadString();
        }
    }
}
