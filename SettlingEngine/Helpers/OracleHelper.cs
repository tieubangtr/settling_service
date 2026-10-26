using DapperLib.LinQToSql.Core;
using Oracle.ManagedDataAccess.Client;
using SettlingEngine.Constants;
using System.Data;

namespace SettlingEngine.Helpers
{
    public interface IOracleHelper
    {
        DBContext CreateNewDbContext();
    }
    public interface ITransactionHelper : IDisposable
    {
        IDbTransaction Transaction { get; }
        IDbConnection Connection { get; }

        void Commit();
        void Rollback();
    }

    public class OracleHelper : IOracleHelper
    {
        private static string ConnectionString => ConfigData.ConnectionString;

        public DBContext CreateNewDbContext()
        {
            OracleConnection conn = new(ConnectionString);
            return new DBContext(conn);
        }
    }

    public class TransactionHelper : ITransactionHelper
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private bool _isCommitted = false;
        private bool _isRolledBack = false;

        public IDbTransaction Transaction => _transaction;

        public IDbConnection Connection => _connection;

        public TransactionHelper(IDbConnection connection)
        {
            _connection = connection;

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            if (_isCommitted || _isRolledBack)
            {
                throw new InvalidOperationException("Transaction has already been committed or rolled back.");
            }

            _transaction.Commit();
            _isCommitted = true;
        }

        public void Rollback()
        {
            if (_isCommitted || _isRolledBack)
            {
                throw new InvalidOperationException("Transaction has already been committed or rolled back.");
            }

            _transaction.Rollback();
            _isRolledBack = true;
        }

        public void Dispose()
        {
            if (!_isCommitted && !_isRolledBack)
            {
                try
                {
                    _transaction?.Rollback();
                }
                catch
                {
                    // Log only, don't throw in Dispose
                }
            }

            _transaction?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
