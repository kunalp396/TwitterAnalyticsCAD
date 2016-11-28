using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAnalyticsCommon
{
    public class AzureSQLConnMngr
    {
        private SqlConnection sqlConnection;
        private static AzureSQLConnMngr _instance;

        public static AzureSQLConnMngr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AzureSQLConnMngr();
                    _instance.sqlConnection = new SqlConnection();
                }
                return _instance;
            }
        }        

        public SqlConnection AzureSqlConnection
        {
            get
            {

                return this.sqlConnection;
            }
        }


    }
}
