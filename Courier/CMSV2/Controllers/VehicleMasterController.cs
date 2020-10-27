using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;

namespace CMSV2.Controllers
{
    public class VehicleMasterController : Controller
    {
         Entities1 db = new Entities1();

       

        public ActionResult Index()
        {
            List<VehiclesVM> lst = new List<VehiclesVM>();
            var data = db.VehicleMasters.ToList();

            foreach (var item in data)
            {
                VehiclesVM v = new VehiclesVM();

                v.VehicleID = item.VehicleID;
                v.VehicleDescription = item.VehicleDescription;
                v.RegistrationNo = item.RegistrationNo;
                v.Model = item.Model;
                v.VehicleNO = item.VehicleNo;
                lst.Add(v);
            }


            return View(lst);
        }

        //
        // GET: /VehicleMaster/Details/5

        public ActionResult Details(int id = 0)
        {
            VehicleMaster vehiclemaster = db.VehicleMasters.Find(id);
            if (vehiclemaster == null)
            {
                return HttpNotFound();
            }
            return View(vehiclemaster);
        }

        //
        // GET: /VehicleMaster/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /VehicleMaster/Create

        [HttpPost]
        public ActionResult Create(VehiclesVM vm)
        {
            if (ModelState.IsValid)
            {
                VehicleMaster v = new VehicleMaster();

                int max = (from d in db.VehicleMasters orderby d.VehicleID descending select d.VehicleID).FirstOrDefault();

                if (max == null)
                {
                    v.VehicleID = 1;
                    v.VehicleDescription = vm.VehicleDescription;
                    v.RegistrationNo = vm.RegistrationNo;
                    v.Model = vm.Model;
                    v.Type = vm.Type;
                    v.VehicleValue = vm.VehicleValue;
                    v.ValueDate = vm.ValueDate;
                    v.PurchaseDate = vm.PurchaseDate;
                    v.RegExpirydate = vm.RegExpirydate;
                    v.AcCompanyID = 1;
                    v.VehicleNo = vm.VehicleNO;

                }
                else
                {
                    v.VehicleID = max + 1;
                    v.VehicleDescription = vm.VehicleDescription;
                    v.RegistrationNo = vm.RegistrationNo;
                    v.Model = vm.Model;
                    v.Type = vm.Type;
                    v.VehicleValue = vm.VehicleValue;
                    v.ValueDate = vm.ValueDate;
                    v.PurchaseDate = vm.PurchaseDate;
                    v.RegExpirydate = vm.RegExpirydate;
                    v.AcCompanyID = 1;
                    v.VehicleNo = vm.VehicleNO;
                }


                db.VehicleMasters.Add(v);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Vehicle.";
                return RedirectToAction("Index");
            }

            return View();
        }

        //
        // GET: /VehicleMaster/Edit/5

        public ActionResult Edit(int id)
        {
            VehiclesVM v = new VehiclesVM();
            var data = (from d in db.VehicleMasters where d.VehicleID == id select d).FirstOrDefault();

            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                v.VehicleID = data.VehicleID;
                v.VehicleDescription = data.VehicleDescription;
                v.RegistrationNo = data.RegistrationNo;
                v.Model = data.Model;
                v.Type = data.Type;
                v.VehicleValue = data.VehicleValue.Value;
                v.ValueDate = data.ValueDate.Value;
                v.PurchaseDate = data.PurchaseDate.Value;
                v.RegExpirydate = data.RegExpirydate.Value;
                v.AcCompanyID = 1;
                v.VehicleNO = data.VehicleNo;
            }
            return View(v);
        }

        //
        // POST: /VehicleMaster/Edit/5

        [HttpPost]
        public ActionResult Edit(VehiclesVM data)
        {
            VehicleMaster v = new VehicleMaster();
            v.VehicleID = data.VehicleID;
                v.VehicleDescription = data.VehicleDescription;
                v.RegistrationNo = data.RegistrationNo;
                v.Model = data.Model;
                v.Type = data.Type;
                v.VehicleValue = data.VehicleValue;
                v.ValueDate = data.ValueDate;
                v.PurchaseDate = data.PurchaseDate;
                v.RegExpirydate = data.RegExpirydate;
                v.AcCompanyID = 1;
                v.VehicleNo = data.VehicleNO;

            if (ModelState.IsValid)
            {
                db.Entry(v).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated Vehicle.";
                return RedirectToAction("Index");
            }
            return View();
        }

      
        public ActionResult DeleteConfirmed(int id)
        {
            VehicleMaster vehiclemaster = db.VehicleMasters.Find(id);
            db.VehicleMasters.Remove(vehiclemaster);
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully Deleted Vehicle.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}