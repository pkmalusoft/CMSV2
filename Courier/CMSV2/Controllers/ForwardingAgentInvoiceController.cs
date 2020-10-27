using CMSV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSV2.Controllers
{
    public class ForwardingAgentInvoiceController : Controller
    {
        Entities1 db = new Entities1();

        public ActionResult Index()
        {
            List<ForwardingAgentInvoiceVM> lst = new List<ForwardingAgentInvoiceVM>();
            var data = db.FAInvoices.ToList();
          
            var query = (from t in db.FAInvoices
                         //join t1 in db.FAInvoiceDetails on t.FAInvoiceDetails equals t1.FAInvoiceDetailID
                         join t3 in db.FuelSurcharges on t.FAgentID equals t3.FAgentID
                         select new ForwardingAgentInvoiceVM

                         {
                             FAInvoiceID = t.FAInvoiceID,
                             //FAInvoiceDetailID=t.FAInvoiceDetails,
                             FAgentID = t.FAgentID,
                             





                         }).ToList();





            return View(lst);
        }
        public ActionResult Create()
        {
            ViewBag.Column = db.FAInvoiceDetails.ToList();
            ViewBag.Value = db.FuelSurcharges.ToList();
            var a = db.FAInvoices.ToList();
            ViewBag.FAInvoiceID = db.FAInvoices.ToList();
            ViewBag.ID = db.FuelSurcharges.ToList();
            //return RedirectToAction("Index");
            return View();
        }

        [HttpPost]
        public ActionResult Create(ForwardingAgentInvoiceVM v)
        {
            FAInvoice  FAIn = new FAInvoice();
            

            FAIn.FAInvoiceNo = v.FAInvoiceNo;
            FAIn.FAInvoiceDate = v.FAInvoiceDate;


            FAIn.FAInvoiceDate = v.FAInvoiceDate;

            //FAIn.FromDate = v.FromDate;
            //foreach (var item in v.FAInvoiceDetail)
            //{
            //    FAInvoiceDetail a = new FAInvoiceDetail();

            //    a.FuelSurchargePer = item.FuelSurchargePer;
            //    a.AdditionalWeightFrom = item.AddWtFrom;
            //    a.AdditionalWeightTo = item.AddWtTo;
            //    a.IncrementalWeight = item.IncrWt;
            //    a.AdditionalRate = item.AddRate;

            //    db.ForwardingAgentRateDets.Add(a);


            //    db.SaveChanges();
            //}

            return View();
 
        }
    }
}
