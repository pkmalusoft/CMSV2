using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class ShipmentInvoiceVM:ShipmentInvoice
    {
        public string EnteredBy { get; set; }
    //    public string MAWB { get; set; }
        public decimal TaxPercent { get; set; }
        public DateTime ImportDate { get; set; }
        public string AWBDetails { get; set; }
        
        public List<ShipmentInvoiceDetailVM> Details { get; set; }

    }

    public class ShipmentInvoiceDetailVM : ShipmentInvoiceDetail
    {
        public decimal CustomValue { get; set; }
        public bool AWBChecked { get; set; }
        public string AWBNo { get; set;}
        public string Shipper { get; set; }
        public string BagNo { get; set; }
        public int CurrencyID { get; set; }
    }

    public class ShipmentInvoiceSearch
    {
        public string AWBNo { get; set; }
        public int ShipmentInvoiceDetailId { get; set; }
        public int ShipmentInvoiceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<ShipmentInvoiceVM> Details { get; set; }
    }
}