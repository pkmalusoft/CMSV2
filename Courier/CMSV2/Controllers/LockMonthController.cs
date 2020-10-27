using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using System.Data.Entity;

namespace CMSV2.Controllers
{
    public class LockMonthController : Controller
    {
        Entities1 db = new Entities1();



        public ActionResult Index()
        {

            List<Months> m = new List<Months>();

            m.Add(new Months { MonthNumber = 1, MonthName = "January" });
            m.Add(new Months { MonthNumber = 2, MonthName = "February" });
            m.Add(new Months { MonthNumber = 3, MonthName = "March" });
            m.Add(new Months { MonthNumber = 4, MonthName = "April" });
            m.Add(new Months { MonthNumber = 5, MonthName = "May" });
            m.Add(new Months { MonthNumber = 6, MonthName = "June" });
            m.Add(new Months { MonthNumber = 7, MonthName = "July" });
            m.Add(new Months { MonthNumber = 8, MonthName = "August" });
            m.Add(new Months { MonthNumber = 9, MonthName = "September" });
            m.Add(new Months { MonthNumber = 10, MonthName = "October" });
            m.Add(new Months { MonthNumber = 11, MonthName = "November" });
            m.Add(new Months { MonthNumber = 12, MonthName = "December" });


            var data = db.Locks.ToList();

            List<LockUnlock> lst = new List<LockUnlock>();

            foreach (var item in data)
            {
                LockUnlock l = new LockUnlock();
                l.ID = item.ID;
                l.Month = Convert.ToInt32(item.Month);
                l.CurrentYear = Convert.ToInt32(item.CurrentYear);
                l.IsLock = Convert.ToBoolean(item.IsLock);

                string fullMonthName = new DateTime(2015, Convert.ToInt32(item.Month), 1).ToString("MMMM");
                l.MonthName = fullMonthName;
                lst.Add(l);
            }


            return View(lst);
        }

        public ActionResult Create()
        {
            List<Months> m = new List<Months>();

            m.Add(new Months { MonthNumber = 1, MonthName = "January" });
            m.Add(new Months { MonthNumber = 2, MonthName = "February" });
            m.Add(new Months { MonthNumber = 3, MonthName = "March" });
            m.Add(new Months { MonthNumber = 4, MonthName = "April" });
            m.Add(new Months { MonthNumber = 5, MonthName = "May" });
            m.Add(new Months { MonthNumber = 6, MonthName = "June" });
            m.Add(new Months { MonthNumber = 7, MonthName = "July" });
            m.Add(new Months { MonthNumber = 8, MonthName = "August" });
            m.Add(new Months { MonthNumber = 9, MonthName = "September" });
            m.Add(new Months { MonthNumber = 10, MonthName = "October" });
            m.Add(new Months { MonthNumber = 11, MonthName = "November" });
            m.Add(new Months { MonthNumber = 12, MonthName = "December" });

            ViewBag.months = m.ToList();

            return View();
        }

        public ActionResult Edit(int id)
        {
            List<Months> m = new List<Months>();

            m.Add(new Months { MonthNumber = 1, MonthName = "January" });
            m.Add(new Months { MonthNumber = 2, MonthName = "February" });
            m.Add(new Months { MonthNumber = 3, MonthName = "March" });
            m.Add(new Months { MonthNumber = 4, MonthName = "April" });
            m.Add(new Months { MonthNumber = 5, MonthName = "May" });
            m.Add(new Months { MonthNumber = 6, MonthName = "June" });
            m.Add(new Months { MonthNumber = 7, MonthName = "July" });
            m.Add(new Months { MonthNumber = 8, MonthName = "August" });
            m.Add(new Months { MonthNumber = 9, MonthName = "September" });
            m.Add(new Months { MonthNumber = 10, MonthName = "October" });
            m.Add(new Months { MonthNumber = 11, MonthName = "November" });
            m.Add(new Months { MonthNumber = 12, MonthName = "December" });

            ViewBag.months = m.ToList();

            Lock l = db.Locks.Find(id);
            if (l != null)
            {
                LockUnlock v = new LockUnlock();
                v.ID = l.ID;
                v.Month = Convert.ToInt32(l.Month);
                v.CurrentYear = v.CurrentYear;
                v.IsLock = Convert.ToBoolean(l.IsLock);

                return View(v);
            }
            else
            {
                return HttpNotFound();
            }
        }



        [HttpPost]
        public ActionResult Edit(LockUnlock v)
        {
            Lock l = db.Locks.Find(v.ID);


            l.CurrentYear = v.CurrentYear;
            l.Month= v.Month;
            l.IsLock = v.IsLock;

            db.Entry(l).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult Create(LockUnlock v)
        {
            Lock l = new Lock();

            l.CurrentYear = v.CurrentYear;
            l.Month = v.Month;
            l.IsLock = v.IsLock;

            db.Locks.Add(l);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        public class Months
        {
            public int MonthNumber { get; set; }
            public string MonthName { get; set; }
        }

    }
}
