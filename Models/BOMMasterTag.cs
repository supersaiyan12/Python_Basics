using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Json;
using Newtonsoft.Json;

namespace MvcApplication2.Models
{
    public class BOMMasterTag
    {

        public string PITag { get; set; }
        public Boolean isManual { get; set; }
        public string BOMCategory { get; set; }
        public string component { get; set; }
        public string componentDescription { get; set; }
        public double quantity { get; set; }
        public string uom { get; set; }
        public double ConcentrationQuantity { get; set; }
        public Boolean isManualWithTag { get; set; }
        public double ActualQuantity { get; set; }
        public string SubRefinery { get; set; }
        public string Batch { get; set; }

        public List<BOMMasterTag> getMasterBOM(string subRefinery, string PONO, int IDOCNumber)
        {
            List<BOMMasterTag> result = new List<BOMMasterTag>();
            List<BOMMasterTag> result1 = new List<BOMMasterTag>();
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader objSqlDataReader = null;

            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlParameter[] objSqlParameter = new SqlParameter[3];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@SubRefinery";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = subRefinery;


            objSqlParameter[1] = new SqlParameter();
            objSqlParameter[1].ParameterName = "@PONO";
            objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[1].Value = PONO;

            objSqlParameter[2] = new SqlParameter();
            objSqlParameter[2].ParameterName = "@IDOCNumber";
            objSqlParameter[2].SqlDbType = SqlDbType.Int;
            objSqlParameter[2].Value = IDOCNumber;

            cmd = new SqlCommand("GetBOMMasterPITags", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);
            cmd.Parameters.Add(objSqlParameter[2]);

            objSqlDataReader = cmd.ExecuteReader();


            while (objSqlDataReader.Read())
            {
                BOMMasterTag objBOMMasterTag = new BOMMasterTag();
                objBOMMasterTag.componentDescription = Convert.ToString(objSqlDataReader["ComponentDescription"]).Trim();
                objBOMMasterTag.component = Convert.ToString(objSqlDataReader["Component"]).Trim();
                objBOMMasterTag.PITag = Convert.ToString(objSqlDataReader["PITagName"]).Trim();
                objBOMMasterTag.SubRefinery = Convert.ToString(objSqlDataReader["Resource"]).Trim();
                objBOMMasterTag.BOMCategory = Convert.ToString(objSqlDataReader["BOMCategory"]).Trim();
                objBOMMasterTag.uom = Convert.ToString(objSqlDataReader["UOM"]).Trim();
                objBOMMasterTag.Batch = Convert.ToString(objSqlDataReader["Batch"]).Trim();
                objBOMMasterTag.quantity = Convert.ToDouble(objSqlDataReader["Quantity"]);
                //objBOMMasterTag.uom = GetUOMFromPOTable(objBOMMasterTag.component, subRefinery, PONO);
                //objBOMMasterTag.Batch = GetBatchFromPOTable(objBOMMasterTag.component, subRefinery, PONO);
                objBOMMasterTag.isManual = Convert.ToBoolean(objSqlDataReader["isManual"]);
                result.Add(objBOMMasterTag);
            }
            objSqlDataReader.Close();
            connection.Close();
            result1 = getMasterBOMUnAvailable(subRefinery, PONO, IDOCNumber);
            foreach (BOMMasterTag objBOMMasterTag1 in result1)
            {
                result.Add(objBOMMasterTag1);
            }



            return result;
        }

        public BOMMasterTag getMainMaterialCode(string subRefinery, string PONO, int IDOCNumber)
        {
            //  List<BOMMasterTag> result = new List<BOMMasterTag>();
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader objSqlDataReader = null;

            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlParameter[] objSqlParameter = new SqlParameter[3];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@SubRefinery";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = subRefinery;


            objSqlParameter[1] = new SqlParameter();
            objSqlParameter[1].ParameterName = "@PONO";
            objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[1].Value = PONO;

            objSqlParameter[2] = new SqlParameter();
            objSqlParameter[2].ParameterName = "@IDOCNumber";
            objSqlParameter[2].SqlDbType = SqlDbType.Int;
            objSqlParameter[2].Value = IDOCNumber;

            cmd = new SqlCommand("GetMainMaterialCode", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);
            cmd.Parameters.Add(objSqlParameter[2]);



            objSqlDataReader = cmd.ExecuteReader();
            BOMMasterTag objBOMMasterTag = new BOMMasterTag();

            while (objSqlDataReader.Read())
            {

                objBOMMasterTag.componentDescription = Convert.ToString(objSqlDataReader["ComponentDescription"]).Trim();
                objBOMMasterTag.component = Convert.ToString(objSqlDataReader["Component"]).Trim();
                objBOMMasterTag.PITag = Convert.ToString(objSqlDataReader["PITagName"]).Trim();
                objBOMMasterTag.BOMCategory = Convert.ToString(objSqlDataReader["BOMCategory"]).Trim();
                objBOMMasterTag.uom = Convert.ToString(objSqlDataReader["UOM"]).Trim();
                objBOMMasterTag.isManual = Convert.ToBoolean(objSqlDataReader["isManual"]);
                //   result.Add(objBOMMasterTag);
            }
            objSqlDataReader.Close();
            connection.Close();
            return objBOMMasterTag;
        }

        public List<BOMMasterTag> getMasterBOMUnAvailable(string subRefinery, string PONO, int IDOCNumber)
        {
            List<BOMMasterTag> result = new List<BOMMasterTag>();
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader objSqlDataReader = null;

            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlParameter[] objSqlParameter = new SqlParameter[3];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@SubRefinery";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = subRefinery;


            objSqlParameter[1] = new SqlParameter();
            objSqlParameter[1].ParameterName = "@PONO";
            objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[1].Value = PONO;

            objSqlParameter[2] = new SqlParameter();
            objSqlParameter[2].ParameterName = "@IDOCNumber";
            objSqlParameter[2].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[2].Value = IDOCNumber;

            cmd = new SqlCommand("GetUnAvailablePO", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);
            cmd.Parameters.Add(objSqlParameter[2]);

            objSqlDataReader = cmd.ExecuteReader();


            while (objSqlDataReader.Read())
            {
                BOMMasterTag objBOMMasterTag = new BOMMasterTag();
                objBOMMasterTag.componentDescription = Convert.ToString(objSqlDataReader["ComponentDescription"]).Trim();
                objBOMMasterTag.component = Convert.ToString(objSqlDataReader["Component"]).Trim();
                objBOMMasterTag.PITag = "";
                objBOMMasterTag.BOMCategory = "";
                objBOMMasterTag.uom = Convert.ToString(objSqlDataReader["BOMUOM"]);
                objBOMMasterTag.Batch = Convert.ToString(objSqlDataReader["Batch"]).Trim();
                objBOMMasterTag.quantity = Convert.ToDouble(objSqlDataReader["Quantity"]);
                objBOMMasterTag.isManual = true;
                result.Add(objBOMMasterTag);
            }
            objSqlDataReader.Close();
            connection.Close();
            return result;

        }

        public string GetUOMFromPOTable(string component, string subRefinery, string PONO)
        {

            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            string uom = "";
            sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            sqlConnection.Open();
            SqlDataReader objSqlDataReader = null;

            SqlParameter[] objSqlParameter = new SqlParameter[3];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@Component";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = component;

            objSqlParameter[1] = new SqlParameter();
            objSqlParameter[1].ParameterName = "@SubRefinery";
            objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[1].Value = subRefinery;

            objSqlParameter[2] = new SqlParameter();
            objSqlParameter[2].ParameterName = "@PONO";
            objSqlParameter[2].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[2].Value = PONO;

            cmd = new SqlCommand("GetUOMFromPOTable", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);
            cmd.Parameters.Add(objSqlParameter[2]);
            objSqlDataReader = cmd.ExecuteReader();

            while (objSqlDataReader.Read())
            {

                uom = Convert.ToString(objSqlDataReader["BOMUOM"]);
            }
            objSqlDataReader.Close();
            sqlConnection.Close();
            return uom;

        }
        public string GetBatchFromPOTable(string component, string subRefinery, string PONO)
        {

            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            string batch = "";
            sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            sqlConnection.Open();
            SqlDataReader objSqlDataReader = null;

            SqlParameter[] objSqlParameter = new SqlParameter[3];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@Component";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = component;

            objSqlParameter[1] = new SqlParameter();
            objSqlParameter[1].ParameterName = "@SubRefinery";
            objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[1].Value = subRefinery;

            objSqlParameter[2] = new SqlParameter();
            objSqlParameter[2].ParameterName = "@PONO";
            objSqlParameter[2].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[2].Value = PONO;

            cmd = new SqlCommand("[GetBatchFromPOTable]", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);
            cmd.Parameters.Add(objSqlParameter[2]);
            objSqlDataReader = cmd.ExecuteReader();

            while (objSqlDataReader.Read())
            {

                batch = Convert.ToString(objSqlDataReader["Batch"]);
            }
            objSqlDataReader.Close();
            sqlConnection.Close();
            return batch;

        }

        public List<BOMMasterTag> getIsSavedList(string PONO, int IDOCNumber)
        {
            List<BOMMasterTag> result = new List<BOMMasterTag>();
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader objSqlDataReader = null;

            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlParameter[] objSqlParameter = new SqlParameter[2];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@PONO";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = PONO;

            objSqlParameter[1] = new SqlParameter();
            objSqlParameter[1].ParameterName = "@IDOCNumber";
            objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[1].Value = IDOCNumber;

            cmd = new SqlCommand("GetIsSaveValue", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);
            objSqlDataReader = cmd.ExecuteReader();

            while (objSqlDataReader.Read())
            {
                BOMMasterTag objBOMMasterTag = new BOMMasterTag();
                objBOMMasterTag.componentDescription = Convert.ToString(objSqlDataReader["ComponentDescription"]).Trim();
                objBOMMasterTag.component = Convert.ToString(objSqlDataReader["Component"]).Trim();
                objBOMMasterTag.SubRefinery = Convert.ToString(objSqlDataReader["SubRefinery"]).Trim();
                objBOMMasterTag.uom = Convert.ToString(objSqlDataReader["UOM"]).Trim();
                objBOMMasterTag.Batch = Convert.ToString(objSqlDataReader["Batch"]).Trim();
                //double quantity = Convert.ToDouble(objSqlDataReader["Quantity"]);
                objBOMMasterTag.ConcentrationQuantity = Convert.ToDouble(objSqlDataReader["ConcentrationQuantity"]);
                string manual_category = GetBomCategoryDetails(objBOMMasterTag.component, objBOMMasterTag.SubRefinery);



                string[] manual_category_split = manual_category.Split('$');

                objBOMMasterTag.BOMCategory = manual_category_split[0].Trim();
                objBOMMasterTag.ActualQuantity = Convert.ToDouble(objSqlDataReader["Quantity"]);
                objBOMMasterTag.quantity = Convert.ToDouble(objSqlDataReader["Quantity"]);
                objBOMMasterTag.isManual = Convert.ToBoolean(manual_category_split[1]);
                //objBOMMasterTag.isManual = Convert.ToBoolean(objSqlDataReader["IsManualWhileSubmission"]);

                //if ((objBOMMasterTag.isManual == false && objBOMMasterTag.BOMCategory == "Chemicals") || (objBOMMasterTag.isManual == false && objBOMMasterTag.BOMCategory == "Utilities" && ((objBOMMasterTag.SubRefinery == "0103NUT1") || (objBOMMasterTag.SubRefinery == "0103WIN1") || (objBOMMasterTag.SubRefinery == "0103BDO1"))))
                if ((objBOMMasterTag.isManual == false && objBOMMasterTag.BOMCategory == "Chemicals") || (objBOMMasterTag.isManual == false && objBOMMasterTag.BOMCategory == "Utilities" && ((objBOMMasterTag.SubRefinery == "101RNT01") || (objBOMMasterTag.SubRefinery == "101RDW01") || (objBOMMasterTag.SubRefinery == "101RBD01"))))
                {
                    //objBOMMasterTag.quantity = Convert.ToDouble(objSqlDataReader["ActualQuantity"]);
                    //objBOMMasterTag.ActualQuantity = Convert.ToDouble(objSqlDataReader["Quantity"]);
                    objBOMMasterTag.ActualQuantity = Convert.ToDouble(objSqlDataReader["ActualQuantity"]);
                    objBOMMasterTag.quantity = Convert.ToDouble(objSqlDataReader["Quantity"]);
                }
                //objBOMMasterTag.quantity = quantity;

                result.Add(objBOMMasterTag);
            }
            objSqlDataReader.Close();
            connection.Close();
            return result;
        }

        public string GetBomCategoryDetails(string component, string subRefinery)
        {

            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            sqlConnection.Open();
            SqlDataReader objSqlDataReader = null;

            SqlParameter[] objSqlParameter = new SqlParameter[2];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@Component";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = component;

            objSqlParameter[1] = new SqlParameter();
            objSqlParameter[1].ParameterName = "@SubRefinery";
            objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[1].Value = subRefinery;

            cmd = new SqlCommand("GetBomCategoryDetails", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);
            objSqlDataReader = cmd.ExecuteReader();
            if (objSqlDataReader.HasRows)
            {
                objSqlDataReader.Read();
                string BOMCategory = Convert.ToString(objSqlDataReader["BOMCategory"]);
                Boolean isManual = Convert.ToBoolean(objSqlDataReader["isManual"]);
                return BOMCategory + '$' + isManual;
            }
            else
            {
                isManual = true;
                BOMCategory = "";
                return BOMCategory + '$' + isManual;
            }
            objSqlDataReader.Close();
            sqlConnection.Close();

        }
    }
}