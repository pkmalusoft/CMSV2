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
    public class BranchMasterController : Controller
    {
         Entities1 db = new Entities1();

      
        public ActionResult Index()
        {

            //List<BranchVM> lst = (from b in db.BranchMasters join t1 in db.CountryMasters on b.CountryID equals t1.CountryID join t2 in db.CityMasters on b.CityID equals t2.CityID join t3 in db.LocationMasters on b.LocationID equals t3.LocationID join t4 in db.CurrencyMasters on b.CurrencyID equals t4.CurrencyID select new BranchVM {BranchID=b.BranchID,BranchName=b.BranchName,Country=t1.CountryName,City=t2.City,Location=t3.Location,Currency=t4.CurrencyName }).ToList();
            List<BranchVM> lst = (from b in db.BranchMasters join t4 in db.CurrencyMasters on b.CurrencyID equals t4.CurrencyID select new BranchVM { BranchID = b.BranchID, BranchName = b.BranchName, CountryName = b.CountryName, CityName = b.CityName, LocationName = b.LocationName, Currency = t4.CurrencyName }).ToList();



            return View(lst);
        }

        //
        // GET: /BranchMaster/Details/5

        public ActionResult Details(int id = 0)
        {
            BranchMaster branchmaster = db.BranchMasters.Find(id);
            if (branchmaster == null)
            {
                return HttpNotFound();
            }
            return View(branchmaster);
        }

      

        public ActionResult Create()
        {
            ViewBag.years = db.AcFinancialYears.ToList();
            ViewBag.designation = db.Designations.ToList();
            ViewBag.currency = db.CurrencyMasters.ToList();
            return View();
        }

     

        [HttpPost]
  
        public ActionResult Create(BranchVM item)
        {
            if (ModelState.IsValid)
            {
                BranchMaster a = new BranchMaster();

                int max = (from d in db.BranchMasters orderby d.BranchID descending select d.BranchID).FirstOrDefault();



                if (max == null)
                {
                    a.BranchID = 1;
                    a.BranchName = item.BranchName;
                    a.Address1 = item.Address1;
                    a.Address2 = item.Address2;
                    a.Address3 = item.Address3;
                    a.CountryID = 1; // item.CountryID;
                    a.CityID = 19; // item.CityID;
                    a.LocationID = 7; // item.LocationID;
                    a.CountryName = item.CountryName;
                    a.CityName = item.CityName;
                    a.LocationName = item.LocationName;
                    a.KeyPerson = item.KeyPerson;
                    a.DesignationID = item.DesignationID;
                    a.Phone = item.Phone;
                    a.PhoneNo1 = item.PhoneNo1;
                  //  a.PhoneNo2 = item.PhoneNo2;
//                    a.PhoneNo3 = item.PhoneNo3;
  //                  a.PhoneNo4 = item.PhoneNo4;
                    a.MobileNo1 = item.MobileNo1;
    //                a.MobileNo2 = item.MobileNo2;
                   
                    a.EMail = item.EMail;
                   
                    a.Website = item.Website;
                    a.BranchPrefix = item.BranchPrefix;
                    a.CurrencyID = item.CurrencyID;
                    a.AcCompanyID = item.AcCompanyID;
                    a.StatusAssociate = item.StatusAssociate;
                    a.AWBFormat = item.AWBFormat;
                    a.InvoicePrefix = item.InvoicePrefix;
                    a.InvoiceFormat = item.InvoiceFormat;
                    a.AcFinancialYearID = item.AcFinancialYearID;
                }
                else
                {
                    a.BranchID = max + 1;
                    a.BranchName = item.BranchName;
                    a.Address1 = item.Address1;
                    a.Address2 = item.Address2;
                    a.Address3 = item.Address3;
                    //a.CountryID = 1; // item.CountryID;
                    //a.CityID = 19; // item.CityID;
                    //a.LocationID = 7; // item.LocationID;
                    a.LocationName = item.LocationName;
                    a.CityName = item.CityName;
                    a.CountryName = item.CountryName;
                    a.KeyPerson = item.KeyPerson;
                    a.DesignationID = item.DesignationID;
                    a.Phone = item.Phone;
                    a.PhoneNo1 = item.PhoneNo1;
                    //a.PhoneNo2 = item.PhoneNo2;
                    //a.PhoneNo3 = item.PhoneNo3;
                    //a.PhoneNo4 = item.PhoneNo4;
                    a.MobileNo1 = item.MobileNo1;
                    //a.MobileNo2 = item.MobileNo2;
                    a.EMail = item.EMail;
                    a.Website = item.Website;
                    a.BranchPrefix = item.BranchPrefix;
                    a.CurrencyID = item.CurrencyID;
                    a.AcCompanyID = item.AcCompanyID;
                    a.StatusAssociate = item.StatusAssociate;
                    a.AWBFormat = item.AWBFormat;
                    a.InvoicePrefix = item.InvoicePrefix;
                    a.InvoiceFormat = item.InvoiceFormat;
                    a.VATPercent = item.VATPercent;
                    a.VATRegistrationNo = item.VATRegistrationNo;
                    a.AcFinancialYearID = item.AcFinancialYearID;
                }


                db.BranchMasters.Add(a);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Branch.";
                return RedirectToAction("Index");
            }

          
            return View();
        }

       

        public ActionResult Edit(int id)
        {
            ViewBag.years = db.AcFinancialYears.ToList();
            var data = (from c in db.BranchMasters where c.BranchID == id select c).FirstOrDefault();
            BranchVM v = new BranchVM();
            //ViewBag.BranchID = db.BranchMasters.ToList();
            //ViewBag.country = db.CountryMasters.ToList();
            //ViewBag.city = db.CityMasters.ToList().Where(x=>x.CountryID==data.CountryID);
            //ViewBag.location = db.LocationMasters.ToList().Where(x=>x.CityID==data.CityID);
            ViewBag.designation = db.Designations.ToList();
            ViewBag.currency = db.CurrencyMasters.ToList();
           
            if (data == null)
            {
                return HttpNotFound();
            }
             else
            {
                    v.BranchID = data.BranchID;
                    v.BranchName = data.BranchName;   
                    v.Address1 = data.Address1;
                    v.Address2 = data.Address2;
                    v.Address3 = data.Address3;
                    //v.CountryID = data.CountryID.Value;
                    //v.CityID = data.CityID.Value;
                    //v.LocationID = data.LocationID.Value;
                v.LocationName = data.LocationName;
                v.CityName = data.CityName;
                v.CountryName = data.CountryName;
                    v.KeyPerson = data.KeyPerson;
                    v.DesignationID = data.DesignationID.Value;
                    v.Phone = data.Phone;
                    v.PhoneNo1 = data.PhoneNo1;
                    v.PhoneNo2 = data.PhoneNo2;
                    v.PhoneNo3 = data.PhoneNo3;
                    v.PhoneNo4 = data.PhoneNo4;
                    v.MobileNo1 = data.MobileNo1;
                    v.MobileNo2 = data.MobileNo2;
                    v.EMail = data.EMail;
                    v.Website = data.Website;
                    v.BranchPrefix = data.BranchPrefix;
                    v.CurrencyID = data.CurrencyID.Value;
                    v.AcCompanyID = data.AcCompanyID.Value;
                    v.StatusAssociate = data.StatusAssociate.Value;
                    v.AWBFormat = data.AWBFormat;
                    v.InvoicePrefix = data.InvoicePrefix;
                    v.InvoiceFormat = data.InvoiceFormat;
                    v.AcFinancialYearID =Convert.ToInt32(data.AcFinancialYearID);
                if (data.VATPercent == null)
                { v.VATPercent = 0; }
                else
                {
                    v.VATPercent = Convert.ToDecimal(data.VATPercent);
                }

                    v.VATRegistrationNo = data.VATRegistrationNo;
            }
            //ViewBag.CityID = new SelectList(db.CityMasters, "CityID", "City", locationmaster.CityID);
            return View(v);
    
        }

        //
        // POST: /BranchMaster/Edit/5

        [HttpPost]
      
        public ActionResult Edit(BranchVM b)
        {
            BranchMaster a = new BranchMaster();
            a = db.BranchMasters.Find(b.BranchID);
                    //a.BranchID = b.BranchID;
                    a.BranchName = b.BranchName;
                    a.Address1 = b.Address1;
                    a.Address2 = b.Address2;
                    a.Address3 = b.Address3;                    
            //a.CountryID = 1; // item.CountryID;
            //a.CityID = 19; // item.CityID;
            //a.LocationID = 7; // item.LocationID;
            a.CountryName = b.CountryName;
            a.CityName = b.CityName;
            a.LocationName = b.LocationName;
            a.KeyPerson = b.KeyPerson;
                    a.DesignationID = b.DesignationID;
                    a.Phone = b.Phone;
                    a.PhoneNo1 = b.PhoneNo1;
                    //a.PhoneNo2 = b.PhoneNo2;
                    //a.PhoneNo3 = b.PhoneNo3;
                    //a.PhoneNo4 = b.PhoneNo4;
                    a.MobileNo1 = b.MobileNo1;
                    //a.MobileNo2 = b.MobileNo2;
                    a.EMail = b.EMail;
                    a.Website = b.Website;
                    a.BranchPrefix = b.BranchPrefix;
                    a.CurrencyID = b.CurrencyID;
                    a.AcCompanyID = b.AcCompanyID;
                    a.StatusAssociate = b.StatusAssociate;
                    a.AWBFormat = b.AWBFormat;
                    a.InvoicePrefix = b.InvoicePrefix;
                    a.InvoiceFormat = b.InvoiceFormat;
                    a.VATPercent = b.VATPercent;
                    a.VATRegistrationNo = b.VATRegistrationNo;
                    a.AcFinancialYearID = b.AcFinancialYearID;
            


            if (ModelState.IsValid)
            {
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully Updated Branch.";
                return RedirectToAction("Index");
            }
         
            return View();
        
        }

        //
        // GET: /BranchMaster/Delete/5

        //public ActionResult Delete(int id=0)
        //{
        //    BranchMaster branchmaster = db.BranchMasters.Find(id);
        //    if (branchmaster == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(branchmaster);
        //}

        //
        // POST: /BranchMaster/Delete/5

        //[HttpPost, ActionName("Delete")]
      
        public ActionResult DeleteConfirmed(int id)
        {
            BranchMaster branchmaster = db.BranchMasters.Find(id);
            db.BranchMasters.Remove(branchmaster);
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully Deleted Branch.";
            return RedirectToAction("Index");
        }

        public ActionResult GeneralSetup(int id)
        {
            int setuptypeid = 0;
            if (Session["SetupTypeID"] != null)
            {
                setuptypeid = Convert.ToInt32(Session["SetupTypeID"].ToString());
            }
            else
            {
                Session["SetupTypeID"] = 1;
                Session["SetupBranchID"] = id;
                setuptypeid = 1;
            }
            GeneralSetup v = new GeneralSetup();
            GeneralSetupVM vm = new GeneralSetupVM();
            v = db.GeneralSetups.Where(cc => cc.BranchId == id && cc.SetupTypeID == setuptypeid).FirstOrDefault();

            if (v == null)
            {

                vm.SetupID = 0;
                vm.BranchId = id;
                vm.SetupTypeID = setuptypeid; // db.GeneralSetupTypes.FirstOrDefault().ID;
                vm.TextDoc = "";
                vm.Text1 = "";
                vm.Text2 = "";
                vm.Text3 = "";
                vm.Text4 = "";
                vm.Text5 = "";
            }
            else
            {
                vm.SetupID = v.SetupID;
                vm.BranchId = v.BranchId;
                vm.SetupTypeID = v.SetupTypeID;
                vm.TextDoc = v.Text1;
                vm.Text1 = v.Text1;
                vm.Text2 = v.Text2;
                vm.Text3 = v.Text3;
                vm.Text4 = v.Text4;
                vm.Text5 = v.Text5;            
                

            }
            
            ViewBag.SetupType = db.GeneralSetupTypes.ToList();
            return View(vm);

        }

        [HttpPost]
        public ActionResult GeneralSetup(GeneralSetupVM vm)
        {
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            GeneralSetup v = new GeneralSetup();
            v = db.GeneralSetups.Where(cc => cc.BranchId == vm.BranchId && cc.SetupTypeID == vm.SetupTypeID).FirstOrDefault();

            if (v==null)
            {
                v = new GeneralSetup();
                int max = (from d in db.GeneralSetups orderby d.SetupID descending select d.SetupID).FirstOrDefault();
                v.SetupID = max + 1; ;
                v.BranchId = vm.BranchId;
                v.AcCompanyId = companyid;
                v.SetupTypeID = vm.SetupTypeID;
                //v.Text1 = vm.TextDoc;
                if (v.SetupTypeID == 2)
                {
                    v.Text1 = vm.TextDoc;
                }
                else
                {
                    v.Text1 = vm.Text1;
                    v.Text2 = vm.Text2;
                    v.Text3 = vm.Text3;
                    v.Text4 = vm.Text4;
                    v.Text5 = vm.Text5;
                }
                db.GeneralSetups.Add(v);
                db.SaveChanges();
            }
            else
            {
                if (v.SetupTypeID == 2)
                {
                    v.Text1 = vm.TextDoc;
                }
                else
                {
                    v.Text1 = vm.Text1; // Doc;
                    v.Text2 = vm.Text2;
                    v.Text3 = vm.Text3;
                    v.Text4 = vm.Text4;
                    v.Text5 = vm.Text5;
                }

                db.Entry(v).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");

        }

        public ActionResult SetupType()
        {
            GeneralSetupVM vm = new GeneralSetupVM();
            if (Session["SetupTypeID"] !=null) {
                int setuptypeid = Convert.ToInt32(Session["SetupTypeID"].ToString());
                vm.SetupTypeID = setuptypeid;
           }
            int branchid = Convert.ToInt32(Session["SetupBranchId"]);
            ViewBag.SetupType = db.GeneralSetupTypes.ToList();
            ViewBag.BranchName = db.BranchMasters.Find(branchid).BranchName;
            return View(vm);
        }


        [HttpPost]
        public ActionResult SetupType(GeneralSetupVM vm)
        {

            if (vm.SetupTypeID != null)
                Session["SetupTypeID"] = vm.SetupTypeID;

            int branchid = Convert.ToInt32(Session["SetupBranchId"]);

            return RedirectToAction("GeneralSetup", "BranchMaster", new {id= branchid});                      
            
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public JsonResult GetCity(int id)
        {
            List<CityM> objCity = new List<CityM>();
            var city = (from c in db.CityMasters where c.CountryID == id select c).ToList();

            foreach (var item in city)
            {
                objCity.Add(new CityM { City = item.City, CityID = item.CityID });

            }
            return Json(objCity, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocation(int id)
        {
            List<LocationM> objLoc = new List<LocationM>();
            var loc = (from c in db.LocationMasters where c.CityID == id select c).ToList();

            foreach (var item in loc)
            {
                objLoc.Add(new LocationM { Location = item.Location, LocationID = item.LocationID });

            }
            return Json(objLoc, JsonRequestBehavior.AllowGet);
        }

        public class CityM
        {
            public int CityID { get; set; }
            public String City { get; set; }
        }

        public class LocationM
        {
            public int LocationID { get; set; }
            public String Location { get; set; }
        }
    }
}