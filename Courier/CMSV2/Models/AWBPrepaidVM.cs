

using System;
using System.Collections.Generic;

namespace CMSV2.Models
{
    public class AWBPrepaidVM : PrepaidAWB
    {
        public string CustomerName { get; set; }
        public string CashBank { get; set; }
        public string ChequeBank { get; set; }        
        public List<AWBDetailVM> Details { get; set; }
    }
    public class AWBDetailVM : AWBDetail
    {

    }

    public class AWBPrepaidSearch {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string DocumentNo { get; set; }
        public List<AWBPrepaidList> Details { get; set; }
    }

    public class AWBPrepaidList : PrepaidAWB { 
        public string EmployeeName { get; set; }
        public string CustomerName { get; set; }        
        
    }
}
