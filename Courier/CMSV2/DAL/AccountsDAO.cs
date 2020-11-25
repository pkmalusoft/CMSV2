using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Collections;
using CMSV2.Models;
using System.Configuration;

namespace CMSV2.DAL
{
    public class AccountsDAO
    {

        public static int InsertOrUpdateAcBankDetails(AcBankDetail ObjectAcBankDetail, int isupdate)
        {
            int iReturn = 0;
            SqlCommand cmd = new SqlCommand();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            cmd.Connection = new SqlConnection(strConnString);
            cmd.CommandText = "SP_InsertOrUpdateAcBankDetails";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@AcBankDetailID", SqlDbType.Int);
            cmd.Parameters["@AcBankDetailID"].Value = ObjectAcBankDetail.AcBankDetailID;

            cmd.Parameters.Add("@AcJournalID", SqlDbType.Int);
            cmd.Parameters["@AcJournalID"].Value = ObjectAcBankDetail.AcJournalID;

            cmd.Parameters.Add("@BankName", SqlDbType.NVarChar);
            cmd.Parameters["@BankName"].Value = ObjectAcBankDetail.BankName;

            cmd.Parameters.Add("@ChequeNo", SqlDbType.NVarChar);
            cmd.Parameters["@ChequeNo"].Value = ObjectAcBankDetail.ChequeNo;

            if (ObjectAcBankDetail.ChequeDate != null)
            {
                cmd.Parameters.Add("@ChequeDate", SqlDbType.DateTime);
                cmd.Parameters["@ChequeDate"].Value = ObjectAcBankDetail.ChequeDate;
            }
            cmd.Parameters.Add("@PartyName", SqlDbType.NVarChar);
            cmd.Parameters["@PartyName"].Value = ObjectAcBankDetail.PartyName;

            cmd.Parameters.Add("@StatusTrans", SqlDbType.NVarChar);
            cmd.Parameters["@StatusTrans"].Value = ObjectAcBankDetail.StatusTrans;

            cmd.Parameters.Add("@IsUpdate", SqlDbType.Int);
            cmd.Parameters["@IsUpdate"].Value = isupdate;
            if (ObjectAcBankDetail.StatusReconciled != null)
            {
                cmd.Parameters.Add("@StatusReconciled", SqlDbType.Bit);
                cmd.Parameters["@StatusReconciled"].Value = ObjectAcBankDetail.StatusReconciled;
            }
            if (ObjectAcBankDetail.ValueDate != null)
            {
                cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime);
                cmd.Parameters["@ValueDate"].Value = ObjectAcBankDetail.ValueDate;
            }
            try
            {
                cmd.Connection.Open();
                iReturn = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {

            }
            return iReturn;
        }

        public static List<AcJournalDetailsVM> GetAcJournalDetails(int AcJournalID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SELECT ah.AcHead,aj.Amount, aj.Remarks FROM AcJournalDetail as aj INNER JOIN AcHead as ah on aj.AcHeadID=ah.AcHeadID WHERE aj.AcJournalID = @AcJournalID";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add("@AcJournalID", SqlDbType.Int);
            cmd.Parameters["@AcJournalID"].Value = AcJournalID;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<AcJournalDetailsVM> objList = new List<AcJournalDetailsVM>();
            AcJournalDetailsVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new AcJournalDetailsVM();
                    obj.AcHead = ds.Tables[0].Rows[i]["AcHead"].ToString();
                    obj.Amount = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    obj.Remarks = ds.Tables[0].Rows[i]["Remarks"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static int UpdateAcJournalDetail(AcJournalDetail ObjectAcJournalDetail)
        {
            int iReturn = 0;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "UPDATE AcJournalDetail SET AcJournalID=@AcJournalID,AcHeadID=@AcHeadID,AnalysisHeadID=@AnalysisHeadID,Amount=@Amount,Remarks=@Remarks,BranchID=@BranchID,AmountIncludingTax=@AmountIncludingTax,SupplierId=@SupplierId WHERE AcJournalDetailID = @AcJournalDetailID";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add("@AcJournalDetailID", SqlDbType.Int);
            cmd.Parameters["@AcJournalDetailID"].Value = ObjectAcJournalDetail.AcJournalDetailID;

            cmd.Parameters.Add("@AcJournalID", SqlDbType.Int);
            cmd.Parameters["@AcJournalID"].Value = ObjectAcJournalDetail.AcJournalID;

            cmd.Parameters.Add("@AcHeadID", SqlDbType.Int);
            cmd.Parameters["@AcHeadID"].Value = ObjectAcJournalDetail.AcHeadID;

            cmd.Parameters.Add("@AnalysisHeadID", SqlDbType.Int);
            cmd.Parameters["@AnalysisHeadID"].Value = ObjectAcJournalDetail.AnalysisHeadID;

            cmd.Parameters.Add("@Amount", SqlDbType.Money);
            cmd.Parameters["@Amount"].Value = ObjectAcJournalDetail.Amount;

            cmd.Parameters.Add("@Remarks", SqlDbType.VarChar);
            cmd.Parameters["@Remarks"].Value = ObjectAcJournalDetail.Remarks;

            cmd.Parameters.Add("@BranchID", SqlDbType.Int);
            cmd.Parameters["@BranchID"].Value = ObjectAcJournalDetail.BranchID;

            cmd.Parameters.Add("@AmountIncludingTax", SqlDbType.Bit);
            cmd.Parameters["@AmountIncludingTax"].Value = ObjectAcJournalDetail.AmountIncludingTax;

            cmd.Parameters.Add("@SupplierId", SqlDbType.Int);
            cmd.Parameters["@SupplierId"].Value = ObjectAcJournalDetail.SupplierId;


            try
            {
                cmd.Connection.Open();
                iReturn = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {

            }

            return iReturn;
        }

        public static int DeleteAcJournalDetail(int AcJournalDetailID)
        {
            int iReturn = 0;
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd2.CommandText = "DELETE FROM AcAnalysisHeadAllocation WHERE AcjournalDetailID = @AcjournalDetailID";
            cmd2.CommandType = CommandType.Text;
            cmd2.Parameters.Add("@AcjournalDetailID", SqlDbType.Int);
            cmd2.Parameters["@AcjournalDetailID"].Value = AcJournalDetailID;
            try
            {
                cmd2.Connection.Open();
                iReturn = cmd2.ExecuteNonQuery();
                cmd2.Connection.Close();
            }
            catch (Exception ex)
            {

            }


            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "DELETE FROM AcJournalDetail WHERE AcJournalDetailID = @AcJournalDetailID";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add("@AcJournalDetailID", SqlDbType.Int);
            cmd.Parameters["@AcJournalDetailID"].Value = AcJournalDetailID;

            try
            {
                cmd.Connection.Open();
                iReturn = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {

            }

            return iReturn;
        }
        public static int InsertAcJournalDetail(AcJournalDetail ObjectAcJournalDetail)
        {
            int iReturn = 0;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "INSERT INTO AcJournalDetail(AcJournalDetailID,AcJournalID,AcHeadID,Amount,Remarks,BranchID,AmountIncludingTax,SupplierId) VALUES(@AcJournalDetailID,@AcJournalID,@AcHeadID,@Amount,@Remarks,@BranchID,@AmountIncludingTax,@SupplierId)";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add("@AcJournalDetailID", SqlDbType.Int);
            cmd.Parameters["@AcJournalDetailID"].Value = ObjectAcJournalDetail.AcJournalDetailID;

            cmd.Parameters.Add("@AcJournalID", SqlDbType.Int);
            cmd.Parameters["@AcJournalID"].Value = ObjectAcJournalDetail.AcJournalID;

            cmd.Parameters.Add("@AcHeadID", SqlDbType.Int);
            cmd.Parameters["@AcHeadID"].Value = ObjectAcJournalDetail.AcHeadID;

            //   cmd.Parameters.Add("@AnalysisHeadID", SqlDbType.Int);
            //    cmd.Parameters["@AnalysisHeadID"].Value = ObjectAcJournalDetail.AnalysisHeadID;

            cmd.Parameters.Add("@Amount", SqlDbType.Money);
            cmd.Parameters["@Amount"].Value = ObjectAcJournalDetail.Amount;

            cmd.Parameters.Add("@Remarks", SqlDbType.VarChar);
            cmd.Parameters["@Remarks"].Value = ObjectAcJournalDetail.Remarks;

            cmd.Parameters.Add("@BranchID", SqlDbType.Int);
            cmd.Parameters["@BranchID"].Value = ObjectAcJournalDetail.BranchID;

            cmd.Parameters.Add("@AmountIncludingTax", SqlDbType.Bit);
            cmd.Parameters["@AmountIncludingTax"].Value = ObjectAcJournalDetail.AmountIncludingTax;
            
            cmd.Parameters.Add("@SupplierId", SqlDbType.Int);
            cmd.Parameters["@SupplierId"].Value = ObjectAcJournalDetail.SupplierId;
            try
            {
                cmd.Connection.Open();
                iReturn = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {

            }

            return iReturn;
        }

        public static int UpdateAcAnalysisHeadAllocation(AcAnalysisHeadAllocation ObjectAcAnalysisHeadAllocation)
        {
            int iReturn = 0;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "UPDATE AcAnalysisHeadAllocation SET AcjournalDetailID=@AcjournalDetailID,AnalysisHeadID=@AnalysisHeadID,Amount=@Amount WHERE AcAnalysisHeadAllocationID=@AcAnalysisHeadAllocationID";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add("@AcAnalysisHeadAllocationID", SqlDbType.Int);
            cmd.Parameters["@AcAnalysisHeadAllocationID"].Value = ObjectAcAnalysisHeadAllocation.AcAnalysisHeadAllocationID;

            cmd.Parameters.Add("@AcjournalDetailID", SqlDbType.Int);
            cmd.Parameters["@AcjournalDetailID"].Value = ObjectAcAnalysisHeadAllocation.AcjournalDetailID;

            cmd.Parameters.Add("@AnalysisHeadID", SqlDbType.Int);
            cmd.Parameters["@AnalysisHeadID"].Value = ObjectAcAnalysisHeadAllocation.AnalysisHeadID;

            cmd.Parameters.Add("@Amount", SqlDbType.Money);
            cmd.Parameters["@Amount"].Value = ObjectAcAnalysisHeadAllocation.Amount;

            try
            {
                cmd.Connection.Open();
                iReturn = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {

            }
            return iReturn;
        }

        public static int InsertAcAnalysisHeadAllocation(AcAnalysisHeadAllocation ObjectAcAnalysisHeadAllocation)
        {
            int iReturn = 0;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "INSERT INTO AcAnalysisHeadAllocation(AcjournalDetailID,AnalysisHeadID,Amount) VALUES(@AcjournalDetailID,@AnalysisHeadID,@Amount)";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add("@AcjournalDetailID", SqlDbType.Int);
            cmd.Parameters["@AcjournalDetailID"].Value = ObjectAcAnalysisHeadAllocation.AcjournalDetailID;

            cmd.Parameters.Add("@AnalysisHeadID", SqlDbType.Int);
            cmd.Parameters["@AnalysisHeadID"].Value = ObjectAcAnalysisHeadAllocation.AnalysisHeadID;

            cmd.Parameters.Add("@Amount", SqlDbType.Money);
            cmd.Parameters["@Amount"].Value = ObjectAcAnalysisHeadAllocation.Amount;

            try
            {
                cmd.Connection.Open();
                iReturn = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {

            }
            return iReturn;
        }

        public static int DeleteAcAnalysisHeadAllocation(int AcAnalysisHeadAllocationID)
        {
            int iReturn = 0;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "DELETE FROM AcAnalysisHeadAllocation WHERE AcAnalysisHeadAllocationID=@AcAnalysisHeadAllocationID";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add("@AcAnalysisHeadAllocationID", SqlDbType.Int);
            cmd.Parameters["@AcAnalysisHeadAllocationID"].Value = AcAnalysisHeadAllocationID;

            try
            {
                cmd.Connection.Open();
                iReturn = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {

            }
            return iReturn;
        }

        public static List<AcAnalysisHeadAllocationVM> GetAcJDetailsExpenseAllocation(int AcJournalDetailID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "GetAcJDetailsExpenseAllocation";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@AcJournalDetailID", SqlDbType.Int);
            cmd.Parameters["@AcJournalDetailID"].Value = AcJournalDetailID;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<AcAnalysisHeadAllocationVM> objList = new List<AcAnalysisHeadAllocationVM>();
            AcAnalysisHeadAllocationVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new AcAnalysisHeadAllocationVM();
                    obj.AcAnalysisHeadAllocationID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["AcAnalysisHeadAllocationID"].ToString());
                    obj.AcjournalDetailID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["AcjournalDetailID"].ToString());
                    obj.AnalysisHeadID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["AnalysisHeadID"].ToString());
                    if (ds.Tables[0].Rows[i]["Amount"] == DBNull.Value)
                    {
                        obj.Amount = 0;
                    }
                    else
                    {
                        obj.Amount = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    }
                    obj.AnalysisHead = ds.Tables[0].Rows[i]["AnalysisHead"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<AcHeadSelectAllVM> GetAcHeadSelectAll(int BranchID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "AcHeadSelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@BranchID", SqlDbType.Int);
            cmd.Parameters["@BranchID"].Value = BranchID;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<AcHeadSelectAllVM> objList = new List<AcHeadSelectAllVM>();
            AcHeadSelectAllVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new AcHeadSelectAllVM();
                    obj.AcHeadID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["AcHeadID"].ToString());
                    obj.AcHeadKey = ds.Tables[0].Rows[i]["AcHeadKey"].ToString();
                    obj.AcHead = ds.Tables[0].Rows[i]["AcHead"].ToString();
                    obj.AcGroupID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["AcGroupID"].ToString());
                    obj.ParentID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["ParentID"].ToString());
                    obj.HeadOrder = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["HeadOrder"].ToString());
                    if (ds.Tables[0].Rows[i]["StatusHide"] == DBNull.Value)
                    {
                        obj.StatusHide = false;
                    }
                    else
                    {
                        obj.StatusHide = Convert.ToBoolean(ds.Tables[0].Rows[i]["StatusHide"].ToString());
                    }
                    obj.UserID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["UserID"].ToString());
                    obj.Prefix = ds.Tables[0].Rows[i]["Prefix"].ToString();
                    obj.AcGroup = ds.Tables[0].Rows[i]["AcGroup"].ToString();
                    obj.AccountType = ds.Tables[0].Rows[i]["AccountType"].ToString();
                    if (ds.Tables[0].Rows[i]["TaxApplicable"] == DBNull.Value)
                    {
                        obj.TaxApplicable = false;

                    }
                    else
                    {                        
                        obj.TaxApplicable = Convert.ToBoolean(ds.Tables[0].Rows[i]["TaxApplicable"].ToString());
                        if (obj.TaxApplicable==true)
                        {
                            obj.TaxPercent = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["TaxPercent"].ToString());
                        }
                        else
                        {
                            obj.TaxPercent = 0;
                        }
                        
                    }


                    objList.Add(obj);
                }
            }
            return objList;
        }


        public static List<AcJournalDetailVM> AcJournalDetailSelectByAcJournalID(int AcJournalID, string PaymentType)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "AcJournalDetailSelectByAcJournalID";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@AcJournalID", SqlDbType.Int);
            cmd.Parameters["@AcJournalID"].Value = AcJournalID;

            cmd.Parameters.Add("@PaymentType", SqlDbType.VarChar);
            cmd.Parameters["@PaymentType"].Value = PaymentType;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<AcJournalDetailVM> objList = new List<AcJournalDetailVM>();
            AcJournalDetailVM obj;
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new AcJournalDetailVM();
                    obj.AcJournalDetID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["AcJournalDetailID"].ToString());
                    obj.AcHeadID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["AcHeadID"].ToString());
                    obj.TaxPercent = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["TaxPercent"].ToString());
                    obj.TaxAmount = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["TaxAmount"].ToString());
                                        
                    if (ds.Tables[0].Rows[i]["AmountIncludingTax"]==null)
                    {
                        obj.AmountIncludingTax = false;                        
                    }
                    else
                    {
                        obj.AmountIncludingTax = Convert.ToBoolean(ds.Tables[0].Rows[i]["AmountIncludingTax"].ToString());
                    }

                    if (obj.AmountIncludingTax ==true && obj.TaxAmount>0)
                    {
                        obj.Amt = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Amount"].ToString())+ obj.TaxAmount;
                    }
                    else
                    {
                        obj.Amt = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    }
                    
                    obj.Rem = ds.Tables[0].Rows[i]["Remarks"].ToString();
                    obj.AcHead = ds.Tables[0].Rows[i]["AcHead"].ToString();
                    obj.SupplierID = Convert.ToInt32(ds.Tables[0].Rows[i]["SupplierId"].ToString());
                    obj.SupplierName = ds.Tables[0].Rows[i]["SupplierName"].ToString();

                    objList.Add(obj);
                }
            }
            return objList;
        }

        //index jv voucher book
        public static List<AcJournalMaster> AcJournalMasterSelect(int FYearId, int BranchID,DateTime FromDate,DateTime ToDate)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "AcJournalMasterSelectAllJVNew";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@FYearID", SqlDbType.Int);
            cmd.Parameters["@FYearID"].Value = FYearId;

            cmd.Parameters.Add("@BranchID", SqlDbType.Int);
            cmd.Parameters["@BranchID"].Value = BranchID;

            cmd.Parameters.Add("@FromDate", SqlDbType.VarChar);
            cmd.Parameters["@FromDate"].Value = FromDate.ToString("MM/dd/yyyy");

            cmd.Parameters.Add("@ToDate", SqlDbType.VarChar);
            cmd.Parameters["@ToDate"].Value = ToDate.ToString("MM/dd/yyyy");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<AcJournalMaster> objList = new List<AcJournalMaster>();
            AcJournalMaster obj;
            




            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new AcJournalMaster();
                    obj.AcJournalID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["AcJournalID"].ToString());
                    obj.ID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.VoucherNo = ds.Tables[0].Rows[i]["VoucherNo"].ToString();
                    obj.TransDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["TransDate"].ToString());
                    obj.Remarks = ds.Tables[0].Rows[i]["Remarks"].ToString();
                    obj.TransactionNo = ds.Tables[0].Rows[i]["TransactionNo"].ToString();
                    obj.Reference= ds.Tables[0].Rows[i]["Reference"].ToString();
                    obj.VoucherType= ds.Tables[0].Rows[i]["VoucherType"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }

        
        //Indexacbook page
        public static List<AcJournalMasterVM> AcJournalMasterSelectAll(int FYearId, int BranchID, DateTime FromDate, DateTime ToDate,string VoucherType)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "AcJournalMasterAll";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@FYearID", SqlDbType.Int);
            cmd.Parameters["@FYearID"].Value = FYearId;

            cmd.Parameters.Add("@BranchID", SqlDbType.Int);
            cmd.Parameters["@BranchID"].Value = BranchID;

            cmd.Parameters.Add("@FromDate", SqlDbType.VarChar);
            cmd.Parameters["@FromDate"].Value = FromDate.ToString("MM/dd/yyyy");

            cmd.Parameters.Add("@ToDate", SqlDbType.VarChar);
            cmd.Parameters["@ToDate"].Value = ToDate.ToString("MM/dd/yyyy");

            cmd.Parameters.Add("@VoucherType", SqlDbType.VarChar);
            cmd.Parameters["@VoucherType"].Value = VoucherType;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            List<AcJournalMasterVM> objList = new List<AcJournalMasterVM>();
            AcJournalMasterVM obj;

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj = new AcJournalMasterVM();
                    obj.AcJournalID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["AcJournalID"].ToString());
                    //obj.ID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.VoucherNo = ds.Tables[0].Rows[i]["VoucherNo"].ToString();
                    obj.TransDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["TransDate"].ToString());
                    obj.Remarks = ds.Tables[0].Rows[i]["Remarks"].ToString();
                    //obj.TransactionNo = ds.Tables[0].Rows[i]["TransactionNo"].ToString();
                    //obj.Reference = ds.Tables[0].Rows[i]["Reference"].ToString();
                    obj.Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    obj.VoucherType = ds.Tables[0].Rows[i]["VoucherType"].ToString();
                    objList.Add(obj);
                }
            }
            return objList;
        }
    }
}
    


