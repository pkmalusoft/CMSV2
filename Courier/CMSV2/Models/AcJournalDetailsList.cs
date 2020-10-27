using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace  CMSV2.Models
{
    public class AcJournalDetailsList
    {

        public int acHeadID { get; set; }
        public decimal Amount { get; set; }
        public short IsDebit { get; set; }
        public string AcRemark { get; set; }
        public string AcHead { get; set; }
        public int AcJournalDetID { get; set; }

        public string drcr { get; set; }

    }
}