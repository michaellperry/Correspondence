using System;
using System.Data;

namespace UpdateControls.Correspondence.SSCE
{
    internal class UnitOfWork : IDisposable
    {
        [ThreadStatic]
        private static UnitOfWork _currentUnitOfWork;

        private string _connectionString;
        private IDbConnection _connection;
        private UnitOfWork _prior;

        public UnitOfWork(UnitOfWork prior)
        {
            _prior = prior;
        }

        public IDbConnection GetConnection(string connectionString)
        {
            // Trickle down to the bottom of the stack.
            if (_prior != null)
                return _prior.GetConnection(connectionString);

            // Create a connection on the first attempt.
            if (_connection == null)
            {
                _connectionString = connectionString;
                _connection = new System.Data.SqlServerCe.SqlCeConnection(_connectionString);
                _connection.Open();
            }

            // If the connection string is the same, reuse the connection.
            return _connectionString == connectionString
                ? (IDbConnection)_connection
                : null;
        }

        public void Dispose()
        {
            // Close the connection, if it was creted.
            if (_connection != null)
                _connection.Close();

            // Pop myself off the stack.
            _currentUnitOfWork = _currentUnitOfWork._prior;
        }

        public static IDbConnection Enlist(string connectionString)
        {
            // If a unit of work was begun, use it.
            return _currentUnitOfWork != null
                ? (IDbConnection)_currentUnitOfWork.GetConnection(connectionString)
                : null;
        }

        public static IDisposable Begin()
        {
            // Push a new unit of work to the stack.
            _currentUnitOfWork = new UnitOfWork(_currentUnitOfWork);
            return _currentUnitOfWork;
        }
    }
}
