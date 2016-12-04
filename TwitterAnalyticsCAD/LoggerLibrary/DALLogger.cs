﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAnalyticsCommon
{
    public class DALLogger: Logger
    {
        public DALLogger()
        {
            fileName = string.Format("DALLog{0}{1}{2}{3}{4}",
                            DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Second, DateTime.Now.Millisecond, ".txt");
        }

        public override void Log(string message)
        {
            File.AppendAllText(fileName, string.Format(messageFormatter, DateTime.Now.ToString(), LOGLEVELS.INFO, message));
            File.AppendAllText(fileName, Environment.NewLine);
        }

        public override void Log(string message, LOGLEVELS loglevel)
        {
            File.AppendAllText(fileName, string.Format(messageFormatter, DateTime.Now.ToString(), loglevel, message));
            File.AppendAllText(fileName, Environment.NewLine);
        }
    }
}
