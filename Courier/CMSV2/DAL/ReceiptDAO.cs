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
            cmd.Parameters.AddWithValue("@JobID",  JobID);
                        
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
        public static List<ReceiptAllocationDetailVM> GetAWBAllocation(List<ReceiptAllocationDetailVM> list, int InvoiceId, decimal Amount,int RecpayId)
        {
            try
            {
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
    }
}