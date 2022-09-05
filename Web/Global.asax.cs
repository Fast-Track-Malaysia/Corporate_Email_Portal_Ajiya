using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Security;
using System.Web.Configuration;
using System.IO;

namespace Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            //DAC.Webcon = WebConfigurationManager.ConnectionStrings["WebConnectionString"].ConnectionString;
            //DAC.SAPcon = WebConfigurationManager.ConnectionStrings["SAPConnectionString"].ConnectionString;

            Application["OnlineVisitors"] = 0;

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Default.aspx", false);
            }

            Session["CompnyCode"] = null;
            Session["CompnyName"] = null;
            Session["UserId"] = null;
            Session["UserName"] = null;
            Session["Email"] = null;
            Session["Role"] = null;
            Session["ConnectedDB"] = null;
            Session["ConnString"] = null;
            Session["SAPDB"] = null;
            Session["SAPDBConnString"] = null;
            Session["AjiyaDBConnString"] = null;

            Application.Lock();
            Application["OnlineVisitors"] = (int)Application["OnlineVisitors"] + 1;
            Application.UnLock();

            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
            string pc = Environment.MachineName;
            string browser = bc.Browser + " " + bc.Version;
            string ip = Request.UserHostAddress;

            ft_logs.dir = Path.GetTempPath();
            ft_logs.init();
            ft_logs.WriteLine("Visitor:" + Application["OnlineVisitors"] + " Ip:[" + ip + "] Pc:[" + pc + "] Browser:[" + browser + "]", "Session_Start");


            // get current context
            HttpContext currentContext = HttpContext.Current;
            if (currentContext != null)
            {
                if (!currentContext.Request.Browser.Crawler)
                {
                    WebsiteVisitor currentVisitor = new WebsiteVisitor(currentContext);
                    OnlineVisitorsContainer.Visitors[currentVisitor.SessionId] = currentVisitor;
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            ft_logs.dir = Path.GetTempPath();
            ft_logs.WriteLine("Sender:" + sender.ToString() + " Event:" + e.ToString(), "Global Error");

            //Response.Redirect("~/Default.aspx", false);

        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            ft_logs.dir = Path.GetTempPath();
            ft_logs.init();
            ft_logs.WriteLine("Sender:" + Session["UserName"] + " CurRole:" + Session["Role"] + " CurCompany:" + Session["CompnyName"] , "Session_End");

            Session["CompnyCode"] = null;
            Session["CompnyName"] = null;
            Session["UserId"] = null;
            Session["UserName"] = null;
            Session["Email"] = null;
            Session["Role"] = null;
            Session["ConnectedDB"] = null;
            Session["ConnString"] = null;
            Session["SAPDB"] = null;
            Session["SAPDBConnString"] = null;
            Session["AjiyaDBConnString"] = null;

            Application.Lock();
            Application["OnlineVisitors"] = (int)Application["OnlineVisitors"] - 1;
            Application.UnLock();

            if (this.Session != null)
            {
                WebsiteVisitor visitor;
                OnlineVisitorsContainer.Visitors.TryRemove(this.Session.SessionID, out visitor);
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs eventArgs)
        {
            var session = HttpContext.Current.Session;
            if (session != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (OnlineVisitorsContainer.Visitors.ContainsKey(session.SessionID))
                    OnlineVisitorsContainer.Visitors[session.SessionID].AuthUser = HttpContext.Current.User.Identity.Name;
            }
        }
    }
}