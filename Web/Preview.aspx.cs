using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Security;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace Web
{
    public partial class Preview : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserId"] == null || Session["UserId"].ToString() == "")
                {
                    Response.Redirect("~/Default.aspx", false);
                }
            }
            string cardCode = Request.QueryString["CardCode"];
            string stmtDate = Request.QueryString["StmtDate"];
            string currency = Request.QueryString["Currency"];
            string queryMode = Request.QueryString["Mode"];
            if (cardCode != null)
            {

                DateTime statementdate = DateTime.ParseExact(stmtDate, "dd/MM/yyyy", null);
                string stmtDateStr = statementdate.ToString("yyyy-MM-dd");
                string layoutName = Session["CompnyCode"].ToString() + "_Statement";


                ReportDocument crystalReport = new ReportDocument();

                crystalReport.Load(Server.MapPath("~/Reports/" + layoutName + ".rpt"));
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Session["SAPDBConnString"].ToString());
                crystalReport.DataSourceConnections[0].SetConnection(builder.DataSource, builder.InitialCatalog, builder.UserID, builder.Password);

                ParameterFieldDefinitions crParameterdef;
                crParameterdef = crystalReport.DataDefinition.ParameterFields;

                foreach (ParameterFieldDefinition param in crParameterdef)
                {
                    if (param.Name.Equals("StatementDate"))
                    {
                        crystalReport.SetParameterValue("StatementDate", stmtDateStr);
                        continue;
                    }

                    if (param.Name.Equals("CardCode@FROM OCRD WHERE CardType = 'C'"))
                    {
                        crystalReport.SetParameterValue("CardCode@FROM OCRD WHERE CardType = 'C'", cardCode);
                        continue;
                    }

                    if (param.Name.Equals("IncludeNoBal"))
                    {
                        crystalReport.SetParameterValue("IncludeNoBal", true);
                        continue;
                    }
                }
                
                string fileName = "[Preview]_" + "SOA_" + cardCode + "_" + statementdate.ToString("yyyyMMdd") + ".pdf";
                string ExportPath = Path.GetTempPath() + fileName;
                crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, ExportPath);
                crystalReport.Dispose();
                builder.Clear();

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