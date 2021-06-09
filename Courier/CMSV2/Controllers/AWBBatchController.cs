using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using CMSV2.DAL;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Reflection;

namespace CMSV2.Controllers
{
    [SessionExpireFilter]
    public class AWBBatchController : Controller
    {
        Entities1 db = new Entities1();

        // GET: AWBBatch
        public ActionResult Index()
        {

            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            AWBBatchSearch obj = (AWBBatchSearch)Session["AWBBatchSearch"];
            AWBBatchSearch model = new AWBBatchSearch();
            AWBDAO _dao = new AWBDAO();
            if (obj != null)
            {
                List<AWBBatchList> translist = new List<AWBBatchList>();
                
                model.FromDate = obj.FromDate;
                model.ToDate = obj.ToDate;
                model.DocumentNo = obj.DocumentNo;
                translist = AWBDAO.GetAWBBatchList(BranchID);
                model.Details = translist;
            }
            else
            {
                model.FromDate = CommanFunctions.GetLastDayofMonth().Date;
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
                Session["AWBBatchSearch"] = model;
                List<AWBBatchList> translist = new List<AWBBatchList>();                
                translist = AWBDAO.GetAWBBatchList(BranchID);
                model.Details = translist;

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(AWBBatchSearch obj)
        {
            Session["AWBBatchSearch"] = obj;
            return RedirectToAction("Index");
        }


        public ActionResult Create()
        {
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            int branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            AWBBatchVM vm = new AWBBatchVM();
            ViewBag.Movement = db.CourierMovements.ToList();
            ViewBag.ProductType = db.ProductTypes.ToList();
            ViewBag.parceltype = db.ParcelTypes.ToList();
            ViewBag.Employee = db.EmployeeMasters.ToList();
            ViewBag.PaymentMode = db.tblPaymentModes.ToList();
            ViewBag.Vehicle = db.VehicleMasters.ToList();
            ViewBag.Title = "AWB Batch - Create";
            string DocNo = AWBDAO.GetMaxBathcNo(); //batch no
            vm.BatchNumber = DocNo;
            DateTime pFromDate = CommanFunctions.GetCurrentDateTime(); //  AccountsDAO.CheckParamDate(DateTime.Now, FyearId).Date;
            vm.BatchDate = pFromDate;
            vm.AWBDate = pFromDate;

            var branch = db.BranchMasters.Find(branchid);
            if (branch != null)
            {
                vm.BranchLocation = branch.LocationName;
                vm.BranchCountry = branch.CountryName;
                vm.BranchCity = branch.CityName;
            }


            vm.AssignedDate = pFromDate;
            vm.TaxPercent = 5;
            var defaultproducttype = db.ProductTypes.ToList().Where(cc => cc.DefaultType == true).FirstOrDefault();
            if (defaultproducttype != null)
                vm.ProductTypeID = defaultproducttype.ProductTypeID;

            var defaultmovementtype = db.CourierMovements.ToList().Where(cc => cc.DefaultType == true).FirstOrDefault();
            if (defaultmovementtype != null)
                vm.MovementID = defaultmovementtype.MovementID;

            var defaultparceltype = db.ParcelTypes.ToList().Where(cc => cc.DefaultType == true).FirstOrDefault();
            if (defaultparceltype != null)
                vm.ParcelTypeID = defaultparceltype.ID;

            string customername = "";
            

            customername = "WALK-IN-CUSTOMER";
            var CashCustomer = (from c1 in db.CustomerMasters
                                where c1.CustomerName == customername
                                orderby c1.CustomerName ascending
                                select new { CustomerID = c1.CustomerID, CustomerName = c1.CustomerName }).FirstOrDefault();
            if (CashCustomer != null)
            {
                vm.CASHCustomerId = CashCustomer.CustomerID;
                vm.CASHCustomerName = customername;
            }

            customername = "COD-CUSTOMER";
            var CODCustomer = (from c1 in db.CustomerMasters
                               where c1.CustomerName == customername
                               orderby c1.CustomerName ascending
                               select new { CustomerID = c1.CustomerID, CustomerName = c1.CustomerName }).FirstOrDefault();
            if (CODCustomer != null)
            {
                vm.CODCustomerID = CODCustomer.CustomerID;
                vm.CODCustomerName = "COD-CUSTOMER";
            }

            customername = "FOC CUSTOMER";
            var FOCCustomer = (from c1 in db.CustomerMasters
                               where c1.CustomerName == customername
                               orderby c1.CustomerName ascending
                               select new { CustomerID = c1.CustomerID, CustomerName = c1.CustomerName }).FirstOrDefault();
            if (FOCCustomer != null)
            {
                vm.FOCCustomerID = FOCCustomer.CustomerID;
                vm.FOCCustomerName = "FOC CUSTOMER";
            }

            var FAgent = db.ForwardingAgentMasters.Where(cc => cc.StatusDefault == true).FirstOrDefault();
            if (FAgent != null)
            {
                vm.FAgentName = FAgent.FAgentName;
                vm.FAgentID = FAgent.FAgentID;
            }
            else
            {
                vm.FAgentID = 0;
                vm.FAgentName = "";
            }
            return View(vm);
        }
        public ActionResult Edit(int id)
        {
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            int branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            AWBBatch v = db.AWBBatches.Find(id);
            AWBBatchVM vm = new AWBBatchVM();
            ViewBag.Movement = db.CourierMovements.ToList();
            ViewBag.ProductType = db.ProductTypes.ToList();
            ViewBag.parceltype = db.ParcelTypes.ToList();
            ViewBag.Employee = db.EmployeeMasters.ToList();
            ViewBag.PaymentMode = db.tblPaymentModes.ToList();
            ViewBag.Vehicle = db.VehicleMasters.ToList();
            ViewBag.Title = "AWB Batch - Create";
            vm.ID = v.ID;
            vm.BatchNumber = v.BatchNumber;
            vm.BatchDate = v.BatchDate;
            var branch = db.BranchMasters.Find(branchid);
            var FAgent = db.ForwardingAgentMasters.Where(cc => cc.StatusDefault == true).FirstOrDefault();
            if (FAgent != null)
            {
                vm.FAgentName = FAgent.FAgentName;
                vm.FAgentID = FAgent.FAgentID;
            }
            else
            {
                vm.FAgentID = 0;
                vm.FAgentName = "";
            }
            if (branch != null)
            {
                vm.BranchLocation = branch.LocationName;
                vm.BranchCountry = branch.CountryName;
                vm.BranchCity = branch.CityName;
            }
            vm.TaxPercent = 5;
            var defaultproducttype = db.ProductTypes.ToList().Where(cc => cc.DefaultType == true).FirstOrDefault();
            if (defaultproducttype != null)
                vm.ProductTypeID = defaultproducttype.ProductTypeID;

            var defaultmovementtype = db.CourierMovements.ToList().Where(cc => cc.DefaultType == true).FirstOrDefault();
            if (defaultmovementtype != null)
                vm.MovementID = defaultmovementtype.MovementID;

            var defaultparceltype = db.ParcelTypes.ToList().Where(cc => cc.DefaultType == true).FirstOrDefault();
            if (defaultparceltype != null)
                vm.ParcelTypeID = defaultparceltype.ID;

            string customername = "";


            customername = "WALK-IN-CUSTOMER";
            var CashCustomer = (from c1 in db.CustomerMasters
                                where c1.CustomerName == customername
                                orderby c1.CustomerName ascending
                                select new { CustomerID = c1.CustomerID, CustomerName = c1.CustomerName }).FirstOrDefault();
            if (CashCustomer != null)
            {
                vm.CASHCustomerId = CashCustomer.CustomerID;
                vm.CASHCustomerName = customername;
            }

            customername = "COD-CUSTOMER";
            var CODCustomer = (from c1 in db.CustomerMasters
                               where c1.CustomerName == customername
                               orderby c1.CustomerName ascending
                               select new { CustomerID = c1.CustomerID, CustomerName = c1.CustomerName }).FirstOrDefault();
            if (CODCustomer != null)
            {
                vm.CODCustomerID = CODCustomer.CustomerID;
                vm.CODCustomerName = "COD-CUSTOMER";
            }

            customername = "FOC CUSTOMER";
            var FOCCustomer = (from c1 in db.CustomerMasters
                               where c1.CustomerName == customername
                               orderby c1.CustomerName ascending
                               select new { CustomerID = c1.CustomerID, CustomerName = c1.CustomerName }).FirstOrDefault();
            if (FOCCustomer != null)
            {
                vm.FOCCustomerID = FOCCustomer.CustomerID;
                vm.FOCCustomerName = "FOC CUSTOMER";
            }
            vm.ID = id;
            vm.Details = AWBDAO.GetBatchAWBInfo(id);
            return View(vm);
        }

        [HttpPost]
        public string SaveBatch(string BatchNo, string BatchDate, string Details)
        {
            try
            {
                Details.Replace("{}", "");
                var IDetails = JsonConvert.DeserializeObject<List<AWBBatchDetail>>(Details);
                DataTable ds = new DataTable();
                DataSet dt = new DataSet();
                dt = ToDataTable(IDetails);
                int FyearId = Convert.ToInt32(Session["fyearid"]);
                string xml = dt.GetXml();
                xml = xml.Replace("T00:00:00+05:30", "");
                if (Session["UserID"] != null)
                {
                    int userid = Convert.ToInt32(Session["UserID"].ToString());
                    int CompanyID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    int BranchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    int DepotID = Convert.ToInt32(Session["CurrentDepotID"].ToString());
                    AWBBatch batch = new AWBBatch();
                    batch.BatchNumber = BatchNo;
                    batch.BatchDate = Convert.ToDateTime(BatchDate);
                    batch.UserID = userid;
                    batch.AcFinancialYearid = FyearId;
                    batch.BranchID = BranchId;
                    db.AWBBatches.Add(batch);
                    db.SaveChanges();

                    string result = AWBDAO.SaveAWBBatch(batch.ID, BranchId, CompanyID, DepotID, userid, FyearId, xml);
                    return result;
                }
                else
                {
                    return "Failed!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string UpdateBatch(int BatchId, string Details)
        {
            try
            {
                Details.Replace("{}", "");
                var IDetails = JsonConvert.DeserializeObject<List<AWBBatchDetail>>(Details);
                DataTable ds = new DataTable();
                DataSet dt = new DataSet();
                dt = ToDataTable(IDetails);
                int FyearId = Convert.ToInt32(Session["fyearid"]);
                string xml = dt.GetXml();
                xml = xml.Replace("T00:00:00+05:30", "");
                if (Session["UserID"] != null)
                {
                    int userid = Convert.ToInt32(Session["UserID"].ToString());
                    int CompanyID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    int BranchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    int DepotID = Convert.ToInt32(Session["CurrentDepotID"].ToString());

                    string result = AWBDAO.UpdateAWBBatch(BatchId, BranchId, CompanyID, DepotID, userid, FyearId, xml);
                    return result;
                }
                else
                {
                    return "Failed!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public JsonResult GetConsignorCustomer(string customername)
        {
            var customerlist = (from c1 in db.CustomerMasters
                                where c1.CustomerName == customername
                                orderby c1.CustomerName ascending
                                select new { CustomerID = c1.CustomerID, CustomerName = c1.CustomerName }).FirstOrDefault();

            return Json(customerlist, JsonRequestBehavior.AllowGet);

        }
        public static DataSet ToDataTable<T>(List<T> items)
        {
            DataSet ds = new DataSet();
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            ds.Tables.Add(dataTable);
            //put a breakpoint here and check datatable
            return ds;
        }

        [HttpGet]
        public JsonResult GetShipperName(string term)
        {
            List<Consignor> shipperlist = (List<Consignor>)Session["ConsignorMaster"];
            //if (shipperlist == null)
            //{
            //    shipperlist = (from c1 in db.ConsignorMasters                               
            //                   orderby c1.ConsignorName ascending
            //                   select new Consignor { ShipperName = c1.ConsignorName, ContactPerson = c1.ConsignorContactName, Phone = c1.ConsignorPhoneNo, LocationName = c1.ConsignorLocationName, CityName = c1.ConsignorCityName, CountryName = c1.ConsignorCountryname, Address1 = c1.ConsignorAddress1, Address2 = c1.ConsignorAddress2, PinCode = c1.ConsignorAddress3, ConsignorMobileNo =  c1.MobileNo }).Distinct().ToList();
            //    Session["ConsignorMaster"] = shipperlist;
            //}
            if (term.Trim() != "")
            {
                shipperlist = (from c1 in db.ConsignorMasters
                               where c1.ConsignorName.ToLower().Contains(term.Trim().ToLower())
                               orderby c1.ConsignorName ascending
                               select new Consignor { ShipperName = c1.ConsignorName, ContactPerson = c1.ConsignorContactName, Phone = c1.ConsignorPhoneNo, LocationName = c1.ConsignorLocationName, CityName = c1.ConsignorCityName, CountryName = c1.ConsignorCountryname, Address1 = c1.ConsignorAddress1, Address2 = c1.ConsignorAddress2, PinCode = c1.ConsignorAddress3, ConsignorMobileNo = c1.MobileNo }).Distinct().Take(20).ToList();
                //shipperlist = shipperlist.Where(cc => cc.ShipperName.ToLower().Contains(term.Trim().ToLower())).ToList();
                //if (shipperlist.Count > 100)
                //{
                //    shipperlist = shipperlist.Take(100).ToList();
                //}

                //var shipperlist = (from c1 in db.InScanMasters
                //                   where c1.IsDeleted == false && c1.Consignor.ToLower().StartsWith(term.ToLower())
                //                   orderby c1.Consignor ascending
                //                   select new { ShipperName = c1.Consignor, ContactPerson = c1.ConsignorContact, Phone = c1.ConsignorPhone, LocationName = c1.ConsignorLocationName, CityName = c1.ConsignorCityName, CountryName = c1.ConsignorCountryName, Address1 = c1.ConsignorAddress1_Building, Address2 = c1.ConsignorAddress2_Street, PinCode = c1.ConsignorAddress3_PinCode, ConsignorMobileNo = c1.ConsignorMobileNo }).Distinct();
                return Json(shipperlist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //if (shipperlist == null)
                //{
                //    shipperlist = (from c1 in db.ConsignorMasters
                //                   where c1.ConsignorName.ToLower().StartsWith(term.ToLower())
                //                   orderby c1.ConsignorName ascending
                //                   select new Consignor { ShipperName = c1.ConsignorName, ContactPerson = c1.ConsignorName, Phone = c1.ConsignorPhoneNo, LocationName = c1.ConsignorLocationName, CityName = c1.ConsignorCityName, CountryName = c1.ConsignorCountryname, Address1 = c1.ConsignorAddress1, Address2 = c1.ConsignorAddress2, PinCode = c1.ConsignorAddress3, ConsignorMobileNo = "" }).Distinct().ToList();
                //}
                //var shipperlist = (from c1 in db.ConsignorMasters                                    
                //                    orderby c1.ConsignorName ascending
                //                    select new { ShipperName = c1.ConsignorName, ContactPerson = c1.ConsignorName, Phone = c1.ConsignorPhoneNo, LocationName = c1.ConsignorLocationName, CityName = c1.ConsignorCityName, CountryName = c1.ConsignorCountryname, Address1 = c1.ConsignorAddress1, Address2 = c1.ConsignorAddress2, PinCode = c1.ConsignorAddress3, ConsignorMobileNo = "" }).Distinct();

                //var shipperlist = (from c1 in db.InScanMasters
                //                   where c1.IsDeleted == false
                //                   orderby c1.Consignor ascending
                //                   select new { ShipperName = c1.Consignor, ContactPerson = c1.ConsignorContact, Phone = c1.ConsignorPhone, LocationName = c1.ConsignorLocationName, CityName = c1.ConsignorCityName, CountryName = c1.ConsignorCountryName, Address1 = c1.ConsignorAddress1_Building, Address2 = c1.ConsignorAddress2_Street, PinCode = c1.ConsignorAddress3_PinCode, ConsignorMobileNo = c1.ConsignorMobileNo }).Distinct();
                shipperlist = (from c1 in db.ConsignorMasters                               
                               orderby c1.ConsignorName ascending
                               select new Consignor { ShipperName = c1.ConsignorName, ContactPerson = c1.ConsignorContactName, Phone = c1.ConsignorPhoneNo, LocationName = c1.ConsignorLocationName, CityName = c1.ConsignorCityName, CountryName = c1.ConsignorCountryname, Address1 = c1.ConsignorAddress1, Address2 = c1.ConsignorAddress2, PinCode = c1.ConsignorAddress3, ConsignorMobileNo = c1.MobileNo }).Distinct().Take(20).ToList();
                if (shipperlist.Count > 100)
                {
                    shipperlist = shipperlist.Take(100).ToList();
                }
                else
                {
                    //shipperlist = shipperlist.Take(100).ToList();
                }
                return Json(shipperlist, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public JsonResult GetReceiverName(string term, string Shipper="",bool ShowAll=false)
        {
            if (term.Trim() != "")
            {
                if (ShowAll == false)
                {
                    var shipperlist = (from c1 in db.ConsigneeMasters
                                       where c1.ConsigneeName.ToLower().StartsWith(term.ToLower())
                                       && c1.ConsignorName.ToLower().StartsWith(Shipper.ToLower())
                                       orderby c1.ConsigneeName ascending
                                       select new { Name = c1.ConsigneeName, ContactPerson = c1.ConsigneeContactName, Phone = c1.ConsigneePhoneNo, LocationName = c1.ConsigneeLocationName, CityName = c1.ConsigneeCityName, CountryName = c1.ConsigneeCountryname, Address1 = c1.ConsigneeAddress1, Address2 = c1.ConsigneeAddress2, PinCode = c1.ConsigneeAddress3, ConsigneeMobileNo = c1.MobileNo }).Distinct();

                    //var shipperlist = (from c1 in db.InScanMasters
                    //                   where c1.Consignee.ToLower().StartsWith(term.ToLower())
                    //                   && c1.Consignor.ToLower().StartsWith(Shipper.ToLower())
                    //                   orderby c1.Consignee ascending
                    //                   select new { Name = c1.Consignee, ContactPerson = c1.ConsigneeContact, Phone = c1.ConsigneePhone, LocationName = c1.ConsigneeLocationName, CityName = c1.ConsigneeCityName, CountryName = c1.ConsigneeCountryName, Address1 = c1.ConsigneeAddress1_Building, Address2 = c1.ConsigneeAddress2_Street, PinCode = c1.ConsigneeAddress3_PinCode, ConsignorMobileNo = c1.ConsignorMobileNo, ConsigneeMobileNo = c1.ConsigneeMobileNo }).Distinct();
                    if (shipperlist.Count() > 100)
                    {
                        shipperlist = shipperlist.Take(100);
                    }
                    return Json(shipperlist, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var shipperlist = (from c1 in db.ConsigneeMasters
                                       where c1.ConsigneeName.ToLower().StartsWith(term.ToLower())                                       
                                       orderby c1.ConsigneeName ascending
                                       select new { Name = c1.ConsigneeName, ContactPerson = c1.ConsigneeContactName, Phone = c1.ConsigneePhoneNo, LocationName = c1.ConsigneeLocationName, CityName = c1.ConsigneeCityName, CountryName = c1.ConsigneeCountryname, Address1 = c1.ConsigneeAddress1, Address2 = c1.ConsigneeAddress2, PinCode = c1.ConsigneeAddress3, ConsigneeMobileNo = c1.MobileNo }).Distinct();
                    if (shipperlist.Count() >100)
                    {
                        shipperlist = shipperlist.Take(100);
                    }
                    return Json(shipperlist, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (ShowAll == false)
                {
                    var shipperlist = (from c1 in db.ConsigneeMasters
                                       where
                                       c1.ConsignorName.ToLower().StartsWith(Shipper.ToLower())
                                       orderby c1.ConsigneeName ascending
                                       select new { Name = c1.ConsigneeName, ContactPerson = c1.ConsigneeContactName, Phone = c1.ConsigneePhoneNo, LocationName = c1.ConsigneeLocationName, CityName = c1.ConsigneeCityName, CountryName = c1.ConsigneeCountryname, Address1 = c1.ConsigneeAddress1, Address2 = c1.ConsigneeAddress2, PinCode = c1.ConsigneeAddress3, ConsigneeMobileNo = c1.MobileNo }).Distinct();
                    if (shipperlist.Count() > 100)
                    {
                        shipperlist = shipperlist.Take(100);
                    }
                    return Json(shipperlist, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var shipperlist = (from c1 in db.ConsigneeMasters                                       
                                       orderby c1.ConsigneeName ascending
                                       select new { Name = c1.ConsigneeName, ContactPerson = c1.ConsigneeContactName, Phone = c1.ConsigneePhoneNo, LocationName = c1.ConsigneeLocationName, CityName = c1.ConsigneeCityName, CountryName = c1.ConsigneeCountryname, Address1 = c1.ConsigneeAddress1, Address2 = c1.ConsigneeAddress2, PinCode = c1.ConsigneeAddress3, ConsigneeMobileNo = c1.MobileNo }).Take(100);
                    //if (shipperlist.Count() > 100)
                    //{
                    //    shipperlist = shipperlist.Take(100);
                    //}
                    return Json(shipperlist, JsonRequestBehavior.AllowGet);
                }

            }

        }

        [HttpGet]
        public JsonResult GetAWBInfo(string awbno)
        {
            AWBInfo info = AWBDAO.GetAWBInfo(awbno.Trim());

            return Json(info, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetAWBTrackStatus(string awbno)
        {
            AWBBatchDetail info = AWBDAO.GetAWBTrackStatus(awbno);

            return Json(info, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetAWBBatch(string term)
        {
            if (term.Trim() != "")
            {
                var customerlist = (from c1 in db.AWBBatches
                                    where c1.BatchNumber.Contains(term)
                                    orderby c1.BatchNumber descending
                                    select new { BatchId = c1.ID, BatchNo = c1.BatchNumber }).ToList();

                return Json(customerlist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var customerlist = (from c1 in db.AWBBatches
                                    orderby c1.BatchNumber descending
                                    select new { BatchId = c1.ID, BatchNo = c1.BatchNumber }).ToList();

                return Json(customerlist, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetBatchAWB(int id)
        {
            List<AWBBatchDetail> Details = AWBDAO.GetBatchAWBInfo(id);
            return Json(Details, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTypeofGoods(string term)
        {
            if (term.Trim() !="")
            {
                var list = db.TypeOfGoods.Where(cc => cc.TypeOfGood1.Contains(term.Trim())).OrderBy(cc => cc.TypeOfGood1).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = db.TypeOfGoods.ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult ConsignorAddress()
        {
            AWBBatchVM vm = new AWBBatchVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult ShowConsignorAddres(AWBBatchVM model)
        {
            return PartialView("ConsignorAddress", model);
        }

        [HttpPost]
        public JsonResult SaveConsignorAddress(Consignor model)
        {
            bool newentry = false;
            ConsignorMaster obj = db.ConsignorMasters.Where(cc => cc.ConsignorName == model.ShipperName).FirstOrDefault();

            if (obj == null)
            {
                newentry = true;
                obj = new ConsignorMaster();
            }
            obj.ConsignorName = model.ShipperName;
            obj.ConsignorContactName = model.ContactPerson;
            obj.ConsignorPhoneNo = model.Phone;
            obj.ConsignorAddress1 = model.Address1;
            obj.ConsignorAddress2 = model.Address2;
            obj.ConsignorAddress3 = model.PinCode;
            obj.ConsignorPhoneNo = model.Phone;
            obj.MobileNo = model.ConsignorMobileNo;
            obj.ConsignorLocationName = model.LocationName;
            obj.ConsignorCountryname = model.CountryName;
            obj.ConsignorCityName = model.CityName;
            
            if (newentry==true)
            {
                db.ConsignorMasters.Add(obj);
                db.SaveChanges();
            }
            else
            {
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SaveConsigeeAddress(Consignor model)
        {
            bool newentry = false;
            ConsigneeMaster obj = db.ConsigneeMasters.Where(cc => cc.ConsigneeName == model.ConsignorName && cc.ConsignorName==model.ShipperName ).FirstOrDefault();

            if (obj == null)
            {
                newentry = true;
                obj = new ConsigneeMaster();
            }
            obj.ConsignorName = model.ShipperName;
            obj.ConsigneeName = model.ConsignorName; //it will return consignee name only
            obj.ConsigneeContactName = model.ContactPerson;
            obj.ConsigneePhoneNo = model.Phone;
            obj.ConsigneeAddress1 = model.Address1;
            obj.ConsigneeAddress2 = model.Address2;
            obj.ConsigneeAddress3 = model.PinCode;
            obj.ConsigneePhoneNo = model.Phone;
            obj.MobileNo = model.ConsignorMobileNo;
            obj.ConsigneeLocationName = model.LocationName;
            obj.ConsigneeCountryname = model.CountryName;
            obj.ConsigneeCityName = model.CityName;

            if (newentry == true)
            {
                db.ConsigneeMasters.Add(obj);
                db.SaveChanges();
            }
            else
            {
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ShowConsigneeAddres(AWBBatchVM model)
        {
            return PartialView("ConsigneeAddress", model);
        }

        public JsonResult GetCustomerRateType(string term, string CustomerId,string MovementId,string ProductTypeID,string PaymentModeId,string FAgentID,string CityName,string CountryName,string OriginCountry,string OriginCity)
        {
             int pCustomerId = 0;
            int pMovementId = 0;
            int pProductTypeID = 0;
            int pPaymentModeId = 0;
            int pAgentID = 0;
            string pCountryname = ""; ;
            string pcityname = "";            

            if (CustomerId != "")
                pCustomerId = Convert.ToInt32(CustomerId);

            if (MovementId != "")
                pMovementId = Convert.ToInt32(MovementId);

            if (ProductTypeID != "")
                pProductTypeID = Convert.ToInt32(ProductTypeID);

            if (PaymentModeId != "")
                pPaymentModeId = Convert.ToInt32(PaymentModeId);

            if (FAgentID != "")
                pAgentID = Convert.ToInt32(FAgentID);

            if (pMovementId == 3)
            {
                pCountryname = OriginCountry;
                pcityname = OriginCity;
            }
            else
            {
                pCountryname = CountryName;
                pcityname = CityName;

            }

            List<CustomerRateType> lst = new List<CustomerRateType>();
            var loc = AWBDAO.GetRateList(pCustomerId, pMovementId, pProductTypeID, pPaymentModeId,pAgentID,pcityname,pCountryname);

            if (term.Trim() != "")
            {
                lst = (from c in loc where c.CustomerRateType1.Contains(term) orderby c.CustomerRateType1 select c).ToList();
            }
            else
            {
                lst = (from c in loc  orderby c.CustomerRateType1 select c).ToList();
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult GetCourierCharge(string CustomerRateTypeID, string CustomerId, string MovementId, string ProductTypeID, string PaymentModeId,string Weight,string CountryName,string CityName)
        {
            int pRateTypeID = 0;
            int pCustomerId = 0;
            int pMovementId = 0;
            int pProductTypeID = 0;
            int pPaymentModeId = 0;
            decimal pWeight = 0;
            if (CustomerRateTypeID!="")
            {
                pRateTypeID = Convert.ToInt32(CustomerRateTypeID);
            }
            if (CustomerId != "")
                pCustomerId = Convert.ToInt32(CustomerId);

            if (MovementId != "")
                pMovementId = Convert.ToInt32(MovementId);

            if (ProductTypeID != "")
                pProductTypeID = Convert.ToInt32(ProductTypeID);

            if (PaymentModeId != "")
                pPaymentModeId = Convert.ToInt32(PaymentModeId);

            if (Weight != "")
                pWeight = Convert.ToDecimal(Weight);

            CustomerRateTypeVM vm = AWBDAO.GetCourierCharge(pRateTypeID,pCustomerId, pMovementId, pProductTypeID, pPaymentModeId,pWeight,CountryName,CityName);
                        
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetParcelType(string ProductTypeId)
        {
            int pProductTypeID = 0;

            if (ProductTypeId != "")
                pProductTypeID = Convert.ToInt32(ProductTypeId);

            int ParcelTypeId = 0;
            var prod = db.ProductTypes.Find(ProductTypeId);
            if (prod!=null)
            {
                ParcelTypeId=prod.ParcelTypeID;
            }

            return Json(ParcelTypeId, JsonRequestBehavior.AllowGet);

        }

      
        public JsonResult GetForwardingAgent(string term)
        {
            if (term.Trim() != "")
            {
                var list = (from c in db.ForwardingAgentMasters where c.FAgentName.Contains(term.Trim()) orderby c.FAgentName select new { FAgentID = c.FAgentID, AgentName = c.FAgentName }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = (from c in db.ForwardingAgentMasters orderby c.FAgentName select new { FAgentID = c.FAgentID, AgentName = c.FAgentName }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetCustomerRateDetail(int id)
        {
            string ratename = AWBDAO.GetCustomerRateName(id);
            return Json(ratename, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFAgentName(int id)
        {
            var list = (from c in db.ForwardingAgentMasters where c.FAgentID == id orderby c.FAgentName select new { FAgentID = c.FAgentID, AgentName = c.FAgentName }).FirstOrDefault();
            if (list != null)
            {
                return Json(list.AgentName, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CheckShipperName(string term)
        {
            List<Consignor> shipperlist = (List<Consignor>)Session["ConsignorMaster"];
            if (shipperlist == null)
            {
                shipperlist = (from c1 in db.ConsignorMasters
                               orderby c1.ConsignorName ascending
                               select new Consignor { ShipperName = c1.ConsignorName, ContactPerson = c1.ConsignorName, Phone = c1.ConsignorPhoneNo, LocationName = c1.ConsignorLocationName, CityName = c1.ConsignorCityName, CountryName = c1.ConsignorCountryname, Address1 = c1.ConsignorAddress1, Address2 = c1.ConsignorAddress2, PinCode = c1.ConsignorAddress3, ConsignorMobileNo = c1.MobileNo }).Distinct().ToList();
                Session["ConsignorMaster"] = shipperlist;
            }
            if (term.Trim() != "")
            {

                var shipper = shipperlist.Where(cc => cc.ShipperName.ToLower()==term.Trim().ToLower()).FirstOrDefault();

                //var shipperlist = (from c1 in db.InScanMasters
                //                   where c1.IsDeleted == false && c1.Consignor.ToLower().StartsWith(term.ToLower())
                //                   orderby c1.Consignor ascending
                //                   select new { ShipperName = c1.Consignor, ContactPerson = c1.ConsignorContact, Phone = c1.ConsignorPhone, LocationName = c1.ConsignorLocationName, CityName = c1.ConsignorCityName, CountryName = c1.ConsignorCountryName, Address1 = c1.ConsignorAddress1_Building, Address2 = c1.ConsignorAddress2_Street, PinCode = c1.ConsignorAddress3_PinCode, ConsignorMobileNo = c1.ConsignorMobileNo }).Distinct();
                if (shipper!=null)
                    return Json("Exists", JsonRequestBehavior.AllowGet);
                else
                    return Json("NotExists", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            
          
        }

    }
}

