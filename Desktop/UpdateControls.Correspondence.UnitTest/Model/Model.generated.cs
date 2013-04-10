using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System;
using System.IO;

/**
/ For use with http://graphviz.org/
digraph "UpdateControls.Correspondence.UnitTest.Model"
{
    rankdir=BT
    User__favoriteColor -> User [color="red"]
    User__favoriteColor -> User__favoriteColor [label="  *"]
    User__betterFavoriteColor -> User [color="red"]
    User__betterFavoriteColor -> User__betterFavoriteColor [label="  *"]
    User__betterFavoriteColor -> Color
    LogOn -> User
    LogOn -> Machine
    LogOff -> LogOn
    GameName -> Game
    GameName -> GameName [label="  *"]
    Player -> User [color="red"]
    Player -> Game [color="red"]
    Move -> Player
    Outcome -> Game [color="red"]
    Outcome -> Player [label="  ?"]
}
**/

namespace UpdateControls.Correspondence.UnitTest.Model
{
    public partial class Machine : CorrespondenceFact
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
				Machine newFact = new Machine(memento);

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
				Machine fact = (Machine)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Machine", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static Machine _nullInstance;

        public static Machine GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new Machine((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles

        // Queries
        private static Query _cacheQueryActiveLogOns;

        public static Query GetQueryActiveLogOns()
		{
            if (_cacheQueryActiveLogOns == null)
            {
			    _cacheQueryActiveLogOns = new Query()
    				.JoinSuccessors(LogOn.GetRoleMachine(), Condition.WhereIsEmpty(LogOn.GetQueryIsActive())
				)
                ;
            }
            return _cacheQueryActiveLogOns;
		}

        // Predicates

        // Predecessors

        // Unique
        private Guid _unique;

        // Fields

        // Results
        private Result<LogOn> _activeLogOns;

        // Business constructor
        public Machine(
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
        }

        // Hydration constructor
        private Machine(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _activeLogOns = new Result<LogOn>(this, GetQueryActiveLogOns());
        }

        // Predecessor access

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access
        public Result<LogOn> ActiveLogOns
        {
            get { return _activeLogOns; }
        }

        // Mutable property access

    }
    
    public partial class User : CorrespondenceFact
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
				User newFact = new User(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._userName = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				User fact = (User)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._userName);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.User", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static User _nullInstance;

        public static User GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new User((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles

        // Queries
        private static Query _cacheQueryFavoriteColor;

        public static Query GetQueryFavoriteColor()
		{
            if (_cacheQueryFavoriteColor == null)
            {
			    _cacheQueryFavoriteColor = new Query()
    				.JoinSuccessors(User__favoriteColor.GetRoleUser(), Condition.WhereIsEmpty(User__favoriteColor.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryFavoriteColor;
		}
        private static Query _cacheQueryBetterFavoriteColor;

        public static Query GetQueryBetterFavoriteColor()
		{
            if (_cacheQueryBetterFavoriteColor == null)
            {
			    _cacheQueryBetterFavoriteColor = new Query()
    				.JoinSuccessors(User__betterFavoriteColor.GetRoleUser(), Condition.WhereIsEmpty(User__betterFavoriteColor.GetQueryIsCurrent())
				)
                ;
            }
            return _cacheQueryBetterFavoriteColor;
		}
        private static Query _cacheQueryActivePlayers;

        public static Query GetQueryActivePlayers()
		{
            if (_cacheQueryActivePlayers == null)
            {
			    _cacheQueryActivePlayers = new Query()
    				.JoinSuccessors(Player.GetRoleUser(), Condition.WhereIsEmpty(Player.GetQueryIsActive())
				)
                ;
            }
            return _cacheQueryActivePlayers;
		}
        private static Query _cacheQueryFinishedPlayers;

        public static Query GetQueryFinishedPlayers()
		{
            if (_cacheQueryFinishedPlayers == null)
            {
			    _cacheQueryFinishedPlayers = new Query()
    				.JoinSuccessors(Player.GetRoleUser(), Condition.WhereIsNotEmpty(Player.GetQueryIsNotActive())
				)
                ;
            }
            return _cacheQueryFinishedPlayers;
		}
        private static Query _cacheQueryFinishedGames;

        public static Query GetQueryFinishedGames()
		{
            if (_cacheQueryFinishedGames == null)
            {
			    _cacheQueryFinishedGames = new Query()
		    		.JoinSuccessors(Player.GetRoleUser())
    				.JoinPredecessors(Player.GetRoleGame(), Condition.WhereIsNotEmpty(Game.GetQueryIsFinished())
				)
                ;
            }
            return _cacheQueryFinishedGames;
		}

        // Predicates

        // Predecessors

        // Fields
        private string _userName;

        // Results
        private Result<User__favoriteColor> _favoriteColor;
        private Result<User__betterFavoriteColor> _betterFavoriteColor;
        private Result<Player> _activePlayers;
        private Result<Player> _finishedPlayers;
        private Result<Game> _finishedGames;

        // Business constructor
        public User(
            string userName
            )
        {
            InitializeResults();
            _userName = userName;
        }

        // Hydration constructor
        private User(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _favoriteColor = new Result<User__favoriteColor>(this, GetQueryFavoriteColor());
            _betterFavoriteColor = new Result<User__betterFavoriteColor>(this, GetQueryBetterFavoriteColor());
            _activePlayers = new Result<Player>(this, GetQueryActivePlayers());
            _finishedPlayers = new Result<Player>(this, GetQueryFinishedPlayers());
            _finishedGames = new Result<Game>(this, GetQueryFinishedGames());
        }

        // Predecessor access

        // Field access
        public string UserName
        {
            get { return _userName; }
        }

        // Query result access
        public Result<Player> ActivePlayers
        {
            get { return _activePlayers; }
        }
        public Result<Player> FinishedPlayers
        {
            get { return _finishedPlayers; }
        }
        public Result<Game> FinishedGames
        {
            get { return _finishedGames; }
        }

        // Mutable property access
        public TransientDisputable<User__favoriteColor, string> FavoriteColor
        {
            get { return _favoriteColor.AsTransientDisputable(fact => fact.Value); }
			set
			{
				var current = _favoriteColor.Ensure().ToList();
				if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
				{
					Community.AddFact(new User__favoriteColor(this, _favoriteColor, value.Value));
				}
			}
        }

        public TransientDisputable<User__betterFavoriteColor, Color> BetterFavoriteColor
        {
            get { return _betterFavoriteColor.AsTransientDisputable(fact => fact.Value); }
			set
			{
				var current = _betterFavoriteColor.Ensure().ToList();
				if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
				{
					Community.AddFact(new User__betterFavoriteColor(this, _betterFavoriteColor, value.Value));
				}
			}
        }
    }
    
    public partial class User__favoriteColor : CorrespondenceFact
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
				User__favoriteColor newFact = new User__favoriteColor(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				User__favoriteColor fact = (User__favoriteColor)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.User__favoriteColor", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static User__favoriteColor _nullInstance;

        public static User__favoriteColor GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new User__favoriteColor((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles
        private static Role _cacheRoleUser;
        public static Role GetRoleUser()
        {
            if (_cacheRoleUser == null)
            {
                _cacheRoleUser = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "user",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.User", 1),
			        true));
            }
            return _cacheRoleUser;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.User__favoriteColor", 1),
			        false));
            }
            return _cacheRolePrior;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(User__favoriteColor.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<User> _user;
        private PredecessorList<User__favoriteColor> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public User__favoriteColor(
            User user
            ,IEnumerable<User__favoriteColor> prior
            ,string value
            )
        {
            InitializeResults();
            _user = new PredecessorObj<User>(this, GetRoleUser(), user);
            _prior = new PredecessorList<User__favoriteColor>(this, GetRolePrior(), prior);
            _value = value;
        }

        // Hydration constructor
        private User__favoriteColor(FactMemento memento)
        {
            InitializeResults();
            _user = new PredecessorObj<User>(this, GetRoleUser(), memento, User.GetNullInstance);
            _prior = new PredecessorList<User__favoriteColor>(this, GetRolePrior(), memento, User__favoriteColor.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public User User
        {
            get { return _user.Fact; }
        }
        public IEnumerable<User__favoriteColor> Prior
        {
            get { return _prior; }
        }
     
        // Field access
        public string Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class User__betterFavoriteColor : CorrespondenceFact
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
				User__betterFavoriteColor newFact = new User__betterFavoriteColor(memento);

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
				User__betterFavoriteColor fact = (User__betterFavoriteColor)obj;
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.User__betterFavoriteColor", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static User__betterFavoriteColor _nullInstance;

        public static User__betterFavoriteColor GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new User__betterFavoriteColor((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles
        private static Role _cacheRoleUser;
        public static Role GetRoleUser()
        {
            if (_cacheRoleUser == null)
            {
                _cacheRoleUser = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "user",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.User", 1),
			        true));
            }
            return _cacheRoleUser;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.User__betterFavoriteColor", 1),
			        false));
            }
            return _cacheRolePrior;
        }
        private static Role _cacheRoleValue;
        public static Role GetRoleValue()
        {
            if (_cacheRoleValue == null)
            {
                _cacheRoleValue = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "value",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Color", 1),
			        false));
            }
            return _cacheRoleValue;
        }

        // Queries
        private static Query _cacheQueryIsCurrent;

        public static Query GetQueryIsCurrent()
		{
            if (_cacheQueryIsCurrent == null)
            {
			    _cacheQueryIsCurrent = new Query()
		    		.JoinSuccessors(User__betterFavoriteColor.GetRolePrior())
                ;
            }
            return _cacheQueryIsCurrent;
		}

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(GetQueryIsCurrent());

        // Predecessors
        private PredecessorObj<User> _user;
        private PredecessorList<User__betterFavoriteColor> _prior;
        private PredecessorObj<Color> _value;

        // Fields

        // Results

        // Business constructor
        public User__betterFavoriteColor(
            User user
            ,IEnumerable<User__betterFavoriteColor> prior
            ,Color value
            )
        {
            InitializeResults();
            _user = new PredecessorObj<User>(this, GetRoleUser(), user);
            _prior = new PredecessorList<User__betterFavoriteColor>(this, GetRolePrior(), prior);
            _value = new PredecessorObj<Color>(this, GetRoleValue(), value);
        }

        // Hydration constructor
        private User__betterFavoriteColor(FactMemento memento)
        {
            InitializeResults();
            _user = new PredecessorObj<User>(this, GetRoleUser(), memento, User.GetNullInstance);
            _prior = new PredecessorList<User__betterFavoriteColor>(this, GetRolePrior(), memento, User__betterFavoriteColor.GetNullInstance);
            _value = new PredecessorObj<Color>(this, GetRoleValue(), memento, Color.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public User User
        {
            get { return _user.Fact; }
        }
        public IEnumerable<User__betterFavoriteColor> Prior
        {
            get { return _prior; }
        }
             public Color Value
        {
            get { return _value.Fact; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class Color : CorrespondenceFact
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
				Color newFact = new Color(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._name = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Color fact = (Color)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._name);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Color", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static Color _nullInstance;

        public static Color GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new Color((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles

        // Queries

        // Predicates

        // Predecessors

        // Fields
        private string _name;

        // Results

        // Business constructor
        public Color(
            string name
            )
        {
            InitializeResults();
            _name = name;
        }

        // Hydration constructor
        private Color(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access

        // Field access
        public string Name
        {
            get { return _name; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class LogOn : CorrespondenceFact
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
				LogOn newFact = new LogOn(memento);

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
				LogOn fact = (LogOn)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.LogOn", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static LogOn _nullInstance;

        public static LogOn GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new LogOn((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles
        private static Role _cacheRoleUser;
        public static Role GetRoleUser()
        {
            if (_cacheRoleUser == null)
            {
                _cacheRoleUser = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "user",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.User", 1),
			        false));
            }
            return _cacheRoleUser;
        }
        private static Role _cacheRoleMachine;
        public static Role GetRoleMachine()
        {
            if (_cacheRoleMachine == null)
            {
                _cacheRoleMachine = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "machine",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Machine", 1),
			        false));
            }
            return _cacheRoleMachine;
        }

        // Queries
        private static Query _cacheQueryIsActive;

        public static Query GetQueryIsActive()
		{
            if (_cacheQueryIsActive == null)
            {
			    _cacheQueryIsActive = new Query()
		    		.JoinSuccessors(LogOff.GetRoleLogOn())
                ;
            }
            return _cacheQueryIsActive;
		}

        // Predicates
        public static Condition IsActive = Condition.WhereIsEmpty(GetQueryIsActive());

        // Predecessors
        private PredecessorObj<User> _user;
        private PredecessorObj<Machine> _machine;

        // Unique
        private Guid _unique;

        // Fields

        // Results

        // Business constructor
        public LogOn(
            User user
            ,Machine machine
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _user = new PredecessorObj<User>(this, GetRoleUser(), user);
            _machine = new PredecessorObj<Machine>(this, GetRoleMachine(), machine);
        }

        // Hydration constructor
        private LogOn(FactMemento memento)
        {
            InitializeResults();
            _user = new PredecessorObj<User>(this, GetRoleUser(), memento, User.GetNullInstance);
            _machine = new PredecessorObj<Machine>(this, GetRoleMachine(), memento, Machine.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public User User
        {
            get { return _user.Fact; }
        }
        public Machine Machine
        {
            get { return _machine.Fact; }
        }

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access

        // Mutable property access

    }
    
    public partial class LogOff : CorrespondenceFact
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
				LogOff newFact = new LogOff(memento);

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
				LogOff fact = (LogOff)obj;
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.LogOff", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static LogOff _nullInstance;

        public static LogOff GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new LogOff((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles
        private static Role _cacheRoleLogOn;
        public static Role GetRoleLogOn()
        {
            if (_cacheRoleLogOn == null)
            {
                _cacheRoleLogOn = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "logOn",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.LogOn", 1),
			        false));
            }
            return _cacheRoleLogOn;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<LogOn> _logOn;

        // Fields

        // Results

        // Business constructor
        public LogOff(
            LogOn logOn
            )
        {
            InitializeResults();
            _logOn = new PredecessorObj<LogOn>(this, GetRoleLogOn(), logOn);
        }

        // Hydration constructor
        private LogOff(FactMemento memento)
        {
            InitializeResults();
            _logOn = new PredecessorObj<LogOn>(this, GetRoleLogOn(), memento, LogOn.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public LogOn LogOn
        {
            get { return _logOn.Fact; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class Game : CorrespondenceFact
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
				Game newFact = new Game(memento);

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
				Game fact = (Game)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Game", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static Game _nullInstance;

        public static Game GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new Game((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles

        // Queries
        private static Query _cacheQueryPlayers;

        public static Query GetQueryPlayers()
		{
            if (_cacheQueryPlayers == null)
            {
			    _cacheQueryPlayers = new Query()
		    		.JoinSuccessors(Player.GetRoleGame())
                ;
            }
            return _cacheQueryPlayers;
		}
        private static Query _cacheQueryMoves;

        public static Query GetQueryMoves()
		{
            if (_cacheQueryMoves == null)
            {
			    _cacheQueryMoves = new Query()
		    		.JoinSuccessors(Player.GetRoleGame())
		    		.JoinSuccessors(Move.GetRolePlayer())
                ;
            }
            return _cacheQueryMoves;
		}
        private static Query _cacheQueryOutcomes;

        public static Query GetQueryOutcomes()
		{
            if (_cacheQueryOutcomes == null)
            {
			    _cacheQueryOutcomes = new Query()
		    		.JoinSuccessors(Outcome.GetRoleGame())
                ;
            }
            return _cacheQueryOutcomes;
		}
        private static Query _cacheQueryIsFinished;

        public static Query GetQueryIsFinished()
		{
            if (_cacheQueryIsFinished == null)
            {
			    _cacheQueryIsFinished = new Query()
		    		.JoinSuccessors(Outcome.GetRoleGame())
                ;
            }
            return _cacheQueryIsFinished;
		}

        // Predicates
        public static Condition IsFinished = Condition.WhereIsNotEmpty(GetQueryIsFinished());

        // Predecessors

        // Unique
        private Guid _unique;

        // Fields

        // Results
        private Result<Player> _players;
        private Result<Move> _moves;
        private Result<Outcome> _outcomes;

        // Business constructor
        public Game(
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
        }

        // Hydration constructor
        private Game(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _players = new Result<Player>(this, GetQueryPlayers());
            _moves = new Result<Move>(this, GetQueryMoves());
            _outcomes = new Result<Outcome>(this, GetQueryOutcomes());
        }

        // Predecessor access

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access
        public Result<Player> Players
        {
            get { return _players; }
        }
        public Result<Move> Moves
        {
            get { return _moves; }
        }
        public Result<Outcome> Outcomes
        {
            get { return _outcomes; }
        }

        // Mutable property access

    }
    
    public partial class GameName : CorrespondenceFact
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
				GameName newFact = new GameName(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._name = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				GameName fact = (GameName)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._name);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.GameName", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static GameName _nullInstance;

        public static GameName GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new GameName((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles
        private static Role _cacheRoleGame;
        public static Role GetRoleGame()
        {
            if (_cacheRoleGame == null)
            {
                _cacheRoleGame = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "game",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Game", 1),
			        false));
            }
            return _cacheRoleGame;
        }
        private static Role _cacheRolePrior;
        public static Role GetRolePrior()
        {
            if (_cacheRolePrior == null)
            {
                _cacheRolePrior = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "prior",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.GameName", 1),
			        false));
            }
            return _cacheRolePrior;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Game> _game;
        private PredecessorList<GameName> _prior;

        // Fields
        private string _name;

        // Results

        // Business constructor
        public GameName(
            Game game
            ,IEnumerable<GameName> prior
            ,string name
            )
        {
            InitializeResults();
            _game = new PredecessorObj<Game>(this, GetRoleGame(), game);
            _prior = new PredecessorList<GameName>(this, GetRolePrior(), prior);
            _name = name;
        }

        // Hydration constructor
        private GameName(FactMemento memento)
        {
            InitializeResults();
            _game = new PredecessorObj<Game>(this, GetRoleGame(), memento, Game.GetNullInstance);
            _prior = new PredecessorList<GameName>(this, GetRolePrior(), memento, GameName.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Game Game
        {
            get { return _game.Fact; }
        }
        public IEnumerable<GameName> Prior
        {
            get { return _prior; }
        }
     
        // Field access
        public string Name
        {
            get { return _name; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class Player : CorrespondenceFact
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
				Player newFact = new Player(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._index = (int)_fieldSerializerByType[typeof(int)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Player fact = (Player)obj;
				_fieldSerializerByType[typeof(int)].WriteData(output, fact._index);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Player", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static Player _nullInstance;

        public static Player GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new Player((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles
        private static Role _cacheRoleUser;
        public static Role GetRoleUser()
        {
            if (_cacheRoleUser == null)
            {
                _cacheRoleUser = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "user",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.User", 1),
			        true));
            }
            return _cacheRoleUser;
        }
        private static Role _cacheRoleGame;
        public static Role GetRoleGame()
        {
            if (_cacheRoleGame == null)
            {
                _cacheRoleGame = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "game",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Game", 1),
			        true));
            }
            return _cacheRoleGame;
        }

        // Queries
        private static Query _cacheQueryMoves;

        public static Query GetQueryMoves()
		{
            if (_cacheQueryMoves == null)
            {
			    _cacheQueryMoves = new Query()
		    		.JoinSuccessors(Move.GetRolePlayer())
                ;
            }
            return _cacheQueryMoves;
		}
        private static Query _cacheQueryIsActive;

        public static Query GetQueryIsActive()
		{
            if (_cacheQueryIsActive == null)
            {
			    _cacheQueryIsActive = new Query()
		    		.JoinPredecessors(Player.GetRoleGame())
		    		.JoinSuccessors(Outcome.GetRoleGame())
                ;
            }
            return _cacheQueryIsActive;
		}
        private static Query _cacheQueryIsNotActive;

        public static Query GetQueryIsNotActive()
		{
            if (_cacheQueryIsNotActive == null)
            {
			    _cacheQueryIsNotActive = new Query()
		    		.JoinPredecessors(Player.GetRoleGame())
		    		.JoinSuccessors(Outcome.GetRoleGame())
                ;
            }
            return _cacheQueryIsNotActive;
		}

        // Predicates
        public static Condition IsActive = Condition.WhereIsEmpty(GetQueryIsActive());
        public static Condition IsNotActive = Condition.WhereIsNotEmpty(GetQueryIsNotActive());

        // Predecessors
        private PredecessorObj<User> _user;
        private PredecessorObj<Game> _game;

        // Fields
        private int _index;

        // Results
        private Result<Move> _moves;

        // Business constructor
        public Player(
            User user
            ,Game game
            ,int index
            )
        {
            InitializeResults();
            _user = new PredecessorObj<User>(this, GetRoleUser(), user);
            _game = new PredecessorObj<Game>(this, GetRoleGame(), game);
            _index = index;
        }

        // Hydration constructor
        private Player(FactMemento memento)
        {
            InitializeResults();
            _user = new PredecessorObj<User>(this, GetRoleUser(), memento, User.GetNullInstance);
            _game = new PredecessorObj<Game>(this, GetRoleGame(), memento, Game.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
            _moves = new Result<Move>(this, GetQueryMoves());
        }

        // Predecessor access
        public User User
        {
            get { return _user.Fact; }
        }
        public Game Game
        {
            get { return _game.Fact; }
        }

        // Field access
        public int Index
        {
            get { return _index; }
        }

        // Query result access
        public Result<Move> Moves
        {
            get { return _moves; }
        }

        // Mutable property access

    }
    
    public partial class Move : CorrespondenceFact
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
				Move newFact = new Move(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._index = (int)_fieldSerializerByType[typeof(int)].ReadData(output);
						newFact._square = (int)_fieldSerializerByType[typeof(int)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Move fact = (Move)obj;
				_fieldSerializerByType[typeof(int)].WriteData(output, fact._index);
				_fieldSerializerByType[typeof(int)].WriteData(output, fact._square);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Move", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static Move _nullInstance;

        public static Move GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new Move((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles
        private static Role _cacheRolePlayer;
        public static Role GetRolePlayer()
        {
            if (_cacheRolePlayer == null)
            {
                _cacheRolePlayer = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "player",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Player", 1),
			        false));
            }
            return _cacheRolePlayer;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Player> _player;

        // Fields
        private int _index;
        private int _square;

        // Results

        // Business constructor
        public Move(
            Player player
            ,int index
            ,int square
            )
        {
            InitializeResults();
            _player = new PredecessorObj<Player>(this, GetRolePlayer(), player);
            _index = index;
            _square = square;
        }

        // Hydration constructor
        private Move(FactMemento memento)
        {
            InitializeResults();
            _player = new PredecessorObj<Player>(this, GetRolePlayer(), memento, Player.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Player Player
        {
            get { return _player.Fact; }
        }

        // Field access
        public int Index
        {
            get { return _index; }
        }
        public int Square
        {
            get { return _square; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class Outcome : CorrespondenceFact
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
				Outcome newFact = new Outcome(memento);

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
				Outcome fact = (Outcome)obj;
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Outcome", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null instance
        private static Outcome _nullInstance;

        public static Outcome GetNullInstance()
        {
            if (_nullInstance == null)
            {
                _nullInstance = new Outcome((FactMemento)null) { IsNull = true };
            }
            return _nullInstance;
        }

        // Roles
        private static Role _cacheRoleGame;
        public static Role GetRoleGame()
        {
            if (_cacheRoleGame == null)
            {
                _cacheRoleGame = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "game",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Game", 1),
			        true));
            }
            return _cacheRoleGame;
        }
        private static Role _cacheRoleWinner;
        public static Role GetRoleWinner()
        {
            if (_cacheRoleWinner == null)
            {
                _cacheRoleWinner = new Role(new RoleMemento(
			        _correspondenceFactType,
			        "winner",
			        new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Player", 1),
			        false));
            }
            return _cacheRoleWinner;
        }

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Game> _game;
        private PredecessorOpt<Player> _winner;

        // Fields

        // Results

        // Business constructor
        public Outcome(
            Game game
            ,Player winner
            )
        {
            InitializeResults();
            _game = new PredecessorObj<Game>(this, GetRoleGame(), game);
            _winner = new PredecessorOpt<Player>(this, GetRoleWinner(), winner);
        }

        // Hydration constructor
        private Outcome(FactMemento memento)
        {
            InitializeResults();
            _game = new PredecessorObj<Game>(this, GetRoleGame(), memento, Game.GetNullInstance);
            _winner = new PredecessorOpt<Player>(this, GetRoleWinner(), memento, Player.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Game Game
        {
            get { return _game.Fact; }
        }
        public Player Winner
        {
            get { return _winner.Fact; }
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
				Machine._correspondenceFactType,
				new Machine.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Machine._correspondenceFactType }));
			community.AddQuery(
				Machine._correspondenceFactType,
				Machine.GetQueryActiveLogOns().QueryDefinition);
			community.AddType(
				User._correspondenceFactType,
				new User.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { User._correspondenceFactType }));
			community.AddQuery(
				User._correspondenceFactType,
				User.GetQueryFavoriteColor().QueryDefinition);
			community.AddQuery(
				User._correspondenceFactType,
				User.GetQueryBetterFavoriteColor().QueryDefinition);
			community.AddQuery(
				User._correspondenceFactType,
				User.GetQueryActivePlayers().QueryDefinition);
			community.AddQuery(
				User._correspondenceFactType,
				User.GetQueryFinishedPlayers().QueryDefinition);
			community.AddQuery(
				User._correspondenceFactType,
				User.GetQueryFinishedGames().QueryDefinition);
			community.AddType(
				User__favoriteColor._correspondenceFactType,
				new User__favoriteColor.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { User__favoriteColor._correspondenceFactType }));
			community.AddQuery(
				User__favoriteColor._correspondenceFactType,
				User__favoriteColor.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				User__betterFavoriteColor._correspondenceFactType,
				new User__betterFavoriteColor.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { User__betterFavoriteColor._correspondenceFactType }));
			community.AddQuery(
				User__betterFavoriteColor._correspondenceFactType,
				User__betterFavoriteColor.GetQueryIsCurrent().QueryDefinition);
			community.AddType(
				Color._correspondenceFactType,
				new Color.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Color._correspondenceFactType }));
			community.AddType(
				LogOn._correspondenceFactType,
				new LogOn.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { LogOn._correspondenceFactType }));
			community.AddQuery(
				LogOn._correspondenceFactType,
				LogOn.GetQueryIsActive().QueryDefinition);
			community.AddType(
				LogOff._correspondenceFactType,
				new LogOff.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { LogOff._correspondenceFactType }));
			community.AddType(
				Game._correspondenceFactType,
				new Game.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Game._correspondenceFactType }));
			community.AddQuery(
				Game._correspondenceFactType,
				Game.GetQueryPlayers().QueryDefinition);
			community.AddQuery(
				Game._correspondenceFactType,
				Game.GetQueryMoves().QueryDefinition);
			community.AddQuery(
				Game._correspondenceFactType,
				Game.GetQueryOutcomes().QueryDefinition);
			community.AddQuery(
				Game._correspondenceFactType,
				Game.GetQueryIsFinished().QueryDefinition);
			community.AddType(
				GameName._correspondenceFactType,
				new GameName.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { GameName._correspondenceFactType }));
			community.AddType(
				Player._correspondenceFactType,
				new Player.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Player._correspondenceFactType }));
			community.AddQuery(
				Player._correspondenceFactType,
				Player.GetQueryMoves().QueryDefinition);
			community.AddQuery(
				Player._correspondenceFactType,
				Player.GetQueryIsActive().QueryDefinition);
			community.AddQuery(
				Player._correspondenceFactType,
				Player.GetQueryIsNotActive().QueryDefinition);
			community.AddUnpublisher(
				Player.GetRoleUser(),
				Condition.WhereIsEmpty(Player.GetQueryIsActive())
				);
			community.AddUnpublisher(
				Player.GetRoleGame(),
				Condition.WhereIsEmpty(Player.GetQueryIsActive())
				);
			community.AddType(
				Move._correspondenceFactType,
				new Move.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Move._correspondenceFactType }));
			community.AddType(
				Outcome._correspondenceFactType,
				new Outcome.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Outcome._correspondenceFactType }));
		}
	}
}
