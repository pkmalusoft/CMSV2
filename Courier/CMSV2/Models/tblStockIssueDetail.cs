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
    
    public partial class tblStockIssueDetail
    {
        public long ID { get; set; }
        public Nullable<int> StockIssueID { get; set; }
        public string AWBNo { get; set; }
        public string Status { get; set; }
        public Nullable<int> InscanID { get; set; }
    
        public virtual tblStockIssue tblStockIssue { get; set; }
    }
}
