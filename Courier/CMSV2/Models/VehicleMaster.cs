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
    
    public partial class VehicleMaster
    {
        public int VehicleID { get; set; }
        public string VehicleDescription { get; set; }
        public string RegistrationNo { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public Nullable<decimal> VehicleValue { get; set; }
        public Nullable<System.DateTime> ValueDate { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<System.DateTime> RegExpirydate { get; set; }
        public Nullable<int> AcCompanyID { get; set; }
        public string VehicleNo { get; set; }
    }
}
