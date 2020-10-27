using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;

namespace CMSV2.Controllers
{
    public class UserController : Controller
    {
        private Entities1 db = new Entities1();

        //
        // GET: /User/

        public ActionResult Index()
        {
            return View(db.UMUserMasters.ToList());
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id = 0)
        {
            UMUserMaster umusermaster = db.UMUserMasters.Find(id);
            if (umusermaster == null)
            {
                return HttpNotFound();
            }
            return View(umusermaster);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(UMUserMaster umusermaster)
        {
            if (ModelState.IsValid)
            {
                db.UMUserMasters.Add(umusermaster);
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Country.";
                return RedirectToAction("Index");
            }

            return View(umusermaster);
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id = 0)
        {
            UMUserMaster umusermaster = db.UMUserMasters.Find(id);
            if (umusermaster == null)
            {
                return HttpNotFound();
            }
            return View(umusermaster);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(UMUserMaster umusermaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(umusermaster).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully added Country.";
                return RedirectToAction("Index");
            }
            return View(umusermaster);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id = 0)
        {
            UMUserMaster umusermaster = db.UMUserMasters.Find(id);
            if (umusermaster == null)
            {
                return HttpNotFound();
            }
            return View(umusermaster);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            UMUserMaster umusermaster = db.UMUserMasters.Find(id);
            db.UMUserMasters.Remove(umusermaster);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}