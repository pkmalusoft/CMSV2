using CMSV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSV2.Controllers
{
    public class PrepaidAWBController : Controller
    {
        Entities1 db = new Entities1();

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult AWB()
        {

            var data = db.EmployeeMasters.ToList();
            return View();
        }

        public ActionResult Create()
        {
            var a = db.CountryMasters.ToList();
            var a1 = db.TaxConfigurations.FirstOrDefault();

            ViewBag.Employee = db.EmployeeMasters.ToList();
            ViewBag.Agent = db.tblAgents.ToList();
            ViewBag.Customer = db.CustomerMasters.ToList();
            ViewBag.CustomerRateType = db.CustomerRateTypes.ToList();
            ViewBag.ShipperCountry = db.CountryMasters.ToList();
            ViewBag.ShipperCity = db.CityMasters.ToList();


            ViewBag.ShipperLocation = db.LocationMasters.ToList();
            ViewBag.ConsigneeCountry = db.CountryMasters.ToList();
            ViewBag.ConsigneeCity = db.CityMasters.ToList();
            ViewBag.ConsigneeDepartment = db.Departments.ToList();
            ViewBag.ConsigneeLocation = db.LocationMasters.ToList();
            ViewBag.CourierType = db.InScans.ToList();

            ViewBag.Movement = db.CourierMovements.ToList();
            ViewBag.CourierDescription = db.CourierDescriptions.ToList();
            ViewBag.ProductType = db.CourierServices.ToList();
            ViewBag.SpecialInstructions = db.InScans.ToList();
            ViewBag.FAgentName = db.ForwardingAgentMasters.ToList();
            ViewBag.CollectedByDetails = db.InScans.ToList();



            return View();
        }

        [HttpPost]
        public ActionResult Create(AWBillVM v)
        {
            try
            {
                string AWBNo = string.Empty;


                if (v.AWBNO != string.Empty)
                {
                    AWBNo = v.AWBNO;
                }
                else
                {
                    string aw = (from c in db.InScans orderby c.AWBNo descending select c.AWBNo).FirstOrDefault();

                    int a = Convert.ToInt32(aw) + 1;
                    AWBNo = a.ToString();
                }
                int CurrentBranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                var ahead = db.AcHeadAssigns.Where(x => x.BranchID == CurrentBranchID).FirstOrDefault();
                try
                {


                    var ajm = new AcJournalMaster()
                    {
                        AcJournalID = GetMaxAcJournalID() + 1,
                        VoucherNo = "C.Note" + AWBNo,
                        TransDate = DateTime.UtcNow,
                        AcFinancialYearID = Convert.ToInt32(Session["CurrentYear"].ToString()),
                        VoucherType = "DX",
                        TransType = 1,
                        StatusDelete = false,
                        Remarks = "",
                        UserID = Convert.ToInt32(Session["UserID"].ToString()),
                        AcCompanyID = Convert.ToInt32(Session["CurrenctCompanyID"].ToString()),
                        //BranchID = this.CurrentBranchID,
                        Reference = "",
                        ShiftID = 0,
                    };
                    db.AcJournalMasters.Add(ajm);
                    db.SaveChanges();
                    if (v.paymentmode != "CSR" && v.TotalCharges > 0)
                    {
                        if (ahead != null)
                        {
                            int aheadassign = 0;

                            switch (v.paymentmode)
                            {
                                case "COD": aheadassign = ahead.CODControlID.Value; break;  //CODControlID
                                case "FOC": aheadassign = ahead.FOCControlID.Value; break;  //FOCControlID
                                case "PKP": aheadassign = ahead.UnPostedSalesAcHeadID.Value; break;  //UnPostedSalesAcHeadID
                                default:
                                    break;
                            }

                            if (aheadassign != 0 && Convert.ToDecimal(v.TotalCharges) != 0)
                            {
                                AcJournalDetail ajd = new AcJournalDetail()
                                {
                                    AcJournalDetailID = GetMaxAcJournalDetailID() + 1,
                                    AcJournalID = ajm.AcJournalID,
                                    AcHeadID = aheadassign,
                                    Amount = Convert.ToDecimal(v.TotalCharges),
                                    Remarks = "",
                                };
                                db.AcJournalDetails.Add(ajd);
                                db.SaveChanges();

                                ajd = new AcJournalDetail()
                                {
                                    AcJournalDetailID = GetMaxAcJournalDetailID() + 1,
                                    AcJournalID = ajm.AcJournalID,
                                    AcHeadID = aheadassign,
                                    Amount = -Convert.ToDecimal(v.TotalCharges),
                                    Remarks = "",
                                };
                                db.AcJournalDetails.Add(ajd);
                                db.SaveChanges();
                            }
                        }

                    }
                    else if (v.paymentmode == "CSR" && v.TotalCharges > 0)
                    {

                        AcJournalDetail ajd = new AcJournalDetail()
                        {
                            AcJournalDetailID = GetMaxAcJournalDetailID() + 1,
                            AcJournalID = ajm.AcJournalID,
                            AcHeadID = ahead.MaterialCostControlReceivableAcHeadID,
                            Amount = Convert.ToDecimal(v.TotalCharges),
                            Remarks = "",
                        };

                        db.AcJournalDetails.Add(ajd);
                        db.SaveChanges();

                        ajd = new AcJournalDetail()
                        {
                            AcJournalDetailID = GetMaxAcJournalDetailID() + 1,
                            AcJournalID = ajm.AcJournalID,
                            AcHeadID = ahead.MaterialCostControlReceivableAcHeadID,
                            Amount = -Convert.ToDecimal(v.TotalCharges),
                            Remarks = "",
                        };
                        db.AcJournalDetails.Add(ajd);
                        db.SaveChanges();

                    }

                    InScan inscan = new InScan();

                    inscan.InScanID = GetMaxInscanID();
                    //inscan.AWBNumberMode = IsAWBAutoManual;
                    inscan.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    inscan.AWBNo = AWBNo;
                    inscan.Remarks = v.Remarks;
                    inscan.UserID = Convert.ToInt32(Session["UserID"].ToString());
                    inscan.EnquiryID = 0;
                    inscan.StatusClose = false;
                    inscan.MovementID = v.movementID;
                    inscan.AcJournalID = ajm.AcJournalID;
                    inscan.AcCompanyID = Convert.ToInt32(Session["CurrenctCompanyID"].ToString());
                    inscan.StatusReconciled = false;
                    if (v.Date != null)
                    {
                        inscan.InScanDate = v.Date.Value;
                        //inscan.InScanDate = DateTime.Parse(deDate.SelectedDate.Value.Date.ToString("dd/MM/yyyy") + " " + deDate.SelectedDate.Value.ToString("HH:mm:ss")); //DateTime.Parse(deDate.SelectedDate.Value.Date.ToString("dd/MM/yyyy") + " " + this.rtpTime.SelectedDate.Value.ToString("HH:mm:ss")),
                    }

                    inscan.StatusInScan = "CN";
                    inscan.CustomerRateTypeID = 0;
                    if (v.CustomerRateTypeID != null)
                    {
                        inscan.CustomerRateTypeID = v.CustomerRateTypeID;
                    }
                    inscan.Tax = 0;
                    if (v.Tax != null)
                    {
                        inscan.Tax = v.Tax;
                    }
                    if (v.Pieces != null)
                    {
                        inscan.Pieces = v.Pieces;
                    }
                    inscan.StatedWeight = 0;
                    if (v.Weight != null)
                    {
                        inscan.StatedWeight = v.Weight;
                    }
                    inscan.CourierDescriptionID = 0;
                    if (v.CourierType != null)
                    {
                        inscan.CourierDescriptionID = v.CourierType;
                    }

                    inscan.CourierServiceID = 0;
                    if (v.ProductType != null)
                    {
                        inscan.CourierServiceID = v.ProductType;
                    }


                    if (v.Agent != null)
                    {
                        inscan.IAgentID = 0;
                        inscan.PickupCharges = 0;
                        inscan.CollectedAmount = 0;
                        inscan.PickupCharges = 0;
                        inscan.CollectedBy = 0;
                        inscan.ReceivedBy = 0;
                        if (v.Agent != null)
                        {
                            inscan.IAgentID = Convert.ToInt32(v.Agent);
                            if (v.collectedamt != null)
                            {
                                inscan.CollectedAmount = Convert.ToDecimal(v.collectedamt);
                            }
                            if (v.pickupcharge != null)
                            {
                                inscan.PickupCharges = Convert.ToDecimal(v.pickupcharge);
                            }
                        }
                    }
                    else
                    {
                        if (v.CollectedByDetails != null)
                        {
                            inscan.CollectedBy = v.CollectedByDetails;
                        }
                    }

                    inscan.ReceivedBy = 0;
                    if (v.ReceivedBy != null)
                    {
                        inscan.ReceivedBy = v.ReceivedBy;
                    }
                    inscan.CourierCharge = 0;
                    if (v.CourierCharges != null)
                    {
                        inscan.CourierCharge = v.CourierCharges;
                    }
                    inscan.OtherCharge = 0;
                    if (v.OtherCharges != null)
                    {
                        inscan.OtherCharge = v.OtherCharges;
                    }
                    inscan.ServiceCharge = 0;
                    if (v.ServiceCharges != null)
                    {
                        inscan.ServiceCharge = v.ServiceCharges;
                    }
                    inscan.PackingCharge = 0;
                    if (v.PackingCharges != null)
                    {
                        inscan.PackingCharge = v.PackingCharges;

                    }

                    if (v.paymentmode != null)
                    {
                        inscan.StatusPaymentMode = v.paymentmode;
                    }

                    inscan.OriginID = 0;
                    if (v.origincountry != null)
                    {
                        inscan.OriginID = v.origincountry;
                    }
                    if (v.destinationCountry != null)
                    {
                        inscan.DestinationID = v.destinationCountry.ToString();
                    }
                    inscan.ConsignorCityID = 0;
                    if (v.origincity != null)
                    {
                        inscan.ConsignorCityID = v.origincity;
                    }
                    inscan.ConsigneeCityID = 0;
                    if (v.destinationCity != null)
                    {
                        inscan.ConsigneeCityID = v.destinationCity;
                    }
                    if (v.CourierMode != null && v.CourierType != null)
                    {
                        inscan.TaxconfigurationID = (from tc1 in db.TaxConfigurations
                                                     where tc1.EffectFromDate <= DateTime.UtcNow && tc1.CourierMoveMentID == Convert.ToInt32(v.CourierMode) && tc1.ParcelTypeId== v.CourierType
                                                     select tc1.ParcelTypeId).FirstOrDefault();
                    }
                    inscan.CustomerID = 0;
                    if (v.CustomerID != null)
                    {
                        inscan.CustomerID = v.CustomerID;
                    }

                    inscan.Consignee = v.consignee;
                    inscan.Consignor = v.shipper;
                    inscan.ConsigneeAddress = v.Consigneeaddress;
                    inscan.ConsignorAddress = v.shipperaddress;
                    if (v.ConsigneePhone != null)
                    {
                        inscan.ConsigneePhone = v.ConsigneePhone;
                    }
                    if (v.shipperphone != null)
                    {
                        inscan.ConsignorPhone = v.shipperphone;
                    }

                    inscan.ConsigneeCountryID = 0;
                    if (v.destinationCountry != null)
                    {
                        inscan.ConsigneeCountryID = v.destinationCountry;
                    }
                    inscan.ConsignorCountryID = 0;
                    if (v.origincountry != null)
                    {
                        inscan.ConsignorCountryID = v.origincountry;
                    }
                    if (v.originlocation != null)
                    {
                        inscan.ConsignorLocation = v.originlocation;
                    }
                    if (v.destinationLocation != null)
                    {
                        inscan.ConsigneeLocation = v.destinationLocation;
                    }

                    inscan.AcTaxJournalID = 0;
                    inscan.BalanceAmt = 0;
                    if (v.TotalCharges != null)
                    {
                        inscan.BalanceAmt = v.TotalCharges;
                    }

                    inscan.TypeOfGoodID = 0;
                    if (v.InvoiceValue != null)
                    {
                        inscan.InvoiceValue = v.InvoiceValue;
                    }

                    inscan.MaterialCost = 0;
                    if (v.MaterialCost != null)
                    {
                        inscan.MaterialCost = v.MaterialCost;
                    }

                    inscan.ReferenceAWBNo = v.ReferenceNumber;
                    inscan.MaterialDescription = v.MaterialDescription;
                    if (v.RevisedRate != null)
                    {
                        inscan.RevisedRate = v.RevisedRate;
                    }
                    if (v.SpecialInstructions != null)
                    {
                        inscan.SpecialInstructions = v.SpecialInstructions;
                    }
                    inscan.ConsigneeAddress1 = "";
                    inscan.ConsigneeAddress2 = "";
                    inscan.ConsignorAddress1 = "";
                    inscan.ConsignorAddress2 = "";
                    inscan.ConsigneeContact = v.consigneecontact;
                    inscan.ConsignorContact = v.consigneecontact;
                    inscan.CargoDescription = v.CargoDescription;
                    inscan.HandlingInstruction = v.HandlingInstructions;

                    inscan.CustomsCollectedBy = 0;
                    if (v.ReceivedBy != null)
                    {
                        inscan.CustomsCollectedBy = v.ReceivedBy;
                    }
                    db.InScans.Add(inscan);
                    db.SaveChanges();


                    InScanInternationalDeatil isid = new InScanInternationalDeatil();


                    isid.InScanID = 0;
                    ////InScanInternationalDeatilID = Common.GetMaxNumber("InScanInternationalDeatilID", "InScanInternationalDeatils"),
                    isid.InScanID = inscan.InScanID;
                    ////InscanInternationalID = isi.InScanInternationalID,

                    isid.VerifiedWeight = 0;
                    if (v.VerifiedWeight != null)
                    {
                        isid.VerifiedWeight = Convert.ToDecimal(v.VerifiedWeight);
                    }
                    //if (txtForwardingCharge.Text != string.Empty)
                    //{
                    //    isid.ForwardingCharge = Convert.ToDecimal(txtForwardingCharge.Value);
                    //}

                    ////context.InScanInternationalDeatils.InsertOnSubmit(isid);


                    InScanInternational isi = new InScanInternational();

                    ////InScanInternationalID = Common.GetMaxNumber("InScanInternationalID", "InScanInternational"),
                    ////FAgentID = -1,

                    isi.FAgentID = 0;
                    if (v.ForwardingAgentID != null)
                    {
                        isi.FAgentID = v.ForwardingAgentID;
                    }
                    else
                    {
                        isi.FAgentID = 0;
                    }

                    if (v.ForwardingDate != null)
                    {
                        isi.ForwardingDate = v.ForwardingDate.Value;
                    }

                    isi.VerifiedWeight = 0;
                    if (v.VerifiedWeight != null)
                    {
                        isi.VerifiedWeight = v.VerifiedWeight;
                    }
                    //if (txtForwardingCharge.Text != string.Empty)
                    //{
                    //    isi.ForwardingCharge = Convert.ToDecimal(txtForwardingCharge.Value);
                    //}
                    isi.StatusAssignment = false;
                    isi.ForwardingAWBNo = v.ForwardingAWB;
                    isid.InscanInternationalID = isi.InScanInternationalID;
                    isi.InScanID = inscan.InScanID;
                    if (v.ForwardingAgentID != null)
                    {
                        isi.FAgentID = v.ForwardingAgentID;
                    }
                    if (v.ForwardingDate != null)
                    {
                        isi.ForwardingDate = v.ForwardingDate.Value;

                    }
                    db.InScanInternationals.Add(isi);
                    db.SaveChanges();

                    db.InScanInternationalDeatils.Add(isid);
                    db.SaveChanges();


                    int max = (from c in db.AWBStatus orderby c.AWBStatusID descending select c.AWBStatusID).FirstOrDefault();
                    if (max == null)
                    {
                        max = 1;
                    }
                    else
                    {
                        max = max + 1;
                    }

                    AWBStatu astat = new AWBStatu()
                    {
                        AWBStatusID = max,
                        StatusDescriptionID = inscan.InScanID,
                        AWBNO = AWBNo,
                        Date = inscan.InScanDate,
                        Status = "DetailsUpdated",
                        FormID = "IN",
                    };

                    db.AWBStatus.Add(astat);
                    db.SaveChanges();


                }
                catch (Exception e)
                {

                }

            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("Index");

        }


        public int GetMaxAcJournalID()
        {
            int x = (from c in db.AcJournalMasters orderby c.AcJournalID descending select c.AcJournalID).FirstOrDefault();
            return x;
        }

        public int GetMaxAcJournalDetailID()
        {
            int x = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();
            return x;
        }

        public int GetMaxInscanID()
        {
            int x = (from c in db.InScans orderby c.InScanID descending select c.InScanID).FirstOrDefault();
            return x;
        }
        public JsonResult GetPickUpData(string id)
        {
            PickUp objCust = new PickUp();
            var cust = (from c in db.CustomerEnquiries where c.AWBNo == id select c).FirstOrDefault();
            if (cust != null)
            {
                objCust.CustomerID = cust.CustomerID.Value;
                objCust.shipper = cust.Consignor;
                objCust.contactperson = cust.ConsignorContact;
                objCust.shipperaddress = cust.ConsignorAddress;
                objCust.shipperphone = cust.ConsignorPhone;
                objCust.shippercountry = cust.ConsignerCountryId.Value;
                objCust.shippercity = cust.ConsignerCityId.Value;
                objCust.shipperlocation = cust.ConsignorLocationName;
                objCust.weight = cust.Weight.Value;

                objCust.consignee = cust.Consignee;
                objCust.consigneecontact = cust.ConsigneeContact;
                objCust.consigneeaddress = cust.ConsigneeAddress;
                objCust.consigneephone = cust.ConsigneePhone;
                objCust.consigneecountry = cust.ConsigneeCountryID.Value;
                objCust.consigneecity = cust.ConsigneeCityId.Value;
                objCust.consigneelocation = cust.ConsigneeLocationName;

                objCust.Exist = 1;
            }
            else
            {
                objCust.Exist = 0;
            }



            return Json(objCust, JsonRequestBehavior.AllowGet);
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
            public String City { get; set; }
        }

        public JsonResult GetCity(int id)
        {
            List<CityM> objCity = new List<CityM>();
            var city = (from c in db.CityMasters where c.CountryID == id select c).ToList();

            foreach (var item in city)
            {
                objCity.Add(new CityM { City = item.City, CityID = item.CityID });

            }
            return Json(objCity, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetCustomerRate(int custRateType, int prodType, int LocationID, int x, float flWeight, string Status)
        {
            // var monCustomerRate = new ObjectParameter("monCustomerRate", typeof(decimal));
            CustRate obj = new CustRate();
            //var data = db.proGetCustomerRate(custRateType, prodType, LocationID, x, flWeight, Status, monCustomerRate);


            obj.crate = 40;
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public class CustRate
        {
            public decimal crate { get; set; }
        }

    }

}





