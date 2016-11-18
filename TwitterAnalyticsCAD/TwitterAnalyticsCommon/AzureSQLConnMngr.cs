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
        private AzureSQLConnMngr()
        { }

        private static AzureSQLConnMngr _instance;
        private readonly SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["AzureSQLDBConn"].ConnectionString);

        private static AzureSQLConnMngr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AzureSQLConnMngr();
                }
                return _instance;
            }

        }

        public static AzureSQLConnMngr GetInstance(bool isUserAuthenticated)
        {

            if (isUserAuthenticated)
            {
                return Instance;
            }
            else
            {
                throw new UnauthorizedAccessException("User is not Autherized !!"); 
            }
            
        }

        public SqlConnection GetSqlConnection()
        {
            return this.sqlConn;
        }


    }
}
