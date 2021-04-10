// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.PODStatusVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using System;
using System.Collections.Generic;

namespace CMSV2.Models
{
  public class PODVM :POD
  {

    public string AWBNo { get; set; }

    public DateTime AWBDate { get; set; }

        public bool DeliveryAttempted { get; set; } //Delivery Attempt Date
        public int DeliveryAttemptedBy { get; set; } //
        public DateTime? DelieveryAttemptDate { get; set; }

        public bool Delivered { get; set; } //Delivered Date
        //public int DeliveredBy { get; set; } //
        //public DateTime? DeliveredDate { get; set; }
        public string Shipper { get; set; }
        public string ShipperContact { get; set; }
        public string Consignee { get; set; }
        public string ConsigneeContact { get; set; }
        public string CourierStatus { get; set; }
        public int? CourierStatusId { get; set; }
        public int? StatusTypeId { get; set; }
        public List<PODAWBDetail> Details { get; set; }
    }
    public class PODAWBDetail : POD
    {
        public string AWBNo { get; set; }
        public bool DeliveryAttempted { get; set; } //Delivery Attempt Date
        public int DeliveryAttemptedBy { get; set; } //
        public DateTime? DelieveryAttemptDate { get; set; }
        public bool Delivered { get; set; } //Delivered Date
        
    }

    public class PODSearch
    {
       
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }     
     
        public List<PODVM> Details { get; set; }
    }
}
