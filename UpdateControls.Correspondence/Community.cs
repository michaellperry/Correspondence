using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.FieldSerializer;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence
{
    /// <summary>
	/// </summary>
	public class Community
	{
        private Model _model;
        private Network _network;
        private IDictionary<Type, IFieldSerializer> _fieldSerializerByType = new Dictionary<Type, IFieldSerializer>();

        public Community(IStorageStrategy storageStrategy, ITypeStrategy typeStrategy)
        {
            _model = new Model(this, storageStrategy, typeStrategy);
            _network = new Network(_model, storageStrategy);

            // Register the default types.
            this
                .AddFieldSerializer<string>(new StringFieldSerializer())
                .AddFieldSerializer<decimal>(new DecimalFieldSerializer())
				.AddFieldSerializer<DateTime>(new DateTimeFieldSerializer())
                .AddFieldSerializer<Guid>(new GuidFieldSerializer())
                .AddFieldSerializer<int>(new IntFieldSerializer());
        }

        public Community(IStorageStrategy storageStrategy)
            : this(storageStrategy, new AttributeTypeStrategy())
        {
        }

        public Community AddCommunicationStrategy(ICommunicationStrategy communicationStrategy)
        {
            _network.AddCommunicationStrategy(communicationStrategy);
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

        public Community Subscribe<T>(Func<IEnumerable<T>> pivots)
            where T : CorrespondenceFact
        {
            _network.Subscribe(new Subscription(() => pivots().OfType<CorrespondenceFact>()));
            return this;
        }

        public Community Subscribe<T>(Func<T> pivot)
            where T : CorrespondenceFact
        {
            _network.Subscribe(new Subscription(() => Enumerable.Repeat(pivot(), 1).OfType<CorrespondenceFact>()));
            return this;
        }

        public IDisposable BeginDuration()
        {
            return _model.BeginDuration();
        }

		public T AddFact<T>( T prototype ) where T : CorrespondenceFact
		{
            return _model.AddFact<T>(prototype);
		}

        public T FindFact<T>(T prototype) where T : CorrespondenceFact
        {
            return _model.FindFact<T>(prototype);
        }

        public T LoadFact<T>(string factName) where T : CorrespondenceFact
        {
            CorrespondenceFact fact = _model.LoadFact(factName);
            if (fact != null && fact is T)
                return (T)fact;
            return null;
        }

		public void SetFact( string objectName, CorrespondenceFact fact )
		{
            _model.SetFact(objectName, fact);
		}

        public event Action FactAdded
        {
            add { _model.FactAdded += value; }
            remove { _model.FactAdded -= value; }
        }

        public bool Synchronize()
        {
            return _network.Synchronize();
        }

        internal IDictionary<Type, IFieldSerializer> FieldSerializerByType
        {
            get { return _fieldSerializerByType; }
        }

		internal CorrespondenceFact GetFactByID( FactID id )
		{
            return _model.GetFactByID(id);
		}

        internal IEnumerable<CorrespondenceFact> ExecuteQuery(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            return _model.ExecuteQuery(queryDefinition, startingId, options);
        }
    }
}
