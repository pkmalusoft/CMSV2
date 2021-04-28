using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class ImportManifestVM : ImportShipment
    {


    }
    public class ImportManifestSearch
    {

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<ImportManifestVM> Details { get; set; }
    }
    public class ImportManifestItem
    {
        public int Bag { get; set; }
        public string Bill { get; set; }
        public string Rfnc { get; set; }
        public string Shipper { get; set; }
        public string Cnee { get; set; }
        public string CneeAddr { get; set; }
        public string CneeTel { get; set; }
        public string DSTN { get; set; }
        public string Content { get; set; }
        public string Pcs { get; set; }
        public string Weight {get;set;}
        public string Value { get; set; }
        public string Remark { get; set; }
        public string CCAmnt { get; set; }
        public string lastStatusRmk {get;set;}

    }
}