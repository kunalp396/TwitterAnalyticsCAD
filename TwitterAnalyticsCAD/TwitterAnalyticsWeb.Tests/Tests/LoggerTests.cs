using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterAnalyticsCommon;

namespace TwitterAnalyticsWeb.Tests.Tests
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public void Logger_Setup_IsComplete()
        {
            try
            {
                SetupLoggers();
                var count = LoggerFactory<ILogger>.ItemsCount;
                Assert.IsTrue(count > 0);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void WebLogger_Without_LogLevel_IsWorking()
        {
            try
            {
                SetupLoggers();
                ILogger logger = LoggerFactory<ILogger>.Create(typeof(WebLogger));
                logger.Log("WebLoggerTest is working!!");
                Assert.IsNotNull(logger);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void WebLogger_With_LogLevel_IsWorking()
        {
            try
            {
                SetupLoggers();
                ILogger logger = LoggerFactory<ILogger>.Create(typeof(WebLogger));
                logger.Log("WebLoggerTest with LogLevel is working!!",LOGLEVELS.DEBUG);
                Assert.IsNotNull(logger);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DALLogger_Without_LogLevel_IsWorking()
        {
            try
            {
                SetupLoggers();
                ILogger logger = LoggerFactory<ILogger>.Create(typeof(DALLogger));
                logger.Log("DALLoggerTest is working!!");
                Assert.IsNotNull(logger);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DALLogger_With_Loglevel_IsWorking()
        {
            try
            {
                SetupLoggers();
                ILogger logger = LoggerFactory<ILogger>.Create(typeof(DALLogger));
                logger.Log("DALLoggerTest with LogLevel is working!!", LOGLEVELS.DEBUG);
                Assert.IsNotNull(logger);
            }
            catch
            {
                Assert.Fail();
            }
        }

        void SetupLoggers()
        {
            LoggerFactory<ILogger>.Register(typeof(WebLogger), () => new WebLogger());
            LoggerFactory<ILogger>.Register(typeof(DALLogger), () => new DALLogger());
        }
    }
}
