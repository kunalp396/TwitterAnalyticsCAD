/*************************************************************
** Class generated by CodeTrigger, Version 4.8.6.9
** This class was generated on 18-11-2016 02:12:11
**************************************************************/

using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using TwitterAnalyticsCommon;

namespace TwitterAnalyticsDBL.DataObjects
{
	public partial class AzureSQLDBConn_BaseData : IDisposable
	{
		#region members
		protected AzureSQLDBConn_TxConnectionProvider _connectionProvider;
		static string _staticConnectionString;
		bool _isDisposed = false;
		#endregion

		#region initialisation
		public AzureSQLDBConn_BaseData()
		{
			Init();
		}

		private void Init()
		{
		}
		#endregion

		#region disposable interface support
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
					if(_connectionProvider != null)
					{
						((IDisposable)_connectionProvider).Dispose();
						_connectionProvider = null;
					}
				}
			}
			_isDisposed = true;
		}
		#endregion

		#region connection support
		public static SqlConnection StaticSqlConnection
		{
			get
			{
                SqlConnection staticConnection = new SqlConnection();
                staticConnection.ConnectionString = StaticConnectionString;
                return staticConnection;
            }
		}

		public virtual AzureSQLDBConn_TxConnectionProvider ConnectionProvider
		{
			set
			{
				if(value == null)
					throw new Exception("Connection provider cannot be null");
				
				_connectionProvider = value;
			}
		}

		public static string StaticConnectionString
		{
			set { _staticConnectionString = value; }
			get
			{
				if (string.IsNullOrEmpty(_staticConnectionString))
					_staticConnectionString=ConfigurationManager.ConnectionStrings["AzureSQLDBConn"].ConnectionString;

                return _staticConnectionString;
			}
		}
		#endregion

	}
}