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
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
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
            string reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output=="EXCEL")
            {
                
                reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if(reportparam.Output=="WORD")
            {
                reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
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
            comd.Parameters.AddWithValue("@AsOnDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
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
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AccTrialBalance.rpt")); //"AccTrialBalance.rpt"

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
            string period = "Period as on " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "AccTrialBal_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
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
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
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
            string period = "Period From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
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
            rd.Close();
            rd.Dispose();
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
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@FYearId", yearid);
            
            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "EmpostAnalysisReport");

            //generate XSD to design report
           // System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "EmpostAnalysisReport.xsd"));
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
            string reportname = "EmpostFee_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "EmpostFee_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "EmpostFee_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
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
            string reportname = "CustomerReceipt_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            
             //reportparam.ReportFileName = reportname;
             rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            reportpath = "~/ReportsPDF/" + reportname;
            rd.Close();
            rd.Dispose();
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
            rd.Close();
            rd.Dispose();
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


        public static string GenerateAWBRegister()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            AWBReportParam reportparam = SessionDataModel.GetAWBReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "sp_AWBRegisterReport";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@FYearId",yearid);
            if (reportparam.PaymentModeId == null)
            {
                comd.Parameters.AddWithValue("@PaymentModeId", 0);
            }
            else
            {
                comd.Parameters.AddWithValue("@PaymentModeId", reportparam.PaymentModeId);
            }
            if (reportparam.ParcelTypeId == null)
            {
                comd.Parameters.AddWithValue("@ParcelTypeId", 0);
            }
            else
            {
                comd.Parameters.AddWithValue("@ParcelTypeId", reportparam.ParcelTypeId);
            }
            comd.Parameters.AddWithValue("@MovementId", reportparam.MovementId);

            comd.Parameters.AddWithValue("@SortBy", reportparam.SortBy);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AWBRegister");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "AWBRegister.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            if (reportparam.ReportType == "Date")
            {

                rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AWBRegister.rpt"));
            }
            else if (reportparam.ReportType == "Movement")
            {
                rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AWBRegister_MovementWise.rpt"));
            }
            else if (reportparam.ReportType == "PaymentMode")
            {
                rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AWBRegister_Payment.rpt"));
            }
            else if (reportparam.ReportType == "ParcelType")
            {
                rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AWBRegister_ParcelType.rpt"));
            }
            else if (reportparam.ReportType == "Summary")
            {
                rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AWBRegister_Summary.rpt"));
            }

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            //rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Airway Bill Register");
            string period = "For the Period From " + reportparam.FromDate.Date.ToString("dd MMM yyyy") + " to " + reportparam.ToDate.Date.ToString("dd MMM yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);


            rd.ParameterFields["GroupBy"].CurrentValues.AddValue(reportparam.ReportType);
            rd.ParameterFields["SortBy"].CurrentValues.AddValue(reportparam.SortBy);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            
            string reportname = "AWBRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "AWBRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "AWBRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }


        public static string GenerateTaxRegister()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            TaxReportParam reportparam = SessionDataModel.GetTaxReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "sp_TaxRegisterReport";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@FYearId", yearid);
            if (reportparam.TransactionType == null)
            {
                comd.Parameters.AddWithValue("@VoucherType", "All");
            }
            else
            {
                comd.Parameters.AddWithValue("@VoucherType", reportparam.TransactionType);
            }
            comd.Parameters.AddWithValue("@SortBy", reportparam.SortBy);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "TaxRegister");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "TaxRegister.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "TaxRegister.rpt"));            

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            //rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Tax Register");
            string period = "For the Period From " + reportparam.FromDate.Date.ToString("dd MMM yyyy") + " to " + reportparam.ToDate.Date.ToString("dd MMM yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);


          //  rd.ParameterFields["GroupBy"].CurrentValues.AddValue(reportparam.ReportType);
           // rd.ParameterFields["SortBy"].CurrentValues.AddValue(reportparam.SortBy);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();

            string reportname = "TaxRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "TaxRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "TaxRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }

        public static string GenerateProfitLostReport()
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
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
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
            string reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }



        public static string GenerateAWBReport(int id)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_AWBPrintReport";
            comd.Parameters.AddWithValue("@InscanId", id);            

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AWBPrint");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "AWBPrint.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AWBPrint.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);

            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);            
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("AWB PRINT");
            string period = "AWB Print";
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);


            //  rd.ParameterFields["GroupBy"].CurrentValues.AddValue(reportparam.ReportType);
            // rd.ParameterFields["SortBy"].CurrentValues.AddValue(reportparam.SortBy);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();

            string reportname = "AWBPrint_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                    
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }

        public static string GenerateDayBookReport()
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
            comd.CommandText = "SP_AccDayBook";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));          
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@YearId", yearid);
            comd.Parameters.AddWithValue("@VoucherType", reportparam.VoucherTypeId);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AccDayBook");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "DayBook.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();           

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "DayBook.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Day Book");
            string period = "From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "DayBook_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }

        public static string GenerateCustomerLedgerReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            CustomerLedgerReportParam reportparam = SessionDataModel.GetCustomerLedgerReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_CustomerLedger";            
            comd.Parameters.AddWithValue("@CustomerId", reportparam.CustomerId);
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));            
            comd.Parameters.AddWithValue("@FYearId", yearid);
            
            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "CustomerLedger");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "CustomerLedger.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();           

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CustomerLedger.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Customer Ledger");
            string period = "From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "CustomerLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "CustomerLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "CustomerLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }

        public static string GenerateCustomerLedgerDetailReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            CustomerLedgerReportParam reportparam = SessionDataModel.GetCustomerLedgerReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_CustomerLedgerDetail";
            comd.Parameters.AddWithValue("@CustomerId", reportparam.CustomerId);
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@FYearId", yearid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "CustomerLedger");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/reportsxsd"), "CustomerLedger.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CustomerLedger.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Customer Ledger");
            string period = "From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "CustomerLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "CustomerLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "CustomerLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }
        public static string GenerateCustomerOutStandingReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            CustomerLedgerReportParam reportparam = SessionDataModel.GetCustomerLedgerReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_CustomerOutStanding";            
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@FYearId", yearid);
            comd.Parameters.AddWithValue("@CustomerId", reportparam.CustomerId);
             
            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "CustomerOutStanding");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "CustomerOutStanding.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CustomerOutStanding.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Customer OutStanding Report");
            string period = "From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "CustomerOutStanding_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "CustomerOutStanding_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "CustomerOutStanding_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }
               

        public static string GenerateAWBOutStandingReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int depotid = Convert.ToInt32(HttpContext.Current.Session["CurrentDepotID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            CustomerLedgerReportParam reportparam = SessionDataModel.GetCustomerLedgerReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_AWBOutStanding";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@CustomerId", reportparam.CustomerId);
            comd.Parameters.AddWithValue("@FYearId", yearid);
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@DepotId", depotid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AWBOutStanding");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "AWBOutStanding.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AWBOutStanding.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            if (reportparam.CustomerName!="" && reportparam.CustomerName!=null)
              rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("AWB OutStanding Report for Customer " + reportparam.CustomerName );
            else
                rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("AWB OutStanding Report for All Customer");
            string period = "From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "AWBOutStanding_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "AWBOutStanding_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "AWBOutStanding_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }
        public static string GenerateAWBUnInvoiced()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int depotid = Convert.ToInt32(HttpContext.Current.Session["CurrentDepotID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            CustomerLedgerReportParam reportparam = SessionDataModel.GetCustomerLedgerReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_AWBOutStanding1";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@CustomerId", reportparam.CustomerId);
            comd.Parameters.AddWithValue("@FYearId", yearid);
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@DepotId", depotid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AWBUnInvoiced");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "AWBUnInvoiced.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AWBUnInvoiced.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            if (reportparam.CustomerName != "" && reportparam.CustomerName != null)
                rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("AWB UnInvoiced Report for Customer " + reportparam.CustomerName);
            else
                rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("AWB UnInvoiced Report for All Customer");
            string period = "From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "AWBUnInvoiced_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "AWBUnInvoiced_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "AWBUnInvoiced_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }

        public static string GenerateCustomerAgingReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            CustomerLedgerReportParam reportparam = SessionDataModel.GetCustomerLedgerReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_CustomerAging";
            comd.Parameters.AddWithValue("@CustomerId", reportparam.CustomerId);
            comd.Parameters.AddWithValue("@AsonDate", reportparam.AsonDate.ToString("MM/dd/yyyy"));
            //comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@FYearId", yearid);
            comd.Parameters.AddWithValue("@ReportOption", reportparam.ReportType);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "CustomerAging");

            //generate XSD to design report          
            //if (reportparam.ReportType == "Detail")
            //{
            //    System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "CustomerAgingDetail.xsd"));

            //    ds.WriteXmlSchema(writer);
            //    writer.Close();
            //}
            //else
            //{
            //    System.IO.StreamWriter writer1 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "CustomerAgingSummary.xsd"));
            //    ds.WriteXmlSchema(writer1);
            //    writer1.Close();
            //}


            ReportDocument rd = new ReportDocument();
            if (reportparam.ReportType == "Detail")
                rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CustomerAgingDetail.rpt"));
            else
                rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CustomerAgingSummary.rpt"));
            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Customer Aging " + reportparam.ReportType);
            string period = " As on " + reportparam.AsonDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "CustomerAging_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "CustomerAging_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "CustomerAging_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }

        public static string GenerateSupplierLedgerDetailReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            SupplierLedgerReportParam reportparam = SessionDataModel.GetSupplierLedgerReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_SupplierLedgerDetail";
            comd.Parameters.AddWithValue("@SupplierId", reportparam.SupplierId);
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@FYearId", yearid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "SupplierLedgerDetail");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "SupplierLedger.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "SupplierLedger.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Supplier Ledger");
            string period = "From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "SupplierLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "SupplierLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "SupplierLedger_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }
        public static string GenerateSupplierStatementDetailReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            SupplierLedgerReportParam reportparam = SessionDataModel.GetSupplierLedgerReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_SupplierStatement";
            comd.Parameters.AddWithValue("@SupplierId", reportparam.SupplierId);
            comd.Parameters.AddWithValue("@SupplierTypeId", reportparam.SupplierTypeId);
            comd.Parameters.AddWithValue("@AsonDate", reportparam.AsonDate.ToString("MM/dd/yyyy"));
            //comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@FYearId", yearid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "SupplierLedgerDetail");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "SupplierPayment.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "SupplierStatement.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Supplier Statement");
            string period = "As on " + reportparam.AsonDate.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "SupplierStatement_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "SupplierStatement_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "SupplierStatement_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }


        public static string GenerateSupplierAgingReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            SupplierLedgerReportParam reportparam = SessionDataModel.GetSupplierLedgerReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_SupplierAging";
            comd.Parameters.AddWithValue("@SupplierId", reportparam.SupplierId);
            comd.Parameters.AddWithValue("@AsonDate", reportparam.AsonDate.ToString("MM/dd/yyyy"));
            //comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@FYearId", yearid);
            comd.Parameters.AddWithValue("@ReportOption", reportparam.ReportType);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "SupplierAging");

            //generate XSD to design report      
            //if (reportparam.ReportType == "Detail")
            //{
            //    System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "SupplierAgingDetail.xsd"));
            //    ds.WriteXmlSchema(writer);
            //    writer.Close();
            //}
            //else
            //{
            //    System.IO.StreamWriter writer1 = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "SupplierAgingSummary.xsd"));
            //    ds.WriteXmlSchema(writer1);
            //    writer1.Close();
            //}

            ReportDocument rd = new ReportDocument();

            if (reportparam.ReportType == "Detail")
                rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "SupplierAgingDetail.rpt"));
            else
                rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "SupplierAgingSummary.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Supplier Aging " + reportparam.ReportType);
            string period = "As on " + reportparam.AsonDate.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "SupplierAging_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "SupplierAging_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "SupplierAging_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }
        public static string GenerateCustomerStatementReport()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            CustomerLedgerReportParam reportparam = SessionDataModel.GetCustomerLedgerReportParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_CustomerStatement";
            comd.Parameters.AddWithValue("@CustomerId", reportparam.CustomerId);
            comd.Parameters.AddWithValue("@AsonDate", reportparam.AsonDate.ToString("MM/dd/yyyy"));
            //comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@FYearId", yearid);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "CustomerLedgerDetail");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "CustomerStatement.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CustomerStatement.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("Customer Statement");
            string period = " As on " + reportparam.AsonDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "CustomerStatement_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "CustomerStatement_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "CustomerStatement_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }
        public static string GenerateCODRegister()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            CODReportParam reportparam = (CODReportParam)HttpContext.Current.Session["CODReportParam"];
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_CODRegisterReport";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@FYearId", yearid);
           
            if (reportparam.ParcelTypeId == null)
            {
                comd.Parameters.AddWithValue("@ParcelTypeId", 0);
            }
            else
            {
                comd.Parameters.AddWithValue("@ParcelTypeId", reportparam.ParcelTypeId);
            }
            comd.Parameters.AddWithValue("@MovementId", reportparam.MovementId);

            comd.Parameters.AddWithValue("@SortBy", reportparam.SortBy);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "CODRegister");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "CODRegister.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister.rpt"));
            //if (reportparam.ReportType == "Date")
            //{
                            
            //}
            //else if (reportparam.ReportType == "Movement")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_MovementWise.rpt"));
            //}
            //else if (reportparam.ReportType == "PaymentMode")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_Payment.rpt"));
            //}
            //else if (reportparam.ReportType == "ParcelType")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_ParcelType.rpt"));
            //}
            //else if (reportparam.ReportType == "Summary")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_Summary.rpt"));
            //}

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            //rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("COD Register");
            string period = "For the Period From " + reportparam.FromDate.Date.ToString("dd MMM yyyy") + " to " + reportparam.ToDate.Date.ToString("dd MMM yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);


            rd.ParameterFields["GroupBy"].CurrentValues.AddValue(reportparam.ReportType);
            rd.ParameterFields["SortBy"].CurrentValues.AddValue(reportparam.SortBy);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();

            string reportname = "CODRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "CODRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "CODRegister_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }
        public static string GenerateCODPending()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            CODReportParam reportparam = (CODReportParam)HttpContext.Current.Session["CODReportParam"];
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_CODRegisterPending";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@FYearId", yearid);

            if (reportparam.ParcelTypeId == null)
            {
                comd.Parameters.AddWithValue("@ParcelTypeId", 0);
            }
            else
            {
                comd.Parameters.AddWithValue("@ParcelTypeId", reportparam.ParcelTypeId);
            }
            comd.Parameters.AddWithValue("@MovementId", reportparam.MovementId);

            comd.Parameters.AddWithValue("@SortBy", reportparam.SortBy);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "CODPending");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "CODPending.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODPending.rpt"));
            //if (reportparam.ReportType == "Date")
            //{

            //}
            //else if (reportparam.ReportType == "Movement")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_MovementWise.rpt"));
            //}
            //else if (reportparam.ReportType == "PaymentMode")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_Payment.rpt"));
            //}
            //else if (reportparam.ReportType == "ParcelType")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_ParcelType.rpt"));
            //}
            //else if (reportparam.ReportType == "Summary")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_Summary.rpt"));
            //}

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            //rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("COD Pending");
            string period = "For the Period From " + reportparam.FromDate.Date.ToString("dd MMM yyyy") + " to " + reportparam.ToDate.Date.ToString("dd MMM yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);


            rd.ParameterFields["GroupBy"].CurrentValues.AddValue(reportparam.ReportType);
            rd.ParameterFields["SortBy"].CurrentValues.AddValue(reportparam.SortBy);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();

            string reportname = "CODPending_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "CODPending_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "CODPending_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }
        public static string GenerateCODSummary()
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            CODReportParam reportparam = (CODReportParam)HttpContext.Current.Session["CODReportParam"];
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_CODRegisterSummary";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate.ToString("MM/dd/yyyy"));
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@FYearId", yearid);

            if (reportparam.ParcelTypeId == null)
            {
                comd.Parameters.AddWithValue("@ParcelTypeId", 0);
            }
            else
            {
                comd.Parameters.AddWithValue("@ParcelTypeId", reportparam.ParcelTypeId);
            }
            comd.Parameters.AddWithValue("@MovementId", reportparam.MovementId);

            comd.Parameters.AddWithValue("@SortBy", reportparam.SortBy);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "CODSummary");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "CODSummary.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODSummary.rpt"));
            //if (reportparam.ReportType == "Date")
            //{

            //}
            //else if (reportparam.ReportType == "Movement")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_MovementWise.rpt"));
            //}
            //else if (reportparam.ReportType == "PaymentMode")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_Payment.rpt"));
            //}
            //else if (reportparam.ReportType == "ParcelType")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_ParcelType.rpt"));
            //}
            //else if (reportparam.ReportType == "Summary")
            //{
            //    rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CODRegister_Summary.rpt"));
            //}

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            //rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("COD Register Summary");
            string period = "For the Period From " + reportparam.FromDate.Date.ToString("dd MMM yyyy") + " to " + reportparam.ToDate.Date.ToString("dd MMM yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);


            rd.ParameterFields["GroupBy"].CurrentValues.AddValue(reportparam.ReportType);
            rd.ParameterFields["SortBy"].CurrentValues.AddValue(reportparam.SortBy);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();

            string reportname = "CODSummary_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            if (reportparam.Output == "PDF")
            {
                reportparam.ReportFileName = reportname;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            }
            else if (reportparam.Output == "EXCEL")
            {

                reportname = "CODSummary_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".xlsx";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.ExcelWorkbook, reportpath);
            }
            else if (reportparam.Output == "WORD")
            {
                reportname = "CODSummary_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".doc";
                reportparam.ReportFileName = reportname;
                reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
                rd.ExportToDisk(ExportFormatType.WordForWindows, reportpath);
            }
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportpath;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));

            //return File(stream, "application/pdf", "AccLedger.pdf");
        }
        #region "ReceiptVoucher"
        //Account Cash/Bank book receipt/payment voucher print        
        public static string GenerateReceiptPaymentVoucherPrint(int id)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_ReceiptPaymentVoucher";
            comd.Parameters.AddWithValue("@ID", id);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AcReceiptPaymentVoucherPrint");

            string PaymentType = "";
            string TransType = "";
            string Title = "";
            if (ds.Tables[0].Rows.Count > 0)
            {
                PaymentType = ds.Tables[0].Rows[0]["PaymentType"].ToString();
                TransType = ds.Tables[0].Rows[0]["TransType"].ToString();
            }
            if (PaymentType == "1" && TransType == "1")
                Title = "CASH RECEIPT VOUCHER";
            else if (PaymentType == "2" && TransType == "1")
                Title = "BANK RECEIPT VOUCHER";
            else if (PaymentType == "1" && TransType == "2")
                Title = "CASH PAYMENT VOUCHER";
            else if (PaymentType == "2" && TransType == "2")
                Title = "BANK PAYMENT VOUCHER";


            //generate XSD to design report            
            //if (PaymentType == "1") --Cash
            //{
            //    System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "AcReceiptPaymentVoucherPrint.xsd"));
            //    ds.WriteXmlSchema(writer);
            //    writer.Close();
            //}
            //else  --Bank
            //{
            //    System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "BankReceiptPaymentVoucherPrint.xsd"));
            //    ds.WriteXmlSchema(writer);
            //    writer.Close();
            //}


            ReportDocument rd = new ReportDocument();

            if (PaymentType == "1") //--cash
                rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CashReceiptPaymentVoucherPrint.rpt"));
            else
                rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "BankReceiptPaymentVoucherPrint.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue(Title);
            //string period = "As on " + reportparam.FromDate.Date.ToString("dd-MM-yyyy"); // + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            //rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "AcReceiptPaymentVoucherPrint_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            rd.Close();
            rd.Dispose();
            reportpath = "~/ReportsPDF/" + reportname;
            rd.Close();
            rd.Dispose();
            return reportname;

        }

        public static string GenerateJournalVoucherPrint(int id)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_JournalVoucher";
            comd.Parameters.AddWithValue("@ID", id);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AcJournalVoucherPrint");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "AcJournalVoucherPrint.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "AcJournalVoucherPrint.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("JOURNAL VOUCHER");
            //string period = "As on " + reportparam.FromDate.Date.ToString("dd-MM-yyyy"); // + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            //rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "AcJournalVoucherPrint_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportname;

        }


        public static string GenerateDebitNoteVoucherPrint(int id)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();

            
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_DebitNoteVoucher";
            comd.Parameters.AddWithValue("@ID", id);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "DebitNoteVoucherPrint");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "DebitNoteVoucherPrint.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "DebitNoteVoucherPrint.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("DEBIT NOTE VOUCHER");
            //string period = "As on " + reportparam.FromDate.Date.ToString("dd-MM-yyyy"); // + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            //rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "AcJournalVoucherPrint_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportname;

        }

        public static string GenerateCreditNoteVoucherPrint(int id)
        {
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();
            
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "SP_CreditNoteVoucher";
            comd.Parameters.AddWithValue("@ID", id);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "CreditNoteVoucherPrint");

            //generate XSD to design report            
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(HostingEnvironment.MapPath("~/ReportsXSD"), "CreditNoteVoucherPrint.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(HostingEnvironment.MapPath("~/Reports"), "CreditNoteVoucherPrint.rpt"));

            rd.SetDataSource(ds);

            //Set Paramerter Field Values -General
            #region "param"
            string companyaddress = SourceMastersModel.GetCompanyAddress(branchid);
            string companyname = SourceMastersModel.GetCompanyname(branchid);
            string companylocation = SourceMastersModel.GetCompanyLocation(branchid);

            // Assign the params collection to the report viewer            
            rd.ParameterFields["CompanyName"].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["CompanyLocation"].CurrentValues.AddValue(companylocation);
            rd.ParameterFields["ReportTitle"].CurrentValues.AddValue("CREDIT NOTE VOUCHER");
            //string period = "As on " + reportparam.FromDate.Date.ToString("dd-MM-yyyy"); // + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            //rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);
            #endregion

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            string reportname = "CreditNoteVoucherPrint_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            string reportpath = Path.Combine(HostingEnvironment.MapPath("~/ReportsPDF"), reportname);
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, reportpath);
            rd.Close();
            rd.Dispose();
            HttpContext.Current.Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
            return reportname;

        }

        #endregion
    }
}