using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Networking;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Debugging;
using System.ComponentModel;
using UpdateControls.Correspondence.WorkQueues;

namespace UpdateControls.Correspondence
{
    /// <summary>
	/// </summary>
	public partial class Community : ICommunity
	{
        private IWorkQueue _storageWorkQueue;
        private IWorkQueue _asyncNetworkWorkQueue;
        private Model _model;
        private Network _network;
        private IDictionary<Type, IFieldSerializer> _fieldSerializerByType = new Dictionary<Type, IFieldSerializer>();

		public Community(IStorageStrategy storageStrategy)
        {
            if (storageStrategy.IsSynchronous)
                _storageWorkQueue = new SynchronousWorkQueue();
            else
                _storageWorkQueue = new AsynchronousWorkQueue();
            _asyncNetworkWorkQueue = new AsynchronousWorkQueue();

            _model = new Model(this, storageStrategy, _storageWorkQueue);
            _network = new Network(_model, storageStrategy, _storageWorkQueue, _asyncNetworkWorkQueue);

            // Register the default types.
			RegisterDefaultTypes();
		}

		partial void RegisterDefaultTypes();

        public bool ClientApp
        {
            get { return _model.ClientApp; }
            set { _model.ClientApp = value; }
        }

        public Community AddCommunicationStrategy(ICommunicationStrategy communicationStrategy)
        {
            _network.AddCommunicationStrategy(communicationStrategy);
            return this;
        }

        public Community AddAsynchronousCommunicationStrategy(IAsynchronousCommunicationStrategy asynchronousCommunicationStrategy)
        {
            _network.AddAsynchronousCommunicationStrategy(asynchronousCommunicationStrategy);
            return this;
        }

        public Community AddFieldSerializer<T>(IFieldSerializer fieldSerializer)
        {
            _fieldSerializerByType.Add(typeof(T), fieldSerializer);
            return this;
        }

        public Community AddType(CorrespondenceFactType type, ICorrespondenceFactFactory factory, FactMetadata factMetadata)
        {
            _model.AddType(type, factory, factMetadata);
            return this;
        }

        public Community AddQuery(CorrespondenceFactType type, QueryDefinition query)
        {
            _model.AddQuery(type, query);
            return this;
        }

        public Community AddUnpublisher(Role role, Condition publishCondition)
        {
            _model.AddUnpublisher(role.RoleMemento, publishCondition);
            return this;
        }

        public Community Subscribe<T>(Func<IEnumerable<T>> pivots)
            where T : CorrespondenceFact
        {
            _network.Subscribe(new Subscription(this, () => pivots().OfType<CorrespondenceFact>()));
            return this;
        }

        public Community Subscribe<T>(Func<Result<T>> pivots)
            where T : CorrespondenceFact
        {
            _network.Subscribe(new Subscription(this, () => pivots().OfType<CorrespondenceFact>()));
            return this;
        }

        public Community Subscribe<T>(Func<T> pivot)
            where T : CorrespondenceFact
        {
            _network.Subscribe(new Subscription(this, () => Enumerable.Repeat(pivot(), 1).OfType<CorrespondenceFact>()));
            return this;
        }

		public Community Register<T>()
			where T : ICorrespondenceModel, new()
		{
			ICorrespondenceModel model = new T();
			model.RegisterAllFactTypes(this, _fieldSerializerByType);
			return this;
		}

        public async Task<T> AddFactAsync<T>(T prototype)
            where T : CorrespondenceFact
        {
            return await _model.AddFactAsync<T>(prototype);
        }

        public T FindFact<T>(T prototype)
            where T : CorrespondenceFact
        {
            return _model.FindFact<T>(prototype);
        }

        public async Task<T> LoadFactAsync<T>(string factName) where T : CorrespondenceFact
        {
            CorrespondenceFact fact = await _model.LoadFactAsync(factName);
            if (fact != null && fact is T)
                return (T)fact;
            return null;
        }

		public async Task SetFactAsync( string objectName, CorrespondenceFact fact )
		{
            await _model.SetFactAsync(objectName, fact);
		}

        public event Action<CorrespondenceFact> FactAdded
        {
            add { _model.FactAdded += value; }
            remove { _model.FactAdded -= value; }
        }

        public event Action FactReceived
        {
            add { _model.FactReceived += value; }
            remove { _model.FactReceived -= value; }
        }

        public async Task<bool> SynchronizeAsync()
        {
            return await _network.SynchronizeAsync();
        }

        public void BeginReceiving()
        {
            _network.BeginReceiving();
        }

        public void BeginSending()
        {
            _network.BeginSending();
        }

        public bool Synchronizing
        {
            get { return _network.Synchronizing; }
        }

        public Exception LastException
        {
            get
            {
                return
                    _network.LastException ??
                    _model.LastException ??
                    _storageWorkQueue.LastException;
            }
        }

        public void Perform(Func<Task> asyncDelegate)
        {
            _storageWorkQueue.Perform(asyncDelegate);
        }

        public async Task<bool> QuiesceAsync()
        {
            var tasks = _storageWorkQueue.Tasks
                .Union(_asyncNetworkWorkQueue.Tasks)
                .ToArray();
            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
                return true;
            }

            return false;
        }

        public void Notify(CorrespondenceFact pivot, string text1, string text2)
        {
            _network.Notify(pivot, text1, text2);
        }

        internal IDictionary<Type, IFieldSerializer> FieldSerializerByType
        {
            get { return _fieldSerializerByType; }
        }

		internal async Task<CorrespondenceFact> GetFactByIDAsync( FactID id )
		{
            return await _model.GetFactByIDAsync(id);
		}

        internal async Task<List<CorrespondenceFact>> ExecuteQueryAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            return await _model.ExecuteQueryAsync(queryDefinition, startingId, options);
        }

        [Obsolete("This property is only for debugging. Do not code against it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TypeDescriptor[] BrowseTheDataStore
        {
            get { return _model.TypeDescriptors; }
        }

        internal FactDescriptor BrowseTheDataStoreFromThisPoint(CorrespondenceFact fact)
        {
            return _model.LoadFactDescriptorById(fact);
        }
    }
}
