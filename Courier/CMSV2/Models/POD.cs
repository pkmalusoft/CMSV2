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
    
    public partial class POD
    {
        public int PODID { get; set; }
        public int InScanID { get; set; }
        public string ReceiverName { get; set; }
        public Nullable<System.DateTime> ReceivedTime { get; set; }
        public Nullable<int> CourierStatusID { get; set; }
        public Nullable<int> DRRConsignmentID { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> UpdationDate { get; set; }
        public Nullable<bool> IsSkyLarkUpdate { get; set; }
    
        public virtual DRRConsignment DRRConsignment { get; set; }
    }
}
