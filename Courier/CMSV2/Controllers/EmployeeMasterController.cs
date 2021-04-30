using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using CMSV2.DAL;
using System.Data.Entity;

namespace CMSV2.Controllers
{
    [SessionExpireFilter]
    public class EmployeeMasterController : Controller
    {
        Entities1 db = new Entities1();

        public ActionResult Home()
        {
            //
            List<int> RoleId = (List<int>)Session["RoleID"];
            
            int roleid = RoleId[0];

            if (roleid == 1)
            {
                var Query = (from t in db.Menus where t.IsAccountMenu.Value == false && t.RoleID == null orderby t.MenuOrder select t).ToList();
                Session["Menu"] = Query;
                ViewBag.UserName = SourceMastersModel.GetUserFullName(Convert.ToInt32(Session["UserId"].ToString()), Session["UserType"].ToString());
                return View();
            }
            else
            {
                //List<Menu> Query2 = new List<Menu>();
                var Query = (from t in db.Menus join t1 in db.MenuAccessLevels on t.MenuID equals t1.MenuID where t1.RoleID == roleid && t.IsAccountMenu.Value == false orderby t.MenuOrder select t).ToList();

                var Query1 = (from t in db.Menus join t1 in db.MenuAccessLevels on t.MenuID equals t1.ParentID where t1.RoleID == roleid && t.ParentID == 0 && t.IsAccountMenu.Value == false orderby t.MenuOrder select t).ToList();

               var Query2 = (from t in db.Menus join t1 in db.MenuAccessLevels on t.MenuID equals t1.ParentID where t1.RoleID == roleid && t.IsAccountMenu.Value == false orderby t.MenuOrder select t).ToList();

                if (Query2!=null)
                {
                    foreach (Menu q in Query1)
                    {
                        var query3 = Query.Where(cc => cc.MenuID == q.MenuID).FirstOrDefault();
                        if (query3 == null)
                            Query2.Add(q);
                    }
                }

                if (Query1 != null)
                {
                    foreach (Menu q in Query1)
                    {
                        var query3 = Query.Where(cc => cc.MenuID == q.MenuID).FirstOrDefault();
                        if (query3 == null)
                            Query.Add(q);
                    }
                }
                                               
               

                Session["Menu"] = Query;

                ViewBag.UserName = SourceMastersModel.GetUserFullName(Convert.ToInt32(Session["UserId"].ToString()), Session["UserType"].ToString());
                return View();
            }
        }

        public ActionResult Index()
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());

            List<EmployeeVM > lst = (from c in db.EmployeeMasters join t in db.Designations on c.DesignationID equals t.DesignationID where c.UserID!=1 && c.BranchID== branchid && c.DepotID==depotId   select new EmployeeVM {EmployeeID=c.EmployeeID,EmployeeName=c.EmployeeName,EmployeeCode=c.EmployeeCode,Designation=t.Designation1 ,Email=c.Email}).ToList();
            return View(lst);
        }

        public ActionResult Create()
        {
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            var data = db.tblDepots.Where(c => c.BranchID == BranchID).ToList();
            ViewBag.Depot = data;
            //ViewBag.Country = db.CountryMasters.ToList();
            ViewBag.Designation = db.Designations.ToList();
            
            ViewBag.roles = db.RoleMasters.ToList();
            EmployeeVM v = new EmployeeVM();
            v.JoinDate = DateTime.Now.Date;
            v.StatusActive = true;
            ViewBag.AcHead = db.AcHeads.OrderBy(c => c.AcHead1).ToList();
            return View(v);
        }

        [HttpPost]
        public ActionResult Create(EmployeeVM v)
        {
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            PickupRequestDAO _dao = new PickupRequestDAO();
            //if (ModelState.IsValid)
            //{
                EmployeeMaster a = new EmployeeMaster();
                int max = (from c in db.EmployeeMasters orderby c.EmployeeID descending select c.EmployeeID).FirstOrDefault();

                a.EmployeeID = max + 1;
                a.EmployeeName = v.EmployeeName;
                a.EmployeeCode = v.EmployeeCode;
                a.Address1 = v.Address1;
                a.Address2 = v.Address2;
                a.Address3 = v.Address3;
                a.Phone = v.Phone;
               
                a.Fax = v.Fax;
                a.Email = v.Email;
                a.Mobile = v.MobileNo;
                a.AcCompanyID = companyid;     

                a.CountryName = v.CountryName;
                a.CityName = v.CityName;
                
                a.DesignationID = v.DesignationID;
                a.JoinDate = Convert.ToDateTime(v.JoinDate);
                a.BranchID = BranchID;
                a.DepotID = v.Depot;
               // a.Password = v.Password;
                //a.MobileDeviceID = v.MobileDeviceID;
                //a.MobileDevicePwd = v.MobileDevicePWD;
                a.StatusCommission = v.StatusCommision;
                a.statusDefault = v.StatusDefault;
                a.StatusActive = v.StatusActive;
                a.RoleId = v.RoleID;
                a.Type = "E";
            a.AcHeadID = v.AcHeadID;

                       UserRegistration u = new UserRegistration();

            UserRegistration x = (from b in db.UserRegistrations where b.UserName == v.Email select b).FirstOrDefault();
            if (x == null)
            {

                int max1 = (from c1 in db.UserRegistrations orderby c1.UserID descending select c1.UserID).FirstOrDefault();
                u.UserID = max1 + 1;
                u.UserName = v.Email;
                u.EmailId = v.Email;
                u.Password = "12345";
                u.Phone = v.Phone;
                u.IsActive = true;
                u.RoleID = v.RoleID;
                db.UserRegistrations.Add(u);
                db.SaveChanges();
                }
                
                a.UserID = u.UserID;

                db.EmployeeMasters.Add(a);
                db.SaveChanges();



                TempData["SuccessMsg"] = "You have successfully added Employee.";
                return RedirectToAction("Index");
            //}
        
        }

        public ActionResult DeleteConfirmed(int id)
        {
            EmployeeMaster a = (from c in db.EmployeeMasters where c.EmployeeID == id select c).FirstOrDefault();
            UserRegistration u = (from c in db.UserRegistrations where c.UserName == a.Email select c).FirstOrDefault();
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (a != null)
                {
                    db.EmployeeMasters.Remove(a);
                    db.SaveChanges();
                }
                if (u != null)
                {
                    db.UserRegistrations.Remove(u);
                    db.SaveChanges();
                }
                TempData["SuccessMsg"] = "You have successfully Deleted Employee.";
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public JsonResult GetEmployeeName()
        {
            var employeelist = (from c1 in db.EmployeeMasters where c1.StatusActive==true  select c1.EmployeeName).ToList();

            return Json(new { data = employeelist }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Edit(int id)
         {

            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            EmployeeVM v =new EmployeeVM();
             //ViewBag.Country = db.CountryMasters.ToList();
             ViewBag.Designation = db.Designations.ToList();
             //ViewBag.Depots = db.tblDepots.ToList();
             ViewBag.roles = db.RoleMasters.ToList();

            ViewBag.AcHead = db.AcHeads.OrderBy(c => c.AcHead1).ToList();
            List<DepotClass> lst = new List<DepotClass>();
             var data = db.tblDepots.Where(c=>c.BranchID==BranchID).ToList();

             foreach (var i in data)
             {
                 DepotClass x = new DepotClass();
                 x.Depot = i.ID;
                 x.Name = i.Depot;

                 lst.Add(x);
             }

             ViewBag.Depots = lst;

             EmployeeMaster a = (from c in db.EmployeeMasters where c.EmployeeID == id select c).FirstOrDefault();
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                
                v.EmployeeID = a.EmployeeID;
                v.EmployeeName = a.EmployeeName;
                v.EmployeeCode = a.EmployeeCode;
                v.Address1 = a.Address1;
                v.Address2 = a.Address2;
                v.Address3 = a.Address3;
                v.Phone = a.Phone;
                v.Email = a.Email;
                v.JoinDate = a.JoinDate.Value;
                v.Fax = a.Fax;
                v.MobileNo = a.Mobile;
                if (a.UserID != null)
                {
                    var user = db.UserRegistrations.Where(cc => cc.UserID == a.UserID).FirstOrDefault();

                    if (user != null)
                    {
                        v.RoleID = Convert.ToInt32(user.RoleID);
                        v.Password = user.Password;
                    }
                }
                if (a.AcHeadID!=null && a.AcHeadID!=0)
                {
                    v.AcHeadID =Convert.ToInt32(a.AcHeadID);
                }

                v.CountryName = a.CountryName;
                v.CityName = a.CityName;                
                v.DesignationID = a.DesignationID.Value;
                v.BranchID = a.BranchID.Value;                
                v.Depot = a.DepotID;
                
                //v.MobileDeviceID = a.MobileDeviceID;
                //v.MobileDevicePWD = a.MobileDevicePwd;
                v.StatusCommision = a.StatusCommission.Value;
                v.StatusDefault = a.statusDefault.Value;
                v.StatusActive = a.StatusActive.Value;
                v.UserID = a.UserID;
                int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());

                if (a.AcCompanyID == null)
                    v.AcCompanyID = companyid;
                else
                    v.AcCompanyID = a.AcCompanyID.Value;

            }

             return View(v);
         }

         [HttpPost]
         public ActionResult Edit(EmployeeVM a)
         {
            UserRegistration u = new UserRegistration();
            PickupRequestDAO _dao = new PickupRequestDAO();
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            EmployeeMaster v=new EmployeeMaster();
            v = db.EmployeeMasters.Find(a.EmployeeID);
             
             v.EmployeeName = a.EmployeeName;
             v.EmployeeCode = a.EmployeeCode;
             v.Address1 = a.Address1;
             v.Address2 = a.Address2;
             v.Address3 = a.Address3;
             v.Phone = a.Phone;
             //v.Email = a.Email;
             v.Fax = a.Fax;
             v.Mobile = a.MobileNo;         
            v.CountryName = a.CountryName;
            v.AcCompanyID = companyid;
             v.DesignationID = a.DesignationID;
            v.RoleId = a.RoleID;
            v.AcHeadID = a.AcHeadID;
            v.BranchID = BranchID;
            v.DepotID = a.Depot;
            //if (v.Password!=a.Password)
            //   v.Password = a.Password;
             //v.MobileDeviceID = a.MobileDeviceID;
             //v.MobileDevicePwd = a.MobileDevicePWD;
             v.JoinDate = a.JoinDate;
             v.StatusCommission = a.StatusCommision;
             v.statusDefault = a.StatusDefault;
             v.StatusActive = a.StatusActive;

            UserRegistration x = null;

            if (a.UserID != null && a.UserID > 0)
                x = (from b in db.UserRegistrations where b.UserID == a.UserID select b).FirstOrDefault();

            if (x!=null)
            {
                x.RoleID = a.RoleID;
                db.Entry(x).State = EntityState.Modified;
                db.SaveChanges();
                //        db.UserRegistrations.Add(u);
                //        db.SaveChanges();
            }
            else
            {
                int max1 = (from c1 in db.UserRegistrations orderby c1.UserID descending select c1.UserID).FirstOrDefault();
                u.UserID = max1 + 1;
                u.UserName = a.Email;
                u.EmailId = a.Email;
                u.Password = "12345";
                u.Phone = a.Phone;
                u.IsActive = true;
                u.RoleID = a.RoleID;
                db.UserRegistrations.Add(u);
                db.SaveChanges();
            }

            //if (v.Email != a.Email)
            //{
            //    if (x == null)
            //    {

            //        int max1 = (from c1 in db.UserRegistrations orderby c1.UserID descending select c1.UserID).FirstOrDefault();

            //        int roleid = a.RoleID; // db.RoleMasters.Where(t => t.RoleName == "Agent").FirstOrDefault().RoleID;
            //        u.UserID = max1 + 1;
            //        u.UserName = a.Email;
            //        u.EmailId = a.Email;
            //        if (a.Password == "")
            //            u.Password = _dao.RandomPassword(6);
            //        else
            //            u.Password = a.Password; //  _dao.RandomPassword(6);

            //        u.Phone = a.Phone;
            //        u.IsActive = true;
            //        u.RoleID = roleid;
            //        db.UserRegistrations.Add(u);
            //        db.SaveChanges();

            //        v.Email = a.Email;
            //        a.UserID = u.UserID;
            //    }
            //    else
            //    {
            //        //checking duplicate
            //        UserRegistration x1 = (from b in db.UserRegistrations where b.UserName == a.Email && b.UserID != a.UserID select b).FirstOrDefault();
            //        if (x1 == null)
            //        {
            //            x.EmailId = a.Email;
            //            if (a.Password=="")
            //                x.Password =  _dao.RandomPassword(6);
            //            else
            //                x.Password = a.Password; //  _dao.RandomPassword(6);

            //            db.Entry(x).State = EntityState.Modified;
            //            db.SaveChanges();
            //        }

            //        v.Email = a.Email;
            //        a.UserID = x.UserID;
            //    }


            //}
            //else
            //{
            //    if (a.UserID == null || a.UserID == 0)
            //    {
            //        int max1 = (from c1 in db.UserRegistrations orderby c1.UserID descending select c1.UserID).FirstOrDefault();

            //        int roleid = db.RoleMasters.Where(t => t.RoleName == "Agent").FirstOrDefault().RoleID;
            //        u.UserID = max1 + 1;
            //        u.UserName = a.Email;
            //        u.EmailId = a.Email;

            //        if (a.Password == "")
            //            u.Password = _dao.RandomPassword(6);
            //        else
            //            u.Password = a.Password; //  _dao.RandomPassword(6);

            //        u.Phone = a.Phone;
            //        u.IsActive = true;
            //        u.RoleID = a.RoleID;
            //        db.UserRegistrations.Add(u);
            //        db.SaveChanges();

            //        v.UserID = u.UserID;
            //    }
            //    else
            //    {
            //        u = (from b in db.UserRegistrations where b.UserID == a.UserID select b).FirstOrDefault();
            //        if (u.Password != a.Password)
            //            u.Password = a.Password;

            //        if (u.RoleID != a.RoleID)
            //            u.RoleID = a.RoleID;
            //        if (u.Password==null)
            //        {
            //            u.Password = "1234";
            //        }
            //        try
            //        {
            //            db.Entry(u).State = EntityState.Modified;
            //            db.SaveChanges();
            //        }
            //        catch(Exception ex1)
            //        {

            //        }
            //    }
            //}




            db.Entry(v).State=EntityState.Modified;
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully Update Employee.";
            return RedirectToAction("Index");
             
             
         }

        public JsonResult CheckUserEmailExist(string EmailId,int UserId=0)
        {
            string status = "true";
           UserRegistration x = (from b in db.UserRegistrations where b.UserName == EmailId && (b.UserID!=UserId || UserId==0) select b).FirstOrDefault();
            if (x!=null)
            {
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                status = "false";
                return Json(status, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult UserProfile()
        {
            int id = 49;
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            EmployeeVM v = new EmployeeVM();
            //ViewBag.Country = db.CountryMasters.ToList();
            ViewBag.Designation = db.Designations.ToList();
            //ViewBag.Depots = db.tblDepots.ToList();
            ViewBag.roles = db.RoleMasters.ToList();


            List<DepotClass> lst = new List<DepotClass>();
            var data = db.tblDepots.Where(c => c.BranchID == BranchID).ToList();

            foreach (var i in data)
            {
                DepotClass x = new DepotClass();
                x.Depot = i.ID;
                x.Name = i.Depot;

                lst.Add(x);
            }

            ViewBag.Depots = lst;

            EmployeeMaster a = (from c in db.EmployeeMasters where c.EmployeeID == id select c).FirstOrDefault();
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {

                v.EmployeeID = a.EmployeeID;
                v.EmployeeName = a.EmployeeName;
                v.EmployeeCode = a.EmployeeCode;
                v.Address1 = a.Address1;
                v.Address2 = a.Address2;
                v.Address3 = a.Address3;
                v.Phone = a.Phone;
                v.Email = a.Email;
                v.JoinDate = a.JoinDate.Value;
                v.Fax = a.Fax;
                v.MobileNo = a.Mobile;
                if (a.UserID != null)
                {
                    var user = db.UserRegistrations.Where(cc => cc.UserID == a.UserID).FirstOrDefault();

                    if (user != null)
                    {
                        v.RoleID = Convert.ToInt32(user.RoleID);
                        v.Password = user.Password;
                    }
                }



                v.CountryName = a.CountryName;
                v.CityName = a.CityName;
                v.DesignationID = a.DesignationID.Value;
                v.BranchID = a.BranchID.Value;

                v.Depot = a.DepotID;


                v.MobileDeviceID = a.MobileDeviceID;
                v.MobileDevicePWD = a.MobileDevicePwd;
                v.StatusCommision = a.StatusCommission.Value;
                v.StatusDefault = a.statusDefault.Value;
                v.StatusActive = a.StatusActive.Value;
                v.UserID = a.UserID;
                int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());

                if (a.AcCompanyID == null)
                    v.AcCompanyID = companyid;
                else
                    v.AcCompanyID = a.AcCompanyID.Value;

            }

            return View("UserProfile",v);
            //return PartialView("_UserProfile", v);

        }

        [HttpPost]
        public JsonResult GetEmployeeCode(string EmployeeName)
        {
            string status = "ok";
            string customercode = "";
            //List<CourierStatu> _cstatus = new List<CourierStatu>();
            try
            {
                string custform = "000000";
                string maxcustomercode = (from d in db.EmployeeMasters orderby d.EmployeeID descending select d.EmployeeCode).FirstOrDefault();
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

                    string customerfirst = EmployeeName.Substring(0, 1);
                    string customersecond = "";
                    try
                    {
                        customersecond = EmployeeName.Split(' ')[1];
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
                        customercode = customerfirst + "E" + String.Format("{0:000000}", Convert.ToInt32(last6digit) + 1);
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
        public class GetDateClass
         {
             public int MonthID { get; set; }
             public int CYear { get; set; }
         }

         public JsonResult GetDate(int Month, int Year)
         {
             List<GetDateClass> lst =new List<GetDateClass>();
             var data = (from c in db.Locks where c.Month == Month & c.CurrentYear == Year select c).ToList();

             foreach (var item in data)
             {
                 GetDateClass d = new GetDateClass();

                 d.MonthID = Convert.ToInt32(item.Month);
                 d.CYear =Convert.ToInt32(item.CurrentYear);
                 lst.Add(d);
             }



             return Json(lst,JsonRequestBehavior.AllowGet);
         }


         public class dyclass
         {
             public int month { get; set; }
             public int year { get; set; }
         }
         public JsonResult GetLock(string year)
         {
             List<dyclass> lst = new List<dyclass>();

             var data = (from c in db.Locks where c.CurrentYear == Convert.ToInt32(year) select c).ToList();

             foreach (var item in data)
             {
                 lst.Add(new dyclass { month = Convert.ToInt32(item.Month), year =Convert.ToInt32(item.CurrentYear) });
             }
             return Json(lst, JsonRequestBehavior.AllowGet);

         }

    }

    public class DepotClass
    {
        public int Depot { get; set; }
        public string Name { get; set; }
    }
}
