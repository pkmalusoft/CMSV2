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
    
    public partial class RecPayDetail
    {
        public int RecPayDetailID { get; set; }
        public Nullable<int> RecPayID { get; set; }
        public Nullable<int> InvoiceID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remarks { get; set; }
        public string StatusInvoice { get; set; }
        public Nullable<bool> StatusAdvance { get; set; }
        public string statusReceipt { get; set; }
        public string InvDate { get; set; }
        public string InvNo { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<bool> Lock { get; set; }
        public Nullable<int> JobID { get; set; }
        public Nullable<decimal> AdjustmentAmount { get; set; }
        public Nullable<int> AcOPInvoiceDetailID { get; set; }
    }
}
