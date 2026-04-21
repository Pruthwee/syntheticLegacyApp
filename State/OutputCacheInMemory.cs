// =============================================================================
// RULE ID   : cr-dotnet-0046
// RULE NAME : Output Cache Provider
// CATEGORY  : State Management
// DESCRIPTION: Application uses in-memory output caching through System.Web
//              Caching.OutputCache or [OutputCache] attributes. In-memory caches
//              do not synchronize across multiple instances causing inconsistency.
// =============================================================================
using System;
using System.Web;
using System.Web.Caching;

namespace SyntheticLegacyApp.State
{
    public class OutputCacheInMemory
    {
        public void CacheHomepageContent(string content)
        {
            // VIOLATION cr-dotnet-0046: HttpRuntime.Cache writes to in-memory store only
            HttpContext.Current.Cache.Insert(
                "HomepageContent", content, null,
                Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
        }

        public string GetCachedHomepage()
        {
            // VIOLATION cr-dotnet-0046: Reading from in-memory cache - not distributed
            return HttpContext.Current.Cache["HomepageContent"] as string;
        }

        public void CacheNavigationMenu(object menuItems)
        {
            // VIOLATION cr-dotnet-0046: Navigation data cached per-instance in memory
            HttpRuntime.Cache.Insert("NavigationMenu", menuItems);
        }
    }
}
