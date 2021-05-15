// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.CustomerRateTypeVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

namespace CMSV2.Models
{
  public class CustomerRateTypeVM
  {
    public int CustomerRateTypeID { get; set; }

    public string CustomerRateType { get; set; }

    public int ZoneCategoryID { get; set; }

    public bool StatusDefault { get; set; }

    public string ZoneCategory { get; set; }

    public decimal CourierCharge { get; set; }
  }
}
