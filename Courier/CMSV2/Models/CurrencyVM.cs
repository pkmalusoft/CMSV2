// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.CurrencyVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

namespace CMSV2.Models
{
  public class CurrencyVM
  {
    public int CurrencyID { get; set; }

    public string CurrencyName { get; set; }

    public string Symbol { get; set; }

    public short NoOfDecimals { get; set; }

    public string MonetaryUnit { get; set; }

    public int CountryID { get; set; }

    public bool StatusBaseCurrency { get; set; }
  }
}
