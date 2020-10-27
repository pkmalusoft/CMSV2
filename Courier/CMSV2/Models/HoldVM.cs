// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.HoldVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using System;

namespace CMSV2.Models
{
  public class HoldVM
  {
    public int InScanID { get; set; }

    public string AWBNo { get; set; }

    public DateTime TransactionnDate { get; set; }
        public DateTime date { get; set; }

    public int? CollectedBy { get; set; }

    public decimal? Weight { get; set; }

    public string Pieces { get; set; }

    public Decimal? CourierCharges { get; set; }

    public int OriginID { get; set; }

    public string Consignee { get; set; }

    public int? DestinationID { get; set; }

    public string OriginCountry { get; set; }

    public string ConsigneeCountry { get; set; }

    public string ConsigneeID { get; set; }

    public string Consignor { get; set; }

    public string StatusPaymentMOde { get; set; }

    public string DestinationName { get; set; }

    public string OriginName { get; set; }

    public int HeldBy { get; set; }

    public DateTime HeldOn { get; set; }

    public string HeldResoan { get; set; }

    public string countryname { get; set; }
     public string CollectedByName { get; set; }

    public string CourierStatus { get; set; }
        public string Action { get; set; } //Create/Edit
        public string ActionType { get; set; } //Hold/Release
    }
}
