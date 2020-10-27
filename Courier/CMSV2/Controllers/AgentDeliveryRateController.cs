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
    public class AgentDeliveryRateController : Controller
    {
      
        Entities1 db = new Entities1(); 

        public ActionResult Index()
        {
            List<AgentDeliveryRateVM> lst = (from c in db.tblAgentDeliveryRates join e in db.EmployeeMasters on c.AgentID equals e.EmployeeID

                                               join d in db.ZoneCharts on c.ZoneChartID equals d.ZoneChartID
                                               join y in db.ProductTypes on c.CourierServiceID equals y.ProductTypeID

                                             select new AgentDeliveryRateVM

                                               {
                                                  AgentName=e.EmployeeName,
                                                   CourierService = y.ProductName,
                                                   ZoneID = d.ZoneChartID,
                                                   ID = c.ID



                                               }).ToList();

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
        public ActionResult Create(AgentDeliveryRateVM v)
        {
            tblAgentDeliveryRate a = new tblAgentDeliveryRate();
            int max = (from d in db.tblAgentDeliveryRates orderby d.ID descending select d.ID).FirstOrDefault();

            a.ID = max + 1;
            a.AgentID = v.AgentID;
            a.ZoneChartID = v.ZoneID;

            int countryid = Convert.ToInt32(Session["depotcountry"].ToString());
            a.CountryID = countryid;
            a.CourierServiceID = v.ProductTypeID;
            a.BaseWeight = v.BaseWeight;
            a.BaseRate = v.BaseRate;

            db.tblAgentDeliveryRates.Add(a);
            db.SaveChanges();


            foreach (var item in v.AgentDeliveryRateDetailVM)
            {
                tblAgentDeliveryRateDetail b = new tblAgentDeliveryRateDetail();

                b.AgentDeliveryRateID = a.ID;
                b.AdditionalWeightFrom = item.AddWtFrom;
                b.AdditionalWeightTo = item.AddWtTo;
                b.IncrementalWeight = item.IncrWt;
                b.AdditionalRate = item.AddRate;

                db.tblAgentDeliveryRateDetails.Add(b);


                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }



        public ActionResult Edit(int id = 0)
        {
            AgentDeliveryRateVM a = new AgentDeliveryRateVM();
            ViewBag.producttypeid = db.ProductTypes.ToList();
            ViewBag.agent = (from c in db.EmployeeMasters where c.Type == "A" select c).ToList();
            ViewBag.ZoneID = db.ZoneCharts.ToList();

            tblAgentDeliveryRate data = (from c in db.tblAgentDeliveryRates where c.ID == id select c).FirstOrDefault();

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
        public ActionResult Edit(AgentDeliveryRateVM v)
        {
            tblAgentDeliveryRate a = new tblAgentDeliveryRate();

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

            var data = (from c in db.tblAgentDeliveryRateDetails where c.AgentDeliveryRateID == v.ID select c).ToList();
            foreach (var item in data)
            {
                db.tblAgentDeliveryRateDetails.Remove(item);
                db.SaveChanges();
            }
            foreach (var item in v.AgentDeliveryRateDetailVM)
            {
                tblAgentDeliveryRateDetail b = new tblAgentDeliveryRateDetail();

                b.AgentDeliveryRateID = a.ID;
                b.AdditionalWeightFrom = item.AddWtFrom;
                b.AdditionalWeightTo = item.AddWtTo;
                b.IncrementalWeight = item.IncrWt;
                b.AdditionalRate = item.AddRate;

                db.tblAgentDeliveryRateDetails.Add(b);


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


            var data = (from c in db.tblAgentDeliveryRateDetails where c.AgentDeliveryRateID == id select c).ToList();

            List<det> lst = new List<det>();

            if (data != null)
            {

                foreach (var item in data)
                {
                    det d = new det();

                    d.ID = item.AgentDeliveryRateID;

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
            tblAgentDeliveryRate a = db.tblAgentDeliveryRates.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.tblAgentDeliveryRates.Remove(a);
                db.SaveChanges();

                List<tblAgentDeliveryRateDetail> lst = (from c in db.tblAgentDeliveryRateDetails where c.ID == id select c).ToList();

                foreach (var item in lst)
                {
                    db.tblAgentDeliveryRateDetails.Remove(item);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
        }
    }
}


