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
    
    public partial class AcCompany
    {
        public int AcCompanyID { get; set; }
        public string AcCompany1 { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> CountryID { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<int> LocationID { get; set; }
        public string KeyPerson { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string PhoneNo3 { get; set; }
        public string PhoneNo4 { get; set; }
        public string MobileNo1 { get; set; }
        public string MobileNo2 { get; set; }
        public string Website { get; set; }
        public string CompanyPrefix { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<bool> IsAWBAutoGenrated { get; set; }
        public string AWBFormat { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<int> IncrementBy { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string LocationName { get; set; }
        public string InvoicePrefix { get; set; }
        public string InvoiceFormat { get; set; }
        public byte[] CompanyLogo { get; set; }
        public string LogoFileName { get; set; }
        public string Logo { get; set; }
        public bool EnableAPI { get; set; }
        public bool EnableCashCustomerInvoice { get; set; }
    
        public virtual CityMaster CityMaster { get; set; }
        public virtual CurrencyMaster CurrencyMaster { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual LocationMaster LocationMaster { get; set; }
    }
}
