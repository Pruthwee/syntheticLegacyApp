// =============================================================================
// RULE ID   : cr-dotnet-0023
// RULE NAME : ThreadStatic Attribute
// CATEGORY  : Threading
// DESCRIPTION: Application uses [ThreadStatic] attribute to store thread-local
//              data. In cloud hosting with thread pooling and request recycling,
//              ThreadStatic variables cause memory leaks and unpredictable behavior.
// =============================================================================
using System;
using System.Collections.Generic;

namespace SyntheticLegacyApp.Threading
{
    public class ThreadStaticUsage
    {
        // VIOLATION cr-dotnet-0023: ThreadStatic for per-request context storage
        [ThreadStatic]
        private static string _currentUserId;

        [ThreadStatic]
        private static Dictionary<string, string> _requestContext;

        [ThreadStatic]
        private static Guid _correlationId;

        public static void SetCurrentUser(string userId)
        {
            // VIOLATION cr-dotnet-0023: Thread-local state reused across pooled requests
            _currentUserId = userId;
        }

        public static string GetCurrentUser() => _currentUserId;

        public static void InitRequestContext()
        {
            // VIOLATION cr-dotnet-0023: Dictionary in ThreadStatic - may be dirty on reuse
            _requestContext = new Dictionary<string, string>();
            _correlationId  = Guid.NewGuid();
        }

        public static void AddContextValue(string key, string value)
        {
            _requestContext[key] = value; // VIOLATION cr-dotnet-0023
        }
    }
}
