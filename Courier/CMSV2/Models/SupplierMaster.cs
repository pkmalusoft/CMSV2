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
    
    public partial class SupplierMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SupplierMaster()
        {
            this.PurchaseInvoices = new HashSet<PurchaseInvoice>();
            this.PurchaseOrders = new HashSet<PurchaseOrder>();
            this.tblSupplierAdjustments = new HashSet<tblSupplierAdjustment>();
            this.SupplierInvoices = new HashSet<SupplierInvoice>();
        }
    
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ReferenceCode { get; set; }
        public string ContactPerson { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<bool> StatusActive { get; set; }
        public Nullable<int> AcCompanyID { get; set; }
        public string MobileNo { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> SupplierTypeID { get; set; }
        public string RegistrationNo { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> MaxCreditDays { get; set; }
        public Nullable<double> MaxCreditLimit { get; set; }
        public string RevenueTypeIds { get; set; }
        public Nullable<int> ExportCode { get; set; }
        public Nullable<bool> StatusInventory { get; set; }
        public Nullable<bool> StatusReserved { get; set; }
        public string POBoxNo { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> AcHeadID { get; set; }
    
        public virtual BranchMaster BranchMaster { get; set; }
        public virtual CurrencyMaster CurrencyMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual SupplierType SupplierType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSupplierAdjustment> tblSupplierAdjustments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SupplierInvoice> SupplierInvoices { get; set; }
    }
}
