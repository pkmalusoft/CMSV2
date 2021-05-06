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
    [SessionExpire]
    //[Authorize]
    public class SupplierController : Controller
    {
        Entities1 db = new Entities1();                
        SourceMastersModel ObjectSourceModel = new SourceMastersModel();
        //
        // GET: /Supplier/

        public ActionResult Index()
        {
            return View(db.SupplierMasters.OrderBy(x => x.SupplierName).ToList());
        }

        //
        // GET: /Supplier/Details/5

        public ActionResult Details(int id = 0)
        {
            SupplierMaster supplier = db.SupplierMasters.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        //
        // GET: /Supplier/Create

        public ActionResult Create()
        {
            //var maximumcust = (from d in db.SupplierMasters orderby d.SupplierID descending select d.ReferenceCode).FirstOrDefault();
            //var custnum = maximumcust.ReferenceCode.Substring(maximumcust.ReferenceCode.Length - 5);
            //ViewBag.custnum = Convert.ToInt32(custnum) + 1;
            //ViewBag.country = DropDownList<CountryMaster>.LoadItems(
            //    ObjectSourceModel.GetCountry(), "CountryID", "CountryName");
            var supplierMasterTypes = (from d in db.SupplierTypes select d).ToList();
            ViewBag.SupplierType = supplierMasterTypes;
          
            var data = db.RevenueTypes.ToList();
            ViewBag.revenue = data;
            return View();
        }
        public static class DropDownList<T>
        {
            public static SelectList LoadItems(IList<T> collection, string value, string text)
            {
                return new SelectList(collection, value, text);
            }
        }
        //
        // POST: /Supplier/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SupplierMaster supplier)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            //if (ModelState.IsValid)
            //{
            //ViewBag.country = DropDownList<CountryMaster>.LoadItems(

            //ObjectSourceModel.GetCountry(), "CountryID", "CountryName");

            var supplierMasterTypes = (from d in db.SupplierTypes select d).ToList();
            ViewBag.SupplierType = supplierMasterTypes;
          
                var query = (from t in db.SupplierMasters where t.SupplierName == supplier.SupplierName select t).ToList();

                if (query.Count > 0)
                {

                    ViewBag.SuccessMsg = "Supplier name is already exist";
                    return View();
                }
                supplier.BranchID = branchid;
                supplier.AcCompanyID = companyid;
                //supplier.SupplierID = ObjectSourceModel.GetMaxNumberSupplier();
                db.SupplierMasters.Add(supplier);
                db.SaveChanges();
                ReceiptDAO.ReSaveSupplierCode();
                ViewBag.SuccessMsg = "You have successfully added Supplier.";
                return RedirectToAction("Index");
            //}

            
        }

        //
        // GET: /Supplier/Edit/5

        public ActionResult Edit(int id = 0)
        {

            var data = db.RevenueTypes.ToList();
            ViewBag.revenue = data;
            SupplierMaster supplier = db.SupplierMasters.Find(id);
            
            var supplierMasterTypes = (from d in db.SupplierTypes  select d).ToList();
            ViewBag.SupplierType = supplierMasterTypes;           
            
            return View(supplier);
        }

        //
        // POST: /Supplier/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SupplierMaster supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.SuccessMsg = "You have successfully updated Supplier.";
                return RedirectToAction("Index");
            }
            else
            {
                var supplierMasterTypes = (from d in db.SupplierTypes select d).ToList();
                ViewBag.SupplierType = supplierMasterTypes;
            }
            return View(supplier);
        }

        //
        // GET: /Supplier/Delete/5

        //public ActionResult Delete(int id = 0)
        //{
        //    Supplier supplier = db.Suppliers.Find(id);
        //    if (supplier == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(supplier);
        //}

        //
        // POST: /Supplier/Delete/5

        public ActionResult DeleteConfirmed(int id)
        {
            //int k = 0;
            if (id != 0)
            {
                DataTable dt = ReceiptDAO.DeleteSupplier(id);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        //if (dt.Rows[0][0] == "OK")
                        TempData["SuccessMsg"] = dt.Rows[0][1].ToString();
                    }

                }
                else
                {
                    TempData["ErrorMsg"] = "Error at delete";
                }
            }

            return RedirectToAction("Index");


        }



        public JsonResult GetID(string supid)
        {
            int sid = Convert.ToInt32(supid);

            string x = (from c in db.SupplierMasters where c.SupplierID == sid select c.RevenueTypeIds).FirstOrDefault();

            return Json(x, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetSupplierCode(string suppliername)
        {
            string status = "ok";
            string customercode = "";
            //List<CourierStatu> _cstatus = new List<CourierStatu>();
            try
            {
                string custform = "000000";
                string maxcustomercode = (from d in db.SupplierMasters orderby d.SupplierID descending select d.ReferenceCode).FirstOrDefault();
                string last6digit = "";
                    if (maxcustomercode == null)
                {
                    //maxcustomercode="AA000000";
                    last6digit = "0";

                }
                else
                {
                    last6digit = maxcustomercode.Substring(maxcustomercode.Length - 6); //, maxcustomercode.Length - 6);
                }
                if (last6digit != "")
                {

                    string customerfirst = suppliername.Substring(0, 1);
                    string customersecond = "";
                    try
                    {
                        customersecond = suppliername.Split(' ')[1];
                        customersecond = customersecond.Substring(0, 1);
                    }
                    catch (Exception ex)
                    {

                    }

                    if (customerfirst != "" && customersecond != "")
                    {
                        customercode = customerfirst + customersecond + String.Format("{0:000000}", Convert.ToInt32(last6digit) + 1);
                    }
                    else
                    {
                        customercode = customerfirst + "S" + String.Format("{0:000000}", Convert.ToInt32(last6digit) + 1);
                    }

                }

                return Json(new { data = customercode, result = status }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                status = ex.Message;
            }

            return Json(new { data = "", result = "failed" }, JsonRequestBehavior.AllowGet);

        }
        public class Rev
        {
            public int RevenueTypeID { get; set; }
            public string RevenueType1 { get; set; }
        }
        public JsonResult GetRevenue()
        {
            List<Rev> lst = new List<Rev>();

            var data = db.RevenueTypes.ToList();

            foreach (var item in data)
            {
                Rev v = new Rev();
                v.RevenueTypeID = item.RevenueTypeID;
                v.RevenueType1 = item.RevenueType1;
                lst.Add(v);

            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SupplierType()
        {
            return View(db.SupplierTypes.OrderBy(x => x.SupplierType1).ToList());
        }
        public ActionResult CreateSupplierType()
        {
            return View();
        }

       
        [HttpPost]
        public ActionResult CreateSupplierType(SupplierType suppliertypemaster)
        {
            if (ModelState.IsValid)
            {
                var query = (from t in db.SupplierTypes where t.SupplierType1 == suppliertypemaster.SupplierType1 select t).ToList();

                if (query.Count > 0)
                {

                    ViewBag.SuccessMsg = "Supplier Type already exist";
                    return View();
                }
                db.SupplierTypes.Add(suppliertypemaster);
                db.SaveChanges();
                ViewBag.SuccessMsg = "You have successfully added SupplierType.";
                return View("SupplierType", db.SupplierTypes.ToList());
            }

            return View(suppliertypemaster);
        }
        public ActionResult EditSupplierType(int id = 0)
        {
            SupplierType Supmaster = db.SupplierTypes.Find(id);
            if (Supmaster == null)
            {
                return HttpNotFound();
            }
            return View(Supmaster);
        }

    

        [HttpPost]
        public ActionResult EditSupplierType(SupplierType SupplierTypemaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(SupplierTypemaster).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.SuccessMsg = "You have successfully updated Role.";
                return View("SupplierType", db.SupplierTypes.ToList());
            }
            return View(SupplierTypemaster);
        }

     
        public ActionResult DeletesupplierTypeConfirmed(int id)
        {
            SupplierType suppliertype = db.SupplierTypes.Find(id);
            db.SupplierTypes.Remove(suppliertype);
            db.SaveChanges();
            ViewBag.SuccessMsg = "You have successfully deleted Supplier Type.";
            return View("SupplierType", db.SupplierTypes.ToList());
        }

        //CreateSupplierType
    }
}