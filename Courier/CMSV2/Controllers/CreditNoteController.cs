using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using CMSV2.DAL;

namespace CMSV2.Controllers
{
    [SessionExpire]
    public class CreditNoteController : Controller
    {

        SourceMastersModel objSourceMastersModel = new SourceMastersModel();

        Entities1 db = new Entities1();

        public ActionResult Index()
        {
            var data = db.CreditNotes.Where(d => d.IsShipping == true).ToList();

            List<CreditNoteVM> lst = new List<CreditNoteVM>();
            foreach (var item in data)
            {
                //var job = (from c in db.JInvoices where c.InvoiceID == item.InvoiceID select c).FirstOrDefault();
                string jobcode = "";

                if (item.InvoiceType == "TR")
                {
                    var purchaseinvoice = (from d in db.CustomerInvoices where d.CustomerInvoiceID == item.InvoiceID select d).FirstOrDefault();
                    jobcode = purchaseinvoice.CustomerInvoiceNo;
                }
                else if (item.InvoiceType == "OP")
                {
                    var purchaseinvoice = (from d in db.AcOPInvoiceDetails where d.AcOPInvoiceDetailID == item.InvoiceID select d).FirstOrDefault();
                    jobcode = purchaseinvoice.InvoiceNo;

                }

                string customer = (from c in db.CustomerMasters where c.CustomerID == item.CustomerID && c.CustomerType == "CR" select c.CustomerName).FirstOrDefault();

                CreditNoteVM v = new CreditNoteVM();
                v.JobNO = jobcode;
                v.CreditNoteNo = item.CreditNoteNo;
                v.CreditNoteID = item.CreditNoteID;
                v.Date = item.CreditNoteDate.Value;
                v.CustomerName = customer;
                v.Amount = item.Amount.Value;
                lst.Add(v);                

            }

            return View(lst);

        }


        public ActionResult Create(int id = 0)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.customer = db.CustomerMasters.Where(d => d.CustomerType == "CR").OrderBy(x => x.CustomerName).ToList();
            ViewBag.achead = db.AcHeads.ToList();

            if (id == 0)
            {
                ViewBag.Title = "Credit Note - Create";
                CreditNoteVM vm = new CreditNoteVM();
                vm.CreditNoteNo = AccountsDAO.GetMaxCreditNoteNo(fyearid);
                vm.Date = CommanFunctions.GetLastDayofMonth().Date;
                return View(vm);
            }
            else
            {
                ViewBag.Title = "Credit Note - Modify";
                CreditNoteVM vm = new CreditNoteVM();
                var v = db.CreditNotes.Find(id);
                vm.CreditNoteID = v.CreditNoteID;
                vm.CreditNoteNo = v.CreditNoteNo;
                vm.Date = Convert.ToDateTime(v.CreditNoteDate);
                vm.AcJournalID = Convert.ToInt32(v.AcJournalID);
                vm.CustomerID = Convert.ToInt32(v.CustomerID);
                vm.AcHeadID = Convert.ToInt32(v.AcHeadID);
                vm.Amount = Convert.ToDecimal(v.Amount);
                vm.InvoiceID = Convert.ToInt32(v.InvoiceID);
                vm.InvoiceType = v.InvoiceType;
                vm.Description = v.Description;
                SetTradeInvoiceOfCustomer(vm.CustomerID, 0, vm.CreditNoteID);
                List<CustomerTradeReceiptVM> lst = (List<CustomerTradeReceiptVM>)Session["CustomerInvoice"];
                if (v.InvoiceType == "TR")
                {
                    var invoice = lst.Where(cc => cc.SalesInvoiceID == vm.InvoiceID && cc.InvoiceType == "TR").FirstOrDefault();
                    if (invoice != null)
                    {
                        vm.InvoiceNo = invoice.InvoiceNo;
                        vm.InvoiceDate = invoice.DateTime;
                        vm.InvoiceAmount = Convert.ToDecimal(invoice.InvoiceAmount);
                        vm.ReceivedAmount = Convert.ToDecimal(invoice.AmountReceived);
                    }
                }
                else if (v.InvoiceType == "OP")
                {
                    //var invoice1 = db.AcOPInvoiceDetails.Where(cc=>cc.AcOPInvoiceDetailID ==vm.InvoiceID).FirstOrDefault();
                    //if (invoice1 != null)
                    //{
                    //    vm.InvoiceNo = invoice1.InvoiceNo;
                    //    vm.InvoiceDate = Convert.ToDateTime(invoice1.InvoiceDate).ToString("dd/MM/yyyy");
                    //    vm.InvoiceAmount = Convert.ToDecimal(invoice1.Amount);
                    //}
                    var invoice = lst.Where(cc => cc.SalesInvoiceID == vm.InvoiceID && cc.InvoiceType == "OP").FirstOrDefault();
                    vm.InvoiceNo = invoice.InvoiceNo;
                    vm.InvoiceDate = invoice.DateTime;
                    vm.InvoiceAmount = Convert.ToDecimal(invoice.InvoiceAmount);
                    vm.ReceivedAmount = Convert.ToDecimal(invoice.AmountReceived);
                }



                vm.Date = Convert.ToDateTime(v.CreditNoteDate);
                //vm.
                return View(vm);

            }


        }

        [HttpPost]
        public ActionResult Create(CreditNoteVM v)
        {
            AcJournalMaster ajm = new AcJournalMaster();
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            if (v.AcJournalID > 0)
            {
                ajm = db.AcJournalMasters.Find(v.AcJournalID);
            }
            if (v.CreditNoteID == 0 && v.AcJournalID == 0)
            {
                int acjm = 0;
                acjm = (from c in db.AcJournalMasters orderby c.AcJournalID descending select c.AcJournalID).FirstOrDefault();

                ajm.AcJournalID = acjm + 1;
                ajm.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
                ajm.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                ajm.AcFinancialYearID = fyearid;
                ajm.PaymentType = 1;
                var customer = db.CustomerMasters.Find(v.CustomerID).CustomerName;
                ajm.Remarks = "Credit Note for " + customer + " invoice : " + v.InvoiceNo;
                ajm.StatusDelete = false;
                ajm.VoucherNo = AccountsDAO.GetMaxVoucherNo("CN", fyearid);
                ajm.TransDate = v.Date;

                ajm.TransType = 2;
                ajm.VoucherType = "CN";
                db.AcJournalMasters.Add(ajm);
                db.SaveChanges();
            }
            else
            {
                var customer = db.CustomerMasters.Find(v.CustomerID).CustomerName;
                ajm.Remarks = "Credit Note for " + customer + " invoice : " + v.InvoiceNo;
                db.Entry(ajm).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            AcJournalDetail b = new AcJournalDetail();
            b = db.AcJournalDetails.Where(cc => cc.AcJournalID == ajm.AcJournalID && cc.Amount > 0).FirstOrDefault();
            if (b == null)
            {
                b = new AcJournalDetail();
                b.AcJournalDetailID = 0;
            }
            if (b.AcJournalDetailID == 0)
            {
                int maxacj = 0;
                maxacj = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();

                b.AcJournalDetailID = maxacj + 1;
                b.AcJournalID = ajm.AcJournalID;
            }
            b.AcHeadID = v.AcHeadID;
            b.Amount = v.Amount;
            b.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            b.Remarks = "credit note";
            if (b.ID == 0)
            {
                db.AcJournalDetails.Add(b);
                db.SaveChanges();
            }
            else
            {
                db.Entry(b).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            AcJournalDetail a = new AcJournalDetail();
            a = db.AcJournalDetails.Where(cc => cc.AcJournalID == ajm.AcJournalID && cc.Amount < 0).FirstOrDefault();
            if (a == null)
            {
                a = new AcJournalDetail();
                a.AcJournalDetailID = 0;
            }
            if (a.AcJournalDetailID == 0)
            {
                int maxacj = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();
                a.AcJournalDetailID = maxacj + 1;
                a.AcJournalID = ajm.AcJournalID;
                var customercon = db.AcHeads.Where(cc => cc.AcHead1 == "Customer Control A/c").FirstOrDefault();
                a.AcHeadID = customercon.AcHeadID; ;
            }

            a.Amount = -1 * v.Amount;
            a.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            a.Remarks = "";

            if (a.ID == 0)
            {
                db.AcJournalDetails.Add(a);
                db.SaveChanges();
            }
            else
            {
                db.Entry(a).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            var invid = 0;
            int? recpayid = 0;
            //if (v.TradingInvoice == false)
            //{
            //    int jobid = (from j in db.JobGenerations where j.JobCode == v.JobNO select j.JobID).FirstOrDefault();

            //     invid = (from c in db.JInvoices where c.JobID == jobid select c.InvoiceID).FirstOrDefault();


            //    var recpay = (from c in db.RecPayDetails where c.InvoiceID == invid select c).FirstOrDefault();
            //    if (recpay != null)
            //    {
            //        recpayid = recpay.RecPayID;
            //    }
            //}
            //else
            //{
            //    invid =Convert.ToInt32(v.JobNO);

            //  var  recpay = (from c in db.RecPayDetails where c.InvoiceID == invid select c).FirstOrDefault();
            //    if(recpay != null)
            //    {
            //        recpayid = recpay.RecPayID;
            //    }
            //}
            // invid = Convert.ToInt32(v.JobNO);
            //var ids = (from x in db.SalesInvoiceDetails where x.SalesInvoiceID == invid select (int?)x.SalesInvoiceDetailID).ToList();

            // recpayid = (from c in db.RecPayDetails where ids.Contains(c.InvoiceID) select c.RecPayID).FirstOrDefault().Value;

            CreditNote d = new CreditNote();
            if (v.CreditNoteID == 0)
            {
                int maxid = 0;

                var data = (from c in db.CreditNotes orderby c.CreditNoteID descending select c).FirstOrDefault();

                if (data == null)
                {
                    maxid = 1;
                }
                else
                {
                    maxid = data.CreditNoteID + 1;
                }

                d.CreditNoteID = maxid;
                d.CreditNoteNo = AccountsDAO.GetMaxCreditNoteNo(fyearid);
                d.FYearID = Convert.ToInt32(Session["fyearid"].ToString());
                d.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
                d.statusclose = false;
            }
            else
            {
                d = db.CreditNotes.Find(v.CreditNoteID);
            }
            d.InvoiceID = v.InvoiceID;
            d.InvoiceType = v.InvoiceType;
            d.CreditNoteDate = v.Date;
            d.Amount = v.Amount;
            d.AcJournalID = ajm.AcJournalID;
            d.RecPayID = recpayid;
            d.AcHeadID = v.AcHeadID;
            d.CustomerID = v.CustomerID;
            d.Description = v.Description;
            d.InvoiceType = v.InvoiceType;
            d.IsShipping = true;
            if (v.CreditNoteID == 0)
            {
                db.CreditNotes.Add(d);
                db.SaveChanges();
                TempData["SuccessMsg"] = "Successfully Added Credit Note";
            }
            else
            {
                db.Entry(d).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "Successfully Updated Credit Note";
            }

            return RedirectToAction("Index", "CreditNote");





        }

        //public JsonResult GetJobNo(int id)
        //{
        //    List<jobno> lst = new List<jobno>();
        //    var jobs = (from c in db.JobGenerations where c.ShipperID == id select c).ToList();
        //    if (jobs != null)
        //    {
        //        foreach (var item in jobs)
        //        {
        //            jobno obj = new jobno();
        //            obj.JobNo = item.JobCode;
        //            lst.Add(obj);
        //        }
        //    }

        //    return Json(lst,JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetInvoiceNo(string term)
        {
            var salesinvoice = new List<CustomerTradeReceiptVM>();
            var lst = new List<CustomerTradeReceiptVM>();
            salesinvoice = (List<CustomerTradeReceiptVM>)Session["CustomerInvoice"];
            if (term.Trim() != "")
            {
                lst = salesinvoice.Where(cc => cc.InvoiceNo.Contains(term)).OrderBy(cc => cc.InvoiceNo).ToList();
            }
            else
            {
                lst = salesinvoice.OrderBy(cc => cc.InvoiceNo).ToList();
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public class jobno
        {

            public string JobNo { get; set; }
            public int JobNum { get; set; }
            public bool Istrading { get; set; }

        }
        [HttpPost]
        public JsonResult GetTradeInvoiceOfCustomer(int? ID, decimal? amountreceived, int? CreditNoteId)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            DateTime fromdate = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime todate = Convert.ToDateTime(Session["FyearTo"].ToString());
            var AllInvoices = (from d in db.CustomerInvoices where d.CustomerID == ID select d).OrderBy(cc => cc.InvoiceDate).ToList();
            List<ReceiptAllocationDetailVM> AWBAllocation = new List<ReceiptAllocationDetailVM>();
            var salesinvoice = new List<CustomerTradeReceiptVM>();
            var AllOPInvoices = (from d in db.AcOPInvoiceDetails join m in db.AcOPInvoiceMasters on d.AcOPInvoiceMasterID equals m.AcOPInvoiceMasterID where d.Amount > 0 && m.AcFinancialYearID == fyearid && m.StatusSDSC == "C" && m.PartyID == ID select d).OrderBy(cc => cc.InvoiceDate).ToList();
            //decimal Advance = 0;
            //Advance = ReceiptDAO.SP_GetCustomerAdvance(Convert.ToInt32(ID), Convert.ToInt32(RecPayId), fyearid);
            //if (amountreceived > 0)
            //    amountreceived = amountreceived + Advance;
            foreach (var item in AllOPInvoices)
            {
                decimal? totamt = 0;
                decimal? totamtpaid = 0;
                decimal? totadjust = 0;
                totamtpaid = ReceiptDAO.SP_GetCustomerInvoiceReceived(Convert.ToInt32(ID), item.AcOPInvoiceDetailID, 0, Convert.ToInt32(CreditNoteId), "OP");
                var Invoice = new CustomerTradeReceiptVM();
                Invoice.AcOPInvoiceDetailID = item.AcOPInvoiceDetailID;
                Invoice.InvoiceType = "OP";
                Invoice.InvoiceNo = item.InvoiceNo;
                Invoice.InvoiceAmount = item.Amount;
                Invoice.date = item.InvoiceDate;
                Invoice.DateTime = Convert.ToDateTime(item.InvoiceDate).ToString("dd/MM/yyyy");
                Invoice.AmountReceived = totamtpaid;
                Invoice.Balance = Invoice.InvoiceAmount - totamtpaid;
                Invoice.AdjustmentAmount = totadjust;

                if (Invoice.Balance > 0)
                {
                    if (amountreceived != null)
                    {
                        if (amountreceived >= Invoice.Balance)
                        {
                            Invoice.Allocated = true;
                            Invoice.Amount = Invoice.Balance;
                            amountreceived = amountreceived - Invoice.Amount;
                        }
                        else if (amountreceived > 0)
                        {
                            Invoice.Allocated = true;
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
            foreach (var item in AllInvoices)
            {
                //var invoicedeails = (from d in db.SalesInvoiceDetails where d.SalesInvoiceID == item.SalesInvoiceID where (d.RecPayStatus < 2 || d.RecPayStatus == null) select d).ToList();
                //var invoicedeails = (from d in db.CustomerInvoiceDetails where d.CustomerInvoiceID == item.CustomerInvoiceID where (d.RecPayStatus < 2 || d.RecPayStatus == null)  select d).ToList();
                var invoicedeails = (from d in db.CustomerInvoiceDetails where d.CustomerInvoiceID == item.CustomerInvoiceID select d).ToList();
                //where (d.RecPayStatus < 2 || d.RecPayStatus == null) select d).ToList();
                decimal? totamt = 0;
                decimal? totamtpaid = 0;
                decimal? totadjust = 0;
                decimal? CreditAmount = 0;

                totamtpaid = ReceiptDAO.SP_GetCustomerInvoiceReceived(Convert.ToInt32(ID), item.CustomerInvoiceID, 0, Convert.ToInt32(CreditNoteId), "TR");

                totamt = totamtpaid + totadjust + CreditAmount;


                var Invoice = new CustomerTradeReceiptVM();
                //Invoice.JobID = det.JobID;
                Invoice.InvoiceType = "TR";
                Invoice.JobCode = "";
                Invoice.SalesInvoiceID = item.CustomerInvoiceID; // SalesInvoiceID;
                Invoice.InvoiceNo = item.CustomerInvoiceNo;
                //Invoice.SalesInvoiceDetailID = det.CustomerInvoiceDetailID;
                Invoice.InvoiceAmount = item.InvoiceTotal; // CourierCharge;
                Invoice.date = item.InvoiceDate;
                Invoice.DateTime = item.InvoiceDate.ToString("dd/MM/yyyy");
                //var RecPay = (from d in db.RecPayDetails where d.RecPayDetailID == det.RecPayDetailId select d).FirstOrDefault();

                Invoice.AmountReceived = totamt;
                Invoice.Balance = Invoice.InvoiceAmount - totamtpaid;
                Invoice.AdjustmentAmount = totadjust;

                if (Invoice.Balance > 0)
                {
                    if (amountreceived != null)
                    {
                        if (amountreceived >= Invoice.Balance)
                        {
                            Invoice.Allocated = true;
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
                    //if (RecPayId == null)
                    //{
                    //    AWBAllocation = ReceiptDAO.GetAWBAllocation(AWBAllocation, Convert.ToInt32(Invoice.SalesInvoiceID), Convert.ToDecimal(Invoice.Amount), 0); //customer invoiceid,amount
                    //}
                    //else
                    //{
                    //    AWBAllocation = ReceiptDAO.GetAWBAllocation(AWBAllocation, Convert.ToInt32(Invoice.SalesInvoiceID), Convert.ToDecimal(Invoice.Amount), Convert.ToInt32(RecPayId)); //customer invoiceid,amount
                    //}
                }
            }


            Session["CustomerInvoice"] = salesinvoice;
            return Json(salesinvoice, JsonRequestBehavior.AllowGet);
        }


        public void SetTradeInvoiceOfCustomer(int? ID, decimal? amountreceived, int? CreditNoteId)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"].ToString());
            DateTime fromdate = Convert.ToDateTime(Session["FyearFrom"].ToString());
            DateTime todate = Convert.ToDateTime(Session["FyearTo"].ToString());
            var AllInvoices = (from d in db.CustomerInvoices where d.CustomerID == ID select d).ToList();
            List<ReceiptAllocationDetailVM> AWBAllocation = new List<ReceiptAllocationDetailVM>();
            var salesinvoice = new List<CustomerTradeReceiptVM>();
            var AllOPInvoices = (from d in db.AcOPInvoiceDetails join m in db.AcOPInvoiceMasters on d.AcOPInvoiceMasterID equals m.AcOPInvoiceMasterID where m.AcFinancialYearID == fyearid && m.StatusSDSC == "C" && m.PartyID == ID && d.RecPayDetailId == null && (d.RecPayStatus == null || d.RecPayStatus < 2) select d).ToList();

            foreach (var item in AllOPInvoices)
            {

                decimal? totamtpaid = 0;
                decimal? totadjust = 0;

                totamtpaid = ReceiptDAO.SP_GetCustomerInvoiceReceived(Convert.ToInt32(ID), Convert.ToInt32(item.AcOPInvoiceDetailID), 0, Convert.ToInt32(CreditNoteId), "OP");

                var Invoice = new CustomerTradeReceiptVM();
                Invoice.AcOPInvoiceDetailID = item.AcOPInvoiceDetailID;
                Invoice.SalesInvoiceID = item.AcOPInvoiceDetailID;
                Invoice.InvoiceType = "OP";
                Invoice.InvoiceNo = item.InvoiceNo; ;
                Invoice.InvoiceAmount = item.Amount;
                Invoice.date = item.InvoiceDate;
                Invoice.DateTime = Convert.ToDateTime(item.InvoiceDate).ToString("dd/MM/yyyy");
                Invoice.AmountReceived = totamtpaid;
                Invoice.Balance = Invoice.InvoiceAmount - totamtpaid;
                Invoice.AdjustmentAmount = totadjust;

                if (Invoice.Balance > 0)
                {
                    if (amountreceived != null)
                    {
                        if (amountreceived >= Invoice.Balance)
                        {
                            Invoice.Allocated = true;
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
            foreach (var item in AllInvoices)
            {
                //var invoicedeails = (from d in db.SalesInvoiceDetails where d.SalesInvoiceID == item.SalesInvoiceID where (d.RecPayStatus < 2 || d.RecPayStatus == null) select d).ToList();
                //var invoicedeails = (from d in db.CustomerInvoiceDetails where d.CustomerInvoiceID == item.CustomerInvoiceID where (d.RecPayStatus < 2 || d.RecPayStatus == null) select d).ToList();                                
                decimal? totamtpaid = 0;
                decimal? totadjust = 0;

                totamtpaid = ReceiptDAO.SP_GetCustomerInvoiceReceived(Convert.ToInt32(ID), Convert.ToInt32(item.CustomerInvoiceID), 0, Convert.ToInt32(CreditNoteId), "TR");

                var Invoice = new CustomerTradeReceiptVM();
                Invoice.InvoiceType = "TR";
                Invoice.JobCode = "";
                Invoice.SalesInvoiceID = item.CustomerInvoiceID; // SalesInvoiceID;
                Invoice.InvoiceNo = item.CustomerInvoiceNo;
                Invoice.InvoiceAmount = item.InvoiceTotal; // CourierCharge;
                Invoice.date = item.InvoiceDate;
                Invoice.DateTime = item.InvoiceDate.ToString("dd/MM/yyyy");
                Invoice.AmountReceived = totamtpaid;
                Invoice.Balance = Invoice.InvoiceAmount - totamtpaid;
                Invoice.AdjustmentAmount = totadjust;

                if (Invoice.Balance > 0)
                {
                    if (amountreceived != null)
                    {
                        if (amountreceived >= Invoice.Balance)
                        {
                            Invoice.Allocated = true;
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
                    //if (RecPayId == null)
                    //{
                    //    AWBAllocation = ReceiptDAO.GetAWBAllocation(AWBAllocation, Convert.ToInt32(Invoice.SalesInvoiceID), Convert.ToDecimal(Invoice.Amount), 0); //customer invoiceid,amount
                    //}
                    //else
                    //{
                    //    AWBAllocation = ReceiptDAO.GetAWBAllocation(AWBAllocation, Convert.ToInt32(Invoice.SalesInvoiceID), Convert.ToDecimal(Invoice.Amount), Convert.ToInt32(RecPayId)); //customer invoiceid,amount
                    //}
                }
            }

            Session["CustomerInvoice"] = salesinvoice;

        }

        public ActionResult DeleteCreditNote(int id)
        {
            //int k = 0;
            if (id != 0)
            {
                string result = AccountsDAO.DeleteCreditNote(id);
                TempData["SuccessMsg"] = result;

            }

            return RedirectToAction("Index", "CreditNote");

        }
        public JsonResult GetAmountByinvono(int invno, bool IsTrading)
        {
            Getamtclass ob = new Getamtclass();
            ob.invoiceamt = 0;
            ob.recamt = 0;
            if (IsTrading == false)
            {
                //int jobid = (from j in db.JobGenerations where j.JobCode == invno select j.JobID).FirstOrDefault();

                //int invid = (from c in db.JInvoices where c.JobID == jobid select c.InvoiceID).FirstOrDefault();

                //decimal invamt = (from c in db.JInvoices where c.InvoiceID == invid select c.SalesHome).FirstOrDefault().Value;

                //decimal recamt = (from r in db.RecPayDetails where r.InvoiceID == invid select r.Amount).FirstOrDefault().Value;


                //ob.invoiceamt = invamt;
                //ob.recamt = recamt;
            }
            else
            {
                var sinvoice = (from d in db.CustomerInvoices where d.CustomerInvoiceID == invno select d).FirstOrDefault();
                var sinDetails = (from c in db.CustomerInvoiceDetails where c.CustomerInvoiceID == sinvoice.CustomerInvoiceID select c).ToList();
                List<int?> pinvoiceids = sinDetails.Select(d => (int?)d.CustomerInvoiceDetailID).ToList();
                ob.invoiceamt = Convert.ToDecimal(sinDetails.Sum(d => d.NetValue));
                var recpay = (from x in db.RecPayDetails where pinvoiceids.Contains(x.InvoiceID) select x).ToList();

                if (recpay.Count > 0)
                {
                    ob.recamt = Convert.ToDecimal(Math.Abs(recpay.Sum(s => s.Amount.Value)));
                }

            }


            return Json(ob, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ServiceIndex()
        {
            var data = db.CreditNotes.Where(d => d.IsShipping == false).ToList();

            List<CreditNoteVM> lst = new List<CreditNoteVM>();
            foreach (var item in data)
            {
                //var job = (from c in db.JInvoices where c.InvoiceID == item.InvoiceID select c).FirstOrDefault();
                string jobcode = "";
                //if (job != null)
                //{
                //    var jobid = job.JobID;
                //     jobcode = (from j in db.JobGenerations where j.JobID == jobid select j.JobCode).FirstOrDefault();

                //}
                //else
                //{
                var purchaseinvoice = (from d in db.CustomerInvoices where d.CustomerInvoiceID == item.InvoiceID select d).FirstOrDefault();
                jobcode = purchaseinvoice.CustomerInvoiceNo;

                //jobcode = item.InvoiceID.ToString();
                //}
                string customer = (from c in db.CustomerMasters where c.CustomerID == item.CustomerID && c.CustomerType == "CR" select c.CustomerName).FirstOrDefault();

                CreditNoteVM v = new CreditNoteVM();
                v.JobNO = jobcode;
                v.Date = item.CreditNoteDate.Value;
                v.CustomerName = customer;
                v.Amount = item.Amount.Value;
                lst.Add(v);

            }

            return View(lst);

        }

        public JsonResult CreditNoteVoucher(int id)
        {
            string reportpath = "";
            if (id != 0)
            {
                reportpath = AccountsReportsDAO.GenerateCreditNoteVoucherPrint(id);

            }

            return Json(new { path = reportpath, result = "ok" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ServiceCreate()
        {
            ViewBag.customer = db.CustomerMasters.Where(d => d.CustomerType == "CR").OrderBy(x => x.CustomerName).ToList();
            ViewBag.achead = db.AcHeads.ToList();
            List<jobno> lst = new List<jobno>();
            ViewBag.jobno = lst;

            return View();


        }


        public class Getamtclass
        {
            public decimal? invoiceamt { get; set; }
            public decimal? recamt { get; set; }

        }

    }
}
