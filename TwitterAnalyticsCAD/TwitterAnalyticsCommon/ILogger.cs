using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAnalyticsCommon
{
    public interface ILogger
    {
        void Log(string message);

        void Log(string message, LOGLEVELS loglevel);
    }

    public enum LOGLEVELS
    {
        ERROR=-1,
        DEBUG=0,
        INFO=1
    }
}
