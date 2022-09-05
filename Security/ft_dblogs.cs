using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class ft_dblogs
    {
        #region Constructor
        public ft_dblogs() { }
        #endregion

        #region Private Members

        private string _Screen;
        private string _MessageType;
        private string _Message;
        private string _UserSign;
        private DateTime _TimeStamp;
        private string _Remarks;
        private string _DBName;
        #endregion

        #region Const Column Name
        private const string H_Screen = "Screen";
        private const string H_MessageType = "MessageType";
        private const string H_Message = "Message";
        private const string H_UserSign = "UserSign";
        private const string H_TimeStamp = "TimeStamp";
        private const string H_Remarks = "Remarks";
        private const string H_DBName = "DBName";
        #endregion

        #region Properties
        public string Screen { get { return _Screen; } set { _Screen = value; } }
        public string MessageType { get { return _MessageType; } set { _MessageType = value; } }
        public string Message { get { return _Message; } set { _Message = value; } }
        public string UserSign { get { return _UserSign; } set { _UserSign = value; } }
        public DateTime TimeStamp { get { return _TimeStamp; } set { _TimeStamp = value; } }
        public string Remarks { get { return _Remarks; } set { _Remarks = value; } }
        public string DBName { get { return _DBName; } set { _DBName = value; } }
        #endregion

        #region Functions

        public Boolean SaveLogs(string connString)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = DAC.SaveLogsExecuteDataTable(Properties.Resources.SP_SaveLog, connString,
                   DAC.Parameter(H_Screen, Screen),
                   DAC.Parameter(H_MessageType, MessageType),
                   DAC.Parameter(H_Message, Message),
                   DAC.Parameter(H_UserSign, UserSign),
                   DAC.Parameter(H_TimeStamp, TimeStamp),
                   DAC.Parameter(H_Remarks, Remarks),
                   DAC.Parameter(H_DBName, DBName));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return true;
        }

        #endregion

    }
}
