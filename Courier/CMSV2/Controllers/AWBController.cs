// Decompiled with JetBrains decompiler
// Type: CMSV2.Controllers.AWBController
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using CMSV2.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace CMSV2.Controllers
{
  public class AWBController : Controller
  {
    private Entities1 db = new Entities1();

    public ActionResult Index()
    {
      return (ActionResult) this.View((object) this.db.InScans.Select<InScan, AWBillVM>((Expression<Func<InScan, AWBillVM>>) (c => new AWBillVM() { AWBNO = c.AWBNo, shipper = c.Consignor, consignee = c.Consignee, destinationLocation = c.DestinationLocation, InScanID = c.InScanID, InScanDate = c.InScanDate })).ToList<AWBillVM>());
    }

    public ActionResult AWB()
    {
      this.db.EmployeeMasters.ToList<EmployeeMaster>();
      return (ActionResult) this.View();
    }

    public ActionResult Create()
    {
      this.db.CountryMasters.ToList<CountryMaster>();
      TaxConfiguration taxConfiguration = this.db.TaxConfigurations.FirstOrDefault<TaxConfiguration>();
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site1 = CallSite<Func<CallSite, object, List<EmployeeMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Employee", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site1.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site1, this.ViewBag, this.db.EmployeeMasters.ToList<EmployeeMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site2 = CallSite<Func<CallSite, object, List<tblAgent>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Agent", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site2.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site2, this.ViewBag, this.db.tblAgents.ToList<tblAgent>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site3 = CallSite<Func<CallSite, object, List<CustomerMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Customer", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site3.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site3, this.ViewBag, this.db.CustomerMasters.ToList<CustomerMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site4 = CallSite<Func<CallSite, object, List<CustomerRateType>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "CustomerRateType", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site4.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site4, this.ViewBag, this.db.CustomerRateTypes.ToList<CustomerRateType>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site5 = CallSite<Func<CallSite, object, List<CountryMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ShipperCountry", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj5 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site5.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site5, this.ViewBag, this.db.CountryMasters.ToList<CountryMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site6 = CallSite<Func<CallSite, object, List<CityMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ShipperCity", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site6.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site6, this.ViewBag, this.db.CityMasters.ToList<CityMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site7 = CallSite<Func<CallSite, object, List<LocationMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ShipperLocation", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj7 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site7.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site7, this.ViewBag, this.db.LocationMasters.ToList<LocationMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site8 = CallSite<Func<CallSite, object, List<CountryMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ConsigneeCountry", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site8.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site8, this.ViewBag, this.db.CountryMasters.ToList<CountryMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site9 = CallSite<Func<CallSite, object, List<CityMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ConsigneeCity", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj9 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site9.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site9, this.ViewBag, this.db.CityMasters.ToList<CityMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitea == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitea = CallSite<Func<CallSite, object, List<LocationMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ConsigneeLocation", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj10 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitea.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitea, this.ViewBag, this.db.LocationMasters.ToList<LocationMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Siteb == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Siteb = CallSite<Func<CallSite, object, List<InScan>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "CourierType", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj11 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Siteb.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Siteb, this.ViewBag, this.db.InScans.ToList<InScan>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitec == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitec = CallSite<Func<CallSite, object, List<CourierMovement>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Movement", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj12 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitec.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitec, this.ViewBag, this.db.CourierMovements.ToList<CourierMovement>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sited == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sited = CallSite<Func<CallSite, object, List<CourierDescription>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "CourierDescription", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj13 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sited.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sited, this.ViewBag, this.db.CourierDescriptions.ToList<CourierDescription>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitee == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitee = CallSite<Func<CallSite, object, List<CourierService>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ProductType", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj14 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitee.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitee, this.ViewBag, this.db.CourierServices.ToList<CourierService>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitef == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitef = CallSite<Func<CallSite, object, List<InScan>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "SpecialInstructions", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj15 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitef.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Sitef, this.ViewBag, this.db.InScans.ToList<InScan>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site10 = CallSite<Func<CallSite, object, List<ForwardingAgentMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "FAgentName", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj16 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site10.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site10, this.ViewBag, this.db.ForwardingAgentMasters.ToList<ForwardingAgentMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site11 = CallSite<Func<CallSite, object, List<InScan>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "CollectedByDetails", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj17 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site11.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site11, this.ViewBag, this.db.InScans.ToList<InScan>());
      Decimal num = Convert.ToDecimal(taxConfiguration.TaxPercentage) / new Decimal(100);
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site12 = CallSite<Func<CallSite, object, Decimal, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Tax", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj18 = AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site12.Target((CallSite) AWBController.\u003CCreate\u003Eo__SiteContainer0.\u003C\u003Ep__Site12, this.ViewBag, num);
      return (ActionResult) this.View();
    }

    [HttpPost]
    public ActionResult Create(AWBillVM v)
    {
      try
      {
        string empty = string.Empty;
        string str1 = this.db.InScans.OrderByDescending<InScan, string>((Expression<Func<InScan, string>>) (c => c.AWBNo)).Select<InScan, string>((Expression<Func<InScan, string>>) (c => c.AWBNo)).FirstOrDefault<string>();
        string str2 = str1 != null ? (Convert.ToInt32(str1) + 1).ToString() : "00001";
        int CurrentBranchID = Convert.ToInt32(this.Session["CurrentBranchID"].ToString());
        AcHeadAssign acHeadAssign = this.db.AcHeadAssigns.Where<AcHeadAssign>((Expression<Func<AcHeadAssign, bool>>) (x => x.BranchID == (int?) CurrentBranchID)).FirstOrDefault<AcHeadAssign>();
        try
        {
          AcJournalMaster entity1 = new AcJournalMaster() { AcJournalID = this.GetMaxAcJournalID() + 1, VoucherNo = "C.Note" + str2, TransDate = new DateTime?(DateTime.UtcNow), AcFinancialYearID = new int?(Convert.ToInt32(this.Session["CurrentYear"].ToString())), VoucherType = "DX", TransType = new short?((short) 1), StatusDelete = new bool?(false), Remarks = "", UserID = new int?(Convert.ToInt32(this.Session["UserID"].ToString())), AcCompanyID = new int?(Convert.ToInt32(this.Session["CurrenctCompanyID"].ToString())), Reference = "", ShiftID = new int?(0) };
          this.db.AcJournalMasters.Add(entity1);
          this.db.SaveChanges();
          if (v.paymentmode != "CSR" && v.TotalCharges > new Decimal(0))
          {
            if (acHeadAssign != null)
            {
              int num = 0;
              switch (v.paymentmode)
              {
                case "COD":
                  num = acHeadAssign.CODControlID.Value;
                  break;
                case "FOC":
                  num = acHeadAssign.FOCControlID.Value;
                  break;
                case "PKP":
                  num = acHeadAssign.UnPostedSalesAcHeadID.Value;
                  break;
              }
              if (num != 0 && Convert.ToDecimal(v.TotalCharges) != new Decimal(0))
              {
                this.db.AcJournalDetails.Add(new AcJournalDetail()
                {
                  AcJournalDetailID = this.GetMaxAcJournalDetailID() + 1,
                  AcJournalID = new int?(entity1.AcJournalID),
                  AcHeadID = new int?(num),
                  Amount = new Decimal?(Convert.ToDecimal(v.TotalCharges)),
                  Remarks = ""
                });
                this.db.SaveChanges();
                this.db.AcJournalDetails.Add(new AcJournalDetail()
                {
                  AcJournalDetailID = this.GetMaxAcJournalDetailID() + 1,
                  AcJournalID = new int?(entity1.AcJournalID),
                  AcHeadID = new int?(num),
                  Amount = new Decimal?(-Convert.ToDecimal(v.TotalCharges)),
                  Remarks = ""
                });
                this.db.SaveChanges();
              }
            }
          }
          else if (v.paymentmode == "CSR" && v.TotalCharges > new Decimal(0))
          {
            this.db.AcJournalDetails.Add(new AcJournalDetail()
            {
              AcJournalDetailID = this.GetMaxAcJournalDetailID() + 1,
              AcJournalID = new int?(entity1.AcJournalID),
              AcHeadID = acHeadAssign.MaterialCostControlReceivableAcHeadID,
              Amount = new Decimal?(Convert.ToDecimal(v.TotalCharges)),
              Remarks = ""
            });
            this.db.SaveChanges();
            this.db.AcJournalDetails.Add(new AcJournalDetail()
            {
              AcJournalDetailID = this.GetMaxAcJournalDetailID() + 1,
              AcJournalID = new int?(entity1.AcJournalID),
              AcHeadID = acHeadAssign.MaterialCostControlReceivableAcHeadID,
              Amount = new Decimal?(-Convert.ToDecimal(v.TotalCharges)),
              Remarks = ""
            });
            this.db.SaveChanges();
          }
          InScan entity2 = new InScan();
          entity2.InScanID = this.GetMaxInscanID() + 1;
          entity2.BranchID = new int?(Convert.ToInt32(this.Session["CurrentBranchID"].ToString()));
          entity2.AWBNo = str2;
          entity2.Remarks = v.Remarks;
          entity2.UserID = Convert.ToInt32(this.Session["UserID"].ToString());
          entity2.EnquiryID = new int?(0);
          entity2.StatusClose = new bool?(false);
          entity2.MovementID = new int?(v.movementID);
          entity2.AcJournalID = new int?(entity1.AcJournalID);
          entity2.AcCompanyID = new int?(Convert.ToInt32(this.Session["CurrenctCompanyID"].ToString()));
          entity2.StatusReconciled = new bool?(false);
          if (v.Date.HasValue)
            entity2.InScanDate = v.Date.Value;
          entity2.StatusInScan = "CN";
          entity2.CustomerRateTypeID = new int?(0);
          int customerRateTypeId = v.CustomerRateTypeID;
          entity2.CustomerRateTypeID = new int?(v.CustomerRateTypeID);
          entity2.Tax = new Decimal?(new Decimal(0));
          Decimal tax = v.Tax;
          entity2.Tax = new Decimal?(v.Tax);
          if (v.Pieces != null)
            entity2.Pieces = v.Pieces;
          entity2.StatedWeight = new double?(0.0);
          double weight = v.Weight;
          entity2.StatedWeight = new double?(v.Weight);
          entity2.CourierDescriptionID = new int?(0);
          int courierType1 = v.CourierType;
          entity2.CourierDescriptionID = new int?(v.CourierType);
          entity2.CourierServiceID = new int?(0);
          int productType = v.ProductType;
          entity2.CourierServiceID = new int?(v.ProductType);
          int agent1 = v.Agent;
          entity2.IAgentID = new int?(0);
          entity2.PickupCharges = new Decimal?(new Decimal(0));
          entity2.CollectedAmount = new Decimal?(new Decimal(0));
          entity2.PickupCharges = new Decimal?(new Decimal(0));
          entity2.CollectedBy = new int?(0);
          entity2.ReceivedBy = new int?(0);
          int agent2 = v.Agent;
          entity2.IAgentID = new int?(Convert.ToInt32(v.Agent));
          Decimal collectedamt = v.collectedamt;
          entity2.CollectedAmount = new Decimal?(Convert.ToDecimal(v.collectedamt));
          Decimal pickupcharge = v.pickupcharge;
          entity2.PickupCharges = new Decimal?(Convert.ToDecimal(v.pickupcharge));
          entity2.ReceivedBy = new int?(0);
          int receivedBy1 = v.ReceivedBy;
          entity2.ReceivedBy = new int?(v.ReceivedBy);
          entity2.CourierCharge = new Decimal(0);
          Decimal courierCharges = v.CourierCharges;
          entity2.CourierCharge = v.CourierCharges;
          entity2.OtherCharge = new Decimal?(new Decimal(0));
          Decimal otherCharges = v.OtherCharges;
          entity2.OtherCharge = new Decimal?(v.OtherCharges);
          entity2.ServiceCharge = new Decimal?(new Decimal(0));
          Decimal serviceCharges = v.ServiceCharges;
          entity2.ServiceCharge = new Decimal?(v.ServiceCharges);
          entity2.PackingCharge = new Decimal?(new Decimal(0));
          Decimal packingCharges = v.PackingCharges;
          entity2.PackingCharge = new Decimal?(v.PackingCharges);
          if (v.paymentmode != null)
            entity2.StatusPaymentMode = v.paymentmode;
          entity2.OriginID = new int?(0);
          int origincountry1 = v.origincountry;
          entity2.OriginID = new int?(v.origincountry);
          int destinationCountry1 = v.destinationCountry;
          entity2.DestinationID = v.destinationCountry.ToString();
          entity2.ConsignorCityID = new int?(0);
          int origincity = v.origincity;
          entity2.ConsignorCityID = new int?(v.origincity);
          entity2.ConsigneeCityID = new int?(0);
          int destinationCity = v.destinationCity;
          entity2.ConsigneeCityID = new int?(v.destinationCity);
          int courierMode = v.CourierMode;
          int courierType2 = v.CourierType;
          int cmode = Convert.ToInt32(v.CourierMode);
          entity2.TaxconfigurationID = new int?(this.db.TaxConfigurations.Where<TaxConfiguration>((Expression<Func<TaxConfiguration, bool>>) (tc1 => tc1.EffectFromDate <= (DateTime?) DateTime.UtcNow && tc1.CourierMoveMentID == cmode && tc1.CourierDescriptionID == v.CourierType)).Select<TaxConfiguration, int>((Expression<Func<TaxConfiguration, int>>) (tc1 => tc1.CourierDescriptionID)).FirstOrDefault<int>());
          entity2.CustomerID = new int?(0);
          if (v.CustomerID.HasValue)
            entity2.CustomerID = v.CustomerID;
          entity2.Consignee = v.consignee;
          entity2.Consignor = v.shipper;
          entity2.ConsigneeAddress = v.Consigneeaddress;
          entity2.ConsignorAddress = v.shipperaddress;
          if (v.ConsigneePhone != null)
            entity2.ConsigneePhone = v.ConsigneePhone;
          if (v.shipperphone != null)
            entity2.ConsignorPhone = v.shipperphone;
          entity2.ConsigneeCountryID = new int?(0);
          int destinationCountry2 = v.destinationCountry;
          entity2.ConsigneeCountryID = new int?(v.destinationCountry);
          entity2.ConsignorCountryID = new int?(0);
          int origincountry2 = v.origincountry;
          entity2.ConsignorCountryID = new int?(v.origincountry);
          if (v.originlocation != null)
            entity2.ConsignorLocation = v.originlocation;
          if (v.destinationLocation != null)
            entity2.ConsigneeLocation = v.destinationLocation;
          entity2.AcTaxJournalID = new int?(0);
          entity2.BalanceAmt = new Decimal?(new Decimal(0));
          Decimal totalCharges = v.TotalCharges;
          entity2.BalanceAmt = new Decimal?(v.TotalCharges);
          entity2.TypeOfGoodID = new int?(0);
          if (v.InvoiceValue != null)
            entity2.InvoiceValue = v.InvoiceValue;
          entity2.MaterialCost = new Decimal?(new Decimal(0));
          Decimal materialCost = v.MaterialCost;
          entity2.MaterialCost = new Decimal?(v.MaterialCost);
          entity2.ReferenceAWBNo = v.ReferenceNumber;
          entity2.MaterialDescription = v.MaterialDescription;
          Decimal revisedRate = v.RevisedRate;
          entity2.RevisedRate = new Decimal?(v.RevisedRate);
          if (v.SpecialInstructions != null)
            entity2.SpecialInstructions = v.SpecialInstructions;
          entity2.ConsigneeAddress1 = "";
          entity2.ConsigneeAddress2 = "";
          entity2.ConsignorAddress1 = "";
          entity2.ConsignorAddress2 = "";
          entity2.ConsigneeContact = v.consigneecontact;
          entity2.ConsignorContact = v.consigneecontact;
          entity2.CargoDescription = v.CargoDescription;
          entity2.HandlingInstruction = v.HandlingInstructions;
          entity2.CustomsCollectedBy = new int?(0);
          int receivedBy2 = v.ReceivedBy;
          entity2.CustomsCollectedBy = new int?(v.ReceivedBy);
          this.db.InScans.Add(entity2);
          this.db.SaveChanges();
          InScanInternationalDeatil entity3 = new InScanInternationalDeatil();
          entity3.InScanID = 0;
          entity3.InScanID = entity2.InScanID;
          entity3.VerifiedWeight = new Decimal?(new Decimal(0));
          double verifiedWeight1 = v.VerifiedWeight;
          entity3.VerifiedWeight = new Decimal?(Convert.ToDecimal(v.VerifiedWeight));
          InScanInternational entity4 = new InScanInternational();
          entity4.FAgentID = 0;
          int forwardingAgentId1 = v.ForwardingAgentID;
          entity4.FAgentID = v.ForwardingAgentID;
          if (v.ForwardingDate.HasValue)
            entity4.ForwardingDate = v.ForwardingDate.Value;
          entity4.VerifiedWeight = 0.0;
          double verifiedWeight2 = v.VerifiedWeight;
          entity4.VerifiedWeight = v.VerifiedWeight;
          entity4.StatusAssignment = false;
          entity4.ForwardingAWBNo = v.ForwardingAWB;
          entity4.InScanID = new int?(entity2.InScanID);
          int forwardingAgentId2 = v.ForwardingAgentID;
          entity4.FAgentID = v.ForwardingAgentID;
          if (v.ForwardingDate.HasValue)
            entity4.ForwardingDate = v.ForwardingDate.Value;
          this.db.InScanInternationals.Add(entity4);
          this.db.SaveChanges();
          entity3.InscanInternationalID = entity4.InScanInternationalID;
          this.db.InScanInternationalDeatils.Add(entity3);
          this.db.SaveChanges();
          int num1 = this.db.AWBStatus.OrderByDescending<AWBStatu, int>((Expression<Func<AWBStatu, int>>) (c => c.AWBStatusID)).Select<AWBStatu, int>((Expression<Func<AWBStatu, int>>) (c => c.AWBStatusID)).FirstOrDefault<int>() + 1;
          this.db.AWBStatus.Add(new AWBStatu()
          {
            AWBStatusID = num1,
            StatusDescriptionID = new int?(entity2.InScanID),
            AWBNO = str2,
            Date = new DateTime?(entity2.InScanDate),
            Status = "DetailsUpdated",
            FormID = "IN"
          });
          this.db.SaveChanges();
        }
        catch (Exception ex)
        {
        }
      }
      catch (Exception ex)
      {
      }
      return (ActionResult) this.RedirectToAction("Index");
    }

    public ActionResult Edit(int id)
    {
      AWBillVM obj = new AWBillVM();
      this.db.CountryMasters.ToList<CountryMaster>();
      TaxConfiguration taxConfiguration = this.db.TaxConfigurations.FirstOrDefault<TaxConfiguration>();
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site20 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site20 = CallSite<Func<CallSite, object, List<EmployeeMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Employee", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site20.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site20, this.ViewBag, this.db.EmployeeMasters.ToList<EmployeeMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site21 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site21 = CallSite<Func<CallSite, object, List<tblAgent>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Agent", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site21.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site21, this.ViewBag, this.db.tblAgents.ToList<tblAgent>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site22 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site22 = CallSite<Func<CallSite, object, List<CustomerMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Customer", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site22.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site22, this.ViewBag, this.db.CustomerMasters.ToList<CustomerMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site23 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site23 = CallSite<Func<CallSite, object, List<CustomerRateType>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "CustomerRateType", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site23.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site23, this.ViewBag, this.db.CustomerRateTypes.ToList<CustomerRateType>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site24 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site24 = CallSite<Func<CallSite, object, List<CountryMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ShipperCountry", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj5 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site24.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site24, this.ViewBag, this.db.CountryMasters.ToList<CountryMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site25 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site25 = CallSite<Func<CallSite, object, List<CityMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ShipperCity", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site25.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site25, this.ViewBag, this.db.CityMasters.ToList<CityMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site26 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site26 = CallSite<Func<CallSite, object, List<LocationMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ShipperLocation", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj7 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site26.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site26, this.ViewBag, this.db.LocationMasters.ToList<LocationMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site27 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site27 = CallSite<Func<CallSite, object, List<CountryMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ConsigneeCountry", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site27.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site27, this.ViewBag, this.db.CountryMasters.ToList<CountryMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site28 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site28 = CallSite<Func<CallSite, object, List<CityMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ConsigneeCity", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj9 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site28.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site28, this.ViewBag, this.db.CityMasters.ToList<CityMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site29 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site29 = CallSite<Func<CallSite, object, List<LocationMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ConsigneeLocation", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj10 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site29.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site29, this.ViewBag, this.db.LocationMasters.ToList<LocationMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2a == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2a = CallSite<Func<CallSite, object, List<InScan>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "CourierType", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj11 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2a.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2a, this.ViewBag, this.db.InScans.ToList<InScan>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2b == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2b = CallSite<Func<CallSite, object, List<CourierMovement>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Movement", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj12 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2b.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2b, this.ViewBag, this.db.CourierMovements.ToList<CourierMovement>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2c == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2c = CallSite<Func<CallSite, object, List<CourierDescription>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "CourierDescription", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj13 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2c.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2c, this.ViewBag, this.db.CourierDescriptions.ToList<CourierDescription>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2d == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2d = CallSite<Func<CallSite, object, List<CourierService>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ProductType", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj14 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2d.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2d, this.ViewBag, this.db.CourierServices.ToList<CourierService>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2e == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2e = CallSite<Func<CallSite, object, List<InScan>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "SpecialInstructions", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj15 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2e.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2e, this.ViewBag, this.db.InScans.ToList<InScan>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2f == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2f = CallSite<Func<CallSite, object, List<ForwardingAgentMaster>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "FAgentName", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj16 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2f.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site2f, this.ViewBag, this.db.ForwardingAgentMasters.ToList<ForwardingAgentMaster>());
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site30 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site30 = CallSite<Func<CallSite, object, List<InScan>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "CollectedByDetails", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj17 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site30.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site30, this.ViewBag, this.db.InScans.ToList<InScan>());
      Decimal num = Convert.ToDecimal(taxConfiguration.TaxPercentage) / new Decimal(100);
      // ISSUE: reference to a compiler-generated field
      if (AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site31 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site31 = CallSite<Func<CallSite, object, Decimal, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Tax", typeof (AWBController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj18 = AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site31.Target((CallSite) AWBController.\u003CEdit\u003Eo__SiteContainer1f.\u003C\u003Ep__Site31, this.ViewBag, num);
      InScan inScan = this.db.InScans.Where<InScan>((Expression<Func<InScan, bool>>) (c => c.InScanID == id)).FirstOrDefault<InScan>();
      if (inScan == null)
        return (ActionResult) this.HttpNotFound();
      obj.InScanID = inScan.InScanID;
      obj.AWBNO = inScan.AWBNo;
      obj.Remarks = inScan.Remarks;
      obj.movementID = 0;
      int movementId = obj.movementID;
      obj.movementID = inScan.MovementID.Value;
      obj.Date = new DateTime?(inScan.InScanDate);
      if (obj.Depot != null)
        obj.Depot = inScan.DepotID.Value.ToString();
      obj.CustomerRateTypeID = 0;
      int customerRateTypeId = obj.CustomerRateTypeID;
      obj.CustomerRateTypeID = inScan.CustomerRateTypeID.Value;
      obj.Tax = inScan.Tax.Value;
      obj.Pieces = inScan.Pieces;
      obj.Weight = inScan.StatedWeight.Value;
      obj.CourierType = inScan.CourierDescriptionID.Value;
      obj.ProductType = inScan.CourierServiceID.Value;
      obj.pickupcharge = inScan.PickupCharges.Value;
      obj.collectedamt = inScan.CollectedAmount.Value;
      obj.pickupcharge = inScan.PickupCharges.Value;
      obj.CollectedByDetails = inScan.CollectedBy.Value;
      obj.ReceivedBy = inScan.ReceivedBy.Value;
      obj.CourierCharges = inScan.CourierCharge;
      obj.OtherCharges = inScan.OtherCharge.Value;
      obj.ServiceCharges = inScan.ServiceCharge.Value;
      obj.PackingCharges = inScan.PackingCharge.Value;
      if (obj.paymentmode != null)
        obj.paymentmode = inScan.StatusPaymentMode;
      obj.origincountry = 0;
      int origincountry1 = obj.origincountry;
      obj.origincountry = inScan.OriginID.Value;
      obj.destinationCountry = 0;
      int destinationCountry1 = obj.destinationCountry;
      obj.destinationCountry = Convert.ToInt32(inScan.DestinationID);
      obj.origincity = 0;
      int origincity = obj.origincity;
      obj.origincity = inScan.ConsignorCityID.Value;
      obj.destinationCity = 0;
      int destinationCity = obj.destinationCity;
      obj.destinationCity = inScan.ConsignorCityID.Value;
      obj.CustomerID = new int?(0);
      if (obj.CustomerID.HasValue)
        obj.CustomerID = inScan.CustomerID;
      obj.consignee = inScan.Consignee;
      obj.shipper = inScan.Consignor;
      obj.Consigneeaddress = inScan.ConsigneeAddress;
      obj.shipperaddress = inScan.ConsignorAddress;
      obj.ConsigneePhone = inScan.ConsigneePhone;
      obj.shipperphone = inScan.ConsignorPhone;
      obj.destinationCountry = 0;
      int destinationCountry2 = obj.destinationCountry;
      obj.destinationCountry = inScan.ConsigneeCountryID.Value;
      obj.origincountry = 0;
      int origincountry2 = obj.origincountry;
      obj.origincountry = inScan.ConsigneeCountryID.Value;
      obj.originlocation = inScan.ConsignorLocation;
      obj.destinationLocation = inScan.ConsigneeLocation;
      obj.TotalCharges = inScan.BalanceAmt.Value;
      obj.InvoiceValue = inScan.InvoiceValue;
      obj.MaterialCost = inScan.MaterialCost.Value;
      obj.ReferenceNumber = inScan.ReferenceAWBNo;
      obj.MaterialDescription = inScan.MaterialDescription;
      obj.RevisedRate = inScan.RevisedRate.Value;
      obj.SpecialInstructions = inScan.SpecialInstructions;
      obj.consigneecontact = inScan.ConsigneeContact;
      obj.shippercontact = inScan.ConsignorContact;
      obj.CargoDescription = inScan.CargoDescription;
      obj.HandlingInstructions = inScan.HandlingInstruction;
      obj.ReceivedBy = inScan.CustomsCollectedBy.Value;
      InScanInternational scanInternational = this.db.InScanInternationals.Where<InScanInternational>((Expression<Func<InScanInternational, bool>>) (c => c.InScanID == (int?) obj.InScanID)).FirstOrDefault<InScanInternational>();
      if (scanInternational != null)
      {
        obj.ForwardingAgentID = 0;
        int forwardingAgentId = obj.ForwardingAgentID;
        obj.ForwardingAgentID = scanInternational.FAgentID;
        obj.ForwardingAWB = scanInternational.ForwardingAWBNo;
        obj.ForwardingDate = new DateTime?(scanInternational.ForwardingDate);
        obj.VerifiedWeight = scanInternational.VerifiedWeight;
        obj.ForwardingCharge = scanInternational.ForwardingCharge;
      }
      return (ActionResult) this.View((object) obj);
    }

    [HttpPost]
    public ActionResult Edit(AWBillVM v)
    {
      InScan entity1 = new InScan();
      entity1.InScanID = v.InScanID;
      entity1.BranchID = new int?(Convert.ToInt32(this.Session["CurrentBranchID"].ToString()));
      entity1.AWBNo = v.AWBNO;
      entity1.Remarks = v.Remarks;
      entity1.UserID = Convert.ToInt32(this.Session["UserID"].ToString());
      entity1.EnquiryID = new int?(0);
      entity1.StatusClose = new bool?(false);
      entity1.MovementID = new int?(v.movementID);
      entity1.AcCompanyID = new int?(Convert.ToInt32(this.Session["CurrenctCompanyID"].ToString()));
      entity1.StatusReconciled = new bool?(false);
      if (v.Date.HasValue)
        entity1.InScanDate = v.Date.Value;
      entity1.StatusInScan = "CN";
      entity1.CustomerRateTypeID = new int?(0);
      int customerRateTypeId = v.CustomerRateTypeID;
      entity1.CustomerRateTypeID = new int?(v.CustomerRateTypeID);
      entity1.Tax = new Decimal?(new Decimal(0));
      Decimal tax = v.Tax;
      entity1.Tax = new Decimal?(v.Tax);
      if (v.Pieces != null)
        entity1.Pieces = v.Pieces;
      entity1.StatedWeight = new double?(0.0);
      double weight = v.Weight;
      entity1.StatedWeight = new double?(v.Weight);
      entity1.CourierDescriptionID = new int?(0);
      int courierType1 = v.CourierType;
      entity1.CourierDescriptionID = new int?(v.CourierType);
      entity1.CourierServiceID = new int?(v.ProductType);
      int agent1 = v.Agent;
      entity1.IAgentID = new int?(0);
      entity1.PickupCharges = new Decimal?(new Decimal(0));
      entity1.CollectedAmount = new Decimal?(new Decimal(0));
      entity1.PickupCharges = new Decimal?(new Decimal(0));
      entity1.CollectedBy = new int?(0);
      entity1.ReceivedBy = new int?(0);
      int agent2 = v.Agent;
      entity1.IAgentID = new int?(Convert.ToInt32(v.Agent));
      Decimal collectedamt = v.collectedamt;
      entity1.CollectedAmount = new Decimal?(Convert.ToDecimal(v.collectedamt));
      Decimal pickupcharge = v.pickupcharge;
      entity1.PickupCharges = new Decimal?(Convert.ToDecimal(v.pickupcharge));
      entity1.ReceivedBy = new int?(0);
      int receivedBy1 = v.ReceivedBy;
      entity1.ReceivedBy = new int?(v.ReceivedBy);
      entity1.CourierCharge = new Decimal(0);
      Decimal courierCharges = v.CourierCharges;
      entity1.CourierCharge = v.CourierCharges;
      entity1.OtherCharge = new Decimal?(new Decimal(0));
      Decimal otherCharges = v.OtherCharges;
      entity1.OtherCharge = new Decimal?(v.OtherCharges);
      entity1.ServiceCharge = new Decimal?(new Decimal(0));
      Decimal serviceCharges = v.ServiceCharges;
      entity1.ServiceCharge = new Decimal?(v.ServiceCharges);
      entity1.PackingCharge = new Decimal?(new Decimal(0));
      Decimal packingCharges = v.PackingCharges;
      entity1.PackingCharge = new Decimal?(v.PackingCharges);
      if (v.paymentmode != null)
        entity1.StatusPaymentMode = v.paymentmode;
      entity1.OriginID = new int?(0);
      int origincountry1 = v.origincountry;
      entity1.OriginID = new int?(v.origincountry);
      int destinationCountry1 = v.destinationCountry;
      entity1.DestinationID = v.destinationCountry.ToString();
      entity1.ConsignorCityID = new int?(0);
      int origincity = v.origincity;
      entity1.ConsignorCityID = new int?(v.origincity);
      entity1.ConsigneeCityID = new int?(0);
      int destinationCity = v.destinationCity;
      entity1.ConsigneeCityID = new int?(v.destinationCity);
      int courierMode = v.CourierMode;
      int courierType2 = v.CourierType;
      int cmode = Convert.ToInt32(v.CourierMode);
      entity1.TaxconfigurationID = new int?(this.db.TaxConfigurations.Where<TaxConfiguration>((Expression<Func<TaxConfiguration, bool>>) (tc1 => tc1.EffectFromDate <= (DateTime?) DateTime.UtcNow && tc1.CourierMoveMentID == cmode && tc1.CourierDescriptionID == v.CourierType)).Select<TaxConfiguration, int>((Expression<Func<TaxConfiguration, int>>) (tc1 => tc1.CourierDescriptionID)).FirstOrDefault<int>());
      entity1.CustomerID = new int?(0);
      if (v.CustomerID.HasValue)
        entity1.CustomerID = v.CustomerID;
      entity1.Consignee = v.consignee;
      entity1.Consignor = v.shipper;
      entity1.ConsigneeAddress = v.Consigneeaddress;
      entity1.ConsignorAddress = v.shipperaddress;
      if (v.ConsigneePhone != null)
        entity1.ConsigneePhone = v.ConsigneePhone;
      if (v.shipperphone != null)
        entity1.ConsignorPhone = v.shipperphone;
      entity1.ConsigneeCountryID = new int?(0);
      int destinationCountry2 = v.destinationCountry;
      entity1.ConsigneeCountryID = new int?(v.destinationCountry);
      entity1.ConsignorCountryID = new int?(0);
      int origincountry2 = v.origincountry;
      entity1.ConsignorCountryID = new int?(v.origincountry);
      if (v.originlocation != null)
        entity1.ConsignorLocation = v.originlocation;
      if (v.destinationLocation != null)
        entity1.ConsigneeLocation = v.destinationLocation;
      entity1.AcTaxJournalID = new int?(0);
      entity1.BalanceAmt = new Decimal?(new Decimal(0));
      Decimal totalCharges = v.TotalCharges;
      entity1.BalanceAmt = new Decimal?(v.TotalCharges);
      entity1.TypeOfGoodID = new int?(0);
      if (v.InvoiceValue != null)
        entity1.InvoiceValue = v.InvoiceValue;
      entity1.MaterialCost = new Decimal?(new Decimal(0));
      Decimal materialCost = v.MaterialCost;
      entity1.MaterialCost = new Decimal?(v.MaterialCost);
      entity1.ReferenceAWBNo = v.ReferenceNumber;
      entity1.MaterialDescription = v.MaterialDescription;
      Decimal revisedRate = v.RevisedRate;
      entity1.RevisedRate = new Decimal?(v.RevisedRate);
      if (v.SpecialInstructions != null)
        entity1.SpecialInstructions = v.SpecialInstructions;
      entity1.ConsigneeAddress1 = "";
      entity1.ConsigneeAddress2 = "";
      entity1.ConsignorAddress1 = "";
      entity1.ConsignorAddress2 = "";
      entity1.ConsigneeContact = v.consigneecontact;
      entity1.ConsignorContact = v.consigneecontact;
      entity1.CargoDescription = v.CargoDescription;
      entity1.HandlingInstruction = v.HandlingInstructions;
      entity1.CustomsCollectedBy = new int?(0);
      int receivedBy2 = v.ReceivedBy;
      entity1.CustomsCollectedBy = new int?(v.ReceivedBy);
      this.db.Entry<InScan>(entity1).State = EntityState.Modified;
      this.db.SaveChanges();
      InScanInternational entity2 = this.db.InScanInternationals.Where<InScanInternational>((Expression<Func<InScanInternational, bool>>) (c => c.InScanID == (int?) v.InScanID)).FirstOrDefault<InScanInternational>();
      if (entity2 != null)
      {
        entity2.FAgentID = 0;
        int forwardingAgentId1 = v.ForwardingAgentID;
        entity2.FAgentID = v.ForwardingAgentID;
        if (v.ForwardingDate.HasValue)
          entity2.ForwardingDate = v.ForwardingDate.Value;
        entity2.VerifiedWeight = 0.0;
        double verifiedWeight = v.VerifiedWeight;
        entity2.VerifiedWeight = v.VerifiedWeight;
        entity2.StatusAssignment = false;
        entity2.ForwardingAWBNo = v.ForwardingAWB;
        entity2.InScanID = new int?(entity1.InScanID);
        int forwardingAgentId2 = v.ForwardingAgentID;
        entity2.FAgentID = v.ForwardingAgentID;
        if (v.ForwardingDate.HasValue)
          entity2.ForwardingDate = v.ForwardingDate.Value;
        this.db.Entry<InScanInternational>(entity2).State = EntityState.Modified;
        this.db.SaveChanges();
      }
      return (ActionResult) this.RedirectToAction("Index");
    }

    public int GetMaxAcJournalID()
    {
      return this.db.AcJournalMasters.OrderByDescending<AcJournalMaster, int>((Expression<Func<AcJournalMaster, int>>) (c => c.AcJournalID)).Select<AcJournalMaster, int>((Expression<Func<AcJournalMaster, int>>) (c => c.AcJournalID)).FirstOrDefault<int>();
    }

    public int GetMaxAcJournalDetailID()
    {
      return this.db.AcJournalDetails.OrderByDescending<AcJournalDetail, int>((Expression<Func<AcJournalDetail, int>>) (c => c.AcJournalDetailID)).Select<AcJournalDetail, int>((Expression<Func<AcJournalDetail, int>>) (c => c.AcJournalDetailID)).FirstOrDefault<int>();
    }

    public int GetMaxInscanID()
    {
      return this.db.InScans.OrderByDescending<InScan, int>((Expression<Func<InScan, int>>) (c => c.InScanID)).Select<InScan, int>((Expression<Func<InScan, int>>) (c => c.InScanID)).FirstOrDefault<int>();
    }

    public JsonResult GetPickUpData(string id)
    {
      AWBController.PickUp pickUp = new AWBController.PickUp();
      CustomerEnquiry customerEnquiry = this.db.CustomerEnquiries.Where<CustomerEnquiry>((Expression<Func<CustomerEnquiry, bool>>) (c => c.AWBNo == id)).FirstOrDefault<CustomerEnquiry>();
      if (customerEnquiry != null)
      {
        pickUp.CustomerID = customerEnquiry.CustomerID.Value;
        pickUp.shipper = customerEnquiry.Consignor;
        pickUp.contactperson = customerEnquiry.ConsignorContact;
        pickUp.shipperaddress = customerEnquiry.ConsignorAddress;
        pickUp.shipperphone = customerEnquiry.ConsignorPhone;
        pickUp.shippercountry = customerEnquiry.ConsignerCountryId.Value;
        pickUp.shippercity = customerEnquiry.ConsignerCityId.Value;
        pickUp.shipperlocation = customerEnquiry.ConsignorLocationName;
        pickUp.weight = customerEnquiry.Weight.Value;
        pickUp.consignee = customerEnquiry.Consignee;
        pickUp.consigneecontact = customerEnquiry.ConsigneeContact;
        pickUp.consigneeaddress = customerEnquiry.ConsigneeAddress;
        pickUp.consigneephone = customerEnquiry.ConsigneePhone;
        pickUp.consigneecountry = customerEnquiry.ConsigneeCountryID.Value;
        pickUp.consigneecity = customerEnquiry.ConsigneeCityId.Value;
        pickUp.consigneelocation = customerEnquiry.ConsigneeLocationName;
        pickUp.Exist = 1;
      }
      else
        pickUp.Exist = 0;
      return this.Json((object) pickUp, JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetCity(int id)
    {
      List<AWBController.CityM> cityMList = new List<AWBController.CityM>();
      DbSet<CityMaster> cityMasters = this.db.CityMasters;
      Expression<Func<CityMaster, bool>> predicate = (Expression<Func<CityMaster, bool>>) (c => c.CountryID == (int?) id);
      foreach (CityMaster cityMaster in cityMasters.Where<CityMaster>(predicate).ToList<CityMaster>())
        cityMList.Add(new AWBController.CityM()
        {
          City = cityMaster.City,
          CityID = cityMaster.CityID
        });
      return this.Json((object) cityMList, JsonRequestBehavior.AllowGet);
    }

    public JsonResult SetCustomerRate(int custRateType, int prodType, int LocationID, int x, float flWeight, string Status)
    {
      return this.Json((object) new AWBController.CustRate() { crate = new Decimal(40) }, JsonRequestBehavior.AllowGet);
    }

    public class PickUp
    {
      public int CustomerID { get; set; }

      public string shipper { get; set; }

      public string contactperson { get; set; }

      public string shipperaddress { get; set; }

      public string shipperphone { get; set; }

      public int shippercountry { get; set; }

      public int shippercity { get; set; }

      public string shipperlocation { get; set; }

      public double weight { get; set; }

      public string consignee { get; set; }

      public string consigneecontact { get; set; }

      public string consigneeaddress { get; set; }

      public string consigneephone { get; set; }

      public int consigneecountry { get; set; }

      public int consigneecity { get; set; }

      public string consigneelocation { get; set; }

      public int Exist { get; set; }
    }

    public class CityM
    {
      public int CityID { get; set; }

      public string City { get; set; }
    }

    public class CustRate
    {
      public Decimal crate { get; set; }
    }
  }
}
