using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class AcJournalDetailVM
    {
        public int AcHeadID { get; set; }
        public string AcHead { get; set; }
        public string Rem { get; set; }
        public decimal Amt { get; set; }
        public decimal TaxPercent { get; set; }

        public decimal TaxAmount { get; set; }
        public int AcJournalDetID { get; set; }        
        public bool AmountIncludingTax { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }

        public List<AcExpenseAllocationVM> AcExpAllocationVM { get; set; }
    }
}