using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Security;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using System.Web.Configuration;

namespace Web
{
    public partial class PreviewInvoice : System.Web.UI.Page
    {
        private XafReport xafReport { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserId"] == null || Session["UserId"].ToString() == "")
                {
                    Response.Redirect("~/Default.aspx", false);
                    return;
                }
            }
            int DocEntry = int.Parse(Request.QueryString["DocEntry"]);
            string PortalDocNo = Request.QueryString["PortalDocNo"];
            string CardCode = Request.QueryString["CardCode"];
            string queryMode = Request.QueryString["Mode"];

            if (Request.QueryString["DocEntry"] != null)
            {
                string layoutName = Session["CompnyCode"].ToString() + "_Invoice";

                xafReport = new XafReport(Session["AjiyaDBConnString"].ToString(), WebConfigurationManager.AppSettings.Get("XafSource"));
                IObjectSpace objectSpace = xafReport.objectSpaceProvider.CreateObjectSpace();
                ReportDataV2 reportData = objectSpace.FindObject<ReportDataV2>(CriteriaOperator.Parse("DisplayName=?", WebConfigurationManager.AppSettings.Get("InvoiceDisplayName")));// .FirstOrDefault<ReportDataV2>(data => data.DisplayName == sourcefile);
                XtraReport report = ReportDataProvider.ReportsStorage.LoadReport(reportData);
                MyReportDataSourceHelper reportDataSourceHelper = new MyReportDataSourceHelper(xafReport.objectSpaceProvider);
                ReportDataProvider.ReportObjectSpaceProvider = new MyReportObjectSpaceProvider(xafReport.objectSpaceProvider);

                ParameterCollection prmc = report.Parameters;
                CriteriaOperator op = null;

                op = CriteriaOperator.Parse("Oid=?", DocEntry);

                foreach (Parameter parameter in prmc)
                {
                    if (parameter.Name == "Oid")
                    {
                        parameter.Value = DocEntry;
                    }
                }

                string fileName = "[Preview]_" + "INV_" + CardCode + "_" + PortalDocNo + ".pdf";
                string ExportPath = Path.GetTempPath() + fileName;
                reportDataSourceHelper.SetupBeforePrint(report, null, op, true, null, false);
                report.ExportToPdf(ExportPath);
                xafReport = null;
                #region Crystal report
                /*
                ReportDocument crystalReport = new ReportDocument();

                crystalReport.Load(Server.MapPath("~/Reports/" + layoutName + ".rpt"));
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(user.SAPDBConnString);
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

                string fileName = "[Preview]_" + DocEntry + "_INV_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf";
                string ExportPath = Path.GetTempPath() + fileName;
                crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, ExportPath);
                crystalReport.Dispose();
                builder.Clear();
                */
                #endregion

                if (System.IO.File.Exists(ExportPath))
                {
                    if (queryMode.Equals("Download"))
                    {
                        FileInfo fileInfo = new FileInfo(ExportPath);
                        FileStream sourcefile = new FileStream(ExportPath, FileMode.Open);
                        long FileSize = sourcefile.Length;

                        byte[] Buffer = new byte[(int)FileSize];
                        sourcefile.Read(Buffer, 0, (int)FileSize);
                        sourcefile.Close();

                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ClearContent();
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
                        Response.AddHeader("Content-Length", fileInfo.Length.ToString());


                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(Buffer);

                        Response.Flush();
                        Response.Close();

                    }
                    if (queryMode.Equals("View") || queryMode == null)
                    {
                        FileStream fs = new FileStream(ExportPath, FileMode.Open);
                        BinaryReader stream = new BinaryReader(fs);
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(stream.ReadBytes(Convert.ToInt32(stream.BaseStream.Length)));
                        fs.Close();
                        Response.Flush();
                        Response.Close();
                    }

                    if (File.Exists(ExportPath))
                    {
                        File.Delete(ExportPath);
                    }

                    return;
                }

            }
        }
    }
}