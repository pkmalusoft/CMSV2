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
    
    public partial class PrepaidAWBInvoice
    {
        public int PrepaidAWBInvoiceID { get; set; }
        public string PrepaidAWBInvoiceNo { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public int CustomerID { get; set; }
        public Nullable<int> AcJournalID { get; set; }
        public Nullable<int> FYearID { get; set; }
        public Nullable<int> AcCompanyID { get; set; }
    }
}
