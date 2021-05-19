using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.DAL;
using CMSV2.Models;

namespace CMSV2.Controllers
{
    [SessionExpireFilter]
    public class ForwardingAgentController : Controller
    {
        Entities1  db = new Entities1();
         

         public ActionResult Home()
         {
             //var Query = (from t in db.Menus where t.IsAccountMenu.Value == false orderby t.MenuOrder select t).ToList();
            var Query = (from t in db.Menus join t1 in db.MenuAccessLevels on t.MenuID equals t1.MenuID where t1.RoleID == 14 && t.IsAccountMenu.Value == false orderby t.MenuOrder select t).ToList();

            var Query1 = (from t in db.Menus join t1 in db.MenuAccessLevels on t.MenuID equals t1.ParentID where t1.RoleID == 14 && t.IsAccountMenu.Value == false orderby t.MenuOrder select t).ToList();

            foreach (Menu q in Query)
            {
                Query1.Add(q);
            }
            Session["Menu"] = Query1;
            ViewBag.UserName = SourceMastersModel.GetUserFullName(Convert.ToInt32(Session["UserId"].ToString()), Session["UserType"].ToString());
            return View();

         }

        public ActionResult Index()
        {
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            List<AgentVM> lst = new List<AgentVM>();
            //t.BranchID == BranchID
            lst = (from t in db.ForwardingAgentMasters select new AgentVM { ID = t.FAgentID, AgentName = t.FAgentName, AgentCode = t.FAgentCode, Phone = t.Phone, Fax = t.Fax }).ToList();

          
            return View(lst);
        }

       

        public ActionResult Details(int id = 0)
        {
            tblAgent tblagent = db.tblAgents.Find(id);
            if (tblagent == null)
            {
                return HttpNotFound();
            }
            return View(tblagent);
        }

      

        public ActionResult Create()
        {
            //ViewBag.country = db.CountryMasters.ToList();
            //ViewBag.city = db.CityMasters.ToList();
            //ViewBag.location = db.LocationMasters.ToList();
            ViewBag.currency = db.CurrencyMasters.ToList();
            ViewBag.zonecategory = db.ZoneCategories.ToList();
            ViewBag.achead = db.AcHeads.ToList();
            ViewBag.roles = db.RoleMasters.ToList();
            FAgentVM v = new FAgentVM();
          
            v.FAgentID = 0;
            v.StatusActive = true;
            return View(v);
        }



        [HttpPost]

        public ActionResult Create(FAgentVM item)
        {
            int companyId = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int? max = (from c in db.AgentMasters orderby c.AgentID descending select c.AgentID).FirstOrDefault();
            ForwardingAgentMaster a = new ForwardingAgentMaster();
            PickupRequestDAO _dao = new PickupRequestDAO();
         
            if (max == null || max==0)
            {

                a.FAgentID = 1;
                a.FAgentName = item.FAgentName;
                a.FAgentCode = item.FAgentCode;
                a.Address1 = item.Address1;
                a.Address2 = item.Address2;
                a.Address3 = item.Address3;
                a.Phone = item.Phone;
                a.Fax = item.Fax;
                a.WebSite = item.WebSite;
                a.ContactPerson = item.ContactPerson;
                a.AcCompanyID = companyId;                
                a.CurrencyID = Convert.ToInt32(item.CurrencyID);
                a.ZoneCategoryID = item.ZoneCategoryID;
                a.AcHeadID = item.AcHeadID;
                a.CountryName = item.CountryName;
                a.CityName = item.CityName;
                a.LocationName = item.LocationName;
                //a.CreditLimit = item.CreditLimit;
                //a.CountryName = item.CountryName;
                //a.CityName = item.CityName;
                //a.LocationName = item.LocationName;
                               
                
                //a.UserID = u.UserID;
                
                if (item.StatusActive == null)
                    a.StatusActive = false;
                else
                    a.StatusActive = Convert.ToBoolean(item.StatusActive);

               // a.BranchID = BranchID;
                a.AcHeadID = item.AcHeadID;
            }
            else
            {
                 a.FAgentID = Convert.ToInt32(max) + 1;
                a.FAgentName = item.FAgentName;
                a.FAgentCode = item.FAgentCode;
                a.Address1 = item.Address1;
                a.Address2 = item.Address2;
                a.Address3 = item.Address3;
                a.Phone = item.Phone;
                a.Fax = item.Fax;
                a.WebSite = item.WebSite;
                a.ContactPerson = item.ContactPerson;                
                a.AcCompanyID = companyId;
                a.CountryName = item.CountryName;
                a.CityName = item.CityName;
                a.LocationName = item.LocationName;
                a.CurrencyID = Convert.ToInt32(item.CurrencyID);
                a.ZoneCategoryID = item.ZoneCategoryID;
                a.AcHeadID = item.AcHeadID;
              
                a.Email = item.Email;              
               
                a.AcHeadID = item.AcHeadID;
                //a.UserID = u.UserID;
                //a.BranchID = BranchID;
                if (item.StatusActive == null)
                    a.StatusActive = false;
                else
                    a.StatusActive = Convert.ToBoolean(item.StatusActive);

            }

            try
            {
                db.ForwardingAgentMasters.Add(a);
                db.SaveChanges();
               

                TempData["SuccessMsg"] = "You have successfully added Forwarding Agent.";
                return RedirectToAction("Index");

            }

            catch(Exception ex )
            {
                ViewBag.currency = db.CurrencyMasters.ToList();
                ViewBag.zonecategory = db.ZoneCategories.ToList();
                ViewBag.achead = db.AcHeads.ToList();
                ViewBag.roles = db.RoleMasters.ToList();
                TempData["WarningMsg"] = ex.Message;
                return View(item);
            }
                                                 
                
                        


        }
        


        public ActionResult Edit(int id)
        {
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            FAgentVM a = new FAgentVM();                      

            var item = (from c in db.ForwardingAgentMasters where c.FAgentID == id select c).FirstOrDefault();

            //ViewBag.country = db.CountryMasters.ToList();
            //ViewBag.city = (from c in db.CityMasters where c.CountryID == item.CountryID select c).ToList();
            //ViewBag.location = (from c in db.LocationMasters where c.CityID == item.CityID select c).ToList();
            ViewBag.currency = db.CurrencyMasters.ToList();
            ViewBag.zonecategory = db.ZoneCategories.ToList();
            ViewBag.achead = db.AcHeads.ToList();
            ViewBag.roles = db.RoleMasters.ToList();

            if (item == null)
            {
                return HttpNotFound();
            }
            else
            {
                //a.Fagent = item.AgentID;
                a.FAgentName = item.FAgentName;
                a.FAgentCode = item.FAgentCode;
                a.Address1 = item.Address1;
                a.Address2 = item.Address2;
                a.Address3 = item.Address3;
                a.Phone = item.Phone; 
                a.Fax = item.Fax;
                a.WebSite = item.WebSite;
                a.ContactPerson = item.ContactPerson;
                a.CountryName = item.CountryName;
                a.CityName = item.CityName;
                a.LocationName = item.LocationName;
                a.CurrencyID = item.CurrencyID;
                //a.ZoneCategoryID = item.ZoneCategoryID;
                a.AcHeadID =Convert.ToInt32(item.AcHeadID);

              
               
                
             

          //      a.BranchID = BranchID;
                a.Email = item.Email;
                if (item.StatusDefault!=null)
                    a.StatusDefault =Convert.ToBoolean(item.StatusDefault);
                a.StatusActive = item.StatusActive;
            }
            return View(a);
        }

       

        [HttpPost]
     
        public ActionResult Edit(FAgentVM item)
        {
            UserRegistration u = new UserRegistration();
            PickupRequestDAO _dao = new PickupRequestDAO();
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int accompanyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());

            ForwardingAgentMaster a = db.ForwardingAgentMasters.Find(item.FAgentID);
            
            a.AcCompanyID = accompanyid;
            a.FAgentName = item.FAgentName;
            a.FAgentCode = item.FAgentCode;
            a.Address1 = item.Address1;
            a.Address2 = item.Address2;
            a.Address3 = item.Address3;
            a.Phone = item.Phone;
            a.Fax = item.Fax;
            a.WebSite = item.WebSite;
            a.ContactPerson = item.ContactPerson;
            a.CountryName = item.CountryName;
            a.CityName = item.CityName;
            a.LocationName = item.LocationName;
            a.CurrencyID = Convert.ToInt32(item.CurrencyID);                      
            
            a.AcHeadID = item.AcHeadID;
            if (item.StatusActive!=null)
                a.StatusActive =Convert.ToBoolean(item.StatusActive);
           if (item.StatusDefault!=null)
                a.StatusDefault  = Convert.ToBoolean(item.StatusDefault);

            db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                
                
                TempData["SuccessMsg"] = "You have successfully Updated Forwarding Agent.";
                return RedirectToAction("Index");
          
            
        }

        [HttpGet]
        public JsonResult GetAgentName()
        {
            var agentlist = (from c1 in db.AgentMasters where c1.StatusActive == true select c1.Name).ToList();

            return Json(new { data = agentlist }, JsonRequestBehavior.AllowGet);

        }
        //
        // POST: /Agent/Delete/5

        public ActionResult DeleteConfirmed(int id)
        {
            ForwardingAgentMaster agentMaster = db.ForwardingAgentMasters.Find(id);
            db.ForwardingAgentMasters.Remove(agentMaster);
            db.SaveChanges();                        
            TempData["SuccessMsg"] = "You have successfully Deleted Forwarding Agent.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}