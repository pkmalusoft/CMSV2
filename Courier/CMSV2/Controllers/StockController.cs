﻿using CMSV2.DAL;
using CMSV2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CMSV2.Controllers
{
    [SessionExpireFilter]
    public class StockController : Controller
    {
        Entities1 db = new Entities1();
        // GET: Stock
        public ActionResult Index()
        {
            int fyearid = Convert.ToInt32(Session["fyearid"]);
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"]);
          
            int BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            //StockSearch obj = (StockSearch)Session["StockSearch"];
            StockSearch model = new StockSearch();
            AWBDAO _dao = new AWBDAO();
            List<StockVM> translist = new List<StockVM>();
            translist = AWBDAO.GetStockList(BranchID, fyearid);
            model.Details = translist;
           
            return View(model);
        }

        public ActionResult Create(int id=0)
        {
            ViewBag.Item = db.Items.ToList();
            ViewBag.ItemType = db.ItemTypes.ToList();

            StockVM vm = new StockVM();
            if (id==0)
            {
                vm.ID = 0;
                vm.PurchaseDate = CommanFunctions.GetFirstDayofMonth();
            }
            else
            {
                ItemPurchase item = db.ItemPurchases.Find(id);
                vm.ItemId = item.ItemId;
                var item1=db.Items.Find(vm.ItemId);
                if (item!=null)
                {
                    vm.ItemTypeId =Convert.ToInt32(item1.ItemTypeId);
                }
                vm.AWBCount = item.AWBCount;
                vm.AWBNOFrom = item.AWBNOFrom;
                vm.AWBNOTo = item.AWBNOTo;
                vm.ReferenceNo = item.ReferenceNo;
                vm.Qty = item.Qty;
                vm.Rate = item.Rate;
                vm.Amount = item.Amount;
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Create(StockVM vm)
        {
            int fyearid = Convert.ToInt32(Session["fyearid"]);
            int companyid = Convert.ToInt32(Session["CurrentCompanyID"]);
            int branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            int UserId = Convert.ToInt32(Session["UserID"]);
            ItemPurchase v = new ItemPurchase();
            
            if (vm.ID == 0)
            {
                v.BranchID = branchid;
                v.CreatedBy = UserId;
                v.ModifiedBy = UserId;
                v.StockType = "OB";
                v.PurchaseDate = CommanFunctions.GetFirstDayofYear();
                v.CreatedDate = CommanFunctions.GetCurrentDateTime();
                v.ModifiedDate = CommanFunctions.GetCurrentDateTime();
                v.AcFinancialYearID = fyearid;
                v.ItemId = vm.ItemId;
                v.ItemTypeId = vm.ItemTypeId;
                v.AWBNOFrom = vm.AWBNOFrom;
                v.AWBNOTo = vm.AWBNOTo;
                v.AWBCount = vm.AWBCount;
                v.ReferenceNo = vm.ReferenceNo;
                v.Qty = vm.Qty;
                v.Rate = vm.Rate;
                v.Amount = vm.Amount;
                db.ItemPurchases.Add(v);
                db.SaveChanges();
                
            }
            else
            {
                v = db.ItemPurchases.Find(vm.ID);
                v.ModifiedBy = UserId;
                v.ModifiedDate = CommanFunctions.GetCurrentDateTime();
                v.ItemId = vm.ItemId;
                v.ItemTypeId = vm.ItemTypeId;
                v.AWBNOFrom = vm.AWBNOFrom;
                v.AWBNOTo = vm.AWBNOTo;
                v.AWBCount = vm.AWBCount;
                v.ReferenceNo = vm.ReferenceNo;
                v.Qty = vm.Qty;
                v.Rate = vm.Rate;
                v.Amount = vm.Amount;
                db.Entry(v).State = EntityState.Modified;
                db.SaveChanges();                

            }

            return RedirectToAction("Index", "Stock");

        }

        public JsonResult GetItemType(int id)
        {
            var list = db.Items.Where(cc => cc.ItemID == id).FirstOrDefault();
            if (list != null)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetItem(string term,int itemtypeid)
        {
            var list = db.Items.Where(cc => cc.ItemTypeId == itemtypeid).ToList();
            if (list != null)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteConfirmed(int id)
        {
            ItemPurchase item= db.ItemPurchases.Find(id);
            db.ItemPurchases.Remove(item);
            db.SaveChanges();
            TempData["SuccessMsg"] = "You have successfully Deleted Stocked Entry.";
            return RedirectToAction("Index");
        }
    }
}