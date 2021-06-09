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
    using System.Collections.Generic;
    
    public partial class AgentInvoice
    {
        public int AgentInvoiceID { get; set; }
        public string InvoiceNo { get; set; }
        public int CustomerID { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public Nullable<decimal> InvoiceTax { get; set; }
        public Nullable<int> AcJournalID { get; set; }
        public bool StatusClose { get; set; }
        public Nullable<int> AcCompanyID { get; set; }
        public Nullable<decimal> OtherCharge { get; set; }
        public Nullable<decimal> FuelPer { get; set; }
        public Nullable<decimal> AdminPer { get; set; }
        public Nullable<decimal> FuelAmt { get; set; }
        public Nullable<decimal> AdminAmt { get; set; }
        public Nullable<double> ChargeableWT { get; set; }
        public string CalogiRef { get; set; }
        public Nullable<System.DateTime> calogidate { get; set; }
        public Nullable<bool> statusColoader { get; set; }
        public Nullable<decimal> CostAmt { get; set; }
        public Nullable<decimal> Revenue { get; set; }
        public Nullable<decimal> CCharge { get; set; }
        public Nullable<decimal> InvoiceTotal { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> AcFinancialYearID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    }
}
