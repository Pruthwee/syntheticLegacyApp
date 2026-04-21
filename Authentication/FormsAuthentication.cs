// =============================================================================
// RULE ID   : cr-dotnet-0029
// RULE NAME : Forms Authentication
// CATEGORY  : Authentication
// DESCRIPTION: Application uses ASP.NET Forms Authentication for user authentication
//              and session management. Relies on machine keys and server affinity
//              which do not work in distributed cloud environments with load balancing.
// =============================================================================
using System;
using System.Web.Security;

namespace SyntheticLegacyApp.Authentication
{
    public class FormsAuthenticationUsage
    {
        public void AuthenticateUser(string username, string password)
        {
            if (ValidateCredentials(username, password))
            {
                // VIOLATION cr-dotnet-0029: SetAuthCookie uses machine-key-signed ticket
                FormsAuthentication.SetAuthCookie(username, createPersistentCookie: true);
            }
        }

        public void SignOut()
        {
            // VIOLATION cr-dotnet-0029: FormsAuthentication sign-out is per-server
            FormsAuthentication.SignOut();
        }

        public string CreateFormsTicket(string userId, string userData)
        {
            // VIOLATION cr-dotnet-0029: Ticket encrypted with per-server machine key
            var ticket = new FormsAuthenticationTicket(
                version: 1, name: userId, issueDate: DateTime.Now,
                expiration: DateTime.Now.AddHours(8), isPersistent: false,
                userData: userData, cookiePath: FormsAuthentication.FormsCookiePath);
            return FormsAuthentication.Encrypt(ticket);
        }

        public FormsAuthenticationTicket DecryptTicket(string encrypted)
        {
            // VIOLATION cr-dotnet-0029: Decrypt requires same machine key on every instance
            return FormsAuthentication.Decrypt(encrypted);
        }

        private bool ValidateCredentials(string u, string p) => true;
    }
}
