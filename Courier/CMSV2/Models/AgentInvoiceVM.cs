using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class AgentInvoiceVM :AgentInvoice
    {
        public string CustomerName { get; set; }
        public List<AgentInvoiceDetailVM> Details { get; set; }
    }
    public class AgentDatePicker
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string MAWB { get; set; }
    }
        public class AgentInvoiceDetailVM : AgentInvoiceDetail
    {
        public string ConsigneeName { get; set; }
        public DateTime AWBDateTime { get; set; }
        public string ConsigneeCountryName { get; set; }
        public bool AWBChecked { get; set; }
    }

    public class AgentInvoiceSearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string InvoiceNo { get; set; }
        public List<AgentInvoiceVM> Details { get; set; }
    }
}