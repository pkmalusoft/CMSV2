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

}
