using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;

namespace CMSV2.Controllers
{
    public class ManifestController : Controller
    {
        Entities1 db = new Entities1();


        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetManifest(DateTime tdate)
        {
            string view = "";
            List<ManifestVM> lst = new List<ManifestVM>();

            var data = db.InScans.Where(x => x.InScanDate == tdate).ToList();

            foreach (var item in data)
            {
                ManifestVM v=new ManifestVM();
                v.Shipper = item.Consignor;
                v.date = item.InScanDate;
                v.awbno = item.AWBNo;
                v.consignee = item.Consignee;
                v.destination = item.Destination;
                v.noofpcs = Convert.ToInt32(item.Pieces);
                v.weight = item.StatedWeight.Value;

                int i=(from c in db.ParcelTypes where c.ID==item.ParcelTypeID select c.ParcelTypeID).FirstOrDefault().Value;

                string type="";
                if(i==0)
                {
                    type="Non Docs";
                }
                else{
                    type="Docs";
                }
                v.type = type;
                v.contents = item.CargoDescription;
                v.remark = item.Remarks;

                lst.Add(v);

            }

             view = this.RenderPartialView("ucManifest", lst);
          
            

            //var data = from t in db.AcJournalMasters select t;
            
            return new JsonResult
            {
                Data = new
                {
                    success = true,
                    view = view
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

    }
}

public static class MvcHelpers1
{
    public static string RenderPartialView(this Controller controller, string viewName, object model)
    {
        if (string.IsNullOrEmpty(viewName))
            viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

        controller.ViewData.Model = model;
        using (var sw = new StringWriter())
        {
            ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
            var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
            viewResult.View.Render(viewContext, sw);

            return sw.GetStringBuilder().ToString();
        }
    }
}
