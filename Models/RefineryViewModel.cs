using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MvcApplication2.Models
{
    public class RefineryViewModel
    {
        public string RId { get; set; }
        public string RName{ get; set; }



        public List<SelectListItem> GetRefineryList()
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
                objRefineryViewModel.RId = Convert.ToString(objSqlDataReader["RefineryID"]);
                objRefineryViewModel.RName = Convert.ToString(objSqlDataReader["RefineryName"]);
                items.Add(new SelectListItem { Text = objRefineryViewModel.RName, Value = objRefineryViewModel.RId });
                objRefineryViewModel = null;
            }
            objSqlDataReader.Close();
            connection.Close();
            return items;
        
            }
        }
   
}