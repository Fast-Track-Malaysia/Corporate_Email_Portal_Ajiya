using ClosedXML.Excel;
using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class EmailLogs : System.Web.UI.Page
    {
        private int totalRecords = 0;
        private String pageSize = "";
        private String pagerInfo = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserId"] == null || Session["UserId"].ToString() == "")
                {
                    Response.Redirect("~/Default.aspx", false);
                }

                string curRole = Session["Role"].ToString();
                if (curRole != "1" && curRole != "2")
                {
                    Response.Redirect("~/Home.aspx", false);
                }
            }
            SqlDataSource.SelectParameters.Clear();
            SqlDataSource.SelectParameters.Add("DBName", "SSBC");
        }


        // Web gridview render ----------------------------------------------------------------
        protected void SqlDataSource_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            /* Get Total number of records */
            int First = (GridView1.PageIndex * GridView1.PageSize) + 1;
            int Last = First + GridView1.PageSize - 1;
            pagerInfo = " " + First + " - " + Last + " of " + totalRecords + " items";
            pageSize = GridView1.PageSize.ToString();
            LabelPrimary.Text = pagerInfo;
            LabelInfo.Text = "Page size " + pageSize;
        }
        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            GridView1.AllowPaging = false;
            totalRecords = GridView1.Rows.Count;
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
                Label.Text = pagerInfo;
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

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataView dv = SqlDataSource.Select(DataSourceSelectArguments.Empty) as DataView;
            DataTable dt = dv.ToTable();

            try
            {
                if (dt.Rows.Count > 0)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "EmailLogs");
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=EmailLogs"+ DateTime.Now.ToString("yyyy-M-d") +".xlsx");
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