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
    public class ForwardingAgentMasterController : Controller
    {
        Entities1 db = new Entities1();

        public ActionResult Index()
        {

            List<FAgentVM> lst = (from c in db.ForwardingAgentMasters join t in db.CountryMasters on c.CountryID equals t.CountryID select new FAgentVM {FAgentID=c.FAgentID,ReferenceCode=c.ReferenceCode,FAgentName=c.FAgentName,ContactPerson=c.ContactPerson,CountryName=t.CountryName }).ToList();
            return View(lst);
        }

        public ActionResult Create()
        {
            ViewBag.city = db.CityMasters.ToList();
        
            ViewBag.country = db.CountryMasters.ToList();
            ViewBag.Designations = db.Designations.ToList();
            ViewBag.ZoneCategory = db.ZoneCategories.ToList();
            //ViewBag.LocationID = new SelectList(db.LocationMasters, "LocationID", "Location");
            ViewBag.Currency = db.CurrencyMasters.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(FAgentVM v)
        {
            ForwardingAgentMaster a = new ForwardingAgentMaster();
            int max = (from c in db.ForwardingAgentMasters orderby c.FAgentID descending select c.FAgentID).FirstOrDefault();
            if (ModelState.IsValid)
            {
                a.FAgentID = max + 1;
                a.ReferenceCode = v.ReferenceCode;
                a.FAgentName = v.FAgentName;
                a.AcCompanyID = 1;
                a.AcHeadID = 1;
                a.Address1 = v.Address1;
                a.Address2 = v.Address2;
                a.Phone = v.Phone;
                a.Fax = v.Fax;
                a.Email = v.Email;
                a.WebSite = v.WebSite;
                a.ContactPerson = v.ContactPerson;
                a.CountryID = v.CountryID;
               
                a.CityID = v.CityID;
                a.CurrencyID = v.CurrencyID;
                a.ZoneCategoryID = v.ZoneCategoryID;
                a.StatusActive = v.StatusActive;
                a.StatusDefault = v.StatusDefault;
                a.StatusSigned = v.StatusSigned;

                db.ForwardingAgentMasters.Add(a);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Forwarding Agent.";
                return RedirectToAction("Index");
               
            }
            return View();
        }

        public ActionResult Edit(int id)
        {
            FAgentVM v = new FAgentVM();
            ForwardingAgentMaster a = (from c in db.ForwardingAgentMasters where c.FAgentID == id select c).FirstOrDefault();

            if(a==null)
            {
                return HttpNotFound();
            }
            else
            {
                    v.FAgentID = a.FAgentID;
                    v.ReferenceCode = a.ReferenceCode;
                    v.FAgentName = a.FAgentName;
                    v.AcCompanyID = 1;
                    v.AcHeadID = 1;
                    v.Address1 = a.Address1;
                    v.Address2 = a.Address2;
                    v.Phone = a.Phone;
                    v.Fax = a.Fax;
                    v.Email = a.Email;
                    v.WebSite = a.WebSite;
                    v.ContactPerson = a.ContactPerson;
                    v.CountryID = a.CountryID;
                  
                    v.CityID = a.CityID;
                    v.CurrencyID = a.CurrencyID;
                    v.ZoneCategoryID = a.ZoneCategoryID.Value;
                    v.StatusActive = a.StatusActive;
                    v.StatusDefault = a.StatusDefault.Value;
                    v.StatusSigned = a.StatusSigned;

                  ViewBag.city = db.CityMasters.ToList().Where(x=>x.CountryID==a.CountryID);
        
            ViewBag.country = db.CountryMasters.ToList();
            ViewBag.Designations = db.Designations.ToList();
            ViewBag.ZoneCategory = db.ZoneCategories.ToList();
            //ViewBag.LocationID = new SelectList(db.LocationMasters, "LocationID", "Location");
            ViewBag.Currency = db.CurrencyMasters.ToList();
            return View(v);
            }
          
        }

        [HttpPost]
        public ActionResult Edit(FAgentVM v)
        {
            ForwardingAgentMaster a = new ForwardingAgentMaster();
            if (ModelState.IsValid)
            {
                a.FAgentID = v.FAgentID;
                a.ReferenceCode = v.ReferenceCode;
                a.FAgentName = v.FAgentName;
                a.AcCompanyID = 1;
                a.AcHeadID = 1;
                a.Address1 = v.Address1;
                a.Address2 = v.Address2;
                a.Phone = v.Phone;
                a.Fax = v.Fax;
                a.Email = v.Email;
                a.WebSite = v.WebSite;
                a.ContactPerson = v.ContactPerson;
                a.CountryID = v.CountryID;
             
                a.CityID = v.CityID;
                a.CurrencyID = v.CurrencyID;
                a.ZoneCategoryID = v.ZoneCategoryID;
                a.StatusActive = v.StatusActive;
                a.StatusDefault = v.StatusDefault;
                a.StatusSigned = v.StatusSigned;

                db.Entry(a).State=EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated Forwarding Agent.";
                return RedirectToAction("Index");
            }

            return View();
        }

        public ActionResult DeleteConfirmed(int id)
        {
            ForwardingAgentMaster a = (from c in db.ForwardingAgentMasters where c.FAgentID == id select c).FirstOrDefault();
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.ForwardingAgentMasters.Remove(a);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Deleted Forwarding Agent.";
                return RedirectToAction("Index");

            }

            return View();
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

        public JsonResult GetLocation(int id)
        {
            List<LocationM> objLoc = new List<LocationM>();
            var loc = (from c in db.LocationMasters where c.CityID == id select c).ToList();

            foreach (var item in loc)
            {
                objLoc.Add(new LocationM { Location = item.Location, LocationID = item.LocationID });

            }
            return Json(objLoc, JsonRequestBehavior.AllowGet);
        }

        public class CityM
        {
            public int CityID { get; set; }
            public String City { get; set; }
        }

        public class LocationM
        {
            public int LocationID { get; set; }
            public String Location { get; set; }
        }

    }

    
           
}
