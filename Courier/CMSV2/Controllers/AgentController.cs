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
    public class AgentController : Controller
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
            lst = (from t in db.AgentMasters select new AgentVM { ID = t.AgentID, AgentName = t.Name, AgentCode = t.AgentCode, Phone = t.Phone, Fax = t.Fax }).ToList();

             //var data = db.EmployeeMasters.ToList();
            ////List<LocationVM> ls = (from c in db.CityMasters on c.CityID join t in db.CountryMasters join l in db.LocationMasters on c.CountryID equals t.CountryID select new LocationVM { CityID = c.CityID,  City = c.City, Country = t.CountryName }).ToList();
            //var query = (from t in db.CityMasters
            //             join t1 in db.LocationMasters on t.CityID equals t1.CityID
            //             join t3 in db.CountryMasters on t.CountryID equals t3.CountryID
            //             select new LocationVM

            //             {
            //                 CountryID = t.CountryID.Value,
            //                 CityID = t1.CityID.Value,
            //                 LocationID =t1.LocationID,
            //                 Location=t1.Location
                             



            //             }).ToList();
            //foreach (var item in data)
            //{
            //    AgentVM a = new AgentVM();


            //    a.EmployeeID = item.EmployeeID;
            //    a.EmployeeName = item.EmployeeName;
            //    a.EmployeeCode = item.EmployeeCode;
            //    a.Address1 = item.Address1;
            //    a.Address2 = item.Address2;
            //    a.Address3 = item.Address3;
            //    a.Phone = item.Phone;
            //    a.Fax = item.Fax;
            //    a.WebSite = item.WebSite;
            //    a.ContactPerson = item.ContactPerson;
            //    a.CountryID = item.CountryID;
            //    a.CityID = item.CityID;
            //    a.LocationID = item.LocationID;
            //    a.CurrencyID = item.CurrencyID;
            //    a.ZoneCategoryID = item.ZoneCategoryID;
            //    a.AcHeadID = item.AcHeadID;
            //    a.CreditLimit = item.CreditLimit;

            //    a.Email = item.Email;
            //    a.Password = item.Password;
            //     lst.Add(a);
            //}
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
            AgentVM v = new AgentVM();
            v.sCreditLimit = "0";
            v.ID = 0;
            v.StatusActive = true;
            return View(v);
        }



        [HttpPost]

        public ActionResult Create(AgentVM item)
        {
            int companyId = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int? max = (from c in db.AgentMasters orderby c.AgentID descending select c.AgentID).FirstOrDefault();
            AgentMaster a = new AgentMaster();
            PickupRequestDAO _dao = new PickupRequestDAO();
            int roleid = db.RoleMasters.Where(t => t.RoleName == "Agent").FirstOrDefault().RoleID;

            UserRegistration u = new UserRegistration();

            UserRegistration x = (from b in db.UserRegistrations where b.UserName == item.Email select b).FirstOrDefault();
            if (x == null)
            {

                int max1 = (from c1 in db.UserRegistrations orderby c1.UserID descending select c1.UserID).FirstOrDefault();
                
                u.UserID = max1 + 1;
                u.UserName = item.Email;
                u.EmailId = item.Email;
                
                if (item.Password == null)
                    u.Password = _dao.RandomPassword(6);
                else
                    u.Password = item.Password;

                u.Phone = item.Phone;
                u.IsActive = true;
                u.RoleID = roleid;
                
                db.UserRegistrations.Add(u);
                db.SaveChanges();
            }

            if (max == null || max==0)
            {

                a.AgentID = 1;
                a.Name = item.AgentName;
                a.AgentCode = item.AgentCode;
                a.Address1 = item.Address1;
                a.Address2 = item.Address2;
                a.Address3 = item.Address3;
                a.Phone = item.Phone;
                a.Fax = item.Fax;
                a.WebSite = item.WebSite;
                a.ContactPerson = item.ContactPerson;
                a.AcCompanyID = companyId;
                //a.CountryID = item.CountryID;
                //a.CityID = item.CityID;
                //a.LocationID = item.LocationID;
                a.CurrencyID = item.CurrencyID;
                a.ZoneCategoryID = item.ZoneCategoryID;
                a.AcHeadID = item.AcHeadID;
                a.CreditLimit = item.CreditLimit;
                a.CountryName = item.CountryName;
                a.CityName = item.CityName;
                a.LocationName = item.LocationName;
                a.Email = item.Email;
                a.Password = "";
                a.AgentType = item.AgentType;
                a.UserID = u.UserID;
                
                if (item.StatusActive == null)
                    a.StatusActive = false;
                else
                    a.StatusActive = Convert.ToBoolean(item.StatusActive);

                a.BranchID = BranchID;
                a.AcHeadID = item.AcHeadID;
            }
            else
            {
                 a.AgentID = Convert.ToInt32(max) + 1;
                a.Name = item.AgentName;
                a.AgentCode = item.AgentCode;
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
                a.CurrencyID = item.CurrencyID;
                a.ZoneCategoryID = item.ZoneCategoryID;
                a.AcHeadID = item.AcHeadID;
                a.CreditLimit = item.CreditLimit;

                a.Email = item.Email;
                a.Password = "";
                a.AgentType = item.AgentType;
                a.AcHeadID = item.AcHeadID;
                a.UserID = u.UserID;
                a.BranchID = BranchID;
                if (item.StatusActive == null)
                    a.StatusActive = false;
                else
                    a.StatusActive = Convert.ToBoolean(item.StatusActive);

            }

            try
            {
                db.AgentMasters.Add(a);
                db.SaveChanges();

                if (item.EmailNotify == true)
                {
                    EmailDAO _emaildao = new EmailDAO();
                    _emaildao.SendCustomerEmail(item.Email, item.Email, u.Password);

                }

                TempData["SuccessMsg"] = "You have successfully added Agent.";
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

            AgentVM a = new AgentVM();                      

            var item = (from c in db.AgentMasters where c.AgentID == id select c).FirstOrDefault();

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
                a.ID = item.AgentID;
                a.AgentName = item.Name;
                a.AgentCode = item.AgentCode;
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
                a.ZoneCategoryID = item.ZoneCategoryID;
                a.AcHeadID = item.AcHeadID;

                a.CreditLimit = item.CreditLimit;
                decimal cvalue = 0;
                if (item.CreditLimit != null)
                    cvalue = Convert.ToDecimal(item.CreditLimit);

                a.sCreditLimit = string.Format("{0:#,0}", cvalue);

                a.BranchID = BranchID;
                a.Email = item.Email;
                

                if (item.UserID!=0)
                {
                    var user = db.UserRegistrations.Where(cc => cc.UserID == item.UserID).FirstOrDefault();

                    if (user != null)
                    {
                        a.RoleID = Convert.ToInt32(user.RoleID);
                        a.Password = user.Password;
                    }
                }
                else
                {
                    a.UserID = 0;
                    a.Password = "";
                }

                a.AgentType = item.AgentType;
                a.StatusActive = item.StatusActive;
            }
            return View(a);
        }

       

        [HttpPost]
     
        public ActionResult Edit(AgentVM item)
        {
            UserRegistration u = new UserRegistration();
            PickupRequestDAO _dao = new PickupRequestDAO();
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int accompanyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());

            AgentMaster a = db.AgentMasters.Find(item.ID);
            
            a.AcCompanyID = accompanyid;
            a.Name = item.AgentName;
            a.AgentCode = item.AgentCode;
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
           // a.ZoneCategoryID = item.ZoneCategoryID;
            //a.AcHeadID = item.AcHeadID;
            a.CreditLimit = item.CreditLimit;
            a.AgentType = item.AgentType;
            a.AcHeadID = item.AcHeadID;
            if (item.StatusActive!=null)
                a.StatusActive =Convert.ToBoolean(item.StatusActive);
            if (a.BranchID==null)
                a.BranchID = BranchID;
          
            UserRegistration x=null;

            if (a.UserID!=null && a.UserID>0)
             x= (from b in db.UserRegistrations where b.UserID == a.UserID select b).FirstOrDefault();

            if (a.Email != item.Email || x==null)
            {                            
              
                if (x == null)
                {
                    
                    int max1 = (from c1 in db.UserRegistrations orderby c1.UserID descending select c1.UserID).FirstOrDefault();
                   
                    int roleid = db.RoleMasters.Where(t => t.RoleName == "Agent").FirstOrDefault().RoleID;
                    u.UserID = max1 + 1;
                    u.UserName = item.Email;
                    u.EmailId = item.Email;

                    if (item.Password == "" || item.Password==null)
                        u.Password = _dao.RandomPassword(6);
                    else
                        u.Password = item.Password;
                    u.Phone = item.Phone;
                    u.IsActive = true;
                    u.RoleID = roleid;
                    db.UserRegistrations.Add(u);
                    db.SaveChanges();

                    a.Email = item.Email;
                    a.UserID = u.UserID;
                }
                else
                {
                    //checking duplicate
                    UserRegistration x1 = (from b in db.UserRegistrations where b.UserName == item.Email && b.UserID!=a.UserID select b).FirstOrDefault();
                    if (x1 == null)
                    {
                        x.EmailId = item.Email;
                        if (item.Password == "")
                            x.Password = _dao.RandomPassword(6);
                        else
                            x.Password = item.Password;

                        db.Entry(x).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    a.Email = item.Email;
                    a.UserID = x.UserID;
                }

            
            }
            else
            {
                if (a.UserID == null || a.UserID == 0)
                {
                    int max1 = (from c1 in db.UserRegistrations orderby c1.UserID descending select c1.UserID).FirstOrDefault();

                    int roleid = db.RoleMasters.Where(t => t.RoleName == "Agent").FirstOrDefault().RoleID;
                    u.UserID = max1 + 1;
                    u.UserName = item.Email;
                    u.EmailId = item.Email;
                    if (item.Password == "")
                        u.Password = _dao.RandomPassword(6);
                    else
                        u.Password = item.Password;
                    u.Phone = item.Phone;
                    u.IsActive = true;
                    u.RoleID = roleid;
                    db.UserRegistrations.Add(u);
                    db.SaveChanges();
                    a.UserID = u.UserID;
                }
                else
                {
                    u = (from b in db.UserRegistrations where b.UserID == a.UserID select b).FirstOrDefault();
                    if (item.Password != u.Password)
                        u.Password = item.Password;

                    db.Entry(u).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            
                        

            if (ModelState.IsValid)
            {
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                
                if (item.EmailNotify == true)
                {
                    EmailDAO _emaildao = new EmailDAO();
                    _emaildao.SendCustomerEmail(item.Email, item.Email, u.Password);

                }
                TempData["SuccessMsg"] = "You have successfully Updated Agent.";
                return RedirectToAction("Index");
            }
            return View();
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
            AgentMaster agentMaster = db.AgentMasters.Find(id);
            UserRegistration a = (from c in db.UserRegistrations where c.UserID == agentMaster.UserID select c).FirstOrDefault();
            db.AgentMasters.Remove(agentMaster);
            db.SaveChanges();

            if (a != null)
            {
                db.UserRegistrations.Remove(a);
            }
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully Deleted Agent.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}