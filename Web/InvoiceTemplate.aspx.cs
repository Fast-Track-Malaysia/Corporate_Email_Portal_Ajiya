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
    public partial class InvoiceTemplate : System.Web.UI.Page
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

                lblUser.Text = Session["UserId"].ToString();

                MasterPage master = this.Master;
                HtmlGenericControl lblMsg = (HtmlGenericControl)master.FindControl("loginErrorMsgWrong");
                Label msg = (Label)master.FindControl("lblMsg");
                lblMsg.Visible = false;

                LoadEmailTemplate();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                Response.Redirect("~/Default.aspx", false);
                return;
            }

            MasterPage master = this.Master;
            HtmlGenericControl lblMsg = (HtmlGenericControl)master.FindControl("loginErrorMsgWrong");
            Label msg = (Label)master.FindControl("lblMsg");
            lblMsg.Visible = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
                {
                    if (Mode.Text == "Add")
                    {
                        using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                        {
                            dat.SelectCommand.CommandText = "SELECT * FROM ft_email_template_invoice WHERE Format = '" + txtFormat.Text + "'";
                            DataTable dt = new DataTable();
                            dat.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                lblMsg.Visible = true;
                                msg.Text = "Duplicated format.";
                                ResetTemplate();
                                return;
                            }
                            dt.Dispose();
                        }

                        string addTemplate = "INSERT INTO ft_email_template_invoice (Format, EmailSubject, EmailContent, LastUpdateUser, LastUpdateTime, isDefault) VALUES ('" +
                            txtFormat.Text + "', '" + txtSubject.Text.Replace("'", "") + "', '" + EmailEditor.Content.Replace("'", "") + "', '"
                            + Session["UserId"].ToString() + "','" + DateTime.Now.ToString("yyyyMMdd") + "', '0'); ";

                        SqlCommand cmdAddTemplate = new SqlCommand(addTemplate, conn);

                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        cmdAddTemplate.ExecuteNonQuery();
                        cmdAddTemplate.Dispose();
                    }

                    if (Mode.Text == "Edit")
                    {
                        string updateTemplate = "UPDATE ft_email_template_invoice SET EmailSubject = '" + txtSubject.Text.Replace("'", "") + "', EmailContent ='" + EmailEditor.Content.Replace("'", "") +
                            "', LastUpdateUser = '" + Session["UserId"].ToString() + "', LastUpdateTime = '" + DateTime.Now.ToString("yyyyMMdd") + "' WHERE ID = '" + ID.Text + "'";

                        SqlCommand cmdupdateTemplate = new SqlCommand(updateTemplate, conn);
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        cmdupdateTemplate.ExecuteNonQuery();
                        cmdupdateTemplate.Dispose();
                        conn.Close();
                    }

                    if (chkIsDefault.Checked)
                    {
                        string setDefTemplate = "UPDATE ft_email_template_invoice SET isDefault = '0' " +
                            "UPDATE ft_email_template_invoice SET isDefault = '1' WHERE Format = '" + txtFormat.Text + "'";

                        SqlCommand cmdsetDefTemplate = new SqlCommand(setDefTemplate, conn);

                        cmdsetDefTemplate.ExecuteNonQuery();
                        cmdsetDefTemplate.Dispose();
                    }

                    conn.Close();
                }

                lblMsg.Visible = true;
                msg.Text = "Email template saved.";

                Response.Redirect("InvoiceTemplate.aspx");
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                msg.Text = ex.Message;
                ResetTemplate();
            }
        }
        protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                Response.Redirect("~/Default.aspx", false);
                return;
            }

            MasterPage master = this.Master;
            HtmlGenericControl lblMsg = (HtmlGenericControl)master.FindControl("loginErrorMsgWrong");
            Label msg = (Label)master.FindControl("lblMsg");
            lblMsg.Visible = false;

            int i = int.Parse(e.CommandArgument.ToString());
            int id = int.Parse(((Label)GridView1.Rows[i].FindControl("lblID")).Text);

            try
            {
                if (e.CommandName == "E")
                {
                    Mode.Text = "Edit";
                    txtFormat.Enabled = false;
                    ID.Text = id.ToString();

                    using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
                    {
                        using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                        {
                            dat.SelectCommand.CommandText = "SELECT * FROM ft_email_template_invoice WHERE ID = '" + id + "' ";
                            DataTable dt = new DataTable();
                            dat.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    txtFormat.Text = row["Format"].ToString();
                                    txtSubject.Text = row["EmailSubject"].ToString();
                                    EmailEditor.Content = row["EmailContent"].ToString();

                                    if (int.Parse(row["isDefault"].ToString()) == 1)
                                    {
                                        chkIsDefault.Checked = true;
                                    }
                                    else
                                    {
                                        chkIsDefault.Checked = false;
                                    }
                                }
                            }
                            dt.Dispose();
                        }
                        conn.Close();
                    }

                    this.pnlPopUp_ModalPopupExtender.Show();
                }
                if (e.CommandName == "D")
                {
                    string deleteTemplate = "DELETE FROM ft_email_template_invoice WHERE ID = '" + id + "'";

                    SqlConnection conn = new SqlConnection(Session["ConnString"].ToString());
                    SqlCommand cmdDeleteTemplate = new SqlCommand(deleteTemplate, conn);

                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    conn.Open();

                    cmdDeleteTemplate.ExecuteNonQuery();
                    cmdDeleteTemplate.Dispose();
                    conn.Close();

                    Response.Redirect("InvoiceTemplate.aspx");
                }
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                msg.Text = ex.Message;
                ResetTemplate();
            }
        }
        protected void btnNew_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                Response.Redirect("~/Default.aspx", false);
                return;
            }

            Button btndetails = sender as Button;
            Mode.Text = "Add";
            txtFormat.Enabled = true;
            this.pnlPopUp_ModalPopupExtender.Show();
        }
        protected void LoadEmailTemplate()
        {
            using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
            {
                using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                {
                    dat.SelectCommand.CommandText = "SELECT T0.ID, T0.Format, T0.EmailSubject, T0.EmailContent, " +
                        "CASE WHEN T0.isDefault = '1' THEN 'Yes' ELSE 'No' END AS [isDefault], T0.LastUpdateUser, T0.LastUpdateTime " +
                        "FROM ft_email_template_invoice T0 ORDER BY T0.ID ";
                    DataTable dt = new DataTable();
                    dat.Fill(dt);

                    GridView1.DataSource = null;
                    GridView1.DataBind();

                    if (dt.Rows.Count > 0)
                    {
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                    }
                    dt.Dispose();
                }
                conn.Close();
            }
        }
        protected void ResetTemplate()
        {
            txtFormat.Text = "";
            txtSubject.Text = "";
            EmailEditor.Content = "";
            chkIsDefault.Checked = false;
        }
    }
}