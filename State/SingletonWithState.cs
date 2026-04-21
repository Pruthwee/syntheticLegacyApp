// =============================================================================
// RULE ID   : cr-dotnet-0008
// RULE NAME : Singleton Pattern with State
// CATEGORY  : State Management
// DESCRIPTION: Application implements singleton with mutable internal state using
//              static properties or fields. In multi-instance cloud deployments
//              each instance creates its own singleton, violating the contract.
// =============================================================================
using System.Collections.Generic;

namespace SyntheticLegacyApp.State
{
    public class ApplicationConfigSingleton
    {
        // VIOLATION cr-dotnet-0008: Classic thread-safe singleton with mutable state
        private static ApplicationConfigSingleton _instance;
        private static readonly object _lock = new object();

        // VIOLATION cr-dotnet-0008: Mutable fields - diverge per-instance in cloud
        private Dictionary<string, string> _runtimeSettings = new Dictionary<string, string>();
        private List<string> _featureFlags = new List<string>();
        private int _activeUserCount = 0;

        private ApplicationConfigSingleton() { }

        public static ApplicationConfigSingleton Instance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new ApplicationConfigSingleton();
                return _instance;
            }
        }

        public void SetSetting(string key, string value)
        {
            _runtimeSettings[key] = value; // VIOLATION cr-dotnet-0008
        }

        public void IncrementUserCount() => _activeUserCount++; // VIOLATION cr-dotnet-0008
        public int  GetUserCount()       => _activeUserCount;
    }
}
