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
    public class AgentCollectionRateController : Controller
    {
        Entities1 db = new Entities1();

        public ActionResult Index()
        {
            List<AgentCollectionRateVM> lst = (from c in db.tblAgentCollectionRates
                                               join e in db.EmployeeMasters on c.AgentID equals e.EmployeeID

                                               join d in db.ZoneCharts on c.ZoneChartID equals d.ZoneChartID
                                               join y in db.ProductTypes on c.CourierServiceID equals y.ProductTypeID

                                               select new AgentCollectionRateVM

                                      {
                                          Fname = e.EmployeeName,
                                          CourierService = y.ProductName,
                                          ZoneID = d.ZoneChartID,
                                          ID = c.ID



                                      }).ToList();

            //List<AgentCollectionRateVM> lst = new List<AgentCollectionRateVM>();

            //var data = db.tblAgentCollectionRates.ToList();

            //foreach (var item in data)
            //{
            //    AgentCollectionRateVM c = new AgentCollectionRateVM();

            //    c.ID = item.ID;
            //    c.AgentID = item.AgentID;
            //    c.CourierServiceID = item.CourierServiceID;
            //    c.ZoneChartID = item.ZoneChartID;
            //    lst.Add(c);


            //}

            return View(lst);


           
        }

        public ActionResult Create()
        {
            ViewBag.producttypeid = db.ProductTypes.ToList();
           
            ViewBag.agent = (from c in db.EmployeeMasters where c.Type == "A" select c).ToList();
            ViewBag.ZoneID = db.ZoneCharts.ToList();
           
            return View();
        }

        [HttpPost]
        public ActionResult Create(AgentCollectionRateVM v)
        {
            tblAgentCollectionRate a = new tblAgentCollectionRate();

            int max = (from d in db.tblAgentCollectionRates orderby d.ID descending select d.ID).FirstOrDefault();

            a.ID = max + 1;
            a.AgentID = v.AgentID;
            a.ZoneChartID = v.ZoneID;

            int countryid = Convert.ToInt32(Session["depotcountry"].ToString());
            a.CountryID = countryid;
            a.CourierServiceID = v.ProductTypeID;
            a.BaseWeight = v.BaseWeight;
            a.BaseRate = v.BaseRate;

            db.tblAgentCollectionRates.Add(a);
            db.SaveChanges();

            foreach (var item in v.AgentCollectionRateDetailVM)
            {
                tblAgentCollectionRateDetail b = new tblAgentCollectionRateDetail();

                b.AgentCollectionRateID = a.ID;
                b.AdditionalWeightFrom = item.AddWtFrom;
                b.AdditionalWeightTo = item.AddWtTo;
                b.IncrementalWeight = item.IncrWt;
                b.AdditionalRate = item.AddRate;

                db.tblAgentCollectionRateDetails.Add(b);


                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        public ActionResult Edit(int id = 0)
        {
            AgentCollectionRateVM a = new AgentCollectionRateVM();
            ViewBag.producttypeid = db.ProductTypes.ToList();
            ViewBag.agent = (from c in db.EmployeeMasters where c.Type == "A" select c).ToList();
            ViewBag.ZoneID = db.ZoneCharts.ToList();


            tblAgentCollectionRate data = (from c in db.tblAgentCollectionRates where c.ID == id select c).FirstOrDefault();

            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {

                a.ID = data.ID;
                a.AgentID = data.AgentID;
                a.ZoneChartID = data.ZoneChartID;

                int countryid = Convert.ToInt32(Session["depotcountry"].ToString());

                a.ProductTypeID = data.CourierServiceID;
                a.BaseWeight = data.BaseWeight;
                a.BaseRate = data.BaseRate;

            }

            return View(a);

        }


        [HttpPost]
        public ActionResult Edit( AgentCollectionRateVM v)
        {
            tblAgentCollectionRate a = new tblAgentCollectionRate();

            a.ID = v.ID;
            a.AgentID = v.AgentID;
            a.ZoneChartID = v.ZoneID;

            int countryid = Convert.ToInt32(Session["depotcountry"].ToString());
            a.CountryID = countryid;
            a.CourierServiceID = v.ProductTypeID;
            a.BaseWeight = v.BaseWeight;
            a.BaseRate = v.BaseRate;

            db.Entry(a).State = EntityState.Modified;
            db.SaveChanges();

            var data = (from c in db.tblAgentCollectionRateDetails where c.AgentCollectionRateID == v.ID select c).ToList();
            foreach (var item in data)
            {
                db.tblAgentCollectionRateDetails.Remove(item);
                db.SaveChanges();
            }
            foreach (var item in v.AgentCollectionRateDetailVM)
            {
                tblAgentCollectionRateDetail b = new tblAgentCollectionRateDetail();

                b.AgentCollectionRateID = a.ID;
                b.AdditionalWeightFrom = item.AddWtFrom;
                b.AdditionalWeightTo = item.AddWtTo;
                b.IncrementalWeight = item.IncrWt;
                b.AdditionalRate = item.AddRate;

                db.tblAgentCollectionRateDetails.Add(b);


                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }





        public class det
        {
            public int ID { get; set; }

            public decimal AddWtFrom { get; set; }
            public decimal AddWtTo { get; set; }
            public decimal IncrWt { get; set; }
            public decimal ContractRate { get; set; }
            public decimal AddRate { get; set; }
        }


        public JsonResult GetDetails(int id)
        {


            var data = (from c in db.tblAgentCollectionRateDetails where c.AgentCollectionRateID == id select c).ToList();

            List<det> lst = new List<det>();

            if (data != null)
            {

                foreach (var item in data)
                {
                    det d = new det();

                    d.ID = item.AgentCollectionRateID;

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
            tblAgentCollectionRate a = db.tblAgentCollectionRates.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.tblAgentCollectionRates.Remove(a);
                db.SaveChanges();

                List<tblAgentCollectionRateDetail> lst = (from c in db.tblAgentCollectionRateDetails where c.ID == id select c).ToList();

                foreach (var item in lst)
                {
                    db.tblAgentCollectionRateDetails.Remove(item);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
        }

      
    }
}
