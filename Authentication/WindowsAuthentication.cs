// =============================================================================
// RULE ID   : cr-dotnet-0030
// RULE NAME : Windows Authentication
// CATEGORY  : Authentication
// DESCRIPTION: Application uses Windows Authentication (NTLM/Kerberos) for user
//              authentication. Does not work in non-Windows cloud platforms or
//              environments without Active Directory integration.
// =============================================================================
using System.Security.Principal;
using System.Web;

namespace SyntheticLegacyApp.Authentication
{
    public class WindowsAuthentication
    {
        public string GetCurrentWindowsUser()
        {
            // VIOLATION cr-dotnet-0030: WindowsIdentity relies on Windows kernel authentication
            return WindowsIdentity.GetCurrent()?.Name;
        }

        public bool IsInWindowsGroup(string groupName)
        {
            // VIOLATION cr-dotnet-0030: WindowsPrincipal group check - NTLM/Kerberos required
            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return principal.IsInRole(groupName);
        }

        public void ImpersonateServiceAccount()
        {
            // VIOLATION cr-dotnet-0030: Windows impersonation token - no cloud equivalent
            using (WindowsIdentity.Impersonate(GetServiceAccountToken()))
            {
                PerformPrivilegedOperation();
            }
        }

        public string GetFromContext()
        {
            // VIOLATION cr-dotnet-0030: LogonUserIdentity is IIS/Windows-only
            return HttpContext.Current.Request.LogonUserIdentity?.Name;
        }

        private System.IntPtr GetServiceAccountToken() => System.IntPtr.Zero;
        private void PerformPrivilegedOperation() { }
    }
}
