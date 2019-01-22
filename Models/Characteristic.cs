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
    public class Characteristic
    {
        public int CID { get; set; }
        public string CharName { get; set; }
        // public string BatchClassName { get; set; }
        public string CharValue { get; set; }
        public List<string> SubCharactristic { get; set; }


        public List<Characteristic> CalculateCharacteristic(string PONO)
        {
            List<Characteristic> result = new List<Characteristic>();

            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader objSqlDataReader = null;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                connection.Open();
                SqlParameter[] objSqlParameter = new SqlParameter[1];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@PONO";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = PONO;


                cmd = new SqlCommand("GetBatchClassCharacteristic", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);


                objSqlDataReader = cmd.ExecuteReader();


                while (objSqlDataReader.Read())
                {
                    Characteristic objCharacteristic = new Characteristic();
                    objCharacteristic.CID = Convert.ToInt32(objSqlDataReader["CID"]);
                    objCharacteristic.CharName = Convert.ToString(objSqlDataReader["CDescription"]).Trim();
                    //  objCharacteristic.CharValue = Convert.ToDouble(objSqlDataReader["CName"]);
                    objCharacteristic.SubCharactristic = getCharDropdownValue(objCharacteristic.CID);
                    result.Add(objCharacteristic);

                }

                objSqlDataReader.Close();
                connection.Close();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }

        }


        public List<string> getCharDropdownValue(int CID)
        {
            List<string> result = new List<string>();
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader objSqlDataReader = null;

            try
            {

                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                connection.Open();
                SqlParameter[] objSqlParameter = new SqlParameter[1];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@CID";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = CID;


                cmd = new SqlCommand("GetCharacteristicDropdown", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);


                objSqlDataReader = cmd.ExecuteReader();


                while (objSqlDataReader.Read())
                {

                    string CharName = Convert.ToString(objSqlDataReader["CDescription"]).Trim();

                    result.Add(CharName);
                }

                objSqlDataReader.Close();
                connection.Close();
            }
            catch (Exception e)
            {

            }
            return result;
        }


        public List<Characteristic> SavedCalculateCharacteristic(string PONO)
        {
            List<Characteristic> result = new List<Characteristic>();
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader objSqlDataReader = null;

            try
            {

                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                connection.Open();
                SqlParameter[] objSqlParameter = new SqlParameter[1];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@PONO";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = PONO;


                cmd = new SqlCommand("GetSavedCharacteristic", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);


                objSqlDataReader = cmd.ExecuteReader();


                while (objSqlDataReader.Read())
                {
                    Characteristic objCharacteristic = new Characteristic();
                    objCharacteristic.CID = Convert.ToInt32(objSqlDataReader["CID"]);
                    objCharacteristic.CharName = Convert.ToString(objSqlDataReader["CDescription"]).Trim();
                    objCharacteristic.CharValue = Convert.ToString(objSqlDataReader["CharValue"]).Trim();
                    objCharacteristic.SubCharactristic = getCharDropdownValue(objCharacteristic.CID);
                    result.Add(objCharacteristic);
                }

                objSqlDataReader.Close();
                connection.Close();
            }
            catch (Exception e)
            {

            }
            return result;
        }

        public Boolean SaveCharacteristics(string result, string PONO, int userId, int IDOCNumber)
        {
            SqlCommand cmd;
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                connection.Open();


                List<Dictionary<string, string>> obj = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result);
                foreach (Dictionary<string, string> bcd in obj)
                {
                    KeyValue objKeyValue = new KeyValue();
                    foreach (KeyValuePair<string, string> pair in bcd)
                    {
                        if (pair.Key == "key")
                        {
                            objKeyValue.key = Convert.ToString(pair.Value);
                        }
                        if (pair.Key == "value")
                        {
                            objKeyValue.value = Convert.ToString(pair.Value);
                        }
                    }
                    if ((objKeyValue.value) == null)
                        objKeyValue.value = "";

                    SqlParameter[] objSqlParameter = new SqlParameter[5];
                    objSqlParameter[0] = new SqlParameter();
                    objSqlParameter[0].ParameterName = "@CharactristicID";
                    objSqlParameter[0].SqlDbType = SqlDbType.Int;
                    objSqlParameter[0].Value = Convert.ToInt32((objKeyValue.key));

                    objSqlParameter[1] = new SqlParameter();
                    objSqlParameter[1].ParameterName = "@CharactristicValue";
                    objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[1].Value = objKeyValue.value;

                    objSqlParameter[2] = new SqlParameter();
                    objSqlParameter[2].ParameterName = "@PONO";
                    objSqlParameter[2].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[2].Value = PONO;

                    objSqlParameter[3] = new SqlParameter();
                    objSqlParameter[3].ParameterName = "@SessionUserId";
                    objSqlParameter[3].SqlDbType = SqlDbType.Int;
                    objSqlParameter[3].Value = userId;

                    objSqlParameter[4] = new SqlParameter();
                    objSqlParameter[4].ParameterName = "@IDOCNumber";
                    objSqlParameter[4].SqlDbType = SqlDbType.Int;
                    objSqlParameter[4].Value = IDOCNumber;



                    cmd = new SqlCommand("SaveCharacteristics", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(objSqlParameter[0]);
                    cmd.Parameters.Add(objSqlParameter[1]);
                    cmd.Parameters.Add(objSqlParameter[2]);
                    cmd.Parameters.Add(objSqlParameter[3]);


                    Convert.ToBoolean(cmd.ExecuteNonQuery());//connection, CommandType.StoredProcedure, "OES_ACA_SP_Get_1095CReportRun", objSqlParameter)
                }

                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


    }
}