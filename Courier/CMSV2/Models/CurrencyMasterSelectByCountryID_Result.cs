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
    
    public partial class CurrencyMasterSelectByCountryID_Result
    {
        public int CurrencyID { get; set; }
        public string CurrencyName { get; set; }
        public string Symbol { get; set; }
        public Nullable<short> NoOfDecimals { get; set; }
        public string MonetaryUnit { get; set; }
        public Nullable<int> CountryID { get; set; }
        public Nullable<bool> StatusBaseCurrency { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public string CurrencyCode { get; set; }
    }
}
