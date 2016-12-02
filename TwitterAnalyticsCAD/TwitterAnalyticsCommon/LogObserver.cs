using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAnalyticsCommon
{
    public class LogObserver : IObserver<string>
    {
        private Logger logger = Logger.Instance;
           
        public void OnNext(string message)
        {
            logger.Log(message, LOGLEVELS.INFO);
        }
        
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
