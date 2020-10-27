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
    
    public partial class QuotationDetail
    {
        public int QuotationDetailID { get; set; }
        public Nullable<int> QuotationID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> ItemUnitID { get; set; }
        public Nullable<int> CustomerRateTypeID { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<int> JobID { get; set; }
        public string Description { get; set; }
    
        public virtual ItemUnit ItemUnit { get; set; }
        public virtual Product Product { get; set; }
        public virtual Quotation Quotation { get; set; }
    }
}
