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
    
    public partial class proDRSDetailed_Result
    {
        public int DRRID { get; set; }
        public System.DateTime DRSDate { get; set; }
        public string DRSNo { get; set; }
        public Nullable<int> DeliveredBy { get; set; }
        public string EmployeeName { get; set; }
        public decimal NetCash { get; set; }
        public decimal MCost { get; set; }
        public decimal CourierChargeSales { get; set; }
        public decimal PKPCADSales { get; set; }
        public decimal InvoiceReceipts { get; set; }
        public decimal OtherReceipts { get; set; }
        public decimal Expense { get; set; }
    }
}
