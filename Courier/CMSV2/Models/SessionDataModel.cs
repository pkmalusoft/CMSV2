using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using CMSV2.Models;
using CMSV2.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public static class SessionDataModel
    {
        public static void SetCurrency(CurrencyMaster currency)
        {
            System.Web.HttpContext.Current.Session["Currency"] = currency;
        }
        public static CurrencyMaster GetCurrency()
        {
            return (CurrencyMaster)System.Web.HttpContext.Current.Session["Currency"];
        }
        public static void ClearTableVariable()
        {
            System.Web.HttpContext.Current.Session["TableFilter"] = null;
        }
        public static void SetTableVariable(DatePicker datePicker)
        {
            System.Web.HttpContext.Current.Session["TableFilter"] = datePicker;
        }

        public static DatePicker GetTableVariable()
        {
            return (DatePicker)System.Web.HttpContext.Current.Session["TableFilter"];
        }

        public static AccountsReportParam GetAccountsParam()
        {
            return (AccountsReportParam)System.Web.HttpContext.Current.Session["AccountsParam"];
        }
        public static void  SetAccountsParam(AccountsReportParam reportparam)
        {
            System.Web.HttpContext.Current.Session["AccountsParam"] = reportparam;            
        }
        public static void SetCookie(byte[] arr)
        {
            System.Web.HttpContext.Current.Session["sessionVariable"] = arr;
        }
        public static byte[] GetCookie()
        {
            return (byte[])System.Web.HttpContext.Current.Session["sessionVariable"];
        }
        public static void SetEmpName(string Name)
        {
            System.Web.HttpContext.Current.Session["EN"] = Name;
        }
        public static string GetEmpName()
        {
            return (string)System.Web.HttpContext.Current.Session["EN"];
        }
        //public static void SetSideMenu(SideMenus menus)
        //{
        //    System.Web.HttpContext.Current.Session["SideMenu"] = menus;
        //}
        //public static SideMenus GetSideMenu()
        //{
        //    return (SideMenus)System.Web.HttpContext.Current.Session["SideMenu"];
        //}
        public static void ResetSideMenu()
        {
            System.Web.HttpContext.Current.Session["SideMenu"] = null;
        }
        public static void ResetCookies()
        {
            System.Web.HttpContext.Current.Session["SideMenu"] = null;
            System.Web.HttpContext.Current.Session["sessionVariable"] = null;
            System.Web.HttpContext.Current.Session["EN"] = null;
        }
        public static void SetCustomerLogin(byte[] login)
        {
            System.Web.HttpContext.Current.Session["sessionVariable"] = login;
        }
        public static byte[] GetCustomerLogin()
        {
            return (byte[])System.Web.HttpContext.Current.Session["sessionVariable"];
        }
        public static void SetProfileImage(string photo)
        {
            System.Web.HttpContext.Current.Session["profilePic"] = photo;
        }
        public static string GetProfileImage()
        {
            return System.Web.HttpContext.Current.Session["profilePic"]?.ToString();
        }
    }
}