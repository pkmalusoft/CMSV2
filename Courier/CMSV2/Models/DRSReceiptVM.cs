// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.DRSReceiptVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using System;
using System.Collections.Generic;

namespace CMSV2.Models
{
  public class DRSReceiptVM :DRSRecPay
  {
    public string DRSNo { get; set; }        
        public string CashBank { get; set; }
        //public string BankName { get; set; }
        public int DeliveredBy { get; set; }
        public DateTime DRSDate { get; set; }
        public decimal DRSAmount { get; set; }
        public string ChequeBank { get; set; }
        public string CourierEmpName { get; set; }
   
        //public Nullable<System.DateTime> ChequeDate { get; set; }
        public List<DRSReceiptDetailVM> Details { get; set; }
  }

    public class DRSReceiptDetailVM : DRSRecPayDetail
    {
                public string AWBDate { get; set; }
        public decimal TotalAmount { get; set; } 
        public decimal Balance { get; set; }
    }
    
    public class DRRVM :DRR
    {
        public string DRSNo { get; set; }
        public DateTime DRSDate { get; set; }
        public string DRSReceiptNo { get; set; }
        public string DRSReceiptDate { get; set; }
        public DateTime ReceiptDate { get; set; }
        public decimal ReceivedAmount { get; set; }
        public int DeliveredBy { get; set; }
        public List<DRRDetailVM> Details { get; set; }

    }

    public class DRRDetailVM
    {
        public int? DRRID { get; set; }
        public int? ReferenceId { get; set; }
        public string Type { get; set; }
        public string Reference { get; set; }
        public decimal? COD { get; set; }
        public decimal? Discount { get; set; }
        public decimal? MCReceived { get; set; }
        public decimal? PKPCash { get; set; }
        public decimal? Receipt { get; set; }
        public decimal? Expense { get; set; }

        public decimal? Total { get; set; }
    }
}
