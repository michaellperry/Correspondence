using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Correspondence.Mementos;
using Correspondence.Networking;
using Correspondence.Queries;
using Correspondence.Strategy;
using Correspondence.Debugging;
using System.ComponentModel;
using Correspondence.WorkQueues;
using Correspondence.Threading;
using Correspondence.Service;

namespace Correspondence
{
    /// <summary>
	/// </summary>
	public partial class Community : ICommunity
	{
        private IWorkQueue _userWorkQueue;
        private IWorkQueue _storageWorkQueue;
        private IWorkQueue _outgoingNetworkWorkQueue;
        private IWorkQueue _incomingNetworkWorkQueue;
        private Model _model;
        private Network _network;
        private IDictionary<Type, IFieldSerializer> _fieldSerializerByType = new Dictionary<Type, IFieldSerializer>();
        private List<Process> _services = new List<Process>();

		public Community(IStorageStrategy storageStrategy)
        {
            if (storageStrategy.IsSynchronous)
            {
                _userWorkQueue = new SynchronousWorkQueue();
                _storageWorkQueue = new SynchronousWorkQueue();
            }
            else
            {
                _userWorkQueue = new AsynchronousWorkQueue();
                _storageWorkQueue = new AsynchronousWorkQueue();
            }
            _outgoingNetworkWorkQueue = new AsynchronousWorkQueue();
            _incomingNetworkWorkQueue = new AsynchronousWorkQueue();

            _model = new Model(this, storageStrategy, _storageWorkQueue);
            _network = new Network(_model, storageStrategy, _storageWorkQueue, _outgoingNetworkWorkQueue, _incomingNetworkWorkQueue);

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
                    _storageWorkQueue.LastException ??
                    _userWorkQueue.LastException;
            }
        }

        public void Perform(Func<Task> asyncDelegate)
        {
            _userWorkQueue.Perform(asyncDelegate);
        }

        public async Task QuiesceAsync()
        {
            await Task.WhenAll(
                _storageWorkQueue.JoinAsync(),
                _outgoingNetworkWorkQueue.JoinAsync(),
                _incomingNetworkWorkQueue.JoinAsync(),
                _userWorkQueue.JoinAsync());
        }

        public void Notify(CorrespondenceFact pivot, string text1, string text2)
        {
            _network.Notify(pivot, text1, text2);
        }

        public Community AddService<TFact>(
            Func<IEnumerable<TFact>> queue,
            Func<TFact, Task> asyncAction)
            where TFact: CorrespondenceFact
        {
            _services.Add(new FactService<TFact>(
                () => (!Synchronizing && LastException == null)
                    ? queue().ToList()
                    : null,
                asyncAction));
            return this;
        }

        internal IDictionary<Type, IFieldSerializer> FieldSerializerByType
        {
            get { return _fieldSerializerByType; }
        }

		internal async Task<CorrespondenceFact> GetFactByIDAsync( FactID id )
		{
            return await _model.GetFactByIDAsync(id);
		}

        internal void ExecuteQuery(
            FactID startingId,
            QueryDefinition queryDefinition,
            QueryOptions options,
            Action<List<CorrespondenceFact>> doneLoading)
        {
            _storageWorkQueue.Perform(async delegate
            {
                var facts = await _model.ExecuteQueryAsync(
                    queryDefinition, startingId, options);

                doneLoading(facts);
            });
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
