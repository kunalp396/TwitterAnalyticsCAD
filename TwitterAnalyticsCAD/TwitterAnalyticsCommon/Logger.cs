using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAnalyticsCommon
{
    public class Logger : ILogger
    {
        private static object mLock;
        private static Logger mLogger = null;
        private string fileName = string.Empty;

        //Log Format will be DateTime : LogLevel(Debug/Error/Info) : Log Message
        private string messageFormatter = "{0} : {1} : {2} " ;

        public static Logger Instance
        {
            get
            {
                if (mLogger == null)
                {
                    lock (mLock)
                    {
                        mLogger = new Logger();
                        mLogger.fileName = string.Format("Log{0}{1}{2}{3}{4}",                       
                            DateTime.Now.Month , DateTime.Now.Day , DateTime.Now.Second , DateTime.Now.Millisecond , ".txt");
                    }
                }
                return mLogger;
            }
        }

        public void Log(string message)
        {
            File.AppendAllText(fileName, string.Format(messageFormatter,DateTime.Now.ToString(), LOGLEVELS.INFO , message));
            File.AppendAllText(fileName, Environment.NewLine);
        }

        public void Log(string message, LOGLEVELS loglevel)
        {
            File.AppendAllText(fileName, string.Format(messageFormatter, DateTime.Now.ToString(), loglevel , message));
            File.AppendAllText(fileName, Environment.NewLine);

        }

        private Logger()
        {
            mLock = new object();
        }
    }
}
