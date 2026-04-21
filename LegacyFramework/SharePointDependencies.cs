// =============================================================================
// RULE ID   : cr-dotnet-0058
// RULE NAME : SharePoint Dependencies
// CATEGORY  : LegacyFramework
// DESCRIPTION: Application depends on Microsoft SharePoint APIs through
//              Microsoft.SharePoint assemblies for document management or workflow
//              features. SharePoint APIs assume on-premises SharePoint deployment
//              and don't work in cloud-native applications.
// =============================================================================

using System;
using System.IO;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;

namespace SyntheticLegacyApp.LegacyFramework
{
    public class SharePointDocumentManager
    {
        // VIOLATION cr-dotnet-0058: SPSite — on-premises SharePoint server object model
        private readonly string _siteUrl = "http://intranet.corp.local/sites/orders";

        // VIOLATION cr-dotnet-0058: Opening SPSite by URL — requires local SharePoint install
        public void UploadOrderDocument(string fileName, byte[] content)
        {
            using (SPSite site = new SPSite(_siteUrl))
            using (SPWeb web  = site.OpenWeb())
            {
                // VIOLATION cr-dotnet-0058: SPList — SharePoint list object model
                SPList docLibrary = web.Lists["OrderDocuments"];

                // VIOLATION cr-dotnet-0058: SPFileCollection.Add — server-side file upload
                SPFolder rootFolder = docLibrary.RootFolder;
                rootFolder.Files.Add(fileName, content, overwrite: true);
                rootFolder.Update();

                Console.WriteLine($"Uploaded {fileName} to SharePoint library.");
            }
        }

        // VIOLATION cr-dotnet-0058: Querying SharePoint list with CAML — server-side model
        public void ListRecentOrders(int topN)
        {
            using (SPSite site = new SPSite(_siteUrl))
            using (SPWeb web  = site.OpenWeb())
            {
                SPList list = web.Lists["Orders"];

                SPQuery query = new SPQuery
                {
                    Query = $@"<OrderBy><FieldRef Name='Created' Ascending='FALSE'/></OrderBy>",
                    RowLimit = (uint)topN
                };

                // VIOLATION cr-dotnet-0058: GetItems — executes CAML against SP content DB
                SPListItemCollection items = list.GetItems(query);

                foreach (SPListItem item in items)
                    Console.WriteLine($"Order: {item["Title"]} — {item["Created"]}");
            }
        }

        // VIOLATION cr-dotnet-0058: Starting a SharePoint workflow — on-prem SPWorkflowManager
        public void StartApprovalWorkflow(int orderId)
        {
            using (SPSite site = new SPSite(_siteUrl))
            using (SPWeb web  = site.OpenWeb())
            {
                SPList list = web.Lists["Orders"];
                SPListItem item = list.GetItemById(orderId);

                // VIOLATION cr-dotnet-0058: SPWorkflowManager — SharePoint workflow engine
                SPWorkflowManager wfManager = site.WorkflowManager;
                SPWorkflowAssociation wfAssoc =
                    list.WorkflowAssociations.GetAssociationByName(
                        "OrderApprovalWorkflow",
                        System.Globalization.CultureInfo.InvariantCulture);

                wfManager.StartWorkflow(item, wfAssoc, wfAssoc.AssociationData);
                Console.WriteLine($"Approval workflow started for order {orderId}.");
            }
        }
    }
}
