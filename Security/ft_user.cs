using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class ft_user
    {
        #region Constructor

        public ft_user() { }

        #endregion

        #region Private Members

        private int _Id;
        private string _Code;
        private string _Password;
        private string _Name;
        private int _isActive;
        private int _Role;
        private string _LastUpdateUser;
        private DateTime _LastUpdateTime;
        private string _Email;
        private string _DBName;

        #endregion

        #region Const Column Name

        private const string Col_Id             = "Id";
        private const string Col_Code           = "Code";
        private const string Col_Password       = "Password";
        private const string Col_Name           = "Name";
        private const string Col_IsActive       = "isActive";
        private const string Col_Role           = "Role";
        private const string Col_LastUpdateUser = "LastUpdateUser";
        private const string Col_LastUpdateTime = "LastUpdateTime";
        private const string Col_Email          = "Email";
        private const string Col_DBName         = "DBName";
        #endregion

        #region Properties

        public int ID { get { return _Id; } set { _Id = value; } }
        public string Code { get { return _Code; } set { _Code = value; } }
        public string Password { get { return _Password; } set { _Password = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }
        public int IsActive { get { return _isActive; } set { _isActive = value; } }
        public int Role { get { return _Role; } set { _Role = value; } }
        public string LastUpdateUser { get { return _LastUpdateUser; } set { _LastUpdateUser = value; } }
        public DateTime LastUpdateTime { get { return _LastUpdateTime; } set { _LastUpdateTime = value; } }
        public string Email { get { return _Email; } set { _Email = value; } }
        public string DBName { get { return _DBName; } set { _DBName = value; } }
        #endregion

        #region Functions

        public static ft_user LoadUser(string db, string userID, string password)
        {
            ft_user user = new ft_user();



            //DataTable dt = new DataTable();

            //if (id == 0)
            //{
            //    dt = DAC.ExecuteDataTable(Properties.Resources.SP_LoadUserByCode,
            //       DAC.Parameter(Col_Code, code));
            //}
            //else
            //{
            //    dt = DAC.ExecuteDataTable(Properties.Resources.SP_LoadUserByID,
            //           DAC.Parameter(Col_Id, id));
            //}

            //if (dt.Rows.Count > 0)
            //{
            //    try
            //    {
            //        string lastUpdateTime = dt.Rows[0][Col_LastUpdateTime].ToString().Trim();

            //        user.ID = int.Parse(dt.Rows[0][Col_Id].ToString().Trim());
            //        user.Code = dt.Rows[0][Col_Code].ToString().Trim();
            //        user.Password = dt.Rows[0][Col_Password].ToString().Trim();
            //        user.Name = dt.Rows[0][Col_Name].ToString().Trim();
            //        user.IsActive = int.Parse(dt.Rows[0][Col_IsActive].ToString().Trim());
            //        user.Role = int.Parse(dt.Rows[0][Col_Role].ToString().Trim());
            //        user.LastUpdateUser = dt.Rows[0][Col_LastUpdateUser].ToString().Trim();
            //        if (lastUpdateTime != null && lastUpdateTime != "")
            //        {
            //            user.LastUpdateTime = DateTime.Parse(dt.Rows[0][Col_LastUpdateTime].ToString().Trim());
            //        }
            //        user.Email = dt.Rows[0][Col_Email].ToString().Trim();
            //        user.DBName = dt.Rows[0][Col_DBName].ToString().Trim();
            //    }
            //    catch (Exception ex)
            //    {
            //        user.ID = 0;
            //        throw new Exception(ex.Message);
            //    }
            //}
            //else
            //{
            //    user.ID = 0;
            //}

            return user;
        }
        public Boolean Save()
        {
            Boolean a = false;

            DataTable dt = new DataTable();
            try
            {

                dt = DAC.ExecuteDataTable(Properties.Resources.SP_SaveUser,
                    DAC.Parameter(Col_Code, Code),
                    DAC.Parameter(Col_Password, Password),
                    DAC.Parameter(Col_Name, Name),              
                    DAC.Parameter(Col_Role, Role),
                    DAC.Parameter(Col_IsActive, IsActive),
                    DAC.Parameter(Col_LastUpdateUser, LastUpdateUser),
                    DAC.Parameter(Col_Email, Email),
                    DAC.Parameter(Col_DBName, DBName));
                a = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return a;
        }
        public Boolean Delete()
        {
            Boolean a = false;

            DataTable dt = new DataTable();
            try
            {

                dt = DAC.ExecuteDataTable(Properties.Resources.SP_DeleteUser,
                    DAC.Parameter(Col_Id, ID));

                a = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return a;
        }
        #endregion
    }
}
