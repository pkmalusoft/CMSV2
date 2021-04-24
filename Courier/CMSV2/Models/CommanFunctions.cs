// Decompiled with JetBrains decompiler
// Type: CMSV2.Models.CommanFunctions
// Assembly: Courier_27_09_16, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B3B4E05-393A-455A-A5DE-86374CE9B081
// Assembly location: D:\Courier09022018\Decompiled\obj\Release\Package\PackageTmp\bin\CMSV2.dll

using System;
using System.Web.Mvc;
using System.Data;
using System.Globalization;
using System.Web;
//using System.Data.Objects;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Linq;

namespace CMSV2.Models
{
  public class CommanFunctions
  {
        public static string GetConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            }
        }

        public static double GetGMTHours
        {
            get
            {
                return Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GMTHours"].ToString());
            }
        }
        public static DateTime ParseDate(string str, string Format = "dd-MMM-yyyy")
        {
            DateTime dt = DateTime.MinValue;
            if (DateTime.TryParseExact(str, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return dt;
            }
            return dt;
        }
        public static int ParseInt(string str)
        {
            int k = 0;
            if (Int32.TryParse(str, out k))
            {
                return k;
            }
            return 0;
        }
        public static Decimal ParseDecimal(string str)
        {
            Decimal k = 0;
            if (Decimal.TryParse(str, out k))
            {
                return k;
            }
            return 0;
        }

        public static string GetMinFinancialDate()
        {
            Entities1 db = new Entities1();
            
            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());

            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearFrom);

            string ss = "";
            if (startdate != null)
                ss = startdate.Year + "/" + startdate.Month + "/" + startdate.Day; // string.Format("{0:YYYY MM dd}", (object)startdate.ToString());

            return ss;
        }
        public static string GetMaxFinancialDate()
        {
            Entities1 db = new Entities1();

            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());

            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearTo);
            string ss = "";
            if (startdate != null)
                ss = startdate.Year + "/" + startdate.Month + "/" + startdate.Day; // string.Format("{0:YYYY MM dd}", (object)startdate.ToString());

            return ss;
        }

        public static string GetShortDateFormat(object iInputDate)
    {
      if (iInputDate != null)
        return string.Format("{0:dd/MM/yyyy}", (object) Convert.ToDateTime(iInputDate));
      return "";
    }

        public static string GetShortDateFormat1(object iInputDate)
        {
            if (iInputDate != null)
                return string.Format("{0:dd-MM-yyyy}", (object)Convert.ToDateTime(iInputDate));
            return "";
        }
        public static bool CheckCreateEntryValid()
        {
            //Entities1 db = new Entities1();
            //int currentfyearid = db.AcFinancialYears.Where(cc => cc.CurrentFinancialYear == true).FirstOrDefault().AcFinancialYearID;
            //int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            //if (currentfyearid != fyearid)
            //    return false;
            return true;
        }

        public static DateTime GetFirstDayofMonth()
        {
            Entities1 db = new Entities1();

            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearFrom);
            DateTime enddate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearTo);

            string vdate = "01" + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
            DateTime todaydate = DateTime.Now.Date;

            return Convert.ToDateTime(vdate);

            //if (todaydate>=startdate && todaydate <=enddate ) //current date between current financial year
            //    return Convert.ToDateTime(vdate);
            //else
            //{
            //    vdate = "01" + "-" + enddate.Month.ToString() + "-" + enddate.Year.ToString();
            //    return Convert.ToDateTime(vdate);
            //}

        }

        public static DateTime GetLastDayofMonth()
        {
            Entities1 db = new Entities1();

            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearFrom);
            DateTime enddate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearTo);
            double hours = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GMTHours"].ToString());

            DateTime todaydate = DateTime.UtcNow.AddHours(hours);// DateTimeOffset.Now.Date; // DateTime.Now.Date;            
            return todaydate;

            //DateTime todaydate = DateTimeOffset.Now.Date; // DateTime.Now.Date;            
            //return todaydate;
            //if (todaydate >= startdate && todaydate <= enddate) //current date between current financial year
            //    return todaydate;
            //else
            //{                
            //    return enddate;
            //}

        }

        public static DateTime GetCurrentDateTime()
        {
            Entities1 db = new Entities1();

            int fyearid = Convert.ToInt32(HttpContext.Current.Session["fyearid"].ToString());
            DateTime startdate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearFrom);
            DateTime enddate = Convert.ToDateTime(db.AcFinancialYears.Find(fyearid).AcFYearTo);
            double hours = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["GMTHours"].ToString());

            DateTime todaydate = DateTime.UtcNow.AddHours(hours);// DateTimeOffset.Now.Date; // DateTime.Now.Date;            
            return todaydate;
            
        }
        public static string GetLongDateFormat(object iInputDate)
        {
            if (iInputDate != null)
                return string.Format("{0:dd MMM yyyy hh:mm}", (object)Convert.ToDateTime(iInputDate));
            return "";
        }

        public static string GetDecimalFormat(object iInputValue, string Decimals)
        {
            if (Decimals == "2")
            {
                if (iInputValue != null)
                    return  String.Format("{0:0.00}", (object)Convert.ToDecimal(iInputValue));
            }
            else if (Decimals == "3")
            {
                if (iInputValue != null)
                    return String.Format("{0:0.000}", (object)Convert.ToDecimal(iInputValue));
            }
            return "";
        }
        public static string GetDecimalFormat1(object iInputValue, string Decimals="")
        {
            if (Decimals == "")
                Decimals = HttpContext.Current.Session["Decimal"].ToString();

            if (Convert.ToString(iInputValue) == "")
                return "";
            
            if (Decimals == "2")
            {
                if (iInputValue != null)
                    return String.Format("{0:0.00}", (object)Convert.ToDecimal(iInputValue));
            }
            else if (Decimals == "3")
            {
                if (iInputValue != null)
                    return String.Format("{0:0.000}", (object)Convert.ToDecimal(iInputValue));
            }
            return "";
        }

        public static string GetCurrencyId(int CurrencyId)
        {
            Entities1 db = new Entities1();
            try
            {
                string currencyname = db.CurrencyMasters.Find(CurrencyId).CurrencyName;

                return currencyname;
            }
            catch(Exception ex)
            {
                return "";
            }
        }
        public static string GetFormatNumber(object iInputValue, string Decimals="")
        {
            if (Decimals == "")
                Decimals = HttpContext.Current.Session["Decimal"].ToString(); 

            if (Decimals == "2")
            {
                                
                    if (iInputValue != null)
                    {
                     decimal v=0;
                      v = Decimal.Parse(((object)Convert.ToDecimal(iInputValue)).ToString());
                    if ( v >0)
                        return String.Format("{0:#,0.00}", (object)Convert.ToDecimal(iInputValue));
                    else
                        return "";
                    }
            }
            else if (Decimals == "3")
            {
                if (iInputValue != null)
                    return String.Format("{0:#,0.000}", (object)Convert.ToDecimal(iInputValue));
            }
            return "";
            
        }

        
    }

    public class StatusModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ValidDate  { get; set; }
    }
}
