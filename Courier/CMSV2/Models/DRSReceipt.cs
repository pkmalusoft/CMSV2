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
    
    public partial class DRSReceipt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DRSReceipt()
        {
            this.DRRs = new HashSet<DRR>();
        }
    
        public int DRSReceiptID { get; set; }
        public Nullable<System.DateTime> DRSReceiptDate { get; set; }
        public string DRSNo { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> VehicleID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public Nullable<int> AcJournalID { get; set; }
        public string StatusRunSheet { get; set; }
        public Nullable<int> User1 { get; set; }
        public Nullable<int> FYearID { get; set; }
        public Nullable<int> AcCompanyID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DRR> DRRs { get; set; }
    }
}
