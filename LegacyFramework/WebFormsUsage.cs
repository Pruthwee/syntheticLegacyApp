// =============================================================================
// RULE ID   : cr-dotnet-0026
// RULE NAME : Web Forms Usage
// CATEGORY  : LegacyFramework
// DESCRIPTION: Application uses ASP.NET Web Forms which has architectural patterns
//              that create scalability challenges and heavy resource requirements.
//              This reduces deployment efficiency and horizontal scaling capabilities
//              in cloud environments.
// =============================================================================

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SyntheticLegacyApp.LegacyFramework
{
    // VIOLATION cr-dotnet-0026: Inherits from System.Web.UI.Page — ASP.NET Web Forms page
    public class OrderDashboard : Page
    {
        // VIOLATION cr-dotnet-0026: Server-side controls declared as fields (code-behind model)
        protected GridView  gvOrders;
        protected Label     lblStatus;
        protected Button    btnRefresh;
        protected TextBox   txtSearch;
        protected DropDownList ddlStatus;

        // VIOLATION cr-dotnet-0026: Page_Load is Web Forms lifecycle event — not cloud-portable
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindOrderGrid();
                PopulateStatusDropdown();
            }
        }

        // VIOLATION cr-dotnet-0026: ViewState dependency — large server-side state bloat
        private void BindOrderGrid()
        {
            // ViewState stores entire grid state on each round-trip
            ViewState["LastBoundTime"] = DateTime.UtcNow;

            gvOrders.DataSource = GetOrderData();
            gvOrders.DataBind();
            lblStatus.Text = "Orders loaded.";
        }

        private void PopulateStatusDropdown()
        {
            ddlStatus.Items.Add(new ListItem("All",      ""));
            ddlStatus.Items.Add(new ListItem("Pending",  "P"));
            ddlStatus.Items.Add(new ListItem("Complete", "C"));
        }

        // VIOLATION cr-dotnet-0026: Server-side button click handler — PostBack round-trip
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            BindOrderGrid();
        }

        // VIOLATION cr-dotnet-0026: GridView row command — tightly coupled to Web Forms events
        protected void gvOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetail")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"OrderDetail.aspx?id={orderId}");
            }
        }

        // VIOLATION cr-dotnet-0026: Session used to carry state between Web Forms pages
        protected void SaveSearchCriteria()
        {
            Session["SearchQuery"]  = txtSearch.Text;
            Session["StatusFilter"] = ddlStatus.SelectedValue;
        }

        private object GetOrderData() => new object[0]; // stub
    }

    // VIOLATION cr-dotnet-0026: UserControl — Web Forms component model
    public class OrderSummaryControl : UserControl
    {
        protected Label lblTotal;
        protected Label lblCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblTotal.Text = "$0.00";
            lblCount.Text = "0 orders";
        }
    }
}
