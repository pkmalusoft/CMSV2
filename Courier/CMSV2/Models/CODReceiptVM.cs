using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class CODReceiptVM : CODReceipt
    {
        
        public string AgentName {get;set;}
        public string AcHeadName { get; set; }
        public int[] SelectedValues { get; set; }
        public decimal allocatedtotalamount { get; set; }

        public string CurrencyName { get; set; }
        public List<CODReceiptDetailVM> ReceiptDetails { get; set; }
    }
    public class CODReceiptDetailVM :CODReceiptDetail
    {
        public string ManifestNumber { get; set; }
        public DateTime? AWBDate { get; set; }
    }
    
}