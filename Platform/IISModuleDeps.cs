// =============================================================================
// RULE ID   : cr-dotnet-0044
// RULE NAME : IIS Module Dependencies
// CATEGORY  : Platform
// DESCRIPTION: Application depends on Internet Information Services (IIS) modules,
//              handlers, or IIS-specific features. These dependencies prevent
//              deployment to non-IIS cloud hosting environments and PaaS services.
// =============================================================================

using System;
using System.Web;

namespace SyntheticLegacyApp.Platform
{
    // VIOLATION cr-dotnet-0044: Implements IHttpModule — IIS pipeline specific
    public class RequestAuditModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            // VIOLATION cr-dotnet-0044: Hooking into IIS HTTP pipeline events
            context.BeginRequest      += OnBeginRequest;
            context.EndRequest        += OnEndRequest;
            context.AuthenticateRequest += OnAuthenticate;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            // VIOLATION cr-dotnet-0044: HttpContext.Current is IIS-specific
            HttpContext.Current.Items["RequestStart"] = DateTime.UtcNow;
            Console.WriteLine($"[IIS] Request begin: {app.Request.Url}");
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            var start = (DateTime?)HttpContext.Current.Items["RequestStart"];
            Console.WriteLine($"[IIS] Request end. Duration: {(DateTime.UtcNow - start)?.TotalMs()}ms");
        }

        private void OnAuthenticate(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            // VIOLATION cr-dotnet-0044: Reading IIS server variables
            string serverName = app.Request.ServerVariables["SERVER_NAME"];
            Console.WriteLine($"[IIS] Auth request on server: {serverName}");
        }

        public void Dispose() { }
    }

    // VIOLATION cr-dotnet-0044: IHttpHandler — IIS handler, not cloud-portable
    public class LegacyReportHandler : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            // VIOLATION cr-dotnet-0044: Direct use of HttpContext request pipeline
            string reportId = context.Request.QueryString["id"];

            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("Content-Disposition",
                $"attachment; filename=report_{reportId}.pdf");

            byte[] pdf = GenerateReport(reportId);
            context.Response.BinaryWrite(pdf);
            context.Response.End(); // VIOLATION cr-dotnet-0044: Response.End() is IIS-specific
        }

        private byte[] GenerateReport(string id) => new byte[0];
    }

    public class IISFeatureHelper
    {
        public void ConfigureCompression()
        {
            // VIOLATION cr-dotnet-0044: Manipulating IIS server manager programmatically
            // Microsoft.Web.Administration.ServerManager is IIS-only
            Console.WriteLine("Configuring IIS dynamic/static compression via ServerManager...");
        }

        public string GetAppPoolName()
        {
            // VIOLATION cr-dotnet-0044: Reading IIS application pool from server variables
            return HttpContext.Current?.Request.ServerVariables["APP_POOL_ID"] ?? "DefaultAppPool";
        }
    }
}

// Helper to avoid compile error on TotalMs()
public static class TimeSpanExtensions
{
    public static double TotalMs(this System.TimeSpan ts) => ts.TotalMilliseconds;
}
