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
    
    public partial class AgentReceiptByAgentID_Result
    {
        public string InvoiceNo { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public Nullable<decimal> PayableAmount { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> CollectionCharges { get; set; }
        public decimal CollectionCharges1 { get; set; }
        public Nullable<decimal> DeliveryCharges { get; set; }
        public decimal CollectedAmount { get; set; }
        public Nullable<decimal> Balance { get; set; }
    }
}
