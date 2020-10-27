using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;

namespace CMSV2.Controllers
{
    public class LocationMasterController : Controller
    {
        Entities1 db = new Entities1();


        public ActionResult Index()
        {

            List<LocationVM> lst = new List<LocationVM>();
            var data = db.LocationMasters.ToList();
            foreach (var item in data)
            {
                LocationVM obj = new LocationVM();
                obj.LocationID = item.LocationID;
                obj.Location = item.Location;
                obj.CityID = item.CityID.Value;
                lst.Add(obj);
            }
            return View(data);
        }

        //
        // GET: /LocationMaster/Details/5

        public ActionResult Details(int id = 0)
        {
            LocationMaster locationmaster = db.LocationMasters.Find(id);
            if (locationmaster == null)
            {
                return HttpNotFound();
            }
            return View(locationmaster);
        }

        //
        // GET: /LocationMaster/Create

        public ActionResult Create()
        {
            ViewBag.city = db.CityMasters.ToList();
            ViewBag.country = db.CountryMasters.ToList();
            return View();
        }

        //
        // POST: /LocationMaster/Create

        [HttpPost]

        public ActionResult Create(LocationVM v)
        {
            if (ModelState.IsValid)
            {

                LocationMaster ob = new LocationMaster();


                int max = (from c in db.LocationMasters orderby c.LocationID descending select c.LocationID).FirstOrDefault();

                if (max == null)
                {
                    ob.LocationID = 1;
                    ob.Location = v.Location;
                    ob.CityID = v.CityID;

                }
                else
                {
                    ob.LocationID = max + 1;
                    ob.Location = v.Location;
                    ob.CityID = v.CityID;
                }

                db.LocationMasters.Add(ob);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Location.";
                return RedirectToAction("Index");
            }


            return View(v);
        }



        public ActionResult Edit(int id)
        {
            LocationVM v = new LocationVM();
            
            ViewBag.country = db.CountryMasters.ToList();
            var data = (from c in db.LocationMasters where c.LocationID == id select c).FirstOrDefault();

            int countryid=(from c in db.CityMasters where c.CityID==data.CityID select c.CountryID).FirstOrDefault().Value;
            ViewBag.city = (from c in db.CityMasters where c.CountryID == countryid select c).ToList();

            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                v.LocationID = data.LocationID;
                v.Location = data.Location;
                v.CityID = data.CityID.Value;
                v.CountryID=countryid;
            }

            return View(v);
        }



        [HttpPost]

        public ActionResult Edit(LocationVM l)
        {
            LocationMaster a = new LocationMaster();
            a.LocationID = l.LocationID;
            a.Location = l.Location;
            a.CityID = l.CityID;

            if (ModelState.IsValid)
            {
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated Location.";
                return RedirectToAction("Index");
            }

            return View();
        }



        public ActionResult DeleteConfirmed(int id)
        {
            LocationMaster locationmaster = db.LocationMasters.Find(id);
            db.LocationMasters.Remove(locationmaster);
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully Deleted Location.";
            return RedirectToAction("Index");
        }

        public JsonResult GetCity(int id)
        {
            List<CityM> objCity = new List<CityM>();
            var city = (from c in db.CityMasters where c.CountryID == id select c).ToList();

            foreach (var item in city)
            {
                objCity.Add(new CityM { City = item.City, CityID = item.CityID });

            }
            return Json(objCity, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

public class CityM
{
    public int CityID { get; set; }
    public String City { get; set; }
}