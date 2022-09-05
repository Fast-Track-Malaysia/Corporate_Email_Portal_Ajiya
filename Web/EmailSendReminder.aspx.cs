using ClosedXML.Excel;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
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
    public partial class EmailSendReminder : System.Web.UI.Page
    {
        protected static Dictionary<string, List<FileInfo>> UserFiles = new Dictionary<string, List<FileInfo>>();
        protected static Dictionary<string, List<FileInfo>> Documents = new Dictionary<string, List<FileInfo>>();
        protected static DataTable dt;
        protected static List<string> SelectedCardCode = new List<string>();
        protected static List<string> SelectedCardCodeAll = new List<string>();
        protected static List<string> noRecordCardCode = new List<string>();
        protected static List<string> docPath = new List<string>();
        protected static List<ReminderSelectedRow> ReminderSelectedRow = new List<ReminderSelectedRow>();
        protected static List<ReminderSelectedRow> ReminderSelectedRowAll = new List<ReminderSelectedRow>();
        protected int totalRecords = 0;
        protected String pagerInfo = "";
        protected static string folderDir = "";
        protected static string info;
        protected static string warning;
        protected static bool inProcess = false;
        protected static DateTime reminderdate;
        protected static string reminderdateStr = "";
        protected static int exceedDaysFr = 0;
        protected static int exceedDaysTo = 0;
        protected static decimal minAmount = 0;
        protected static string letterType = "R1";
        protected static string strFr = "";
        protected static string strTo = "";
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
                LabelCompany.Text = Session["CompnyCode"].ToString(); ;
                SqlDataSource.ConnectionString = Session["ConnString"].ToString();
            }

            if (!Page.IsPostBack)
            {

                lblUser.Text = Session["UserId"].ToString(); ;

                txtReminderDate.Value = DateTime.Now.ToString("yyyy-MM-dd");

                info = "Page Load";

                dt = null;
                GridView1.DataSource = null;
                GridView1.DataBind();
            }
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            int a = GridView1.PageIndex;
            ReminderSelectedRow.Clear();
            LabelInfo.Text = "";
            foreach (GridViewRow row in GridView1.Rows)
            {
                Label LabelCardcode = (Label)row.FindControl("LabelCardcode");
                Label LabelCardName = (Label)row.FindControl("CardName");
                Label LabelEmailTo = (Label)row.FindControl("lblEmailTo");
                Label LabelEmailCC = (Label)row.FindControl("lblEmailCC");
                CheckBox chkBox = (CheckBox)row.FindControl("chkBox");

                ReminderSelectedRow.Add(new ReminderSelectedRow
                {
                    CardCode = LabelCardcode.Text,
                    CardName = LabelCardName.Text,
                    EmailTo = LabelEmailTo.Text,
                    EmailCC = LabelEmailCC.Text,
                    isChecked = chkBox.Checked
                });

                LabelInfo.Text = "Selected : " + ReminderSelectedRow.Count();
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
            ft_logs.WriteLine("Perform search for as at date " + txtReminderDate.Value.ToString(), "Info");

            init();
            ReminderSelectedRow.Clear();
            SqlDataSource.SelectParameters.Clear();
            try
            {   

                reminderdate = DateTime.Parse(txtReminderDate.Value.ToString());
                reminderdateStr = reminderdate.ToString("yyyyMMdd");
                exceedDaysFr = int.Parse(txtExceedDaysFr.Value.ToString());
                exceedDaysTo = int.Parse(txtExceedDaysTo.Value.ToString());
                minAmount = decimal.Parse(txtMinAmount.Value.ToString());
                letterType = cbxLetterType.SelectedValue.ToString();
                strFr = txtCardCodeFr.Value.Equals("") ? "*" : txtCardCodeFr.Value.ToString();
                strTo = txtCardCodeTo.Value.Equals("") ? "*" : txtCardCodeTo.Value.ToString();

                SqlDataSource.SelectParameters.Add("reminderdateStr", reminderdateStr);
                SqlDataSource.SelectParameters.Add("exceedDaysFr", exceedDaysFr.ToString());
                SqlDataSource.SelectParameters.Add("exceedDaysTo", exceedDaysTo.ToString());
                SqlDataSource.SelectParameters.Add("minAmount", minAmount.ToString());
                SqlDataSource.SelectParameters.Add("letterType", letterType);
                SqlDataSource.SelectParameters.Add("strFr", strFr);
                SqlDataSource.SelectParameters.Add("strTo", strTo);

                LabelRmdDate.Text = "As at " + txtReminderDate.Value.ToString();

                GridView1.DataBind();

                using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
                {
                    using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                    {
                        query = "SELECT * FROM [FTS_fn_GetBPList_Reminder] ('" + reminderdateStr + "', '" +
                            exceedDaysFr + "', '" + exceedDaysTo + "', '" +
                            minAmount + "', '" + letterType + "', '" +
                            strFr + "', '" + strTo + "') T0 ORDER BY T0.CardCode ";

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
                logs.Screen = "EmailReminder";
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
            if (ReminderSelectedRow.Count <= 0)
            {
                LabelDanger.Text = "No business partner is selected.";
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
            ft_logs.WriteLine("Reminder Date:  " + txtReminderDate.Value.ToString(), "");
            ft_logs.WriteLine("-------------------------------------------------------------------------------", "");
            // Core Function
            GenerateDocument();
            SendEmail();

            if (Directory.Exists(folderDir))
            {
                Directory.Delete(folderDir, true);
            }

            inProcess = false;
            ReminderSelectedRow.Clear();
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
            DateTime reminderdate = DateTime.Parse(txtReminderDate.Value.ToString());
            string reminderdateStr = reminderdate.ToString("yyyyMMdd");
            string layoutName = LabelCompany.Text + "_Reminder";


            folderDir = "C:\\EmailPortal\\[" + LabelCompany.Text + "] " + lblUser.Text + "\\Reminder\\" + reminderdate.ToString("dd-MM-yyyy");

            try
            {
                ft_logs.WriteLine("① Clear attachment folder...", "Info");
                foreach (string CardCode in ReminderSelectedRow.Select(x => x.CardCode).Distinct())
                {
                    string path = folderDir + "\\" + CardCode + "\\";
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

                for (int i = 0; i < ReminderSelectedRow.Count; i++)
                {
                    Checked = ReminderSelectedRow[i].isChecked;

                    if (Checked)
                    {
                        string CardCode = ReminderSelectedRow[i].CardCode;
                        string CardName = ReminderSelectedRow[i].CardName;

                        string path = folderDir + "\\" + CardCode + "\\";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        info = "③ Generate Reminder Letter [" + (i + 1).ToString() + "/" + ReminderSelectedRow.Count() + "] ➜ [" + CardCode + "-" + CardName + "]";
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
                                crystalReport.SetParameterValue("AsAtDate", reminderdate.ToString("yyyy-MM-dd"));
                                continue;
                            }

                            if (param.Name.Equals("MinAmount"))
                            {
                                crystalReport.SetParameterValue("MinAmount", minAmount);
                                continue;
                            }

                            if (param.Name.Equals("ExceedDaysFr"))
                            {
                                crystalReport.SetParameterValue("ExceedDaysFr", exceedDaysFr);
                                continue;
                            }

                            if (param.Name.Equals("ExceedDaysTo"))
                            {
                                crystalReport.SetParameterValue("ExceedDaysTo", exceedDaysTo);
                                continue;
                            }

                            if (param.Name.Equals("LetterType"))
                            {
                                crystalReport.SetParameterValue("LetterType", letterType);
                                continue;
                            }

                            if (param.Name.Equals("CardCode@FROM OCRD WHERE CardType = 'C'"))
                            {
                                crystalReport.SetParameterValue("CardCode@FROM OCRD WHERE CardType = 'C'", CardCode);
                                continue;
                            }
                        }

                        string ExportPath = path + "RMD_" + CardCode + "_" + reminderdate.ToString("yyyyMMdd") + ".pdf";
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
                ft_logs.WriteLine("Generating Reminder Letter error:" + ex.Message, "Error");
                inProcess = false;
            }
            finally
            {
                ft_logs.WriteLine("Generate Reminder Letter ended!", "Info");
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

            DateTime reminderdate = DateTime.Parse(txtReminderDate.Value.ToString());

            bool Checked;

            ft_email envelope = new ft_email();

            using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
            {
                using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                {
                    dat.SelectCommand.CommandText = "SELECT * FROM ft_email_template_reminder WHERE isDefault = '1' ";
                    DataTable dt = new DataTable();
                    dat.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            envelope.Id = int.Parse(row["ID"].ToString());
                            envelope.Format = row["Format"].ToString();
                            envelope.EmailSubject = row["EmailSubject"].ToString();
                            envelope.EmailContent = row["EmailContent"].ToString();
                            envelope.LastUpdateUser = row["LastUpdateUser"].ToString();
                            envelope.LastUpdateTime = DateTime.Parse(row["LastUpdateTime"].ToString());
                        }
                    }
                    dt.Dispose();
                }
                conn.Close();
            }

            for (int i = 0; i < ReminderSelectedRow.Count; i++)
            {
                ReminderSelectedRow reminderSelectedObj = ReminderSelectedRow[i];
                Checked = reminderSelectedObj.isChecked;

                if (Checked)
                {
                    string CardCode = reminderSelectedObj.CardCode;
                    string CardName = reminderSelectedObj.CardName;

                    string to = reminderSelectedObj.EmailTo;
                    string cc = reminderSelectedObj.EmailCC;

                    info = "④ Sending Reminder Letter [" + (i + 1).ToString() + "/" + ReminderSelectedRow.Count() + "] ➜ [" + CardCode + "-" + CardName + "] TO: [" + to + "] CC: [" + cc + "]";
                    ft_logs.Write(info, "Info");

                    string subject = envelope.EmailSubject;
                    subject = subject.Replace(@"[CardName]", CardName);
                    subject = subject.Replace(@"[MMM]", reminderdate.ToString("MMM"));
                    subject = subject.Replace(@"[YYYY]", reminderdate.ToString("yyyy"));

                    string content = envelope.EmailContent;
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
                        path = folderDir + "\\" + CardCode + "\\";
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

                        ft_logs.WriteLine("➜✉ E-Mail Sent " + CardCode + "-" + CardName + " @ " + DateTime.Today.ToString("d MMM"), "");

                        using (SqlConnection connection = new SqlConnection(Session["ConnString"].ToString()))
                        {
                            string query = "INSERT INTO ft_email_logs_reminder (CardCode, CardName, EmailTo, EmailCC, " +
                            "EmailSubject, EmailContent, ReminderDate, MinAmt, InvDaysFrom, InvDaysTo, LetterType, SendDate, SendBy, SendResult, ErrorDesc) " +
                            "VALUES ('" + CardCode + "', '" + CardName + "', '" + to + "', '" + cc + "', '" + subject + "', '" + content + "', '" +
                            reminderdate.ToString("yyyy-MM-dd") + "', '" + minAmount + "', '"+ exceedDaysFr + "', '"+ exceedDaysTo + "', '" + letterType +"', '" +
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
                            "VALUES ('" + CardCode + "', '" + CardName + "', '" + to + "', '" + cc + "', '" + subject + "', '" + content + "', '" +
                            reminderdate.ToString("yyyy-MM-dd") + "', '" + minAmount + "', '" + exceedDaysFr + "', '" + exceedDaysTo + "', '" + letterType + "', '" +
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
            ft_logs.WriteLine("✉ Send E-Mail ended!", "Info");
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
                        wb.Worksheets.Add(dt, "BPList");
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=BPList.xlsx");
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

        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            GenerateDocument();

            DateTime reminderdate = DateTime.Parse(txtReminderDate.Value.ToString());
            string ExportPath = Path.GetTempPath() + @"Combined_RMD @ " + reminderdate.ToString("MMM\\'yy") + ".pdf";
            int i = 0;
            while (System.IO.File.Exists(ExportPath))
            {
                ExportPath = Path.GetTempPath() + @"Combined_RMD @ " + reminderdate.ToString("MMM\\'yy") + "[" + (i++).ToString() + "].pdf";
            }
            MergePDFs(ExportPath, docPath.ToArray());

            if (System.IO.File.Exists(ExportPath))
            {

                FileInfo fileInfo = new FileInfo(ExportPath);
                Response.Clear();
                Response.AppendHeader("content-disposition", "attachment; filename=" + fileInfo.Name);
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
                Response.ContentType = "Application/pdf";
                Response.WriteFile(fileInfo.FullName);
                Response.Flush();
                Response.End();
            }
        }

        protected void btnExportZIP_Click(object sender, EventArgs e)
        {
            GenerateDocument();

            DateTime reminderdate = DateTime.Parse(txtReminderDate.Value.ToString());
            string ExportPath = Path.GetTempPath() + @"Combined_RMD @ " + reminderdate.ToString("MMM\\'yy") + ".zip";

            if (File.Exists(ExportPath))
            {
                File.Delete(ExportPath);
            }


            // Create and open a new ZIP file
            var zip = ZipFile.Open(ExportPath, ZipArchiveMode.Create);
            foreach (var file in docPath.ToArray())
            {
                // Add the entry for each file
                zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
            }
            // Dispose of the object when we are done
            zip.Dispose();

            if (System.IO.File.Exists(ExportPath))
            {

                FileInfo fileInfo = new FileInfo(ExportPath);
                Response.Clear();
                Response.AppendHeader("content-disposition", "attachment; filename=" + fileInfo.Name);
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
                Response.ContentType = "Application/zip";
                Response.WriteFile(fileInfo.FullName);
                Response.Flush();
                Response.End();
            }
        }


        private void MergePDFs(string outPutFilePath, params string[] filesPath)
        {
            List<PdfReader> readerList = new List<PdfReader>();
            foreach (string filePath in filesPath)
            {
                PdfReader pdfReader = new PdfReader(filePath);
                readerList.Add(pdfReader);
            }

            //Define a new output document and its size, type
            Document document = new Document(PageSize.A4, 0, 0, 0, 0);
            //Create blank output pdf file and get the stream to write on it.
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create));
            document.Open();

            foreach (PdfReader reader in readerList)
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    PdfImportedPage page = writer.GetImportedPage(reader, i);
                    document.Add(iTextSharp.text.Image.GetInstance(page));
                }
            }
            document.Close();

        }

        // -- Web Element -----------------------------------------------------------------------
        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkAll = (CheckBox)sender;
            ViewState["chkAll"] = chkAll.Checked;
            ReminderSelectedRow = new List<ReminderSelectedRow>();
            if (chkAll.Checked)
            {
                ReminderSelectedRow = ReminderSelectedRowAll;
            }
            else
            {
                ReminderSelectedRow.Clear();
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
            string cardcode = GridView1.DataKeys[selRowIndex]["CardCode"].ToString();
            Label cardname = (Label)GridView1.Rows[selRowIndex].FindControl("CardName");
            Label emailto = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailTo");
            Label emailcc = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailCC");

            if (chk.Checked)
            {
                ReminderSelectedRow.Add(new ReminderSelectedRow
                {
                    CardCode = cardcode,
                    CardName = cardname.Text,
                    EmailTo = emailto.Text,
                    EmailCC = emailcc.Text,
                    isChecked = chk.Checked
                });
            }
            else
            {
                ReminderSelectedRow.RemoveAll(x => x.CardCode == cardcode);
            }
            ReminderSelectedRow.Select(x => x.CardCode).Distinct();
            LabelInfo.Text = "Selected : " + ReminderSelectedRow.Count();
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

            ReminderSelectedRowAll = new List<ReminderSelectedRow>();
            GridView1.AllowPaging = false;
            totalRecords = GridView1.Rows.Count;
            foreach (GridViewRow row in GridView1.Rows)
            {
                Label LabelCardcode = (Label)row.FindControl("LabelCardcode");
                Label LabelCardName = (Label)row.FindControl("CardName");
                Label LabelEmailTo = (Label)row.FindControl("lblEmailTo");
                Label LabelEmailCC = (Label)row.FindControl("lblEmailCC");
                CheckBox chkBox = (CheckBox)row.FindControl("chkBox");

                ReminderSelectedRowAll.Add(new ReminderSelectedRow
                {
                    CardCode = LabelCardcode.Text,
                    CardName = LabelCardName.Text,
                    EmailTo = LabelEmailTo.Text,
                    EmailCC = LabelEmailCC.Text,
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

                    if (ReminderSelectedRow.Count() == 0)
                    {
                        chkAll.Checked = false;
                    }
                    if (ReminderSelectedRow.Count() == ReminderSelectedRowAll.Count())
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
                Label LabelCardcode = (Label)GridView1.Rows[i].FindControl("LabelCardcode");
                Label LabelLastSent = (Label)GridView1.Rows[i].FindControl("lblLastSent");
                Label LabelDanger = (Label)GridView1.Rows[i].FindControl("LabelDanger");
                Label LabelWarning = (Label)GridView1.Rows[i].FindControl("LabelWarning");
                Label LabelInfo = (Label)GridView1.Rows[i].FindControl("LabelInfo");

                string cardcode = LabelCardcode.Text.Trim();

                // ChkBox Rendering
                if (ReminderSelectedRow.Exists(x => x.CardCode == cardcode) && !chkBox.Enabled)
                {
                    ReminderSelectedRow.RemoveAll(x => x.CardCode == cardcode);
                    ReminderSelectedRowAll.RemoveAll(x => x.CardCode == cardcode);
                    chkBox.Checked = false;
                }
                else if (ReminderSelectedRow.Exists(x => x.CardCode == cardcode) && chkBox.Enabled)
                {
                    chkBox.Checked = true;
                }
                else
                {
                    chkBox.Checked = false;
                }
            }
            if (ReminderSelectedRow.Count() > 0)
            {
                LabelInfo.Text = "Selected records : " + ReminderSelectedRow.Count();
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            init();
            Timer1.Enabled = true;
            inProcess = true;
            Thread workerThread = new Thread(new ThreadStart(refresh));
            workerThread.Start();

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                HyperLink LinkView = (HyperLink)GridView1.Rows[i].FindControl("LinkView");
                HyperLink LinkDownload = (HyperLink)GridView1.Rows[i].FindControl("LinkDownload");
                LinkView.Visible = true;
                LinkDownload.Visible = true;
            }
        }
        protected void refresh()
        {

            DateTime reminderdate = DateTime.Parse(txtReminderDate.Value.ToString());
            string reminderdateStr = reminderdate.ToString("yyyyMMdd");
            // Special For SSB
            info = "② Updating [" + LabelCompany.Text + "] reminder table...";
            ft_logs.Write(info, "Info");

            // Switch Connection
            DAC.SAPcon = WebConfigurationManager.ConnectionStrings[LabelCompany.Text + "SAPConnectionString"].ConnectionString;
            DAC.ExecuteSAPDataTable("sp_FT_WEB_SOA_Load", DAC.Parameter("@ReminderDate", reminderdate));

            info = "Reminder table has been updated";
            ft_logs.WriteLine(info, "");
            inProcess = false;
        }
    }
}