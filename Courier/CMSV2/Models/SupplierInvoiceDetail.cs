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
    
    public partial class SupplierInvoiceDetail
    {
        public int SupplierInvoiceDetailID { get; set; }
        public int SupplierInvoiceID { get; set; }
        public string Particulars { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public int CurrencyID { get; set; }
        public decimal CurrencyAmount { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> TaxPercentage { get; set; }
        public decimal Value { get; set; }
        public Nullable<int> RecPayStatus { get; set; }
        public Nullable<int> RecPayDetailId { get; set; }
        public Nullable<int> AcHeadID { get; set; }
    
        public virtual SupplierInvoice SupplierInvoice { get; set; }
    }
}