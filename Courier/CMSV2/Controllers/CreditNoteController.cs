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
            var data = db.CreditNotes.Where(d=>d.IsShipping==true).ToList();

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
                var purchaseinvoice = (from d in db.CustomerInvoices  where d.CustomerInvoiceID == item.InvoiceID  select d).FirstOrDefault();
                jobcode = purchaseinvoice.CustomerInvoiceNo;

                //jobcode = item.InvoiceID.ToString();
                //}
                string customer = (from c in db.CustomerMasters where c.CustomerID == item.CustomerID && c.CustomerType=="CR" select c.CustomerName).FirstOrDefault();

                CreditNoteVM v = new CreditNoteVM();
                v.JobNO = jobcode;
                v.Date = item.CreditNoteDate.Value;
                v.CustomerName = customer;
                v.Amount = item.Amount.Value;
                lst.Add(v);

            }

            return View(lst);

        }


        public ActionResult Create()
        {
            ViewBag.customer = db.CustomerMasters.Where(d=>d.CustomerType=="CR").OrderBy(x => x.CustomerName).ToList();
            ViewBag.achead = db.AcHeads.ToList();
            List<jobno> lst = new List<jobno>();
            ViewBag.jobno = lst;
            
            return View();


        }

        [HttpPost]
        public ActionResult Create(CreditNoteVM v)
        {
            AcJournalMaster ajm = new AcJournalMaster();

            int acjm = 0;
            acjm = (from c in db.AcJournalMasters orderby c.AcJournalID descending select c.AcJournalID).FirstOrDefault();

            ajm.AcJournalID = acjm + 1;
            ajm.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            ajm.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
            ajm.PaymentType = 1;
            ajm.Remarks = "Credit Note";
            ajm.StatusDelete = false;
            ajm.TransDate = v.Date;
            ajm.VoucherNo = "C-" + ajm.AcJournalID;
            ajm.TransType = 2;
            ajm.VoucherType = "";

            db.AcJournalMasters.Add(ajm);
            db.SaveChanges();


            AcJournalDetail b = new AcJournalDetail();

            int maxacj = 0;
            maxacj = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();

            b.AcJournalDetailID = maxacj + 1;
            b.AcJournalID = ajm.AcJournalID;
            b.AcHeadID = v.AcHeadID;
            b.Amount = -v.Amount;
            b.BranchID = Convert.ToInt32(Session["AcCompanyID"].ToString());
            b.Remarks = "";

            db.AcJournalDetails.Add(b);
            db.SaveChanges();


            AcJournalDetail a = new AcJournalDetail();
            maxacj = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();
            a.AcJournalDetailID = maxacj + 1;
            a.AcJournalID = ajm.AcJournalID;
            a.AcHeadID = v.AcHeadID;
            a.Amount = v.Amount;
            a.BranchID = Convert.ToInt32(Session["AcCompanyID"].ToString());
            a.Remarks = "";
            


           



            db.AcJournalDetails.Add(a);
            db.SaveChanges();

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
             invid = Convert.ToInt32(v.JobNO);
            var ids = (from x in db.SalesInvoiceDetails where x.SalesInvoiceID == invid select (int?)x.SalesInvoiceDetailID).ToList();

             recpayid = (from c in db.RecPayDetails where ids.Contains(c.InvoiceID) select c.RecPayID).FirstOrDefault().Value;

            CreditNote d = new CreditNote();

            //int max = (from c in db.CreditNotes orderby c.CreditNoteNo descending select c.CreditNoteNo).FirstOrDefault().Value;
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
            d.CreditNoteNo = maxid;
            d.InvoiceID = invid;
            d.CreditNoteDate = v.Date;
            d.Amount = v.Amount;
            d.AcJournalID = ajm.AcJournalID;
            d.FYearID = Convert.ToInt32(Session["fyearid"].ToString());
            d.AcCompanyID = Convert.ToInt32(Session["AcCompanyID"].ToString());
            d.RecPayID = recpayid;
            d.AcHeadID = v.AcHeadID;
            d.CustomerID = v.CustomerID;
            d.statusclose = false;
            d.InvoiceType = "C";
            d.IsShipping = true;

            db.CreditNotes.Add(d);
            db.SaveChanges();

            TempData["SuccessMsg"] = "Successfully Added Credit Note";
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
        //public JsonResult GetJobNoAutocomp(int id,string term)
        //{
        //    List<jobno> lst = new List<jobno>();
        //    //var jobs = (from c in db.JobGenerations where c.ShipperID == id  && c.JobCode.Contains(term) select c).ToList();
        //    //if (jobs != null)
        //    //{
        //    //    foreach (var item in jobs)
        //    //    {
        //    //        jobno obj = new jobno();
        //    //        obj.JobNo = item.JobCode;
        //    //        obj.Istrading = false;
        //    //        lst.Add(obj);
        //    //    }
        //    //}
        //    var data1 = (from c in db.SalesInvoices where c.CustomerID == id && c.SalesInvoiceNo.Contains(term)  select c).ToList();
        //    foreach (var item in data1)
        //    {
        //        var salesinvoicedetails = (from d in db.SalesInvoiceDetails where d.SalesInvoiceID == item.SalesInvoiceID select d).ToList();
               
        //            jobno s = new jobno();
        //            s.JobNo = item.SalesInvoiceNo.ToString() + "/ " + salesinvoicedetails.Sum(d => d.NetValue); ;
        //            s.Istrading = true;
        //            s.JobNum = item.SalesInvoiceID;

        //            lst.Add(s);
                
        //    }
        //   // lst = lst.Where(d => d.JobNo.Contains(term)).ToList();


        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}
        public class jobno
        {
          
            public string JobNo { get; set; }
            public int JobNum { get; set; }
            public bool Istrading { get; set; }
         
        }

        //public JsonResult GetAmount(string id)
        //{
        //    Getamtclass ob = new Getamtclass();

        //    int jobid=(from j in db.JobGenerations where j.JobCode==id select j.JobID).FirstOrDefault();

        //   int invid=(from c in db.JInvoices where c.JobID==jobid select c.InvoiceID).FirstOrDefault();

        //    decimal invamt=(from c in db.JInvoices where c.InvoiceID==invid select c.SalesHome).FirstOrDefault().Value;

        //    decimal recamt=(from r in db.RecPayDetails where r.InvoiceID==invid select r.Amount).FirstOrDefault().Value;


        //    ob.invoiceamt = invamt;
        //    ob.recamt = recamt;


        
        //    return Json(ob, JsonRequestBehavior.AllowGet);
        //}
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


        public ActionResult ServiceCreate()
        {
            ViewBag.customer = db.CustomerMasters.Where(d => d.CustomerType == "CR").OrderBy(x => x.CustomerName).ToList();
            ViewBag.achead = db.AcHeads.ToList();
            List<jobno> lst = new List<jobno>();
            ViewBag.jobno = lst;

            return View();


        }

        [HttpPost]
        public ActionResult ServiceCreate(CreditNoteVM v)
        {
            AcJournalMaster ajm = new AcJournalMaster();

            int acjm = 0;
            acjm = (from c in db.AcJournalMasters orderby c.AcJournalID descending select c.AcJournalID).FirstOrDefault();

            ajm.AcJournalID = acjm + 1;
            ajm.AcCompanyID = Convert.ToInt32(Session["AcCompanyID"].ToString());
            ajm.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
            ajm.PaymentType = 1;
            ajm.Remarks = "Credit Note";
            ajm.StatusDelete = false;
            ajm.TransDate = v.Date;
            ajm.VoucherNo = "C-" + ajm.AcJournalID;
            ajm.TransType = 2;
            ajm.VoucherType = "";

            db.AcJournalMasters.Add(ajm);
            db.SaveChanges();


            AcJournalDetail b = new AcJournalDetail();

            int maxacj = 0;
            maxacj = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();

            b.AcJournalDetailID = maxacj + 1;
            b.AcJournalID = ajm.AcJournalID;
            b.AcHeadID = v.AcHeadID;
            b.Amount = -v.Amount;
            b.BranchID = Convert.ToInt32(Session["AcCompanyID"].ToString());
            b.Remarks = "";

            db.AcJournalDetails.Add(b);
            db.SaveChanges();


            AcJournalDetail a = new AcJournalDetail();
            maxacj = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();
            a.AcJournalDetailID = maxacj + 1;
            a.AcJournalID = ajm.AcJournalID;
            a.AcHeadID = v.AcHeadID;
            a.Amount = v.Amount;
            a.BranchID = Convert.ToInt32(Session["AcCompanyID"].ToString());
            a.Remarks = "";







            db.AcJournalDetails.Add(a);
            db.SaveChanges();

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
            invid = Convert.ToInt32(v.JobNO);
            var ids = (from x in db.SalesInvoiceDetails where x.SalesInvoiceID == invid select (int?)x.SalesInvoiceDetailID).ToList();

            recpayid = (from c in db.RecPayDetails where ids.Contains(c.InvoiceID) select c.RecPayID).FirstOrDefault().Value;

            CreditNote d = new CreditNote();

            //int max = (from c in db.CreditNotes orderby c.CreditNoteNo descending select c.CreditNoteNo).FirstOrDefault().Value;
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
            d.CreditNoteNo = maxid;
            d.InvoiceID = invid;
            d.CreditNoteDate = v.Date;
            d.Amount = v.Amount;
            d.AcJournalID = ajm.AcJournalID;
            d.FYearID = Convert.ToInt32(Session["fyearid"].ToString());
            d.AcCompanyID = Convert.ToInt32(Session["AcCompanyID"].ToString());
            d.RecPayID = recpayid;
            d.AcHeadID = v.AcHeadID;
            d.CustomerID = v.CustomerID;
            d.statusclose = false;
            d.InvoiceType = "C";
            d.IsShipping = false;

            db.CreditNotes.Add(d);
            db.SaveChanges();

            TempData["SuccessMsg"] = "Successfully Added Credit Note";
            return RedirectToAction("ServiceIndex", "CreditNote");





        }

        public class Getamtclass
        {
            public decimal? invoiceamt { get; set; }
            public decimal? recamt { get; set; }

        }

    }
}
