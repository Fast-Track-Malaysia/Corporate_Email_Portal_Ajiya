using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class ConvertDate
    {

        public static string Convert(string date)
        {
            //
            // TODO: Add constructor logic here
            //
            string dd = date.Substring(0, 2);
            string MM = date.Substring(3, 2);
            string yyyy = date.Substring(6, 4);

            return yyyy + "/" + MM + "/" + dd;

        }
        public static string ConvertShort(string date)
        {
            //
            // TODO: Add constructor logic here
            //
            string dd = date.Substring(0, 2);
            string MM = date.Substring(3, 2);
            string yyyy = date.Substring(6, 2);

            return "20" + yyyy + "/" + MM + "/" + dd;

        }
        public static string ConvertddMMyyyy(string date)
        {
            //
            // TODO: Add constructor logic here
            //
            string dd = date.Substring(8, 2);
            string MM = date.Substring(5, 2);
            string yyyy = date.Substring(0, 4);

            return dd + "-" + MM + "-" + yyyy;

        }
    }
}
