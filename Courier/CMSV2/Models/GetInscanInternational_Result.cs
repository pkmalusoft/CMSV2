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
    
    public partial class GetInscanInternational_Result
    {
        public int InScanInternationalID { get; set; }
        public Nullable<int> inscanID { get; set; }
        public System.DateTime ForwardingDate { get; set; }
        public string ForwardingAWBNo { get; set; }
        public string MAWBNo { get; set; }
        public string Flight { get; set; }
        public Nullable<System.DateTime> FlightDate { get; set; }
        public decimal ForwardingCharge { get; set; }
        public string FAgentName { get; set; }
    }
}
