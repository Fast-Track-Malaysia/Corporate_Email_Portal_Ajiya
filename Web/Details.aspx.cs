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
    public partial class Details : System.Web.UI.Page
    {
        static ft_user user;
        static string curUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                MasterPage master = this.Master;
                HtmlGenericControl lblMsg = (HtmlGenericControl)master.FindControl("loginErrorMsgWrong");
                Label msg = (Label)master.FindControl("lblMsg");
                lblMsg.Visible = false;
                try
                {
                    LoadRoleCmb();
                    GetInfo();

                }
                catch (Exception ex)
                {
                    lblMsg.Visible = true;
                    msg.Text = ex.Message;
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            MasterPage master = this.Master;
            HtmlGenericControl lblMsg = (HtmlGenericControl)master.FindControl("loginErrorMsgWrong");
            Label msg = (Label)master.FindControl("lblMsg");
            try
            {
                user.Name = txtName.Text;
                if (txtPassword.Text != "")
                {
                    user.Password = txtPassword.Text;
                }
                user.LastUpdateUser = curUser;
                user.LastUpdateTime = DateTime.Now;
                if (!isValidEmail(txtEmail.Text))
                {
                    lblMsg.Visible = true;
                    msg.Text = "[" + txtEmail.Text + "] is not a valid email address";
                    return;
                }
                user.Email = txtEmail.Text;
                user.Save();
                lblMsg.Visible = true;
                msg.Text = "Record Updated.";
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                msg.Text = ex.Message;
            }


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
                        dat.SelectCommand.CommandText = "SELECT T0.*, T1.Name AS [RoleName] FROM ft_user T0 INNER JOIN ft_role T1 ON T0.Role = T1.ID " +
                            "WHERE T0.Code = '" + Session["UserId"].ToString() + "' ";
                        DataTable dt = new DataTable();
                        dat.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                lblUserID.Text = row["Code"].ToString();
                                txtPassword.Text = row["Password"].ToString();
                                txtName.Text = row["Name"].ToString();
                                txtEmail.Text = row["Email"].ToString();

                                ddlRole.SelectedValue = row["Role"].ToString();
                                lblRole.Text = row["RoleName"].ToString();
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