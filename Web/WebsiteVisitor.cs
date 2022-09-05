using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web
{
    public class WebsiteVisitor
    {
        public string SessionId { get; set; }

        public string IpAddress { get; set; }

        public string AuthUser { get; set; }

        public string UrlReferrer { get; set; }

        public string EnterUrl { get; set; }

        public string UserAgent { get; set; }

        public DateTime SessionStarted { get; set; }

        public WebsiteVisitor(HttpContext context)
        {
            if (context != null && context.Request != null && context.Session != null)
            {
                this.SessionId = context.Session.SessionID;

                this.SessionStarted = DateTime.UtcNow;

                //this.UserAgent = String.IsNullOrEmpty(context.Request.UserAgent) ? "" : context.Request.UserAgent;
                this.UserAgent = context.Request.UserAgent ?? String.Empty;

                this.IpAddress = context.Request.UserHostAddress;

                //-------------------------------------------------------------
                if (context.Request.IsAuthenticated)
                {
                    this.AuthUser = context.User.Identity.Name;
                    if (!String.IsNullOrEmpty(context.Request.ServerVariables["REMOTE_USER"]))
                        this.AuthUser = context.Request.ServerVariables["REMOTE_USER"];
                    else if (!String.IsNullOrEmpty(context.Request.ServerVariables["AUTH_USER"]))
                        this.AuthUser = context.Request.ServerVariables["AUTH_USER"];
                }

                //-------------------------------------------------------------
                if (context.Request.UrlReferrer != null)
                {
                    this.UrlReferrer = String.IsNullOrWhiteSpace(context.Request.UrlReferrer.OriginalString) ? "" : context.Request.UrlReferrer.OriginalString;
                }

                this.EnterUrl = String.IsNullOrWhiteSpace(context.Request.Url.OriginalString) ? "" : context.Request.Url.OriginalString;
            }
        }
    }

    /// <summary>
    /// Online visitors list
    /// </summary>
    public static class OnlineVisitorsContainer
    {
        public static readonly ConcurrentDictionary<string, WebsiteVisitor> Visitors = new ConcurrentDictionary<string, WebsiteVisitor>();
    }

}