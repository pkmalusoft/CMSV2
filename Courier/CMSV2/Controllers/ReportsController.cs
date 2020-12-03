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
                if (reportparam.FromDate.Date.ToString() == "01-01-0001 00:00:00")
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

            SessionDataModel.SetTaxReportParam(reportparam);
            List<VoucherTypeVM> lsttype = new List<VoucherTypeVM>();
            lsttype.Add(new VoucherTypeVM { TypeName = "All" });
            var typeitems = (from c in db.AcJournalMasters select new VoucherTypeVM { TypeName = c.VoucherType }).Distinct().ToList();
            foreach (VoucherTypeVM Item in typeitems)
            {
                lsttype.Add(Item);
            }
            
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


    }
}