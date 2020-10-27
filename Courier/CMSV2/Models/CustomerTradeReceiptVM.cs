using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class CustomerTradeReceiptVM
    {
        public string InvoiceNo { get; set; }
        public int SalesInvoiceDetailID { get; set; }
        public Nullable<int> SalesInvoiceID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string ProductName { get; set; }       
        public Nullable<decimal> NetValue { get; set; }
        public Nullable<int> JobID { get; set; }
        public string JobCode { get; set; }
        public string Description { get; set; }
        public DateTime? date { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public decimal? AmountReceived { get; set; }
        public decimal? Balance { get; set; }
        public string DateTime { get; set; }
        public decimal? AdjustmentAmount { get; set; }
    }
}