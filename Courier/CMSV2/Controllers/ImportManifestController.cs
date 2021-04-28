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
        public ActionResult Index(AWBBatchSearch obj)
        {
            Session["ImportManifestSearch"] = obj;
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            var userid = Convert.ToInt32(Session["UserID"]);
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
            //ViewBag.AgentName = agent.Name;
            //ViewBag.AgentCity = agent.CityName;
            ViewBag.CompanyName = company.AcCompany1;
            ImportShipmentFormModel vm = new ImportShipmentFormModel();            
            vm.ManifestNumber = ImportDAO.GetMaxManifestNo(1, 1);
            vm.CreatedDate = CommanFunctions.GetCurrentDateTime();
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
                return Json(new { Status = 1,  data=fileData, Message = "File Imported Successfully " });
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
                                Bag = Convert.ToInt32(objDataRow["Bag"].ToString()),
                                Bill = objDataRow["Bill"].ToString(),
                                Rfnc = objDataRow["Rfnc"].ToString()                                
                            });
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

    }
}