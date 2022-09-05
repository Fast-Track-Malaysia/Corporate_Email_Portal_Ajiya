using ClosedXML.Excel;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraReports.UI;
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
    public partial class EmailSendInvoice : System.Web.UI.Page
    {
        protected static Dictionary<string, List<FileInfo>> UserFiles = new Dictionary<string, List<FileInfo>>();
        protected static Dictionary<string, List<FileInfo>> Documents = new Dictionary<string, List<FileInfo>>();
        protected static DataTable dt;
        protected static List<string> SelectedCardCode = new List<string>();
        protected static List<string> SelectedCardCodeAll = new List<string>();
        protected static List<string> noRecordCardCode = new List<string>();
        protected static List<string> docPath = new List<string>();
        protected static List<InvoiceSelectedRow> InvoiceSelectedRow = new List<InvoiceSelectedRow>();
        protected static List<InvoiceSelectedRow> InvoiceSelectedRowAll = new List<InvoiceSelectedRow>();
        protected int totalRecords = 0;
        protected String pagerInfo = "";
        protected static string folderDir = "";
        protected static string info;
        protected static string warning;
        protected static bool inProcess = false;
        protected static DateTime DateFrom;
        protected static DateTime DateTo;
        protected static string DateFromStr = "";
        protected static string DateToStr = "";
        protected static string strFr = "";
        protected static string strTo = "";
        protected static string query = "";
        private XafReport xafReport { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                Response.Redirect("~/Default.aspx", false);
                return;
            }

            if (Session["CompnyCode"].ToString() == null)
            {
                LabelDanger.Text = "You has been logged out!";
                Response.Redirect("~/Default.aspx", true);
            }
            else
            {
                LabelCompany.Text = Session["CompnyCode"].ToString();
                SqlDataSource.ConnectionString = Session["ConnString"].ToString();
            }

            if (!IsPostBack)
            {

                lblUser.Text = Session["UserId"].ToString();

                txtDateFrom.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDateTo.Text = DateTime.Now.ToString("dd/MM/yyyy");

                info = "Page Load";

                dt = null;
                GridView1.DataSource = null;
                GridView1.DataBind();
            }
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            int a = GridView1.PageIndex;
            InvoiceSelectedRow.Clear();
            LabelInfo.Text = "";
            foreach (GridViewRow row in GridView1.Rows)
            {
                Label lblDocEntry = (Label)row.FindControl("lblDocEntry");
                Label lblSAPDocNo = (Label)row.FindControl("lblSAPDocNo");
                Label lblPortalOid = (Label)row.FindControl("lblPortalOid");
                Label lblPortalDocNo = (Label)row.FindControl("lblPortalDocNo");
                Label lblDocDate = (Label)row.FindControl("lblDocDate");
                Label LabelCardcode = (Label)row.FindControl("LabelCardcode");
                Label LabelCardName = (Label)row.FindControl("CardName");
                Label LabelEmailTo = (Label)row.FindControl("lblEmailTo");
                Label LabelEmailCC = (Label)row.FindControl("lblEmailCC");
                CheckBox chkBox = (CheckBox)row.FindControl("chkBox");

                InvoiceSelectedRow.Add(new InvoiceSelectedRow
                {
                    DocEntry = int.Parse(lblDocEntry.Text),
                    SAPDocNo = lblSAPDocNo.Text,
                    PortalOid = int.Parse(lblPortalOid.Text),
                    PortalDocNo = lblPortalDocNo.Text,
                    DocDate = DateTime.ParseExact(lblDocDate.Text, "dd/MM/yyyy", null),
                    CardCode = LabelCardcode.Text,
                    CardName = LabelCardName.Text,
                    EmailTo = LabelEmailTo.Text,
                    EmailCC = LabelEmailCC.Text,
                    isChecked = chkBox.Checked
                });

                LabelInfo.Text = "Selected : " + InvoiceSelectedRow.Count();
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
            ft_logs.WriteLine("Perform search for Invoice.", "Info");
            init();
            InvoiceSelectedRow.Clear();
            SqlDataSource.SelectParameters.Clear();
            try
            {
                DateFrom = DateTime.ParseExact(txtDateFrom.Text, "dd/MM/yyyy", null);
                DateTo = DateTime.ParseExact(txtDateTo.Text, "dd/MM/yyyy", null);
                DateFromStr = DateFrom.ToString("yyyyMMdd");
                DateToStr = DateTo.ToString("yyyyMMdd");
                strFr = txtCardCodeFr.Text.Equals("") ? "*" : txtCardCodeFr.Text;
                strTo = txtCardCodeTo.Text.Equals("") ? "*" : txtCardCodeTo.Text;

                SqlDataSource.SelectParameters.Add("DateFromStr", DateFromStr);
                SqlDataSource.SelectParameters.Add("DateToStr", DateToStr);
                SqlDataSource.SelectParameters.Add("strFr", strFr);
                SqlDataSource.SelectParameters.Add("strTo", strTo);

                GridView1.DataBind();

                using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
                {
                    using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                    {
                        query = "SELECT * FROM [FTS_fn_GetBPList_Invoice] ('" + DateFromStr + "', '" + DateToStr + "', '"
                            + strFr + "', '" + strTo + "') T0 ORDER BY T0.DocEntry ";

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
                logs.Screen = "EmailInvoice";
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
            if (InvoiceSelectedRow.Count <= 0)
            {
                LabelDanger.Text = "No document is selected!";
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
            ft_logs.WriteLine("Invoice Date From:  " + txtDateFrom.Text, "");
            ft_logs.WriteLine("Invoice Date To:  " + txtDateTo.Text, "");
            ft_logs.WriteLine("-------------------------------------------------------------------------------", "");
            // Core Function
            GenerateDocument();
            SendEmail();

            if (Directory.Exists(folderDir))
            {
                Directory.Delete(folderDir, true);
            }

            inProcess = false;
            InvoiceSelectedRow.Clear();
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
            ft_logs.WriteLine("Generate Invoice begin...", "Info");
            string todaydateStr = DateTime.Now.ToString("dd-MM-yyyy");
            string layoutName = LabelCompany.Text + "_Invoice";


            folderDir = "C:\\EmailPortal\\[" + LabelCompany.Text + "] " + lblUser.Text + "\\Invoice\\" + todaydateStr;

            try
            {
                ft_logs.WriteLine("① Clear attachment folder...", "Info");
                foreach (string DocEntry in InvoiceSelectedRow.Select(x => x.DocEntry.ToString()).Distinct())
                {
                    string path = folderDir + "\\" + DocEntry + "\\";
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

                for (int i = 0; i < InvoiceSelectedRow.Count; i++)
                {
                    Checked = InvoiceSelectedRow[i].isChecked;

                    if (Checked)
                    {
                        int DocEntry = InvoiceSelectedRow[i].DocEntry;
                        string DocNum = InvoiceSelectedRow[i].SAPDocNo;
                        int PortalOid = InvoiceSelectedRow[i].PortalOid;
                        string PortalDocNum = InvoiceSelectedRow[i].PortalDocNo;
                        string CardCode = InvoiceSelectedRow[i].CardCode;
                        string CardName = InvoiceSelectedRow[i].CardName;

                        string path = folderDir + "\\" + DocEntry + "\\";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        info = "③ Generate Invoice [" + (i + 1).ToString() + "/" + InvoiceSelectedRow.Count() + "] ➜ [" + CardCode + "-" + CardName + "-" + PortalDocNum + "]";
                        ft_logs.Write(info, "Info");

                        string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                        Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                        CardName = r.Replace(CardName, "");

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

                        string ExportPath = path + "INV_" + CardCode + "_" + PortalDocNum.ToString() + ".pdf";
                        reportDataSourceHelper.SetupBeforePrint(report, null, op, true, null, false);
                        report.ExportToPdf(ExportPath);
                        xafReport = null;
                        #region Crystal report
                        /*
                        ReportDocument crystalReport = new ReportDocument();
                        crystalReport.Load(Server.MapPath("~/Reports/" + layoutName + ".rpt"));

                        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(CurrentLoggedUserDetails.SAPDBConnString);
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
                        }
       
                        string ExportPath = path + CardCode + "_INV_" + DocEntry.ToString() + ".pdf";
                        crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, ExportPath);
                        docPath.Add(ExportPath);
                        crystalReport.Dispose();
                        */
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

            ft_email envelope = new ft_email();

            using (SqlConnection conn = new SqlConnection(Session["ConnString"].ToString()))
            {
                using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                {
                    dat.SelectCommand.CommandText = "SELECT * FROM ft_email_template_invoice WHERE isDefault = '1' ";
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

            /* Changed from Parellel.For to For loop to avoid no attachment in email because path might be deleted before email sent */
            for (int i = 0; i < InvoiceSelectedRow.Count; i++)
            {
                InvoiceSelectedRow invoiceSelectedObj = InvoiceSelectedRow[i];
                Checked = invoiceSelectedObj.isChecked;

                if (Checked)
                {
                    string DocEntry = invoiceSelectedObj.DocEntry.ToString();
                    string DocNum = invoiceSelectedObj.SAPDocNo;
                    string PortalOid = invoiceSelectedObj.PortalOid.ToString(); ;
                    string PortalDocNum = invoiceSelectedObj.PortalDocNo;
                    DateTime DocDate = invoiceSelectedObj.DocDate;
                    string CardCode = invoiceSelectedObj.CardCode;
                    string CardName = invoiceSelectedObj.CardName;

                    string to = invoiceSelectedObj.EmailTo;
                    string cc = invoiceSelectedObj.EmailCC;

                    info = "④ Sending Invoice [" + (i + 1).ToString() + "/" + InvoiceSelectedRow.Count() + "] ➜ [" + CardCode + "-" + CardName + "-" + PortalDocNum + "] TO: [" + to + "] CC: [" + cc + "]";
                    ft_logs.Write(info, "Info");

                    string subject = envelope.EmailSubject;
                    subject = subject.Replace(@"[CardName]", CardName);
                    subject = subject.Replace(@"[DD]", DocDate.ToString("dd"));
                    subject = subject.Replace(@"[MMM]", DocDate.ToString("MMM"));
                    subject = subject.Replace(@"[YYYY]", DocDate.ToString("yyyy"));

                    string content = envelope.EmailContent;
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
                        path = folderDir + "\\" + DocEntry + "\\";
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
                            string query = "INSERT INTO ft_email_logs_invoice (DocEntry, DocNum, PortalOid, PortalDocNum, CardCode, CardName, EmailTo, EmailCC, " +
                            "EmailSubject, EmailContent, InvoiceDate, SendDate, SendBy, SendResult, ErrorDesc) " +
                            "VALUES ('" + DocEntry + "', '" + DocNum + "', '"+ PortalOid + "', '" + PortalDocNum + "', '" + CardCode + "', '" + CardName + "', '" + to + "', '" + cc + "', '" +
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
                            string query = "INSERT INTO ft_email_logs_invoice (DocEntry, DocNum, PortalOid,PortalDocNum, CardCode, CardName, EmailTo, EmailCC, " +
                            "EmailSubject, EmailContent, InvoiceDate, SendDate, SendBy, SendResult, ErrorDesc) " +
                            "VALUES ('" + DocEntry + "', '" + DocNum + "', '"+ PortalOid + "', '" + PortalDocNum + "', '" + CardCode + "', '" + CardName + "', '" + to + "', '" + cc + "', '" +
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

            DateTime statementdate = DateTime.ParseExact(txtDateTo.Text, "dd/MM/yyyy", null);
            string ExportPath = Path.GetTempPath() + @"Combined_INV @ " + statementdate.ToString("MMM\\'yy") + ".pdf";
            int i = 0;
            while (System.IO.File.Exists(ExportPath))
            {
                ExportPath = Path.GetTempPath() + @"Combined_INV @ " + statementdate.ToString("MMM\\'yy") + "[" + (i++).ToString() + "].pdf";
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

            DateTime statementdate = DateTime.ParseExact(txtDateTo.Text, "dd/MM/yyyy", null);
            string ExportPath = Path.GetTempPath() + @"Combined_SOA @ " + statementdate.ToString("MMM\\'yy") + ".zip";

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
            InvoiceSelectedRow = new List<InvoiceSelectedRow>();
            if (chkAll.Checked)
            {
                InvoiceSelectedRow = InvoiceSelectedRowAll;
            }
            else
            {
                InvoiceSelectedRow.Clear();
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
            int docentry = int.Parse(GridView1.DataKeys[selRowIndex]["DocEntry"].ToString());
            Label sapdocno = (Label)GridView1.Rows[selRowIndex].FindControl("lblSAPDocNo");
            Label portaloid = (Label)GridView1.Rows[selRowIndex].FindControl("lblPortalOid");
            Label portaldocno = (Label)GridView1.Rows[selRowIndex].FindControl("lblPortalDocNo");
            Label docdate = (Label)GridView1.Rows[selRowIndex].FindControl("lblDocDate");
            Label cardcode = (Label)GridView1.Rows[selRowIndex].FindControl("LabelCardCode");
            Label cardname = (Label)GridView1.Rows[selRowIndex].FindControl("CardName");
            Label emailto = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailTo");
            Label emailcc = (Label)GridView1.Rows[selRowIndex].FindControl("lblEmailCC");

            if (chk.Checked)
            {
                InvoiceSelectedRow.Add(new InvoiceSelectedRow
                {
                    DocEntry = docentry,
                    SAPDocNo = sapdocno.Text,
                    PortalOid = int.Parse(portaloid.Text),
                    PortalDocNo = portaldocno.Text,
                    DocDate = DateTime.ParseExact(docdate.Text, "dd/MM/yyyy", null),
                    CardCode = cardcode.Text,
                    CardName = cardname.Text,
                    EmailTo = emailto.Text,
                    EmailCC = emailcc.Text,
                    isChecked = chk.Checked
                });
            }
            else
            {
                InvoiceSelectedRow.RemoveAll(x => x.DocEntry == docentry);
            }
            InvoiceSelectedRow.Select(x => x.DocEntry).Distinct();
            LabelInfo.Text = "Selected : " + InvoiceSelectedRow.Count();
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

            InvoiceSelectedRowAll = new List<InvoiceSelectedRow>();
            GridView1.AllowPaging = false;
            totalRecords = GridView1.Rows.Count;
            foreach (GridViewRow row in GridView1.Rows)
            {
                Label lblDocEntry = (Label)row.FindControl("lblDocEntry");
                Label lblSAPDocNo = (Label)row.FindControl("lblSAPDocNo");
                Label lblPortalOid = (Label)row.FindControl("lblPortalOid");
                Label lblPortalDocNo = (Label)row.FindControl("lblPortalDocNo");
                Label lblDocDate = (Label)row.FindControl("lblDocDate");
                Label LabelCardcode = (Label)row.FindControl("LabelCardcode");
                Label LabelCardName = (Label)row.FindControl("CardName");
                Label LabelEmailTo = (Label)row.FindControl("lblEmailTo");
                Label LabelEmailCC = (Label)row.FindControl("lblEmailCC");
                CheckBox chkBox = (CheckBox)row.FindControl("chkBox");

                InvoiceSelectedRowAll.Add(new InvoiceSelectedRow
                {
                    DocEntry = int.Parse(lblDocEntry.Text),
                    SAPDocNo = lblSAPDocNo.Text,
                    PortalOid = int.Parse(lblPortalOid.Text),
                    PortalDocNo = lblPortalDocNo.Text,
                    DocDate = DateTime.ParseExact(lblDocDate.Text, "dd/MM/yyyy", null),
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

                    if (InvoiceSelectedRow.Count() == 0)
                    {
                        chkAll.Checked = false;
                    }
                    if (InvoiceSelectedRow.Count() == InvoiceSelectedRowAll.Count())
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
                Label lblDocEntry = (Label)GridView1.Rows[i].FindControl("lblDocEntry");

                int docentry = int.Parse(lblDocEntry.Text.Trim());

                // ChkBox Rendering
                if (InvoiceSelectedRow.Exists(x => x.DocEntry == docentry) && !chkBox.Enabled)
                {
                    InvoiceSelectedRow.RemoveAll(x => x.DocEntry == docentry);
                    InvoiceSelectedRowAll.RemoveAll(x => x.DocEntry == docentry);
                    chkBox.Checked = false;
                }
                else if (InvoiceSelectedRow.Exists(x => x.DocEntry == docentry) && chkBox.Enabled)
                {
                    chkBox.Checked = true;
                }
                else
                {
                    chkBox.Checked = false;
                }
            }
            if (InvoiceSelectedRow.Count() > 0)
            {
                LabelInfo.Text = "Selected records : " + InvoiceSelectedRow.Count();
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

            DateTime statementdate = DateTime.ParseExact(txtDateTo.Text, "dd/MM/yyyy", null);
            string statementdateStr = statementdate.ToString("yyyyMMdd");
            // Special For SSB
            info = "② Updating [" + LabelCompany.Text + "] statement table...";
            ft_logs.Write(info, "Info");

            // Switch Connection
            DAC.SAPcon = WebConfigurationManager.ConnectionStrings[LabelCompany.Text + "SAPConnectionString"].ConnectionString;
            DAC.ExecuteSAPDataTable("sp_FT_WEB_SOA_Load", DAC.Parameter("@StatementDate", statementdate));

            info = "Statement table has been updated";
            ft_logs.WriteLine(info, "");
            inProcess = false;
        }
    }
}