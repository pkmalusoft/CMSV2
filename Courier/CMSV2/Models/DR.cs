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
    
    public partial class DR
    {
        public int DRSID { get; set; }
        public string DRSNo { get; set; }
        public System.DateTime DRSDate { get; set; }
        public Nullable<int> DeliveredBy { get; set; }
        public Nullable<int> VehicleID { get; set; }
        public string StatusDRS { get; set; }
        public Nullable<int> AcCompanyID { get; set; }
        public Nullable<bool> StatusInbound { get; set; }
        public string DrsType { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<decimal> TotalMaterialCost { get; set; }
        public Nullable<int> DRSRecPayId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> FYearId { get; set; }
        public Nullable<decimal> TotalCourierCharge { get; set; }
    }
}
