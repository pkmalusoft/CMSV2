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
    
    public partial class ColoaderAllocation
    {
        public int ColoaderAllocationID { get; set; }
        public Nullable<int> CustomerInvoiceID { get; set; }
        public Nullable<int> InscanID { get; set; }
        public Nullable<double> Weight { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<decimal> Revenue { get; set; }
    }
}
