using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using System.Data.Entity;
//using OfficeOpenXml.FormulaParsing.LexicalAnalysis.TokenSeparatorHandlers;

namespace CMSV2.Controllers
{
    [SessionExpire]
    public class HoldController : Controller
    {
        Entities1 db = new Entities1();

        public ActionResult Index(int? StatusId, string FromDate, string ToDate)
        {
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
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date;//.AddDays(-1); // FromDate = DateTime.Now;
                pToDate = DateTime.Now.Date.AddDays(1); // // ToDate = DateTime.Now;
            }
            else
            {
                pFromDate = Convert.ToDateTime(FromDate);//.AddDays(-1);
                pToDate = Convert.ToDateTime(ToDate).AddDays(1);

            }
            List<HoldVM> lst = new List<HoldVM>();

            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());

            //List<HoldVM> ls = (from c in db.InScans join e in db.CountryMasters on c.ConsignorCityID equals e.CountryID join t in db.CityMasters on c.ConsigneeCityID equals t.CityID join x in db.CountryMasters on c.ConsigneeCountryID equals x.CountryID where c.HeldBy == null select new HoldVM { AWBNo = c.AWBNo, InScanID = c.InScanID, date = c.InScanDate, CollectedBy = c.CollectedBy, OriginName = e.CountryName, Consignor = c.Consignor, Consignee = c.Consignee, ConsigneeCountry = t.City, DestinationName = x.CountryName}).ToList();

            int statustypeid = db.tblStatusTypes.Where(cc => cc.Name == "HOLD").FirstOrDefault().ID;
            int holdsatusid = db.CourierStatus.Where(cc => cc.StatusTypeID == statustypeid && cc.CourierStatus == "OnHold").FirstOrDefault().CourierStatusID;
            int releasestatusid = db.CourierStatus.Where(cc => cc.StatusTypeID == statustypeid && cc.CourierStatus == "Released").FirstOrDefault().CourierStatusID;

            lst = (from c in db.InScanMasters  join p in db.tblPaymentModes on c.PaymentModeId equals p.ID 
                        join cs in db.CourierStatus on c.CourierStatusID equals cs.CourierStatusID
                        join hr in db.HoldReleases on c.InScanID equals hr.InScanId
                        join em in db.EmployeeMasters on c.PickedUpEmpID equals em.EmployeeID
                   where c.BranchID == branchid && c.StatusTypeId == statustypeid //&& c.CourierStatusID == courisestatusid
                    && (hr.EntryDate >= pFromDate && hr.EntryDate < pToDate) && (c.CourierStatusID == pStatusId || pStatusId == 0)                    
                    && ((hr.ActionType=="Hold" && c.CourierStatusID==holdsatusid) || (hr.ActionType=="Release" && c.CourierStatusID==releasestatusid))
                   orderby hr.EntryDate descending 
                   select new HoldVM {InScanID=c.InScanID,AWBNo=c.AWBNo, date=hr.EntryDate,
                       TransactionnDate = c.TransactionDate,CollectedByName=em.EmployeeName,Consignor=c.Consignor,OriginCountry=c.ConsignorCountryName,CourierStatus=cs.CourierStatus,
                            Weight=c.Weight,Pieces=c.Pieces,CourierCharges=c.CourierCharge,Consignee=c.Consignee,ConsigneeCountry=c.ConsigneeCountryName,StatusPaymentMOde=p.PaymentModeText }).ToList();

            ViewBag.FromDate = pFromDate.Date.ToString("dd-MM-yyyy");
            ViewBag.ToDate = pToDate.Date.AddDays(-1).ToString("dd-MM-yyyy");
            //foreach (var item in data)
            //{
            //    HoldVM obj = new HoldVM();
            //    if (item.HeldBy == null)
            //    {
            //        obj.InScanID = item.InScanID;
            //        obj.AWBNo = item.AWBNo;
            //        obj.date = item.TransactionDate;
            //        obj.CollectedBy = item.PickedUpEmpID;
            //        obj.StatedWeight = item.StatedWeight;
            //        obj.Pieces = item.Pieces;
            //        obj.CourierCharges = Convert.ToDecimal(item.CourierCharge);
            //        //obj.OriginID = item.ConsignorCountryID.Value;
            //        obj.Consignee = item.Consignee;
            //        obj.DestinationID = item.ConsigneeCountryName;
            //        obj.StatusPaymentMOde = db.tblStatusTypes
            //        obj.Consignor = item.Consignor;

            //        obj.HeldBy = item.HeldBy;
            //        obj.HeldOn = item.HeldOn;
            //        obj.HeldResoan = item.HeldReason;
            //    }
            //    lst.Add(obj);
            //}

            ViewBag.CourierStatusList = db.CourierStatus.Where(cc => cc.StatusType =="HOLD").ToList();
            ViewBag.ReleaseBy = db.EmployeeMasters.ToList();
            return View(lst);
        }


        public ActionResult Create(int id=0)
        {
            ViewBag.HeldBy = db.EmployeeMasters.ToList();
            HoldVM vm = new HoldVM();
            if (id>0)
            {
                vm = (from c in db.InScanMasters
                       join p in db.tblPaymentModes on c.PaymentModeId equals p.ID
                       join cs in db.CourierStatus on c.CourierStatusID equals cs.CourierStatusID
                       join hr in db.HoldReleases on c.InScanID equals hr.InScanId
                       join em in db.EmployeeMasters on c.PickedUpEmpID equals em.EmployeeID
                       where c.InScanID==id 
                       select new HoldVM
                       {
                           InScanID = c.InScanID,
                           AWBNo = c.AWBNo,
                           date = hr.EntryDate,
                           TransactionnDate = c.TransactionDate,
                           CollectedByName = em.EmployeeName,
                           Consignor = c.Consignor,
                           OriginCountry = c.ConsignorCountryName,
                           CourierStatus = cs.CourierStatus,
                           Weight = c.Weight,
                           Pieces = c.Pieces,
                           CourierCharges = c.CourierCharge,
                           Consignee = c.Consignee,
                           ConsigneeCountry = c.ConsigneeCountryName,
                           StatusPaymentMOde = p.PaymentModeText
                       }).FirstOrDefault();

                vm.Action = "Edit";
                if (vm.CourierStatus == "OnHold")
                {
                    vm.ActionType = "Hold";
                    var releasedata = db.HoldReleases.Where(cc => cc.InScanId == id && cc.ActionType == "Hold").FirstOrDefault();
                    if (releasedata != null)
                    {
                        vm.HeldOn = releasedata.EntryDate;
                        vm.HeldBy = releasedata.EmployeeId;
                        vm.HeldResoan = releasedata.Remarks;
                    }
                    ViewBag.Title = "AWB Hold -  Modify";
                }
                else
                {
                    vm.ActionType = "Release";
                    var releasedata = db.HoldReleases.Where(cc => cc.InScanId == id && cc.ActionType == "Release").FirstOrDefault();
                    if (releasedata != null)
                    {
                        vm.HeldOn = releasedata.EntryDate;
                        vm.HeldBy = releasedata.EmployeeId;
                        vm.HeldResoan = releasedata.Remarks;
                    }
                    ViewBag.Title = "Air WayBill Released -  Modify";
                }
            }
            else
            {
                vm.Action = "Create";
                vm.InScanID = 0;
                ViewBag.Title = "Air WayBill Hold - Create";
            }
            
            return View(vm);

        }
        public ActionResult Edit(int id)
        {
            HoldVM obj = new HoldVM();



            var item = (from d in db.InScans where d.InScanID == id select d).FirstOrDefault();


            if (item == null)
            {
                return HttpNotFound();
            }
            else
            {
                obj.OriginCountry = (from c in db.CountryMasters where c.CountryID == item.ConsignorCountryID select c.CountryName).FirstOrDefault();

                obj.ConsigneeCountry = (from c in db.CountryMasters where c.CountryID == item.ConsigneeCountryID select c.CountryName).FirstOrDefault();



                obj.InScanID = item.InScanID;
                obj.AWBNo = item.AWBNo;
                obj.date = item.InScanDate;
                obj.CollectedBy = item.CollectedBy;
                obj.Weight = item.Weight;
                obj.Pieces = item.Pieces;
                obj.CourierCharges = item.CourierCharge;
                obj.OriginID = item.ConsigneeCountryID.Value;
                obj.Consignee = item.Consignee;
                obj.DestinationID = item.ConsigneeCountryID;
                obj.StatusPaymentMOde = item.StatusPaymentMode;
                obj.Consignor = item.Consignor;

                //obj.HeldBy = item.HeldBy;
                //obj.HeldOn = item.HeldOn;
                obj.HeldResoan = item.HeldReason;
            }

            return View(obj);

        }

        [HttpPost]
        public ActionResult Create(HoldVM item)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int UserId = Convert.ToInt32(Session["UserID"].ToString());
            if (item.Action == "Create")
            {
                var inscan = db.InScanMasters.Where(itm => itm.InScanID == item.InScanID).FirstOrDefault();

                HoldRelease v = new HoldRelease();
                v.InScanId = item.InScanID;
                v.EntryDate = item.HeldOn;
                v.EmployeeId = item.HeldBy;
                v.Remarks = item.HeldResoan;
                v.ActionType = "Hold";
                v.BranchId = branchid;
                db.HoldReleases.Add(v);
                db.SaveChanges();

                //updating status to inscan
                inscan.StatusTypeId = db.tblStatusTypes.Where(cc => cc.Name == "HOLD").First().ID;
                inscan.CourierStatusID = db.CourierStatus.Where(cc => cc.StatusTypeID == inscan.StatusTypeId && cc.CourierStatus == "OnHold").FirstOrDefault().CourierStatusID;
                // db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                //updateing awbstaus table for tracking
                AWBTrackStatu _awbstatus = new AWBTrackStatu();
                int? id = (from c in db.AWBTrackStatus orderby c.AWBTrackStatusId descending select c.AWBTrackStatusId).FirstOrDefault();

                if (id == null)
                    id = 1;
                else
                    id = id + 1;

                _awbstatus.AWBTrackStatusId = Convert.ToInt32(id);
                _awbstatus.AWBNo = inscan.AWBNo;
                _awbstatus.EntryDate = DateTime.Now;
                _awbstatus.InScanId = inscan.InScanID;
                _awbstatus.StatusTypeId = Convert.ToInt32(inscan.StatusTypeId);
                _awbstatus.CourierStatusId = Convert.ToInt32(inscan.CourierStatusID);
                _awbstatus.ShipmentStatus = db.tblStatusTypes.Find(inscan.StatusTypeId).Name;
                _awbstatus.CourierStatus = db.CourierStatus.Find(inscan.CourierStatusID).CourierStatus;
                _awbstatus.UserId = UserId;

                db.AWBTrackStatus.Add(_awbstatus);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added AWB to Hold Status";
            }

            else
            {
                HoldRelease v = db.HoldReleases.Where(cc => cc.InScanId == item.InScanID && cc.ActionType == item.ActionType).First();
                
                v.EntryDate = item.HeldOn;
                v.EmployeeId = item.HeldBy;
                v.Remarks = item.HeldResoan;
                db.Entry(v).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated AWB to " + item.ActionType + " Entry";

            }
            
            return RedirectToAction("Index");


        }


        //public ActionResult IndexRelease()
        //{
        //    List<RealeseHoldVM> lst = new List<RealeseHoldVM>();



        //    //List<RealeseHoldVM> ls = (from c in db.InScans join e in db.CountryMasters on c.ConsignorCityID equals e.CountryID join t in db.CityMasters on c.ConsigneeCityID equals t.CityID join x in db.CountryMasters on c.ConsigneeCountryID equals x.CountryID where c.HeldBy != null select new RealeseHoldVM { AWBNo = c.AWBNo, InScanID = c.InScanID, date = c.InScanDate, CollectedBy = c.CollectedBy, OriginName = e.CountryName, Consignor = c.Consignor, Consignee = c.Consignee, ConsigneeCountry = t.City, DestinationName=x.CountryName }).ToList();



        //    var data = db.InScans.ToList();

        //    foreach (var item in data)
        //    {
        //        RealeseHoldVM obj = new RealeseHoldVM();
        //        if (item.HeldBy != null)
        //        {
        //            obj.InScanID = item.InScanID;
        //            obj.AWBNo = item.AWBNo;
        //            obj.date = item.InScanDate;
        //            obj.CollectedBy = item.CollectedBy;
        //            obj.StatedWeight = item.StatedWeight;
        //            obj.Pieces = item.Pieces;
        //            obj.CourierCharges = item.CourierCharge;
        //            //obj.OriginID = item.ConsignorCountryID.Value;
        //            obj.Consignee = item.Consignee;
        //            obj.DestinationID = item.ConsigneeCountryID;
        //            obj.StatusPaymentMOde = item.StatusPaymentMode;
        //            obj.Consignor = item.Consignor;

        //            obj.ReleaseBy = item.ReleasedBy;
        //            obj.ReleaseOn = item.ReleasedOn;
        //            obj.ReleaseResoan = item.ReleasedReason;

        //            lst.Add(obj);
        //        }




        //    }
         
        //    return View(lst);

        //}

        //public ActionResult EditRealese(int id)
        //{
        //    RealeseHoldVM obj = new RealeseHoldVM();

        //    var item = (from d in db.InScans where d.InScanID == id select d).FirstOrDefault();


        //    if (item == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    else
        //    {
        //        obj.OriginCountry = (from c in db.CountryMasters where c.CountryID == item.ConsignorCountryID select c.CountryName).FirstOrDefault();

        //        obj.ConsigneeCountry = (from c in db.CountryMasters where c.CountryID == item.ConsigneeCountryID select c.CountryName).FirstOrDefault();



        //        obj.InScanID = item.InScanID;
        //        obj.AWBNo = item.AWBNo;
        //        obj.date = item.InScanDate;
        //        obj.CollectedBy = item.CollectedBy;
        //        obj.StatedWeight = item.StatedWeight;
        //        obj.Pieces = item.Pieces;
        //        obj.CourierCharges = item.CourierCharge;
        //        obj.OriginID = item.ConsignorCountryID.Value;
        //        obj.Consignee = item.Consignee;
        //        obj.DestinationID = item.ConsigneeCountryID;
        //        obj.StatusPaymentMOde = item.StatusPaymentMode;
        //        obj.Consignor = item.Consignor;

        //        obj.ReleaseBy = item.ReleasedBy;
        //        obj.ReleaseOn = item.ReleasedOn;
        //        obj.ReleaseResoan = item.ReleasedReason;
        //    }

        //    return View(obj);

        //}


        [HttpPost]

        public JsonResult SaveReleaseStatus(RealeseHoldVM item)
        {                        
            int UserId = Convert.ToInt32(Session["UserID"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            try
            {
                var inscan = db.InScanMasters.Where(itm => itm.InScanID == item.InScanID).FirstOrDefault();


                HoldRelease v = new HoldRelease();
                v.InScanId = item.InScanID;
                v.EntryDate = item.ReleaseOn;
                v.EmployeeId = item.ReleaseBy;
                v.Remarks = item.ReleaseReason;
                v.ActionType = "Release";
                v.BranchId = branchid;
                db.HoldReleases.Add(v);
                db.SaveChanges();

                //updating status to inscan
                inscan.StatusTypeId = db.tblStatusTypes.Where(cc => cc.Name == "HOLD").First().ID;
                inscan.CourierStatusID = db.CourierStatus.Where(cc => cc.StatusTypeID == inscan.StatusTypeId && cc.CourierStatus == "Released").FirstOrDefault().CourierStatusID;
                // db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                //updateing awbstaus table for tracking
                AWBTrackStatu _awbstatus = new AWBTrackStatu();
                int? id = (from c in db.AWBTrackStatus orderby c.AWBTrackStatusId descending select c.AWBTrackStatusId).FirstOrDefault();

                if (id == null)
                    id = 1;
                else
                    id = id + 1;

                _awbstatus.AWBTrackStatusId = Convert.ToInt32(id);
                _awbstatus.AWBNo = inscan.AWBNo;
                _awbstatus.EntryDate = DateTime.Now;
                _awbstatus.InScanId = inscan.InScanID;
                _awbstatus.StatusTypeId = Convert.ToInt32(inscan.StatusTypeId);
                _awbstatus.CourierStatusId = Convert.ToInt32(inscan.CourierStatusID);
                _awbstatus.ShipmentStatus = db.tblStatusTypes.Find(inscan.StatusTypeId).Name;
                _awbstatus.CourierStatus = db.CourierStatus.Find(inscan.CourierStatusID).CourierStatus;
                _awbstatus.UserId = UserId;

                db.AWBTrackStatus.Add(_awbstatus);
                db.SaveChanges();
                return Json(new { status = "ok", message = "AWB Item Released Successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = "Failed", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
                        


        }


        public JsonResult GetAWBData(string id)
        {
            //Received at Origin Facility
            //var l = (from c in db.InScans where c.InScanDate >= s  && c.InScanDate <= e select c).ToList();
            int courierstatusid = db.CourierStatus.Where(cc => cc.CourierStatus == "Received at Origin Facility").FirstOrDefault().CourierStatusID;

            var l = (from c in db.InScanMasters where c.AWBNo == id && c.DRSID == null && c.CourierStatusID == courierstatusid select c).FirstOrDefault();

            if (l != null)
            {
                HoldVM obj = new HoldVM();
                if (l != null)
                {

                    obj.AWBNo = l.AWBNo;
                    obj.InScanID = l.InScanID;
                    obj.Consignor = l.Consignor;
                    obj.Consignee = l.Consignee;
                    obj.CollectedBy = l.PickedUpEmpID;
                    obj.date = l.TransactionDate;
                    obj.Weight = l.Weight;
                    obj.Pieces = l.Pieces;
                    obj.OriginCountry = l.ConsignorCountryName;
                    obj.CourierCharges = l.CourierCharge;
                    obj.StatusPaymentMOde = db.tblPaymentModes.Find(l.PaymentModeId).PaymentModeText;
                    obj.CollectedByName = db.EmployeeMasters.Find(l.PickedUpEmpID).EmployeeName;
                    obj.ConsigneeCountry = l.ConsigneeCountryName.ToString();
                }

                return Json(new { status = "ok", data = obj, message = "Data Found" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "failed", data = l, message = "Data Not Found" }, JsonRequestBehavior.AllowGet);
            }
            //return Json(obj, JsonRequestBehavior.AllowGet);
        }


    }
}
