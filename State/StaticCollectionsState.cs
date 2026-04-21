// =============================================================================
// RULE ID   : cr-dotnet-0006
// RULE NAME : Static Collections for State
// CATEGORY  : State Management
// DESCRIPTION: Application uses static collections to store mutable application
//              state. In multi-instance cloud deployments each instance maintains
//              separate static state, causing data inconsistency.
// =============================================================================
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SyntheticLegacyApp.State
{
    public class StaticCollectionsState
    {
        // VIOLATION cr-dotnet-0006: Static Dictionary storing mutable per-server state
        private static readonly Dictionary<string, string> _userSessions
            = new Dictionary<string, string>();

        // VIOLATION cr-dotnet-0006: Static List as in-process data store
        private static readonly List<string> _activeOrders
            = new List<string>();

        // VIOLATION cr-dotnet-0006: ConcurrentDictionary - still per-instance
        private static readonly ConcurrentDictionary<string, int> _productInventory
            = new ConcurrentDictionary<string, int>();

        public void AddUserSession(string sessionId, string userId)
        {
            lock (_userSessions) { _userSessions[sessionId] = userId; }
        }

        public void AddOrder(string orderId)
        {
            lock (_activeOrders) { _activeOrders.Add(orderId); }
        }

        public void UpdateInventory(string productId, int quantity)
        {
            // VIOLATION cr-dotnet-0006: Per-instance inventory count - diverges across pods
            _productInventory[productId] = quantity;
        }
    }
}
