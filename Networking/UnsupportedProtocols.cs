// =============================================================================
// RULE ID   : cr-dotnet-0125
// RULE NAME : Unsupported Protocols
// CATEGORY  : Networking
// DESCRIPTION: Application depends on legacy network protocols like FTP through
//              FtpWebRequest, SMB/CIFS file shares via UNC paths, or other protocols
//              that may not be supported in cloud networking environments.
// =============================================================================
using System.IO;
using System.Net;

namespace SyntheticLegacyApp.Networking
{
    public class UnsupportedProtocols
    {
        public void UploadReportViaFTP(string localFilePath, string remoteFileName)
        {
            // VIOLATION cr-dotnet-0125: FTP - blocked in most cloud networking configurations
            var request = (FtpWebRequest)WebRequest.Create(
                "ftp://reports-ftp.corp.internal/reports/" + remoteFileName);
            request.Method      = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential("ftpuser", "Ftp!Pass2024");

            byte[] fileContents = File.ReadAllBytes(localFilePath);
            using (var reqStream = request.GetRequestStream())
                reqStream.Write(fileContents, 0, fileContents.Length);
        }

        public void ReadFromSMBShare(string filename)
        {
            // VIOLATION cr-dotnet-0125: UNC path (SMB/CIFS) - not available in cloud VPC
            string uncPath = @"\fileserver.corp.internal\SharedDocs" + filename;
            File.ReadAllText(uncPath);
        }

        public void CopyToNetworkShare(string localFile)
        {
            // VIOLATION cr-dotnet-0125: Mapping to Windows network share
            string uncDest = @"\nas01.corp.localackups" + Path.GetFileName(localFile);
            File.Copy(localFile, uncDest);
        }
    }
}
