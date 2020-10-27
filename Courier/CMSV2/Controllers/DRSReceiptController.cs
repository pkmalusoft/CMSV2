using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using System.Data.Entity;

namespace CMSV2.Controllers
{
    public class DRSReceiptController : Controller
    {

        Entities1 db = new Entities1();

        public ActionResult Index()
        {

          
            List<DRSReceiptVM> ls = (from c in db.DRSReceipts join e in db.EmployeeMasters on c.EmployeeID equals e.EmployeeID join t in db.VehicleMasters on c.VehicleID equals t.VehicleID select new DRSReceiptVM {DRSReceiptID=c.DRSReceiptID,DRSNo=c.DRSNo,Amount=c.Amount,DRSReceiptDate=c.DRSReceiptDate,Deliver=e.EmployeeName,Vehicle=t.VehicleNo,Remarks=c.Remarks }).ToList();


            return View(ls);
        }


        public ActionResult Create()
        {
            ViewBag.AcHeads = db.AcHeads.ToList();
            ViewBag.Vehicle = db.VehicleMasters.ToList();
            ViewBag.Employee = db.EmployeeMasters.ToList();
            ViewBag.Department = db.Departments.ToList();

            return View();

        }

        [HttpPost]
        public ActionResult Create(DRSReceiptVM v)
        {
            DRSReceipt tbl = new DRSReceipt();
            AcJournalMaster _AcJournalMaster = new AcJournalMaster();
            AcJournalDetail _AcJournalDetail = new AcJournalDetail();

            try
            {
                int max = (from c in db.DRSReceipts orderby c.DRSReceiptID descending select c.DRSReceiptID).FirstOrDefault();
                if (max == null)
                {
                    max = 1;
                }
                else
                {
                    max = max + 1;
                }



                tbl.DRSReceiptID = max;


                _AcJournalMaster.VoucherNo = tbl.DRSReceiptID.ToString();
                tbl.DRSNo = v.DRSNo;
                tbl.EmployeeID = v.EmployeeID;
                tbl.DRSReceiptDate = v.DRSReceiptDate;
                tbl.DepartmentID = v.DepartmentID;
                tbl.VehicleID = v.VehicleID;
                tbl.Amount = v.Amount;
                tbl.Remarks = v.Remarks;
                tbl.AcCompanyID = Convert.ToInt32(Session["CurrenctCompanyID"].ToString());
                tbl.User1 = Convert.ToInt32(Session["UserID"].ToString());
                tbl.FYearID = Convert.ToInt32(Session["fyearid"].ToString());

                db.DRSReceipts.Add(tbl);
                db.SaveChanges();

                int acjmax = (from c in db.AcJournalMasters orderby c.AcJournalID descending select c.AcJournalID).FirstOrDefault();
                if (acjmax == null)
                    acjmax = 1;
                else
                    acjmax = acjmax + 1;

                _AcJournalMaster.AcJournalID = acjmax;
                _AcJournalMaster.TransDate = v.DRSReceiptDate;
                _AcJournalMaster.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
                _AcJournalMaster.UserID = Convert.ToInt32(Session["UserID"].ToString());
                _AcJournalMaster.AcCompanyID = Convert.ToInt32(Session["CurrenctCompanyID"].ToString());

                _AcJournalMaster.Remarks = v.Remarks;
                _AcJournalMaster.VoucherType = "CI";
                _AcJournalMaster.TransType = 1;
                _AcJournalMaster.StatusDelete = Convert.ToBoolean(0);
                db.AcJournalMasters.Add(_AcJournalMaster);
                db.SaveChanges();

                var DRSUpdate = (from a in db.DRSReceipts where a.DRSReceiptID == tbl.DRSReceiptID select a).FirstOrDefault();

                var id = (from a in db.AcJournalMasters orderby a.AcJournalID descending select a).FirstOrDefault();
                DRSUpdate.AcJournalID = id.AcJournalID;
                db.Entry(DRSUpdate).State = EntityState.Modified;
                db.SaveChanges();


                int maxacjdid = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();
                if (maxacjdid == null)
                    maxacjdid = 1;
                else
                    maxacjdid = maxacjdid + 1;

                _AcJournalDetail.AcJournalDetailID = maxacjdid;
                _AcJournalDetail.AcJournalID = id.AcJournalID;
                _AcJournalDetail.Amount = v.Amount;
                _AcJournalDetail.Remarks = v.Remarks;
                _AcJournalDetail.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());

                db.AcJournalDetails.Add(_AcJournalDetail);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Added DRS Receipt.";

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                
                throw;
            }
          

          
        }






        public ActionResult Edit(int id=0)
        {
            DRSReceiptVM obj = new DRSReceiptVM();
            ViewBag.employee = db.EmployeeMasters.ToList();
            ViewBag.verhicle = db.VehicleMasters.ToList();
            ViewBag.Department = db.Departments.ToList();

            var data = (from d in db.DRSReceipts where d.DRSReceiptID == id select d).FirstOrDefault();

            if (data == null)
            {
                return HttpNotFound();
            }
            else
            
{
    obj.DRSReceiptID = data.DRSReceiptID;
                //obj.DRSID = data.DRSNoS;
    obj.DRSNo = data.DRSNo;
                obj.EmployeeID = data.EmployeeID;
                obj.DRSReceiptDate = data.DRSReceiptDate;
                obj.DepartmentID = data.DepartmentID;
                obj.VehicleID = data.VehicleID;
                obj.Remarks = data.Remarks;
                obj.AcCompanyID = data.AcCompanyID;
                obj.User1 = data.User1;
                obj.FYearID = data.FYearID;
                obj.Amount = data.Amount;
            }
            return View(obj);
        }

        [HttpPost]

        public ActionResult Edit(DRSReceiptVM data)
        {
            DRSReceipt obj = new DRSReceipt();
            obj.DRSReceiptID = data.DRSReceiptID;
            obj.DRSNo = data.DRSNo;
            obj.EmployeeID = data.EmployeeID;
            obj.DRSReceiptDate = data.DRSReceiptDate;
            obj.DepartmentID = data.DepartmentID;
            obj.VehicleID = data.VehicleID;
            obj.Remarks = data.Remarks;
            obj.AcCompanyID = data.AcCompanyID;
            obj.User1 = data.User1;
            obj.FYearID = data.FYearID;
            obj.Amount = data.Amount;


            if (ModelState.IsValid)
            {
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated DRS Receipt.";
                return RedirectToAction("Index");
            }
            return View();

        }



        public class Details
        {
            public string DRSID { get; set; }
            public string DRSNo { get; set; }
            public int VehicleID { get; set; }
           
            public decimal Amount { get; set; }
            public int DeliveredBy { get; set; }
        

        }

        public JsonResult GetDRSData(string id)
        {

            //var l = (from c in db.InScans where c.InScanDate >= s  && c.InScanDate <= e select c).ToList();
            var l = (from c in db.DRS where c.DRSNo == id select c).FirstOrDefault();

            Details obj = new Details();
            if (l != null)
            {
                obj.DRSID = l.DRSID.ToString();
                obj.DRSNo = l.DRSNo;
                obj.VehicleID = l.VehicleID.Value;
                obj.DeliveredBy = l.DeliveredBy.Value;

                decimal tot = 0;
                var amtdata = (from c in db.DRSDetails where c.DRSID == l.DRSID select c).ToList();
                foreach (var item in amtdata)
                {
                    tot = tot + Convert.ToDecimal(item.CourierCharge);

                }
                obj.Amount = tot;
            }

            return Json(obj, JsonRequestBehavior.AllowGet);
        }



        public ActionResult DeleteConfirmed(int id)
        {
            DRSReceipt drs = db.DRSReceipts.Find(id);
            db.DRSReceipts.Remove(drs);
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully Deleted DRS Receipt.";
            return RedirectToAction("Index");
        }

    }
}
