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

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Machine.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Machine.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Machine", 2);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Machine GetUnloadedInstance()
        {
            return new Machine((FactMemento)null) { IsLoaded = false };
        }

        public static Machine GetNullInstance()
        {
            return new Machine((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Machine> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Machine)t.Result);
            else
                return Task.FromResult(this);
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
            _activeLogOns = new Result<LogOn>(this, GetQueryActiveLogOns(), LogOn.GetUnloadedInstance, LogOn.GetNullInstance);
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

            public CorrespondenceFact GetUnloadedInstance()
            {
                return User.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return User.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.User", 8);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static User GetUnloadedInstance()
        {
            return new User((FactMemento)null) { IsLoaded = false };
        }

        public static User GetNullInstance()
        {
            return new User((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<User> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (User)t.Result);
            else
                return Task.FromResult(this);
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
            _favoriteColor = new Result<User__favoriteColor>(this, GetQueryFavoriteColor(), User__favoriteColor.GetUnloadedInstance, User__favoriteColor.GetNullInstance);
            _betterFavoriteColor = new Result<User__betterFavoriteColor>(this, GetQueryBetterFavoriteColor(), User__betterFavoriteColor.GetUnloadedInstance, User__betterFavoriteColor.GetNullInstance);
            _activePlayers = new Result<Player>(this, GetQueryActivePlayers(), Player.GetUnloadedInstance, Player.GetNullInstance);
            _finishedPlayers = new Result<Player>(this, GetQueryFinishedPlayers(), Player.GetUnloadedInstance, Player.GetNullInstance);
            _finishedGames = new Result<Game>(this, GetQueryFinishedGames(), Game.GetUnloadedInstance, Game.GetNullInstance);
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
                Action setter = async delegate()
                {
                    var current = (await _favoriteColor.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new User__favoriteColor(this, _favoriteColor, value.Value));
                    }
                };
                setter();
			}
        }

        public TransientDisputable<User__betterFavoriteColor, Color> BetterFavoriteColor
        {
            get { return _betterFavoriteColor.AsTransientDisputable(fact => (Color)fact.Value); }
			set
			{
				Action setter = async delegate()
				{
					var current = (await _betterFavoriteColor.EnsureAsync()).ToList();
					if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
					{
						await Community.AddFactAsync(new User__betterFavoriteColor(this, _betterFavoriteColor, value.Value));
					}
				};
				setter();
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

            public CorrespondenceFact GetUnloadedInstance()
            {
                return User__favoriteColor.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return User__favoriteColor.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.User__favoriteColor", 1286312876);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static User__favoriteColor GetUnloadedInstance()
        {
            return new User__favoriteColor((FactMemento)null) { IsLoaded = false };
        }

        public static User__favoriteColor GetNullInstance()
        {
            return new User__favoriteColor((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<User__favoriteColor> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (User__favoriteColor)t.Result);
            else
                return Task.FromResult(this);
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
			        User._correspondenceFactType,
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
			        User__favoriteColor._correspondenceFactType,
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
            _user = new PredecessorObj<User>(this, GetRoleUser(), memento, User.GetUnloadedInstance, User.GetNullInstance);
            _prior = new PredecessorList<User__favoriteColor>(this, GetRolePrior(), memento, User__favoriteColor.GetUnloadedInstance, User__favoriteColor.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public User User
        {
            get { return IsNull ? User.GetNullInstance() : _user.Fact; }
        }
        public PredecessorList<User__favoriteColor> Prior
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


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				User__betterFavoriteColor fact = (User__betterFavoriteColor)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return User__betterFavoriteColor.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return User__betterFavoriteColor.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.User__betterFavoriteColor", 266066068);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static User__betterFavoriteColor GetUnloadedInstance()
        {
            return new User__betterFavoriteColor((FactMemento)null) { IsLoaded = false };
        }

        public static User__betterFavoriteColor GetNullInstance()
        {
            return new User__betterFavoriteColor((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<User__betterFavoriteColor> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (User__betterFavoriteColor)t.Result);
            else
                return Task.FromResult(this);
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
			        User._correspondenceFactType,
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
			        User__betterFavoriteColor._correspondenceFactType,
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
			        Color._correspondenceFactType,
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
            _user = new PredecessorObj<User>(this, GetRoleUser(), memento, User.GetUnloadedInstance, User.GetNullInstance);
            _prior = new PredecessorList<User__betterFavoriteColor>(this, GetRolePrior(), memento, User__betterFavoriteColor.GetUnloadedInstance, User__betterFavoriteColor.GetNullInstance);
            _value = new PredecessorObj<Color>(this, GetRoleValue(), memento, Color.GetUnloadedInstance, Color.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public User User
        {
            get { return IsNull ? User.GetNullInstance() : _user.Fact; }
        }
        public PredecessorList<User__betterFavoriteColor> Prior
        {
            get { return _prior; }
        }
        public Color Value
        {
            get { return IsNull ? Color.GetNullInstance() : _value.Fact; }
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

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Color.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Color.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Color", 8);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Color GetUnloadedInstance()
        {
            return new Color((FactMemento)null) { IsLoaded = false };
        }

        public static Color GetNullInstance()
        {
            return new Color((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Color> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Color)t.Result);
            else
                return Task.FromResult(this);
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

            public CorrespondenceFact GetUnloadedInstance()
            {
                return LogOn.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return LogOn.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.LogOn", 291259386);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static LogOn GetUnloadedInstance()
        {
            return new LogOn((FactMemento)null) { IsLoaded = false };
        }

        public static LogOn GetNullInstance()
        {
            return new LogOn((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<LogOn> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (LogOn)t.Result);
            else
                return Task.FromResult(this);
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
			        User._correspondenceFactType,
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
			        Machine._correspondenceFactType,
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
            _user = new PredecessorObj<User>(this, GetRoleUser(), memento, User.GetUnloadedInstance, User.GetNullInstance);
            _machine = new PredecessorObj<Machine>(this, GetRoleMachine(), memento, Machine.GetUnloadedInstance, Machine.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public User User
        {
            get { return IsNull ? User.GetNullInstance() : _user.Fact; }
        }
        public Machine Machine
        {
            get { return IsNull ? Machine.GetNullInstance() : _machine.Fact; }
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


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				LogOff fact = (LogOff)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return LogOff.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return LogOff.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.LogOff", -334052112);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static LogOff GetUnloadedInstance()
        {
            return new LogOff((FactMemento)null) { IsLoaded = false };
        }

        public static LogOff GetNullInstance()
        {
            return new LogOff((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<LogOff> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (LogOff)t.Result);
            else
                return Task.FromResult(this);
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
			        LogOn._correspondenceFactType,
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
            _logOn = new PredecessorObj<LogOn>(this, GetRoleLogOn(), memento, LogOn.GetUnloadedInstance, LogOn.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public LogOn LogOn
        {
            get { return IsNull ? LogOn.GetNullInstance() : _logOn.Fact; }
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

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Game.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Game.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Game", 2);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Game GetUnloadedInstance()
        {
            return new Game((FactMemento)null) { IsLoaded = false };
        }

        public static Game GetNullInstance()
        {
            return new Game((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Game> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Game)t.Result);
            else
                return Task.FromResult(this);
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
            _players = new Result<Player>(this, GetQueryPlayers(), Player.GetUnloadedInstance, Player.GetNullInstance);
            _moves = new Result<Move>(this, GetQueryMoves(), Move.GetUnloadedInstance, Move.GetNullInstance);
            _outcomes = new Result<Outcome>(this, GetQueryOutcomes(), Outcome.GetUnloadedInstance, Outcome.GetNullInstance);
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

            public CorrespondenceFact GetUnloadedInstance()
            {
                return GameName.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return GameName.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.GameName", 1716804944);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static GameName GetUnloadedInstance()
        {
            return new GameName((FactMemento)null) { IsLoaded = false };
        }

        public static GameName GetNullInstance()
        {
            return new GameName((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<GameName> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (GameName)t.Result);
            else
                return Task.FromResult(this);
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
			        Game._correspondenceFactType,
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
			        GameName._correspondenceFactType,
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
            _game = new PredecessorObj<Game>(this, GetRoleGame(), memento, Game.GetUnloadedInstance, Game.GetNullInstance);
            _prior = new PredecessorList<GameName>(this, GetRolePrior(), memento, GameName.GetUnloadedInstance, GameName.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Game Game
        {
            get { return IsNull ? Game.GetNullInstance() : _game.Fact; }
        }
        public PredecessorList<GameName> Prior
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

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Player.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Player.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Player", -1815448412);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Player GetUnloadedInstance()
        {
            return new Player((FactMemento)null) { IsLoaded = false };
        }

        public static Player GetNullInstance()
        {
            return new Player((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Player> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Player)t.Result);
            else
                return Task.FromResult(this);
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
			        User._correspondenceFactType,
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
			        Game._correspondenceFactType,
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
            _user = new PredecessorObj<User>(this, GetRoleUser(), memento, User.GetUnloadedInstance, User.GetNullInstance);
            _game = new PredecessorObj<Game>(this, GetRoleGame(), memento, Game.GetUnloadedInstance, Game.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
            _moves = new Result<Move>(this, GetQueryMoves(), Move.GetUnloadedInstance, Move.GetNullInstance);
        }

        // Predecessor access
        public User User
        {
            get { return IsNull ? User.GetNullInstance() : _user.Fact; }
        }
        public Game Game
        {
            get { return IsNull ? Game.GetNullInstance() : _game.Fact; }
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

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Move.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Move.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Move", 99937088);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Move GetUnloadedInstance()
        {
            return new Move((FactMemento)null) { IsLoaded = false };
        }

        public static Move GetNullInstance()
        {
            return new Move((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Move> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Move)t.Result);
            else
                return Task.FromResult(this);
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
			        Player._correspondenceFactType,
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
            _player = new PredecessorObj<Player>(this, GetRolePlayer(), memento, Player.GetUnloadedInstance, Player.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Player Player
        {
            get { return IsNull ? Player.GetNullInstance() : _player.Fact; }
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


				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Outcome fact = (Outcome)obj;
			}

            public CorrespondenceFact GetUnloadedInstance()
            {
                return Outcome.GetUnloadedInstance();
            }

            public CorrespondenceFact GetNullInstance()
            {
                return Outcome.GetNullInstance();
            }
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Outcome", 887939540);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Null and unloaded instances
        public static Outcome GetUnloadedInstance()
        {
            return new Outcome((FactMemento)null) { IsLoaded = false };
        }

        public static Outcome GetNullInstance()
        {
            return new Outcome((FactMemento)null) { IsNull = true };
        }

        // Ensure
        public Task<Outcome> EnsureAsync()
        {
            if (_loadedTask != null)
                return _loadedTask.ContinueWith(t => (Outcome)t.Result);
            else
                return Task.FromResult(this);
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
			        Game._correspondenceFactType,
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
			        Player._correspondenceFactType,
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
            _game = new PredecessorObj<Game>(this, GetRoleGame(), memento, Game.GetUnloadedInstance, Game.GetNullInstance);
            _winner = new PredecessorOpt<Player>(this, GetRoleWinner(), memento, Player.GetUnloadedInstance, Player.GetNullInstance);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Game Game
        {
            get { return IsNull ? Game.GetNullInstance() : _game.Fact; }
        }
        public Player Winner
        {
            get { return IsNull ? Player.GetNullInstance() : _winner.Fact; }
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
