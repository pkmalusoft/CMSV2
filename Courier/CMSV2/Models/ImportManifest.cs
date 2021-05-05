﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class ImportManifestVM : ImportShipment
    {
        public string CompanyCountryName { get; set; }
        public string ManifestDate { get; set; }
        public string FlightDate1 { get; set; }

    }
    public class ImportManifestSearch
    {

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<ImportManifestVM> Details { get; set; }
    }

    public class ImportManifestFixation
    {
        public string FieldName { get; set; }
        public string SourceValue { get; set; }
        public string DestinationValue { get; set; }
        public string DestinationLocation { get; set; }
        public string DestinationCountry { get; set; }
        public string DestinationCity { get; set; }
        public bool AllItem { get; set; }
        public string FilterItem { get; set; }
    }
    public class ImportManifestItem
    {
        public string Bag { get; set; }
        public string AWBNo { get; set; }
        public string AWBDate { get; set; }
        public string Rfnc { get; set; }
        public string Shipper { get; set; }
        public string Receiver { get; set; }
        public string ReceiverContact{ get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverPhone { get; set; }
        public string DestinationLocation { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationCountry { get; set; }
        public string ImportType { get; set; }
        public string Content { get; set; }
        public string Pcs { get; set; }
        public string Weight {get;set;}
        public string Value { get; set; }
        public string COD { get; set; }
        public string Remark { get; set; }        
        public string lastStatusRmk {get;set;}

    }
}