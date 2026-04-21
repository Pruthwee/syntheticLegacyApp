// =============================================================================
// RULE ID   : cr-dotnet-0028
// RULE NAME : Enterprise Library Usage
// CATEGORY  : LegacyFramework
// DESCRIPTION: Application depends on Microsoft Enterprise Library for logging,
//              data access, or exception handling. Enterprise Library is legacy
//              technology that lacks cloud optimization and modern async patterns.
// =============================================================================

using System;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using System.Data.Common;

namespace SyntheticLegacyApp.LegacyFramework
{
    public class EnterpriseLibraryDataAccess
    {
        // VIOLATION cr-dotnet-0028: DatabaseFactory from Enterprise Library Data block
        private readonly Database _db;

        public EnterpriseLibraryDataAccess()
        {
            // VIOLATION cr-dotnet-0028: DatabaseFactory.CreateDatabase — EntLib database factory
            _db = DatabaseFactory.CreateDatabase("DefaultConnection");
        }

        public DataSet GetOrdersByCustomer(int customerId)
        {
            // VIOLATION cr-dotnet-0028: DbCommand created via Enterprise Library helper
            DbCommand cmd = _db.GetStoredProcCommand("usp_GetOrdersByCustomer");
            _db.AddInParameter(cmd, "@CustomerId", DbType.Int32, customerId);

            // VIOLATION cr-dotnet-0028: ExecuteDataSet — synchronous, no async equivalent in EntLib
            return _db.ExecuteDataSet(cmd);
        }

        public int CreateOrder(string orderJson)
        {
            DbCommand cmd = _db.GetStoredProcCommand("usp_CreateOrder");
            _db.AddInParameter(cmd, "@OrderJson", DbType.String, orderJson);
            _db.AddParameter(cmd, "@OrderId", DbType.Int32, ParameterDirection.Output,
                "", DataRowVersion.Default, null);

            _db.ExecuteNonQuery(cmd);
            return (int)_db.GetParameterValue(cmd, "@OrderId");
        }
    }

    public class EnterpriseLibraryLogger
    {
        // VIOLATION cr-dotnet-0028: Logger.Write from Enterprise Library Logging block
        public void LogInformation(string message)
        {
            // VIOLATION cr-dotnet-0028: LogEntry with Enterprise Library — writes to local file/EventLog
            var entry = new LogEntry
            {
                Message    = message,
                Categories = { "General" },
                Severity   = System.Diagnostics.TraceEventType.Information,
                EventId    = 1000,
                MachineName = Environment.MachineName
            };

            Logger.Write(entry);
        }

        public void LogError(Exception ex, string context)
        {
            var entry = new LogEntry
            {
                Message    = $"{context}: {ex.Message}\n{ex.StackTrace}",
                Categories = { "Errors" },
                Severity   = System.Diagnostics.TraceEventType.Error,
                EventId    = 9000
            };

            Logger.Write(entry);
        }
    }

    public class EnterpriseLibraryExceptionManager
    {
        // VIOLATION cr-dotnet-0028: ExceptionManager from Enterprise Library Exception block
        private readonly ExceptionManager _exceptionManager;

        public EnterpriseLibraryExceptionManager()
        {
            _exceptionManager = EnterpriseLibraryContainer.Current
                .GetInstance<ExceptionManager>();
        }

        public void HandleException(Exception ex, string policy)
        {
            // VIOLATION cr-dotnet-0028: HandleException using EntLib policy config
            bool rethrow = _exceptionManager.HandleException(ex, policy);
            if (rethrow) throw ex;
        }
    }
}
