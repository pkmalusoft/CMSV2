using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using CMSV2.DAL;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
namespace CMSV2.Controllers
{
    [SessionExpire]
    public class TaxInvoiceController : Controller
    {
        Entities1 db = new Entities1();

        public ActionResult Index(int? StatusId, string FromDate, string ToDate)
        {
            ShipmentInvoiceSearch obj = (ShipmentInvoiceSearch)Session["ShipmentInvoiceSearch"];
            ShipmentInvoiceSearch model = new ShipmentInvoiceSearch();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            if (obj == null)
            {
                DateTime pFromDate;
                DateTime pToDate;
                //int pStatusId = 0;
                pFromDate = CommanFunctions.GetFirstDayofWeek().Date;
                pToDate = CommanFunctions.GetLastDayofMonth().Date;
                obj = new ShipmentInvoiceSearch();
                obj.AWBNo = "";
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.Details = new List<ShipmentInvoiceVM>();
                Session["ShipmentInvoiceSearch"] = obj;
                model.FromDate = pFromDate;
                model.ToDate = pToDate;
                model.Details = new List<ShipmentInvoiceVM>();
            }
            else
            {
                model = obj;
                obj.Details = new List<ShipmentInvoiceVM>();
            }
            List<ShipmentInvoiceVM> lst = PickupRequestDAO.GetTaxInvoiceList(obj.AWBNo,obj.FromDate, obj.ToDate, yearid, branchid, depotId);
            model.Details = lst;
            return View(model);

        }
        [HttpPost]
        public ActionResult Index(ShipmentInvoiceSearch obj)
        {
            Session["ShipmentInvoiceSearch"] = obj;
            return RedirectToAction("Index");
        }

        public ActionResult Create(int id = 0)
        {
            int UserId = Convert.ToInt32(Session["UserID"].ToString());
            int BranchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotid = Convert.ToInt32(Session["CurrentDepotID"].ToString());
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            //ViewBag.depot = db.tblDepots.ToList();
            ViewBag.depot = (from c in db.tblDepots where c.BranchID == BranchId select c).ToList();
            ViewBag.employee = db.EmployeeMasters.ToList();
            ViewBag.employeerec = db.EmployeeMasters.ToList();
            ViewBag.Vehicles = db.VehicleMasters.ToList();
            ViewBag.Agents = db.AgentMasters.ToList();
            ViewBag.CourierService = db.CourierServices.ToList();
            ShipmentInvoiceVM vm = new ShipmentInvoiceVM();
            if (id == 0)
            {
                ViewBag.Title = "Tax Invoice";

                vm.ShipmentInvoiceID = 0;

                PickupRequestDAO _dao = new PickupRequestDAO();
                vm.InvoiceDate = CommanFunctions.GetCurrentDateTime();
                vm.InvoiceNo = _dao.GetMaxTaxInvoiceNo(companyid, BranchId, yearid);
                vm.BranchId = BranchId;
                var emp = db.EmployeeMasters.Where(cc => cc.UserID == UserId).FirstOrDefault();
                if (emp != null)
                {
                    vm.EmployeeID = emp.EmployeeID;
                }
                ViewBag.EditMode = "false";
                List<ShipmentInvoiceDetailVM> awblist = new List<ShipmentInvoiceDetailVM>();
                vm.Details = awblist;
                Session["TaxInvoiceDetail"] = vm.Details;
                return View(vm);
            }
            else
            {
                ShipmentInvoice qvm = db.ShipmentInvoices.Find(id);

                vm.ShipmentInvoiceID = qvm.ShipmentInvoiceID;
                vm.TaxPercent = 5;
                vm.InvoiceDate= qvm.InvoiceDate;
                vm.InvoiceNo = qvm.InvoiceNo;
                vm.EmployeeID = qvm.EmployeeID;
                vm.MAWB = qvm.MAWB;
                vm.ShipmentImportID = qvm.ShipmentImportID;
                vm.FYearId = qvm.FYearId;
                vm.BranchId = Convert.ToInt32(qvm.BranchId);
                ViewBag.EditMode = "true";
                ViewBag.Title = "Tax Invoice - Modify";
                var awblist = (from _qmaster in db.ShipmentInvoiceDetails
                               join _shipdetail in db.ImportShipmentDetails on _qmaster.ShipmentImportDetailID equals _shipdetail.ShipmentDetailID
                               where _qmaster.ShipmentInvoiceID == qvm.ShipmentInvoiceID
                               orderby _shipdetail.AWB descending
                               select new ShipmentInvoiceDetailVM
                               {
                                   ShipmentInvoiceDetailID = _qmaster.ShipmentInvoiceDetailID,
                                   ShipmentInvoiceID = _qmaster.ShipmentInvoiceID,
                                   ShipmentImportDetailID = _qmaster.ShipmentImportDetailID,
                                   AWBNo = _shipdetail.AWB,
                                   BagNo = _shipdetail.BagNo,
                                   Shipper = _shipdetail.Shipper,
                                   TaxP = _qmaster.TaxP,
                                   Tax = _qmaster.Tax,
                                   adminCharges = _qmaster.adminCharges,
                                   CustomValue = _shipdetail.CustomValue,
                                   InvoiceValue = _qmaster.InvoiceValue,
                                   //RemoveAllowed = true,
                                   AWBChecked = true

                                   //StatusTypeId _shipdetail.StatusTypeId
                               }).ToList();


                //int i = 0;
                //foreach (var item in awblist)
                //{
                //    if (item.CourierStatusId != 21) //received at desitnation/nice facility
                //    {
                //        awblist[i].RemoveAllowed = false;
                //    }
                //    i++;
                //}
                vm.Details = awblist;
                Session["TaxInvoiceDetail"] = vm.Details;
                return View(vm);
            }

        }

        public ActionResult Details(int id = 0)
        {
            int UserId = Convert.ToInt32(Session["UserID"].ToString());
            int BranchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotid = Convert.ToInt32(Session["CurrentDepotID"].ToString());
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            //ViewBag.depot = db.tblDepots.ToList();
            ViewBag.depot = (from c in db.tblDepots where c.BranchID == BranchId select c).ToList();
            ViewBag.employee = db.EmployeeMasters.ToList();
            ViewBag.employeerec = db.EmployeeMasters.ToList();
            ViewBag.Vehicles = db.VehicleMasters.ToList();
            ViewBag.Agents = db.AgentMasters.ToList();
            ViewBag.CourierService = db.CourierServices.ToList();
            ShipmentInvoiceVM vm = new ShipmentInvoiceVM();
            if (id == 0)
            {
                ViewBag.Title = "Tax Invoice";

                vm.ShipmentInvoiceID = 0;

                PickupRequestDAO _dao = new PickupRequestDAO();
                vm.InvoiceDate = CommanFunctions.GetCurrentDateTime();
                vm.InvoiceNo = _dao.GetMaxTaxInvoiceNo(companyid, BranchId, yearid);
                vm.BranchId = BranchId;
                var emp = db.EmployeeMasters.Where(cc => cc.UserID == UserId).FirstOrDefault();
                if (emp != null)
                {
                    vm.EmployeeID = emp.EmployeeID;
                }
                ViewBag.EditMode = "false";
                List<ShipmentInvoiceDetailVM> awblist = new List<ShipmentInvoiceDetailVM>();
                vm.Details = awblist;
                Session["TaxInvoiceDetail"] = vm.Details;
                return View(vm);
            }
            else
            {
                ShipmentInvoice qvm = db.ShipmentInvoices.Find(id);

                vm.ShipmentInvoiceID = qvm.ShipmentInvoiceID;
                vm.TaxPercent = 5;
                vm.InvoiceDate = qvm.InvoiceDate;
                vm.InvoiceNo = qvm.InvoiceNo;
                vm.EmployeeID = qvm.EmployeeID;
                vm.MAWB = qvm.MAWB;
                vm.ShipmentImportID = qvm.ShipmentImportID;
                vm.FYearId = qvm.FYearId;
                vm.BranchId = Convert.ToInt32(qvm.BranchId);
                ViewBag.EditMode = "true";
                ViewBag.Title = "Tax Invoice - Modify";
                var awblist = (from _qmaster in db.ShipmentInvoiceDetails
                               join _shipdetail in db.ImportShipmentDetails on _qmaster.ShipmentImportDetailID equals _shipdetail.ShipmentDetailID
                               where _qmaster.ShipmentInvoiceID == qvm.ShipmentInvoiceID
                               orderby _shipdetail.AWB descending
                               select new ShipmentInvoiceDetailVM
                               {
                                   ShipmentInvoiceDetailID = _qmaster.ShipmentInvoiceDetailID,
                                   ShipmentInvoiceID = _qmaster.ShipmentInvoiceID,
                                   ShipmentImportDetailID = _qmaster.ShipmentImportDetailID,
                                   AWBNo = _shipdetail.AWB,
                                   BagNo = _shipdetail.BagNo,
                                   Shipper = _shipdetail.Shipper,
                                   TaxP = _qmaster.TaxP,
                                   Tax = _qmaster.Tax,
                                   adminCharges = _qmaster.adminCharges,
                                   CustomValue = _shipdetail.CustomValue,
                                   InvoiceValue = _qmaster.InvoiceValue,
                                   //RemoveAllowed = true,
                                   AWBChecked = true

                                   //StatusTypeId _shipdetail.StatusTypeId
                               }).ToList();



                vm.Details = awblist;
                Session["TaxInvoiceDetail"] = vm.Details;
                return View(vm);
            }

        }

        [HttpPost]
        public JsonResult SaveTaxInvoice(ShipmentInvoiceVM v)
        {
            PickupRequestDAO _dao = new PickupRequestDAO();
            var bills = new List<updateitem>();
            var IDetails = JsonConvert.DeserializeObject<List<ShipmentInvoiceDetailVM>>(v.AWBDetails);
            int UserId = Convert.ToInt32(Session["UserID"].ToString());
            int BranchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int CompanyId = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            try
            {
                ShipmentInvoice _invoice = new ShipmentInvoice();
                if (v.ShipmentInvoiceID > 0)
                {
                    _invoice = db.ShipmentInvoices.Find(v.ShipmentInvoiceID);
                    _invoice.ModifiedBy = UserId;
                    _invoice.ModifiedDate = CommanFunctions.GetCurrentDateTime();
                    _invoice.EmployeeID = v.EmployeeID;
                }
                else
                {
                    v.InvoiceNo = _dao.GetMaxTaxInvoiceNo(CompanyId, BranchId, yearid);
                    _invoice.InvoiceNo = v.InvoiceNo;
                    _invoice.InvoiceDate = v.InvoiceDate;
                    _invoice.AcCompanyID = CompanyId;
                    _invoice.MAWB = v.MAWB;
                    _invoice.BranchId = BranchId;
                    _invoice.EmployeeID = v.EmployeeID;
                    _invoice.FYearId = yearid;
                    _invoice.ShipmentImportID = v.ShipmentImportID;
                    _invoice.CreatedBy = UserId;
                    _invoice.CreatedDate = CommanFunctions.GetCurrentDateTime();
                    _invoice.ModifiedBy = UserId;
                    _invoice.ModifiedDate = CommanFunctions.GetCurrentDateTime();
                    _invoice.ShipmentImportID = v.ShipmentImportID;
                }

                if (v.ShipmentInvoiceID > 0)
                {
                    db.Entry(_invoice).State = EntityState.Modified;
                    db.SaveChanges();

                    //var stockdetails = (from d in db.SupplierInvoiceStocks where d.SupplierInvoiceID == Supplierinvoice.SupplierInvoiceID select d).ToList();
                    var sdetails = (from d in db.ShipmentInvoiceDetails where d.ShipmentInvoiceID == v.ShipmentInvoiceID select d).ToList();
                    db.ShipmentInvoiceDetails.RemoveRange(sdetails);
                    db.SaveChanges();


                }
                else
                {
                    db.ShipmentInvoices.Add(_invoice);
                    db.SaveChanges();
                }

                List<ShipmentInvoiceDetailVM> Details1 = (List<ShipmentInvoiceDetailVM>)Session["TaxInvoiceDetail"];
                decimal totalinvoice = 0;
                foreach (var _item in IDetails)
                {
                    if (_item.AWBChecked)
                    {
                        var selitem = Details1.Find(cc => cc.AWBNo == _item.AWBNo);
                        if (selitem != null)
                        {
                            int _inscanid = Convert.ToInt32(_item.ShipmentImportDetailID);

                            ShipmentInvoiceDetail _shipdetail = new ShipmentInvoiceDetail();
                            _shipdetail.ShipmentInvoiceID = _invoice.ShipmentInvoiceID;
                            _shipdetail.ShipmentImportDetailID = selitem.ShipmentImportDetailID;
                            _shipdetail.Tax = selitem.Tax;
                            _shipdetail.TaxP = selitem.TaxP;
                            _shipdetail.InvoiceValue = selitem.InvoiceValue;
                            _shipdetail.adminCharges = selitem.adminCharges;
                            db.ShipmentInvoiceDetails.Add(_shipdetail);
                            db.SaveChanges();

                            totalinvoice = totalinvoice + Convert.ToDecimal(_shipdetail.InvoiceValue);

                            var shipment = db.ImportShipmentDetails.Find(selitem.ShipmentImportDetailID);
                            shipment.ShipmentInvoiceId = _invoice.ShipmentInvoiceID;
                            db.Entry(shipment).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }

                //Update invoice total
                _invoice = db.ShipmentInvoices.Find(_invoice.ShipmentInvoiceID);
                _invoice.InvoiceTotal = totalinvoice;
                db.Entry(_invoice).State = EntityState.Modified;
                db.SaveChanges();

                //Accounts Posting

                _dao.GenerateTaxInvoicePosting(_invoice.ShipmentInvoiceID);
                //TempData["SuccessMsg"] = "You have successfully Saved InScan Items.";             

                return Json(new { status = "ok", message = "You have successfully Saved VAT Invoice!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "Failed", message = ex.Message }, JsonRequestBehavior.AllowGet);
                //return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetMAWBList(string term)
        {

            List<ShipmentInvoiceVM> lst = PickupRequestDAO.GetShipmentMAWBList();
            if (term.Trim() != "")
            {
                var list = lst.Where(cc => cc.MAWB.Contains(term.Trim())).OrderBy(cc => cc.MAWB).Take(25).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = lst.OrderBy(cc => cc.MAWB).Take(25).ToList();
            }

            return Json(lst, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetMAWBDetail(int ShipmentImportID, string MAWB, bool checkall)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            var branch = db.BranchMasters.Find(branchid);
            decimal percent = 5;
            if (branch != null)
            {
                if (branch.VATPercent != null)
                {
                    percent = Convert.ToDecimal(branch.VATPercent);
                }
            }

            var lst = (from c in db.ImportShipmentDetails
                       join imp in db.ImportShipments on c.ImportID equals imp.ID
                       where c.MAWB.Trim() == MAWB.Trim() && c.ImportID == ShipmentImportID
                       && c.CustomValue > 0 && c.CustomValue <= 269 && c.ShipmentInvoiceId == null
                       select new ShipmentInvoiceDetailVM { Shipper = c.Shipper, CurrencyID = c.CurrencyID, BagNo = c.BagNo, AWBNo = c.AWB, ShipmentImportDetailID = c.ShipmentDetailID, TaxP = 5, Tax = 0, CustomValue = c.CustomValue, adminCharges = 0, AWBChecked = checkall }).ToList();  //forwarded to agent status only

            int i = 0;
            if (lst.Count > 0)
            {
                var currency = db.CurrencyMasters.Find(lst[0].CurrencyID);
                var currencyvalue = currency.ExchangeRate;

                foreach (var item in lst)
                {
                    
                    decimal invoiceval = decimal.Round(Convert.ToDecimal(Convert.ToDecimal(lst[i].CustomValue) * currencyvalue),2);
                    lst[i].InvoiceValue = invoiceval;
                    lst[i].TaxP = percent;
                    decimal taxval = decimal.Round(Convert.ToDecimal(invoiceval * (percent / 100)), 2);

                    decimal adminval = 0;
                    if (taxval < 1)
                    {
                        adminval = taxval;
                        lst[i].adminCharges = 1 - adminval;
                    }
                    else
                    {
                        adminval = ((taxval * 100) % 100);
                        lst[i].adminCharges = (100 - adminval)/100;
                    }
                    lst[i].Tax = taxval;
                    
                    i++;
                }
            }

            if (lst == null)
            {
                return Json(new { status = "Failed", Message = "AWB No. Not found" }, JsonRequestBehavior.AllowGet);
            }
            else if (lst.Count > 0)
            {


                Session["TaxInvoiceDetail"] = lst;
                return Json(new { status = "ok", Message = "AWB Number found" }, JsonRequestBehavior.AllowGet);


            }
            else
            {
                Session["TaxInvoiceDetail"] = new List<ShipmentInvoiceDetailVM>();
                return Json(new { status = "Failed", Message = "No AWB Number found" }, JsonRequestBehavior.AllowGet);


            }

        }


        public JsonResult GetInvoiceDetail(int ShipmentInvoiceID,bool checkall)
         {
              var awblist = (from _qmaster in db.ShipmentInvoiceDetails
                             join _shipdetail in db.ImportShipmentDetails on _qmaster.ShipmentImportDetailID equals _shipdetail.ShipmentDetailID
                             where _qmaster.ShipmentInvoiceID == ShipmentInvoiceID
                             orderby _shipdetail.AWB descending
                             select new ShipmentInvoiceDetailVM
                             {
                                 ShipmentInvoiceDetailID = _qmaster.ShipmentInvoiceDetailID,
                                 ShipmentInvoiceID = _qmaster.ShipmentInvoiceID,
                                 ShipmentImportDetailID = _qmaster.ShipmentImportDetailID,
                                 AWBNo = _shipdetail.AWB,
                                 BagNo = _shipdetail.BagNo,
                                 Shipper = _shipdetail.Shipper,
                                 TaxP = _qmaster.TaxP,
                                 Tax = _qmaster.Tax,
                                 adminCharges = _qmaster.adminCharges,
                                 CustomValue = _shipdetail.CustomValue,
                                 InvoiceValue = _qmaster.InvoiceValue,                                 
                                 AWBChecked = checkall                                 
                             }).ToList();


                    Session["TaxInvoiceDetail"] = awblist;
                return Json(new { status = "ok", Message = "Invoice Item found" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShowAWBList()
        {
            ShipmentInvoiceVM vm = new ShipmentInvoiceVM();
            vm.Details = (List<ShipmentInvoiceDetailVM>)Session["TaxInvoiceDetail"];
            return PartialView("ItemList", vm);
        }
        public ActionResult InvoicePrint(int id, string Details="")
        {
            ViewBag.ReportName = "Invoice Printing";
            //LabelPrintingParam picker = SessionDataModel.GetLabelPrintParam();
            string monetaryunit = Session["MonetaryUnit"].ToString();
            AccountsReportsDAO.CustomerVATTaxInvoiceReport(id, Details);
            //AccountsReportsDAO.CustomerTaxInvoiceReport(id, monetaryunit);
            return View();
        }

        public JsonResult GetImportAWBList(string term)
        {

            if (term.Trim() != "")
            {
                var list = (from c in db.ShipmentInvoiceDetails
                            join d in db.ImportShipmentDetails on c.ShipmentImportDetailID equals d.ShipmentDetailID
                            where d.AWB.Contains(term.Trim())
                            orderby d.AWB
                            select new { AWB = d.AWB, ShipmentInvoiceId=c.ShipmentInvoiceID, ShipmentInvoiceDetailId = c.ShipmentInvoiceDetailID }).Take(25).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = (from c in db.ShipmentInvoiceDetails
                            join d in db.ImportShipmentDetails on c.ShipmentImportDetailID equals d.ShipmentDetailID
                            orderby d.AWB
                            select new { AWB = d.AWB, ShipmentInvoiceId=c.ShipmentInvoiceID, ShipmentInvoiceDetailId = c.ShipmentInvoiceDetailID }).Take(25).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }
    }
}