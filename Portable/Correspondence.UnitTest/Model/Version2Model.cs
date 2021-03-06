﻿using System.Collections.Generic;
using System.Linq;
using Correspondence;
using Correspondence.Mementos;
using Correspondence.Strategy;
using System;
using System.IO;

namespace Correspondence.UnitTest.Model
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

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Message.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Message.GetNullInstance();
            }
        }

        // Type
        internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
            "Correspondence.UnitTest.Model.Message", 1);

        protected override CorrespondenceFactType GetCorrespondenceFactType()
        {
            return _correspondenceFactType;
        }

        // Null and unloaded instances
        public static Message GetUnloadedInstance()
        {
            return new Message((FactMemento)null) { IsLoaded = false };
        }

        public static Message GetNullInstance()
        {
            return new Message((FactMemento)null) { IsNull = true };
        }

        public Move Ensure()
        {
            throw new NotImplementedException();
        }

        // Roles
        public static Role RoleSender = new Role(new RoleMemento(
            _correspondenceFactType,
            "sender",
            new CorrespondenceFactType("Correspondence.UnitTest.Model.Player", 1),
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
            _sender = new PredecessorObj<Player>(this, RoleSender, memento, Player.GetUnloadedInstance, Player.GetNullInstance);
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
