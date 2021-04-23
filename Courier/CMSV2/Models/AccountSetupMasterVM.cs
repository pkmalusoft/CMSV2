using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class AccountSetupMasterVM :AccountSetup
    {
        public string DebitHead { get; set; }
        public string CreditHead { get; set; }

    }
}