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
    
    public partial class AgentInvoiceDetail
    {
        public int AgentInvoiceDetailID { get; set; }
        public Nullable<int> AgentInvoiceID { get; set; }
        public string AWBNo { get; set; }
        public Nullable<decimal> CourierCharge { get; set; }
        public Nullable<int> InScanInternationalDetailID { get; set; }
        public string StatusPaymentMode { get; set; }
        public Nullable<int> InscanID { get; set; }
        public Nullable<int> TaxPercentage { get; set; }
        public Nullable<decimal> OtherCharge { get; set; }
        public Nullable<decimal> FuelSurcharge { get; set; }
        public Nullable<decimal> CustomCharge { get; set; }
        public Nullable<int> RecPayStatus { get; set; }
        public Nullable<int> RecPayDetailId { get; set; }
        public Nullable<decimal> NetValue { get; set; }
    }
}