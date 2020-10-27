using CMSV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace  CMSV2.Controllers
{
    [SessionExpire]
    public class AutoCompleteController : Controller
    {
        //
        // GET: /AutoComplete/

        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Supplier (string term)
        //{
        //    MastersModel MM = new MastersModel();
        //    if (!String.IsNullOrEmpty(term))
        //    {
        //        List<SP_GetAllSupplier_Result> Supplier = new List<SP_GetAllSupplier_Result>();
        //        Supplier = MM.GetAllSupplier(term);
        //        return Json(Supplier, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        List<SP_GetAllSupplier_Result> Supplier = new List<SP_GetAllSupplier_Result>();
        //        Supplier = MM.GetAllSupplier();
        //        return Json(Supplier, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public ActionResult SupplierById(int Id)
        //{
        //    MastersModel MM = new MastersModel();
        //        List<SP_GetAllSupplier_Result> Supplier = new List<SP_GetAllSupplier_Result>();
        //        Supplier = MM.GetSupplierById(Id);
        //        return Json(Supplier, JsonRequestBehavior.AllowGet);   
        //}
        public ActionResult Currency(string term)
        {
            MastersModel MM = new MastersModel();
            if (!String.IsNullOrEmpty(term))
            {
                List<SP_GetCurrency_Result> CurrencyList = new List<SP_GetCurrency_Result>();
                CurrencyList = MM.GetCurrency(term);
                return Json(CurrencyList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<SP_GetCurrency_Result> CurrencyList = new List<SP_GetCurrency_Result>();
                CurrencyList = MM.GetCurrency();
                return Json(CurrencyList, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CurrencyById(int Id)
        {
            MastersModel MM = new MastersModel();
            List<SP_GetCurrency_Result> CurrencyList = new List<SP_GetCurrency_Result>();
            CurrencyList = MM.GetCurrencyById(Id);
            return Json(CurrencyList, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Employee(string term)
        //{
        //    MastersModel MM = new MastersModel();
        //    if (!String.IsNullOrEmpty(term))
        //    {
        //        List<SP_GetAllEmployees_Result> EmployeeList = new List<SP_GetAllEmployees_Result>();
        //        EmployeeList = MM.GetAllEmployees(term);
        //        return Json(EmployeeList, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        List<SP_GetAllEmployees_Result> EmployeeList = new List<SP_GetAllEmployees_Result>();
        //        EmployeeList = MM.GetAllEmployees();
        //        return Json(EmployeeList, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public ActionResult EmployeeById(int Id)
        //{
        //    MastersModel MM = new MastersModel();
        //    List<SP_GetAllEmployees_Result> EmployeeList = new List<SP_GetAllEmployees_Result>();
        //    EmployeeList = MM.GetEmployeeById(Id);
        //    return Json(EmployeeList, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Customer(string term)
        //{
        //    MastersModel MM = new MastersModel();
        //    if (!String.IsNullOrEmpty(term))
        //    {
        //        List<SP_GetAllCustomers_Result> CustomerList = new List<SP_GetAllCustomers_Result>();
        //        CustomerList = MM.GetAllCustomer(term);
        //        return Json(CustomerList, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        List<SP_GetAllCustomers_Result> CustomerList = new List<SP_GetAllCustomers_Result>();
        //        CustomerList = MM.GetAllCustomer();
        //        return Json(CustomerList, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public ActionResult CustomerById(int Id)
        //{
        //    MastersModel MM = new MastersModel();
        //    List<SP_GetAllCustomers_Result> CustomerList = new List<SP_GetAllCustomers_Result>();
        //    CustomerList = MM.GetCustomerById(Id);
        //    return Json(CustomerList, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Ports(string term)
        //{
        //    MastersModel MM = new MastersModel();
        //    if (!String.IsNullOrEmpty(term))
        //    {
        //        List<SP_GetAllPorts_Result> PortsList = new List<SP_GetAllPorts_Result>();
        //        PortsList = MM.GetAllPorts(term);
        //        return Json(PortsList, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        List<SP_GetAllPorts_Result> PortsList = new List<SP_GetAllPorts_Result>();
        //        PortsList = MM.GetAllPort();
        //        return Json(PortsList, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public ActionResult PortById(int Id)
        //{
        //    MastersModel MM = new MastersModel();
        //    List<SP_GetAllPorts_Result> PortList = new List<SP_GetAllPorts_Result>();
        //    PortList = MM.GetPortById(Id);
        //    return Json(PortList, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult AccountHead(string term)
        //{
        //    MastersModel MM = new MastersModel();
        //    if (!String.IsNullOrEmpty(term))
        //    {
        //        List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
        //        AccountHeadList = MM.AcHeadSelectAll(Common.ParseInt(Session["CurrentBranchID"].ToString()),term);
        //        return Json(AccountHeadList, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
        //        AccountHeadList = MM.AcHeadSelectAll(Common.ParseInt(Session["branchid"].ToString()));
        //        return Json(AccountHeadList, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult AcHeadById(int Id)
        {
            MastersModel MM = new MastersModel();
            List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
            AccountHeadList = MM.AcHeadById(Common.ParseInt(Session["branchid"].ToString()),Id);
            return Json(AccountHeadList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Job(string term)
        {
            MastersModel MM = new MastersModel();
            if (!String.IsNullOrEmpty(term))
            {
                List<SP_GetAllJobsDetails_Result> JobList = new List<SP_GetAllJobsDetails_Result>();
                JobList = DAL.GetAllJobsDetails(term);
                return Json(JobList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<SP_GetAllJobsDetails_Result> JobList = new List<SP_GetAllJobsDetails_Result>();
                JobList = MM.GetAllJobsDetails();
                return Json(JobList, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult JobById(int Id)
        {
            MastersModel MM = new MastersModel();
            List<SP_GetAllJobsDetails_Result> JobList = new List<SP_GetAllJobsDetails_Result>();
            JobList = MM.JobById(Id);
            return Json(JobList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Products(string term)
        {
            MastersModel MM = new MastersModel();
            if (!String.IsNullOrEmpty(term))
            {
                List<SP_GetAllProductServices_Result> ProductList = new List<SP_GetAllProductServices_Result>();
                ProductList = MM.GetAllProductServices(term);
                return Json(ProductList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<SP_GetAllProductServices_Result> ProductList = new List<SP_GetAllProductServices_Result>();
                ProductList = MM.GetAllProductServices();
                return Json(ProductList, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ProductById(int Id)
        {
            MastersModel MM = new MastersModel();
            List<SP_GetAllProductServices_Result> ProductList = new List<SP_GetAllProductServices_Result>();
            ProductList = MM.ProductById(Id);
            return Json(ProductList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AnalysisHeadSelectAll(string term)
        {
            MastersModel MM = new MastersModel();
            if (!String.IsNullOrEmpty(term))
            {
                List<AnalysisHeadSelectAll_Result> AnalysisHeadSelectList = new List<AnalysisHeadSelectAll_Result>();
                AnalysisHeadSelectList = MM.GetAnalysisHeadSelectList(Common.ParseInt(Session["AcCompanyID"].ToString()),term);
                return Json(AnalysisHeadSelectList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<AnalysisHeadSelectAll_Result> AnalysisHeadSelectList = new List<AnalysisHeadSelectAll_Result>();
                AnalysisHeadSelectList = MM.GetAnalysisHeadSelectList(Common.ParseInt(Session["AcCompanyID"].ToString()),"");
                return Json(AnalysisHeadSelectList, JsonRequestBehavior.AllowGet);
            }
        }       
    }
}
