using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class DebitNoteVM
    {
        public int DebitNoteId { get; set; }
        public string DebitNoteNo { get; set; }
        public DateTime? Date { get; set; }
        public int SupplierID { get; set; }
        public int AcHeadID { get; set; }
        public int InvoiceID{ get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate{ get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Amount { get; set; }

        public string JobNo { get; set; }
        public string SupplierName { get; set; }
        public string Remarks { get; set; }
        public int AcJournalID { get; set; }
    }
}