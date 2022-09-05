using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web.Schema;

namespace Web
{
    public partial class EmailDebug : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string curRole = Session["CurRole"].ToString();
                if (curRole != "1")
                {
                    Response.Redirect("~/Home.aspx", false);
                }
            }
        }

        protected void GenerateScheme_Click(object sender, EventArgs e)
        {
            String schema = Server.MapPath("~/Schema/Debug");
            CreateSchema("Invoices");
            CreateSchema("CreditNotes");
            CreateSchema("Statement");
            Label_CreateSchema.Text = "Schema has been created @ " + schema;
        }

        // One Time Use - Use to create crystal report schema file xsd
        protected void CreateSchema(String doc)
        {
            String conString = ConfigurationManager.ConnectionStrings["SAPConnectionString"].ConnectionString;
            String schema = Server.MapPath("~/Schema/Debug");

            if (doc.Equals("Invoices"))
            {
                DataSet ds = new DataSet("Invoices");

                using (SqlConnection conn = new SqlConnection(conString))
                {
                    SqlCommand sqlComm = new SqlCommand("sp_FT_Web_ARInvoices", conn);
                    sqlComm.Parameters.AddWithValue("@DateFr", "20160101");
                    sqlComm.Parameters.AddWithValue("@DateTo", "20171231");
                    sqlComm.Parameters.AddWithValue("@CardCodeFr", "AAAA000");
                    sqlComm.Parameters.AddWithValue("@CardCodeTo", "ZZZZ999");
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = sqlComm;
                    da.Fill(ds, "Data");
                }

                ds.WriteXmlSchema(schema + "Invoices.xsd");
                ds.WriteXml(Path.GetTempPath() + "Invoice_" + "CARDCODE" + "_" + DateTime.Today.ToString("yyyyMMdd") + "_.xml", XmlWriteMode.WriteSchema);
            }
            if (doc.Equals("CreditNotes"))
            {
                DataSet ds = new DataSet("CreditNotes");

                using (SqlConnection conn = new SqlConnection(conString))
                {
                    SqlCommand sqlComm = new SqlCommand("sp_FT_Web_ARCreditNotes", conn);
                    sqlComm.Parameters.AddWithValue("@DateFr", "20160101");
                    sqlComm.Parameters.AddWithValue("@DateTo", "20171231");
                    sqlComm.Parameters.AddWithValue("@CardCodeFr", "AAAA000");
                    sqlComm.Parameters.AddWithValue("@CardCodeTo", "ZZZZ999");
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = sqlComm;
                    da.Fill(ds, "Data");
                }

                ds.WriteXmlSchema(schema + "CreditNotes.xsd");
                ds.WriteXml(Path.GetTempPath() + "CreditNote_" + "CARDCODE" + "_" + DateTime.Today.ToString("yyyyMMdd") + "_.xml", XmlWriteMode.WriteSchema);
            }
            if (doc.Equals("Statement"))
            {
                DataSet ds = new DataSet("Statements");
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    SqlCommand sqlComm = new SqlCommand("sp_FT_Web_StatementOfAccount", conn);
                    sqlComm.Parameters.AddWithValue("@StatementDate", "20160601");
                    sqlComm.Parameters.AddWithValue("@CardCodeFr", "RNBE001");
                    sqlComm.Parameters.AddWithValue("@CardCodeTo", "RTAI013");
                    sqlComm.Parameters.AddWithValue("@CardType", "C");
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = sqlComm;
                    da.Fill(ds, "Data");
                }

                ds.WriteXmlSchema(schema + "Statements.xsd");
                ds.WriteXml(Path.GetTempPath() + "Statements_" + "CARDCODE" + "_" + DateTime.Today.ToString("yyyyMMdd") + "_.xml", XmlWriteMode.WriteSchema);
            }

        }

        protected void ButtonBrowser_Click(object sender, EventArgs e)
        {
            string[] computer_name = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
            String ecname = System.Environment.MachineName;

            //LabelComputerName.Text = computer_name[0].ToString();
            //LabelInfo.Text = ecname;
            //LabelIP.Text = Request.UserHostAddress;
            //LabelUser.Text = Request.UserHostName;
            //string info = "";
            //HttpBrowserCapabilities bc = Request.Browser;
            //info = "<p>Browser Capabilities:</p>";
            //info += "Type = " + bc.Type + "<br>";
            //info += "Name = " + bc.Browser + "<br>";
            //info += "Version = " + bc.Version + "<br>";
            //info += "Major Version = " + bc.MajorVersion + "<br>";
            //info += "Minor Version = " + bc.MinorVersion + "<br>";
            //info += "Platform = " + bc.Platform + "<br>";
            //info += "Is Beta = " + bc.Beta + "<br>";
            //info += "Is Crawler = " + bc.Crawler + "<br>";
            //info += "Is AOL = " + bc.AOL + "<br>";
            //info += "Is Win16 = " + bc.Win16 + "<br>";
            //info += "Is Win32 = " + bc.Win32 + "<br>";
            //info += "Supports Frames = " + bc.Frames + "<br>";
            //info += "Supports Tables = " + bc.Tables + "<br>";
            //info += "Supports Cookies = " + bc.Cookies + "<br>";
            //info += "Supports VB Script = " + bc.VBScript + "<br>";
            //info += "Supports JavaScript = " + bc.JavaScript + "<br>";
            //info += "Supports Java Applets = " + bc.JavaApplets + "<br>";
            //info += "Supports ActiveX Controls = " + bc.ActiveXControls + "<br>";
            //info += "CDF = " + bc.CDF + "<br>";
            //LabelBrowserInfo.Text = info;
        }

        protected void ButtonAccess_Click(object sender, EventArgs e)
        {
            string access = "";
            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
            OperatingSystem os = Environment.OSVersion;

            string windows = "";
            if (Request.UserAgent.IndexOf("Windows NT 5.1") > 0)
            {
                windows = "Windows XP";
            }
            else if (Request.UserAgent.IndexOf("Windows NT 6.0") > 0)
            {
                windows = "Windows Vista";//VISTA
            }
            else if (Request.UserAgent.IndexOf("Windows NT 6.1") > 0)
            {
                windows = "Windows 7";//7
            }
            else if (Request.UserAgent.IndexOf("Windows NT 6.2") > 0)
            {
                windows = "Windows 8";//8
            }
            else if (Request.UserAgent.IndexOf("Windows NT 6.3") > 0)
            {
                windows = "Windows 8.1";//8.1
            }
            else if (Request.UserAgent.IndexOf("Windows NT 10.0") > 0)
            {
                windows = "Windows 10";//10
            }
            access = "<p>Access Information:</p>";
            access += "Remote Host: " + Request.ServerVariables["REMOTE_HOST"] + "<br>";
            access += "User Host Address: " + Request.UserHostAddress + "<br>";
            access += "Computer Name: " + System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName + "<br>";
            access += "Current User: " + System.Web.HttpContext.Current.User + "<br>";
            access += "Page Viewed: " + Request.Url.AbsolutePath + "<br>";
            access += "Browser: " + bc.Browser + " " + bc.Version + "<br>";

            access += "UserAgent: " + Request.UserAgent + "<br>";
            access += "Operating System: " + windows + "<br>";

            access += "Platform: " + os.Platform.ToString() + "<br>";
            access += "version: " + os.Version.ToString() + "<br>";
            access += "ServicePack: " + os.ServicePack.ToString() + "<br>";

            LabelAccess.Text = access;  

        }

        protected void ButtonRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (OnlineVisitorsContainer.Visitors != null)
                {
                    IOrderedEnumerable<WebsiteVisitor> sorted = OnlineVisitorsContainer.Visitors.Values.OrderByDescending(x => x.SessionStarted);

                    DataTable dt = new DataTable();
                    dt.Clear();
                    dt.Columns.Add("AuthUser");
                    dt.Columns.Add("IpAddress");
                    dt.Columns.Add("SessionId");
                    dt.Columns.Add("SessionStarted");
                    dt.Columns.Add("UserAgent");
                    dt.Columns.Add("UrlReferrer");
                    dt.Columns.Add("EnterUrl");
                    foreach (WebsiteVisitor visitor in sorted)
                    {
                        DataRow _dr = dt.NewRow();
                        _dr["AuthUser"] = visitor.AuthUser;
                        _dr["IpAddress"] = visitor.IpAddress;
                        _dr["SessionId"] = visitor.SessionId;
                        _dr["SessionStarted"] = visitor.SessionStarted;
                        _dr["UserAgent"] = visitor.UserAgent;
                        _dr["UrlReferrer"] = visitor.UrlReferrer;
                        _dr["EnterUrl"] = visitor.EnterUrl;                 
                        dt.Rows.Add(_dr);
                    }
                    UserGrid.DataSource = dt; //return online users
                    UserGrid.DataBind();
                    Label_OnlineUser.Text = (sorted.ToArray().Count()).ToString(); //return no. of online user           
                }       
            }
            catch (Exception ex)
            {
                Label_Errors.Text = ex.Message;
            }
        }

    }
}