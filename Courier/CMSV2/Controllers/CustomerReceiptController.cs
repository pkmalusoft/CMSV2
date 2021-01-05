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


        [HttpGet]
        public ActionResult CustomerRecieptDetails(int ID)
        {
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            List<ReceiptVM> Reciepts = new List<ReceiptVM>();

            Reciepts = ReceiptDAO.GetCustomerReceipts(FyearId); // RP.GetAllReciepts();
            //var data = (from t in Reciepts where (t.RecPayDate >= Convert.ToDateTime(Session["FyearFrom"]) && t.RecPayDate <= Convert.ToDateTime(Session["FyearTo"])) select t).ToList();

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

        //[HttpGet]
        //public ActionResult DeleteCustomerDet(int id)
        //{
        //    //int k = 0;
        //    if (id != 0)
        //    {
        //        RP.DeleteCustomerDetails(id);
        //    }

        //    return RedirectToAction("CustomerRecieptDetails", "CustomerReciept", new { ID = 10 });

        //}
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

        public JsonResult GetAllCustomer()
        {
            DateTime d = DateTime.Now;
            DateTime fyear = Convert.ToDateTime(Session["FyearFrom"].ToString());
            int FyearId = Convert.ToInt32(Session["fyearid"]);
            DateTime mstart = new DateTime(fyear.Year, d.Month, 01);

            int maxday = DateTime.DaysInMonth(fyear.Year, d.Month);
            DateTime mend = new DateTime(fyear.Year, d.Month, maxday);

            var cust = ReceiptDAO.GetCustomerReceipts(FyearId).Where(x => x.RecPayDate >= mstart && x.RecPayDate <= mend).OrderByDescending(x => x.RecPayDate).ToList();
            //Context1.SP_GetAllRecieptsDetails().Where(x => x.RecPayDate >= mstart && x.RecPayDate <= mend).OrderByDescending(x => x.RecPayDate).ToList();

            string view = this.RenderPartialView("_GetAllCustomer", cust);

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

            var sdate = DateTime.Parse(fdate);
            var edate = DateTime.Parse(tdate);


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
        public ActionResult CustomerTradeReceiptDetails(int ID)
        {
            int FyearId=Convert.ToInt32(Session["fyearid"]);
            List<ReceiptVM> Reciepts = new List<ReceiptVM>();

            Reciepts = ReceiptDAO.GetCustomerReceipts(FyearId); // RP.GetAllReciepts();
            var data = (from t in Reciepts where (t.RecPayDate >= Convert.ToDateTime(Session["FyearFrom"]) && t.RecPayDate <= Convert.ToDateTime(Session["FyearTo"])) select t).ToList();
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
        [HttpGet]
        public ActionResult CustomerTradeReceipt(int id)
        {
            CustomerRcieptVM cust = new CustomerRcieptVM();
            cust.CustomerRcieptChildVM = new List<CustomerRcieptChildVM>();
            if (Session["UserID"] != null)
            {
                var branchid = Convert.ToInt32(Session["CurrentBranchID"]);

                if (id > 0)
                {
                    cust = RP.GetRecPayByRecpayID(id);

                    var acheadforcash = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
                    //ViewBag.achead = Context1.AcHeadSelectForCash(Convert.ToInt32(Session["AcCompanyID"].ToString())).ToList();
                    //ViewBag.acheadbank = Context1.AcHeadSelectForBank(Convert.ToInt32(Session["AcCompanyID"].ToString())).ToList();
                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
                    cust.recPayDetail = Context1.RecPayDetails.Where(item => item.RecPayID == id).ToList();
                    cust.CustomerRcieptChildVM = new List<CustomerRcieptChildVM>();
                    foreach (var item in cust.recPayDetail)
                    {
                        if (item.InvoiceID > 0)
                        {
                            var sInvoiceDetail = (from d in Context1.CustomerInvoiceDetails where d.CustomerInvoiceDetailID == item.InvoiceID select d).FirstOrDefault();
                            if (sInvoiceDetail != null)
                            {
                                var Sinvoice = (from d in Context1.CustomerInvoices where d.CustomerInvoiceID == sInvoiceDetail.CustomerInvoiceID select d).FirstOrDefault();
                                var allrecpay = (from d in Context1.RecPayDetails where d.InvoiceID == item.InvoiceID select d).ToList();
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
                                customerinvoice.SInvoiceNo = Sinvoice.CustomerInvoiceNo;
                                customerinvoice.strDate = Convert.ToDateTime(item.InvDate).ToString("dd/MM/yyyy");
                                customerinvoice.AmountToBePaid = Convert.ToDecimal(totamtpaid);
                                customerinvoice.Amount = Convert.ToDecimal(item.Amount) * -1;
                                customerinvoice.Balance = Convert.ToDecimal(sInvoiceDetail.NetValue - totamt);
                                customerinvoice.RecPayDetailID = item.RecPayDetailID;
                                customerinvoice.AmountToBeRecieved = Convert.ToDecimal(sInvoiceDetail.NetValue);
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
                    BindAllMasters(2);

                    var acheadforcash = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                    var acheadforbank = (from c in Context1.AcHeads join g in Context1.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

                    ViewBag.achead = acheadforcash;
                    ViewBag.acheadbank = acheadforbank;
                    

                    cust.RecPayDate = System.DateTime.UtcNow;
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
            ViewBag.CustomerNotification = customernotification;
            return View(cust);

        }

        [HttpPost]
        public JsonResult GetTradeInvoiceOfCustomer(int? ID,decimal? amountreceived)
        {

            DateTime fromdate = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime todate = Convert.ToDateTime(Session["FyearTo"].ToString());
            var AllInvoices = (from d in Context1.CustomerInvoices where d.CustomerID == ID select d).ToList();
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
                }
            }

            return Json(salesinvoice, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CustomerTradeReceipt(CustomerRcieptVM RecP, string Command, string Currency)
        {
            int RPID = 0;
            int i = 0;
            RecP.FYearID = Convert.ToInt32(Session["fyearid"]);
            RecP.UserID = Convert.ToInt32(Session["UserID"]);
            var StaffNotes = (from d in Context1.StaffNotes where d.RecPayID== RecP.RecPayID && d.PageTypeId == 2 orderby d.NotesId descending select d).ToList();
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
                    RecP.FMoney = Fmoney;
                }
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
                    Advance = Convert.ToDecimal(item.Amount )- item.AmountToBeRecieved;
                    DateTime vInvoiceDate = Convert.ToDateTime(item.InvoiceDate);
                    string vInvoiceDate1 = Convert.ToDateTime(vInvoiceDate).ToString("yyyy-MM-dd h:mm tt");
                    if (item.Amount > 0 || item.AdjustmentAmount > 0)
                    {
                        var maxrecpaydetailid = (from c in Context1.RecPayDetails orderby c.RecPayDetailID descending select c.RecPayDetailID).FirstOrDefault();

                        ReceiptDAO.InsertRecpayDetailsForCust(RecP.RecPayID, item.InvoiceID , item.InvoiceID, Convert.ToDecimal(-item.Amount), "", "C", false, "", vInvoiceDate1, item.InvoiceNo.ToString(), Convert.ToInt32(RecP.CurrencyId), 3, item.JobID);

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
                recpay.FMoney = Fmoney;
                recpay.StatusEntry = RecP.StatusEntry;
                recpay.IsTradingReceipt = true;
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
                    Context1.Entry(recpd).State = EntityState.Modified;
                    Context1.SaveChanges();
                    var salesinvoicedetails = (from d in Context1.CustomerInvoiceDetails where d.CustomerInvoiceDetailID == item.InvoiceID select d).FirstOrDefault();
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
                editrecPay = editfu.EditRecpayDetailsCustR(RecP.RecPayID, Convert.ToInt32(sumOfAmount));
                int editAcJdetails = editfu.EditAcJDetails(RecP.AcJournalID.Value, Convert.ToInt32(sumOfAmount));
            }


            BindAllMasters(2);
            return RedirectToAction("CustomerTradeReceiptDetails", "CustomerReceipt", new { ID = RecP.RecPayID });
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
