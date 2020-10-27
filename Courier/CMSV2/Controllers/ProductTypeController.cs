using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using CMSV2.DAL;
namespace CMSV2.Controllers
{
    [SessionExpireFilter]
    public class ProductTypeController : Controller
    {
        private Entities1 db = new Entities1();

       

        public ActionResult Index()
        {
            List<ProductTypeVM> lst = new List<ProductTypeVM>();
            var data = (from c in db.ProductTypes select c).ToList();

            foreach (var item in data)
            {
                ProductTypeVM v = new ProductTypeVM();
                v.ProductTypeID = item.ProductTypeID;
                v.ProductName = item.ProductName;
              
                lst.Add(v);
            }
            return View(lst);
        }

       

        public ActionResult Create()
        {
            ViewBag.ParcelType = db.ParcelTypes.ToList();
          
            ViewBag.transport = db.TransportModes.ToList();
         
            return View();
        }

   

        [HttpPost]
        public ActionResult Create(ProductTypeVM v)
        {

            ProductType a = new ProductType();
            if (ModelState.IsValid)
            {

                int max = (from c in db.ProductTypes orderby c.ProductTypeID descending select c.ProductTypeID).FirstOrDefault();
                a.ProductTypeID = max + 1;
                a.ProductName = v.ProductName;
            
                a.ParcelTypeID = v.ParcelType;
           
                a.TransportModeID = v.TransportModeID;
                a.CBMBasedCharges = v.CBMbasedCharges;
                a.Length = v.Length;
                a.Width = v.Width;
                a.Height = v.Height;
                a.CBM = v.CBM;
                db.ProductTypes.Add(a);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Product Type.";
                return RedirectToAction("Index");

            }
          
            return View(v);
        }

       

        public ActionResult Edit(int id = 0)
        {
            ViewBag.ParcelType = db.ParcelTypes.ToList();

            ViewBag.transport = db.TransportModes.ToList();

            ProductTypeVM v = new ProductTypeVM();
            ProductType a = (from c in db.ProductTypes where c.ProductTypeID == id select c).FirstOrDefault();

            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                v.ProductTypeID = a.ProductTypeID;
                v.ProductName = a.ProductName;

                v.ParcelType = a.ParcelTypeID;

                v.TransportModeID = a.TransportModeID;
                v.CBMbasedCharges = a.CBMBasedCharges;
                v.Length = a.Length.Value;
                v.Width = a.Width.Value;
                v.Height = a.Height.Value;
                v.CBM = a.CBM.Value;
            }
            return View(v);
        }

       
      

        [HttpPost]
        public ActionResult Edit(ProductTypeVM a)
        {

            if (ModelState.IsValid)
            {
                ProductType v = new ProductType();

                v.ProductTypeID = a.ProductTypeID;
                v.ProductName = a.ProductName;

                v.ParcelTypeID = a.ParcelType;

                v.TransportModeID = a.TransportModeID;
                v.CBMBasedCharges = a.CBMbasedCharges;
                v.Length = a.Length;
                v.Width = a.Width;
                v.Height = a.Height;
                v.CBM = a.CBM;

                db.Entry(v).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated Product Type.";
                return RedirectToAction("Index");
            }
            return View();
        }

       

      
        public ActionResult DeleteConfirmed(int id)
        {
            ProductType a = db.ProductTypes.Find(id);
            db.ProductTypes.Remove(a);
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully Deleted Product Type.";
            return RedirectToAction("Index");
        }

     
    }
}