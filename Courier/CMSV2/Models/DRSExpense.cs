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
    
    public partial class DRSExpense
    {
        public int DRSExpenseID { get; set; }
        public int DRSID { get; set; }
        public int ExpenseID { get; set; }
        public decimal Amount { get; set; }
    
        public virtual DR DR { get; set; }
    }
}
