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
    public partial class EmailReceiptLogs : System.Web.UI.Page
    {
        protected static Dictionary<string, List<FileInfo>> UserFiles = new Dictionary<string, List<FileInfo>>();
        protected static Dictionary<string, List<FileInfo>> Documents = new Dictionary<string, List<FileInfo>>();
        protected static DataTable dt;
        protected static List<string> SelectedCardCode = new List<string>();
        protected static List<string> SelectedCardCodeAll = new List<string>();
        protected static List<string> noRecordCardCode = new List<string>();
        protected static List<string> docPath = new List<string>();
        protected static List<LReceiptSelectedRow> LReceiptSelectedRow = new List<LReceiptSelectedRow>();
        protected static List<LReceiptSelectedRow> LReceiptSelectedRowAll = new List<LReceiptSelectedRow>();
        protected int totalRecords = 0;
        protected String pagerInfo = "";
        protected static string folderDir = "";
        protected static string info;
        protected static string warning;
        protected static bool inProcess = false;
        protected static DateTime senddateFr;
        protected static DateTime senddateTo;
        protected static DateTime dateFr;
        protected static DateTime dateTo;
        protected static string senddateFrStr = "";
        protected static string senddateToStr = "";
        protected static string dateFrStr = "";
        protected static string dateToStr = "";
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
            }
        }
        protected void btnSelect_Click(object sender, EventArgs e)
        {
            int a = GridView1.PageIndex;
            LReceiptSelectedRow.Clear();
            LabelInfo.Text = "";
            foreach (GridViewRow row in GridView1.Rows)
            {
                Label LabelID = (Label)row.FindControl("lblID");
                Label LabelDocEntry = (Label)row.FindControl("lblDocEntry");
                Label LabelDocNum = (Label)row.FindControl("lblDocNum");
                Label LabelCardcode = (Label)row.FindControl("LabelCardcode");
                Label LabelCardName = (Label)row.FindControl("CardName");
                Label LabelEmailTo = (Label)row.FindControl("lblEmailTo");
                Label LabelEmailCC = (Label)row.FindControl("lblEmailCC");
                Label LabelEmailSubject = (Label)row.FindControl("lblEmailSubject");
                Label LabelEmailContent = (Label)row.FindControl("lblEmailContent");
                Label LabelReceiptDate = (Label)row.FindControl("lblReceiptDate");
                CheckBox chkBox = (CheckBox)row.FindControl("chkBox");

                LReceiptSelectedRow.Add(new LReceiptSelectedRow
                {
                    ID = int.Parse(LabelID.Text),
                    DocEntry = int.Parse(LabelDocEntry.Text),
                    DocNum = LabelDocNum.Text,
                    CardCode = LabelCardcode.Text,
                    CardName = LabelCardName.Text,
                    EmailTo = LabelEmailTo.Text,
                    EmailCC = LabelEmailCC.Text,
                    EmailSubject = LabelEmailSubject.Text,
                    EmailContent = LabelEmailContent.Text,
                    ReceiptDate = DateTime.ParseExact(LabelReceiptDate.Text, "dd/MM/yyyy", null),
                    isChecked = chkBox.Checked
                });

                LabelInfo.Text = "Selected : " + LReceiptSelectedRow.Count();
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
            ft_logs.WriteLine("Perform search for Receipt log Date From: " + txtSendDateFr.Text + " To: " + txtSendDateTo.Text, "Info");
            init();
            LReceiptSelectedRow.Clear();
            SqlDataSource.SelectParameters.Clear();
            try
            {
                senddateFr = DateTime.ParseExact(txtSendDateFr.Text, "dd/MM/yyyy", null);
                senddateTo = DateTime.ParseExact(txtSendDateTo.Text, "dd/MM/yyyy", null);

                if (txtDateFr.Text != "")
                    dateFr = DateTime.ParseExact(txtDateFr.Text, "dd/MM/yyyy", null);
                else
                    dateFr = DateTime.ParseExact("01/01/1900", "dd/MM/yyyy", null);

                if (txtDateTo.Text != "")
                    dateTo = DateTime.ParseExact(txtDateTo.Text, "dd/MM/yyyy", null);
                else
                    dateTo = DateTime.ParseExact("01/01/1900", "dd/MM/yyyy", null);

                senddateFrStr = senddateFr.ToString("yyyyMMdd");
                senddateToStr = senddateTo.ToString("yyyyMMdd");
                dateFrStr = dateFr.ToString("yyyyMMdd");
                dateToStr = dateTo.ToString("yyyyMMdd");
                strFr = txtCardCodeFr.Text.Equals("") ? "*" : txtCardCodeFr.Text;
                strTo = txtCardCodeTo.Text.Equals("") ? "*" : txtCardCodeTo.Text;
                sendResult = ddlSendResult.SelectedValue.ToString();

                SqlDataSource.SelectParameters.Add("senddateFrStr", senddateFrStr);
                SqlDataSource.SelectParameters.Add("senddateToStr", senddateToStr);
                SqlDataSource.SelectParameters.Add("dateFrStr", dateFrStr);
                SqlDataSource.SelectParameters.Add("dateToStr", dateToStr);
                SqlDataSource.SelectParameters.Add("strFr", strFr);
                SqlDataSource.SelectParameters.Add("strTo", strTo);
                SqlDataSource.SelectParameters.Add("sendResult", sendResult);

                GridView1.DataBind();

                using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
                {
                    using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                    {
                        query = "SELECT * FROM [FTS_fn_ReceiptLogs] ('" + senddateFrStr + "', '" + senddateToStr + "', '" + dateFrStr + "', '" + dateToStr + "', '" + strFr + "', '" + strTo + "', '" + sendResult + "') T0 ORDER BY T0.SendDate DESC, T0.DocEntry ";

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
                logs.Screen = "ReceiptLog";
                logs.MessageType = "Error";
                logs.Message = ex.Message;
                logs.UserSign = lblUser.Text;
                logs.TimeStamp = DateTime.Now;
                logs.Remarks = "Search Error";
                logs.DBName = LabelCompany.Text;
                logs.SaveLogs(Session["ConnString"].ToString());
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
            if (LReceiptSelectedRow.Count <= 0)
            {
                LabelDanger.Text = "No record is selected!";
                return;
            }

            Timer1.Enabled = true;
            btnSend.Enabled = false;
            imgLoading.Visible = true;
            info = "App starting...";
            try
            {
                Thread workerThread = new Thread(new ThreadStart(MainTask));
                workerThread.Start();
            }
            catch (ThreadInterruptedException ex)
            {
                LabelDanger.Text = ex.Message;
                info = ex.Message;
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
        protected void MainTask()
        {
            inProcess = true;
            ft_logs.dir = "C:\\WebMail\\[" + LabelCompany.Text + "] Logs";
            ft_logs.WriteLine("-- Log started at " + DateTime.Now.ToString("hh:mm:ss tt") + " --------------------------------------------", "");
            ft_logs.WriteLine("User:    " + lblUser.Text, "");
            ft_logs.WriteLine("Company: " + LabelCompany.Text, "");
            ft_logs.WriteLine("Log Receipt send Date From:  " + txtSendDateFr.Text + " To: " + txtSendDateTo.Text, "");
            ft_logs.WriteLine("-------------------------------------------------------------------------------", "");
            // Core Function
            GenerateDocument();
            SendEmail();

            if (Directory.Exists(folderDir))
            {
                Directory.Delete(folderDir, true);
            }

            inProcess = false;
            LReceiptSelectedRow.Clear();
            LabelInfo.Text = "";
            ViewState["chkAll"] = null;
            SqlDataSource.SelectCommand = query;
            GridView1.DataBind();
            GC.Collect();
            ft_logs.WriteLine("-- Log ended at " + DateTime.Now.ToString("hh:mm:ss tt") + " --------------------------------------------", "");
        }
        protected void GenerateDocument()
        {
            ft_logs.WriteLine("Generate receipt begin...", "Info");

            string layoutName = LabelCompany.Text + "_Receipt";


            folderDir = "C:\\EmailPortal\\[" + LabelCompany.Text + "] " + lblUser.Text + "\\LReceipt\\" + DateTime.Now.ToString("yyyymmdd");

            try
            {
                ft_logs.WriteLine("① Clear attachment folder...", "Info");
                foreach (int ID in LReceiptSelectedRow.Select(x => x.ID).Distinct())
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

                for (int i = 0; i < LReceiptSelectedRow.Count; i++)
                {
                    Checked = LReceiptSelectedRow[i].isChecked;

                    if (Checked)
                    {
                        string ID = LReceiptSelectedRow[i].ID.ToString();
                        int DocEntry = LReceiptSelectedRow[i].DocEntry;
                        string DocNum = LReceiptSelectedRow[i].DocNum;
                        string receiptdateStr = LReceiptSelectedRow[i].ReceiptDate.ToString("yyyy-MM-dd");
                        string CardCode = LReceiptSelectedRow[i].CardCode;
                        string CardName = LReceiptSelectedRow[i].CardName;

                        string path = folderDir + "\\" + ID + "\\";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        info = "③ Generate Receipt [" + (i + 1).ToString() + "/" + LReceiptSelectedRow.Count() + "] ➜ [" + CardCode + "-" + CardName + "]";
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
                            if (param.Name.Equals("DocKey@"))
                            {
                                crystalReport.SetParameterValue("DocKey@", DocEntry);
                                continue;
                            }
                        }

                        string ExportPath = path + "LRCT_" + CardCode + "_" + DocNum + ".pdf";
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
                ft_logs.WriteLine("Generating Receipt error:" + ex.Message, "Error");
                inProcess = false;
            }
            finally
            {
                ft_logs.WriteLine("Generate Receipt ended!", "Info");
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

            for (int i = 0; i < LReceiptSelectedRow.Count; i++)
            {
                LReceiptSelectedRow lreceiptSelectedObj = LReceiptSelectedRow[i];
                Checked = lreceiptSelectedObj.isChecked;

                if (Checked)
                {
                    string ID = lreceiptSelectedObj.ID.ToString();
                    string DocEntry = lreceiptSelectedObj.DocEntry.ToString();
                    string DocNum = lreceiptSelectedObj.DocNum;
                    DateTime DocDate = lreceiptSelectedObj.ReceiptDate;
                    string CardCode = lreceiptSelectedObj.CardCode;
                    string CardName = lreceiptSelectedObj.CardName;

                    string to = lreceiptSelectedObj.EmailTo;
                    string cc = lreceiptSelectedObj.EmailCC;

                    info = "④ Sending Receipt [" + (i + 1).ToString() + "/" + LReceiptSelectedRow.Count() + "] ➜ [" + CardCode + "-" + CardName + "-" + DocNum + "] TO: [" + to + "] CC: [" + cc + "]";
                    ft_logs.Write(info, "Info");

                    string subject = lreceiptSelectedObj.EmailSubject;
                    subject = subject.Replace(@"[CardName]", CardName);
                    subject = subject.Replace(@"[DD]", DocDate.ToString("dd"));
                    subject = subject.Replace(@"[MMM]", DocDate.ToString("MMM"));
                    subject = subject.Replace(@"[YYYY]", DocDate.ToString("yyyy"));

                    string content = lreceiptSelectedObj.EmailContent;
                    content = content.Replace(@"[DD]", DocDate.ToString("dd"));
                    content = content.Replace(@"[MMM]", DocDate.ToString("MMM"));
                    content = content.Replace(@"[YYYY]", DocDate.ToString("yyyy"));

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

                        SmtpClient oSTMP = new SmtpClient();
                        oSTMP.Host = smtp;
                        oSTMP.Port = int.Parse(port);
                        oSTMP.EnableSsl = ssl;
                        oSTMP.Credentials = new NetworkCredential(user, pass);
                        oSTMP.Send(oMail);
                        oSTMP.Dispose();
                        oMail.Attachments.Dispose();

                        ft_logs.WriteLine("➜✉ E-Mail Sent " + CardCode + "-" + CardName + "-" + DocNum + " @ " + DateTime.Today.ToString("d MMM"), "");

                        using (SqlConnection connection = new SqlConnection(Session["ConnString"].ToString()))
                        {
                            string query = "INSERT INTO ft_email_logs_receipt (DocEntry, DocNum, CardCode, CardName, EmailTo, EmailCC, " +
                            "EmailSubject, EmailContent, ReceiptDate, SendDate, SendBy, SendResult, ErrorDesc) " +
                            "VALUES ('" + DocEntry + "', '" + DocNum + "', '" + CardCode + "', '" + CardName + "', '" + to + "', '" + cc + "', '" +
                            subject + "', '" + content + "', '" + DocDate.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "', '" + Session["UserId"].ToString() + "', 'Success', '');";

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
                            string query = "INSERT INTO ft_email_logs_receipt (DocEntry, DocNum, CardCode, CardName, EmailTo, EmailCC, " +
                            "EmailSubject, EmailContent, ReceiptDate, SendDate, SendBy, SendResult, ErrorDesc) " +
                            "VALUES ('" + DocEntry + "', '" + DocNum + "', '" + CardCode + "', '" + CardName + "', '" + to + "', '" + cc + "', '" +
                            subject + "', '" + content + "', '" + DocDate.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "', '" + Session["UserId"].ToString() + "', 'Failed', '" +
                            ex.Message + "');";

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
            ft_logs.WriteLine("✉ Send E-Mail ended!", "Info");
        }
        protected void init()
        {
            LabelDanger.Text = "";
            LabelWarning.Text = "";
            LabelInfo.Text = "";
            LabelDefault.Text = "";
            LabelPrimary.Text = "";
        }
        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkAll = (CheckBox)sender;
            ViewState["chkAll"] = chkAll.Checked;
            LReceiptSelectedRow = new List<LReceiptSelectedRow>();
            if (chkAll.Checked)
            {
                LReceiptSelectedRow = LReceiptSelectedRowAll;
            }
            else
            {
                LReceiptSelectedRow.Clear();
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
            Label docentry = (Label)GridView1.Rows[selRowIndex].FindControl("lblDocEntry");
            Label docnum = (Label)GridView1.Rows[selRowIndex].FindControl("lblDocNum");
            Label cardcode = (Label)GridView1.Rows[selRowIndex].FindControl("LabelCardCode");
            Label cardname = (Label)GridView1.Rows[selRowIndex].FindControl("CardName");
            Label emailto = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailTo");
            Label emailcc = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailCC");
            Label emailsubject = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailSubject");
            Label emailcontent = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailContent");
            Label receiptdate = (Label)GridView1.Rows[selRowIndex].FindControl("lblReceiptDate");

            if (chk.Checked)
            {
                LReceiptSelectedRow.Add(new LReceiptSelectedRow
                {
                    ID = id,
                    DocEntry = int.Parse(docentry.Text),
                    DocNum = docnum.Text,
                    CardCode = cardcode.Text,
                    CardName = cardname.Text,
                    EmailTo = emailto.Text,
                    EmailCC = emailcc.Text,
                    EmailSubject = emailsubject.Text,
                    EmailContent = emailcontent.Text,
                    ReceiptDate = DateTime.ParseExact(receiptdate.Text, "dd/MM/yyyy", null),
                    isChecked = chk.Checked
                });
            }
            else
            {
                LReceiptSelectedRow.RemoveAll(x => x.ID == id);
            }
            LReceiptSelectedRow.Select(x => x.ID).Distinct();
            LabelInfo.Text = "Selected : " + LReceiptSelectedRow.Count();
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

            LReceiptSelectedRowAll = new List<LReceiptSelectedRow>();
            GridView1.AllowPaging = false;
            totalRecords = GridView1.Rows.Count;
            foreach (GridViewRow row in GridView1.Rows)
            {
                Label LabelID = (Label)row.FindControl("lblID");
                Label LabelDocEntry = (Label)row.FindControl("lblDocEntry");
                Label LabelDocNum = (Label)row.FindControl("lblDocNum");
                Label LabelCardcode = (Label)row.FindControl("LabelCardcode");
                Label LabelCardName = (Label)row.FindControl("CardName");
                Label LabelEmailTo = (Label)row.FindControl("lblEmailTo");
                Label LabelEmailCC = (Label)row.FindControl("lblEmailCC");
                Label LabelEmailSubject = (Label)row.FindControl("lblEmailSubject");
                Label LabelEmailContent = (Label)row.FindControl("lblEmailContent");
                Label LabelReceiptDate = (Label)row.FindControl("lblReceiptDate");
                CheckBox chkBox = (CheckBox)row.FindControl("chkBox");

                LReceiptSelectedRowAll.Add(new LReceiptSelectedRow
                {
                    ID = int.Parse(LabelID.Text),
                    DocEntry = int.Parse(LabelDocEntry.Text),
                    DocNum = LabelDocNum.Text,
                    CardCode = LabelCardcode.Text,
                    CardName = LabelCardName.Text,
                    EmailTo = LabelEmailTo.Text,
                    EmailCC = LabelEmailCC.Text,
                    EmailSubject = LabelEmailSubject.Text,
                    EmailContent = LabelEmailContent.Text,
                    ReceiptDate = DateTime.ParseExact(LabelReceiptDate.Text, "dd/MM/yyyy", null),
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

                    if (LReceiptSelectedRow.Count() == 0)
                    {
                        chkAll.Checked = false;
                    }
                    if (LReceiptSelectedRow.Count() == LReceiptSelectedRowAll.Count())
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
                if (LReceiptSelectedRow.Exists(x => x.ID == ID) && !chkBox.Enabled)
                {
                    LReceiptSelectedRow.RemoveAll(x => x.ID == ID);
                    LReceiptSelectedRowAll.RemoveAll(x => x.ID == ID);
                    chkBox.Checked = false;
                }
                else if (LReceiptSelectedRow.Exists(x => x.ID == ID) && chkBox.Enabled)
                {
                    chkBox.Checked = true;
                }
                else
                {
                    chkBox.Checked = false;
                }
            }
            if (LReceiptSelectedRow.Count() > 0)
            {
                LabelInfo.Text = "Selected records : " + LReceiptSelectedRow.Count();
            }
        }
    }
}