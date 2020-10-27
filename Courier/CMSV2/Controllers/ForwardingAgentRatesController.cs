using CMSV2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSV2.Controllers
{
    public class ForwardingAgentRatesController : Controller
    {
        Entities1 db = new Entities1();

        public ActionResult Index()
        {
       
            List<FAgentRateVM> lst= (from c in db.ForwardingAgentRates join t in db.ForwardingAgentMasters on c.FAgentID equals t.FAgentID join d in db.ZoneCharts on c.ZoneChartID equals d.ZoneChartID join y in db.ProductTypes on c.CourierServiceID equals y.ProductTypeID

                         select new FAgentRateVM

                         {

                           Fname=t.FAgentName,
                         CourierService=y.ProductName,
                         ZoneID=d.ZoneChartID,
                         FAgentRateID=c.FAgentRateID



                         }).ToList();


            return View(lst);
        }
        public ActionResult Create()
        {
            ViewBag.ProductTypeID = db.ProductTypes.ToList();
            var a = db.ForwardingAgentMasters.ToList();
            ViewBag.FAgentID = db.ForwardingAgentMasters.ToList();
            ViewBag.ZoneID = db.ZoneCharts.ToList();
          
            return View();
        }

        [HttpPost]
        public ActionResult Create(FAgentRateVM v)
        {
            ForwardingAgentRate FAge = new ForwardingAgentRate();

            int max = (from d in db.ForwardingAgentRates orderby d.FAgentRateID descending select d.FAgentRateID).FirstOrDefault();


                FAge.FAgentRateID = max + 1;
                FAge.FAgentID = v.FAgentID;
                FAge.ZoneChartID = v.ZoneID;

                int countryid = Convert.ToInt32(Session["depotcountry"].ToString());
                FAge.CountryID = countryid;
                FAge.CourierServiceID = v.ProductTypeID;
                FAge.BaseWeight = v.BaseWeight;
                FAge.BaseRate = v.BaseRate;

                db.ForwardingAgentRates.Add(FAge);
                db.SaveChanges();



            foreach (var item in v.FAgentRateDetails)
            {
                ForwardingAgentRateDet a = new ForwardingAgentRateDet();

                a.FAgentRateID = FAge.FAgentRateID;
                a.AdditionalWeightFrom = item.AddWtFrom;
                a.AdditionalWeightTo = item.AddWtTo;
                a.IncrementalWeight = item.IncrWt;
                a.AdditionalRate = item.AddRate;

                db.ForwardingAgentRateDets.Add(a);

               
                db.SaveChanges();
            }


            return RedirectToAction("Index");
        }



        public ActionResult Edit(int id)
        {
          
            FAgentRateVM FAge= new FAgentRateVM();
            ViewBag.ProductTypeID = db.ProductTypes.ToList();
            var h = db.ForwardingAgentMasters.ToList();
            ViewBag.FAgentID = db.ForwardingAgentMasters.ToList();
            ViewBag.ZoneID = db.ZoneCharts.ToList();


            ForwardingAgentRate data = (from c in db.ForwardingAgentRates where c.FAgentRateID == id select c).FirstOrDefault();
       

            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {

                FAge.FAgentRateID = data.FAgentRateID;
                FAge.FAgentID = data.FAgentID;
                FAge.ZoneID = data.ZoneChartID;
                
                int countryid = Convert.ToInt32(Session["depotcountry"].ToString());
                FAge.CountryID = countryid;
                FAge.ProductTypeID = data.CourierServiceID.Value;
                FAge.BaseWeight = data.BaseWeight;
                FAge.BaseRate = data.BaseRate;




            }

            return View(FAge);
            

        }

        [HttpPost]
        public ActionResult Edit(FAgentRateVM v)
        {

            ForwardingAgentRate obj = new ForwardingAgentRate();
            obj.FAgentRateID = v.FAgentRateID;
        
            obj.FAgentID = v.FAgentID;
            obj.ZoneChartID = v.ZoneID;

            int countryid = Convert.ToInt32(Session["depotcountry"].ToString());
            obj.CountryID = countryid;
            obj.CourierServiceID = v.ProductTypeID;
            obj.BaseWeight = v.BaseWeight;
            obj.BaseRate = v.BaseRate;
            db.Entry(obj).State=EntityState.Modified;
            db.SaveChanges();
          


            var data = (from c in db.ForwardingAgentRateDets where c.FAgentRateID == v.FAgentRateID select c).ToList();
            foreach (var item in data)
            {
                db.ForwardingAgentRateDets.Remove(item);
                db.SaveChanges();
            }
             foreach (var item in v.FAgentRateDetails)
            {
                ForwardingAgentRateDet ob = new ForwardingAgentRateDet();

                ob.FAgentRateID = v.FAgentRateID;
                ob.AdditionalWeightFrom = item.AddWtFrom;
                ob.AdditionalWeightTo = item.AddWtTo;
                ob.IncrementalWeight = item.IncrWt;
                ob.AdditionalRate = item.AddRate;

                db.ForwardingAgentRateDets.Add(ob);

                db.SaveChanges();
               
            }

             return RedirectToAction("Index");
            }


        public class det
        {
            public int FAgentRateID { get; set; }

            public decimal AddWtFrom { get; set; }
            public decimal AddWtTo { get; set; }
            public decimal IncrWt { get; set; }
            public decimal ContractRate { get; set; }
            public decimal AddRate { get; set; }
        }


        public JsonResult GetDetails(int id)
        {


            var data = (from c in db.ForwardingAgentRateDets where c.FAgentRateID == id select c).ToList();
          
            List<det> lst = new List<det>();

            if (data != null)
            {
            
                  foreach (var item in data)
            {
               det d = new det();

                d.FAgentRateID = item.FAgentRateID;

                d.AddWtFrom = item.AdditionalWeightFrom;
                d.AddWtTo = item.AdditionalWeightTo;
                d.IncrWt = item.IncrementalWeight;
                d.AddRate = item.AdditionalRate;

                lst.Add(d);
                  
                }



            }
            return Json(lst, JsonRequestBehavior.AllowGet);
          
        }



        public ActionResult DeleteConfirmed(int id = 0)
        {
            ForwardingAgentRate a = db.ForwardingAgentRates.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.ForwardingAgentRates.Remove(a);
                db.SaveChanges();

                List<ForwardingAgentRateDet> lst = (from c in db.ForwardingAgentRateDets where c.FAgentRateDetID == id select c).ToList();

                foreach (var item in lst)
                {
                    db.ForwardingAgentRateDets.Remove(item);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
        }

      
       


    }
}
