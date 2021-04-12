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
            return View();
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
            DateTime pFromDate = AccountsDAO.CheckParamDate(DateTime.Now, FyearId).Date;
            vm.BatchDate = pFromDate;
            vm.AWBDate = pFromDate;
            
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
            if (CODCustomer!=null)
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
            catch(Exception ex)
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
        public JsonResult GetReceiverName(string term,string Shipper)
        {
            if (term.Trim() != "")
            {
                var shipperlist = (from c1 in db.InScanMasters
                                   where c1.Consignee.ToLower().StartsWith(term.ToLower())
                                   && c1.Consignor.ToLower().StartsWith(Shipper.ToLower())
                                   orderby c1.Consignee ascending
                                   select new { Name = c1.Consignee, ContactPerson = c1.ConsigneeContact, Phone = c1.ConsigneePhone, LocationName = c1.ConsigneeLocationName, CityName = c1.ConsigneeCityName, CountryName = c1.ConsigneeCountryName, Address1 = c1.ConsigneeAddress1_Building, Address2 = c1.ConsigneeAddress2_Street, PinCode = c1.ConsigneeAddress3_PinCode, ConsignorMobileNo = c1.ConsignorMobileNo, ConsigneeMobileNo = c1.ConsigneeMobileNo }).Distinct();

                return Json(shipperlist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var shipperlist = (from c1 in db.InScanMasters
                                   where c1.Consignor.ToLower().StartsWith(Shipper.ToLower())
                                   orderby c1.Consignee ascending
                                   select new { Name = c1.Consignee, ContactPerson = c1.ConsigneeContact, Phone = c1.ConsigneePhone, LocationName = c1.ConsigneeLocationName, CityName = c1.ConsigneeCityName, CountryName = c1.ConsigneeCountryName, Address1 = c1.ConsigneeAddress1_Building, Address2 = c1.ConsigneeAddress2_Street, PinCode = c1.ConsigneeAddress3_PinCode, ConsignorMobileNo = c1.ConsignorMobileNo, ConsigneeMobileNo = c1.ConsigneeMobileNo }).Distinct();

                return Json(shipperlist, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpGet]
        public JsonResult GetAWBInfo(string awbno)
        {
            AWBInfo info = AWBDAO.GetAWBInfo(awbno);

            return Json(info, JsonRequestBehavior.AllowGet);

        }
    }
}