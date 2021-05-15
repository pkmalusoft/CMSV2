using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.DAL;
using CMSV2.Models;

namespace CMSV2.Controllers
{
    [SessionExpire]
    public class CustomerRatesMasterController : Controller
    {
        private Entities1 db = new Entities1();


        public ActionResult Index()
        {
            var data = db.GetCustomerRates();
            return View(data);
        }

        public ActionResult Create(int id=0)
        {
            ViewBag.Movement = db.CourierMovements.ToList();
            ViewBag.PaymentMode = db.tblPaymentModes.ToList();
            ViewBag.CustRate = db.CustomerRateTypes.ToList();
            ViewBag.ProductType = db.ProductTypes.ToList();
            ViewBag.ForwardingAgent = db.ForwardingAgentMasters.ToList();
            ViewBag.Zones = db.ZoneCharts.ToList();
            CustRateVM vm = new CustRateVM();
            CustomerRate r = new CustomerRate();
            if (id > 0)
            {
                r = db.CustomerRates.Find(id);
                vm.BaseRate = r.BaseRate;
                vm.BaseWt = r.BaseWeight;
                vm.CustomerRateID = r.CustomerRateID;
                vm.MovementID = Convert.ToInt32(r.MovementID);
                vm.PaymentModeID = Convert.ToInt32(r.PaymentModeID);
                vm.CustomerRateTypeID = r.CustomerRateTypeID;
                vm.ContractRateTypeID = r.CustomerRateTypeID;
                vm.ProductTypeID = r.CourierServiceID;
                vm.ZoneChartID = r.ZoneChartID;
                var loc = db.GetZoneChartByCustomer(r.CustomerRateTypeID);
                Zones lst = new Zones(); 
                lst = (from c in loc where c.ZoneChartID==r.ZoneChartID select new Zones { ZoneID = c.ZoneChartID, ZoneName = c.ZoneName }).FirstOrDefault();
                vm.ZoneChartName = lst.ZoneName;
                //vm.CountryID = r.CountryID.Value;
                vm.FAgentID = r.FAgentID.Value;
                vm.CustomerRateID = r.CustomerRateID;
                if (r.AdditionalCharges != null)
                {
                    vm.AdditionalCharges = r.AdditionalCharges.Value;
                }
                if (r.WithTax != null)
                {
                    vm.withtax = r.WithTax.Value;
                }
                if (r.WithoutTax != null)
                {
                    vm.withouttax = r.WithoutTax.Value;
                }
                ViewBag.Title = "Rate Chart - Modify";
                ViewBag.EditMode = "true";
            }
            else
            {
                ViewBag.Title = "Rate Chart";
                ViewBag.EditMode = "false";
                vm.CustomerRateID = 0;
            }

            return View(vm);

        }

        [HttpPost]
        public ActionResult Create(CustRateVM v)
        {
            try
            {
                CustomerRate r = new CustomerRate();
                if (v.CustomerRateID > 0)
                    r = db.CustomerRates.Find(v.CustomerRateID);

                r.CustomerRateTypeID = v.ContractRateTypeID;
                r.CourierServiceID = v.ProductTypeID;
                r.ZoneChartID = v.ZoneChartID;
                r.MovementID = v.MovementID;
                r.PaymentModeID = v.PaymentModeID;
                r.FAgentID = v.FAgentID;
                r.BaseWeight = v.BaseWt;
                r.WithTax = v.withtax;
                r.WithoutTax = v.withouttax;
                r.AdditionalCharges = v.AdditionalCharges;

                r.BaseRate = v.BaseRate;

                if (v.CustomerRateID > 0)
                {
                    db.Entry(r).State = EntityState.Modified;
                    db.SaveChanges();
                    var data = (from c in db.CustomerRateDets where c.CustomerRateID == v.CustomerRateID select c).ToList();
                    foreach (var item in data)
                    {
                        db.CustomerRateDets.Remove(item);
                        db.SaveChanges();
                    }
                }
                else
                {
                    db.CustomerRates.Add(r);
                    db.SaveChanges();
                }

                if (v.CustRateDetails != null)
                {
                    foreach (var item in v.CustRateDetails)
                    {
                        if (item.Deleted == false)
                        {
                            CustomerRateDet a = new CustomerRateDet();

                            a.CustomerRateID = r.CustomerRateID;
                            a.AdditionalWeightFrom = item.AddWtFrom;
                            a.AdditionalWeightTo = item.AddWtTo;
                            a.IncrementalWeight = item.IncrWt;
                            a.AdditionalRate = item.AddRate;

                            db.CustomerRateDets.Add(a);
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {

            }

            ViewBag.CustRate = db.CustomerRateTypes.ToList();
            ViewBag.ProductType = db.ProductTypes.ToList();
            ViewBag.ForwardingAgent = db.ForwardingAgentMasters.ToList();
            ViewBag.Zones = db.ZoneCharts.ToList();
            return View();
        }


        public ActionResult Edit(int id)
        {

            ViewBag.CustRate = db.CustomerRateTypes.ToList();
            ViewBag.ProductType = db.ProductTypes.ToList();
            ViewBag.ForwardingAgent = db.ForwardingAgentMasters.ToList();
            ViewBag.Zones = db.ZoneCharts.ToList();

            CustRateVM vm = new CustRateVM();
            CustomerRate r = db.CustomerRates.Find(id);
            if (r != null)
            {
                vm.BaseRate = r.BaseRate;
                vm.BaseWt = r.BaseWeight;
                vm.ContractRateID = r.CustomerRateID;
                vm.ContractRateTypeID = r.CustomerRateTypeID;
                vm.ProductTypeID = r.CourierServiceID;
                vm.ZoneChartID = r.ZoneChartID;
                //vm.CountryID = r.CountryID.Value;
                vm.FAgentID = r.FAgentID.Value;
                vm.CustomerRateID = r.CustomerRateID;
                vm.AdditionalCharges = r.AdditionalCharges.Value;
                vm.withtax = r.WithTax.Value;
                vm.withouttax = r.WithoutTax.Value;
            }
            else
            {
                return HttpNotFound();
            }


            return View(vm);


        }

        [HttpPost]
        public ActionResult Edit(CustRateVM v)
        {
            CustomerRate r = new CustomerRate();
            r.CustomerRateID = v.CustomerRateID;
            r.CustomerRateTypeID = v.ContractRateTypeID;
            r.CourierServiceID = v.ProductTypeID;
            r.ZoneChartID = v.ZoneChartID;
            r.FAgentID = v.FAgentID;
            r.BaseWeight = v.BaseWt;
            r.WithTax = v.withtax;
            r.WithoutTax = v.withouttax;
            r.AdditionalCharges = v.AdditionalCharges;

            r.BaseRate = v.BaseRate;

            db.Entry(r).State = EntityState.Modified;
            db.SaveChanges();

            var data = (from c in db.CustomerRateDets where c.CustomerRateID == v.CustomerRateID select c).ToList();
            foreach (var item in data)
            {
                db.CustomerRateDets.Remove(item);
                db.SaveChanges();
            }

            foreach (var item in v.CustRateDetails)
            {
                CustomerRateDet a = new CustomerRateDet();

                a.CustomerRateID = r.CustomerRateID;
                a.AdditionalWeightFrom = item.AddWtFrom;
                a.AdditionalWeightTo = item.AddWtTo;
                a.IncrementalWeight = item.IncrWt;
                a.AdditionalRate = item.AddRate;

                db.CustomerRateDets.Add(a);
                db.SaveChanges();
            }

            return RedirectToAction("Index");

        }

        public class det
        {
            public int CustomerRateID { get; set; }

            public decimal AddWtFrom { get; set; }
            public decimal AddWtTo { get; set; }
            public decimal IncrWt { get; set; }
            public decimal ContractRate { get; set; }
            public decimal AddRate { get; set; }
        }


        public JsonResult GetDetails(int id)
        {


            var data = (from c in db.CustomerRateDets where c.CustomerRateID == id select c).ToList();

            List<det> lst = new List<det>();

            if (data != null)
            {

                foreach (var item in data)
                {
                    det d = new det();

                    d.CustomerRateID = item.CustomerRateID;

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
            CustomerRate a = db.CustomerRates.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.CustomerRates.Remove(a);
                db.SaveChanges();

                List<CustomerRateDet> lst = (from c in db.CustomerRateDets where c.CustomerRateID == id select c).ToList();

                foreach (var item in lst)
                {
                    db.CustomerRateDets.Remove(item);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
        }

        public JsonResult GetZoneByCustomer(string term,int contractid)
        {

            List<ZoneNameVM> lst = new List<ZoneNameVM>();
            var loc = AWBDAO.GetZoneChartMaster(contractid);

            if (term.Trim()!="")
            {
                lst = (from c in loc   where c.ZoneName.Contains(term) orderby c.ZoneName select c).ToList();
            }
            else
            {
                lst = (from c in loc orderby c.ZoneName select c).ToList();
            }
            //foreach (var item in loc)
            //{
            //    lst.Add(new Zones { ZoneID = item.ZoneChartID, ZoneName = item.ZoneName });

            //}
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetZoneByCustomerold(int contractid)
        {
            List<Zones> lst = new List<Zones>();
            var loc = db.GetZoneChartByCustomer(contractid);

            foreach (var item in loc)
            {
                lst.Add(new Zones { ZoneID = item.ZoneChartID, ZoneName = item.ZoneName });

            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public class Zones
        {
            public int ZoneID { get; set; }
            public string ZoneName { get; set; }           
        }
        public ActionResult RateChartPrint(int id)
        {
            ViewBag.ReportName = "Rate Chart Printing";
            AccountsReportsDAO.RateChartPrintReport(id);            
            return View();

        }
    }
}