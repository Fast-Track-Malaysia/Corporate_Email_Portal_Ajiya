using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class ft_email
    {

        #region Constructor
        public ft_email()
        { }
        #endregion

        #region Private Members
        private int _Id;
        private string _EmailSubject;
        private string _EmailContent;
        private string _LastUpdateUser;
        private DateTime _LastUpdateTime;
        private string _Format;
        private string _DBName;
        #endregion

        #region Const Column Name
        private const string Col_Id = "Id";
        private const string Col_EmailSubject = "EmailSubject";
        private const string Col_EmailContent = "EmailContent";
        private const string Col_LastUpdateUser = "LastUpdateUser";
        private const string Col_LastUpdateTime = "LastUpdateTime";
        private const string Col_Format = "Format";
        private const string Col_DBName = "DBName";
        #endregion

        #region Properties
        public int Id { get { return _Id; } set { _Id = value; } }
        public string EmailSubject { get { return _EmailSubject; } set { _EmailSubject = value; } }
        public string EmailContent { get { return _EmailContent; } set { _EmailContent = value; } }
        public string LastUpdateUser { get { return _LastUpdateUser; } set { _LastUpdateUser = value; } }
        public DateTime LastUpdateTime { get { return _LastUpdateTime; } set { _LastUpdateTime = value; } }
        public string Format { get { return _Format; } set { _Format = value; } }
        public string DBName { get { return _DBName; } set { _DBName = value; } }
        #endregion

        #region Functions


        public static ft_email LoadEmailTemplate(string Id, string DBName, string Format)  //pass in the session
        {
            ft_email r = new ft_email();
            DataTable dt = new DataTable();

            dt = DAC.ExecuteDataTable(Properties.Resources.SP_LoadEmailTemplate, 
                DAC.Parameter(Col_Id, Id),
                DAC.Parameter(Col_DBName, DBName),
                DAC.Parameter(Col_Format, Format));
            

            if (dt.Rows.Count > 0)
            {
                try
                {

                    string lastupdatetime = dt.Rows[0][Col_LastUpdateTime].ToString().Trim();

                    r.Id = int.Parse(dt.Rows[0][Col_Id].ToString().Trim());
                    r.EmailSubject = dt.Rows[0][Col_EmailSubject].ToString().Trim();
                    r.EmailContent = dt.Rows[0][Col_EmailContent].ToString().Trim();
                    r.LastUpdateUser = dt.Rows[0][Col_LastUpdateUser].ToString().Trim();
                    if (lastupdatetime != null && lastupdatetime != "")
                    {
                        r.LastUpdateTime = DateTime.Parse(dt.Rows[0][Col_LastUpdateTime].ToString().Trim());
                    }
                    r.Format = dt.Rows[0][Col_Format].ToString().Trim();
                    r.DBName = dt.Rows[0][Col_DBName].ToString().Trim();
                }
                catch (Exception ex)
                {
                    r.Id = 0;
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                r.Id = 0;
            }

            return r;
        }
        public Boolean Save(string id)
        {
            Boolean success = false;

            DataTable dt = new DataTable();


            try
            {
                dt = DAC.ExecuteDataTable(Properties.Resources.SP_SaveEmailTemplate,
                    DAC.Parameter(Col_EmailSubject, EmailSubject),
                    DAC.Parameter(Col_EmailContent, EmailContent),
                    DAC.Parameter(Col_LastUpdateUser, LastUpdateUser),
                    DAC.Parameter(Col_Format, Format),
                    DAC.Parameter(Col_DBName, DBName),
                    DAC.Parameter(Col_Id, id));

                success = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return success;
        }
        #endregion
    }
}
