// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.PODStatusVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using System;

namespace CMSV2.Models
{
  public class PODStatusVM
  {
    public int InscanID { get; set; }

    public string AWBNo { get; set; }

    public DateTime Date { get; set; }

    public int? CollectedBy { get; set; }

    public double? StatedWeight { get; set; }

    public string Pieces { get; set; }

    public Decimal CourierCharges { get; set; }

    public string Consignor { get; set; }

    public int ConsignorCountryID { get; set; }

    public string Consignee { get; set; }

    public int? CosigneeCountryID { get; set; }

    public string StatusPaymentMOde { get; set; }

    public int AWBStatusID { get; set; }

    public int PODID { get; set; }

    public int? CourierStatusID { get; set; }

    public string CourierStatus { get; set; }

    public string RecevierName { get; set; }

    public string Remarks { get; set; }
  }
}
