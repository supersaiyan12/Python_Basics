using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication2.Models;
using System.Dynamic;
using OSIsoft.AF;
using OSIsoft.AF.PI;
using System.Net;
using OSIsoft.AF.Time;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;

using System.Web.Script.Serialization;
using System.IO;
using System.Globalization;

namespace MvcApplication2.Controllers
{
    public class AdminController : Controller
    {

        // Get PO List From Database
        public ActionResult GetPOList()
        {
            var sessionObj = Session["SessionBO"] as UserModel;

            if (sessionObj != null)
            {
                BOMDetails objBOMDetails = new BOMDetails();
                // SP -> GetPOList - Get BOM List
                objBOMDetails.GetBOMDetailList(sessionObj.id);
                GenericTextValue objGenericTextValue = new GenericTextValue();
                ViewBag.lstRefinery = objGenericTextValue.GetSubRefineryList(sessionObj.id);
                ViewBag.IsAuthorized = sessionObj.isAuthorized;
                ViewBag.LoggedInUser = sessionObj.firstName;
                // Control go to MaterialList.cshtml page
                return View("../Admin/MaterialList", objBOMDetails);
            }
            else
            {
                return RedirectToAction("LoginUser", "Login");
            }

        }

        // Get Submitted PO List
        public ActionResult GetSubmitedPOList()
        {
            var sessionObj = Session["SessionBO"] as UserModel;



            if (sessionObj != null)
            {
                BOMDetails objBOMDetails = new BOMDetails();
                // SP -> GetSubmitedPOList 
                objBOMDetails.GetSubmitedPOList(sessionObj.id);
                ViewBag.IsAuthorized = sessionObj.isAuthorized;
                ViewBag.LoggedInUser = sessionObj.firstName;
                // Control go to SubmitedPOList.cshtml page
                return View("../Admin/SubmitedPOList", objBOMDetails);
            }
            else
            {
                return RedirectToAction("LoginUser", "Login");
            }

        }

        // Submitted PO List -> on Preview 
        public ActionResult LoadProcessOrder(string Name)
        {

            var sessionObj = Session["SessionBO"] as UserModel;
            if (sessionObj != null)
            {
                BOMDetails objBOMDetails = new BOMDetails();
                string[] split = Name.Split(',');
                string PONumber = split[0];
                int IDOCNumber = Convert.ToInt32(split[1]);
                objBOMDetails = objBOMDetails.getProcessOrder(PONumber, IDOCNumber);
                ViewBag.IsAuthorized = sessionObj.isAuthorized;
                ViewBag.LoggedInUser = sessionObj.firstName;// +" " + sessionObj.LastName;
                return View("../Admin/ProcessOrder", objBOMDetails);
            }
            else
            {
                return RedirectToAction("LoginUser", "Login");
            }

        }

        public JsonResult LoadGraph(string PlantStartDateTime, string PlantEndDateTime, string SubRefinery)
        {

            var sessionObj = Session["SessionBO"] as UserModel;
            if (sessionObj != null)
            {
                string PlantStartDateTime1 = PlantStartDateTime;
                string PlantEndDateTime1 = PlantEndDateTime;

                BOMDetails objBOMDetails = new BOMDetails();

                AFLocaleIndependentFormatProvider myTimeZone = new AFLocaleIndependentFormatProvider();
                //  DateTime StartDate = Convert.ToDateTime(PlantStartDateTime1);
                //  DateTime EndDate = Convert.ToDateTime(PlantEndDateTime1);


                DateTime StartDate = Convert.ToDateTime(PlantStartDateTime1, System.Globalization.CultureInfo.CurrentCulture);
                DateTime EndDate = Convert.ToDateTime(PlantEndDateTime1, System.Globalization.CultureInfo.CurrentCulture);
                PIPoint feedInTag;
                PIPoint feedOutTag;
                AFValues feedInVal;
                AFValues feedOutVal;
                AFValues filteredfeedInVal = new AFValues();
                AFValues filteredoutInVal = new AFValues();

                try
                {
                    PIServerDetails piServerDetails = new PIServerDetails();
                    PISystems myPISystems = new PISystems();
                    PISystem mypiSystem = myPISystems[piServerDetails.PIServerName];
                    PIServer myPiServer = PIServer.FindPIServer(mypiSystem, piServerDetails.PIServerName);
                    NetworkCredential Credentials = new NetworkCredential(piServerDetails.UserName, piServerDetails.Password);
                    mypiSystem.Connect(Credentials);


                    StartDate = StartDate.AddMinutes(-330);
                    AFTime sAFTime = new AFTime(StartDate);
                    //DateTime endDT = Convert.ToDateTime(szDTend);
                    EndDate = EndDate.AddMinutes(-330);
                    AFTime eAFTime = new AFTime(EndDate);
                    AFTimeRange GraphTimeRange = new AFTimeRange(sAFTime, eAFTime);
                    //   mypiSystem.Connect(Credentials);

                    objBOMDetails = objBOMDetails.getFeedInFeedOutTag(SubRefinery);




                    feedInTag = PIPoint.FindPIPoint(myPiServer, objBOMDetails.feedInTag);
                    feedInVal = feedInTag.RecordedValues(GraphTimeRange, 0, null, true, 0);

                    foreach (AFValue val in feedInVal)
                    {
                        if (val.IsGood)
                        {
                            filteredfeedInVal.Add(val);
                        }
                        else
                        {
                            val.Value = 0;
                            filteredfeedInVal.Add(val);
                        }
                    }

                    feedOutTag = PIPoint.FindPIPoint(myPiServer, objBOMDetails.feedOutTag);
                    feedOutVal = feedOutTag.RecordedValues(GraphTimeRange, 0, null, true, 0);

                    foreach (AFValue val in feedOutVal)
                    {
                        if (val.IsGood)
                        {
                            filteredoutInVal.Add(val);
                        }
                        else
                        {
                            val.Value = 0;
                            filteredoutInVal.Add(val);
                        }
                    }

                    int inLenArray = filteredfeedInVal.Count;
                    object[] feedInValArr = new object[inLenArray];
                    DateTime[] feedInDateArr = new DateTime[inLenArray];
                    AFValueStatus[] feedInValStatus = new AFValueStatus[inLenArray];
                    filteredfeedInVal.GetValueArrays(out  feedInValArr, out  feedInDateArr, out  feedInValStatus);
                    int outLenArray = feedOutVal.Count;
                    object[] feedOutValArr = new object[outLenArray];

                    DateTime[] feedOutDateArr = new DateTime[outLenArray];
                    AFValueStatus[] feedOutValStatus = new AFValueStatus[outLenArray];
                    filteredoutInVal.GetValueArrays(out  feedOutValArr, out  feedOutDateArr, out  feedOutValStatus);
                    List<KeyValuePair<string, double>> fInlist = new List<KeyValuePair<string, double>>();
                    List<KeyValuePair<string, double>> fOutlist = new List<KeyValuePair<string, double>>();
                    //  double[] fInDateValdouble = new double[inLenArray];
                    double[] fOutDateVal = new double[outLenArray];
                    double[] fInDateVal = new double[inLenArray];
                    for (int i = 0; i < feedInValArr.Length; i++)
                    {

                        //string fIn = feedInValArr[i].ToString();
                        double fint = Convert.ToInt32(feedInValArr[i]);
                        if (fint == 0.0)
                        {
                            fInDateVal[i] = -99999;
                        }
                        else if (fint < 0.0)
                        {
                            fInDateVal[i] = -99999;
                        }
                        else
                        {
                            fInDateVal[i] = Math.Round(Convert.ToDouble(feedInValArr[i]), 2);
                        }
                    }
                    string[] fInDateArr = new string[inLenArray];
                    for (int i = 0; i < feedInDateArr.Length; i++)
                    {
                        fInDateArr[i] = feedInDateArr[i].ToString("MM-dd-yyyy HH:mm:ss");
                        var element = new KeyValuePair<string, double>(fInDateArr[i], fInDateVal[i]);
                        fInlist.Add(element);
                    }

                    string[] fOutDateArr = new string[outLenArray];

                    for (int i = 0; i < feedOutDateArr.Length; i++)   //feedOutDateArr
                    {
                        //string fOut = feedOutValArr[i].ToString();    //fOutDateArr
                        double fout = Convert.ToInt32(feedOutValArr[i]);
                        if (fout == 0.0)
                        {
                            fOutDateVal[i] = -99999;
                        }
                        else if (fout < 0.0)
                        {
                            fOutDateVal[i] = -99999;
                        }
                        else
                        {
                            fOutDateVal[i] = Math.Round(Convert.ToDouble(feedOutValArr[i]), 2);
                        }
                    }
                    // string[] fOutDateArr = new string[inLenArray];
                    for (int i = 0; i < feedOutDateArr.Length; i++)
                    {
                        fOutDateArr[i] = feedOutDateArr[i].ToString("MM-dd-yyyy HH:mm:ss");
                        var element1 = new KeyValuePair<string, double>(fOutDateArr[i], fOutDateVal[i]);
                        fOutlist.Add(element1);
                    }


                    Dictionary<string, List<KeyValuePair<string, double>>> dec = new Dictionary<string, List<KeyValuePair<string, double>>>();

                    dec.Add("feedIn", fInlist);
                    dec.Add("feedOut", fOutlist);



                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    // string json = javaScriptSerializer.Serialize(feedInDateArr);

                    JsonResult js = Json(new { value = dec }, JsonRequestBehavior.AllowGet);

                    return js;

                    // return Json(new { value = dec }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { value = "Exception Occured" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult dosearch(FormCollection form)
        {

            var sessionObj = Session["SessionBO"] as UserModel;

            if (sessionObj != null)
            {
                string selected_start = form["selected_start"];
                string selected_end = form["selected_end"];

                string[] data = selected_end.Split('-');
                string new_start = data[1] + '-' + data[0] + '-' + data[2];

                string[] data1 = selected_start.Split('-');
                string new_start1 = data1[1] + '-' + data1[0] + '-' + data1[2];


                DateTime selected_start1 = Convert.ToDateTime(new_start1, System.Globalization.CultureInfo.CurrentCulture);

                DateTime selected_end1 = Convert.ToDateTime(new_start, System.Globalization.CultureInfo.CurrentCulture);

                BOMDetails objBOMDetails = new BOMDetails();
                objBOMDetails.dosearch(selected_start1, selected_end1, sessionObj.id);
                GenericTextValue objGenericTextValue = new GenericTextValue();
                ViewBag.lstRefinery = objGenericTextValue.GetSubRefineryList(sessionObj.id);
                ViewBag.IsAuthorized = sessionObj.isAuthorized;
                ViewBag.LoggedInUser = sessionObj.firstName;
                return View("../Admin/MaterialList", objBOMDetails);
            }
            else
            {
                return RedirectToAction("LoginUser", "Login");
            }


        }

        [HttpPost]
        public ActionResult searchSubmitedPO(FormCollection form)
        {


            var sessionObj = Session["SessionBO"] as UserModel;

            if (sessionObj != null)
            {
                if (form == null)
                {
                    return RedirectToAction("GetSubmitedPOList", "Admin");
                }

                string selected_start = form["selected_start"];
                string selected_end = form["selected_end"];

                string[] data = selected_end.Split('-');
                string new_start = data[1] + '-' + data[0] + '-' + data[2];

                string[] data1 = selected_start.Split('-');
                string new_start1 = data1[1] + '-' + data1[0] + '-' + data1[2];


                DateTime selected_start1 = Convert.ToDateTime(new_start1, System.Globalization.CultureInfo.CurrentCulture);

                DateTime selected_end1 = Convert.ToDateTime(new_start, System.Globalization.CultureInfo.CurrentCulture);

                BOMDetails objBOMDetails = new BOMDetails();
                objBOMDetails.searchSubmitedPO(selected_start1, selected_end1, sessionObj.id);
                GenericTextValue objGenericTextValue = new GenericTextValue();
                ViewBag.lstRefinery = objGenericTextValue.GetSubRefineryList(sessionObj.id);
                ViewBag.IsAuthorized = sessionObj.isAuthorized;
                ViewBag.LoggedInUser = sessionObj.firstName;
                return View("../Admin/SubmitedPOList", objBOMDetails);
            }
            else
            {
                return RedirectToAction("LoginUser", "Login");
            }


        }

        public JsonResult CalculateBill(string ActualStartDateTime, string ActualEndDateTime, string SubRefinery, string isSave_status, string PONO, int IDOCNumber)
        {
            var sessionObj = Session["SessionBO"] as UserModel;

            if (sessionObj != null)
            {
                BOMMasterTag objBOMMasterTag = new BOMMasterTag();
                AFLocaleIndependentFormatProvider myTimeZone = new AFLocaleIndependentFormatProvider();

                DateTime ActualStartDate = Convert.ToDateTime(ActualStartDateTime);
                DateTime ActualEndDate = Convert.ToDateTime(ActualEndDateTime);

                AFValue strtVal;
                AFValue endVal;
                PIPoint strtTag;
                string exceptionMsg = "";

                try
                {
                    List<BOMMasterTag> lstBOMMasterTag = new List<BOMMasterTag>();
                    if (isSave_status == "1")
                    {

                        lstBOMMasterTag = objBOMMasterTag.getIsSavedList(PONO, IDOCNumber);
                        List<string> lslComponent = GetIsManualWithTag(ActualStartDateTime, ActualEndDateTime, SubRefinery, isSave_status, PONO, IDOCNumber);
                        foreach (BOMMasterTag objBOMMasterTag1 in lstBOMMasterTag)
                        {
                            foreach (String component in lslComponent)
                            {
                                if (objBOMMasterTag1.component == component)
                                {
                                    objBOMMasterTag1.isManual = true;
                                    objBOMMasterTag1.isManualWithTag = true;
                                }
                            }
                        }
                        //List<string> lstComponent1 = new List<string>();
                        //List<string> lstComponent2 = new List<string>();

                        //lstBOMMasterTag = objBOMMasterTag.getIsSavedList(PONO, IDOCNumber);
                        //List<string> lslComponent = GetIsManualWithTag(ActualStartDateTime, ActualEndDateTime, SubRefinery, isSave_status, PONO, IDOCNumber);

                        //foreach (BOMMasterTag objBOMMasterTag1 in lstBOMMasterTag)
                        //{

                        //    foreach (String component in lslComponent) {
                        //     if (objBOMMasterTag1.component == component)
                        //    {
                        //        objBOMMasterTag1.isManual = true;
                        //        objBOMMasterTag1.isManualWithTag = true;
                        //    }
                        //    }
                        //    lstComponent1.Add(objBOMMasterTag1.component);
                        //    lstComponent2.Add(objBOMMasterTag1.BOMCategory);
                        //}                   


                        //string duplicate = "";
                        //List<string> NonDuplicateList = new List<string>();
                        //foreach (string s in lstComponent2)
                        //{
                        //    if (!NonDuplicateList.Contains(s))
                        //        NonDuplicateList.Add(s);
                        //    else
                        //        duplicate = s;

                        //}

                        //foreach (BOMMasterTag objBOMMasterTag1 in lstBOMMasterTag)
                        //{
                        //    if (duplicate != "")
                        //    {
                        //        if (objBOMMasterTag1.BOMCategory == "Raw-Material" || objBOMMasterTag1.BOMCategory == "Product")
                        //        {
                        //            if (duplicate == objBOMMasterTag1.BOMCategory)
                        //            {
                        //               // lstBOMMasterTag.Remove(objBOMMasterTag1);
                        //                objBOMMasterTag1.BOMCategory = "Product";
                        //            }
                        //        }
                        //    }
                        //}




                    }
                    else
                    {
                        PIServerDetails piServerDetails = new PIServerDetails();
                        PISystems myPISystems = new PISystems();
                        PISystem mypiSystem = myPISystems[piServerDetails.PIServerName];
                        PIServer myPiServer = PIServer.FindPIServer(mypiSystem, piServerDetails.PIServerName);
                        NetworkCredential Credentials = new NetworkCredential(piServerDetails.UserName, piServerDetails.Password);

                        ActualStartDate = ActualStartDate.AddMinutes(-330);
                        AFTime sAFTime = new AFTime(ActualStartDate);
                        ActualEndDate = ActualEndDate.AddMinutes(-330);
                        AFTime eAFTime = new AFTime(ActualEndDate);
                        mypiSystem.Connect(Credentials);

                        //Tag Mapping with BOMMaster.
                        List<BOMMasterTag> lstBOMMasterTag1 = objBOMMasterTag.getMasterBOM(SubRefinery, PONO, IDOCNumber);

                        IDictionary<AFSummaryTypes, AFValue> sm = null;
                        IDictionary<AFSummaryTypes, AFValue> smin = null;
                        List<string> lstComponent = new List<string>();

                        foreach (BOMMasterTag objBOMMasterTag1 in lstBOMMasterTag1)
                        {
                            lstComponent.Add(objBOMMasterTag1.component);
                        }

                        //string duplicate = "";
                        //List<string> NonDuplicateList = new List<string>();
                        //foreach (string s in lstComponent)
                        //{
                        //    if (!NonDuplicateList.Contains(s))
                        //        NonDuplicateList.Add(s);
                        //    else
                        //        duplicate = s;

                        //}
                        foreach (BOMMasterTag objBOMMasterTag1 in lstBOMMasterTag1)
                        {
                            if (objBOMMasterTag1.isManual || objBOMMasterTag1.PITag == "")
                            {
                                //objBOMMasterTag1.quantity = 00;
                                objBOMMasterTag1.isManual = true;

                                if (objBOMMasterTag1.component == "")
                                {

                                }
                            }
                            else
                            {
                                double resultValue = 0;
                                strtTag = PIPoint.FindPIPoint(myPiServer, objBOMMasterTag1.PITag);
                                strtVal = strtTag.RecordedValue(sAFTime, AFRetrievalMode.AtOrAfter);
                                endVal = strtTag.RecordedValue(eAFTime, AFRetrievalMode.AtOrAfter);

                                AFTimeRange graphTimeReange = new AFTimeRange(sAFTime, eAFTime);

                                sm = strtTag.Summary(graphTimeReange, AFSummaryTypes.Maximum, 0, 0);
                                smin = strtTag.Summary(graphTimeReange, AFSummaryTypes.Minimum, 0, 0);
                                AFValue mx = sm[AFSummaryTypes.Maximum];
                                AFValue mn = smin[AFSummaryTypes.Minimum];
                                Boolean isWrongValue = false;

                                if (endVal.Value.ToString().Equals("No Data") || mx.IsGood == false || strtVal.Value.ToString().Equals("No Data") || mx.IsGood == false)
                                {
                                    resultValue = 0.0;
                                    isWrongValue = true;
                                }
                                else
                                {
                                    if (Convert.ToDouble(endVal.Value) < Convert.ToDouble(mx.Value))
                                    {
                                        resultValue = Convert.ToDouble(mx.Value) - Convert.ToDouble(strtVal.Value) + Convert.ToDouble(endVal.Value) - Convert.ToDouble(mn.Value);
                                    }
                                    else if (Convert.ToDouble(endVal.Value) == Convert.ToDouble(strtVal.Value))
                                    {
                                        resultValue = 0;
                                        isWrongValue = true;
                                    }
                                    else
                                    {
                                        resultValue = Convert.ToDouble(endVal.Value) - Convert.ToDouble(strtVal.Value);
                                    }
                                }

                                //if (objBOMMasterTag1.component == "power")
                                //{
                                //    BOMDetails bomDetails = new BOMDetails();
                                //    List<MasterRefineryModel> lstMasterRefinery = bomDetails.getPlantWiseSubRefineries(SubRefinery);

                                //    double upTimeHours = 0;
                                //    double totalWeightIntoUptime = 0;
                                //    double weightIntoUptime = 0;
                                //    double weightIntoUptime1 = 0;
                                //    double effectiveWeight = 0;

                                //    foreach (MasterRefineryModel RefineryDetails in lstMasterRefinery)
                                //    {
                                //        Boolean flag = bomDetails.InsertForUpTime(ActualStartDate, ActualEndDate, RefineryDetails.SubRefineryCode, RefineryDetails.SubRefineryName);
                                //        if (flag)
                                //        {

                                //            strtTag = PIPoint.FindPIPoint(myPiServer, RefineryDetails.UpTimeTag);
                                //            strtVal = strtTag.RecordedValue(sAFTime, AFRetrievalMode.AtOrAfter);
                                //            endVal = strtTag.RecordedValue(eAFTime, AFRetrievalMode.AtOrAfter);

                                //            upTimeHours = Convert.ToDouble(endVal.Value);
                                //            weightIntoUptime = (upTimeHours * RefineryDetails.UpTimePercentage);
                                //            totalWeightIntoUptime = totalWeightIntoUptime + weightIntoUptime;
                                //            if (objBOMMasterTag1.SubRefinery == RefineryDetails.SubRefineryCode)
                                //            {
                                //                weightIntoUptime1 = weightIntoUptime;
                                //            }
                                //        }
                                //    }
                                //    effectiveWeight = weightIntoUptime1 / totalWeightIntoUptime;
                                //    resultValue = resultValue * effectiveWeight;
                                //}

                                if (resultValue <= 0 || isWrongValue)
                                {
                                    objBOMMasterTag1.isManual = true;
                                    objBOMMasterTag1.isManualWithTag = true;
                                }
                                objBOMMasterTag1.quantity = (float)resultValue;
                            }
                            lstBOMMasterTag.Add(objBOMMasterTag1);
                            //if (duplicate != "")
                            //{
                            //    if (objBOMMasterTag1.BOMCategory == "Raw-Material" || objBOMMasterTag1.BOMCategory == "Product")
                            //    {
                            //        if (duplicate == objBOMMasterTag1.component)
                            //        {
                            //            lstBOMMasterTag.Remove(objBOMMasterTag1);
                            //        }
                            //    }
                            //}
                        }
                    }

                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string json = javaScriptSerializer.Serialize(lstBOMMasterTag);
                    JsonResult js = Json(new { value = json }, JsonRequestBehavior.AllowGet);
                    return js;

                }
                catch (Exception e)
                {
                    return Json(new { value = exceptionMsg }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SavePODetails(string ActualStartDate, string ActualStartTime, string ActualEndDate, string ActualEndTime, string roles, string data, string ManualEntryResult, string PONO, int IDOCNumber, string PostingDate, string ProductionDate)
        {
            var sessionObj = Session["SessionBO"] as UserModel;

            if (sessionObj != null)
            {
                BOMDetails objBOMDetails = new BOMDetails();

                bool result = objBOMDetails.SavePODetails(ActualStartDate, ActualStartTime, ActualEndDate, ActualEndTime, roles, PONO, sessionObj.id, IDOCNumber, PostingDate, ProductionDate);

                Characteristic objCharacteristic = new Characteristic();

                if (result)
                {
                    bool manualResult = objBOMDetails.SaveChemicalManualEntry(ManualEntryResult, PONO, sessionObj.id, IDOCNumber);
                    bool charResult = objCharacteristic.SaveCharacteristics(data, PONO, sessionObj.id, IDOCNumber);
                }

                return Json(new { value = "Successfully Saved!" }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SubmtPODetails(string ActualStartDate, string ActualStartTime, string ActualEndDate, string ActualEndTime, string roles, string data, string ManualEntryResult, string PONO, int IDOCNumber, string PostingDate, string ProductionDate)
        {
            var sessionObj = Session["SessionBO"] as UserModel;

            if (sessionObj != null)
            {
                BOMDetails objBOMDetails = new BOMDetails();
                Characteristic objCharacteristic = new Characteristic();

                bool charResult = objCharacteristic.SaveCharacteristics(data, PONO, sessionObj.id, IDOCNumber);
                bool manualResult = objBOMDetails.SaveChemicalManualEntry(ManualEntryResult, PONO, sessionObj.id, IDOCNumber);
                bool result = objBOMDetails.SubmitPODetails(ActualStartDate, ActualStartTime, ActualEndDate, ActualEndTime, roles, PONO, sessionObj.id, IDOCNumber, PostingDate, ProductionDate);

                if (manualResult && result)
                {
                    return Json(new { value = "Successfully Submitted!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        // Process Order List -> on Load 
        public ActionResult LoadProcessOrderPreview(string Name)
        {
            var sessionObj = Session["SessionBO"] as UserModel;
            if (sessionObj != null)
            {
                BOMDetails objBOMDetails = new BOMDetails();
                string[] split = Name.Split(',');
                string PONumber = split[0];
                int IDOCNumber = Convert.ToInt32(split[1]);
                objBOMDetails = objBOMDetails.getProcessOrder(PONumber, IDOCNumber);

                ViewBag.IsAuthorized = sessionObj.isAuthorized;
                ViewBag.LoggedInUser = sessionObj.firstName;// +" " + sessionObj.LastName;
                return View("../Admin/ProcessOrderPreview", objBOMDetails);
            }
            else
            {
                return RedirectToAction("LoginUser", "Login");
            }

        }

        public JsonResult ValidateActualDatePO(string ActualStartDate, string ActualstartTime, string SubRefinery)
        {
            var sessionObj = Session["SessionBO"] as UserModel;
            if (sessionObj != null)
            {

                BOMDetails objBOMDetails = new BOMDetails();

                try
                {
                    string result = objBOMDetails.ValidateActualDatePO(ActualStartDate, ActualstartTime, SubRefinery);


                    return Json(new { value = result }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception e)
                {
                    return Json(new { value = "Exception Occured" }, JsonRequestBehavior.AllowGet);
                }


            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult CalculateCharacteristic(string PONO, string SaveStatus)
        {
            var sessionObj = Session["SessionBO"] as UserModel;
            if (sessionObj != null)
            {

                List<Characteristic> lstCharacteristic = new List<Characteristic>();
                Characteristic objCharacteristic = new Characteristic();
                try
                {
                    if (SaveStatus == "1")
                    {
                        lstCharacteristic = objCharacteristic.SavedCalculateCharacteristic(PONO);
                    }
                    else
                    {
                        lstCharacteristic = objCharacteristic.CalculateCharacteristic(PONO);
                    }

                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string json = javaScriptSerializer.Serialize(lstCharacteristic);
                    JsonResult js = Json(new { value = json }, JsonRequestBehavior.AllowGet);

                    return js;

                }
                catch (Exception e)
                {
                    return Json(new { value = "Exception Occured" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public List<string> GetIsManualWithTag(string ActualStartDateTime, string ActualEndDateTime, string SubRefinery, string isSave_status, string PONO, int IDOCNumber)
        {
            DateTime ActualStartDate = Convert.ToDateTime(ActualStartDateTime);
            DateTime ActualEndDate = Convert.ToDateTime(ActualEndDateTime);
            BOMMasterTag objBOMMasterTag = new BOMMasterTag();
            AFValue strtVal;
            AFValue endVal;
            PIPoint strtTag;
            string exceptionMsg = "";
            List<string> lstString = new List<string>();
            try
            {
                PIServerDetails piServerDetails = new PIServerDetails();
                PISystems myPISystems = new PISystems();
                PISystem mypiSystem = myPISystems[piServerDetails.PIServerName];
                PIServer myPiServer = PIServer.FindPIServer(mypiSystem, piServerDetails.PIServerName);
                NetworkCredential Credentials = new NetworkCredential(piServerDetails.UserName, piServerDetails.Password);

                ActualStartDate = ActualStartDate.AddMinutes(-330);
                AFTime sAFTime = new AFTime(ActualStartDate);
                ActualEndDate = ActualEndDate.AddMinutes(-330);
                AFTime eAFTime = new AFTime(ActualEndDate);
                mypiSystem.Connect(Credentials);

                //Tag Mapping with BOMMaster.
                List<BOMMasterTag> lstBOMMasterTag1 = objBOMMasterTag.getMasterBOM(SubRefinery, PONO, IDOCNumber);

                IDictionary<AFSummaryTypes, AFValue> sm = null;
                IDictionary<AFSummaryTypes, AFValue> smin = null;

                foreach (BOMMasterTag objBOMMasterTag1 in lstBOMMasterTag1)
                {

                    if (objBOMMasterTag1.isManual || objBOMMasterTag1.PITag == "")
                    {

                        objBOMMasterTag1.quantity = 00;
                        //objBOMMasterTag1.isManual = true;
                        //lstBOMMasterTag.Add(objBOMMasterTag1);
                    }
                    else
                    {
                        double resultValue = 0;
                        strtTag = PIPoint.FindPIPoint(myPiServer, objBOMMasterTag1.PITag);
                        strtVal = strtTag.RecordedValue(sAFTime, AFRetrievalMode.AtOrAfter);
                        endVal = strtTag.RecordedValue(eAFTime, AFRetrievalMode.AtOrAfter);

                        AFTimeRange graphTimeReange = new AFTimeRange(sAFTime, eAFTime);

                        sm = strtTag.Summary(graphTimeReange, AFSummaryTypes.Maximum, 0, 0);
                        smin = strtTag.Summary(graphTimeReange, AFSummaryTypes.Minimum, 0, 0);
                        AFValue mx = sm[AFSummaryTypes.Maximum];
                        AFValue mn = smin[AFSummaryTypes.Minimum];
                        string strtValString = strtVal.Value.ToString();
                        string endValString = endVal.Value.ToString();

                        if (endValString.Equals("No Data") || mx.IsGood == false || strtValString.Equals("No Data") || mx.IsGood == false)
                        {
                            resultValue = 00000;
                            objBOMMasterTag1.isManual = true;
                            objBOMMasterTag1.isManualWithTag = true;
                            lstString.Add(objBOMMasterTag1.component);
                        }
                        else
                        {
                            if (Convert.ToDouble(endVal.Value) < Convert.ToDouble(mx.Value))
                            {
                                try
                                {
                                    resultValue = Convert.ToDouble(mx.Value) - Convert.ToDouble(strtVal.Value) + Convert.ToDouble(endVal.Value) - Convert.ToDouble(mn.Value);
                                }
                                catch (Exception e)
                                {
                                    objBOMMasterTag1.isManual = true;
                                    objBOMMasterTag1.isManualWithTag = true;
                                    exceptionMsg = "Bad value from PI Tag";
                                    lstString.Add(objBOMMasterTag1.component);
                                }
                            }
                            else if (Convert.ToDouble(endVal.Value) == Convert.ToDouble(strtVal.Value))
                            {
                                resultValue = 00000;
                                objBOMMasterTag1.isManual = true;
                                objBOMMasterTag1.isManualWithTag = true;
                                lstString.Add(objBOMMasterTag1.component);
                            }
                            else
                            {
                                resultValue = Convert.ToDouble(endVal.Value) - Convert.ToDouble(strtVal.Value);
                            }
                            objBOMMasterTag1.quantity = (float)resultValue;
                        }
                    }


                }
                return lstString;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public BOMMasterTag CheckIsManualBasedOnValue(string ActualStartDateTime, string ActualEndDateTime, BOMMasterTag objBOMMasterTag1)
        {
            DateTime ActualStartDate = Convert.ToDateTime(ActualStartDateTime);
            DateTime ActualEndDate = Convert.ToDateTime(ActualEndDateTime);
            BOMMasterTag objBOMMasterTag = new BOMMasterTag();
            AFValue strtVal;
            AFValue endVal;
            PIPoint strtTag;
            string exceptionMsg = "";
            List<string> lstString = new List<string>();
            try
            {
                PIServerDetails piServerDetails = new PIServerDetails();
                PISystems myPISystems = new PISystems();
                PISystem mypiSystem = myPISystems[piServerDetails.PIServerName];
                PIServer myPiServer = PIServer.FindPIServer(mypiSystem, piServerDetails.PIServerName);
                NetworkCredential Credentials = new NetworkCredential(piServerDetails.UserName, piServerDetails.Password);

                ActualStartDate = ActualStartDate.AddMinutes(-330);
                AFTime sAFTime = new AFTime(ActualStartDate);
                ActualEndDate = ActualEndDate.AddMinutes(-330);
                AFTime eAFTime = new AFTime(ActualEndDate);
                mypiSystem.Connect(Credentials);
                IDictionary<AFSummaryTypes, AFValue> sm = null;
                IDictionary<AFSummaryTypes, AFValue> smin = null;
                List<string> lstComponent = new List<string>();

                double resultValue = 0;
                strtTag = PIPoint.FindPIPoint(myPiServer, objBOMMasterTag1.PITag);
                strtVal = strtTag.RecordedValue(sAFTime, AFRetrievalMode.AtOrAfter);
                endVal = strtTag.RecordedValue(eAFTime, AFRetrievalMode.AtOrAfter);

                AFTimeRange graphTimeReange = new AFTimeRange(sAFTime, eAFTime);

                sm = strtTag.Summary(graphTimeReange, AFSummaryTypes.Maximum, 0, 0);
                smin = strtTag.Summary(graphTimeReange, AFSummaryTypes.Minimum, 0, 0);
                AFValue mx = sm[AFSummaryTypes.Maximum];
                AFValue mn = smin[AFSummaryTypes.Minimum];
                string strtValString = strtVal.Value.ToString();
                string endValString = endVal.Value.ToString();

                if (endValString.Equals("No Data") || mx.IsGood == false || strtValString.Equals("No Data") || mx.IsGood == false)
                {
                    resultValue = 00000;
                    objBOMMasterTag1.isManual = true;
                    objBOMMasterTag1.isManualWithTag = true;
                    lstString.Add(objBOMMasterTag1.component);
                }
                else
                {
                    if (Convert.ToDouble(endVal.Value) < Convert.ToDouble(mx.Value))
                    {
                        try
                        {
                            resultValue = Convert.ToDouble(mx.Value) - Convert.ToDouble(strtVal.Value) + Convert.ToDouble(endVal.Value) - Convert.ToDouble(mn.Value);
                        }
                        catch (Exception e)
                        {
                            objBOMMasterTag1.isManual = true;
                            objBOMMasterTag1.isManualWithTag = true;
                            exceptionMsg = "Bad value from PI Tag";
                            lstString.Add(objBOMMasterTag1.component);
                        }
                    }
                    else if (Convert.ToDouble(endVal.Value) == Convert.ToDouble(strtVal.Value))
                    {
                        resultValue = 00000;
                        objBOMMasterTag1.isManual = true;
                        objBOMMasterTag1.isManualWithTag = true;
                        lstString.Add(objBOMMasterTag1.component);
                    }
                    else
                    {
                        resultValue = Convert.ToDouble(endVal.Value) - Convert.ToDouble(strtVal.Value);
                    }
                    // objBOMMasterTag1.quantity = (float)resultValue;
                }

                return objBOMMasterTag1;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        // User Details --> Manage Users
        #region User

        // Register a new User
        public ActionResult RegisterUser(UserModel objUserModel)
        {
            objUserModel = new UserModel();
            //     RefineryViewModel objRefineryViewModel = new RefineryViewModel();
            objUserModel = objUserModel.GetUserDetails();
            objUserModel.lstrefinery = UserModel.PopulateRefinery();
            ViewBag.IsAuthorized = false;
            var sessionObj = Session["SessionBO"] as UserModel;
            if (sessionObj != null)
            {
                ViewBag.IsAuthorized = sessionObj.isAuthorized;
                ViewBag.LoggedInUser = sessionObj.firstName;
            }
            else
            {
                return RedirectToAction("LoginUser", "Login");
            }
            return View("../admin/RegisterUser", objUserModel);
        }

        // Update existing user
        [HttpPost]
        public ActionResult SaveUser(UserModel objUserModel)
        {

            //  string userName = form["multiselectRefinery"];

            bool status = objUserModel.AddUserDetails(objUserModel);
            objUserModel.GetUserDetails();
            objUserModel.email = null;
            objUserModel.firstName = null;
            objUserModel.lastName = null;
            objUserModel.userName = null;
            objUserModel.id = 0;
            var sessionObj = Session["SessionBO"] as UserModel;
            if (sessionObj != null)
            {
                ViewBag.IsAuthorized = sessionObj.isAuthorized;
                ViewBag.LoggedInUser = sessionObj.firstName;// +" " + sessionObj.LastName;
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            // return View(objAdminModel);
            return RedirectToAction("RegisterUser", "Admin");
        }

        // Edit User's Details
        public JsonResult EditUser(int UserID)
        {

            UserModel objUserModel = new UserModel();

            objUserModel = objUserModel.EditUser(UserID);

            try
            {
                return Json(new { ID = objUserModel.id, UserName = objUserModel.userName, FirstName = objUserModel.firstName, LastName = objUserModel.lastName, Email = objUserModel.email, Refinery = objUserModel.refinery }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        // Delete existing User
        public JsonResult DeleteUser(int UserID)
        {
            UserModel objUserModel = new UserModel();

            bool status = objUserModel.DeleteUser(UserID);
            if (status)
            {
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);

        }

        // Check User is Exist or not in Database
        public JsonResult CheckExitUser(string UserName)
        {
            UserModel objUserModel = new UserModel();

            bool status = objUserModel.CheckExitUser(UserName);
            if (status)
            {

                return Json(new { status = "exist" }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { status = "notexist" }, JsonRequestBehavior.AllowGet);


            }


        }

        #endregion User

    }
}
