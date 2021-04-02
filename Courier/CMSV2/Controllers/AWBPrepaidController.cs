using CMSV2.DAL;
using CMSV2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace CMSV2.Controllers
{
    [SessionExpireFilter]
    public class AWBPrepaidController : Controller
    {
        Entities1 db = new Entities1();
        // GET: AWBBookIssue
        public ActionResult Index()
        {

            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            AWBPrepaidSearch obj = (AWBPrepaidSearch)Session["AWBPrepaidSearch"];
            AWBPrepaidSearch model = new AWBPrepaidSearch();
            AWBDAO _dao = new AWBDAO();
            if (obj != null)
            {
                List<AWBPrepaidList> translist = new List<AWBPrepaidList>();
                translist = AWBDAO.GetAWBPrepaid(BranchID);
                model.FromDate = obj.FromDate;
                model.ToDate = obj.ToDate;
                model.DocumentNo = obj.DocumentNo;
                model.Details = translist;
            }
            else
            {
                Session["AWBPrepaidSearch"] = model;
                List<AWBPrepaidList> translist = new List<AWBPrepaidList>();
                model.FromDate = CommanFunctions.GetLastDayofMonth().Date;
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
                translist = AWBDAO.GetAWBPrepaid(BranchID);
                model.Details = translist;

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(AWBPrepaidSearch obj)
        {
            Session["AWBPrepaidSearch"] = obj;
            return RedirectToAction("Index");
        }


        public ActionResult Create(int id = 0)
        {
            ViewBag.Employee = db.EmployeeMasters.ToList();
            ViewBag.Department = db.Departments.ToList();
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            AWBPrepaidVM cust = new AWBPrepaidVM();
            cust.Details = new List<AWBDetailVM>();
            if (Session["UserID"] != null)
            {
                var branchid = Convert.ToInt32(Session["CurrentBranchID"]);

                if (id > 0)
                {
                    ViewBag.Title = "Prepaid AWB - Modify";
                    //cust = RP.GetRecPayByRecpayID(id);

                    var acheadforcash = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
                    PrepaidAWB dpay = db.PrepaidAWBs.Find(id);
                    cust.Documentno = dpay.Documentno;
                    cust.PrepaidAWBID = dpay.PrepaidAWBID;
                    cust.TransDate = dpay.TransDate;
                    var cashOrBankID = (from t in db.AcHeads where t.AcHead1 == dpay.BankName select t.AcHeadID).FirstOrDefault();
                    cust.CashBank = (cashOrBankID).ToString();
                    cust.ChequeBank = (cashOrBankID).ToString();
                    cust.ChequeNo = dpay.ChequeNo;
                    cust.ChequeDate = dpay.ChequeDate;                    
                    cust.CustomerID = Convert.ToInt32(dpay.CustomerID);
                    cust.CustomerName = db.CustomerMasters.Find(dpay.CustomerID).CustomerName;
                    cust.AWBNOFrom = dpay.AWBNOFrom;
                    cust.AWBNOTo = dpay.AWBNOTo;
                    cust.NoOfAWBs = dpay.NoOfAWBs;                    
                    cust.Total = dpay.Total;
                    cust.CourierCharge = dpay.CourierCharge;
                    cust.OriginLocation = dpay.OriginLocation;
                    cust.DestinationLocation = dpay.DestinationLocation;
                    //cust.DepartmentID = dpay.DepartmentID;
                    cust.Reference = dpay.Reference;
                    cust.Details = new List<AWBDetailVM>();
                    List<AWBDetailVM> details = new List<AWBDetailVM>();
                    details = (from c in db.AWBDetails where c.PrepaidAWBID == id select new AWBDetailVM { AWBNo = c.AWBNo }).ToList();
                    cust.Details = details;                    
                }
                else
                {
                    ViewBag.Title = "Prepaid AWB - Create";
                    string DocNo = AWBDAO.GetMaxPrepaidAWBDocumentNo();
                    
                    var acheadforcash = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;

                    DateTime pFromDate = AccountsDAO.CheckParamDate(DateTime.Now, FyearId).Date;
                    cust.TransDate = pFromDate;
                    cust.PrepaidAWBID = 0;
                    cust.Documentno = DocNo;
                    
                }
            }
            else
            {
                return RedirectToAction("Home", "Home");
            }

            return View(cust);

        }

        [HttpPost]
        public ActionResult Create(AWBPrepaidVM vm)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"]);
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"]);
            int branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            int UserId = Convert.ToInt32(Session["UserID"]);
            PrepaidAWB v = new PrepaidAWB();
            
            if (vm.CashBank != null)
            {
                vm.StatusEntry = "CS";
                int acheadid = Convert.ToInt32(vm.CashBank);
                var achead = (from t in db.AcHeads where t.AcHeadID == acheadid select t.AcHead1).FirstOrDefault();
                vm.BankName = achead;
                vm.AcHeadID = acheadid;
            }
            else
            {
                vm.StatusEntry = "BK";
                int acheadid = Convert.ToInt32(vm.ChequeBank);
                var achead = (from t in db.AcHeads where t.AcHeadID == acheadid select t.AcHead1).FirstOrDefault();
                vm.BankName = achead;

                vm.AcHeadID = acheadid;
            }
            if (vm.PrepaidAWBID == 0)
            {
                v.AcCompanyID = companyid;
                v.CreatedUserID=UserId;
                v.CreatedDate = DateTime.Now;
                v.ModifiedDate = DateTime.Now;
                v.ModifiedUserID = UserId;
                v.AcHeadID = vm.AcHeadID;
                v.BranchID = branchid;
                v.Documentno = vm.Documentno;
                v.TransDate = vm.TransDate;                
                v.StatusEntry = vm.StatusEntry;
                v.BankName = vm.BankName;
                v.NoOfAWBs = vm.NoOfAWBs;
                v.CourierCharge = vm.CourierCharge;
                v.Total = vm.Total;
                v.CustomerID = vm.CustomerID;
                v.OriginLocation = vm.OriginLocation;
                v.DestinationLocation = vm.DestinationLocation;
                v.ChequeDate = vm.ChequeDate;
                v.ChequeNo = vm.ChequeNo;
                v.AWBNOFrom = vm.AWBNOFrom;
                v.AWBNOTo = vm.AWBNOTo;
                v.FYearId = fyearid;
                v.Reference = vm.Reference;                
                //v.DepartmentID = vm.DepartmentID;
                v.AcjournalID = null;
                db.PrepaidAWBs.Add(v);
                db.SaveChanges();             
            }
            else
            {
                PrepaidAWB preawb = db.PrepaidAWBs.Find(vm.PrepaidAWBID);
                preawb.TransDate = vm.TransDate;
                preawb.BankName = vm.BankName;
                preawb.AcHeadID = vm.AcHeadID;
                preawb.ChequeDate = vm.ChequeDate;
                preawb.ChequeNo = vm.ChequeNo;
                preawb.CourierCharge = vm.CourierCharge;
                preawb.Total = vm.Total;
                preawb.StatusEntry = vm.StatusEntry;
                preawb.BranchID = branchid;                
                preawb.OriginLocation = vm.OriginLocation;
                preawb.DestinationLocation = vm.DestinationLocation;
                preawb.AWBNOFrom = vm.AWBNOFrom;
                preawb.AWBNOTo = vm.AWBNOTo;
                preawb.NoOfAWBs = vm.NoOfAWBs;
                preawb.Reference = vm.Reference;
                preawb.ModifiedDate = DateTime.Now;
                preawb.ModifiedUserID = UserId;
                db.Entry(preawb).State = EntityState.Modified;
                db.SaveChanges();
            }
            //posting
            //generate awb in AWBDetail
            AWBDAO.GenerateAWBPrepaid(v.PrepaidAWBID);

            return RedirectToAction("Index", "AWBPrepaid");

        }

        [HttpGet]
        public JsonResult GetCustomerName(string term)
        {
            var customerlist = (from c1 in db.CustomerMasters
                                where c1.CustomerType == "CR" && c1.CustomerName.ToLower().StartsWith(term.ToLower())
                                orderby c1.CustomerName ascending
                                select new { CustomerID = c1.CustomerID, CustomerName = c1.CustomerName, CustomerType = c1.CustomerType,LocationName=c1.LocationName }).ToList();

            return Json(customerlist, JsonRequestBehavior.AllowGet);

        }
    }
}