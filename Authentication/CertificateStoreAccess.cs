// =============================================================================
// RULE ID   : cr-dotnet-0032
// RULE NAME : Certificate Store Access
// CATEGORY  : Authentication
// DESCRIPTION: Application accesses the Windows Certificate Store using X509Store
//              or certificate management APIs. Windows Certificate Store does not
//              exist on non-Windows cloud platforms, causing certificate load failures.
// =============================================================================
using System.Security.Cryptography.X509Certificates;

namespace SyntheticLegacyApp.Authentication
{
    public class CertificateStoreAccess
    {
        public X509Certificate2 LoadServiceCertificate(string thumbprint)
        {
            // VIOLATION cr-dotnet-0032: Opening Windows LocalMachine Certificate Store
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(
                    X509FindType.FindByThumbprint, thumbprint, validOnly: true);
                return certs.Count > 0 ? certs[0] : null;
            }
        }

        public void InstallCertificate(byte[] certBytes, string password)
        {
            // VIOLATION cr-dotnet-0032: Installing cert into Windows store
            var cert = new X509Certificate2(certBytes, password,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            using (var store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);
            }
        }

        public bool ValidateBySubject(string subjectName)
        {
            // VIOLATION cr-dotnet-0032: LocalMachine Root store - does not exist in containers
            using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                return store.Certificates.Find(
                    X509FindType.FindBySubjectName, subjectName, true).Count > 0;
            }
        }
    }
}
