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
    
    public partial class AWBDetail
    {
        public int ID { get; set; }
        public string AWBNo { get; set; }
        public Nullable<int> AWBBookIssueID { get; set; }
        public Nullable<int> PrepaidAWBID { get; set; }
        public Nullable<int> InScanID { get; set; }
        public bool UserStatus { get; set; }
    }
}