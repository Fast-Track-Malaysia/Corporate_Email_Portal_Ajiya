using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                Response.Redirect("~/Default.aspx", false);
                return;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                Response.Redirect("~/Default.aspx", false);
                return;
            }

            if (!Page.IsPostBack)
            {
                string curRole = Session["Role"].ToString();
                if (Session["CompnyCode"].ToString() != null)
                {
                    Page.Title = Session["CompnyName"].ToString();
                }

                if (curRole == "1")
                {
                    liAdmin.Visible = true;
                    Users.Visible = true;
                    Roles.Visible = true;
                    li_EmailSettings.Visible = true;
                    StatementTemplate.Visible = true;
                    InvoiceTemplate.Visible = true;
                    ReceiptTemplate.Visible = true;
                    ReminderTemplate.Visible = true;
                    liBP.Visible = true;
                    liBP01.Visible = true;
                    EmailStatement.Visible = true;
                    EmailInvoice.Visible = true;
                    EmailReceipt.Visible = true;
                    EmailReminder.Visible = true;
                    liLogs.Visible = true;
                    StatementLogs.Visible = true;
                    InvoiceLogs.Visible = true;
                    ReceiptLogs.Visible = true;
                    ReminderLogs.Visible = true;
                    GeneralLogs.Visible = true;
                }
                else
                {
                    string query = "";
                    using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
                    {
                        using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                        {
                            query = "SELECT * FROM ft_vw_usermenu T0 WHERE T0.Role = '" + Session["Role"].ToString() + "' ORDER BY T0.ID ";

                            dat.SelectCommand.CommandText = query;

                            DataTable dt = new DataTable();
                            dat.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    switch (row["PageCode"].ToString())
                                    {
                                        case "liAdmin":
                                            liAdmin.Visible = true;
                                            break;
                                        case "Users":
                                            Users.Visible = true;
                                            break;
                                        case "li_EmailSettings":
                                            li_EmailSettings.Visible = true;
                                            break;
                                        case "StatementTemplate":
                                            StatementTemplate.Visible = true;
                                            break;
                                        case "InvoiceTemplate":
                                            InvoiceTemplate.Visible = true;
                                            break;
                                        case "ReceiptTemplate":
                                            ReceiptTemplate.Visible = true;
                                            break;
                                        case "ReminderTemplate":
                                            ReminderTemplate.Visible = true;
                                            break;
                                        case "liBP":
                                            liBP.Visible = true;
                                            break;
                                        case "liBP01":
                                            liBP01.Visible = true;
                                            break;
                                        case "EmailStatement":
                                            EmailStatement.Visible = true;
                                            break;
                                        case "EmailInvoice":
                                            EmailInvoice.Visible = true;
                                            break;
                                        case "EmailReceipt":
                                            EmailReceipt.Visible = true;
                                            break;
                                        case "EmailReminder":
                                            EmailReminder.Visible = true;
                                            break;
                                        case "liLogs":
                                            liLogs.Visible = true;
                                            break;
                                        case "StatementLogs":
                                            StatementLogs.Visible = true;
                                            break;
                                        case "InvoiceLogs":
                                            InvoiceLogs.Visible = true;
                                            break;
                                        case "ReceiptLogs":
                                            ReceiptLogs.Visible = true;
                                            break;
                                        case "ReminderLogs":
                                            ReminderLogs.Visible = true;
                                            break;
                                        case "GeneralLogs":
                                            GeneralLogs.Visible = true;
                                            break;
                                    }
                                }
                            }
                            dt.Dispose();
                        }
                        conn.Close();
                    }
                }
            }
        }
        protected void btnProfile_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Details.aspx");
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session["CompnyCode"] = null;
            Session["CompnyName"] = null;
            Session["UserId"] = null;
            Session["UserName"] = null;
            Session["Email"] = null;
            Session["Role"] = null;
            Session["ConnectedDB"] = null;
            Session["ConnString"] = null;
            Session["SAPDB"] = null;
            Session["SAPDBConnString"] = null;
            Session["AjiyaDBConnString"] = null;
            Response.Redirect("~/Default.aspx");
        }
    }
}