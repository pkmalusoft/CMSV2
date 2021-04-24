// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.DRSVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using System;
using System.Collections.Generic;

namespace CMSV2.Models
{
  public class DRSVM
  {
    public int DRSID { get; set; }

    public string DRSNo { get; set; }

    public DateTime DRSDate { get; set; }

    public int? DeliveredBy { get; set; }

    public int CheckedBy { get; set; }

    public Decimal? TotalCourierCharge { get; set; }
        public Decimal? TotalMaterialCost{ get; set; }

        public int? VehicleID { get; set; }

    public string StatusDRS { get; set; }

    public int? AcCompanyID { get; set; }

    public bool? StatusInbound { get; set; }

    public string DrsType { get; set; }

    public string Depot { get; set; }

    public List<DRSAWB> lst { get; set; }

    public string Deliver { get; set; }

    public string vehicle { get; set; }

    public Decimal Cash { get; set; }

    public Decimal MaterialCost { get; set; }
  }
}
