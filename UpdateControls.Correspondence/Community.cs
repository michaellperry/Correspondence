using System;
using System.Collections.Generic;
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
        private IDictionary<Type, IFieldSerializer> _fieldSerializerByType = new Dictionary<Type, IFieldSerializer>();

        public Community(IStorageStrategy storageStrategy, ITypeStrategy typeStrategy)
        {
            _model = new Model(this, storageStrategy, typeStrategy);

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
            communicationStrategy.AttachMessageRepository(_model);
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

        public IDisposable BeginUnitOfWork()
        {
            return _model.BeginUnitOfWork();
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
