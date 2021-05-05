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

namespace CMSV2.Controllers
{
    [SessionExpire]
    public class ImportManifestController : Controller
    {
        // GET: ImportManifest
        Entities1 db = new Entities1();
        public ActionResult Index()
        {

            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            ImportManifestSearch obj = (ImportManifestSearch)Session["ImportManifestSearch"];
            ImportManifestSearch model = new ImportManifestSearch();
            AWBDAO _dao = new AWBDAO();
            if (obj != null)
            {
                List<ImportManifestVM> translist = new List<ImportManifestVM>();
                translist = ImportDAO.GetImportManifestList();
                model.FromDate = obj.FromDate;
                model.ToDate = obj.ToDate;
                model.Details = translist;
            }
            else
            {
                model.FromDate = CommanFunctions.GetLastDayofMonth().Date;
                model.ToDate = CommanFunctions.GetLastDayofMonth().Date;
                Session["ImportManifestSearch"] = model;
                List<ImportManifestVM> translist = new List<ImportManifestVM>();
                translist = ImportDAO.GetImportManifestList();
                model.Details = translist;

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ImportManifestSearch obj)
        {
            Session["ImportManifestSearch"] = obj;
            return RedirectToAction("Index");
        }

        public ActionResult Create(int id = 0)
        {
            var userid = Convert.ToInt32(Session["UserID"]);
            var CompanyID = Convert.ToInt32(Session["CurrentCompanyID"]);

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
            string CompanyCountryName = db.AcCompanies.Find(CompanyID).CountryName;
            //ViewBag.AgentName = agent.Name;
            //ViewBag.AgentCity = agent.CityName;
            ViewBag.CompanyName = company.AcCompany1;
            ImportManifestVM vm = new ImportManifestVM();
            if (id == 0)
            {
                vm.CompanyCountryName = CompanyCountryName;
                vm.ManifestNumber = ImportDAO.GetMaxManifestNo(1, 1);
                vm.ManifestDate = CommanFunctions.GetCurrentDateTime().ToString();
                vm.ID = 0;
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

            }
            return View(vm);
        }


        [HttpPost]
        public JsonResult ImportFile(HttpPostedFileBase importFile)
        {
            if (importFile == null) return Json(new { Status = 0, Message = "No File Selected" });

            try
            {
                var fileData = GetDataFromCSVFile(importFile.InputStream);

                //var dtEmployee = fileData.ToDataTable();
                //var tblEmployeeParameter = new SqlParameter("tblEmployeeTableType", SqlDbType.Structured)
                //{
                //    TypeName = "dbo.tblTypeEmployee",
                //    Value = dtEmployee
                //};
                //await db.Database.ExecuteSqlCommandAsync("EXEC spBulkImportEmployee @tblEmployeeTableType", tblEmployeeParameter);
                return Json(new { Status = 1, data = fileData, Message = "File Imported Successfully " });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message });
            }
        }
        private List<ImportManifestItem> GetDataFromCSVFile(Stream stream)
        {
            var empList = new List<ImportManifestItem>();
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
                        foreach (DataRow objDataRow in dataTable.Rows)
                        {
                            if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                            empList.Add(new ImportManifestItem()
                            {
                                Bag = objDataRow["BagNo"].ToString(),
                                AWBNo = objDataRow["AWBNo"].ToString(),
                                AWBDate = objDataRow["AWBDate"].ToString(),
                                Shipper = objDataRow["Shipper"].ToString(),
                                Receiver = objDataRow["ReceiverName"].ToString(),
                                ReceiverContact = objDataRow["ReceiverContactName"].ToString(),
                                ReceiverPhone = objDataRow["ReceiverPhone"].ToString(),
                                ReceiverAddress = objDataRow["ReceiverAddress"].ToString(),
                                DestinationLocation = objDataRow["DestinationLocation"].ToString(),
                                DestinationCity = objDataRow["DestinationCity"].ToString(),
                                DestinationCountry = objDataRow["DestinationCountry"].ToString(),
                                Pcs = objDataRow["Pcs"].ToString(),
                                Weight = objDataRow["Weight"].ToString(),
                                Value = objDataRow["CustomsValue"].ToString(),
                                COD = objDataRow["COD"].ToString(),
                                Content = objDataRow["Content"].ToString(),
                                ImportType = "Import"
                                // Reference = objDataRow["Content"].ToString(),



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
                var IDetails = JsonConvert.DeserializeObject<List<ImportManifestItem>>(Details);
                ImportShipment importShipment = new ImportShipment();
                if (model.ID == 0)
                {
                    importShipment.ManifestNumber = model.ManifestNumber;
                    importShipment.CreatedDate = Convert.ToDateTime(model.ManifestDate);
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

                    var details = (from d in db.ImportShipmentDetails where d.ImportID == model.ID select d).ToList();
                    db.ImportShipmentDetails.RemoveRange(details);
                    db.SaveChanges();

                }

                foreach (var item in IDetails)
                {
                    ImportShipmentDetail detail = new ImportShipmentDetail();
                    detail.ImportID = importShipment.ID;
                    detail.AWB = item.AWBNo;
                    detail.AWBDate = Convert.ToDateTime(item.AWBDate);
                    detail.Receiver = item.Receiver;
                    detail.ReceiverAddress = item.ReceiverAddress;
                    detail.ReceiverContact = item.ReceiverContact;
                    detail.ReceiverTelephone = item.ReceiverPhone;
                    detail.DestinationCountry = item.DestinationCountry;
                    detail.DestinationCity = item.DestinationCity;
                    detail.DestinationLocation = item.DestinationLocation;
                    detail.ImportType = item.ImportType;
                    if (item.ImportType == "Import")
                    {
                        detail.StatusTypeId = 9;
                        detail.CourierStatusID = 20;
                    }
                    else
                    {
                        detail.StatusTypeId = 2;
                        detail.CourierStatusID = 21;
                    }
                    detail.Shipper = item.Shipper;
                    detail.Contents = item.Content;
                    if (item.Pcs != "")
                        detail.PCS = Convert.ToInt32(item.Pcs);

                    if (item.Weight != "")
                        detail.Weight = Convert.ToDecimal(item.Weight);

                    if (item.COD != "")
                        detail.COD = Convert.ToDecimal(item.COD);
                    detail.BagNo = item.Bag;
                    detail.CustomValue = Convert.ToDecimal(item.Value);
                    detail.CurrencyID = 1;
                    db.ImportShipmentDetails.Add(detail);
                    db.SaveChanges();


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
            details = (from c in db.ImportShipmentDetails where c.ImportID == id select new ImportManifestItem { AWBNo = c.AWB, AWBDate = c.AWBDate.ToString(), Shipper = c.Shipper, Receiver = c.Receiver, ReceiverContact = c.ReceiverContact, ReceiverAddress = c.ReceiverAddress, ReceiverPhone = c.ReceiverTelephone, Content = c.Contents, Bag = c.BagNo.ToString(), Weight = c.Weight.ToString(), Pcs = c.PCS.ToString(), Value = c.CustomValue.ToString(), COD = c.COD.ToString(), DestinationCountry = c.DestinationCountry, DestinationLocation = c.DestinationLocation, DestinationCity = c.DestinationCity, ImportType = c.ImportType }).ToList();

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

        [HttpGet]
        public JsonResult GetSourceValue(string term, string FieldName)
        {
            var IDetails = (List<ImportManifestItem>)Session["ManifestImported"];
            if (IDetails != null)
            {
                if (term.Trim() != "")
                {
                    if (FieldName == "DestinationCountry")
                    {
                        var list = (from c in IDetails
                                    where c.DestinationCountry.Contains(term)
                                    orderby c.DestinationCountry
                                    select new { SourceValue = c.DestinationCountry }).ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationCity")
                    {
                        var list = (from c in IDetails
                                    where c.DestinationCity.Contains(term)
                                    orderby c.DestinationCity
                                    select new { SourceValue = c.DestinationCity }).ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationLocation")
                    {
                        var list = (from c in IDetails
                                    where c.DestinationLocation.Contains(term)
                                    orderby c.DestinationLocation
                                    select new { SourceValue = c.DestinationLocation }).ToList();
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
                                    orderby c.DestinationCountry
                                    select new { SourceValue = c.DestinationCountry }).ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationCity")
                    {
                        var list = (from c in IDetails                                    
                                    orderby c.DestinationCity
                                    select new { SourceValue = c.DestinationCity }).ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationLocation")
                    {
                        var list = (from c in IDetails                                    
                                    orderby c.DestinationLocation
                                    select new { SourceValue = c.DestinationLocation }).ToList();
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
    }
}