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
    public class ReExportShipmentController : Controller
    {
        private Entities1 db = new Entities1();
        //private Repos repos = new Repos();

        // GET: ExportShipment
        public ActionResult Index()
        {
           // AuthHelp Token = repos.Authenticate();
            //if (1==1)
            //{
                DatePicker model = new DatePicker
                {
                    FromDate = DateTime.Now.Date,
                    ToDate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59)
              //      Delete = (bool)Token.Permissions.Deletion,
                //    Update = (bool)Token.Permissions.Updation,
                  //  Create = (bool)Token.Permissions.Creation
                };
                ViewBag.Token = model;
                SessionDataModel.SetTableVariable(model);
                return View(model);
           

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "FromDate,ToDate")] DatePicker picker)
        {
            //AuthHelp Token = repos.Authenticate();
            //if (1==1)
            //{
                DatePicker model = new DatePicker
                {
                    FromDate = picker.FromDate,
                    ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                    Delete = true, // (bool)Token.Permissions.Deletion,
                    Update = true, //(bool)Token.Permissions.Updation,
                    Create = true //.ToStrin//(bool)Token.Permissions.Creation
                };
                ViewBag.Token = model;
                SessionDataModel.SetTableVariable(model);
                return View(model);
            //}
            ////return RedirectToAction(Token.Function, Token.Controller);
        }

        public ActionResult Table()
        {
            //AuthHelp Token = repos.Authenticate();
        
                DatePicker datePicker = SessionDataModel.GetTableVariable();
                ViewBag.Token = datePicker;
                var s_ImportShipment = db.ExportShipments; //.Where(d => d.ID == id).FirstOrDefault();              
                return PartialView("Table", s_ImportShipment.ToList());
        
            // //return RedirectToAction(Token.Function, Token.Controller);
          //  return View();
        }
        public ActionResult CreateExport(int id=0)
        {
            var userid = Convert.ToInt32(Session["UserID"]);            
            var today = DateTime.Now.Date;
            Session["PreviousShipments"] = new List<ExportShipmentDetail>();
            var company = db.AcCompanies.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryName, x.CountryID, x.Phone, x.AcCompany1, x.CityName }).FirstOrDefault();
            var Emp = db.EmployeeMasters.Where(x => x.UserID == userid).Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.EmployeeName, x.EmployeeCode, x.EmployeeID }).FirstOrDefault();

            ExportShipmentFormModel _ExportShipment = new ExportShipmentFormModel();
            if (id == 0) //new entry
            {
                long manifestNumber = 0;
                _ExportShipment.ID = 0;
                _ExportShipment.ConsignorName = company.AcCompany1;
                _ExportShipment.ConsignorAddress = company.Address;
                _ExportShipment.ConsignorCountryName = company.CountryName;
                _ExportShipment.ConsignorCityName = company.CityName;
                _ExportShipment.OriginAirportCity = company.CityName;
                _ExportShipment.FlightDate = DateTime.Now;
                _ExportShipment.CreatedDate = DateTime.Now;

                string ManifestNumber = $"{DateTime.Now.ToString("yyyyMMdd")}{Emp.EmployeeCode}{(db.ExportShipments.Where(x => x.EmployeeID == Emp.EmployeeID && x.CreatedDate >= today).Count() + 1).ToString("D4")}";

                _ExportShipment.ManifestNumber = ManifestNumber;


                if (_ExportShipment.Shipments == null)
                {
                    _ExportShipment.Shipments = new List<ExportShipmentDetail>();
                }
            }
            else
            {
                var exportshipment = db.ExportShipments.Find(id);
                _ExportShipment.ID = exportshipment.ID;
                _ExportShipment.ManifestNumber = exportshipment.ManifestNumber;
                _ExportShipment.Type = exportshipment.Type;
                _ExportShipment.FlightNo = exportshipment.FlightNo;
                _ExportShipment.RunNo = exportshipment.RunNo;
                _ExportShipment.ConsignorName = exportshipment.ConsignorName;
                _ExportShipment.ConsignorCountryName = exportshipment.ConsignorCountryName;
                _ExportShipment.ConsignorCityName = exportshipment.ConsignorCityName;
                _ExportShipment.ConsignorAddress = exportshipment.ConsignorAddress1_Building + ',' + exportshipment.ConsignorAddress2_Street + "," + exportshipment.ConsignorAddress3_PinCode;
                _ExportShipment.ConsigneeName = exportshipment.ConsigneeName;
                _ExportShipment.ConsigneeAddress = exportshipment.ConsigneeAddress1_Building + ',' + exportshipment.ConsigneeAddress2_Street + "," + exportshipment.ConsigneeAddress3_PinCode;
                _ExportShipment.ConsigneeCountryName = exportshipment.ConsigneeCountryName;
                _ExportShipment.ConsigneeCityName = exportshipment.ConsigneeCityName;
                _ExportShipment.AgentID = exportshipment.AgentID;
                _ExportShipment.Bags = exportshipment.Bags;
                _ExportShipment.CD = exportshipment.CD;                
                _ExportShipment.MAWB = exportshipment.MAWB;
                _ExportShipment.TotalAWB = exportshipment.TotalAWB;
                _ExportShipment.FlightDate = exportshipment.FlightDate;
                _ExportShipment.CreatedDate = exportshipment.CreatedDate;
                _ExportShipment.OriginAirportCity = exportshipment.OriginAirportCity;
                _ExportShipment.DestinationAirportCity = exportshipment.DestinationAirportCity;
                _ExportShipment.Shipments = db.ExportShipmentDetails.Where(d => d.ExportID == id).ToList();
                Session["PreviousShipments"] = _ExportShipment.Shipments;
            }                               
                
                string selectedVal = _ExportShipment.Type;

                var types = new List<SelectListItem>
            {
                new SelectListItem{Text = "Select Shipment Type", Value = null, Selected = selectedVal == null},
                new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
                new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
            };
                ViewBag.Type = types; // db.tblStatusTypes.ToList();
                //ViewBag.Type = db.tblStatusTypes.ToList(); // db.tblStatusTypes.ToList();
                var currency= new SelectList(db.CurrencyMasters.OrderBy(x => x.CurrencyName), "CurrencyID", "CurrencyName").ToList();
                ViewBag.CurrencyID = db.CurrencyMasters.ToList();  // db.CurrencyMasters.ToList();
                ViewBag.Currencies = db.CurrencyMasters.ToList();
                ViewBag.AgentName = "ss"; // Emp.EmployeeName;
                ViewBag.CompanyName = company.AcCompany1;
                ViewBag.FwdAgentId = db.AgentMasters.Where(cc => cc.AgentType == 4).ToList();// .ForwardingAgentMasters.ToList();
                var agent = db.AgentMasters.OrderBy(x => x.Name).ToList(); // .ToList new SelectList(db.AgentMasters.OrderBy(x => x.Name), "AgentID", "Name").ToList();
                ViewBag.AgentList = agent; //  db.ForwardingAgentMasters.ToList();

                return View(_ExportShipment);
                        
        }
        public bool AddShippmentToTable(FormCollection data)
        {
            var shipmentmodel = new ExportShipmentDetail();
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
            shipmentmodel.FwdAgentId = Convert.ToInt32(data["tfwdagent"]);
            shipmentmodel.FwdAgentAWBNo = data["tfwdawb"];
            if (data["tfwddate"] == null || data["tfwddate"] == "")
            {

            }
            else
            {
                //shipmentmodel.FwdDate =DateTime.Parse(data["tfwddate"]);
            }
            shipmentmodel.FwdCharge = Convert.ToDecimal(data["tfwdcharge"]);
            shipmentmodel.FwdFlight =data["tfwdflight"];
            shipmentmodel.OtherCharge = Convert.ToDecimal(data["totherchrg"]);
            Session["EShipmentdetails"] = shipmentmodel;
            Session["EShipSerialNumber"] = Convert.ToInt32(data["tSerialNum"]);
            Session["EIsUpdate"] = Convert.ToBoolean(data["isupdate"]);
            return true;
        }
         
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddOrRemoveShipment1(ExportShipmentFormModel s_ImportShipment, int? i)
        {
            var Prevshipmentsession = Session["PreviousShipments"] as List<ExportShipmentDetail>;

            if (i.HasValue)
            {
                if (Prevshipmentsession == null)
                {

                }
                else
                {
                    if (s_ImportShipment.Shipments == null)
                    {
                        s_ImportShipment.Shipments = new List<ExportShipmentDetail>();
                    }
                    int index = 0;
                    foreach (var item in Prevshipmentsession)
                    {
                        if (i != index)
                        {
                            s_ImportShipment.Shipments.Add(item);
                        }
                        index++;
                    }
                }
                //s_ImportShipment.Shipments.RemoveAt(i.Value);
                Session["PreviousShipments"] = s_ImportShipment.Shipments;
            }
            else
            {
                if (s_ImportShipment.Shipments == null)
                {
                    s_ImportShipment.Shipments = new List<ExportShipmentDetail>();
                }
                var shipmentsession = Session["EShipmentdetails"] as ExportShipmentDetail;
                var Serialnumber = Convert.ToInt32(Session["EShipSerialNumber"]);
                var isupdate = Convert.ToBoolean(Session["EIsUpdate"]);
                if (Prevshipmentsession==null)
                {

                }
                else
                {
                    foreach(var item in Prevshipmentsession)
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
                Session["EShipmentdetails"] = new ExportShipmentDetail();
                Session["EShipSerialNumber"] = "";
                Session["EIsUpdate"] = false;
                Session["PreviousShipments"] = s_ImportShipment.Shipments;
            }
            ViewBag.Cities =db.CityMasters.ToList();
            ViewBag.FwdAgentId = db.ForwardingAgentMasters.ToList();
            ViewBag.Countries = db.CountryMasters.ToList();
            ViewBag.DestinationCountryID = db.CountryMasters.ToList();
            ViewBag.CurrencyID = db.CurrencyMasters.ToList();
            ViewBag.Currencies = db.CurrencyMasters.ToList();
            return PartialView("ExportShipmentList", s_ImportShipment);
        }

        public ActionResult Create(int ImportShipId,string SelectedImportdetail)
        {
            var userid = Convert.ToInt32(Session["UserID"]);
            var today = DateTime.Now.Date;

            //AuthHelp Token = repos.Authenticate();
            if (1==1)
            {
                var Emp = db.EmployeeMasters.Where(x => x.UserID==userid).Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.EmployeeName,x.EmployeeCode,x.EmployeeID,x.CountryName }).FirstOrDefault();

                var company = db.AcCompanies.FirstOrDefault(); // .Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.Phone, x.AcCompany,x.Country }).FirstOrDefault();
                //var countryname = db.CountryMasters.Where(d => d.CountryID == company.CountryID).Select(x => x.CountryName).FirstOrDefault();
                ExportShipmentFormModel s_ImportShipment = new ExportShipmentFormModel
                {
                    //ConsigneeAddress = agent.Address,
                    //OriginCityID = agent.CityID,
                    ManifestNumber = $"{DateTime.Now.ToString("yyyyMMdd")}{Emp.EmployeeCode}{(db.ExportShipments.Where(x => x.EmployeeID == Emp.EmployeeID && x.CreatedDate >= today).Count() + 1).ToString("D4")}",

                    ConsignorAddress = company.Address1 + ", " + company.CountryName + ", Tel: " + company.Phone,
                    FlightDate = DateTime.Now,
                    ConsignorCountryName=company.CountryName
                    //DestinationCountryID = Convert.ToInt32(company.CountryID),

                    //Shipments = new List<S_ImportShipmentDetails>()
                    //{
                    //    new S_ImportShipmentDetails
                    //    {
                    //    }
                    //}
                };
                if (s_ImportShipment.Shipments == null)
                {
                    s_ImportShipment.Shipments = new List<ExportShipmentDetail>();
                }
                List<int> importdetailids = SelectedImportdetail.Split(',').Select(Int32.Parse).ToList(); ;
                var shipmentdetails = db.ImportShipmentDetails.Where(d => d.ImportID == ImportShipId && importdetailids.Contains(d.ShipmentDetailID) && (d.CourierStatusID < 2 || d.CourierStatusID == null)).ToList();
                foreach (var item in shipmentdetails)
                {
                    var model = new ExportShipmentDetail();
                    model.AWB = item.AWB;
                    model.BagNo = item.BagNo;
                    model.Contents = item.Contents;
                    model.CurrencyID = item.CurrencyID;
                    model.DestinationCity = item.DestinationCity;
                    model.DestinationCountry = item.DestinationCountry;
                    model.HAWB = item.HAWB;
                    model.ImportDetailID = item.ShipmentDetailID;
                    model.Shipper = item.Shipper;
                    model.Value = item.Value;
                    model.Weight = item.Weight;
                    model.Reciver = item.Reciver;
                    model.ExportID = s_ImportShipment.ID;
                    model.Reciver = item.Reciver;
                    model.PCS = item.PCS;
                    model.DestinationCountry = item.DestinationCountry;
                    model.DestinationCity = item.DestinationCity;
                    s_ImportShipment.Shipments.Add(model);

                }
             
                ViewBag.ImportManifest = db.ImportShipments.Find(ImportShipId).ManifestNumber;
                var countries = db.CountryMasters.ToList();// db.CountryMasters.ToList();
                ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
                ViewBag.FwdAgentId = db.ForwardingAgentMasters.ToList();

                ViewBag.AgentID = db.ForwardingAgentMasters.ToList();// db.ForwardingAgentMasters.ToList();
                ViewBag.DestinationCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
                ViewBag.OriginCountryID = countries;
                ViewBag.DestinationCountryID = countries;
                ViewBag.Cities =db.CityMasters.ToList();
                ViewBag.Countries = db.CountryMasters.ToList();
                string selectedVal = null;
                var types= new List<SelectListItem>
            {
                new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
                new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
            };
                ViewBag.Type = types; // db.tblStatusTypes.ToList();
                ViewBag.CurrencyID = db.CurrencyMasters.ToList();// db.CurrencyMasters.ToList();
                ViewBag.Currencies = db.CurrencyMasters.ToList();
                ViewBag.AgentName = Emp.EmployeeName;
                ViewBag.CompanyName = company.AcCompany1;
                return View(s_ImportShipment);
            }
            ////return RedirectToAction(Token.Function, Token.Controller);
        }

        public JsonResult GetAgentBy_Id(int Id)
        {
            var agent = db.AgentMasters.Find(Id); // db.ForwardingAgentMasters.FirstOrDefault();
            var CountryId = agent.CountryName;
            var address = agent.Address1 + ", " + agent.Address2 + ", " +  ", Tel: " + agent.Phone;
            return Json(new { CountryName = CountryId, Cityname= agent.CityName, address = address }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrRemoveShipment(ExportShipmentFormModel s_Exportshipment, string DestinationId)
        {
            s_Exportshipment.Shipments = new List<ExportShipmentDetail>();

            var shipmentdetails = db.ImportShipmentDetails.Where(d => d.DestinationCity == DestinationId &&( d.CourierStatusID==15  || d.CourierStatusID==null)).ToList();
            foreach (var item in shipmentdetails)
            {
                var model = new ExportShipmentDetail();
                model.AWB = item.AWB;
                model.BagNo = item.BagNo;
                model.Contents = item.Contents;
                model.CurrencyID = item.CurrencyID;               
                model.DestinationCity = item.DestinationCity;
                model.DestinationCountry = item.DestinationCountry;
                model.HAWB = item.HAWB;
                model.ImportDetailID = item.ShipmentDetailID;
                model.Shipper = item.Shipper;
                model.Value = item.Value;
                model.Weight = item.Weight;
                model.Reciver = item.Reciver;
                model.ExportID = s_Exportshipment.ID;
                model.Reciver = item.Reciver;
                model.PCS = item.PCS;
             
                s_Exportshipment.Shipments.Add(model);

            }

            ViewBag.Cities = db.CityMasters.ToList();
            ViewBag.FwdAgentId = db.ForwardingAgentMasters.ToList();
            ViewBag.Countries = db.CountryMasters.ToList();
            ViewBag.DestinationCountryID = db.CountryMasters.ToList();
            ViewBag.CurrencyID = db.CurrencyMasters.ToList();
            ViewBag.Currencies = db.CurrencyMasters.ToList();


            return PartialView("ShipmentList", s_Exportshipment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExportShipmentFormModel model)
        {
            var userid = Convert.ToInt32(Session["UserID"]);
            var today = DateTime.Now.Date;
            //AuthHelp Token = repos.Authenticate();
            if (1==1)
            {
                var emp = db.EmployeeMasters.Where(x => x.UserID== userid).Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3,  x.CountryID, x.EmployeeID, x.EmployeeCode, x.EmployeeName }).FirstOrDefault();
                var company = db.AcCompanies.FirstOrDefault(); // db.S_AcCompany.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.Phone, x.AcCompany,x.Country }).FirstOrDefault();
                //var countryname = db.CountryMasters.Where(d => d.CountryID == company.CountryID).Select(x => x.CountryName).FirstOrDefault();
                var agent = db.AgentMasters.Where(d => d.AgentID == model.AgentID).FirstOrDefault();

                if (ModelState.IsValid)
                {

                    var exportShipment = new ExportShipment
                    {
                        AgentID = model.AgentID,
                        EmployeeID = 1, //1.Value,
                        Bags = model.Bags,
                        CD = model.CD,
                        ConsignorName =company.AcCompany1,
                        ConsignorAddress1_Building =company.Address1,
                        ConsignorAddress2_Street = company.Address2,
                        ConsignorAddress3_PinCode = company.Address3,                        
                        ConsignorCountryName=company.CountryName,
                        ConsignorCityName=company.CityName,
                        ConsigneeName = agent.Name,
                        ConsigneeAddress1_Building =agent.Address1,
                        ConsigneeAddress2_Street =agent.Address2,
                        ConsigneeAddress3_PinCode=agent.Address3,                        
                        FlightDate = model.FlightDate,
                        CreatedDate = DateTime.Now,
                        DestinationAirportCity =model.DestinationAirportCity,
                        OriginAirportCity =model.OriginAirportCity,
                        FlightNo = model.FlightNo,
                        LastEditedByLoginID = 1, //1.Value,
                        ManifestNumber = $"{DateTime.Now.ToString("yyyyMMdd")}{emp.EmployeeCode}{(db.ExportShipments.Where(x => x.EmployeeID == emp.EmployeeID && x.CreatedDate >= today).Count() + 1).ToString("D4")}",
                        ID = db.ExportShipments.Select(x => x.ID).DefaultIfEmpty(0).Max() + 1,
                        MAWB = model.MAWB,
                        RunNo = model.RunNo,
                        TotalAWB = model.TotalAWB,
                        Type = model.Type                        
                    };
                    db.ExportShipments.Add(exportShipment);
                    db.SaveChanges();
                    var max = db.ExportShipmentDetails.Select(x => x.ShipmentDetailID).DefaultIfEmpty(0).Max() + 1;
                    if (model.Shipments != null)
                    {
                        model.Shipments.ForEach(x =>
                        {
                            x.ShipmentDetailID = max;
                            x.ExportID = exportShipment.ID;
                            max++;
                        });
                        //db.ExportShipmentDetails.AddRange(model.Shipments);
                        db.SaveChanges();

                        var importdetailsid = model.Shipments.Select(s => s.ImportDetailID).ToList();
                        var importdetails = db.ImportShipmentDetails.Where(d => importdetailsid.Contains(d.ShipmentDetailID)).ToList();
                        importdetails.ForEach(x => { x.CourierStatusID = 2; x.ExportShipmentID = exportShipment.ID; });
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                model.ConsignorAddress = company.Address1 + ", " + company.CountryName + ", Tel: " + company.Phone;
                //model.OriginCountryID =0;
                model.ConsigneeAddress = agent.Address1 + ", " + agent.Address2 + ", " +  ", Tel: " + agent.Phone; //agent.Address3 +
                //model.DestinationCountryID = 0;

                if (model.Shipments == null)
                {
                    model.Shipments = new List<ExportShipmentDetail>();
                }
                var importdetailid = model.Shipments.Select(d => d.ImportDetailID).FirstOrDefault();
                var importid = db.ImportShipmentDetails.Find(importdetailid).ImportID;
                ViewBag.ImportManifest = db.ImportShipments.Find(importid).ManifestNumber;
                var countries = db.CountryMasters.ToList();
                ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City"); 
                ViewBag.AgentID = db.ForwardingAgentMasters.ToList();
                ViewBag.OriginCountryID = countries;
                ViewBag.DestinationCountryID = countries;
                ViewBag.CurrencyID = db.CurrencyMasters.ToList();
                ViewBag.Cities =db.CityMasters.ToList();
                ViewBag.Countries = db.CountryMasters.ToList();
                ViewBag.Type = db.tblStatusTypes.ToList();
                ViewBag.Currencies = db.CurrencyMasters.ToList();
                ViewBag.AgentName = emp.EmployeeName;
                ViewBag.CompanyName = company.AcCompany1;
                ViewBag.FwdAgentId = db.ForwardingAgentMasters.ToList(); // Where(d => d.IsForwardingAgent == true).ToList();

                return View(model);
            }
            //return RedirectToAction(Token.Function, Token.Controller);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateExport(ExportShipmentFormModel model)
        {
            var userid = Convert.ToInt32(Session["UserID"]);
            var today = DateTime.Now.Date;
            var emp = db.EmployeeMasters.Where(u=>u.UserID==userid).Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.EmployeeID, x.EmployeeCode, x.EmployeeName }).FirstOrDefault();
            var company = db.AcCompanies.FirstOrDefault(); // db.S_AcCompany.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID,x.Country, x.Phone, x.AcCompany }).FirstOrDefault();
            var agent = db.AgentMasters.Where(d => d.AgentID == model.AgentID).FirstOrDefault();
            var _exportShipment = new ExportShipment();
                if (ModelState.IsValid)
                {

                if (model.ID == 0) // new entry mode
                {

                    _exportShipment = new ExportShipment
                    {
                        AgentID = model.AgentID,
                        EmployeeID = emp.EmployeeID, //1.Value,
                        Bags = model.Bags,
                        CD = model.CD,
                        ConsignorName = company.AcCompany1,
                        ConsignorAddress1_Building = company.Address1,
                        ConsignorAddress2_Street = company.Address2,
                        ConsignorAddress3_PinCode = company.Address3,
                        ConsignorCountryName = company.CountryName,
                        ConsignorCityName = company.CityName,
                        ConsigneeName = agent.Name,
                        ConsigneeAddress1_Building = agent.Address1,
                        ConsigneeAddress2_Street = agent.Address2,
                        ConsigneeAddress3_PinCode = agent.Address3,
                        ConsigneeCityName = agent.CityName,
                        ConsigneeCountryName = agent.CountryName,
                        FlightDate = model.FlightDate,
                        CreatedDate = DateTime.Now,
                        DestinationAirportCity = model.DestinationAirportCity,
                        OriginAirportCity = model.OriginAirportCity,
                        FlightNo = model.FlightNo,
                        LastEditedByLoginID = emp.EmployeeID,// 1.Value,
                        ManifestNumber = $"{DateTime.Now.ToString("yyyyMMdd")}{emp.EmployeeCode}{(db.ExportShipments.Where(x => x.EmployeeID == emp.EmployeeID && x.CreatedDate >= today).Count() + 1).ToString("D4")}",
                        ID = db.ExportShipments.Select(x => x.ID).DefaultIfEmpty(0).Max() + 1,
                        MAWB = model.MAWB,
                        RunNo = model.RunNo,
                        TotalAWB = model.TotalAWB,
                        Type = model.Type
                    };
                }
                else
                {
                    _exportShipment = db.ExportShipments.Find(model.ID);
                    _exportShipment.CD = model.CD;
                    _exportShipment.Bags = model.Bags;
                    _exportShipment.MAWB = model.MAWB;
                    _exportShipment.RunNo = model.RunNo;
                    _exportShipment.TotalAWB = model.TotalAWB;
                    _exportShipment.Type = model.Type;
                    _exportShipment.DestinationAirportCity = model.DestinationAirportCity;
                    _exportShipment.OriginAirportCity = model.OriginAirportCity;
                    _exportShipment.FlightDate = model.FlightDate;
                    _exportShipment.FlightNo = model.FlightNo;

                }

                if (model.ID==0)
                {
                    db.ExportShipments.Add(_exportShipment);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(_exportShipment).State = EntityState.Modified;
                    db.SaveChanges();
                }

                    
                    var max = db.ExportShipmentDetails.Select(x => x.ShipmentDetailID).DefaultIfEmpty(0).Max() + 1;
                if (model.Shipments != null)
                {
                    //model.Shipments.ForEach(x =>
                    //{
                    //    x.ShipmentDetailID = max;
                    //    x.ExportID = _exportShipment.ID;
                    //    max++;
                    //});
                    foreach (var e_details in model.Shipments)
                    {
                        if (e_details.ShipmentDetailID == 0)
                        {
                            e_details.ShipmentDetailID = max;
                            e_details.ExportID = _exportShipment.ID;
                            db.ExportShipmentDetails.Add(e_details);
                            db.SaveChanges();
                            max++;
                        }
                        else
                        {
                            e_details.ExportID = _exportShipment.ID;
                            db.Entry(e_details).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                    //removed items
                    var exportdetails = db.ExportShipmentDetails.Where(d => d.ExportID == _exportShipment.ID).ToList();
                    var exportdetailsid = model.Shipments.Select(s => s.ImportDetailID).ToList();
                    foreach (var e_details in exportdetails)
                    {
                        var _exportfound = model.Shipments.Where(cc => cc.ShipmentDetailID == e_details.ShipmentDetailID).FirstOrDefault();
                        if (_exportfound==null)
                        {
                            db.Entry(e_details).State = EntityState.Deleted;
                            db.SaveChanges();
                        }
                    }

                    
                    //var importdetails = db.ImportShipmentDetails.Where(d => importdetailsid.Contains(d.ShipmentDetailID)).ToList();
                    //if (importdetails.Count > 0) {
                    //    importdetails.ForEach(x => { x.Status = 2; x.ExportShipmentID = _exportShipment.ID; });
                    //    db.SaveChanges();
                    //}
                }
                    return RedirectToAction("Index");
                }

                model.ConsignorAddress = company.Address1 + ", " + company.CountryName + ", Tel: " + company.Phone;
                model.ConsigneeAddress = agent.Address1 + ", " + agent.Address2 + ", " +  ", Tel: " + agent.Phone; //agent.Address3 +

                if (model.Shipments == null)
                {
                    model.Shipments = new List<ExportShipmentDetail>();
                }

                
                ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
                var agents = db.AgentMasters.ToList(); // new SelectList(db.ForwardingAgentMasters.OrderBy(x => x.FAgentName), "FAgentID", "FAgentName").ToList();
                ViewBag.AgentList = agents;                                                
                ViewBag.CurrencyID = db.CurrencyMasters.ToList();                 
                string selectedVal = model.Type;
                var types = new List<SelectListItem>
            {
                new SelectListItem{Text = "Select Shipment Type", Value = null, Selected = selectedVal == null},
                new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
                new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
            };
                ViewBag.Type = types;
                ViewBag.Currencies = db.CurrencyMasters.ToList();
                ViewBag.AgentName = emp.EmployeeName;
                ViewBag.CompanyName = company.AcCompany1;
               ViewBag.FwdAgentId = db.AgentMasters.Where(cc => cc.AgentType == 4).ToList(); //. ForwardingAgentMasters.ToList(); // .Where(d => d.IsForwardingAgent == true).ToList();

                return View(model);
            
            //return RedirectToAction(Token.Function, Token.Controller);
        }
        //public ActionResult EditExport(int? id)
        //{
        //    //AuthHelp Token = repos.Authenticate();
        //    if (1==1)
        //    {
        //        var data = db.ExportShipments.Where(x => x.ID == id).Select(x => new ExportShipmentFormModel
        //        {
        //            ID = x.ID,
        //            AgentID = x.AgentID,
        //            EmployeeID = x.EmployeeID,                    
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
        //            OriginAirportCityID = x.OriginAirportCityID,
        //            OriginCountryID = x.OriginCountryID,
        //            RunNo = x.RunNo,
        //            //Shipments = x.ExportShipmentDetails.ToList(),
        //            TotalAWB = x.TotalAWB,
        //            Type = x.Type,
        //            OriginCity=x.OriginCity,
        //            DestinationCity=x.DestinationCity,
        //            OrginCountry=x.OrginCountry,
        //            DestinationCountry=x.DestinationCountry
        //        }).FirstOrDefault();
        //        data.Shipments = db.ExportShipmentDetails.Where(d => d.ExportID == id).ToList();
        //        var countries = db.CountryMasters.ToList();
        //        ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
        //        var agent = new SelectList(db.ForwardingAgentMasters.OrderBy(x => x.FAgentName), "FAgentID", "FAgentName").ToList();

        //        ViewBag.AgentID = agent;
        //        ViewBag.OriginCountryID = countries;
        //        ViewBag.DestinationCountryID = countries;
        //        ViewBag.CurrencyID = db.CurrencyMasters.ToList();
        //        ViewBag.Cities = db.CityMasters.ToList();
        //        ViewBag.Countries = db.CountryMasters.ToList();
        //        string selectedVal = null;
        //        var types = new List<SelectListItem>
        //    {
        //        new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
        //        new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
        //    };
        //        ViewBag.Type = types;
        //        ViewBag.Currencies = db.CurrencyMasters.ToList();
        //        ViewBag.FwdAgentId = db.ForwardingAgentMasters.ToList(); // .Where(d => d.IsForwardingAgent == true).ToList();
               
        //        var emp = db.EmployeeMasters.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.EmployeeID, x.EmployeeCode, x.EmployeeName }).FirstOrDefault();
        //        var company = db.AcCompanies.FirstOrDefault(); // .Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.Phone, x.AcCompany }).FirstOrDefault();

        //        ViewBag.AgentName = emp.EmployeeName;
        //        ViewBag.CompanyName = company.AcCompany1;

        //        return View(data);
        //    }
        //    //return RedirectToAction(Token.Function, Token.Controller);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditExport(ExportShipmentFormModel model)
        //{
        //    //var company = db.S_AcCompany.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.AcCompany,x.Country }).FirstOrDefault();
        //    var company = db.AcCompanies.FirstOrDefault(); // db.S_AcCompany.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID,x.Country, x.Phone, x.AcCompany }).FirstOrDefault();
        //    var emp = db.EmployeeMasters.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.EmployeeID, x.EmployeeCode, x.EmployeeName }).FirstOrDefault();

        //    var today = DateTime.Now.Date;
        //    //AuthHelp Token = repos.Authenticate();
        //    if (1==1)
        //    {
        //        var exportshipment = db.ExportShipments.Find(model.ID);
        //        if (ModelState.IsValid)
        //        {
        //            exportshipment.AgentID = model.AgentID;
        //            exportshipment.EmployeeID = emp.EmployeeID; // 1.Value;
        //            exportshipment.AirportOfShipmentID = model.AirportOfShipmentID;
        //            exportshipment.OriginCountryID =0;
        //            exportshipment.OriginAirportCityID = Convert.ToInt32(model.AirportOfShipmentID);
        //            exportshipment.DestinationAirportOfShipmentId = model.DestinationAirportOfShipmentId;
        //            exportshipment.Bags = model.Bags;
        //            exportshipment.CD = model.CD;
        //            exportshipment.Date = model.Date;
        //            exportshipment.DestinationCityID = Convert.ToInt32(model.DestinationAirportOfShipmentId);
        //            exportshipment.FlightNo = model.FlightNo;
        //            exportshipment.LastEditedByLoginID = emp.EmployeeID;  // 1.Value;
        //            exportshipment.MAWB = model.MAWB;
        //            exportshipment.RunNo = model.RunNo;
        //            exportshipment.TotalAWB = model.TotalAWB;
        //            exportshipment.Type = model.Type;
        //            exportshipment.DestinationCity = model.DestinationCity;
        //            exportshipment.OriginCity = model.OriginCity;
        //            exportshipment.OrginCountry = company.CountryName;
        //            exportshipment.DestinationCountry = model.DestinationCountry;
        //            db.Entry(exportshipment).State = EntityState.Modified;
        //            db.SaveChanges();
        //            var max = db.ExportShipmentDetails.Select(x => x.ShipmentDetailID).DefaultIfEmpty(0).Max() + 1;
        //            if (model.Shipments != null)
        //            {
        //                model.Shipments.ForEach(x =>
        //                {
        //                    x.ExportID = exportshipment.ID;
        //                    if (x.ShipmentDetailID == 0)
        //                    {
        //                        x.ShipmentDetailID = max;

        //                        max++;
        //                        db.ExportShipmentDetails.Add(x);
        //                    }
        //                    else
        //                    {
        //                        db.Entry(x).State = EntityState.Modified;
        //                    }
        //                    db.SaveChanges();
        //                });
        //                var li = model.Shipments.Select(x => x.ShipmentDetailID).ToList();
        //                var exportdetails = db.ExportShipmentDetails.Where(x => !li.Contains(x.ShipmentDetailID) && x.ExportID == exportshipment.ID).ToList();
        //                foreach(var rexport in exportdetails)
        //                {
        //                    db.ExportShipmentDetails.Remove(rexport);
        //                    db.SaveChanges();
        //                }
                            
        //                //db.ExportShipmentDetails.RemoveRange(db.ExportShipmentDetails.Where(x => !li.Contains(x.ShipmentDetailID) && x.ExportID == exportshipment.ID));
        //                //db.SaveChanges();
        //            }
        //            else
        //            {
        //                var exportdetails = db.ExportShipmentDetails.Where(s => s.ExportID == exportshipment.ID).ToList();
        //                foreach (var rexport in exportdetails)
        //                {
        //                    db.ExportShipmentDetails.Remove(rexport);
        //                    db.SaveChanges();
        //                }
        //                //db.ExportShipmentDetails.RemoveRange(db.ExportShipmentDetails.Where(s => s.ExportID == exportshipment.ID));
        //                //db.SaveChanges();
        //            }


        //            return RedirectToAction("Index");
        //        }
        //        model.ConsignorAddress = exportshipment.ConsignorAddress;
        //        model.OriginAirportCityID = exportshipment.OriginAirportCityID;
        //        model.OriginCountryID = exportshipment.OriginCountryID;
        //        model.ConsigneeAddress = exportshipment.ConsigneeAddress;
        //        model.DestinationCountryID = exportshipment.DestinationCountryID;
        //        if (model.Shipments == null)
        //        {
        //            model.Shipments = new List<ExportShipmentDetail>();
        //        }
            
        //        var countries = db.CountryMasters.ToList();
        //        ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
        //        ViewBag.AgentID = db.ForwardingAgentMasters.ToList();
        //        ViewBag.OriginCountryID = countries;
        //        ViewBag.DestinationCountryID = countries;
        //        ViewBag.CurrencyID = db.CurrencyMasters.ToList();
        //        ViewBag.Cities =db.CityMasters.ToList();
        //        ViewBag.Countries = db.CountryMasters.ToList();
        //        ViewBag.Currencies = db.CurrencyMasters.ToList();
        //        ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
        //        var agent = new SelectList(db.ForwardingAgentMasters.OrderBy(x => x.FAgentName), "FAgentID", "FAgentName").ToList();

        //        ViewBag.AgentID = agent;
        //        ViewBag.OriginCountryID = countries;
        //        ViewBag.DestinationCountryID = countries;
        //        ViewBag.CurrencyID = db.CurrencyMasters.ToList();
        //        ViewBag.Cities = db.CityMasters.ToList();
        //        ViewBag.Countries = db.CountryMasters.ToList();
        //        string selectedVal = null;
        //        var types = new List<SelectListItem>
        //    {
        //        new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
        //        new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
        //    };
        //        ViewBag.Type = types;
        //        var agentname = db.ForwardingAgentMasters.Where(x => x.FAgentID == model.AgentID).FirstOrDefault();
        //        ViewBag.AgentName = agentname.FAgentName;
        //        ViewBag.CompanyName = company.AcCompany1;
        //        return View(model);
        //    }
        //    //return RedirectToAction(Token.Function, Token.Controller);
        //}

           
        public JsonResult GetShipmentDetails(ExportShipmentFormModel s_ImportShipment, int? i)
        {
            if (i.HasValue)
            {
                var Prevshipmentsession = Session["PreviousShipments"] as List<ExportShipmentDetail>;
                if (Prevshipmentsession == null)
                {

                }
                else
                {
                    if (s_ImportShipment.Shipments == null)
                    {
                        s_ImportShipment.Shipments = new List<ExportShipmentDetail>();
                    }
                    foreach (var item in Prevshipmentsession)
                    {
                        s_ImportShipment.Shipments.Add(item);
                    }
                }
                var s = s_ImportShipment.Shipments[i.Value];
                return Json(new { success = true, data = s, ival = i.Value }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }
        //public ActionResult Edit(int? id)
        //{
        //    //AuthHelp Token = repos.Authenticate();
        //    if (1==1)
        //    {
        //        var data = db.ExportShipments.Where(x => x.ID == id).Select(x => new ExportShipmentFormModel
        //        {
        //            ID = x.ID,
        //            AgentID = x.AgentID,
        //            EmployeeID = x.EmployeeID,
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
        //            OriginAirportCityID = x.OriginAirportCityID,
        //            OriginCountryID = x.OriginCountryID,
        //            RunNo = x.RunNo,
        //            Shipments = x.ExportShipmentDetails.ToList(),
        //            TotalAWB = x.TotalAWB,
        //            Type = x.Type,
        //            OriginCity=x.OriginCity,
        //            DestinationCity=x.DestinationCity,
        //            OrginCountry=x.OrginCountry,
        //            DestinationCountry=x.DestinationCountry,
        //        }).FirstOrDefault();
        //        var countries = db.CountryMasters.ToList();
        //        ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
        //        ViewBag.AgentID = db.ForwardingAgentMasters.ToList();
        //        ViewBag.OriginCountryID = countries;
        //        ViewBag.DestinationCountryID = countries;
        //        ViewBag.CurrencyID = db.CurrencyMasters.ToList();
        //        ViewBag.Cities =db.CityMasters.ToList();
        //        ViewBag.Countries = db.CountryMasters.ToList();
        //        ViewBag.Type = db.tblStatusTypes.FirstOrDefault(); // repos.GetShipmentType(data.Type);
        //        ViewBag.Currencies = db.CurrencyMasters.ToList();
        //        ViewBag.FwdAgentId = db.ForwardingAgentMasters.ToList();  //Where(d=>d.IsForwardingAgent==true)
        //        var importdetailid = data.Shipments.Select(d => d.ImportDetailID).FirstOrDefault();
        //        var importid = db.ImportShipmentDetails.Find(importdetailid).ImportID;
        //        ViewBag.ImportManifest = db.ImportShipments.Find(importid).ManifestNumber;

        //        var emp = db.EmployeeMasters.Where(x => x.EmployeeID == 1).Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.EmployeeID, x.EmployeeCode, x.EmployeeName }).FirstOrDefault();
        //        var company = db.AcCompanies.FirstOrDefault();// db.S_AcCompany.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.Phone, x.AcCompany }).FirstOrDefault();
              
        //        ViewBag.AgentName = emp.EmployeeName;
        //        ViewBag.CompanyName = company.AcCompany1;

        //        return View(data);
        //    }
        //    //return RedirectToAction(Token.Function, Token.Controller);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(ExportShipmentFormModel model)
        //{
        //    //var company = db.S_AcCompany.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.AcCompany }).FirstOrDefault();
        //    var company = db.AcCompanies.FirstOrDefault();
        //    var today = DateTime.Now.Date;
        //  //  AuthHelp Token = repos.Authenticate();
        //    if (1==1)  //1==1
        //    {
        //        var exportshipment = db.ExportShipments.Find(model.ID);
        //        if (ModelState.IsValid)
        //        {
        //            exportshipment.AgentID = model.AgentID;
        //            exportshipment.EmployeeID = 1; // 1.Value;
        //            exportshipment.AirportOfShipmentID = model.AirportOfShipmentID;
        //            exportshipment.OriginCountryID = 0;
        //            exportshipment.OriginAirportCityID =Convert.ToInt32(model.AirportOfShipmentID);
        //            exportshipment.DestinationAirportOfShipmentId = model.DestinationAirportOfShipmentId;
        //            exportshipment.Bags = model.Bags;
        //            exportshipment.CD = model.CD;
        //            exportshipment.Date = model.Date;
        //            exportshipment.DestinationCityID = Convert.ToInt32(model.DestinationAirportOfShipmentId);
        //            exportshipment.FlightNo = model.FlightNo;
        //            exportshipment.LastEditedByLoginID = 1;// 1.Value;
        //            exportshipment.MAWB = model.MAWB;
        //            exportshipment.RunNo = model.RunNo;
        //            exportshipment.TotalAWB = model.TotalAWB;
        //            exportshipment.Type = model.Type;
        //            exportshipment.OriginCity = model.OriginCity;
        //            exportshipment.DestinationCity = model.DestinationCity;
        //            db.Entry(exportshipment).State = EntityState.Modified;
        //            db.SaveChanges();
        //            var max = db.ExportShipmentDetails.Select(x => x.ShipmentDetailID).DefaultIfEmpty(0).Max() + 1;
        //            if (model.Shipments != null)
        //            {
        //                model.Shipments.ForEach(x =>
        //                {
        //                    x.ExportID = exportshipment.ID;
        //                    if (x.ShipmentDetailID == 0)
        //                    {
        //                        x.ShipmentDetailID = max;

        //                        max++;
        //                        db.ExportShipmentDetails.Add(x);
        //                    }
        //                    else
        //                    {
        //                        db.Entry(x).State = EntityState.Modified;
        //                    }
        //                });
        //                var li = model.Shipments.Select(x => x.ShipmentDetailID).ToList();
        //                //db.ExportShipmentDetails.RemoveRange(db.ExportShipmentDetails.Where(x => !li.Contains(x.ShipmentDetailID) && x.ExportID == exportshipment.ID));
        //                //db.SaveChanges();
        //            }
        //            else
        //            {

        //                //db.S_ExportShipmentDetails.RemoveRange(db.S_ExportShipmentDetails.Where(s => s.ExportID == exportshipment.ID));
        //                //db.SaveChanges();
        //            }


        //            return RedirectToAction("Index");
        //        }
        //        model.ConsignorAddress = exportshipment.ConsignorAddress;
        //        model.OriginAirportCityID = exportshipment.OriginAirportCityID;
        //        model.OriginCountryID = exportshipment.OriginCountryID;
        //        model.ConsigneeAddress = exportshipment.ConsigneeAddress;
        //        model.DestinationCountryID = exportshipment.DestinationCountryID;
        //        if (model.Shipments == null)
        //        {
        //            model.Shipments = new List<ExportShipmentDetail>();
        //        }
        //        var importdetailid = model.Shipments.Select(d => d.ImportDetailID).FirstOrDefault();
        //        var importid = db.ImportShipmentDetails.Find(importdetailid).ImportID;
        //        ViewBag.ImportManifest = db.ImportShipments.Find(importid).ManifestNumber;

        //        var countries = db.CountryMasters.ToList();
        //        ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
        //        ViewBag.AgentID = db.ForwardingAgentMasters.ToList();
        //        ViewBag.OriginCountryID = countries;
        //        ViewBag.DestinationCountryID = countries;
        //        ViewBag.CurrencyID = db.CurrencyMasters.ToList();
        //        ViewBag.Cities =db.CityMasters.ToList();
        //        ViewBag.Countries = db.CountryMasters.ToList();
        //        ViewBag.Currencies = db.CurrencyMasters.ToList();
        //        ViewBag.Type = db.tblStatusTypes.FirstOrDefault(); // repos.GetShipmentType(model.Type);
        //        var agent = db.ForwardingAgentMasters.ToList(); // .Where(x => x.ID == model.AgentID).FirstOrDefault();
        //        ViewBag.AgentName = "agent"; // agent.AgentName;
        //        ViewBag.CompanyName = company.AcCompany1;
        //        return View(model);
        //    }
        //    //return RedirectToAction(Token.Function, Token.Controller);
        //}
        public ActionResult Details(int? id)
        {
           // AuthHelp Token = repos.Authenticate();
            if (1==1)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //var s_ImportShipment = db.ExportShipments.Where(x => x.ID == id).Include(s => s.EmployeeMaster).Include(s => s.EmployeeMaster1)
                //    .Include(s => ).Include(s => s.S_ExportShipmentDetails)
                //    .FirstOrDefault();
                //if (s_ImportShipment == null)
                //{
                //    return HttpNotFound();
                //}
                ViewBag.Edit = true; // Token.Permissions.Updation;
                ViewBag.agents = db.ForwardingAgentMasters.ToList();
                return View();
            }
            ////return RedirectToAction(Token.Function, Token.Controller);
        }
        public ActionResult ExportShipmentReport(int id)
        {
            //AuthHelp Token = repos.Authenticate();
            if (1==1)
            {
                ViewBag.Id = id;
                return View();
            }
            ////return RedirectToAction(Token.Function, Token.Controller);
        }
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
           // AuthHelp Token = repos.Authenticate();
            if (1==1)
            {
               
                try
                {
                    //var exportsid = db.S_ImportShipmentDetails.Where(d => d.ExportShipmentID == id).ToList();
                    //exportsid.ForEach(s => { s.ExportShipmentID = null; s.Status = 3; });//status=3 export deleted
                    //S_ExportShipment s_ExportShipment = db.S_ExportShipment.Find(id);
                    //List<S_ExportShipmentDetails> Details = db.S_ExportShipmentDetails.Where(d => d.ExportID == id).ToList();
                    //db.S_ExportShipmentDetails.RemoveRange(Details);
                    //db.S_ExportShipment.Remove(s_ExportShipment);
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch(Exception e)
                {
                    return RedirectToAction("Index");
                }
            }
            //return RedirectToAction(Token.Function, Token.Controller);
        }
        public ActionResult ViewImport()
        {
           // AuthHelp Token = repos.Authenticate();
            if (1==1)
            {
                DatePicker model = new DatePicker
                {
                    FromDate = DateTime.Now.Date,
                    Delete = true,// (bool)Token.Permissions.Deletion,
                    Update = true, //(bool)Token.Permissions.Updation,
                    Create = true //(bool)Token.Permissions.Creation
                };
                ImportShipmentFormModel model1 = new ImportShipmentFormModel
                {
                    FlightDate =DateTime.Now,
                    CreatedDate = DateTime.Now,
                };
                ViewBag.Agents = db.ForwardingAgentMasters.ToList(); // .Where(d => d.IsForwardingAgent == false).ToList(); ;
                ViewBag.Countries = db.CountryMasters.ToList();
                ViewBag.Cities =db.CityMasters.ToList();
                ViewBag.Token = model;
                Session["Filters"] = model1;
                SessionDataModel.SetTableVariable(model);
                return View(model1);
            }
            ////return RedirectToAction(Token.Function, Token.Controller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewImport( ImportShipmentFormModel picker)
        {
         //   AuthHelp Token = repos.Authenticate();
            if (1==1)
            {
                DatePicker model = new DatePicker
                {
                    FromDate = picker.FlightDate.Value,
                    Delete = true,
                    Update = true,
                    Create = true
                };
                ImportShipmentFormModel model1 = new ImportShipmentFormModel
                {
                    FlightDate = picker.FlightDate,
                    ManifestNumber = picker.ManifestNumber,
                    AgentID = picker.AgentID,
                    FlightNo = picker.FlightNo,
                    MAWB = picker.MAWB,
                    ConsignorCityName = picker.ConsignorCityName,
                    ConsignorCountryName = picker.ConsignorCountryName,
                    DestinationAirportCity = picker.DestinationAirportCity,
                    ConsigneeCountryName = picker.ConsigneeCountryName,
                    CreatedDate=picker.CreatedDate,
                };
                ViewBag.Token = model;
                Session["Filters"] = model1;
                ViewBag.Agents = db.ForwardingAgentMasters.ToList();// .Where(d => d.IsForwardingAgent == false).ToList(); ;
                ViewBag.Countries = db.CountryMasters.ToList();
                ViewBag.Cities =db.CityMasters.ToList();
              
                SessionDataModel.SetTableVariable(model);
                return View(model1);
            }
            //return RedirectToAction(Token.Function, Token.Controller);
        }

        public ActionResult ImportTable()
        {
           // AuthHelp Token = repos.Authenticate();
            if (1==1)
            {
                ImportShipmentFormModel datePicker = Session["Filters"] as ImportShipmentFormModel;

                DatePicker model = new DatePicker
                {
                    FromDate = datePicker.FlightDate.Value.Date,
                    Delete = true,// (bool)Token.Permissions.Deletion,
                    Update = true, //(bool)Token.Permissions.Updation,
                    Create = true, //(bool)Token.Permissions.Creation
                };
                ViewBag.Token = model;
                var s_ImportShipment = new List<ImportShipment>();
                var strposteddate =datePicker.FlightDate.Value.ToShortDateString();
                var strposteddate1 =datePicker.CreatedDate.ToShortDateString();
                if (strposteddate == "01-01-0001")
                {
                    strposteddate = null;
                }
                if (strposteddate1 == "01-01-0001")
                {
                    strposteddate1 = null;
                }

                s_ImportShipment = db.ImportShipments.Include(s => s.UserRegistration.UserID).Include(s => s.UserRegistration.UserID).Include(s => s.UserRegistration).ToList();


                if (datePicker.FlightDate!=null && strposteddate !=null)
                {
                    s_ImportShipment = s_ImportShipment.Where(x =>x.FlightDate == datePicker.FlightDate).ToList();

                }
                if (datePicker.CreatedDate != null && strposteddate1 != null)
                {
                    s_ImportShipment = s_ImportShipment.Where(x => x.CreatedDate.Date == datePicker.CreatedDate.Date).ToList();

                }

                if (datePicker.ManifestNumber!=null)
                {
                    s_ImportShipment = s_ImportShipment.Where(d => datePicker.ManifestNumber.Contains(d.ManifestNumber)).ToList();
                }
                if(datePicker.AgentID!=null && datePicker.AgentID>0)
                {
                    s_ImportShipment = s_ImportShipment.Where(d => d.AgentID==datePicker.AgentID).ToList();

                }
                if (datePicker.MAWB != null )
                {
                    s_ImportShipment = s_ImportShipment.Where(d => d.MAWB.Contains(datePicker.MAWB)).ToList();

                }
                if (datePicker.FlightNo != null)
                {
                    s_ImportShipment = s_ImportShipment.Where(d => d.FlightNo == datePicker.FlightNo).ToList();

                }
                if (datePicker.ConsignorCountryName != null )
                {
                    s_ImportShipment = s_ImportShipment.Where(d => datePicker.ConsignorCountryName.Contains(d.ConsignorCountryName)).ToList();

                }
                if (datePicker.ConsignorCountryName != null )
                {
                    s_ImportShipment = s_ImportShipment.Where(d => datePicker.ConsignorCityName.Contains(d.ConsignorCityName)).ToList();

                }
                if (datePicker.ConsigneeCountryName != null && datePicker.ConsigneeCountryName !="")
                {
                    s_ImportShipment = s_ImportShipment.Where(d => datePicker.ConsigneeCountryName.Contains(d.ConsigneeCountryName)).ToList();

                }
                if (datePicker.DestinationAirportCity != null && datePicker.DestinationAirportCity != "")
                {
                    s_ImportShipment = s_ImportShipment.Where(d => datePicker.ConsigneeCityName.Contains(d.ConsigneeCityName)).ToList();

                }
                s_ImportShipment = s_ImportShipment.OrderByDescending(d => d.CreatedDate).ToList();
                s_ImportShipment.ForEach(d => { d.ConsignorCountryName = d.ConsignorCountryName; d.ConsigneeCountryName = d.ConsigneeCountryName; });
                return PartialView("ImportTable", s_ImportShipment.ToList());
            }
            //return RedirectToAction(Token.Function, Token.Controller);
        }
        public ActionResult EditImport(int ImportId)
        {
            //AuthHelp Token = repos.Authenticate();
            if (1==1)
            {
                var data = db.ImportShipments.Where(x => x.ID == ImportId).Select(x => new ImportShipmentFormModel
                {
                    ID = x.ID,
                    AgentID = x.AgentID,
                    AgentLoginID = x.AgentLoginID,
                    //AirportOfShipmentID = x.AirportOfShipmentID,
                    Bags = x.Bags,
                    CD = x.CD,                    
                    CreatedDate = x.CreatedDate,
                    FlightDate = x.FlightDate,
                    ConsigneeCityName=x.ConsigneeCityName,
                    ConsigneeCountryName=x.ConsigneeCountryName,                    
                    FlightNo = x.FlightNo,
                    LastEditedByLoginID = x.LastEditedByLoginID,
                    ManifestNumber = x.ManifestNumber,
                    MAWB = x.MAWB,
                    OriginAirportCity = x.OriginAirportCity,
                    DestinationAirportCity=x.DestinationAirportCity,                    
                    RunNo = x.RunNo,
                    Shipments = x.ImportShipmentDetails.Where(d=>d.CourierStatusID ==15).ToList(),
                    TotalAWB = x.TotalAWB,
                    Type = x.Type,
                    ConsignorCountryName=x.ConsignorCountryName
                }).FirstOrDefault();
                var countries = db.CountryMasters.ToList();
                ViewBag.DestinationCountryID = countries;
                var agent = db.ForwardingAgentMasters.Where(d=>d.FAgentID==data.AgentID).FirstOrDefault();
                ViewBag.AgentName = agent.FAgentName;
                return View(data);
            }
            //return RedirectToAction(Token.Function, Token.Controller);
        }
    }

    
}