using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System;
using System.IO;

/**
/ For use with http://graphviz.org/
digraph "Incentives.Model"
{
    rankdir=BT
    IndividualProfile -> Individual
    IndividualProfile -> Profile
    Profile__name -> Profile
    Profile__name -> Profile__name [label="  *"]
    Quarter -> Company
    Category -> Company
    Category__description -> Category
    Category__description -> Category__description [label="  *"]
    Category__ordinal -> Category
    Category__ordinal -> Category__ordinal [label="  *"]
    ActivityDefinition -> Category
    ActivityDefinition__description -> ActivityDefinition
    ActivityDefinition__description -> ActivityDefinition__description [label="  *"]
    ActivityDefinition__qualifier -> ActivityDefinition
    ActivityDefinition__qualifier -> ActivityDefinition__qualifier [label="  *"]
    ActivityDefinition__ordinal -> ActivityDefinition
    ActivityDefinition__ordinal -> ActivityDefinition__ordinal [label="  *"]
    ActivityReward -> ActivityDefinition
    ActivityReward -> Quarter
    ActivityReward__points -> ActivityReward
    ActivityReward__points -> ActivityReward__points [label="  *"]
    ProfileQuarter -> Profile
    ProfileQuarter -> Quarter
    Activity -> ProfileQuarter
    Activity -> ActivityReward
    Activity__description -> Activity
    Activity__description -> Activity__description [label="  *"]
    Activity__multiplier -> Activity
    Activity__multiplier -> Activity__multiplier [label="  *"]
}
**/

namespace Incentives.Model
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

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Individual fact = (Individual)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._anonymousId);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.Individual", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles

        // Queries
        public static Query MakeQueryProfiles()
		{
			return new Query()
				.JoinSuccessors(IndividualProfile.RoleIndividual)
				.JoinPredecessors(IndividualProfile.RoleProfile)
            ;
		}
        public static Query QueryProfiles = MakeQueryProfiles();

        // Predicates

        // Predecessors

        // Fields
        private string _anonymousId;

        // Results
        private Result<Profile> _profiles;

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
            _profiles = new Result<Profile>(this, QueryProfiles);
        }

        // Predecessor access

        // Field access
        public string AnonymousId
        {
            get { return _anonymousId; }
        }

        // Query result access
        public Result<Profile> Profiles
        {
            get { return _profiles; }
        }

        // Mutable property access

    }
    
    public partial class IndividualProfile : CorrespondenceFact
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
				IndividualProfile newFact = new IndividualProfile(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				IndividualProfile fact = (IndividualProfile)obj;
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.IndividualProfile", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleIndividual = new Role(new RoleMemento(
			_correspondenceFactType,
			"individual",
			new CorrespondenceFactType("Incentives.Model.Individual", 1),
			false));
        public static Role RoleProfile = new Role(new RoleMemento(
			_correspondenceFactType,
			"profile",
			new CorrespondenceFactType("Incentives.Model.Profile", 1),
			false));

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Individual> _individual;
        private PredecessorObj<Profile> _profile;

        // Fields

        // Results

        // Business constructor
        public IndividualProfile(
            Individual individual
            ,Profile profile
            )
        {
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, RoleIndividual, individual);
            _profile = new PredecessorObj<Profile>(this, RoleProfile, profile);
        }

        // Hydration constructor
        private IndividualProfile(FactMemento memento)
        {
            InitializeResults();
            _individual = new PredecessorObj<Individual>(this, RoleIndividual, memento);
            _profile = new PredecessorObj<Profile>(this, RoleProfile, memento);
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
        public Profile Profile
        {
            get { return _profile.Fact; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class Profile : CorrespondenceFact
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
				Profile newFact = new Profile(memento);

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

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Profile fact = (Profile)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.Profile", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles

        // Queries
        public static Query MakeQueryName()
		{
			return new Query()
				.JoinSuccessors(Profile__name.RoleProfile, Condition.WhereIsEmpty(Profile__name.MakeQueryIsCurrent())
				)
            ;
		}
        public static Query QueryName = MakeQueryName();
        public static Query MakeQueryActivities()
		{
			return new Query()
				.JoinSuccessors(ProfileQuarter.RoleProfile)
				.JoinSuccessors(Activity.RoleProfileQuarter)
            ;
		}
        public static Query QueryActivities = MakeQueryActivities();

        // Predicates

        // Predecessors

        // Unique
        private Guid _unique;

        // Fields

        // Results
        private Result<Profile__name> _name;
        private Result<Activity> _activities;

        // Business constructor
        public Profile(
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
        }

        // Hydration constructor
        private Profile(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _name = new Result<Profile__name>(this, QueryName);
            _activities = new Result<Activity>(this, QueryActivities);
        }

        // Predecessor access

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access
        public Result<Activity> Activities
        {
            get { return _activities; }
        }

        // Mutable property access
        public TransientDisputable<Profile__name, string> Name
        {
            get { return _name.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Action setter = async delegate()
                {
                    var current = (await _name.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Profile__name(this, _name, value.Value));
                    }
                };
                setter();
			}
        }

    }
    
    public partial class Profile__name : CorrespondenceFact
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
				Profile__name newFact = new Profile__name(memento);

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

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Profile__name fact = (Profile__name)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.Profile__name", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleProfile = new Role(new RoleMemento(
			_correspondenceFactType,
			"profile",
			new CorrespondenceFactType("Incentives.Model.Profile", 1),
			false));
        public static Role RolePrior = new Role(new RoleMemento(
			_correspondenceFactType,
			"prior",
			new CorrespondenceFactType("Incentives.Model.Profile__name", 1),
			false));

        // Queries
        public static Query MakeQueryIsCurrent()
		{
			return new Query()
				.JoinSuccessors(Profile__name.RolePrior)
            ;
		}
        public static Query QueryIsCurrent = MakeQueryIsCurrent();

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(QueryIsCurrent);

        // Predecessors
        private PredecessorObj<Profile> _profile;
        private PredecessorList<Profile__name> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public Profile__name(
            Profile profile
            ,IEnumerable<Profile__name> prior
            ,string value
            )
        {
            InitializeResults();
            _profile = new PredecessorObj<Profile>(this, RoleProfile, profile);
            _prior = new PredecessorList<Profile__name>(this, RolePrior, prior);
            _value = value;
        }

        // Hydration constructor
        private Profile__name(FactMemento memento)
        {
            InitializeResults();
            _profile = new PredecessorObj<Profile>(this, RoleProfile, memento);
            _prior = new PredecessorList<Profile__name>(this, RolePrior, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Profile Profile
        {
            get { return _profile.Fact; }
        }
        public IEnumerable<Profile__name> Prior
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
    
    public partial class Company : CorrespondenceFact
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
				Company newFact = new Company(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._identifier = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Company fact = (Company)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._identifier);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.Company", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles

        // Queries
        public static Query MakeQueryCategories()
		{
			return new Query()
				.JoinSuccessors(Category.RoleCompany)
            ;
		}
        public static Query QueryCategories = MakeQueryCategories();

        // Predicates

        // Predecessors

        // Fields
        private string _identifier;

        // Results
        private Result<Category> _categories;

        // Business constructor
        public Company(
            string identifier
            )
        {
            InitializeResults();
            _identifier = identifier;
        }

        // Hydration constructor
        private Company(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _categories = new Result<Category>(this, QueryCategories);
        }

        // Predecessor access

        // Field access
        public string Identifier
        {
            get { return _identifier; }
        }

        // Query result access
        public Result<Category> Categories
        {
            get { return _categories; }
        }

        // Mutable property access

    }
    
    public partial class Quarter : CorrespondenceFact
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
				Quarter newFact = new Quarter(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._startDate = (DateTime)_fieldSerializerByType[typeof(DateTime)].ReadData(output);
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Quarter fact = (Quarter)obj;
				_fieldSerializerByType[typeof(DateTime)].WriteData(output, fact._startDate);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.Quarter", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleCompany = new Role(new RoleMemento(
			_correspondenceFactType,
			"company",
			new CorrespondenceFactType("Incentives.Model.Company", 1),
			false));

        // Queries
        public static Query MakeQueryRewards()
		{
			return new Query()
				.JoinSuccessors(ActivityReward.RoleQuarter)
            ;
		}
        public static Query QueryRewards = MakeQueryRewards();

        // Predicates

        // Predecessors
        private PredecessorObj<Company> _company;

        // Fields
        private DateTime _startDate;

        // Results
        private Result<ActivityReward> _rewards;

        // Business constructor
        public Quarter(
            Company company
            ,DateTime startDate
            )
        {
            InitializeResults();
            _company = new PredecessorObj<Company>(this, RoleCompany, company);
            _startDate = startDate;
        }

        // Hydration constructor
        private Quarter(FactMemento memento)
        {
            InitializeResults();
            _company = new PredecessorObj<Company>(this, RoleCompany, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
            _rewards = new Result<ActivityReward>(this, QueryRewards);
        }

        // Predecessor access
        public Company Company
        {
            get { return _company.Fact; }
        }

        // Field access
        public DateTime StartDate
        {
            get { return _startDate; }
        }

        // Query result access
        public Result<ActivityReward> Rewards
        {
            get { return _rewards; }
        }

        // Mutable property access

    }
    
    public partial class Category : CorrespondenceFact
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
				Category newFact = new Category(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._identifier = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Category fact = (Category)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._identifier);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.Category", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleCompany = new Role(new RoleMemento(
			_correspondenceFactType,
			"company",
			new CorrespondenceFactType("Incentives.Model.Company", 1),
			false));

        // Queries
        public static Query MakeQueryDescription()
		{
			return new Query()
				.JoinSuccessors(Category__description.RoleCategory, Condition.WhereIsEmpty(Category__description.MakeQueryIsCurrent())
				)
            ;
		}
        public static Query QueryDescription = MakeQueryDescription();
        public static Query MakeQueryOrdinal()
		{
			return new Query()
				.JoinSuccessors(Category__ordinal.RoleCategory, Condition.WhereIsEmpty(Category__ordinal.MakeQueryIsCurrent())
				)
            ;
		}
        public static Query QueryOrdinal = MakeQueryOrdinal();
        public static Query MakeQueryActivities()
		{
			return new Query()
				.JoinSuccessors(ActivityDefinition.RoleCategory)
            ;
		}
        public static Query QueryActivities = MakeQueryActivities();

        // Predicates

        // Predecessors
        private PredecessorObj<Company> _company;

        // Fields
        private string _identifier;

        // Results
        private Result<Category__description> _description;
        private Result<Category__ordinal> _ordinal;
        private Result<ActivityDefinition> _activities;

        // Business constructor
        public Category(
            Company company
            ,string identifier
            )
        {
            InitializeResults();
            _company = new PredecessorObj<Company>(this, RoleCompany, company);
            _identifier = identifier;
        }

        // Hydration constructor
        private Category(FactMemento memento)
        {
            InitializeResults();
            _company = new PredecessorObj<Company>(this, RoleCompany, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
            _description = new Result<Category__description>(this, QueryDescription);
            _ordinal = new Result<Category__ordinal>(this, QueryOrdinal);
            _activities = new Result<ActivityDefinition>(this, QueryActivities);
        }

        // Predecessor access
        public Company Company
        {
            get { return _company.Fact; }
        }

        // Field access
        public string Identifier
        {
            get { return _identifier; }
        }

        // Query result access
        public Result<ActivityDefinition> Activities
        {
            get { return _activities; }
        }

        // Mutable property access
        public TransientDisputable<Category__description, string> Description
        {
            get { return _description.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Action setter = async delegate()
                {
                    var current = (await _description.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Category__description(this, _description, value.Value));
                    }
                };
                setter();
			}
        }
        public TransientDisputable<Category__ordinal, int> Ordinal
        {
            get { return _ordinal.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Action setter = async delegate()
                {
                    var current = (await _ordinal.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Category__ordinal(this, _ordinal, value.Value));
                    }
                };
                setter();
			}
        }

    }
    
    public partial class Category__description : CorrespondenceFact
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
				Category__description newFact = new Category__description(memento);

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

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Category__description fact = (Category__description)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.Category__description", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleCategory = new Role(new RoleMemento(
			_correspondenceFactType,
			"category",
			new CorrespondenceFactType("Incentives.Model.Category", 1),
			false));
        public static Role RolePrior = new Role(new RoleMemento(
			_correspondenceFactType,
			"prior",
			new CorrespondenceFactType("Incentives.Model.Category__description", 1),
			false));

        // Queries
        public static Query MakeQueryIsCurrent()
		{
			return new Query()
				.JoinSuccessors(Category__description.RolePrior)
            ;
		}
        public static Query QueryIsCurrent = MakeQueryIsCurrent();

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(QueryIsCurrent);

        // Predecessors
        private PredecessorObj<Category> _category;
        private PredecessorList<Category__description> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public Category__description(
            Category category
            ,IEnumerable<Category__description> prior
            ,string value
            )
        {
            InitializeResults();
            _category = new PredecessorObj<Category>(this, RoleCategory, category);
            _prior = new PredecessorList<Category__description>(this, RolePrior, prior);
            _value = value;
        }

        // Hydration constructor
        private Category__description(FactMemento memento)
        {
            InitializeResults();
            _category = new PredecessorObj<Category>(this, RoleCategory, memento);
            _prior = new PredecessorList<Category__description>(this, RolePrior, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Category Category
        {
            get { return _category.Fact; }
        }
        public IEnumerable<Category__description> Prior
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
    
    public partial class Category__ordinal : CorrespondenceFact
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
				Category__ordinal newFact = new Category__ordinal(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (int)_fieldSerializerByType[typeof(int)].ReadData(output);
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Category__ordinal fact = (Category__ordinal)obj;
				_fieldSerializerByType[typeof(int)].WriteData(output, fact._value);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.Category__ordinal", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleCategory = new Role(new RoleMemento(
			_correspondenceFactType,
			"category",
			new CorrespondenceFactType("Incentives.Model.Category", 1),
			false));
        public static Role RolePrior = new Role(new RoleMemento(
			_correspondenceFactType,
			"prior",
			new CorrespondenceFactType("Incentives.Model.Category__ordinal", 1),
			false));

        // Queries
        public static Query MakeQueryIsCurrent()
		{
			return new Query()
				.JoinSuccessors(Category__ordinal.RolePrior)
            ;
		}
        public static Query QueryIsCurrent = MakeQueryIsCurrent();

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(QueryIsCurrent);

        // Predecessors
        private PredecessorObj<Category> _category;
        private PredecessorList<Category__ordinal> _prior;

        // Fields
        private int _value;

        // Results

        // Business constructor
        public Category__ordinal(
            Category category
            ,IEnumerable<Category__ordinal> prior
            ,int value
            )
        {
            InitializeResults();
            _category = new PredecessorObj<Category>(this, RoleCategory, category);
            _prior = new PredecessorList<Category__ordinal>(this, RolePrior, prior);
            _value = value;
        }

        // Hydration constructor
        private Category__ordinal(FactMemento memento)
        {
            InitializeResults();
            _category = new PredecessorObj<Category>(this, RoleCategory, memento);
            _prior = new PredecessorList<Category__ordinal>(this, RolePrior, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Category Category
        {
            get { return _category.Fact; }
        }
        public IEnumerable<Category__ordinal> Prior
        {
            get { return _prior; }
        }
     
        // Field access
        public int Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class ActivityDefinition : CorrespondenceFact
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
				ActivityDefinition newFact = new ActivityDefinition(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._identifier = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				ActivityDefinition fact = (ActivityDefinition)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._identifier);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.ActivityDefinition", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleCategory = new Role(new RoleMemento(
			_correspondenceFactType,
			"category",
			new CorrespondenceFactType("Incentives.Model.Category", 1),
			false));

        // Queries
        public static Query MakeQueryDescription()
		{
			return new Query()
				.JoinSuccessors(ActivityDefinition__description.RoleActivityDefinition, Condition.WhereIsEmpty(ActivityDefinition__description.MakeQueryIsCurrent())
				)
            ;
		}
        public static Query QueryDescription = MakeQueryDescription();
        public static Query MakeQueryQualifier()
		{
			return new Query()
				.JoinSuccessors(ActivityDefinition__qualifier.RoleActivityDefinition, Condition.WhereIsEmpty(ActivityDefinition__qualifier.MakeQueryIsCurrent())
				)
            ;
		}
        public static Query QueryQualifier = MakeQueryQualifier();
        public static Query MakeQueryOrdinal()
		{
			return new Query()
				.JoinSuccessors(ActivityDefinition__ordinal.RoleActivityDefinition, Condition.WhereIsEmpty(ActivityDefinition__ordinal.MakeQueryIsCurrent())
				)
            ;
		}
        public static Query QueryOrdinal = MakeQueryOrdinal();

        // Predicates

        // Predecessors
        private PredecessorObj<Category> _category;

        // Fields
        private string _identifier;

        // Results
        private Result<ActivityDefinition__description> _description;
        private Result<ActivityDefinition__qualifier> _qualifier;
        private Result<ActivityDefinition__ordinal> _ordinal;

        // Business constructor
        public ActivityDefinition(
            Category category
            ,string identifier
            )
        {
            InitializeResults();
            _category = new PredecessorObj<Category>(this, RoleCategory, category);
            _identifier = identifier;
        }

        // Hydration constructor
        private ActivityDefinition(FactMemento memento)
        {
            InitializeResults();
            _category = new PredecessorObj<Category>(this, RoleCategory, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
            _description = new Result<ActivityDefinition__description>(this, QueryDescription);
            _qualifier = new Result<ActivityDefinition__qualifier>(this, QueryQualifier);
            _ordinal = new Result<ActivityDefinition__ordinal>(this, QueryOrdinal);
        }

        // Predecessor access
        public Category Category
        {
            get { return _category.Fact; }
        }

        // Field access
        public string Identifier
        {
            get { return _identifier; }
        }

        // Query result access

        // Mutable property access
        public TransientDisputable<ActivityDefinition__description, string> Description
        {
            get { return _description.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Action setter = async delegate()
                {
                    var current = (await _description.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new ActivityDefinition__description(this, _description, value.Value));
                    }
                };
                setter();
			}
        }
        public TransientDisputable<ActivityDefinition__qualifier, string> Qualifier
        {
            get { return _qualifier.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Action setter = async delegate()
                {
                    var current = (await _qualifier.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new ActivityDefinition__qualifier(this, _qualifier, value.Value));
                    }
                };
                setter();
			}
        }
        public TransientDisputable<ActivityDefinition__ordinal, int> Ordinal
        {
            get { return _ordinal.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Action setter = async delegate()
                {
                    var current = (await _ordinal.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new ActivityDefinition__ordinal(this, _ordinal, value.Value));
                    }
                };
                setter();
			}
        }

    }
    
    public partial class ActivityDefinition__description : CorrespondenceFact
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
				ActivityDefinition__description newFact = new ActivityDefinition__description(memento);

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

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				ActivityDefinition__description fact = (ActivityDefinition__description)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.ActivityDefinition__description", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleActivityDefinition = new Role(new RoleMemento(
			_correspondenceFactType,
			"activityDefinition",
			new CorrespondenceFactType("Incentives.Model.ActivityDefinition", 1),
			false));
        public static Role RolePrior = new Role(new RoleMemento(
			_correspondenceFactType,
			"prior",
			new CorrespondenceFactType("Incentives.Model.ActivityDefinition__description", 1),
			false));

        // Queries
        public static Query MakeQueryIsCurrent()
		{
			return new Query()
				.JoinSuccessors(ActivityDefinition__description.RolePrior)
            ;
		}
        public static Query QueryIsCurrent = MakeQueryIsCurrent();

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(QueryIsCurrent);

        // Predecessors
        private PredecessorObj<ActivityDefinition> _activityDefinition;
        private PredecessorList<ActivityDefinition__description> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public ActivityDefinition__description(
            ActivityDefinition activityDefinition
            ,IEnumerable<ActivityDefinition__description> prior
            ,string value
            )
        {
            InitializeResults();
            _activityDefinition = new PredecessorObj<ActivityDefinition>(this, RoleActivityDefinition, activityDefinition);
            _prior = new PredecessorList<ActivityDefinition__description>(this, RolePrior, prior);
            _value = value;
        }

        // Hydration constructor
        private ActivityDefinition__description(FactMemento memento)
        {
            InitializeResults();
            _activityDefinition = new PredecessorObj<ActivityDefinition>(this, RoleActivityDefinition, memento);
            _prior = new PredecessorList<ActivityDefinition__description>(this, RolePrior, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public ActivityDefinition ActivityDefinition
        {
            get { return _activityDefinition.Fact; }
        }
        public IEnumerable<ActivityDefinition__description> Prior
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
    
    public partial class ActivityDefinition__qualifier : CorrespondenceFact
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
				ActivityDefinition__qualifier newFact = new ActivityDefinition__qualifier(memento);

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

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				ActivityDefinition__qualifier fact = (ActivityDefinition__qualifier)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.ActivityDefinition__qualifier", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleActivityDefinition = new Role(new RoleMemento(
			_correspondenceFactType,
			"activityDefinition",
			new CorrespondenceFactType("Incentives.Model.ActivityDefinition", 1),
			false));
        public static Role RolePrior = new Role(new RoleMemento(
			_correspondenceFactType,
			"prior",
			new CorrespondenceFactType("Incentives.Model.ActivityDefinition__qualifier", 1),
			false));

        // Queries
        public static Query MakeQueryIsCurrent()
		{
			return new Query()
				.JoinSuccessors(ActivityDefinition__qualifier.RolePrior)
            ;
		}
        public static Query QueryIsCurrent = MakeQueryIsCurrent();

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(QueryIsCurrent);

        // Predecessors
        private PredecessorObj<ActivityDefinition> _activityDefinition;
        private PredecessorList<ActivityDefinition__qualifier> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public ActivityDefinition__qualifier(
            ActivityDefinition activityDefinition
            ,IEnumerable<ActivityDefinition__qualifier> prior
            ,string value
            )
        {
            InitializeResults();
            _activityDefinition = new PredecessorObj<ActivityDefinition>(this, RoleActivityDefinition, activityDefinition);
            _prior = new PredecessorList<ActivityDefinition__qualifier>(this, RolePrior, prior);
            _value = value;
        }

        // Hydration constructor
        private ActivityDefinition__qualifier(FactMemento memento)
        {
            InitializeResults();
            _activityDefinition = new PredecessorObj<ActivityDefinition>(this, RoleActivityDefinition, memento);
            _prior = new PredecessorList<ActivityDefinition__qualifier>(this, RolePrior, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public ActivityDefinition ActivityDefinition
        {
            get { return _activityDefinition.Fact; }
        }
        public IEnumerable<ActivityDefinition__qualifier> Prior
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
    
    public partial class ActivityDefinition__ordinal : CorrespondenceFact
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
				ActivityDefinition__ordinal newFact = new ActivityDefinition__ordinal(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (int)_fieldSerializerByType[typeof(int)].ReadData(output);
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				ActivityDefinition__ordinal fact = (ActivityDefinition__ordinal)obj;
				_fieldSerializerByType[typeof(int)].WriteData(output, fact._value);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.ActivityDefinition__ordinal", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleActivityDefinition = new Role(new RoleMemento(
			_correspondenceFactType,
			"activityDefinition",
			new CorrespondenceFactType("Incentives.Model.ActivityDefinition", 1),
			false));
        public static Role RolePrior = new Role(new RoleMemento(
			_correspondenceFactType,
			"prior",
			new CorrespondenceFactType("Incentives.Model.ActivityDefinition__ordinal", 1),
			false));

        // Queries
        public static Query MakeQueryIsCurrent()
		{
			return new Query()
				.JoinSuccessors(ActivityDefinition__ordinal.RolePrior)
            ;
		}
        public static Query QueryIsCurrent = MakeQueryIsCurrent();

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(QueryIsCurrent);

        // Predecessors
        private PredecessorObj<ActivityDefinition> _activityDefinition;
        private PredecessorList<ActivityDefinition__ordinal> _prior;

        // Fields
        private int _value;

        // Results

        // Business constructor
        public ActivityDefinition__ordinal(
            ActivityDefinition activityDefinition
            ,IEnumerable<ActivityDefinition__ordinal> prior
            ,int value
            )
        {
            InitializeResults();
            _activityDefinition = new PredecessorObj<ActivityDefinition>(this, RoleActivityDefinition, activityDefinition);
            _prior = new PredecessorList<ActivityDefinition__ordinal>(this, RolePrior, prior);
            _value = value;
        }

        // Hydration constructor
        private ActivityDefinition__ordinal(FactMemento memento)
        {
            InitializeResults();
            _activityDefinition = new PredecessorObj<ActivityDefinition>(this, RoleActivityDefinition, memento);
            _prior = new PredecessorList<ActivityDefinition__ordinal>(this, RolePrior, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public ActivityDefinition ActivityDefinition
        {
            get { return _activityDefinition.Fact; }
        }
        public IEnumerable<ActivityDefinition__ordinal> Prior
        {
            get { return _prior; }
        }
     
        // Field access
        public int Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class ActivityReward : CorrespondenceFact
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
				ActivityReward newFact = new ActivityReward(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				ActivityReward fact = (ActivityReward)obj;
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.ActivityReward", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleDefinition = new Role(new RoleMemento(
			_correspondenceFactType,
			"definition",
			new CorrespondenceFactType("Incentives.Model.ActivityDefinition", 1),
			false));
        public static Role RoleQuarter = new Role(new RoleMemento(
			_correspondenceFactType,
			"quarter",
			new CorrespondenceFactType("Incentives.Model.Quarter", 1),
			false));

        // Queries
        public static Query MakeQueryPoints()
		{
			return new Query()
				.JoinSuccessors(ActivityReward__points.RoleActivityReward, Condition.WhereIsEmpty(ActivityReward__points.MakeQueryIsCurrent())
				)
            ;
		}
        public static Query QueryPoints = MakeQueryPoints();

        // Predicates

        // Predecessors
        private PredecessorObj<ActivityDefinition> _definition;
        private PredecessorObj<Quarter> _quarter;

        // Fields

        // Results
        private Result<ActivityReward__points> _points;

        // Business constructor
        public ActivityReward(
            ActivityDefinition definition
            ,Quarter quarter
            )
        {
            InitializeResults();
            _definition = new PredecessorObj<ActivityDefinition>(this, RoleDefinition, definition);
            _quarter = new PredecessorObj<Quarter>(this, RoleQuarter, quarter);
        }

        // Hydration constructor
        private ActivityReward(FactMemento memento)
        {
            InitializeResults();
            _definition = new PredecessorObj<ActivityDefinition>(this, RoleDefinition, memento);
            _quarter = new PredecessorObj<Quarter>(this, RoleQuarter, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
            _points = new Result<ActivityReward__points>(this, QueryPoints);
        }

        // Predecessor access
        public ActivityDefinition Definition
        {
            get { return _definition.Fact; }
        }
        public Quarter Quarter
        {
            get { return _quarter.Fact; }
        }

        // Field access

        // Query result access

        // Mutable property access
        public TransientDisputable<ActivityReward__points, int> Points
        {
            get { return _points.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Action setter = async delegate()
                {
                    var current = (await _points.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new ActivityReward__points(this, _points, value.Value));
                    }
                };
                setter();
			}
        }

    }
    
    public partial class ActivityReward__points : CorrespondenceFact
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
				ActivityReward__points newFact = new ActivityReward__points(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (int)_fieldSerializerByType[typeof(int)].ReadData(output);
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				ActivityReward__points fact = (ActivityReward__points)obj;
				_fieldSerializerByType[typeof(int)].WriteData(output, fact._value);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.ActivityReward__points", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleActivityReward = new Role(new RoleMemento(
			_correspondenceFactType,
			"activityReward",
			new CorrespondenceFactType("Incentives.Model.ActivityReward", 1),
			false));
        public static Role RolePrior = new Role(new RoleMemento(
			_correspondenceFactType,
			"prior",
			new CorrespondenceFactType("Incentives.Model.ActivityReward__points", 1),
			false));

        // Queries
        public static Query MakeQueryIsCurrent()
		{
			return new Query()
				.JoinSuccessors(ActivityReward__points.RolePrior)
            ;
		}
        public static Query QueryIsCurrent = MakeQueryIsCurrent();

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(QueryIsCurrent);

        // Predecessors
        private PredecessorObj<ActivityReward> _activityReward;
        private PredecessorList<ActivityReward__points> _prior;

        // Fields
        private int _value;

        // Results

        // Business constructor
        public ActivityReward__points(
            ActivityReward activityReward
            ,IEnumerable<ActivityReward__points> prior
            ,int value
            )
        {
            InitializeResults();
            _activityReward = new PredecessorObj<ActivityReward>(this, RoleActivityReward, activityReward);
            _prior = new PredecessorList<ActivityReward__points>(this, RolePrior, prior);
            _value = value;
        }

        // Hydration constructor
        private ActivityReward__points(FactMemento memento)
        {
            InitializeResults();
            _activityReward = new PredecessorObj<ActivityReward>(this, RoleActivityReward, memento);
            _prior = new PredecessorList<ActivityReward__points>(this, RolePrior, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public ActivityReward ActivityReward
        {
            get { return _activityReward.Fact; }
        }
        public IEnumerable<ActivityReward__points> Prior
        {
            get { return _prior; }
        }
     
        // Field access
        public int Value
        {
            get { return _value; }
        }

        // Query result access

        // Mutable property access

    }
    
    public partial class ProfileQuarter : CorrespondenceFact
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
				ProfileQuarter newFact = new ProfileQuarter(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				ProfileQuarter fact = (ProfileQuarter)obj;
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.ProfileQuarter", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleProfile = new Role(new RoleMemento(
			_correspondenceFactType,
			"profile",
			new CorrespondenceFactType("Incentives.Model.Profile", 1),
			false));
        public static Role RoleQuarter = new Role(new RoleMemento(
			_correspondenceFactType,
			"quarter",
			new CorrespondenceFactType("Incentives.Model.Quarter", 1),
			false));

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Profile> _profile;
        private PredecessorObj<Quarter> _quarter;

        // Fields

        // Results

        // Business constructor
        public ProfileQuarter(
            Profile profile
            ,Quarter quarter
            )
        {
            InitializeResults();
            _profile = new PredecessorObj<Profile>(this, RoleProfile, profile);
            _quarter = new PredecessorObj<Quarter>(this, RoleQuarter, quarter);
        }

        // Hydration constructor
        private ProfileQuarter(FactMemento memento)
        {
            InitializeResults();
            _profile = new PredecessorObj<Profile>(this, RoleProfile, memento);
            _quarter = new PredecessorObj<Quarter>(this, RoleQuarter, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Profile Profile
        {
            get { return _profile.Fact; }
        }
        public Quarter Quarter
        {
            get { return _quarter.Fact; }
        }

        // Field access

        // Query result access

        // Mutable property access

    }
    
    public partial class Activity : CorrespondenceFact
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
				Activity newFact = new Activity(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._activityDate = (DateTime)_fieldSerializerByType[typeof(DateTime)].ReadData(output);
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Activity fact = (Activity)obj;
				_fieldSerializerByType[typeof(DateTime)].WriteData(output, fact._activityDate);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.Activity", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleProfileQuarter = new Role(new RoleMemento(
			_correspondenceFactType,
			"profileQuarter",
			new CorrespondenceFactType("Incentives.Model.ProfileQuarter", 1),
			false));
        public static Role RoleActivityReward = new Role(new RoleMemento(
			_correspondenceFactType,
			"activityReward",
			new CorrespondenceFactType("Incentives.Model.ActivityReward", 1),
			false));

        // Queries
        public static Query MakeQueryDescription()
		{
			return new Query()
				.JoinSuccessors(Activity__description.RoleActivity, Condition.WhereIsEmpty(Activity__description.MakeQueryIsCurrent())
				)
            ;
		}
        public static Query QueryDescription = MakeQueryDescription();
        public static Query MakeQueryMultiplier()
		{
			return new Query()
				.JoinSuccessors(Activity__multiplier.RoleActivity, Condition.WhereIsEmpty(Activity__multiplier.MakeQueryIsCurrent())
				)
            ;
		}
        public static Query QueryMultiplier = MakeQueryMultiplier();

        // Predicates

        // Predecessors
        private PredecessorObj<ProfileQuarter> _profileQuarter;
        private PredecessorObj<ActivityReward> _activityReward;

        // Fields
        private DateTime _activityDate;

        // Results
        private Result<Activity__description> _description;
        private Result<Activity__multiplier> _multiplier;

        // Business constructor
        public Activity(
            ProfileQuarter profileQuarter
            ,ActivityReward activityReward
            ,DateTime activityDate
            )
        {
            InitializeResults();
            _profileQuarter = new PredecessorObj<ProfileQuarter>(this, RoleProfileQuarter, profileQuarter);
            _activityReward = new PredecessorObj<ActivityReward>(this, RoleActivityReward, activityReward);
            _activityDate = activityDate;
        }

        // Hydration constructor
        private Activity(FactMemento memento)
        {
            InitializeResults();
            _profileQuarter = new PredecessorObj<ProfileQuarter>(this, RoleProfileQuarter, memento);
            _activityReward = new PredecessorObj<ActivityReward>(this, RoleActivityReward, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
            _description = new Result<Activity__description>(this, QueryDescription);
            _multiplier = new Result<Activity__multiplier>(this, QueryMultiplier);
        }

        // Predecessor access
        public ProfileQuarter ProfileQuarter
        {
            get { return _profileQuarter.Fact; }
        }
        public ActivityReward ActivityReward
        {
            get { return _activityReward.Fact; }
        }

        // Field access
        public DateTime ActivityDate
        {
            get { return _activityDate; }
        }

        // Query result access

        // Mutable property access
        public TransientDisputable<Activity__description, string> Description
        {
            get { return _description.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Action setter = async delegate()
                {
                    var current = (await _description.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Activity__description(this, _description, value.Value));
                    }
                };
                setter();
			}
        }
        public TransientDisputable<Activity__multiplier, int> Multiplier
        {
            get { return _multiplier.AsTransientDisputable(fact => fact.Value); }
			set
			{
                Action setter = async delegate()
                {
                    var current = (await _multiplier.EnsureAsync()).ToList();
                    if (current.Count != 1 || !object.Equals(current[0].Value, value.Value))
                    {
                        await Community.AddFactAsync(new Activity__multiplier(this, _multiplier, value.Value));
                    }
                };
                setter();
			}
        }

    }
    
    public partial class Activity__description : CorrespondenceFact
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
				Activity__description newFact = new Activity__description(memento);

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

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Activity__description fact = (Activity__description)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._value);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.Activity__description", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleActivity = new Role(new RoleMemento(
			_correspondenceFactType,
			"activity",
			new CorrespondenceFactType("Incentives.Model.Activity", 1),
			false));
        public static Role RolePrior = new Role(new RoleMemento(
			_correspondenceFactType,
			"prior",
			new CorrespondenceFactType("Incentives.Model.Activity__description", 1),
			false));

        // Queries
        public static Query MakeQueryIsCurrent()
		{
			return new Query()
				.JoinSuccessors(Activity__description.RolePrior)
            ;
		}
        public static Query QueryIsCurrent = MakeQueryIsCurrent();

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(QueryIsCurrent);

        // Predecessors
        private PredecessorObj<Activity> _activity;
        private PredecessorList<Activity__description> _prior;

        // Fields
        private string _value;

        // Results

        // Business constructor
        public Activity__description(
            Activity activity
            ,IEnumerable<Activity__description> prior
            ,string value
            )
        {
            InitializeResults();
            _activity = new PredecessorObj<Activity>(this, RoleActivity, activity);
            _prior = new PredecessorList<Activity__description>(this, RolePrior, prior);
            _value = value;
        }

        // Hydration constructor
        private Activity__description(FactMemento memento)
        {
            InitializeResults();
            _activity = new PredecessorObj<Activity>(this, RoleActivity, memento);
            _prior = new PredecessorList<Activity__description>(this, RolePrior, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Activity Activity
        {
            get { return _activity.Fact; }
        }
        public IEnumerable<Activity__description> Prior
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
    
    public partial class Activity__multiplier : CorrespondenceFact
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
				Activity__multiplier newFact = new Activity__multiplier(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._value = (int)_fieldSerializerByType[typeof(int)].ReadData(output);
					}
				}

				return newFact;
			}

            public T GetUnloadedInstance()
            {
                throw new NotImplementedException();
            }
            public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Activity__multiplier fact = (Activity__multiplier)obj;
				_fieldSerializerByType[typeof(int)].WriteData(output, fact._value);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"Incentives.Model.Activity__multiplier", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleActivity = new Role(new RoleMemento(
			_correspondenceFactType,
			"activity",
			new CorrespondenceFactType("Incentives.Model.Activity", 1),
			false));
        public static Role RolePrior = new Role(new RoleMemento(
			_correspondenceFactType,
			"prior",
			new CorrespondenceFactType("Incentives.Model.Activity__multiplier", 1),
			false));

        // Queries
        public static Query MakeQueryIsCurrent()
		{
			return new Query()
				.JoinSuccessors(Activity__multiplier.RolePrior)
            ;
		}
        public static Query QueryIsCurrent = MakeQueryIsCurrent();

        // Predicates
        public static Condition IsCurrent = Condition.WhereIsEmpty(QueryIsCurrent);

        // Predecessors
        private PredecessorObj<Activity> _activity;
        private PredecessorList<Activity__multiplier> _prior;

        // Fields
        private int _value;

        // Results

        // Business constructor
        public Activity__multiplier(
            Activity activity
            ,IEnumerable<Activity__multiplier> prior
            ,int value
            )
        {
            InitializeResults();
            _activity = new PredecessorObj<Activity>(this, RoleActivity, activity);
            _prior = new PredecessorList<Activity__multiplier>(this, RolePrior, prior);
            _value = value;
        }

        // Hydration constructor
        private Activity__multiplier(FactMemento memento)
        {
            InitializeResults();
            _activity = new PredecessorObj<Activity>(this, RoleActivity, memento);
            _prior = new PredecessorList<Activity__multiplier>(this, RolePrior, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Activity Activity
        {
            get { return _activity.Fact; }
        }
        public IEnumerable<Activity__multiplier> Prior
        {
            get { return _prior; }
        }
     
        // Field access
        public int Value
        {
            get { return _value; }
        }

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
				Individual.QueryProfiles.QueryDefinition);
			community.AddType(
				IndividualProfile._correspondenceFactType,
				new IndividualProfile.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { IndividualProfile._correspondenceFactType }));
			community.AddType(
				Profile._correspondenceFactType,
				new Profile.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Profile._correspondenceFactType }));
			community.AddQuery(
				Profile._correspondenceFactType,
				Profile.QueryName.QueryDefinition);
			community.AddQuery(
				Profile._correspondenceFactType,
				Profile.QueryActivities.QueryDefinition);
			community.AddType(
				Profile__name._correspondenceFactType,
				new Profile__name.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Profile__name._correspondenceFactType }));
			community.AddQuery(
				Profile__name._correspondenceFactType,
				Profile__name.QueryIsCurrent.QueryDefinition);
			community.AddType(
				Company._correspondenceFactType,
				new Company.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Company._correspondenceFactType }));
			community.AddQuery(
				Company._correspondenceFactType,
				Company.QueryCategories.QueryDefinition);
			community.AddType(
				Quarter._correspondenceFactType,
				new Quarter.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Quarter._correspondenceFactType }));
			community.AddQuery(
				Quarter._correspondenceFactType,
				Quarter.QueryRewards.QueryDefinition);
			community.AddType(
				Category._correspondenceFactType,
				new Category.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Category._correspondenceFactType }));
			community.AddQuery(
				Category._correspondenceFactType,
				Category.QueryDescription.QueryDefinition);
			community.AddQuery(
				Category._correspondenceFactType,
				Category.QueryOrdinal.QueryDefinition);
			community.AddQuery(
				Category._correspondenceFactType,
				Category.QueryActivities.QueryDefinition);
			community.AddType(
				Category__description._correspondenceFactType,
				new Category__description.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Category__description._correspondenceFactType }));
			community.AddQuery(
				Category__description._correspondenceFactType,
				Category__description.QueryIsCurrent.QueryDefinition);
			community.AddType(
				Category__ordinal._correspondenceFactType,
				new Category__ordinal.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Category__ordinal._correspondenceFactType }));
			community.AddQuery(
				Category__ordinal._correspondenceFactType,
				Category__ordinal.QueryIsCurrent.QueryDefinition);
			community.AddType(
				ActivityDefinition._correspondenceFactType,
				new ActivityDefinition.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { ActivityDefinition._correspondenceFactType }));
			community.AddQuery(
				ActivityDefinition._correspondenceFactType,
				ActivityDefinition.QueryDescription.QueryDefinition);
			community.AddQuery(
				ActivityDefinition._correspondenceFactType,
				ActivityDefinition.QueryQualifier.QueryDefinition);
			community.AddQuery(
				ActivityDefinition._correspondenceFactType,
				ActivityDefinition.QueryOrdinal.QueryDefinition);
			community.AddType(
				ActivityDefinition__description._correspondenceFactType,
				new ActivityDefinition__description.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { ActivityDefinition__description._correspondenceFactType }));
			community.AddQuery(
				ActivityDefinition__description._correspondenceFactType,
				ActivityDefinition__description.QueryIsCurrent.QueryDefinition);
			community.AddType(
				ActivityDefinition__qualifier._correspondenceFactType,
				new ActivityDefinition__qualifier.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { ActivityDefinition__qualifier._correspondenceFactType }));
			community.AddQuery(
				ActivityDefinition__qualifier._correspondenceFactType,
				ActivityDefinition__qualifier.QueryIsCurrent.QueryDefinition);
			community.AddType(
				ActivityDefinition__ordinal._correspondenceFactType,
				new ActivityDefinition__ordinal.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { ActivityDefinition__ordinal._correspondenceFactType }));
			community.AddQuery(
				ActivityDefinition__ordinal._correspondenceFactType,
				ActivityDefinition__ordinal.QueryIsCurrent.QueryDefinition);
			community.AddType(
				ActivityReward._correspondenceFactType,
				new ActivityReward.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { ActivityReward._correspondenceFactType }));
			community.AddQuery(
				ActivityReward._correspondenceFactType,
				ActivityReward.QueryPoints.QueryDefinition);
			community.AddType(
				ActivityReward__points._correspondenceFactType,
				new ActivityReward__points.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { ActivityReward__points._correspondenceFactType }));
			community.AddQuery(
				ActivityReward__points._correspondenceFactType,
				ActivityReward__points.QueryIsCurrent.QueryDefinition);
			community.AddType(
				ProfileQuarter._correspondenceFactType,
				new ProfileQuarter.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { ProfileQuarter._correspondenceFactType }));
			community.AddType(
				Activity._correspondenceFactType,
				new Activity.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Activity._correspondenceFactType }));
			community.AddQuery(
				Activity._correspondenceFactType,
				Activity.QueryDescription.QueryDefinition);
			community.AddQuery(
				Activity._correspondenceFactType,
				Activity.QueryMultiplier.QueryDefinition);
			community.AddType(
				Activity__description._correspondenceFactType,
				new Activity__description.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Activity__description._correspondenceFactType }));
			community.AddQuery(
				Activity__description._correspondenceFactType,
				Activity__description.QueryIsCurrent.QueryDefinition);
			community.AddType(
				Activity__multiplier._correspondenceFactType,
				new Activity__multiplier.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Activity__multiplier._correspondenceFactType }));
			community.AddQuery(
				Activity__multiplier._correspondenceFactType,
				Activity__multiplier.QueryIsCurrent.QueryDefinition);
		}
	}
}
