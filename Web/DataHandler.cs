using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web
{
    public class DataHandler
    {
        public static string GetSQLSafeString(string data)
        {
            if (string.IsNullOrEmpty(data) == true)
            {
                return "";
            }
            return data.Replace("'", "''");
        }
    }
}