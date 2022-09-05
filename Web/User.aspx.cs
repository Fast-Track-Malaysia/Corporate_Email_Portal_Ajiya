using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Security;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

namespace Web
{
    public partial class User : System.Web.UI.Page
    {
        static string mode;
        static string curUser;
        static string curRole;
        static int id;
        static ft_user user;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserId"] == null || Session["UserId"].ToString() == "")
                {
                    Response.Redirect("~/Default.aspx", false);
                    return;
                }

                LoadRoleCmb();
                mode = Request.QueryString["m"].ToString();
                curUser = Session["UserName"].ToString();
                curRole = Session["Role"].ToString();
                if (curRole != "1")
                {
                    Response.Redirect("~/Home.aspx", false);
                }
                switch (mode)
                {
                    case "a":
                        lblHeader.Text = "Add User";
                        txtUserID.Enabled = true;
                        chkIsActive.Checked = true;
                        break;
                    case "e":
                        lblHeader.Text = "Edit User";
                        txtUserID.Enabled = false;
                        id = int.Parse(Request.QueryString["id"].ToString());
                        GetInfo();
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
                        dat.SelectCommand.CommandText = "SELECT * FROM ft_user WHERE ID = '" + id + "' ";
                        DataTable dt = new DataTable();
                        dat.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                txtUserID.Text = row["Code"].ToString();
                                lblPassword.Text = row["Password"].ToString();
                                txtPassword.Text = row["Password"].ToString();
                                txtName.Text = row["Name"].ToString();
                                txtEmail.Text = row["Email"].ToString();

                                ddlRole.SelectedValue = row["Role"].ToString();

                                if (int.Parse(row["isActive"].ToString()) == 1)
                                {
                                    chkIsActive.Checked = true;
                                }
                                else
                                {
                                    chkIsActive.Checked = false;
                                }
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

            try
            {
                if (mode == "a")
                {
                    using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
                    {
                        using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                        {
                            dat.SelectCommand.CommandText = "SELECT * FROM ft_user WHERE Code = '" + txtUserID.Text + "'";
                            DataTable dt = new DataTable();
                            dat.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                lblMsg.Visible = true;
                                msg.Text = "This User Name already been used by other user.";
                                return;
                            }
                            dt.Dispose();
                        }
                        conn.Close();
                    }
                }

                if (txtUserID.Text == "")
                {
                    lblMsg.Visible = true;
                    msg.Text = "User Name is Required.";
                    return;
                }
                if (lblPassword.Text == "" && txtPassword.Text == "")
                {
                    lblMsg.Visible = true;
                    msg.Text = "Password is Required.";
                    return;
                }
                if (txtName.Text == "")
                {
                    lblMsg.Visible = true;
                    msg.Text = "Name is Required.";
                    return;
                }

                if (!isValidEmail(txtEmail.Text))
                {
                    lblMsg.Visible = true;
                    msg.Text = "["+ txtEmail.Text + "] is not a valid email address";
                    return;
                }

                if (mode == "a")
                {
                    string addUser = "INSERT INTO ft_user (Code, Name, Role, LastUpdateUser, LastUpdateTime, Email, Password, isActive) VALUES ('"
                        + txtUserID.Text + "', '" + txtName.Text + "', '" + ddlRole.SelectedValue.ToString() + "', '" + Session["UserId"].ToString() + "', "
                        + "'" + DateTime.Now.ToString("yyyyMMdd") + "', '" + txtEmail.Text + "'";

                    if (txtPassword.Text != "")
                    {
                        addUser += ", '" + txtPassword.Text + "'";
                    }

                    if (chkIsActive.Checked)
                    {
                        addUser += ", '1'";
                    }
                    else
                    {
                        addUser += ", '0'";
                    }

                    addUser += "); ";

                    SqlConnection conn = new SqlConnection(Session["ConnString"].ToString());
                    SqlCommand cmdAddUser = new SqlCommand(addUser, conn);

                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    conn.Open();

                    cmdAddUser.ExecuteNonQuery();
                    cmdAddUser.Dispose();
                    conn.Close();

                    msg.Text = "Record Added.";

                    Reset();
                }
                else
                {
                    string updateUser = "UPDATE T0 SET T0.[Name] = '" + txtName.Text + "', T0.[Role] = '" + ddlRole.SelectedValue.ToString() + "', " +
                        "T0.LastUpdateUser = '" + Session["UserId"].ToString() + "', T0.LastUpdateTime = '" + DateTime.Now.ToString("yyyyMMdd") + "', " +
                        "T0.Email = '" + txtEmail.Text + "' ";

                    if (txtPassword.Text != "")
                    {
                        updateUser += ", T0.Password = '" + txtPassword.Text + "'";
                    }

                    if (chkIsActive.Checked)
                    {
                        updateUser += ", isActive = '1' ";
                    }
                    else
                    {
                        updateUser += ", isActive = '0' ";
                    }

                    updateUser += "FROM ft_user T0 WHERE T0.ID = '" + id + "' ";

                    SqlConnection conn = new SqlConnection(Session["ConnString"].ToString());
                    SqlCommand cmdUpdateUser = new SqlCommand(updateUser, conn);

                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    conn.Open();

                    cmdUpdateUser.ExecuteNonQuery();
                    cmdUpdateUser.Dispose();
                    conn.Close();

                    msg.Text = "Record Updated.";
                }
                lblMsg.Visible = true;
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                msg.Text = ex.Message;
            }
        }
        protected void Reset()
        {
            txtUserID.Text = "";
            txtPassword.Text = "";
            txtName.Text = "";
            txtEmail.Text = "";
            ddlRole.SelectedIndex = -1;
            chkIsActive.Checked = true;
        }
        public static bool isValidEmail(string eMails)
        {
            eMails = Regex.Replace(eMails, @"[;|:/]", ",");
            eMails = Regex.Replace(eMails, @"[\s]", "");

            foreach (string eMail in Regex.Split(eMails, ","))
            {               
                string pattern = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
                if (!Regex.IsMatch(eMail, pattern))
                    return false;
            }
            return true;
        }
        private void LoadRoleCmb()
        {
            using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
            {
                using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                {
                    dat.SelectCommand.CommandText = "SELECT * FROM ft_role ";
                    DataTable dt = new DataTable();
                    dat.Fill(dt);

                    ddlRole.Items.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            ddlRole.Items.Add(new ListItem(row["Name"].ToString(), row["ID"].ToString()));
                        }
                    }
                    dt.Dispose();
                }
                conn.Close();
            }
        }
    }
}