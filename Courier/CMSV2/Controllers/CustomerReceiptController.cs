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
    public class CustomerReceiptController : Controller
    {
        SourceMastersModel MM = new SourceMastersModel();
        RecieptPaymentModel RP = new RecieptPaymentModel();
        CustomerRcieptVM cust = new CustomerRcieptVM();
        Entities1 Context1 = new Entities1();

        EditCommanFu editfu = new EditCommanFu();

        [HttpGet]
        public ActionResult CustomerTradeReceiptDetails(int ID, string FromDate, string ToDate)
        {
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            List<ReceiptVM> Reciepts = new List<ReceiptVM>();
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

            Reciepts = ReceiptDAO.GetCustomerReceipts(FyearId, pFromDate, pToDate); // RP.GetAllReciepts();
            ViewBag.FromDate = pFromDate.Date.ToString("dd-MM-yyyy");
            ViewBag.ToDate = pToDate.Date.ToString("dd-MM-yyyy");
            // var data = (from t in Reciepts where (t.RecPayDate >= Convert.ToDateTime(Session["FyearFrom"]) && t.RecPayDate <= Convert.ToDateTime(Session["FyearTo"])) select t).ToList();
            if (ID > 0)
            {
                ViewBag.SuccessMsg = "You have successfully added Customer Reciept.";
            }


            if (ID == 10)
            {
                ViewBag.SuccessMsg = "You have successfully deleted Customer Reciept.";
            }

            if (ID == 20)
            {
                ViewBag.SuccessMsg = "You have successfully updated Customer Reciept.";
            }


            Session["ID"] = ID;


            return View(Reciepts);
        }
        //
        // GET: /CustomerReciept/
        [HttpGet]
        public ActionResult CustomerReciept(int id)
        {
            CustomerRcieptVM cust = new CustomerRcieptVM();
            cust.CustomerRcieptChildVM = new List<CustomerRcieptChildVM>();
            var branchid = Convert.ToInt32(Session["CurrentBranchID"]);

            if (Session["UserID"] != null)
            {

                if (id > 0)
                {
                    cust = RP.GetRecPayByRecpayID(id);

                    var acheadforcash = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;

                    //var acheadforcash = (from d in Context1.AcHeads
                    //                     join
                    //                    s in Context1.AcGroups on d.AcGroupID equals s.AcGroupID
                    //                     join
                    //                     t in Context1.AcTypes on s.AcTypeId equals t.Id
                    //                     where
                    //                     t.AccountType.ToLower() == "cash" && s.AcBranchID == branchid
                    //                     select d).ToList();
                    //var acheadforbank = (from d in Context1.AcHeads
                    //                     join
                    //                    s in Context1.AcGroups on d.AcGroupID equals s.AcGroupID
                    //                     join
                    //                     t in Context1.AcTypes on s.AcTypeId equals t.Id
                    //                     where
                    //                     t.AccountType.ToLower() == "bank" && s.AcBranchID == branchid
                    //                     select d).ToList();
                    //ViewBag.achead = Context1.AcHeadSelectForCash(Convert.ToInt32(Session["AcCompanyID"].ToString())).ToList();
                    //ViewBag.acheadbank = Context1.AcHeadSelectForBank(Convert.ToInt32(Session["AcCompanyID"].ToString())).ToList();
                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
                    //ViewBag.achead = Context1.AcHeadSelectForCash(Convert.ToInt32(Session["AcCompanyID"].ToString())).ToList();
                    //ViewBag.acheadbank = Context1.AcHeadSelectForBank(Convert.ToInt32(Session["AcCompanyID"].ToString())).ToList();
                    cust.recPayDetail = Context1.RecPayDetails.Where(item => item.RecPayID == id).ToList();
                    //cust.CustomerRcieptChildVM = (from t in Context1.JInvoices
                    //                              join
                    //                                  p in Context1.RecPayDetails on t.InvoiceID equals p.InvoiceID
                    //                              join s in Context1.RecPays on p.RecPayID equals s.RecPayID

                    //                              where (s.RecPayID == id && p.InvoiceID != 0)
                    //                              select new CustomerRcieptChildVM
                    //                              {
                    //                                  InvoiceDate = s.RecPayDate,
                    //                                  InvoiceID = t.InvoiceID,
                    //                                  AmountToBeRecieved = t.SalesHome.Value,
                    //                                  Amount = -(p.Amount.Value),

                    //                                  Balance = t.SalesHome.Value,
                    //                                  RecPayDetailID = p.RecPayDetailID,
                    //                                  CurrencyId = p.CurrencyID.Value



                    //                              }).OrderBy(x => x.InvoiceDate).ToList();
                    cust.CustomerRcieptChildVM = MM.GetCustomerReceiptDetail(id);                    
//                                                 DAL.GetCustomerReciept(id);                    
                    BindMasters_ForEdit(cust);
                }
                else
                {
                    BindAllMasters(1);
                    //ViewBag.achead = Context1.AcHeads.ToList().Where(x => x.AcGroupID == 10);
                    //ViewBag.acheadbank = Context1.AcHeads.ToList().Where(x => x.AcGroupID == 49);

                    var acheadforcash = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
                    
                    //var acheadforcash = (from d in Context1.AcHeads
                    //                     join
                    //                    s in Context1.AcGroups on d.AcGroupID equals s.AcGroupID
                    //                     join
                    //                     t in Context1.AcTypes on s.AcTypeId equals t.Id
                    //                     where
                    //                     t.AccountType.ToLower() == "cash" && s.AcBranchID == branchid
                    //                     select d).ToList();
                    //var acheadforbank = (from d in Context1.AcHeads
                    //                     join
                    //                    s in Context1.AcGroups on d.AcGroupID equals s.AcGroupID
                    //                     join
                    //                     t in Context1.AcTypes on s.AcTypeId equals t.Id
                    //                     where
                    //                     t.AccountType.ToLower() == "bank" && s.AcBranchID == branchid
                    //                     select d).ToList();
                    
                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
                    cust.RecPayDate = System.DateTime.UtcNow;
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            //var StaffNotes = (from d in Context1.StaffNotes where d.JobId == id && d.PageTypeId == 2 orderby d.Id descending select d).ToList();
            var StaffNotes = (from d in Context1.StaffNotes  orderby d.NotesId descending select d).ToList();
            var users = (from d in Context1.UserRegistrations select d).ToList();
          
            var staffnotemodel = new List<StaffNoteModel>();
            //foreach (var item in StaffNotes)
            //{
            //    var model = new StaffNoteModel();
            //    model.id = item.Id;
            //    model.employeeid = item.EmployeeId;
            //    model.jobid = item.JobId;
            //    model.TaskDetails = item.TaskDetails;
            //    model.Datetime = item.Datetime;
            //    model.EmpName = users.Where(d => d.UserID == item.EmployeeId).FirstOrDefault().UserName;
            //    staffnotemodel.Add(model);
            //}
            ViewBag.StaffNoteModel = staffnotemodel;
            //var customerdetails = (from d in Context1.CustomerMasters where d.CustomerID == cust.CustomerID && d.CustomerType==1 select d).FirstOrDefault();
            var customerdetails = (from d in Context1.CustomerMasters where d.CustomerID == cust.CustomerID select d).FirstOrDefault();
            if (customerdetails == null)
            {
                customerdetails = new CustomerMaster();
            }
            ViewBag.CustomerDetail = customerdetails;
            //var CustomerNotification = (from d in Context1.CustomerNotifications where d.JobId == id && d.PageTypeId == 2 orderby d.Id descending select d).ToList();
            var CustomerNotification = (from d in Context1.CustomerNotifications orderby d.NotificationId descending select d).ToList();

            var customernotification = new List<CustomerNotificationModel>();
            foreach (var item in CustomerNotification)
            {
                var model = new CustomerNotificationModel();
                model.id = item.NotificationId;
                model.employeeid = item.UserId;
                //model.jobid = item.JobId;
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

        //[HttpPost]
        //public ActionResult CustomerReciept(CustomerRcieptVM RecP, string Command, string Currency)
        //{
        //    List<SP_GetCustomerInvoiceDetailsForReciept_Result> AllInvoices = new List<SP_GetCustomerInvoiceDetailsForReciept_Result>();
        //    List<SP_GetJInvoiceDetailsByInvoiceID_Result> InvDetails = new List<SP_GetJInvoiceDetailsByInvoiceID_Result>();

        //    int RPID = 0;
        //    int i = 0;
        //    RecP.FYearID = Convert.ToInt32(Session["fyearid"]);
        //    RecP.UserID = Convert.ToInt32(Session["UserID"]);

        //    if (RecP.RecPayID > 0)
        //    {
        //        RP.EditCustomerRecPay(RecP, Session["UserID"].ToString());
        //        RP.EditCustomerRecieptDetails(RecP.recPayDetail, RecP.RecPayID);
        //    }
        //    else
        //    {
        //        if (Session["UserID"] != null)
        //        {
        //            if (RecP.CashBank != null)
        //            {
        //                RecP.BankName = RecP.CashBank;
        //            }
        //            else
        //            {
        //                RecP.BankName = RecP.ChequeBank;

        //            }
        //            i = RP.AddCustomerRecieptPayment(RecP, Session["UserID"].ToString());

        //            if (i > 0)
        //            {
        //                RPID = RP.GetMaxRecPayID();
        //            }

        //            if (RPID > 0)
        //            {
        //                //RecPayDetails Records Inserting Logic

        //                //Total Amount calculating using ExRate and enterted Amount
        //                decimal TotalAmount = Convert.ToDecimal(RecP.EXRate * RecP.FMoney);
        //                decimal AdvanceAmtToinsert = TotalAmount;

        //                //Advance Amount getting for customer
        //                decimal AdvanceAmount = RP.GetAdvanceAmount(Convert.ToInt32(RecP.CustomerID));

        //                //All Pending Invoices getting
        //                AllInvoices = RP.GetCustomerInvoiceDetails(Convert.ToInt32(RecP.CustomerID));

        //                decimal InvoiceAmt = 0;

        //                foreach (var item in AllInvoices)
        //                {
        //                    DateTime dateTime = Convert.ToDateTime(item.InvoiceDate);
        //                    string newd = Convert.ToDateTime(dateTime).ToString("yyyy-MM-dd h:mm tt");
        //                    InvDetails = RP.InvDtls(item.InvoiceID);

        //                    foreach (var item1 in InvDetails)
        //                    {
        //                        InvoiceAmt = Convert.ToDecimal(item1.SalesHome);
        //                    }

        //                    if (item.AmtPaidTillDate == 0)
        //                    {
        //                        if (AdvanceAmtToinsert >= item.AmountToBeRecieved)
        //                        {

        //                            int k = RP.InsertRecpayDetailsForCust(RPID, item.InvoiceID, Convert.ToDecimal(-item.AmountToBeRecieved), "", "C", false, "", newd, item.InvoiceNo.ToString(), Convert.ToInt32(RecP.CurrencyId), 3);

        //                            AdvanceAmtToinsert = AdvanceAmtToinsert - Convert.ToDecimal(item.AmountToBeRecieved);

        //                        }
        //                        else if (item.AmountToBeRecieved > AdvanceAmtToinsert)
        //                        {
        //                            int k = RP.InsertRecpayDetailsForCust(RPID, item.InvoiceID, Convert.ToDecimal(-AdvanceAmtToinsert), "", "C", false, "", newd, item.InvoiceNo.ToString(), Convert.ToInt32(RecP.CurrencyId), 2);

        //                            AdvanceAmtToinsert = AdvanceAmtToinsert - Convert.ToDecimal(AdvanceAmtToinsert);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (AdvanceAmtToinsert >= item.Balance)
        //                        {

        //                            int k = RP.InsertRecpayDetailsForCust(RPID, item.InvoiceID, Convert.ToDecimal(-item.Balance), "", "C", false, "", newd, item.InvoiceNo.ToString(), Convert.ToInt32(RecP.CurrencyId), 3);

        //                            AdvanceAmtToinsert = AdvanceAmtToinsert - Convert.ToDecimal(item.Balance);

        //                        }
        //                        else if (item.Balance > AdvanceAmtToinsert)
        //                        {
        //                            int k = RP.InsertRecpayDetailsForCust(RPID, item.InvoiceID, Convert.ToDecimal(-AdvanceAmtToinsert), "", "C", false, "", newd, item.InvoiceNo.ToString(), Convert.ToInt32(RecP.CurrencyId), 2);

        //                            AdvanceAmtToinsert = AdvanceAmtToinsert - Convert.ToDecimal(AdvanceAmtToinsert);
        //                        }
        //                    }

        //                }


        //                int l = RP.InsertRecpayDetailsForCust(RPID, 0, TotalAmount, null, "C", false, null, null, null, Convert.ToInt32(RecP.CurrencyId), 4);

        //                //Advance Amount entry
        //                if (AdvanceAmtToinsert > 0)
        //                {
        //                    int k = RP.InsertRecpayDetailsForCust(RPID, 0, AdvanceAmtToinsert, null, "C", true, null, null, null, Convert.ToInt32(RecP.CurrencyId), 4);
        //                }

        //                int fyaerId = Convert.ToInt32(Session["fyearid"].ToString());

        //                RP.InsertJournalOfCustomer(RPID, fyaerId);
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("Login", "Login");
        //        }
        //    }

        //    BindAllMasters();


        //    return RedirectToAction("CustomerRecieptDetails", "CustomerReciept", new { ID = RPID });


        //}

        [HttpPost]
        public ActionResult CustomerReciept(CustomerRcieptVM RecP, string Command, string Currency)
        {
            int RPID = 0;
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
                model.jobid = item.RecPayID;
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
            if(RecP.CustomerRcieptChildVM==null)
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
                    RecP.FMoney = Fmoney;
                }
                RPID = ReceiptDAO.AddCustomerRecieptPayment(RecP, Session["UserID"].ToString());

                RecP.RecPayID = (from c in Context1.RecPays orderby c.RecPayID descending select c.RecPayID).FirstOrDefault();
                decimal TotalAmount = 0;
                foreach (var item in RecP.CustomerRcieptChildVM)
                {
                    decimal Advance = 0;
                    //if (item.Amount > 0 && (item.AmountToBeRecieved < item.Amount || item.AmountToBeRecieved == item.Amount))///900<1000
                    //{
                    // item.Amount = Convert.ToDecimal(RecP.EXRate * item.Amount);
                    //100=1000-900
                    Advance = Convert.ToDecimal(item.Amount) -Convert.ToDecimal(item.AmountToBeRecieved);
                    DateTime vInvoiceDate = Convert.ToDateTime(item.InvoiceDate);
                    string vInvoiceDate1 = Convert.ToDateTime(vInvoiceDate).ToString("yyyy-MM-dd h:mm tt");
                    //RP.InsertRecpayDetailsForCust(RecP.RecPayID, item.InvoiceID, item.InvoiceID, Convert.ToDecimal(-item.Amount), "", "C", false, "", vInvoiceDate1, item.InvoiceNo.ToString(), Convert.ToInt32(RecP.CurrencyId), 3, item.JobID);

                    if (Advance > 0)
                    {
                        //Advance Amount entry
                        //RP.InsertRecpayDetailsForCust(RecP.RecPayID, 0, 0, Advance, null, "C", true, null, null, null, Convert.ToInt32(RecP.CurrencyId), 4, item.JobID);
                    }
                    TotalAmount = Convert.ToDecimal(TotalAmount) +  Convert.ToDecimal(item.Amount);
                }
                if(RecP.CustomerRcieptChildVM.Count==0)
                {
                    //RP.InsertRecpayDetailsForCust(RecP.RecPayID, 0, 0,Convert.ToDecimal(RecP.FMoney), null, "C", true, null, null, null, Convert.ToInt32(RecP.CurrencyId), 4, 0);
                    int fyaerId = Convert.ToInt32(Session["fyearid"].ToString());
                    //RP.InsertJournalOfCustomer(RecP.RecPayID, fyaerId);
                }

                //To Balance Invoice AMount
                if (TotalAmount > 0)
                {
                    //int l = RP.InsertRecpayDetailsForCust(RecP.RecPayID, 0, 0, TotalAmount, null, "C", false, null, null, null, Convert.ToInt32(RecP.CurrencyId), 4, 0);
                    int fyaerId = Convert.ToInt32(Session["fyearid"].ToString());
                    //RP.InsertJournalOfCustomer(RecP.RecPayID, fyaerId);
                }
                var Recpaydata = (from d in Context1.RecPays where d.RecPayID == RecP.RecPayID select d).FirstOrDefault();

                Recpaydata.RecPayID = RecP.RecPayID;
                Recpaydata.IsTradingReceipt = false;
                Context1.Entry(Recpaydata).State = EntityState.Modified;
                Context1.SaveChanges();
            }
            else
            {
                decimal Fmoney = 0;
                for (int j = 0; j < RecP.CustomerRcieptChildVM.Count; j++)
                {
                    Fmoney = Fmoney + Convert.ToDecimal(RecP.CustomerRcieptChildVM[j].Amount);
                }

                RecPay recpay = new RecPay();
                recpay.RecPayDate = RecP.RecPayDate;
                recpay.RecPayID = RecP.RecPayID;
                recpay.AcJournalID = RecP.AcJournalID;
                recpay.BankName = RecP.BankName;
                recpay.ChequeDate = RecP.ChequeDate;
                recpay.ChequeNo = RecP.ChequeNo;
                recpay.CustomerID = RecP.CustomerID;
                recpay.DocumentNo = RecP.DocumentNo;
                recpay.EXRate = RecP.EXRate;
                recpay.FYearID = RecP.FYearID;
                recpay.FMoney = Fmoney;
                recpay.StatusEntry = RecP.StatusEntry;
                recpay.IsTradingReceipt = true;

                //recpay.SupplierID = RecP.SupplierID;
                Context1.Entry(recpay).State = EntityState.Modified;
                Context1.SaveChanges();

                foreach (var item in RecP.CustomerRcieptChildVM)
                {
                    RecPayDetail recpd = new RecPayDetail();
                    recpd.RecPayDetailID = item.RecPayDetailID;
                    recpd.Amount = -(item.Amount);
                    recpd.CurrencyID = item.CurrencyId;
                    //recpd.InvDate = item.InvoiceDate.Value;
                    recpd.RecPayID = RecP.RecPayID;
                    recpd.Remarks = item.Remarks;
                    recpd.InvoiceID = item.InvoiceID;
                    recpd.StatusInvoice = "C";
                    /*  if (item.AmountToBeRecieved < item.Amount)
                      {
                          RecPayDetail recpd1 = new RecPayDetail();
                          recpd1.RecPayDetailID = (from c in Context1.RecPayDetails orderby c.RecPayDetailID descending select c.RecPayDetailID).FirstOrDefault();
                          recpd1.Amount = item.AmountToBeRecieved - item.Amount;
                          recpd1.RecPayID = RecP.RecPayID;
                          recpd1.Remarks = item.Remarks;
                          recpd1.CurrencyID = item.CurrencyId;
                          recpd1.StatusAdvance = true;
                          recpd.StatusInvoice = "C";
                          Context1.Entry(recpd1).State = EntityState.Modified;
                          Context1.SaveChanges();
                      }*/

                    Context1.Entry(recpd).State = EntityState.Modified;
                    Context1.SaveChanges();
                }
                int editrecPay = 0;
                var sumOfAmount = Context1.RecPayDetails.Where(m => m.RecPayID == RecP.RecPayID && m.InvoiceID != 0).Sum(c => c.Amount);
                editrecPay = editfu.EditRecpayDetailsCustR(RecP.RecPayID, Convert.ToInt32(sumOfAmount));
                int editAcJdetails = editfu.EditAcJDetails(RecP.AcJournalID.Value, Convert.ToInt32(sumOfAmount));
            }

            BindAllMasters(1);
            return RedirectToAction("CustomerRecieptDetails", "CustomerReciept", new { ID = RecP.RecPayID });
        }


        //[HttpGet]
        //public ActionResult CustomerRecieptDetails(int ID)
        //{
        //    int FyearId = Convert.ToInt32(Session["fyearid"]);
        //    List<ReceiptVM> Reciepts = new List<ReceiptVM>();

        //    Reciepts = ReceiptDAO.GetCustomerReceipts(FyearId); // RP.GetAllReciepts();
        //    //var data = (from t in Reciepts where (t.RecPayDate >= Convert.ToDateTime(Session["FyearFrom"]) && t.RecPayDate <= Convert.ToDateTime(Session["FyearTo"])) select t).ToList();

        //    if (ID > 0)
        //    {
        //        ViewBag.SuccessMsg = "You have successfully added Customer Reciept.";
        //    }


        //    if (ID == 10)
        //    {
        //        ViewBag.SuccessMsg = "You have successfully deleted Customer Reciept.";
        //    }

        //    if (ID == 20)
        //    {
        //        ViewBag.SuccessMsg = "You have successfully updated Customer Reciept.";
        //    }


        //    Session["ID"] = ID;


        //    return View(Reciepts);
        //}

        public JsonResult GetInvoiceOfCustomer(string ID)
        {
            //List<SP_GetCustomerInvoiceDetailsForReciept_Result> AllInvoices = new List<SP_GetCustomerInvoiceDetailsForReciept_Result>();

            DateTime fromdate = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime todate = Convert.ToDateTime(Session["FyearTo"].ToString());
            var AllInvoices = ReceiptDAO.GetCustomerInvoiceDetailsForReciept(Convert.ToInt32(ID),fromdate.Date.ToString(), todate.Date.ToString()).OrderBy(x => x.InvoiceDate).ToList();

            return Json(AllInvoices, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExchangeRateByCurID(string ID)
        {
            //List<SP_GetCustomerInvoiceDetailsForReciept_Result> AllInvoices = new List<SP_GetCustomerInvoiceDetailsForReciept_Result>();

            var ER = RP.GetExchgeRateByCurID(Convert.ToInt32(ID));

            return Json(ER, JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult DeleteCustomerDetTrade(int id)
        {
            //int k = 0;
            if (id != 0)
            {
               DataTable dt= ReceiptDAO.DeleteCustomerReceipt(id);
                if (dt !=null)                     
                {
                    if (dt.Rows.Count>0)
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
            
            return RedirectToAction("CustomerTradeReceiptDetails", "CustomerReceipt", new { ID = 10 });

        }

        public JsonResult ReceiptReport(int id)
        {
            string reportpath = "";
            //int k = 0;
            if (id != 0)
            {
                reportpath=AccountsReportsDAO.GenerateCustomerReceipt(id);                
                
            }

            return Json(new { path =  reportpath , result = "ok" }, JsonRequestBehavior.AllowGet);

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

            string DocNo = RP.GetMaxRecieptDocumentNo();

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

        //public JsonResult GetAllCustomer()
        //{
        //    DateTime d = DateTime.Now;
        //    DateTime fyear = Convert.ToDateTime(Session["FyearFrom"].ToString());
        //    int FyearId = Convert.ToInt32(Session["fyearid"]);
        //    DateTime mstart = new DateTime(fyear.Year, d.Month, 01);

        //    int maxday = DateTime.DaysInMonth(fyear.Year, d.Month);
        //    DateTime mend = new DateTime(fyear.Year, d.Month, maxday);

        //    var cust = ReceiptDAO.GetCustomerReceipts(FyearId).Where(x => x.RecPayDate >= mstart && x.RecPayDate <= mend).OrderByDescending(x => x.RecPayDate).ToList();
        //    //Context1.SP_GetAllRecieptsDetails().Where(x => x.RecPayDate >= mstart && x.RecPayDate <= mend).OrderByDescending(x => x.RecPayDate).ToList();

        //    string view = this.RenderPartialView("_GetAllCustomer", cust);

        //    return new JsonResult
        //    {
        //        Data = new
        //        {
        //            success = true,
        //            view = view
        //        },
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //    };
        //}




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
        public ActionResult CustomerTradeReceipt(int id)
        {
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            CustomerRcieptVM cust = new CustomerRcieptVM();
            cust.CustomerRcieptChildVM = new List<CustomerRcieptChildVM>();
            cust.AWBAllocation = new List<ReceiptAllocationDetailVM>();
            if (Session["UserID"] != null)
            {
                var branchid = Convert.ToInt32(Session["CurrentBranchID"]);

                if (id > 0)
                {
                    ViewBag.Title = "Customer Receipt - Modify";
                    cust = RP.GetRecPayByRecpayID(id);

                    var acheadforcash = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
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
                                //var allrecpay = (from d in Context1.RecPayDetails where d.AcOPInvoiceDetailID == item.AcOPInvoiceDetailID select d).ToList();
                                var allrecpay = (from d in Context1.RecPayDetails join c in Context1.RecPays on d.RecPayID equals c.RecPayID where c.RecPayDate < cust.RecPayDate && d.AcOPInvoiceDetailID == item.AcOPInvoiceDetailID select d).ToList();
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
                                customerinvoice.JobCode = customerinvoice.InvoiceType + customerinvoice.AcOPInvoiceDetailID;
                                customerinvoice.SInvoiceNo = sInvoiceDetail[0].InvoiceNo;
                                customerinvoice.strDate = Convert.ToDateTime(item.InvDate).ToString("dd/MM/yyyy");
                                customerinvoice.AmountToBeRecieved = Convert.ToDecimal(invoicetotal);// - Convert.ToDecimal(totamt) - Convert.ToDecimal(item.Amount);
                                customerinvoice.AmountToBePaid = Convert.ToDecimal(totamt); // Convert.ToDecimal(totamtpaid) - Convert.ToDecimal(totadjust);// ;// customerinvoice.AmountToBeRecieved;
                                customerinvoice.Amount = Convert.ToDecimal(item.Amount) * -1;
                                customerinvoice.Balance = customerinvoice.AmountToBeRecieved - customerinvoice.AmountToBePaid; // (Convert.ToDecimal(invoicetotal) - Convert.ToDecimal(totamt)) - Convert.ToDecimal(item.Amount); //  Convert.ToDecimal(sInvoiceDetail.NetValue - totamt);
                                customerinvoice.RecPayDetailID = item.RecPayDetailID;

                                customerinvoice.RecPayID = Convert.ToInt32(item.RecPayID);
                                customerinvoice.AdjustmentAmount = Convert.ToDecimal(item.AdjustmentAmount);
                                cust.CustomerRcieptChildVM.Add(customerinvoice);
                            }
                        }
                        else if (item.InvoiceID > 0 && item.AcOPInvoiceDetailID == 0)
                        {
                            cust.AWBAllocation = ReceiptDAO.GetAWBAllocation(cust.AWBAllocation, Convert.ToInt32(item.InvoiceID), Convert.ToDecimal(item.Amount), cust.RecPayID); //customer invoiceid,amount
                            var sInvoiceDetail = (from d in Context1.CustomerInvoiceDetails where d.CustomerInvoiceID == item.InvoiceID select d).ToList();
                            var awbDetail = (from d in Context1.RecPayAllocationDetails where d.CustomerInvoiceID == item.InvoiceID && d.RecPayDetailID == item.RecPayDetailID select d).ToList();
                            if (sInvoiceDetail != null)
                            {
                                var invoicetotal = sInvoiceDetail.Sum(d => d.NetValue); //  sInvoiceDetail.Sum(d=>d.OtherCharge);
                                var awbtotal = awbDetail.Sum(d => d.AllocatedAmount);
                                var Sinvoice = (from d in Context1.CustomerInvoices where d.CustomerInvoiceID == item.InvoiceID select d).FirstOrDefault();
                                var allrecpay = (from d in Context1.RecPayDetails join c in Context1.RecPays on d.RecPayID equals c.RecPayID where c.RecPayDate < cust.RecPayDate && d.InvoiceID == item.InvoiceID select d).ToList();
                                var totamtpaid = allrecpay.Sum(d => d.Amount) * -1;
                                var totadjust = allrecpay.Sum(d => d.AdjustmentAmount);
                                var CreditNote = (from d in Context1.CreditNotes where d.InvoiceID == item.InvoiceID && d.CustomerID == Sinvoice.CustomerID select d).ToList();
                                decimal? CreditAmount = 0;
                                if (CreditNote.Count > 0)
                                {
                                    CreditAmount = CreditNote.Sum(d => d.Amount);
                                }
                                var totamt = totamtpaid + totadjust + CreditAmount;
                                var customerinvoice = new CustomerRcieptChildVM();
                                customerinvoice.InvoiceID = Convert.ToInt32(item.InvoiceID);
                                customerinvoice.AcOPInvoiceDetailID = 0;
                                customerinvoice.InvoiceType = "TR";
                                customerinvoice.JobCode = customerinvoice.InvoiceType + customerinvoice.InvoiceID;
                                customerinvoice.SInvoiceNo = Sinvoice.CustomerInvoiceNo;
                                customerinvoice.strDate = Convert.ToDateTime(item.InvDate).ToString("dd/MM/yyyy");
                                customerinvoice.AmountToBeRecieved = Convert.ToDecimal(invoicetotal);// - Convert.ToDecimal(totamt) - Convert.ToDecimal(item.Amount);
                                customerinvoice.Amount = Convert.ToDecimal(item.Amount) * -1;
                                customerinvoice.AmountToBePaid = Convert.ToDecimal(totamt); // - customerinvoice.AmountToBeRecieved;
                                customerinvoice.Balance = (Convert.ToDecimal(invoicetotal) - customerinvoice.AmountToBePaid); // - Convert.ToDecimal(totamt)) - Convert.ToDecimal(item.Amount); //  Convert.ToDecimal(sInvoiceDetail.NetValue - totamt);
                                customerinvoice.RecPayDetailID = item.RecPayDetailID;

                                customerinvoice.RecPayID = Convert.ToInt32(item.RecPayID);
                                customerinvoice.AdjustmentAmount = Convert.ToDecimal(item.AdjustmentAmount);
                                cust.CustomerRcieptChildVM.Add(customerinvoice);
                            }
                        }
                    }
                    Session["AWBAllocation"] = cust.AWBAllocation;
                    BindMasters_ForEdit(cust);
                }
                else
                {
                    ViewBag.Title = "Customer Receipt - Create";
                    BindAllMasters(2);

                    var acheadforcash = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;

                    DateTime pFromDate = AccountsDAO.CheckParamDate(DateTime.Now, FyearId).Date;
                    cust.RecPayDate = pFromDate;
                    cust.RecPayID = 0;
                    cust.CurrencyId = Convert.ToInt32(Session["CurrencyId"].ToString());
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
            var customerdetails = (from d in Context1.CustomerMasters where d.CustomerID == cust.CustomerID && d.CustomerType == "CS" select d).FirstOrDefault();
            if (customerdetails == null)
            {
                customerdetails = new CustomerMaster();
            }
            ViewBag.CustomerDetail = customerdetails;
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
            cust.AWBAllocation = new List<ReceiptAllocationDetailVM>();
            ViewBag.CustomerNotification = customernotification;
            return View(cust);

        }
        [HttpPost]
        public ActionResult CustomerTradeReceipt(CustomerRcieptVM RecP, string Command, string Currency)
        {
            int RPID = 0;
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            int i = 0;
            RecP.FYearID = Convert.ToInt32(Session["fyearid"]);
            RecP.UserID = Convert.ToInt32(Session["UserID"]);
            var StaffNotes = (from d in Context1.StaffNotes where d.RecPayID == RecP.RecPayID && d.PageTypeId == 2 orderby d.NotesId descending select d).ToList();
            var branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            var users = (from d in Context1.UserRegistrations select d).ToList();
            List<ReceiptAllocationDetailVM> AWBAllocationall = (List<ReceiptAllocationDetailVM>)Session["AWBAllocation"];
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
                //if (Fmoney > 0)
                //{
                //    RecP.FMoney = Fmoney;
                //}
                RecP.AcCompanyID = branchid;
                RPID = ReceiptDAO.AddCustomerRecieptPayment(RecP, Session["UserID"].ToString()); //.AddCustomerRecieptPayment(RecP, Session["UserID"].ToString());

                RecP.RecPayID = (from c in Context1.RecPays orderby c.RecPayID descending select c.RecPayID).FirstOrDefault();
                decimal TotalAmount = 0;

                foreach (var item in RecP.CustomerRcieptChildVM)
                {
                    decimal Advance = 0;
                    //if (item.Amount > 0 && (item.AmountToBeRecieved < item.Amount || item.AmountToBeRecieved == item.Amount))///900<1000
                    //{
                    // item.Amount = Convert.ToDecimal(RecP.EXRate * item.Amount);
                    //100=1000-900
                    Advance = Convert.ToDecimal(item.Amount) - item.AmountToBeRecieved;
                    DateTime vInvoiceDate = Convert.ToDateTime(item.InvoiceDate);
                    string vInvoiceDate1 = Convert.ToDateTime(vInvoiceDate).ToString("yyyy-MM-dd h:mm tt");
                    if (1 == 1) //item.Amount > 0 || item.AdjustmentAmount > 0)
                    {
                        var maxrecpaydetailid = (from c in Context1.RecPayDetails orderby c.RecPayDetailID descending select c.RecPayDetailID).FirstOrDefault();
                        string invoicetype = "C";
                        if (item.AcOPInvoiceDetailID != 0 && item.InvoiceID == 0)
                        {
                            invoicetype = "COP";
                            ReceiptDAO.InsertRecpayDetailsForCust(RecP.RecPayID, item.AcOPInvoiceDetailID, item.InvoiceID, Convert.ToDecimal(-item.Amount), "", invoicetype, false, "", vInvoiceDate1, item.InvoiceNo.ToString(), Convert.ToInt32(RecP.CurrencyId), 3, item.JobID);
                        }
                        else
                        {
                            ReceiptDAO.InsertRecpayDetailsForCust(RecP.RecPayID, item.InvoiceID, item.InvoiceID, Convert.ToDecimal(-item.Amount), "", invoicetype, false, "", vInvoiceDate1, item.InvoiceNo.ToString(), Convert.ToInt32(RecP.CurrencyId), 3, item.JobID);
                        }


                        var recpaydetail = (from d in Context1.RecPayDetails where d.RecPayDetailID == maxrecpaydetailid + 1 select d).FirstOrDefault();
                        var recpd = recpaydetail;
                        recpaydetail.AdjustmentAmount = item.AdjustmentAmount;
                        Context1.Entry(recpd).State = EntityState.Modified;
                        Context1.SaveChanges();
                        if (Advance > 0)
                        {
                            //   Advance Amount entry
                            ReceiptDAO.InsertRecpayDetailsForCust(RecP.RecPayID, 0, 0, Advance, null, "C", true, null, null, null, Convert.ToInt32(RecP.CurrencyId), 4, item.JobID);
                        }
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

                            var allocationdetail = AWBAllocationall.Where(cc => cc.CustomerInvoiceId == item.InvoiceID).ToList();
                            foreach (var aitem in allocationdetail)
                            {

                                if (aitem.RecPayDetailID == 0)
                                {
                                    aitem.RecPayDetailID = 0;
                                    RecPayAllocationDetail allocation = new RecPayAllocationDetail();
                                    allocation.CustomerInvoiceDetailID = aitem.CustomerInvoiceDetailID;
                                    allocation.CustomerInvoiceID = aitem.CustomerInvoiceId;
                                    allocation.RecPayID = RecP.RecPayID;
                                    allocation.InScanID = aitem.InScanID;
                                    allocation.RecPayDetailID = salesinvoicedetails.RecPayDetailId;
                                    allocation.AllocatedAmount = aitem.AllocatedAmount;
                                    Context1.RecPayAllocationDetails.Add(allocation);
                                    Context1.SaveChanges();
                                }
                                else
                                {
                                    RecPayAllocationDetail allocation = Context1.RecPayAllocationDetails.Where(cc => cc.ID == aitem.ID && cc.RecPayDetailID == salesinvoicedetails.RecPayDetailId).FirstOrDefault();
                                    allocation.CustomerInvoiceDetailID = aitem.CustomerInvoiceDetailID;
                                    allocation.CustomerInvoiceID = aitem.CustomerInvoiceId;
                                    allocation.RecPayID = RecP.RecPayID;
                                    allocation.InScanID = aitem.InScanID;
                                    allocation.RecPayDetailID = salesinvoicedetails.RecPayDetailId;
                                    allocation.AllocatedAmount = aitem.AllocatedAmount;
                                    Context1.Entry(allocation).State = EntityState.Modified;
                                    Context1.SaveChanges();
                                }
                            }

                        }


                    }
                    TotalAmount = TotalAmount + Convert.ToDecimal(item.Amount);
                }
                if (RecP.CustomerRcieptChildVM.Count == 0)
                {
                    //ReceiptDAO.AddCustomerRecieptPayment(rec)
                    //RP.InsertRecpayDetailsForCust(RecP.RecPayID, 0, 0, Convert.ToInt32(RecP.FMoney), null, "C", true, null, null, null, Convert.ToInt32(RecP.CurrencyId), 4, 0);
                    int fyaerId = Convert.ToInt32(Session["fyearid"].ToString());
                    //RP.InsertJournalOfCustomer(RecP.RecPayID, fyaerId);
                }
                //To Balance Invoice AMount
                if (TotalAmount > 0)
                {
                    int l = ReceiptDAO.InsertRecpayDetailsForCust(RecP.RecPayID, 0, 0, TotalAmount, null, "C", false, null, null, null, Convert.ToInt32(RecP.CurrencyId), 4, 0);
                    int fyaerId = Convert.ToInt32(Session["fyearid"].ToString());
                    ReceiptDAO.InsertJournalOfCustomer(RecP.RecPayID, fyaerId);

                }
                var Recpaydata = (from d in Context1.RecPays where d.RecPayID == RecP.RecPayID select d).FirstOrDefault();

                Recpaydata.RecPayID = RecP.RecPayID;
                Recpaydata.IsTradingReceipt = true;
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

                RecPay recpay = new RecPay();
                recpay.RecPayDate = RecP.RecPayDate;
                recpay.RecPayID = RecP.RecPayID;
                recpay.AcJournalID = RecP.AcJournalID;
                recpay.BankName = RecP.BankName;
                recpay.ChequeDate = RecP.ChequeDate;
                recpay.ChequeNo = RecP.ChequeNo;
                recpay.CustomerID = RecP.CustomerID;
                recpay.DocumentNo = RecP.DocumentNo;
                recpay.EXRate = RecP.EXRate;
                recpay.FYearID = RecP.FYearID;
                recpay.FMoney = RecP.FMoney;
                recpay.StatusEntry = RecP.StatusEntry;
                recpay.IsTradingReceipt = true;
                recpay.Remarks = RecP.Remarks;
                Context1.Entry(recpay).State = EntityState.Modified;
                Context1.SaveChanges();

                foreach (var item in RecP.CustomerRcieptChildVM)
                {
                    RecPayDetail recpd = Context1.RecPayDetails.Find(item.RecPayDetailID);
                    //recpd.RecPayDetailID = item.RecPayDetailID;
                    recpd.Amount = -(item.Amount);
                    //recpd.CurrencyID = item.CurrencyId;
                    recpd.AdjustmentAmount = item.AdjustmentAmount;
                    //recpd.InvDate = item.InvoiceDate.Value;
                    //recpd.RecPayID = RecP.RecPayID;
                    recpd.Remarks = item.Remarks;
                    recpd.InvoiceID = item.InvoiceID;
                    recpd.AcOPInvoiceDetailID = item.AcOPInvoiceDetailID;
                    recpd.StatusInvoice = "C";
                    Context1.Entry(recpd).State = EntityState.Modified;
                    Context1.SaveChanges();
                    //
                    if (recpd.InvoiceID > 0 && item.AcOPInvoiceDetailID == 0)
                    {

                        var allocationdetail = AWBAllocationall.Where(cc => cc.CustomerInvoiceId == item.InvoiceID).ToList();
                        foreach (var aitem in allocationdetail)
                        {

                            RecPayAllocationDetail allocation = Context1.RecPayAllocationDetails.Where(cc => cc.RecPayID == RecP.RecPayID && cc.CustomerInvoiceDetailID == aitem.CustomerInvoiceDetailID).FirstOrDefault();
                            if (allocation == null)
                            {
                                allocation = new RecPayAllocationDetail();
                                allocation.CustomerInvoiceDetailID = aitem.CustomerInvoiceDetailID;
                                allocation.CustomerInvoiceID = aitem.CustomerInvoiceId;
                                allocation.RecPayID = RecP.RecPayID;
                                allocation.InScanID = aitem.InScanID;
                                allocation.RecPayDetailID = aitem.RecPayDetailID;
                                allocation.AllocatedAmount = aitem.AllocatedAmount;
                                Context1.RecPayAllocationDetails.Add(allocation);
                                Context1.SaveChanges();
                            }
                            else
                            {
                                allocation.CustomerInvoiceDetailID = aitem.CustomerInvoiceDetailID;
                                allocation.CustomerInvoiceID = aitem.CustomerInvoiceId;
                                allocation.RecPayID = RecP.RecPayID;
                                allocation.InScanID = aitem.InScanID;
                                allocation.RecPayDetailID = recpd.RecPayDetailID;
                                allocation.AllocatedAmount = aitem.AllocatedAmount;

                                Context1.Entry(allocation).State = EntityState.Modified;
                                Context1.SaveChanges();
                            }
                        }


                        var salesinvoicedetails = (from d in Context1.CustomerInvoiceDetails where d.CustomerInvoiceID == item.InvoiceID select d).FirstOrDefault();
                        var totamount = (from d in Context1.RecPayDetails where d.InvoiceID == salesinvoicedetails.CustomerInvoiceDetailID select d).ToList();
                        var totsum = totamount.Sum(d => d.Amount) * -1;
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
                    /*  if (item.AmountToBeRecieved < item.Amount)
                      {
                          RecPayDetail recpd1 = new RecPayDetail();
                          recpd1.RecPayDetailID = (from c in Context1.RecPayDetails orderby c.RecPayDetailID descending select c.RecPayDetailID).FirstOrDefault();
                          recpd1.Amount = item.AmountToBeRecieved - item.Amount;
                          recpd1.RecPayID = RecP.RecPayID;
                          recpd1.Remarks = item.Remarks;
                          recpd1.CurrencyID = item.CurrencyId;
                          recpd1.StatusAdvance = true;
                          recpd.StatusInvoice = "C";
                          Context1.Entry(recpd1).State = EntityState.Modified;
                          Context1.SaveChanges();
                      }*/


                }
                int editrecPay = 0;
                var sumOfAmount = Context1.RecPayDetails.Where(m => m.RecPayID == RecP.RecPayID && m.InvoiceID != 0).Sum(c => c.Amount);
                if (sumOfAmount != null)
                {
                    editrecPay = editfu.EditRecpayDetailsCustR(RecP.RecPayID, Convert.ToInt32(sumOfAmount));
                    int editAcJdetails = editfu.EditAcJDetails(RecP.AcJournalID.Value, Convert.ToInt32(sumOfAmount));
                }
            }


            BindAllMasters(2);
            return RedirectToAction("CustomerTradeReceiptDetails", "CustomerReceipt", new { ID = RecP.RecPayID });
        }


        [HttpPost]
        public JsonResult GetTradeInvoiceOfCustomer(int? ID,decimal? amountreceived,int? RecPayId)
        {

           DateTime fromdate = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime todate = Convert.ToDateTime(Session["FyearTo"].ToString());
            var AllInvoices = (from d in Context1.CustomerInvoices where d.CustomerID == ID select d).ToList();
            List<ReceiptAllocationDetailVM> AWBAllocation = new List<ReceiptAllocationDetailVM>();
            var salesinvoice = new List<CustomerTradeReceiptVM>();
            foreach (var item in AllInvoices)
            {
                //var invoicedeails = (from d in Context1.SalesInvoiceDetails where d.SalesInvoiceID == item.SalesInvoiceID where (d.RecPayStatus < 2 || d.RecPayStatus == null) select d).ToList();
                var invoicedeails = (from d in Context1.CustomerInvoiceDetails where d.CustomerInvoiceID == item.CustomerInvoiceID where (d.RecPayStatus < 2 || d.RecPayStatus == null)  select d).ToList();
                //where (d.RecPayStatus < 2 || d.RecPayStatus == null) select d).ToList();
                decimal? totamt = 0;
                decimal? totamtpaid = 0;
                decimal? totadjust = 0;
                decimal? CreditAmount = 0;
                //foreach (var det in invoicedeails)
                //{
                    var allrecpay = (from d in Context1.RecPayDetails where d.InvoiceID == item.CustomerInvoiceID  select d).ToList();
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
                Invoice.JobCode = "";
                Invoice.SalesInvoiceID = item.CustomerInvoiceID; // SalesInvoiceID;
                Invoice.InvoiceNo = item.CustomerInvoiceNo;
                //Invoice.SalesInvoiceDetailID = det.CustomerInvoiceDetailID;
                Invoice.InvoiceAmount = item.InvoiceTotal; // CourierCharge;
                Invoice.date = item.InvoiceDate;
                Invoice.DateTime = item.InvoiceDate.ToString("dd/MM/yyyy");
                //var RecPay = (from d in Context1.RecPayDetails where d.RecPayDetailID == det.RecPayDetailId select d).FirstOrDefault();

                Invoice.AmountReceived = totamt;
                Invoice.Balance = Invoice.InvoiceAmount - totamtpaid;
                Invoice.AdjustmentAmount = totadjust;             
                
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
                    if (RecPayId == null)
                    {
                        AWBAllocation = ReceiptDAO.GetAWBAllocation(AWBAllocation, Convert.ToInt32(Invoice.SalesInvoiceID), Convert.ToDecimal(Invoice.Amount), 0); //customer invoiceid,amount
                    }
                    else
                    {
                        AWBAllocation = ReceiptDAO.GetAWBAllocation(AWBAllocation, Convert.ToInt32(Invoice.SalesInvoiceID), Convert.ToDecimal(Invoice.Amount), Convert.ToInt32(RecPayId)); //customer invoiceid,amount
                    }
                }
            }

            Session["AWBAllocation"] = AWBAllocation;
            return Json(salesinvoice, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult GetAWBAllocation(int InvoiceId)
        {
            List<ReceiptAllocationDetailVM> AWBAllocationall = new List<ReceiptAllocationDetailVM>();
            List<ReceiptAllocationDetailVM> AWBAllocation = new List<ReceiptAllocationDetailVM>();
            AWBAllocationall = (List<ReceiptAllocationDetailVM>)Session["AWBAllocation"];
            AWBAllocation = AWBAllocationall.Where(cc => cc.CustomerInvoiceId == InvoiceId).ToList();
            return Json(AWBAllocation,JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public JsonResult GetAWBReAllocation(int InvoiceId,decimal amount,int RecPayId)
        {
            List<ReceiptAllocationDetailVM> AWBAllocationall = new List<ReceiptAllocationDetailVM>();
            List<ReceiptAllocationDetailVM> AWBAllocation = new List<ReceiptAllocationDetailVM>();
            AWBAllocationall = (List<ReceiptAllocationDetailVM>)Session["AWBAllocation"];
            //AWBAllocation = AWBAllocationall.Where(cc => cc.CustomerInvoiceId == InvoiceId).ToList();
            AWBAllocationall.RemoveAll(cc => cc.CustomerInvoiceId == InvoiceId);
            if (RecPayId == 0)
            {
                AWBAllocationall = ReceiptDAO.GetAWBAllocation(AWBAllocationall, InvoiceId, Convert.ToDecimal(amount), 0); //customer invoiceid,amount
            }
            else
            {
                AWBAllocationall = ReceiptDAO.GetAWBAllocation(AWBAllocationall, Convert.ToInt32(InvoiceId), Convert.ToDecimal(amount), Convert.ToInt32(RecPayId)); //customer invoiceid,amount
            }
            AWBAllocation = AWBAllocationall.Where(cc => cc.CustomerInvoiceId == InvoiceId).ToList();
            return Json(AWBAllocation, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SaveAWBAllocation(List<ReceiptAllocationDetailVM> RecP)
        {
            var dd = RecP;
            List<ReceiptAllocationDetailVM> AWBAllocationall = new List<ReceiptAllocationDetailVM>();
            List<ReceiptAllocationDetailVM> AWBAllocation = new List<ReceiptAllocationDetailVM>();
            AWBAllocationall = (List<ReceiptAllocationDetailVM>)Session["AWBAllocation"];
            foreach(var item in AWBAllocationall)
            {
                foreach (var item2 in RecP)
                {
                    if (item.CustomerInvoiceDetailID==item2.CustomerInvoiceDetailID)
                    {
                        item.AllocatedAmount = item2.AllocatedAmount;                     
                        break;
                    }                   

                }
                AWBAllocation.Add(item);
            }
            Session["AWBAllocation"] = AWBAllocation;
            //AWBAllocation = AWBAllocationall.Where(cc => cc.CustomerInvoiceId == InvoiceId).ToList();
            //   AWBAllocation = updatelist;
            return Json(AWBAllocation, JsonRequestBehavior.AllowGet);

        }       

        [HttpGet]
        public JsonResult GetCustomerName(string term)
        {
            var customerlist = (from c1 in Context1.CustomerMasters
                                where c1.CustomerType != "CN" && c1.CustomerName.ToLower().Contains(term.ToLower())
                                orderby c1.CustomerName ascending
                                select new { CustomerID = c1.CustomerID, CustomerName = c1.CustomerName, CustomerType = c1.CustomerType }).ToList();

            return Json(customerlist, JsonRequestBehavior.AllowGet);

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
        public JsonResult UpdateStaffNote(int Jobid, string staffnote)
        {
            try
            {
                var note = new StaffNote();
                note.EntryDate = DateTime.Now;
                note.RecPayID= Jobid;
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

        public JsonResult SendCustomerNotification( int JobId,string Message, int Customerid, bool whatsapp, bool Email, bool sms)
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
                catch(Exception e)
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
