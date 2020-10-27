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
    
    public partial class EmployeeMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EmployeeMaster()
        {
            this.CustomerEnquiries = new HashSet<CustomerEnquiry>();
            this.ExportShipments = new HashSet<ExportShipment>();
            this.ExportShipments1 = new HashSet<ExportShipment>();
            this.ExportShipments2 = new HashSet<ExportShipment>();
            this.ExportShipments3 = new HashSet<ExportShipment>();
            this.ExportShipments4 = new HashSet<ExportShipment>();
            this.ExportShipments5 = new HashSet<ExportShipment>();
            this.ExportShipments6 = new HashSet<ExportShipment>();
            this.ExportShipments7 = new HashSet<ExportShipment>();
            this.JobEnquiries = new HashSet<JobEnquiry>();
            this.PurchaseInvoices = new HashSet<PurchaseInvoice>();
            this.PurchaseInvoices1 = new HashSet<PurchaseInvoice>();
            this.PurchaseOrders = new HashSet<PurchaseOrder>();
            this.Quotations = new HashSet<Quotation>();
            this.SalesInvoices = new HashSet<SalesInvoice>();
            this.SalesInvoices1 = new HashSet<SalesInvoice>();
        }
    
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public Nullable<bool> StatusActive { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string PassportNo { get; set; }
        public Nullable<System.DateTime> PassportExpiryDate { get; set; }
        public string VisaNo { get; set; }
        public Nullable<System.DateTime> VisaExpiryDate { get; set; }
        public Nullable<System.DateTime> JoinDate { get; set; }
        public Nullable<decimal> BasicSalary { get; set; }
        public Nullable<decimal> HRA { get; set; }
        public Nullable<decimal> Allowances { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public Nullable<bool> StatusCommission { get; set; }
        public string Mobile { get; set; }
        public Nullable<int> AcCompanyID { get; set; }
        public Nullable<bool> statusDefault { get; set; }
        public Nullable<bool> statusMobileLogin { get; set; }
        public string Password { get; set; }
        public Nullable<int> BranchID { get; set; }
        public string MobileDeviceID { get; set; }
        public string MobileDevicePwd { get; set; }
        public string ContactPerson { get; set; }
        public string WebSite { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<int> LocationID { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<decimal> CreditLimit { get; set; }
        public Nullable<int> ZoneCategoryID { get; set; }
        public Nullable<int> AcHeadID { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> TerminationDate { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> DepotID { get; set; }
        public Nullable<int> UserID { get; set; }
    
        public virtual CityMaster CityMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerEnquiry> CustomerEnquiries { get; set; }
        public virtual Designation Designation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExportShipment> ExportShipments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExportShipment> ExportShipments1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExportShipment> ExportShipments2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExportShipment> ExportShipments3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExportShipment> ExportShipments4 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExportShipment> ExportShipments5 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExportShipment> ExportShipments6 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExportShipment> ExportShipments7 { get; set; }
        public virtual LocationMaster LocationMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobEnquiry> JobEnquiries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseInvoice> PurchaseInvoices1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quotation> Quotations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesInvoice> SalesInvoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesInvoice> SalesInvoices1 { get; set; }
    }
}
