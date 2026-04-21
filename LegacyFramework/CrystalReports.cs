// =============================================================================
// RULE ID   : cr-dotnet-0057
// RULE NAME : Crystal Reports Usage
// CATEGORY  : LegacyFramework
// DESCRIPTION: Application uses Crystal Reports for report generation through
//              CrystalDecisions assemblies. Crystal Reports runtime components
//              are not available in cloud environments, preventing report
//              generation functionality.
// =============================================================================

using System;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace SyntheticLegacyApp.LegacyFramework
{
    public class CrystalReportGenerator
    {
        // VIOLATION cr-dotnet-0057: ReportDocument from CrystalDecisions — requires Crystal runtime
        private readonly ReportDocument _reportDoc = new ReportDocument();

        // VIOLATION cr-dotnet-0057: Loading a Crystal Reports .rpt file — runtime-only format
        public void LoadReport(string rptFilePath)
        {
            _reportDoc.Load(rptFilePath);
        }

        // VIOLATION cr-dotnet-0057: Setting Crystal Reports database login info
        public void SetDatabaseCredentials(string server, string db, string user, string password)
        {
            TableLogOnInfo logOnInfo = new TableLogOnInfo();
            logOnInfo.ConnectionInfo.ServerName   = server;
            logOnInfo.ConnectionInfo.DatabaseName = db;
            logOnInfo.ConnectionInfo.UserID       = user;
            logOnInfo.ConnectionInfo.Password     = password;

            // VIOLATION cr-dotnet-0057: Iterating Crystal report tables to apply connection
            foreach (Table table in _reportDoc.Database.Tables)
            {
                table.ApplyLogOnInfo(logOnInfo);
            }
        }

        // VIOLATION cr-dotnet-0057: Exporting to PDF via Crystal Reports engine
        public byte[] ExportToPdf()
        {
            ExportOptions exportOpts = new ExportOptions
            {
                ExportFormatType      = ExportFormatType.PortableDocFormat,
                ExportDestinationType = ExportDestinationType.Stream
            };

            _reportDoc.ExportOptions.ExportFormatType      = ExportFormatType.PortableDocFormat;
            _reportDoc.ExportOptions.ExportDestinationType = ExportDestinationType.Stream;

            using (var ms = new MemoryStream())
            {
                // VIOLATION cr-dotnet-0057: ExportToStream — Crystal Reports export pipeline
                Stream reportStream = _reportDoc.ExportToStream(ExportFormatType.PortableDocFormat);
                reportStream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        // VIOLATION cr-dotnet-0057: Passing DataSet to Crystal Reports subreport
        public void BindDataSetToReport(System.Data.DataSet ds, string tableName)
        {
            _reportDoc.SetDataSource(ds.Tables[tableName]);
        }

        // VIOLATION cr-dotnet-0057: Setting Crystal parameter fields at runtime
        public void SetReportParameter(string paramName, object value)
        {
            ParameterFieldDefinitions paramDefs =
                _reportDoc.DataDefinition.ParameterFields;

            ParameterFieldDefinition paramField = paramDefs[paramName];
            ParameterValues values = new ParameterValues();
            values.Add(new ParameterDiscreteValue { Value = value });
            paramField.ApplyCurrentValues(values);
        }

        public void Dispose()
        {
            _reportDoc?.Close();
            _reportDoc?.Dispose();
        }
    }
}
