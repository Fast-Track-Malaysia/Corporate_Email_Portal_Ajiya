using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Security;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblLive.Text = "Live : " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                
                LoadCompanyList();
                ddlCompany.SelectedValue = WebConfigurationManager.AppSettings.Get("DefaultDB");
            }
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
            string pc = Environment.MachineName;
            string browser = bc.Browser + " " + bc.Version;
            string ip = Request.UserHostAddress;
            CurrentLoggedUserDetails user = new CurrentLoggedUserDetails();
            try
            {
                loginErrorMsgWrong.Visible = false;

                using (SqlConnection conn = new SqlConnection(AppUtilities.getCommonDBConnString()))
                {
                    using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                    {
                        dat.SelectCommand.CommandText = "SELECT T0.*, T1.* " +
                            "FROM " + ddlCompany.SelectedValue.ToString() + "..ft_user T0 " +
                            "INNER JOIN ODBC T1 ON T1.PortalDB = '" + ddlCompany.SelectedValue.ToString() + "'" +
                            "WHERE T0.Code = '" + UserId.Value.ToString() + "' AND T0.Password = '" + password.Value.ToString() + "' AND T0.isActive = '1' ";

                        DataTable dt = new DataTable();
                        dat.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                Session["CompnyCode"] = row["CompnyCode"].ToString();
                                Session["CompnyName"] = row["CompnyName"].ToString();
                                Session["UserId"] = row["Code"].ToString();
                                Session["UserName"] = row["Name"].ToString();
                                Session["Email"] = row["Email"].ToString();
                                Session["Role"] = int.Parse(row["Role"].ToString());
                                Session["ConnectedDB"] = row["PortalDB"].ToString();
                                Session["ConnString"] = row["PortalDBConnString"].ToString();
                                Session["SAPDB"] = row["SAPDBName"].ToString();
                                Session["SAPDBConnString"] = row["SAPDBConnString"].ToString();
                                Session["AjiyaDBConnString"] = row["AjiyaPortalDBConnString"].ToString();

                                user.CompnyCode = row["CompnyCode"].ToString();
                                user.CompnyName = row["CompnyName"].ToString();
                                user.UserId = row["Code"].ToString();
                                user.UserName = row["Name"].ToString();
                                user.Email = row["Email"].ToString();
                                user.Role = int.Parse(row["Role"].ToString());
                                user.ConnectedDB = row["PortalDB"].ToString();
                                user.ConnString = row["PortalDBConnString"].ToString();
                                user.SAPDB = row["SAPDBName"].ToString();
                                user.SAPDBConnString = row["SAPDBConnString"].ToString();
                                user.AjiyaDBConnString = row["AjiyaPortalDBConnString"].ToString();

                                FormsAuthentication.RedirectFromLoginPage(user.UserId, true);

                                ft_logs.WriteLine("Visitor:" + Application["OnlineVisitors"] + 
                                    " Ip:[" + ip + "] PC:[" + pc + "] Browser:[" + browser + "] " +
                                    "Logged In as User:[" + user.UserId + "]", "Session_Start");
                            }
                        }
                        else
                        {
                            loginErrorMsgWrong.Visible = true;
                            lblMsg.Text = "Your login attempt has failed. Please try again.";
                            ft_logs.WriteLine("User:[" + UserId + "] Ip:[" + ip + "] PC:[" + pc + "] Browser:[" + browser + "] user not exists", "Security Warning");
                        }
                        dt.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                loginErrorMsgWrong.Visible = true;
                lblMsg.Text = ex.Message;
            }
        }

        private void LoadCompanyList()
        { 
            using (SqlConnection conn = new SqlConnection(AppUtilities.getCommonDBConnString()))
            {
                using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                {
                    dat.SelectCommand.CommandText = "SELECT * FROM ODBC";
                    DataTable dt = new DataTable();
                    dat.Fill(dt);

                    ddlCompany.Items.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            ddlCompany.Items.Add(new ListItem(row["CompnyCode"].ToString() + " - " + row["CompnyName"].ToString(), row["PortalDB"].ToString()));
                        }
                    }
                    dt.Dispose();
                }
                conn.Close();
            }
        }
    }
}