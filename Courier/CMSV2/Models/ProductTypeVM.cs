﻿// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.ProductTypeVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using System;

namespace CMSV2.Models
{
  public class ProductTypeVM
  {
    public int ProductTypeID { get; set; }

    public string ProductName { get; set; }

    public int ParcelTypeID { get; set; }

    public int TransportModeID { get; set; }

    public bool CBMbasedCharges { get; set; }

    public Decimal Length { get; set; }

    public Decimal Width { get; set; }

    public Decimal Height { get; set; }

    public Decimal? CBM { get; set; }

        public string ParcelType { get; set; }
        public string MovementType { get; set; }
  }
}
