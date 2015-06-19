using System;
using System.Data;

namespace Correspondence.SSCE
{
    internal class Session : IDisposable
    {
        private IDbConnection _connection;
        private bool _enlisted;
        private IDbCommand _command;

        public Session(string connectionString)
        {
            // First try to enlist.
            _connection = Duration.Enlist(connectionString);
            _enlisted = _connection != null;

            // If we did not enlist, then create our own connection.
            if (_connection == null)
            {
                _connection = new System.Data.SqlServerCe.SqlCeConnection(connectionString);
                _connection.Open();
            }

            _command = _connection.CreateCommand();
        }

        public IDbCommand Command
        {
            get { return _command; }
        }

        public void Dispose()
        {
            if (_command != null)
                _command.Dispose();

            if (!_enlisted)
                _connection.Close();
        }
    }
}
