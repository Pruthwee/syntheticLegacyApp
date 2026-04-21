// =============================================================================
// RULE ID   : cr-dotnet-0031
// RULE NAME : IP Address Restrictions
// CATEGORY  : Authentication
// DESCRIPTION: Application implements IP address-based security restrictions.
//              Cloud load balancers and reverse proxies mask original client IPs,
//              causing security rules to fail or incorrectly block/allow access.
// =============================================================================
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace SyntheticLegacyApp.Authentication
{
    public class IPAddressRestrictions
    {
        // VIOLATION cr-dotnet-0031: IP whitelist hard-coded for corporate network ranges
        private static readonly HashSet<string> AllowedIPs = new HashSet<string>
        {
            "192.168.1.100", "10.0.0.5", "203.0.113.45"
        };

        public bool IsRequestAllowed(HttpRequest request)
        {
            // VIOLATION cr-dotnet-0031: UserHostAddress will be LB IP behind cloud NLB
            return AllowedIPs.Contains(request.UserHostAddress);
        }

        public void EnforceIPRestriction()
        {
            // VIOLATION cr-dotnet-0031: REMOTE_ADDR is cloud LB IP, not client IP
            string ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (!AllowedIPs.Contains(ip))
            {
                HttpContext.Current.Response.StatusCode = 403;
                HttpContext.Current.Response.End();
            }
        }

        public bool IsAdminRequest(string remoteAddr)
        {
            // VIOLATION cr-dotnet-0031: Localhost check invalid behind cloud proxy
            return remoteAddr == "127.0.0.1" || remoteAddr == "::1";
        }
    }
}
