using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAnalyticsCommon
{
    public class LoggerFactory<T>
    {
        private static object mLock;
        private static LoggerFactory<T> mLogger = null;

        private LoggerFactory()
        {
            mLock = new object();
        }
        public static LoggerFactory<T> Instance
        {
            get
            {
                if (mLogger == null)
                {
                    lock (mLock)
                    {
                        mLogger = new LoggerFactory<T>();
                    }
                }
                return mLogger;
            }
        }
        static readonly Dictionary<Type, Func<T>> _dict
             = new Dictionary<Type, Func<T>>();

        public int Count { get { return _dict.Count; } }

        public static T Create(Type loggerType)
        {
            Func<T> constructor = null;

            if (_dict.TryGetValue(loggerType, out constructor))
                return constructor();

            throw new ArgumentException("No type registered for this id");
        }

        public static void Register(Type loggerType, Func<T> ctor)
        {
            if (!_dict.Keys.Contains(loggerType))
            {
                _dict.Add(loggerType, ctor);
            }
        }
    }
}
