// =============================================================================
// RULE ID   : cr-dotnet-0005
// RULE NAME : IIS Application State
// CATEGORY  : State Management
// DESCRIPTION: Application stores data in HttpApplication.Application state object
//              creating server affinity and preventing horizontal scaling. State
//              is not shared across instances causing inconsistency in cloud.
// =============================================================================
using System.Collections;
using System.Web;

namespace SyntheticLegacyApp.State
{
    public class IISApplicationState
    {
        public void CacheExchangeRates(Hashtable rates)
        {
            // VIOLATION cr-dotnet-0005: HttpContext.Application used as shared in-memory store
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application["ExchangeRates"] = rates;
            HttpContext.Current.Application.UnLock();
        }

        public Hashtable GetExchangeRates()
        {
            // VIOLATION cr-dotnet-0005: Reading from Application state - per-server only
            return HttpContext.Current.Application["ExchangeRates"] as Hashtable;
        }

        public void IncrementRequestCounter()
        {
            // VIOLATION cr-dotnet-0005: Server-local counter in Application state
            HttpContext.Current.Application.Lock();
            int count = (int)(HttpContext.Current.Application["RequestCount"] ?? 0);
            HttpContext.Current.Application["RequestCount"] = count + 1;
            HttpContext.Current.Application.UnLock();
        }
    }
}
