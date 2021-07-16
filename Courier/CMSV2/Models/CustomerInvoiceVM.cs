// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.CustomerInvoiceVM
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMSV2.Models
{
  public class CustomerInvoiceVM:CustomerInvoice
  {  

    public string CustomerName { get; set; }
        public string CustomerCountryName { get; set; }
        public string CurrencyName { get; set; }
        public string CustomerPhoneNo { get; set; }
        public string CustomerCityName { get; set; }
        public string InvoiceTotalInWords { get; set; }
        public string ParcelType { get; set; }
        public string CourierMovement { get; set; }
        public string LogoFilePath { get; set; }
        public DateTime FromDate { get; set; }
         
    public DateTime ToDate { get; set; }
        public int MovementTypeID { get; set; }
        public decimal TotalCharges { get; set; }

        [AllowHtml]
        public string invoiceFooter { get; set; }

        public GeneralSetup generalSetup { get; set; }
        public List<CMSV2.Models.CustomerInvoiceDetailVM> CustomerInvoiceDetailsVM { get; set; }

        public string MovementId { get; set; }
        public int[] SelectedValues { get; set; }
    }

    public class CustomerInvoiceDetailVM :CustomerInvoiceDetail
    {
        public DateTime AWBDateTime { get; set; }
        public string Origin { get; set; }
        public string  ConsigneeName { get; set; }
        public string ConsigneeCountryName { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal? Weight { get; set; }
        public string Pieces { get; set; }
        public int? MovementId { get; set; }

        public int? ParcelTypeId { get; set; }        
        public bool AWBChecked { get; set; }
    }

    public class InvoiceDatePicker
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool Create { get; set; }

        public int? StatusId { get; set; }
        public int? AgentId { get; set; }
    }

    public class CustomerInvoiceSearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string InvoiceNo { get; set; }
        public List<CustomerInvoiceVM> Details { get; set; }
    }
}
