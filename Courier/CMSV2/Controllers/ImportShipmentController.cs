using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CMSV2.Models;
using CMSV2.DAL;

namespace CMSV2.Controllers
{
    [SessionExpireFilter]
    public class ImportShipmentController : Controller
    {
        private Entities1 db = new Entities1();

        public ActionResult Index()
        {
            //AuthHelp Token = repos.Authenticate();
            //if (Token.Status)
            //{
            DatePicker model = new DatePicker
            {
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                //Delete = (bool)Token.Permissions.Deletion,
                //Update = (bool)Token.Permissions.Updation,
                //Create = (bool)Token.Permissions.Creation
            };
            ViewBag.Token = model;
            SessionDataModel.SetTableVariable(model);
            ViewBag.Status = db.CourierStatus.Where(cc => cc.CourierStatus == "Received at Origin Facility" || cc.CourierStatus == "Forwarded to Agent").ToList();
            ViewBag.Agents = db.AgentMasters.ToList();
            ViewBag.StatusId = 0;
            ViewBag.AgentId = 0;
            return View(model);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "FromDate,ToDate,StatusId,AgentId")] DatePicker picker)
        {
            //AuthHelp Token = repos.Authenticate();
            //if (Token.Status)
            //{
            DatePicker model = new DatePicker
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                StatusId=picker.StatusId,
                AgentId=picker.AgentId
                //Delete = (bool)Token.Permissions.Deletion,
                //Update = (bool)Token.Permissions.Updation,
                //Create = (bool)Token.Permissions.Creation
            };
            ViewBag.Token = model;
            ViewBag.Status = db.CourierStatus.Where(cc => cc.CourierStatus == "Received at Origin Facility" || cc.CourierStatus == "Forwarded to Agent").ToList();
            ViewBag.Agents = db.AgentMasters.ToList();
            ViewBag.StatusId = model.StatusId;
            ViewBag.AgentId = model.AgentId;
            SessionDataModel.SetTableVariable(model);
            return View(model);
            //}
            //return RedirectToAction(Token.Function, Token.Controller);
        }

        public ActionResult Table()
        {
            DatePicker datePicker = SessionDataModel.GetTableVariable();
            ViewBag.Token = datePicker;
            List<ImportShipmentVM> importshipments = (from c in db.ImportShipments join a in db.AgentMasters on c.AgentID equals a.AgentID                                                     
                                                      where (c.AgentID == datePicker.AgentId || datePicker.AgentId == 0 || datePicker.AgentId == null) &&
                                                      (c.Status == datePicker.StatusId || datePicker.StatusId == 0 || datePicker.StatusId==null)
                                                      && c.CreatedDate >= datePicker.FromDate && c.CreatedDate <= datePicker.ToDate
                                                      select new ImportShipmentVM { AgentName = a.Name, ManifestNumber = c.ManifestNumber, FlightNo = c.FlightNo, MAWB = c.MAWB, TotalAWB = c.TotalAWB, Bags = c.Bags, RunNo = c.RunNo, Type=c.Type,CreatedDate=c.CreatedDate, OriginAirportCity = c.OriginAirportCity, DestinationAirportCity = c.DestinationAirportCity, OriginCity = c.ConsignorCityName }).ToList();

            //var s_ImportShipment = db.ImportShipments.Where(x => x.CreatedDate >= datePicker.FromDate && x.CreatedDate <= datePicker.ToDate && (x.AgentID== datePicker.AgentId || datePicker.AgentId==0)).OrderByDescending(x => x.CreatedDate);

            return PartialView("Table", importshipments.ToList());
            
        }

        // GET: ImportShipment/Details/5
        public ActionResult Details(int? id)
        {
            //AuthHelp Token = repos.Authenticate();
            //if (Token.Status)
            //{
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var s_ImportShipment = db.ImportShipments.Where(x => x.ID == id)
                .FirstOrDefault();
            if (s_ImportShipment == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Edit = Token.Permissions.Updation;
            return View(s_ImportShipment);
            //}
            //return RedirectToAction(Token.Function, Token.Controller);
        }

        // GET: ImportShipment/Create
        public ActionResult Create(int? id=0)
        {
            var userid = Convert.ToInt32(Session["UserID"]);            
            var agent = db.AgentMasters.Where(cc => cc.UserID == userid).FirstOrDefault();            
            var company = db.AcCompanies.FirstOrDefault(); // .Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.Phone, x.AcCompany1, x.CountryName }).FirstOrDefault();
            ImportShipmentFormModel model = new ImportShipmentFormModel();
            if (id == 0)
            {
                model.ConsignorName = agent.Name;
                model.ConsignorCountryName = agent.CountryName;
                model.ConsignorCityName = agent.CityName;
                model.ConsignorAddress = agent.Address1 + "," + agent.Address2 + "," + agent.Address3;
                model.ConsigneeName = company.AcCompany1;
                model.ConsigneeCityName = company.CityName;
                model.ConsigneeCountryName = company.CountryName;
                model.ConsigneeAddress = company.Address1 + "," + company.Address2 + "," + company.Address3;
                long manifestNumber = 0;
                manifestNumber = db.ImportShipments.Where(x => x.AgentID == agent.AgentID).Count() + 1;
                var Manifest = $"{DateTime.Now.ToString("yyyyMMdd")}{agent.AgentCode}{(manifestNumber.ToString("D4"))}";

                model.ManifestNumber = Manifest;
                model.FlightDate = DateTime.Now;
                model.CreatedDate = DateTime.Now;
                
                if (model.Shipments == null)
                {
                    model.Shipments = new List<ImportShipmentDetail>();
                }
            }
            else
            {
                model = db.ImportShipments.Where(x => x.ID == id).Select(x => new ImportShipmentFormModel
                {
                    ID = x.ID,
                    AgentID = x.AgentID,
                    AgentLoginID = x.AgentLoginID,                    
                    Bags = x.Bags,
                    CD = x.CD,
                    ConsignorName =x.ConsignorName,
                    ConsignorAddress1_Building =x.ConsignorAddress1_Building,
                    ConsignorAddress2_Street = x.ConsignorAddress2_Street,
                    ConsignorAddress3_PinCode =x.ConsignorAddress3_PinCode,
                    ConsignorCountryName = x.ConsignorCountryName,
                    ConsignorCityName = x.ConsignorCityName,
                    ConsigneeName=x.ConsigneeName,
                    ConsigneeAddress1_Building = x.ConsigneeAddress1_Building,
                    ConsigneeAddress2_Street = x.ConsigneeAddress2_Street,
                    ConsigneeAddress3_PinCode = x.ConsigneeAddress3_PinCode,
                    ConsigneeCountryName = x.ConsigneeCountryName,
                    ConsigneeCityName = x.ConsigneeCityName,
                    CreatedDate = x.CreatedDate,
                    FlightDate = x.FlightDate,              
                    FlightNo = x.FlightNo,
                    LastEditedByLoginID = x.LastEditedByLoginID,
                    ManifestNumber = x.ManifestNumber,
                    MAWB = x.MAWB,
                    OriginAirportCity = x.OriginAirportCity,
                    DestinationAirportCity =x.DestinationAirportCity,
                    RunNo = x.RunNo,
                    TotalAWB = x.TotalAWB,
                    Type = x.Type                    
                }).FirstOrDefault();

                model.ConsigneeAddress = model.ConsigneeAddress1_Building + "," + model.ConsigneeAddress2_Street + "\n" + model.ConsigneeAddress3_PinCode;
                 model.ConsignorAddress = model.ConsignorAddress1_Building + "," + model.ConsignorAddress2_Street + "\n" + model.ConsignorAddress3_PinCode;

                 model.Shipments = db.ImportShipmentDetails.Where(d => d.ImportID == id).ToList();
            }

            string selectedVal = model.Type;
            var types = new List<SelectListItem>
            {
                new SelectListItem{Text = "Select Shipment Type", Value = null, Selected = selectedVal == null},
                new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
                new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
            };

            ViewBag.Type = types; 
            var currency = new SelectList(db.CurrencyMasters.OrderBy(x => x.CurrencyName), "CurrencyID", "CurrencyName").ToList();
            ViewBag.CurrencyID = db.CurrencyMasters.ToList();  // db.CurrencyMasters.ToList();
            ViewBag.Currencies = db.CurrencyMasters.ToList();
            ViewBag.AgentName = agent.Name;
            ViewBag.AgentCity = agent.CityName;
            ViewBag.CompanyName = company.AcCompany1;
            return View(model);
        
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ImportShipmentFormModel model)
    {
        var today = DateTime.Now.Date;
            var userid = Convert.ToInt32(Session["UserID"]); 
            //var agent1 = (from u2 in db.UserRegistrations join c1 in db.AgentMasters on u2.UserID equals c1.UserID where u2.UserID == userid select c1).FirstOrDefault();
            //var user = (from u2 in db.UserRegistrations select u2).FirstOrDefault();

            var agent = db.AgentMasters.Where(aa=>aa.UserID==userid).FirstOrDefault();
            var fwdagent = db.ForwardingAgentMasters.FirstOrDefault();

            //var company = db.AcCompanies.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.Phone, x.AcCompany1, x.CountryName }).FirstOrDefault();

            var company = db.AcCompanies.FirstOrDefault(); //. Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.Phone, x.AcCompany1, x.CountryName }).FirstOrDefault();
            ImportShipment importShipment = new ImportShipment();

            if (ModelState.IsValid)
            {
                if (model.ID == 0)
                {
                    importShipment.ID = db.ImportShipments.Select(x => x.ID).DefaultIfEmpty(0).Max() + 1;

                    long manifestNumber = 0;
                    manifestNumber = db.ImportShipments.Where(x => x.AgentID == agent.AgentID).Count() + 1;
                    var Manifest = $"{DateTime.Now.ToString("yyyyMMdd")}{agent.AgentCode}{(manifestNumber.ToString("D4"))}";
                    importShipment.ManifestNumber = Manifest;
                    importShipment.AgentID = agent.AgentID;
                    importShipment.CreatedDate = DateTime.Now;
                    importShipment.ConsignorName = agent.Name;
                    importShipment.ConsignorCityName = agent.CityName;
                    importShipment.ConsignorCountryName = agent.CountryName;
                    importShipment.ConsignorAddress1_Building = agent.Address1;
                    importShipment.ConsignorAddress2_Street = agent.Address2;
                    importShipment.ConsignorAddress3_PinCode = agent.Address3;
                    importShipment.ConsignorLocationName = agent.LocationName;
                    importShipment.ConsigneeName = company.AcCompany1;
                    importShipment.ConsigneeCountryName = company.CountryName;
                    importShipment.ConsigneeCityName = company.CityName;
                    importShipment.ConsigneeLocationName = company.LocationName;
                    importShipment.ConsigneeAddress1_Building = company.Address1;
                    importShipment.ConsigneeAddress2_Street = company.Address2;
                    importShipment.ConsigneeAddress3_PinCode = company.Address3;
                    importShipment.AgentLoginID = userid;
                }
                else
                {
                    importShipment = db.ImportShipments.Find(model.ID);
                }
                
                
                importShipment.Bags = model.Bags;
                importShipment.CD = model.CD;
                importShipment.FlightDate = model.FlightDate;                
                importShipment.FlightNo = model.FlightNo;
                importShipment.LastEditedByLoginID = userid;                                
                importShipment.MAWB = model.MAWB;
                importShipment.RunNo = model.RunNo;
                importShipment.TotalAWB = model.TotalAWB;
                importShipment.Type = model.Type;
                importShipment.DestinationAirportCity = model.DestinationAirportCity;
                importShipment.OriginAirportCity = model.OriginAirportCity;

                if (model.ID == 0)
                {
                    db.ImportShipments.Add(importShipment);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(importShipment).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var max = db.ImportShipmentDetails.Select(x => x.ShipmentDetailID).DefaultIfEmpty(0).Max() + 1;
                if (model.Shipments != null)
                
                {
                    //model.Shipments.ForEach(x =>
                    //{
                    //    x.ShipmentDetailID = max;
                    //    x.ImportID = importShipment.ID;
                    //    x.Status = 1;
                    //    max++;
                    //});
                    foreach(var det in model.Shipments)
                    {
                        if  (det.ShipmentDetailID==0 )
                        {
                            det.ShipmentDetailID = max;
                            det.ImportID = importShipment.ID;                            
                            det.StatusTypeId = db.tblStatusTypes.Where(cc => cc.Name == "FORWARDED").FirstOrDefault().ID;
                            det.CourierStatusID = db.CourierStatus.Where(cc => cc.CourierStatus == "Forwarded to Agent").FirstOrDefault().CourierStatusID;
                            db.ImportShipmentDetails.Add(det);
                            db.SaveChanges();
                            max++;
                        }
                        else
                        {
                            det.ImportID = importShipment.ID;
                            det.StatusTypeId = db.tblStatusTypes.Where(cc => cc.Name == "FORWARDED").FirstOrDefault().ID;
                            det.CourierStatusID = db.CourierStatus.Where(cc => cc.CourierStatus == "Forwarded to Agent").FirstOrDefault().CourierStatusID;
                            db.Entry(det).State= EntityState.Modified; 
                            db.SaveChanges();
                        }

                        //updateing awbstaus table for tracking
                        AWBTrackStatu _awbstatus = new AWBTrackStatu();
                        int? id = (from c in db.AWBTrackStatus orderby c.AWBTrackStatusId descending select c.AWBTrackStatusId).FirstOrDefault();

                        if (id == null)
                            id = 1;
                        else
                            id = id + 1;

                        _awbstatus.AWBTrackStatusId = Convert.ToInt32(id);
                        _awbstatus.AWBNo = det.HAWB;
                        _awbstatus.EntryDate = DateTime.Now;
                        _awbstatus.ShipmentDetailID = det.ShipmentDetailID;
                        _awbstatus.StatusTypeId =Convert.ToInt32(det.StatusTypeId);
                        _awbstatus.CourierStatusId =Convert.ToInt32(det.CourierStatusID);
                        _awbstatus.ShipmentStatus = db.tblStatusTypes.Find(det.StatusTypeId).Name;
                        _awbstatus.CourierStatus = db.CourierStatus.Find(det.CourierStatusID).CourierStatus;
                        _awbstatus.UserId = userid;

                        db.AWBTrackStatus.Add(_awbstatus);
                        db.SaveChanges();
                    }

                    var li = model.Shipments.Select(x => x.ShipmentDetailID).ToList();
                    var importdetails = db.ImportShipmentDetails.Where(x => !li.Contains(x.ShipmentDetailID) && x.ImportID == importShipment.ID).ToList();
                    foreach (var import in importdetails)
                    {
                        db.ImportShipmentDetails.Add(import);
                        db.SaveChanges();
                       
                    }
                    
                }
                else
                {                     
                        var remove_datas = db.ImportShipmentDetails.Where(s => s.ImportID == importShipment.ID).ToList();
                        foreach (var rdata in remove_datas)
                        {
                            db.ImportShipmentDetails.Remove(rdata);
                            db.SaveChanges();

                        }
                    
                }
                return RedirectToAction("Index");
            }
            model.ConsignorName = agent.Name;
            model.ConsignorCountryName = agent.CountryName;
            model.ConsignorCityName = agent.CityName;
            model.ConsignorAddress = agent.Address1 + "," + agent.Address2 + "," + agent.Address3;

            model.ConsigneeName = company.AcCompany1;
            model.ConsigneeCityName = company.CityName;
            model.ConsigneeCountryName = company.CountryName;
            model.ConsigneeAddress = company.Address1 + "," + company.Address2 + "," + company.Address3;
                   


            if (model.Shipments == null)
            {
                model.Shipments = new List<ImportShipmentDetail>();
            }
            var countries = db.CountryMasters.ToList();// db.CountryMasters.ToList();
            ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
            ViewBag.DestinationCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
            ViewBag.OriginCountryID = countries;
            ViewBag.DestinationCountryID = countries;
            ViewBag.CurrencyID = db.CurrencyMasters.ToList();
            ViewBag.Cities = db.CityMasters.ToList();
            ViewBag.Countries = db.CountryMasters.ToList();
            string selectedVal = null;
            var types = new List<SelectListItem>
            {
                new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
                new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
            };
            ViewBag.Type = types;
            ViewBag.Currencies = db.CurrencyMasters.ToList();
            ViewBag.AgentName = agent.Name;
            ViewBag.AgentCity = agent.CityName;
            ViewBag.CompanyName = company.AcCompany1;

            return View(model);
       
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddOrRemoveShipment(ImportShipmentFormModel s_ImportShipment, int? i)
    {
            var Prevshipmentsession = Session["PreviousShipments"] as List<ImportShipmentDetail>;

            if (i.HasValue)
        {
                if (Prevshipmentsession == null)
                {

                }
                else
                {
                    foreach (var item in Prevshipmentsession)
                    {
                        s_ImportShipment.Shipments.Add(item);
                    }
                }
                s_ImportShipment.Shipments.RemoveAt(i.Value);
                Session["PreviousShipments"] = s_ImportShipment.Shipments;
            }
            else
        {
            if (s_ImportShipment.Shipments == null)
            {
                s_ImportShipment.Shipments = new List<ImportShipmentDetail>();
            }
            var shipmentsession = Session["Shipmentdetails"] as ImportShipmentDetail;
            var Serialnumber = Convert.ToInt32(Session["ShipSerialNumber"]);
            var isupdate = Convert.ToBoolean(Session["IsUpdate"]);
                if (Prevshipmentsession == null)
                {

                }
                else
                {
                    foreach (var item in Prevshipmentsession)
                    {
                        s_ImportShipment.Shipments.Add(item);
                    }
                }
                if (isupdate == true)
            {
                //s_ImportShipment.Shipments.RemoveAt(Serialnumber);                  
                s_ImportShipment.Shipments[Serialnumber] = shipmentsession;
            }
            else
            {
                s_ImportShipment.Shipments.Add(shipmentsession);
            }
            Session["Shipmentdetails"] = new ImportShipmentDetail();
            Session["ShipSerialNumber"] = "";
            Session["IsUpdate"] = false;
        }
        ViewBag.Cities = db.CityMasters.ToList();
        ViewBag.Countries = db.CountryMasters.ToList();
        ViewBag.DestinationCountryID = db.CountryMasters.ToList();
            ViewBag.CurrencyID = db.CurrencyMasters.ToList();
            ViewBag.Currencies = db.CurrencyMasters.ToList();

            return PartialView("ShipmentList", s_ImportShipment);
    }
    public bool AddShippmentToTable(FormCollection data)
    {
        var shipmentmodel = new ImportShipmentDetail();
        shipmentmodel.CurrencyID = Convert.ToInt32(data["tCurrencyID"]);
        shipmentmodel.AWB = data["tAWB"];
        shipmentmodel.HAWB = data["tHAWB"];
        shipmentmodel.BagNo = data["tBagNo"];
        shipmentmodel.PCS = Convert.ToInt32(data["tPCS"]);
        shipmentmodel.Weight = Convert.ToDecimal(data["tWeight"]);
        shipmentmodel.Value = Convert.ToDecimal(data["tValue"]);
        shipmentmodel.Shipper = data["tShipper"];
        shipmentmodel.Reciver = data["tReciver"];
        shipmentmodel.Contents = data["tContents"];
        shipmentmodel.DestinationCountry = data["tDestinationCountryID"];
        shipmentmodel.DestinationCity = data["tDestinationCityID"];
        shipmentmodel.ShipmentDetailID = Convert.ToInt32(data["tId"]);
        Session["Shipmentdetails"] = shipmentmodel;
        Session["ShipSerialNumber"] = Convert.ToInt32(data["tSerialNum"]);
        Session["IsUpdate"] = Convert.ToBoolean(data["isupdate"]);
        return true;
    }

    public JsonResult GetShipmentDetails(ImportShipmentFormModel s_ImportShipment, int? i)
    {
        if (i.HasValue)
        {
            var s = s_ImportShipment.Shipments[i.Value];
            return Json(new { success = true, data = s, ival = i.Value }, JsonRequestBehavior.AllowGet);
        }
        else
        {
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

    }
    //// GET: ImportShipment/Edit/5
    //public ActionResult Edit(int? id)
    //{
      
    //        var data = db.ImportShipments.Where(x => x.ID == id).Select(x => new ImportShipmentFormModel
    //        {
    //            ID = x.ID,
    //            AgentID = x.AgentID,
    //            AgentLoginID = x.AgentLoginID,
    //            AirportOfShipmentID = x.AirportOfShipmentID,
    //            Bags = x.Bags,
    //            CD = x.CD,
    //            ConsigneeAddress = x.ConsigneeAddress,
    //            ConsignorAddress = x.ConsignorAddress,
    //            CreatedDate = x.CreatedDate,
    //            Date = x.Date,
    //            DestinationCityID = x.DestinationCityID,
    //            DestinationCountryID = x.DestinationCountryID,
    //            DestinationAirportOfShipmentId = x.DestinationAirportOfShipmentId,
    //            FlightNo = x.FlightNo,
    //            LastEditedByLoginID = x.LastEditedByLoginID,
    //            ManifestNumber = x.ManifestNumber,
    //            MAWB = x.MAWB,               
    //            OriginCountryID = x.OriginCountryID,
    //            RunNo = x.RunNo,
    //            TotalAWB = x.TotalAWB,
    //            Type = x.Type,
    //            OriginCity = x.OriginCity,
    //            DestinationCity = x.DestinationCity,
    //            OrginCountry = x.OrginCountry,
    //            DestinationCountry = x.DestinationCountry,
    //        }).FirstOrDefault();
    //        data.Shipments = db.ImportShipmentDetails.Where(d => d.ImportID == id).ToList();
    //        var countries = db.CountryMasters.ToList();// db.CountryMasters.ToList();
    //        ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
    //        ViewBag.DestinationCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
    //        ViewBag.OriginCountryID = countries;
    //        ViewBag.DestinationCountryID = countries;
    //        ViewBag.Cities = db.CityMasters.ToList();
    //        ViewBag.Countries = db.CountryMasters.ToList();
    //        string selectedVal = null;
    //        var types = new List<SelectListItem>
    //        {
    //            new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
    //            new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
    //        };
    //        ViewBag.Type = types;
    //        var userid = Convert.ToInt32(Session["UserID"]);

    //        ViewBag.CurrencyID = db.CurrencyMasters.ToList();  // db.CurrencyMasters.ToList();
    //        ViewBag.Currencies = db.CurrencyMasters.ToList();
    //        var agent1 = (from u2 in db.UserRegistrations join c1 in db.AgentMasters on u2.UserID equals c1.UserID where u2.UserID == userid select c1).FirstOrDefault();
    //        var agent = db.AgentMasters.FirstOrDefault();

    //        var company = db.AcCompanies.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.Phone, x.AcCompany1 }).FirstOrDefault();
    //        ViewBag.AgentName = agent.Name;
    //        ViewBag.CompanyName = company.AcCompany1;
    //        ViewBag.AgentCity = agent.CityName;

    //        return View(data);
      
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult Edit(ImportShipmentFormModel model)
    //{
    //    var company = db.AcCompanies.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.AcCompany1, x.CountryName }).FirstOrDefault();
    //        var agent = db.AgentMasters.FirstOrDefault();
    //        var fwdagent = db.ForwardingAgentMasters.FirstOrDefault();

    //        var today = DateTime.Now.Date;
       
    //        var importShipment = db.ImportShipments.Find(model.ID);
    //        var userid = Convert.ToInt32(Session["UserID"]);

    //        if (ModelState.IsValid)
    //        {
    //            importShipment.AgentLoginID = fwdagent.FAgentID;
    //            importShipment.AirportOfShipmentID = 0;
    //            importShipment.DestinationCountryID = Convert.ToInt32(company.CountryID);
    //            importShipment.DestinationAirportOfShipmentId = 0;
    //            importShipment.Bags = model.Bags;
    //            importShipment.CD = model.CD;
    //            importShipment.Date = model.Date;
    //            importShipment.DestinationCityID = 0;
    //            importShipment.FlightNo = model.FlightNo;
    //            importShipment.LastEditedByLoginID = userid;
    //            importShipment.MAWB = model.MAWB;
    //            importShipment.RunNo = model.RunNo;
    //            importShipment.TotalAWB = model.TotalAWB;
    //            importShipment.Type = model.Type;
    //            importShipment.Status = 1;
    //            importShipment.DestinationCity = model.DestinationCity;
    //            importShipment.OriginCity = model.OriginCity;
    //            importShipment.DestinationCountry = company.CountryName;
    //            db.Entry(importShipment).State = EntityState.Modified;
    //            db.SaveChanges();
    //            var max = db.ImportShipmentDetails.Select(x => x.ShipmentDetailID).DefaultIfEmpty(0).Max() + 1;
    //            if (model.Shipments != null)
    //            {
    //                model.Shipments.ForEach(x =>
    //            {
    //                x.ImportID = importShipment.ID;
    //                if (x.ShipmentDetailID == 0)
    //                {
    //                    x.ShipmentDetailID = max;

    //                    max++;
    //                    db.ImportShipmentDetails.Add(x);
    //                }
    //                else
    //                {
    //                    db.Entry(x).State = EntityState.Modified;
    //                }
    //            });
    //                var li = model.Shipments.Select(x => x.ShipmentDetailID).ToList();
    //                var remove_datas = db.ImportShipmentDetails.Where(x => !li.Contains(x.ShipmentDetailID) && x.ImportID == importShipment.ID).ToList();
    //                foreach(var rdata in remove_datas)
    //                {
    //                    db.ImportShipmentDetails.Remove(rdata);
    //                    db.SaveChanges();

    //                }

    //            }
    //            else
    //            {
    //                var remove_datas = db.ImportShipmentDetails.Where(s => s.ImportID == importShipment.ID).ToList();
    //                foreach (var rdata in remove_datas)
    //                {
    //                    db.ImportShipmentDetails.Remove(rdata);
    //                    db.SaveChanges();

    //                }

    //            }


    //            return RedirectToAction("Index");
    //        }
    //        model.ConsignorAddress = importShipment.ConsignorAddress;
    //        model.OriginCity = importShipment.OriginCity;
    //        model.OriginCountryID = importShipment.OriginCountryID;
    //        model.ConsigneeAddress = importShipment.ConsigneeAddress;
    //        model.DestinationCountryID = importShipment.DestinationCountryID;
    //        if (model.Shipments == null)
    //        {
    //            model.Shipments = new List<ImportShipmentDetail>();
    //        }
    //        var countries = db.CountryMasters.ToList();// db.CountryMasters.ToList();
    //        ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
    //        ViewBag.DestinationCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
    //        ViewBag.OriginCountryID = countries;
    //        ViewBag.DestinationCountryID = countries;

    //        ViewBag.CurrencyID = db.CurrencyMasters.ToList();  // db.CurrencyMasters.ToList();
    //        ViewBag.Currencies = db.CurrencyMasters.ToList();
    //        ViewBag.Cities = db.CityMasters.ToList();
    //        ViewBag.Countries = db.CountryMasters.ToList();
    //        string selectedVal = null;

    //        var types = new List<SelectListItem>
    //        {
    //            new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
    //            new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
    //        };
    //        ViewBag.Type = types;
    //        var agent1 = (from u2 in db.UserRegistrations join c1 in db.AgentMasters on u2.UserID equals c1.UserID where u2.UserID == userid select c1).FirstOrDefault();

    //        ViewBag.AgentName = agent.Name;
    //        ViewBag.CompanyName = company.AcCompany1;
    //        ViewBag.AgentCity = agent.CityName;

    //        return View(model);
        
    //}

    // POST: ImportShipment/Delete/5
    [ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
       
            try
            {
                ImportShipment s_ImportShipment = db.ImportShipments.Find(id);
                List<ImportShipmentDetail> Details = db.ImportShipmentDetails.Where(d => d.ImportID == id).ToList();
                db.ImportShipments.Remove(s_ImportShipment);
                foreach(var det in Details)
                {
                    db.ImportShipmentDetails.Remove(det);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
      
    }

    //[ActionName("Shipment")]
    //public ActionResult GetShipmentImport(int? id)
    //{
       
    //        var enquiry = id == null ? null : repos.GetImportShipment(id.Value);
    //        if (enquiry != null)
    //        {
    //            ReportDocument report = new ReportDocument();
    //            report.Load(Server.MapPath("/CrystalReport/Importshipment.rpt"));
    //            TextObject txtManifestNo = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["txtManifestNo"];
    //            TextObject TxtDate = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtDate"];
    //            TextObject txtAgentAddress = (TextObject)report.ReportDefinition.Sections["Section5"].ReportObjects["txtAgentAddress"];
    //            TextObject Txtorigin = (TextObject)report.ReportDefinition.Sections["Section5"].ReportObjects["Txtorigin"];
    //            TextObject TxtConsigneAddr = (TextObject)report.ReportDefinition.Sections["Section4"].ReportObjects["TxtConsigneAddr"];
    //            TextObject TxtDestination = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtDestination"];
    //            TextObject txtFlightDate = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["txtFlightDate"];
    //            TextObject TxtAirport = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtAirport"];
    //            TextObject TxtFlightNo = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtFlightNo"];
    //            TextObject TxtMAWB = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtMAWB"];
    //            TextObject TxtCD = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtCD"];
    //            TextObject Txtbags = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["Txtbags"];
    //            TextObject TxtRunNo = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtRunNo"];
    //            TextObject TxtType = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtType"];
    //            TextObject TotAWB = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TotAWB"];
    //            report.SetDataSource(enquiry.ImportShipmentDetails.ToList());
    //            TxtDate.Text = DateTime.Now.ToShortDateString();
    //            txtManifestNo.Text = enquiry.ManifestNumber;
    //            txtAgentAddress.Text = enquiry.ConsignorAddress;
    //            Txtorigin.Text = enquiry.OriginCity;
    //            TxtConsigneAddr.Text = enquiry.ConsigneeAddress;
    //            TxtDestination.Text = enquiry.DestinationCity;
    //            txtFlightDate.Text = enquiry.Date.ToShortDateString();
    //            TxtFlightNo.Text = enquiry.FlightNo;
    //            TxtAirport.Text = enquiry.AirportOfShipment;
    //            TxtMAWB.Text = enquiry.MAWB;
    //            TxtCD.Text = enquiry.CD;
    //            Txtbags.Text = enquiry.Bags.ToString();
    //            TxtRunNo.Text = enquiry.RunNo;
    //            TxtType.Text = enquiry.Type;
    //            TotAWB.Text = enquiry.TotalAWB.ToString();
    //            //report.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.Excel,Response,false,"");
    //            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
    //            stream.Seek(0, System.IO.SeekOrigin.Begin);
    //            return new FileStreamResult(stream, "application/pdf");
    //        }
    //        else
    //        {
    //            return RedirectToAction("PageNotFound", "Error");
    //        }
    //    }
    //    return RedirectToAction(Token.Function, Token.Controller);
    //}

    //[ActionName("Shipmentxl")]
    //public ActionResult GetShipmentImportExcel(int? id)
    //{
    //    AuthHelp Token = repos.Authenticate();
    //    if (Token.Status)
    //    {
    //        var enquiry = id == null ? null : repos.GetImportShipment(id.Value);
    //        if (enquiry != null)
    //        {
    //            ReportDocument report = new ReportDocument();
    //            report.Load(Server.MapPath("/CrystalReport/Importshipment.rpt"));
    //            TextObject txtManifestNo = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["txtManifestNo"];
    //            TextObject TxtDate = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtDate"];
    //            TextObject txtAgentAddress = (TextObject)report.ReportDefinition.Sections["Section5"].ReportObjects["txtAgentAddress"];
    //            TextObject Txtorigin = (TextObject)report.ReportDefinition.Sections["Section5"].ReportObjects["Txtorigin"];
    //            TextObject TxtConsigneAddr = (TextObject)report.ReportDefinition.Sections["Section4"].ReportObjects["TxtConsigneAddr"];
    //            TextObject TxtDestination = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtDestination"];
    //            TextObject txtFlightDate = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["txtFlightDate"];
    //            TextObject TxtAirport = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtAirport"];
    //            TextObject TxtFlightNo = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtFlightNo"];
    //            TextObject TxtMAWB = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtMAWB"];
    //            TextObject TxtCD = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtCD"];
    //            TextObject Txtbags = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["Txtbags"];
    //            TextObject TxtRunNo = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtRunNo"];
    //            TextObject TxtType = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TxtType"];
    //            TextObject TotAWB = (TextObject)report.ReportDefinition.Sections["Section1"].ReportObjects["TotAWB"];
    //            report.SetDataSource(enquiry.ImportShipmentDetails.ToList());
    //            TxtDate.Text = DateTime.Now.ToShortDateString();
    //            txtManifestNo.Text = enquiry.ManifestNumber;
    //            txtAgentAddress.Text = enquiry.ConsignorAddress;
    //            Txtorigin.Text = enquiry.OriginCity;
    //            TxtConsigneAddr.Text = enquiry.ConsigneeAddress;
    //            TxtDestination.Text = enquiry.DestinationCity;
    //            txtFlightDate.Text = enquiry.Date.ToShortDateString();
    //            TxtFlightNo.Text = enquiry.FlightNo;
    //            TxtAirport.Text = enquiry.AirportOfShipment;
    //            TxtMAWB.Text = enquiry.MAWB;
    //            TxtCD.Text = enquiry.CD;
    //            Txtbags.Text = enquiry.Bags.ToString();
    //            TxtRunNo.Text = enquiry.RunNo;
    //            TxtType.Text = enquiry.Type;
    //            TotAWB.Text = enquiry.TotalAWB.ToString();
    //            //report.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.Excel,Response,false,"");
    //            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
    //            stream.Seek(0, System.IO.SeekOrigin.Begin);
    //            return new FileStreamResult(stream, "application/vnd.ms-excel");
    //        }
    //        else
    //        {
    //            return RedirectToAction("PageNotFound", "Error");
    //        }
    //    }
    //    return RedirectToAction(Token.Function, Token.Controller);
    //}


    //protected override void Dispose(bool disposing)
    //{
    //    if (disposing)
    //    {
    //        db.Dispose();
    //    }
    //    base.Dispose(disposing);
    //}
    //public ActionResult ImportShipmentReport(int id)
    //{
    //    AuthHelp Token = repos.Authenticate();
    //    if (Token.Status)
    //    {
    //        ViewBag.Id = id;
    //        return View();
    //    }
    //    return RedirectToAction(Token.Function, Token.Controller);
    //}
}
}
