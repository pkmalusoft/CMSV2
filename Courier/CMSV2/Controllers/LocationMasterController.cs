using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using Newtonsoft.Json;

namespace CMSV2.Controllers
{
    public class LocationMasterController : Controller
    {
        Entities1 db = new Entities1();


        public ActionResult Index()
        {
            //UpdateLocationMaster();

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
            a.PlaceID = l.PlaceID;
            a.CountryName = l.CountryName;
            a.CityName = l.CityName;
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

        public void UpdateLocationMaster()
        {

            string GooglePlaceAPIKey = "AIzaSyAKwJ15dRInM0Vi1IAvv6C4V4vVM5HVnMc";
            //string GooglePlaceAPIUrl = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&types=geocode&language=en&key={1}";
            string GooglePlaceAPIUrl = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&language=en&key={1}";
            //< add key = "GooglePlaceAPIUrl" value = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&types=geocode&language=en&key={1}" />
            //< add key = "GooglePlaceAPIKey" value = "Your API Key" ></ add >
            string placeApiUrl = GooglePlaceAPIUrl; // ConfigurationManager.AppSettings["GooglePlaceAPIUrl"];

            try
            {
                var data1 = db.LocationMasters.ToList();
                foreach (var item in data1)
                {
                    placeApiUrl = placeApiUrl.Replace("{0}", item.LocationName);
                    placeApiUrl = placeApiUrl.Replace("{1}", GooglePlaceAPIKey);// ConfigurationManager.AppSettings["GooglePlaceAPIKey"]);

                    var result = new System.Net.WebClient().DownloadString(placeApiUrl);
                    var Jsonobject = JsonConvert.DeserializeObject<RootObject>(result);

                    List<Prediction> list = Jsonobject.predictions;
                    item.PlaceID = list[0].place_id;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
                               

                
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
public class RootObject
{
    public List<Prediction> predictions { get; set; }
    public string status { get; set; }
}
public class CityM
{
    public int CityID { get; set; }
    public String City { get; set; }
}

public class Prediction
{
    public string description { get; set; }
    public string id { get; set; }
    public string place_id { get; set; }
    public string reference { get; set; }
    public List<string> types { get; set; }
}