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
                translist = ImportDAO.GetImportManifestList(3);
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
                translist = ImportDAO.GetImportManifestList(3);
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

        public ActionResult Create(int id = 0,string InputDate="")
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
            vm.Details = new List<ImportManifestItem>();
            if (id == 0)
            {
                vm.CompanyCountryName = CompanyCountryName;
                vm.ManifestDate = CommanFunctions.GetCurrentDateTime().ToString();
                vm.ManifestNumber = ImportDAO.GetMaxManifestNo(CompanyID, BranchID, Convert.ToDateTime(vm.ManifestDate), "I");                
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
                var IDetails = (from c in db.ImportShipmentDetails where c.ImportID == vm.ID select new ImportManifestItem { AWBNo = c.AWB,   Shipper = c.Shipper, Bag = c.BagNo, COD = c.COD.ToString(), Content = c.Contents, DestinationCity = c.DestinationCity, DestinationCountry = c.DestinationCountry, Pcs = c.PCS, Receiver = c.Receiver, ReceiverAddress = c.ReceiverAddress, Value = c.CustomValue.ToString() }).ToList();

                Session["ManifestImported"] = IDetails;
                vm.Details = IDetails;

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
                                Pcs = CommanFunctions.ParseInt(objDataRow["Pcs"].ToString()),
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
        public JsonResult SaveImport(string Master, string Details)
        {
            try
            {
                var bills = new List<updateitem>();
                var userid = Convert.ToInt32(Session["UserID"]);
                var model = JsonConvert.DeserializeObject<ImportManifestVM>(Master);
                //var IDetails = JsonConvert.DeserializeObject<List<ImportManifestItem>>(Details);
                var IDetails = (List<ImportManifestItem>)Session["ManifestImported"];
                ImportShipment importShipment = new ImportShipment();
                if (model.ID == 0)
                {
                    importShipment.ManifestNumber = model.ManifestNumber;
                    importShipment.CreatedDate = Convert.ToDateTime(model.ManifestDate);
                    importShipment.ShipmentTypeId = 3;
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
                    updateitem uitem = new updateitem();
                    uitem.AWBNo = item.AWBNo;
                    uitem.synchronisedDateTime = Convert.ToDateTime(model.ManifestDate).ToString("dd-MMM-yyyy HH:mm");
                    bills.Add(uitem);
                    ImportShipmentDetail detail = new ImportShipmentDetail();
                    detail.ImportID = importShipment.ID;
                    detail.AWB = item.AWBNo;
                    detail.AWBDate = Convert.ToDateTime(item.AWBDate);
                    detail.Shipper = item.Shipper;
                    detail.Contents = item.Content;
                    detail.Receiver = item.Receiver;
                    detail.ReceiverAddress = item.ReceiverAddress;
                    detail.ReceiverContact = item.ReceiverContact;
                    detail.ReceiverTelephone = item.ReceiverPhone;
                    detail.DestinationCountry = item.DestinationCountry;
                    detail.DestinationCity = item.DestinationCity;
                    detail.DestinationLocation = item.DestinationLocation;
                    detail.ImportType = "Import";
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
                    
                    if (item.Pcs != null)
                        detail.PCS = Convert.ToInt32(item.Pcs);

                    if (item.Weight != "")
                        detail.Weight = Convert.ToDecimal(item.Weight);

                    if (item.COD != "")
                        detail.COD = Convert.ToDecimal(item.COD);
                    detail.BagNo = item.Bag;
                    detail.CustomValue = Convert.ToDecimal(item.Value);
                    var currency = db.CurrencyMasters.Where(cc => cc.CurrencyName == item.Currency).FirstOrDefault();
                    if (currency != null)
                        detail.CurrencyID = currency.CurrencyID;
                    else
                        detail.CurrencyID = 1;

                    db.ImportShipmentDetails.Add(detail);
                    db.SaveChanges();


                }
                postbill postbill = new postbill();
                postbill.bills = bills;
                Session["bills"] = postbill;
                var json = JsonConvert.SerializeObject(bills);
                // CallPostAPI(model.ManifestDate.ToString());
                return Json(new { status="ok", data =bills });
            }
            catch (Exception ex)
            {
                //return ex.Message;
                return Json(new { status = "Failed", Message=ex.Message });

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
                                    where c.DestinationCountry.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.DestinationCountry
                                    select new { SourceValue = c.DestinationCountry }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationCity")
                    {
                        var list = (from c in IDetails
                                    where c.DestinationCity.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.DestinationCity
                                    select new { SourceValue = c.DestinationCity }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationLocation")
                    {
                        var list = (from c in IDetails
                                    where c.DestinationLocation.ToLower().Contains(term.Trim().ToLower())
                                    orderby c.DestinationLocation
                                    select new { SourceValue = c.DestinationLocation }).Distinct().ToList();
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
                                    select new { SourceValue = c.DestinationCountry }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationCity")
                    {
                        var list = (from c in IDetails
                                    orderby c.DestinationCity
                                    select new { SourceValue = c.DestinationCity }).Distinct().ToList();
                        return Json(list, JsonRequestBehavior.AllowGet);
                    }
                    else if (FieldName == "DestinationLocation")
                    {
                        var list = (from c in IDetails
                                    orderby c.DestinationLocation
                                    select new { SourceValue = c.DestinationLocation }).Distinct().ToList();
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
        
        [HttpPost]
        public ActionResult UpdateDataFixation(string TargetColumn,string SourceValue,string TargetValue)
        {
            ImportManifestVM model = new ImportManifestVM();
            List<ImportManifestItem> Details = (List<ImportManifestItem>)Session["ManifestImported"];
            if (TargetColumn== "DestinationCountry")
            {
                int i = 0;
                foreach(var item in Details)
                {
                    if (item.DestinationCountry == SourceValue)
                    {
                        Details[i].DestinationCountry = TargetValue;
                    }
                    i++;
                }
            }
            else if (TargetColumn == "DestinationCity")
            {
                int i = 0;
                foreach (var item in Details)
                {
                    if (item.DestinationCity == SourceValue)
                    {
                        Details[i].DestinationCity = TargetValue;
                    }
                    i++;
                }
            }
            else if (TargetColumn == "DestinationLocation")
            {
                int i = 0;
                foreach (var item in Details)
                {
                    if (item.DestinationLocation == SourceValue)
                    {
                        Details[i].DestinationLocation= TargetValue;
                    }
                    i++;
                }
            }

            model.Details = Details;
            Session["ManifestImported"] = Details;
            return View("ItemList",model);
        }

        [HttpPost]
        public async Task<ActionResult> CallPostAPI()
        {
            string URL = "http://www.niceexpress.net/API/v1/postAPI.do";
            //string idate = Convert.ToDateTime(InputDate).ToString("dd-MMM-yyyy hh:mm");
            
            postbill bills = (postbill)Session["bills"];            
            //var json = JsonConvert.SerializeObject(bills);
            //string urlParameters = "?bills=" + json;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                //HTTP GET
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("niceexpress-apikey", "14c3993c0bb082dcafae9183ac7946d5be7a0e565e12bbd32f5d8d5d78bf3121");
                client.DefaultRequestHeaders.Add("niceexpress-signature", "37ef0bccb326b2057121ab74fd81cbeee892debaeccdd632d57fff66ffd86ece");

                // List data response.
                //var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                //var result  = await client.PostAsync("method",content);  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                //string resultContent = await result.Content.ReadAsStringAsync();
                var postResponse = await client.PostAsJsonAsync(URL, bills);
                var response=postResponse.EnsureSuccessStatusCode();
                //if (response.IsSuccessStatusCode)
                //{
                //    // Parse the response body.

                //    var ItemJsonString = await response.Content.ReadAsStringAsync(); // .ReadAsAsync<MasterDataObject>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                //    var dataObjects = JsonConvert.DeserializeObject<MasterDataObject>(ItemJsonString);
                //    // return RedirectToAction("Index");
                //    return Json(new { Status = "ok",  Message = "API Updated Successfully " });
                //}
                //else
                //{
                //    return Json(new { Status = "Failed", Message = "API Not Updated Successfully " });
                //}
                
                return Json(new { Status = "ok", Message = "API Updated Successfully " }, JsonRequestBehavior.AllowGet);
            }
            //return "notworked";
        }

        public async Task<ActionResult> SaveItemList(string InputDate)
        {
            ImportManifestVM vm = new ImportManifestVM();
            vm.Details = new List<ImportManifestItem>();
            string URL = "http://www.niceexpress.net/API/v1/getAPI.do";
            string idate = Convert.ToDateTime(InputDate).ToString("dd-MMM-yyyy");
            string urlParameters = "?dateTime=" + idate;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                //HTTP GET
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("niceexpress-apikey", "14c3993c0bb082dcafae9183ac7946d5be7a0e565e12bbd32f5d8d5d78bf3121");
                client.DefaultRequestHeaders.Add("niceexpress-signature", "37ef0bccb326b2057121ab74fd81cbeee892debaeccdd632d57fff66ffd86ece");

                // List data response.
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.

                    var ItemJsonString = await response.Content.ReadAsStringAsync(); // .ReadAsAsync<MasterDataObject>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                    var dataObjects = JsonConvert.DeserializeObject<MasterDataObject>(ItemJsonString);
                    int sno = 0;
                    foreach (Importproductdata d in dataObjects.data)
                    {
                        ImportManifestItem item = new ImportManifestItem();
                        sno++;
                        item.Sno = sno;
                        item.AWBNo = d.awbNo;
                        item.AWBDate = d.awbDate;
                        item.Bag = d.bagNo;
                        if (d.cod == "")
                            item.COD = "0";
                        else
                            item.COD = d.cod;
                        item.Weight = d.weight;
                        item.ImportType = d.reference;
                        item.Shipper = d.shipper;
                        item.ShipperPhone = d.cnorTel;
                        item.Content = d.content;
                        item.DestinationCountry = d.destination;
                        if (d.destinationCity == "")
                            item.DestinationCity = d.destination;
                        else
                            item.DestinationCity = d.destinationCity;
                        item.DestinationLocation = d.receiverAddress;
                        if (d.receiverAddress.IndexOf(':') > 0)
                        {
                            string contact = d.receiverAddress.Split(':')[0];
                            string address = d.receiverAddress.Split(':')[1];
                            item.ReceiverContact = contact;
                            item.ReceiverAddress = address;
                        }
                        else
                        {
                            item.ReceiverContact = "";
                            item.ReceiverAddress = d.receiverAddress;
                        }
                        item.Pcs =CommanFunctions.ParseInt(d.pcs);
                        item.Receiver = d.receiverName;


                        item.ReceiverPhone = d.receiverPhone;

                        item.Currency = d.currency;
                        item.Value = d.customsValue;
                        vm.Details.Add(item);
                        // Console.WriteLine("{0}", d.awbNo);
                    }
                    Session["ManifestImported"] = vm.Details;
                    return PartialView("ItemList", vm);
                    //return View(vm);
                    //return Json(new { data = "ok"});
                }
                else
                {
                    //return View(vm);
                    return PartialView("ItemList", vm);
                    //return Json(new { data = response.StatusCode });
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
            }
        }
        public async Task<ActionResult> ShowItemList(string InputDate) 
        {
            ImportManifestVM vm = new ImportManifestVM();
            vm.Details = new List<ImportManifestItem>();
            string URL = "http://www.niceexpress.net/API/v1/getAPI.do";
            string idate = Convert.ToDateTime(InputDate).ToString("dd-MMM-yyyy");
            string urlParameters = "?dateTime=" + idate;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                //HTTP GET
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("niceexpress-apikey", "14c3993c0bb082dcafae9183ac7946d5be7a0e565e12bbd32f5d8d5d78bf3121");
                client.DefaultRequestHeaders.Add("niceexpress-signature", "37ef0bccb326b2057121ab74fd81cbeee892debaeccdd632d57fff66ffd86ece");

                // List data response.
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.

                    var ItemJsonString = await response.Content.ReadAsStringAsync(); // .ReadAsAsync<MasterDataObject>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                    var dataObjects = JsonConvert.DeserializeObject<MasterDataObject>(ItemJsonString);
                    int sno = 0;
                    foreach (Importproductdata d in dataObjects.data)
                    {
                        ImportManifestItem item = new ImportManifestItem();
                        sno++;
                        item.Sno = sno;
                        item.AWBNo = d.awbNo;
                        item.AWBDate = d.awbDate;
                        item.Bag = d.bagNo;
                        if (d.cod == "")
                            item.COD ="0";
                        else
                            item.COD = d.cod;
                        item.Weight = d.weight;
                        item.ImportType = d.reference;
                        item.Shipper = d.shipper;
                        item.ShipperPhone = d.cnorTel;
                        item.Content = d.content;
                        item.DestinationCountry = d.destination;
                        if (d.destinationCity == "")
                            item.DestinationCity = d.destination;
                        else
                        item.DestinationCity = d.destinationCity;
                        item.DestinationLocation = d.receiverAddress;
                        if (d.receiverAddress.IndexOf(':')>0)
                        {
                            string contact = d.receiverAddress.Split(':')[0];
                            string address = d.receiverAddress.Split(':')[1];
                            item.ReceiverContact = contact;
                            item.ReceiverAddress = address;
                        }
                        else
                        {
                            item.ReceiverContact = "";
                            item.ReceiverAddress = d.receiverAddress;
                        }
                        item.Pcs = CommanFunctions.ParseInt(d.pcs);
                        item.Receiver = d.receiverName;
                        
                        
                        item.ReceiverPhone = d.receiverPhone;

                        item.Currency = d.currency;
                        item.Value = d.customsValue;
                        vm.Details.Add(item);
                       // Console.WriteLine("{0}", d.awbNo);
                    }
                    Session["ManifestImported"] = vm.Details;
                    return PartialView("ItemList",vm);
                    //return View(vm);
                    //return Json(new { data = "ok"});
                }
                else
                {
                    //return View(vm);
                    return PartialView("ItemList", vm);
                    //return Json(new { data = response.StatusCode });
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
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
            catch(Exception ex)
            {
                mDateTime = CommanFunctions.GetCurrentDateTime();
            }
             
            string manifestno = ImportDAO.GetMaxManifestNo(CompanyID, BranchID, mDateTime, "I");
            return manifestno;

        }
    }


    public class MasterDataObject
        {
          public int code { get; set; }
        public List<Importproductdata> data { get; set; }
        public string message { get; set; }

        }

    public class Importproductdata
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
        public string cnorTel { get; set; }
        public string cod { get; set; }
        public string currency { get; set; }

        public string id { get; set; }

        public string bagNo { get; set; }

        public string status { get; set; }
    }

    public class postbill
    {
        public List<updateitem> bills { get; set; }
    }
    public class updateitem
    {
        public string AWBNo { get; set; }
        public string synchronisedDateTime { get; set; }
     }
}