// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.ForwardingAgentInvoiceDetailsVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using System.Collections.Generic;

namespace CMSV2.Models
{
  public class ForwardingAgentInvoiceDetailsVM
  {
    public double FuelSurcharge { get; set; }

    public double Forwardingcharge { get; set; }

    public int FAInvoiceDetailID { get; set; }

    public string Descrepancy { get; set; }

    public double OriginalAmt { get; set; }

    public double Total { get; set; }

    public string FAAWBNO { get; set; }

    public List<CMSV2.Models.FuelSurchargeVM> FuelSurchargeVM { get; set; }
  }
}
