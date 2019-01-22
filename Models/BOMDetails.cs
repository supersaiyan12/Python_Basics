using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Json;
using Newtonsoft.Json;
using System.IO;


namespace MvcApplication2.Models
{
    public class BOMDetails
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public string processOrederNo { get; set; }
        public float itemNo { get; set; }
        public string component { get; set; }
        public string componentDescription { get; set; }
        public double quantity { get; set; }
        public string uom { get; set; }
        public double actualQuantity { get; set; }
        public string status { get; set; }
        public string material { get; set; }
        public string refinery { get; set; }
        public string refineryName { get; set; }
        public string subRefineryName { get; set; }
        public string PlanStartDate { get; set; }
        public string PlanStartTime { get; set; }
        public string PlanEndDate { get; set; }
        public string PlanEndTime { get; set; }
        public string ActualStartDate { get; set; }
        public string ActualStartTime { get; set; }
        public string ActualEndDate { get; set; }
        public string ActualEndTime { get; set; }
        public string isSaved { get; set; }
        public string feedInTag { get; set; }
        public string feedOutTag { get; set; }
        public string refineryac { get; set; }
        public string subrefinerysub { get; set; }
        public string subrefineryName { get; set; }
        public string submitedDateTime { get; set; }
        public int SubmittedUserID { get; set; }
        public string isSaveddate { get; set; }
        public Boolean isSaved_id { get; set; }
        public string PITag { get; set; }
        public Boolean isManual { get; set; }
        public Boolean IsManualWhileSubmission { get; set; }
        public string BOMCategory { get; set; }
        public int IDOCNumber { get; set; }
        public string postingDate { get; set; }
        public string productionDate { get; set; }
        public string UserName { get; set; }
        public string AddedDateTime { get; set; }
        public List<BOMDetails> lstBOMDetails { get; set; }

        public void GetBOMDetailList(int userId)
        {

            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            SqlDataReader objSqlDataReader = null;
            try
            {
                sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                sqlConnection.Open();

                SqlParameter[] objSqlParameter = new SqlParameter[1];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@UserId";
                objSqlParameter[0].SqlDbType = SqlDbType.Int;
                objSqlParameter[0].Value = userId;

                this.lstBOMDetails = new List<BOMDetails>();
                cmd = new SqlCommand("GetPOList", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                objSqlDataReader = cmd.ExecuteReader();
                while (objSqlDataReader.Read())
                {

                    BOMDetails objBOMDetails = new BOMDetails();

                    objBOMDetails.processOrederNo = Convert.ToString(objSqlDataReader["ProcessOrderNo"]);
                    objBOMDetails.material = Convert.ToString(objSqlDataReader["MaterialDescription"]);
                    string rf = Convert.ToString(objSqlDataReader["SubRefinery"]);
                    string po_no = Convert.ToString(objSqlDataReader["ProcessOrderNo"]);
                    objBOMDetails.PlanEndDate = Convert.ToString(objSqlDataReader["PlanEndDate"]);
                    objBOMDetails.PlanEndTime = Convert.ToString(objSqlDataReader["PlanEndTime"]);
                    objBOMDetails.PlanStartDate = Convert.ToString(objSqlDataReader["PlanStartDate"]);
                    objBOMDetails.PlanStartTime = Convert.ToString(objSqlDataReader["PlanStartTime"]);
                    objBOMDetails.AddedDateTime = Convert.ToDateTime(objSqlDataReader["AddedDateTime"]).ToString("dd/MM/yyyy");
                    objBOMDetails.isSaved_id = Convert.ToBoolean(objSqlDataReader["IsSaved"]);
                    objBOMDetails.IDOCNumber = Convert.ToInt32(objSqlDataReader["IDOCNumber"]);
                    objBOMDetails.refineryName = Convert.ToString(objSqlDataReader["RefineryName"]);
                    objBOMDetails.subrefineryName = Convert.ToString(objSqlDataReader["SubRefineryName"]);

                    string data_date = Getsaveddate(po_no, objBOMDetails.IDOCNumber);
                    objBOMDetails.isSaveddate = data_date;

                    lstBOMDetails.Add(objBOMDetails);
                    objBOMDetails = null;
                }
                objSqlDataReader.Close();
                sqlConnection.Close();
            }
            catch (Exception e)
            {

            }

        }



        public void dosearch(DateTime selected_start, DateTime selected_end, int userId)
        {

            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            SqlDataReader objSqlDataReader = null;
            try
            {
                sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                sqlConnection.Open();


                // string 
                SqlParameter[] objSqlParameter = new SqlParameter[3];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@UserId";
                objSqlParameter[0].SqlDbType = SqlDbType.Int;
                objSqlParameter[0].Value = userId;

                objSqlParameter[1] = new SqlParameter();
                objSqlParameter[1].ParameterName = "@StartDate";
                objSqlParameter[1].SqlDbType = SqlDbType.DateTime;
                objSqlParameter[1].Value = selected_start;

                objSqlParameter[2] = new SqlParameter();
                objSqlParameter[2].ParameterName = "@EndDate";
                objSqlParameter[2].SqlDbType = SqlDbType.DateTime;
                objSqlParameter[2].Value = selected_end;

                this.lstBOMDetails = new List<BOMDetails>();
                cmd = new SqlCommand("GetSearchPOList", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                cmd.Parameters.Add(objSqlParameter[1]);
                cmd.Parameters.Add(objSqlParameter[2]);

                objSqlDataReader = cmd.ExecuteReader();
                while (objSqlDataReader.Read())
                {

                    BOMDetails objBOMDetails = new BOMDetails();
                    // objBOMDetails.id = Convert.ToInt32(objSqlDataReader["id"]);
                    objBOMDetails.processOrederNo = Convert.ToString(objSqlDataReader["ProcessOrderNo"]);
                    objBOMDetails.material = Convert.ToString(objSqlDataReader["MaterialDescription"]);
                    string rf = Convert.ToString(objSqlDataReader["SubRefinery"]);
                    //    string data_check = GetRefineryName(rf);

                    string po_no = Convert.ToString(objSqlDataReader["ProcessOrderNo"]);



                    //     string[] data_check_split = data_check.Split('$');

                    objBOMDetails.IDOCNumber = Convert.ToInt32(objSqlDataReader["IDOCNumber"]);
                    objBOMDetails.refinery = Convert.ToString(objSqlDataReader["PlanStartDate"]);
                    objBOMDetails.PlanEndDate = Convert.ToString(objSqlDataReader["PlanEndDate"]);
                    objBOMDetails.PlanEndTime = Convert.ToString(objSqlDataReader["PlanEndTime"]);
                    objBOMDetails.PlanStartDate = Convert.ToString(objSqlDataReader["PlanStartDate"]);
                    objBOMDetails.PlanStartTime = Convert.ToString(objSqlDataReader["PlanStartTime"]);
                    objBOMDetails.isSaved_id = Convert.ToBoolean(objSqlDataReader["IsSaved"]);
                    string data_date = Getsaveddate(po_no, objBOMDetails.IDOCNumber);
                    objBOMDetails.isSaveddate = data_date;
                    objBOMDetails.refineryName = Convert.ToString(objSqlDataReader["RefineryName"]);
                    objBOMDetails.subrefineryName = Convert.ToString(objSqlDataReader["SubRefineryName"]);
                    objBOMDetails.AddedDateTime = Convert.ToDateTime(objSqlDataReader["AddedDateTime"]).ToString("dd/MM/yyyy");

                    // objBOMDetails.refinery = refinery;
                    lstBOMDetails.Add(objBOMDetails);
                    objBOMDetails = null;
                }
                objSqlDataReader.Close();
                sqlConnection.Close();
            }
            catch (Exception e)
            {

            }
        }



        public void searchSubmitedPO(DateTime selected_start, DateTime selected_end, int userId)
        {

            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            SqlDataReader objSqlDataReader = null;
            try
            {
                sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                sqlConnection.Open();


                // string 
                SqlParameter[] objSqlParameter = new SqlParameter[3];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@UserId";
                objSqlParameter[0].SqlDbType = SqlDbType.Int;
                objSqlParameter[0].Value = userId;

                objSqlParameter[1] = new SqlParameter();
                objSqlParameter[1].ParameterName = "@StartDate";
                objSqlParameter[1].SqlDbType = SqlDbType.DateTime;
                objSqlParameter[1].Value = selected_start;

                objSqlParameter[2] = new SqlParameter();
                objSqlParameter[2].ParameterName = "@EndDate";
                objSqlParameter[2].SqlDbType = SqlDbType.DateTime;
                objSqlParameter[2].Value = selected_end;

                this.lstBOMDetails = new List<BOMDetails>();
                cmd = new SqlCommand("GetSearchSubmitedPOList", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                cmd.Parameters.Add(objSqlParameter[1]);
                cmd.Parameters.Add(objSqlParameter[2]);

                objSqlDataReader = cmd.ExecuteReader();
                while (objSqlDataReader.Read())
                {

                    BOMDetails objBOMDetails = new BOMDetails();
                    objBOMDetails.processOrederNo = Convert.ToString(objSqlDataReader["ProcessOrderNo"]);
                    objBOMDetails.material = Convert.ToString(objSqlDataReader["MaterialDescription"]);
                    objBOMDetails.IDOCNumber = Convert.ToInt32(objSqlDataReader["IDOCNumber"]);
                    string rf = Convert.ToString(objSqlDataReader["SubRefinery"]);
                    //  string data_check = GetRefineryName(rf);
                    string po_no = Convert.ToString(objSqlDataReader["ProcessOrderNo"]);
                    string data_date = Getsubmitteddate(po_no, objBOMDetails.IDOCNumber);

                    //   string[] data_check_split = data_check.Split('$');

                    objBOMDetails.refinery = Convert.ToString(objSqlDataReader["PlanStartDate"]);
                    objBOMDetails.PlanStartDate = Convert.ToString(objSqlDataReader["PlanStartDate"]);
                    objBOMDetails.PlanStartTime = Convert.ToString(objSqlDataReader["PlanStartTime"]);
                    objBOMDetails.isSaved_id = Convert.ToBoolean(objSqlDataReader["IsSaved"]);
                    objBOMDetails.submitedDateTime = data_date;
                    objBOMDetails.UserName = Convert.ToString(objSqlDataReader["SubmitedBy"]);
                    objBOMDetails.refineryName = Convert.ToString(objSqlDataReader["RefineryName"]);
                    objBOMDetails.subrefineryName = Convert.ToString(objSqlDataReader["SubRefineryName"]);
                    objBOMDetails.AddedDateTime = Convert.ToDateTime(objSqlDataReader["AddedDateTime"]).ToString("dd/MM/yyyy");
                    //  objBOMDetails.refineryName = data_check_split[0];
                    //  objBOMDetails.subrefineryName = data_check_split[1];
                    lstBOMDetails.Add(objBOMDetails);
                    objBOMDetails = null;
                }
                objSqlDataReader.Close();
                sqlConnection.Close();
            }
            catch (Exception e)
            {

            }
        }



        public void GetSubmitedPOList(int userId)
        {

            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                sqlConnection.Open();
                SqlDataReader objSqlDataReader = null;

                // string 
                SqlParameter[] objSqlParameter = new SqlParameter[1];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@UserId";
                objSqlParameter[0].SqlDbType = SqlDbType.Int;
                objSqlParameter[0].Value = userId;

                this.lstBOMDetails = new List<BOMDetails>();
                cmd = new SqlCommand("GetSubmitedPOList", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                objSqlDataReader = cmd.ExecuteReader();
                while (objSqlDataReader.Read())
                {
                    BOMDetails objBOMDetails = new BOMDetails();
                    objBOMDetails.processOrederNo = Convert.ToString(objSqlDataReader["ProcessOrderNo"]);
                    objBOMDetails.material = Convert.ToString(objSqlDataReader["MaterialDescription"]);
                    objBOMDetails.IDOCNumber = Convert.ToInt32(objSqlDataReader["IDOCNumber"]);
                    string po_no = Convert.ToString(objSqlDataReader["ProcessOrderNo"]);
                    string data_date = Getsubmitteddate(po_no, objBOMDetails.IDOCNumber);

                    objBOMDetails.submitedDateTime = data_date;
                    string rf = Convert.ToString(objSqlDataReader["SubRefinery"]);
                    //   string data_check = GetRefineryName(rf);

                    //  string[] data_check_split = data_check.Split('$');

                    objBOMDetails.refineryName = Convert.ToString(objSqlDataReader["RefineryName"]);
                    objBOMDetails.subrefineryName = Convert.ToString(objSqlDataReader["SubRefineryName"]);
                    objBOMDetails.UserName = Convert.ToString(objSqlDataReader["SubmitedBy"]);
                    //objBOMDetails.refineryName = data_check_split[0];
                    //objBOMDetails.subrefineryName = data_check_split[1];

                    lstBOMDetails.Add(objBOMDetails);
                    objBOMDetails = null;
                }
                objSqlDataReader.Close();
                sqlConnection.Close();
            }
            catch (Exception e)
            {

            }
        }



        public string Getsubmitteddate(string ProcessOrderNo, int IDOCNumber)
        {

            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                sqlConnection.Open();
                SqlDataReader objSqlDataReader = null;

                SqlParameter[] objSqlParameter = new SqlParameter[2];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@ProcessOrderNo";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = ProcessOrderNo;

                objSqlParameter[1] = new SqlParameter();
                objSqlParameter[1].ParameterName = "@IDOCNumber";
                objSqlParameter[1].SqlDbType = SqlDbType.Int;
                objSqlParameter[1].Value = IDOCNumber;

                cmd = new SqlCommand("GetSubmittedPoDate", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                cmd.Parameters.Add(objSqlParameter[1]);

                objSqlDataReader = cmd.ExecuteReader();
                string sDate = "";
                while (objSqlDataReader.Read())
                {
                    sDate = Convert.ToString(objSqlDataReader["SubmittedDateTime"]);
                }

                objSqlDataReader.Close();
                sqlConnection.Close();
                return sDate;
            }
            catch (Exception e)
            {
                return null;
            }

        }


        public string Getsaveddate(string ProcessOrderNo, int IDOCNumber)
        {

            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            string saveDate = "";
            try
            {
                sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                sqlConnection.Open();
                SqlDataReader objSqlDataReader = null;

                SqlParameter[] objSqlParameter = new SqlParameter[2];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@ProcessOrderNo";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = ProcessOrderNo;

                objSqlParameter[1] = new SqlParameter();
                objSqlParameter[1].ParameterName = "@IDOCNumber";
                objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[1].Value = IDOCNumber;

                cmd = new SqlCommand("GetsavePoDate", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                cmd.Parameters.Add(objSqlParameter[1]);
                objSqlDataReader = cmd.ExecuteReader();

                while (objSqlDataReader.Read())
                {
                    saveDate = Convert.ToString(objSqlDataReader["SavedDateTime"]);
                }
                objSqlDataReader.Close();
                sqlConnection.Close();
                return saveDate;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public string GetRefineryName(string subRefineryName)
        {

            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            SqlDataReader objSqlDataReader = null;
            string refineryName = "";
            string sRefineryName = "";
            try
            {
                sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                sqlConnection.Open();

                SqlParameter[] objSqlParameter = new SqlParameter[1];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@SubRefineryName";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = subRefineryName;

                cmd = new SqlCommand("GetRefineryName", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                objSqlDataReader = cmd.ExecuteReader();

                while (objSqlDataReader.Read())
                {

                    refineryName = Convert.ToString(objSqlDataReader["RName"]);
                    sRefineryName = Convert.ToString(objSqlDataReader["SubRefineryName"]);
                }
                objSqlDataReader.Close();
                sqlConnection.Close();

            }
            catch (Exception ex)
            {

            }
            return refineryName + '$' + sRefineryName;
        }


        public BOMDetails getProcessOrder(string PONumber, int IDOCNumber)
        {
            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            SqlDataReader objSqlDataReader = null;
            BOMDetails objBOMDetails = new BOMDetails();
            try
            {
                sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                sqlConnection.Open();


                SqlParameter[] objSqlParameter = new SqlParameter[2];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@ProcessOrderNO";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = PONumber;

                objSqlParameter[1] = new SqlParameter();
                objSqlParameter[1].ParameterName = "@IDOCNumber";
                objSqlParameter[1].SqlDbType = SqlDbType.Int;
                objSqlParameter[1].Value = IDOCNumber;


                cmd = new SqlCommand("GetProcessOrderDetails", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                cmd.Parameters.Add(objSqlParameter[1]);
                objSqlDataReader = cmd.ExecuteReader();

                while (objSqlDataReader.Read())
                {

                    objBOMDetails.processOrederNo = Convert.ToString(objSqlDataReader["ProcessOrderNo"]);
                    objBOMDetails.material = Convert.ToString(objSqlDataReader["MaterialDescription"]);
                    objBOMDetails.quantity = Convert.ToDouble(objSqlDataReader["Quantity"]);
                    //objBOMDetails.actualQuantity = Convert.ToDouble(objSqlDataReader["ActualQuantity"]);
                    objBOMDetails.refinery = Convert.ToString(objSqlDataReader["SubRefinery"]);
                    objBOMDetails.PlanStartDate = Convert.ToString(objSqlDataReader["PlanStartDate"]);
                    objBOMDetails.PlanStartTime = Convert.ToString(objSqlDataReader["PlanStartTime"]);
                    objBOMDetails.PlanEndDate = Convert.ToString(objSqlDataReader["PlanEndDate"]);
                    objBOMDetails.PlanEndTime = Convert.ToString(objSqlDataReader["PlanEndTime"]);
                    objBOMDetails.isSaved = Convert.ToString(objSqlDataReader["isSaved"]);
                    objBOMDetails.ActualStartDate = Convert.ToString(objSqlDataReader["ActualStartDate"]);
                    objBOMDetails.ActualStartTime = Convert.ToString(objSqlDataReader["ActualStartTime"]);
                    objBOMDetails.ActualEndDate = Convert.ToString(objSqlDataReader["ActualEndDate"]);
                    objBOMDetails.ActualEndTime = Convert.ToString(objSqlDataReader["ActualEndTime"]);
                    objBOMDetails.postingDate = Convert.ToString(objSqlDataReader["PostingDate"]);
                    objBOMDetails.productionDate = Convert.ToString(objSqlDataReader["ProductionDate"]);
                    objBOMDetails.AddedDateTime = Convert.ToDateTime(objSqlDataReader["AddedDateTime"]).ToString("dd/MM/yyyy");
                    objBOMDetails.uom = Convert.ToString(objSqlDataReader["UOM"]);
                    string getRefinery = GetRefineryName(objBOMDetails.refinery);
                    string[] getRefinery_split = getRefinery.Split('$');
                    objBOMDetails.subRefineryName = getRefinery_split[1];
                    objBOMDetails.IDOCNumber = Convert.ToInt32(objSqlDataReader["IDOCNumber"]); //getIDOCNo(Convert.ToString(objSqlDataReader["ProcessOrderNo"]));

                }
                objSqlDataReader.Close();
                sqlConnection.Close();
            }
            catch (Exception ed)
            {

            }
            return objBOMDetails;
        }


        public int getIDOCNo(string PONo)
        {
            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            SqlDataReader objSqlDataReader = null;
            int idocNo = 0;
            try
            {
                sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                sqlConnection.Open();


                SqlParameter[] objSqlParameter = new SqlParameter[1];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@ProcessOrderNO";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = PONo;
                // this.lstBOMDetails = new List<BOMDetails>();
                cmd = new SqlCommand("GetProcessOrderIDOCNo", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                objSqlDataReader = cmd.ExecuteReader();

                while (objSqlDataReader.Read())
                {

                    idocNo = Convert.ToInt32(objSqlDataReader["IDOCNumber"]);
                }
                objSqlDataReader.Close();
                sqlConnection.Close();
            }
            catch (Exception)
            {

            }
            return idocNo;
        }


        public BOMDetails getFeedInFeedOutTag(string subRefinery)
        {
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader objSqlDataReader = null;
            BOMDetails objBOMDetails = new BOMDetails();
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                connection.Open();
                SqlParameter[] objSqlParameter = new SqlParameter[1];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@SubRefinery";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = subRefinery;

                cmd = new SqlCommand("GetFeedInFeedOutPITags", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                objSqlDataReader = cmd.ExecuteReader();

                while (objSqlDataReader.Read())
                {
                    objBOMDetails.feedInTag = Convert.ToString(objSqlDataReader["InputTag"]).Trim();
                    objBOMDetails.feedOutTag = Convert.ToString(objSqlDataReader["OutputTag"]).Trim();
                }
                objSqlDataReader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {

            }
            return objBOMDetails;
        }


        public Boolean SavePODetails(string ActualStartDate, string ActualStartTime, string ActualEndDate, string ActualEndTime, string result, string PONO, int userId, int IDOCNumber, string PostingDate, string ProductionDate)
        {
            SqlCommand cmd;
            SqlConnection connection = null;
            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();

            List<Dictionary<string, string>> obj = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result);
            foreach (Dictionary<string, string> bcd in obj)
            {
                KeyValueBatch keyValueBatch = new KeyValueBatch();
                foreach (KeyValuePair<string, string> pair in bcd)
                {
                    if (pair.Key == "key")
                    {
                        string valueOfCOmponant = Convert.ToString(pair.Value);

                        if (valueOfCOmponant.Contains("_"))
                        {
                            string[] splittedData = valueOfCOmponant.Split('_');
                            string componentKey = splittedData[0];
                            string batchKey = splittedData[1];

                            keyValueBatch.key = componentKey;
                            keyValueBatch.BatchName = batchKey;

                        }
                        else
                        {
                            string componentKey = valueOfCOmponant;
                            string batchKey = "";

                            keyValueBatch.key = componentKey;
                            keyValueBatch.BatchName = batchKey;
                        }
                    }

                    if (pair.Key == "value")
                    {

                        keyValueBatch.value = Convert.ToDouble(pair.Value);
                    }

                    if (pair.Key == "IsmanualSubmit")
                    {

                        keyValueBatch.IsmanualSubmit = Convert.ToBoolean(pair.Value);
                    }
                }


                SqlParameter[] objSqlParameter = new SqlParameter[15];
                objSqlParameter[0] = new SqlParameter();
                objSqlParameter[0].ParameterName = "@ActualStartDate";
                objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[0].Value = ActualStartDate;

                objSqlParameter[1] = new SqlParameter();
                objSqlParameter[1].ParameterName = "@ActualStartTime";
                objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[1].Value = ActualStartTime;

                objSqlParameter[2] = new SqlParameter();
                objSqlParameter[2].ParameterName = "@ActualEndDate";
                objSqlParameter[2].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[2].Value = ActualEndDate;

                objSqlParameter[3] = new SqlParameter();
                objSqlParameter[3].ParameterName = "@ActualEndTime";
                objSqlParameter[3].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[3].Value = ActualEndTime;

                objSqlParameter[4] = new SqlParameter();
                objSqlParameter[4].ParameterName = "@Component";
                objSqlParameter[4].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[4].Value = (keyValueBatch.key).Trim();

                objSqlParameter[5] = new SqlParameter();
                objSqlParameter[5].ParameterName = "@TagValue";
                objSqlParameter[5].SqlDbType = SqlDbType.Float;
                objSqlParameter[5].Value = keyValueBatch.value;

                objSqlParameter[6] = new SqlParameter();
                objSqlParameter[6].ParameterName = "@PONO";
                objSqlParameter[6].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[6].Value = PONO;

                objSqlParameter[7] = new SqlParameter();
                objSqlParameter[7].ParameterName = "@UserId";
                objSqlParameter[7].SqlDbType = SqlDbType.Int;
                objSqlParameter[7].Value = userId;

                objSqlParameter[8] = new SqlParameter();
                objSqlParameter[8].ParameterName = "@IDOCNumber";
                objSqlParameter[8].SqlDbType = SqlDbType.Int;
                objSqlParameter[8].Value = IDOCNumber;

                objSqlParameter[9] = new SqlParameter();
                objSqlParameter[9].ParameterName = "@PostingDate";
                objSqlParameter[9].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[9].Value = PostingDate;


                objSqlParameter[10] = new SqlParameter();
                objSqlParameter[10].ParameterName = "@ProductionDate";
                objSqlParameter[10].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[10].Value = ProductionDate;

                objSqlParameter[11] = new SqlParameter();
                objSqlParameter[11].ParameterName = "@Batch";
                objSqlParameter[11].SqlDbType = SqlDbType.VarChar;
                objSqlParameter[11].Value = keyValueBatch.BatchName;

                objSqlParameter[12] = new SqlParameter();
                objSqlParameter[12].ParameterName = "@IsManualWhileSubmission";
                objSqlParameter[12].SqlDbType = SqlDbType.Bit;
                objSqlParameter[12].Value = keyValueBatch.IsmanualSubmit;


                cmd = new SqlCommand("SavePODetails", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(objSqlParameter[0]);
                cmd.Parameters.Add(objSqlParameter[1]);
                cmd.Parameters.Add(objSqlParameter[2]);
                cmd.Parameters.Add(objSqlParameter[3]);
                cmd.Parameters.Add(objSqlParameter[4]);
                cmd.Parameters.Add(objSqlParameter[5]);
                cmd.Parameters.Add(objSqlParameter[6]);
                cmd.Parameters.Add(objSqlParameter[7]);
                cmd.Parameters.Add(objSqlParameter[8]);
                cmd.Parameters.Add(objSqlParameter[9]);
                cmd.Parameters.Add(objSqlParameter[10]);
                cmd.Parameters.Add(objSqlParameter[11]);
                cmd.Parameters.Add(objSqlParameter[12]);

                Convert.ToBoolean(cmd.ExecuteNonQuery());
            }

            connection.Close();
            return true;
        }


        public Boolean SaveChemicalManualEntry(string result, string PONO, int userId, int IDOCNumber)
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
                    KeyValueBatch keyValueBatch = new KeyValueBatch();
                    double actualQuantity1 = 0;
                    double concentrationQuantity = 0;
                    foreach (KeyValuePair<string, string> pair in bcd)
                    {
                        if (pair.Key == "key")
                        {
                            keyValueBatch.key = Convert.ToString(pair.Value);
                        }
                        if (pair.Key == "value")
                        {
                            string[] val = (pair.Value).Split('#');
                            actualQuantity1 = Convert.ToDouble(val[0]);
                            concentrationQuantity = Convert.ToDouble(val[1]);
                        }
                        if (pair.Key == "batch")
                        {
                            keyValueBatch.BatchName = Convert.ToString(pair.Value);
                        }
                    }

                    SqlParameter[] objSqlParameter = new SqlParameter[8];
                    objSqlParameter[0] = new SqlParameter();
                    objSqlParameter[0].ParameterName = "@ActualQuantity";
                    objSqlParameter[0].SqlDbType = SqlDbType.Float;
                    objSqlParameter[0].Value = actualQuantity1;

                    objSqlParameter[1] = new SqlParameter();
                    objSqlParameter[1].ParameterName = "@ConcentrationQuantity";
                    objSqlParameter[1].SqlDbType = SqlDbType.Float;
                    objSqlParameter[1].Value = concentrationQuantity;

                    objSqlParameter[2] = new SqlParameter();
                    objSqlParameter[2].ParameterName = "@PONO";
                    objSqlParameter[2].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[2].Value = PONO;

                    objSqlParameter[3] = new SqlParameter();
                    objSqlParameter[3].ParameterName = "@UserId";
                    objSqlParameter[3].SqlDbType = SqlDbType.Int;
                    objSqlParameter[3].Value = userId;

                    objSqlParameter[4] = new SqlParameter();
                    objSqlParameter[4].ParameterName = "@Component";
                    objSqlParameter[4].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[4].Value = (keyValueBatch.key).Trim();

                    objSqlParameter[5] = new SqlParameter();
                    objSqlParameter[5].ParameterName = "@Batch";
                    objSqlParameter[5].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[5].Value = keyValueBatch.BatchName;

                    objSqlParameter[6] = new SqlParameter();
                    objSqlParameter[6].ParameterName = "@IDOCNumber";
                    objSqlParameter[6].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[6].Value = IDOCNumber;

                    cmd = new SqlCommand("SaveConcentrationPODetails", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(objSqlParameter[0]);
                    cmd.Parameters.Add(objSqlParameter[1]);
                    cmd.Parameters.Add(objSqlParameter[2]);
                    cmd.Parameters.Add(objSqlParameter[3]);
                    cmd.Parameters.Add(objSqlParameter[4]);
                    cmd.Parameters.Add(objSqlParameter[5]);
                    cmd.Parameters.Add(objSqlParameter[6]);

                    Convert.ToBoolean(cmd.ExecuteNonQuery());
                }

                connection.Close();

                return true;
            }
            catch (Exception e)
            {
                return RevertOnExceptionWhilePOSubmission(PONO, IDOCNumber);
            }
        }

        public Boolean SubmitPODetails(string ActualStartDate, string ActualStartTime, string ActualEndDate, string ActualEndTime, string result, string PONO, int userId, int IDOCNumber, string PostingDate, string ProductionDate)
        {

            SqlCommand cmd;
            SqlConnection connection = null;

            int count = 1;
            try
            {
                connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

                connection.Open();


                List<Dictionary<string, string>> obj = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result);
                foreach (Dictionary<string, string> bcd in obj)
                {
                    KeyValue1 objKeyValue = new KeyValue1();
                    KeyValueBatch keyValueBatch = new KeyValueBatch();
                    foreach (KeyValuePair<string, string> pair in bcd)
                    {
                        if (pair.Key == "key")
                        {
                            string valueOfCOmponant = Convert.ToString(pair.Value);

                            if (valueOfCOmponant.Contains("_"))
                            {
                                string[] splittedData = valueOfCOmponant.Split('_');
                                string componentKey = splittedData[0];
                                string batchKey = splittedData[1];

                                keyValueBatch.key = componentKey;
                                keyValueBatch.BatchName = batchKey;

                            }
                            else
                            {
                                string componentKey = valueOfCOmponant;
                                string batchKey = "";

                                keyValueBatch.key = componentKey;
                                keyValueBatch.BatchName = batchKey;
                            }
                        }

                        if (pair.Key == "value")
                        {
                            keyValueBatch.value = Convert.ToDouble(pair.Value);
                        }

                        if (pair.Key == "IsmanualSubmit")
                        {
                            keyValueBatch.IsmanualSubmit = Convert.ToBoolean(pair.Value);
                        }
                    }

                    SqlParameter[] objSqlParameter = new SqlParameter[15];
                    objSqlParameter[0] = new SqlParameter();
                    objSqlParameter[0].ParameterName = "@ActualStartDate";
                    objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[0].Value = ActualStartDate;

                    objSqlParameter[1] = new SqlParameter();
                    objSqlParameter[1].ParameterName = "@ActualStartTime";
                    objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[1].Value = ActualStartTime;

                    objSqlParameter[2] = new SqlParameter();
                    objSqlParameter[2].ParameterName = "@ActualEndDate";
                    objSqlParameter[2].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[2].Value = ActualEndDate;

                    objSqlParameter[3] = new SqlParameter();
                    objSqlParameter[3].ParameterName = "@ActualEndTime";
                    objSqlParameter[3].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[3].Value = ActualEndTime;

                    objSqlParameter[4] = new SqlParameter();
                    objSqlParameter[4].ParameterName = "@Component";
                    objSqlParameter[4].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[4].Value = (keyValueBatch.key).Trim();

                    objSqlParameter[5] = new SqlParameter();
                    objSqlParameter[5].ParameterName = "@TagValue";
                    objSqlParameter[5].SqlDbType = SqlDbType.Float;
                    objSqlParameter[5].Value = keyValueBatch.value;

                    objSqlParameter[6] = new SqlParameter();
                    objSqlParameter[6].ParameterName = "@PONO";
                    objSqlParameter[6].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[6].Value = PONO;

                    objSqlParameter[7] = new SqlParameter();
                    objSqlParameter[7].ParameterName = "@UserId";
                    objSqlParameter[7].SqlDbType = SqlDbType.Int;
                    objSqlParameter[7].Value = userId;

                    objSqlParameter[8] = new SqlParameter();
                    objSqlParameter[8].ParameterName = "@IDOCNumber";
                    objSqlParameter[8].SqlDbType = SqlDbType.Int;
                    objSqlParameter[8].Value = IDOCNumber;

                    objSqlParameter[9] = new SqlParameter();
                    objSqlParameter[9].ParameterName = "@PostingDate";
                    objSqlParameter[9].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[9].Value = PostingDate;

                    objSqlParameter[10] = new SqlParameter();
                    objSqlParameter[10].ParameterName = "@ProductionDate";
                    objSqlParameter[10].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[10].Value = ProductionDate;

                    objSqlParameter[11] = new SqlParameter();
                    objSqlParameter[11].ParameterName = "@POCount";
                    objSqlParameter[11].SqlDbType = SqlDbType.Int;
                    objSqlParameter[11].Value = count;

                    objSqlParameter[12] = new SqlParameter();
                    objSqlParameter[12].ParameterName = "@Batch";
                    objSqlParameter[12].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[12].Value = keyValueBatch.BatchName;

                    objSqlParameter[13] = new SqlParameter();
                    objSqlParameter[13].ParameterName = "@IsManualWhileSubmission";
                    objSqlParameter[13].SqlDbType = SqlDbType.Bit;
                    objSqlParameter[13].Value = keyValueBatch.IsmanualSubmit;



                    cmd = new SqlCommand("SubmitPODetails", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(objSqlParameter[0]);
                    cmd.Parameters.Add(objSqlParameter[1]);
                    cmd.Parameters.Add(objSqlParameter[2]);
                    cmd.Parameters.Add(objSqlParameter[3]);
                    cmd.Parameters.Add(objSqlParameter[4]);
                    cmd.Parameters.Add(objSqlParameter[5]);
                    cmd.Parameters.Add(objSqlParameter[6]);
                    cmd.Parameters.Add(objSqlParameter[7]);
                    cmd.Parameters.Add(objSqlParameter[8]);
                    cmd.Parameters.Add(objSqlParameter[9]);
                    cmd.Parameters.Add(objSqlParameter[10]);
                    cmd.Parameters.Add(objSqlParameter[11]);
                    cmd.Parameters.Add(objSqlParameter[12]);
                    cmd.Parameters.Add(objSqlParameter[13]);

                    Convert.ToBoolean(cmd.ExecuteNonQuery());//connection, CommandType.StoredProcedure, "OES_ACA_SP_Get_1095CReportRun", objSqlParameter)

                    count++;
                }
                connection.Close();
                return true;
            }
            catch (Exception e)
            {
                connection.Close();
                return RevertOnExceptionWhilePOSubmission(PONO, IDOCNumber);
            }
        }

        public Boolean UpdateManualAutoStatus(string result, string PONO, int userId, int IDOCNumber)
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
                    KeyValueBatch keyValueBatch = new KeyValueBatch();
                    foreach (KeyValuePair<string, string> pair in bcd)
                    {
                        if (pair.Key == "key")
                        {
                            keyValueBatch.key = Convert.ToString(pair.Value);
                        }
                        if (pair.Key == "value")
                        {
                            keyValueBatch.IsmanualSubmit = Convert.ToBoolean(pair.Value);
                        }
                    }

                    SqlParameter[] objSqlParameter = new SqlParameter[8];


                    objSqlParameter[0] = new SqlParameter();
                    objSqlParameter[0].ParameterName = "@PONO";
                    objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[0].Value = PONO;

                    objSqlParameter[1] = new SqlParameter();
                    objSqlParameter[1].ParameterName = "@UserId";
                    objSqlParameter[1].SqlDbType = SqlDbType.Int;
                    objSqlParameter[1].Value = userId;

                    objSqlParameter[2] = new SqlParameter();
                    objSqlParameter[2].ParameterName = "@Component";
                    objSqlParameter[2].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[2].Value = (keyValueBatch.key).Trim();

                    objSqlParameter[3] = new SqlParameter();
                    objSqlParameter[3].ParameterName = "@IsManualWhileSubmission";
                    objSqlParameter[3].SqlDbType = SqlDbType.Bit;
                    objSqlParameter[3].Value = keyValueBatch.IsmanualSubmit;

                    objSqlParameter[4] = new SqlParameter();
                    objSqlParameter[4].ParameterName = "@IDOCNumber";
                    objSqlParameter[4].SqlDbType = SqlDbType.VarChar;
                    objSqlParameter[4].Value = IDOCNumber;

                    cmd = new SqlCommand("UpdateManualAutoStatus", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(objSqlParameter[0]);
                    cmd.Parameters.Add(objSqlParameter[1]);
                    cmd.Parameters.Add(objSqlParameter[2]);
                    cmd.Parameters.Add(objSqlParameter[3]);
                    cmd.Parameters.Add(objSqlParameter[4]);


                    Convert.ToBoolean(cmd.ExecuteNonQuery());
                }

                connection.Close();

                return true;
            }
            catch (Exception e)
            {
                return RevertOnExceptionWhilePOSubmission(PONO, IDOCNumber);
            }
        }

        public Boolean RevertOnExceptionWhilePOSubmission(string PONO, int IDOCNumber)
        {

            SqlCommand cmd;
            SqlConnection connection = null;

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

            cmd = new SqlCommand("RevertOnExceptionWhilePOSubmission", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);


            Convert.ToBoolean(cmd.ExecuteNonQuery());

            connection.Close();
            return false;
        }

        public string ValidateActualDatePO(string ActualStartDate, string ActualstartTime, string SubRefinery)
        {
            SqlConnection connection = null;
            SqlCommand cmd = null;
            SqlDataReader objSqlDataReader = null;


            connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlParameter[] objSqlParameter = new SqlParameter[3];
            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@ActualStartDate";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = ActualStartDate;

            objSqlParameter[1] = new SqlParameter();
            objSqlParameter[1].ParameterName = "@ActualStartTime";
            objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[1].Value = ActualstartTime;

            objSqlParameter[2] = new SqlParameter();
            objSqlParameter[2].ParameterName = "@SubRefinery";
            objSqlParameter[2].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[2].Value = SubRefinery;

            cmd = new SqlCommand("ValidateActualDatePO1", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);
            cmd.Parameters.Add(objSqlParameter[2]);

            objSqlDataReader = cmd.ExecuteReader();


            string checkexits = "";

            while (objSqlDataReader.Read())
            {
                checkexits = Convert.ToString(objSqlDataReader["IsExits"]).Trim();
            }
            objSqlDataReader.Close();
            connection.Close();
            return checkexits;
        }

        public Boolean InsertForUpTime(DateTime ActualStartDateTime, DateTime ActualEndDateTime, string SubRefinery, string SubRefineryName)
        {
            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            sqlConnection.Open();
            SqlDataReader objSqlDataReader = null;

            SqlParameter[] objSqlParameter = new SqlParameter[5];

            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@ActualStartTime";
            objSqlParameter[0].SqlDbType = SqlDbType.DateTime;
            objSqlParameter[0].Value = ActualStartDateTime;

            objSqlParameter[1] = new SqlParameter();
            objSqlParameter[1].ParameterName = "@ActualEndTime";
            objSqlParameter[1].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[1].Value = ActualEndDateTime;

            objSqlParameter[2] = new SqlParameter();
            objSqlParameter[2].ParameterName = "@SubRefinery";
            objSqlParameter[2].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[2].Value = SubRefinery;

            objSqlParameter[3] = new SqlParameter();
            objSqlParameter[3].ParameterName = "@Status";
            objSqlParameter[3].SqlDbType = SqlDbType.Bit;
            objSqlParameter[3].Value = false;

            objSqlParameter[4] = new SqlParameter();
            objSqlParameter[4].ParameterName = "@SubRefineryName";
            objSqlParameter[4].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[4].Value = SubRefineryName;

            cmd = new SqlCommand("UpdateStartEndTimeForUptime", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            cmd.Parameters.Add(objSqlParameter[1]);
            cmd.Parameters.Add(objSqlParameter[2]);
            cmd.Parameters.Add(objSqlParameter[3]);
            cmd.Parameters.Add(objSqlParameter[4]);
            Convert.ToBoolean(cmd.ExecuteNonQuery());

            objSqlDataReader.Close();
            sqlConnection.Close();

            return true;

        }

        public List<MasterRefineryModel> getPlantWiseSubRefineries(string SubRefinery)
        {
            SqlCommand cmd = null;
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            sqlConnection.Open();
            SqlDataReader objSqlDataReader = null;

            List<MasterRefineryModel> lstMasterRefinery = new List<MasterRefineryModel>();

            SqlParameter[] objSqlParameter = new SqlParameter[1];

            objSqlParameter[0] = new SqlParameter();
            objSqlParameter[0].ParameterName = "@SubRefinery";
            objSqlParameter[0].SqlDbType = SqlDbType.VarChar;
            objSqlParameter[0].Value = SubRefinery;

            cmd = new SqlCommand("GetPlantWiseSubRefineries", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(objSqlParameter[0]);
            objSqlDataReader = cmd.ExecuteReader();

            while (objSqlDataReader.Read())
            {
                MasterRefineryModel objRefinery = new MasterRefineryModel();
                objRefinery.RefineryId = Convert.ToInt32(objSqlDataReader["RefineryId"]);
                objRefinery.SubRefineryName = Convert.ToString(objSqlDataReader["SubRefineryName"]).Trim();
                objRefinery.SubRefineryCode = Convert.ToString(objSqlDataReader["SubRefineryCode"]).Trim();
                // objRefinery.CTTagValue = Convert.ToString(objSqlDataReader["CTTagValue"]).Trim();
                objRefinery.UpTimeTag = Convert.ToString(objSqlDataReader["UpTimeTag"]).Trim();
                objRefinery.InputTag = Convert.ToString(objSqlDataReader["InputTag"]).Trim();
                objRefinery.OutputTag = Convert.ToString(objSqlDataReader["OutputTag"]).Trim();
                objRefinery.UpTimePercentage = Convert.ToDouble(objSqlDataReader["UpTimePercentage"]);
                lstMasterRefinery.Add(objRefinery);
            }
            objSqlDataReader.Close();
            sqlConnection.Close();

            return lstMasterRefinery;
        }
    }
}