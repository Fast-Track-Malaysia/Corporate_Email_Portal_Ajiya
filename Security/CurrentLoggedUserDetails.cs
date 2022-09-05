using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class CurrentLoggedUserDetails
    {
        public CurrentLoggedUserDetails() { }
        private string _CompnyCode;
        private string _CompnyName;
        private string _UserId;
        private string _UserName;
        private string _Email;
        private int _Role;
        private string _ConnectedDB;
        private string _ConnString;
        private string _SAPDB;
        private string _SAPDBConnString;
        private string _AjiyaDBConnString;

        public string CompnyCode { get { return _CompnyCode; } set { _CompnyCode = value; } }
        public string CompnyName { get { return _CompnyName; } set { _CompnyName = value; } }
        public string UserId { get { return _UserId; } set { _UserId = value; } }
        public string UserName { get { return _UserName; } set { _UserName = value; } }
        public string Email { get { return _Email; } set { _Email = value; } }
        public int Role { get { return _Role; } set { _Role = value; } }
        public string ConnectedDB { get { return _ConnectedDB; } set { _ConnectedDB = value; } }
        public string ConnString { get { return _ConnString; } set { _ConnString = value; } }
        public string SAPDB { get { return _SAPDB; } set { _SAPDB = value; } }
        public string SAPDBConnString { get { return _SAPDBConnString; } set { _SAPDBConnString = value; } }
        public string AjiyaDBConnString { get { return _AjiyaDBConnString; } set { _AjiyaDBConnString = value; } }

    }
}
