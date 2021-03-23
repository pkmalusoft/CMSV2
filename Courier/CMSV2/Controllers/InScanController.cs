using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using CMSV2.DAL;
using System.Data.Entity;

namespace CMSV2.Controllers
{
    [SessionExpireFilter]
    public class InScanController : Controller
    {
        Entities1 db = new Entities1();

        public ActionResult Index(int? StatusId, string FromDate, string ToDate)
        {
            ViewBag.Employee = db.EmployeeMasters.ToList();
            ViewBag.PickupRequestStatus = db.PickUpRequestStatus.ToList();

            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());

            DateTime pFromDate;
            DateTime pToDate;
            int pStatusId = 0;
            if (StatusId == null)
            {
                pStatusId = 0;
            }
            else
            {
                pStatusId = Convert.ToInt32(StatusId);
            }
            if (FromDate == null || ToDate == null)
            {
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date;// DateTime.Now.Date;//.AddDays(-1); // FromDate = DateTime.Now;
                pToDate = CommanFunctions.GetLastDayofMonth().Date.AddDays(1); // DateTime.Now.Date.AddDays(1); // // ToDate = DateTime.Now;
            }
            else
            {
                pFromDate = Convert.ToDateTime(FromDate);//.AddDays(-1);
                pToDate = Convert.ToDateTime(ToDate).AddDays(1);

            }

            // List<PickupRequestVM> lst = (from c in db.CustomerEnquiries join t1 in db.EmployeeMasters on c.CollectedEmpID equals t1.EmployeeID join t2 in db.EmployeeMasters on c.EmployeeID equals t2.EmployeeID select new PickupRequestVM { EnquiryID = c.EnquiryID, EnquiryDate = c.EnquiryDate, Consignor = c.Consignor, Consignee = c.Consignee, eCollectedBy = t1.EmployeeName, eAssignedTo = t2.EmployeeName,AWBNo=c.AWBNo }).ToList();

            //List<PickupRequestVM> lst = (from c in db.CustomerEnquiries
            //            join status in db.PickUpRequestStatus on c.PickupRequestStatusId equals status.Id
            //            join pet in db.EmployeeMasters on c.CollectedEmpID equals pet.EmployeeID into gj
            //            from subpet in gj.DefaultIfEmpty()
            //            join pet1 in db.EmployeeMasters on c.EmployeeID equals  pet1.EmployeeID into gj1
            //            from subpet1 in gj1.DefaultIfEmpty()
            //            where  c.EnquiryDate >=pFromDate &&  c.EnquiryDate <=pToDate
            //            select new PickupRequestVM { EnquiryID = c.EnquiryID, EnquiryNo=c.EnquiryNo, EnquiryDate = c.EnquiryDate, Consignor = c.Consignor, Consignee = c.Consignee, eCollectedBy =subpet.EmployeeName ?? string.Empty, eAssignedTo = subpet1.EmployeeName ?? string.Empty , AWBNo = c.AWBNo ,PickupRequestStatus=status.PickRequestStatus }).ToList();

            int Customerid = 0;
            if (Session["UserType"].ToString() == "Customer")
            {

                Customerid = Convert.ToInt32(Session["CustomerId"].ToString());

            }
            List<InScanVM> lst = (from c in db.QuickInscanMasters
                                         //join status in db.CourierStatus on c.CourierStatusID equals status.CourierStatusID
                                         join pet in db.EmployeeMasters on c.CollectedByID equals pet.EmployeeID into gj
                                         from subpet in gj.DefaultIfEmpty()
                                         join pet1 in db.EmployeeMasters on c.ReceivedByID equals pet1.EmployeeID into gj1
                                         from subpet1 in gj1.DefaultIfEmpty()
                                         where c.BranchId == branchid && (c.QuickInscanDateTime >= pFromDate && c.QuickInscanDateTime < pToDate)
                                         && c.DepotId==depotId
                                         //&& (c.CourierStatusID == pStatusId || pStatusId == 0)
                                         //&& c.IsDeleted == false
                                         //&& (c.CustomerID == Customerid || Customerid == 0)
                                         && c.Source == "Inhouse"
                                        orderby c.QuickInscanDateTime descending
                                         select new InScanVM{ QuickInscanID=c.QuickInscanID,InScanSheetNo=c.InscanSheetNumber,QuickInscanDateTime=c.QuickInscanDateTime, CollectedBy = subpet.EmployeeName ,ReceivedBy=subpet1.EmployeeName , DriverName=c.DriverName }).ToList();

            //ViewBag.FromDate = pFromDate.Date.AddDays(1).ToString("dd-MM-yyyy");
            ViewBag.FromDate = pFromDate.Date.ToString("dd-MM-yyyy");
            ViewBag.ToDate = pToDate.Date.AddDays(-1).ToString("dd-MM-yyyy");
            ViewBag.PickupRequestStatus = db.CourierStatus.Where(cc => cc.StatusTypeID == 1).ToList();
            ViewBag.StatusId = StatusId;
            return View(lst);
        }
             

        public ActionResult Details(int id)
        {
            return View();
        }

      

        public ActionResult Create(int id=0)
        {
            int BranchId= Convert.ToInt32( Session["CurrentBranchID"].ToString());
            int depotid = Convert.ToInt32(Session["CurrentDepotID"].ToString());
            int companyid= Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            //ViewBag.depot = db.tblDepots.ToList();
            ViewBag.depot = (from c in db.tblDepots where c.BranchID == BranchId select c).ToList();
            ViewBag.employee = db.EmployeeMasters.ToList();
            ViewBag.employeerec = db.EmployeeMasters.ToList();
            ViewBag.Vehicles = db.VehicleMasters.ToList();
            ViewBag.CourierService = db.CourierServices.ToList();
            if (id==0)
            {
                InScanVM vm = new InScanVM();                
                vm.QuickInscanID = 0;
                
                PickupRequestDAO _dao = new PickupRequestDAO();
                vm.InScanSheetNo = _dao.GetMaxInScanSheetNo(companyid, BranchId, "Inhouse");
                vm.QuickInscanDateTime = CommanFunctions.GetLastDayofMonth();
                vm.DepotID = depotid;
                ViewBag.EditMode ="false";
                ViewBag.Title = "InScan Local - Create";
                return View(vm);                
            }
            else
            {
                QuickInscanMaster qvm = db.QuickInscanMasters.Find(id);
                InScanVM vm = new InScanVM();
                vm.QuickInscanID = qvm.QuickInscanID;
                vm.CollectedByID = Convert.ToInt32(qvm.CollectedByID);
                vm.ReceivedByID = Convert.ToInt32(qvm.ReceivedByID);
                vm.DriverName = qvm.DriverName;
                vm.InScanSheetNo = qvm.InscanSheetNumber;
                vm.VehicleId = Convert.ToInt32(qvm.VehicleId);
                vm.DepotID = Convert.ToInt32(qvm.DepotId);
                vm.BranchId = Convert.ToInt32(qvm.BranchId);
                ViewBag.EditMode = "true";
                ViewBag.Title = "InScan Local - Modify";
                return View(vm);
            }
            
        }
        
        [HttpPost]
        public JsonResult SaveQuickInScan(InScanVM v)
        {
            //InScan inscan = new InScan();
            int UserId = Convert.ToInt32(Session["UserID"].ToString());
            int BranchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int CompanyId = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            var inscanitems = v.SelectedInScanId.Split(',');

            try
            {
                QuickInscanMaster _qinscan = new QuickInscanMaster();
                if (v.QuickInscanID > 0)
                {
                    _qinscan = db.QuickInscanMasters.Find(v.QuickInscanID);
                }
                else
                {
                    int? maxid = (from c in db.QuickInscanMasters orderby c.QuickInscanID descending select c.QuickInscanID).FirstOrDefault();

                    if (maxid == null)
                        _qinscan.QuickInscanID = 1;
                    else
                        _qinscan.QuickInscanID = Convert.ToInt32(maxid) + 1;
                    
                    _qinscan.AcFinancialYearID = yearid;

                }
                
                _qinscan.InscanSheetNumber = v.InScanSheetNo;
                _qinscan.AcCompanyId = CompanyId;
                _qinscan.ReceivedByID = v.ReceivedByID;
                _qinscan.CollectedByID = v.CollectedByID;
                _qinscan.QuickInscanDateTime = v.QuickInscanDateTime;
                _qinscan.VehicleId = v.VehicleId;
                _qinscan.DriverName = v.DriverName;
                _qinscan.BranchId = BranchId;
                _qinscan.DepotId = v.DepotID;
                _qinscan.UserId = UserId;
                _qinscan.Source = "Inhouse";
                if (v.QuickInscanID > 0)
                {
                    db.Entry(_qinscan).State= EntityState.Modified;
                    var removeinscanitems = v.RemovedInScanId.Split(',');

                    
                    foreach (var _item in removeinscanitems)
                    {
                        int _inscanid = Convert.ToInt32(_item);

                        var _inscan = db.InScanMasters.Find(_inscanid);
                        _inscan.QuickInscanID = null;
                        _inscan.PickedUpEmpID = null;
                        _inscan.DepotReceivedBy = null;
                        //_inscan.TransactionDate = v.QuickInscanDateTime;
                        _inscan.VehicleTypeId = null;
                        _inscan.StatusTypeId = db.tblStatusTypes.Where(cc => cc.Name == "PICKUP REQUEST").First().ID;
                        _inscan.CourierStatusID = db.CourierStatus.Where(cc => cc.StatusTypeID == _inscan.StatusTypeId && cc.CourierStatus == "Assigned For Collections").FirstOrDefault().CourierStatusID;
                        db.Entry(_inscan).State = EntityState.Modified;
                        db.SaveChanges();

                        //updateing awbstaus table for tracking
                        AWBTrackStatu _awbstatus = new AWBTrackStatu();
                        int? id = (from c in db.AWBTrackStatus orderby c.AWBTrackStatusId descending select c.AWBTrackStatusId).FirstOrDefault();

                        if (id == null)
                            id = 1;
                        else
                            id = id + 1;

                        _awbstatus.AWBTrackStatusId = Convert.ToInt32(id);
                        _awbstatus.AWBNo = _inscan.AWBNo;
                        _awbstatus.EntryDate = DateTime.Now;
                        _awbstatus.InScanId = _inscan.InScanID;
                        _awbstatus.StatusTypeId = Convert.ToInt32(_inscan.StatusTypeId);
                        _awbstatus.CourierStatusId = Convert.ToInt32(_inscan.CourierStatusID);
                        _awbstatus.ShipmentStatus = db.tblStatusTypes.Find(_inscan.StatusTypeId).Name;
                        _awbstatus.CourierStatus = db.CourierStatus.Find(_inscan.CourierStatusID).CourierStatus;
                        _awbstatus.UserId = UserId;

                        db.AWBTrackStatus.Add(_awbstatus);
                        db.SaveChanges();
                    }
                }
                else
                {
                    db.QuickInscanMasters.Add(_qinscan);
                    db.SaveChanges();
                }

                foreach (var item in inscanitems)
                {
                    int _inscanid =Convert.ToInt32(item);
                    InScanMaster _inscan = db.InScanMasters.Find(_inscanid);
                    _inscan.QuickInscanID = _qinscan.QuickInscanID;
                    _inscan.PickedUpEmpID = v.CollectedByID;
                    _inscan.DepotReceivedBy = v.ReceivedByID;
                    //_inscan.TransactionDate = v.QuickInscanDateTime;
                    _inscan.VehicleTypeId = v.VehicleId;
                    _inscan.StatusTypeId = db.tblStatusTypes.Where(cc => cc.Name == "INSCAN").First().ID;
                    _inscan.CourierStatusID = db.CourierStatus.Where(cc => cc.StatusTypeID == _inscan.StatusTypeId && cc.CourierStatus == "Received at Origin Facility").FirstOrDefault().CourierStatusID;
                    db.Entry(_inscan).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
                //TempData["SuccessMsg"] = "You have successfully Saved InScan Items.";             
           
                return Json(new { status = "ok", message = "You have successfully Saved InScan Items.!" } , JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = "Failed", message = ex.Message }, JsonRequestBehavior.AllowGet);
                //return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAWB(int id)
        {

            List<AWBList> obj = new List<AWBList>();
            
            QuickInscanMaster _qinscanvm = db.QuickInscanMasters.Where(cc => cc.QuickInscanID == id).FirstOrDefault();

            //InScanVM _qinscanvm = new InScanVM();

            //_qinscanvm = (from _qmaster in db.QuickInscanMasters
            //              where _qmaster.InscanSheetNumber == id
            //              select new InScanVM { QuickInscanID=_qmaster.QuickInscanID,QuickInscanDateTime=_qmaster.QuickInscanDateTime,BranchId=_qmaster.BranchId,DepotID=_qmaster.DepotId,VehicleId=_qmaster.VehicleId,DriverName = _qmaster.DriverName });.first

            if (_qinscanvm != null)
            { 
                obj = (from _qmaster in db.QuickInscanMasters
                            join _inscan in db.InScanMasters on _qmaster.QuickInscanID equals _inscan.QuickInscanID
                            //join _inscan in db.InScanMasters on _qdetail.InScanId equals _inscan.InScanID
                            where _qmaster.QuickInscanID == id
                            orderby _inscan.AWBNo descending
                            select new AWBList { InScanId = _inscan.InScanID, AWB = _inscan.AWBNo, Origin = _inscan.ConsignorCountryName, Destination = _inscan.ConsigneeCountryName }).ToList();

              return Json(new { status = "ok", masterdata=_qinscanvm, data = obj, message = "Data Found" }, JsonRequestBehavior.AllowGet);
        }
        else
        {
           return Json(new { status = "failed",masterdata= _qinscanvm, data = obj, message = "Data Not Found" }, JsonRequestBehavior.AllowGet);
        }


            //List<AWBList> obj = new List<AWBList>();
            //var lst = (from c in db.CustomerEnquiries where c.CollectedEmpID == id select c).ToList();

            //foreach (var item in lst)
            //{
            //    obj.Add(new AWBList { AWB=item.AWBNo});

            //}
            //return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAWBDetail(string id,int? collectedby)
        {
            if (collectedby == null)
                collectedby = 0;
            AWBList obj = new AWBList();
            var lst = (from c in db.InScanMasters where c.AWBNo == id &&  (c.PickedUpEmpID==collectedby || collectedby==0) select c).FirstOrDefault();
            if (lst==null)
            {
                return Json(new { status="failed", data = obj, message = "AWB No. Not found"}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (lst.QuickInscanID == null)
                {
                    obj.Origin = lst.ConsignorCountryName;
                    obj.Destination = lst.ConsigneeCountryName;
                    obj.AWB = lst.AWBNo;
                    obj.InScanId = lst.InScanID;

                    return Json(new { status = "ok", data = obj, message = "AWB Not.found" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { status = "failed", data = obj, message = "InScan already Done!" }, JsonRequestBehavior.AllowGet);
                }
            }            
            
        }
        public class AWBList
        {            
            public int InScanId { get; set; }
            public string AWB { get; set; }
            public string Origin { get; set; }            
            public string Destination { get; set; }
        }
        //
        // GET: /InScan/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /InScan/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /InScan/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /InScan/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
