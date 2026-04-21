// =============================================================================
// RULE ID   : cr-dotnet-0034
// RULE NAME : TraceListener File Output
// CATEGORY  : Logging
// DESCRIPTION: Application uses TraceListener implementations that write to local
//              files for diagnostic output. In cloud environments with ephemeral
//              storage, file-based trace output may be lost during instance changes.
// =============================================================================
using System.Diagnostics;

namespace SyntheticLegacyApp.Logging
{
    public class TraceListenerFileOutput
    {
        public void ConfigureTracing()
        {
            // VIOLATION cr-dotnet-0034: TextWriterTraceListener writes to local file
            Trace.Listeners.Add(
                new TextWriterTraceListener(@"C:\Logs\SyntheticApp	race.log", "FileTraceListener"));
            Trace.AutoFlush = true;
        }

        public void AddDelimitedListener()
        {
            // VIOLATION cr-dotnet-0034: DelimitedListTraceListener also writes local
            Trace.Listeners.Add(
                new DelimitedListTraceListener(@"C:\Logs\SyntheticApp	race.csv"));
        }

        public void TraceApplicationEvent(string message, int id)
        {
            // VIOLATION cr-dotnet-0034: Trace routed to local file listeners above
            Trace.TraceInformation("Event {0}: {1}", id, message);
            Trace.Flush();
        }
    }
}
