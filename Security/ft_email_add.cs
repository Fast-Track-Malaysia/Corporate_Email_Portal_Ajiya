using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class ft_email_add
    {

        #region Constructor
        public ft_email_add()
        { }
        #endregion

        #region Private Members
        private int _Id;
        private string _CardCode;
        private string _CardName;
        private string _Name;
        private string _CardType;
        private string _Team;
        private string _Format;
        private string _To;
        private string _Cc;
        private string _Dept;
        private string _Link;
        private string _UserSign;
        private DateTime _TimeStamp;
        private string _DBName;
        #endregion

        #region Const Column Name
        private const string Col_Id = "Id";
        private const string Col_CardCode = "CardCode";
        private const string Col_CardName = "CardName";
        private const string Col_Name = "Name";
        private const string Col_CardType = "CardType";
        private const string Col_Team = "Team";
        private const string Col_Format = "Format";
        private const string Col_To = "To";
        private const string Col_Cc = "Cc";
        private const string Col_Dept = "Dept";
        private const string Col_Link = "Link";
        private const string Col_UserSign = "UserSign";
        private const string Col_TimeStamp = "TimeStamp";
        private const string Col_DBName = "DBName";
        #endregion

        #region Properties
        public int Id { get { return _Id; } set { _Id = value; } }
        public string CardCode { get { return _CardCode; } set { _CardCode = value; } }
        public string CardName { get { return _CardName; } set { _CardName = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }
        public string CardType { get { return _CardType; } set { _CardType = value; } }
        public string Team { get { return _Team; } set { _Team = value; } }
        public string Format { get { return _Format; } set { _Format = value; } }
        public string To { get { return _To; } set { _To = value; } }
        public string Cc { get { return _Cc; } set { _Cc = value; } }
        public string Dept { get { return _Dept; } set { _Dept = value; } }
        public string Link { get { return _Link; } set { _Link = value; } }
        public string UserSign { get { return _UserSign; } set { _UserSign = value; } }
        public DateTime TimeStamp { get { return _TimeStamp; } set { _TimeStamp = value; } }
        public string DBName { get { return _DBName; } set { _DBName = value; } }
        #endregion

        #region Functions


        public static ft_email_add LoadEmailAddress(string id, string CardCode, string DBName)  //pass in the session
        {
            ft_email_add r = new ft_email_add();
            DataTable dt = new DataTable();

            dt = DAC.ExecuteDataTable(Properties.Resources.SP_LoadEmailAddress, 
                DAC.Parameter(Col_Id, id), 
                DAC.Parameter(Col_CardCode, CardCode),
                DAC.Parameter(Col_DBName, DBName));

            if (dt.Rows.Count > 0)
            {
                try
                {
                    string timeStamp = dt.Rows[0][Col_TimeStamp].ToString().Trim();

                    r.Id = int.Parse(dt.Rows[0][Col_Id].ToString().Trim());
                    r.CardCode = dt.Rows[0][Col_CardCode].ToString().Trim();
                    r.CardName = dt.Rows[0][Col_CardName].ToString().Trim();
                    r.Name = dt.Rows[0][Col_Name].ToString().Trim();
                    r.CardType = dt.Rows[0][Col_CardType].ToString().Trim();
                    r.Team = dt.Rows[0][Col_Team].ToString().Trim();
                    r.Format = dt.Rows[0][Col_Format].ToString().Trim();
                    r.To = dt.Rows[0][Col_To].ToString().Trim();
                    r.Cc = dt.Rows[0][Col_Cc].ToString().Trim();
                    r.Dept = dt.Rows[0][Col_Dept].ToString().Trim();
                    r.Link = dt.Rows[0][Col_Link].ToString().Trim();
                    r.UserSign = dt.Rows[0][Col_UserSign].ToString().Trim();
                    r.Format = dt.Rows[0][Col_Format].ToString().Trim();
                    r.DBName = dt.Rows[0][Col_DBName].ToString().Trim();
                    if (timeStamp != null && timeStamp != "")
                    {
                        r.TimeStamp = DateTime.Parse(dt.Rows[0][Col_TimeStamp].ToString().Trim());
                    }

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
                dt = DAC.ExecuteDataTable(Properties.Resources.SP_SaveEmailAddress,
                    DAC.Parameter(Col_CardCode, CardCode),
                    DAC.Parameter(Col_CardName, CardName),
                    DAC.Parameter(Col_Name, Name),
                    DAC.Parameter(Col_CardType, CardType),
                    //DAC.Parameter(Col_Team, Team),
                    DAC.Parameter(Col_To, To),
                    DAC.Parameter(Col_Cc, Cc),
                    DAC.Parameter(Col_Dept, Dept),
                    //DAC.Parameter(Col_Link, Link),
                    DAC.Parameter(Col_UserSign, UserSign),
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
