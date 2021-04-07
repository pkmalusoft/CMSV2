using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using CMSV2.Models;
using System.Configuration;
namespace CMSV2.DAL
{
    public class AWBDAO
    {

        #region "AWBBookIssue"
        public static DataTable DeleteAWBCourier(int AWBBookIssueID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteAWBBookIssue";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AWBBookIssueID", AWBBookIssueID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }


        }
        public static List<AWBBookIssueList> GetAWBBookIssue(int branchId)
        {
            AWBBookIssueSearch paramobj = (AWBBookIssueSearch)(HttpContext.Current.Session["AWBBookIssueSearch"]);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetAWBBookIssueDetails";
            cmd.CommandType = CommandType.StoredProcedure;

            if (paramobj.DocumentNo != null)
                cmd.Parameters.AddWithValue("@DocumentNo", paramobj.DocumentNo);

            if (paramobj.FromDate!=null)
            cmd.Parameters.AddWithValue("@FromDate",  Convert.ToDateTime(paramobj.FromDate).ToString("MM/dd/yyyy"));

            if (paramobj.ToDate != null)
                cmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(paramobj.ToDate).ToString("MM/dd/yyyy"));
            
            cmd.Parameters.AddWithValue("@BranchID", branchId);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<AWBBookIssueList> objList = new List<AWBBookIssueList>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AWBBookIssueList obj = new AWBBookIssueList();
                    obj.AWBBOOKIssueID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["AWBBOOKIssueID"].ToString());
                    obj.TransDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["TransDate"].ToString()); // CommanFunctions.ParseDate(ds.Tables[0].Rows[i]["RecPayDate"].ToString());
                    obj.Documentno = ds.Tables[0].Rows[i]["Documentno"].ToString();
                    obj.AWBNOFrom = ds.Tables[0].Rows[i]["AWBNOFrom"].ToString();
                    obj.AWBNOTo = ds.Tables[0].Rows[i]["AWBNOTo"].ToString();
                    obj.BookNo = ds.Tables[0].Rows[i]["BookNo"].ToString();
                    obj.EmployeeName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static string GetMaxAWBBookIssueDocumentNo()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "select isnull(max(Isnull(cast(documentno as integer),0)) +1,1) From AWBBOOKIssue ";
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);            
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }

        }

        public static string GenerateAWBBookIssue(int AWBBookIssueID)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_GenerateAWBBookIssue"; 
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@AWBBookIssueID", AWBBookIssueID);                        
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

        #endregion

        #region "AWBPrepaid"

        public static DataTable DeleteAWBPrepaid(int PrepaidAWBID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteAWBPrepaid";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PrepaidAWBID", PrepaidAWBID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }


        }
        public static string GetMaxPrepaidAWBDocumentNo()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "select isnull(max(Isnull(cast(documentno as integer),0)) +1,1) From PrepaidAWB ";
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }

        }
        public static List<AWBPrepaidList> GetAWBPrepaid(int branchId)
        {
            AWBPrepaidSearch paramobj = (AWBPrepaidSearch)(HttpContext.Current.Session["AWBPrepaidSearch"]);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetAWBPrepaidDetails";
            cmd.CommandType = CommandType.StoredProcedure;

            if (paramobj.DocumentNo != null)
                cmd.Parameters.AddWithValue("@DocumentNo", paramobj.DocumentNo);

            if (paramobj.FromDate != null)
                cmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(paramobj.FromDate).ToString("MM/dd/yyyy"));

            if (paramobj.ToDate != null)
                cmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(paramobj.ToDate).ToString("MM/dd/yyyy"));

            cmd.Parameters.AddWithValue("@BranchID", branchId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<AWBPrepaidList> objList = new List<AWBPrepaidList>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AWBPrepaidList obj = new AWBPrepaidList();
                    obj.PrepaidAWBID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["PrepaidAWBID"].ToString());
                    obj.TransDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["TransDate"].ToString()); // CommanFunctions.ParseDate(ds.Tables[0].Rows[i]["RecPayDate"].ToString());
                    obj.Documentno = ds.Tables[0].Rows[i]["Documentno"].ToString();
                    obj.AWBNOFrom = ds.Tables[0].Rows[i]["AWBNOFrom"].ToString();
                    obj.AWBNOTo = ds.Tables[0].Rows[i]["AWBNOTo"].ToString();
                    obj.Reference = ds.Tables[0].Rows[i]["Reference"].ToString();
                    obj.CustomerName = ds.Tables[0].Rows[i]["CustomerName"].ToString();
                    
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static AWBInfo GetAWBInfo(string AWBNo)
        {
            AWBInfo obj = new AWBInfo();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GETAWBUsedStatus";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AWBNo", @AWBNo);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
                     
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    obj.ReferenceID = CommanFunctions.ParseInt(ds.Tables[0].Rows[0]["ReferenceID"].ToString());
                    obj.AWBNo = ds.Tables[0].Rows[0]["AWBNo"].ToString();
                    obj.Status = ds.Tables[0].Rows[0]["Status"].ToString();
                    obj.Mode = ds.Tables[0].Rows[0]["Mode"].ToString();

                    if (obj.Status != "Available")
                        return obj;

                    if (obj.Status == "Available" && (obj.Mode == "Prepaid"))
                    {
                        obj.CourierCharge = ds.Tables[0].Rows[0]["CourierCharge"] == DBNull.Value ? 0 : Convert.ToDecimal(ds.Tables[0].Rows[0]["CourierCharge"].ToString());
                    }
                    if (obj.Status == "Available" && (obj.Mode == "Prepaid" || obj.Mode == "NotPrepaid"))
                    {
                        obj.CustomerID = ds.Tables[0].Rows[0]["CustomerID"]== DBNull.Value ?0 : Convert.ToInt32(ds.Tables[0].Rows[0]["CustomerID"].ToString());
                        obj.CustomerName = ds.Tables[0].Rows[0]["CustomerName"].ToString();
                        obj.OriginLocation = ds.Tables[0].Rows[0]["OriginLocation"].ToString();
                        obj.DestinationLocation = ds.Tables[0].Rows[0]["DestinationLocation"].ToString();
                        obj.CountryName = ds.Tables[0].Rows[0]["CountryName"].ToString();
                        obj.CityName = ds.Tables[0].Rows[0]["CityName"].ToString();
                        obj.LocationName = ds.Tables[0].Rows[0]["LocationName"].ToString();
                        obj.LocationName = ds.Tables[0].Rows[0]["LocationName"].ToString();
                        obj.Phone = ds.Tables[0].Rows[0]["Phone"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["Phone"].ToString();
                        obj.Mobile = ds.Tables[0].Rows[0]["Mobile"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["Mobile"].ToString();
                        obj.Address1 = ds.Tables[0].Rows[0]["Address1"].ToString();
                        obj.Address2 = ds.Tables[0].Rows[0]["Address2"].ToString();
                        obj.Address3 = ds.Tables[0].Rows[0]["Address3"].ToString();
                        obj.PickupSubLocality = ds.Tables[0].Rows[0]["PickupSubLocality"].ToString();
                        obj.DeliverySubLocality = ds.Tables[0].Rows[0]["DeliverySubLocality"].ToString();
                        obj.OriginPlaceID = ds.Tables[0].Rows[0]["OriginPlaceID"].ToString();
                        obj.DestinationPlaceID = ds.Tables[0].Rows[0]["DestinationPlaceID"].ToString();
                    }
                    else
                    {
                        return obj;
                    }
                }
            }

            return obj;
           
        }
        public static string GenerateAWBPrepaid(int PrepaidAWBID)
        {
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SP_GenerateAWBPrepaid";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@PrepaidAWBID", PrepaidAWBID);
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

        public static string SavePrepaidAWBPosting(int DRSRecPayId, int FyearId, int BranchId, int CompanyId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DRSRecPayPosting";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayId", DRSRecPayId);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            return "ok";

        }
        public static List<AWBDetailVM> CheckAWBDuplicate(int AWBFrom,int AWBTo, int AWBBookIssueID,int PrePaidAWBID )
        {
            
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_CheckAWBDuplicate";
            cmd.CommandType = CommandType.StoredProcedure;            
            cmd.Parameters.AddWithValue("@AWBFrom", AWBFrom);
            cmd.Parameters.AddWithValue("@AWBTo", AWBTo);
            cmd.Parameters.AddWithValue("@AWBBookIssueID", AWBBookIssueID);
            cmd.Parameters.AddWithValue("@PrePaidAWBID", PrePaidAWBID);
            

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<AWBDetailVM> objList = new List<AWBDetailVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AWBDetailVM obj = new AWBDetailVM();
                    obj.AWBNo = ds.Tables[0].Rows[i]["AWBNo"].ToString();                    
                    objList.Add(obj);
                }
            }
            return objList;
        }
        #endregion

        #region "AWBBAtch"
        public static string GetMaxBathcNo()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetMaxAWBBatchNo";
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }

        }
        public static string SaveAWBBatch(int BATCHID  ,int BranchID,int AcCompanyID,int DepotID, int UserID , int FYearID, string Details)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
                cmd.CommandText = "SP_SaveAWBBatch";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchID", BATCHID);
                cmd.Parameters.AddWithValue("@BranchID", BranchID);
                cmd.Parameters.AddWithValue("@AcCompanyID", AcCompanyID);
                cmd.Parameters.AddWithValue("@DepotID", DepotID);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@FYearId", FYearID);
                cmd.Parameters.AddWithValue("@FormatForXMLitem", Details);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return "Ok";
                }
                else
                {
                    return "No AWB added";
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

        }
        #endregion
    }
}