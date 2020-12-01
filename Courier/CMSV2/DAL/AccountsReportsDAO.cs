using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CMSV2.Models;
using System.Data.SqlClient;
using System.IO;
using System.Web.Hosting;

namespace CMSV2.DAL
{
    public class AccountsReportsDAO
    {

        public static string GenerateLedgerReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "sp_accledger";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate);
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate);
            comd.Parameters.AddWithValue("@AcHeadId", reportparam.AcHeadId);
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@YearId", yearid);
            
            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AccLedger");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(Server.MapPath("~/Reports"),"AccLedger.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();           

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AccLedger.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["AccountHead"].CurrentValues.AddValue(reportparam.AcHeadName);
            string period = "Period From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmm") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output=="EXCEL")
            {
                
                reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmm") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if(reportparam.Output=="WORD")
            {
                reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmm") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
                        
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }


        public static string GenerateTrialBalanceReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_AccTrailBalance";
            comd.Parameters.AddWithValue("@AsOnDate", reportparam.ToDate);
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@YearId", yearid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AccTrialBalance");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "AccTrialBalance.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AccTrialBalance.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            string reporttile = "Trial Balance";
            rd.ParameterFields["AccountHead"].CurrentValues.AddValue(reporttile);
            string period = "As on :" + reportparam.ToDate.Date.ToString("dd MMMM yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "AccTrialBal_" + DateTime.Now.ToString("ddMMyyHHmm") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }

        public static string GenerateTradingAccountReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_AccTradingAccount";
            comd.Parameters.AddWithValue("@AsOnDate", reportparam.ToDate);
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@YearId", yearid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AccTradingAccount");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "AccTradingAccount.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AccTradingAccount.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            string reporttile = "Trading Account";
            rd.ParameterFields["AccountHead"].CurrentValues.AddValue(reporttile);
            string period = "As on :" + reportparam.ToDate.Date.ToString("dd MMMMM yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "AccTrading_" + DateTime.Now.ToString("ddMMyyHHmmSS") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }

        public static string GenerateEmposFeeReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "sp_EmpostAnalysisReport";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@BranchId", branchid);           

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "EmpostAnalysisReport");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "EmpostAnalysisReport.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "EmpostFeeReport.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            //rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Empost Fees Statement");
            string period = "For the Period From " + reportparam.FromDate.Date.ToString("dd MMM yyyy") + " to " + reportparam.ToDate.Date.ToString("dd MMM yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "EmpostFee_" + DateTime.Now.ToString("ddMMyyHHmm") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "EmpostFee_" + DateTime.Now.ToString("ddMMyyHHmm") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "EmpostFee_" + DateTime.Now.ToString("ddMMyyHHmm") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }

            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }


        public static string GenerateCustomerReceipt(int id)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_GetCustomerReceipt";
            comd.Parameters.AddWithValue("@Id", id);
            
            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "CustomerReceipt");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "CustomerReceipt.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CustomerReceipt.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            //rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("RECEIPT VOUCHER");
            //string period = "Period From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            //rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "CustomerReceipt_" + DateTime.Now.ToString("ddMMyyHHmm") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            
             //reportparam.ReportFileName = reportname;
             rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            reportpath = "~/ReportsPDF/" + reportname;
            return reportname;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }

        public static string GenerateDefaultReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();
            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
                    
            //comd.CommandText = "up_GetAllCustomer"; comd.Parameters.Add("@Companyname", SqlDbType.VarChar, 50);
            //if (TextBox1.Text.Trim() != "")
            //    comd.Parameters[0].Value = TextBox1.Text;
            //else
            //    comd.Parameters[0].Value = DBNull.Value;
            //SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            //sqlAdapter.SelectCommand = comd;
            //DataSet ds = new DataSet();
            //sqlAdapter.Fill(ds, "AccLedger");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(Server.MapPath("~/Reports"),"AccLedger.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();           

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "DefaultReport.rpt"));

            //rd.SetDataSource(ds);


            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["AccountHead"].CurrentValues.AddValue("Default Report");
            string period = "Reprot Period as on Date "; // + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            //string reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmm") + ".pdf";
            string reportname = "DefaultReport.pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);

            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            reportpath = "~/ReportsPDF/" + reportname;
            return reportpath;
            //Session["ReportOutput"] = "~/ReportsPDF/" + reportname;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //return stream;
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));
            //SaveStreamAsFile(reportpath, stream, reportname);
            //reportpath = Path.Combine(Server.MapPath("~/ReportsPDF"),reportname);            
            //return reportpath;
        }
    }
}