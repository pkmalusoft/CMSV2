using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using CMSV2.Models;

namespace CMSV2.DAL
{
    public class ReceiptDAO
    {
        //CustomerInvoiceDetailForReceipt
        public static List<ReceiptVM> GetCustomerReceipts(int FYearId,DateTime FromDate,DateTime ToDate)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetAllRecieptsDetails";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@FYearId", FYearId);
            
            //cmd.Parameters.Add("@AcJournalDetailID", SqlDbType.Int);
            //cmd.Parameters["@AcJournalDetailID"].Value = AcJournalDetailID;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<ReceiptVM> objList = new List<ReceiptVM>();
            
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ReceiptVM obj = new ReceiptVM();
                    obj.RecPayID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["RecPayID"].ToString());
                    obj.RecPayDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["RecPayDate"].ToString()); // CommanFunctions.ParseDate(ds.Tables[0].Rows[i]["RecPayDate"].ToString());
                    obj.DocumentNo = ds.Tables[0].Rows[i]["DocumentNo"].ToString();
                    obj.PartyName= ds.Tables[0].Rows[i]["PartyName"].ToString();
                    obj.PartyName = ds.Tables[0].Rows[i]["PartyName"].ToString();
                    if (ds.Tables[0].Rows[i]["Amount"] == DBNull.Value)
                    {
                        obj.Amount = 0;
                    }
                    else
                    {
                        obj.Amount = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    }
                    obj.Currency= CommanFunctions.ParseDecimal( ds.Tables[0].Rows[i]["Currency"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<ReceiptVM> GetCustomerReceiptsByDate(string FromDate,string ToDate,int FyearID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetAllRecieptsDetailsByDate";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FromDate",Convert.ToDateTime(FromDate).ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@Todate", Convert.ToDateTime(ToDate).ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@FyearId", FyearID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<ReceiptVM> objList = new List<ReceiptVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ReceiptVM obj = new ReceiptVM();
                    obj.RecPayID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["RecPayID"].ToString());
                    obj.RecPayDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["RecPayDate"].ToString());
                    obj.DocumentNo = ds.Tables[0].Rows[i]["DocumentNo"].ToString();
                    obj.PartyName = ds.Tables[0].Rows[i]["PartyName"].ToString();
                    obj.PartyName = ds.Tables[0].Rows[i]["PartyName"].ToString();
                    if (ds.Tables[0].Rows[i]["Amount"] == DBNull.Value)
                    {
                        obj.Amount = 0;
                    }
                    else
                    {
                        obj.Amount = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    }
                    obj.Currency = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Currency"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<CustomerInvoiceDetailForReceipt> GetCustomerInvoiceDetailsForReciept(int CustomerID,string FromDate,string ToDate)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCustomerInvoiceDetailsForReciept";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters["@CustomerID"].Value = CustomerID;
            cmd.Parameters["@FromDate"].Value =Convert.ToDateTime(FromDate).Date;
            cmd.Parameters["@ToDate"].Value = Convert.ToDateTime(ToDate).Date;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<CustomerInvoiceDetailForReceipt> objList = new List<CustomerInvoiceDetailForReceipt>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    CustomerInvoiceDetailForReceipt obj = new CustomerInvoiceDetailForReceipt();
                    obj.InvoiceNo = ds.Tables[0].Rows[i]["InvoiceNo"].ToString();
                    obj.CustomerInvoiceID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["InvoiceNo"].ToString());                
                    obj.InvoiceDate = CommanFunctions.ParseDate(ds.Tables[0].Rows[i]["InvoiceDate"].ToString());
                    obj.CurrencyName = ds.Tables[0].Rows[i]["CurrencyName"].ToString();                    
                    
                    if (ds.Tables[0].Rows[i]["AmountToBeReceived"] == DBNull.Value)
                    {
                        obj.AmountToBeReceived = 0;
                    }
                    else
                    {
                        obj.AmountToBeReceived = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["AmountToBeReceived"].ToString());
                    }
                    
                    if (ds.Tables[0].Rows[i]["AmtPaidTillDate"] == DBNull.Value)
                    {
                        obj.AmtPaidTillDate = 0;
                    }
                    else
                    {
                        obj.AmtPaidTillDate = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["AmtPaidTillDate"].ToString());
                    }

                    if (ds.Tables[0].Rows[i]["Balance"] == DBNull.Value)
                    {
                        obj.Balance = 0;
                    }
                    else
                    {
                        obj.Balance = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Balance"].ToString());
                    }

                    if (ds.Tables[0].Rows[i]["Amount"] == DBNull.Value)
                    {
                        obj.Amount = 0;
                    }
                    else
                    {
                        obj.Amount = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    }


                    if (ds.Tables[0].Rows[i]["Advance"] == DBNull.Value)
                    {
                        obj.Advance = 0;
                    }
                    else
                    {
                        obj.Advance = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Advance"].ToString());
                    }
                    


                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static int AddCustomerRecieptPayment(CustomerRcieptVM RecPy, string UserID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_InsertRecPay";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayDate", RecPy.RecPayDate);
            cmd.Parameters.AddWithValue("@DocumentNo", RecPy.DocumentNo);
            cmd.Parameters.AddWithValue("@CustomerID", RecPy.CustomerID);
            //cmd.Parameters.AddWithValue("@SupplierID", RecPy.SupplierID);
            cmd.Parameters.AddWithValue("@BusinessCentreID", RecPy.BusinessCentreID);
            cmd.Parameters.AddWithValue("@BankName", RecPy.BankName);
            cmd.Parameters.AddWithValue("@ChequeNo", RecPy.ChequeNo);
            cmd.Parameters.AddWithValue("@ChequeDate", RecPy.ChequeDate);
            cmd.Parameters.AddWithValue("@Remarks", RecPy.Remarks);
            cmd.Parameters.AddWithValue("@AcJournalID", RecPy.AcJournalID);
            cmd.Parameters.AddWithValue("@StatusRec", RecPy.StatusRec);
            cmd.Parameters.AddWithValue("@StatusEntry", RecPy.StatusEntry);
            cmd.Parameters.AddWithValue("@StatusOrigin", RecPy.StatusOrigin);
            cmd.Parameters.AddWithValue("@FYearID", RecPy.FYearID);
            cmd.Parameters.AddWithValue("@AcCompanyID", RecPy.AcCompanyID);
            cmd.Parameters.AddWithValue("@EXRate", RecPy.EXRate);
            cmd.Parameters.AddWithValue("@FMoney", RecPy.FMoney);
            cmd.Parameters.AddWithValue("@UserID", RecPy.UserID);
            cmd.Parameters.AddWithValue("@EntryTime", CommanFunctions.GetCurrentDateTime());
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count>0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }


        }

        public static List<CustomerReceivable> SPGetAllLocalCurrencyCustRecievable(int FinancialyearId)
        {
            List<CustomerReceivable> crecs = new List<CustomerReceivable>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_InsertRecPay";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AcFinancialYearID", FinancialyearId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    CustomerReceivable crec = new CustomerReceivable();
                    crec.InvoiceId = Convert.ToInt32(ds.Tables[0].Rows[i]["InvoiceId"].ToString());
                    crec.Receivable = Convert.ToDecimal(ds.Tables[0].Rows[i]["Receivable"].ToString());
                    crecs.Add(crec);
                }
            }
            else
            {
                
            }

            return crecs;
        }

       
        //public static CustomerRcieptVM GetRecPayByRecpayID(int RecpayID)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    CustomerRcieptVM cust = new CustomerRcieptVM();
        //    cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
        //    cmd.CommandText = "SP_GetCustomerRecieptByRecPayID";
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    cmd.Parameters["@RecPayID"].Value = RecpayID;
        //    if (RecpayID <= 0)
        //        return new CustomerRcieptVM();
        //    var query = Context1.SP_GetCustomerRecieptByRecPayID(RecpayID);

        //    if (query != null)
        //    {
        //        var item = query.FirstOrDefault();
        //        cust.RecPayDate = item.RecPayDate;
        //        cust.DocumentNo = item.DocumentNo;
        //        cust.CustomerID = item.CustomerID;

        //        var cashOrBankID = (from t in Context1.AcHeads where t.AcHead1 == item.BankName select t.AcHeadID).FirstOrDefault();
        //        cust.CashBank = (cashOrBankID).ToString();
        //        cust.ChequeBank = (cashOrBankID).ToString();
        //        cust.ChequeNo = item.ChequeNo;
        //        cust.ChequeDate = item.ChequeDate;
        //        cust.Remarks = item.Remarks;
        //        cust.EXRate = item.EXRate;
        //        cust.FMoney = item.FMoney;
        //        cust.RecPayID = item.RecPayID;
        //        cust.SupplierID = item.SupplierID;
        //        cust.AcJournalID = item.AcJournalID;
        //        cust.StatusEntry = item.StatusEntry;

        //        var a = (from t in Context1.RecPayDetails where t.RecPayID == RecpayID select t.CurrencyID).FirstOrDefault();
        //        cust.CurrencyId = Convert.ToInt32(a.HasValue ? a.Value : 0);



        //    }

        //    else
        //    {
        //        return new CustomerRcieptVM();
        //    }

        //    return cust;
        //}

        public static string SP_GetMaxPVID()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetMaxPVID";
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
        public static int InsertRecpayDetailsForCust(int RecPayID, int InvoiceID, int JInvoiceID, decimal Amount, string Remarks, string StatusInvoice, bool StatusAdvance, string statusReceip, string InvDate, string InvNo, int CurrencyID, int invoiceStatus, int JobID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_InsertRecPayDetailsForCustomer";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayID", RecPayID);
            cmd.Parameters.AddWithValue("@InvoiceID", InvoiceID);
            cmd.Parameters.AddWithValue("@Amount", Amount);
            cmd.Parameters.AddWithValue("@Remarks", Remarks);
            cmd.Parameters.AddWithValue("@StatusInvoice",StatusInvoice);
            cmd.Parameters.AddWithValue("@StatusAdvance", StatusAdvance);
            cmd.Parameters.AddWithValue("@statusReceipt", statusReceip);
            cmd.Parameters.AddWithValue("@InvDate", InvDate);
            cmd.Parameters.AddWithValue("@InvNo", InvNo);
            cmd.Parameters.AddWithValue("@CurrencyID", CurrencyID);            
            cmd.Parameters.AddWithValue("@invoiceStatus", invoiceStatus);
            //cmd.Parameters.AddWithValue("@JobID",  JobID);
                        
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }

                                   
        }

        public static void InsertJournalOfCustomer(int RecpayID, int fyearId)
        {
            //SP_InsertJournalEntryForRecPay
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_InsertJournalEntryForRecPay";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayID", RecpayID);
            cmd.Parameters.AddWithValue("@AcFinnancialYearId",fyearId);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            
            //Context1.SP_InsertJournalEntryForRecPay(RecpayID, fyaerId);
        }

        //public int InsertRecpayDetailsForSup(int RecPayID, int InvoiceID, int JInvoiceID, decimal Amount, string Remarks, string StatusInvoice, bool StatusAdvance, string statusReceip, string InvDate, string InvNo, int CurrencyID, int invoiceStatus, int JobID)
        //{
        //    //todo:fix to run by sethu
        //    int query = Context1.SP_InsertRecPayDetailsForSupplier(RecPayID, InvoiceID, Amount, Remarks, StatusInvoice, StatusAdvance, statusReceip, InvDate, InvNo, CurrencyID, invoiceStatus, JobID);

        //    return query;
        //}

        
        //Delete Manifest Receipt
        public static DataTable DeleteManifestReceipt(int RecPayID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteManifestReceipts";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayID", RecPayID);
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
        public static DataTable DeleteCustomerReceipt(int RecPayID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteCustomerReciepts";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayID", RecPayID);
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

        public static DataTable DeleteAccountHead(int AcheadId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteAcHead";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AcHeadId", AcheadId);
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
        public static DataTable DeleteCustomer(int CustomerId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteCustomer";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
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
        public static DataTable DeleteInvoice(int InvoiceId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteCustomerInvoice";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerInvoiceId", InvoiceId);
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

        public static DataTable DeleteInscan(int InscanId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteInscan";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InScanId", InscanId);
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
        
        //Quick Inscan
        public static DataTable DeleteDepotInscan(int InscanId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteDepotInscan";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@QuickInScanId", InscanId);
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
        public static DataTable DeleteSupplierPayments(int RecPayID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteSupplierPayments";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayID", RecPayID);
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

        public static DataTable DeleteDomesticCOD(int ReceiptId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteDomesticCODReciepts";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ReceiptID", ReceiptId);
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
        public static DataTable DeleteDRS(int DRSID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteDRS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DRSID", DRSID);
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

        public static DataTable DeleteSupplier(int SupplierId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteSupplier";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierId", SupplierId);
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
        public static List<ReceiptAllocationDetailVM> GetAWBAllocation(List<ReceiptAllocationDetailVM> list, int InvoiceId, decimal Amount,int RecpayId)
        {
            try

            {

                if (list == null)
                    list = new List<ReceiptAllocationDetailVM>();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
                cmd.CommandText = "SP_GetInvoiceAWBAllocation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
                cmd.Parameters.AddWithValue("@ReceivedAmount", Amount);
                cmd.Parameters.AddWithValue("@RecPayId",  RecpayId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow drrow = ds.Tables[0].Rows[i];
                        ReceiptAllocationDetailVM item = new ReceiptAllocationDetailVM();
                        item.ID= Convert.ToInt32(drrow["ID"].ToString());
                        item.CustomerInvoiceId = Convert.ToInt32(drrow["CustomerInvoiceId"].ToString());
                        item.CustomerInvoiceDetailID = Convert.ToInt32(drrow["CustomerInvoiceDetailID"].ToString());
                        item.InScanID = Convert.ToInt32(drrow["InScanId"].ToString());
                        item.RecPayID = Convert.ToInt32(drrow["RecPayID"].ToString());
                        item.RecPayDetailID = Convert.ToInt32(drrow["RecPayDetailID"].ToString());
                        item.CustomerInvoiceDetailID = Convert.ToInt32(drrow["CustomerInvoiceDetailID"].ToString());
                        item.AWBNo = drrow["AWBNo"].ToString();
                        item.AWBDate = Convert.ToDateTime(drrow["AWBDate"].ToString()).ToString("dd-MM-yyyy");
                        item.TotalAmount = Convert.ToDecimal(drrow["TotalAmount"].ToString());
                        item.ReceivedAmount = Convert.ToDecimal(drrow["ReceivedAmount"].ToString());
                        item.PendingAmount = Convert.ToDecimal(drrow["PendingAmount"].ToString());
                        item.AllocatedAmount = Convert.ToDecimal(drrow["AllocatedAmount"].ToString());
                        item.Allocated = Convert.ToBoolean(drrow["Allocated"].ToString());

                        list.Add(item);

                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }

            return list;
        }

        #region "ExportCODReceipt"
        public static List<ExportShipmentVM> GetManifestId(int AgentId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetManifestId";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AgentId", AgentId);
            

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<ExportShipmentVM> objList = new List<ExportShipmentVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ExportShipmentVM obj = new ExportShipmentVM();
                    obj.ID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.ManifestNumber = ds.Tables[0].Rows[i]["ManifestNumber"].ToString();                    
                    objList.Add(obj);
                }
            }
            return objList;
        }
        #endregion
        #region "DRSREceipt"

        public static DataTable DeleteDRSRecPay(int DRSRecPayId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteDRSRecpay";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DRSRecPayId", DRSRecPayId);
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

        public static string SaveDRSRecPayPosting(int DRSRecPayId,int FyearId,int BranchId,int CompanyId)
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
        public static int AddDRSReceipt(DRSReceiptVM RecPy, string UserID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_InsertDRSRecPay";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayDate", RecPy.DRSRecPayDate);
            cmd.Parameters.AddWithValue("@DocumentNo", RecPy.DocumentNo);
            cmd.Parameters.AddWithValue("@DRSID", RecPy.DRSID);
            cmd.Parameters.AddWithValue("@BankName", RecPy.BankName);
            cmd.Parameters.AddWithValue("@ChequeNo", RecPy.ChequeNo);
            cmd.Parameters.AddWithValue("@ChequeDate", RecPy.ChequeDate);
            cmd.Parameters.AddWithValue("@Remarks", RecPy.Remarks);
            cmd.Parameters.AddWithValue("@AcJournalID", RecPy.AcJournalID);            
            cmd.Parameters.AddWithValue("@StatusEntry", RecPy.StatusEntry);            
            cmd.Parameters.AddWithValue("@FYearID", RecPy.FYearID);
            cmd.Parameters.AddWithValue("@AcCompanyID", RecPy.AcCompanyID);
            cmd.Parameters.AddWithValue("@EXRate", RecPy.EXRate);
            cmd.Parameters.AddWithValue("@FMoney", RecPy.ReceivedAmount);
            cmd.Parameters.AddWithValue("@UserID", RecPy.UserID);
            cmd.Parameters.AddWithValue("@DeliveredBy", RecPy.DeliveredBy);
            cmd.Parameters.AddWithValue("@BranchId", RecPy.BranchId);
            cmd.Parameters.AddWithValue("@DRSBased", RecPy.DRSBased);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }


        }
        public static string GetMaxDRSReceiptNO()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetMaxDRSReceiptNo";
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

        public static List<DRRVM> GetDRRList(int FYearId, DateTime FromDate, DateTime ToDate)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetDRRList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@FYearId", FYearId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<DRRVM> objList = new List<DRRVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DRRVM obj = new DRRVM();
                    obj.DRRID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["DRRID"].ToString());
                    obj.DRRDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["DRRDate"].ToString()); // CommanFunctions.ParseDate(ds.Tables[0].Rows[i]["RecPayDate"].ToString());
                    obj.DRSReceiptNo = ds.Tables[0].Rows[i]["DRSRecpayNo"].ToString();
                    obj.DRSNo = ds.Tables[0].Rows[i]["DRSNO"].ToString();
                    obj.DRSReceiptDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["DRSRecPayDate"].ToString()).ToString("dd-MM-yyyy"); // CommanFunctions.ParseDate(ds.Tables[0].Rows[i]["RecPayDate"].ToString());
                    obj.ReconciledAmount =Convert.ToDecimal(ds.Tables[0].Rows[i]["ReconciledAmount"].ToString());                  
                    
                    objList.Add(obj);
                }
            }
            return objList;
        }
        public static List<DRSReceiptVM> GetDRSReceipts(int FYearId, DateTime FromDate, DateTime ToDate)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetAllDRSRecieptsDetails";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@FYearId", FYearId);
         
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<DRSReceiptVM> objList = new List<DRSReceiptVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DRSReceiptVM obj = new DRSReceiptVM();
                    obj.DRSRecPayID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["DRSRecPayID"].ToString());
                    obj.DRSRecPayDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["DRSRecPayDate"].ToString()); // CommanFunctions.ParseDate(ds.Tables[0].Rows[i]["RecPayDate"].ToString());
                    obj.DocumentNo = ds.Tables[0].Rows[i]["DocumentNo"].ToString();
                    obj.DRSNo= ds.Tables[0].Rows[i]["DRSNo"].ToString();
                    obj.CourierEmpName = ds.Tables[0].Rows[i]["CourierName"].ToString();
                    obj.Status  = ds.Tables[0].Rows[i]["Status"].ToString();
                    if (ds.Tables[0].Rows[i]["Amount"] == DBNull.Value)
                    {
                        obj.ReceivedAmount = 0;
                    }
                    else
                    {
                       obj.ReceivedAmount = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                    }
                    //obj.Currency = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Currency"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<DRSReceiptDetailVM> GetDRSAWB(int DRSID,int RecPayId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetDRSAWB";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DRSID", DRSID );
            cmd.Parameters.AddWithValue("@DRSRecPayID",RecPayId);           

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<DRSReceiptDetailVM> objList = new List<DRSReceiptDetailVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DRSReceiptDetailVM obj = new DRSReceiptDetailVM();
                    obj.DRSRecPayID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["DRSRecPayID"].ToString());
                    obj.DRSRecPayDetailID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["DRSRecPayDetailID"].ToString());
                    obj.InScanID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["InScanID"].ToString());
                    obj.AWBDate = ds.Tables[0].Rows[i]["AWBDate"].ToString();
                    obj.AWBNO = ds.Tables[0].Rows[i]["AWBNo"].ToString();                    
                    obj.CourierCharge= CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["CourierCharge"].ToString());
                    obj.MaterialCost= CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["MaterialCost"].ToString());
                    obj.CCReceived = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["CCReceived"].ToString());
                    obj.MCReceived = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["MCReceived"].ToString());
                    obj.TotalAmount = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["TotalAmount"].ToString());
                    obj.Discount = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Discount"].ToString());
                    //obj.Currency = CommanFunctions.ParseDecimal(ds.Tables[0].Rows[i]["Currency"].ToString());
                    objList.Add(obj);
                }
            }
            return objList;
        }
        #endregion

        #region "supplierInvoice"

        public static DataTable DeleteSupplierInvoice(int InvoiceId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_DeleteSupplierInvoice";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierInvoiceId", InvoiceId);
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
        public static string SP_GetMaxSINo()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetMaxSupplierInvoiceNo";
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
        #endregion

        #region "Supplierpayment"
        public static int AddSupplierRecieptPayment(CustomerRcieptVM RecPy, string UserID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_InsertSupplierRecPay";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayDate", RecPy.RecPayDate);
            cmd.Parameters.AddWithValue("@DocumentNo", RecPy.DocumentNo);
            //cmd.Parameters.AddWithValue("@CustomerID", RecPy.CustomerID);
            cmd.Parameters.AddWithValue("@SupplierID", RecPy.SupplierID);
            cmd.Parameters.AddWithValue("@BusinessCentreID", RecPy.BusinessCentreID);
            cmd.Parameters.AddWithValue("@BankName", RecPy.BankName);
            cmd.Parameters.AddWithValue("@ChequeNo", RecPy.ChequeNo);
            cmd.Parameters.AddWithValue("@ChequeDate", RecPy.ChequeDate);
            cmd.Parameters.AddWithValue("@Remarks", RecPy.Remarks);
            cmd.Parameters.AddWithValue("@AcJournalID", RecPy.AcJournalID);
            cmd.Parameters.AddWithValue("@StatusRec", RecPy.StatusRec);
            cmd.Parameters.AddWithValue("@StatusEntry", RecPy.StatusEntry);
            cmd.Parameters.AddWithValue("@StatusOrigin", RecPy.StatusOrigin);
            cmd.Parameters.AddWithValue("@FYearID", RecPy.FYearID);
            cmd.Parameters.AddWithValue("@AcCompanyID", RecPy.AcCompanyID);
            cmd.Parameters.AddWithValue("@EXRate", RecPy.EXRate);
            cmd.Parameters.AddWithValue("@FMoney", RecPy.FMoney);
            cmd.Parameters.AddWithValue("@UserID", RecPy.UserID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }


        }

        public static void InsertJournalOfSupplier(int RecpayID, int fyearId)
        {
            //SP_InsertJournalEntryForRecPay
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_InsertJournalEntryForSupplierRecPay";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayID", RecpayID);
            cmd.Parameters.AddWithValue("@AcFinnancialYearId", fyearId);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            //Context1.SP_InsertJournalEntryForRecPay(RecpayID, fyaerId);
        }
        public static int InsertRecpayDetailsForSupplier(int RecPayID, int InvoiceID, int JInvoiceID, decimal Amount, string Remarks, string StatusInvoice, bool StatusAdvance, string statusReceip, string InvDate, string InvNo, int CurrencyID, int invoiceStatus, int JobID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_InsertRecPayDetailsForSupplier";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecPayID", RecPayID);
            cmd.Parameters.AddWithValue("@InvoiceID", InvoiceID);
            cmd.Parameters.AddWithValue("@Amount", Amount);
            cmd.Parameters.AddWithValue("@Remarks", Remarks);
            cmd.Parameters.AddWithValue("@StatusInvoice", StatusInvoice);
            cmd.Parameters.AddWithValue("@StatusAdvance", StatusAdvance);
            cmd.Parameters.AddWithValue("@statusReceipt", statusReceip);
            cmd.Parameters.AddWithValue("@InvDate", InvDate);
            cmd.Parameters.AddWithValue("@InvNo", InvNo);
            cmd.Parameters.AddWithValue("@CurrencyID", CurrencyID);
            cmd.Parameters.AddWithValue("@invoiceStatus", invoiceStatus);
            //cmd.Parameters.AddWithValue("@InScanId", JobID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }


        }

        #endregion
        #region "Reconc"
        public static int SaveReconc(int DRRID,DateTime  DRRDate, int DRSID, int DRSRecpayID, decimal ReconcAmount, int courierId ,int BranchId,int FYearId ,int UserId,string Details)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_SaveReconc";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DRRID", DRRID);
            cmd.Parameters.AddWithValue("@DRRDate",DRRDate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@DRSID", DRSID);
            cmd.Parameters.AddWithValue("@DRSRecPayID", DRSRecpayID);            
            cmd.Parameters.AddWithValue("@CourierId", courierId);
            cmd.Parameters.AddWithValue("@Amount", ReconcAmount);
            cmd.Parameters.AddWithValue("@BranchId", BranchId);
            cmd.Parameters.AddWithValue("@FyearID", FYearId);
            cmd.Parameters.AddWithValue("@UserID", UserId);
            cmd.Parameters.AddWithValue("@FormatForXMLitem", Details);
            //cmd.Parameters.AddWithValue("@InScanId", JobID);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }


        }

        #endregion


        #region customersupplieradvance
        public static decimal SP_GetCustomerAdvance(int CustomerId, int RecPayId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCustomerAdvance";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }

        }

        public static decimal SP_GetCustomerInvoiceReceived(int CustomerId, int InvoiceId, int RecPayId, int CreditNoteId, string Type)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetCustomerInvoiceReceived";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@CreditNoteId", CreditNoteId);
            cmd.Parameters.AddWithValue("@Type", Type);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }

        }

        public static decimal SP_GetSupplierAdvance(int SupplierId, int RecPayId, int FyearId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetSupplierAdvance";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierId", SupplierId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@FYearId", FyearId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }

        }

        public static decimal SP_GetSupplierInvoicePaid(int SupplierId, int InvoiceId, int RecPayId, int DebitNoteId, string Type)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetSupplierInvoicePaid";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierId", SupplierId);
            cmd.Parameters.AddWithValue("@InvoiceId", InvoiceId);
            cmd.Parameters.AddWithValue("@RecPayId", RecPayId);
            cmd.Parameters.AddWithValue("@DebitNoteId", DebitNoteId);
            cmd.Parameters.AddWithValue("@Type", Type);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            //int query = Context1.SP_InsertRecPay(RecPy.RecPayDate, RecPy.DocumentNo, RecPy.CustomerID, RecPy.SupplierID, RecPy.BusinessCentreID, RecPy.BankName, RecPy.ChequeNo, RecPy.ChequeDate, RecPy.Remarks, RecPy.AcJournalID, RecPy.StatusRec, RecPy.StatusEntry, RecPy.StatusOrigin, RecPy.FYearID, RecPy.AcCompanyID, RecPy.EXRate, RecPy.FMoney, Convert.ToInt32(UserID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }

        }
        #endregion

        #region MCPayment
        public static List<DRRRecPayDetailVM> GetMCAWBPending( MCDatePicker datePicker, int MCPVVID, int BranchId,int FyearId )
        {
            List<DRRRecPayDetailVM> list = new List<DRRRecPayDetailVM>();
            try
            {
                              SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
                cmd.CommandText = "SP_GetMCRAWBPending";
                cmd.CommandType = CommandType.StoredProcedure;
                if (datePicker != null)
                {
                    if (datePicker.SearchOption == null)
                    {
                        datePicker.SearchOption = "Date";
                    }
                    cmd.Parameters.AddWithValue("@SearchOption", datePicker.SearchOption);
                    if (datePicker.AWBNo == null)
                        datePicker.AWBNo = "";
                    cmd.Parameters.AddWithValue("@AWBNo", datePicker.AWBNo);
                    if (datePicker.Shipper == null)
                    {
                        datePicker.Shipper = "";
                    }
                    cmd.Parameters.AddWithValue("@Shipper", datePicker.Shipper);
                    cmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(datePicker.FromDate1).ToString("MM/dd/yyyy"));
                    cmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(datePicker.ToDate1).ToString("MM/dd/yyyy"));
                }
                cmd.Parameters.AddWithValue("@MCPVID", MCPVVID);
                cmd.Parameters.AddWithValue("@FYearID", FyearId);
                cmd.Parameters.AddWithValue("@BranchID",BranchId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow drrow = ds.Tables[0].Rows[i];
                        DRRRecPayDetailVM item = new DRRRecPayDetailVM();
                        item.InScanID = Convert.ToInt32(drrow["InScanID"].ToString());
                        item.AWBNo = drrow["AWBNo"].ToString();
                        item.AWBDateTime = Convert.ToDateTime(drrow["AWBDate"].ToString());
                        item.AWBDate= Convert.ToDateTime(drrow["AWBDate"].ToString()).ToString("dd/MM/yyyy");
                        item.ConsigneeName = drrow["Consignee"].ToString();
                        item.ConsignorName = drrow["Consignor"].ToString();
                        item.MaterialCost = Convert.ToDecimal(drrow["MaterialCost"].ToString());
                        item.AmountReceived = Convert.ToDecimal(drrow["ReceivedAmount"].ToString());
                        item.AmountPaid = Convert.ToDecimal(drrow["PaidAmount"].ToString());
                        item.AmountPending = Convert.ToDecimal(drrow["PendingAmount"].ToString());
                        item.Amount= Convert.ToDecimal(drrow["PayingAmount"].ToString());
                        item.AdjustmentAmount = Convert.ToDecimal(drrow["AdjustmentAmount"].ToString());
                        list.Add(item);

                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }

            return list;
        }
        #endregion

        #region CodeGeneration
        public static void ReSaveSupplierCode()
        {
            //SP_InsertJournalEntryForRecPay
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_ReSaveSupplierCode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            //Context1.SP_InsertJournalEntryForRecPay(RecpayID, fyaerId);
        }
        public static void ReSaveEmployeeCode()
        {
            //SP_InsertJournalEntryForRecPay
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_ReSaveEmployeeCode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            //Context1.SP_InsertJournalEntryForRecPay(RecpayID, fyaerId);
        }

        public static void ReSaveCustomerCode()
        {
            //SP_InsertJournalEntryForRecPay
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_ReSaveCustomerCode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
        }

        public static string GetMaxCustomerCode(string CustomerName)
        {
            //SP_InsertJournalEntryForRecPay
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetMaxCustomerCode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerName", CustomerName);
            cmd.Connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                string cutomercode = ds.Tables[0].Rows[0][0].ToString();

                return cutomercode;
            }
            else
            {
                return "";
            }
        }
        #endregion
    }
}