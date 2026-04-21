// =============================================================================
// RULE ID   : cr-dotnet-0045
// RULE NAME : Session State Provider
// CATEGORY  : State Management
// DESCRIPTION: Application uses in-process session state (InProc) for user
//              session management through HttpSessionState. In-process sessions
//              create server affinity and do not scale across multiple instances.
// =============================================================================
using System;
using System.Web;

namespace SyntheticLegacyApp.State
{
    public class SessionStateInProc
    {
        public void StoreShoppingCart(string cartJson)
        {
            // VIOLATION cr-dotnet-0045: Storing object in InProc HttpSessionState
            HttpContext.Current.Session["ShoppingCart"] = cartJson;
        }

        public string RetrieveShoppingCart()
        {
            // VIOLATION cr-dotnet-0045: Reading from InProc session - server-sticky
            return HttpContext.Current.Session["ShoppingCart"] as string;
        }

        public void SetUserProfile(object userProfile)
        {
            // VIOLATION cr-dotnet-0045: Complex object in InProc session
            HttpContext.Current.Session["UserProfile"]   = userProfile;
            HttpContext.Current.Session["LastActivity"]  = DateTime.Now;
        }

        public bool IsAuthenticated()
        {
            // VIOLATION cr-dotnet-0045: Auth check via InProc session key
            return HttpContext.Current.Session["UserId"] != null;
        }
    }
}
