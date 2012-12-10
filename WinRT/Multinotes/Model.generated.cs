using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System;
using System.IO;

/**
/ For use with http://graphviz.org/
digraph "Multinotes2.Model"
{
    rankdir=BT
    Share -> Individual [color="red"]
    Share -> MessageBoard
    ShareDelete -> Share [color="red"]
    Message -> MessageBoard [color="red"]
    EnableToastNotification -> Individual
    DisableToastNotification -> EnableToastNotification [label="  *"]
}
**/

namespace Multinotes2.Model
{
    public partial class Individual : CorrespondenceFact
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
				Individual newFact = new Individual(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._anonymousId = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Individual fact = (Individual)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._anonymousId);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Multinotes2.Model.Individual", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles

        // Queries
        public static Query MakeQueryMessageBoards()
		{
			return new Query()
				.JoinSuccessors(Share.RoleIndividual, Condition.WhereIsEmpty(Share.MakeQueryIsDeleted())
				)
				.JoinPredecessors(Share.RoleMessageBoard)
            ;
		}
        public static Query QueryMessageBoards = MakeQueryMessageBoards();
        public static Query MakeQueryShares()
		{
			return new Query()
				.JoinSuccessors(Share.RoleIndividual, Condition.WhereIsEmpty(Share.MakeQueryIsDeleted())
				)
            ;
		}
        public static Query QueryShares = MakeQueryShares();
        public static Query MakeQueryIsToastNotificationEnabled()
		{
			return new Query()
				.JoinSuccessors(EnableToastNotification.RoleIndividual, Condition.WhereIsEmpty(EnableToastNotification.MakeQueryIsDisabled())
				)
            ;
		}
        public static Query QueryIsToastNotificationEnabled = MakeQueryIsToastNotificationEnabled();

        // Predicates

        // Predecessors

        // Fields
        private string _anonymousId;

        // Results
        private Result<MessageBoard> _messageBoards;
        private Result<Share> _shares;
        private Result<EnableToastNotification> _isToastNotificationEnabled;

        // Business constructor
        public Individual(
            string anonymousId
            )
        {
            InitializeResults();
            _anonymousId = anonymousId;
        }

        // Hydration constructor
        private Individual(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _messageBoards = new Result<MessageBoard>(this, QueryMessageBoards);
            _shares = new Result<Share>(this, QueryShares);
            _isToastNotificationEnabled = new Result<EnableToastNotification>(this, QueryIsToastNotificationEnabled);
        }

        // Predecessor access

        // Field access
        public string AnonymousId
        {
            get { return _anonymousId; }
        }

        // Query result access
        public Result<MessageBoard> MessageBoards
        {
            get { return _messageBoards; }
        }
        public Result<Share> Shares
        {
            get { return _shares; }
        }
        public Result<EnableToastNotification> IsToastNotificationEnabled
        {
            get { return _isToastNotificationEnabled; }
        }

        // Mutable property access

    }
    
    public partial class Share : CorrespondenceFact
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
				Share newFact = new Share(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Share fact = (Share)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Multinotes2.Model.Share", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleIndividual = new Role(new RoleMemento(
			_correspondenceFactType,
			"individual",
			new CorrespondenceFactType("Multinotes2.Model.Individual", 1),
			true));
        public static Role RoleMessageBoard = new Role(new RoleMemento(
			_correspondenceFactType,
			"messageBoard",
			new CorrespondenceFactType("Multinotes2.Model.MessageBoard", 1),
			false));

        // Queries
        public static Query MakeQueryIsDeleted()
		{
			return new Query()
				.JoinSuccessors(ShareDelete.RoleShare)
            ;
		}
        public static Query QueryIsDeleted = MakeQueryIsDeleted();

        // Predicates
        public static Condition IsDeleted = Condition.WhereIsNotEmpty(QueryIsDeleted);

        // Predecessors
        private PredecessorObj<Individual> _individual;
        private PredecessorObj<MessageBoard> _messageBoard;

        // Unique
        private Guid _unique;

        // Fields

        // Results

        // Business constructor
        public Share(
            Individual individual
            ,MessageBoard messageBoard
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, RoleIndividual, individual);
            _messageBoard = new PredecessorObj<MessageBoard>(this, RoleMessageBoard, messageBoard);
        }

        // Hydration constructor
        private Share(FactMemento memento)
        {
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, RoleIndividual, memento);
            _messageBoard = new PredecessorObj<MessageBoard>(this, RoleMessageBoard, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Individual Individual
        {
            get { return _individual.Fact; }
        }
        public MessageBoard MessageBoard
        {
            get { return _messageBoard.Fact; }
        }

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access

        // Mutable property access

    }
    
    public partial class ShareDelete : CorrespondenceFact
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
				ShareDelete newFact = new ShareDelete(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				ShareDelete fact = (ShareDelete)obj;
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Multinotes2.Model.ShareDelete", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleShare = new Role(new RoleMemento(
			_correspondenceFactType,
			"share",
			new CorrespondenceFactType("Multinotes2.Model.Share", 1),
			true));

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Share> _share;

        // Fields

        // Results

        // Business constructor
        public ShareDelete(
            Share share
            )
        {
            InitializeResults();
            _share = new PredecessorObj<Share>(this, RoleShare, share);
        }

        // Hydration constructor
        private ShareDelete(FactMemento memento)
        {
            InitializeResults();
            _share = new PredecessorObj<Share>(this, RoleShare, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Share Share
        {
            get { return _share.Fact; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class MessageBoard : CorrespondenceFact
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
				MessageBoard newFact = new MessageBoard(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._topic = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				MessageBoard fact = (MessageBoard)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._topic);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Multinotes2.Model.MessageBoard", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles

        // Queries
        public static Query MakeQueryMessages()
		{
			return new Query()
				.JoinSuccessors(Message.RoleMessageBoard)
            ;
		}
        public static Query QueryMessages = MakeQueryMessages();

        // Predicates

        // Predecessors

        // Fields
        private string _topic;

        // Results
        private Result<Message> _messages;

        // Business constructor
        public MessageBoard(
            string topic
            )
        {
            InitializeResults();
            _topic = topic;
        }

        // Hydration constructor
        private MessageBoard(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _messages = new Result<Message>(this, QueryMessages);
        }

        // Predecessor access

        // Field access
        public string Topic
        {
            get { return _topic; }
        }

        // Query result access
        public Result<Message> Messages
        {
            get { return _messages; }
        }

        // Mutable property access

    }
    
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
			"Multinotes2.Model.Message", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleMessageBoard = new Role(new RoleMemento(
			_correspondenceFactType,
			"messageBoard",
			new CorrespondenceFactType("Multinotes2.Model.MessageBoard", 1),
			true));

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<MessageBoard> _messageBoard;

        // Unique
        private Guid _unique;

        // Fields
        private string _text;

        // Results

        // Business constructor
        public Message(
            MessageBoard messageBoard
            ,string text
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _messageBoard = new PredecessorObj<MessageBoard>(this, RoleMessageBoard, messageBoard);
            _text = text;
        }

        // Hydration constructor
        private Message(FactMemento memento)
        {
            InitializeResults();
            _messageBoard = new PredecessorObj<MessageBoard>(this, RoleMessageBoard, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public MessageBoard MessageBoard
        {
            get { return _messageBoard.Fact; }
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
    
    public partial class EnableToastNotification : CorrespondenceFact
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
				EnableToastNotification newFact = new EnableToastNotification(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				EnableToastNotification fact = (EnableToastNotification)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Multinotes2.Model.EnableToastNotification", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleIndividual = new Role(new RoleMemento(
			_correspondenceFactType,
			"individual",
			new CorrespondenceFactType("Multinotes2.Model.Individual", 1),
			false));

        // Queries
        public static Query MakeQueryIsDisabled()
		{
			return new Query()
				.JoinSuccessors(DisableToastNotification.RoleEnable)
            ;
		}
        public static Query QueryIsDisabled = MakeQueryIsDisabled();

        // Predicates
        public static Condition IsDisabled = Condition.WhereIsNotEmpty(QueryIsDisabled);

        // Predecessors
        private PredecessorObj<Individual> _individual;

        // Unique
        private Guid _unique;

        // Fields

        // Results

        // Business constructor
        public EnableToastNotification(
            Individual individual
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, RoleIndividual, individual);
        }

        // Hydration constructor
        private EnableToastNotification(FactMemento memento)
        {
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, RoleIndividual, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Individual Individual
        {
            get { return _individual.Fact; }
        }

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access

        // Mutable property access

    }
    
    public partial class DisableToastNotification : CorrespondenceFact
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
				DisableToastNotification newFact = new DisableToastNotification(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				DisableToastNotification fact = (DisableToastNotification)obj;
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Multinotes2.Model.DisableToastNotification", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleEnable = new Role(new RoleMemento(
			_correspondenceFactType,
			"enable",
			new CorrespondenceFactType("Multinotes2.Model.EnableToastNotification", 1),
			false));

        // Queries

        // Predicates

        // Predecessors
        private PredecessorList<EnableToastNotification> _enable;

        // Fields

        // Results

        // Business constructor
        public DisableToastNotification(
            IEnumerable<EnableToastNotification> enable
            )
        {
            InitializeResults();
            _enable = new PredecessorList<EnableToastNotification>(this, RoleEnable, enable);
        }

        // Hydration constructor
        private DisableToastNotification(FactMemento memento)
        {
            InitializeResults();
            _enable = new PredecessorList<EnableToastNotification>(this, RoleEnable, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public IEnumerable<EnableToastNotification> Enable
        {
            get { return _enable; }
        }
     
        // Field access

        // Query result access

        // Mutable property access

    }
    

	public class CorrespondenceModel : ICorrespondenceModel
	{
		public void RegisterAllFactTypes(Community community, IDictionary<Type, IFieldSerializer> fieldSerializerByType)
		{
			community.AddType(
				Individual._correspondenceFactType,
				new Individual.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Individual._correspondenceFactType }));
			community.AddQuery(
				Individual._correspondenceFactType,
				Individual.QueryMessageBoards.QueryDefinition);
			community.AddQuery(
				Individual._correspondenceFactType,
				Individual.QueryShares.QueryDefinition);
			community.AddQuery(
				Individual._correspondenceFactType,
				Individual.QueryIsToastNotificationEnabled.QueryDefinition);
			community.AddType(
				Share._correspondenceFactType,
				new Share.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Share._correspondenceFactType }));
			community.AddQuery(
				Share._correspondenceFactType,
				Share.QueryIsDeleted.QueryDefinition);
			community.AddUnpublisher(
				Share.RoleIndividual,
				Condition.WhereIsEmpty(Share.QueryIsDeleted)
				);
			community.AddType(
				ShareDelete._correspondenceFactType,
				new ShareDelete.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { ShareDelete._correspondenceFactType }));
			community.AddType(
				MessageBoard._correspondenceFactType,
				new MessageBoard.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { MessageBoard._correspondenceFactType }));
			community.AddQuery(
				MessageBoard._correspondenceFactType,
				MessageBoard.QueryMessages.QueryDefinition);
			community.AddType(
				Message._correspondenceFactType,
				new Message.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Message._correspondenceFactType }));
			community.AddType(
				EnableToastNotification._correspondenceFactType,
				new EnableToastNotification.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { EnableToastNotification._correspondenceFactType }));
			community.AddQuery(
				EnableToastNotification._correspondenceFactType,
				EnableToastNotification.QueryIsDisabled.QueryDefinition);
			community.AddType(
				DisableToastNotification._correspondenceFactType,
				new DisableToastNotification.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { DisableToastNotification._correspondenceFactType }));
		}
	}
}
