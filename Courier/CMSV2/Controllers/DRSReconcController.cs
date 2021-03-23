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
using System.Reflection;
using Newtonsoft.Json;

namespace CMSV2.Controllers
{
    [SessionExpireFilter]
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

            List<DRRVM> list = (from c in db.DRRs join c2 in db.DRS on c.DRSID equals c2.DRSID join c3 in db.DRSRecPays on c.DRSRecPayID equals c3.DRSRecPayID
                                where c.DRRDate>= pFromDate && c.DRRDate <=pToDate
                                select new DRRVM { DRRID = c.DRRID, DRRDate=c.DRRDate,ReceiptDate=c3.DRSRecPayDate, DRSID = c.DRSID, DRSNo = c2.DRSNo, DRSDate = c2.DRSDate, DRSReceiptNo = c3.DocumentNo, ReconciledAmount = c.ReconciledAmount }).ToList();
            //Receipts = ReceiptDAO.GetDRSReceipts(FyearId, pFromDate, pToDate); // RP.GetAllReciepts();
            ViewBag.FromDate = pFromDate.Date.ToString("dd-MM-yyyy");
            ViewBag.ToDate = pToDate.Date.ToString("dd-MM-yyyy");
            return View(list);
        }
        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Deliverdby = db.EmployeeMasters.ToList();
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            DRRVM cust = new DRRVM();
            cust.Details = new List<DRRDetailVM>();
            if (Session["UserID"] != null)
            {
                var branchid = Convert.ToInt32(Session["CurrentBranchID"]);

                if (id > 0)
                {
                    ViewBag.Title = "DRS Reconciliation - Modify";
                    //cust = RP.GetRecPayByRecpayID(id);
                    DRR dr = db.DRRs.Find(id);
                    cust.DRRID = dr.DRRID;
                    cust.DRRDate = dr.DRRDate;
                    cust.CourierId = dr.CourierId;
                    cust.DeliveredBy =Convert.ToInt32(dr.CourierId);
                    cust.DRSID = dr.DRSID;
                    var drs = db.DRS.Find(dr.DRSID);
                    cust.DRSNo = drs.DRSNo;
                    var drsrecpay = db.DRSRecPays.Find(dr.DRSRecPayID);
                    cust.DRSRecPayID = dr.DRSRecPayID;
                    cust.DRSReceiptNo = drsrecpay.DocumentNo;
                    cust.DRSReceiptDate = drsrecpay.DRSRecPayDate.ToString("dd/MM/yyyy");
                    cust.ReceiptDate = drsrecpay.DRSRecPayDate;
                    cust.ReceivedAmount = Convert.ToDecimal(drsrecpay.ReceivedAmount);
                    cust.ReconciledAmount = dr.ReconciledAmount;

                    var acheadforcash = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;

                    cust.Details = new List<DRRDetailVM>();
                    cust.Details = (from c in db.DRRDetails where c.DRRID == id select new DRRDetailVM { DRRID=c.DRRID,Reference=c.Reference,ReferenceId=c.ReferenceId,COD=c.CODAmount,PKPCash=c.PKPCash,MCReceived=c.MCReceived,Discount=c.Discount,Expense=c.Expense,Total=c.Total}).ToList();

                    //BindMasters_ForEdit(cust);
                }
                else
                {
                    ViewBag.Title = "DRS Reconciliation - Create";
                  

                    var acheadforcash = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;

                    DateTime pFromDate = AccountsDAO.CheckParamDate(DateTime.Now, FyearId).Date;
                    //cust.DRSReceiptDate = pFromDate;
                    cust.DRSRecPayID = 0;                                       

                    //cust.CurrencyId = Convert.ToInt32(Session["CurrencyId"].ToString());
                }
            }
            else
            {
                return RedirectToAction("Home", "Home");
            }

            return View(cust);

        }

        [HttpGet]
        public JsonResult GetDRSReceiptNo(string term)
        {
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            if (term.Trim() != "")
            {
                var drslist = (from c1 in db.DRSRecPays
                               //join c2 in db.DRS on c1.DRSID equals c2.DRSID
                               where c1.DocumentNo.ToLower().Contains(term.ToLower()) && c1.FYearID ==FyearId
                                && c1.DRRID==null
                               orderby c1.DocumentNo ascending
                               select new { DRSRecPayId=c1.DRSRecPayID,DRSRecPayDate=c1.DRSRecPayDate,DocumentNo=c1.DocumentNo, DRSID = c1.DRSID, DeliveredBy = c1.DeliveredBy, ReceivedAmount = c1.ReceivedAmount }).ToList();

                return Json(drslist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var drslist = (from c1 in db.DRSRecPays
                               //join c2 in db.DRS on c1.DRSID equals c2.DRSID
                               where c1.FYearID ==FyearId
                               && c1.DRRID == null
                               orderby c1.DocumentNo ascending
                               select new { DRSRecPayId = c1.DRSRecPayID, DRSRecPayDate = c1.DRSRecPayDate, DocumentNo = c1.DocumentNo, DRSID = c1.DRSID,DeliveredBy=c1.DeliveredBy, ReceivedAmount = c1.ReceivedAmount }).ToList();
                return Json(drslist, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetDRS(string term)
        {
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            if (term.Trim() != "")
            {
                var drslist = (from c1 in db.DRSRecPays
                              join c2 in db.DRS on c1.DRSID equals c2.DRSID
                               where c1.DocumentNo.ToLower().Contains(term.ToLower()) && c1.FYearID == FyearId
                                && c1.DRRID == null
                               orderby c1.DocumentNo ascending
                               select new { DRSRecPayId = c1.DRSRecPayID, DRSRecPayDate = c1.DRSRecPayDate, DocumentNo = c1.DocumentNo, DRSID = c1.DRSID, DRSNo = c2.DRSNo, DRSDate = c2.DRSDate, DeliveredBy = c1.DeliveredBy, CheckedBy = c2.CheckedBy, ReceivedAmount = c1.ReceivedAmount }).ToList();

                return Json(drslist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var drslist = (from c1 in db.DRSRecPays
                               join c2 in db.DRS on c1.DRSID equals c2.DRSID
                               where c1.FYearID == FyearId
                               && c1.DRRID == null
                               orderby c1.DocumentNo ascending
                               select new { DRSRecPayId = c1.DRSRecPayID, DRSRecPayDate = c1.DRSRecPayDate, DocumentNo = c1.DocumentNo, DRSID = c1.DRSID, DRSNo = c2.DRSNo, DRSDate = c2.DRSDate, DeliveredBy = c1.DeliveredBy, CheckedBy = c2.CheckedBy, ReceivedAmount = c1.ReceivedAmount }).ToList();
                return Json(drslist, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetDRSDetail(int DRSID)
        {
            int FyearId = Convert.ToInt32(Session["fyearid"]);

            var drslist = (from c1 in db.DRS
                           where c1.DRSID == DRSID
                           select new { DRSID = c1.DRSID, DRSNo = c1.DRSNo, DRSDate = c1.DRSDate, DeliveredBy = c1.DeliveredBy, CheckedBy = c1.CheckedBy }).FirstOrDefault();

             return Json(drslist, JsonRequestBehavior.AllowGet);
            
        }


        [HttpGet]
        public JsonResult GetDRSAWB(string term,int DRSID)
        {
         int FyearId = Convert.ToInt32(Session["fyearid"]);
            if (term.Trim() != "")
            {
                var drslist = (from c3 in db.InScanMasters 
                               where c3.AWBNo.ToLower().Contains(term.Trim().ToLower()) && c3.AcFinancialYearID == FyearId
                               && (c3.DRSID !=null)
                               && c3.IsDeleted == false
                               && (c3.PaymentModeId==2 && c3.CODReceiptId==null)                               
                               orderby c3.AWBNo ascending
                               select new { InscanId= c3.InScanID, AWBNo=c3.AWBNo,Shipper=c3.Consignor,Consignee=c3.Consignee,AWBDate=c3.TransactionDate,CourierCharge=c3.CourierCharge,OtherCharge=c3.OtherCharge,TotalCharge=c3.NetTotal,MaterialCost=c3.MaterialCost,IsNCND=c3.IsNCND,IsCashOnly=c3.IsCashOnly,IsCollectMaterial=c3.IsCollectMaterial,IsCheque=c3.IsChequeOnly, IsDoCopyBack =c3.IsDOCopyBack ,Pieces=c3.Pieces,Weight=c3.Weight}).ToList();

                return Json(drslist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var drslist = (from c3 in db.InScanMasters 
                               where c3.AcFinancialYearID == FyearId
                               && c3.IsDeleted==false
                               //&& (c3.DRSID == DRSID || DRSID == 0)
                               && (c3.DRSID != null)
                               && (c3.PaymentModeId == 2 && c3.CODReceiptId == null)
                               orderby c3.AWBNo ascending
                                select new { InscanId = c3.InScanID, AWBNo = c3.AWBNo, Shipper = c3.Consignor, Consignee = c3.Consignee, AWBDate = c3.TransactionDate, CourierCharge = c3.CourierCharge, OtherCharge = c3.OtherCharge, TotalCharge = c3.NetTotal, MaterialCost = c3.MaterialCost, IsNCND = c3.IsNCND, IsCashOnly = c3.IsCashOnly, IsCollectMaterial = c3.IsCollectMaterial, IsCheque = c3.IsDOCopyBack, d = c3.IsDOCopyBack, Pieces = c3.Pieces, Weight = c3.Weight }).ToList();

                return Json(drslist, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public string SaveReconc(int DRRID, int DRSID, int DRSRecpayID, decimal ReconcAmount, int CourierId, string Details)
        {
            var IDetails = JsonConvert.DeserializeObject<List<DRRDetailVM>>(Details);
            DataTable ds = new DataTable();
            DataSet dt = new DataSet();
            dt = ToDataTable(IDetails);
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            string xml = dt.GetXml();
            if (Session["UserID"] != null)
            { 
                int userid =Convert.ToInt32(Session["UserID"].ToString());
            int BranchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            DateTime drrdate = CommanFunctions.GetLastDayofMonth().Date;
            ReceiptDAO.SaveReconc(DRRID, drrdate, DRSID, DRSRecpayID, ReconcAmount, CourierId, BranchId ,FyearId, userid, xml);
            return "Ok";
            }
            else
            {
                return "Failed!";
            }

        }

        public static DataSet ToDataTable<T>(List<T> items)
        {
            DataSet ds = new DataSet();
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            ds.Tables.Add(dataTable);
            //put a breakpoint here and check datatable
            return ds;
        }

    }
}