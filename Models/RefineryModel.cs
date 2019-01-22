using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace MvcApplication2.Models
{
    public class RefineryModel
    {
        public int RID { get; set; }
        public string RName { get; set; }
    }
    public class GenericTextValue
    {
        public string RID { get; set; }
        public string RName { get; set; }

        internal dynamic GetRefineryList()
        {
            SqlCommand cmd;
            SqlConnection connection = null;
            BOMDetails objBOMDetails = new BOMDetails();

            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            connection.Open();
            List<GenericTextValue> lstGenericTextValue = new List<GenericTextValue>();
            SqlDataReader objSqlDataReader = null;

            cmd = new SqlCommand("GetRefineryList", connection);
            cmd.CommandType = CommandType.StoredProcedure;
           //  cmd.Parameters.Add(objSqlParameter[0]);
            objSqlDataReader = cmd.ExecuteReader();
            while (objSqlDataReader.Read())
            {
                GenericTextValue objGenericTextValue = new GenericTextValue();
                objGenericTextValue.RID = Convert.ToString(objSqlDataReader["RefineryID"]);
                objGenericTextValue.RName = Convert.ToString(objSqlDataReader["RefineryName"]);
                lstGenericTextValue.Add(objGenericTextValue);
                objGenericTextValue = null;
            }
            objSqlDataReader.Close();
            connection.Close();
            return lstGenericTextValue;
        }


        internal dynamic GetSubRefineryList(int userId)
        {
            SqlCommand cmd;
            SqlConnection connection = null;
            BOMDetails objBOMDetails = new BOMDetails();

            List<GenericTextValue> lstGenericTextValue = new List<GenericTextValue>();
            SqlDataReader objSqlDataReader = null;
            try { 
            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlParameter[] objSqlParameter = new SqlParameter[1];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@UserId";
            objSqlParameter[0].SqlDbType = SqlDbType.Int;
            objSqlParameter[0].Value = userId;



            cmd = new SqlCommand("GetSubRefineryList", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            objSqlDataReader = cmd.ExecuteReader();
            while (objSqlDataReader.Read())
            {
                GenericTextValue objGenericTextValue = new GenericTextValue();
                objGenericTextValue.RID = Convert.ToString(objSqlDataReader["SubRefinery"]);
                objGenericTextValue.RName = Convert.ToString(objSqlDataReader["SubRefinery"]);
                lstGenericTextValue.Add(objGenericTextValue);
                objGenericTextValue = null;
            }
            objSqlDataReader.Close();
            connection.Close();
            }
            catch (Exception ex)
            {

            }
            return lstGenericTextValue;
        }
    }
}