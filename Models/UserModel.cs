using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.DirectoryServices;
using System.Web.Mvc;

namespace MvcApplication2.Models
{
    public class UserModel 
    {
        public int id {get; set;}
        public string userName {get; set;}
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public bool isAdminUser { get; set; }        
        public string refinery { get; set; }
        public bool isAuthorized { get; set; }
        public string[] SelectedRefinery { get; set; }
        public bool status { get; set; }

       // public string RName { get; set; }
        public List<UserModel> lstUserModel { get; set; }
        public List<SelectListItem> lstrefinery { get; set; }
        
            
        public  bool IsAuthenticated(string usr, string pwd)
        {
            SearchResultCollection objResults = null;
            DirectoryEntry  objDseUserEntry = null;
           string Errmsg = "";
           string ldap = WebConfigurationManager.AppSettings["LDAPPath"];
            try
            {
                DirectoryEntry entry = new DirectoryEntry(ldap, usr, pwd);
                object nativeObject = entry.NativeObject;
                DirectorySearcher objDseSearcher = new DirectorySearcher(entry);
                objDseSearcher.SearchScope = SearchScope.Subtree;
                objDseSearcher.CacheResults = false;
               SearchResult result = objDseSearcher.FindOne();
               if (result != null)
               {
                   objDseUserEntry = result.GetDirectoryEntry();
                   return true;
               }
               else
               {
                   return false;
               }
            //    objResults = objDseSearcher.FindAll();
               
             /*   if (objResults.Count > 0)
                {
                    objDseUserEntry = objResults[0].GetDirectoryEntry();
                    return true;
                }
                else
                {
                    return false;
                }
                */
            }catch (Exception ex)
            {
                Errmsg = ex.Message;
                return false;
                throw new Exception("Error authenticating user." + ex.Message);  
            }       
        }
        
        internal UserModel ValidateUser(string userName)
        {
            SqlCommand cmd;
            SqlConnection connection = null;
            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

             connection.Open();
            UserModel objUserModel = new UserModel();
            SqlDataReader objSqlDataReader = null;

            SqlParameter[] objSqlParameter = new SqlParameter[1];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@UserName";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = userName;
            
            cmd = new SqlCommand("ValidateUser", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            objSqlDataReader = cmd.ExecuteReader();
           
            while (objSqlDataReader.Read())
            {
                objUserModel.id = Convert.ToInt32(objSqlDataReader["id"]);
                objUserModel.isAdminUser = Convert.ToBoolean(objSqlDataReader["IsAdminUser"]);
                objUserModel.firstName = Convert.ToString(objSqlDataReader["FirstName"]);
                objUserModel.lastName = Convert.ToString(objSqlDataReader["lastName"]);
                objUserModel.isAuthorized = Convert.ToBoolean(objSqlDataReader["ValidUser"]);
                objUserModel.refinery = Convert.ToString(objSqlDataReader["Refinery"]);
                //objUserModel.status = Convert.ToBoolean(objSqlDataReader["status"]);
                objUserModel.userName = userName;
            }
            objSqlDataReader.Close();
            connection.Close();
            return objUserModel;
        }


        public static List<SelectListItem> PopulateRefinery()
        {

             SqlCommand cmd;
            SqlConnection connection = null;
           
            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
           // List items = new List();
            SqlDataReader objSqlDataReader = null;
            cmd = new SqlCommand("GetRefineryList", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            objSqlDataReader = cmd.ExecuteReader();
            List<SelectListItem> items = new List<SelectListItem>();
 
            while (objSqlDataReader.Read())
            {
                RefineryViewModel objRefineryViewModel = new RefineryViewModel();
                objRefineryViewModel.RId = Convert.ToString(objSqlDataReader["RID"]);
                objRefineryViewModel.RName = Convert.ToString(objSqlDataReader["RName"]);
                items.Add(new SelectListItem { Text = objRefineryViewModel.RName, Value = objRefineryViewModel.RId });
                objRefineryViewModel = null;
            }
            objSqlDataReader.Close();
            connection.Close();
            return items;
        
            }


        #region Get User Details
        public UserModel GetUserDetails()
        {

            SqlCommand cmd;
            SqlConnection connection = null;
            UserModel objAdminModel = new UserModel();

            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            connection.Open();
            //objSqlDataReader = new SqlDataReader();
            SqlDataReader objSqlDataReader = null;
            try
            {

                cmd = new SqlCommand("GetUsersList", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                lstUserModel = new List<UserModel>();
                objSqlDataReader = cmd.ExecuteReader();//connection, CommandType.StoredProcedure, "OES_ACA_SP_Get_1095CReportRun", objSqlParameter)
                while (objSqlDataReader.Read())
                {
                    UserModel objUserModel = new UserModel();
                    objUserModel.id = Convert.ToInt32(objSqlDataReader["ID"]);
                    objUserModel.firstName = Convert.ToString(objSqlDataReader["FirstName"]);
                    objUserModel.lastName = Convert.ToString(objSqlDataReader["LastName"]);
                    objUserModel.isAdminUser = Convert.ToBoolean(objSqlDataReader["IsAdminUser"]);
                    objUserModel.email = Convert.ToString(objSqlDataReader["EmailID"]);
                    objUserModel.userName = Convert.ToString(objSqlDataReader["UserName"]);
                    objUserModel.refinery = Convert.ToString(objSqlDataReader["refinery"]);
                 //   objAdminModel.SelectedRefinery=[];
                    lstUserModel.Add(objUserModel);

                }
                objSqlDataReader.Close();
                connection.Close();
                objAdminModel.lstUserModel = lstUserModel;
            }
            catch (Exception e)
            {
                return null;
            }
         
            return objAdminModel;
        }
        #endregion Get User Details


        public static string generateID()
        {
            return Guid.NewGuid().ToString("N");
        }

        #region Add User Details
        public bool AddUserDetails(UserModel objUserModel)
        {
            SqlCommand cmd;
            SqlConnection connection = null;


         /*   string result = "";
            for (int i = 0; i < SelectedRefinery.Length; i++)
            {
                if(i==0)
                    result = result + "'"+SelectedRefinery[i]+"'";
                else 
                    result = result + ",'" + SelectedRefinery[i] + "'";
            }*/
            string refineryList = string.Join(",", SelectedRefinery);
            string result = getRefineryName(SelectedRefinery); //string.Join(",", SelectedRefinery);
            string randomId = generateID();
            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();

            SqlParameter[] objSqlParameter = new SqlParameter[7];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@UserName";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = this.userName.Trim();

            objSqlParameter[1] = new SqlParameter();
            objSqlParameter[1].ParameterName = "@Refinery";
            objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[1].Value = result;

            objSqlParameter[2] = new SqlParameter();
            objSqlParameter[2].ParameterName = "@FirstName";
            objSqlParameter[2].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[2].Value = this.firstName.Trim();

            objSqlParameter[3] = new SqlParameter();
            objSqlParameter[3].ParameterName = "@LastName";
            objSqlParameter[3].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[3].Value = this.lastName.Trim();

            objSqlParameter[4] = new SqlParameter();
            objSqlParameter[4].ParameterName = "@Email";
            objSqlParameter[4].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[4].Value = this.email.Trim();

            objSqlParameter[5] = new SqlParameter();
            objSqlParameter[5].ParameterName = "@ID";
            objSqlParameter[5].SqlDbType = SqlDbType.Int;
            objSqlParameter[5].Value = this.id;

            objSqlParameter[6] = new SqlParameter();
            objSqlParameter[6].ParameterName = "@RandomId";
            objSqlParameter[6].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[6].Value = randomId;


            cmd = new SqlCommand("SaveUserDetails", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);
            cmd.Parameters.Add(objSqlParameter[2]);
            cmd.Parameters.Add(objSqlParameter[3]);
            cmd.Parameters.Add(objSqlParameter[4]);
            cmd.Parameters.Add(objSqlParameter[5]);
            cmd.Parameters.Add(objSqlParameter[6]);
            
            bool status = Convert.ToBoolean(cmd.ExecuteNonQuery());//connection, CommandType.StoredProcedure, "OES_ACA_SP_Get_1095CReportRun", objSqlParameter)

            if (status)
            {
                bool userStatus = saveUserWiseRefinery(refineryList, randomId);
            }
            connection.Close();
            return status;
        }
        #endregion Add User Details


        public Boolean CheckExitUser(string UserName)
        {

            SqlCommand cmd;
            SqlConnection connection = null;
            UserModel objAdminModel = new UserModel();

            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            connection.Open();
            Boolean status = false ;
            //objSqlDataReader = new SqlDataReader();
            SqlDataReader objSqlDataReader = null;
            try
            {

                SqlParameter[] objSqlParameter = new SqlParameter[1];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@UserName";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = UserName;

                cmd = new SqlCommand("CheckExistUser", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                objSqlDataReader = cmd.ExecuteReader();//connection, CommandType.StoredProcedure, "OES_ACA_SP_Get_1095CReportRun", objSqlParameter)
                while (objSqlDataReader.Read())
                {
                    status = Convert.ToBoolean(objSqlDataReader["isExistUser"]);                  
                }
            }
            catch (Exception e)
            {
                return false;
            }
            connection.Close();
            objAdminModel.lstUserModel = lstUserModel;
            return status;
        }

        public Boolean deleteUserWiseRefinery(string randomId)
        {
            bool status = false;
            SqlCommand cmd=null;

            SqlConnection connection=null;
            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();


            SqlParameter[] objSqlParameter = new SqlParameter[1];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@RandomId";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = randomId.Trim();

         

            cmd = new SqlCommand("DeleteUserRefineryDetails", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            status = Convert.ToBoolean(cmd.ExecuteNonQuery());
            connection.Close();
          return status;
        }


        public Boolean saveUserWiseRefinery(string result,string randomId)
        {
            bool status = false;
            SqlCommand cmd=null;
            SqlCommand cmd1 = null;

            SqlConnection connection=null;
            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            string[] refinery=result.Split(',');

            deleteUserWiseRefinery(randomId);


            for (int i = 0; i < refinery.Length; i++) { 

            SqlParameter[] objSqlParameter = new SqlParameter[2];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@RandomId";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = randomId.Trim();

            objSqlParameter[1] = new SqlParameter();
            objSqlParameter[1].ParameterName = "@RefineryId";
            objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[1].Value = refinery[i];

            cmd = new SqlCommand("SaveUserRefineryDetails", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);
            status = Convert.ToBoolean(cmd.ExecuteNonQuery());
            }
            connection.Close();
            return status;
        }


        public static string getRefineryName(string[] result)
        {
            string resultStr = "";
            SqlCommand cmd = null;
            SqlConnection connection = null;
            SqlDataReader objSqlDataReader = null; 
            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();


            for (int i = 0; i < result.Length; i++)
            {

                SqlParameter[] objSqlParameter = new SqlParameter[1];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@RefineryId";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = result[i];

        

                cmd = new SqlCommand("GetRefineryNames", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                objSqlDataReader =cmd.ExecuteReader();
                while(objSqlDataReader.Read()){

                  string  resultStr1 = Convert.ToString(objSqlDataReader["refinery"]);
                  resultStr = resultStr + resultStr1 + ",";
                }
                objSqlDataReader.Close();
            }

            connection.Close();
            return resultStr;
        }
 

        #region Edit User Detials
        public UserModel EditUser(int UserID)
        {

            SqlCommand cmd;
            SqlConnection connection = null;
            SqlDataReader objSqlDataReader = null;
            UserModel objUserModel = new UserModel();

            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            connection.Open();

            SqlParameter[] objSqlParameter = new SqlParameter[1];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@UserID";
            objSqlParameter[0].SqlDbType = SqlDbType.Int;
            objSqlParameter[0].Value = UserID;



            cmd = new SqlCommand("EditUserDetails", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);

            objSqlDataReader = cmd.ExecuteReader();
            while (objSqlDataReader.Read())
            {
                objUserModel.id = Convert.ToInt32(objSqlDataReader["ID"]);
                objUserModel.firstName = Convert.ToString(objSqlDataReader["FirstName"]);
                objUserModel.lastName = Convert.ToString(objSqlDataReader["LastName"]);
                objUserModel.userName = Convert.ToString(objSqlDataReader["UserName"]);
                objUserModel.email = Convert.ToString(objSqlDataReader["EmailID"]);
                objUserModel.refinery = Convert.ToString(objSqlDataReader["refinery"]);
             

            }
            objSqlDataReader.Close();
            connection.Close();
            return objUserModel;
        }
        #endregion

        #region Delete User Details
        internal bool DeleteUser(int UserID)
        {
            SqlCommand cmd;
            SqlConnection connection = null;

            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            connection.Open();

            SqlParameter[] objSqlParameter = new SqlParameter[1];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@UserID";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = UserID;



            cmd = new SqlCommand("DeleteUserDetails", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);

            bool status = Convert.ToBoolean(cmd.ExecuteNonQuery());//connection, CommandType.StoredProcedure, "OES_ACA_SP_Get_1095CReportRun", objSqlParameter)

            connection.Close();
            return status;
        }
        #endregion

    }
}