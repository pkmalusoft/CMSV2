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
    public class ExportShipmentController : Controller
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
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //DateTime.Now.Date,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date.AddDays(1),// DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59)
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
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());

            DatePicker datePicker = SessionDataModel.GetTableVariable();
                ViewBag.Token = datePicker;               

            List<ExportShipmentFormModel> _ExportShipment = (from c in db.ExportShipments
                                                 join employee in db.EmployeeMasters on c.EmployeeID equals employee.EmployeeID
                                                 join agent in db.AgentMasters on c.AgentID equals agent.AgentID
                                                 join shipment in db.tblShipmentTypes on c.ShipmentTypeId equals shipment.ID
                                                 where (c.CreatedDate >= datePicker.FromDate && c.CreatedDate < datePicker.ToDate)
                                                 orderby c.CreatedDate descending
                                                 select new ExportShipmentFormModel
                                                 {ID=c.ID, ManifestNumber = c.ManifestNumber, FlightDate = c.FlightDate, FlightNo = c.FlightNo, MAWB = c.MAWB, CD = c.CD, Bags = c.Bags, RunNo = c.RunNo, Type = shipment.ShipmentType, TotalAWB = c.TotalAWB, CreatedByName = employee.EmployeeName, AgentName = agent.Name, CreatedDate=c.CreatedDate
                                                     
                                                 }).ToList();    
                                             
           return PartialView("Table", _ExportShipment);        
            
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
                ViewBag.Title = "Export Shipment -Create";
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
                ViewBag.Title = "Export Shipment - Modify";
                var exportshipment = db.ExportShipments.Find(id);
                _ExportShipment.ID = exportshipment.ID;
                _ExportShipment.ManifestNumber = exportshipment.ManifestNumber;                
                _ExportShipment.ShipmentTypeId = exportshipment.ShipmentTypeId;
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
                _ExportShipment.ShipmentTypeId = exportshipment.ShipmentTypeId;
                _ExportShipment.Shipments = db.ExportShipmentDetails.Where(cc => cc.ExportID == _ExportShipment.ID).ToList();
                List<CMSV2.Models.ExportShipmentDetail> _details = new List<ExportShipmentDetail>(); // _ExportShipment.Shipments;
                foreach (var item in _ExportShipment.Shipments)
                {
                    ExportShipmentDetail ee = new ExportShipmentDetail();
                    ee.ShipmentDetailID = item.ShipmentDetailID;
                    ee.ExportID = item.ExportID;
                    ee.AWB = item.AWB;
                    ee.BagNo = item.BagNo;
                    ee.Contents = item.Contents;
                    ee.CurrencyID = item.CurrencyID;
                    ee.DestinationCity = item.DestinationCity;
                    ee.DestinationCountry = item.DestinationCountry;
                    ee.HAWB = item.HAWB;
                    ee.ImportDetailID = item.ShipmentDetailID;
                    ee.Shipper = item.Shipper;
                    ee.Value = item.Value;
                    ee.Weight = item.Weight;
                    ee.Reciver = item.Reciver;                                        
                    ee.PCS = item.PCS;
                    ee.FwdDate = item.FwdDate;
                    ee.FwdFlight = item.FwdFlight;
                    ee.FwdAgentId = item.FwdAgentId;
                    ee.FwdCharge = item.FwdCharge;
                    ee.FwdAgentAWBNo = item.FwdAgentAWBNo;
                    ee.OtherCharge = item.OtherCharge;
                    ee.InscanId = item.InscanId;
                    
                    _details.Add(ee);
                }
                
                Session["PreviousShipments"] = _details;
            }                               
                
                string selectedVal = _ExportShipment.Type;

            //    var types = new List<SelectListItem>
            //{
            //    new SelectListItem{Text = "Select Shipment Type", Value = null, Selected = selectedVal == null},
            //    new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
            //    new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
            //};
            //    ViewBag.Type = types; // db.tblStatusTypes.ToList();

            ViewBag.Type = db.tblShipmentTypes.ToList();
            //ViewBag.Type = db.tblStatusTypes.ToList(); // db.tblStatusTypes.ToList();
            var currency= new SelectList(db.CurrencyMasters.OrderBy(x => x.CurrencyName), "CurrencyID", "CurrencyName").ToList();
                ViewBag.CurrencyID = db.CurrencyMasters.ToList();  // db.CurrencyMasters.ToList();
                ViewBag.Currencies = db.CurrencyMasters.ToList();
                ViewBag.AgentName = "ss"; // Emp.EmployeeName;
                ViewBag.CompanyName = company.AcCompany1;
                ViewBag.FwdAgentId = db.AgentMasters.Where(cc => cc.AgentType == 4).ToList();// .ForwardingAgentMasters.ToList();
                var agent = db.AgentMasters.OrderBy(x => x.Name).ToList(); // .ToList new SelectList(db.AgentMasters.OrderBy(x => x.Name), "AgentID", "Name").ToList();
                ViewBag.AgentList = agent; //  db.ForwardingAgentMasters.ToList();
            ViewBag.FwdAgentId = db.AgentMasters.Where(cc => cc.AgentType == 4).ToList(); //. ForwardingAgentMasters.ToList(); // .Where(d => d.IsForwardingAgent == true).ToList();
            return View(_ExportShipment);
                        
        }
        public bool AddShippmentToTable(FormCollection data)
        {
            var shipmentmodel = new ExportShipmentDetail();
            shipmentmodel.CurrencyID =Convert.ToInt32(Session["CurrencyId"].ToString()); // Convert.ToInt32(data["tCurrencyID"]);
            shipmentmodel.AWB = data["tAWB"];
            shipmentmodel.InscanId = Convert.ToInt32(data["tInScanId"]);
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
            if (data["tfwdagent"] == null || data["tfwdagent"] == "" || data["tfwdagent"] == "null")
                { 
            }
            else
            {
                shipmentmodel.FwdAgentId = Convert.ToInt32(data["tfwdagent"]);
            }
            
            shipmentmodel.FwdAgentAWBNo = data["tfwdawb"];
            if (data["tfwddate"] == null || data["tfwddate"] == "")
            {

            }
            else
            {
                //shipmentmodel.ForwardDate =DateTime.Parse(data["tfwddate"]);
                shipmentmodel.FwdDate = DateTime.Parse(data["tfwddate"]);
            }
            //shipmentmodel.FwdDate = DateTime.Now;
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
           
            if (i.HasValue) //delete mode
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
                    if (s_ImportShipment.Shipments.Count ==0)
                    {
                        s_ImportShipment.Shipments.Add(shipmentsession);
                    }
                    else
                    {
                        s_ImportShipment.Shipments[Serialnumber] = shipmentsession;
                    }
                    //s_ImportShipment.Shipments.RemoveAt(Serialnumber);                  
                    
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
            //ViewBag.Cities =db.CityMasters.ToList();
            ViewBag.FwdAgentId =db.AgentMasters.Where(cc=>cc.AgentType==4).ToList(); //  db.ForwardingAgentMasters.ToList();
            //ViewBag.Countries = db.CountryMasters.ToList();
            //ViewBag.DestinationCountryID = db.CountryMasters.ToList();
            ViewBag.CurrencyID = db.CurrencyMasters.ToList();
            ViewBag.Currencies = db.CurrencyMasters.ToList();
            return PartialView("ExportShipmentList", s_ImportShipment);
        }

        public ActionResult Create(int ImportShipId,string SelectedImportdetail)
        {
            var userid = Convert.ToInt32(Session["UserID"]);
            var today = DateTime.Now.Date;
            var Emp = db.EmployeeMasters.Where(x => x.UserID == userid).Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.EmployeeName, x.EmployeeCode, x.EmployeeID, x.CountryName }).FirstOrDefault();
            var company = db.AcCompanies.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryName, x.CountryID, x.Phone, x.AcCompany1, x.CityName }).FirstOrDefault();

            ImportShipment simportshipment = db.ImportShipments.Find(ImportShipId);



            //var countryname = db.CountryMasters.Where(d => d.CountryID == company.CountryID).Select(x => x.CountryName).FirstOrDefault();
            ExportShipmentFormModel _ExportShipment = new ExportShipmentFormModel();
                
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

                List<int> importdetailids = SelectedImportdetail.Split(',').Select(Int32.Parse).ToList(); ; //&& (d.CourierStatusID < 2 || d.CourierStatusID == null
                var shipmentdetails = db.ImportShipmentDetails.Where(d => d.ImportID == ImportShipId && importdetailids.Contains(d.ShipmentDetailID)).ToList();
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
                    //model.ExportID = simportshipment.ID; // s_ImportShipment.ID;
                    model.Reciver = item.Reciver;
                    model.PCS = item.PCS;
                    model.DestinationCountry = item.DestinationCountry;
                model.DestinationCity = item.DestinationCity;
                    model.ImportDetailID = item.ImportID;
                    model.ImportShipmentDetailID = item.ShipmentDetailID;
                    _ExportShipment.Shipments.Add(model);                

                }

            Session["PreviousShipments"] = _ExportShipment.Shipments;
            _ExportShipment.TotalAWB = _ExportShipment.Shipments.Count;
            ViewBag.ImportManifest = db.ImportShipments.Find(ImportShipId).ManifestNumber;
                var countries = db.CountryMasters.ToList();// db.CountryMasters.ToList();
                ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");

                ViewBag.FwdAgentId = db.AgentMasters.Where(cc => cc.AgentType == 4).ToList();

                ViewBag.AgentList = db.AgentMasters.Where(cc => cc.AgentType != 4).ToList(); // db.ForwardingAgentMasters.ToList();// db.ForwardingAgentMasters.ToList();

                //ViewBag.DestinationCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
                //ViewBag.OriginCountryID = countries;
                //ViewBag.DestinationCountryID = countries;
                //ViewBag.Cities =db.CityMasters.ToList();
                //ViewBag.Countries = db.CountryMasters.ToList();         
                ViewBag.Type = db.tblShipmentTypes.ToList(); // types; // db.tblStatusTypes.ToList();
                ViewBag.CurrencyID = db.CurrencyMasters.ToList();// db.CurrencyMasters.ToList();
                ViewBag.Currencies = db.CurrencyMasters.ToList();
                ViewBag.AgentName = Emp.EmployeeName;
                ViewBag.CompanyName = company.AcCompany1;
                return View(_ExportShipment);
                       
            
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

            //ViewBag.Cities = db.CityMasters.ToList();
            ViewBag.FwdAgentId = db.AgentMasters.Where(cc => cc.AgentType == 4).ToList(); //  db.ForwardingAgentMasters.ToList();
            //ViewBag.Countries = db.CountryMasters.ToList();
            //ViewBag.DestinationCountryID = db.CountryMasters.ToList();
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
            var emp = db.EmployeeMasters.Where(u => u.UserID == userid).Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.EmployeeID, x.EmployeeCode, x.EmployeeName }).FirstOrDefault();
            var company = db.AcCompanies.FirstOrDefault(); // db.S_AcCompany.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID,x.Country, x.Phone, x.AcCompany }).FirstOrDefault();
            var agent = db.AgentMasters.Where(d => d.AgentID == model.AgentID).FirstOrDefault();
            var _exportShipment = new ExportShipment();
            int ImportId = 0;

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
                        ShipmentTypeId = model.ShipmentTypeId,
                        Type = "import"

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

                if (model.ID == 0)
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
                            if (e_details.InscanId > 0)
                            {
                                var _inscan = db.InScanMasters.Find(e_details);
                                _inscan.StatusTypeId = db.tblStatusTypes.Where(tt => tt.Name == "FORWARDED").FirstOrDefault().ID;

                                _inscan.CourierStatusID = db.CourierStatus.Where(tt => tt.CourierStatus == "Forwarded to Agent").FirstOrDefault().CourierStatusID;
                                db.Entry(_inscan).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                            else if (e_details.ImportShipmentDetailID>0) //update to importshipmentdetails
                            {
                                var _importshipments = db.ImportShipmentDetails.Find(e_details.ImportShipmentDetailID);
                                _importshipments.ExportShipmentID = _exportShipment.ID;
                                ImportId = _importshipments.ImportID;
                                db.Entry(_importshipments).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            e_details.ShipmentDetailID = max;
                            e_details.ExportID = _exportShipment.ID;
                        e_details.HAWB = "";
                            db.ExportShipmentDetails.Add(e_details);
                            db.SaveChanges();
                            max++;
                        }
                        else
                        {
                        ExportShipmentDetail shipmentmodel = db.ExportShipmentDetails.Find(e_details.ShipmentDetailID);
                        shipmentmodel.CurrencyID = e_details.CurrencyID; // Convert.ToInt32(data["tCurrencyID"]);
                        shipmentmodel.AWB = e_details.AWB;
                        shipmentmodel.InscanId = e_details.InscanId;
                        shipmentmodel.HAWB = e_details.HAWB;
                        shipmentmodel.BagNo = e_details.BagNo;
                        shipmentmodel.PCS = e_details.PCS;
                        shipmentmodel.Weight = e_details.Weight;
                        shipmentmodel.Value = e_details.Value;
                        shipmentmodel.Shipper = e_details.Shipper;
                        shipmentmodel.Reciver = e_details.Reciver;
                        shipmentmodel.Contents = e_details.Contents;
                        shipmentmodel.DestinationCountry = e_details.DestinationCountry;
                        shipmentmodel.DestinationCity = e_details.DestinationCity;
                        shipmentmodel.FwdAgentId = e_details.FwdAgentId;
                        shipmentmodel.FwdAgentAWBNo = e_details.FwdAgentAWBNo;
                        //shipmentmodel.ForwardDate = e_details.ForwardDate;
                        shipmentmodel.FwdDate = e_details.FwdDate;
                        shipmentmodel.FwdCharge = e_details.FwdCharge;
                        shipmentmodel.FwdFlight = e_details.FwdFlight;
                        shipmentmodel.OtherCharge = e_details.OtherCharge;
                        //shipmentmodel.ExportID = _exportShipment.ID;
                        db.Entry(shipmentmodel).State = EntityState.Modified;
                        db.SaveChanges();
                        
                        }

                    }
                if (ImportId >0) //close import shipments
                {
                    var importdetails = db.ImportShipmentDetails.Where(cc => cc.ImportID == ImportId && cc.ExportShipmentID == null).ToList();
                    if (importdetails==null || importdetails.Count==0)
                    {
                        var import = db.ImportShipments.Find(ImportId);
                        import.Status = 1;
                        db.Entry(import).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }               
                


                //removed items
                var exportdetails = db.ExportShipmentDetails.Where(d => d.ExportID == _exportShipment.ID).ToList();
                    var exportdetailsid = model.Shipments.Select(s => s.ImportDetailID).ToList();
                    foreach (var e_details in exportdetails)
                    {
                        var _exportfound = model.Shipments.Where(cc => cc.ShipmentDetailID == e_details.ShipmentDetailID).FirstOrDefault();
                        if (_exportfound == null)
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
           

            model.ConsignorAddress = company.Address1 + ", " + company.CountryName + ", Tel: " + company.Phone;
            model.ConsigneeAddress = agent.Address1 + ", " + agent.Address2 + ", " + ", Tel: " + agent.Phone; //agent.Address3 +

            if (model.Shipments == null)
            {
                model.Shipments = new List<ExportShipmentDetail>();
            }


            ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
            var agents = db.AgentMasters.ToList(); // new SelectList(db.ForwardingAgentMasters.OrderBy(x => x.FAgentName), "FAgentID", "FAgentName").ToList();
            ViewBag.AgentList = agents;
            ViewBag.CurrencyID = db.CurrencyMasters.ToList();
            string selectedVal = model.Type;
            //    var types = new List<SelectListItem>
            //{
            //    new SelectListItem{Text = "Select Shipment Type", Value = null, Selected = selectedVal == null},
            //    new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
            //    new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
            //};
            ViewBag.Type = db.tblShipmentTypes.ToList();
            ViewBag.Currencies = db.CurrencyMasters.ToList();
            ViewBag.AgentName = emp.EmployeeName;
            ViewBag.CompanyName = company.AcCompany1;
            ViewBag.FwdAgentId = db.AgentMasters.Where(cc => cc.AgentType == 4).ToList(); //. ForwardingAgentMasters.ToList(); // .Where(d => d.IsForwardingAgent == true).ToList();

            return View(model);

            //return RedirectToAction(Token.Function, Token.Controller);
        }
        public ActionResult Create1(ExportShipmentFormModel model)
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
                        Type = "import",
                        ShipmentTypeId= model.ShipmentTypeId
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
                //ViewBag.AgentID = db.AgentMasters.ToList();
                var agents = db.AgentMasters.ToList(); // new SelectList(db.ForwardingAgentMasters.OrderBy(x => x.FAgentName), "FAgentID", "FAgentName").ToList();
                ViewBag.AgentList = agents;

                ViewBag.OriginCountryID = countries;
                ViewBag.DestinationCountryID = countries;
                ViewBag.CurrencyID = db.CurrencyMasters.ToList();
                ViewBag.Cities =db.CityMasters.ToList();
                ViewBag.Countries = db.CountryMasters.ToList();
                ViewBag.Type = db.tblStatusTypes.ToList();
                ViewBag.Currencies = db.CurrencyMasters.ToList();
                ViewBag.AgentName = emp.EmployeeName;
                ViewBag.CompanyName = company.AcCompany1;
                ViewBag.FwdAgentId = db.AgentMasters.Where(c => c.AgentType == 4).ToList(); //. ForwardingAgentMasters.ToList(); // Where(d => d.IsForwardingAgent == true).ToList();

                return View(model);
            }
            //return RedirectToAction(Token.Function, Token.Controller);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateExport(ExportShipmentFormModel model)
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            var userid = Convert.ToInt32(Session["UserID"]);
            var today = DateTime.Now.Date;
            var emp = db.EmployeeMasters.Where(u=>u.UserID==userid).Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID, x.EmployeeID, x.EmployeeCode, x.EmployeeName }).FirstOrDefault();
            var company = db.AcCompanies.FirstOrDefault(); // db.S_AcCompany.Select(x => new { Address = x.Address1 + ", " + x.Address2 + ", " + x.Address3, x.CountryID,x.Country, x.Phone, x.AcCompany }).FirstOrDefault();
            var agent = db.AgentMasters.Where(d => d.AgentID == model.AgentID).FirstOrDefault();

            List<ExportShipmentDetail> lstdetail = Session["PreviousShipments"] as List<ExportShipmentDetail>;
            model.Shipments = lstdetail;
            var _exportShipment = new ExportShipment();

            if (1==1) // (ModelState.IsValid)
            {
                if (model.ID == 0) // new entry mode
                {

                    _exportShipment = new ExportShipment
                    {
                        AgentID = model.AgentID,
                        EmployeeID = emp.EmployeeID, //1.Value,
                        Bags = model.Bags,
                        CD = model.CD,
                        AcFinancialYearID= yearid,
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
                        ShipmentTypeId = model.ShipmentTypeId,
                        Type = "import"

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
                    _exportShipment.Type = "";
                }

                if (model.ID == 0)
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
                            if (e_details.InscanId > 0)
                            {
                                var _inscan = db.InScanMasters.Find(e_details.InscanId);
                                _inscan.StatusTypeId = db.tblStatusTypes.Where(tt => tt.Name == "READY TO EXPORT").FirstOrDefault().ID;

                                _inscan.CourierStatusID = db.CourierStatus.Where(tt => tt.CourierStatus == "Export Manifest Prepared").FirstOrDefault().CourierStatusID;
                                db.Entry(_inscan).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                            e_details.HAWB = "";
                            e_details.ShipmentDetailID = max;
                            e_details.ExportID = _exportShipment.ID;
                            db.ExportShipmentDetails.Add(e_details);
                            db.SaveChanges();
                            max++;
                        }
                        else
                        {
                            ExportShipmentDetail shipmentmodel= db.ExportShipmentDetails.Find(e_details.ShipmentDetailID);
                            shipmentmodel.CurrencyID = e_details.CurrencyID; // Convert.ToInt32(data["tCurrencyID"]);
                            shipmentmodel.AWB = e_details.AWB;
                            shipmentmodel.InscanId = e_details.InscanId;
                            shipmentmodel.HAWB = e_details.HAWB;
                            shipmentmodel.BagNo = e_details.BagNo;
                            shipmentmodel.PCS = e_details.PCS;
                            shipmentmodel.Weight = e_details.Weight;
                            shipmentmodel.Value = e_details.Value;
                            shipmentmodel.Shipper = e_details.Shipper;
                            shipmentmodel.Reciver = e_details.Reciver;
                            shipmentmodel.Contents = e_details.Contents;
                            shipmentmodel.DestinationCountry = e_details.DestinationCountry;
                            shipmentmodel.DestinationCity = e_details.DestinationCity;
                            shipmentmodel.FwdAgentId = e_details.FwdAgentId;
                            shipmentmodel.FwdAgentAWBNo = e_details.FwdAgentAWBNo;
                            //shipmentmodel.ForwardDate = e_details.ForwardDate;
                            shipmentmodel.FwdDate = e_details.FwdDate;
                            shipmentmodel.FwdCharge = e_details.FwdCharge;
                            shipmentmodel.FwdFlight = e_details.FwdFlight;
                            shipmentmodel.OtherCharge = e_details.OtherCharge;
                            //shipmentmodel.ExportID = _exportShipment.ID;
                            db.Entry(shipmentmodel).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                    //removed items
                    var exportdetails = db.ExportShipmentDetails.Where(d => d.ExportID == _exportShipment.ID).ToList();
                    var exportdetailsid = model.Shipments.Select(s => s.ImportDetailID).ToList();
                    foreach (var e_details in exportdetails)
                    {
                        var _exportfound = model.Shipments.Where(cc => cc.ShipmentDetailID == e_details.ShipmentDetailID).FirstOrDefault();
                        if (_exportfound == null)
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
            else
            {
                foreach(var item in ModelState.Values)
                {
                    if (item.Errors.Count>0)
                    {
                        var ss = item.Errors[0];
                    }
                }

                model.ConsignorAddress = company.Address1 + ", " + company.CountryName + ", Tel: " + company.Phone;
                model.ConsigneeAddress = agent.Address1 + ", " + agent.Address2 + ", " + ", Tel: " + agent.Phone; //agent.Address3 +

                if (model.Shipments == null)
                {
                    model.Shipments = new List<ExportShipmentDetail>();
                }


                ViewBag.OriginCityID = new SelectList(new List<CityMaster>(), "CityID", "City");
                var agents = db.AgentMasters.ToList(); // new SelectList(db.ForwardingAgentMasters.OrderBy(x => x.FAgentName), "FAgentID", "FAgentName").ToList();
                ViewBag.AgentList = agents;
                ViewBag.CurrencyID = db.CurrencyMasters.ToList();
                string selectedVal = model.Type;
                //    var types = new List<SelectListItem>
                //{
                //    new SelectListItem{Text = "Select Shipment Type", Value = null, Selected = selectedVal == null},
                //    new SelectListItem{Text = "Transhipment", Value = "Transhipment", Selected = selectedVal == "Transhipment"},
                //    new SelectListItem{Text = "Import", Value = "Import", Selected = selectedVal == "Import"},
                //};
                ViewBag.Type = db.tblShipmentTypes.ToList();
                ViewBag.Currencies = db.CurrencyMasters.ToList();
                ViewBag.AgentName = emp.EmployeeName;
                ViewBag.CompanyName = company.AcCompany1;
                ViewBag.FwdAgentId = db.AgentMasters.Where(cc => cc.AgentType == 4).ToList(); //. ForwardingAgentMasters.ToList(); // .Where(d => d.IsForwardingAgent == true).ToList();

                return View(model);
            }
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
                        foreach (var item in Prevshipmentsession)
                        {
                            s_ImportShipment.Shipments.Add(item);
                        }
                    }
                    
                }
                s_ImportShipment.Shipments = Session["PreviousShipments"] as List<ExportShipmentDetail>;
                CMSV2.Models.ExportShipmentDetail s = s_ImportShipment.Shipments[Convert.ToInt32(i)];
               
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
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ExportShipmentFormModel _ExportShipment = new ExportShipmentFormModel();

                var exportshipment = db.ExportShipments.Find(id);
                _ExportShipment.ID = exportshipment.ID;
                _ExportShipment.ManifestNumber = exportshipment.ManifestNumber;
                _ExportShipment.ShipmentTypeId = exportshipment.ShipmentTypeId;
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
                _ExportShipment.ShipmentTypeId = exportshipment.ShipmentTypeId;
                _ExportShipment.Type = db.tblShipmentTypes.Find(exportshipment.ShipmentTypeId).ShipmentType;
                _ExportShipment.ShipmentsVM = (from c in db.ExportShipmentDetails 
                                               join cur in db.CurrencyMasters on c.CurrencyID equals cur.CurrencyID
                                               join agent in db.AgentMasters on c.FwdAgentId equals agent.AgentID into gj
                                               from subpet in gj.DefaultIfEmpty()
                                               where c.ExportID == id
                                            select new ExportShipmentDetailVM {ShipmentDetailID =c.ShipmentDetailID,
                                        ImportDetailID=c.ImportDetailID,
                                        ExportID = c.ExportID,HAWB=c.HAWB,AWB=c.AWB,PCS=c.PCS,
                                        Weight=c.Weight,Contents=c.Contents, Shipper=c.Shipper,Value=c.Value,            
                                        Reciver=c.Reciver,DestinationCountry=c.DestinationCountry,DestinationCity=c.DestinationCity,
                                        BagNo=c.BagNo,CurrencyID=c.CurrencyID,FwdAgentId=c.FwdAgentId,FwdAgentAWBNo=c.FwdAgentAWBNo,FwdDate=c.FwdDate,FwdFlight=c.FwdFlight,
                                        FwdCharge=c.FwdCharge,OtherCharge=c.OtherCharge,InscanId=c.InscanId,ImportShipmentDetailID=c.ImportShipmentDetailID,
                                                ForwardAgentName=subpet.Name,CurrencyName=cur.CurrencyName,
                                                CurrenySymbol=cur.Symbol
                                            }).ToList();

               ViewBag.Edit = true; // Token.Permissions.Updation;
               ViewBag.agents = db.AgentMasters.Where(cc => cc.AgentType == 4).ToList();
               return View(_ExportShipment);            
        }
        public ActionResult ExportShipmentReport(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ExportShipmentFormModel _ExportShipment = new ExportShipmentFormModel();

            var exportshipment = db.ExportShipments.Find(id);
            _ExportShipment.ID = exportshipment.ID;
            _ExportShipment.ManifestNumber = exportshipment.ManifestNumber;
            _ExportShipment.ShipmentTypeId = exportshipment.ShipmentTypeId;
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
            _ExportShipment.ShipmentTypeId = exportshipment.ShipmentTypeId;
            _ExportShipment.Type = db.tblShipmentTypes.Find(exportshipment.ShipmentTypeId).ShipmentType;
            _ExportShipment.ShipmentsVM = (from c in db.ExportShipmentDetails
                                           join ins in db.InScanMasters on c.InscanId equals ins.InScanID
                                           join cur in db.CurrencyMasters on c.CurrencyID equals cur.CurrencyID
                                           join agent in db.AgentMasters on c.FwdAgentId equals agent.AgentID into gj
                                           from subpet in gj.DefaultIfEmpty()
                                           where c.ExportID == id
                                           select new ExportShipmentDetailVM
                                           {
                                               ShipmentDetailID = c.ShipmentDetailID,
                                               ImportDetailID = c.ImportDetailID,
                                               ExportID = c.ExportID,
                                               HAWB = c.HAWB,
                                               AWB = c.AWB,
                                               PCS = c.PCS,
                                               Weight = c.Weight,
                                               Contents = c.Contents,
                                               Shipper = c.Shipper,
                                               Value = c.Value,
                                               Reciver = c.Reciver,
                                               DestinationCountry = c.DestinationCountry,
                                               DestinationCity = c.DestinationCity,
                                               OriginCountry = ins.ConsignorCountryName,
                                               ConsignorPhone = ins.ConsignorPhone,
                                               ConsigneePhone = ins.ConsigneePhone,
                                               AWBCourierCharge = ins.CourierCharge,
                                               AWBOtherCharge = ins.OtherCharge,
                                               BagNo = c.BagNo,
                                               CurrencyID = c.CurrencyID,
                                               FwdAgentId = c.FwdAgentId,
                                               FwdAgentAWBNo = c.FwdAgentAWBNo,
                                               FwdDate = c.FwdDate,
                                               FwdFlight = c.FwdFlight,
                                               FwdCharge = c.FwdCharge,
                                               OtherCharge = c.OtherCharge,
                                               InscanId = c.InscanId,
                                               ImportShipmentDetailID = c.ImportShipmentDetailID,
                                               ForwardAgentName = subpet.Name,
                                               CurrencyName = cur.CurrencyName,
                                               CurrenySymbol = cur.Symbol
                                           }).ToList();

            ViewBag.Edit = true; // Token.Permissions.Updation;
            ViewBag.agents = db.AgentMasters.Where(cc => cc.AgentType == 4).ToList();
            return View(_ExportShipment);
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
                ViewBag.Agents = db.AgentMasters.ToList(); // db.ForwardingAgentMasters.ToList(); // .Where(d => d.IsForwardingAgent == false).ToList(); ;
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
        public ActionResult ViewImport([Bind(Include = "FlightDate,ManifestNumber,AgentID,FlightNo,MAWB,OriginAirportCity,ConsignorCountryName,ConsigneeCountryName,CreatedDate,DestinationAirportCity")] ImportShipmentFormModel picker)
        {
         //   AuthHelp Token = repos.Authenticate();
            if (1==1)
            {
                DatePicker model = new DatePicker
                {
                    //FromDate = picker.FlightDate.Value,
                    Delete = true,
                    Update = true,
                    Create = true,
                    AgentId=0,
                    StatusId=0
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
                    OriginAirportCity=picker.OriginAirportCity,
                    DestinationAirportCity = picker.DestinationAirportCity,
                    ConsigneeCountryName = picker.ConsigneeCountryName,
                    CreatedDate=picker.CreatedDate,
                };
                ViewBag.Token = model;
                Session["Filters"] = model1;
                ViewBag.Agents = db.AgentMasters.ToList(); // ForwardingAgentMasters.ToList();// .Where(d => d.IsForwardingAgent == false).ToList(); ;
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

                //DatePicker model = new DatePicker
                //{
                //    FromDate = datePicker.FlightDate.Value.Date,
                //    Delete = true,// (bool)Token.Permissions.Deletion,
                //    Update = true, //(bool)Token.Permissions.Updation,
                //    Create = true, //(bool)Token.Permissions.Creation
                //};

                ViewBag.Token = datePicker;
                var s_ImportShipment = new List<ImportShipment>();
                //var strposteddate =datePicker.FlightDate.Value.ToShortDateString();
                var strposteddate1 =datePicker.CreatedDate.ToShortDateString();
                //if (strposteddate == "01-01-0001")
                //{
                //    strposteddate = null;
                //}
                if (strposteddate1 == "01-01-0001")
                {
                    strposteddate1 = null;
                }

                s_ImportShipment = db.ImportShipments.Where(cc=>cc.Status==0).ToList(); // .Include(s => s.UserRegistration.UserID).Include(s => s.UserRegistration.UserID).Include(s => s.UserRegistration).ToList();


                if (datePicker.FlightDate!=null) // && strposteddate !=null)
                {
                    s_ImportShipment = s_ImportShipment.Where(x =>x.FlightDate > datePicker.FlightDate.Value.Date.AddHours(-23).AddMinutes(-59).AddSeconds(-59) && x.FlightDate<=datePicker.FlightDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59)).ToList();

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
                if (datePicker.OriginAirportCity != null )
                {
                    s_ImportShipment = s_ImportShipment.Where(d => datePicker.OriginAirportCity.Contains(d.OriginAirportCity) || d.OriginAirportCity==datePicker.OriginAirportCity).ToList();

                }
                if (datePicker.ConsigneeCountryName != null && datePicker.ConsigneeCountryName !="")
                {
                    s_ImportShipment = s_ImportShipment.Where(d => datePicker.ConsigneeCountryName.Contains(d.ConsigneeCountryName)).ToList();

                }
                if (datePicker.DestinationAirportCity != null && datePicker.DestinationAirportCity != "")
                {
                    s_ImportShipment = s_ImportShipment.Where(d => datePicker.DestinationAirportCity.Contains(d.DestinationAirportCity)).ToList();

                }
                s_ImportShipment = s_ImportShipment.OrderByDescending(d => d.CreatedDate).ToList();
                //s_ImportShipment.ForEach(d => { d.ConsignorCountryName = d.ConsignorCountryName; d.ConsigneeCountryName = d.ConsigneeCountryName; });
                //s_ImportShipment = db.ImportShipments.ToList();
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
                    TotalAWB = x.TotalAWB,
                    Type = x.Type,
                    ConsignorCountryName=x.ConsignorCountryName,
                    
                    
                }).FirstOrDefault();

                data.Shipments = db.ImportShipmentDetails.Where(cc => cc.ImportID == data.ID && cc.ExportShipmentID==null).ToList();

                    //(from impd in db.ImportShipmentDetails
                    //              join c in db.CurrencyMasters on impd.CurrencyID equals c.CurrencyID
                    //              where impd.ImportID == data.ID
                    //              select new ImportShipmentDetailVM
                    //              {
                    //                  ShipmentDetailID = impd.ShipmentDetailID,
                    //                  ImportID = impd.ImportID,
                    //                  AWB = impd.AWB,
                    //                  PCS = impd.PCS,
                    //                  Weight = impd.Weight,
                    //                  Contents = impd.Contents,
                    //                  Shipper = impd.Shipper,
                    //                  Value = impd.Value,
                    //                  Reciver = impd.Reciver,
                    //                  DestinationCity = impd.DestinationCity,
                    //                  DestinationCountry = impd.DestinationCountry,
                    //                  BagNo = impd.BagNo,
                    //                  CurrencyID = impd.CurrencyID,
                    //                  ExportShipmentID = impd.ExportShipmentID,
                    //                  QuickInscanID = impd.QuickInscanID,
                    //                  StatusTypeId = impd.StatusTypeId,
                    //                  CourierStatusID = impd.CourierStatusID,
                    //                  CurrencyName = c.CurrencyName
                    //              }).ToList();
                                  
                               
                                   
            //db.ImportShipmentDetails.Where(d => d.ImportID == data.ID).ToList(); //  && d.CourierStatusID == 15).ToList();

                //var countries = db.CountryMasters.ToList();
                //ViewBag.DestinationCountryID = countries;
                var agent = db.AgentMasters.Where(d => d.AgentType == 4).FirstOrDefault();// db.ForwardingAgentMasters.Where(d=>d.FAgentID==data.AgentID).FirstOrDefault();
                ViewBag.AgentName = agent.Name;
                return View(data);
            }
            //return RedirectToAction(Token.Function, Token.Controller);
        }

        public JsonResult GetAWBDetail(string id)
        {
            ExportAWBList obj = new ExportAWBList();
            var lst = (from c in db.InScanMasters where c.AWBNo == id select c).FirstOrDefault();
            if (lst == null)
            {
                return Json(new { status = "failed", data = obj, message = "AWB No. Not found" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //if (lst.QuickInscanID == null)
                //{
                obj.DestinationCity = lst.ConsigneeCityName;
                 obj.DestinationCountry = lst.ConsigneeCountryName;
                //obj.Bags = lst.BagNo;
                obj.Weight = lst.Weight;
                obj.Shipper = lst.Consignor;
                obj.Receiver = lst.Consignee;
                obj.Pieces = Convert.ToInt32(lst.Pieces);
                obj.Contents = lst.CargoDescription;
                obj.AWB = lst.AWBNo;
                obj.InScanId = lst.InScanID;
                obj.Value = Convert.ToDecimal(lst.NetTotal);

                    return Json(new { status = "ok", data = obj, message = "AWB NO.found" }, JsonRequestBehavior.AllowGet);

                //}
                //else
                //{
                //    return Json(new { status = "failed", data = obj, message = "InScan already Done!" }, JsonRequestBehavior.AllowGet);
                //}
            }

        }

        public bool AddShippmentToTable1(FormCollection data)
        {
            var shipmentmodel = new ExportShipmentDetail();
            shipmentmodel.CurrencyID = Convert.ToInt32(data["tCurrencyID"]);
            shipmentmodel.AWB = data["tAWB"];
            shipmentmodel.InscanId = Convert.ToInt32(data["tInScanId"]);
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
                shipmentmodel.FwdDate = DateTime.Parse(data["tfwddate"]);
            }
            shipmentmodel.FwdCharge = Convert.ToDecimal(data["tfwdcharge"]);
            shipmentmodel.FwdFlight = data["tfwdflight"];
            shipmentmodel.OtherCharge = Convert.ToDecimal(data["totherchrg"]);
            shipmentmodel.ImportDetailID = Convert.ToInt32(data["timportid"]);
            shipmentmodel.ImportShipmentDetailID = Convert.ToInt32(data["timportdetailid"]);
            Session["EShipmentdetails"] = shipmentmodel;
            Session["EShipSerialNumber"] = Convert.ToInt32(data["tSerialNum"]);
            Session["EIsUpdate"] = Convert.ToBoolean(data["isupdate"]);
            return true;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddOrRemoveShipment2(ExportShipmentFormModel s_ImportShipment, int? i)
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
                s_ImportShipment.Shipments.RemoveAt(i.Value);
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
                    if (s_ImportShipment.Shipments.Count == 0)
                    {
                        s_ImportShipment.Shipments.Add(shipmentsession);
                    }
                    else
                    {
                        s_ImportShipment.Shipments[Serialnumber] = shipmentsession;
                    }
                    //s_ImportShipment.Shipments.RemoveAt(Serialnumber);                  

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
            //ViewBag.Cities = db.CityMasters.ToList();
            ViewBag.FwdAgentId = db.AgentMasters.Where(cc => cc.AgentType == 4).ToList(); //  db.ForwardingAgentMasters.ToList();
            //ViewBag.Countries = db.CountryMasters.ToList();
            ViewBag.DestinationCountryID = db.CountryMasters.ToList();
            ViewBag.CurrencyID = db.CurrencyMasters.ToList();
            ViewBag.Currencies = db.CurrencyMasters.ToList();
            return PartialView("ShipmentList", s_ImportShipment);
        }

    }

    public class ExportAWBList
    {
        public int InScanId { get; set; }
        public string AWB { get; set; }
        
        public string Shipper { get; set; }

        public string Receiver { get; set; }

        public string Bags { get; set; }

        public decimal? Weight { get; set; }
        public int Pieces { get; set; }
        
        public string Contents { get; set; }
        public decimal Value { get; set; }
        public string DestinationCountry { get; set; }
        public string DestinationCity { get; set; }

    }


}