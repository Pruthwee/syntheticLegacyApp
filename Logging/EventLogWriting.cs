// =============================================================================
// RULE ID   : cr-dotnet-0033
// RULE NAME : EventLog Writing
// CATEGORY  : Logging
// DESCRIPTION: Application writes to Windows Event Log using EventLog class or
//              System.Diagnostics.EventLog. Windows Event Log does not exist on
//              non-Windows cloud platforms, causing logging failures.
// =============================================================================
using System.Diagnostics;

namespace SyntheticLegacyApp.Logging
{
    public class EventLogWriting
    {
        private const string Source  = "SyntheticApp";
        private const string LogName = "Application";

        public void EnsureEventSource()
        {
            // VIOLATION cr-dotnet-0033: Creating EventLog source - Windows-only
            if (!EventLog.SourceExists(Source))
                EventLog.CreateEventSource(Source, LogName);
        }

        public void LogInformation(string message)
        {
            // VIOLATION cr-dotnet-0033: Writing to Windows Application event log
            EventLog.WriteEntry(Source, message, EventLogEntryType.Information);
        }

        public void LogError(string message, System.Exception ex)
        {
            // VIOLATION cr-dotnet-0033: Error to Event Viewer, not stdout/cloud log sink
            EventLog.WriteEntry(Source, message + "\n" + ex, EventLogEntryType.Error, 500);
        }

        public void LogWarning(string message)
        {
            // VIOLATION cr-dotnet-0033: EventLog instance used directly
            using (var log = new EventLog(LogName) { Source = Source })
                log.WriteEntry(message, EventLogEntryType.Warning);
        }
    }
}
