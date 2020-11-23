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
    
    public partial class PurchaseOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseOrder()
        {
            this.PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
        }
    
        public int PurchaseOrderID { get; set; }
        public string PurchaseOrderNo { get; set; }
        public Nullable<System.DateTime> PurchaseOrderDate { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<bool> StatusDiscountAmt { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> FYearID { get; set; }
    
        public virtual EmployeeMaster EmployeeMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual SupplierMaster SupplierMaster { get; set; }
    }
}
