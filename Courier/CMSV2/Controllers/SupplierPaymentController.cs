using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Dynamic;
using System.Data;
//using Microsoft.Reporting.WebForms;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml;
using iTextSharp.text.xml.simpleparser;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using Newtonsoft.Json;
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
    public class SupplierPaymentController : Controller
    {
        SourceMastersModel MM = new SourceMastersModel();
        RecieptPaymentModel RP = new RecieptPaymentModel();
        CustomerRcieptVM cust = new CustomerRcieptVM();
        Entities1 Context1 = new Entities1();

        EditCommanFu editfu = new EditCommanFu();
        //
        // GET: /CustomerReciept/




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

        [HttpGet]
        public ActionResult DeletePayment(int id)
        {            
            if (id != 0)
            {
                ReceiptDAO.DeleteSupplierPayments(id);
            }

            return RedirectToAction("SupplierPaymentDetails", "SupplierPayment", new { ID = 10 });

        }

        public JsonResult ReceiptReport(int id)
        {
            string reportpath = "";
            //int k = 0;
            if (id != 0)
            {
                reportpath = AccountsReportsDAO.GenerateCustomerReceipt(id);

            }

            return Json(new { path = reportpath, result = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public string getSuccessID()
        {
            string ID = "";

            if (Session["ID"] != null)
            {
                ID = Session["ID"].ToString();
            }

            return ID;
        }

        //public JsonResult GetInvoiceOfCustomer(string ID)
        //{
        //    List<SP_GetCustomerInvoiceDetailsForReciept_Result> AllInvoices = new List<SP_GetCustomerInvoiceDetailsForReciept_Result>();

        //    AllInvoices = RP.GetCustomerInvoiceDetails(199);

        //    return this.Json(AllInvoices.ToList());
        //}

        public void BindAllMasters(int pagetype)
        {
            List<CustomerMaster> Customers = new List<CustomerMaster>();
            Customers = MM.GetAllCustomer();

            List<CurrencyMaster> Currencys = new List<CurrencyMaster>();
            Currencys = MM.GetCurrency();

            string DocNo = RP.GetMaxPaymentDocumentNo();

            ViewBag.DocumentNos = DocNo;
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

        public JsonResult GetAllCurrencyCustReciept()
        {
            //List<SP_GetCustomerInvoiceDetailsForReciept_Result> AllInvoices = new List<SP_GetCustomerInvoiceDetailsForReciept_Result>();
            // var AllInvoices;


            //var CostReciept = (from t in Context1.SPGetAllLocalCurrencyCustRecievable(Convert.ToInt32(Session["fyearid"].ToString()))
            //                   select t).ToList();

            var CostReciept = ReceiptDAO.SPGetAllLocalCurrencyCustRecievable(Convert.ToInt32(Session["fyearid"].ToString()));


            return Json(CostReciept, JsonRequestBehavior.AllowGet);



        }

        public JsonResult GetAllCustomer()
        {
            DateTime d = DateTime.Now;
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            DateTime fyear = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime mstart = new DateTime(fyear.Year, d.Month, 01);

            int maxday = DateTime.DaysInMonth(fyear.Year, d.Month);
            DateTime mend = new DateTime(fyear.Year, d.Month, maxday);

            var cust = ReceiptDAO.GetCustomerReceipts(fyearid, mstart, mend);// ().Where(x => x.RecPayDate >= mstart && x.RecPayDate <= mend).OrderByDescending(x => x.RecPayDate).ToList();
            //Context1.SP_GetAllRecieptsDetails().Where(x => x.RecPayDate >= mstart && x.RecPayDate <= mend).OrderByDescending(x => x.RecPayDate).ToList();

            string view = this.RenderPartialView("_GetAllSupplier", cust);

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

            string view = this.RenderPartialView("_GetAllSupplierByDate", data);

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

            return PartialView("_GetAllSupplierByDate", data);

        }

        public JsonResult GetAllTradeCustomerByDate(string fdate, string tdate, int FYearID)
        {
            DateTime d = DateTime.Now;
            DateTime fyear = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime mstart = new DateTime(fyear.Year, d.Month, 01);

            int maxday = DateTime.DaysInMonth(fyear.Year, d.Month);
            DateTime mend = new DateTime(fyear.Year, d.Month, maxday);

            var sdate = DateTime.Parse(fdate);
            var edate = DateTime.Parse(tdate);


            //var data = Context1.RecPays.Where(x => x.RecPayDate >= sdate && x.RecPayDate <= edate && x.CustomerID != null && x.IsTradingReceipt == true && x.FYearID == FYearID).OrderByDescending(x => x.RecPayDate).ToList();
            //var cust = Context1.SP_GetAllRecieptsDetailsByDate(fdate, tdate, FYearID).ToList();
            //var data = ReceiptDAO.GetCustomerReceiptsByDate(fdate, tdate, FYearID);
            //data.ForEach(s => s.Remarks = (from x in Context1.RecPayDetails where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select x).FirstOrDefault() != null ? (from x in Context1.RecPayDetails join C in Context1.CurrencyMasters on x.CurrencyID equals C.CurrencyID where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select C.CurrencyName).FirstOrDefault() : "");

            ViewBag.AllCustomers = Context1.CustomerMasters.ToList();
            var Reciepts = (from r in Context1.RecPays
                            //join de in Context1.RecPayDetails on r.RecPayID equals de.RecPayID
                            join s in Context1.SupplierMasters on r.SupplierID equals s.SupplierID
                            select new ReceiptVM
                            {
                                RecPayDate = r.RecPayDate,
                                DocumentNo = r.DocumentNo,
                                RecPayID = r.RecPayID,
                                PartyName = s.SupplierName,
                                Amount =r.FMoney
                            }).ToList();
            //Reciepts.ForEach(x => x.Amount = (from s in Context1.RecPayDetails where s.RecPayID == x.RecPayID where s.Amount > 0 select s).ToList().Sum(a => a.Amount));
            var data = (from t in Reciepts where (t.RecPayDate >= sdate && t.RecPayDate <= edate) select t).OrderByDescending(cc => cc.RecPayDate).ToList();
            var result = data.GroupBy(p => p.RecPayID).Select(grp => grp.FirstOrDefault());

            string view = this.RenderPartialView("_GetAllTradeSupplierByDate", result);
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
            //var cust = ReceiptDAO.GetCustomerReceiptsByDate(fdate, tdate, FYearID).ToList();

            //var data = Context1.RecPays.Where(x => x.RecPayDate >= sdate && x.RecPayDate <= edate && x.CustomerID != null && x.FYearID == FYearID && x.IsTradingReceipt == true).OrderByDescending(x => x.RecPayDate).ToList();

            //data.ForEach(s => s.Remarks = (from x in Context1.RecPayDetails where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select x).FirstOrDefault() != null ? (from x in Context1.RecPayDetails join C in Context1.CurrencyMasters on x.CurrencyID equals C.CurrencyID where x.RecPayID == s.RecPayID && (x.CurrencyID != null || x.CurrencyID > 0) select C.CurrencyName).FirstOrDefault() : "");


            var Reciepts = (from r in Context1.RecPays
                            join de in Context1.RecPayDetails on r.RecPayID equals de.RecPayID
                            join s in Context1.SupplierMasters on r.SupplierID equals s.SupplierID
                            select new ReceiptVM
                            {
                                RecPayDate = r.RecPayDate,
                                DocumentNo = r.DocumentNo,
                                RecPayID = r.RecPayID,
                                PartyName = s.SupplierName
                            }).ToList();
            Reciepts.ForEach(x => x.Amount = (from s in Context1.RecPayDetails where s.RecPayID == x.RecPayID where s.Amount > 0 select s).ToList().Sum(a => a.Amount));
            var data = (from t in Reciepts where (t.RecPayDate >= sdate && t.RecPayDate <= edate) select t).ToList();
            var result = data.GroupBy(p => p.RecPayID).Select(grp => grp.FirstOrDefault());


            return PartialView("_GetAllTradeSupplierByDate", result);

        }
        [HttpGet]
        public ActionResult SupplierPaymentDetails(int ID)
        {
            List<ReceiptVM> Reciepts = new List<ReceiptVM>();

            //Reciepts = ReceiptDAO.GetCustomerReceipts(); // RP.GetAllReciepts();
            Reciepts = (from r in Context1.RecPays
                        //join d in Context1.RecPayDetails on r.RecPayID equals d.RecPayID
                        join s in Context1.SupplierMasters on r.SupplierID equals s.SupplierID
                        orderby r.RecPayDate descending
                        select new ReceiptVM
                        {
                            RecPayDate = r.RecPayDate,
                            DocumentNo = r.DocumentNo,
                            RecPayID = r.RecPayID,
                            PartyName = s.SupplierName,
                            Amount =r.FMoney
                        }).ToList();
            //Reciepts.ForEach(d => d.Amount = (from s in Context1.RecPayDetails where s.RecPayID == d.RecPayID where s.Amount > 0 select s).ToList().Sum(a => a.Amount));
            var data = (from t in Reciepts where (t.RecPayDate >= Convert.ToDateTime(Session["FyearFrom"]) && t.RecPayDate <= Convert.ToDateTime(Session["FyearTo"])) select t).ToList();
            var result = data.GroupBy(p => p.RecPayID).Select(grp => grp.FirstOrDefault());

            if (ID > 0)
            {
                ViewBag.SuccessMsg = "You have successfully added Supplier Payment.";
            }


            if (ID == 10)
            {
                ViewBag.SuccessMsg = "You have successfully deleted Supplier Payment.";
            }

            if (ID == 20)
            {
                ViewBag.SuccessMsg = "You have successfully updated Supplier Payment.";
            }


            Session["ID"] = ID;


            return View(result);
        }
        [HttpGet]
        public ActionResult SupplierPayment(int id)
        {
            CustomerRcieptVM cust = new CustomerRcieptVM();
            cust.CustomerRcieptChildVM = new List<CustomerRcieptChildVM>();
            if (Session["UserID"] != null)
            {
                var branchid = Convert.ToInt32(Session["CurrentBranchID"]);

                if (id > 0)
                {
                    ViewBag.Title = "Supplier Payment - Modify";
                    cust = RP.GetSupplierRecPayByRecpayID(id);

                    var acheadforcash = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
                    ViewBag.SupplierType = Context1.SupplierTypes.ToList();
                    cust.recPayDetail = Context1.RecPayDetails.Where(item => item.RecPayID == id).ToList();
                    cust.CustomerRcieptChildVM = new List<CustomerRcieptChildVM>();
                    foreach (var item in cust.recPayDetail)
                    {
                        if (item.AcOPInvoiceDetailID > 0)
                        {
                            var sInvoiceDetail = (from d in Context1.AcOPInvoiceDetails where d.AcOPInvoiceDetailID == item.AcOPInvoiceDetailID select d).ToList();
                            if (sInvoiceDetail != null)
                            {
                                var invoicetotal = sInvoiceDetail.Sum(d => d.Amount); //  sInvoiceDetail.Sum(d=>d.OtherCharge);                                                                
                                var allrecpay = (from d in Context1.RecPayDetails where d.AcOPInvoiceDetailID == item.AcOPInvoiceDetailID select d).ToList();
                                var totamtpaid = allrecpay.Sum(d => d.Amount) * -1;
                                var totadjust = allrecpay.Sum(d => d.AdjustmentAmount);
                                //var CreditNote = (from d in Context1.CreditNotes where d.InvoiceID == item.InvoiceID && d.CustomerID == Sinvoice.CustomerID select d).ToList();
                                //decimal? CreditAmount = 0;
                                //if (CreditNote.Count > 0)
                                //{
                                //    CreditAmount = CreditNote.Sum(d => d.Amount);
                                //}
                                var totamt = totamtpaid + totadjust;// + CreditAmount;
                                var customerinvoice = new CustomerRcieptChildVM();
                                customerinvoice.InvoiceID = 0;
                                customerinvoice.AcOPInvoiceDetailID = sInvoiceDetail[0].AcOPInvoiceDetailID;
                                customerinvoice.InvoiceType = "OP";
                                customerinvoice.SInvoiceNo = sInvoiceDetail[0].InvoiceNo;
                                customerinvoice.strDate = Convert.ToDateTime(item.InvDate).ToString("dd/MM/yyyy");
                                customerinvoice.AmountToBeRecieved = Convert.ToDecimal(invoicetotal) - Convert.ToDecimal(totamt) - Convert.ToDecimal(item.Amount);
                                customerinvoice.AmountToBePaid = Convert.ToDecimal(invoicetotal) - customerinvoice.AmountToBeRecieved;
                                customerinvoice.Amount = Convert.ToDecimal(item.Amount) * -1;
                                customerinvoice.Balance = (Convert.ToDecimal(invoicetotal) - Convert.ToDecimal(totamt)) - Convert.ToDecimal(item.Amount); //  Convert.ToDecimal(sInvoiceDetail.NetValue - totamt);
                                customerinvoice.RecPayDetailID = item.RecPayDetailID;

                                customerinvoice.RecPayID = Convert.ToInt32(item.RecPayID);
                                customerinvoice.AdjustmentAmount = Convert.ToDecimal(item.AdjustmentAmount);
                                cust.CustomerRcieptChildVM.Add(customerinvoice);
                            }
                        }
                        else if (item.InvoiceID > 0 && item.AcOPInvoiceDetailID==0)
                        {
                            var sInvoiceDetail = (from d in Context1.SupplierInvoiceDetails where d.SupplierInvoiceID == item.InvoiceID select d).FirstOrDefault();
                            if (sInvoiceDetail != null)
                            {
                                var Sinvoice = (from d in Context1.SupplierInvoices where d.SupplierInvoiceID == sInvoiceDetail.SupplierInvoiceID select d).FirstOrDefault();
                                var allrecpay = (from d in Context1.RecPayDetails where d.InvoiceID == item.InvoiceID select d).ToList();
                                var totamtpaid = allrecpay.Sum(d => d.Amount) * -1;
                                var totadjust = allrecpay.Sum(d => d.AdjustmentAmount);
                                //var CreditNote = (from d in Context1.DebitNotes where d.InvoiceID == item.InvoiceID && d.SupplierID == Sinvoice.SupplierID select d).ToList();
                                //decimal? CreditAmount = 0;
                                //if (CreditNote.Count > 0)
                                //{
                                //    CreditAmount = CreditNote.Sum(d => d.Amount);
                                //}
                                var totamt = totamtpaid + totadjust;// + CreditAmount;
                                var customerinvoice = new CustomerRcieptChildVM();
                                customerinvoice.InvoiceID = Convert.ToInt32(item.InvoiceID);
                                customerinvoice.SInvoiceNo = Sinvoice.InvoiceNo;
                                customerinvoice.strDate = Convert.ToDateTime(item.InvDate).ToString("dd/MM/yyyy");
                                customerinvoice.AmountToBePaid = Convert.ToDecimal(totamtpaid);
                                customerinvoice.Amount = Convert.ToDecimal(item.Amount) * -1;
                                customerinvoice.Balance = Convert.ToDecimal(sInvoiceDetail.Value - totamt);
                                customerinvoice.RecPayDetailID = item.RecPayDetailID;
                                customerinvoice.AmountToBeRecieved = Convert.ToDecimal(sInvoiceDetail.Value);
                                customerinvoice.RecPayID = Convert.ToInt32(item.RecPayID);
                                customerinvoice.AdjustmentAmount = Convert.ToDecimal(item.AdjustmentAmount);
                                cust.CustomerRcieptChildVM.Add(customerinvoice);
                            }
                        }
                       
                    }

                    BindMasters_ForEdit(cust);
                }
                else
                {
                    ViewBag.Title = "Supplier Payment - Create";
                    BindAllMasters(2);
                    cust.CurrencyId = Convert.ToInt32(Session["CurrencyId"].ToString());
                    var acheadforcash = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
                    ViewBag.SupplierType = Context1.SupplierTypes.ToList();

                    cust.CurrencyId = Convert.ToInt32(Session["CurrencyId"].ToString());
                    cust.RecPayDate = System.DateTime.UtcNow;
                    List<CustomerRcieptChildVM> list = new List<CustomerRcieptChildVM>();
                    cust.CustomerRcieptChildVM = list;
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            var StaffNotes = (from d in Context1.StaffNotes where d.PageTypeId == 2 orderby d.NotesId descending select d).ToList();
            var users = (from d in Context1.UserRegistrations select d).ToList();

            var staffnotemodel = new List<StaffNoteModel>();
            foreach (var item in StaffNotes)
            {
                var model = new StaffNoteModel();
                model.id = item.NotesId;
                model.employeeid = item.EmployeeId;
                //model.jobid = item.JobId;
                model.TaskDetails = item.Notes;
                model.Datetime = item.EntryDate;
                model.EmpName = users.Where(d => d.UserID == item.EmployeeId).FirstOrDefault().UserName;
                staffnotemodel.Add(model);
            }
            ViewBag.StaffNoteModel = staffnotemodel;

            var CustomerNotification = (from d in Context1.CustomerNotifications where d.RecPayID == id && d.PageTypeId == 2 orderby d.NotificationId descending select d).ToList();

            var customernotification = new List<CustomerNotificationModel>();
            foreach (var item in CustomerNotification)
            {
                var model = new CustomerNotificationModel();
                model.id = item.NotificationId;
                model.employeeid = item.UserId;
                model.jobid = item.RecPayID;
                model.Message = item.MessageText;
                model.Datetime = item.EntryDate;
                model.IsEmail = item.NotifyByEmail;
                model.IsSms = item.NotifyBySMS;
                model.IsWhatsapp = item.NotifyByWhatsApp;
                model.EmpName = users.Where(d => d.UserID == item.UserId).FirstOrDefault().UserName;
                customernotification.Add(model);
            }
            ViewBag.CustomerNotification = customernotification;
            return View(cust);

        }
        [HttpPost]
        public ActionResult SupplierPayment(CustomerRcieptVM RecP)
        {
            int RPID = 0;
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int i = 0;
            RecP.FYearID = Convert.ToInt32(Session["fyearid"]);
            RecP.UserID = Convert.ToInt32(Session["UserID"]);
            var StaffNotes = (from d in Context1.StaffNotes where d.RecPayID == RecP.RecPayID && d.PageTypeId == 2 orderby d.NotesId descending select d).ToList();
            var branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            var users = (from d in Context1.UserRegistrations select d).ToList();

            var staffnotemodel = new List<StaffNoteModel>();
            foreach (var item in StaffNotes)
            {
                var model = new StaffNoteModel();
                model.id = item.NotesId;
                model.employeeid = item.EmployeeId;
                //model.jobid = item.JobId;
                model.TaskDetails = item.Notes;
                model.Datetime = item.EntryDate;
                model.EmpName = users.Where(d => d.UserID == item.EmployeeId).FirstOrDefault().UserName;
                staffnotemodel.Add(model);
            }
            ViewBag.StaffNoteModel = staffnotemodel;
            //if (RecP.RecPayID > 0)
            //{
            //    RP.EditCustomerRecPay(RecP, Session["UserID"].ToString());
            //    RP.EditCustomerRecieptDetails(RecP.recPayDetail, RecP.RecPayID);
            //}
            if (RecP.CashBank != null)
            {
                RecP.StatusEntry = "CS";
                int acheadid = Convert.ToInt32(RecP.CashBank);
                var achead = (from t in Context1.AcHeads where t.AcHeadID == acheadid select t.AcHead1).FirstOrDefault();
                RecP.BankName = achead;
            }
            else
            {
                RecP.StatusEntry = "BK";
                int acheadid = Convert.ToInt32(RecP.ChequeBank);
                var achead = (from t in Context1.AcHeads where t.AcHeadID == acheadid select t.AcHead1).FirstOrDefault();
                RecP.BankName = achead;
            }
            if (RecP.CustomerRcieptChildVM == null)
            {
                RecP.CustomerRcieptChildVM = new List<CustomerRcieptChildVM>();
            }
            //Adding Entry in Rec PAY

            ///Insert Entry For RecPay Details 
            ///
            if (RecP.RecPayID <= 0)
            {
                decimal Fmoney = 0;
                for (int j = 0; j < RecP.CustomerRcieptChildVM.Count; j++)
                {
                    Fmoney = Fmoney + Convert.ToDecimal(RecP.CustomerRcieptChildVM[j].Amount);
                }
                if (Fmoney > 0)
                {
                    RecP.AllocatedAmount = Fmoney;
                }
                else
                {

                }
                RecP.Balance =Convert.ToDecimal(RecP.FMoney) -Convert.ToDecimal(RecP.AllocatedAmount);

                RecP.AcCompanyID = branchid;
                RPID = ReceiptDAO.AddSupplierRecieptPayment(RecP, Session["UserID"].ToString()); //.AddCustomerRecieptPayment(RecP, Session["UserID"].ToString());

                RecP.RecPayID = (from c in Context1.RecPays orderby c.RecPayID descending select c.RecPayID).FirstOrDefault();
                decimal TotalAmount = 0;
                var recpitem = RecP.CustomerRcieptChildVM.Where(cc => cc.Amount > 0 || cc.AdjustmentAmount>0).ToList();
                foreach (var item in recpitem)
                {
                    //decimal Advance = 0;                    
                    //Advance = Convert.ToDecimal(item.Amount) - item.AmountToBeRecieved;
                    DateTime vInvoiceDate = Convert.ToDateTime(item.InvoiceDate);
                    string vInvoiceDate1 = Convert.ToDateTime(vInvoiceDate).ToString("yyyy-MM-dd h:mm tt");
                    if (item.Amount > 0 || item.AdjustmentAmount > 0)
                    {
                        var maxrecpaydetailid = (from c in Context1.RecPayDetails orderby c.RecPayDetailID descending select c.RecPayDetailID).FirstOrDefault();
                        string invoicetype = "S";
                        if (item.AcOPInvoiceDetailID != 0 && item.InvoiceID == 0)
                        {
                            invoicetype = "SOP";
                            ReceiptDAO.InsertRecpayDetailsForCust(RecP.RecPayID, item.AcOPInvoiceDetailID, item.InvoiceID, Convert.ToDecimal(-item.Amount), "", invoicetype, false, "", vInvoiceDate1, item.InvoiceNo.ToString(), Convert.ToInt32(RecP.CurrencyId), 3, item.JobID);
                        }
                        else
                        {
                            ReceiptDAO.InsertRecpayDetailsForCust(RecP.RecPayID, item.InvoiceID, item.InvoiceID, Convert.ToDecimal(-item.Amount), "", invoicetype, false, "", vInvoiceDate1, item.InvoiceNo.ToString(), Convert.ToInt32(RecP.CurrencyId), 3, item.JobID);
                        }
                        // ReceiptDAO.InsertRecpayDetailsForSupplier(RecP.RecPayID, item.InvoiceID, item.InvoiceID, Convert.ToDecimal(-item.Amount), "", "S", false, "", vInvoiceDate1, item.InvoiceNo.ToString(), Convert.ToInt32(RecP.CurrencyId), 3, item.JobID);

                        var recpaydetail = (from d in Context1.RecPayDetails where d.RecPayDetailID == maxrecpaydetailid + 1 select d).FirstOrDefault();
                        var recpd = recpaydetail;
                        recpaydetail.AdjustmentAmount = item.AdjustmentAmount;
                        Context1.Entry(recpd).State = EntityState.Modified;
                        Context1.SaveChanges();
                        //if (Advance > 0)
                        //{
                        //    //   Advance Amount entry
                        //    ReceiptDAO.InsertRecpayDetailsForSupplier(RecP.RecPayID, 0, 0, Advance, null, "S", true, null, null, null, Convert.ToInt32(RecP.CurrencyId), 4, item.JobID);
                        //}
                        if (invoicetype == "COP")
                        {
                            var salesinvoicedetails = (from d in Context1.AcOPInvoiceDetails
                                                       join m in Context1.AcOPInvoiceMasters on d.AcOPInvoiceMasterID equals m.AcOPInvoiceMasterID
                                                       where m.AcFinancialYearID == fyearid && d.AcOPInvoiceDetailID == item.AcOPInvoiceDetailID
                                                       select d).FirstOrDefault();
                            if (salesinvoicedetails != null)
                            {
                                var totamount = (from d in Context1.RecPayDetails where d.AcOPInvoiceDetailID == salesinvoicedetails.AcOPInvoiceDetailID select d).ToList();
                                var totsum = totamount.Sum(d => d.Amount);
                                var totAdsum = totamount.Sum(d => d.AdjustmentAmount);
                                var CreditNote = (from d in Context1.CreditNotes where d.InvoiceID == item.InvoiceID && d.CustomerID == item.CustomerID select d).ToList();
                                decimal? CreditAmount = 0;
                                if (CreditNote.Count > 0)
                                {
                                    CreditAmount = CreditNote.Sum(d => d.Amount);
                                }
                                var tamount = totsum + totAdsum + CreditAmount;
                                if (tamount >= salesinvoicedetails.Amount)
                                {
                                    salesinvoicedetails.RecPayStatus = 2;
                                }
                                else
                                {
                                    salesinvoicedetails.RecPayStatus = 1;
                                }
                                salesinvoicedetails.RecPayDetailId = maxrecpaydetailid + 1;
                                Context1.SaveChanges();
                            }
                        }
                        else
                        {

                            var salesinvoicedetails = (from d in Context1.CustomerInvoiceDetails where d.CustomerInvoiceID == item.InvoiceID select d).FirstOrDefault();
                            if (salesinvoicedetails != null)
                            {
                                var totamount = (from d in Context1.RecPayDetails where d.InvoiceID == salesinvoicedetails.CustomerInvoiceID select d).ToList();
                                var totsum = totamount.Sum(d => d.Amount);
                                var totAdsum = totamount.Sum(d => d.AdjustmentAmount);
                                var CreditNote = (from d in Context1.CreditNotes where d.InvoiceID == item.InvoiceID && d.CustomerID == item.CustomerID select d).ToList();
                                decimal? CreditAmount = 0;
                                if (CreditNote.Count > 0)
                                {
                                    CreditAmount = CreditNote.Sum(d => d.Amount);
                                }
                                var tamount = totsum + totAdsum + CreditAmount;
                                if (tamount >= salesinvoicedetails.NetValue)
                                {
                                    salesinvoicedetails.RecPayStatus = 2;
                                }
                                else
                                {
                                    salesinvoicedetails.RecPayStatus = 1;
                                }
                                salesinvoicedetails.RecPayDetailId = maxrecpaydetailid + 1;
                                Context1.SaveChanges();
                            }

                        }
                    }
                    TotalAmount = TotalAmount + Convert.ToDecimal(item.Amount);
                }
                if (RecP.Balance >0)
                {
                    int l = ReceiptDAO.InsertRecpayDetailsForSupplier(RecP.RecPayID, 0, 0, -1 *  Convert.ToDecimal(RecP.Balance), null, "S", true, null, null, null, Convert.ToInt32(RecP.CurrencyId), 4, 0);
                   
                }

                if (RecP.FMoney > 0)
                {
                    int l = ReceiptDAO.InsertRecpayDetailsForSupplier(RecP.RecPayID, 0, 0, Convert.ToDecimal(RecP.FMoney), null, "S", false, null, null, null, Convert.ToInt32(RecP.CurrencyId), 4, 0);

                }

                int fyaerId = Convert.ToInt32(Session["fyearid"].ToString());
                ReceiptDAO.InsertJournalOfSupplier(RecP.RecPayID, fyaerId);

                var Recpaydata = (from d in Context1.RecPays where d.RecPayID == RecP.RecPayID select d).FirstOrDefault();

                Recpaydata.RecPayID = RecP.RecPayID;
                Recpaydata.IsTradingReceipt = false;
                Context1.Entry(Recpaydata).State = EntityState.Modified;
                Context1.SaveChanges();

            }
            else //edit mode
            {
                decimal Fmoney = 0;
                for (int j = 0; j < RecP.CustomerRcieptChildVM.Count; j++)
                {
                    Fmoney = Fmoney + Convert.ToDecimal(RecP.CustomerRcieptChildVM[j].Amount);
                }

                RecP.AllocatedAmount = Fmoney;
                RecP.Balance = Convert.ToDecimal(RecP.FMoney) - Convert.ToDecimal(RecP.AllocatedAmount);
                RecPay recpay = new RecPay();
                recpay.RecPayDate = RecP.RecPayDate;
                recpay.RecPayID = RecP.RecPayID;
                recpay.AcJournalID = RecP.AcJournalID;
                recpay.BankName = RecP.BankName;
                recpay.ChequeDate = RecP.ChequeDate;
                recpay.ChequeNo = RecP.ChequeNo;
                recpay.SupplierID = RecP.SupplierID;
                recpay.DocumentNo = RecP.DocumentNo;
                recpay.EXRate = RecP.EXRate;
                recpay.FYearID = RecP.FYearID;
                recpay.FMoney = RecP.FMoney;
                recpay.StatusEntry = RecP.StatusEntry;
                recpay.IsTradingReceipt = true;
                recpay.Remarks = RecP.Remarks;
                Context1.Entry(recpay).State = EntityState.Modified;
                Context1.SaveChanges();
                var recpitem = RecP.CustomerRcieptChildVM.Where(cc => cc.Amount > 0 || cc.AdjustmentAmount>0).ToList();
                foreach (var item in recpitem)
                {
                    RecPayDetail recpd = new RecPayDetail();
                    recpd.RecPayDetailID = item.RecPayDetailID;
                    recpd.Amount = -(item.Amount);
                    recpd.CurrencyID = item.CurrencyId;
                    //recpd.InvDate = item.InvoiceDate.Value;
                    recpd.RecPayID = RecP.RecPayID;
                    recpd.Remarks = item.Remarks;
                    recpd.InvoiceID = item.InvoiceID;
                    recpd.StatusInvoice = "S";
                    Context1.Entry(recpd).State = EntityState.Modified;
                    Context1.SaveChanges();
                    if (recpd.InvoiceID > 0 && item.AcOPInvoiceDetailID == 0)
                    {
                        var salesinvoicedetails = (from d in Context1.SupplierInvoiceDetails where d.SupplierInvoiceID == item.InvoiceID select d).FirstOrDefault();
                        var totamount = (from d in Context1.RecPayDetails where d.InvoiceID == salesinvoicedetails.SupplierInvoiceDetailID select d).ToList();
                        var totsum = totamount.Sum(d => d.Amount) * -1;
                        var totAdsum = totamount.Sum(d => d.AdjustmentAmount);
                        var CreditNote = (from d in Context1.CreditNotes where d.InvoiceID == item.InvoiceID && d.CustomerID == item.CustomerID select d).ToList();
                        decimal? CreditAmount = 0;
                        if (CreditNote.Count > 0)
                        {
                            CreditAmount = CreditNote.Sum(d => d.Amount);
                        }
                        var tamount = totsum + totAdsum + CreditAmount;
                        if (tamount >= salesinvoicedetails.Value)
                        {
                            salesinvoicedetails.RecPayStatus = 2;
                        }
                        else
                        {
                            salesinvoicedetails.RecPayStatus = 1;
                        }
                        var maxrecpaydetailid = (from c in Context1.RecPayDetails orderby c.RecPayDetailID descending select c.RecPayDetailID).FirstOrDefault();

                        var recpaydet = (from d in Context1.RecPayDetails where d.InvoiceID == item.InvoiceID select d).FirstOrDefault();
                        if (recpaydet != null)
                        {
                            salesinvoicedetails.RecPayDetailId = recpaydet.RecPayDetailID;
                        }
                        else
                        {
                            salesinvoicedetails.RecPayDetailId = maxrecpaydetailid + 1;
                        }
                        Context1.SaveChanges();
                    }
                    else if (recpd.AcOPInvoiceDetailID > 0)
                    {
                        var salesinvoicedetails = (from d in Context1.AcOPInvoiceDetails where d.AcOPInvoiceDetailID == item.AcOPInvoiceDetailID select d).FirstOrDefault();
                        var totamount = (from d in Context1.RecPayDetails where d.AcOPInvoiceDetailID == salesinvoicedetails.AcOPInvoiceDetailID select d).ToList();
                        var totsum = totamount.Sum(d => d.Amount) * -1;
                        var totAdsum = totamount.Sum(d => d.AdjustmentAmount);
                        //var CreditNote = (from d in Context1.CreditNotes where d.InvoiceID == item.InvoiceID && d.CustomerID == item.CustomerID select d).ToList();
                        decimal? CreditAmount = 0;
                        //if (CreditNote.Count > 0)
                        //{
                        //    CreditAmount = CreditNote.Sum(d => d.Amount);
                        //}
                        var tamount = totsum + totAdsum + CreditAmount;
                        if (tamount >= salesinvoicedetails.Amount)
                        {
                            salesinvoicedetails.RecPayStatus = 2;
                        }
                        else
                        {
                            salesinvoicedetails.RecPayStatus = 1;
                        }
                        var maxrecpaydetailid = (from c in Context1.RecPayDetails orderby c.RecPayDetailID descending select c.RecPayDetailID).FirstOrDefault();

                        var recpaydet = (from d in Context1.RecPayDetails where d.AcOPInvoiceDetailID == item.AcOPInvoiceDetailID select d).FirstOrDefault();
                        if (recpaydet != null)
                        {
                            salesinvoicedetails.RecPayDetailId = recpaydet.RecPayDetailID;
                        }
                        else
                        {
                            salesinvoicedetails.RecPayDetailId = maxrecpaydetailid + 1;
                        }
                        Context1.SaveChanges();
                    }
                    if (RecP.Balance>0)    // ( item.AmountToBeRecieved < item.Amount)
                    {
                        RecPayDetail recpd1 = new RecPayDetail();
                        recpd1 = Context1.RecPayDetails.Where(cc => cc.RecPayID == 0 && cc.StatusAdvance == true && cc.InvoiceID == 0).FirstOrDefault();
                        bool newentry = false;
                        if (recpd1 == null)
                        {
                            recpd1 = new RecPayDetail();
                            recpd1.RecPayDetailID = (from c in Context1.RecPayDetails orderby c.RecPayDetailID descending select c.RecPayDetailID).FirstOrDefault();
                            newentry = true;
                        }
                        recpd1.Amount = RecP.Balance;
                        recpd1.RecPayID = RecP.RecPayID;
                        recpd1.Remarks = item.Remarks;
                        recpd1.CurrencyID = item.CurrencyId;
                        recpd1.StatusAdvance = true;
                        recpd.StatusInvoice = "S";
                        if (newentry == true)
                        {
                            Context1.RecPayDetails.Add(recpd1);
                            Context1.SaveChanges();
                        }
                        else
                        {
                            Context1.Entry(recpd1).State = EntityState.Modified;
                            Context1.SaveChanges();
                        }
                        
                    }


                }
                int editrecPay = 0;
                
                    var sumOfAmount = RecP.FMoney;
                    editrecPay = editfu.EditRecpayDetailsCustR(RecP.RecPayID, Convert.ToInt32(sumOfAmount));
                    int editAcJdetails = editfu.EditAcJDetails(RecP.AcJournalID.Value, Convert.ToInt32(sumOfAmount));
                
            }


            BindAllMasters(2);
            return RedirectToAction("SupplierPaymentDetails", "SupplierPayment", new { ID = 0 });
        }


        [HttpPost]
        public JsonResult GetTradeInvoiceOfSupplier(int? ID, decimal? amountreceived)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());

            DateTime fromdate = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime todate = Convert.ToDateTime(Session["FyearTo"].ToString());
            var receipt = (from d in Context1.RecPays where d.FYearID == fyearid && d.SupplierID == ID select d.FMoney).ToList();
            decimal receipts = 0;

            if (receipt!=null)
               receipts = receipt.Sum();
                
            var receiptdetails = (from d in Context1.RecPays join c in Context1.RecPayDetails on d.RecPayID equals c.RecPayID where d.FYearID == fyearid && d.SupplierID == ID && c.InvoiceID>0 && c.Amount<0 select (-1 * c.Amount)).Sum();

            var advance = receipts - receiptdetails;
            var salesinvoice = new List<CustomerTradeReceiptVM>();

            var AllOPInvoices = (from d in Context1.AcOPInvoiceDetails join m in Context1.AcOPInvoiceMasters on d.AcOPInvoiceMasterID equals m.AcOPInvoiceMasterID where m.AcFinancialYearID == fyearid && m.StatusSDSC != "C" && m.PartyID == ID && d.RecPayDetailId == null && (d.RecPayStatus == null || d.RecPayStatus < 2) select d).ToList();

            foreach (var item in AllOPInvoices)
            {
                decimal? totamt = 0;
                decimal? totamtpaid = 0;
                decimal? totadjust = 0;
                decimal? CreditAmount = 0;
                var allrecpay = (from d in Context1.RecPayDetails where d.AcOPInvoiceDetailID == item.AcOPInvoiceDetailID select d).ToList();
                totamtpaid = allrecpay.Sum(d => d.Amount) * -1;
                totadjust = allrecpay.Sum(d => d.AdjustmentAmount);
                totamt = totamtpaid + totadjust + CreditAmount;
                var Invoice = new CustomerTradeReceiptVM();
                Invoice.AcOPInvoiceDetailID = item.AcOPInvoiceDetailID;
                Invoice.SalesInvoiceID = 0;
                Invoice.InvoiceType = "OP";
                Invoice.JobCode = "OP" + item.AcOPInvoiceDetailID.ToString();
                Invoice.InvoiceNo = item.InvoiceNo; ;
                Invoice.InvoiceAmount = item.Amount * -1;
                Invoice.date = item.InvoiceDate;
                Invoice.DateTime = Convert.ToDateTime(item.InvoiceDate).ToString("dd/MM/yyyy");
                Invoice.AmountReceived = totamt;
                Invoice.Balance = Invoice.InvoiceAmount - totamtpaid;
                Invoice.AdjustmentAmount = totadjust;
                Invoice.Amount = 0;
                if (Invoice.Balance > 0)
                {
                    if (amountreceived != null)
                    {
                        if (amountreceived >= Invoice.Balance)
                        {
                            Invoice.Amount = Invoice.Balance;
                            amountreceived = amountreceived - Invoice.Amount;
                        }
                        else if (amountreceived > 0)
                        {
                            Invoice.Amount = amountreceived;
                            amountreceived = amountreceived - Invoice.Amount;
                        }
                        else
                        {
                            Invoice.Amount = 0;
                        }
                    }
                    salesinvoice.Add(Invoice);
                }

            }

            //transaction
            var AllInvoices = (from d in Context1.SupplierInvoices where d.SupplierID == ID select d).ToList();
            foreach (var item in AllInvoices)
            {
                //var invoicedeails = (from d in Context1.SalesInvoiceDetails where d.SalesInvoiceID == item.SalesInvoiceID where (d.RecPayStatus < 2 || d.RecPayStatus == null) select d).ToList();
                var invoicedeails = (from d in Context1.SupplierInvoiceDetails where d.SupplierInvoiceID == item.SupplierInvoiceID where (d.RecPayStatus < 2 || d.RecPayStatus == null) select d).ToList();
                //where (d.RecPayStatus < 2 || d.RecPayStatus == null) select d).ToList();
                decimal? totamt = 0;
                decimal? totamtpaid = 0;
                decimal? totadjust = 0;
                decimal? CreditAmount = 0;
                //foreach (var det in invoicedeails)
                //{
                var allrecpay = (from d in Context1.RecPayDetails where d.InvoiceID == item.SupplierInvoiceID select d).ToList();
                totamtpaid = allrecpay.Sum(d => d.Amount) * -1;
                totadjust = allrecpay.Sum(d => d.AdjustmentAmount);
                //var CreditNote = (from d in Context1.CreditNotes where d.InvoiceID == det.SalesInvoiceDetailID && d.CustomerID == item.CustomerID select d).ToList();
                //var CreditNote = (from d in Context1.CreditNotes where d.InvoiceID == det.CustomerInvoiceDetailID && d.CustomerID == item.CustomerID select d).ToList();

                //if (CreditNote.Count > 0)
                //{
                //    CreditAmount = CreditNote.Sum(d => d.Amount);
                //}
                totamt = totamtpaid + totadjust + CreditAmount;
                //}

                var Invoice = new CustomerTradeReceiptVM();
                //Invoice.JobID = det.JobID;
                Invoice.JobCode = "TR" + item.SupplierInvoiceID.ToString();
                Invoice.SalesInvoiceID = item.SupplierInvoiceID; // SalesInvoiceID;
                Invoice.InvoiceNo = item.InvoiceNo;
                //Invoice.SalesInvoiceDetailID = det.CustomerInvoiceDetailID;
                Invoice.InvoiceAmount = item.InvoiceTotal; // CourierCharge;
                Invoice.date = item.InvoiceDate;
                Invoice.DateTime = item.InvoiceDate.ToString("dd/MM/yyyy");
                //var RecPay = (from d in Context1.RecPayDetails where d.RecPayDetailID == det.RecPayDetailId select d).FirstOrDefault();

                Invoice.AmountReceived = totamt;
                Invoice.Balance = Invoice.InvoiceAmount - totamt;
                Invoice.AdjustmentAmount = totadjust;
                Invoice.Amount = 0;
                if (Invoice.Balance > 0)
                {
                    if (amountreceived != null)
                    {
                        if (amountreceived >= Invoice.Balance)
                        {
                            Invoice.Amount = Invoice.Balance;
                            amountreceived = amountreceived - Invoice.Amount;
                        }
                        else if (amountreceived > 0)
                        {
                            Invoice.Amount = amountreceived;
                            amountreceived = amountreceived - Invoice.Amount;
                        }
                        else
                        {
                            Invoice.Amount = 0;
                        }
                    }
                    salesinvoice.Add(Invoice);
                }
            }

            return Json(new { advance = advance, salesinvoice = salesinvoice }, JsonRequestBehavior.AllowGet);
        }
      
        [HttpGet]
        public JsonResult GetSupplierName(string term, int SupplierTypeId)
        {
            var customerlist = (from c1 in Context1.SupplierMasters
                                where c1.SupplierName.ToLower().Contains(term.ToLower()) && c1.SupplierTypeID == SupplierTypeId
                                orderby c1.SupplierName ascending
                                select new { SupplierID = c1.SupplierID, SupplierName = c1.SupplierName }).ToList();

            return Json(customerlist, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult ExportToPDF(int recpayid)
        //{
        //    //Report  
        //    try
        //    {
        //        decimal? totalamt = 0;
        //        int? currencyId = 0;

        //        ReportViewer reportViewer = new ReportViewer();

        //        reportViewer.ProcessingMode = ProcessingMode.Local;
        //        reportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/ReceiptVoucher.rdlc");

        //        DataTable dtcompany = new DataTable();
        //        dtcompany.Columns.Add("CompanyName");
        //        dtcompany.Columns.Add("Address1");
        //        dtcompany.Columns.Add("Address2");
        //        dtcompany.Columns.Add("Address3");
        //        dtcompany.Columns.Add("Phone");
        //        dtcompany.Columns.Add("AcHead");
        //        dtcompany.Columns.Add("Todate");

        //        var company = Context1.AcCompanies.FirstOrDefault();
        //        string imagePath = new Uri(Server.MapPath("~/Content/Logo/" + company.logo)).AbsoluteUri;

        //        DataRow dr = dtcompany.NewRow();
        //        dr[0] = company.AcCompany1;
        //        dr[1] = company.Address1;
        //        dr[2] = company.Address2;
        //        dr[3] = company.Address3;
        //        dr[4] = company.Phone;
        //        dr[5] = imagePath;
        //        dr[6] = DateTime.Now;

        //        dtcompany.Rows.Add(dr);

        //        var receipt = (from d in Context1.RecPays where d.RecPayID == recpayid select d).FirstOrDefault();
        //        totalamt = receipt.FMoney;

        //        if (receipt.IsTradingReceipt == true)
        //        {
        //            var recpaydetails = (from d in Context1.RecPayDetails where d.RecPayID == recpayid where d.InvoiceID > 0 select d).ToList();
        //            var currency = recpaydetails.Where(d => d.CurrencyID > 0).FirstOrDefault();
        //            if (currency != null)
        //            {
        //                currencyId = currency.CurrencyID;
        //            }
        //            var cust = Context1.CustomerMasters.Where(d => d.CustomerID == receipt.CustomerID).FirstOrDefault();
        //            var listofdet = new List<ReportCustomerReceipt_Result>();
        //            foreach (var item in recpaydetails)
        //            {
        //                var sinvoicedet = (from d in Context1.SalesInvoiceDetails where d.SalesInvoiceDetailID == item.InvoiceID select d).FirstOrDefault();
        //                var sinvoice = (from d in Context1.SalesInvoices where d.SalesInvoiceID == sinvoicedet.SalesInvoiceID select d).FirstOrDefault();
        //                var customerrecpay = new ReportCustomerReceipt_Result();
        //                customerrecpay.Date = receipt.RecPayDate.Value.ToString("dd-MMM-yyyy");
        //                customerrecpay.ReceivedFrom = cust.Customer1;
        //                customerrecpay.DocumentNo = receipt.DocumentNo;
        //                customerrecpay.Amount = Convert.ToDecimal(receipt.FMoney);
        //                customerrecpay.Remarks = receipt.Remarks;
        //                customerrecpay.Account = receipt.BankName;
        //                if (receipt.ChequeDate != null)
        //                {
        //                    customerrecpay.ChequeDate = receipt.ChequeDate.Value.ToString("dd-MMM-yyyy");
        //                }
        //                else
        //                {
        //                    customerrecpay.ChequeDate = "";
        //                }
        //                if (!string.IsNullOrEmpty(receipt.ChequeNo))
        //                {
        //                    customerrecpay.ChequeNo = Convert.ToDecimal(receipt.ChequeNo);
        //                }
        //                customerrecpay.CustomerBank = "";
        //                customerrecpay.DetailDocNo = sinvoice.SalesInvoiceNo;
        //                customerrecpay.DocDate = sinvoice.SalesInvoiceDate.Value.ToString("dd-MMM-yyyy");
        //                customerrecpay.DocAmount = Convert.ToDecimal(sinvoicedet.NetValue);

        //                if (item.Amount > 0)
        //                {
        //                    customerrecpay.SettledAmount = Convert.ToDecimal(item.Amount);
        //                    customerrecpay.AdjustmentAmount = Convert.ToInt32(item.AdjustmentAmount);
        //                }
        //                else
        //                {
        //                    customerrecpay.SettledAmount = Convert.ToDecimal(item.Amount) * -1;
        //                    customerrecpay.AdjustmentAmount = Convert.ToInt32(item.AdjustmentAmount);
        //                }
        //                listofdet.Add(customerrecpay);
        //            }

        //            ReportDataSource _rsource;

        //            //var dd = entity.ReportCustomerReceipt(recpayid).ToList();
        //            _rsource = new ReportDataSource("ReceiptVoucher", listofdet);
        //            reportViewer.LocalReport.DataSources.Add(_rsource);

        //        }
        //        else
        //        {
        //            var recpaydetails = (from d in Context1.RecPayDetails where d.RecPayID == recpayid where d.InvoiceID > 0 select d).ToList();
        //            var currency = recpaydetails.Where(d => d.CurrencyID > 0).FirstOrDefault();
        //            if (currency != null)
        //            {
        //                currencyId = currency.CurrencyID;
        //            }
        //            var cust = Context1.CUSTOMERs.Where(d => d.CustomerID == receipt.CustomerID).FirstOrDefault();
        //            var listofdet = new List<ReportCustomerReceipt_Result>();
        //            foreach (var item in recpaydetails)
        //            {
        //                var sinvoicedet = (from d in Context1.JInvoices where d.InvoiceID == item.InvoiceID select d).FirstOrDefault();
        //                var sinvoice = (from d in Context1.JobGenerations where d.JobID == sinvoicedet.JobID select d).FirstOrDefault();
        //                var customerrecpay = new ReportCustomerReceipt_Result();
        //                customerrecpay.Date = receipt.RecPayDate.Value.ToString("dd-MMM-yyyy");
        //                customerrecpay.ReceivedFrom = cust.Customer1;
        //                customerrecpay.DocumentNo = receipt.DocumentNo;
        //                customerrecpay.Amount = Convert.ToDecimal(receipt.FMoney);
        //                customerrecpay.Remarks = receipt.Remarks;
        //                customerrecpay.Account = receipt.BankName;
        //                if (receipt.ChequeDate != null)
        //                {
        //                    customerrecpay.ChequeDate = receipt.ChequeDate.Value.ToString("dd-MMM-yyyy");
        //                }
        //                else
        //                {
        //                    customerrecpay.ChequeDate = "";
        //                }
        //                if (!string.IsNullOrEmpty(receipt.ChequeNo))
        //                {
        //                    customerrecpay.ChequeNo = Convert.ToDecimal(receipt.ChequeNo);
        //                }
        //                customerrecpay.CustomerBank = "";
        //                customerrecpay.DetailDocNo = sinvoice.InvoiceNo.ToString();
        //                customerrecpay.DocDate = sinvoice.InvoiceDate.Value.ToString("dd-MMM-yyyy");
        //                customerrecpay.DocAmount = Convert.ToDecimal(sinvoicedet.SalesHome);

        //                if (item.Amount > 0)
        //                {
        //                    customerrecpay.SettledAmount = Convert.ToDecimal(item.Amount);
        //                    customerrecpay.AdjustmentAmount = Convert.ToInt32(item.AdjustmentAmount);
        //                }
        //                else
        //                {
        //                    customerrecpay.SettledAmount = Convert.ToDecimal(item.Amount) * -1;
        //                    customerrecpay.AdjustmentAmount = Convert.ToInt32(item.AdjustmentAmount);
        //                }
        //                listofdet.Add(customerrecpay);
        //            }

        //            ReportDataSource _rsource;

        //            //var dd = entity.ReportCustomerReceipt(recpayid).ToList();
        //            _rsource = new ReportDataSource("ReceiptVoucher", listofdet);
        //            reportViewer.LocalReport.DataSources.Add(_rsource);

        //        }
        //        ReportDataSource _rsource1 = new ReportDataSource("Company", dtcompany);


        //        reportViewer.LocalReport.DataSources.Add(_rsource1);



        //        //foreach (var item in dd)
        //        //{
        //        //    totalamt = 5000;
        //        //}


        //        //DataTable dtuser = new DataTable();
        //        //dtuser.Columns.Add("UserName");

        //        //DataRow dr1 = dtuser.NewRow();
        //        //int uid = Convert.ToInt32(Session["UserID"].ToString());
        //        //dr1[0] = (from c in entity.UserRegistrations where c.UserID == uid select c.UserName).FirstOrDefault();
        //        //dtuser.Rows.Add(dr1);

        //        //ReportDataSource _rsource2 = new ReportDataSource("User", dtuser);

        //        //ReportViewer1.LocalReport.DataSources.Add(_rsource2);


        //        DataTable dtcurrency = new DataTable();
        //        dtcurrency.Columns.Add("SalesCurrency");
        //        dtcurrency.Columns.Add("ForeignCurrency");
        //        dtcurrency.Columns.Add("SalesCurrencySymbol");
        //        dtcurrency.Columns.Add("ForeignCurrencySymbol");
        //        dtcurrency.Columns.Add("InWords");

        //        var currencyName = (from d in Context1.CurrencyMasters where d.CurrencyID == currencyId select d).FirstOrDefault();
        //        if (currencyName == null)
        //        {
        //            currencyName = new CurrencyMaster();
        //        }

        //        DataRow r = dtcurrency.NewRow();
        //        r[0] = currencyName.CurrencyName;
        //        r[1] = "";
        //        r[2] = "";
        //        r[3] = "";
        //        r[4] = currencyName.CurrencyName + ",  " + NumberToWords(Convert.ToInt32(totalamt)) + " /00 baisa.";


        //        dtcurrency.Rows.Add(r);


        //        ReportDataSource _rsource3 = new ReportDataSource("Currency", dtcurrency);

        //        reportViewer.LocalReport.DataSources.Add(_rsource3);
        //        reportViewer.LocalReport.EnableExternalImages = true;
        //        reportViewer.LocalReport.Refresh();

        //        //Byte  
        //        Warning[] warnings;
        //        string[] streamids;
        //        string mimeType, encoding, filenameExtension;

        //        byte[] bytes = reportViewer.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

        //        //File  
        //        string FileName = "Customer_" + DateTime.Now.Ticks.ToString() + ".pdf";
        //        string FilePath = Server.MapPath(@"~\TempFile\") + FileName;
        //        string path = Server.MapPath(@"~\TempFile\");
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        string[] files = Directory.GetFiles(path);

        //        foreach (string file in files)
        //        {
        //            FileInfo fi = new FileInfo(file);
        //            if (fi.LastAccessTime < DateTime.Now.AddMinutes(-5))
        //                try
        //                {
        //                    fi.Delete();
        //                }
        //                catch
        //                {

        //                }
        //        }
        //        //create and set PdfReader  
        //        PdfReader reader = new PdfReader(bytes);
        //        FileStream output = new FileStream(FilePath, FileMode.Create);

        //        string Agent = Request.Headers["User-Agent"].ToString();

        //        //create and set PdfStamper  
        //        PdfStamper pdfStamper = new PdfStamper(reader, output, '0', true);

        //        if (Agent.Contains("Firefox"))
        //            pdfStamper.JavaScript = "var res = app.loaded('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);');";
        //        else
        //            pdfStamper.JavaScript = "var res = app.setTimeOut('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);', 200);";

        //        pdfStamper.FormFlattening = false;
        //        pdfStamper.Close();
        //        reader.Close();

        //        //return file path  
        //        string FilePathReturn = @"TempFile/" + FileName;
        //        return Json(new { success = true, path = FilePathReturn }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);


        //    }
        //}
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
        public JsonResult UpdateStaffNote(int Jobid, string staffnote)
        {
            try
            {
                var note = new StaffNote();
                note.EntryDate = DateTime.Now;
                note.RecPayID = Jobid;
                note.Notes = staffnote;
                note.PageTypeId = 2;//job 
                note.EmployeeId = Convert.ToInt32(Session["UserID"]);
                Context1.StaffNotes.Add(note);
                Context1.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult SendCustomerNotification(int JobId, string Message, int Customerid, bool whatsapp, bool Email, bool sms)
        {
            var customer = (from d in Context1.CustomerMasters where d.CustomerID == Customerid select d).FirstOrDefault();
            var isemail = false;
            var issms = false;
            var iswhatsapp = false;
            if (Email)
            {
                try
                {
                    var status = SendMailForCustomerNotification(customer.CustomerName, Message, customer.Email);
                    isemail = true;
                }
                catch { }
            }
            if (sms)
            {
                try
                {
                    sendsms(Message);
                    issms = true;
                }
                catch (Exception e)
                {

                }
            }
            if (whatsapp)
            {
                iswhatsapp = true;

            }
            try
            {
                UpdateCustomerNotification(JobId, Message, isemail, issms, iswhatsapp);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);

            }
        }

        public string SendMailForCustomerNotification(string UserName, string Message, string Email)
        {
            var Success = "False";
            System.IO.StreamReader objReader;
            objReader = new System.IO.StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("/Templates/CustomerNotification.html"));
            string content = objReader.ReadToEnd();


            objReader.Close();
            content = Regex.Replace(content, "@username", UserName);
            content = Regex.Replace(content, "@Message", Message);
            try
            {
                using (MailMessage msgMail = new MailMessage())
                {

                    msgMail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmailAddress"].ToString());
                    msgMail.Subject = "Shipping System";
                    msgMail.IsBodyHtml = true;
                    msgMail.Body = HttpUtility.HtmlDecode(content);
                    msgMail.To.Add(Email);
                    msgMail.IsBodyHtml = true;

                    //client = new SmtpClient(ConfigurationManager.AppSettings["Host"].ToString());
                    //client.Port = int.Parse(ConfigurationManager.AppSettings["SMTPServerPort"].ToString());
                    //client.UseDefaultCredentials = false;
                    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTPUserName"].ToString(), ConfigurationManager.AppSettings["SMTPPassword"].ToString());
                    //client.EnableSsl = true;
                    //client.Send(msgMail);
                    using (SmtpClient smtp = new SmtpClient("smtp.mail.yahoo.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTPUserName"].ToString(), ConfigurationManager.AppSettings["SMTPPassword"].ToString());
                        smtp.EnableSsl = true;
                        smtp.Send(msgMail);
                    }
                }
                Success = "True";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return Success;
        }
        public bool UpdateCustomerNotification(int Jobid, string Messge, bool isemail, bool issms, bool iswhatsapp)
        {
            try
            {
                var note = new CustomerNotification();
                note.EntryDate = DateTime.Now;
                //note.JobId = Jobid;
                note.MessageText = Messge;
                note.PageTypeId = 2;//job 
                note.UserId = Convert.ToInt32(Session["UserID"]);
                note.NotifyByEmail = isemail;
                note.NotifyBySMS = issms;
                note.NotifyByWhatsApp = iswhatsapp;
                Context1.CustomerNotifications.Add(note);
                Context1.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;

            }
        }
        public string sendsms(string Message)
        {
            //var res= sendSMS();
            // return res;
            String message = HttpUtility.UrlEncode(Message);
            using (var wb = new WebClient())
            {

                byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                {
                {"apikey" , "iCglLGvDnCM-UaAfKLWZ1cEveOQhCfSAakkqn86jbv"},
                {"numbers" , "919344452870"},
                {"message" , message},
                {"sender" , "MTRADE"}
                });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }
        public string sendSMS()
        {
            String result;
            string apiKey = "iCglLGvDnCM-UaAfKLWZ1cEveOQhCfSAakkqn86jbv";
            string numbers = "919344452870"; // in a comma seperated list
            string message = "This is your message";
            string sender = "MTRADE";

            String url = "https://api.textlocal.in/send/?apikey=" + apiKey + "&numbers=" + numbers + "&message=" + message + "&sender=" + sender;
            //refer to parameters to complete correct url string

            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);

            objRequest.Method = "POST";
            objRequest.ContentLength = Encoding.UTF8.GetByteCount(url);
            objRequest.ContentType = "application/x-www-form-urlencoded";
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(url);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                myWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                // Close and clean up the StreamReader
                sr.Close();
            }
            return result;
        }
        public void sendsmsss(string Message)
        {
            var message = ""; var from = "MTRADE";
            var uname = "veepeek@yahoo.com"; var hash = "d9fe2afa2f0b66e8418ffcc2f892259b04ddbcc37c22674c5717f2f7a8e21ad0";
            var selectednums = "9344452870"; var url = "";
            var address = "https://www.txtlocal.com/sendsmspost.php";
            var info = 1; var test = 1;

            message = Message;
            message = HttpUtility.UrlEncode(message);
            //encode special characters (e.g. £, & etc) 
            from = ""; uname = ""; hash = ""; selectednums = "";
            url = address + "?uname=" + uname + "&hash=" + hash + "&message=" + message + "&from=" + from + "&selectednums=" + selectednums + "&info=" + info + "&test=" + test;
            Response.Redirect(url);

        }

    }
}
