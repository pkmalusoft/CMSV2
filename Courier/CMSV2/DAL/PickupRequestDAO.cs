using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CMSV2.Models;
using System.Web.Hosting;

namespace CMSV2.DAL
{
    public class PickupRequestDAO
    {
        public string GetMaxPickupRequest(int Companyid,int BranchId)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "GetMaxPickupRequest";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", Companyid);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return MaxPickUpNo;

        }

        public string GetMaxCustomerCode(int BranchId)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "GetMaxCustomerNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@BranchId", BranchId);

                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return MaxPickUpNo;

        }


        public string GetMaAWBNo(int CompanyId,int BranchId=1)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "GetMaxAWBNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return MaxPickUpNo;

        }


        public string GetMaxInScanSheetNo(int Companyid, int BranchId,string Type)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "GetMaxInScanSheetNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", Companyid);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@SourceType", Type);
                        
                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return MaxPickUpNo;

        }

        public string GetMaxInvoiceNo(int Companyid, int BranchId,int FYearId)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "GetMaxInvoiceNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", Companyid);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@FYearId", FYearId);
                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return MaxPickUpNo;

        }

        public string GetMaxTaxInvoiceNo(int Companyid, int BranchId, int FYearId)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "GetTAXInvoiceNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", Companyid);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@FYearId", FYearId);
                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return MaxPickUpNo;

        }

        public string GetMaxDRSNo(int Companyid, int BranchId)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "GetMaxDRSSheetNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", Companyid);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);                        

                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return MaxPickUpNo;

        }


        public string GetMaxDomesticReceiptNo(int Companyid, int BranchId, int FYearId)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "GetMaxDomesticCODReceiptNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", Companyid);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@FYearId", FYearId);
                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return MaxPickUpNo;

        }


        public string GetMaxMCDocumentNo(int Companyid, int BranchId, int FYearId)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "GetMaxMCDocumentNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", Companyid);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@FYearId", FYearId);
                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return MaxPickUpNo;

        }

        // Generate a random password of a given length (optional)  
        public string RandomPassword(int size = 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }
        // Generate a random string with a given size and case.   
        // If second parameter is true, the return string is lowercase  
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        // Generate a random number between two numbers    
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public string SaveMenuAccess(int RoleId ,string Menus)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SaveRoleMenuAccessRights";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@RoleId", RoleId);
                        cmd.Parameters.AddWithValue("@MenusList", Menus);
                        con.Open();
                        cmd.ExecuteNonQuery();

                        //SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        //SqlDA.Fill(dt);
                        //if (dt.Rows.Count > 0)
                        //    MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return MaxPickUpNo;
        }


        public string GetMaxCODReceiptNo(int Companyid, int BranchId)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "GetMaxCODReceiptNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", Companyid);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return MaxPickUpNo;

        }


        public string DeleteExportShipment(int Id)
        {            
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "Delete from exportshipmentDetails where ExportId=" + Id.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "Delete from exportshipment where Id=" + Id.ToString();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";

        }

        //SP_AWBPosting
        public string AWBAccountsPosting(int Id)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_AWBPosting " + Id.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";

        }

        //Generate Invoice Posting
        public string GenerateInvoicePosting(int Id)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_GenerateInvoicePosting " + Id.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";

        }



        //Generate COD Receipt posting
        public string GenerateCODPosting(int Id)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_CODReceiptPosting " + Id.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";

        }

        //Generate Domestic COD Receipt posting
        public string GenerateDomesticCODPosting(int Id)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_DomesticCODReceiptPosting " + Id.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";

        }

        ///Export Manifest Posting SP_ExportManifestPosting 5
        ///
        public string GenerateExportManifestPosting(int Id)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_ExportManifestPosting " + Id.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";

        }


        public string GenerateInvoiceAll(InvoiceAllParam obj)
        {
            int companyId = Convert.ToInt32(HttpContext.Current.Session["CurrentCompanyID"].ToString());
            int branchid = Convert.ToInt32(HttpContext.Current.Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
            string usertype = HttpContext.Current.Session["UserType"].ToString();
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_GenerateMultipleInvoice ";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromDate", obj.FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", obj.ToDate);
                        cmd.Parameters.AddWithValue("@InvoiceDate", obj.InvoiceDate);
                        cmd.Parameters.AddWithValue("@MovementId", obj.MovementId);
                        cmd.Parameters.AddWithValue("@Companyid", companyId);
                        cmd.Parameters.AddWithValue("@FYearId",  yearid);
                        cmd.Parameters.AddWithValue("@BranchId", branchid);

                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";

        }

        public static List<QuickAWBVM> GetAWBList(int StatusId,DateTime FromDate,DateTime ToDate,int BranchId,int DepotId, string AWBNo,int MovementId,int PaymentModeId,string ConsignorText,string Origin,string Destination)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetAWBList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pStatusId",StatusId);
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@DepotId", DepotId);
                         
            cmd.Parameters.AddWithValue("@MovementId", MovementId);
            cmd.Parameters.AddWithValue("@PaymentModeId", PaymentModeId);
            if (ConsignorText == null)
                ConsignorText = "";
            else
                ConsignorText = ConsignorText + "%";
            cmd.Parameters.AddWithValue("@ConsignorConsignee", ConsignorText);
            if (Origin == null)
                Origin = "";
            cmd.Parameters.AddWithValue("@Origin", Origin);
            if (Destination == null)
                Destination = "";
            cmd.Parameters.AddWithValue("@Destination", Destination);
             
            if (AWBNo == null)
                AWBNo = "";
                cmd.Parameters.AddWithValue("@AWBNo", AWBNo);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<QuickAWBVM> objList = new List<QuickAWBVM>();
            QuickAWBVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new QuickAWBVM();
                    obj.InScanID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["InScanID"].ToString());
                    obj.HAWBNo = ds.Tables[0].Rows[i]["HAWBNo"].ToString();
                    obj.shippername= ds.Tables[0].Rows[i]["shippername"].ToString();
                    obj.consigneename = ds.Tables[0].Rows[i]["consigneename"].ToString();
                    obj.destination = ds.Tables[0].Rows[i]["destination"].ToString();
                    obj.InScanDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["InScanDate"].ToString());
                    obj.CourierStatus = ds.Tables[0].Rows[i]["CourierStatus"].ToString();
                    obj.StatusType = ds.Tables[0].Rows[i]["StatusType"].ToString();
                    obj.totalCharge = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["totalCharge"].ToString());                    
                    obj.paymentmode = ds.Tables[0].Rows[i]["paymentmode"].ToString();
                    obj.ConsigneePhone= ds.Tables[0].Rows[i]["ConsigneePhone"].ToString();
                    obj.CreatedByName = ds.Tables[0].Rows[i]["CreatedByName"].ToString();
                    obj.LastModifiedByName= ds.Tables[0].Rows[i]["LastModifiedByName"].ToString();
                    obj.CreatedByDate = ds.Tables[0].Rows[i]["CreatedByDate"].ToString();
                    obj.LastModifiedDate = ds.Tables[0].Rows[i]["LastModifiedDate"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }

        //Generate SupplierInvoice posting
        public string GenerateSupplierInvoicePosting(int Id)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_SupplierInvoicePosting " + Id.ToString();
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";

        }

        public static List<CustomerInvoiceVM> GetInvoiceList(DateTime FromDate,DateTime ToDate,string InvoiceNo,int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetInvoiceList";
            cmd.CommandType = CommandType.StoredProcedure;            
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@FYearId", FyearId);            
            if (InvoiceNo == null)
                InvoiceNo = "";
            cmd.Parameters.AddWithValue("@InvoiceNo", @InvoiceNo);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<CustomerInvoiceVM> objList = new List<CustomerInvoiceVM>();
            CustomerInvoiceVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new CustomerInvoiceVM();
                    obj.CustomerInvoiceID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["CustomerInvoiceID"].ToString());
                    obj.CustomerInvoiceNo = ds.Tables[0].Rows[i]["CustomerInvoiceNo"].ToString();
                    obj.InvoiceDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["InvoiceDate"].ToString());
                    obj.CustomerName = ds.Tables[0].Rows[i]["CustomerName"].ToString();
                    obj.CustomerID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["CustomerID"].ToString());
                    obj.InvoiceTotal = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["InvoiceTotal"].ToString());                    
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<InScanVM> GetInScanList(DateTime FromDate, DateTime ToDate, int FyearId,int BranchId,int DepotId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetInScanList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            cmd.Parameters.AddWithValue("@BranchID", BranchId);
            cmd.Parameters.AddWithValue("@DepotId", DepotId);            

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<InScanVM> objList = new List<InScanVM>();
            InScanVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new InScanVM();                      
                    obj.QuickInscanID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["QuickInscanID"].ToString());
                    obj.InScanSheetNo= ds.Tables[0].Rows[i]["InscanSheetNumber"].ToString();
                    obj.QuickInscanDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["QuickInscanDateTime"].ToString());
                    obj.CollectedBy = ds.Tables[0].Rows[i]["CollectedBy"].ToString();
                    obj.ReceivedBy = ds.Tables[0].Rows[i]["ReceivedBy"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<DRSVM> GetOutScanList(DateTime FromDate, DateTime ToDate, int FyearId, int BranchId, int DepotId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetOutScanList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            cmd.Parameters.AddWithValue("@BranchID", BranchId);
            cmd.Parameters.AddWithValue("@DepotId", DepotId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<DRSVM> objList = new List<DRSVM>();
            DRSVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new DRSVM();
                    obj.DRSID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["DRSID"].ToString());
                    obj.DRSNo = ds.Tables[0].Rows[i]["DRSNo"].ToString();
                    obj.DRSDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["DRSDate"].ToString());
                    obj.Deliver = ds.Tables[0].Rows[i]["DeliveredBy"].ToString();
                    obj.vehicle = ds.Tables[0].Rows[i]["vehicle"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }


        public static List<ShipmentInvoiceVM> GetShipmentMAWBList()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetShipmentMAWB";
            cmd.CommandType = CommandType.StoredProcedure;                        

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<ShipmentInvoiceVM> objList = new List<ShipmentInvoiceVM>();
            ShipmentInvoiceVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new ShipmentInvoiceVM();
                    obj.MAWB = ds.Tables[0].Rows[i]["MAWB"].ToString();
                    obj.ShipmentImportID = Convert.ToInt32(ds.Tables[0].Rows[i]["ShipmentImportID"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<ShipmentInvoiceVM> GetTaxInvoiceList( string AWBNo, DateTime FromDate, DateTime ToDate, int FyearId, int BranchId, int DepotId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetTAxInvoiceList";
            cmd.CommandType = CommandType.StoredProcedure;
            if (AWBNo==null)
                cmd.Parameters.AddWithValue("@AWBNo", "");
            else
                cmd.Parameters.AddWithValue("@AWBNo", AWBNo);
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            cmd.Parameters.AddWithValue("@BranchID", BranchId);
            

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<ShipmentInvoiceVM> objList = new List<ShipmentInvoiceVM>();
            ShipmentInvoiceVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new ShipmentInvoiceVM();
                    obj.ShipmentInvoiceID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["ShipmentInvoiceID"].ToString());
                    obj.InvoiceNo = ds.Tables[0].Rows[i]["InvoiceNo"].ToString();
                    obj.InvoiceDate  = Convert.ToDateTime(ds.Tables[0].Rows[i]["InvoiceDate"].ToString());
                    obj.InvoiceTotal  = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["InvoiceTotal"].ToString());
                    obj.EnteredBy = ds.Tables[0].Rows[i]["EnteredBy"].ToString();
                    obj.MAWB = ds.Tables[0].Rows[i]["MAWB"].ToString();
                    if (ds.Tables[0].Rows[i]["ImportDate"]!=System.DBNull.Value)
                        obj.ImportDate =  Convert.ToDateTime(ds.Tables[0].Rows[i]["ImportDate"].ToString());

                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<CustomerContractVM> GetCustomerContracts(int CustomerId,string CourierType)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCustomerContracts";
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            //cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            cmd.Parameters.AddWithValue("@CourierType", "");
            //cmd.Parameters.AddWithValue("@BranchID", BranchId);
            //cmd.Parameters.AddWithValue("@DepotId", DepotId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<CustomerContractVM> objList = new List<CustomerContractVM>();
            CustomerContractVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new CustomerContractVM();
                    obj.ID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.CustomerID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["CustomerID"].ToString());
                    obj.CustomerRateTypeID= Convert.ToInt32(ds.Tables[0].Rows[i]["CustomerRateTypeID"].ToString());
                    obj.CustomerRateType = ds.Tables[0].Rows[i]["CustomerRateType"].ToString();
                    obj.CustomerName = ds.Tables[0].Rows[i]["CustomerName"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }

     
    }

}