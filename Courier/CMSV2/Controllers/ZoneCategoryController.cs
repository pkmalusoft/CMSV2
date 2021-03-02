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
    public class ZoneCategoryController : Controller
    {
        Entities1 db = new Entities1();


        public ActionResult Index()
        {
            List<ZoneCategoryVM> lst = new List<ZoneCategoryVM>();
            var data = (from c in db.ZoneCategories select c).ToList();


            foreach (var item in data)
            {
                ZoneCategoryVM v = new ZoneCategoryVM();
                v.ZoneCategoryID = item.ZoneCategoryID;
                v.ZoneCategory = item.ZoneCategory1;
                v.StatusBaseCategory = item.StatusBaseCategory.Value;

                lst.Add(v);
            }
            return View(lst);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ZoneCategoryVM v)
        {
            ZoneCategory a = new ZoneCategory();
            if(ModelState.IsValid)
            {
                a.ZoneCategory1 = v.ZoneCategory;
                a.StatusBaseCategory = v.StatusBaseCategory;


                db.ZoneCategories.Add(a);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Zone Category.";
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Edit(int id)
        {
            ZoneCategory a = (from c in db.ZoneCategories where c.ZoneCategoryID == id select c).FirstOrDefault();
            ZoneCategoryVM v = new ZoneCategoryVM();
            v.ZoneCategoryID = a.ZoneCategoryID;
            v.ZoneCategory = a.ZoneCategory1;
            v.StatusBaseCategory = a.StatusBaseCategory.Value;

            return View(v);
        }

        [HttpPost]
        public ActionResult Edit(ZoneCategoryVM v)
        {
            ZoneCategory a = new ZoneCategory();
            if (ModelState.IsValid)
            {
                a.ZoneCategoryID = v.ZoneCategoryID;
                a.ZoneCategory1 = v.ZoneCategory;
                a.StatusBaseCategory = v.StatusBaseCategory;

                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated Zone Category.";
                return RedirectToAction("Index");
            }

            return View();

        }


        public ActionResult DeleteConfirmed(int id)
        {
            ZoneCategory a = (from c in db.ZoneCategories where c.ZoneCategoryID == id select c).FirstOrDefault();
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (a.ZoneCategory1 != "Employee")
                {
                    db.ZoneCategories.Remove(a);
                    db.SaveChanges();
                    TempData["SuccessMsg"] = "You have successfully Deleted Zone Category.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["SuccessMsg"] = "Could not delete this Employee Category!";
                    return RedirectToAction("Index");
                }
            }
        }
    }
}
