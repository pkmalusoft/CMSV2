// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.AgentDeliveryRateVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using System;
using System.Collections.Generic;

namespace CMSV2.Models
{
  public class AgentDeliveryRateVM
  {
    public int ID { get; set; }

    public int AgentID { get; set; }

    public string AgentName { get; set; }

    public int? ProductTypeID { get; set; }

    public int ZoneID { get; set; }

    public int ZoneChartID { get; set; }

    public Decimal BaseWeight { get; set; }

    public Decimal BaseRate { get; set; }

    public string CourierService { get; set; }

    public int? ZoneCategoryID { get; set; }

    public int AgentDeliveryRateID { get; set; }

    public List<CMSV2.Models.AgentDeliveryRateDetailVM> AgentDeliveryRateDetailVM { get; set; }
  }
}
