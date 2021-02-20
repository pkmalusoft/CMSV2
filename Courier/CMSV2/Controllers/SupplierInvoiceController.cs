using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using CMSV2.DAL;
using Newtonsoft.Json;
using System.Data;
using System.Data.Entity;
namespace CMSV2.Controllers
{ [SessionExpire]
    public class SupplierInvoiceController : Controller
    {
        Entities1 db = new Entities1();
        // GET: SupplierInvoice
        public ActionResult Index(string FromDate, string ToDate)
        {
                        
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());

            DateTime pFromDate;
            DateTime pToDate;
                     
            if (FromDate == null || ToDate == null)
            {
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date;//.AddDays(-1); // FromDate = DateTime.Now;
                pToDate = CommanFunctions.GetLastDayofMonth().Date.AddDays(1); // // ToDate = DateTime.Now;
            }
            else
            {
                pFromDate = Convert.ToDateTime(FromDate);//.AddDays(-1);
                pToDate = Convert.ToDateTime(ToDate).AddDays(1);

            }

            var lst = (from c in db.SupplierInvoices                       
                       join s in db.SupplierMasters on c.SupplierID equals s.SupplierID orderby c.InvoiceDate descending
                       where c.InvoiceNo.StartsWith("SI")
                       select new SupplierInvoiceVM {SupplierInvoiceID=c.SupplierInvoiceID, InvoiceNo = c.InvoiceNo, InvoiceDate = c.InvoiceDate, SupplierName = s.SupplierName, Amount = 0,SupplierType=s.SupplierType.SupplierType1,Remarks=s.Remarks }).ToList();
            lst.ForEach(d => d.Amount = (from s in db.SupplierInvoiceDetails where s.SupplierInvoiceID == d.SupplierInvoiceID select s).ToList().Sum(a => a.Value));
            ViewBag.FromDate = pFromDate.Date.ToString("dd-MM-yyyy");
            ViewBag.ToDate = pToDate.Date.AddDays(-1).ToString("dd-MM-yyyy");

            return View(lst);
        }

        public ActionResult Create(int id)
        {
           var suppliers = db.SupplierMasters.ToList();
            ViewBag.Supplier = suppliers;
            ViewBag.SupplierType = db.SupplierTypes.ToList();
            ViewBag.Currency = db.CurrencyMasters.ToList();
            SupplierInvoiceVM _supinvoice = new SupplierInvoiceVM();
            ViewBag.CurrencyId = Convert.ToInt32(Session["CurrencyId"].ToString());
            if (id > 0)
            {
                ViewBag.Title = "Supplier Invoice -Modify";
                var _invoice = db.SupplierInvoices.Find(id);
                _supinvoice.SupplierInvoiceID = _invoice.SupplierInvoiceID;
                _supinvoice.InvoiceDate = _invoice.InvoiceDate;
                _supinvoice.InvoiceNo = _invoice.InvoiceNo;
                _supinvoice.SupplierID = _invoice.SupplierID;
                _supinvoice.Remarks = _invoice.Remarks;
                var supplier = suppliers.Where(d => d.SupplierID == _invoice.SupplierID).FirstOrDefault();
                if (supplier != null)
                {
                    _supinvoice.SupplierName = supplier.SupplierName;
                    _supinvoice.SupplierTypeId = Convert.ToInt32(supplier.SupplierTypeID);
                }

                //List<SupplierInvoiceDetail> _details = new List<SupplierInvoiceDetail>();
                List<SupplierInvoiceDetailVM> _details = new List<SupplierInvoiceDetailVM>();
                _details = (from c in db.SupplierInvoiceDetails join a in db.AcHeads on c.AcHeadID equals a.AcHeadID
                            where c.SupplierInvoiceID == id
                            select new SupplierInvoiceDetailVM {SupplierInvoiceDetailID=c.SupplierInvoiceDetailID,SupplierInvoiceID=c.SupplierInvoiceID,AcHeadId=c.AcHeadID,AcHeadName=a.AcHead1,Particulars=c.Particulars,TaxPercentage=c.TaxPercentage,CurrencyID=c.CurrencyID,Amount=c.Amount,Rate=c.Rate, Quantity=c.Quantity, Value=c.Value}   ).ToList();


                _supinvoice.details = _details;
                
                Session["SInvoiceListing"] = _details;
                               
                
                List<SupplierInvoiceAWBVM> AWBAllocationall = (from c in db.SupplierInvoiceAWBs  join d in db.InScanMasters on c.InScanID equals d.InScanID 
                                                                       where c.SupplierInvoiceId == id select new SupplierInvoiceAWBVM { ID = c.ID, SupplierInvoiceId = c.SupplierInvoiceId, SupplierInvoiceDetailId = c.SupplierInvoiceDetailId,
                                                                                    AcHeadId = c.AcHeadId, Amount = c.Amount, InScanID = c.InScanID, ConsignmentNo = d.AWBNo, ConsignmentDate = d.TransactionDate }).ToList();
                Session["SIAWBAllocation"] = AWBAllocationall;
            }
            else
            {
                ViewBag.Title = "Supplier Invoice - Create";
                var Maxnumber = db.SupplierInvoices.ToList().LastOrDefault();
                _supinvoice.InvoiceNo = ReceiptDAO.SP_GetMaxSINo();
            }
            return View(_supinvoice);

        }
        public JsonResult SetSupplierInvDetails(int acheadid,string achead, string invno,string Particulars, decimal Rate, int Qty,decimal amount,int currency, decimal Taxpercent,decimal netvalue)
        {
            Random rnd = new Random();
            int dice = rnd.Next(1, 7);   // creates a number between 1 and 6
           
            var invoice = new SupplierInvoiceDetailVM();
            invoice.AcHeadId = acheadid;
            invoice.AcHeadName = achead;
            invoice.InvNo = invno+"_"+ dice;
            invoice.Particulars = Particulars;
            invoice.Rate =Rate;
            invoice.Quantity = Qty;
            invoice.CurrencyID = currency;
            var currencyMaster = db.CurrencyMasters.Find(currency);
            invoice.CurrencyAmount =Convert.ToDecimal(currencyMaster.ExchangeRate);
            invoice.Currency =currencyMaster.CurrencyName;
            //var amount = (Qty * Rate);
            //var value = amount + (amount * Taxpercent / 100);
          
            invoice.Amount = amount;
            invoice.Value =netvalue;
            invoice.TaxPercentage = Taxpercent;

            return Json(new { InvoiceDetails = invoice }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSupplierInvDetails(int Id)
        {
            Random rnd = new Random();
             // creates a number between 1 and 6

            var _invoice = db.SupplierInvoices.Find(Id);
            List<SupplierInvoiceDetailVM> _details = new List<SupplierInvoiceDetailVM>();
            List<SupplierInvoiceDetailVM> _details1 = new List<SupplierInvoiceDetailVM>();
            _details = (from c in db.SupplierInvoiceDetails
                        join a in db.AcHeads on c.AcHeadID equals a.AcHeadID
                        join cu in db.CurrencyMasters on c.CurrencyID equals cu.CurrencyID
                        where c.SupplierInvoiceID == Id
                        select new SupplierInvoiceDetailVM { SupplierInvoiceDetailID = c.SupplierInvoiceDetailID, SupplierInvoiceID = c.SupplierInvoiceID, AcHeadId = c.AcHeadID, AcHeadName = a.AcHead1, Particulars = c.Particulars, TaxPercentage = c.TaxPercentage, CurrencyID = c.CurrencyID, Amount = c.Amount, Rate = c.Rate, Quantity = c.Quantity, Value = c.Value ,Currency=cu.CurrencyCode }).ToList();

            //var details = (from c in db.SupplierInvoiceDetails
            //            where c.SupplierInvoiceID == Id
            //            select c).ToList();
            foreach (var item in _details)
            {
                int dice = rnd.Next(1, 7);
                var invoice = new SupplierInvoiceDetailVM();
                invoice.SupplierInvoiceDetailID = item.SupplierInvoiceDetailID;
                invoice.AcHeadId = item.AcHeadId;
                invoice.AcHeadName = item.AcHeadName;
                invoice.InvNo = _invoice.InvoiceNo + "_" + dice;
                invoice.Particulars = item.Particulars;
                invoice.Rate = item.Rate;
                invoice.Quantity = item.Quantity;
                invoice.CurrencyID = item.CurrencyID;
                var currencyMaster = db.CurrencyMasters.Find(item.CurrencyID);
                invoice.CurrencyAmount = Convert.ToDecimal(currencyMaster.ExchangeRate);
                invoice.Currency = currencyMaster.CurrencyName;
                //decimal amount = (item.Quantity * item.Rate);
                //decimal value = amount + (amount * Convert.ToDecimal(item.TaxPercentage) / 100);
                invoice.Amount = item.Amount;
                invoice.Value = item.Value;
                invoice.TaxPercentage = item.TaxPercentage;
                _details1.Add(invoice);
            }

            return Json(new { InvoiceDetails = _details1 }, JsonRequestBehavior.AllowGet);
        }
        //SaveSupplierInvoice
        public JsonResult SaveSupplierInvoice(int Id, int SupplierID, string InvoiceDate, string InvoiceNo, string Remarks, string Details)
        {
            try
            {
                var IDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceDetailVM>>(Details);
                List<SupplierInvoiceAWBVM> AWBAllocationall = new List<SupplierInvoiceAWBVM>();
                List<SupplierInvoiceAWBVM> AWBAllocation = new List<SupplierInvoiceAWBVM>();
                AWBAllocationall = (List<SupplierInvoiceAWBVM>)Session["SIAWBAllocation"];
                var Supplierinvoice = (from d in db.SupplierInvoices where d.SupplierInvoiceID == Id select d).FirstOrDefault();
                if (Supplierinvoice == null)
                {
                    Supplierinvoice = new SupplierInvoice();
                }
                else
                {
                    var details = (from d in db.SupplierInvoiceDetails where d.SupplierInvoiceID == Supplierinvoice.SupplierInvoiceID select d).ToList();
                    db.SupplierInvoiceDetails.RemoveRange(details);
                    db.SaveChanges();

                    var consignmentdetails = (from d in db.SupplierInvoiceAWBs where d.SupplierInvoiceId == Supplierinvoice.SupplierInvoiceID select d).ToList();
                    db.SupplierInvoiceAWBs.RemoveRange(consignmentdetails);
                    db.SaveChanges();
                }

                Supplierinvoice.SupplierID = SupplierID;
                Supplierinvoice.InvoiceDate = Convert.ToDateTime(InvoiceDate);
                Supplierinvoice.InvoiceNo = InvoiceNo;
                Supplierinvoice.AccompanyID = Convert.ToInt32(Session["CurrentCompanyID"]); 
                Supplierinvoice.BranchId = Convert.ToInt32(Session["CurrentBranchID"]); 
                Supplierinvoice.FyearID = Convert.ToInt32(Session["fyearid"]);
                Supplierinvoice.InvoiceTotal = IDetails.Sum(d => d.Amount);
                Supplierinvoice.StatusClose = false;
                Supplierinvoice.IsDeleted = false;
                Supplierinvoice.Remarks = Remarks;
                if (Supplierinvoice.SupplierInvoiceID == 0)
                {
                    db.SupplierInvoices.Add(Supplierinvoice);
                }
                db.SaveChanges();
                foreach (var item in IDetails)
                {
                    var InvoiceDetail = new SupplierInvoiceDetail();
                    InvoiceDetail.SupplierInvoiceID = Supplierinvoice.SupplierInvoiceID;
                    InvoiceDetail.AcHeadID = item.AcHeadId;
                    InvoiceDetail.Particulars = item.Particulars;
                    InvoiceDetail.Quantity = item.Quantity;
                    InvoiceDetail.Rate = item.Rate;
                    InvoiceDetail.CurrencyID = item.CurrencyID;
                    InvoiceDetail.CurrencyAmount = item.CurrencyAmount;
                    InvoiceDetail.Amount = item.Amount;
                    InvoiceDetail.TaxPercentage = item.TaxPercentage;
                    InvoiceDetail.Value = item.Value;

                    db.SupplierInvoiceDetails.Add(InvoiceDetail);
                    db.SaveChanges();

                    //adding consignment referece to this entry
                    int acheadid = Convert.ToInt32(item.AcHeadId);

                    if (AWBAllocationall != null)
                    {

                        var list = AWBAllocationall.Where(cc => cc.AcHeadId == acheadid).ToList();
                        if (list != null)
                        {
                            foreach (var item2 in list)
                            {
                                SupplierInvoiceAWB accons = new SupplierInvoiceAWB();
                                accons.SupplierInvoiceId = Supplierinvoice.SupplierInvoiceID;
                                accons.SupplierInvoiceDetailId = item.SupplierInvoiceDetailID;
                                accons.AcHeadId = acheadid;
                                accons.InScanID = Convert.ToInt32(item2.InScanID);
                                accons.Amount = item2.Amount;
                                db.SupplierInvoiceAWBs.Add(accons);
                                db.SaveChanges();
                            }
                        }
                    }

                }

                PickupRequestDAO dao = new PickupRequestDAO();
                dao.GenerateSupplierInvoicePosting(Supplierinvoice.SupplierInvoiceID);

                return Json(new { status = "ok", message = "Invoice Submitted Successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = "failed", message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Delete (int id)
        {
            //int k = 0;
            if (id != 0)
            {
                DataTable dt = ReceiptDAO.DeleteSupplierInvoice(id);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
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

            return RedirectToAction("Index");

        }

        public ActionResult AccountHead(string term)
        {
            int branchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            if (!String.IsNullOrEmpty(term))
            {
                List<AcHeadSelectAllVM> AccountHeadList = new List<AcHeadSelectAllVM>();
                AccountHeadList = AccountsDAO.GetAcHeadSelectAll(branchID).Where(c => c.AcHead.ToLower().Contains(term.ToLower())).OrderBy(x => x.AcHead).ToList(); ;

                //List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
                //AccountHeadList =db.AcHeadSelectAll(branchID).Where(c => c.AcHead.ToLower().Contains(term.ToLower())).OrderBy(x => x.AcHead).ToList();
                return Json(AccountHeadList, JsonRequestBehavior.AllowGet);

                //List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
                //AccountHeadList = MM.AcHeadSelectAll(Common.ParseInt(Session["CurrentBranchID"].ToString()), term);

            }
            else
            {
                List<AcHeadSelectAllVM> AccountHeadList = new List<AcHeadSelectAllVM>();
                AccountHeadList = AccountsDAO.GetAcHeadSelectAll(branchID);
                //List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
                //AccountHeadList = db.AcHeadSelectAll(branchID).ToList();
                return Json(AccountHeadList, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetAWBAllocation(int AcHeadId)
        {
            List<SupplierInvoiceAWBVM> AWBAllocationall = new List<SupplierInvoiceAWBVM>();
            List<SupplierInvoiceAWBVM> AWBAllocation = new List<SupplierInvoiceAWBVM>();
            AWBAllocationall = (List<SupplierInvoiceAWBVM>)Session["SIAWBAllocation"];
            if (AWBAllocationall == null)
            {
                return Json(AWBAllocation, JsonRequestBehavior.AllowGet);
            }
            else
            {
                AWBAllocation = AWBAllocationall.Where(cc => cc.AcHeadId == AcHeadId).ToList();
            }

            if (AWBAllocation == null)
            {
                AWBAllocation = new List<SupplierInvoiceAWBVM>();

            }
            return Json(AWBAllocation, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveAWBAllocation(List<SupplierInvoiceAWBVM> list)
        {

            List<SupplierInvoiceAWBVM> AWBAllocationall = new List<SupplierInvoiceAWBVM>();
            List<SupplierInvoiceAWBVM> AWBAllocation = new List<SupplierInvoiceAWBVM>();
            AWBAllocationall = (List<SupplierInvoiceAWBVM>)Session["SIAWBAllocation"];

            if (AWBAllocationall == null)
            {
                AWBAllocationall = new List<SupplierInvoiceAWBVM>();
                foreach (var item2 in list)
                {
                    AWBAllocationall.Add(item2);

                }

            }
            else
            {
                int acheadid = list[0].AcHeadId;
                AWBAllocationall.RemoveAll(cc => cc.AcHeadId == acheadid);
                foreach (var item2 in list)
                {
                    AWBAllocationall.Add(item2);

                }
            }

            Session["SIAWBAllocation"] = AWBAllocationall;

            return Json(AWBAllocationall, JsonRequestBehavior.AllowGet);

        }
    }
}