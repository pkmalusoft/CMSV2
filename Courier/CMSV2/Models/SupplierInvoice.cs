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
    
    public partial class SupplierInvoice
    {
        public int SupplierInvoiceID { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> AccompanyID { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> AcJOurnalID { get; set; }
        public Nullable<int> AcDiscJournalID { get; set; }
        public Nullable<int> AcHeadID { get; set; }
        public Nullable<int> FyearID { get; set; }
    }
}
