using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAnalyticsCommon
{
    public abstract class Logger : ILogger
    {
        protected string fileName = string.Empty;

        //Log Format will be DateTime : LogLevel(Debug/Error/Info) : Log Message
        protected string messageFormatter = "{0} : {1} : {2} " ;

        public abstract void Log(string message);

        public abstract void Log(string message, LOGLEVELS loglevel);        

        public Logger()
        {
        }
    }
}
