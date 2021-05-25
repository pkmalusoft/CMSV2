using CMSV2.DAL;
using CMSV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSV2.Controllers
{
    [SessionExpire]
    public class ReportsController :Controller
    {
        Entities1 db = new Entities1();
        #region "Empost"
        public ActionResult EmposFeeReport()
        {
            ViewBag.ReportName = "Empost Fee Analysis Report";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("EmpostFee_"))
                {
                    Session["ReportOutput"] = null;
                }
            }
            return View();
        }
        public ActionResult ReportFrame()
        {
            if (Session["ReportOutput"] != null)
                ViewBag.ReportOutput = Session["ReportOutput"].ToString();
            else
            {
                string reportpath = AccountsReportsDAO.GenerateDefaultReport();
                ViewBag.ReportOutput = reportpath; // "~/Reports/DefaultReport.pdf";
            }
            return PartialView();
        }
        public ActionResult PrintSearch()
        {
            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            DateTime pFromDate;
            DateTime pToDate;

            if (reportparam == null)
            {
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                pToDate = CommanFunctions.GetLastDayofMonth().Date;
                reportparam = new AccountsReportParam();
                reportparam.FromDate = pFromDate;
                reportparam.ToDate = pToDate;
                reportparam.AcHeadId = 0;
                reportparam.AcHeadName = "";
                reportparam.Output = "PDF";
            }
            else
            {
                if (reportparam.FromDate.Date.ToString() == "01-01-0001 00:00:00" || reportparam.FromDate.Date.ToString() == "01-01-0001")
                {
                    pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                    reportparam.FromDate = pFromDate;
                    reportparam.Output = "PDF";
                }

            }
                        
            SessionDataModel.SetAccountsParam(reportparam);

            return View(reportparam);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrintSearch([Bind(Include = "FromDate,ToDate,Output")] AccountsReportParam picker)
        {
            AccountsReportParam model = new AccountsReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate,
                Output = picker.Output
            };


            ViewBag.Token = model;
            SessionDataModel.SetAccountsParam(model);
            
            AccountsReportsDAO.GenerateEmposFeeReport();
            return RedirectToAction("EmposFeeReport", "Reports");


        }

        #endregion

        #region "AWBRegister"
        public ActionResult AWBRegister()
        {
            ViewBag.ReportName = "AirWay Bill Register";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("AWBRegister_"))
                {
                    Session["ReportOutput"] = null;
                }
            }
           
            return View();
        }


        public ActionResult AWBReportParam()
        {
            AWBReportParam reportparam = SessionDataModel.GetAWBReportParam();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            DateTime pFromDate;
            DateTime pToDate;

            if (reportparam == null)
            {
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                pToDate = CommanFunctions.GetLastDayofMonth().Date;
                reportparam = new AWBReportParam();
                reportparam.FromDate = pFromDate;
                reportparam.ToDate = pToDate;
                reportparam.ParcelTypeId = 0;
                reportparam.PaymentModeId= 0;
                reportparam.MovementId = "1,2,3,4";
                reportparam.Output = "PDF";
                reportparam.SortBy = "Date Wise";
                reportparam.ReportType = "Date";
            }
            else
            {
                if (reportparam.FromDate.Date.ToString() == "01-01-0001 00:00:00")
                {
                    pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                    reportparam.FromDate = pFromDate;
                    reportparam.Output = "PDF";
                }
                else
                {

                }

            }

            SessionDataModel.SetAWBReportParam(reportparam);
            ViewBag.PaymentMode = db.tblPaymentModes.ToList();
            ViewBag.parceltype = db.ParcelTypes.ToList();
            ViewBag.Movement = db.CourierMovements.ToList();
            return View(reportparam);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AWBReportParam([Bind(Include = "FromDate,ToDate,PaymentModeId,SelectedValues,MovementId,ParcelTypeId,Output,ReportType,SortBy")] AWBReportParam picker)
        {
            AWBReportParam model = new AWBReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate,
                PaymentModeId=picker.PaymentModeId,
                ParcelTypeId=picker.ParcelTypeId,
                MovementId =picker.MovementId,
                Output = picker.Output,
                ReportType=picker.ReportType,
                SortBy =picker.SortBy
                
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
            SessionDataModel.SetAWBReportParam(model);

            AccountsReportsDAO.GenerateAWBRegister();
            return RedirectToAction("AWBRegister", "Reports");


        }


        #endregion

        #region "TaxRegister"
        public ActionResult TaxRegister()
        {
            ViewBag.ReportName = "Tax Register";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("TaxRegister_"))
                {
                    Session["ReportOutput"] = null;
                }
            }

            return View();
        }


        public ActionResult TaxReportParam()
        {
            TaxReportParam reportparam = SessionDataModel.GetTaxReportParam();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            DateTime pFromDate;
            DateTime pToDate;

            if (reportparam == null)
            {
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                pToDate = CommanFunctions.GetLastDayofMonth().Date;
                reportparam = new TaxReportParam();
                reportparam.FromDate = pFromDate;
                reportparam.ToDate = pToDate;
                reportparam.Output = "PDF";
                reportparam.SortBy = "NonZeroTax";
                reportparam.ReportType = "Date";
            }
            else
            {
                if (reportparam.FromDate.Date.ToString() == "01-01-0001 00:00:00" || reportparam.FromDate.Date.ToString() == "01-01-0001")
                {
                    pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                    reportparam.FromDate = pFromDate;
                    reportparam.Output = "PDF";
                }
                else
                {

                }

            }

            SessionDataModel.SetTaxReportParam(reportparam);
            List<VoucherTypeVM> lsttype = new List<VoucherTypeVM>();
            lsttype.Add(new VoucherTypeVM { TypeName = "Receipts & Payments" });
            lsttype.Add(new VoucherTypeVM { TypeName = "Receipts" });
            lsttype.Add(new VoucherTypeVM { TypeName = "Payments" });
            //var typeitems = (from c in db.AcJournalMasters select new VoucherTypeVM { TypeName = c.VoucherType }).Distinct().ToList();
            //foreach (VoucherTypeVM Item in typeitems)
            //{
            //    lsttype.Add(Item);
            //}
            
            ViewBag.VoucherTypes = lsttype;
            return View(reportparam);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaxReportParam([Bind(Include = "FromDate,ToDate,TransactionType,Output,ReportType,SortBy")] TaxReportParam picker)
        {
            TaxReportParam model = new TaxReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate,
                TransactionType= picker.TransactionType,                
                Output = picker.Output,
                ReportType = picker.ReportType,
                SortBy = picker.SortBy

            };            

            ViewBag.Token = model;
            SessionDataModel.SetTaxReportParam(model);

            AccountsReportsDAO.GenerateTaxRegister();
            return RedirectToAction("TaxRegister", "Reports");


        }


        #endregion

        #region "CustomerLedger"
        public ActionResult CustomerLedger()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            CustomerLedgerReportParam model = SessionDataModel.GetCustomerLedgerReportParam();
            if (model == null)
            {
                model = new CustomerLedgerReportParam
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date,
                    AsonDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    CustomerId = 0,
                    CustomerName = "",
                    Output = "PDF",
                    ReportType = "Ledger"
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommanFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
            }
            SessionDataModel.SetCustomerLedgerParam(model);

            model.FromDate = AccountsDAO.CheckParamDate(model.FromDate, yearid).Date;
            model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Customer Ledger";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("CustomerLedger") && model.ReportType == "Ledger")
                {
                    Session["ReportOutput"] = null;
                }
                else if (!currentreport.Contains("CustomerOutStanding") && model.ReportType == "OutStanding")
                {
                    Session["ReportOutput"] = null;
                }
            }

            return View(model);

        }

        [HttpPost]
        public ActionResult CustomerLedger(CustomerLedgerReportParam picker)
        {

            CustomerLedgerReportParam model = new CustomerLedgerReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                CustomerId = picker.CustomerId,
                CustomerName = picker.CustomerName,
                Output =picker.Output,
                ReportType = picker.ReportType,
                AsonDate = picker.AsonDate
            };

            ViewBag.Token = model;
            SessionDataModel.SetCustomerLedgerParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            AccountsReportsDAO.GenerateCustomerLedgerDetailReport();

            return RedirectToAction("CustomerLedger", "Reports");


        }
        #endregion

        #region "CustomerOutstanding"
        public ActionResult CustomerOutstanding()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            CustomerLedgerReportParam model = SessionDataModel.GetCustomerLedgerReportParam();
            if (model == null)
            {
                model = new CustomerLedgerReportParam
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date,
                    CustomerId = 0,
                    CustomerName = "",
                    Output = "PDF",
                    ReportType = "OutStanding"
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommanFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
            }
            SessionDataModel.SetCustomerLedgerParam(model);

            model.FromDate = AccountsDAO.CheckParamDate(model.FromDate, yearid).Date;
            model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Customer Outstanding";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("CustomerLedger") && model.ReportType == "Ledger")
                {
                    Session["ReportOutput"] = null;
                }
                else if (!currentreport.Contains("CustomerOutStanding") && model.ReportType == "OutStanding")
                {
                    Session["ReportOutput"] = null;
                }
            }

            return View(model);

        }

        [HttpPost]
        public ActionResult CustomerOutstanding(CustomerLedgerReportParam picker)
        {

            CustomerLedgerReportParam model = new CustomerLedgerReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                CustomerId = picker.CustomerId,
                CustomerName = picker.CustomerName,
                Output = picker.Output,
                ReportType = picker.ReportType
            };

            ViewBag.Token = model;
            SessionDataModel.SetCustomerLedgerParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            if (model.ReportType == "Ledger")
            {
                //AccountsReportsDAO.GenerateCustomerLedgerReport();
                AccountsReportsDAO.GenerateCustomerLedgerDetailReport();
            }
            else if (model.ReportType == "OutStanding")
            {
                AccountsReportsDAO.GenerateCustomerOutStandingReport();
            }
            else if (model.ReportType == "AWBUnAllocated")
            {
                AccountsReportsDAO.GenerateAWBOutStandingReport();
            }
            else if (model.ReportType == "AWBOutStanding")
            {
                AccountsReportsDAO.GenerateAWBUnInvoiced();
            }

            return RedirectToAction("CustomerOutstanding", "Reports");


        }
        #endregion

        #region "CustomerStatement"
        public ActionResult CustomerStatement()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            CustomerLedgerReportParam model = SessionDataModel.GetCustomerLedgerReportParam();
            if (model == null)
            {
                model = new CustomerLedgerReportParam
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date,
                    AsonDate = CommanFunctions.GetLastDayofMonth().Date, //.AddDays(-1);,
                    CustomerId = 0,
                    CustomerName = "",
                    Output = "PDF",
                    ReportType = "Ledger"
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommanFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
            }
            if (model.AsonDate.ToString() == "01-01-0001 00:00:00")
            {
                model.AsonDate = CommanFunctions.GetLastDayofMonth().Date;
            }
            SessionDataModel.SetCustomerLedgerParam(model);


            model.AsonDate = AccountsDAO.CheckParamDate(model.FromDate, yearid).Date;
            model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Customer Statement";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("CustomerStatement"))
                {
                    Session["ReportOutput"] = null;
                }

            }

            return View(model);

        }

        [HttpPost]
        public ActionResult CustomerStatement(CustomerLedgerReportParam picker)
        {

            CustomerLedgerReportParam model = new CustomerLedgerReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                CustomerId = picker.CustomerId,
                CustomerName = picker.CustomerName,
                Output = picker.Output,
                ReportType = picker.ReportType,
                AsonDate = picker.AsonDate
            };

            ViewBag.Token = model;
            SessionDataModel.SetCustomerLedgerParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            AccountsReportsDAO.GenerateCustomerStatementReport();

            return RedirectToAction("CustomerStatement", "Reports");


        }
        #endregion
        #region supplierledger
        public ActionResult SupplierLedger()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            var supplierMasterTypes = (from d in db.SupplierTypes select d).ToList();
            ViewBag.SupplierType = supplierMasterTypes;
            SupplierLedgerReportParam model = SessionDataModel.GetSupplierLedgerReportParam();
            if (model == null)
            {
                model = new SupplierLedgerReportParam
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    AsonDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date,
                    SupplierTypeId = 1,
                    SupplierId = 0,
                    SupplierName = "",
                    Output = "PDF",
                    ReportType = "Ledger"
                };
            }
            if (model.AsonDate.ToString() == "01-01-0001 00:00:00")
            {
                model.AsonDate = CommanFunctions.GetFirstDayofMonth().Date;
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommanFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
            }
            SessionDataModel.SetSupplierLedgerParam(model);

            model.FromDate = AccountsDAO.CheckParamDate(model.FromDate, yearid).Date;
            model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Supplier Ledger";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("SupplierLedger") && model.ReportType == "Ledger")
                {
                    Session["ReportOutput"] = null;
                }
                else if (!currentreport.Contains("CustomerOutStanding") && model.ReportType == "OutStanding")
                {
                    Session["ReportOutput"] = null;
                }
            }

            return View(model);

        }

        [HttpPost]
        public ActionResult SupplierLedger(SupplierLedgerReportParam picker)
        {

            SupplierLedgerReportParam model = new SupplierLedgerReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                AsonDate = picker.AsonDate,
                SupplierId = picker.SupplierId,
                SupplierName = picker.SupplierName,
                Output = picker.Output,
                ReportType = picker.ReportType
            };

            ViewBag.Token = model;
            SessionDataModel.SetSupplierLedgerParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            if (model.ReportType == "Ledger")
            {
                //AccountsReportsDAO.GenerateCustomerLedgerReport();
                AccountsReportsDAO.GenerateSupplierLedgerDetailReport();
            }
            else if (model.ReportType == "Statement")
            {
                AccountsReportsDAO.GenerateSupplierStatementDetailReport();
            }

            return RedirectToAction("SupplierLedger", "Reports");


        }
        #endregion


        #region "CustomerAging"
        public ActionResult CustomerAging()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            CustomerLedgerReportParam model = SessionDataModel.GetCustomerLedgerReportParam();
            if (model == null)
            {
                model = new CustomerLedgerReportParam
                {

                    AsonDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    CustomerId = 0,
                    CustomerName = "",
                    Output = "PDF",
                    ReportType = "Summary"
                };
            }
            if (model.AsonDate.ToString() == "01-01-0001 00:00:00")
            {
                model.AsonDate = CommanFunctions.GetLastDayofMonth().Date;
            }

            SessionDataModel.SetCustomerLedgerParam(model);

            model.AsonDate = AccountsDAO.CheckParamDate(model.FromDate, yearid).Date;
            ViewBag.ReportName = "Customer Aging Report";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("CustomerAging"))
                {
                    Session["ReportOutput"] = null;
                }
            }

            return View(model);

        }

        [HttpPost]
        public ActionResult CustomerAging(CustomerLedgerReportParam picker)
        {

            CustomerLedgerReportParam model = new CustomerLedgerReportParam
            {
                CustomerId = picker.CustomerId,
                CustomerName = picker.CustomerName,
                Output = picker.Output,
                ReportType = picker.ReportType,
                AsonDate = picker.AsonDate
            };

            ViewBag.Token = model;
            SessionDataModel.SetCustomerLedgerParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            AccountsReportsDAO.GenerateCustomerAgingReport();

            return RedirectToAction("CustomerAging", "Reports");


        }
        #endregion


        #region "SupplierAging"
        public ActionResult SupplierAging()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            var supplierMasterTypes = (from d in db.SupplierTypes select d).ToList();
            ViewBag.SupplierType = supplierMasterTypes;

            SupplierLedgerReportParam model = SessionDataModel.GetSupplierLedgerReportParam();
            if (model == null)
            {
                model = new SupplierLedgerReportParam
                {

                    AsonDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    SupplierId = 0,
                    SupplierName = "",
                    Output = "PDF",
                    ReportType = "Summary"
                };
            }
            if (model.AsonDate.ToString() == "01-01-0001 00:00:00")
            {
                model.AsonDate = CommanFunctions.GetLastDayofMonth().Date;
            }

            SessionDataModel.SetSupplierLedgerParam(model);

            model.AsonDate = AccountsDAO.CheckParamDate(model.FromDate, yearid).Date;
            ViewBag.ReportName = "Supplier Aging Report";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("SupplierAging"))
                {
                    Session["ReportOutput"] = null;
                }
            }

            return View(model);

        }

        [HttpPost]
        public ActionResult SupplierAging(SupplierLedgerReportParam picker)
        {

            SupplierLedgerReportParam model = new SupplierLedgerReportParam
            {
                SupplierId = picker.SupplierId,
                SupplierName = picker.SupplierName,
                Output = picker.Output,
                ReportType = picker.ReportType,
                AsonDate = picker.AsonDate
            };

            ViewBag.Token = model;
            SessionDataModel.SetSupplierLedgerParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            AccountsReportsDAO.GenerateSupplierAgingReport();

            return RedirectToAction("SupplierAging", "Reports");


        }
        #endregion

        #region "CODRegister & Pending"
        public ActionResult CODRegister()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            CODReportParam model = (CODReportParam)Session["CODReportParam"];
            if (model == null)
            {
                model = new CODReportParam 
                {

                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,                    
                    ToDate = CommanFunctions.GetLastDayofMonth().Date, //.AddDays(-1);,                    
                    Output = "PDF",
                    ReportType = "Detail",
                    MovementId="1,2,3,4"
                };
                Session["CODReportParam"] = model;
            }            
            
            ViewBag.ReportName = "COD Register Report";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("CODRegister") && !currentreport.Contains("CODSummary"))
                {
                    Session["ReportOutput"] = null;
                }
            }
            ViewBag.PaymentMode = db.tblPaymentModes.ToList();
            ViewBag.parceltype = db.ParcelTypes.ToList();
            ViewBag.Movement = db.CourierMovements.ToList();
            return View(model);

        }

        [HttpPost]
        public ActionResult CODRegister(CODReportParam picker)
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
            Session["CODReportParam"] = picker;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            if (picker.ReportType=="Detail")
                AccountsReportsDAO.GenerateCODRegister();
            else
                AccountsReportsDAO.GenerateCODSummary();

            return RedirectToAction("CODRegister", "Reports");
        }

        
        public ActionResult CODPending()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            CODReportParam model = (CODReportParam)Session["CODReportParam"];
            if (model == null)
            {
                model = new CODReportParam
                {

                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,                    
                    ToDate = CommanFunctions.GetLastDayofMonth().Date, //.AddDays(-1);,                    
                    Output = "PDF",
                    ReportType = "Detail",
                    MovementId = "1,2,3,4"
                };
                Session["CODReportParam"] = model;
            }

            ViewBag.ReportName = "COD Pending Report";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("CODPending"))
                {
                    Session["ReportOutput"] = null;
                }
            }
            ViewBag.PaymentMode = db.tblPaymentModes.ToList();
            ViewBag.parceltype = db.ParcelTypes.ToList();
            ViewBag.Movement = db.CourierMovements.ToList();
            return View(model);

        }

        [HttpPost]
        public ActionResult CODPending(CODReportParam picker)
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
            Session["CODReportParam"] = picker;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            AccountsReportsDAO.GenerateCODPending();

            return RedirectToAction("CODPending", "Reports");
        }



        #endregion

        #region "SalesRegistersummary"
        public ActionResult SalesRegisterSummary()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Employee = db.EmployeeMasters.ToList();
            SalesRegisterSummaryParam model = (SalesRegisterSummaryParam)Session["SalesRegisterSummaryParam"];
            if (model == null)
            {
                model = new SalesRegisterSummaryParam
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date,
                    EmployeeID = 0,
                    CustomerId = 0,
                    CustomerName = "",
                    Output = "PDF",
                    ReportType = "Ledger"
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommanFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
            }

            Session["SalesRegisterSummaryParam"] = model;

            //model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Sales Register Summary";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("SalesRegisterSummary"))
                {
                    Session["ReportOutput"] = null;
                }

            }

            return View(model);

        }

        [HttpPost]
        public ActionResult SalesRegisterSummary(SalesRegisterSummaryParam picker)
        {

            SalesRegisterSummaryParam model = new SalesRegisterSummaryParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                CustomerId = picker.CustomerId,
                CustomerName = picker.CustomerName,
                Output = picker.Output,
                EmployeeID = picker.EmployeeID,
                ReportType = picker.ReportType
            };

            ViewBag.Token = model;
            Session["SalesRegisterSummaryParam"] = model;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            AccountsReportsDAO.GenerateSalesRegisterSummaryReport();

            return RedirectToAction("SalesRegisterSummary", "Reports");


        }



        #endregion

        #region "SalesReport"
        public ActionResult SalesReport()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Employee = db.EmployeeMasters.ToList();
            SalesReportParam model = (SalesReportParam)Session["SalesReportParam"];
            if (model == null)
            {
                model = new SalesReportParam
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date,
                    EmployeeID = 0,
                    CustomerId = 0,
                    CustomerName = "",
                    Output = "PDF",
                    ReportType = "Ledger"
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommanFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
            }

            Session["SalesReportParam"] = model;

            //model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Sales Report";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("SalesReport"))
                {
                    Session["ReportOutput"] = null;
                }

            }

            return View(model);

        }

        [HttpPost]
        public ActionResult SalesReport(SalesReportParam picker)
        {

            SalesReportParam model = new SalesReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                CustomerId = picker.CustomerId,
                CustomerName = picker.CustomerName,
                Output = picker.Output,
                EmployeeID = picker.EmployeeID,
                ReportType = picker.ReportType
            };

            ViewBag.Token = model;
            Session["SalesReportParam"] = model;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            AccountsReportsDAO.GenerateSalesReport();

            return RedirectToAction("SalesReport", "Reports");


        }
        #endregion


        #region "MaterialCostReport"
        public ActionResult MaterialCostLedger()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Employee = db.EmployeeMasters.ToList();
            MaterialCostLedgerParam model = (MaterialCostLedgerParam)Session["MaterialCostLedgerParam"];
            if (model == null)
            {
                model = new MaterialCostLedgerParam  
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date,                     
                    Output = "PDF",
                    ReportType = "Ledger"
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommanFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
            }

            Session["MaterialCostLedgerParam"] = model;

            //model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Material Cost Ledger Report";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("MaterialCostLedger"))
                {
                    Session["ReportOutput"] = null;
                }

            }

            return View(model);

        }

        [HttpPost]
        public ActionResult MaterialCostLedger(SalesReportParam picker)
        {

            MaterialCostLedgerParam model = new MaterialCostLedgerParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),                
                Output = picker.Output,                
                ReportType = picker.ReportType
            };

            ViewBag.Token = model;
            Session["SalesReportParam"] = model;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            AccountsReportsDAO.GenerateMaterialCostLedgerReport();

            return RedirectToAction("MaterialCostLedger", "Reports");


        }
        #endregion

        #region "CustomerInvoiceRegister"
        public ActionResult CustomerInvoiceRegister()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            CustomerLedgerReportParam model = (CustomerLedgerReportParam)Session["CustomInvoiceRegisterParam"];

            if (model == null)
            {
                model = new CustomerLedgerReportParam
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date,
                    AsonDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    CustomerId = 0,
                    CustomerName = "",
                    Output = "PDF",
                    ReportType = "Ledger"
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommanFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
            }
            SessionDataModel.SetCustomerLedgerParam(model);

            model.FromDate = AccountsDAO.CheckParamDate(model.FromDate, yearid).Date;
            model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Customer Invoice Register";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("CustomerInvoiceRegister"))
                {
                    Session["ReportOutput"] = null;
                }
                
            }

            return View(model);

        }

        [HttpPost]
        public ActionResult CustomerInvoiceRegister(CustomerLedgerReportParam picker)
        {

            CustomerLedgerReportParam model = new CustomerLedgerReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                CustomerId = picker.CustomerId,
                CustomerName = picker.CustomerName,
                Output = picker.Output,
                ReportType = picker.ReportType,
                AsonDate = picker.AsonDate
            };

            ViewBag.Token = model;
            Session["CustomInvoiceRegisterParam"] = model;
            
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            AccountsReportsDAO.GenerateCustomerLedgerDetailReport();

            return RedirectToAction("CustomerInvoiceRegister", "Reports");


        }
        #endregion


        #region "SalesRegisterCountry"
        public ActionResult SalesRegisterCountry()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            ViewBag.Employee = db.EmployeeMasters.ToList();
            SalesRegisterCountryParam model = (SalesRegisterCountryParam)Session["SalesRegisterCountryParam"];
            if (model == null)
            {
                model = new SalesRegisterCountryParam
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date,               
                    CustomerId = 0,
                    CustomerName = "",
                    Output = "PDF",
                    ReportType = "Ledger"
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommanFunctions.GetFirstDayofMonth().Date;
            }

            if (model.ToDate.ToString() == "01-01-0001 00:00:00")
            {
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
            }

            Session["SalesRegisterCountryParam"] = model;

            //model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Sales Register Country Wise";
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("SalesRegisterCountry"))
                {
                    Session["ReportOutput"] = null;
                }

            }

            return View(model);

        }

        [HttpPost]
        public ActionResult SalesRegisterCountry(SalesRegisterCountryParam picker)
        {

            SalesRegisterCountryParam model = new SalesRegisterCountryParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                CustomerId = picker.CustomerId,
                CustomerName = picker.CustomerName,
                Output = picker.Output,                
                ReportType = picker.ReportType
            };

            ViewBag.Token = model;
            Session["SalesRegisterCountryParam"] = model;
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            AccountsReportsDAO.GenerateSalesRegisterCountryReport();

            return RedirectToAction("SalesRegisterCountry", "Reports");


        }



        #endregion
    }
}