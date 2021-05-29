using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using CMSV2.DAL;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Reflection;
using ExcelDataReader;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;


namespace CMSV2.Controllers
{
    [SessionExpire]
    public class TranshipmentController : Controller
    {
        // GET: Transhipment
        Entities1 db = new Entities1();
        public ActionResult Index()
        {


            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            ImportManifestSearch obj = (ImportManifestSearch)Session["TranshipmentSearch"];
            ImportManifestSearch model = new ImportManifestSearch();
            AWBDAO _dao = new AWBDAO();
            if (obj != null)
            {
                List<ImportManifestVM> translist = new List<ImportManifestVM>();
                translist = ImportDAO.GetTranshipmentManifestList(4);
                model.FromDate = obj.FromDate;
                model.ToDate = obj.ToDate;
                model.Details = translist;
            }
            else
            {
                model.FromDate = CommanFunctions.GetLastDayofMonth().Date;
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
                Session["TranshipmentSearch"] = model;
                List<ImportManifestVM> translist = new List<ImportManifestVM>();
                translist = ImportDAO.GetTranshipmentManifestList(4);
                model.Details = translist;

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ImportManifestSearch obj)
        {
            Session["TranshipmentSearch"] = obj;
            return RedirectToAction("Index");
        }

        public ActionResult Create(int id = 0)
        {
            var userid = Convert.ToInt32(Session["UserID"]);
            var CompanyID = Convert.ToInt32(Session["CurrentCompanyID"]);
            var BranchID = Convert.ToInt32(Session["CurrentBranchID"]);

            var agent = db.AgentMasters.ToList(); // .Where(cc => cc.UserID == userid).FirstOrDefault();
            var company = db.AcCompanies.FirstOrDefault(); // .Select(x => new { Address = x.Address1 
            string selectedVal = ""; ;
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
            ViewBag.Agent = agent;
            string CompanyCountryName = db.BranchMasters.Find(BranchID).CountryName;
            //ViewBag.AgentName = agent.Name;
            //ViewBag.AgentCity = agent.CityName;
            ViewBag.CompanyName = company.AcCompany1;
            ImportManifestVM vm = new ImportManifestVM();
            if (id == 0)
            {
                vm.CompanyCountryName = CompanyCountryName;                
                vm.ManifestDate = CommanFunctions.GetCurrentDateTime().ToString();
                vm.ManifestNumber = ImportDAO.GetMaxManifestNo(CompanyID, BranchID,Convert.ToDateTime(vm.ManifestDate),"T");
                vm.ID = 0;
                vm.TransDetails = new List<TranshipmentModel>();
            }
            else
            {
                vm.CompanyCountryName = CompanyCountryName;
                ImportShipment model = db.ImportShipments.Find(id);
                vm.ID = model.ID;
                vm.ManifestNumber = model.ManifestNumber;
                vm.ManifestDate = model.CreatedDate.ToString();
                vm.FlightDate1 = model.FlightDate.ToString();
                vm.FlightNo = model.FlightNo;
                vm.MAWB = model.MAWB;
                vm.Route = model.Route;
                vm.ParcelNo = model.ParcelNo;
                vm.Bags = model.Bags;
                vm.Type = model.Type;
                vm.Route = model.Route;
                vm.Weight = model.Weight;
                vm.TotalAWB = model.TotalAWB;
                vm.OriginAirportCity = model.OriginAirportCity;
                vm.DestinationAirportCity = model.DestinationAirportCity;
                vm.AgentID = model.AgentID;
                vm.TransDetails = new List<TranshipmentModel>();
                //var IDEtails = (from c in db.InScanMasters where c.ImportShipmentId == vm.ID select new TranshipmentModel { InScanID=c.InScanID, HAWBNo =c.AWBNo,AWBDate=c.TransactionDate.ToString("dd-MM-yyyy"),Consignor=c.Consignor,ConsignorCountryName=c.ConsignorCountryName,ConsignorCityName=c.ConsignorCityName,Consignee=c.Consignee,ConsigneeCityName=c.ConsigneeCityName,Weight=c.Weight,Pieces=c.Pieces,CourierCharge=c.CourierCharge,OtherCharge=c.OtherCharge}).ToList();
                var IDetails = ImportDAO.GetTranshipmenItems(vm.ID);
                vm.TransDetails = IDetails;
            }

            return View(vm);
        }
        public ActionResult ShowItemList()
        {
            ImportManifestVM vm = new ImportManifestVM();
            vm.TransDetails = (List<TranshipmentModel>)Session["ManifestTranshipment"];
            return PartialView("ItemList", vm);
        }

        [HttpPost]
        public JsonResult ImportFile(HttpPostedFileBase importFile)
        {
            if (importFile == null) return Json(new { Status = 0, Message = "No File Selected" });

            try
            {
                var fileData = GetDataFromCSVFile(importFile.InputStream);

                ImportManifestVM vm = new ImportManifestVM();
                vm.TransDetails = fileData; 
                 Session["ManifestTranshipment"] = vm.TransDetails;
                return Json(new { Status = 1, data = fileData, Message = "File Imported Successfully " });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message });
            }
        }
        private List<TranshipmentModel> GetDataFromCSVFile(Stream stream)
        {
            var empList = new List<TranshipmentModel>();
            try
            {
                using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                {
                    var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true // To set First Row As Column Names    
                        }
                    });

                    if (dataSet.Tables.Count > 0)
                    {
                        var dataTable = dataSet.Tables[0];
                        int i = 1;
                        foreach (DataRow objDataRow in dataTable.Rows)
                        {
                            if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                            empList.Add(new TranshipmentModel()
                            {
                                SNo = i++,
                                HAWBNo = objDataRow["HAWBNo"].ToString(),
                                AWBDate = objDataRow["AWBDate"].ToString(),
                                Customer = objDataRow["Customer"].ToString(),
                                ConsignorPhone = objDataRow["TelephoneNo"].ToString(),
                                Consignor = objDataRow["Consignor"].ToString(),
                                ConsignorLocationName = objDataRow["ConsignorLocation"].ToString(),
                                ConsignorCountryName = objDataRow["ConsignorCountry"].ToString(),
                                ConsignorCityName = objDataRow["ConsignorCity"].ToString(),
                                Consignee = objDataRow["Consignee"].ToString(),
                                ConsigneeCountryName = objDataRow["ConsigneeCountry"].ToString(),
                                ConsigneeCityName = objDataRow["ConsigneeCity"].ToString(),
                                ConsigneeLocationName = objDataRow["ConsigneeLocation"].ToString(),
                                ConsignorAddress1_Building = objDataRow["ConsignorAddress"].ToString(),
                                ConsigneeAddress1_Building = objDataRow["ConsigneeAddress"].ToString(),
                                ConsignorMobile = objDataRow["ConsignorTelephone"].ToString(),
                                ConsigneeMobile = objDataRow["ConsigneeTelephone"].ToString(),
                                Weight = CommanFunctions.ParseDecimal(objDataRow["Weight"].ToString()),
                                Pieces = objDataRow["Pieces"].ToString(),
                                CourierCharge = CommanFunctions.ParseDecimal(objDataRow["CourierCharge"].ToString()),
                                OtherCharge = CommanFunctions.ParseDecimal(objDataRow["OtherCharge"].ToString()),
                                PaymentMode = objDataRow["PaymentMode"].ToString(),
                                ReceivedBy = objDataRow["ReceiverName"].ToString(),
                                CollectedBy = objDataRow["CollectedName"].ToString(),
                                FAWBNo = objDataRow["FwdNo"].ToString(),
                                FAgentName = objDataRow["ForwardingAgent"].ToString(),
                                CourierType = objDataRow["Couriertype"].ToString(),
                                MovementType = objDataRow["MovementType"].ToString(),
                                CourierStatus = objDataRow["CourierStatus"].ToString(),
                                remarks = objDataRow["Remarks"].ToString() //Department and Bag no is missing                                                               


                            });
                            //AWBNo AWBDate Bag NO.	Shipper ReceiverName    ReceiverContactName ReceiverPhone   ReceiverAddress DestinationLocation DestinationCountry Pcs Weight CustomsValue    COD Content Reference Status  SynchronisedDateTime

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return empList;
        }


        [HttpPost]
        public string SaveImport(string Master, string Details)
        {
            try
            {
                var userid = Convert.ToInt32(Session["UserID"]);
                var model = JsonConvert.DeserializeObject<ImportManifestVM>(Master);
                //var IDetails = JsonConvert.DeserializeObject<List<ImportManifestItem>>(Details);
                var IDetails = (List<TranshipmentModel>)Session["ManifestTranshipment"];
                ImportShipment importShipment = new ImportShipment();
                if (model.ID == 0)
                {
                    importShipment.ManifestNumber = model.ManifestNumber;
                    importShipment.CreatedDate = Convert.ToDateTime(model.ManifestDate);
                    importShipment.ShipmentTypeId = 4;
                }
                else
                {
                    importShipment = db.ImportShipments.Find(model.ID);
                }
                importShipment.Bags = model.Bags;
                importShipment.FlightNo = model.FlightNo;
                importShipment.FlightDate = Convert.ToDateTime(model.FlightDate1);
                importShipment.LastEditedByLoginID = userid;
                importShipment.MAWB = model.MAWB;
                importShipment.TotalAWB = model.TotalAWB;
                importShipment.Type = "";
                importShipment.Status = 1;
                importShipment.DestinationAirportCity = model.DestinationAirportCity;
                importShipment.OriginAirportCity = model.OriginAirportCity;
                importShipment.AcFinancialYearID = 3;
                importShipment.TotalAWB = model.TotalAWB;
                importShipment.Bags = model.Bags;
                importShipment.ParcelNo = model.ParcelNo;
                importShipment.AgentID = model.AgentID;
                importShipment.Weight = model.Weight;
                importShipment.Route = model.Route;
                importShipment.AgentLoginID = 1;
                importShipment.LastEditedByLoginID = 1;

                if (model.ID == 0)
                {
                    db.ImportShipments.Add(importShipment);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(importShipment).State = EntityState.Modified;
                    db.SaveChanges();

                    //var details = (from d in db.ImportShipmentDetails where d.ImportID == model.ID select d).ToList();
                    //db.ImportShipmentDetails.RemoveRange(details);
                    //db.SaveChanges();

                }

                foreach (var item in IDetails)
                {
                    InScanMaster detail = new InScanMaster();
                    InScanInternationalDeatil isid = new InScanInternationalDeatil();
                    InScanInternational isi = new InScanInternational();

                    if (item.InScanID > 0)
                        detail = db.InScanMasters.Find(item.InScanID);

                    detail.ImportShipmentId = importShipment.ID; //forignkey reference for import tranashipment
                    detail.AWBNo = item.HAWBNo;
                    detail.TransactionDate = Convert.ToDateTime(item.AWBDate);
                    detail.Consignee = item.Consignee;
                    detail.ConsigneeAddress1_Building = item.ConsigneeAddress1_Building;
                    detail.ConsignorPhone = item.ConsignorPhone;
                    detail.ConsigneeCountryName = item.ConsigneeCountryName;
                    detail.ConsigneeCityName = item.ConsigneeCityName;
                    detail.Consignor = item.Consignor;

                    detail.ParcelTypeId = 1;

                    detail.ProductTypeID = 1;

                    detail.MovementID = 4;

                    if (item.CourierType !="")
                    {
                        var productype = db.ProductTypes.Where(cc => cc.ProductName == item.CourierType).FirstOrDefault();
                        if (productype != null)
                        {
                            detail.ProductTypeID = productype.ProductTypeID;
                        }

                    }
                    if (item.ProductType!= "")
                    {
                        var productype = db.ParcelTypes.Where(cc => cc.ParcelType1 == item.ProductType).FirstOrDefault();
                        if (productype != null)
                        {
                            detail.ParcelTypeId = productype.ParcelTypeID;
                        }

                    }

                    if (item.Weight != null)
                        detail.Weight = Convert.ToDecimal(item.Weight);

                    if (item.Pieces != null)
                        detail.Pieces = item.Pieces;                   

                    if (item.CourierCharge !=null)
                        detail.CourierCharge = Convert.ToDecimal(item.CourierCharge);

                    if (item.OtherCharge != null)
                        detail.OtherCharge = Convert.ToDecimal(item.OtherCharge);

                    if (item.FAgentName != "")
                    {
                        var fage = db.ForwardingAgentMasters.Where(cc => cc.FAgentName == item.FAgentName).FirstOrDefault();
                        detail.FAgentId = fage.FAgentID;

                    }

                    if (item.customer!="")
                    {
                        var customer = db.CustomerMasters.Where(cc => cc.CustomerName == item.Customer).FirstOrDefault();
                        if (customer!=null)
                        {
                            detail.CustomerID = customer.CustomerID;
                        }
                    }
                    if (item.PaymentMode=="Customer")
                    {
                        detail.PaymentModeId = 3;
                    }
                    if (item.ReceivedBy!="")
                    {
                        var emp = db.EmployeeMasters.Where(cc => cc.EmployeeName == item.ReceivedBy).FirstOrDefault();
                        if (emp!=null)
                        {
                            detail.DepotReceivedBy = emp.EmployeeID;
                        }

                    }
                    if (item.CollectedBy != "")
                    {
                        var emp = db.EmployeeMasters.Where(cc => cc.EmployeeName == item.CollectedBy).FirstOrDefault();
                        if (emp != null)
                        {
                            detail.PickedUpEmpID = emp.EmployeeID;
                        }

                    }
                    if (item.CourierStatus!="")
                    {
                        var courierstatus = db.CourierStatus.Where(cc => cc.CourierStatus == "Shipment Delivered").FirstOrDefault();
                        if (courierstatus!=null)
                        {
                            detail.CourierStatusID = courierstatus.CourierStatusID;
                            detail.StatusTypeId = courierstatus.StatusTypeID;
                        }
                    }

                    if (item.InScanID == 0)
                    {
                        db.InScanMasters.Add(detail);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (detail.FAgentId > 0)
                    {
                        isid = db.InScanInternationalDeatils.Where(cc => cc.InScanID == detail.InScanID).FirstOrDefault();
                        isi = db.InScanInternationals.Where(cc => cc.InScanID == detail.InScanID).FirstOrDefault();
                        if (isid == null)
                        {
                            isid = new InScanInternationalDeatil();
                            isid.InScanID = detail.InScanID;
                            isid.ForwardingCharge = Convert.ToDecimal(item.ForwardingCharge);
                            isid.VerifiedWeight = detail.Weight;
                        }
                        if (isi == null)
                        {
                            isi = new InScanInternational();
                            isi.InScanID = detail.InScanID;
                            isi.FAgentID = Convert.ToInt32(detail.FAgentId);
                            isi.ForwardingCharge = Convert.ToDecimal(item.ForwardingCharge);
                            isi.StatusAssignment = false;
                            isi.ForwardingAWBNo = item.FAWBNo;
                            isi.ForwardingDate = detail.TransactionDate; // DateTime.UtcNow;
                            isi.VerifiedWeight = Convert.ToDouble(detail.Weight);

                        }

                        if (isi.InScanInternationalID == 0)
                        {
                            db.InScanInternationals.Add(isi);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Entry(isi).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        if (isid.InScanInternationalDeatilID == 0)
                        {

                            isid.InscanInternationalID = isi.InScanInternationalID;
                            db.InScanInternationalDeatils.Add(isid);
                            db.SaveChanges();
                        }
                        else
                        {

                            db.Entry(isid).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;

            }
        }

        [HttpPost]
        public JsonResult GetImportItem(int id)
        {
            List<ImportManifestItem> details = new List<ImportManifestItem>();
            details = (from c in db.ImportShipmentDetails where c.ImportID == id select new ImportManifestItem { AWBNo = c.AWB, AWBDate = c.AWBDate.ToString(), Shipper = c.Shipper, Receiver = c.Receiver, ReceiverContact = c.ReceiverContact, ReceiverAddress = c.ReceiverAddress, ReceiverPhone = c.ReceiverTelephone, Content = c.Contents, Bag = c.BagNo.ToString(), Weight = c.Weight.ToString(), Pcs = c.PCS, Value = c.CustomValue.ToString(), COD = c.COD.ToString(), DestinationCountry = c.DestinationCountry, DestinationLocation = c.DestinationLocation, DestinationCity = c.DestinationCity, ImportType = c.ImportType }).ToList();

            return Json(new { data = details });


        }
        [HttpPost]
        public ActionResult ShowImportDataFixation(string FieldName)
        {
            ImportManifestFixation vm = new ImportManifestFixation();
            ViewBag.ImportFields = db.ImportFields.ToList();
            return PartialView("ImportDataFixation", vm);
        }

        [HttpPost]
        public string UpdateImportedItem(string Details)
        {
            var IDetails = JsonConvert.DeserializeObject<List<ImportManifestItem>>(Details);
            Session["ManifestImported"] = IDetails;
            return "ok";
        }
        [HttpPost]
        public ActionResult UpdateDataFixation(string TargetColumn, string SourceValue, string TargetValue)
        {
            ImportManifestVM model = new ImportManifestVM();
            List<TranshipmentModel> Details = (List<TranshipmentModel>)Session["ManifestTranshipment"];
            if (TargetColumn == "DestinationCountry")
            {
                int i = 0;
                foreach (var item in Details)
                {
                    if (item.ConsigneeCountryName == SourceValue)
                    {
                        Details[i].ConsigneeCountryName = TargetValue;
                    }
                    i++;
                }
            }
            else if (TargetColumn == "DestinationCity")
            {
                int i = 0;
                foreach (var item in Details)
                {
                    if (item.ConsigneeCityName == SourceValue)
                    {
                        Details[i].ConsigneeCityName = TargetValue;
                    }
                    i++;
                }
            }
            else if (TargetColumn == "Customer")
            {
                int i = 0;
                foreach (var item in Details)
                {
                    if (item.Customer == SourceValue)
                    {
                        Details[i].Customer = TargetValue;
                    }
                    i++;
                }
            }
            else if (TargetColumn == "ForwardingAagent")
            {
                int i = 0;
                foreach (var item in Details)
                {
                    if (item.FAgentName == SourceValue)
                    {
                        Details[i].FAgentName = TargetValue;
                    }
                    i++;
                }
            }
            else if (TargetColumn == "ReceivedBy")
            {
                int i = 0;
                foreach (var item in Details)
                {
                    if (item.ReceivedBy == SourceValue)
                    {
                        Details[i].ReceivedBy = TargetValue;
                    }
                    i++;
                }
            }
            else if (TargetColumn == "CollectedBy")
            {
                int i = 0;
                foreach (var item in Details)
                {
                    if (item.CollectedBy == SourceValue)
                    {
                        Details[i].CollectedBy = TargetValue;
                    }
                    i++;
                }
            }
            SaveDataFixation(TargetColumn, SourceValue, TargetValue);
            model.TransDetails = Details;
            Session["ManifestTranshipment"] = Details;
            return View("ItemList", model);
        }

        /// <summary>
        /// this will udpate fixation in to importdatafixation table
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="SourceValue"></param>
        /// <param name="TargetValue"></param>
        public void SaveDataFixation(string FieldName, string SourceValue, string TargetValue)
        {
            ImportDataFixation importdata = new ImportDataFixation();
            var data = db.ImportDataFixations.Where(cc => cc.ShipmentType == "Transhipment" && cc.FieldName == FieldName && cc.SourceValue == SourceValue).FirstOrDefault();
            if (data == null)
            {
                importdata.ShipmentType = "Transhipment";
                importdata.FieldName = FieldName;
                importdata.SourceValue = SourceValue;
                importdata.TargetValue = TargetValue;
                importdata.UpdateDate = CommanFunctions.GetCurrentDateTime();
                db.ImportDataFixations.Add(importdata);
                db.SaveChanges();
            }
            else
            {                
                
                data.TargetValue = TargetValue;
                data.UpdateDate = CommanFunctions.GetCurrentDateTime();
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }


        }

        public string GetDataFixation(string FieldName, string SourceValue)
        {
            ImportDataFixation importdata = new ImportDataFixation();
            string Targetvalue = "";
            var data = db.ImportDataFixations.Where(cc => cc.ShipmentType == "Transhipment" && cc.FieldName == FieldName && cc.SourceValue == SourceValue).FirstOrDefault();
            if (data != null)
                Targetvalue = data.TargetValue;

            return Targetvalue;




        }

        [HttpPost]
        public string AutoDataFixation()
        {
            ImportManifestVM model = new ImportManifestVM();
            List<TranshipmentModel> Details = (List<TranshipmentModel>)Session["ManifestTranshipment"];
            DataTable dt = ToDataTable(Details);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var colname = dt.Columns[i].ColumnName;
                int rowindex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    string targetvalue = GetDataFixation(colname, row[colname].ToString());
                    if (targetvalue != "" && targetvalue!=null)
                    {
                        dt.Rows[rowindex][colname] = targetvalue;
                    }

                    rowindex++;
                }

            }
            List<TranshipmentModel> list = TranshipmentList(dt);
            Session["ManifestTranshipment"] = list;
            model.TransDetails = list;
            return "ok";
            

        }

        [HttpGet]
        public JsonResult GetSourceValue(string term, string FieldName)
        {
            var IDetails = (List<TranshipmentModel>)Session["ManifestTranshipment"];
            if (IDetails != null)
            {
                if (term.Trim() != "")
                {
                    if (FieldName == "DestinationCountry")
                    {
                        var list = (from c in IDetails
                                    where c.ConsigneeCountryName.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.ConsigneeCountryName
                                    select new { SourceValue = c.ConsigneeCountryName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationCity")
                    {
                        var list = (from c in IDetails
                                    where c.ConsigneeCityName.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.ConsigneeCityName
                                    select new { SourceValue = c.ConsigneeCityName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationLocation")
                    {
                        var list = (from c in IDetails
                                    where c.ConsigneeLocationName.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.ConsigneeLocationName
                                    select new { SourceValue = c.ConsigneeLocationName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "ForwardingAgent")
                    {
                        var list = (from c in IDetails
                                    where c.FAgentName.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.FAgentName
                                    select new { SourceValue = c.FAgentName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "Customer")
                    {
                        var list = (from c in IDetails
                                    where c.Customer.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.Customer
                                    select new { SourceValue = c.Customer }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "ReceivedBy")
                    {
                        var list = (from c in IDetails
                                    where c.ReceivedBy.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.ReceivedBy
                                    select new { SourceValue = c.ReceivedBy }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);

                    }
                    else if (FieldName == "CollectedBy")
                    {
                        var list = (from c in IDetails
                                    where c.CollectedBy.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.CollectedBy
                                    select new { SourceValue = c.CollectedBy }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "CourierStatus")
                    {
                        var list = (from c in IDetails
                                    where c.CourierStatus.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.CourierStatus
                                    select new { SourceValue = c.CourierStatus }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);

                    }
                    else if (FieldName == "CourierType")
                    {
                        var list = (from c in IDetails
                                    where c.CourierType.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.CourierType
                                    select new { SourceValue = c.CourierType}).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);

                    }
                    else if (FieldName == "ProductType")
                    {
                        var list = (from c in IDetails
                                    where c.ProductType.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.ProductType
                                    select new { SourceValue = c.ProductType }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        var list = new { SourceValue = "" };
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (FieldName == "DestinationCountry")
                    {
                        var list = (from c in IDetails
                                    orderby c.ConsigneeCountryName
                                    select new { SourceValue = c.ConsigneeCountryName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationCity")
                    {
                        var list = (from c in IDetails
                                    orderby c.ConsigneeCityName
                                    select new { SourceValue = c.ConsigneeCityName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationLocation")
                    {
                        var list = (from c in IDetails
                                    orderby c.ConsigneeLocationName
                                    select new { SourceValue = c.ConsigneeLocationName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "ForwardingAgent")
                    {
                        var list = (from c in IDetails
                                    orderby c.FAgentName
                                    select new { SourceValue = c.FAgentName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "Customer")
                    {
                        var list = (from c in IDetails
                                    orderby c.Customer
                                    select new { SourceValue = c.Customer }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "ReceivedBy")
                    {
                        var list = (from c in IDetails
                                    orderby c.ReceivedBy
                                    select new { SourceValue = c.ReceivedBy }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);

                    }
                    else if (FieldName == "CollectedBy") { 
                        var list = (from c in IDetails
                                    orderby c.CollectedBy
                                    select new { SourceValue = c.CollectedBy }).Distinct().ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);
                  }
                    else if(FieldName=="CourierStatus")
                    {
                        var list = (from c in IDetails
                                    orderby c.CourierStatus
                                    select new { SourceValue = c.CourierStatus }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);

                    }
                    else if (FieldName == "CourierType")
                    {
                        var list = (from c in IDetails                                    
                                    orderby c.CourierType
                                    select new { SourceValue = c.CourierType }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);

                    }
                    else if (FieldName == "ProductType")
                    {
                        var list = (from c in IDetails                                    
                                    orderby c.ProductType
                                    select new { SourceValue = c.ProductType }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        var list = new { SourceValue = "" };
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            else
            {
                var list = new { SourceValue = "" };
                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public JsonResult GetTargetValue(string term, string FieldName)
        {
            var IDetails = (List<TranshipmentModel>)Session["ManifestTranshipment"];
            if (IDetails != null)
            {
                if (term.Trim() != "")
                {
                    if (FieldName == "ReceivedBy" || FieldName=="CollectedBy")
                    {
                        var list = (from c in db.EmployeeMasters
                                    where c.EmployeeName.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.EmployeeName
                                    select new { SourceValue = c.EmployeeName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "CourierStatus")
                    {
                        var list = (from c in db.CourierStatus
                                    where c.CourierStatus.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.CourierStatus
                                    select new { SourceValue = c.CourierStatus }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "CourierType")
                    {
                        var list = (from c in db.CustomerRateTypes
                                    where c.CustomerRateType1.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.CustomerRateType1
                                    select new { SourceValue = c.CustomerRateType1 }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "ProductType")
                    {
                        var list = (from c in db.ProductTypes
                                    where c.ProductName.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.ProductName
                                    select new { SourceValue = c.ProductName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "ForwardingAgent")
                    {
                        var list = (from c in db.ForwardingAgentMasters
                                    where c.FAgentName.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.FAgentName
                                    select new { SourceValue = c.FAgentName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "Customer")
                    {
                        var list = (from c in db.CustomerMasters
                                    where c.CustomerName.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.CustomerName
                                    select new { SourceValue = c.CustomerName }).Distinct().ToList().Take(100);
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var list = new { SourceValue = "" };
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (FieldName == "ReceivedBy" || FieldName == "CollectedBy")
                    {
                        var list = (from c in db.EmployeeMasters                                    
                                    orderby c.EmployeeName
                                    select new { SourceValue = c.EmployeeName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "CourierStatus")
                    {
                        var list = (from c in db.CourierStatus                                    
                                    orderby c.CourierStatus
                                    select new { SourceValue = c.CourierStatus }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "ForwardingAgent")
                    {
                        var list = (from c in db.ForwardingAgentMasters
                                    orderby c.FAgentName
                                    select new { SourceValue = c.FAgentName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "Customer")
                    {
                        var list = (from c in db.CustomerMasters
                                    orderby c.CustomerName
                                    select new { SourceValue = c.CustomerName }).Distinct().ToList().Take(100);
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "CourierType")
                    {
                        var list = (from c in db.CustomerRateTypes                                    
                                    orderby c.CustomerRateType1
                                    select new { SourceValue = c.CustomerRateType1 }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "ProductType")
                    {
                        var list = (from c in db.ProductTypes
                                    
                                    orderby c.ProductName
                                    select new { SourceValue = c.ProductName }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var list = new { SourceValue = "" };
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            else
            {
                var list = new { SourceValue = "" };
                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public string GetManifestNo(string ManifestDateTime)
        {
            var CompanyID = Convert.ToInt32(Session["CurrentCompanyID"]);
            var BranchID = Convert.ToInt32(Session["CurrentBranchID"]);
            DateTime mDateTime = CommanFunctions.GetCurrentDateTime();
            try
            {
                mDateTime = Convert.ToDateTime(ManifestDateTime);
            }
            catch (Exception ex)
            {
                mDateTime = CommanFunctions.GetCurrentDateTime();
            }

            string manifestno = ImportDAO.GetMaxManifestNo(CompanyID, BranchID, mDateTime, "T");
            return manifestno;

        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static List<TranshipmentModel> TranshipmentList(DataTable dt)
        {
            var empList = new List<TranshipmentModel>();
            int i = 0;
            foreach (DataRow objDataRow in dt.Rows)
            {
                empList.Add(new TranshipmentModel()
                {
                    SNo = i++,
                    HAWBNo = objDataRow["HAWBNo"].ToString(),
                    AWBDate = objDataRow["AWBDate"].ToString(),
                    Customer = objDataRow["Customer"].ToString(),
                    ConsignorPhone = objDataRow["ConsignorPhone"].ToString(),
                    Consignor = objDataRow["Consignor"].ToString(),
                    ConsignorLocationName = objDataRow["ConsignorLocationName"].ToString(),
                    ConsignorCountryName = objDataRow["ConsignorCountryName"].ToString(),
                    ConsignorCityName = objDataRow["ConsignorCityName"].ToString(),
                    Consignee = objDataRow["Consignee"].ToString(),
                    ConsigneeCountryName = objDataRow["ConsigneeCountryName"].ToString(),
                    ConsigneeCityName = objDataRow["ConsigneeCityName"].ToString(),
                    ConsigneeLocationName = objDataRow["ConsigneeLocationName"].ToString(),
                    ConsignorAddress1_Building = objDataRow["ConsignorAddress1_Building"].ToString(),
                    ConsignorMobile = objDataRow["ConsignorMobile"].ToString(),
                    ConsigneeMobile = objDataRow["ConsigneeMobile"].ToString(),
                    Weight = CommanFunctions.ParseDecimal(objDataRow["Weight"].ToString()),
                    Pieces = objDataRow["Pieces"].ToString(),
                    CourierCharge = CommanFunctions.ParseDecimal(objDataRow["CourierCharge"].ToString()),
                    OtherCharge = CommanFunctions.ParseDecimal(objDataRow["OtherCharge"].ToString()),
                    PaymentMode = objDataRow["PaymentMode"].ToString(),
                    ReceivedBy = objDataRow["ReceivedBy"].ToString(),
                    CollectedBy = objDataRow["CollectedBy"].ToString(),
                    FAWBNo = objDataRow["FAWBNo"].ToString(),
                    FAgentName = objDataRow["FAgentName"].ToString(),
                    CourierType = objDataRow["CourierType"].ToString(),
                    MovementType = objDataRow["MovementType"].ToString(),
                    CourierStatus = objDataRow["CourierStatus"].ToString(),
                    remarks = objDataRow["remarks"].ToString() //Department and Bag no is missing                                                               


                });
            }
            return empList;
        }

    }

        public class DataObject
        {
            public int code { get; set; }
            public List<productdata> data { get; set; }
            public string message { get; set; }

        }

        public class productdata
        {
            public string shipper { get; set; }
            public string pcs { get; set; }
            public string awbNo { get; set; }

            public string receiverName { get; set; }

            public string customsValue { get; set; }

            public string awbDate { get; set; }
            public string destination { get; set; }

            public string weight { get; set; }
            public string lastStatusRemark { get; set; }

            public string destinationCountry { get; set; }
            public string content { get; set; }
            public string reference { get; set; }

            public string destinationCity { get; set; }

            public string receiverAddress { get; set; }
            public string receiverPhone { get; set; }

            public string cod { get; set; }
            public string currency { get; set; }

            public string id { get; set; }

            public string bagNo { get; set; }

            public string status { get; set; }
        }
    
}