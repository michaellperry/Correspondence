using System;
using System.Data;

namespace UpdateControls.Correspondence.SSCE
{
    internal class Duration : IDisposable
    {
        [ThreadStatic]
        private static Duration _currentDuration;

        private string _connectionString;
        private IDbConnection _connection;
        private Duration _prior;

        public Duration(Duration prior)
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
            _currentDuration = _currentDuration._prior;
        }

        public static IDbConnection Enlist(string connectionString)
        {
            // If a duration has begun, use it.
            return _currentDuration != null
                ? (IDbConnection)_currentDuration.GetConnection(connectionString)
                : null;
        }

        public static IDisposable Begin()
        {
            // Push a new duration to the stack.
            _currentDuration = new Duration(_currentDuration);
            return _currentDuration;
        }
    }
}
