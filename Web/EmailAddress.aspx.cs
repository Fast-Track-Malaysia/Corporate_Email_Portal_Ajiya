using ClosedXML.Excel;
using Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace Web
{
    public partial class EmailAddress : System.Web.UI.Page
    {
        private static int TotalRecords = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblUser.Text = Session["CurUser"].ToString();
                string curRole = Session["CurRole"].ToString();
                if (curRole != "1" && curRole != "2")
                {
                    Response.Redirect("~/Home.aspx", false);
                }
            }
            SqlDataSource.SelectParameters.Clear();
            SqlDataSource.SelectParameters.Add("DBName", Session["CurCompany"].ToString());
        }
        protected void SqlDataSource_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            /* Get Total number of records */
            int First = (GridView1.PageIndex * GridView1.PageSize) + 1;
            int Last = First + GridView1.PageSize - 1;
            String Tag = " " + First + " - " + Last + " of " + TotalRecords + " items";
            Label3.Text = Tag;
            //Label4.Text = GridView1.PageSize.ToString();
        }
        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            GridView1.AllowPaging = false;
            TotalRecords = GridView1.Rows.Count;
            GridView1.AllowPaging = true;
        }
        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                DropDownList DDL = new DropDownList();
                DDL.Items.Add("5");
                DDL.Items.Add("10");
                DDL.Items.Add("15");
                DDL.Items.Add("20");
                DDL.Items.Add("100");
                DDL.AutoPostBack = true;
                DDL.SelectedValue = GridView1.PageSize.ToString();
                DDL.SelectedIndexChanged += DDL_SelectedIndexChanged;

                Label Label = new Label();
                Label.Text = Label3.Text;
                Label.Attributes.Add("class", "pager-info");

                System.Web.UI.WebControls.Table table = e.Row.Cells[0].Controls[0] as System.Web.UI.WebControls.Table;
                table.Width = Unit.Percentage(100);
                TableCell Cell = new TableCell();
                Cell.Controls.Add(DDL);
                Cell.Controls.Add(Label);

                table.Rows[0].Cells.Add(Cell);
            }
        }
        protected void DDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* Allocate Pages Size from Dropdown */
            DropDownList DDL = (DropDownList)sender;
            GridView1.PageSize = Convert.ToInt32(DDL.SelectedValue);

        }


        protected void btnNew_Click(object sender, EventArgs e)
        {
            string id = "0";
            lblId.Text = id;
            txtCardCode.Text = "";
            txtCardName.Text = "";
            txtDept.Text = "";
            txtTo.Text = "";
            txtCc.Text = "";
            this.pnlPopUp_ModalPopupExtender.Show();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            MasterPage master = this.Master;
            HtmlGenericControl lblMsg = (HtmlGenericControl)master.FindControl("loginErrorMsgWrong");
            Label msg = (Label)master.FindControl("lblMsg");
            lblMsg.Visible = false;

            string to = txtTo.Text.Trim();
            string cc = txtCc.Text.Trim();
            string email = "";
            if (!isValidEmail(to, out email) || !isValidEmail(cc, out email))
            {
                lblError.Text = "Email To [" + email + "] address is invalid.";
                this.pnlPopUp_ModalPopupExtender.Show();
                return;
            }

            try
            {
                ft_email_add emailAddress = new ft_email_add();
                emailAddress.CardCode = txtCardCode.Text.Trim();
                emailAddress.CardName = txtCardName.Text.Trim();
                emailAddress.Name = txtCardName.Text.Trim();
                emailAddress.CardType = "C";
                emailAddress.To = txtTo.Text.Trim();
                emailAddress.Cc = txtCc.Text.Trim();
                emailAddress.Dept = txtDept.Text.Trim();
                emailAddress.DBName = ddlCompany.SelectedValue;
                emailAddress.Format = ddlFormat.SelectedValue;
                emailAddress.UserSign = lblUser.Text.Trim();
                emailAddress.Save(lblId.Text.Trim()).ToString();
                lblMsg.Visible = true;
                msg.Text = "Email Address Saved.";
                Label4.Text = "Row [" + lblId.Text + "] Email Address has been updated.";
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                msg.Text = ex.Message;
            }
            GridView1.EditIndex = -1;
            GridView1.DataBind();
            lblError.Text = "";
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {   //Load Email Address
            LinkButton btndetails = sender as LinkButton;
            GridViewRow gvrow = (GridViewRow)btndetails.NamingContainer;

            string id = btndetails.CommandArgument;
            lblId.Text = id;

            ft_email_add emailAddress = new ft_email_add();
            emailAddress = ft_email_add.LoadEmailAddress(id, "", "");
            txtCardCode.Text = emailAddress.CardCode;
            txtCardName.Text = emailAddress.CardName;
            txtTo.Text = emailAddress.To;
            txtCc.Text = emailAddress.Cc;
            txtDept.Text = emailAddress.Dept;
            ddlCompany.SelectedValue = emailAddress.DBName;
            ddlFormat.Text = emailAddress.Format;
            this.pnlPopUp_ModalPopupExtender.Show();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            GridView1.DataBind();
        }

        public static bool isValidEmail(string eMails, out string email)
        {
            EmailAddressAttribute foo = new EmailAddressAttribute();
            eMails = Regex.Replace(eMails, @"[;|:/]", ",");
            eMails = Regex.Replace(eMails, @"[\s]", "");
            foreach (string eMail in Regex.Split(eMails, ","))
            {
                if (eMail.Equals(""))
                {
                    continue;
                }
                bool isValid = foo.IsValid(eMail);
                if (!isValid)
                {
                    email = eMails;
                    return false;
                }
            }
            email = "";
            return true;
        }

        protected void btnSync_Click(object sender, EventArgs e)
        {
            if (Session["CurCompany"] != null)
            {
                DAC.ExecuteDataTable("SyncEmail_sp", DAC.Parameter("DBName", Session["CurCompany"].ToString()));
                Label4.Text = "BP import successful ";
                GridView1.DataBind();
            }
            else
            {
                Label4.Text = "Invalid Company";
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string curCompany = Session["CurCompany"].ToString();
            DataTable dt = DAC.ExecuteDataTable("LoadEmailAdd_sp",
                DAC.Parameter("id", ""),
                DAC.Parameter("CardCode", ""),
                DAC.Parameter("DBName", curCompany));

            try
            {
                if (dt.Rows.Count > 0)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "EmailAddress");
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=EmailAddressList.xlsx");
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string eee = ex.Message;
            }
        }



    }
}