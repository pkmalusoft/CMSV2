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

        public ActionResult EmposFeeReport()
        {
            ViewBag.ReportName = "Empos Fee Analysis Report";
            return View();
        }
        public ActionResult ReportFrame()
        {
            if (Session["ReportOutput"] != null)
                ViewBag.ReportOutput = Session["ReportOutput"].ToString();
            else
                ViewBag.ReportOutput = "~/Reports/DefaultReport.pdf";
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


    }
}