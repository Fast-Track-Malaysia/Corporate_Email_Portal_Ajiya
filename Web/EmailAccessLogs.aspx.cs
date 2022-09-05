using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Web
{
    public partial class EmailAccessLogs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string curRole = Session["CurRole"].ToString();
                if (curRole != "1" && curRole != "2")
                {
                    Response.Redirect("~/Home.aspx", false);
                }
                refreshTable();
                getLocation();
            }
        }
        protected void ButtonBrowser_Click(object sender, EventArgs e)
        {
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
            access += "Computer Name: " + System.Environment.MachineName + "<br>";
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
            refreshTable();
        }
        protected void refreshTable()
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
        protected void ButtonLocation_Click(object sender, EventArgs e)
        {
            getLocation();
        }
        protected void getLocation()
        {
            if (OnlineVisitorsContainer.Visitors != null)
            {
                IOrderedEnumerable<WebsiteVisitor> sorted = OnlineVisitorsContainer.Visitors.Values.OrderByDescending(x => x.SessionStarted);

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("AuthUser");
                dt.Columns.Add("IpAddress");
                dt.Columns.Add("CountryName");
                dt.Columns.Add("RegionName");
                dt.Columns.Add("City");
                dt.Columns.Add("ZipCode");
                dt.Columns.Add("TimeZone");
                dt.Columns.Add("Latitude");
                dt.Columns.Add("Longitude");
                foreach (WebsiteVisitor visitor in sorted)
                {
                    string ip = Server.HtmlEncode(visitor.IpAddress);
                    XmlDocument doc = new XmlDocument();
                    string getdetails = "http://www.freegeoip.net/xml/" + ip;
                    doc.Load(getdetails);
                    if (doc == null)
                    {
                        Label_Errors.Text = "Location not valid.";
                        break;
                    }                    

                    DataRow _dr = dt.NewRow();
                    _dr["AuthUser"] = visitor.AuthUser;
                    _dr["IpAddress"] = ip;
                    _dr["CountryName"] = doc.GetElementsByTagName("CountryName")[0].InnerText;
                    _dr["RegionName"] = doc.GetElementsByTagName("RegionName")[0].InnerText;
                    _dr["City"] = doc.GetElementsByTagName("City")[0].InnerText;
                    _dr["ZipCode"] = doc.GetElementsByTagName("ZipCode")[0].InnerText;
                    _dr["TimeZone"] = doc.GetElementsByTagName("TimeZone")[0].InnerText;
                    _dr["Latitude"] = doc.GetElementsByTagName("Latitude")[0].InnerText;
                    _dr["Longitude"] = doc.GetElementsByTagName("Longitude")[0].InnerText;
                    dt.Rows.Add(_dr);
                }
                DataGridLocation.DataSource = dt; //return online users
                DataGridLocation.DataBind();
            }




        }


    }
}