using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Security
{
    public class AppUtilities
    {
        public static string getCommonDBConnString()
        {
            string connString = WebConfigurationManager.ConnectionStrings["CommonDBConnString"].ConnectionString;
            return connString;
        }
    }
}
