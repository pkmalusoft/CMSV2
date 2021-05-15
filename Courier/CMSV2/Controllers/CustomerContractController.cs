using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using CMSV2.DAL;
using System.Data.Entity;
using Newtonsoft.Json;
namespace CMSV2.Controllers
{
    [SessionExpire]
    public class CustomerContractController : Controller
    {
        private Entities1 db = new Entities1();
        // GET: CustomerContract
        public ActionResult Index()
        {
            List<CustomerContractVM> lst = PickupRequestDAO.GetCustomerContracts(0,"");
            return View(lst);
        }

        public ActionResult Create(int id=0)
        {
            CustomerContractVM vm = new CustomerContractVM();
            if (id>0)
            {
                ViewBag.Title = "Rate Chart - Modify";
                vm.CustomerID = id;
                vm.CustomerName = db.CustomerMasters.Find(id).CustomerName;
                ViewBag.EditMode = "true";
                vm.MovementId = "1,2,3,4";
            }
            else
            {
                ViewBag.Title = "Rate Chart";
                ViewBag.EditMode = "false";
                vm.CustomerID = 0;
                vm.CustomerName = "";
                vm.MovementId = "1,2,3,4";

            }
            return View(vm);
        }

        public JsonResult GetCustomerContract(int id,string CourierType)
        {            
            List<CustomerContractVM> lst = PickupRequestDAO.GetCustomerContracts(id,CourierType);
            return Json(lst, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SaveContract(int CustomerId,string selectedvalues,string Details)
        {
            var IDetails = JsonConvert.DeserializeObject<List<CustomerContractVM>>(Details);
            var data = (from c in db.CustomerMultiContracts where c.CustomerID== CustomerId select c).ToList();
            foreach (var item in data)
            {
                db.CustomerMultiContracts.Remove(item);
                db.SaveChanges();
            }

            foreach (var item in IDetails)
            {
                CustomerMultiContract cm = new CustomerMultiContract();
                cm.CustomerID = CustomerId;
                cm.CustomerRateTypeID = item.CustomerRateTypeID;
                db.CustomerMultiContracts.Add(cm);
                db.SaveChanges();
            }

            return Json(new { status = "ok", message = "You have successfully Saved Customer Contract!" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DeleteConfirmed(int id = 0)
        {
            CustomerMultiContract a = db.CustomerMultiContracts.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.CustomerMultiContracts.Remove(a);
                db.SaveChanges();
                

                return RedirectToAction("Index");
            }
        }
    }
}