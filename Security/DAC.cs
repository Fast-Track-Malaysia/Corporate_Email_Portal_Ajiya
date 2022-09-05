using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class DAC
    {

        public static string Webcon;
        public static string SAPcon;
        /// <summary>
        /// Calls a stored procedure and return the result
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure to execute</param>
        /// <param name="arrParam">Parameters required by the stored procedure</param>
        /// <returns>DataTable containing the result</returns>
        /// <remarks></remarks>
        public static DataTable ExecuteSAPDataTable(string storedProcedureName, params SqlParameter[] arrParam)
        {
            DataTable dt = null;
            //Open the connection
            string s = SAPcon;
            //TozDAC.Properties.Settings.Default.TozConnectionString.ToString();
            SqlConnection cnn = new SqlConnection(s);
            {
                cnn.Open();
                //Define the commands
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureName;

                //Handle the parameters
                if (arrParam != null)
                {
                    foreach (SqlParameter param in arrParam)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                //Define the data adapter and fill the dataset
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                cnn.Close();
            }
            return dt;


        }

        public static DataTable ExecuteDataTable(string storedProcedureName, params SqlParameter[] arrParam)
        {
            DataTable dt = null;
            //Open the connection
            string s = Webcon;
            //TozDAC.Properties.Settings.Default.TozConnectionString.ToString();
            SqlConnection cnn = new SqlConnection(s);
            {
                cnn.Open();
                //Define the commands
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureName;

                //Handle the parameters
                if (arrParam != null)
                {
                    foreach (SqlParameter param in arrParam)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                //Define the data adapter and fill the dataset
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                cnn.Close();
            }
            return dt;


        }
        public static DataTable SaveLogsExecuteDataTable(string storedProcedureName, string connString, params SqlParameter[] arrParam)
        {
            DataTable dt = null;
            //Open the connection
            string s = Webcon;
            //TozDAC.Properties.Settings.Default.TozConnectionString.ToString();
            SqlConnection cnn = new SqlConnection(connString);
            {
                cnn.Open();
                //Define the commands
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureName;

                //Handle the parameters
                if (arrParam != null)
                {
                    foreach (SqlParameter param in arrParam)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                //Define the data adapter and fill the dataset
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                cnn.Close();
            }
            return dt;


        }

        public static DataTable ExecuteDataTable(string storedProceduceName)
        {
            return ExecuteDataTable(storedProceduceName, null);
        }

        /// <summary>
        /// Creates a parameter
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="parameterValue">Value of the parameter</param>
        /// <returns>SqlParameter Object</returns>
        /// <remarks>The parameter name should be the same as the property name</remarks>
        public static SqlParameter Parameter(string parameterName, object parameterValue)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = parameterName;
            param.Value = parameterValue;
            return param;
        }
    }
}
