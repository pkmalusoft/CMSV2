using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMSV2.Models;
using CMSV2.DAL;
namespace CMSV2.Models
{
    public class OpennnigBalanceVM
    {
      
        public int AcHeadID { get; set; }
        public int  AcFinancialYearID { get; set; }
        public int  CrDr { get; set; }
        public decimal Amount { get; set; }
        public string AcHead { get; set; }
      

    }
}