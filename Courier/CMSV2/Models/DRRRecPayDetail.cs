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
    
    public partial class DRRRecPayDetail
    {
        public int RecPayDetailID { get; set; }
        public Nullable<int> RecPayID { get; set; }
        public Nullable<int> InScanID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> AdjustmentAmount { get; set; }
    }
}
