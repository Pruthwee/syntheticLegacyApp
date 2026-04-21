// =============================================================================
// RULE ID   : cr-dotnet-0030
// RULE NAME : Windows Authentication
// CATEGORY  : Authentication
// DESCRIPTION: FIXED - Replaced with AWS Directory Service and LDAP authentication
// =============================================================================
using System;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SyntheticLegacyApp.Authentication
{
    public class WindowsAuthentication
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _ldapServer;
        private readonly int _ldapPort;

        public WindowsAuthentication(IHttpContextAccessor httpContextAccessor = null)
        {
            _httpContextAccessor = httpContextAccessor;
            _ldapServer = Environment.GetEnvironmentVariable("LDAP_SERVER") ?? "ldap.example.com";
            _ldapPort = int.Parse(Environment.GetEnvironmentVariable("LDAP_PORT") ?? "389");
        }

        public string GetCurrentWindowsUser()
        {
            // FIXED: Get user from claims-based authentication instead of Windows identity
            var user = _httpContextAccessor?.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                   user?.FindFirst(ClaimTypes.Name)?.Value ?? 
                   "anonymous";
        }

        public bool IsInWindowsGroup(string groupName)
        {
            // FIXED: Check group membership via claims instead of Windows groups
            var user = _httpContextAccessor?.HttpContext?.User;
            if (user == null) return false;

            // Check if user has role claim matching the group
            return user.IsInRole(groupName) || 
                   user.HasClaim(ClaimTypes.Role, groupName) ||
                   user.HasClaim("groups", groupName);
        }

        public async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            // FIXED: LDAP authentication against AWS Managed Microsoft AD
            try
            {
                using (var connection = new LdapConnection(new LdapDirectoryIdentifier(_ldapServer, _ldapPort)))
                {
                    connection.SessionOptions.ProtocolVersion = 3;
                    connection.SessionOptions.SecureSocketLayer = true;
                    
                    var credential = new NetworkCredential(username, password);
                    connection.Bind(credential);
                    
                    return true;
                }
            }
            catch (LdapException)
            {
                return false;
            }
        }

        public void ImpersonateServiceAccount()
        {
            // FIXED: Service accounts now use IAM roles instead of Windows impersonation
            // In AWS, the ECS task role or EC2 instance profile provides service identity
            // No explicit impersonation needed - credentials are automatically available
            PerformPrivilegedOperation();
        }

        public string GetFromContext()
        {
            // FIXED: Get user from HTTP context claims instead of Windows LogonUserIdentity
            return GetCurrentWindowsUser();
        }

        private void PerformPrivilegedOperation() 
        { 
            // Operations now use IAM role permissions automatically
        }
    }
}
