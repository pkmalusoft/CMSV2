using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using System.IO;
using CMSV2.DAL;
using System.Data.Entity;

namespace CMSV2.Controllers
{
    public class DRSReconcController : Controller
    {
        SourceMastersModel MM = new SourceMastersModel();
        RecieptPaymentModel RP = new RecieptPaymentModel();
        CustomerRcieptVM cust = new CustomerRcieptVM();
        Entities1 db = new Entities1();
        EditCommanFu editfu = new EditCommanFu();
        // GET: DRSReconc
        [HttpGet]
        public ActionResult Index(string FromDate, string ToDate)
        {
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            List<DRSReceiptVM> Receipts = new List<DRSReceiptVM>();
            DateTime pFromDate;
            DateTime pToDate;
            if (FromDate == null || ToDate == null)
            {
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date;//.AddDays(-1); // FromDate = DateTime.Now;
                pToDate = CommanFunctions.GetLastDayofMonth().Date; // // ToDate = DateTime.Now;

                pFromDate = AccountsDAO.CheckParamDate(pFromDate, FyearId).Date;
                pToDate = AccountsDAO.CheckParamDate(pToDate, FyearId).Date;
            }
            else
            {
                pFromDate = Convert.ToDateTime(FromDate);//.AddDays(-1);
                pToDate = Convert.ToDateTime(ToDate);

            }

            Receipts = ReceiptDAO.GetDRSReceipts(FyearId, pFromDate, pToDate); // RP.GetAllReciepts();
            ViewBag.FromDate = pFromDate.Date.ToString("dd-MM-yyyy");
            ViewBag.ToDate = pToDate.Date.ToString("dd-MM-yyyy");
            return View(Receipts);
        }
        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Deliverdby = db.EmployeeMasters.ToList();
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            DRSReceiptVM cust = new DRSReceiptVM();
            cust.Details = new List<DRSReceiptDetailVM>();
            if (Session["UserID"] != null)
            {
                var branchid = Convert.ToInt32(Session["CurrentBranchID"]);

                if (id > 0)
                {
                    ViewBag.Title = "DRS Reconciliation - Modify";
                    //cust = RP.GetRecPayByRecpayID(id);

                    var acheadforcash = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;

                    cust.Details = new List<DRSReceiptDetailVM>();

                    //BindMasters_ForEdit(cust);
                }
                else
                {
                    ViewBag.Title = "DRS Reconciliation - Create";
                    string DocNo = ReceiptDAO.GetMaxDRSReceiptNO();
                    //BindAllMasters(2);

                    var acheadforcash = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;

                    DateTime pFromDate = AccountsDAO.CheckParamDate(DateTime.Now, FyearId).Date;
                    cust.DRSRecPayDate = pFromDate;
                    cust.DRSRecPayID = 0;
                    cust.DocumentNo = DocNo;
                    //cust.CurrencyId = Convert.ToInt32(Session["CurrencyId"].ToString());
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

            return View(cust);

        }
    }
}