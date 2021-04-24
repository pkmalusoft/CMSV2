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
    
    public partial class SalesInvoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalesInvoice()
        {
            this.SalesInvoiceDetails = new HashSet<SalesInvoiceDetail>();
            this.tblSalesInvoiceOtherCharges = new HashSet<tblSalesInvoiceOtherCharge>();
        }
    
        public int SalesInvoiceID { get; set; }
        public string SalesInvoiceNo { get; set; }
        public Nullable<System.DateTime> SalesInvoiceDate { get; set; }
        public string Reference { get; set; }
        public string LPOReference { get; set; }
        public Nullable<System.DateTime> LPODate { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<int> QuotationID { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<int> CreditDays { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<int> AcJournalID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<bool> StatusDiscountAmt { get; set; }
        public Nullable<decimal> OtherCharges { get; set; }
        public Nullable<int> PaymentTermID { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> FYearID { get; set; }
        public Nullable<int> SalesManID { get; set; }
        public string DocumentNo { get; set; }
        public Nullable<System.DateTime> DocumentDate { get; set; }
        public string PaymentType { get; set; }
        public Nullable<int> AcHeadID { get; set; }
    
        public virtual AcFinancialYear AcFinancialYear { get; set; }
        public virtual AcJournalMaster AcJournalMaster { get; set; }
        public virtual BranchMaster BranchMaster { get; set; }
        public virtual CurrencyMaster CurrencyMaster { get; set; }
        public virtual CustomerMaster CustomerMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesInvoiceDetail> SalesInvoiceDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSalesInvoiceOtherCharge> tblSalesInvoiceOtherCharges { get; set; }
    }
}
