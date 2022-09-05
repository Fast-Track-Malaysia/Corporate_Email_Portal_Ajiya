using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{

    public class ft_email_info
    {
        #region Constructor
        public ft_email_info()
        { }
        #endregion

        #region Private Members

        private string _CardCode;
        private string _CardName;
        private string _Format;
        private string _MailFr;
        private string _MailTo;
        private string _MailCC;
        private string _Attachement;
        private string _Subject;
        private string _Content;
        private string _CreatedBy;
        private DateTime _CreateDate;
        private string _Source;
        private string _Screen;

        private string _Currency;
        private string _DisplayPrev;
        private DateTime _Statementdate;
        private Double _Balance;
        private string _DBName;


        #endregion

        #region Const Table Headers 

        private const string H_CardCode = "CardCode";
        private const string H_CardName = "CardName";

        private const string H_Format = "Format";
        private const string H_MailFr = "MailFr";
        private const string H_MailTo = "MailTo";
        private const string H_MailCC = "MailCC";
        private const string H_Attachement = "Attachement";
        private const string H_Subject = "Subject";
        private const string H_Content = "Content";

        private const string H_CreatedBy = "CreatedBy";
        private const string H_CreateDate = "CreateDate";
        private const string H_Source = "Source";
        private const string H_Screen = "Screen";

        private const string H_Currency = "Currency";
        private const string H_DisplayPrev = "DisplayPrev";
        private const string H_Statementdate = "Statementdate";
        private const string H_Balance = "Balance";
        private const string H_DBName = "DBName";
        #endregion

        #region Properties
        public string CardCode{ get { return _CardCode; } set { _CardCode = value; } }
        public string CardName{ get { return _CardName; } set { _CardName = value; } }
        public string Format{ get { return _Format; } set { _Format = value; } }
        public string MailFr{ get { return _MailFr; } set { _MailFr = value; } }
        public string MailTo{ get { return _MailTo; } set { _MailTo = value; } }
        public string MailCC{ get { return _MailCC; } set { _MailCC = value; } }
        public string Attachement{ get { return _Attachement; } set { _Attachement = value; } }
        public string Subject{ get { return _Subject; } set { _Subject = value; } }
        public string Content{ get { return _Content; } set { _Content = value; } }
        public string CreatedBy{ get { return _CreatedBy; } set { _CreatedBy = value; } }
        public DateTime CreateDate { get { return _CreateDate; } set { _CreateDate = value; } }
        public string Source{ get { return _Source; } set { _Source = value; } }
        public string Screen{ get { return _Screen; } set { _Screen = value; } }

        public string Currency{ get { return _Currency; } set { _Currency = value; } }
        public string DisplayPrev{ get { return _DisplayPrev; } set { _DisplayPrev = value; } }
        public DateTime Statementdate{ get { return _Statementdate; } set { _Statementdate = value; } }
        public Double Balance{ get { return _Balance; } set { _Balance = value; } }
        public string DBName { get { return _DBName; } set { _DBName = value; } }




        #endregion

        #region Functions

        public Boolean SaveReceiptEmail()
        {
            Boolean success = false;

            DataTable dt = new DataTable();
            try
            {

                dt = DAC.ExecuteDataTable(Properties.Resources.SP_SaveEmailInfo,       
                    DAC.Parameter(H_CardCode, CardCode),
                    DAC.Parameter(H_CardName, CardName),

                    DAC.Parameter(H_Format, Format),
                    DAC.Parameter(H_MailFr, MailFr),
                    DAC.Parameter(H_MailTo, MailTo),
                    DAC.Parameter(H_MailCC, MailCC),
                    DAC.Parameter(H_Attachement, Attachement),
                    DAC.Parameter(H_Subject, Subject),
                    DAC.Parameter(H_Content, Content),

                    DAC.Parameter(H_CreatedBy, CreatedBy),
                    DAC.Parameter(H_CreateDate, CreateDate),
                    DAC.Parameter(H_Source, Source),
                    DAC.Parameter(H_Screen, Screen),

                    DAC.Parameter(H_Currency, Currency),
                    DAC.Parameter(H_DisplayPrev, DisplayPrev),
                    DAC.Parameter(H_Statementdate, Statementdate),
                    DAC.Parameter(H_Balance, Balance),
                    DAC.Parameter(H_DBName, DBName));


                success = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return success;
        }


        /*
        public Boolean SaveStatementEmail()
        {
            Boolean a = false;

            DataTable dt = new DataTable();
            try
            {

                dt = DAC.ExecuteDataTable(Properties.Resources.SP_SaveEmailInfoStatement,
                    DAC.Parameter(CN_CustomerCode, CustomerCode),
                    DAC.Parameter(CN_CustomerName, CustomerName),
                    DAC.Parameter(CN_StatementDate, StatementDate),
                    DAC.Parameter(CN_currency, Currency),
                    DAC.Parameter(CN_displayPrev, DisplayPrev),
                    DAC.Parameter(CN_EmailAddress, EmailAddress),
                    DAC.Parameter(CN_balance, Balance),
                    DAC.Parameter(CN_Screen, Screen),
                    DAC.Parameter(CN_LastUpdateUser, LastUpdateUser));

                a = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return a;
        }


        public Boolean SaveCreditNoteEmail()
        {
            Boolean a = false;

            DataTable dt = new DataTable();
            try
            {

                dt = DAC.ExecuteDataTable(Properties.Resources.SP_SaveEmailInfoCN,
                    DAC.Parameter(CN_CustomerCode, CustomerCode),
                    DAC.Parameter(CN_CustomerName, CustomerName),
                    DAC.Parameter(CN_DocNum, DocNum),
                    DAC.Parameter(CN_PaymentDate, PaymentDate),
                    DAC.Parameter(CN_PaymentAmount, PaymentAmount),
                    DAC.Parameter(CN_EmailAddress, EmailAddress),
                    DAC.Parameter(CN_Screen, Screen),
                    DAC.Parameter(CN_LastUpdateUser, LastUpdateUser),
                    DAC.Parameter(CN_Source, Source),
                    DAC.Parameter(CN_SourceType, SourceType));

                a = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return a;
        }

        */
        #endregion
    }
}
