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
    public class InBoundController : Controller
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
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date; // DateTime.Now.Date;//.AddDays(-1); // FromDate = DateTime.Now;
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
                                         join pet in db.AgentMasters on c.AgentID equals pet.AgentID into gj
                                         from subpet in gj.DefaultIfEmpty()
                                         join pet1 in db.EmployeeMasters on c.ReceivedByID equals pet1.EmployeeID into gj1
                                         from subpet1 in gj1.DefaultIfEmpty()
                                         where c.BranchId == branchid && (c.QuickInscanDateTime >= pFromDate && c.QuickInscanDateTime < pToDate)
                                          && c.DepotId == depotId
                                         //&& (c.CourierStatusID == pStatusId || pStatusId == 0)
                                         //&& c.IsDeleted == false
                                         //&& (c.CustomerID == Customerid || Customerid == 0)
                                         && c.Source=="Import"
                                         orderby c.QuickInscanDateTime descending
                                         select new InScanVM{ QuickInscanID=c.QuickInscanID,InScanSheetNo=c.InscanSheetNumber,QuickInscanDateTime=c.QuickInscanDateTime, AgentName= subpet.Name ,ReceivedBy=subpet1.EmployeeName , DriverName=c.DriverName }).ToList();

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
            ViewBag.Agents = db.AgentMasters.ToList();
            ViewBag.CourierService = db.CourierServices.ToList();
            if (id==0)
            {
                ViewBag.Title = "InScan Import -Create";
                   InScanVM vm = new InScanVM();                
                vm.QuickInscanID = 0;
                
                PickupRequestDAO _dao = new PickupRequestDAO();
                vm.InScanSheetNo = _dao.GetMaxInScanSheetNo(companyid,BranchId,"Import");
                vm.DepotID = depotid;
                ViewBag.EditMode ="false";
                return View(vm);                
            }
            else
            {
                QuickInscanMaster qvm = db.QuickInscanMasters.Find(id);
                InScanVM vm = new InScanVM();
                vm.QuickInscanID = qvm.QuickInscanID;
                vm.AgentID = Convert.ToInt32(qvm.AgentID);
                vm.ReceivedByID = Convert.ToInt32(qvm.ReceivedByID);
                vm.DriverName = qvm.DriverName;
                vm.InScanSheetNo = qvm.InscanSheetNumber;
                vm.VehicleId = Convert.ToInt32(qvm.VehicleId);
                vm.DepotID = Convert.ToInt32(qvm.DepotId);
                vm.BranchId = Convert.ToInt32(qvm.BranchId);
                ViewBag.EditMode = "true";
                ViewBag.Title = "InScan Import - Modify";
                return View(vm);
            }
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InScanVM v)
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
                _qinscan.AgentID = v.AgentID;
                //_qinscan.CollectedByID = v.CollectedByID;
                _qinscan.QuickInscanDateTime = v.QuickInscanDateTime;
                _qinscan.VehicleId = v.VehicleId;
                _qinscan.DriverName = v.DriverName;
                _qinscan.BranchId = BranchId;
                _qinscan.DepotId = v.DepotID;
                _qinscan.UserId = UserId;
                _qinscan.Source = "Import";
                if (v.QuickInscanID > 0)
                {
                    db.Entry(_qinscan).State = EntityState.Modified;
                    if (v.RemovedInScanId!=null)
                    { 
                    var removeinscanitems = v.RemovedInScanId.Split(',');

                        foreach (var _item in removeinscanitems)
                        {
                            int _inscanid = Convert.ToInt32(_item);

                            var _inscan = db.ImportShipmentDetails.Find(_inscanid);
                            _inscan.QuickInscanID = null;

                            var couriercstatus = db.CourierStatus.Where(c => c.CourierStatus == "Export Manifest Prepared").FirstOrDefault();
                            if (couriercstatus != null)
                            {
                                _inscan.CourierStatusID = couriercstatus.CourierStatusID;
                            }
                            var statustype = db.tblStatusTypes.Where(c => c.Name == "READY TO EXPORT").FirstOrDefault();
                            if (statustype != null)
                            {
                                _inscan.StatusTypeId = statustype.ID;
                            }

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
                            _awbstatus.AWBNo = _inscan.AWB;
                            _awbstatus.EntryDate = DateTime.Now;
                            _awbstatus.ShipmentDetailID = _inscan.ShipmentDetailID;
                            _awbstatus.StatusTypeId = Convert.ToInt32(_inscan.StatusTypeId);
                            _awbstatus.CourierStatusId = Convert.ToInt32(_inscan.CourierStatusID);
                            _awbstatus.ShipmentStatus = db.tblStatusTypes.Find(_inscan.StatusTypeId).Name;
                            _awbstatus.CourierStatus = db.CourierStatus.Find(_inscan.CourierStatusID).CourierStatus;
                            _awbstatus.UserId = UserId;

                            db.AWBTrackStatus.Add(_awbstatus);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    db.QuickInscanMasters.Add(_qinscan);
                    db.SaveChanges();
                }
                if (v.SelectedInScanId != null)
                {
                    foreach (var item in inscanitems)
                    {
                        int _inscanid = Convert.ToInt32(item);
                        ImportShipmentDetail _inscan = db.ImportShipmentDetails.Find(_inscanid);
                        _inscan.QuickInscanID = _qinscan.QuickInscanID;
                        _inscan.StatusTypeId = db.tblStatusTypes.Where(cc => cc.Name == "INSCAN").First().ID;
                        _inscan.CourierStatusID = db.CourierStatus.Where(cc => cc.StatusTypeID == _inscan.StatusTypeId && cc.CourierStatus == "Received at Origin Facility").FirstOrDefault().CourierStatusID;
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
                        _awbstatus.AWBNo = _inscan.AWB;
                        _awbstatus.EntryDate = DateTime.Now;
                        _awbstatus.ShipmentDetailID = _inscan.ShipmentDetailID;
                        _awbstatus.StatusTypeId = Convert.ToInt32(_inscan.StatusTypeId);
                        _awbstatus.CourierStatusId = Convert.ToInt32(_inscan.CourierStatusID);
                        _awbstatus.ShipmentStatus = db.tblStatusTypes.Find(_inscan.StatusTypeId).Name;
                        _awbstatus.CourierStatus = db.CourierStatus.Find(_inscan.CourierStatusID).CourierStatus;
                        _awbstatus.UserId = UserId;

                        db.AWBTrackStatus.Add(_awbstatus);
                        db.SaveChanges();
                    }
                }
                TempData["SuccessMsg"] = "You have successfully Saved InScan Import Items.";             
                return RedirectToAction("Index");
                

              //  return Json(new { status = "ok", message = "You have successfully Saved InScan Items.!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["WarningMsg"] = ex.Message;
                ViewBag.depot = (from c in db.tblDepots where c.BranchID == BranchId select c).ToList();
                ViewBag.employee = db.EmployeeMasters.ToList();
                ViewBag.employeerec = db.EmployeeMasters.ToList();
                ViewBag.Vehicles = db.VehicleMasters.ToList();
                ViewBag.Agents = db.AgentMasters.ToList();
                ViewBag.CourierService = db.CourierServices.ToList();
                return View(v);
                //return Json(new { status = "Failed", message = ex.Message }, JsonRequestBehavior.AllowGet);
                //return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveQuickInScan(InScanVM v)
        {
            //InScan inscan = new InScan();
            int UserId = Convert.ToInt32(Session["UserID"].ToString());
            int BranchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int CompanyId = Convert.ToInt32(Session["CurrentCompanyID"].ToString()); 

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
                }
                
                _qinscan.InscanSheetNumber = v.InScanSheetNo;
                _qinscan.AcCompanyId = CompanyId;
                _qinscan.ReceivedByID = v.ReceivedByID;
                _qinscan.AgentID = v.AgentID;
                //_qinscan.CollectedByID = v.CollectedByID;
                _qinscan.QuickInscanDateTime = v.QuickInscanDateTime;
                _qinscan.VehicleId = v.VehicleId;
                _qinscan.DriverName = v.DriverName;
                _qinscan.BranchId = BranchId;
                _qinscan.DepotId = v.DepotID;
                _qinscan.UserId = UserId;
                _qinscan.Source = "Import";
                if (v.QuickInscanID > 0)
                {
                    db.Entry(_qinscan).State= EntityState.Modified;
                    var removeinscanitems = v.RemovedInScanId.Split(',');

                    
                    foreach (var _item in removeinscanitems)
                    {
                        int _inscanid = Convert.ToInt32(_item);

                        var _inscan = db.ImportShipmentDetails.Find(_inscanid);
                        _inscan.QuickInscanID = null;

                        var couriercstatus = db.CourierStatus.Where(c => c.CourierStatus == "Export Manifest Prepared").FirstOrDefault();
                        if (couriercstatus != null)
                        {
                            _inscan.CourierStatusID = couriercstatus.CourierStatusID;
                        }
                        var statustype = db.tblStatusTypes.Where(c => c.Name == "READY TO EXPORT").FirstOrDefault();
                        if (statustype != null)
                        {
                            _inscan.StatusTypeId = statustype.ID;
                        }
                        
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
                        _awbstatus.AWBNo = _inscan.AWB;
                        _awbstatus.EntryDate = DateTime.Now;
                        _awbstatus.ShipmentDetailID = _inscan.ShipmentDetailID;
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
                    ImportShipmentDetail _inscan = db.ImportShipmentDetails.Find(_inscanid);
                    _inscan.QuickInscanID = _qinscan.QuickInscanID;                    
                    _inscan.StatusTypeId = db.tblStatusTypes.Where(cc => cc.Name == "INSCAN").First().ID;
                    _inscan.CourierStatusID = db.CourierStatus.Where(cc => cc.StatusTypeID == _inscan.StatusTypeId && cc.CourierStatus == "Received at Origin Facility").FirstOrDefault().CourierStatusID;
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
                    _awbstatus.AWBNo = _inscan.AWB;
                    _awbstatus.EntryDate = DateTime.Now;
                    _awbstatus.ShipmentDetailID = _inscan.ShipmentDetailID;
                    _awbstatus.StatusTypeId = Convert.ToInt32(_inscan.StatusTypeId);
                    _awbstatus.CourierStatusId = Convert.ToInt32(_inscan.CourierStatusID);
                    _awbstatus.ShipmentStatus = db.tblStatusTypes.Find(_inscan.StatusTypeId).Name;
                    _awbstatus.CourierStatus = db.CourierStatus.Find(_inscan.CourierStatusID).CourierStatus;
                    _awbstatus.UserId = UserId;

                    db.AWBTrackStatus.Add(_awbstatus);
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

            List<InBoundAWBList> obj = new List<InBoundAWBList>();
            
            QuickInscanMaster _qinscanvm = db.QuickInscanMasters.Where(cc => cc.QuickInscanID == id).FirstOrDefault();

            //InScanVM _qinscanvm = new InScanVM();

            //_qinscanvm = (from _qmaster in db.QuickInscanMasters
            //              where _qmaster.InscanSheetNumber == id
            //              select new InScanVM { QuickInscanID=_qmaster.QuickInscanID,QuickInscanDateTime=_qmaster.QuickInscanDateTime,BranchId=_qmaster.BranchId,DepotID=_qmaster.DepotId,VehicleId=_qmaster.VehicleId,DriverName = _qmaster.DriverName });.first

            if (_qinscanvm != null)
            { 
                obj = (from _qmaster in db.QuickInscanMasters
                            join _shipdetail in db.ImportShipmentDetails on _qmaster.QuickInscanID equals _shipdetail.QuickInscanID     
                            join  _shipment in db.ImportShipments on  _shipdetail.ImportID equals _shipment.ID
                            where _qmaster.QuickInscanID == id
                            orderby _shipdetail.AWB descending
                            select new InBoundAWBList { ShipmentDetailID = _shipdetail.ShipmentDetailID, AWB = _shipdetail.AWB, 
                                OriginCity = _shipment.ConsignorCityName, OriginCountry =_shipment.ConsignorCountryName, DestinationCity  = _shipdetail.DestinationCity, DestinationCountry = _shipdetail.DestinationCountry,Shipper=_shipdetail.Shipper, Receiver=_shipdetail.Receiver   }).ToList();

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
        public JsonResult GetAWBDetail(string id)
        {
            InBoundAWBList obj = new InBoundAWBList();
            //var lst = (from c in db.InScanMasters where c.AWBNo == id select c).FirstOrDefault();
            var couriercstatus = db.CourierStatus.Where(c => c.CourierStatus == "Export Manifest Prepared").FirstOrDefault();
            int CourierStatusId= 0;
            if (couriercstatus != null)
            {
                CourierStatusId = couriercstatus.CourierStatusID;
            }
            else
            {
                return Json(new { status = "failed", data = obj, message = "Courier Status Not found" }, JsonRequestBehavior.AllowGet);
            }

            var lst = (from c in db.ImportShipmentDetails join i in db.ImportShipments on c.ImportID equals i.ID
                       where c.AWB == id &&  c.CourierStatusID == CourierStatusId
                       select new { i.ConsignorName, i.ConsignorCountryName, i.ConsignorCityName, c.DestinationCity, c.DestinationCountry, c.AWB, c.ShipmentDetailID, c.Shipper, c.Receiver }).FirstOrDefault();  //forwarded to agent status only
            if (lst==null)
            {
                return Json(new { status="failed", data = obj, message = "AWB No. Not found"}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                 obj.ConsignorName = lst.ConsignorName;
                 obj.OriginCountry = lst.ConsignorCountryName;
                 obj.OriginCity = lst.ConsignorCityName;
                 obj.DestinationCity = lst.DestinationCity;
                 obj.DestinationCountry = lst.DestinationCountry;
                obj.AWB = lst.AWB;
                 obj.Receiver = lst.Receiver;
                 obj.Shipper = lst.Shipper;
                 obj.ShipmentDetailID = lst.ShipmentDetailID;

                return Json(new { status = "ok", data = obj, message = "AWB Number found" }, JsonRequestBehavior.AllowGet);

                
            }            
            
        }
        public class InBoundAWBList
        {            
            public int ShipmentDetailID { get; set; }
            public string ConsignorName { get; set; }
            public string AWB { get; set; }
            public string OriginCountry { get; set; }
            public string OriginCity { get; set; }

            public string DestinationCountry { get; set; }
            public string DestinationCity { get; set; }

            public string Receiver { get; set; }
            public string Shipper { get; set; }
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
