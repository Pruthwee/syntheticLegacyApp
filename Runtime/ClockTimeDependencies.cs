// =============================================================================
// RULE ID   : cr-dotnet-0121
// RULE NAME : Clock/Time Dependencies
// CATEGORY  : Runtime
// DESCRIPTION: Application relies on server-local timezone settings using
//              DateTime.Now, TimeZone.CurrentTimeZone, or implements scheduled
//              operations without considering distributed cloud environments. In
//              cloud deployments across multiple regions, timezone inconsistencies
//              cause scheduling failures.
// =============================================================================

using System;
using System.Globalization;

namespace SyntheticLegacyApp.Runtime
{
    public class ScheduledJobRunner
    {
        // VIOLATION cr-dotnet-0121: DateTime.Now returns local server time — varies by cloud region
        public bool IsWithinMaintenanceWindow()
        {
            DateTime now = DateTime.Now; // depends on server's local timezone
            return now.Hour >= 2 && now.Hour < 4;
        }

        // VIOLATION cr-dotnet-0121: Comparing local DateTime for business-hours logic
        public bool IsBusinessHours()
        {
            DateTime localNow = DateTime.Now;
            return localNow.DayOfWeek != DayOfWeek.Saturday &&
                   localNow.DayOfWeek != DayOfWeek.Sunday  &&
                   localNow.Hour >= 8  && localNow.Hour < 18;
        }

        // VIOLATION cr-dotnet-0121: TimeZone.CurrentTimeZone — deprecated, server-local
        public string GetServerTimezone()
        {
            return TimeZone.CurrentTimeZone.StandardName;
        }

        // VIOLATION cr-dotnet-0121: Scheduling based on local time — multi-region pods will conflict
        public void RunNightlyBatch()
        {
            if (DateTime.Now.Hour != 1)
            {
                Console.WriteLine("Nightly batch skipped — not 01:00 local time.");
                return;
            }
            Console.WriteLine("Running nightly batch at local 01:00...");
        }
    }

    public class AuditTimestampService
    {
        // VIOLATION cr-dotnet-0121: DateTime.Now used in audit records — timezone not normalised
        public string GetAuditTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // local time, no offset
        }

        // VIOLATION cr-dotnet-0121: ToLocalTime() conversion assumes correct server timezone
        public DateTime ConvertToDisplayTime(DateTime utcTime)
        {
            return utcTime.ToLocalTime(); // server timezone decides output
        }

        // VIOLATION cr-dotnet-0121: Date-only comparison using local midnight boundary
        public bool IsToday(DateTime date)
        {
            return date.Date == DateTime.Today; // DateTime.Today is local server date
        }
    }

    public class ReportScheduler
    {
        // VIOLATION cr-dotnet-0121: TimeZoneInfo.Local — server-specific timezone assignment
        private readonly TimeZoneInfo _reportingZone = TimeZoneInfo.Local;

        public DateTime GetNextReportTime()
        {
            // VIOLATION cr-dotnet-0121: ConvertFromUtc using Local — non-deterministic in cloud
            DateTime localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _reportingZone);
            DateTime nextRun  = localNow.Date.AddDays(1).AddHours(6); // next day 06:00 local
            return TimeZoneInfo.ConvertTimeToUtc(nextRun, _reportingZone);
        }
    }
}
