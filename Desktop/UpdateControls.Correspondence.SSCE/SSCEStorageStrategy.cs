using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;
using System.IO;
using System.Reflection;
using UpdateControls.Correspondence.Tasks;

namespace UpdateControls.Correspondence.SSCE
{
    public class SSCEStorageStrategy : IStorageStrategy
    {
        private const string PRE_HEAD_SELECT =
            "SELECT ff.FactID, ff.Data, t.TypeID, t.TypeName, t.Version, r.RoleID, dt.TypeID, dt.TypeName, dt.Version, r.RoleName, p.FKPredecessorFactID, p.IsPivot " +
            "\r\nFROM (SELECT ";
        private const string POST_HEAD_SELECT =
            "f.FactID, f.Data, f.FKTypeID FROM Fact f ";
        private const string HEAD_SELECT = PRE_HEAD_SELECT + POST_HEAD_SELECT;
        private const string TAIL_JOIN =
            "\r\n) ff " +
            "\r\nJOIN Type t ON ff.FKTypeID = t.TypeID " +
            "\r\nLEFT JOIN Predecessor p ON ff.FactID = p.FKFactID " +
            "\r\nLEFT JOIN Role r ON p.FKRoleID = r.RoleID " +
            "\r\nLEFT JOIN Type dt ON r.DeclaringTypeID = dt.TypeID ";

        private string _connectionString;

        // Cache role IDs and type IDs.
        private IDictionary<RoleMemento, int> _roleIdByMemento = new Dictionary<RoleMemento, int>();
        private IDictionary<CorrespondenceFactType, int> _typeIdByMemento = new Dictionary<CorrespondenceFactType, int>();
        private IDictionary<int, RoleMemento> _roleMementoById = new Dictionary<int, RoleMemento>();
        private IDictionary<int, CorrespondenceFactType> _typeMementoById = new Dictionary<int, CorrespondenceFactType>();
        private IDictionary<string, int> _protocolIdByName = new Dictionary<string, int>();

        /// <summary>
        /// Initialize the SQL Server Compact Edition storage strategy. Provide the location
        /// of your Correspondence.sdf file. The file will be created if it does not exist.
        /// </summary>
        /// <param name="connectionString">The full path to Correspondence.sdf</param>
        public SSCEStorageStrategy(string correspondenceDatabaseFileName)
		{
            CreateDatabase(correspondenceDatabaseFileName);
            _connectionString = string.Format(@"Data Source =""{0}""", correspondenceDatabaseFileName);
        }

        private void CreateDatabase(string databaseFilename)
        {
            if (!File.Exists(databaseFilename))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(databaseFilename));
                using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    typeof(SSCEStorageStrategy), "Correspondence.sdf"))
                {
                    using (Stream fileStream = File.Create(databaseFilename))
                    {
                        byte[] buffer = new byte[1024];
                        while (true)
                        {
                            int read = resourceStream.Read(buffer, 0, buffer.Length);
                            if (read <= 0)
                                break;
                            fileStream.Write(buffer, 0, read);
                        }
                    }
                }
            }
        }

		private static void AddParameter(IDbCommand command, string name, object value)
		{
			System.Data.SqlServerCe.SqlCeParameter param = new System.Data.SqlServerCe.SqlCeParameter();
			param.ParameterName = name;
			param.Value = value;
			command.Parameters.Add(param);
		}

		private static void ReadBinary(IDataReader reader, FactMemento memento, int columnIndex)
		{
			byte[] buffer = new byte[1024];
			int length = (int)reader.GetBytes(columnIndex, 0, buffer, 0, buffer.Length);
			memento.Data = new byte[length];
			Array.Copy(buffer, memento.Data, length);
		}

        public IDisposable BeginDuration()
        {
            return Duration.Begin();
        }

		public Guid ClientGuid
		{
            get
            {
                using (var session = new Session(_connectionString))
                {
                    session.Command.CommandText = "SELECT ClientGUID FROM Client";
                    object result = session.Command.ExecuteScalar();
                    if (result == null)
                    {
                        Guid clientGuid = Guid.NewGuid();
                        session.Command.CommandText = "INSERT INTO Client (ClientGUID) VALUES (@ClientGUID)";
                        AddParameter(session.Command, "@ClientGUID", clientGuid);
                        session.Command.ExecuteNonQuery();
                        session.Command.Parameters.Clear();
                        return clientGuid;
                    }
                    else
                    {
                        return (Guid)result;
                    }
                }
            }
		}

        public bool GetID(string factName, out FactID id)
        {
			id = new FactID();

			using (var session = new Session(_connectionString))
			{
				session.Command.CommandText = "SELECT FKFactID FROM NamedFact WHERE [Name] = @Name";
				AddParameter(session.Command, "@Name", factName);
				object result = session.Command.ExecuteScalar();
				session.Command.Parameters.Clear();
				if (result == null)
				{
					return false;
				}
				else
				{
					id.key = (System.Int64)result;
					return true;
				}
			}
		}

        public void SetID(string factName, FactID id)
        {
			using (var session = new Session(_connectionString))
			{
				// First try an update.
				session.Command.CommandText = "UPDATE NamedFact SET FKFactID=@FactID WHERE [Name] = @Name";
				AddParameter(session.Command, "@Name", factName);
				AddParameter(session.Command, "@FactId", id.key);
				int count = session.Command.ExecuteNonQuery();
				session.Command.Parameters.Clear();
				if (count > 0)
					return;

				// No rows affected, so it's safe to insert.
				session.Command.CommandText = "INSERT INTO NamedFact ([Name], FKFactID) VALUES (@Name, @FactID)";
                AddParameter(session.Command, "@Name", factName);
                AddParameter(session.Command, "@FactId", id.key);
                session.Command.ExecuteNonQuery();
				session.Command.Parameters.Clear();
			}
		}

		public FactMemento Load(FactID id)
		{
			IdentifiedFactMemento identifiedMemento = null;

			using (var session = new Session(_connectionString))
			{
				// Get the fact.
                session.Command.CommandText = HEAD_SELECT +
                    "WHERE f.FactID = @FactID " +
                    TAIL_JOIN +
                    "ORDER BY p.PredecessorID";
				AddParameter(session.Command, "@FactID", id.key);
				using (IDataReader reader = session.Command.ExecuteReader())
				{
					session.Command.Parameters.Clear();
					identifiedMemento = LoadMementosFromReader(reader).FirstOrDefault();
					if (identifiedMemento == null)
						throw new CorrespondenceException(string.Format("Unable to find fact {0}", id.key));
				}
			}

			return identifiedMemento.Memento;
		}

		public bool Save(FactMemento memento, int peerId, out FactID id)
		{
			using (var session = new Session(_connectionString))
			{
				// First see if the fact is already in storage.
				if (FindExistingFact(memento, out id, session))
					return false;

				// It isn't there, so store it.
				int typeId = SaveType(session, memento.FactType);
				session.Command.CommandText = "INSERT Fact (FKTypeID, Data, Hashcode) VALUES (@TypeID, @Data, @Hashcode)";
				AddParameter(session.Command, "@TypeID", typeId);
				AddParameter(session.Command, "@Data", memento.Data);
				AddParameter(session.Command, "@Hashcode", memento.GetHashCode());
				session.Command.ExecuteNonQuery();
				session.Command.Parameters.Clear();

				session.Command.CommandText = "SELECT @@IDENTITY";
				decimal result = (decimal)session.Command.ExecuteScalar();
				session.Command.Parameters.Clear();
				id.key = (Int64)result;

				// Store the predecessors.
				foreach (PredecessorMemento predecessor in memento.Predecessors)
				{
					int roleId = SaveRole(session, predecessor.Role);
					session.Command.CommandText = "INSERT Predecessor (FKFactID, FKRoleID, FKPredecessorFactID, IsPivot) VALUES (@FactID, @RoleID, @PredecessorFactID, @IsPivot)";
					AddParameter(session.Command, "@FactID", id.key);
					AddParameter(session.Command, "@RoleID", roleId);
					AddParameter(session.Command, "@PredecessorFactID", predecessor.ID.key);
                    AddParameter(session.Command, "@IsPivot", predecessor.IsPivot);
                    session.Command.ExecuteNonQuery();
					session.Command.Parameters.Clear();
				}

				// Store a message for each pivot.
				FactID newFactId = id;
				List<MessageMemento> pivotMessages = memento.Predecessors
					.Where(predecessor => predecessor.IsPivot)
					.Select(predecessor => new MessageMemento(predecessor.ID, newFactId))
					.ToList();

				// Store messages for each non-pivot. This fact belongs to all predecessors' pivots.
				string[] nonPivots = memento.Predecessors
					.Where(predecessor => !predecessor.IsPivot)
					.Select(predecessor => predecessor.ID.key.ToString())
					.ToArray();
				List<MessageMemento> nonPivotMessages;
				if (nonPivots.Length > 0)
				{
					string nonPivotGroup = string.Join(",", nonPivots);
					session.Command.CommandText = string.Format(
						"SELECT DISTINCT PivotId FROM Message WHERE FactId IN ({0})",
						nonPivotGroup);
					List<FactID> predecessorsPivots;
					using (IDataReader predecessorPivotReader = session.Command.ExecuteReader())
					{
						session.Command.Parameters.Clear();
						predecessorsPivots = LoadIDsFromReader(predecessorPivotReader).ToList();
					}

					nonPivotMessages = predecessorsPivots
						.Select(predecessorPivot => new MessageMemento(predecessorPivot, newFactId))
						.ToList();
				}
				else
					nonPivotMessages = new List<MessageMemento>();

				SaveMessages(session, pivotMessages.Union(nonPivotMessages).Distinct());

				return true;
			}
		}

        public bool FindExistingFact(FactMemento memento, out FactID id)
        {
			using (var session = new Session(_connectionString))
			{
				// See if the fact is already in storage.
				return FindExistingFact(memento, out id, session);
			}
        }

        public IEnumerable<IdentifiedFactMemento> QueryForFacts(QueryDefinition queryDefinition, FactID id, QueryOptions options)
        {
            using (var session = new Session(_connectionString))
            {
                // Start with the fact query.
                StringBuilder queryString = new StringBuilder();
                if (options == null)
                    queryString.Append(HEAD_SELECT);
                else
                {
                    queryString
                        .Append(PRE_HEAD_SELECT)
                        .AppendFormat("DISTINCT TOP ({0}) ", options.Limit)
                        .Append(POST_HEAD_SELECT);
                }

                // Append the joins.
                List<int> roleIds = new List<int>();
                StringBuilder conditions = new StringBuilder();
                string factAlias = AppendJoins(session, queryDefinition, queryString, roleIds, conditions, 0, "f.FactID");

                // Add with the where clause.
                queryString.AppendFormat("\r\nWHERE {0}=@FactID ", factAlias);
                queryString.Append(conditions);

                // Order the sub select.
                if (options != null)
                    queryString.Append(" ORDER BY f.FactID DESC");

                // Pick up the final fact properties.
                queryString.Append(TAIL_JOIN);

                // Finish with the order by.
                queryString.Append("\r\nORDER BY ff.FactID DESC, p.PredecessorID");

                //System.Diagnostics.Debug.WriteLine(queryDefinition);
                //System.Diagnostics.Debug.WriteLine(queryString);
                //DateTime start = DateTime.Now;
                session.Command.CommandText = queryString.ToString();
                AddParameter(session.Command, "@FactID", id.key);
                for (int i = 0; i < roleIds.Count; i++)
                    AddParameter(session.Command, "@Role" + i, roleIds[i]);
                using (IDataReader factReader = session.Command.ExecuteReader())
                {
                    session.Command.Parameters.Clear();

                    IEnumerable<IdentifiedFactMemento> mementos = LoadMementosFromReader(factReader);
                    if (options != null)
                        mementos = mementos.Take(options.Limit);
                    List<IdentifiedFactMemento> result = mementos.ToList();
                    //DateTime stop = DateTime.Now;
                    //System.Diagnostics.Debug.WriteLine(stop - start);
                    return result;
                }
            }
        }

        internal string AppendJoins(Session session, QueryDefinition queryDefinition, StringBuilder queryString, List<int> roleIds, StringBuilder conditions, int depth, string priorAlias)
        {
            IEnumerable<Join> joins = queryDefinition.Joins.Reverse();
            return AppendJoins(session, joins, queryString, roleIds, conditions, depth, priorAlias);
        }

        internal string AppendJoins(Session session, IEnumerable<Join> joins, StringBuilder queryString, List<int> roleIds, StringBuilder conditions, int depth, string priorAlias)
        {
            // Append joins.
            foreach (Join join in joins)
            {
                int index = roleIds.Count;
                roleIds.Add(SaveRole(session, join.Role));
                string nextAlias = "p" + index;

                if (join.Condition != null)
                {
                    ConditionWriter writer = new ConditionWriter(this, session, conditions, roleIds, depth, priorAlias);
                    join.Condition.Accept(writer);
                }

                if (join.Successor)
                {
                    queryString.AppendFormat("\r\nJOIN Predecessor {1} ON {0}={1}.FKFactID AND {1}.FKRoleID=@Role{2} ", priorAlias, nextAlias, index);
                    priorAlias = nextAlias + ".FKPredecessorFactID";
                }
                else
                {
                    queryString.AppendFormat("\r\nJOIN Predecessor {1} ON {0}={1}.FKPredecessorFactID AND {1}.FKRoleID=@Role{2} ", priorAlias, nextAlias, index);
                    priorAlias = nextAlias + ".FKFactID";
                }
            }
            return priorAlias;
        }

        public IEnumerable<FactID> QueryForIds(QueryDefinition queryDefinition, FactID startingId)
        {
            using (var session = new Session(_connectionString))
            {
                // Start with the fact query.
                StringBuilder queryString = new StringBuilder();
                queryString.Append("SELECT f.FactID FROM Fact f ");

                // Append the joins.
                List<int> roleIds = new List<int>();
                StringBuilder conditions = new StringBuilder();
                string factAlias = AppendJoins(session, queryDefinition, queryString, roleIds, conditions, 0, "f.FactID");

                // Finish with the where clause.
                queryString.AppendFormat("\r\nWHERE {0}=@FactID ", factAlias);
                queryString.Append(conditions);

                //System.Diagnostics.Debug.WriteLine(queryDefinition);
                //System.Diagnostics.Debug.WriteLine(queryString);
                //DateTime start = DateTime.Now;
                session.Command.CommandText = queryString.ToString();
                AddParameter(session.Command, "@FactID", startingId.key);
                for (int i = 0; i < roleIds.Count; i++)
                    AddParameter(session.Command, "@Role" + i, roleIds[i]);
                using (IDataReader factReader = session.Command.ExecuteReader())
                {
                    session.Command.Parameters.Clear();

                    List<FactID> result = LoadIDsFromReader(factReader).ToList();
                    //DateTime stop = DateTime.Now;
                    //System.Diagnostics.Debug.WriteLine(stop - start);
                    return result;
                }
            }
        }

        public TimestampID LoadOutgoingTimestamp(int peerId)
        {
            return LoadTimestamp(peerId, 0);
        }

        public void SaveOutgoingTimestamp(int peerId, TimestampID timestamp)
        {
            SaveTimestamp(peerId, 0, timestamp);
        }

        public TimestampID LoadIncomingTimestamp(int peerId, FactID pivotId)
        {
            return LoadTimestamp(peerId, pivotId.key);
        }

        public void SaveIncomingTimestamp(int peerId, FactID pivotId, TimestampID timestamp)
        {
            SaveTimestamp(peerId, pivotId.key, timestamp);
        }

        public IEnumerable<MessageMemento> LoadRecentMessagesForServer(int peerId, TimestampID timestamp)
        {
            using (var session = new Session(_connectionString))
            {
                session.Command.CommandText = "SELECT TOP (20) PivotId, FactId FROM Message WHERE FactId > @Timestamp ORDER BY FactId";
                AddParameter(session.Command, "@Timestamp", timestamp.Key);
                using (IDataReader messageReader = session.Command.ExecuteReader())
                {
                    session.Command.Parameters.Clear();

                    return LoadMessagesFromReader(messageReader).ToList();
                }
            }
        }

        public IEnumerable<FactID> LoadRecentMessagesForClient(FactID pivotId, TimestampID timestamp)
        {
            using (var session = new Session(_connectionString))
            {
                session.Command.CommandText = "SELECT TOP (20) FactId FROM Message WHERE PivotId = @PivotId AND FactId > @Timestamp ORDER BY FactId";
                AddParameter(session.Command, "@PivotId", pivotId.key);
                AddParameter(session.Command, "@Timestamp", timestamp.Key);
                using (IDataReader messageReader = session.Command.ExecuteReader())
                {
                    session.Command.Parameters.Clear();

                    return LoadIDsFromReader(messageReader).ToList();
                }
            }
        }

        public void Unpublish(FactID factId, RoleMemento role)
        {
            throw new NotImplementedException();
        }

        public int SavePeer(string protocolName, string peerName)
        {
            using (var session = new Session(_connectionString))
            {
                // Save the protocol.
                int protocolId = SaveProtocol(session, protocolName);

                // See if the peer already exists.
                int peerId = 0;
                session.Command.CommandText = "SELECT PeerID FROM Peer WHERE PeerName = @PeerName AND FKProtocolID = @ProtocolID";
                AddParameter(session.Command, "@PeerName", peerName);
                AddParameter(session.Command, "@ProtocolID", protocolId);
                using (IDataReader peerReader = session.Command.ExecuteReader())
                {
                    session.Command.Parameters.Clear();
                    if (peerReader.Read())
                    {
                        peerId = peerReader.GetInt32(0);
                    }
                }

                // If not, create it.
                if (peerId == 0)
                {
                    session.Command.CommandText = "INSERT INTO Peer (PeerName, FKProtocolID) VALUES (@PeerName, @ProtocolID)";
                    AddParameter(session.Command, "@PeerName", peerName);
                    AddParameter(session.Command, "@ProtocolID", protocolId);
                    session.Command.ExecuteNonQuery();
                    session.Command.Parameters.Clear();

                    session.Command.CommandText = "SELECT @@IDENTITY";
                    peerId = (int)(decimal)session.Command.ExecuteScalar();
                    session.Command.Parameters.Clear();
                }

                return peerId;
            }
        }

        public IEnumerable<IdentifiedFactMemento> LoadAllFacts()
        {
            using (var session = new Session(_connectionString))
            {
                //System.Diagnostics.Debug.WriteLine(queryDefinition);
                //System.Diagnostics.Debug.WriteLine(queryString);
                //DateTime start = DateTime.Now;
                session.Command.CommandText = HEAD_SELECT +
                    TAIL_JOIN +
                    "\r\nORDER BY ff.FactID, p.PredecessorID";
                using (IDataReader factReader = session.Command.ExecuteReader())
                {
                    session.Command.Parameters.Clear();

                    List<IdentifiedFactMemento> result = LoadMementosFromReader(factReader).ToList();
                    //DateTime stop = DateTime.Now;
                    //System.Diagnostics.Debug.WriteLine(stop - start);
                    return result;
                }
            }
        }

        public IdentifiedFactMemento LoadNextFact(FactID? lastFactId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NamedFactMemento> LoadAllNamedFacts()
        {
            using (var session = new Session(_connectionString))
            {
                session.Command.CommandText = "SELECT Name, FKFactID FROM NamedFact";
                using (var reader = session.Command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        long factId = reader.GetInt64(1);
                        yield return new NamedFactMemento(name, new FactID() { key = factId });
                    }
                }
            }
        }

        public FactID GetFactIDFromShare(int peerId, FactID remoteFactId)
        {
            FactID id = new FactID();
            using (var session = new Session(_connectionString))
            {
                // Get the local fact ID from the share.
                session.Command.CommandText = "SELECT FKFactID FROM Share WHERE FKPeerID = @PeerID AND RemoteFactID = @RemoteFactID";
                AddParameter(session.Command, "@PeerID", peerId);
                AddParameter(session.Command, "@RemoteFactID", remoteFactId.key);
                object result = session.Command.ExecuteScalar();
                if (result != null)
                    id.key = (System.Int64)result;
            }
            return id;
        }

        public Task<List<IdentifiedFactMemento>> QueryForFactsAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            return Task<List<IdentifiedFactMemento>>.FromResult(QueryForFacts(queryDefinition, startingId, options).ToList());
        }

        public void SaveShare(int peerId, FactID remoteFactId, FactID localFactId)
        {
            using (var session = new Session(_connectionString))
            {
                // See if the share already exists.
                session.Command.CommandText = "SELECT * FROM Share WHERE FKPeerID = @PeerID AND FKFactID = @LocalFactID";
                AddParameter(session.Command, "@PeerID", peerId);
                AddParameter(session.Command, "@LocalFactID", localFactId.key);
                using (IDataReader shareReader = session.Command.ExecuteReader())
                {
                    session.Command.Parameters.Clear();
                    if (shareReader.Read())
                    {
                        // It does.
                        return;
                    }
                }

                // If not, create it.
                session.Command.CommandText = "INSERT INTO Share (FKPeerID, FKFactID, RemoteFactID) VALUES (@PeerID, @LocalFactID, @RemoteFactID)";
                AddParameter(session.Command, "@PeerID", peerId);
                AddParameter(session.Command, "@LocalFactID", localFactId.key);
                AddParameter(session.Command, "@RemoteFactID", remoteFactId.key);
                session.Command.ExecuteNonQuery();
                session.Command.Parameters.Clear();
            }
        }

        private int SaveType(Session session, CorrespondenceFactType typeMemento)
        {
            // Check the cache.
            int typeId = 0;
            if (_typeIdByMemento.TryGetValue(typeMemento, out typeId))
                return typeId;

            // See if the type already exists.
			typeId = 0;
			session.Command.CommandText = "SELECT TypeID FROM Type WHERE TypeName = @TypeName AND Version = @Version";
			AddParameter(session.Command, "@TypeName", typeMemento.TypeName);
			AddParameter(session.Command, "@Version", typeMemento.Version);
			using (IDataReader typeReader = session.Command.ExecuteReader())
			{
				session.Command.Parameters.Clear();
				if (typeReader.Read())
				{
					typeId = typeReader.GetInt32(0);
				}
			}

            // If not, create it.
			if (typeId == 0)
			{
				session.Command.CommandText = "INSERT INTO Type (TypeName, Version) VALUES (@TypeName, @Version)";
				AddParameter(session.Command, "@TypeName", typeMemento.TypeName);
				AddParameter(session.Command, "@Version", typeMemento.Version);
				session.Command.ExecuteNonQuery();
				session.Command.Parameters.Clear();

				session.Command.CommandText = "SELECT @@IDENTITY";
				typeId = (int)(decimal)session.Command.ExecuteScalar();
				session.Command.Parameters.Clear();
			}

			_typeIdByMemento.Add(typeMemento, typeId);
			_typeMementoById.Add(typeId, typeMemento);
			return typeId;
        }

        internal int SaveRole(Session session, RoleMemento roleMemento)
        {
            // See if it's in the cache.
            int roleId;
            if (_roleIdByMemento.TryGetValue(roleMemento, out roleId))
                return roleId;

            int declaringTypeId = SaveType(session, roleMemento.DeclaringType);

			// See if the role already exists.
			roleId = 0;
			session.Command.CommandText = "SELECT RoleID FROM Role WHERE RoleName = @RoleName AND DeclaringTypeID = @DeclaringTypeID";
			AddParameter(session.Command, "@RoleName", roleMemento.RoleName);
			AddParameter(session.Command, "@DeclaringTypeID", declaringTypeId);
			using (IDataReader typeReader = session.Command.ExecuteReader())
			{
				session.Command.Parameters.Clear();
				if (typeReader.Read())
				{
					roleId = typeReader.GetInt32(0);
				}
			}

			// If not, create it.
			if (roleId == 0)
			{
				session.Command.CommandText = "INSERT INTO Role (RoleName, DeclaringTypeID) VALUES (@RoleName, @DeclaringTypeID)";
				AddParameter(session.Command, "@RoleName", roleMemento.RoleName);
				AddParameter(session.Command, "@DeclaringTypeID", declaringTypeId);
				session.Command.ExecuteNonQuery();
				session.Command.Parameters.Clear();

				session.Command.CommandText = "SELECT @@IDENTITY";
				roleId = (int)(decimal)session.Command.ExecuteScalar();
				session.Command.Parameters.Clear();
			}

			_roleIdByMemento.Add(roleMemento, roleId);
			_roleMementoById.Add(roleId, roleMemento);
			return roleId;
        }

        private void SaveMessages(Session session, IEnumerable<MessageMemento> messages)
        {
            session.Command.CommandText = "INSERT INTO Message (FactId, PivotId) VALUES (@FactId, @PivotId)";
            foreach (MessageMemento message in messages)
            {
                AddParameter(session.Command, "@FactId", message.FactId.key);
                AddParameter(session.Command, "@PivotId", message.PivotId.key);
                session.Command.ExecuteNonQuery();
                session.Command.Parameters.Clear();
            }
        }

        private IEnumerable<IdentifiedFactMemento> LoadMementosFromReader(IDataReader factReader)
		{
			IdentifiedFactMemento current = null;

			while (factReader.Read())
			{
				// FactID, Data, TypeID, TypeName, Version, RoleID, DeclaringTypeID, DeclaringTypeName, DeclaringTypeVersion, RoleName, PredecessorFactID
				long factId = factReader.GetInt64(0);

				// Load the header.
				if (current == null || factId != current.Id.key)
				{
					if (current != null)
						yield return current;

					int typeId = factReader.GetInt32(2);
					string typeName = factReader.GetString(3);
					int typeVersion = factReader.GetInt32(4);

					// Create the memento.
					current = new IdentifiedFactMemento(
						new FactID() { key = factId },
						new FactMemento(GetTypeMemento(typeId, typeName, typeVersion)));
					ReadBinary(factReader, current.Memento, 1);
				}

				// Load a predecessor.
				if (!factReader.IsDBNull(5))
				{
					int roleId = factReader.GetInt32(5);
					int declaringTypeId = factReader.GetInt32(6);
					string declaringTypeName = factReader.GetString(7);
					int declaringTypeVersion = factReader.GetInt32(8);
					string roleName = factReader.GetString(9);
					long predecessorFactId = factReader.GetInt64(10);
                    bool isPivot = factReader.GetBoolean(11);

					current.Memento.AddPredecessor(
						GetRoleMemento(roleId, declaringTypeId, declaringTypeName, declaringTypeVersion, roleName),
						new FactID() { key = predecessorFactId },
                        isPivot);
				}
			}

			if (current != null)
				yield return current;
		}

        private IEnumerable<FactID> LoadIDsFromReader(IDataReader factReader)
        {
            while (factReader.Read())
                yield return new FactID() { key = factReader.GetInt64(0) };
        }

        private static IEnumerable<MessageMemento> LoadMessagesFromReader(IDataReader messageReader)
        {
            while (messageReader.Read())
            {
                long pivotId = messageReader.GetInt64(0);
                long factId = messageReader.GetInt64(1);

                yield return new MessageMemento(
                    new FactID() { key = pivotId },
                    new FactID() { key = factId });
            }
        }

        private RoleMemento GetRoleMemento(int roleId, int declaringTypeId, string declaringTypeTypeName, int declaringTypeVersion, string roleName)
		{
			// Look in the cache for the role.
			RoleMemento roleMemento;
			if (!_roleMementoById.TryGetValue(roleId, out roleMemento))
			{
				// Not there, so create it.

				// Look in the cache for the declaring type.
				CorrespondenceFactType type;
				if (!_typeMementoById.TryGetValue(declaringTypeId, out type))
				{
					// Not there, so create it.
					type = new CorrespondenceFactType(declaringTypeTypeName, declaringTypeVersion);
					_typeIdByMemento.Add(type, declaringTypeId);
					_typeMementoById.Add(declaringTypeId, type);
				}

				roleMemento = new RoleMemento(type, roleName, null, false);
				_roleMementoById.Add(roleId, roleMemento);
				_roleIdByMemento.Add(roleMemento, roleId);
			}

			return roleMemento;
		}

		private CorrespondenceFactType GetTypeMemento(int typeId, string typeName, int typeVersion)
		{
			// Look in the cache first.
			CorrespondenceFactType type;
			if (!_typeMementoById.TryGetValue(typeId, out type))
			{
				// Not there, so create it.
				type = new CorrespondenceFactType(typeName, typeVersion);
				_typeIdByMemento.Add(type, typeId);
				_typeMementoById.Add(typeId, type);
			}
			return type;
		}

		private bool FindExistingFact(FactMemento memento, out FactID id, Session session)
		{
			int typeId = SaveType(session, memento.FactType);

			// Load all candidates that have the same hash code.
            session.Command.CommandText = HEAD_SELECT +
                "WHERE f.FKTypeID = @TypeID AND f.Hashcode = @Hashcode " +
                TAIL_JOIN +
                "ORDER BY ff.FactID, p.PredecessorID";
			AddParameter(session.Command, "@TypeID", typeId);
			AddParameter(session.Command, "@Hashcode", memento.GetHashCode());
            using (IDataReader factReader = session.Command.ExecuteReader())
            {
                session.Command.Parameters.Clear();

                List<IdentifiedFactMemento> existingFact = LoadMementosFromReader(factReader).Where(im => im.Memento.Equals(memento)).ToList();
                if (existingFact.Count > 1)
                    throw new CorrespondenceException(string.Format("More than one fact matched the given {0}.", memento.FactType));
                if (existingFact.Count == 1)
                {
                    id = existingFact[0].Id;
                    return true;
                }
                else
                {
                    id = new FactID();
                    return false;
                }
            }
		}

        private int SaveProtocol(Session session, string protocolName)
        {
            // Check the cache.
            int protocolId = 0;
            if (_protocolIdByName.TryGetValue(protocolName, out protocolId))
                return protocolId;

            // See if the protocol already exists.
			protocolId = 0;
			session.Command.CommandText = "SELECT ProtocolID FROM Protocol WHERE ProtocolName = @ProtocolName";
            AddParameter(session.Command, "@ProtocolName", protocolName);
			using (IDataReader typeReader = session.Command.ExecuteReader())
			{
				session.Command.Parameters.Clear();
				if (typeReader.Read())
				{
					protocolId = typeReader.GetInt32(0);
				}
			}

            // If not, create it.
			if (protocolId == 0)
			{
				session.Command.CommandText = "INSERT INTO Protocol (ProtocolName) VALUES (@ProtocolName)";
                AddParameter(session.Command, "@ProtocolName", protocolName);
				session.Command.ExecuteNonQuery();
				session.Command.Parameters.Clear();

				session.Command.CommandText = "SELECT @@IDENTITY";
				protocolId = (int)(decimal)session.Command.ExecuteScalar();
				session.Command.Parameters.Clear();
			}

            _protocolIdByName.Add(protocolName, protocolId);
			return protocolId;
        }

        private TimestampID LoadTimestamp(int peerId, long pivotId)
        {
            using (var session = new Session(_connectionString))
            {
                session.Command.CommandText = "SELECT DatabaseId, FactId FROM Timestamp WHERE FKPeerId=@PeerId AND PivotId=@PivotId";
                AddParameter(session.Command, "@PeerId", peerId);
                AddParameter(session.Command, "@PivotId", pivotId);
                using (IDataReader timestampReader = session.Command.ExecuteReader())
                {
                    session.Command.Parameters.Clear();

                    if (timestampReader.Read())
                    {
                        long databaseId = timestampReader.GetInt64(0);
                        long factId = timestampReader.GetInt64(1);
                        return new TimestampID(databaseId, factId);
                    }
                    else
                        return new TimestampID();
                }
            }
        }

        private void SaveTimestamp(int peerId, long pivotId, TimestampID timestamp)
        {
            using (var session = new Session(_connectionString))
            {
                // First try an update.
                session.Command.CommandText = "UPDATE Timestamp SET DatabaseId=@DatabaseId, FactId=@FactId WHERE FKPeerId=@PeerId AND PivotId=@PivotId";
                AddParameter(session.Command, "@PeerId", peerId);
                AddParameter(session.Command, "@DatabaseId", timestamp.DatabaseId);
                AddParameter(session.Command, "@FactId", timestamp.Key);
                AddParameter(session.Command, "@PivotId", pivotId);
                int count = session.Command.ExecuteNonQuery();
                session.Command.Parameters.Clear();
                if (count > 0)
                    return;

                // No rows affected, so it's safe to insert.
                session.Command.CommandText = "INSERT INTO Timestamp (FKPeerId, PivotId, DatabaseId, FactId) VALUES (@PeerId, @PivotId, @DatabaseId, @FactId)";
                AddParameter(session.Command, "@PeerId", peerId);
                AddParameter(session.Command, "@DatabaseId", timestamp.DatabaseId);
                AddParameter(session.Command, "@FactId", timestamp.Key);
                AddParameter(session.Command, "@PivotId", pivotId);
                session.Command.ExecuteNonQuery();
                session.Command.Parameters.Clear();
            }
        }


        public bool GetRemoteId(FactID localFactId, int peerId, out FactID remoteFactId)
        {
            using (var session = new Session(_connectionString))
            {
                session.Command.CommandText = "SELECT RemoteFactID FROM Share WHERE FKFactID=@LocalFactID AND FKPeerID=@PeerID";
                AddParameter(session.Command, "@LocalFactID", localFactId.key);
                AddParameter(session.Command, "@PeerId", peerId);
                object result = session.Command.ExecuteScalar();
                session.Command.Parameters.Clear();
                if (result == null)
                {
                    remoteFactId = new FactID();
                    return false;
                }
                else
                {
                    remoteFactId = new FactID { key = (long)result };
                    return true;
                }
            }
        }
    }
}
