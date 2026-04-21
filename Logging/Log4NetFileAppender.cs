// =============================================================================
// RULE ID   : cr-dotnet-0035
// RULE NAME : Custom Log4Net Appenders
// CATEGORY  : Logging
// DESCRIPTION: Application uses log4net file appenders that write to local file
//              system. In cloud environments with ephemeral storage, local file
//              logs may be lost during instance lifecycle events.
// =============================================================================
// NOTE: log4net file appender is configured in log4net.config with
//       RollingFileAppender pointing to C:\Logs\SyntheticApppp.log
//       This file shows the C# usage layer that routes to that appender.
using log4net;
using log4net.Config;

namespace SyntheticLegacyApp.Logging
{
    public class Log4NetFileAppender
    {
        // VIOLATION cr-dotnet-0035: log4net configured with RollingFileAppender to local disk
        private static readonly ILog _log = LogManager.GetLogger(typeof(Log4NetFileAppender));

        public void Initialize()
        {
            // VIOLATION cr-dotnet-0035: Config file wires appender to local disk path
            XmlConfigurator.Configure(new System.IO.FileInfo(@"C:\Config\log4net.config"));
        }

        public void LogOrderProcessed(string orderId, decimal amount)
        {
            // VIOLATION cr-dotnet-0035: Log routed to local RollingFileAppender
            _log.InfoFormat("Order processed: {0}, Amount: {1}", orderId, amount);
        }

        public void LogException(System.Exception ex)
        {
            // VIOLATION cr-dotnet-0035: Exception log persisted to ephemeral local storage
            _log.Error("Unhandled exception", ex);
        }
    }
}
