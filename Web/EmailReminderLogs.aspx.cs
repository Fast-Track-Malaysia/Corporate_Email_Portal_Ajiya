using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class EmailReminderLogs : System.Web.UI.Page
    {
        protected static Dictionary<string, List<FileInfo>> UserFiles = new Dictionary<string, List<FileInfo>>();
        protected static Dictionary<string, List<FileInfo>> Documents = new Dictionary<string, List<FileInfo>>();
        protected static DataTable dt;
        protected static List<string> SelectedCardCode = new List<string>();
        protected static List<string> SelectedCardCodeAll = new List<string>();
        protected static List<string> noRecordCardCode = new List<string>();
        protected static List<string> docPath = new List<string>();
        protected static List<LReminderSelectedRow> LReminderSelectedRow = new List<LReminderSelectedRow>();
        protected static List<LReminderSelectedRow> LReminderSelectedRowAll = new List<LReminderSelectedRow>();
        protected int totalRecords = 0;
        protected String pagerInfo = "";
        protected static string folderDir = "";
        protected static string info;
        protected static string warning;
        protected static bool inProcess = false;
        protected static DateTime statementdate;
        protected static DateTime senddateFr;
        protected static DateTime senddateTo;
        protected static string statementdateStr = "";
        protected static string senddateFrStr = "";
        protected static string senddateToStr = "";
        protected static string strFr = "";
        protected static string strTo = "";
        protected static string sendResult = "";
        protected static string query = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                LabelDanger.Text = "You has been logged out!";
                Response.Redirect("~/Default.aspx", false);
            }
            else
            {
                LabelCompany.Text = Session["CompnyCode"].ToString();
                SqlDataSource.ConnectionString = Session["ConnString"].ToString();
            }

            if (!Page.IsPostBack)
            { 
                lblUser.Text = Session["UserId"].ToString();

                txtSendDateFr.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtSendDateTo.Text = DateTime.Now.ToString("dd/MM/yyyy");

                info = "Page Load";

                dt = null;
                GridView1.DataSource = null;
                GridView1.DataBind();

                btnSend.Enabled = true;
                btnSearch.Enabled = true;
            }
        }
        protected void btnSelect_Click(object sender, EventArgs e)
        {
            int a = GridView1.PageIndex;
            LReminderSelectedRow.Clear();
            LabelInfo.Text = "";
            foreach (GridViewRow row in GridView1.Rows)
            {
                Label LabelID = (Label)row.FindControl("lblID");
                Label LabelCardcode = (Label)row.FindControl("LabelCardcode");
                Label LabelCardName = (Label)row.FindControl("CardName");
                Label LabelEmailTo = (Label)row.FindControl("lblEmailTo");
                Label LabelEmailCC = (Label)row.FindControl("lblEmailCC");
                Label LabelEmailSubject = (Label)row.FindControl("lblEmailSubject");
                Label LabelEmailContent = (Label)row.FindControl("lblEmailContent");
                Label LabelReminderDate = (Label)row.FindControl("lblReminderDate");
                Label LabelMinAmt = (Label)row.FindControl("lblMinAmt");
                Label LabelInvDaysFrom = (Label)row.FindControl("lblInvDaysFrom");
                Label LabelInvDaysTo = (Label)row.FindControl("lblInvDaysTo");
                Label LabelLetterType = (Label)row.FindControl("lblLetterType");
                Label LabelSendDate = (Label)row.FindControl("lblSendDate");
                CheckBox chkBox = (CheckBox)row.FindControl("chkBox");

                LReminderSelectedRow.Add(new LReminderSelectedRow
                {
                    ID = int.Parse(LabelID.Text),
                    CardCode = LabelCardcode.Text,
                    CardName = LabelCardName.Text,
                    EmailTo = LabelEmailTo.Text,
                    EmailCC = LabelEmailCC.Text,
                    EmailSubject = LabelEmailSubject.Text,
                    EmailContent = LabelEmailContent.Text,
                    ReminderDate = DateTime.ParseExact(LabelReminderDate.Text, "dd/MM/yyyy", null),
                    MinAmt = decimal.Parse(LabelMinAmt.Text),
                    InvDaysFrom = int.Parse(LabelInvDaysFrom.Text),
                    InvDaysTo = int.Parse(LabelInvDaysTo.Text),
                    LetterType = LabelLetterType.Text,
                    SendDate = DateTime.Parse(LabelSendDate.Text),
                    isChecked = chkBox.Checked
                });

                LabelInfo.Text = "Selected : " + LReminderSelectedRow.Count();
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                LabelDanger.Text = "You has been logged out!";
                Response.Redirect("~/Default.aspx", false);
            }

            warning = "";
            info = "Search...";
            ft_logs.WriteLine("Perform search for reminder log Date From: " + txtSendDateFr.Text + " To: " + txtSendDateTo.Text, "Info");
            init();
            LReminderSelectedRow.Clear();
            SqlDataSource.SelectParameters.Clear();
            try
            {
                senddateFr = DateTime.ParseExact(txtSendDateFr.Text, "dd/MM/yyyy", null);
                senddateTo = DateTime.ParseExact(txtSendDateTo.Text, "dd/MM/yyyy", null);

                senddateFrStr = senddateFr.ToString("yyyyMMdd");
                senddateToStr = senddateTo.ToString("yyyyMMdd");
                strFr = txtCardCodeFr.Text.Equals("") ? "*" : txtCardCodeFr.Text;
                strTo = txtCardCodeTo.Text.Equals("") ? "*" : txtCardCodeTo.Text;
                sendResult = ddlSendResult.SelectedValue.ToString();

                SqlDataSource.SelectParameters.Add("senddateFrStr", senddateFrStr);
                SqlDataSource.SelectParameters.Add("senddateToStr", senddateToStr);
                SqlDataSource.SelectParameters.Add("strFr", strFr);
                SqlDataSource.SelectParameters.Add("strTo", strTo);
                SqlDataSource.SelectParameters.Add("sendResult", sendResult);

                GridView1.DataBind();

                using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
                {
                    using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                    {
                        query = "SELECT * FROM [FTS_fn_ReminderLogs] ('" + senddateFrStr + "', '" + senddateToStr + "', '" + strFr + "', '" + strTo + "', '" + sendResult + "') T0 ORDER BY T0.SendDate DESC, T0.CardCode ";

                        dat.SelectCommand.CommandText = query;
                        SqlDataSource.SelectCommand = query;

                        DataTable searchDT = new DataTable();
                        dat.Fill(searchDT);

                        if (searchDT.Rows.Count > 0)
                        {
                            dt = searchDT;
                        }
                        searchDT.Dispose();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                ft_logs.WriteLine("Perform search error:" + ex.Message, "Error");

                ft_dblogs logs = new ft_dblogs();
                logs.Screen = "ReminderLog";
                logs.MessageType = "Error";
                logs.Message = ex.Message;
                logs.UserSign = lblUser.Text;
                logs.TimeStamp = DateTime.Now;
                logs.Remarks = "Search Error";
                logs.DBName = LabelCompany.Text;
                logs.SaveLogs(Session["ConnString"].ToString());
            }

        }
        protected void btnRefresh_Click(object sender, EventArgs e)
        {

        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            LabelLoading.Text = info;
            LabelWarning.Text = warning;
            if (!inProcess)
            {
                btnSend.Enabled = true;
                Timer1.Enabled = false;
                imgLoading.Visible = false;
                LabelLoading.Text = "Task completed!";
            }
        }
        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                LabelDanger.Text = "You has been logged out!";
                Response.Redirect("~/Default.aspx", false);
            }

            init();
            if (LReminderSelectedRow.Count <= 0)
            {
                LabelDanger.Text = "No record is selected!";
                return;
            }

            Timer1.Enabled = true;
            imgLoading.Visible = true;
            info = "App starting...";
            try
            {
                btnSend.Enabled = false;
                btnSearch.Enabled = false;

                Thread workerThread = new Thread(new ThreadStart(MainTask));
                workerThread.Start();
            }
            catch (ThreadInterruptedException ex)
            {
                LabelDanger.Text = ex.Message;
                info = ex.Message;
            }
            finally
            {
                btnSend.Enabled = true;
                btnSearch.Enabled = true;
            }
        }
        protected void MainTask()
        {
            inProcess = true;
            ft_logs.dir = "C:\\WebMail\\[" + LabelCompany.Text + "] Logs";
            ft_logs.WriteLine("-- Log started at " + DateTime.Now.ToString("hh:mm:ss tt") + " --------------------------------------------", "");
            ft_logs.WriteLine("User:    " + lblUser.Text, "");
            ft_logs.WriteLine("Company: " + LabelCompany.Text, "");
            ft_logs.WriteLine("Log Reminder send Date From:  " + txtSendDateFr.Text + " To: " + txtSendDateTo.Text, "");
            ft_logs.WriteLine("-------------------------------------------------------------------------------", "");
            // Core Function
            GenerateDocument();
            SendEmail();

            if (Directory.Exists(folderDir))
            {
                Directory.Delete(folderDir, true);
            }

            inProcess = false;
            LReminderSelectedRow.Clear();
            LabelInfo.Text = "";
            ViewState["chkAll"] = null;
            SqlDataSource.SelectCommand = query;
            GridView1.DataBind();
            GC.Collect();
            ft_logs.WriteLine("-- Log ended at " + DateTime.Now.ToString("hh:mm:ss tt") + " --------------------------------------------", "");
        }

        protected void init()
        {
            LabelDanger.Text = "";
            LabelWarning.Text = "";
            LabelInfo.Text = "";
            LabelDefault.Text = "";
            LabelPrimary.Text = "";
        }
        protected void GenerateDocument()
        {
            ft_logs.WriteLine("Generate reminder begin...", "Info");

            string layoutName = LabelCompany.Text + "_Reminder";


            folderDir = "C:\\EmailPortal\\[" + LabelCompany.Text + "] " + lblUser.Text + "\\LReminder\\" + DateTime.Now.ToString("yyyymmdd");

            try
            {
                ft_logs.WriteLine("① Clear attachment folder...", "Info");
                foreach (int ID in LReminderSelectedRow.Select(x => x.ID).Distinct())
                {
                    string path = folderDir + "\\" + ID + "\\";
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(folderDir, true);
                    }
                }
            }
            catch (IOException ex)
            {
                info = "Clear attachment error:" + ex.Message;
                LabelDanger.Text = info;
                ft_logs.WriteLine(info, "Error");
                return;
            }

            docPath = new List<string>();
            try
            {
                bool Checked;

                for (int i = 0; i < LReminderSelectedRow.Count; i++)
                {
                    Checked = LReminderSelectedRow[i].isChecked;

                    if (Checked)
                    {
                        string ID = LReminderSelectedRow[i].ID.ToString();
                        string reminderdateStr = LReminderSelectedRow[i].ReminderDate.ToString("yyyy-MM-dd");
                        string CardCode = LReminderSelectedRow[i].CardCode;
                        string CardName = LReminderSelectedRow[i].CardName;
                        decimal MinAmt = LReminderSelectedRow[i].MinAmt;
                        int InvDaysFrom = LReminderSelectedRow[i].InvDaysFrom;
                        int InvDaysTo = LReminderSelectedRow[i].InvDaysTo;
                        string LetterType = LReminderSelectedRow[i].LetterType;
                        string senddatestr = LReminderSelectedRow[i].SendDate.ToString("yyyy-MM-dd");

                        string path = folderDir + "\\" + ID + "\\";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        info = "③ Generate Reminder [" + (i + 1).ToString() + "/" + LReminderSelectedRow.Count() + "] ➜ [" + CardCode + "-" + CardName + "]";
                        ft_logs.Write(info, "Info");

                        string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                        Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                        CardName = r.Replace(CardName, "");

                        ReportDocument crystalReport = new ReportDocument();
                        crystalReport.Load(Server.MapPath("~/Reports/" + layoutName + ".rpt"));

                        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Session["ConnString"].ToString());
                        crystalReport.DataSourceConnections[0].SetConnection(builder.DataSource, Session["SAPDB"].ToString(), builder.UserID, builder.Password);

                        ParameterFieldDefinitions crParameterdef;
                        crParameterdef = crystalReport.DataDefinition.ParameterFields;

                        foreach (ParameterFieldDefinition param in crParameterdef)
                        {
                            if (param.Name.Equals("AsAtDate"))
                            {
                                crystalReport.SetParameterValue("AsAtDate", reminderdateStr);
                                continue;
                            }

                            if (param.Name.Equals("MinAmount"))
                            {
                                crystalReport.SetParameterValue("MinAmount", MinAmt);
                                continue;
                            }

                            if (param.Name.Equals("ExceedDaysFr"))
                            {
                                crystalReport.SetParameterValue("ExceedDaysFr", InvDaysFrom);
                                continue;
                            }

                            if (param.Name.Equals("ExceedDaysTo"))
                            {
                                crystalReport.SetParameterValue("ExceedDaysTo", InvDaysTo);
                                continue;
                            }

                            if (param.Name.Equals("LetterType"))
                            {
                                crystalReport.SetParameterValue("LetterType", LetterType);
                                continue;
                            }

                            if (param.Name.Equals("CardCode@FROM OCRD WHERE CardType = 'C'"))
                            {
                                crystalReport.SetParameterValue("CardCode@FROM OCRD WHERE CardType = 'C'", CardCode);
                                continue;
                            }

                            if (param.Name.Equals("PrintDate"))
                            {
                                crystalReport.SetParameterValue("PrintDate", senddatestr);
                                continue;
                            }
                        }

                        string ExportPath = path + "LRMD_" + CardCode + "_" + reminderdateStr + ".pdf";
                        crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, ExportPath);
                        docPath.Add(ExportPath);
                        crystalReport.Dispose();
                        ft_logs.WriteLine(" Folder:[" + ExportPath + "]", "");
                    }
                }
            }
            catch (Exception ex)
            {
                LabelDanger.Text = ex.Message;
                ft_logs.WriteLine("Generating Reminder error:" + ex.Message, "Error");
                inProcess = false;
            }
            finally
            {
                ft_logs.WriteLine("Generate Reminder ended!", "Info");
            }
        }
        protected void SendEmail()
        {
            ft_logs.WriteLine("✉ Send E-Mail Begin...", "Info");
            // Outgoing Mail Settings
            string smtp = WebConfigurationManager.AppSettings.Get("emailhost");
            string port = WebConfigurationManager.AppSettings.Get("emailport");
            string user = WebConfigurationManager.AppSettings.Get("emailuser");
            string pass = WebConfigurationManager.AppSettings.Get("emailpass");

            string fr = WebConfigurationManager.AppSettings.Get("emailfr");
            string fn = WebConfigurationManager.AppSettings.Get("emailfn");

            Boolean ssl = Boolean.Parse(WebConfigurationManager.AppSettings["EnableSSL"].ToString());

            bool Checked;

            for (int i = 0; i < LReminderSelectedRow.Count; i++)
            {
                LReminderSelectedRow lreminderSelectedObj = LReminderSelectedRow[i];
                Checked = lreminderSelectedObj.isChecked;

                if (Checked)
                {
                    string ID = lreminderSelectedObj.ID.ToString();
                    string CardCode = lreminderSelectedObj.CardCode;
                    string CardName = lreminderSelectedObj.CardName;
                    string to = lreminderSelectedObj.EmailTo;
                    string cc = lreminderSelectedObj.EmailCC;
                    DateTime reminderdate = lreminderSelectedObj.ReminderDate;
                    decimal minamt = lreminderSelectedObj.MinAmt;
                    int invdaysfrom = lreminderSelectedObj.InvDaysFrom;
                    int invdaysto = lreminderSelectedObj.InvDaysTo;
                    string lettertype = lreminderSelectedObj.LetterType;

                    info = "④ Sending Reminder [" + (i + 1).ToString() + "/" + LReminderSelectedRow.Count() + "] ➜ [" + CardCode + "-" + CardName + "] TO: [" + to + "] CC: [" + cc + "]";
                    ft_logs.Write(info, "Info");

                    string subject = lreminderSelectedObj.EmailSubject;
                    subject = subject.Replace(@"[CardName]", CardName);
                    subject = subject.Replace(@"[DD]", reminderdate.ToString("dd"));
                    subject = subject.Replace(@"[MM]", reminderdate.ToString("MM"));
                    subject = subject.Replace(@"[MMM]", reminderdate.ToString("MMM"));
                    subject = subject.Replace(@"[YYYY]", reminderdate.ToString("yyyy"));

                    string content = lreminderSelectedObj.EmailContent;
                    content = content.Replace(@"[DD]", reminderdate.ToString("dd"));
                    content = content.Replace(@"[MM]", reminderdate.ToString("MM"));
                    content = content.Replace(@"[MMM]", reminderdate.ToString("MMM"));
                    content = content.Replace(@"[YYYY]", reminderdate.ToString("yyyy"));

                    string path = "";

                    DateTime timeStamp = DateTime.Now;

                    try
                    {
                        string pattern = "[;,| ]";

                        MailMessage oMail = new MailMessage();
                        oMail.From = new MailAddress(fr, fn);
                        foreach (string t in Regex.Split(to, pattern))
                        {
                            if (t.Equals("")) { continue; }
                            oMail.To.Add(new MailAddress(t));
                        }
                        foreach (string c in Regex.Split(cc, pattern))
                        {
                            if (c.Equals("")) { continue; }
                            oMail.CC.Add(new MailAddress(c));
                        }
                        oMail.Subject = subject;
                        oMail.Body = content;
                        oMail.IsBodyHtml = true;

                        List<string> attached = new List<string>();
                        path = folderDir + "\\" + ID + "\\";
                        if (Directory.Exists(path))
                        {
                            string[] files = Directory.GetFiles(path, "*.pdf");
                            foreach (string file in files)
                            {
                                attached.Add(file);
                                Attachment oAttach = new Attachment(file);
                                oMail.Attachments.Add(oAttach);
                            }
                        }

                        if (oMail.Attachments.Count <= 0)
                        {
                            using (SqlConnection connection = new SqlConnection(Session["ConnString"].ToString()))
                            {
                                string query = "INSERT INTO ft_email_logs_reminder (CardCode, CardName, EmailTo, EmailCC, " +
                                "EmailSubject, EmailContent, ReminderDate, MinAmt, InvDaysFrom, InvDaysTo, LetterType, SendDate, SendBy, SendResult, ErrorDesc) " +
                                "VALUES ('" + CardCode.Replace("'", "''") + "', '" + CardName.Replace("'", "''") + "', '" + to + "', '" + cc + "', '" + subject.Replace("'", "''") + "', '" + content.Replace("'", "''") + "', '" +
                                reminderdate.ToString("yyyy-MM-dd") + "', '" + minamt + "', '" + invdaysfrom + "', '" + invdaysto + "', '" + lettertype + "', '" +
                                DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "', '" +
                                Session["UserId"].ToString() + "', 'Failed', '" + "No attachment." + "');";

                                SqlCommand command = new SqlCommand(query, connection);
                                command.Connection.Open();
                                command.ExecuteNonQuery();
                                command.Connection.Close();

                            }
                            info = "Send E-Mail Error:" + "No attachment.";
                            LabelDanger.Text = info;
                            ft_logs.WriteLine(Environment.NewLine + info, "Error");

                            continue;
                        }

                        SmtpClient oSTMP = new SmtpClient();
                        oSTMP.Host = smtp;
                        oSTMP.Port = int.Parse(port);
                        oSTMP.EnableSsl = ssl;
                        oSTMP.Credentials = new NetworkCredential(user, pass);
                        oSTMP.Send(oMail);
                        oSTMP.Dispose();
                        oMail.Attachments.Dispose();

                        ft_logs.WriteLine("➜✉ E-Mail Sent " + CardCode + "-" + CardName + " @ " + DateTime.Today.ToString("d MMM"), "");

                        using (SqlConnection connection = new SqlConnection(Session["ConnString"].ToString()))
                        {
                            string query = "INSERT INTO ft_email_logs_reminder (CardCode, CardName, EmailTo, EmailCC, " +
                            "EmailSubject, EmailContent, ReminderDate, MinAmt, InvDaysFrom, InvDaysTo, LetterType, SendDate, SendBy, SendResult, ErrorDesc) " +
                            "VALUES ('" + CardCode.Replace("'", "''") + "', '" + CardName.Replace("'", "''") + "', '" + to + "', '" + cc + "', '" + subject.Replace("'", "''") + "', '" + content.Replace("'", "''") + "', '" +
                            reminderdate.ToString("yyyy-MM-dd") + "', '" + minamt + "', '" + invdaysfrom + "', '" + invdaysto + "', '" + lettertype + "', '" +
                            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "', '" +
                            Session["UserId"].ToString() + "', 'Success', '');";

                            SqlCommand command = new SqlCommand(query, connection);
                            command.Connection.Open();
                            command.ExecuteNonQuery();
                            command.Connection.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        using (SqlConnection connection = new SqlConnection(Session["ConnString"].ToString()))
                        {
                            string query = "INSERT INTO ft_email_logs_reminder (CardCode, CardName, EmailTo, EmailCC, " +
                            "EmailSubject, EmailContent, ReminderDate, MinAmt, InvDaysFrom, InvDaysTo, LetterType, SendDate, SendBy, SendResult, ErrorDesc) " +
                            "VALUES ('" + CardCode.Replace("'", "''") + "', '" + CardName.Replace("'", "''") + "', '" + to + "', '" + cc + "', '" + subject.Replace("'", "''") + "', '" + content.Replace("'", "''") + "', '" +
                            reminderdate.ToString("yyyy-MM-dd") + "', '" + minamt + "', '" + invdaysfrom + "', '" + invdaysto + "', '" + lettertype + "', '" +
                            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "', '" +
                            Session["UserId"].ToString() + "', 'Failed', '" + ex.Message + "');";

                            SqlCommand command = new SqlCommand(query, connection);
                            command.Connection.Open();
                            command.ExecuteNonQuery();
                            command.Connection.Close();

                        }
                        info = "Send E-Mail Error:" + ex.Message;
                        LabelDanger.Text = info;
                        ft_logs.WriteLine(Environment.NewLine + info, "Error");
                        inProcess = false;
                    }
                }
            }
            inProcess = false;
            ft_logs.WriteLine("✉ Send E-Mail ended!", "Info");
        }
        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkAll = (CheckBox)sender;
            ViewState["chkAll"] = chkAll.Checked;
            LReminderSelectedRow = new List<LReminderSelectedRow>();
            if (chkAll.Checked)
            {
                LReminderSelectedRow = LReminderSelectedRowAll;
            }
            else
            {
                LReminderSelectedRow.Clear();
            }
        }
        protected void chkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                LabelDanger.Text = "You has been logged out!";
                Response.Redirect("~/Default.aspx", false);
            }

            CheckBox chk = (CheckBox)sender;
            int selRowIndex = int.Parse(chk.Attributes["CommandArgument"]);
            int id = int.Parse(GridView1.DataKeys[selRowIndex]["ID"].ToString());
            Label cardcode = (Label)GridView1.Rows[selRowIndex].FindControl("LabelCardCode");
            Label cardname = (Label)GridView1.Rows[selRowIndex].FindControl("CardName");
            Label emailto = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailTo");
            Label emailcc = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailCC");
            Label emailsubject = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailSubject");
            Label emailcontent = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailContent");
            Label reminderdate = (Label)GridView1.Rows[selRowIndex].FindControl("lblReminderDate");
            Label minamt = (Label)GridView1.Rows[selRowIndex].FindControl("lblMinAmt");
            Label invdaysfrom = (Label)GridView1.Rows[selRowIndex].FindControl("lblInvDaysFrom");
            Label invdaysto = (Label)GridView1.Rows[selRowIndex].FindControl("lblInvDaysTo");
            Label lettertype = (Label)GridView1.Rows[selRowIndex].FindControl("lblLetterType");
            Label senddate = (Label)GridView1.Rows[selRowIndex].FindControl("lblSendDate");

            if (chk.Checked)
            {
                LReminderSelectedRow.Add(new LReminderSelectedRow
                {
                    ID = id,
                    CardCode = cardcode.Text,
                    CardName = cardname.Text,
                    EmailTo = emailto.Text,
                    EmailCC = emailcc.Text,
                    EmailSubject = emailsubject.Text,
                    EmailContent = emailcontent.Text,
                    ReminderDate = DateTime.ParseExact(reminderdate.Text, "dd/MM/yyyy", null),
                    MinAmt = decimal.Parse(minamt.Text),
                    InvDaysFrom = int.Parse(invdaysfrom.Text),
                    InvDaysTo = int.Parse(invdaysto.Text),
                    LetterType = lettertype.Text,
                    SendDate = DateTime.Parse(senddate.Text),
                    isChecked = chk.Checked
                });
            }
            else
            {
                LReminderSelectedRow.RemoveAll(x => x.ID == id);
            }
            LReminderSelectedRow.Select(x => x.ID).Distinct();
            LabelInfo.Text = "Selected : " + LReminderSelectedRow.Count();
        }
        // -- Web Page Rendering -----------------------------------------------------------------------
        protected void SqlDataSource_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            /* Get Total number of records */
            int First = (GridView1.PageIndex * GridView1.PageSize) + 1;
            int Last = First + GridView1.PageSize - 1;
            pagerInfo = " " + First + " - " + Last + " of " + totalRecords + " items";
            LabelPrimary.Text = pagerInfo;
        }
        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            if (inProcess) { return; }

            LReminderSelectedRowAll = new List<LReminderSelectedRow>();
            GridView1.AllowPaging = false;
            totalRecords = GridView1.Rows.Count;
            foreach (GridViewRow row in GridView1.Rows)
            {
                Label LabelID = (Label)row.FindControl("lblID");
                Label LabelCardcode = (Label)row.FindControl("LabelCardcode");
                Label LabelCardName = (Label)row.FindControl("CardName");
                Label LabelEmailTo = (Label)row.FindControl("lblEmailTo");
                Label LabelEmailCC = (Label)row.FindControl("lblEmailCC");
                Label LabelEmailSubject = (Label)row.FindControl("lblEmailSubject");
                Label LabelEmailContent = (Label)row.FindControl("lblEmailContent");
                Label LabelReminderDate = (Label)row.FindControl("lblReminderDate");
                Label LabelMinAmt = (Label)row.FindControl("lblMinAmt");
                Label LabelInvDaysFrom = (Label)row.FindControl("lblInvDaysFrom");
                Label LabelInvDaysTo = (Label)row.FindControl("lblInvDaysTo");
                Label LabelLetterType = (Label)row.FindControl("lblLetterType");
                Label LabelSendDate = (Label)row.FindControl("lblSendDate");
                CheckBox chkBox = (CheckBox)row.FindControl("chkBox");

                LReminderSelectedRowAll.Add(new LReminderSelectedRow
                {
                    ID = int.Parse(LabelID.Text),
                    CardCode = LabelCardcode.Text,
                    CardName = LabelCardName.Text,
                    EmailTo = LabelEmailTo.Text,
                    EmailCC = LabelEmailCC.Text,
                    EmailSubject = LabelEmailSubject.Text,
                    EmailContent = LabelEmailContent.Text,
                    ReminderDate = DateTime.ParseExact(LabelReminderDate.Text, "dd/MM/yyyy", null),
                    MinAmt = decimal.Parse(LabelMinAmt.Text),
                    InvDaysFrom = int.Parse(LabelInvDaysFrom.Text),
                    InvDaysTo = int.Parse(LabelInvDaysTo.Text),
                    LetterType = LabelLetterType.Text,
                    SendDate = DateTime.Parse(LabelSendDate.Text),
                    isChecked = true
                });
            }
            GridView1.AllowPaging = true;
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Populate check box from ViewState
            if (GridView1 != null)
            {
                if (ViewState["chkAll"] != null)
                {
                    CheckBox chkAll = (CheckBox)GridView1.HeaderRow.FindControl("chkAll");
                    chkAll.Checked = (Boolean)ViewState["chkAll"];

                    if (LReminderSelectedRow.Count() == 0)
                    {
                        chkAll.Checked = false;
                    }
                    if (LReminderSelectedRow.Count() == LReminderSelectedRowAll.Count())
                    {
                        chkAll.Checked = true;
                    }
                }
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string splitpattern = "[;,| ]";
                Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.CultureInvariant | RegexOptions.Singleline);
                CheckBox chkBox = e.Row.FindControl("chkBox") as CheckBox;
                Label lblEmailTo = e.Row.FindControl("lblEmailTo") as Label;
                Label lblEmailCC = e.Row.FindControl("lblEmailCC") as Label;
                Label LabelDanger = e.Row.FindControl("LabelDanger") as Label;

                AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
                trigger.ControlID = chkBox.UniqueID;
                trigger.EventName = "CheckedChanged";
                UpdatePanel1.Triggers.Add(trigger);

                if (lblEmailTo.Text == "")
                {
                    LabelDanger.Text = "Email not defined !";
                    chkBox.Enabled = false;
                }

                foreach (string t in Regex.Split(lblEmailTo.Text, splitpattern))
                {
                    if (t.Equals("")) { continue; }
                    bool isValidEmail = regex.IsMatch(t);
                    if (!isValidEmail)
                    {
                        LabelDanger.Text = "Invalid Email !";
                        chkBox.Enabled = false;
                    }
                }

                foreach (string t in Regex.Split(lblEmailCC.Text, splitpattern))
                {
                    if (t.Equals("")) { continue; }
                    bool isValidEmail = regex.IsMatch(t);
                    if (!isValidEmail)
                    {
                        LabelDanger.Text = "Invalid Email CC !";
                        chkBox.Enabled = false;
                    }
                }
            }
        }
        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Visible = true;
            }

            if (e.Row.RowType == DataControlRowType.Pager)
            {
                DropDownList DDL = new DropDownList();
                DDL.Items.Add("10");
                DDL.Items.Add("20");
                DDL.Items.Add("30");
                DDL.Items.Add("40");
                DDL.Items.Add("50");
                DDL.Items.Add("60");
                DDL.Items.Add("70");
                DDL.Items.Add("80");
                DDL.Items.Add("90");
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

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            if (dt == null) return;
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                CheckBox chkBox = (CheckBox)GridView1.Rows[i].FindControl("chkBox");
                Label LabelID = (Label)GridView1.Rows[i].FindControl("lblID");
                Label LabelLastSent = (Label)GridView1.Rows[i].FindControl("lblLastSent");
                Label LabelDanger = (Label)GridView1.Rows[i].FindControl("LabelDanger");
                Label LabelWarning = (Label)GridView1.Rows[i].FindControl("LabelWarning");
                Label LabelInfo = (Label)GridView1.Rows[i].FindControl("LabelInfo");

                int ID = int.Parse(LabelID.Text.Trim());

                // ChkBox Rendering
                if (LReminderSelectedRow.Exists(x => x.ID == ID) && !chkBox.Enabled)
                {
                    LReminderSelectedRow.RemoveAll(x => x.ID == ID);
                    LReminderSelectedRowAll.RemoveAll(x => x.ID == ID);
                    chkBox.Checked = false;
                }
                else if (LReminderSelectedRow.Exists(x => x.ID == ID) && chkBox.Enabled)
                {
                    chkBox.Checked = true;
                }
                else
                {
                    chkBox.Checked = false;
                }
            }
            if (LReminderSelectedRow.Count() > 0)
            {
                LabelInfo.Text = "Selected records : " + LReminderSelectedRow.Count();
            }
        }
    }
}