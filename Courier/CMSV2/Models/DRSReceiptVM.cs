// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.DRSReceiptVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using System;

namespace CMSV2.Models
{
  public class DRSReceiptVM
  {
    public int DRSReceiptID { get; set; }

    public DateTime? DRSReceiptDate { get; set; }

    public string DRSNo { get; set; }

    public Decimal? Amount { get; set; }

    public string Remarks { get; set; }

    public int? VehicleID { get; set; }

    public int? EmployeeID { get; set; }

    public int? DepartmentID { get; set; }

    public int? AcJournalID { get; set; }

    public string StatusRunSheet { get; set; }

    public int? User1 { get; set; }

    public int? FYearID { get; set; }

    public int? AcCompanyID { get; set; }

    public int? DRSID { get; set; }

    public int? AcID { get; set; }

    public string Vehicle { get; set; }

    public string Account { get; set; }

    public string Deliver { get; set; }
  }
}
