using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System;
using System.IO;

namespace UpdateControls.Correspondence.UnitTest.Model
{
    public partial class Message : CorrespondenceFact
    {
        // Factory
        internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
        {
            private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

            public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
            {
                _fieldSerializerByType = fieldSerializerByType;
            }

            public CorrespondenceFact CreateFact(FactMemento memento)
            {
                Message newFact = new Message(memento);

                // Create a memory stream from the memento data.
                using (MemoryStream data = new MemoryStream(memento.Data))
                {
                    using (BinaryReader output = new BinaryReader(data))
                    {
                        newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
                        newFact._text = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
                    }
                }

                return newFact;
            }

            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
            {
                Message fact = (Message)obj;
                _fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
                _fieldSerializerByType[typeof(string)].WriteData(output, fact._text);
            }
        }

        // Type
        internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
            "UpdateControls.Correspondence.UnitTest.Model.Message", 1);

        protected override CorrespondenceFactType GetCorrespondenceFactType()
        {
            return _correspondenceFactType;
        }

        // Roles
        public static Role RoleSender = new Role(new RoleMemento(
            _correspondenceFactType,
            "sender",
            new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Player", 1),
            false));

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Player> _sender;

        // Unique
        private Guid _unique;

        // Fields
        private string _text;

        // Results

        // Business constructor
        public Message(
            Player sender
            , string text
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _sender = new PredecessorObj<Player>(this, RoleSender, sender);
            _text = text;
        }

        // Hydration constructor
        private Message(FactMemento memento)
        {
            InitializeResults();
            _sender = new PredecessorObj<Player>(this, RoleSender, memento, Player.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Player Sender
        {
            get { return _sender.Fact; }
        }

        // Field access
        public Guid Unique { get { return _unique; } }

        public string Text
        {
            get { return _text; }
        }

        // Query result access

        // Mutable property access

    }
    public class Version2Model : ICorrespondenceModel
    {
        public void RegisterAllFactTypes(Community community, IDictionary<Type, IFieldSerializer> fieldSerializerByType)
        {
            community.AddType(
                Message._correspondenceFactType,
                new Message.CorrespondenceFactFactory(fieldSerializerByType),
                new FactMetadata(new List<CorrespondenceFactType> { Message._correspondenceFactType }));
        }
    }
}
