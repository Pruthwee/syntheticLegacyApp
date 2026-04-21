// =============================================================================
// RULE ID   : cr-dotnet-0055
// RULE NAME : ActiveDirectory Dependencies
// CATEGORY  : Authentication
// DESCRIPTION: Application integrates with Active Directory through
//              System.DirectoryServices for authentication, authorization, or
//              user management. AD integration assumes on-premises infrastructure.
// =============================================================================
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace SyntheticLegacyApp.Authentication
{
    public class ActiveDirectoryDeps
    {
        private const string LdapPath = "LDAP://corp.internal/DC=corp,DC=internal";

        public bool AuthenticateWithAD(string username, string password)
        {
            // VIOLATION cr-dotnet-0055: DirectoryEntry bound to on-prem LDAP server
            try
            {
                using (var entry = new DirectoryEntry(LdapPath, username, password))
                {
                    var _ = entry.NativeObject; // throws on invalid credentials
                    return true;
                }
            }
            catch { return false; }
        }

        public string GetUserDisplayName(string samAccountName)
        {
            // VIOLATION cr-dotnet-0055: DirectorySearcher against on-prem AD
            using (var searcher = new DirectorySearcher(new DirectoryEntry(LdapPath)))
            {
                searcher.Filter = $"(&(objectClass=user)(sAMAccountName={samAccountName}))";
                searcher.PropertiesToLoad.Add("displayName");
                var result = searcher.FindOne();
                return result?.Properties["displayName"][0]?.ToString();
            }
        }

        public bool IsInADGroup(string username, string groupName)
        {
            // VIOLATION cr-dotnet-0055: PrincipalContext requires AD domain connectivity
            using (var context = new PrincipalContext(ContextType.Domain, "corp.internal"))
            using (var user    = UserPrincipal.FindByIdentity(context, username))
            using (var group   = GroupPrincipal.FindByIdentity(context, groupName))
            {
                return user?.IsMemberOf(group) ?? false;
            }
        }
    }
}
