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
    
    public partial class Report_ProfitAndLossAccount_Result
    {
        public string AcCategory { get; set; }
        public Nullable<int> AcCategoryID { get; set; }
        public string AcGroup { get; set; }
        public Nullable<int> ParentGroupID { get; set; }
        public string ParenatGroup { get; set; }
        public Nullable<int> AcGroupID { get; set; }
        public Nullable<decimal> DrAmount { get; set; }
        public Nullable<decimal> CrAmount { get; set; }
        public Nullable<int> AcHeadID { get; set; }
        public string AcHead { get; set; }
    }
}
