using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;

namespace CMSV2.Controllers
{
    [SessionExpire]
    //[Authorize]
    public class RevenueTypeController : Controller
    {
        SourceMastersModel objSourceMastersModel = new SourceMastersModel();
        //SourceMastersModel objectSourceModel = new SourceMastersModel();
        //SHIPPING_FinalEntities db = new SHIPPING_FinalEntities();
        Entities1 db = new Entities1();

        public ActionResult Index()
        {
            List<RevenueAcHeadVM> model = new List<RevenueAcHeadVM>();
            var query = (from t in db.RevenueTypes
                         join t1 in db.AcHeads on t.AcHeadID equals t1.AcHeadID
                         select new RevenueAcHeadVM

                         {
                             RevenueType=t.RevenueType1,
                             RevenueCode=t.RevenueCode,
                             AcHead=t1.AcHead1,
                             RevenueTypeID=t.RevenueTypeID

                            

                         }).ToList();
       
            return View(query);
        }

     

        public ActionResult Details(int id = 0)
        {
            RevenueType revenuetype = objSourceMastersModel.GetRevenueTypeById(id);
            if (revenuetype == null)
            {
                return HttpNotFound();
            }
            return View(revenuetype);
        }

     
        public ActionResult Create(int acheadid)
        {
            ViewBag.AcheadId = acheadid;

            ViewBag.accounthead = db.AcHeads.OrderBy(x => x.AcHead1).ToList();
            return View();
        }

     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RevenueType revenuetype)
        {
            if (ModelState.IsValid)
            {


                var query = (from t in db.RevenueTypes where t.RevenueType1 == revenuetype.RevenueType1 select t).ToList();

                if (query.Count > 0)
                {
                    ViewBag.accounthead = db.AcHeads.ToList();
                    ViewBag.SuccessMsg = "Revenue Type is already exist";
                    return View();
                }
                objSourceMastersModel.SaveRevenueType(revenuetype);
                TempData["SuccessMSG"] = "You have successfully added Revenue Type.";
                return RedirectToAction("Index");
            }

            return View(revenuetype);
        }


        public JsonResult GetRevenueTypeName(string name,string achead)
        {

            int ac = Convert.ToInt32(achead);
            var revenue = (from c in db.RevenueTypes where c.RevenueType1 == name && c.AcHeadID==ac select c).FirstOrDefault();
            
            Status obj = new Status();

            if (revenue == null)
            {
                obj.flag = 0;

            }
            else
            {
                obj.flag = 1;
            }

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public class Status
        {
            public int flag { get; set; }
        }

        public ActionResult Edit(int id = 0)
        {
            ViewBag.accounthead = db.AcHeads.ToList();
            RevenueType revenuetype = objSourceMastersModel.GetRevenueTypeById(id);
            if (revenuetype == null)
            {
                return HttpNotFound();
            }
            return View(revenuetype);
        }

    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RevenueType revenuetype)
        {
            if (ModelState.IsValid)
            {

                objSourceMastersModel.SaveRevenueTypeById(revenuetype);
                TempData["SuccessMSG"] = "You have successfully updated Revenue Type.";
                return RedirectToAction("Index");
            }

            return View(revenuetype);
        }

    
        
      
        public ActionResult DeleteConfirmed(int id)
        {
            objSourceMastersModel.DeleteRevenueType(id);
            TempData["SuccessMSG"] = "You have successfully deleted Revenue Type.";
            return RedirectToAction("Index");
        }

     
    }
}