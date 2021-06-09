using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class CreditNoteVM
    {
        public string VoucherNo { get; set; }
        public int CreditNoteID { get; set; }
        public string CreditNoteNo { get; set; }
        public DateTime Date { get; set; }
        public int CustomerID { get; set; }
        public  bool ForInvoice { get; set; }
        public int AcHeadID { get; set; }
        public int AcJournalID { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string JobNO { get; set; }
        public string InvoiceNo { get; set; }

        public int InvoiceID { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public bool TradingInvoice { get; set; }

    }
}