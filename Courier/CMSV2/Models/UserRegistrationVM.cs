// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.UserRegistrationVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

namespace CMSV2.Models
{
  public class UserRegistrationVM
  {
    public int UserID { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }
    public string Password1 { get; set; }

    public string Phone { get; set; }

    public string EmailId { get; set; }

    public bool IsActive { get; set; }

    public int RoleID { get; set; }

    public string RoleName { get; set; }

    public bool EmailNotify { get; set; }
    public int UserReferenceId { get; set; }

  }
}
