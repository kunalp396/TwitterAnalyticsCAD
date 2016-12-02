
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;
using TwitterAnalyticsCommon;

namespace TwitterAnalyticsDBL.DataObjects
{
	public partial class AzureSQLDBConn_TxConnectionProvider : IDisposable
	{
		protected bool _isDisposed;
        protected SqlConnection _txConnection;
        protected SqlTransaction _currTransaction;
		protected Int32 _transactionCount = 0;
		static string _connectionString;

		public AzureSQLDBConn_TxConnectionProvider()
		{
			Init();
		}

		private void Init()
		{
            _txConnection = AzureSQLConnMngr.Instance.AzureSqlConnection;
			_txConnection.ConnectionString = ConnectionString;
			_currTransaction = null;
			_isDisposed = false;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if(!_isDisposed)
			{
				if(isDisposing)
				{
					if(_currTransaction != null)
					{
						_currTransaction.Dispose();
						_currTransaction = null;
					}
					if(_txConnection != null)
					{
						/*this will also rollback any pending transactions on this connection*/
						_txConnection.Close();
						_txConnection.Dispose();
						_txConnection = null;
					}
				}
			}
			_isDisposed = true;
		}

		public virtual void OpenConnection()
		{
			try
			{
				if(_txConnection.State == ConnectionState.Open)
					throw new Exception("Connection is already open");
				
				_txConnection.Open();
				_currTransaction = null;
				_isDisposed = false;
			}
			catch
			{
				throw;
			}
		}

		public virtual void CloseConnection(bool commit)
		{
			if((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
				return;
			try
			{
				if((_currTransaction != null) && commit)
					_currTransaction.Commit();
				
				else if(_currTransaction != null)
					_currTransaction.Rollback();
				
				if(_currTransaction != null)
					_currTransaction.Dispose();
				
				_currTransaction = null;
				_txConnection.Close();
			}
			catch
			{
				throw;
			}
		}

		public virtual void BeginTransaction(string trans)
		{
			if(_currTransaction != null)
				throw new Exception("Transaction nesting not allowed");
			
			if((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
				throw new Exception("Connection not open");
			
			try
			{
				_currTransaction = _txConnection.BeginTransaction(IsolationLevel.ReadCommitted, trans);
			}
			catch
			{
				throw;
			}
		}

		public virtual void CommitTransaction()
		{
			if(_currTransaction == null)
				throw new Exception("No Transaction to commit");
			
			if((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
				throw new Exception("Connection not open");
			
			try
			{
				_currTransaction.Commit();
				_currTransaction.Dispose();
				_currTransaction = null;
			}
			catch
			{
				throw;
			}
		}

		public virtual void RollbackTransaction(string trans)
		{
			if(_currTransaction == null)
				throw new Exception("No Transaction to rollback");
			
			if((_txConnection == null) || (_txConnection.State != ConnectionState.Open))
				throw new Exception("Connection not open");
			
			try
			{
				_currTransaction.Rollback(trans);
				_currTransaction.Dispose();
				_currTransaction = null;
			}
			catch
			{
				throw;
			}
		}

		public virtual SqlTransaction CurrentTransaction
		{
			get
			{
				return _currTransaction;
			}
		}

		public virtual Int32 TransactionCount
		{
			get { return _transactionCount; }
			set { _transactionCount = value; }
		}

		public virtual SqlConnection Connection
		{
			get
			{
				return _txConnection;
			}
		}

		public static string ConnectionString
		{
			set { _connectionString = value; }
			get
			{
				
                    if (string.IsNullOrEmpty(_connectionString))
                    _connectionString = ConfigurationManager.ConnectionStrings["AzureSQLDBConn"].ConnectionString;

                return _connectionString;
            }
		}
	}
}
