﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using CMSV2.DAL;
using System.Data;
using System.Data.Entity;

namespace CMSV2.Controllers
{
    [SessionExpireFilter]
    public class CustomerInvoiceController : Controller
    {
        Entities1 db = new Entities1();



        public ActionResult Index()
        {

            CustomerInvoiceSearch obj = (CustomerInvoiceSearch)Session["CustomerInvoiceSearch"];
            CustomerInvoiceSearch model = new CustomerInvoiceSearch();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            if (obj == null)
            {
                DateTime pFromDate;
                DateTime pToDate;
                //int pStatusId = 0;
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date;
                pToDate = CommanFunctions.GetLastDayofMonth().Date;                                                                      
                obj = new CustomerInvoiceSearch(); 
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.InvoiceNo = "";
                Session["CustomerInvoiceSearch"] = obj;
                model.FromDate = pFromDate;
                model.ToDate = pToDate;
                model.InvoiceNo = "";
            }
            else
            {
                model = obj;
            }
            List<CustomerInvoiceVM> lst = PickupRequestDAO.GetInvoiceList(obj.FromDate, obj.ToDate,model.InvoiceNo, yearid);
            model.Details = lst;
            
            return View(model);


        }
        [HttpPost]
        public ActionResult Index(CustomerInvoiceSearch obj)
        {
            Session["CustomerInvoiceSearch"] = obj;
            return RedirectToAction("Index");
        }
        
        public ActionResult InvoiceSearch()
        {

            DatePicker datePicker = SessionDataModel.GetTableVariable();

            if (datePicker == null)
            {
                datePicker = new DatePicker();
                datePicker.FromDate = CommanFunctions.GetFirstDayofMonth().Date; // DateTime.Now.Date;
                datePicker.ToDate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                datePicker.MovementId = "1,2,3,4";
            }
            if (datePicker != null)
            {
                //ViewBag.Customer = (from c in db.InScanMasters
                //                    join cust in db.CustomerMasters on c.CustomerID equals cust.CustomerID
                //                    where (c.TransactionDate >= datePicker.FromDate && c.TransactionDate < datePicker.ToDate)
                //                    select new CustmorVM { CustomerID = cust.CustomerID, CustomerName = cust.CustomerName }).Distinct();

                ViewBag.Customer = (from c in db.CustomerMasters where c.StatusActive == true select new CustmorVM { CustomerID = c.CustomerID, CustomerName = c.CustomerName }).ToList();
                if (datePicker.MovementId==null)
                    datePicker.MovementId = "1,2,3,4";
            }
            else
            {
                ViewBag.Customer = new CustmorVM { CustomerID = 0, CustomerName = "" };
            }


            //ViewBag.Movement = new MultiSelectList(db.CourierMovements.ToList(),"MovementID","MovementType");
            ViewBag.Movement = db.CourierMovements.ToList();
            
            ViewBag.Token = datePicker;
            SessionDataModel.SetTableVariable(datePicker);
            return View(datePicker);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvoiceSearch([Bind(Include = "FromDate,ToDate,CustomerId,MovementId,SelectedValues,CustomerName")] DatePicker picker)
        {
            DatePicker model = new DatePicker
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                Delete = true, // (bool)Token.Permissions.Deletion,
                Update = true, //(bool)Token.Permissions.Updation,
                Create = true, //.ToStrin//(bool)Token.Permissions.Creation
                CustomerId = picker.CustomerId,
                MovementId = picker.MovementId,
                CustomerName=picker.CustomerName,
                SelectedValues = picker.SelectedValues
            };
            model.MovementId = "";
            if (picker.SelectedValues != null)
            {
                foreach (var item in picker.SelectedValues)
                {
                    if (model.MovementId == "")
                    {
                        model.MovementId = item.ToString();
                    }
                    else
                    {
                        model.MovementId = model.MovementId + "," + item.ToString();
                    }

                }
            }
            ViewBag.Token = model;
            SessionDataModel.SetTableVariable(model);
            return RedirectToAction("Create", "CustomerInvoice");
            //return PartialView("InvoiceSearch",model);

        }
        public ActionResult Table(CustomerInvoiceSearch model)
        {
            return PartialView("Table", model);
            //int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            //int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());

            //DatePicker datePicker = SessionDataModel.GetTableVariable();
            //ViewBag.Token = datePicker;

            //List<CustomerInvoiceVM> _Invoices = (from c in db.CustomerInvoices
            //                                     join cust in db.CustomerMasters on c.CustomerID equals cust.CustomerID
            //                                     where (c.InvoiceDate >= datePicker.FromDate && c.InvoiceDate < datePicker.ToDate)
            //                                     && c.IsDeleted==false
            //                                     orderby c.InvoiceDate descending
            //                                     select new CustomerInvoiceVM
            //                                     {
            //                                         CustomerInvoiceID = c.CustomerInvoiceID,
            //                                         CustomerInvoiceNo = c.CustomerInvoiceNo,
            //                                         InvoiceDate = c.InvoiceDate,
            //                                         CustomerID = c.CustomerID,
            //                                         CustomerName = cust.CustomerName,
            //                                         InvoiceTotal=c.InvoiceTotal

            //                                     }).ToList();

            //return View("Table", _Invoices);

        }
        public ActionResult Create()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            
            CustomerInvoiceVM _custinvoice = new CustomerInvoiceVM();
            PickupRequestDAO _dao = new PickupRequestDAO();                                    
            _custinvoice.InvoiceDate = CommanFunctions.GetCurrentDateTime();//  myDt;// DateTimeKind. DateTimeOffset.Now.UtcDateTime.AddHours(5.30); // DateTime.Now;            
            _custinvoice.CustomerInvoiceNo = _dao.GetMaxInvoiceNo(companyid, branchid,yearid);
            
            List<CustomerInvoiceDetailVM> _details = new List<CustomerInvoiceDetailVM>();
            _custinvoice.FromDate = CommanFunctions.GetFirstDayofMonth().Date; // DateTime.Now.Date;
            _custinvoice.ToDate = CommanFunctions.GetCurrentDateTime().Date; // DateTime.Now.Date;
            _custinvoice.MovementId = "1,2,3,4";
            _custinvoice.InvoiceTotal = 0;
                _custinvoice.ChargeableWT = 0;
                _custinvoice.CustomerInvoiceTax = 0;
                _custinvoice.OtherCharge = 0;
                _custinvoice.AdminPer = 0;
                _custinvoice.AdminAmt = 0;
                _custinvoice.FuelPer = 0;
                _custinvoice.FuelAmt = 0;
                _custinvoice.Discount = 0;                
              
                _custinvoice.InvoiceTotal = _custinvoice.TotalCharges;

            _custinvoice.CustomerInvoiceDetailsVM = _details;

            Session["InvoiceListing"] = _details;
            return View(_custinvoice);
        }


        [HttpPost]
        public ActionResult ShowItemList(int CustomerId, string FromDate, string ToDate,string MovementId)
        {
            CustomerInvoiceVM _custinvoice = new CustomerInvoiceVM();
            _custinvoice.CustomerInvoiceDetailsVM = PickupRequestDAO.GetCustomerAWBforInvoice(CustomerId, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate),MovementId);
            Session["InvoiceListing"] = _custinvoice.CustomerInvoiceDetailsVM;
            return PartialView("InvoiceList", _custinvoice);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerInvoiceVM model)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int companyId = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            var userid = Convert.ToInt32(Session["UserID"]);
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            if (model.CustomerInvoiceID == 0)
            {
                CustomerInvoice _custinvoice = new CustomerInvoice();
                var max = db.CustomerInvoices.Select(x => x.CustomerInvoiceID).DefaultIfEmpty(0).Max() + 1;
                _custinvoice.CustomerInvoiceID = max;
                _custinvoice.CustomerInvoiceNo = model.CustomerInvoiceNo;
                _custinvoice.InvoiceDate = model.InvoiceDate;
                _custinvoice.CustomerID = model.CustomerID;
                _custinvoice.CustomerInvoiceTax = model.CustomerInvoiceTax;
                _custinvoice.ChargeableWT = model.ChargeableWT;                                               
                _custinvoice.AdminPer = model.AdminPer;
                _custinvoice.AdminAmt = model.AdminAmt;
                _custinvoice.FuelPer = model.FuelPer;
                _custinvoice.FuelAmt = model.FuelAmt;
                _custinvoice.OtherCharge = model.OtherCharge;
                _custinvoice.InvoiceTotal = model.InvoiceTotal;
                _custinvoice.Discount = model.Discount;
                _custinvoice.AcFinancialYearID = yearid;                
                _custinvoice.AcCompanyID = companyId;
                _custinvoice.BranchID = branchid;
                _custinvoice.CreatedBy = userid;
                _custinvoice.CreatedDate = CommanFunctions.GetCurrentDateTime();
                _custinvoice.ModifiedBy = userid;
                _custinvoice.ModifiedDate = CommanFunctions.GetCurrentDateTime();
                db.CustomerInvoices.Add(_custinvoice);
                db.SaveChanges();

                List<CustomerInvoiceDetailVM> e_Details = model.CustomerInvoiceDetailsVM; //  Session["InvoiceListing"] as List<CustomerInvoiceDetailVM>;

                model.CustomerInvoiceDetailsVM = e_Details;

                if (model.CustomerInvoiceDetailsVM != null)
                {

                    foreach (var e_details in model.CustomerInvoiceDetailsVM)
                    {
                        if (e_details.CustomerInvoiceDetailID == 0 && e_details.AWBChecked)
                        {
                            CustomerInvoiceDetail _detail = new CustomerInvoiceDetail();
                            _detail.CustomerInvoiceDetailID = db.CustomerInvoiceDetails.Select(x => x.CustomerInvoiceDetailID).DefaultIfEmpty(0).Max() + 1;
                            _detail.CustomerInvoiceID = _custinvoice.CustomerInvoiceID;
                            _detail.AWBNo = e_details.AWBNo;
                            _detail.InscanID = e_details.InscanID;
                            _detail.StatusPaymentMode = e_details.StatusPaymentMode;
                            _detail.CourierCharge = e_details.CourierCharge;
                            _detail.CustomCharge = e_details.CustomCharge;
                            _detail.OtherCharge = e_details.OtherCharge;                            
                            _detail.NetValue =  e_details.NetValue;
                            _detail.VATAmount = e_details.VATAmount;
                            db.CustomerInvoiceDetails.Add(_detail);
                            db.SaveChanges();

                            //inscan invoice modified
                            InScanMaster _inscan = db.InScanMasters.Find(e_details.InscanID);
                            _inscan.InvoiceID = _custinvoice.CustomerInvoiceID;
                            db.Entry(_inscan).State = EntityState.Modified;
                            db.SaveChanges();


                        }

                    }
                }

                //Accounts Posting
                PickupRequestDAO _dao = new PickupRequestDAO();
                _dao.GenerateInvoicePosting(_custinvoice.CustomerInvoiceID);

                TempData["SuccessMsg"] = "You have successfully Saved the Customer Invoice";
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Customer = db.CustomerMasters.ToList();
            ViewBag.Movement = db.CourierMovements.ToList();
            var _invoice = db.CustomerInvoices.Find(id);
            CustomerInvoiceVM _custinvoice = new CustomerInvoiceVM();
            _custinvoice.CustomerInvoiceID = _invoice.CustomerInvoiceID;
            _custinvoice.InvoiceDate = _invoice.InvoiceDate;            
            _custinvoice.CustomerInvoiceNo = _invoice.CustomerInvoiceNo;
            _custinvoice.CustomerID = _invoice.CustomerID;
            _custinvoice.CustomerInvoiceTax = _invoice.CustomerInvoiceTax;
            _custinvoice.FuelPer = _invoice.FuelPer;
            _custinvoice.FuelAmt = _invoice.FuelAmt;
            _custinvoice.AdminPer = _invoice.AdminPer;
            _custinvoice.AdminAmt = _invoice.AdminAmt;
            _custinvoice.OtherCharge = _invoice.OtherCharge;
            _custinvoice.ChargeableWT = _invoice.ChargeableWT;
            _custinvoice.InvoiceTotal= _invoice.InvoiceTotal;
            _custinvoice.Discount = _invoice.Discount;
            _custinvoice.ClearingCharge = _invoice.ClearingCharge;
            List<CustomerInvoiceDetailVM> _details = new List<CustomerInvoiceDetailVM>();
            _details = (from c in db.CustomerInvoiceDetails
                        join ins in db.InScanMasters on  c.InscanID equals ins.InScanID
                        where c.CustomerInvoiceID == id
                        select new CustomerInvoiceDetailVM { CustomerInvoiceDetailID = c.CustomerInvoiceDetailID, CustomerInvoiceID = c.CustomerInvoiceID, AWBNo = c.AWBNo,AWBDateTime=ins.TransactionDate, CustomCharge=c.CustomCharge,
                            CourierCharge = c.CourierCharge, OtherCharge = c.OtherCharge, ConsigneeCountryName=ins.ConsigneeCountryName,ConsigneeName=ins.Consignee,
                            StatusPaymentMode = c.StatusPaymentMode, InscanID = c.InscanID ,AWBChecked=true, VATAmount=c.VATAmount,NetValue=c.NetValue }).ToList();

            int _index = 0;
         
            foreach (var item in _details)
            {
                _details[_index].TotalCharges = Convert.ToDecimal(_details[_index].CourierCharge) + Convert.ToDecimal(_details[_index].CustomCharge) + Convert.ToDecimal(_details[_index].OtherCharge);
                _custinvoice.TotalCharges += _details[_index].TotalCharges;
                _index++;
            }
            
            _custinvoice.CustomerInvoiceDetailsVM = _details;

            Session["InvoiceListing"] = _details;
            return View(_custinvoice);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerInvoiceVM model)
        {
            var userid = Convert.ToInt32(Session["UserID"]);

            if (model.CustomerInvoiceID > 0)
            {
                CustomerInvoice _custinvoice = new CustomerInvoice();
                _custinvoice = db.CustomerInvoices.Find(model.CustomerInvoiceID);                                                              
                _custinvoice.InvoiceDate = model.InvoiceDate;                
                _custinvoice.CustomerInvoiceTax = model.CustomerInvoiceTax;
                _custinvoice.ChargeableWT = model.ChargeableWT;
                _custinvoice.AdminPer = model.AdminPer;
                _custinvoice.AdminAmt = model.AdminAmt;
                _custinvoice.FuelPer = model.FuelPer;
                _custinvoice.FuelAmt = model.FuelAmt;
                _custinvoice.OtherCharge = model.OtherCharge;
                _custinvoice.InvoiceTotal = model.InvoiceTotal;                
                _custinvoice.ModifiedBy = userid;
                _custinvoice.ModifiedDate = CommanFunctions.GetCurrentDateTime();
                db.Entry(_custinvoice).State = EntityState.Modified;
                db.SaveChanges();

                List<CustomerInvoiceDetailVM> e_Details = model.CustomerInvoiceDetailsVM; //  Session["InvoiceListing"] as List<CustomerInvoiceDetailVM>;

                model.CustomerInvoiceDetailsVM = e_Details;

                if (model.CustomerInvoiceDetailsVM != null)
                {

                    foreach (var e_details in model.CustomerInvoiceDetailsVM)
                    {
                        if (e_details.CustomerInvoiceDetailID == 0 && e_details.AWBChecked)
                        {
                            CustomerInvoiceDetail _detail = new CustomerInvoiceDetail();
                            _detail.CustomerInvoiceDetailID = db.CustomerInvoiceDetails.Select(x => x.CustomerInvoiceDetailID).DefaultIfEmpty(0).Max() + 1;
                            _detail.CustomerInvoiceID = _custinvoice.CustomerInvoiceID;
                            _detail.AWBNo = e_details.AWBNo;
                            _detail.InscanID = e_details.InscanID;
                            _detail.StatusPaymentMode = e_details.StatusPaymentMode;
                            _detail.CourierCharge = e_details.CourierCharge;
                            _detail.CustomCharge = e_details.CustomCharge;
                            _detail.OtherCharge = e_details.OtherCharge;
                            db.CustomerInvoiceDetails.Add(_detail);
                            db.SaveChanges();

                            //inscan invoice modified
                            InScanMaster _inscan = db.InScanMasters.Find(e_details.InscanID);
                            _inscan.InvoiceID = _custinvoice.CustomerInvoiceID;
                            db.Entry(_inscan).State = EntityState.Modified;
                            db.SaveChanges();


                        }
                        else  if (e_details.CustomerInvoiceDetailID == 0 && e_details.AWBChecked)
                        {
                            //CustomerInvoiceDetail _detail = new CustomerInvoiceDetail();
                            //_detail = db.CustomerInvoiceDetails.Find(e_details.CustomerInvoiceID);                            
                            //_detail.CourierCharge = e_details.CourierCharge;
                            //_detail.CustomCharge = e_details.CustomCharge;
                            //_detail.OtherCharge = e_details.OtherCharge;
                            //db.CustomerInvoiceDetails.Add(_detail);
                            //db.SaveChanges();

                            ////inscan invoice modified
                            //InScanMaster _inscan = db.InScanMasters.Find(e_details.InscanID);
                            //_inscan.InvoiceID = _custinvoice.CustomerInvoiceID;
                            //db.Entry(_inscan).State = System.Data.EntityState.Modified;
                            //db.SaveChanges();
                        }
                        else if (e_details.CustomerInvoiceDetailID == 0 && e_details.AWBChecked==false)
                        {
                            CustomerInvoiceDetail _detail = new CustomerInvoiceDetail();
                            _detail = db.CustomerInvoiceDetails.Find(e_details.CustomerInvoiceID);
                            db.CustomerInvoiceDetails.Remove(_detail);
                            db.SaveChanges();
                            ////inscan invoice modified
                            InScanMaster _inscan = db.InScanMasters.Find(e_details.InscanID);
                            _inscan.InvoiceID = null;
                            db.Entry(_inscan).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                TempData["SuccessMsg"] = "You have successfully Updated the Customer Invoice";
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public ActionResult GetCustomerAWBList(int Id)
        {
            DatePicker datePicker = SessionDataModel.GetTableVariable();
            List<CustomerInvoiceDetailVM> _details = new List<CustomerInvoiceDetailVM>();
            _details = (from c in db.InScanMasters
                        where (c.TransactionDate >= datePicker.FromDate && c.TransactionDate < datePicker.ToDate)
                        && (c.InvoiceID == null || c.InvoiceID==0)
                        && c.PaymentModeId == 3 //account
                        && c.IsDeleted==false
                        && c.CustomerID == Id
                        select new CustomerInvoiceDetailVM
                        {
                            AWBNo = c.AWBNo,
                            AWBDateTime=c.TransactionDate,
                            ConsigneeName = c.Consignee,
                            ConsigneeCountryName = c.ConsigneeCountryName,
                            CourierCharge = c.CourierCharge,
                            CustomCharge = c.CustomsValue == null ? 0 : c.CustomsValue,
                            OtherCharge = c.OtherCharge == null ? 0 : c.OtherCharge,
                            VATAmount =c.TaxAmount ==null ?0 :c.TaxAmount,
                            //StatusPaymentMode = c.StatusPaymentMode,
                            InscanID = c.InScanID,
                            MovementId = c.MovementID == null ? 0 : c.MovementID.Value,AWBChecked=true
                        }).ToList().Where(tt => tt.MovementId != null).ToList().Where(cc => datePicker.SelectedValues.ToList().Contains(cc.MovementId.Value)).ToList();
            
          

            int _index = 0;
            CustomerInvoiceVM customerInvoice = new CustomerInvoiceVM();
            foreach (var item in _details)
                {
                    _details[_index].TotalCharges = Convert.ToDecimal(_details[_index].CourierCharge) + Convert.ToDecimal(_details[_index].CustomCharge) + Convert.ToDecimal(_details[_index].OtherCharge);
                    customerInvoice.TotalCharges += _details[_index].TotalCharges;
                    _index++;
                }


            if (customerInvoice.CustomerInvoiceTax != 0)
            {
                customerInvoice.ChargeableWT = (Convert.ToDouble(customerInvoice.TotalCharges) * (Convert.ToDouble(customerInvoice.CustomerInvoiceTax) / Convert.ToDouble(100.00)));

                customerInvoice.AdminAmt = (Convert.ToDecimal(customerInvoice.TotalCharges) * (Convert.ToDecimal(customerInvoice.AdminPer) / Convert.ToDecimal(100)));

                customerInvoice.FuelAmt = (Convert.ToDecimal(customerInvoice.TotalCharges) * (Convert.ToDecimal(customerInvoice.FuelPer) / Convert.ToDecimal(100)));

                customerInvoice.InvoiceTotal = Convert.ToDecimal(customerInvoice.TotalCharges) + Convert.ToDecimal(customerInvoice.ChargeableWT) + customerInvoice.AdminAmt + customerInvoice.FuelAmt;
            }

            customerInvoice.CustomerInvoiceDetailsVM = _details;

            Session["InvoiceListing"] = _details;

            return PartialView("InvoiceList", customerInvoice);

        }

        [HttpGet]
        public JsonResult GetCourierType()
        {
            var lstcourier=db.CourierMovements.ToList();
            return Json(new { data = lstcourier }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetTotalCharge(CustomerInvoiceVM customerinvoice)
        {
            int _index = 0;
            List<CustomerInvoiceDetailVM> _details = customerinvoice.CustomerInvoiceDetailsVM;

            //List<CustomerInvoiceDetailVM> _details = Session["InvoiceListing"] as List<CustomerInvoiceDetailVM>;
            if (_details != null)
            {
                foreach (var item in _details)
                {
                    if (item.AWBChecked)
                    {

                        _details[_index].TotalCharges = Convert.ToDecimal(_details[_index].CourierCharge) + Convert.ToDecimal(_details[_index].CustomCharge) + Convert.ToDecimal(_details[_index].OtherCharge);
                        customerinvoice.TotalCharges += _details[_index].TotalCharges;
                    }
                    _index++;

                }
                if (customerinvoice.OtherCharge==null)
                {
                    customerinvoice.OtherCharge = 0;
                }
                if (customerinvoice.CustomerInvoiceTax==null)
                { 
                    customerinvoice.CustomerInvoiceTax = 0; 
                }

                if (customerinvoice.AdminPer ==null)
                {
                    customerinvoice.AdminPer = 0;
                }
                if (customerinvoice.FuelPer == null)
                {
                    customerinvoice.FuelPer = 0;
                }

                if (customerinvoice.CustomerInvoiceTax != 0)
                {
                    customerinvoice.ChargeableWT = (Convert.ToDouble(customerinvoice.TotalCharges) * (Convert.ToDouble(customerinvoice.CustomerInvoiceTax) / Convert.ToDouble(100.00)));
                }
                else
                {
                    customerinvoice.ChargeableWT = 0;
                }
                if (customerinvoice.AdminPer != 0) { 
                    customerinvoice.AdminAmt = (Convert.ToDecimal(customerinvoice.TotalCharges) * (Convert.ToDecimal(customerinvoice.AdminPer) / Convert.ToDecimal(100)));
                }
                else
                {
                    customerinvoice.AdminAmt = 0;
                }
            if (customerinvoice.FuelPer != 0)
            { 
                customerinvoice.FuelAmt = (Convert.ToDecimal(customerinvoice.TotalCharges) * (Convert.ToDecimal(customerinvoice.FuelPer) / Convert.ToDecimal(100)));
            }
            else
            {
                customerinvoice.FuelAmt = 0;
            }

                    customerinvoice.InvoiceTotal = Convert.ToDecimal(customerinvoice.TotalCharges) + Convert.ToDecimal(customerinvoice.ChargeableWT) + customerinvoice.AdminAmt + customerinvoice.FuelAmt + customerinvoice.OtherCharge;
                
            }
            return Json(new { data=customerinvoice  }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteConfirmed(int id)
        {
            //int k = 0;
            if (id != 0)
            {
                DataTable dt = ReceiptDAO.DeleteInvoice(id);
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

            //CustomerInvoice a = db.CustomerInvoices.Find(id);
            //if (a == null)  
            //{
            //    return HttpNotFound();
            //}
            //else
            //{
            //    var _inscans = db.InScanMasters.Where(cc => cc.InvoiceID == id).ToList();
            //    foreach(InScanMaster _inscan in _inscans)
            //    {
            //        _inscan.InvoiceID = null;
            //        db.Entry(_inscan).State = EntityState.Modified;
            //        db.SaveChanges();
            //    }
            //    a.IsDeleted = true;
            //    db.Entry(a).State = EntityState.Modified;
            //    db.SaveChanges();
            //    TempData["SuccessMsg"] = "You have successfully deleted Pickup Request.";


            //    return RedirectToAction("Index");
            //}
        }
        public ActionResult Details(int id)        
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            ViewBag.Customer = db.CustomerMasters.ToList();
            ViewBag.Movement = db.CourierMovements.ToList();
            var _invoice = db.CustomerInvoices.Find(id);
            CustomerInvoiceVM _custinvoice = new CustomerInvoiceVM();
            _custinvoice.CustomerInvoiceID = _invoice.CustomerInvoiceID;
            _custinvoice.InvoiceDate = _invoice.InvoiceDate;
            _custinvoice.CustomerInvoiceNo = _invoice.CustomerInvoiceNo;
            _custinvoice.CustomerID = _invoice.CustomerID;
            _custinvoice.CustomerName = db.CustomerMasters.Find(_invoice.CustomerID).CustomerName;
            _custinvoice.CustomerInvoiceTax = _invoice.CustomerInvoiceTax;
            _custinvoice.FuelPer = _invoice.FuelPer;
            _custinvoice.FuelAmt = _invoice.FuelAmt;
            _custinvoice.AdminPer = _invoice.AdminPer;
            _custinvoice.AdminAmt = _invoice.AdminAmt;
            _custinvoice.OtherCharge = _invoice.OtherCharge;
            _custinvoice.ChargeableWT = _invoice.ChargeableWT;
            _custinvoice.InvoiceTotal = _invoice.InvoiceTotal;
            
            string monetaryunit = Session["MonetaryUnit"].ToString();
            _custinvoice.InvoiceTotalInWords = NumberToWords.ConvertAmount(Convert.ToDouble(_custinvoice.InvoiceTotal), monetaryunit);

            var comp = db.AcCompanies.Find(companyid);
            _custinvoice.CurrencyName = db.CurrencyMasters.Find(comp.CurrencyID).Symbol;
            List<CustomerInvoiceDetailVM> _details = new List<CustomerInvoiceDetailVM>();
            _details = (from c in db.CustomerInvoiceDetails
                        join ins in db.InScanMasters on c.InscanID equals ins.InScanID
                        where c.CustomerInvoiceID == id
                        select new CustomerInvoiceDetailVM
                        {
                            CustomerInvoiceDetailID = c.CustomerInvoiceDetailID,
                            CustomerInvoiceID = c.CustomerInvoiceID,
                            AWBNo = c.AWBNo,
                            AWBDateTime = ins.TransactionDate,
                            CustomCharge = c.CustomCharge,
                            CourierCharge = c.CourierCharge,
                            OtherCharge = c.OtherCharge,
                            ConsigneeCountryName = ins.ConsigneeCountryName,
                            ConsigneeName = ins.Consignee,
                            //StatusPaymentMode = c.StatusPaymentMode,
                            InscanID = c.InscanID,
                            AWBChecked = true
                        }).ToList();

            int _index = 0;
            foreach (var item in _details)
            {
                _details[_index].TotalCharges = Convert.ToDecimal(_details[_index].CourierCharge) + Convert.ToDecimal(_details[_index].CustomCharge) + Convert.ToDecimal(_details[_index].OtherCharge);
                _custinvoice.TotalCharges += _details[_index].TotalCharges;
                _index++;
            }

            _custinvoice.CustomerInvoiceDetailsVM = _details;

            Session["InvoiceListing"] = _details;
            return View(_custinvoice);

        }

        public ActionResult InvoicePrint(int id)
        {
            ViewBag.ReportName = "Invoice Printing";
            LabelPrintingParam picker = SessionDataModel.GetLabelPrintParam();
            string monetaryunit = Session["MonetaryUnit"].ToString();
            AccountsReportsDAO.CustomerInvoiceReport(id, monetaryunit);
            //AccountsReportsDAO.CustomerTaxInvoiceReport(id, monetaryunit);
            return View();
        }


        public ActionResult MultipleInvoice()
        {
            return View();
        }
        public ActionResult InvoiceAll()
        {

            DatePicker datePicker = SessionDataModel.GetTableVariable();

            if (datePicker == null)
            {
                datePicker = new DatePicker();
                datePicker.FromDate = CommanFunctions.GetFirstDayofMonth().Date; // DateTime.Now.Date;
                datePicker.ToDate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                datePicker.MovementId = "1,2,3,4";
            }
            if (datePicker != null)
            {
               
                if (datePicker.MovementId == null)
                    datePicker.MovementId = "1,2,3,4";
            }
            else
            {
             
            }


            //ViewBag.Movement = new MultiSelectList(db.CourierMovements.ToList(),"MovementID","MovementType");
            ViewBag.Movement = db.CourierMovements.ToList();

            ViewBag.Token = datePicker;
            SessionDataModel.SetTableVariable(datePicker);
            return View(datePicker);

        }

        [HttpPost]        
        public JsonResult GenerateInvoice(InvoiceAllParam picker)
        {
            try
            {


                picker.MovementId = "";
                if (picker.SelectedValues != null)
                {
                    foreach (var item in picker.SelectedValues)
                    {
                        if (picker.MovementId == "")
                        {
                            picker.MovementId = item.ToString();
                        }
                        else
                        {
                            picker.MovementId = picker.MovementId + "," + item.ToString();
                        }

                    }
                }


                PickupRequestDAO _dao = new PickupRequestDAO();
                if (picker.InvoiceType == "Customer")
                {
                    _dao.GenerateCustomerInvoiceAll(picker);
                    return Json(new { status = "ok", message = "Customer Invoice Generated Successfully!" }, JsonRequestBehavior.AllowGet);
                }
                else if (picker.InvoiceType== "COLoader")
                {
                    _dao.GenerateCOInvoiceAll(picker);
                    return Json(new { status = "ok", message = "CO Loader Generated Successfully!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "Failed", message = "Invoice Generation Failed"}, JsonRequestBehavior.AllowGet);
                     
                }

            }
            catch(Exception ex)
            {
                return Json(new { status="Failed", message= ex.Message}, JsonRequestBehavior.AllowGet);
            }
            

        }

        
       
    }
}
