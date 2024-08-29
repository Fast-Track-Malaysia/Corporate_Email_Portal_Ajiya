using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraReports.UI;
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
    public partial class EmailInvoiceLogs : System.Web.UI.Page
    {
        protected static Dictionary<string, List<FileInfo>> UserFiles = new Dictionary<string, List<FileInfo>>();
        protected static Dictionary<string, List<FileInfo>> Documents = new Dictionary<string, List<FileInfo>>();
        protected static DataTable dt;
        protected static List<string> SelectedCardCode = new List<string>();
        protected static List<string> SelectedCardCodeAll = new List<string>();
        protected static List<string> noRecordCardCode = new List<string>();
        protected static List<string> docPath = new List<string>();
        protected static List<LInvoiceSelectedRow> LInvoiceSelectedRow = new List<LInvoiceSelectedRow>();
        protected static List<LInvoiceSelectedRow> LInvoiceSelectedRowAll = new List<LInvoiceSelectedRow>();
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
        private XafReport xafReport { get; set; }

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
            LInvoiceSelectedRow.Clear();
            LabelInfo.Text = "";
            foreach (GridViewRow row in GridView1.Rows)
            {
                Label LabelID = (Label)row.FindControl("lblID");
                Label LabelDocEntry = (Label)row.FindControl("lblDocEntry");
                Label LabelDocNum = (Label)row.FindControl("lblDocNum");
                Label LabelPortalOid= (Label)row.FindControl("lblPortalOid");
                Label LabelPortalDocNum = (Label)row.FindControl("lblPortalDocNum");
                Label LabelCardcode = (Label)row.FindControl("LabelCardcode");
                Label LabelCardName = (Label)row.FindControl("CardName");
                Label LabelEmailTo = (Label)row.FindControl("lblEmailTo");
                Label LabelEmailCC = (Label)row.FindControl("lblEmailCC");
                Label LabelEmailSubject = (Label)row.FindControl("lblEmailSubject");
                Label LabelEmailContent = (Label)row.FindControl("lblEmailContent");
                Label LabelInvoiceDate = (Label)row.FindControl("lblInvoiceDate");
                CheckBox chkBox = (CheckBox)row.FindControl("chkBox");

                LInvoiceSelectedRow.Add(new LInvoiceSelectedRow
                {
                    ID = int.Parse(LabelID.Text),
                    DocEntry = int.Parse(LabelDocEntry.Text),
                    DocNum = LabelDocNum.Text,
                    PortalOid = int.Parse(LabelPortalOid.Text),
                    PortalDocNum = LabelPortalDocNum.Text,
                    CardCode = LabelCardcode.Text,
                    CardName = LabelCardName.Text,
                    EmailTo = LabelEmailTo.Text,
                    EmailCC = LabelEmailCC.Text,
                    EmailSubject = LabelEmailSubject.Text,
                    EmailContent = LabelEmailContent.Text,
                    InvoiceDate = DateTime.ParseExact(LabelInvoiceDate.Text, "dd/MM/yyyy", null),
                    isChecked = chkBox.Checked
                });

                LabelInfo.Text = "Selected : " + LInvoiceSelectedRow.Count();
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
            ft_logs.WriteLine("Perform search for Invoice log Date From: " + txtSendDateFr.Text + " To: " + txtSendDateTo.Text, "Info");
            init();
            LInvoiceSelectedRow.Clear();
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
                        query = "SELECT * FROM [FTS_fn_InvoiceLogs] ('" + senddateFrStr + "', '" + senddateToStr + "', '"+ dateFrStr + "', '"+ dateToStr + "', '" + strFr + "', '" + strTo + "', '" + sendResult + "') T0 ORDER BY T0.SendDate DESC, T0.PortalDocNum ";

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
                logs.Screen = "InvoiceLog";
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
            if (LInvoiceSelectedRow.Count <= 0)
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
            ft_logs.WriteLine("Log Invoice send Date From:  " + txtSendDateFr.Text + " To: " + txtSendDateTo.Text, "");
            ft_logs.WriteLine("-------------------------------------------------------------------------------", "");
            // Core Function
            GenerateDocument();
            SendEmail();

            if (Directory.Exists(folderDir))
            {
                Directory.Delete(folderDir, true);
            }

            inProcess = false;
            LInvoiceSelectedRow.Clear();
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
            ft_logs.WriteLine("Generate invoice begin...", "Info");

            string layoutName = LabelCompany.Text + "_Invoice";


            folderDir = "C:\\EmailPortal\\[" + LabelCompany.Text + "] " + lblUser.Text + "\\LInvoice\\" + DateTime.Now.ToString("yyyymmdd");

            try
            {
                ft_logs.WriteLine("① Clear attachment folder...", "Info");
                foreach (int ID in LInvoiceSelectedRow.Select(x => x.ID).Distinct())
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

                for (int i = 0; i < LInvoiceSelectedRow.Count; i++)
                {
                    Checked = LInvoiceSelectedRow[i].isChecked;

                    if (Checked)
                    {
                        string ID = LInvoiceSelectedRow[i].ID.ToString();
                        int DocEntry = LInvoiceSelectedRow[i].DocEntry;
                        string DocNum = LInvoiceSelectedRow[i].DocNum;
                        int PortalOid = LInvoiceSelectedRow[i].PortalOid;
                        string PortalDocNum = LInvoiceSelectedRow[i].PortalDocNum;
                        string invoicedateStr = LInvoiceSelectedRow[i].InvoiceDate.ToString("yyyy-MM-dd");
                        string CardCode = LInvoiceSelectedRow[i].CardCode;
                        string CardName = LInvoiceSelectedRow[i].CardName;

                        string path = folderDir + "\\" + ID + "\\";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        info = "③ Generate Invoice [" + (i + 1).ToString() + "/" + LInvoiceSelectedRow.Count() + "] ➜ [" + CardCode + "-" + CardName + "]";
                        ft_logs.Write(info, "Info");

                        string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                        Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                        CardName = r.Replace(CardName, "");

                        #region XAF report
                        /*
                        xafReport = new XafReport(Session["AjiyaDBConnString"].ToString(), WebConfigurationManager.AppSettings.Get("XafSource"));
                        IObjectSpace objectSpace = xafReport.objectSpaceProvider.CreateObjectSpace();
                        ReportDataV2 reportData = objectSpace.FindObject<ReportDataV2>(CriteriaOperator.Parse("DisplayName=?", WebConfigurationManager.AppSettings.Get("InvoiceDisplayName")));// .FirstOrDefault<ReportDataV2>(data => data.DisplayName == sourcefile);
                        XtraReport report = ReportDataProvider.ReportsStorage.LoadReport(reportData);
                        MyReportDataSourceHelper reportDataSourceHelper = new MyReportDataSourceHelper(xafReport.objectSpaceProvider);
                        ReportDataProvider.ReportObjectSpaceProvider = new MyReportObjectSpaceProvider(xafReport.objectSpaceProvider);

                        DevExpress.XtraReports.Parameters.ParameterCollection prmc = report.Parameters;
                        CriteriaOperator op = null;

                        op = CriteriaOperator.Parse("Oid=?", PortalOid);

                        foreach (DevExpress.XtraReports.Parameters.Parameter parameter in prmc)
                        {
                            if (parameter.Name == "Oid")
                            {
                                parameter.Value = PortalOid;
                            }
                        }

                        string ExportPath = path + "LINV_" + CardCode + "_" + PortalDocNum + ".pdf";
                        reportDataSourceHelper.SetupBeforePrint(report, null, op, true, null, false);
                        report.ExportToPdf(ExportPath);
                        xafReport = null;
                        */
                        #endregion

                        #region Crystal report

                        ReportDocument crystalReport = new ReportDocument();
                        crystalReport.Load(Server.MapPath("~/Reports/" + layoutName + ".rpt"));

                        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Session["SAPDBConnString"].ToString());
                        crystalReport.DataSourceConnections[0].SetConnection(builder.DataSource, builder.InitialCatalog, builder.UserID, builder.Password);

                        ParameterFieldDefinitions crParameterdef;
                        crParameterdef = crystalReport.DataDefinition.ParameterFields;

                        foreach (ParameterFieldDefinition param in crParameterdef)
                        {
                            if (param.Name.Equals("DocKey@"))
                            {
                                crystalReport.SetParameterValue("DocKey@", DocEntry);
                                continue;
                            }

                            if (param.Name.Equals("ObjectId@"))
                            {
                                crystalReport.SetParameterValue("ObjectId@", 13);
                                continue;
                            }
                        }

                        string ExportPath = path + "LINV_" + CardCode + "_" + DocNum + ".pdf";
                        crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, ExportPath);
                        docPath.Add(ExportPath);
                        crystalReport.Dispose();
                     
                        #endregion

                        ft_logs.WriteLine(" Folder:[" + ExportPath + "]", "");
                    }
                }
            }
            catch (Exception ex)
            {
                LabelDanger.Text = ex.Message;
                ft_logs.WriteLine("Generating Invoice error:" + ex.Message, "Error");
                inProcess = false;
            }
            finally
            {
                ft_logs.WriteLine("Generate Invoice ended!", "Info");
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

            for (int i = 0; i < LInvoiceSelectedRow.Count; i++)
            {
                LInvoiceSelectedRow linvoiceSelectedObj = LInvoiceSelectedRow[i];
                Checked = linvoiceSelectedObj.isChecked;

                if (Checked)
                {
                    string ID = linvoiceSelectedObj.ID.ToString();
                    string DocEntry = linvoiceSelectedObj.DocEntry.ToString();
                    string DocNum = linvoiceSelectedObj.DocNum;
                    string PortalOid = linvoiceSelectedObj.PortalOid.ToString();
                    string PortalDocNum = linvoiceSelectedObj.PortalDocNum;
                    DateTime DocDate = linvoiceSelectedObj.InvoiceDate;
                    string CardCode = linvoiceSelectedObj.CardCode;
                    string CardName = linvoiceSelectedObj.CardName;

                    string to = linvoiceSelectedObj.EmailTo;
                    string cc = linvoiceSelectedObj.EmailCC;

                    info = "④ Sending Invoice [" + (i + 1).ToString() + "/" + LInvoiceSelectedRow.Count() + "] ➜ [" + CardCode + "-" + CardName + "-" + DocNum + "] TO: [" + to + "] CC: [" + cc + "]";
                    ft_logs.Write(info, "Info");

                    string subject = linvoiceSelectedObj.EmailSubject;
                    subject = subject.Replace(@"[CardName]", CardName);
                    subject = subject.Replace(@"[DD]", DocDate.ToString("dd"));
                    subject = subject.Replace(@"[MM]", DocDate.ToString("MM"));
                    subject = subject.Replace(@"[MMM]", DocDate.ToString("MMM"));
                    subject = subject.Replace(@"[YYYY]", DocDate.ToString("yyyy"));
                    subject = subject.Replace(@"[InvoiceNo]", PortalDocNum);
                    subject = subject.Replace(@"[DocNum]", DocNum);

                    string content = linvoiceSelectedObj.EmailContent;
                    content = content.Replace(@"[DD]", DocDate.ToString("dd"));
                    content = content.Replace(@"[MM]", DocDate.ToString("MM"));
                    content = content.Replace(@"[MMM]", DocDate.ToString("MMM"));
                    content = content.Replace(@"[YYYY]", DocDate.ToString("yyyy"));
                    content = content.Replace(@"[InvoiceNo]", PortalDocNum);
                    content = content.Replace(@"[DocNum]", DocNum);

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
                                string query = "INSERT INTO ft_email_logs_invoice (DocEntry, DocNum, PortalOid, PortalDocNum, CardCode, CardName, EmailTo, EmailCC, " +
                                "EmailSubject, EmailContent, InvoiceDate, SendDate, SendBy, SendResult, ErrorDesc) " +
                                "VALUES ('" + DocEntry + "', '" + DocNum + "', '" + PortalOid + "', '" + PortalDocNum + "', '" + CardCode.Replace("'", "''") + "', '" + CardName.Replace("'", "''") + "', '" + to + "', '" + cc + "', '" +
                                subject.Replace("'", "''") + "', '" + content.Replace("'", "''") + "', '" + DocDate.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "', '" + Session["UserId"].ToString() + "', 'Failed', '" +
                                "No attachment." + "');";

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

                        ft_logs.WriteLine("➜✉ E-Mail Sent " + CardCode + "-" + CardName + "-" + DocNum + " @ " + DateTime.Today.ToString("d MMM"), "");

                        using (SqlConnection connection = new SqlConnection(Session["ConnString"].ToString()))
                        {
                            string query = "INSERT INTO ft_email_logs_invoice (DocEntry, DocNum, PortalOid, PortalDocNum, CardCode, CardName, EmailTo, EmailCC, " +
                            "EmailSubject, EmailContent, InvoiceDate, SendDate, SendBy, SendResult, ErrorDesc) " +
                            "VALUES ('" + DocEntry + "', '" + DocNum + "', '"+ PortalOid + "', '" + PortalDocNum + "', '" + CardCode.Replace("'", "''") + "', '" + CardName.Replace("'", "''") + "', '" + to + "', '" + cc + "', '" +
                            subject.Replace("'", "''") + "', '" + content.Replace("'", "''") + "', '" + DocDate.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "', '" + Session["UserId"].ToString() + "', 'Success', '');";

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
                            string query = "INSERT INTO ft_email_logs_invoice (DocEntry, DocNum, PortalOid,PortalDocNum, CardCode, CardName, EmailTo, EmailCC, " +
                            "EmailSubject, EmailContent, InvoiceDate, SendDate, SendBy, SendResult, ErrorDesc) " +
                            "VALUES ('" + DocEntry + "', '" + DocNum + "', '"+ PortalOid + "', '" + PortalDocNum + "', '" + CardCode.Replace("'", "''") + "', '" + CardName.Replace("'", "''") + "', '" + to + "', '" + cc + "', '" +
                            subject.Replace("'", "''") + "', '" + content.Replace("'", "''") + "', '" + DocDate.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "', '" + Session["UserId"].ToString() + "', 'Failed', '" +
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
            inProcess = false;
            ft_logs.WriteLine("✉ Send E-Mail ended!", "Info");
        }
        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkAll = (CheckBox)sender;
            ViewState["chkAll"] = chkAll.Checked;
            LInvoiceSelectedRow = new List<LInvoiceSelectedRow>();
            if (chkAll.Checked)
            {
                LInvoiceSelectedRow = LInvoiceSelectedRowAll;
            }
            else
            {
                LInvoiceSelectedRow.Clear();
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
            Label portaloid = (Label)GridView1.Rows[selRowIndex].FindControl("lblPortalOid");
            Label portaldocnum = (Label)GridView1.Rows[selRowIndex].FindControl("lblPortalDocNum");
            Label cardcode = (Label)GridView1.Rows[selRowIndex].FindControl("LabelCardCode");
            Label cardname = (Label)GridView1.Rows[selRowIndex].FindControl("CardName");
            Label emailto = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailTo");
            Label emailcc = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailCC");
            Label emailsubject = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailSubject");
            Label emailcontent = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailContent");
            Label invoicedate = (Label)GridView1.Rows[selRowIndex].FindControl("lblInvoiceDate");

            if (chk.Checked)
            {
                LInvoiceSelectedRow.Add(new LInvoiceSelectedRow
                {
                    ID = id,
                    DocEntry = int.Parse(docentry.Text),
                    DocNum = docnum.Text,
                    PortalOid = int.Parse(portaloid.Text),
                    PortalDocNum = portaldocnum.Text,
                    CardCode = cardcode.Text,
                    CardName = cardname.Text,
                    EmailTo = emailto.Text,
                    EmailCC = emailcc.Text,
                    EmailSubject = emailsubject.Text,
                    EmailContent = emailcontent.Text,
                    InvoiceDate = DateTime.ParseExact(invoicedate.Text, "dd/MM/yyyy", null),
                    isChecked = chk.Checked
                });
            }
            else
            {
                LInvoiceSelectedRow.RemoveAll(x => x.ID == id);
            }
            LInvoiceSelectedRow.Select(x => x.ID).Distinct();
            LabelInfo.Text = "Selected : " + LInvoiceSelectedRow.Count();
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

            LInvoiceSelectedRowAll = new List<LInvoiceSelectedRow>();
            GridView1.AllowPaging = false;
            totalRecords = GridView1.Rows.Count;
            foreach (GridViewRow row in GridView1.Rows)
            {
                Label LabelID = (Label)row.FindControl("lblID");
                Label LabelDocEntry = (Label)row.FindControl("lblDocEntry");
                Label LabelDocNum = (Label)row.FindControl("lblDocNum");
                Label LabelPortalOid = (Label)row.FindControl("lblPortalOid");
                Label LabelPortalDocNum = (Label)row.FindControl("lblPortalDocNum");
                Label LabelCardcode = (Label)row.FindControl("LabelCardcode");
                Label LabelCardName = (Label)row.FindControl("CardName");
                Label LabelEmailTo = (Label)row.FindControl("lblEmailTo");
                Label LabelEmailCC = (Label)row.FindControl("lblEmailCC");
                Label LabelEmailSubject = (Label)row.FindControl("lblEmailSubject");
                Label LabelEmailContent = (Label)row.FindControl("lblEmailContent");
                Label LabelInvoiceDate = (Label)row.FindControl("lblInvoiceDate");
                CheckBox chkBox = (CheckBox)row.FindControl("chkBox");

                LInvoiceSelectedRowAll.Add(new LInvoiceSelectedRow
                {
                    ID = int.Parse(LabelID.Text),
                    DocEntry = int.Parse(LabelDocEntry.Text),
                    DocNum = LabelDocNum.Text,
                    PortalOid = int.Parse(LabelPortalOid.Text),
                    PortalDocNum = LabelPortalDocNum.Text,
                    CardCode = LabelCardcode.Text,
                    CardName = LabelCardName.Text,
                    EmailTo = LabelEmailTo.Text,
                    EmailCC = LabelEmailCC.Text,
                    EmailSubject = LabelEmailSubject.Text,
                    EmailContent = LabelEmailContent.Text,
                    InvoiceDate = DateTime.ParseExact(LabelInvoiceDate.Text, "dd/MM/yyyy", null),
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

                    if (LInvoiceSelectedRow.Count() == 0)
                    {
                        chkAll.Checked = false;
                    }
                    if (LInvoiceSelectedRow.Count() == LInvoiceSelectedRowAll.Count())
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
                if (LInvoiceSelectedRow.Exists(x => x.ID == ID) && !chkBox.Enabled)
                {
                    LInvoiceSelectedRow.RemoveAll(x => x.ID == ID);
                    LInvoiceSelectedRowAll.RemoveAll(x => x.ID == ID);
                    chkBox.Checked = false;
                }
                else if (LInvoiceSelectedRow.Exists(x => x.ID == ID) && chkBox.Enabled)
                {
                    chkBox.Checked = true;
                }
                else
                {
                    chkBox.Checked = false;
                }
            }
            if (LInvoiceSelectedRow.Count() > 0)
            {
                LabelInfo.Text = "Selected records : " + LInvoiceSelectedRow.Count();
            }
        }
    }
}