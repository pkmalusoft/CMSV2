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
    
    public partial class CourierService
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CourierService()
        {
            this.CustomerRates = new HashSet<CustomerRate>();
            this.CustomerRates1 = new HashSet<CustomerRate>();
            this.ForwardingAgentRates = new HashSet<ForwardingAgentRate>();
        }
    
        public int CourierServiceID { get; set; }
        public string CourierService1 { get; set; }
        public string CourierServiceCode { get; set; }
        public int CourierDescriptionID { get; set; }
        public string InScanType { get; set; }
        public bool StatusDefaultD { get; set; }
        public bool StatusDefaultI { get; set; }
        public Nullable<bool> StatusDefault { get; set; }
        public Nullable<int> CourierMode { get; set; }
        public Nullable<int> TransportMode { get; set; }
        public Nullable<bool> CBMBasedCharges { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerRate> CustomerRates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerRate> CustomerRates1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForwardingAgentRate> ForwardingAgentRates { get; set; }
    }
}
