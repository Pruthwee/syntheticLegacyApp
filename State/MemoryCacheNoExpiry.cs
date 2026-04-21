// =============================================================================
// RULE ID   : cr-dotnet-0007
// RULE NAME : MemoryCache Without Expiration
// CATEGORY  : State Management
// DESCRIPTION: Application uses MemoryCache without proper expiration policies.
//              Causes memory growth and stale data inconsistencies across multiple
//              instances in distributed cloud environments.
// =============================================================================
using System.Runtime.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace SyntheticLegacyApp.State
{
    public class MemoryCacheNoExpiry
    {
        // VIOLATION cr-dotnet-0007: MemoryCache.Default with no expiration
        private static readonly MemoryCache _cache = MemoryCache.Default;
        private readonly IMemoryCache _msCache;

        public MemoryCacheNoExpiry(IMemoryCache msCache) { _msCache = msCache; }

        public void CacheProductCatalog(string key, object catalog)
        {
            // VIOLATION cr-dotnet-0007: No expiration policy - data lives indefinitely
            _cache.Set(key, catalog, new CacheItemPolicy());
        }

        public void CacheUserPreferences(string userId, object prefs)
        {
            // VIOLATION cr-dotnet-0007: IMemoryCache.Set without expiry options
            _msCache.Set("prefs_" + userId, prefs);
        }

        public void CacheLargeDataSet(string key, byte[] data)
        {
            // VIOLATION cr-dotnet-0007: Large data cached indefinitely - memory pressure
            var policy = new CacheItemPolicy(); // AbsoluteExpiration never set
            _cache.Add(key, data, policy);
        }
    }
}
