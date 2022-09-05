using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class UserRolesList : System.Web.UI.Page
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
                LoadRolesList();
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

                Response.Redirect("~/UserRoles.aspx?id=" + id + "&m=e", false);
            }
            if (e.CommandName == "D")
            {
                MasterPage master = this.Master;
                HtmlGenericControl lblMsg = (HtmlGenericControl)master.FindControl("loginErrorMsgWrong");
                Label msg = (Label)master.FindControl("lblMsg");
                int i = int.Parse(e.CommandArgument.ToString());
                int id = int.Parse(((Label)gvData.Rows[i].FindControl("lblID")).Text);
                string role = gvData.Rows[i].Cells[2].Text;
                bool roleUsed = false;

                if (role == "Administrator")
                {
                    lblMsg.Visible = true;
                    msg.Text = "Administrator role cannot be deleted";
                    return;
                }

                SqlConnection conn = new SqlConnection(Session["ConnString"].ToString());

                using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                {
                    dat.SelectCommand.CommandText = "SELECT * FROM ft_user T0 WHERE T0.Role = '" + id + "' " +
                        "ORDER BY T0.ID ";
                    DataTable dt = new DataTable();
                    dat.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        roleUsed = true;
                    }
                    dt.Dispose();
                }
                conn.Close();

                if (roleUsed == true)
                {
                    lblMsg.Visible = true;
                    msg.Text = "This role cannot be deleted. Its been used.";
                    return;
                }

                string deleteUser = "DELETE FROM ft_role WHERE ID = '" + id + "'";
                SqlCommand cmdDeleteUser = new SqlCommand(deleteUser, conn);

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                cmdDeleteUser.ExecuteNonQuery();
                cmdDeleteUser.Dispose();
                conn.Close();

                LoadRolesList();

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

            Response.Redirect("~/UserRoles.aspx?m=a", false);
        }
        private void LoadRolesList()
        {
            using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
            {
                using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                {
                    dat.SelectCommand.CommandText = "SELECT * FROM ft_role T0 WHERE T0.Name <> 'Administrator' " +
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