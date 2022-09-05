using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Security;
using System.Data.SqlClient;
using System.Data;

namespace Web
{
    public partial class Users : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null || Session["UserId"].ToString() == "")
                {
                    Response.Redirect("~/Default.aspx", false);
                    return;
                }

                string curRole = Session["Role"].ToString();
                if (curRole != "1")
                {
                    Response.Redirect("~/Home.aspx", false);
                }
                LoadUsersList();
            }
        
        }
        protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                Response.Redirect("~/Default.aspx", false);
                return;
            }

            if (e.CommandName == "Select")
            {
                int i = int.Parse(e.CommandArgument.ToString());

                int id = int.Parse(((Label)gvData.Rows[i].FindControl("lblID")).Text);

                Response.Redirect("~/User.aspx?id=" + id + "&m=e", false);
            }
            if (e.CommandName == "D")
            {
                MasterPage master = this.Master;
                HtmlGenericControl lblMsg = (HtmlGenericControl)master.FindControl("loginErrorMsgWrong");
                Label msg = (Label)master.FindControl("lblMsg");
                int i = int.Parse(e.CommandArgument.ToString());

                int id = int.Parse(((Label)gvData.Rows[i].FindControl("lblID")).Text);
                string userID = gvData.Rows[i].Cells[2].Text;

                if (userID == "Admin")
                {
                    msg.Text = "Admin cannot be deleted";
                    return;
                }

                string deleteUser = "DELETE FROM ft_user WHERE ID = '" + id + "'";

                SqlConnection conn = new SqlConnection(Session["ConnString"].ToString());
                SqlCommand cmdDeleteUser = new SqlCommand(deleteUser, conn);

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                cmdDeleteUser.ExecuteNonQuery();
                cmdDeleteUser.Dispose();
                conn.Close();

                LoadUsersList();

                lblMsg.Visible = true;
                msg.Text = "Record Deleted.";
            }
        }
        protected void btnNew_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                Response.Redirect("~/Default.aspx", false);
                return;
            }

            Response.Redirect("~/User.aspx?m=a", false);
        }

        private void LoadUsersList()
        {
            using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
            {
                using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                {
                    dat.SelectCommand.CommandText = "SELECT T0.ID, T0.Code AS [UserID], T0.[Name] AS [UserName], " +
                        "CASE T0.[isActive] WHEN 1 THEN 'YES' WHEN 0 THEN 'NO' END as [Active], " +
                        "T0.Email, T1.[Name] AS [Role] " +
                        "FROM ft_user T0 " +
                        "INNER JOIN ft_role T1 ON T0.Role = T1.ID " +
                        "ORDER BY T0.ID ";
                    DataTable dt = new DataTable();
                    dat.Fill(dt);

                    gvData.DataSource = null;
                    gvData.DataBind();

                    if (dt.Rows.Count > 0)
                    {
                        gvData.DataSource = dt;
                        gvData.DataBind();
                    }
                    dt.Dispose();
                }
                conn.Close();
            }
        }
    }
}