using System;
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
    public class COLoaderInvoiceController : Controller
    {
        Entities1 db = new Entities1();



        public ActionResult Index()
        {

            AgentInvoiceSearch obj = (AgentInvoiceSearch)Session["AgentInvoiceSearch"];
            AgentInvoiceSearch model = new AgentInvoiceSearch();
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
                obj = new AgentInvoiceSearch(); 
                obj.FromDate = pFromDate;
                obj.ToDate = pToDate;
                obj.InvoiceNo = "";
                Session["AgentInvoiceSearch"] = obj;
                model.FromDate = pFromDate;
                model.ToDate = pToDate;
                model.InvoiceNo = "";
            }
            else
            {
                model = obj;
            }
            List<AgentInvoiceVM> lst = PickupRequestDAO.GetAgentInvoiceList(obj.FromDate, obj.ToDate,model.InvoiceNo, yearid);
            model.Details = lst;
            
            return View(model);


        }
        [HttpPost]
        public ActionResult Index(AgentInvoiceSearch obj)
        {
            Session["AgentInvoiceSearch"] = obj;
            return RedirectToAction("Index");
        }
       
        public ActionResult InvoiceSearch()
        {

            AgentDatePicker datePicker =(AgentDatePicker)Session["AgentDatePicker"];

            if (datePicker == null)
            {
                datePicker = new AgentDatePicker();
                datePicker.FromDate = CommanFunctions.GetFirstDayofMonth().Date; // DateTime.Now.Date;
                datePicker.ToDate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                datePicker.MAWB = "";
            }
            if (datePicker != null)
            {                
                
            }
            else
            {
                ViewBag.Customer = new CustmorVM { CustomerID = 0, CustomerName = "" };
            }                                   
            
            
            Session["AgentDatePicker"] =datePicker;
            return View(datePicker);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvoiceSearch(AgentDatePicker picker)
        {
            AgentDatePicker model = new AgentDatePicker
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),                
                CustomerId = picker.CustomerId,
                //MovementId = picker.MovementId,
                CustomerName=picker.CustomerName,
                //SelectedValues = picker.SelectedValues
            };
            
            ViewBag.Token = model;
            Session["AgentDatePicker"] = model;
            return RedirectToAction("Create", "COLoaderInvoice");
           

        }
        public ActionResult Table(AgentInvoiceSearch model)
        {
            return PartialView("Table", model);            

        }
        public ActionResult Create()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            DatePicker datePicker = SessionDataModel.GetTableVariable();
            ViewBag.Token = datePicker;
            ViewBag.Movement = db.CourierMovements.ToList();
            if (datePicker != null)
            {
                ViewBag.Customer = (from c in db.InScanMasters
                                    join cust in db.CustomerMasters on c.CustomerID equals cust.CustomerID
                                    where (c.TransactionDate >= datePicker.FromDate && c.TransactionDate < datePicker.ToDate)
                                    select new CustmorVM { CustomerID = cust.CustomerID, CustomerName = cust.CustomerName }).Distinct();

            }

            AgentInvoiceVM _custinvoice = new AgentInvoiceVM();
            PickupRequestDAO _dao = new PickupRequestDAO();
            DateTime saveNow = DateTime.Now;
            DateTime myDt;
            myDt = DateTime.SpecifyKind(saveNow, DateTimeKind.Unspecified);

            _custinvoice.InvoiceDate = myDt;// DateTimeKind. DateTimeOffset.Now.UtcDateTime.AddHours(5.30); // DateTime.Now;            
            _custinvoice.InvoiceNo = _dao.GetMaxAgentInvoiceNo(companyid, branchid,yearid);
            
            List<AgentInvoiceDetailVM> _details = new List<AgentInvoiceDetailVM>();
            _custinvoice.Details = _details;

                      
                        Session["AgentInvoiceListing"] = _details;
            return View(_custinvoice);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AgentInvoiceVM model)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int companyId = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            var userid = Convert.ToInt32(Session["UserID"]);
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            if (model.AgentInvoiceID == 0)
            {
               AgentInvoice _custinvoice = new AgentInvoice();
                
                
                _custinvoice.InvoiceNo = model.InvoiceNo;
                _custinvoice.InvoiceDate = model.InvoiceDate;
                _custinvoice.CustomerID = model.CustomerID;
                _custinvoice.InvoiceTax = model.InvoiceTax;
                _custinvoice.AcFinancialYearID = yearid;                
                _custinvoice.AcCompanyID = companyId;
                _custinvoice.BranchID = branchid;
                _custinvoice.CreatedBy = userid;
                _custinvoice.AdminAmt = model.AdminAmt;
                _custinvoice.FuelAmt = model.FuelAmt;
                _custinvoice.OtherCharge = model.OtherCharge;
                _custinvoice.InvoiceTax = model.InvoiceTax;
                _custinvoice.CreatedDate = CommanFunctions.GetCurrentDateTime();
                _custinvoice.ModifiedBy = userid;
                _custinvoice.ModifiedDate = CommanFunctions.GetCurrentDateTime();
                db.AgentInvoices.Add(_custinvoice);
                db.SaveChanges();                

                if (model.Details != null)
                {

                    foreach (var e_details in model.Details)
                    {
                        if (e_details.AgentInvoiceDetailID == 0 && e_details.AWBChecked)
                        {
                            AgentInvoiceDetail _detail = new AgentInvoiceDetail();
                            _detail.AgentInvoiceID = _custinvoice.AgentInvoiceID;
                            _detail.AWBNo = e_details.AWBNo;
                            _detail.InscanID = e_details.InscanID;
                            _detail.StatusPaymentMode = e_details.StatusPaymentMode;
                            _detail.CourierCharge = e_details.CourierCharge;
                            _detail.CustomCharge = e_details.CustomCharge;
                            _detail.OtherCharge = e_details.OtherCharge;
                            _detail.NetValue = e_details.NetValue;
                            db.AgentInvoiceDetails.Add(_detail);
                            db.SaveChanges();

                            //inscan invoice modified
                            InScanMaster _inscan = db.InScanMasters.Find(e_details.InscanID);
                            _inscan.InvoiceID = _custinvoice.AgentInvoiceID;
                            db.Entry(_inscan).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                }

                //Accounts Posting
                PickupRequestDAO _dao = new PickupRequestDAO();
                _dao.GenerateCOLoaderPosting(_custinvoice.AgentInvoiceID);

                TempData["SuccessMsg"] = "You have successfully Saved the CO Loader Invoice";
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult ShowItemList(int CustomerId,string FromDate,string ToDate,string MAWB)
        {
            AgentInvoiceVM vm = new AgentInvoiceVM();
            vm.Details = PickupRequestDAO.GetAgentShipmentList(CustomerId, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), MAWB);
            return PartialView("InvoiceList", vm);

        }
            public ActionResult Edit(int id)
        {
            ViewBag.Customer = db.CustomerMasters.ToList();
            ViewBag.Movement = db.CourierMovements.ToList();
            var _invoice = db.AgentInvoices.Find(id);
            AgentInvoiceVM _custinvoice = new AgentInvoiceVM();
            _custinvoice.AgentInvoiceID = _invoice.AgentInvoiceID;
            _custinvoice.InvoiceDate = _invoice.InvoiceDate;            
            _custinvoice.InvoiceNo = _invoice.InvoiceNo;
            _custinvoice.CustomerID = _invoice.CustomerID;
            _custinvoice.InvoiceTax = _invoice.InvoiceTax;
            _custinvoice.FuelPer = _invoice.FuelPer;
            _custinvoice.FuelAmt = _invoice.FuelAmt;
            _custinvoice.AdminPer = _invoice.AdminPer;
            _custinvoice.AdminAmt = _invoice.AdminAmt;
            _custinvoice.OtherCharge = _invoice.OtherCharge;
            _custinvoice.ChargeableWT = _invoice.ChargeableWT;
            _custinvoice.InvoiceTotal= _invoice.InvoiceTotal;
                       
            List<AgentInvoiceDetailVM> _details = new List<AgentInvoiceDetailVM>();
            _details = (from c in db.AgentInvoiceDetails
                        join ins in db.InScanMasters on  c.InscanID equals ins.InScanID
                        where c.AgentInvoiceID == id
                        select new AgentInvoiceDetailVM { AgentInvoiceDetailID = c.AgentInvoiceDetailID, AgentInvoiceID = c.AgentInvoiceID, AWBNo = c.AWBNo,AWBDateTime=ins.TransactionDate, CustomCharge=c.CustomCharge,
                            CourierCharge = c.CourierCharge, OtherCharge = c.OtherCharge, ConsigneeCountryName=ins.ConsigneeCountryName,ConsigneeName=ins.Consignee,
                            StatusPaymentMode = c.StatusPaymentMode, InscanID = c.InscanID ,AWBChecked=true }).ToList();

            int _index = 0;
         
            foreach (var item in _details)
            {
                _details[_index].NetValue = Convert.ToDecimal(_details[_index].CourierCharge) + Convert.ToDecimal(_details[_index].CustomCharge) + Convert.ToDecimal(_details[_index].OtherCharge);
                _custinvoice.InvoiceTotal += _details[_index].NetValue;
                _index++;
            }
            
            _custinvoice.Details = _details;

            Session["InvoiceListing"] = _details;
            return View(_custinvoice);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AgentInvoiceVM model)
        {
            var userid = Convert.ToInt32(Session["UserID"]);

            if (model.AgentInvoiceID > 0)
            {
                AgentInvoice _custinvoice = new AgentInvoice();
                _custinvoice = db.AgentInvoices.Find(model.AgentInvoiceID);                                                              
                _custinvoice.InvoiceDate = model.InvoiceDate;                
                _custinvoice.InvoiceTax = model.InvoiceTax;
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

                List<AgentInvoiceDetailVM> e_Details = model.Details; //  Session["InvoiceListing"] as List<CustomerInvoiceDetailVM>;
                                

                if (model.Details != null)
                {

                    foreach (var e_details in model.Details )
                    {
                        if (e_details.AgentInvoiceDetailID == 0 && e_details.AWBChecked)
                        {
                            AgentInvoiceDetail _detail = new AgentInvoiceDetail();
                            _detail.AgentInvoiceDetailID = db.AgentInvoiceDetails.Select(x => x.AgentInvoiceDetailID).DefaultIfEmpty(0).Max() + 1;
                            _detail.AgentInvoiceID = _custinvoice.AgentInvoiceID;
                            _detail.AWBNo = e_details.AWBNo;
                            _detail.InscanID = e_details.InscanID;
                            _detail.StatusPaymentMode = e_details.StatusPaymentMode;
                            _detail.CourierCharge = e_details.CourierCharge;
                            _detail.CustomCharge = e_details.CustomCharge;
                            _detail.OtherCharge = e_details.OtherCharge;
                            db.AgentInvoiceDetails.Add(_detail);
                            db.SaveChanges();

                            //inscan invoice modified
                            InScanMaster _inscan = db.InScanMasters.Find(e_details.InscanID);
                            _inscan.InvoiceID = _custinvoice.AgentInvoiceID;
                            db.Entry(_inscan).State = EntityState.Modified;
                            db.SaveChanges();


                        }
                        else  if (e_details.AgentInvoiceDetailID > 0 && e_details.AWBChecked)
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
                        
                    }
                }
                TempData["SuccessMsg"] = "You have successfully Updated the CO Loader Invoice";
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
            ViewBag.Customer = db.CustomerMasters.ToList();
            ViewBag.Movement = db.CourierMovements.ToList();
            var _invoice = db.AgentInvoices.Find(id);
            AgentInvoiceVM _custinvoice = new AgentInvoiceVM();
            _custinvoice.AgentInvoiceID = _invoice.AgentInvoiceID;
            _custinvoice.InvoiceDate = _invoice.InvoiceDate;
            _custinvoice.InvoiceNo = _invoice.InvoiceNo;
            _custinvoice.CustomerID = _invoice.CustomerID;
            _custinvoice.InvoiceTax = _invoice.InvoiceTax;
            _custinvoice.FuelPer = _invoice.FuelPer;
            _custinvoice.FuelAmt = _invoice.FuelAmt;
            _custinvoice.AdminPer = _invoice.AdminPer;
            _custinvoice.AdminAmt = _invoice.AdminAmt;
            _custinvoice.OtherCharge = _invoice.OtherCharge;
            _custinvoice.ChargeableWT = _invoice.ChargeableWT;
            _custinvoice.InvoiceTotal = _invoice.InvoiceTotal;

            List<AgentInvoiceDetailVM> _details = new List<AgentInvoiceDetailVM>();
            _details = (from c in db.AgentInvoiceDetails
                        join ins in db.InScanMasters on c.InscanID equals ins.InScanID
                        where c.AgentInvoiceID == id
                        select new AgentInvoiceDetailVM
                        {
                            AgentInvoiceDetailID = c.AgentInvoiceDetailID,
                            AgentInvoiceID = c.AgentInvoiceID,
                            AWBNo = c.AWBNo,
                            AWBDateTime = ins.TransactionDate,
                            CustomCharge = c.CustomCharge,
                            CourierCharge = c.CourierCharge,
                            OtherCharge = c.OtherCharge,
                            ConsigneeCountryName = ins.ConsigneeCountryName,
                            ConsigneeName = ins.Consignee,
                            StatusPaymentMode = c.StatusPaymentMode,
                            InscanID = c.InscanID,
                            AWBChecked = true
                        }).ToList();

            int _index = 0;

            foreach (var item in _details)
            {
                _details[_index].NetValue = Convert.ToDecimal(_details[_index].CourierCharge) + Convert.ToDecimal(_details[_index].CustomCharge) + Convert.ToDecimal(_details[_index].OtherCharge);
                _custinvoice.InvoiceTotal += _details[_index].NetValue;
                _index++;
            }

            _custinvoice.Details = _details;

            Session["InvoiceListing"] = _details;
            return View(_custinvoice);            

        }

        public ActionResult InvoicePrint(int id,string output="PDF")
        {
            ViewBag.ReportName = "CO Loader Invoice Printing";              
            ViewBag.ReportId=id;
            string filepath=AccountsReportsDAO.COLoaderInvoiceReport(id,output);
            
            if (output != "PDF")
            {
                return RedirectToAction("DownloadFile", "COLoader", new { filePath = filepath });
            }
            else
            {
                return View();
            }            
        }
        //[HttpPost]
        //public ActionResult InvoicePrint([Bind(Include = "ReportId")] int ReportId)
        //{
        //   //string reportpath=AccountsReportsDAO.COLoaderInvoiceReport(id, "EXCEL");

        //    return RedirectToAction("InvoicePrint", "COLoader",new { id = ReportId, output = "EXCEL" });
        //    //return Json(new { data = reportpath }, JsonRequestBehavior.AllowGet);

        //}

        public FileResult DownloadFile(int id)
        {
            string filepath = AccountsReportsDAO.COLoaderInvoiceReport(id, "EXCEL");
            string filename = "AgentInvoiceReport_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx"; // Server.MapPath("~" + filePath);

            byte[] fileBytes = GetFile(filepath);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
        [HttpGet]
        public JsonResult GetCustomerName(string term)
        {
            var customerlist = (from c1 in db.CustomerMasters
                                where c1.CustomerID > 0 && c1.CustomerType == "CL" && c1.CustomerName.ToLower().StartsWith(term.ToLower())
                                orderby c1.CustomerName ascending
                                select new { CustomerID = c1.CustomerID, CustomerName = c1.CustomerName, CustomerType = c1.CustomerType }).Take(100).ToList();

            return Json(customerlist, JsonRequestBehavior.AllowGet);

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
                _dao.GenerateInvoiceAll(picker);
                return Json(new {  status="ok", message="Invoice Generated Successfully!"}, JsonRequestBehavior.AllowGet);

            }
            catch(Exception ex)
            {
                return Json(new { status="Failed", message= ex.Message}, JsonRequestBehavior.AllowGet);
            }
            

        }

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult AddOrRemoveAWBNo(CustomerInvoiceVM _CustomerInvoice, int? i)
        //{
        //    var PrevInvoiceListSession = Session["InvoiceListing"] as List<CustomerInvoiceDetailVM>;

        //    if (i.HasValue) //delete mode
        //    {
        //        if (PrevInvoiceListSession == null)
        //        {

        //        }
        //        else
        //        {
        //            if (_CustomerInvoice.CustomerInvoiceDetailsVM == null)
        //            {
        //                _CustomerInvoice.CustomerInvoiceDetailsVM = new List<CustomerInvoiceDetailVM>();
        //            }
        //            int index = 0;
        //            if (_CustomerInvoice.CustomerInvoiceDetailsVM.Count != PrevInvoiceListSession.Count)
        //            {
        //                foreach (var item in PrevInvoiceListSession)
        //                {
        //                    if (i == index)
        //                    {
        //                        int ii = Convert.ToInt32(i);
        //                        _CustomerInvoice.CustomerInvoiceDetailsVM.Add(item);
        //                        if (_CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked == false)
        //                            _CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked = true;
        //                        else
        //                            _CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked = true;

        //                    }
        //                    else
        //                    {
        //                        _CustomerInvoice.CustomerInvoiceDetailsVM.Add(item);
        //                    }
        //                    index++;
        //                }
        //            }
        //            else
        //            {
        //                foreach (var item in PrevInvoiceListSession)
        //                {
        //                    if (i == index)
        //                    {
        //                        int ii = Convert.ToInt32(i);
        //                        if (_CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked == false)
        //                            _CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked = true;
        //                        else
        //                            _CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked = true;

        //                        //_CustomerInvoice.CustomerInvoiceDetailsVM.Add(item);
        //                    }
        //                    index++;
        //                }
        //            }
        //        }

        //        //s_ImportShipment.Shipments.RemoveAt(i.Value);
        //        Session["InvoiceListing"] = _CustomerInvoice.CustomerInvoiceDetailsVM;
        //    }
        //    else
        //    {
        //        if (_CustomerInvoice.CustomerInvoiceDetailsVM == null)
        //        {
        //            _CustomerInvoice.CustomerInvoiceDetailsVM = new List<CustomerInvoiceDetailVM>();
        //        }
        //        var shipmentsession = Session["EShipmentdetails"] as ExportShipmentDetail;
        //        var Serialnumber = Convert.ToInt32(Session["EShipSerialNumber"]);
        //        var isupdate = Convert.ToBoolean(Session["EIsUpdate"]);
        //        if (PrevInvoiceListSession == null)
        //        {

        //        }
        //        else
        //        {
        //            foreach (var item in PrevInvoiceListSession)
        //            {
        //                _CustomerInvoice.CustomerInvoiceDetailsVM.Add(item);
        //            }
        //        }
        //        if (isupdate == true)
        //        {
        //            if (_CustomerInvoice.CustomerInvoiceDetailsVM.Count == 0)
        //            {
        //                //_CustomerInvoice.CustomerInvoiceDetailsVM.Add(shipmentsession);
        //            }
        //            else
        //            {
        //                // s_ImportShipment.Shipments[Serialnumber] = shipmentsession;
        //            }
        //            //s_ImportShipment.Shipments.RemoveAt(Serialnumber);                  

        //        }
        //        else
        //        {
        //            // s_ImportShipment.Shipments.Add(shipmentsession);
        //        }
        //        //Session["EShipmentdetails"] = new ExportShipmentDetail();
        //        //Session["EShipSerialNumber"] = "";
        //        Session["InvoiceUpdate"] = false;
        //        Session["InvoiceListing"] = _CustomerInvoice.CustomerInvoiceDetailsVM;
        //    }
        //    return PartialView("InvoiceList", _CustomerInvoice);
        //}
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public JsonResult AddOrRemoveAWBNo1(CustomerInvoiceVM _CustomerInvoice, int? i)
        //{
        //    var PrevInvoiceListSession = Session["InvoiceListing"] as List<CustomerInvoiceDetailVM>;

        //    if (i.HasValue) //delete mode
        //    {
        //        if (PrevInvoiceListSession == null)
        //        {

        //        }
        //        else
        //        {
        //            if (_CustomerInvoice.CustomerInvoiceDetailsVM == null)
        //            {
        //                _CustomerInvoice.CustomerInvoiceDetailsVM = new List<CustomerInvoiceDetailVM>();
        //            }
        //            int index = 0;
        //            if (_CustomerInvoice.CustomerInvoiceDetailsVM.Count != PrevInvoiceListSession.Count)
        //            {
        //                foreach (var item in PrevInvoiceListSession)
        //                {
        //                    if (i == index)
        //                    {
        //                        int ii = Convert.ToInt32(i);
        //                        _CustomerInvoice.CustomerInvoiceDetailsVM.Add(item);
        //                        if (_CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked == false)
        //                            _CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked = true;
        //                        else
        //                            _CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked = false;

        //                    }
        //                    else
        //                    {
        //                        _CustomerInvoice.CustomerInvoiceDetailsVM.Add(item);
        //                    }
        //                    index++;
        //                }
        //            }
        //            else
        //            {
        //                foreach (var item in PrevInvoiceListSession)
        //                {
        //                    if (i == index)
        //                    {
        //                        int ii = Convert.ToInt32(i);
        //                        if (_CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked == false)
        //                            _CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked = true;
        //                        else
        //                            _CustomerInvoice.CustomerInvoiceDetailsVM[ii].AWBChecked = true;

        //                        //_CustomerInvoice.CustomerInvoiceDetailsVM.Add(item);
        //                    }
        //                    index++;
        //                }
        //            }
        //        }

        //        //s_ImportShipment.Shipments.RemoveAt(i.Value);
        //        Session["InvoiceListing"] = _CustomerInvoice.CustomerInvoiceDetailsVM;
        //    }

        //    return Json(new { data = "ok" }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult AddOrRemoveAWBAll(CustomerInvoiceVM _CustomerInvoice, int i)
        //{
        //    var PrevInvoiceListSession = Session["InvoiceListing"] as List<CustomerInvoiceDetailVM>;

        //    bool status = false;
        //    if (i == 1)
        //        status = true;

        //    if (PrevInvoiceListSession == null)
        //    {

        //    }
        //    else
        //    {
        //        if (_CustomerInvoice.CustomerInvoiceDetailsVM == null)
        //        {
        //            _CustomerInvoice.CustomerInvoiceDetailsVM = new List<CustomerInvoiceDetailVM>();
        //        }
        //        int index = 0;
        //        if (_CustomerInvoice.CustomerInvoiceDetailsVM.Count != PrevInvoiceListSession.Count)
        //        {
        //            foreach (var item in PrevInvoiceListSession)
        //            {


        //                _CustomerInvoice.CustomerInvoiceDetailsVM.Add(item);
        //                _CustomerInvoice.CustomerInvoiceDetailsVM[index].AWBChecked = status;
        //                index++;
        //            }
        //        }
        //        else
        //        {
        //            foreach (var item in PrevInvoiceListSession)
        //            {

        //                _CustomerInvoice.CustomerInvoiceDetailsVM[index].AWBChecked = true;
        //                index++;
        //            }
        //        }
        //    }

        //    //s_ImportShipment.Shipments.RemoveAt(i.Value);
        //    Session["InvoiceListing"] = _CustomerInvoice.CustomerInvoiceDetailsVM;
        //    return Json(new { data = "ok" }, JsonRequestBehavior.AllowGet);
        //}
    }
}
