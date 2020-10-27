using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;

namespace CMSV2.Controllers
{
    public class ForwardingAgentAssignController : Controller
    {
        Entities1 db = new Entities1();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            int depot = Convert.ToInt32(Session["depotcountry"].ToString());
            ViewBag.Country = (from c in db.CountryMasters where c.CountryID == depot select c.CountryName).FirstOrDefault();
            ViewBag.FAgent = db.ForwardingAgentMasters.ToList();
            return View();

        }
        public JsonResult GetAWBData(DateTime startdate,DateTime enddate)
        {
            DateTime s = Convert.ToDateTime(startdate);
            DateTime e = Convert.ToDateTime(enddate);
            //var l = (from c in db.InScans where c.InScanDate >= s  && c.InScanDate <= e select c).ToList();
            var l = db.InScans.ToList();
            List<InScanDetail> list = new List<InScanDetail>();

            foreach(var data in l)
            {
                InScanDetail obj = new InScanDetail();
                obj.InScanID = data.InScanID;
                obj.AWB = data.AWBNo;
                obj.Consignee = data.Consignee;
                obj.City = data.ConsigneeCityID.ToString();
                obj.Phone = data.ConsigneePhone;
                obj.Address = data.ConsigneeAddress;
                if (data.StatedWeight != null)
                {
                    obj.Weight = data.StatedWeight.Value;
                }
                else
                {
                    obj.Weight = 0;
                }
                obj.ForwardingCharge = 0;

                list.Add(obj);
            }



            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(FAgentAssignVM v)
        {
            int x = 0;
            return View();
        }

        public class InScanDetail
        {
            public int InScanID { get; set; }
            public string AWB { get; set; }
            public string Consignee { get; set; }
            public string City { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public double ForwardingCharge { get; set; }
            public double Weight { get; set; }
        }

    }
}
