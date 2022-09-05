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
    public partial class UserRoles : System.Web.UI.Page
    {
        private DataTable itemTable;
        static string mode;
        static int id;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserId"] == null || Session["UserId"].ToString() == "")
                {
                    Response.Redirect("~/Default.aspx", false);
                    return;
                }

                mode = Request.QueryString["m"].ToString();

                switch (mode)
                {
                    case "e":
                        id = int.Parse(Request.QueryString["id"].ToString());
                        lblHeader.Text = "Edit Role";
                        txtRoleName.Enabled = false;
                        GetInfo();
                        LoadRoleMenuList();
                        break;
                    case "a":
                        lblHeader.Text = "Add Role";
                        txtRoleName.Text = "";
                        gvData.DataSource = null;
                        gvData.DataBind();
                        break;
                }
            }
        }

        protected void GetInfo()
        {
            MasterPage master = this.Master;
            HtmlGenericControl lblMsg = (HtmlGenericControl)master.FindControl("loginErrorMsgWrong");
            Label msg = (Label)master.FindControl("lblMsg");
            lblMsg.Visible = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
                {
                    using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                    {
                        dat.SelectCommand.CommandText = "SELECT * FROM ft_role WHERE ID = '" + id + "' ";
                        DataTable dt = new DataTable();
                        dat.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                txtRoleName.Text = row["Name"].ToString();
                            }
                        }
                        dt.Dispose();
                    }
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                msg.Text = ex.Message;
            }
        }

        private void LoadRoleMenuList()
        {
            using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
            {
                using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                {
                    dat.SelectCommand.CommandText = "SELECT T0.*, T1.PageName FROM ft_rolemenu T0 " +
                        "INNER JOIN ft_menu T1 ON T0.Menu = T1.ID " +
                        "WHERE T0.Role = '" + id + "' " +
                        "ORDER BY T0.ID, T0.Menu ";
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
        protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }
        protected void btnNew_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                Response.Redirect("~/Default.aspx", false);
                return;
            }

            loadcmbMenuItem();
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "ShowPopup();", true);
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                Response.Redirect("~/Default.aspx", false);
                return;
            }

            MasterPage master = this.Master;
            HtmlGenericControl lblMsg = (HtmlGenericControl)master.FindControl("loginErrorMsgWrong");
            Label msg = (Label)master.FindControl("lblMsg");

            if (mode == "a")
            {
                using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
                {
                    using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                    {
                        dat.SelectCommand.CommandText = "SELECT T0.* FROM ft_role T0 WHERE T0.Name = '" + txtRoleName.Text.ToString() + "' ";
                        DataTable dt = new DataTable();
                        dat.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            lblMsg.Visible = true;
                            msg.Text = "Error: Duplicate role detected.";
                            return;
                        }
                        dt.Dispose();
                    }

                    using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                    {
                        string addRole = "INSERT INTO ft_role (Name, LastUpdateUser, LastUpdateTime) " +
                            "VALUES ('" + txtRoleName.Text.ToString() + "', '" + Session["UserId"].ToString() + "', '" + DateTime.Now.ToString("yyyyMMdd") + "'); ";
                        SqlCommand cmdAddRole = new SqlCommand(addRole, conn);

                        if (conn.State == ConnectionState.Open)
                            conn.Close();

                        conn.Open();
                        cmdAddRole.ExecuteNonQuery();
                        cmdAddRole.Dispose();
                    }

                    using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                    {
                        dat.SelectCommand.CommandText = "SELECT T0.* FROM ft_role T0 WHERE T0.Name = '" + txtRoleName.Text.ToString() + "' ";
                        DataTable dt = new DataTable();
                        dat.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                id = int.Parse(row["ID"].ToString());
                            }
                        }
                        dt.Dispose();
                    }

                    for (int i = 0; i < gvData.Rows.Count; i++)
                    {
                        string rolemenuid = (gvData.Rows[i].FindControl("lblID") as Label).Text;
                        string menuid = (gvData.Rows[i].FindControl("lblMenu") as Label).Text;

                        if (rolemenuid != "-1") continue;

                        using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                        {
                            string addRole = "INSERT INTO ft_rolemenu (Role, Menu) " +
                                "VALUES ('" + id + "', '" + menuid + "'); ";
                            SqlCommand cmdAddRole = new SqlCommand(addRole, conn);

                            if (conn.State == ConnectionState.Open)
                                conn.Close();

                            conn.Open();
                            cmdAddRole.ExecuteNonQuery();
                            cmdAddRole.Dispose();
                        }

                    }
                    conn.Close();
                }

                lblMsg.Visible = true;
                msg.Text = "Record Updated.";
            }

            if (mode == "e")
            {
                using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
                {
                    for (int i = 0; i < gvData.Rows.Count; i++)
                    {
                        string rolemenuid = (gvData.Rows[i].FindControl("lblID") as Label).Text;
                        string menuid = (gvData.Rows[i].FindControl("lblMenu") as Label).Text;

                        if (rolemenuid != "-1") continue;

                        using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                        {
                            string addRole = "INSERT INTO ft_rolemenu (Role, Menu) " +
                                "VALUES ('" + id + "', '" + menuid + "'); ";
                            SqlCommand cmdAddRole = new SqlCommand(addRole, conn);

                            if (conn.State == ConnectionState.Open)
                                conn.Close();

                            conn.Open();
                            cmdAddRole.ExecuteNonQuery();
                            cmdAddRole.Dispose();
                        }
                        conn.Close();
                    }
                }

                lblMsg.Visible = true;
                msg.Text = "Record Updated.";
            }
        }
        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            if (cmbMenuItem.SelectedItem == null)
                return;
            if (itemTable == null)
            {
                itemTable = new DataTable();
                itemTable.Columns.Add("ID", typeof(string));
                itemTable.Columns.Add("Menu", typeof(string));
                itemTable.Columns.Add("PageName", typeof(string));

                for (int i = 0; i < gvData.Rows.Count; i++)
                {
                    DataRow dr = itemTable.NewRow();
                    dr["ID"] = (gvData.Rows[i].FindControl("lblID") as Label).Text;
                    dr["Menu"] = (gvData.Rows[i].FindControl("lblMenu") as Label).Text;
                    dr["PageName"] = (gvData.Rows[i].FindControl("lblPageName") as Label).Text;
                    itemTable.Rows.Add(dr);
                }
                DataRow dr2 = itemTable.NewRow();
                dr2["ID"] = "-1";
                dr2["Menu"] = cmbMenuItem.SelectedValue.ToString();
                dr2["PageName"] = cmbMenuItem.SelectedItem.Text.ToString();
                itemTable.Rows.Add(dr2);
                gvData.DataSource = itemTable;
                gvData.DataBind();
            }
        }

        private void loadcmbMenuItem()
        {
            cmbMenuItem.Items.Clear();
            using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
            {
                using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                {
                    dat.SelectCommand.CommandText = "SELECT T0.* FROM ft_menu T0 " +
                        "WHERE T0.ID NOT IN ( SELECT Menu FROM ft_vw_usermenu WHERE Role = '" + id + "') " +
                        "AND T0.PageType = 'C' ";
                    DataTable dt = new DataTable();
                    dat.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            bool isExist = false;
                            for (int i = 0; i < gvData.Rows.Count; i++)
                            {
                                if (row["PageName"].ToString() == (gvData.Rows[i].FindControl("lblPageName") as Label).Text)
                                {
                                    isExist = true;
                                    continue;
                                }
                            }

                            if (isExist == true) continue;
                            else
                                cmbMenuItem.Items.Add(new ListItem(row["PageName"].ToString(), row["ID"].ToString()));
                        }
                    }
                    dt.Dispose();
                }
                conn.Close();
            }
        }

    }
}