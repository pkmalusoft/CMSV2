using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Configuration;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using CMSV2.DAL;
using System.Data.Entity;

namespace CMSV2.Controllers
{
    [SessionExpire]
    //   [Authorize]
    public class DRSReceiptController : Controller
    {
        SourceMastersModel MM = new SourceMastersModel();
        RecieptPaymentModel RP = new RecieptPaymentModel();
        CustomerRcieptVM cust = new CustomerRcieptVM();
        Entities1 Context1 = new Entities1();

        EditCommanFu editfu = new EditCommanFu();       
   
        public JsonResult GetInvoiceOfCustomer(string ID)
        {
            //List<SP_GetCustomerInvoiceDetailsForReciept_Result> AllInvoices = new List<SP_GetCustomerInvoiceDetailsForReciept_Result>();

            DateTime fromdate = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime todate = Convert.ToDateTime(Session["FyearTo"].ToString());
            var AllInvoices = ReceiptDAO.GetCustomerInvoiceDetailsForReciept(Convert.ToInt32(ID), fromdate.Date.ToString(), todate.Date.ToString()).OrderBy(x => x.InvoiceDate).ToList();

            return Json(AllInvoices, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExchangeRateByCurID(string ID)
        {
            //List<SP_GetCustomerInvoiceDetailsForReciept_Result> AllInvoices = new List<SP_GetCustomerInvoiceDetailsForReciept_Result>();

            var ER = RP.GetExchgeRateByCurID(Convert.ToInt32(ID));

            return Json(ER, JsonRequestBehavior.AllowGet);
        }
           



        public void BindAllMasters(int pagetype)
        {
            List<CustomerMaster> Customers = new List<CustomerMaster>();
            Customers = MM.GetAllCustomer();

            List<CurrencyMaster> Currencys = new List<CurrencyMaster>();
            Currencys = MM.GetCurrency();

            

            //ViewBag.DocumentNos = DocNo;
            if (pagetype == 1)
            {
                var customernew = (from d in Context1.CustomerMasters where d.CustomerType == "CS" select d).ToList();

                ViewBag.Customer = new SelectList(customernew, "CustomerID", "Customer1");
            }
            else
            {
                var customernew = (from d in Context1.CustomerMasters where d.CustomerType == "CS" select d).ToList();

                ViewBag.Customer = new SelectList(customernew, "CustomerID", "Customer1");
            }

            ViewBag.Currency = new SelectList(Currencys, "CurrencyID", "CurrencyName");
        }

        public void BindMasters_ForEdit(CustomerRcieptVM cust)
        {
            List<CustomerMaster> Customers = new List<CustomerMaster>();
            Customers = MM.GetAllCustomer();

            List<CurrencyMaster> Currencys = new List<CurrencyMaster>();
            Currencys = MM.GetCurrency();


            ViewBag.DocumentNos = cust.DocumentNo;

            ViewBag.Customer = new SelectList(Customers, "CustomerID", "Customer", cust.CustomerID);

            ViewBag.Currency = new SelectList(Currencys, "CurrencyID", "CurrencyName", cust.CurrencyId);

        }

      
              public JsonResult GetAllCustomerByDate(string fdate, string tdate, int FYearID)
        {
            DateTime d = DateTime.Now;
            DateTime fyear = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime mstart = new DateTime(fyear.Year, d.Month, 01);

            int maxday = DateTime.DaysInMonth(fyear.Year, d.Month);
            DateTime mend = new DateTime(fyear.Year, d.Month, maxday);

            var sdate = DateTime.Parse(fdate);
            var edate = DateTime.Parse(tdate);

            ViewBag.AllCustomers = MM.GetAllCustomer();

            var data = Context1.RecPays.Where(x => x.RecPayDate >= sdate && x.RecPayDate <= edate && x.CustomerID != null && x.IsTradingReceipt != true && x.FYearID == FYearID).OrderByDescending(x => x.RecPayDate).ToList();

            //var recpayid = data.FirstOrDefault().RecPayID;
            //var Recdetails = (from x in Context1.RecPayDetails where x.RecPayID == recpayid && (x.CurrencyID != null || x.CurrencyID > 0) select x).FirstOrDefault();


            data.ForEach(s => s.Remarks = (from x in Context1.RecPayDetails where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select x).FirstOrDefault() != null ? (from x in Context1.RecPayDetails join C in Context1.CurrencyMasters on x.CurrencyID equals C.CurrencyID where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select C.CurrencyName).FirstOrDefault() : "");

            //var cust = Context1.SP_GetAllRecieptsDetailsByDate(fdate, tdate, FYearID).ToList();

            string view = this.RenderPartialView("_GetAllCustomerByDate", data);

            return new JsonResult
            {
                Data = new
                {
                    success = true,
                    view = view
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };


        }

        public ActionResult GetReceiptsByDate(string fdate, string tdate, int FYearID)
        {
            DateTime d = DateTime.Now;
            DateTime fyear = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime mstart = new DateTime(fyear.Year, d.Month, 01);

            int maxday = DateTime.DaysInMonth(fyear.Year, d.Month);
            DateTime mend = new DateTime(fyear.Year, d.Month, maxday);
            var sdate = DateTime.Parse(fdate);
            var edate = DateTime.Parse(tdate);

            ViewBag.AllCustomers = Context1.CustomerMasters.ToList();

            var data = Context1.RecPays.Where(x => x.RecPayDate >= sdate && x.RecPayDate <= edate && x.CustomerID != null && x.IsTradingReceipt != true && x.FYearID == FYearID).OrderByDescending(x => x.RecPayDate).ToList();
            data.ForEach(s => s.Remarks = (from x in Context1.RecPayDetails where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select x).FirstOrDefault() != null ? (from x in Context1.RecPayDetails join C in Context1.CurrencyMasters on x.CurrencyID equals C.CurrencyID where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select C.CurrencyName).FirstOrDefault() : "");


            //var cust = Context1.SP_GetAllRecieptsDetailsByDate(fdate, tdate, FYearID).ToList();

            return PartialView("_GetAllCustomerByDate", data);

        }

        public JsonResult GetAllTradeCustomerByDate(string fdate, string tdate, int FYearID)
        {
            DateTime d = DateTime.Now;
            DateTime fyear = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime mstart = new DateTime(fyear.Year, d.Month, 01);

            int maxday = DateTime.DaysInMonth(fyear.Year, d.Month);
            DateTime mend = new DateTime(fyear.Year, d.Month, maxday);

            //var sdate = DateTime.Parse(fdate);
            //var edate = DateTime.Parse(tdate);
            var sdate = Convert.ToDateTime(fdate);
            var edate = Convert.ToDateTime(tdate);

            //var data = Context1.RecPays.Where(x => x.RecPayDate >= sdate && x.RecPayDate <= edate && x.CustomerID != null && x.IsTradingReceipt == true && x.FYearID == FYearID).OrderByDescending(x => x.RecPayDate).ToList();
            //var cust = Context1.SP_GetAllRecieptsDetailsByDate(fdate, tdate, FYearID).ToList();
            var data = ReceiptDAO.GetCustomerReceiptsByDate(fdate, tdate, FYearID);
            //data.ForEach(s => s.Remarks = (from x in Context1.RecPayDetails where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select x).FirstOrDefault() != null ? (from x in Context1.RecPayDetails join C in Context1.CurrencyMasters on x.CurrencyID equals C.CurrencyID where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select C.CurrencyName).FirstOrDefault() : "");

            ViewBag.AllCustomers = Context1.CustomerMasters.ToList();
            string view = this.RenderPartialView("_GetAllTradeCustomerByDate", data);
            //string view = this.RenderPartialView("_Table", data);

            return new JsonResult
            {
                Data = new
                {
                    success = true,
                    view = view
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };


        }
        public ActionResult GetTradeReceiptsByDate(string fdate, string tdate, int FYearID)
        {
            DateTime d = DateTime.Now;
            DateTime fyear = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime mstart = new DateTime(fyear.Year, d.Month, 01);

            int maxday = DateTime.DaysInMonth(fyear.Year, d.Month);
            DateTime mend = new DateTime(fyear.Year, d.Month, maxday);
            var sdate = DateTime.Parse(fdate);
            var edate = DateTime.Parse(tdate);
            ViewBag.AllCustomers = Context1.CustomerMasters.ToList();
            var cust = ReceiptDAO.GetCustomerReceiptsByDate(fdate, tdate, FYearID).ToList();

            //var data = Context1.RecPays.Where(x => x.RecPayDate >= sdate && x.RecPayDate <= edate && x.CustomerID != null && x.FYearID == FYearID && x.IsTradingReceipt == true).OrderByDescending(x => x.RecPayDate).ToList();

            //data.ForEach(s => s.Remarks = (from x in Context1.RecPayDetails where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select x).FirstOrDefault() != null ? (from x in Context1.RecPayDetails join C in Context1.CurrencyMasters on x.CurrencyID equals C.CurrencyID where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select C.CurrencyName).FirstOrDefault() : "");




            return PartialView("_GetAllTradeCustomerByDate", cust);

        }
        [HttpGet]
        public ActionResult Index(int? id, string FromDate, string ToDate)
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
        public ActionResult Create(int id=0)
        {
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            DRSReceiptVM cust = new DRSReceiptVM();
            cust.Details= new List<DRSReceiptDetailVM>();            
            if (Session["UserID"] != null)
            {
                var branchid = Convert.ToInt32(Session["CurrentBranchID"]);

                if (id > 0)
                {
                    ViewBag.Title = "DRS Receipt - Modify";
                    //cust = RP.GetRecPayByRecpayID(id);

                    var acheadforcash = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
                    
                    cust.Details  = new List<DRSReceiptDetailVM>();                  
                  
                    //BindMasters_ForEdit(cust);
                }
                else
                {
                    ViewBag.Title = "DRS Receipt - Create";
                    string DocNo = ReceiptDAO.GetMaxDRSReceiptNO();
                    //BindAllMasters(2);

                    var acheadforcash = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

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

        [HttpPost]
        public ActionResult Create(DRSReceiptVM vm)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"]);
            int companyid = Convert.ToInt32(Session["ACCompanyID"]);
            DRSRecPay v = new DRSRecPay();
            int RPID = 0;
            if (vm.DRSRecPayID==0)
            {
                vm.AcCompanyID = companyid;
                vm.FYearID = fyearid;
                if (vm.CashBank != null)
                {
                    vm.StatusEntry = "CS";
                    int acheadid = Convert.ToInt32(vm.CashBank);
                    var achead = (from t in Context1.AcHeads where t.AcHeadID == acheadid select t.AcHead1).FirstOrDefault();
                    vm.BankName = achead;
                }
                else
                {
                    vm.StatusEntry = "BK";
                    int acheadid = Convert.ToInt32(vm.ChequeBank);
                    var achead = (from t in Context1.AcHeads where t.AcHeadID == acheadid select t.AcHead1).FirstOrDefault();
                    vm.BankName = achead;
                }
                vm.CourierEMPID= Convert.ToInt32(Session["UserID"]);
                RPID = ReceiptDAO.AddDRSReceipt(vm, Session["UserID"].ToString()); //.AddCustomerRecieptPayment(RecP, Session["UserID"].ToString());
            }
            else
            {
                RPID = vm.DRSRecPayID;
                DRSRecPay recpay = Context1.DRSRecPays.Find(RPID);
                recpay.DRSRecPayDate = vm.DRSRecPayDate;                                
                recpay.BankName = vm.BankName;
                recpay.ChequeDate = vm.ChequeDate;
                recpay.ChequeNo = vm.ChequeNo;                                                                
                recpay.ReceivedAmount = vm.ReceivedAmount;
                recpay.StatusEntry = vm.StatusEntry;                
                recpay.Remarks = vm.Remarks;
                Context1.Entry(recpay).State = EntityState.Modified;
                Context1.SaveChanges();
            }

            foreach (var item in vm.Details)
            {
                if (item.DRSRecPayDetailID==0)
                {
                    DRSRecPayDetail detail =new DRSRecPayDetail();
                    detail.DRSRecPayID = RPID;
                    detail.InScanID = item.InScanID;
                    detail.AWBNO = item.AWBNO;
                    detail.CourierCharge = item.CourierCharge;
                    detail.MaterialCost = item.MaterialCost;
                    detail.CCReceived = item.CCReceived;
                    detail.MCReceived = item.MCReceived;
                    detail.StatusClose = item.StatusClose;
                    detail.Discount = item.Discount;
                    Context1.DRSRecPayDetails.Add(detail);
                    Context1.SaveChanges();

                }
                else
                {
                    DRSRecPayDetail detail = Context1.DRSRecPayDetails.Find(item.DRSRecPayDetailID);                    
                    detail.InScanID = item.InScanID;
                    detail.AWBNO = item.AWBNO;
                    detail.CourierCharge = item.CourierCharge;
                    detail.MaterialCost = item.MaterialCost;
                    detail.CCReceived = item.CCReceived;
                    detail.MCReceived = item.MCReceived;
                    detail.StatusClose = item.StatusClose;
                    detail.Discount = item.Discount;
                    Context1.Entry(detail).State = EntityState.Modified;
                    Context1.SaveChanges();
                }

            }


            return RedirectToAction("Index", "DRSReceipt", new { id=0 });

        }
        [HttpGet]
        public JsonResult GetDRSNo(string term)
        {
            if (term.Trim() != "")
            {
                var drslist = (from c1 in Context1.DRS
                               where c1.DRSNo.ToLower().Contains(term.ToLower())
                               orderby c1.DRSNo ascending
                               select new {DRSID=c1.DRSID, DRSNo = c1.DRSNo, DRSDate = c1.DRSDate }).ToList();

                return Json(drslist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var drslist = (from c1 in Context1.DRS                               
                               orderby c1.DRSNo ascending
                               select new {DRSID=c1.DRSID, DRSNo=c1.DRSNo ,DRSDate=c1.DRSDate  }).ToList();

                return Json(drslist, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetDRSAWB(int DRSID,int RecPayId=0)
        {
            var details= ReceiptDAO.GetDRSAWB(DRSID, RecPayId);
           return Json(details, JsonRequestBehavior.AllowGet);
        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }
            return words;
        }
      
    }
}
