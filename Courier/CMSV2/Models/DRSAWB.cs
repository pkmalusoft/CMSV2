// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.DRSAWB
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

namespace CMSV2.Models
{
  public class DRSAWB
  {
    public int InScanID { get; set; }
        public int ShipmentDetailID { get; set; }
    public string AWB { get; set; }
        public string Consignor { get; set; }
        public string Consignee { get; set; }

    public string City { get; set; }

    public string Phone { get; set; }

    public string Address { get; set; }

    public decimal COD { get; set; }
        public decimal MaterialCost { get; set; }

        public decimal CustomValue { get; set; }
    }
}
