using System;

namespace CMSV2.Models
{
    public class OpeningBalanceVM
    {

        public int AcHeadID { get; set; }
        public int AcFinancialYearID { get; set; }
        public int CrDr { get; set; }
        public decimal Amount { get; set; }
        public string AcHead { get; set; }
        public DateTime Opdate { get; set; }

        public string Remarks { get; set; }
        public int? PartyId { get; set; }
        public string StatusSDSC { get; set; }
        public string BranchId { get; set; }

    }

    public class OpeningInvoiceSearch
    {
        public string OpeningDate { get; set; }
        public string InvoiceType { get; set; }
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public string Remarks { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }




}