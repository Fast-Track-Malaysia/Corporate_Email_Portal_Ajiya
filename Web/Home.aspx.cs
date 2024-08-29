using Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class Home : System.Web.UI.Page
    {
        StringBuilder str = new StringBuilder();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                Response.Redirect("~/Default.aspx", false);
            }

            if (!Page.IsPostBack)
            {
                //SetChart();
                BindChart();
            }
            //if (user.UserName != null)
            //{
            //    LblUser.Text = user.UserName;
            //}
            if (Session["UserName"] != null)
            {
                LblUser.Text = Session["UserName"].ToString();
            }

            lblLive.Text = "Live : " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        }


        //private DataTable GetData()
        //{
        //    string curCompany = Session["CurCompany"].ToString();
        //    DataTable dt = DAC.ExecuteDataTable("getEmailInfo_sp", DAC.Parameter("@DBName", curCompany));
        //    if (dt.Rows.Count > 0)
        //    {
        //        try
        //        {
        //            int n = dt.Rows.Count-1;
        //            labelSent.Text = dt.Rows[n]["TotalSent"].ToString().Trim();
        //            labelCardCode.Text = dt.Rows[n]["Sent"].ToString().Trim();
        //            labelCurSent.Text = dt.Rows[n]["CurSent"].ToString().Trim();

        //            DateTime oDate = Convert.ToDateTime(dt.Rows[n]["LastSent"].ToString().Trim());                    
        //            labelLastSent.Text = oDate.ToRelativeString();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception(ex.Message);
        //        }
        //    }
        //    return dt;
        //}


        private void BindChart()
        {
            //string curCompany = Session["CurCompany"].ToString();
            //DataTable dt = new DataTable();
            //try
            //{

            //    dt = GetData();
            //    str.Append(@"<script type=*text/javascript*> google.load( *visualization*, *1*, {packages:[*corechart*]});
            //           google.setOnLoadCallback(drawChart);
            //           function drawChart() {
            //            var data = new google.visualization.DataTable();
            //            data.addColumn('string', 'Month');
            //            data.addColumn('number', 'Sent');
            //            data.addRows(" + dt.Rows.Count + ");");

            //    for (int i = 0; i <= dt.Rows.Count - 1; i++)
            //    {
            //        str.Append("data.setValue( " + i + "," + 0 + "," + "'" + dt.Rows[i]["Month"].ToString() + "');");
            //        str.Append("data.setValue(" + i + "," + 1 + "," + dt.Rows[i]["Sent"].ToString() + ") ;");
            //    }
            //    str.Append(" var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));");
            //    str.Append(" chart.draw(data, {width: 550, height: 300, title: '" + curCompany + "',");
            //    str.Append("hAxis: {title: 'Month', titleTextStyle: {color: 'green'}}");
            //    str.Append("}); }");
            //    str.Append("</script>");
            //    lt.Text = str.ToString().Replace('*', '"');

            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
        }
    }
}