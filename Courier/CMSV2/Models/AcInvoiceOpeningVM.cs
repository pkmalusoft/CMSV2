using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class AcInvoiceOpeningVM :AcOPInvoiceMaster
    {
        public string PartyName { get; set; }
        public string PartyType { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public List<AcInvoiceOpeningDetailVM> InvoiceDetailVM { get; set; }
    }
    public class AcInvoiceOpeningDetailVM : AcOPInvoiceDetail
    {
        public bool IsDeleted { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}