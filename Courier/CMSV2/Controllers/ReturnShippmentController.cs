using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;


namespace CMSV2.Controllers
{
    public class ReturnShippmentController : Controller
    {
        Entities1 db = new Entities1();

        public ActionResult Index()
        {
            List<ReturnShippmentVM> lst = new List<ReturnShippmentVM>();

            var data=db.InScans.ToList();
            
            foreach (var item in data)
            {
                ReturnShippmentVM obj = new ReturnShippmentVM();
                obj.InscanID = item.InScanID;
                obj.AWBNo = item.AWBNo;
                obj.Date = item.InScanDate;
                obj.CollectedBy = item.CollectedBy;
                obj.StatedWeight = item.StatedWeight;
                obj.Pieces = item.Pieces;
                obj.CourierCharges = item.CourierCharge;
                obj.Consignor = item.Consignor;
                obj.ConsignorCountryID = item.ConsignorCountryID.Value;
                obj.Consignee = item.Consignee;
                obj.CosigneeCountryID = item.ConsigneeCountryID;
                obj.StatusPaymentMOde = item.StatusPaymentMode;
                lst.Add(obj);
            }
            return View(lst);
        }





        public ActionResult Create()
        {
          
            return View();

        }

        [HttpPost]
        public ActionResult Create(InScan v)
        {
            if (ModelState.IsValid)
            {

              

                db.InScans.Add(v);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Location.";
                return RedirectToAction("Index");
            }


            return View(v);
        }


        public JsonResult GetReturnData(string id)
        {
            var item = (from c in db.InScans where c.AWBNo == id select c).FirstOrDefault();

            ReturnShippmentVM obj = new ReturnShippmentVM();

            if (item != null)
            {
                obj.InscanID = item.InScanID;
                obj.AWBNo = item.AWBNo;
                //obj.Date = DateTime.Now;
                obj.CollectedBy = item.CollectedBy;
                obj.StatedWeight = item.StatedWeight;
                obj.Pieces = item.Pieces;
                obj.CourierCharges = item.CourierCharge;
                obj.Consignor = item.Consignor;
                obj.ConsignorCountryID = item.ConsignorCountryID.Value;
                obj.Consignee = item.Consignee;
                obj.CosigneeCountryID = item.ConsigneeCountryID;
                obj.StatusPaymentMOde = item.StatusPaymentMode;
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


    }
}
