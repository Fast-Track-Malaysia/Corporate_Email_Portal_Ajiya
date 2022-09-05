using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class ft_role
    {

        #region Constructor

        public ft_role() { }

        #endregion

        #region Private Members

        private int _id;
        private string _role;
        private string _lastupdateuser;
        private DateTime _lastupdatetime;

        #endregion

        #region Const Column Name

        private const string CN_ID = "id";
        private const string CN_Role = "role";
        private const string CN_LastUpdateUser = "lastupdateuser";
        private const string CN_LastUpdateTime = "lastupdatetime";

        #endregion

        #region Properties

        public int ID { get { return _id; } set { _id = value; } }
        public string Role { get { return _role; } set { _role = value; } }
        public string LastUpdateUser { get { return _lastupdateuser; } set { _lastupdateuser = value; } }
        public DateTime LastUpdateTime { get { return _lastupdatetime; } set { _lastupdatetime = value; } }

        #endregion

        #region Functions

        public static ft_role LoadRoleByID(int id)
        {
            ft_role r = new ft_role();
            DataTable dt = new DataTable();

            dt = DAC.ExecuteDataTable(Properties.Resources.SP_LoadRoleByID,
               DAC.Parameter(CN_ID, id));

            if (dt.Rows.Count > 0)
            {
                try
                {
                    string lastupdatetime = dt.Rows[0][CN_LastUpdateTime].ToString().Trim();

                    r.ID = int.Parse(dt.Rows[0][CN_ID].ToString().Trim());
                    r.Role = dt.Rows[0][CN_Role].ToString().Trim();
                    r.LastUpdateUser = dt.Rows[0][CN_LastUpdateUser].ToString().Trim();
                    if (lastupdatetime != null && lastupdatetime != "")
                    {
                        r.LastUpdateTime = DateTime.Parse(dt.Rows[0][CN_LastUpdateTime].ToString().Trim());
                    }

                }
                catch (Exception ex)
                {
                    r.ID = 0;
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                r.ID = 0;
            }
            return r;
        }

        #endregion
    }
}
