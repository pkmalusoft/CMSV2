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
    
    public partial class tblAgent
    {
        public int ID { get; set; }
        public int AcCompanyID { get; set; }
        public string AgentCode { get; set; }
        public string Name { get; set; }
        public string ReferenceCode { get; set; }
        public string ContactPerson { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public Nullable<int> CountryID { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<int> LocationID { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public bool StatusActive { get; set; }
        public Nullable<decimal> CreditLimit { get; set; }
        public Nullable<bool> StatusGPA { get; set; }
        public Nullable<bool> StatusWalkIn { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> ZoneCategoryID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Nullable<int> AcHeadID { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> AgentType { get; set; }
    }
}
