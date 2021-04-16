using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using CMSV2.DAL;
using Newtonsoft.Json;


namespace CMSV2.Controllers
{
    [SessionExpireFilter]
    public class AWBStatusController : Controller
    {
        Entities1 db = new Entities1();

        // GET: POD
        [HttpGet]
        public ActionResult Index(int id = 0)                   
        {
            PODSearch obj = (PODSearch)Session["PODSearch"];
            PODSearch model = new PODSearch();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            if (obj == null) 
            {
                DateTime pFromDate;
                DateTime pToDate;
                //int pStatusId = 0; 
                pFromDate = CommanFunctions.GetLastDayofMonth().Date; // DateTimeOffset.Now.Date;// CommanFunctions.GetFirstDayofMonth().Date; // DateTime.Now.Date; //.AddDays(-1) ; // FromDate = DateTime.Now;
                pToDate = CommanFunctions.GetLastDayofMonth().Date; // DateTime.Now.Date.AddDays(1); // // ToDate = DateTime.Now;
                
                obj = new PODSearch();
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;                
                Session["PODSearch"] = obj;
                model.FromDate = pFromDate;
                model.ToDate = pToDate;
                
            }
            else
            {
                model = obj;
            }
            
            List<PODVM> lst = (from p in db.PODs join c1 in db.InScanMasters on p.InScanID equals c1.InScanID                                     
                                join c2 in db.CourierStatus on p.CourierStatusID equals  c2.CourierStatusID
                                    where c1.BranchID == branchid && c1.DepotID == depotId
                                    //&& c.AcFinancialYearID==yearid                                
                                    && (p.DeliveredDate >= obj.FromDate && p.DeliveredDate < obj.ToDate)                                                                         
                                    orderby p.DeliveredDate descending
                                    select new PODVM {PODID=p.PODID,CourierStatus=c2.CourierStatus, AWBNo = c1.AWBNo, Shipper = c1.Consignor,Consignee=c1.Consignee,ReceiverName= p.ReceiverName,InScanID= c1.InScanID }).ToList();  //, requestsource=subpet3.RequestTypeName 
            model.Details = lst;
            return View(model);

        }

        [HttpPost]
        public ActionResult Index(AWBSearch obj)
        {
            Session["PODSearch"] = obj;
            return RedirectToAction("Index");
        }


        [HttpGet]
        public JsonResult GetAWBInfo(string awbno)
        {            
            var awb = (from c in db.InScanMasters where c.IsDeleted == false && c.AWBNo == awbno
                       select new PODVM { AWBDate=c.TransactionDate, InScanID = c.InScanID, AWBNo = c.AWBNo,
                           Shipper = c.Consignor, Consignee = c.Consignee, ConsigneeContact = c.ConsigneeContact, CourierStatusId=c.CourierStatusID,StatusTypeId=c.StatusTypeId }).FirstOrDefault();
            
            string pstatus = "Valid";
            if (awb != null)
            {
                if (awb.CourierStatusId != 8 && awb.CourierStatusID!=10) //outscan for delivery or  pending
                    pstatus = "NotValid";
            }
            else
            {
                pstatus = "NotAvailabel";
            }
            return Json( new { Status=pstatus, data=awb }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Create()
        {
            
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            int branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            PODVM vm = new PODVM();            
            
            ViewBag.Employee = db.EmployeeMasters.ToList();
            
            ViewBag.Vehicle = db.VehicleMasters.ToList();
            ViewBag.Title = "POD - Create";
            DateTime pFromDate = AccountsDAO.CheckParamDate(DateTime.Now, FyearId).Date;
            vm.DeliveredDate = pFromDate;
            vm.Delivered = true;
            vm.DelieveryAttemptDate = pFromDate;
            vm.AWBDate = pFromDate;
            return View(vm);

        }

        public JsonResult GetAWBDetail(string awbno,int? batchid)
        {            
            
            if (batchid == null)
                batchid = 0;

            var lst = (from c in db.InScanMasters
                       join c1 in db.CourierStatus on c.CourierStatusID equals c1.CourierStatusID
                       where (c.BATCHID == batchid || c.AWBNo == awbno) && (c.CourierStatusID == 4 || c.CourierStatusID == 8 || c.CourierStatusID == 9 || c.CourierStatusID == 10) 
                       select new AWBBatchDetail { InScanID = c.InScanID, AWBNo = c.AWBNo, ConsignorCountryName  = c.ConsignorCountryName, 
                                    ConsigneeCountryName = c.ConsigneeCountryName ,PickupLocation=c.PickupLocation,DeliveryLocation=c.DeliveryLocation,CurrentCourierStatus=c1.CourierStatus }).ToList();


            //var lst = (from c in db.InScanMasters where c.AWBNo == id &&  (c.PickedUpEmpID==collectedby || collectedby==0) select c).FirstOrDefault();
            if (lst == null)
            {
                return Json(new { status = "failed", data = lst, message = "AWB No. Not found" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "ok", data = lst, message = "AWB Found" }, JsonRequestBehavior.AllowGet);
                //if (lst.QuickInscanID == null)
                //{
                //    obj.Origin = lst.ConsignorCountryName;
                //    obj.Destination = lst.ConsigneeCountryName;
                //    obj.AWB = lst.AWBNo;
                //    obj.InScanId = lst.InScanID;



                //}
                //else
                //{
                //    return Json(new { status = "failed", data = obj, message = "InScan already Done!" }, JsonRequestBehavior.AllowGet);
                //}
            }

        }
        [HttpPost]
        public string SaveBatchPOD(string Details)
        {
            try
            {
                Details.Replace("{}", "");
                var IDetails = JsonConvert.DeserializeObject<List<PODAWBDetail>>(Details);
                //DataTable ds = new DataTable();
                //DataSet dt = new DataSet();
                //dt = ToDataTable(IDetails);
                int FyearId = Convert.ToInt32(Session["fyearid"]);                
                if (Session["UserID"] != null)
                {
                   foreach(PODAWBDetail item in IDetails)
                    {
                        if (item.Delivered)
                        {
                            POD pod = new POD();
                            pod.InScanID = item.InScanID;
                            pod.ReceiverName = item.ReceiverName;
                            pod.ReceivedTime = item.DeliveredDate;
                            pod.DeliveredBy = item.DeliveredBy;
                            pod.DeliveredDate = item.DeliveredDate;
                            pod.UpdationDate = DateTime.Now;
                            db.PODs.Add(pod);
                            db.SaveChanges(); 

                            var inscan = db.InScanMasters.Find(item.InScanID);
                            inscan.StatusTypeId = 4;
                            inscan.CourierStatusID = 13;
                            db.Entry(inscan).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            AWBTrackStatu awbtrack = new AWBTrackStatu();
                            awbtrack.InScanId = item.InScanID;
                            awbtrack.StatusTypeId = 4; //Delivered
                            awbtrack.CourierStatusId = 13; //Shipment Delivered
                            awbtrack.EntryDate =Convert.ToDateTime(item.DeliveredDate);
                            awbtrack.UserId = Convert.ToInt32(Session["UserID"].ToString());
                            db.AWBTrackStatus.Add(awbtrack);
                            db.SaveChanges();
                        }
                        else if(item.DeliveryAttempted)
                        {
                            AWBTrackStatu awbtrack = new AWBTrackStatu();
                            awbtrack.InScanId = item.InScanID;
                            awbtrack.StatusTypeId = 3; //Depot Outscan
                            awbtrack.CourierStatusId =10; //9	Attempted To Deliver -- Delivery Pending  Depot Outscan 10 
                            awbtrack.EntryDate = Convert.ToDateTime(item.DelieveryAttemptDate);
                            awbtrack.UserId = Convert.ToInt32(Session["UserID"].ToString());
                            db.AWBTrackStatus.Add(awbtrack);
                            db.SaveChanges();
                        }
                    }

                    //string result = AWBDAO.SaveAWBBatch(batch.ID, BranchId, CompanyID, DepotID, userid, FyearId, xml);
                    return "ok";
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
    }
}