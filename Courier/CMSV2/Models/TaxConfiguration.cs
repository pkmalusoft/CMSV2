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
    
    public partial class TaxConfiguration
    {
        public int TaxConfigurationID { get; set; }
        public int CourierMoveMentID { get; set; }
        public int ParcelTypeId { get; set; }
        public decimal TaxPercentage { get; set; }
        public int SalesHeadID { get; set; }
        public Nullable<decimal> MinimumRate { get; set; }
        public Nullable<int> AcCompanyID { get; set; }
        public Nullable<System.DateTime> EffectFromDate { get; set; }
    }
}
