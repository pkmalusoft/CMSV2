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
    
    public partial class UserRegistration
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string EmailId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> RoleID { get; set; }
        public Nullable<bool> isLogin { get; set; }
        public Nullable<System.DateTime> loginExpiryDate { get; set; }
    }
}
