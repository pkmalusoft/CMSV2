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
    
    public partial class GetDrsCourierRecordByDrsID_Result
    {
        public string DRSNo { get; set; }
        public System.DateTime DRSDate { get; set; }
        public Nullable<int> DeliveredBy { get; set; }
        public int CheckedBy { get; set; }
        public int DRSID { get; set; }
        public string cname { get; set; }
        public string rname { get; set; }
        public Nullable<bool> StatusInbound { get; set; }
        public string StatusDRS { get; set; }
        public Nullable<int> VehicleID { get; set; }
        public Nullable<int> AcCompanyID { get; set; }
    }
}
