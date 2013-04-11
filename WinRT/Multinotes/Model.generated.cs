using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System;
using System.IO;

/**
/ For use with http://graphviz.org/
digraph "Multinotes.Model"
{
    rankdir=BT
    Share -> Individual [color="red"]
    Share -> MessageBoard
    ShareDelete -> Share [color="red"]
    Message -> MessageBoard [color="red"]
    Message -> Domain [color="red"]
    EnableToastNotification -> Individual
    DisableToastNotification -> EnableToastNotification [label="  *"]
}
**/

namespace Multinotes.Model
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
			"Multinotes.Model.Individual", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        private static Individual _unloadedInstance;
        private static Individual _nullInstance;

        public static Individual GetUnloadedInstance()
        {
            if (_unloadedInstance == null)
            {
                _unloadedInstance = new Individual((FactMemento)null) { IsLoaded = false };
            }
            return _unloadedInstance;
        }

        public static Individual GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new Individual((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Ensure
        public Task<Individual> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Individual)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles

        // Queries
        private static Query _cacheQueryMessageBoards;

        public static Query GetQueryMessageBoards()
		{
            if (_cacheQueryMessageBoards == null)
            {
			    _cacheQueryMessageBoards = new Query()
    				.JoinSuccessors(Share.GetRoleIndividual(), Condition.WhereIsEmpty(Share.GetQueryIsDeleted())
				)
		    		.JoinPredecessors(Share.GetRoleMessageBoard())
                ;
            }
            return _cacheQueryMessageBoards;
		}
        private static Query _cacheQueryShares;

        public static Query GetQueryShares()
		{
            if (_cacheQueryShares == null)
            {
			    _cacheQueryShares = new Query()
    				.JoinSuccessors(Share.GetRoleIndividual(), Condition.WhereIsEmpty(Share.GetQueryIsDeleted())
				)
                ;
            }
            return _cacheQueryShares;
		}
        private static Query _cacheQueryIsToastNotificationEnabled;

        public static Query GetQueryIsToastNotificationEnabled()
		{
            if (_cacheQueryIsToastNotificationEnabled == null)
            {
			    _cacheQueryIsToastNotificationEnabled = new Query()
    				.JoinSuccessors(EnableToastNotification.GetRoleIndividual(), Condition.WhereIsEmpty(EnableToastNotification.GetQueryIsDisabled())
				)
                ;
            }
            return _cacheQueryIsToastNotificationEnabled;
		}

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
            _messageBoards = new Result<MessageBoard>(this, GetQueryMessageBoards(), MessageBoard.GetNullInstance, MessageBoard.GetUnloadedInstance);
            _shares = new Result<Share>(this, GetQueryShares(), Share.GetNullInstance, Share.GetUnloadedInstance);
            _isToastNotificationEnabled = new Result<EnableToastNotification>(this, GetQueryIsToastNotificationEnabled(), EnableToastNotification.GetNullInstance, EnableToastNotification.GetUnloadedInstance);
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
			"Multinotes.Model.Share", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        private static Share _unloadedInstance;
        private static Share _nullInstance;

        public static Share GetUnloadedInstance()
        {
            if (_unloadedInstance == null)
            {
                _unloadedInstance = new Share((FactMemento)null) { IsLoaded = false };
            }
            return _unloadedInstance;
        }

        public static Share GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new Share((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Ensure
        public Task<Share> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Share)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleIndividual;
        public static Role GetRoleIndividual()
        {
            if (_cacheRoleIndividual == null)
            {
                _cacheRoleIndividual = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "individual",
			        new CorrespondenceFactType("Multinotes.Model.Individual", 1),
			        true));
            }
            return _cacheRoleIndividual;
        }
        private static Role _cacheRoleMessageBoard;
        public static Role GetRoleMessageBoard()
        {
            if (_cacheRoleMessageBoard == null)
            {
                _cacheRoleMessageBoard = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "messageBoard",
			        new CorrespondenceFactType("Multinotes.Model.MessageBoard", 1),
			        false));
            }
            return _cacheRoleMessageBoard;
        }

        // Queries
        private static Query _cacheQueryIsDeleted;

        public static Query GetQueryIsDeleted()
		{
            if (_cacheQueryIsDeleted == null)
            {
			    _cacheQueryIsDeleted = new Query()
		    		.JoinSuccessors(ShareDelete.GetRoleShare())
                ;
            }
            return _cacheQueryIsDeleted;
		}

        // Predicates
        public static Condition IsDeleted = Condition.WhereIsNotEmpty(GetQueryIsDeleted());

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
            _individual = new PredecessorObj<Individual>(this, GetRoleIndividual(), individual);
            _messageBoard = new PredecessorObj<MessageBoard>(this, GetRoleMessageBoard(), messageBoard);
        }

        // Hydration constructor
        private Share(FactMemento memento)
        {
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, GetRoleIndividual(), memento, Individual.GetUnloadedInstance, Individual.GetNullInstance);
            _messageBoard = new PredecessorObj<MessageBoard>(this, GetRoleMessageBoard(), memento, MessageBoard.GetUnloadedInstance, MessageBoard.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Individual Individual
        {
            get { return IsNull ? Individual.GetNullInstance() : _individual.Fact; }
        }
        public MessageBoard MessageBoard
        {
            get { return IsNull ? MessageBoard.GetNullInstance() : _messageBoard.Fact; }
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
			"Multinotes.Model.ShareDelete", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        private static ShareDelete _unloadedInstance;
        private static ShareDelete _nullInstance;

        public static ShareDelete GetUnloadedInstance()
        {
            if (_unloadedInstance == null)
            {
                _unloadedInstance = new ShareDelete((FactMemento)null) { IsLoaded = false };
            }
            return _unloadedInstance;
        }

        public static ShareDelete GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new ShareDelete((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Ensure
        public Task<ShareDelete> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (ShareDelete)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleShare;
        public static Role GetRoleShare()
        {
            if (_cacheRoleShare == null)
            {
                _cacheRoleShare = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "share",
			        new CorrespondenceFactType("Multinotes.Model.Share", 1),
			        true));
            }
            return _cacheRoleShare;
        }

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
            _share = new PredecessorObj<Share>(this, GetRoleShare(), share);
        }

        // Hydration constructor
        private ShareDelete(FactMemento memento)
        {
            InitializeResults();
            _share = new PredecessorObj<Share>(this, GetRoleShare(), memento, Share.GetUnloadedInstance, Share.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Share Share
        {
            get { return IsNull ? Share.GetNullInstance() : _share.Fact; }
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
			"Multinotes.Model.MessageBoard", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        private static MessageBoard _unloadedInstance;
        private static MessageBoard _nullInstance;

        public static MessageBoard GetUnloadedInstance()
        {
            if (_unloadedInstance == null)
            {
                _unloadedInstance = new MessageBoard((FactMemento)null) { IsLoaded = false };
            }
            return _unloadedInstance;
        }

        public static MessageBoard GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new MessageBoard((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Ensure
        public Task<MessageBoard> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (MessageBoard)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles

        // Queries
        private static Query _cacheQueryMessages;

        public static Query GetQueryMessages()
		{
            if (_cacheQueryMessages == null)
            {
			    _cacheQueryMessages = new Query()
		    		.JoinSuccessors(Message.GetRoleMessageBoard())
                ;
            }
            return _cacheQueryMessages;
		}

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
            _messages = new Result<Message>(this, GetQueryMessages(), Message.GetNullInstance, Message.GetUnloadedInstance);
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
    
    public partial class Domain : CorrespondenceFact
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
				Domain newFact = new Domain(memento);

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
				Domain fact = (Domain)obj;
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Multinotes.Model.Domain", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        private static Domain _unloadedInstance;
        private static Domain _nullInstance;

        public static Domain GetUnloadedInstance()
        {
            if (_unloadedInstance == null)
            {
                _unloadedInstance = new Domain((FactMemento)null) { IsLoaded = false };
            }
            return _unloadedInstance;
        }

        public static Domain GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new Domain((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Ensure
        public Task<Domain> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Domain)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles

        // Queries

        // Predicates

        // Predecessors

        // Fields

        // Results

        // Business constructor
        public Domain(
            )
        {
            InitializeResults();
        }

        // Hydration constructor
        private Domain(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access

        // Field access

        // Query result access

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
			"Multinotes.Model.Message", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        private static Message _unloadedInstance;
        private static Message _nullInstance;

        public static Message GetUnloadedInstance()
        {
            if (_unloadedInstance == null)
            {
                _unloadedInstance = new Message((FactMemento)null) { IsLoaded = false };
            }
            return _unloadedInstance;
        }

        public static Message GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new Message((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Ensure
        public Task<Message> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Message)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleMessageBoard;
        public static Role GetRoleMessageBoard()
        {
            if (_cacheRoleMessageBoard == null)
            {
                _cacheRoleMessageBoard = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "messageBoard",
			        new CorrespondenceFactType("Multinotes.Model.MessageBoard", 1),
			        true));
            }
            return _cacheRoleMessageBoard;
        }
        private static Role _cacheRoleDomain;
        public static Role GetRoleDomain()
        {
            if (_cacheRoleDomain == null)
            {
                _cacheRoleDomain = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "domain",
			        new CorrespondenceFactType("Multinotes.Model.Domain", 1),
			        true));
            }
            return _cacheRoleDomain;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<MessageBoard> _messageBoard;
        private PredecessorObj<Domain> _domain;

        // Unique
        private Guid _unique;

        // Fields
        private string _text;

        // Results

        // Business constructor
        public Message(
            MessageBoard messageBoard
            ,Domain domain
            ,string text
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _messageBoard = new PredecessorObj<MessageBoard>(this, GetRoleMessageBoard(), messageBoard);
            _domain = new PredecessorObj<Domain>(this, GetRoleDomain(), domain);
            _text = text;
        }

        // Hydration constructor
        private Message(FactMemento memento)
        {
            InitializeResults();
            _messageBoard = new PredecessorObj<MessageBoard>(this, GetRoleMessageBoard(), memento, MessageBoard.GetUnloadedInstance, MessageBoard.GetNullInstance);
            _domain = new PredecessorObj<Domain>(this, GetRoleDomain(), memento, Domain.GetUnloadedInstance, Domain.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public MessageBoard MessageBoard
        {
            get { return IsNull ? MessageBoard.GetNullInstance() : _messageBoard.Fact; }
        }
        public Domain Domain
        {
            get { return IsNull ? Domain.GetNullInstance() : _domain.Fact; }
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
			"Multinotes.Model.EnableToastNotification", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        private static EnableToastNotification _unloadedInstance;
        private static EnableToastNotification _nullInstance;

        public static EnableToastNotification GetUnloadedInstance()
        {
            if (_unloadedInstance == null)
            {
                _unloadedInstance = new EnableToastNotification((FactMemento)null) { IsLoaded = false };
            }
            return _unloadedInstance;
        }

        public static EnableToastNotification GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new EnableToastNotification((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Ensure
        public Task<EnableToastNotification> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (EnableToastNotification)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleIndividual;
        public static Role GetRoleIndividual()
        {
            if (_cacheRoleIndividual == null)
            {
                _cacheRoleIndividual = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "individual",
			        new CorrespondenceFactType("Multinotes.Model.Individual", 1),
			        false));
            }
            return _cacheRoleIndividual;
        }

        // Queries
        private static Query _cacheQueryIsDisabled;

        public static Query GetQueryIsDisabled()
		{
            if (_cacheQueryIsDisabled == null)
            {
			    _cacheQueryIsDisabled = new Query()
		    		.JoinSuccessors(DisableToastNotification.GetRoleEnable())
                ;
            }
            return _cacheQueryIsDisabled;
		}

        // Predicates
        public static Condition IsDisabled = Condition.WhereIsNotEmpty(GetQueryIsDisabled());

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
            _individual = new PredecessorObj<Individual>(this, GetRoleIndividual(), individual);
        }

        // Hydration constructor
        private EnableToastNotification(FactMemento memento)
        {
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, GetRoleIndividual(), memento, Individual.GetUnloadedInstance, Individual.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Individual Individual
        {
            get { return IsNull ? Individual.GetNullInstance() : _individual.Fact; }
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
			"Multinotes.Model.DisableToastNotification", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        private static DisableToastNotification _unloadedInstance;
        private static DisableToastNotification _nullInstance;

        public static DisableToastNotification GetUnloadedInstance()
        {
            if (_unloadedInstance == null)
            {
                _unloadedInstance = new DisableToastNotification((FactMemento)null) { IsLoaded = false };
            }
            return _unloadedInstance;
        }

        public static DisableToastNotification GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new DisableToastNotification((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Ensure
        public Task<DisableToastNotification> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (DisableToastNotification)t.Result);
            else
                return Task.FromResult(this);
        }

        // Roles
        private static Role _cacheRoleEnable;
        public static Role GetRoleEnable()
        {
            if (_cacheRoleEnable == null)
            {
                _cacheRoleEnable = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "enable",
			        new CorrespondenceFactType("Multinotes.Model.EnableToastNotification", 1),
			        false));
            }
            return _cacheRoleEnable;
        }

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
            _enable = new PredecessorList<EnableToastNotification>(this, GetRoleEnable(), enable);
        }

        // Hydration constructor
        private DisableToastNotification(FactMemento memento)
        {
            InitializeResults();
            _enable = new PredecessorList<EnableToastNotification>(this, GetRoleEnable(), memento, EnableToastNotification.GetUnloadedInstance, EnableToastNotification.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public PredecessorList<EnableToastNotification> Enable
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
				Individual.GetQueryMessageBoards().QueryDefinition);
			community.AddQuery(
				Individual._correspondenceFactType,
				Individual.GetQueryShares().QueryDefinition);
			community.AddQuery(
				Individual._correspondenceFactType,
				Individual.GetQueryIsToastNotificationEnabled().QueryDefinition);
			community.AddType(
				Share._correspondenceFactType,
				new Share.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Share._correspondenceFactType }));
			community.AddQuery(
				Share._correspondenceFactType,
				Share.GetQueryIsDeleted().QueryDefinition);
			community.AddUnpublisher(
				Share.GetRoleIndividual(),
				Condition.WhereIsEmpty(Share.GetQueryIsDeleted())
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
				MessageBoard.GetQueryMessages().QueryDefinition);
			community.AddType(
				Domain._correspondenceFactType,
				new Domain.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Domain._correspondenceFactType }));
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
				EnableToastNotification.GetQueryIsDisabled().QueryDefinition);
			community.AddType(
				DisableToastNotification._correspondenceFactType,
				new DisableToastNotification.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { DisableToastNotification._correspondenceFactType }));
		}
	}
}
