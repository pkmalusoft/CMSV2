//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CMSV2.Models
{
    using System;
    
    public partial class GetCustomerReciepts_Result
    {
        public int RecPayID { get; set; }
        public Nullable<System.DateTime> RecPayDate { get; set; }
        public string DocumentNo { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<int> BusinessCentreID { get; set; }
        public string BankName { get; set; }
        public string ChequeNo { get; set; }
        public Nullable<System.DateTime> ChequeDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> AcJournalID { get; set; }
        public Nullable<bool> StatusRec { get; set; }
        public string StatusEntry { get; set; }
        public string StatusOrigin { get; set; }
        public Nullable<int> FYearID { get; set; }
        public Nullable<int> AcCompanyID { get; set; }
        public Nullable<decimal> EXRate { get; set; }
        public Nullable<decimal> FMoney { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<bool> IsTradingReceipt { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string CustomerName { get; set; }
    }
}
