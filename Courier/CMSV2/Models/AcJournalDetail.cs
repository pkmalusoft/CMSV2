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
    
    public partial class AcJournalDetail
    {
        public int ID { get; set; }
        public int AcJournalDetailID { get; set; }
        public Nullable<int> AcJournalID { get; set; }
        public Nullable<int> AcHeadID { get; set; }
        public Nullable<int> AnalysisHeadID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> PartyID { get; set; }
        public string PartyType { get; set; }
        public Nullable<decimal> FAmount { get; set; }
        public Nullable<decimal> ExRate { get; set; }
        public Nullable<bool> Lock { get; set; }
        public Nullable<decimal> TaxPercent { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public bool AmountIncludingTax { get; set; }
        public Nullable<int> SupplierId { get; set; }
    }
}
