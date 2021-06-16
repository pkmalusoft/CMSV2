using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using CMSV2.DAL;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Newtonsoft.Json;
namespace CMSV2.Controllers
{
    [SessionExpire]   
    public class AccountsController : Controller
    {
    //    SHIPPING_FinalEntities context = new SHIPPING_FinalEntities();
        SourceMastersModel objectSourceModel = new SourceMastersModel();
        //SHIPPING_FinalEntities db = new SHIPPING_FinalEntities();
        Entities1 db = new Entities1();

         
        #region Masters
        //Methods For Account Category
        public IEnumerable<AcGroupModel> GetAllAcGroupsByBranch(int Branchid)
        {
            var parents = (from d in db.AcGroups
                           where d.AcBranchID == Branchid
                           select d).ToList();
            var accategory = (from d in db.AcCategories
                              select d).ToList();
            IEnumerable<AcGroupModel> data = (from d in db.AcGroups join a in db.AcTypes on d.AcTypeId equals a.Id
                                              where d.AcBranchID == Branchid
                                              select
new AcGroupModel()
{

    AcGroupID = d.AcGroupID,
    AcGroup = d.AcGroup1,
    AcClass = d.AcClass,
    AcType =  a.AccountType, // d .AcType,
    BranchID = d.AcBranchID,
    GroupCode = d.GroupCode,
    GroupOrder = d.GroupOrder,
    ParentID = d.ParentID,
    UserID = d.UserID,
    AcCategoryID = d.AcCategoryID

}).ToList();
            foreach (var item in data)
            {
                var ParentNode = parents.Where(d => d.AcGroupID == item.ParentID).FirstOrDefault();
                if (ParentNode != null)
                {
                    item.ParentNode = ParentNode.AcGroup1;
                }
                var accat = accategory.Where(d => d.AcCategoryID == item.AcCategoryID).FirstOrDefault();
                if (accat != null)
                {
                    item.AcCategory = accat.AcCategory1;
                }
            }
            return data.OrderBy(cc=>cc.AcGroup);
        }
        public ActionResult IndexAcCategory()
        {

            var x = db.AcCategorySelectAll();
            return View(x);
        }

        public ActionResult CreateAcCategory()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CreateAcCategory(AcCategory c)
        {
            if (ModelState.IsValid)
            {
                db.AcCategoryInsert(c.AcCategory1);
                ViewBag.SuccessMsg = "You have successfully added Account Category";
                return View("IndexAcCategory", db.AcCategorySelectAll());
            }
            return View();
        }

        public ActionResult EditAcCategory(int id)
        {
            var x = db.AcCategorySelectByID(id);
            if (x == null)
            {
                return HttpNotFound();
            }
            return View(x.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult EditAcCategory(AcCategorySelectByID_Result c)
        {
            db.AcCategoryUpdate(c.AcCategoryID, c.AcCategory);
            ViewBag.SuccessMsg = "You have successfully updated Account Category";
            return View("IndexAcCategory", db.AcCategorySelectAll());
        }


        public ActionResult DeleteAcCategory(int id)
        {
            AcCategory c = (from x in db.AcCategories where x.AcCategoryID == id select x).FirstOrDefault();
            if (c != null)
            {
                try
                {
                    db.AcCategories.Remove(c);
                    db.SaveChanges();

                    ViewBag.SuccessMsg = "You have successfully deleted Account Category";

                }
                catch (Exception ex)
                {


                    ViewBag.ErrorMsg = "Transaction in Use. Can not Delete";


                }
            }

            return View("IndexAcCategory", db.AcCategorySelectAll());

        }



        //Methods for Account Groups

        public ActionResult IndexAcGroup()
        {

            var x = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            return View(x);
        }

        public ActionResult CreateAcGroup(int frmpage)
        {
            Session["AcgroupPage"] = frmpage;
            ViewBag.Category = db.AcCategorySelectAll();
            var branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            ViewBag.AccountType = (from d in db.AcTypes where d.BranchId == branchid select d).ToList();
            ViewBag.groups = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));

            int count = (from c in db.AcCompanies select c).ToList().Count();
            ViewBag.IsAuto = count;

            return View();
        }


        public bool GetDuplicateGroup(int AcgroupId, int ParentId, int CategoryID, string name)
        {
            var data = (from d in db.AcGroups where d.AcGroupID != AcgroupId && d.AcGroup1.ToLower() == name.ToLower() && d.AcCategoryID == CategoryID && d.ParentID == ParentId select d).FirstOrDefault();
            if (data == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpGet]
        public JsonResult GetGroupsByID(int Category)
        {
            var groups = db.AcGroupSelectByCategoryID(Category, Convert.ToInt32(Session["CurrentCompanyID"].ToString())).ToList();
            return Json(groups, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAcCategoryByParentid(int parentId)
        {
            var groups = (from d in db.AcGroups where d.AcGroupID == parentId select d).FirstOrDefault();
            return Json(new { categoryid = groups.AcCategoryID, acttypeid = groups.AcTypeId }, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public ActionResult CreateAcGroup(AcGroupVM c)
        {

            var isexist = GetDuplicateGroup(0, c.AcGroup, c.AcCategoryID, c.subgroup);
            if (isexist == true)
            {
                var acgrps = (from d in db.AcGroups orderby d.AcGroupID descending select d.AcGroupID).FirstOrDefault();
                var maxid = acgrps + 1;
                var actype = Getactype(c.AcTypeId);


                if (c.AcGroup == 0)
                {

                    var acgroup = new AcGroup();
                    acgroup.AcGroupID = maxid;
                    acgroup.AcCategoryID = actype.AcCategoryId;
                    acgroup.AcGroup1 = c.subgroup;
                    acgroup.AcBranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    acgroup.ParentID = c.AcGroup;
                    acgroup.UserID = Convert.ToInt32(Session["UserID"].ToString());
                    acgroup.StaticEdit = 0;
                    acgroup.StatusHide = false;
                    acgroup.GroupCode = c.GroupCode;
                    acgroup.AcTypeId = c.AcTypeId;
                    db.AcGroups.Add(acgroup);
                    db.SaveChanges();


                    //db.AcGroupInsert(c.AcCategoryID, c.subgroup, null, Convert.ToInt32(Session["CurrentCompanyID"].ToString()), Convert.ToInt32(Session["UserID"].ToString()), c.IsGroupCodeAuto, c.GroupCode);
                }
                else
                {
                    var acgroup = new AcGroup();
                    acgroup.AcGroupID = maxid;
                    acgroup.AcCategoryID = actype.AcCategoryId;
                    acgroup.AcGroup1 = c.subgroup;
                    acgroup.AcBranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    acgroup.ParentID = c.AcGroup;
                    acgroup.UserID = Convert.ToInt32(Session["UserID"].ToString());
                    acgroup.StaticEdit = 0;
                    acgroup.StatusHide = false;
                    acgroup.GroupCode = c.GroupCode;
                    acgroup.AcTypeId = c.AcTypeId;
                    db.AcGroups.Add(acgroup);
                    db.SaveChanges();
                    // db.AcGroupInsert(c.AcCategoryID, c.subgroup, c.AcGroup, Convert.ToInt32(Session["CurrentCompanyID"].ToString()), Convert.ToInt32(Session["UserID"].ToString()), c.IsGroupCodeAuto, c.GroupCode);
                }
                //db.AcGroupInsert(c.AcCategoryID, c.subgroup, c.ParentID, Convert.ToInt32(Session["CurrentCompanyID"].ToString()), Convert.ToInt32(Session["UserID"].ToString()), c.IsGroupCodeAuto, c.GroupCode);

                //db.AcGroupInsert(c.AcCategoryID, c.AcGroup1, null, Convert.ToInt32(Session["CurrentCompanyID"].ToString()), Convert.ToInt32(Session["UserID"].ToString()), null, c.GroupCode);
                var acgroupfrompage = Convert.ToInt32(Session["AcgroupPage"].ToString());
                if (acgroupfrompage == 1)
                {
                    ViewBag.SuccessMsg = "You have successfully added Account Group";
                    return View("IndexAcGroup", GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString())));
                }
                else
                {
                    ViewBag.AcgroupId = maxid;
                    return RedirectToAction("CreateAcHead", "Accounts", new { frmpage = Convert.ToInt32(Session["AcheadPage"]) });

                }

            }
            else
            {
                var branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                ViewBag.AccountType = (from d in db.AcTypes where d.BranchId == branchid select d).ToList();

                ViewBag.ErrorMsg = "Account Group already exists !!";
                return View(c);
            }


        }

        public ActionResult EditAcGroup(int id)
        {
            var branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            ViewBag.AccountType = (from d in db.AcTypes where d.BranchId == branchid select d).ToList();

            ViewBag.Category = db.AcCategorySelectAll();
            var groups = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            ViewBag.groups = groups.Where(d => d.AcGroupID != id).ToList();
            //var x = db.AcGroupSelectByID(id);
            //if (x == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(x.FirstOrDefault());

            AcGroupVM v = new AcGroupVM();
            var data = db.AcGroups.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                v.AcGroupID = data.AcGroupID;
                v.AcTypeId = data.AcTypeId;
                v.subgroup = data.AcGroup1;
                if (data.ParentID == null)
                {

                    v.ParentID = 0;
                }
                else
                {
                    v.ParentID = data.ParentID.Value;
                }
                v.GroupCode = data.GroupCode;
                v.AcCategoryID = data.AcCategoryID.Value;
            }

            return View(v);
        }
        public AcType Getactype(int? id)
        {
            var actype = (from d in db.AcTypes where d.Id == id select d).FirstOrDefault();
            return actype;
        }
        [HttpPost]
        public ActionResult EditAcGroup(AcGroupVM c)
        {
            var groups = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));

            var isexist = GetDuplicateGroup(c.AcGroupID, c.AcGroup, c.AcCategoryID, c.subgroup);
            var actype = Getactype(c.AcTypeId);

            if (isexist == true)
            {

                var acgroup = (from d in db.AcGroups where d.AcGroupID == c.AcGroupID select d).FirstOrDefault();
                acgroup.ParentID = c.AcGroup;
                acgroup.AcGroup1 = c.subgroup;
                acgroup.AcTypeId = c.AcTypeId;
                acgroup.AcCategoryID = actype.AcCategoryId;
                acgroup.GroupCode = c.GroupCode;
                db.Entry(acgroup).State = EntityState.Modified;
                db.SaveChanges();
                //db.AcGroupUpdate(c.AcGroupID, c.AcGroup, c.subgroup, c.AcCategoryID, 0, c.GroupCode);

                ViewBag.SuccessMsg = "You have successfully updated Account Group";
                return View("IndexAcGroup", GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString())));
            }
            else
            {
                ViewBag.Category = db.AcCategorySelectAll();
                ViewBag.groups = groups.Where(d => d.AcGroupID != c.AcGroupID).ToList();
                var branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                ViewBag.AccountType = (from d in db.AcTypes where d.BranchId == branchid select d).ToList();

                ViewBag.ErrorMsg = "Account Group already exists !!";
                return View(c);
            }
        }


        public ActionResult DeleteAcGroup(int id)
        {
            AcGroup c = (from x in db.AcGroups where x.AcGroupID == id select x).FirstOrDefault();
            if (c != null)
            {
                try
                {
                    var x = (from a in db.AcHeads where a.AcGroupID == id select a).FirstOrDefault();
                    var p = (from a in db.AcGroups where a.ParentID == id select a).FirstOrDefault();
                    if (x != null)
                    {
                        ViewBag.ErrorMsg = "Transaction in Use. Can not Delete";
                        throw new Exception();

                    }
                    else if (p != null)
                    {
                        ViewBag.ErrorMsg = "Transaction in Use. Can not Delete";
                        throw new Exception();

                    }
                    else
                    {
                        db.AcGroups.Remove(c);
                        db.SaveChanges();


                        ViewBag.SuccessMsg = "You have successfully deleted Account Group";
                        return RedirectToAction("IndexAcGroup", GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString())));

                    }

                }
                catch (Exception ex)
                {





                }
            }

            return View("IndexAcGroup", GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentCompanyID"].ToString())));
        }




        //Methods for Expense Analysis Group
        public ActionResult IndexExpenseAnalysisGroup()
        {

            var x = db.AnalysisGroupSelectAll();
            return View(x);
        }

        public ActionResult CreateExpenseAnalysisGroup()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CreateExpenseAnalysisGroup(AnalysisGroup c)
        {
            if (ModelState.IsValid)
            {

                db.AnalysisGroupInsert(GetMaxAnalysisGroupID(), c.AnalysisGroup1);
                ViewBag.SuccessMsg = "You have successfully added Expense Analysis Group";
                return View("IndexExpenseAnalysisGroup", db.AnalysisGroupSelectAll());
            }
            return View();
        }


        public ActionResult EditExpenseAnalysisGroup(int id)
        {
            var result = db.AnalysisGroupSelectByID(id);

            return View(result.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult EditExpenseAnalysisGroup(AnalysisGroupSelectByID_Result a)
        {
            db.AnalysisGroupUpdate(a.AnalysisGroupID, a.AnalysisGroup);
            ViewBag.SuccessMsg = "You have successfully updated Expense Analysis Group";
            return View("IndexExpenseAnalysisGroup", db.AnalysisGroupSelectAll());
        }

        public ActionResult DeleteExpenseAnalysisGroup(int id)
        {
            AnalysisGroup c = (from x in db.AnalysisGroups where x.AnalysisGroupID == id select x).FirstOrDefault();
            if (c != null)
            {
                try
                {
                    db.AnalysisGroups.Remove(c);
                    db.SaveChanges();

                    ViewBag.SuccessMsg = "You have successfully deleted Expense Analysis Group.";

                }
                catch (Exception ex)
                {


                    ViewBag.ErrorMsg = "Transaction in Use. Can not Delete";


                }
            }

            return View("IndexExpenseAnalysisGroup", db.AnalysisGroupSelectAll());
        }

        public int GetMaxAnalysisGroupID()
        {
            var query = db.AnalysisGroups.OrderByDescending(item => item.AnalysisGroupID).FirstOrDefault();

            if (query == null)
            {
                return 1;
            }
            else
            {
                return query.AnalysisGroupID + 1;
            }
        }





        //Methods For AcHead

        public ActionResult IndexAcHead()
        {
            var lst = AccountsDAO.GetAcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            return View(lst);
            //return View(db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString())));
        }

        public ActionResult DeleteAcHead(int id)
        {
            
            if (id != 0)
            {
                DataTable dt = ReceiptDAO.DeleteAccountHead(id);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        //if (dt.Rows[0][0] == "OK")
                        TempData["SuccessMsg"] = dt.Rows[0][1].ToString();
                    }

                }
                else
                {
                    TempData["ErrorMsg"] = "Error at delete";
                }
            }
            return RedirectToAction("IndexAcHead");
        }

        public ActionResult CreateAcHead(int frmpage)
        {
            Session["AcheadPage"] = frmpage;
            ViewBag.groups = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            return View();
        }

        [HttpPost]
        public ActionResult CreateAcHead(AcHead a)
        {

            int id = 0;
            AcHead x = db.AcHeads.OrderByDescending(item => item.AcHeadID).FirstOrDefault();
            if (x == null)
            {

                id = 1;
            }
            else
            {
                id = x.AcHeadID + 1;
            }

            //db.AcHeadInsert(id, a.AcHeadKey, a.AcHead1, a.AcGroupID, Convert.ToInt32(Session["CurrentBranchID"].ToString()), a.Prefix);
            AcHead v = new AcHead();
            v.AcHeadID = id;
            v.AcHeadKey = a.AcHeadKey;
            v.AcHead1 = a.AcHead1;
            v.AccountDescription = a.AccountDescription;
            v.AcGroupID = a.AcGroupID;
            v.UserID = Convert.ToInt32(Session["UserID"].ToString());
            v.AcBranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            v.Prefix = a.Prefix;
            v.StatusHide = false;
            v.TaxApplicable = a.TaxApplicable;
            if (a.TaxApplicable == true)
                v.TaxPercent = a.TaxPercent;
            else
                v.TaxPercent = 0;

            //v.StatusControlAc = false;
            db.AcHeads.Add(v);
            db.SaveChanges();
            var acheadfrompage = Convert.ToInt32(Session["AcheadPage"].ToString());
            if (acheadfrompage == 1)
            {
                ViewBag.SuccessMsg = "You have successfully created Account Head.";
                return RedirectToAction("IndexAcHead"); 
                //return View("IndexAcHead", db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString())));
            }
            else
            {
                //return View("IndexAcHead", db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString())));
                return RedirectToAction("Create", "RevenueType", new { acheadid = id });
            }
        }

        public ActionResult EditAcHead(int id)
        {
            var result = db.AcHeads.Find(id); // AcHeadSelectByID(id);
            ViewBag.groups = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            return View(result);
        }

        [HttpPost]
        public ActionResult EditAcHead(AcHead a)
        {
            //db.AcHeadUpdate(a.AcHeadKey, a.AcHeadID, a.AcHead, a.AcGroupID, a.Prefix);
            AcHead v = db.AcHeads.Find(a.AcHeadID);
            v.AccountDescription = a.AccountDescription;
            v.AcGroupID = a.AcGroupID;
            v.AcHead1 = a.AcHead1;
            v.AcHeadKey = a.AcHeadKey;
            v.TaxApplicable = a.TaxApplicable;
            if (a.TaxApplicable == true)
                v.TaxPercent = a.TaxPercent;
            else
                v.TaxPercent = 0;
            db.Entry(v).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.SuccessMsg = "You have successfully updated Account Head.";
            return RedirectToAction("IndexAcHead");
            //return View("IndexAcHead", db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString())));
        }


        public ActionResult DeleteAcBook(int id)
        {
            
            AcJournalMaster a = (from c in db.AcJournalMasters where c.AcJournalID == id select c).FirstOrDefault();
            a.StatusDelete = true;
            db.Entry(a).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.SuccessMsg = "You have successfully deleted Cash Bank Book Voucher!";
            return RedirectToAction("IndexAcBook");
            //return View("IndexAcHead", db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString())));
        }
        public ActionResult DeleteJVBook(int id)
        {

            AcJournalMaster a = (from c in db.AcJournalMasters where c.AcJournalID == id select c).FirstOrDefault();
            a.StatusDelete = true;
            db.Entry(a).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.SuccessMsg = "You have successfully deleted Journal Voucher!";
            return RedirectToAction("AcJournalVoucherIndex");
            //return View("IndexAcHead", db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString())));
        }

        //Methods for Expense Analysis Head


        public ActionResult IndexExpenseAnalysisHead()
        {
            return View(db.AnalysisHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString())));
        }

        public ActionResult DeleteExpenseAnalysisHead(int id)
        {
            AnalysisHead a = (from c in db.AnalysisHeads where c.AnalysisHeadID == id select c).FirstOrDefault();
            if (a != null)
            {
                db.AnalysisHeads.Remove(a);
                db.SaveChanges();
                ViewBag.SuccessMsg = "You have successfully deleted Analysis Head.";
            }
            return View("IndexExpenseAnalysisHead", db.AnalysisHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString())));
        }


        public ActionResult CreateExpenseAnalysisHead()
        {
            ViewBag.groups = db.AnalysisGroupSelectAll().ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateExpenseAnalysisHead(AnalysisHead a)
        {
            var analysisCode = db.AnalysisHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            var codeIsexist = analysisCode.Where(d => d.AnalysisCode == a.AnalysisCode).FirstOrDefault();
            if (codeIsexist == null)
            {
                db.AnalysisHeadInsert(a.AnalysisCode, a.AnalysisHead1, a.AnalysisGroupID, Convert.ToInt32(Session["CurrentBranchID"].ToString()));
                ViewBag.SuccessMsg = "You have successfully added Analysis Head.";
                return View("IndexExpenseAnalysisHead", db.AnalysisHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString())));
            }
            else
            {
                ViewBag.groups = db.AnalysisGroupSelectAll().ToList();
                ViewBag.ErrorMsg = "Analysis Code Already Exist.";
                return View();
            }
        }



        public ActionResult EditExpenseAnalysisHead(int id)
        {
            var result = db.AnalysisHeadSelectByID(id);
            ViewBag.groups = db.AnalysisGroupSelectAll().ToList();
            return View(result.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult EditExpenseAnalysisHead(AnalysisHeadSelectByID_Result a)
        {
            var analysisCode = db.AnalysisHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            var codeIsexist = analysisCode.Where(d => d.AnalysisCode == a.AnalysisCode && d.AnalysisHeadID != a.AnalysisHeadID).FirstOrDefault();
            if (codeIsexist == null)
            {
                db.AnalysisHeadUpdate(a.AnalysisHeadID, a.AnalysisCode, a.AnalysisHead, a.AnalysisGroupID);

                ViewBag.SuccessMsg = "You have successfully updated Analysis Head.";
                return View("IndexExpenseAnalysisHead", db.AnalysisHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString())));
            }
            else
            {
                var result = db.AnalysisHeadSelectByID(a.AnalysisHeadID);
                ViewBag.groups = db.AnalysisGroupSelectAll().ToList();
                ViewBag.ErrorMsg = "Analysis Code Already Exist.";

                return View(result.FirstOrDefault());
            }
        }



        //Methods for AcHeadAssign

        //public ActionResult IndexAcHeadAssign()
        //{
        //    return View(db.AcHeadAssignSelectAll());
        //}





        public ActionResult CreateAcHeadAssign()
        {

            ViewBag.provisionheads = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            ViewBag.accruedcost = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            ViewBag.openjobrevenue = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            ViewBag.custmorcontrol = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            ViewBag.cashcontrol = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            ViewBag.controlacid = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            return View();
        }

        [HttpPost]
        public ActionResult CreateAcHeadAssign(AcHeadAssign a)
        {
            //db.AcHeadAssignInsert(a.ProvisionCostControlAcID, a.AccruedCostControlAcID, a.OpenJobRevenueAcID,  a.CustomerControlAcID, a.CashControlAcID, a.SupplierControlAcID);
            db.AcHeadAssignInsert(a.ProvisionCostControlAcID, a.MaterialCostControlReceivableAcHeadID, a.OpenJobRevenueAcID, a.CustomerControlAcHeadID, a.CashAcHeadID, a.SupplierAcHeadID);
            ViewBag.SuccessMsg = "You have successfully added Account Assign Head";
            return View("IndexAcHeadAssign", db.AcHeadAssignSelectAll());
        }

        public ActionResult EditAcHeadAssign(int id)
        {
            ViewBag.provisionheads = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            ViewBag.accruedcost = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            ViewBag.openjobrevenue = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            ViewBag.custmorcontrol = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            ViewBag.cashcontrol = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            ViewBag.controlacid = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));

            var result = (from c in db.AcHeadAssigns where c.ID == id select c).FirstOrDefault();
            return View(result);
        }

        [HttpPost]
        public ActionResult EditAcHeadAssign(AcHeadAssign a)
        {
            //db.AcHeadAssignUpdate(a.ProvisionCostControlAcID, a.AccruedCostControlAcID, a.OpenJobRevenueAcID, a.CustomerControlAcID, a.CashControlAcID, a.SupplierControlAcID, a.ID);
            db.AcHeadAssignUpdate(a.ProvisionCostControlAcID, a.MaterialCostControlReceivableAcHeadID, a.OpenJobRevenueAcID, a.CustomerControlAcHeadID, a.CashAcHeadID, a.SupplierAcHeadID, a.ID);
            ViewBag.SuccessMsg = "You have successfully updated Account Assign Head";
            return View("IndexAcHeadAssign", db.AcHeadAssignSelectAll());
        }




        #endregion Masters



        //Cash And Bank Transactions
        #region "CashBAnkBook"

        [HttpPost]
        public ActionResult Create(AcBookVM v)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            string cheque = "";
            string StatusTrans = "";

            if (v.paytype > 1)
            {
                cheque = v.chequeno;
            }
            else
            {
                cheque = "";
            }

            AcJournalMaster ajm = new AcJournalMaster();
            if (v.AcJournalID == 0)
            {
                int voucherno = 0;
                voucherno = (from c in db.AcJournalMasters select c).ToList().Count();

                int max = 0;
                max = (from c in db.AcJournalMasters orderby c.AcJournalID descending select c.AcJournalID).FirstOrDefault();

                int MaxId = 0;
                MaxId = (from c in db.AcJournalMasters orderby c.ID descending select c.ID).FirstOrDefault();
                ajm.AcJournalID = max + 1;
                ajm.VoucherNo = (voucherno + 1).ToString();
                ajm.TransDate = v.transdate;
                ajm.TransType = Convert.ToInt16(v.transtype);
                if (v.transtype == 1)
                {
                    v.TransactionNo = "RE" + (max + 1).ToString().PadLeft(7, '0');
                }
                else if (v.transtype == 2)
                {
                    v.TransactionNo = "PA" + (max + 1).ToString().PadLeft(7, '0');
                }
                ajm.TransactionNo = v.TransactionNo;
                ajm.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
                ajm.VoucherType = v.TransactionType;
                ajm.StatusDelete = false;
                ajm.UserID = Convert.ToInt32(Session["UserID"].ToString());
                ajm.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                ajm.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            }
            else
            {
                ajm = db.AcJournalMasters.Find(v.AcJournalID);
            }

            //ajm.TransactionNo = v.TransactionNo;
            //ajm.AcJournalID = v.AcJournalID;
            //ajm.VoucherNo = v.VoucherNo;
            //ajm.TransDate = v.transdate;
            //ajm.TransType = Convert.ToInt16(v.transtype);
            //ajm.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
            //ajm.VoucherType = v.TransactionType;
            //ajm.StatusDelete = false;
            ajm.Remarks = v.remarks;

            ajm.ShiftID = null;
            ajm.PaymentType = v.paytype;


            ajm.Reference = v.reference;
            if (v.AcJournalID == 0)
            {
                db.AcJournalMasters.Add(ajm);
                db.SaveChanges();
            }
            else
            {
                db.Entry(ajm).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (v.TransactionType == "CBR" || v.TransactionType == "BKR")
                StatusTrans = "R";
            else if (v.TransactionType == "CBP" || v.TransactionType == "BKP")
                StatusTrans = "P";
            int maxBankDetailID = 0;
            int isexistbankdetail = 0;

            if (v.chequeno != null)
            {

                if (v.AcBankDetailID > 0)
                {
                    maxBankDetailID = v.AcBankDetailID;
                    isexistbankdetail = 1;
                }
                else
                {
                    var bankdetailid = (from c in db.AcBankDetails orderby c.AcBankDetailID descending select c.AcBankDetailID).FirstOrDefault();
                    v.AcBankDetailID = bankdetailid + 1;
                    isexistbankdetail = 0;

                }
                AcBankDetail acbankDetails = new AcBankDetail();
                acbankDetails.AcBankDetailID = v.AcBankDetailID;
                acbankDetails.BankName = v.bankname;
                acbankDetails.ChequeDate = v.chequedate;
                acbankDetails.ChequeNo = v.chequeno;
                acbankDetails.PartyName = v.partyname;
                acbankDetails.AcJournalID = ajm.AcJournalID;
                acbankDetails.StatusTrans = StatusTrans;
                acbankDetails.StatusReconciled = false;
                if (acbankDetails.BankName == null)
                {
                    acbankDetails.BankName = "B";
                }
                if (acbankDetails.PartyName == null)
                {
                    acbankDetails.PartyName = "P";
                }
                AccountsDAO.InsertOrUpdateAcBankDetails(acbankDetails, isexistbankdetail);
            }
            else
            {

            }

            decimal TotalAmt = 0;
            decimal totalTaxAmount = 0;
            for (int i = 0; i < v.AcJDetailVM.Count; i++)
            {
                if (v.AcJDetailVM[i].IsDeleted != true)
                {
                    if (v.AcJDetailVM[i].AmountIncludingTax == true && v.AcJDetailVM[i].TaxAmount > 0)
                    {
                        v.AcJDetailVM[i].Amt = v.AcJDetailVM[i].Amt - v.AcJDetailVM[i].TaxAmount;
                    }
                    TotalAmt = TotalAmt + Convert.ToDecimal(v.AcJDetailVM[i].Amt);
                    totalTaxAmount = Convert.ToDecimal(totalTaxAmount) + Convert.ToDecimal(v.AcJDetailVM[i].TaxAmount);
                }
            }
            int maxAcJDetailID = 0;

            AcJournalDetail ac = new AcJournalDetail();
            if (v.AcJournalID == 0)
            {
                maxAcJDetailID = 0;
                maxAcJDetailID = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();

                ac.AcJournalDetailID = maxAcJDetailID + 1;
                ac.AcJournalID = ajm.AcJournalID;
                ac.AcHeadID = v.SelectedAcHead;
            }
            else
            {
                ac = (from c in db.AcJournalDetails where c.AcJournalID == ajm.AcJournalID select c).FirstOrDefault();
            }

            //ac.AcJournalDetailID = ac.AcJournalDetailID;
            //ac.AcJournalID = ajm.AcJournalID;
            ac.AcHeadID = v.SelectedAcHead;
            if (StatusTrans == "P")
            {
                ac.Amount = -(TotalAmt + totalTaxAmount);
            }
            else
            {
                ac.Amount = TotalAmt + totalTaxAmount;
            }
            ac.Remarks = v.AcJDetailVM[0].Rem;
            ac.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            if (v.AcJournalID == 0)
            {
                db.AcJournalDetails.Add(ac);
                db.SaveChanges();
            }
            else
            {
                db.Entry(ac).State = EntityState.Modified;
                db.SaveChanges();
            }


            List<AcJournalAWBVM> AWBAllocationall = (List<AcJournalAWBVM>)Session["ACAWBAllocation"];


            for (int i = 0; i < v.AcJDetailVM.Count; i++)
            {
                if (v.AcJDetailVM[i].IsDeleted != true)
                {
                    AcJournalDetail acJournalDetail = new AcJournalDetail();
                    int IdExists = 0;
                    if (v.AcJDetailVM[i].AcJournalDetID > 0)
                    {
                        //  IdExists = (from c in db.AcJournalDetails where c.AcJournalDetailID == v.AcJDetailVM[i].AcJournalDetID select c.AcJournalDetailID).FirstOrDefault();
                        IdExists = v.AcJDetailVM[i].AcJournalDetID;
                    }
                    //  AcJournalDetID
                    if (IdExists > 0)
                    {
                        acJournalDetail.AcJournalDetailID = v.AcJDetailVM[i].AcJournalDetID;
                    }
                    else
                    {
                        maxAcJDetailID = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();
                        acJournalDetail.AcJournalDetailID = maxAcJDetailID + 1;
                    }
                    acJournalDetail.AcHeadID = v.AcJDetailVM[i].AcHeadID;
                    acJournalDetail.BranchID = Convert.ToInt32(Session["CurrentBranchID"]);
                    acJournalDetail.AcJournalID = ajm.AcJournalID;
                    if (v.AcJDetailVM[i].Rem == null)
                    {
                        acJournalDetail.Remarks = "";
                    }
                    else
                    {
                        acJournalDetail.Remarks = v.AcJDetailVM[i].Rem;
                    }
                    acJournalDetail.TaxPercent = v.AcJDetailVM[i].TaxPercent;
                    acJournalDetail.TaxAmount = v.AcJDetailVM[i].TaxAmount;
                    acJournalDetail.AmountIncludingTax = v.AcJDetailVM[i].AmountIncludingTax;
                    acJournalDetail.SupplierId = v.AcJDetailVM[i].SupplierID;
                    //if (v.AcJDetailVM[i].AmountIncludingTax == true && v.AcJDetailVM[i].TaxAmount > 0)
                    //{
                    //    v.AcJDetailVM[i].Amt = v.AcJDetailVM[i].Amt - v.AcJDetailVM[i].TaxAmount;
                    //}

                    if (StatusTrans == "P")
                    {
                        acJournalDetail.Amount = (v.AcJDetailVM[i].Amt);
                    }
                    else
                    {
                        acJournalDetail.Amount = -v.AcJDetailVM[i].Amt;
                    }
                    if (acJournalDetail.AnalysisHeadID == null)
                    {
                        acJournalDetail.AnalysisHeadID = 0;
                    }
                    if (IdExists > 0)
                    {
                        AccountsDAO.UpdateAcJournalDetail(acJournalDetail);
                        AccountsDAO.DeleteAcJournalAWBs(acJournalDetail);
                    }
                    else
                    {
                        AccountsDAO.InsertAcJournalDetail(acJournalDetail);
                    }

                    //adding consignment referece to this entry
                    int acheadid = Convert.ToInt32(v.AcJDetailVM[i].AcHeadID);

                    var oldlist = db.AcJournalAWBs.Where(cc => cc.AcJournalDetailID == acJournalDetail.AcJournalDetailID && cc.AcHeadID == acheadid).ToList();
                    if (oldlist != null)
                    {
                        foreach (var olditem in oldlist)
                        {
                            db.AcJournalAWBs.Remove(olditem);
                            db.SaveChanges();
                        }
                    }
                    if (AWBAllocationall != null)
                    {

                        var list = AWBAllocationall.Where(cc => cc.AcHeadID == acheadid).ToList();
                        if (list != null)
                        {
                            foreach (var item2 in list)
                            {
                                AcJournalAWB accons = new AcJournalAWB();
                                accons.AcJournalID = ajm.AcJournalID;
                                accons.AcJournalDetailID = acJournalDetail.AcJournalDetailID;
                                accons.AcHeadID = acheadid;
                                accons.InScanID = Convert.ToInt32(item2.InScanID);
                                accons.Amount = item2.Amount;
                                db.AcJournalAWBs.Add(accons);
                                db.SaveChanges();
                            }
                        }
                    }
                    if (v.AcJDetailVM[i].AcExpAllocationVM != null)
                    {
                        for (int k = 0; k < v.AcJDetailVM[i].AcExpAllocationVM.Count; k++)
                        {
                            Nullable<int> AllocationIdExists = 0;
                            AcAnalysisHeadAllocation objAcAnalysisHeadAllocations = new AcAnalysisHeadAllocation();
                            if (v.AcJDetailVM[i].AcExpAllocationVM[k].AcAnalysisHeadAllocationID != null && v.AcJDetailVM[i].AcExpAllocationVM[k].AcAnalysisHeadAllocationID > 0)
                            {
                                AllocationIdExists = v.AcJDetailVM[i].AcExpAllocationVM[k].AcAnalysisHeadAllocationID;
                            }
                            if (AllocationIdExists > 0)
                            {
                                objAcAnalysisHeadAllocations.AcAnalysisHeadAllocationID = (int)AllocationIdExists;
                            }
                            else
                            {
                                objAcAnalysisHeadAllocations.AcAnalysisHeadAllocationID = 0;
                            }
                            objAcAnalysisHeadAllocations.AcjournalDetailID = acJournalDetail.AcJournalDetailID;
                            objAcAnalysisHeadAllocations.Amount = v.AcJDetailVM[i].AcExpAllocationVM[k].ExpAllocatedAmount;
                            objAcAnalysisHeadAllocations.AnalysisHeadID = v.AcJDetailVM[i].AcExpAllocationVM[k].AcHead;
                            if (AllocationIdExists > 0)
                            {
                                AccountsDAO.UpdateAcAnalysisHeadAllocation(objAcAnalysisHeadAllocations);
                            }
                            else
                            {
                                AccountsDAO.InsertAcAnalysisHeadAllocation(objAcAnalysisHeadAllocations);
                            }
                            //  AcJournalDetID
                        }
                    }
                }
            }
            //
            //Insert Tax Payable Account Ledger
            try
            {
                int? vataccountid = db.BranchMasters.Find(branchid).VATAccountId;
                if (vataccountid != null && totalTaxAmount > 0)
                {
                    bool newentry = false;
                    ac = new AcJournalDetail();
                    ac = db.AcJournalDetails.Where(cc => cc.AcJournalID == v.AcJournalID && cc.AcHeadID == vataccountid).FirstOrDefault();
                    if (ac == null)
                    {
                        ac = new AcJournalDetail();
                        maxAcJDetailID = 0;
                        maxAcJDetailID = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();
                        ac.AcJournalDetailID = maxAcJDetailID + 1;
                        ac.AcJournalID = ajm.AcJournalID;
                        ac.AcHeadID = vataccountid;
                        newentry = true;
                    }

                    if (StatusTrans == "P")
                    {
                        ac.Amount = (totalTaxAmount);
                    }
                    else
                    {
                        ac.Amount = -totalTaxAmount;
                    }
                    ac.Remarks = "Tax Payable";
                    ac.BranchID = branchid;
                    if (newentry)
                    {
                        db.AcJournalDetails.Add(ac);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(ac).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
            catch (Exception ex2)
            {

            }

            string DeleteJournalDetails = Request["deletedJournalDetails"];
            string[] DeleteJournalDetailsArr = DeleteJournalDetails.Split(',');
            foreach (string JournalDetails in DeleteJournalDetailsArr)
            {
                int iDeleteJournalDetails = 0;
                int.TryParse(JournalDetails, out iDeleteJournalDetails);
                AccountsDAO.DeleteAcJournalDetail(iDeleteJournalDetails);

            }
            string DeleteAcAnalysisHeadAllocation = Request["deletedExpAllocations"];
            string[] DeleteAcAnalysisHeadAllocationArr = DeleteAcAnalysisHeadAllocation.Split(',');
            foreach (string AcAnalysisHeadAllocation in DeleteAcAnalysisHeadAllocationArr)
            {
                int iAcAnalysisHeadAllocation = 0;
                int.TryParse(AcAnalysisHeadAllocation, out iAcAnalysisHeadAllocation);
                AccountsDAO.DeleteAcAnalysisHeadAllocation(iAcAnalysisHeadAllocation);
            }
            ViewBag.SuccessMsg = "You have successfully added Record";
            return RedirectToAction("IndexAcBook");// View("IndexAcBook", db.AcJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())));
            //return View("IndexAcBook", db.AcJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())));
            //string cheque = "";
            //string StatusTrans = "";

            ////if (v.paytype > 1)
            ////{
            ////    cheque = v.chequeno;
            ////}
            ////else
            ////{
            ////    cheque = "";
            ////}


            ////string voucherno = "B123";
            ////int voucherno = 0;
            ////voucherno = (from c in db.AcJournalMasters select c).ToList().Count();



            //AcJournalMaster ajm = new AcJournalMaster();
            //ajm.AcJournalID = v.AcJournalID;
            //ajm.VoucherNo = v.VoucherNo;
            //ajm.TransDate = v.transdate;
            //ajm.TransType = Convert.ToInt16(v.transtype);
            //ajm.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
            ////ajm.VoucherType = v.TransactionType;
            //ajm.VoucherType = v.VoucherType;

            //ajm.StatusDelete = false;
            //ajm.Remarks = v.remarks;
            //ajm.UserID = Convert.ToInt32(Session["UserID"].ToString());
            //ajm.ShiftID = null;

            ////ajm.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            //ajm.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            //ajm.Reference = v.reference;

            //db.Entry(ajm).State = EntityState.Modified;
            //db.SaveChanges();

            ////if (v.TransactionType == "CBR" || v.TransactionType == "BKR")
            ////    StatusTrans = "R";
            ////else if (v.TransactionType == "CBP" || v.TransactionType == "BKP")
            ////    StatusTrans = "P";

            //if (v.VoucherType == "CBR" || v.VoucherType == "BKR")
            //{
            //    StatusTrans = "R";
            //}
            //else if (v.VoucherType == "CBP" || v.VoucherType == "BKP")
            //{
            //    StatusTrans = "P";
            //}

            //if (v.chequeno != null)
            //{
            //    AcBankDetail acbankDetails = new AcBankDetail();
            //    acbankDetails.AcBankDetailID = v.AcBankDetailID;
            //    acbankDetails.AcJournalID = ajm.AcJournalID;
            //    acbankDetails.BankName = v.bankname;
            //    acbankDetails.ChequeDate = v.chequedate;
            //    acbankDetails.ChequeNo = v.chequeno;
            //    acbankDetails.PartyName = v.partyname;
            //    acbankDetails.StatusTrans = StatusTrans;
            //    acbankDetails.StatusReconciled = false;
            //    db.Entry(acbankDetails).State = EntityState.Modified;
            //    db.SaveChanges();
            //}



            ////AcJournalDetail ac = new AcJournalDetail();
            ////ac.AcJournalID = ajm.AcJournalID;
            ////ac.AcHeadID = v.AcHead;
            ////if (StatusTrans == "P")
            ////{
            ////    ac.Amount = -(v.TotalAmt);
            ////}
            ////else
            ////{
            ////    ac.Amount = v.TotalAmt;
            ////}
            ////ac.Remarks = v.remarks;
            ////ac.BranchID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());

            ////db.AcJournalDetails.Add(ac);
            ////db.SaveChanges();



            //for (int i = 0; i < v.AcJDetailVM.Count; i++)
            //{
            //    AcJournalDetail acJournalDetail = new AcJournalDetail();
            //    acJournalDetail.AcHeadID = v.AcJDetailVM[i].AcHeadID;
            //    acJournalDetail.AcJournalID = ajm.AcJournalID;
            //    acJournalDetail.BranchID = Convert.ToInt32(Session["CurrentCompanyID"]);
            //    acJournalDetail.AcJournalDetailID = v.AcJDetailVM[i].AcJournalDetID;
            //    acJournalDetail.Remarks = v.AcJDetailVM[i].Rem;

            //    if (StatusTrans == "P")
            //    {
            //        acJournalDetail.Amount = -(v.AcJDetailVM[i].Amt);
            //    }
            //    else
            //    {
            //        acJournalDetail.Amount = v.AcJDetailVM[i].Amt;
            //    }

            //    db.Entry(acJournalDetail).State = EntityState.Modified;
            //    db.SaveChanges();

            //}


            //ViewBag.SuccessMsg = "You have successfully added Record";
            //return View("IndexAcBook", db.AcJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())));
        }

        public ActionResult Create(int id = 0)
        {
            AcBookVM v = new AcBookVM();
            if (id > 0)
            {
                ViewBag.EditMode = "true";
                ViewBag.Title = "Cash & Bank Book - Modify";
                AcJournalMaster ajm = db.AcJournalMasters.Find(id);
                AcBankDetail abank = (from a in db.AcBankDetails where a.AcJournalID == id select a).FirstOrDefault();
                v.TransactionNo = ajm.TransactionNo;
                v.transdate = ajm.TransDate.Value;
                v.SelectedAcHead = (from c in db.AcJournalDetails where c.AcJournalID == ajm.AcJournalID select c.AcHeadID).FirstOrDefault();
                v.AcHead = (from c in db.AcHeads where c.AcHeadID == v.SelectedAcHead select c.AcHead1).FirstOrDefault();
                v.remarks = ajm.Remarks;
                v.reference = ajm.Reference;
                v.VoucherType = ajm.VoucherType;
                v.AcJournalID = ajm.AcJournalID;
                v.VoucherNo = ajm.VoucherNo;
                v.TransactionType = v.VoucherType;
                v.paytype = Convert.ToInt16(ajm.PaymentType);
                v.transtype = Convert.ToInt32(ajm.TransType);

                if (abank != null)
                {
                    v.AcBankDetailID = abank.AcBankDetailID;
                    v.bankname = abank.BankName;
                    v.partyname = abank.PartyName;
                    v.chequedate = abank.ChequeDate.Value;
                    v.chequeno = abank.ChequeNo;
                }

                //List<AcJournalAWBVM> AWBAllocationall = (from c in db.AcJournalConsignments join d in db.InScanMasters on c.InScanID equals d.InScanID where c.AcJournalID == id select new AcJournalAWBVM { ID = c.ID, AcJournalID = c.AcJournalID, AcJournalDetailID = c.AcJournalDetailID, AcHeadID = c.AcHeadID, Amount = c.Amount, InScanID = c.InScanID, ConsignmentNo = d.ConsignmentNo, ConsignmentDate = d.TransactionDate }).ToList();

                //Session["ACAWBAllocation"] = AWBAllocationall;
            }
            else
            {
                ViewBag.Title = "Cash & Bank Book - Create";
                ViewBag.EditMode = "false";
                Session["ACAWBAllocation"] = null;
            }

            var transtypes = new SelectList(new[]
                                        {
                                            new { ID = "1", trans = "Receipt" },
                                            new { ID = "2", trans = "Payment" },

                                        },
           "ID", "trans", 1);
            var paytypes = new SelectList(new[]{
                                            new { ID = "1", pay = "Cash" },
                                             new { ID = "2", pay = "Cheque" },
                                              new { ID = "3", pay = "Credit Card" },
                                               new { ID = "4", pay = "Bank Transfer" },
                                                new { ID = "5", pay = "Bank Deposit" },
                                        }, "ID", "pay", 1);
            var paymentterms = (from d in db.PaymentTerms select d).ToList();
            var paytypes1 = new SelectList(paymentterms, "PaymentTermID", "PaymentTerm1");
            ViewBag.transtypes = transtypes;
            ViewBag.paytypes = paytypes;
            if (v.VoucherType == "CBR" || v.VoucherType == "CBP")
            {
                var data = db.AcHeadSelectForCash(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
                //ViewBag.heads = db.AcHeadSelectForCash(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
                ViewBag.heads = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            }
            else if (v.VoucherType == "BKP" || v.VoucherType == "BKR" || v.VoucherType == "RP" || v.VoucherType == "BK")
            {
                //ViewBag.heads = db.AcHeadSelectForBank(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
                ViewBag.heads = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            }
            ViewBag.headsreceived = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));


            return View(v);


        }
        [HttpGet]
        public JsonResult GetAWBAllocation(int AcHeadId)
        {
            List<AcJournalAWBVM> AWBAllocationall = new List<AcJournalAWBVM>();
            List<AcJournalAWBVM> AWBAllocation = new List<AcJournalAWBVM>();
            AWBAllocationall = (List<AcJournalAWBVM>)Session["ACAWBAllocation"];
            if (AWBAllocationall == null)
            {
                return Json(AWBAllocation, JsonRequestBehavior.AllowGet);
            }
            else
            {
                AWBAllocation = AWBAllocationall.Where(cc => cc.AcHeadID == AcHeadId).ToList();
            }

            if (AWBAllocation == null)
            {
                AWBAllocation = new List<AcJournalAWBVM>();

            }
            return Json(AWBAllocation, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAWB(string term)
        {
            int AcCompanyID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            if (term.Trim() != "")
            {
                var list = (from c in db.InScanMasters where c.IsDeleted == false && c.AWBNo.Contains(term.Trim()) orderby c.AWBNo select new { InScanID = c.InScanID, TransactionDate = c.TransactionDate, ConsignmentNo = c.AWBNo }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = (from c in db.InScanMasters where c.IsDeleted == false  orderby c.AWBNo select new { InScanID = c.InScanID, TransactionDate = c.TransactionDate, ConsignmentNo = c.AWBNo }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public JsonResult SaveAWBAllocation(List<AcJournalAWBVM> list)
        {

            List<AcJournalAWBVM> AWBAllocationall = new List<AcJournalAWBVM>();
            List<AcJournalAWBVM> AWBAllocation = new List<AcJournalAWBVM>();
            AWBAllocationall = (List<AcJournalAWBVM>)Session["ACAWBAllocation"];

            if (AWBAllocationall == null)
            {
                AWBAllocationall = new List<AcJournalAWBVM>();
                foreach (var item2 in list)
                {
                    AWBAllocationall.Add(item2);

                }

            }
            else
            {
                int acheadid = list[0].AcHeadID;
                AWBAllocationall.RemoveAll(cc => cc.AcHeadID == acheadid);
                foreach (var item2 in list)
                {
                    AWBAllocationall.Add(item2);

                }
            }

            Session["ACAWBAllocation"] = AWBAllocationall;

            return Json(AWBAllocationall, JsonRequestBehavior.AllowGet);

        }
        #endregion


        public ActionResult AcJournalVoucherIndex(string FromDate, string ToDate)
        {
            DateTime pFromDate;
            DateTime pToDate;
            int pStatusId = 0;
            
            if (FromDate == null || ToDate == null)
            {
                pFromDate = DateTimeOffset.Now.Date;//.AddDays(-1); // FromDate = DateTime.Now;
                pToDate = DateTime.Now.Date; // ToDate = DateTime.Now;
            }
            else
            {
                pFromDate = Convert.ToDateTime(FromDate);//.AddDays(-1);
                pToDate = Convert.ToDateTime(ToDate).AddDays(1);

            }
            List<AcJournalMasterVM> lst = AccountsDAO.AcJournalMasterSelect(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentBranchID"].ToString()), pFromDate.Date, pToDate.Date);
            ViewBag.FromDate = pFromDate.Date.ToString("dd-MM-yyyy");
            ViewBag.ToDate = pToDate.Date.ToString("dd-MM-yyyy");
            return View(lst);


//            return View(db.AcJournalMasterSelectAllJV(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentBranchID"].ToString())));
        }

        public ActionResult AcJournalVoucherCreate(int id = 0)
        {
            var DebitAndCr = new SelectList(new[]
                                        {
                                            new { ID = "1", trans = "Dr" },
                                            new { ID = "2", trans = "Cr" },

                                        },
                                      "ID", "trans", 1);
            ViewBag.Achead = db.AcHeads.ToList();

            AcJournalMasterVoucherVM obj = new AcJournalMasterVoucherVM();
            if (id > 0)
            {
                ViewBag.Title = "Journal Voucher - Modify";
                var data = (from d in db.AcJournalMasters where d.AcJournalID == id select d).FirstOrDefault();


                if (data == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    obj.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
                    obj.AcJournalID = data.AcJournalID;
                    obj.VoucherType = "JV";
                    obj.VoucherNo = data.VoucherNo;
                    obj.userId = Convert.ToInt32(Session["UserID"].ToString());
                    obj.TransDate = data.TransDate.Value;
                    obj.statusDelete = false;
                    //obj.ShiftID = null;
                    obj.Remark = data.Remarks;
                    obj.AcCompanyID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                    obj.Refference = data.Reference;

                }
            }
            else
            {
                ViewBag.Title = "Journal Voucher - Create";
                obj.AcJournalID = 0;
                obj.TransDate = CommanFunctions.GetCurrentDateTime().Date;
            }
            return View(obj);

        }
        [HttpPost]
        public ActionResult AcJournalVoucherCreate(AcJournalMasterVoucherVM data)
        {
            AcJournalMaster acJournalMaster = new AcJournalMaster();
            if (data.AcJournalID == 0)
            {

                acJournalMaster.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
                acJournalMaster.AcJournalID = objectSourceModel.GetMaxNumberAcJournalMasters();
                acJournalMaster.VoucherType = "JV";

                int max = (from c in db.AcJournalMasters select c).ToList().Count();

                acJournalMaster.VoucherNo = (max + 1).ToString();
                acJournalMaster.UserID = Convert.ToInt32(Session["UserID"].ToString());
                acJournalMaster.TransDate = data.TransDate;
                acJournalMaster.StatusDelete = false;
                acJournalMaster.ShiftID = null;
                acJournalMaster.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
                acJournalMaster.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            }
            else
            {
                acJournalMaster = db.AcJournalMasters.Find(data.AcJournalID);
            }


            acJournalMaster.Remarks = data.Remark;

            acJournalMaster.Reference = data.Refference;
            if (data.AcJournalID == 0)
            {
                db.AcJournalMasters.Add(acJournalMaster);
                db.SaveChanges();
            }
            else
            {
                db.Entry(acJournalMaster).State = EntityState.Modified;
                db.SaveChanges();

                var x = (from c in db.AcJournalDetails where c.AcJournalID == data.AcJournalID select c).ToList();

                foreach (var i in x)
                {
                    db.AcJournalDetails.Remove(i);
                    db.SaveChanges();
                }
            }


            for (int i = 0; i < data.acJournalDetailsList.Count; i++)
            {
                if (data.acJournalDetailsList[i].IsDeleted != true)
                {
                    AcJournalDetail acjournalDetails = new AcJournalDetail();
                    if (data.acJournalDetailsList[i].IsDebit == 1)
                    {
                        acjournalDetails.Amount = Convert.ToDecimal(data.acJournalDetailsList[i].Amount);
                    }
                    else
                    {
                        acjournalDetails.Amount = (-1) * Convert.ToDecimal(data.acJournalDetailsList[i].Amount);
                    }

                    acjournalDetails.AcJournalID = acJournalMaster.AcJournalID;
                    acjournalDetails.AcHeadID = data.acJournalDetailsList[i].acHeadID;
                    acjournalDetails.Remarks = data.acJournalDetailsList[i].AcRemark;
                    acjournalDetails.BranchID = Convert.ToInt32(Session["CurrentCompanyID"]);
                    int maxAcJDetailID = 0;
                    maxAcJDetailID = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();

                    acjournalDetails.AcJournalDetailID = maxAcJDetailID + 1;
                    db.AcJournalDetails.Add(acjournalDetails);
                    db.SaveChanges();
                }
            }
            if (data.AcJournalID == 0)
            {
                ViewBag.SuccessMsg = "You have successfully added Journal Voucher.";
            }
            else
            {
                ViewBag.SuccessMsg = "You have successfully Updated Journal Voucher.";
            }

            return RedirectToAction("AcJournalVoucherIndex");


        }


        public ActionResult AcJournalVoucherEdit(int id = 0)
        {
            AcJournalMasterVoucherVM obj = new AcJournalMasterVoucherVM();
            ViewBag.achead = db.AcHeads.ToList();

            var data = (from d in db.AcJournalMasters where d.AcJournalID == id select d).FirstOrDefault();



            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                obj.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
                obj.AcJournalID = data.AcJournalID;
                obj.VoucherType = "JV";
                obj.VoucherNo = data.VoucherNo;
                obj.userId = Convert.ToInt32(Session["UserID"].ToString());
                obj.TransDate = data.TransDate.Value;
                obj.statusDelete = false;
                //obj.ShiftID = null;
                obj.Remark = data.Remarks;
                obj.AcCompanyID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                obj.Refference = data.Reference;

            }
            return View(obj);

        }
        [HttpPost]
        public ActionResult AcJournalVoucherEdit(AcJournalMasterVoucherVM data)
        {
            AcJournalMaster obj = new AcJournalMaster();
            obj.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
            obj.AcJournalID = data.AcJournalID;
            obj.VoucherType = "JV";
            obj.VoucherNo = data.VoucherNo;
            obj.UserID = Convert.ToInt32(Session["UserID"].ToString());
            obj.TransDate = data.TransDate;
            obj.StatusDelete = false;
            //obj.ShiftID = null;
            obj.Remarks = data.Remark;
            obj.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            obj.BranchID= Convert.ToInt32(Session["CurrentBranchID"].ToString());
            obj.Reference = data.Refference;
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();


            var x = (from c in db.AcJournalDetails where c.AcJournalID == data.AcJournalID select c).ToList();

            foreach (var i in x)
            {
                db.AcJournalDetails.Remove(i);
                db.SaveChanges();
            }


            for (int i = 0; i < data.acJournalDetailsList.Count; i++)
            {
                if (data.acJournalDetailsList[i].IsDeleted != true)
                {
                    AcJournalDetail acjournalDetails = new AcJournalDetail();
                    int maxAcJDetailID = 0;
                    maxAcJDetailID = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();

                    acjournalDetails.AcJournalDetailID = maxAcJDetailID + 1;
                    if (data.acJournalDetailsList[i].IsDebit == 1)
                    {
                        acjournalDetails.Amount = Convert.ToDecimal(data.acJournalDetailsList[i].Amount);
                    }
                    else
                    {
                        acjournalDetails.Amount = (-1) * Convert.ToDecimal(data.acJournalDetailsList[i].Amount);
                    }


                    acjournalDetails.AcJournalID = obj.AcJournalID;
                    acjournalDetails.AcHeadID = data.acJournalDetailsList[i].acHeadID;
                    acjournalDetails.Remarks = data.acJournalDetailsList[i].AcRemark;
                    acjournalDetails.BranchID = Convert.ToInt32(Session["CurrentBranchID"]);
                    db.AcJournalDetails.Add(acjournalDetails);
                    db.SaveChanges();
                }
            }
            ViewBag.SuccessMsg = "You have successfully added Journal Voucher.";
            return RedirectToAction("AcJournalVoucherIndex");


        }

        [HttpGet]
        public JsonResult GetAcJVDetails(int id)
        {
            var lst = (from c in db.AcJournalDetails where c.AcJournalID == id select c).ToList();

            List<AcJournalDetailsList> acdetails = new List<AcJournalDetailsList>();

            foreach (var item in lst)
            {
                AcJournalDetailsList v = new AcJournalDetailsList();
                string x = (from a in db.AcHeads where a.AcHeadID == item.AcHeadID select a.AcHead1).FirstOrDefault();

                v.acHeadID = item.AcHeadID.Value;
                v.AcHead = x;
                v.AcRemark = item.Remarks;

                if (item.Amount < 0)
                {
                    v.IsDebit = 0;
                    v.drcr = "Cr";
                    v.Amount = (-item.Amount.Value);
                }
                else
                {
                    v.IsDebit = 1;
                    v.drcr = "Dr";
                    v.Amount = item.Amount.Value;
                }

                v.AcJournalDetID = item.AcJournalDetailID;

                acdetails.Add(v);

            }
            return Json(acdetails, JsonRequestBehavior.AllowGet);
        }


        public ActionResult IndexAcBook(string VoucherType, string FromDate, string ToDate)
        {
            DateTime pFromDate;
            DateTime pToDate;
            int pStatusId = 0;
            
            if (VoucherType == "" || VoucherType == null)
                VoucherType = "All";

            if (FromDate == null || ToDate == null)
            {
                pFromDate = DateTimeOffset.Now.Date;//.AddDays(-1); // FromDate = DateTime.Now;
                pToDate = DateTime.Now.Date; // ToDate = DateTime.Now;
            }
            else
            {
                pFromDate = Convert.ToDateTime(FromDate);//.AddDays(-1);
                pToDate = Convert.ToDateTime(ToDate).AddDays(1);

            }
            List<AcJournalMasterVM> lst = AccountsDAO.AcJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentBranchID"].ToString()), pFromDate.Date, pToDate.Date,VoucherType);
            ViewBag.FromDate = pFromDate.Date.ToString("dd-MM-yyyy");
            ViewBag.ToDate = pToDate.Date.ToString("dd-MM-yyyy");
            List<VoucherTypeVM> lsttype = new List<VoucherTypeVM>();
            lsttype.Add(new VoucherTypeVM { TypeName = "All" }); 
            var typeitems  = (from c in db.AcJournalMasters where (c.VoucherType=="CBP" || c.VoucherType == "CBR" || c.VoucherType=="BKR" || c.VoucherType=="BKP" || c.VoucherType=="BK" || c.VoucherType=="CB" ) select new VoucherTypeVM { TypeName = c.VoucherType }).Distinct().ToList();
            foreach(VoucherTypeVM Item in typeitems)
            {
                lsttype.Add(Item);
            }
            ViewBag.VoucherType = VoucherType;
            ViewBag.VoucherTypes = lsttype;
            return View(lst);

           //return View(db.AcJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())).OrderByDescending(cc=>cc.TransDate));
        }



        public ActionResult IndexOpenningBalance()
        {
            var list = new SelectList(new[]
            {
                new { ID = "1", Name = "Cr" },
                new { ID = "2", Name = "Dr" },

            },
            "ID", "Name", 1);
            ViewBag.crdr = list;
            List<OpennnigBalanceVM> ob = new List<OpennnigBalanceVM>();
            var data = db.AcOpeningMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())).ToList();


            foreach (var item in data)
            {
                OpennnigBalanceVM b = new OpennnigBalanceVM();
                b.AcHeadID = item.AcHeadID.Value;
                b.AcHead = item.AcHead;
                b.AcFinancialYearID = item.AcFinancialYearID.Value;
                if (item.Amount < 0)
                {
                    b.CrDr = 1;
                }
                else if (item.Amount > 0)
                {
                    b.CrDr = 2;
                }
                else
                {
                    b.CrDr = 2;
                }

                b.Amount = item.Amount.Value;

                ob.Add(b);

            }
            //ob.Items = db.AcOpeningMasterSelectAll(1, 1).ToList();
            //return View(db.AcOpeningMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()),Convert.ToInt32(Session["CurrentCompanyID"].ToString())).ToList());

            return View(ob);
        }


        [HttpPost]
        public ActionResult IndexOpenningBalance(List<OpennnigBalanceVM> lst)
        {


            for (int i = 0; i < lst.Count; i++)
            {


                // int fyearid = 1;
                int AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());

                if (lst[i].CrDr == 1)
                {
                    db.AcOpeningMasterInsert(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToDateTime(Session["FyearFrom"].ToString()), lst[i].AcHeadID, -lst[i].Amount, Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
                }
                else
                {
                    db.AcOpeningMasterInsert(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToDateTime(Session["FyearFrom"].ToString()), lst[i].AcHeadID, lst[i].Amount, Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
                }

                //var a = (from x in db.AcOpeningMasters where ((x.StatusImport == null) && (x.AcFinancialYearID == fyearid) && (x.BranchID == AcCompanyID) && (x.AcHeadID == lst[i].AcHeadID)) select x);
                //if (a != null)
                //{
                //    db.AcOpeningMasters.Remove(a.FirstOrDefault());
                //    db.SaveChanges();
                //}

                //AcOpeningMaster aom = new AcOpeningMaster();
                //aom.AcFinancialYearID = fyearid;
                //aom.OPDate = Convert.ToDateTime("01 Jan 2015");
                //aom.AcHeadID = lst[i].AcHeadID;

                //if (lst[i].CrDr == "Cr")
                //{
                //    aom.Amount = -lst[i].Amount;
                //}

                //aom.BranchID = AcCompanyID;
                //aom.AcCompanyID = AcCompanyID;

                //db.AcOpeningMasters.Add(aom);
                //db.SaveChanges();

            }
            ViewBag.SuccessMsg = "Your Record is Successfully Added";
            return RedirectToAction("IndexOpenningBalance", db.AcOpeningMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())).ToList());
        }

        public ActionResult CreateAcBook()
        {
            var transtypes = new SelectList(new[]
                                        {
                                            new { ID = "1", trans = "Receipt" },
                                            new { ID = "2", trans = "Payment" },

                                        },
            "ID", "trans", 1);

            var paytypes = new SelectList(new[]{
                                            new { ID = "1", pay = "Cash" },
                                             new { ID = "2", pay = "Cheque" },
                                              new { ID = "3", pay = "Credit Card" },
                                               new { ID = "4", pay = "Bank Transfer" },
                                                new { ID = "5", pay = "Bank Deposit" },
                                        }, "ID", "pay", 1);

            var paymentterms = (from d in db.PaymentTerms select d).ToList();
            var paytypes1 = new SelectList(paymentterms, "PaymentTermID", "PaymentTerm1");
            ViewBag.transtypes = transtypes;
            ViewBag.paytypes = paytypes;
            //ViewBag.heads = db.AcHeadSelectForCash(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            //ViewBag.headsreceived = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));


            ViewBag.heads = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            ViewBag.headsreceived = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));




            return View();
        }



        public JsonResult ExpAllocation(decimal amount, int acheadid)
        {

            ViewBag.amt = amount;
            ViewBag.headid = acheadid;
            ViewBag.heads = db.AcHeads.ToList();
            string view = this.RenderPartialView("_ExpAllocate", null);

            return new JsonResult
            {
                Data = new
                {
                    success = true,
                    view = view
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }


        [HttpPost]
        public ActionResult CreateAcBook(AcBookVM v)
        {

            string cheque = "";
            string StatusTrans = "";
            int branchid=Convert.ToInt32(Session["CurrentBranchID"].ToString());
            if (v.paytype > 1)
            {
                cheque = v.chequeno;
            }
            else
            {
                cheque = "";
            }


            //string voucherno = "B123";
            int voucherno = 0;
            voucherno = (from c in db.AcJournalMasters select c).ToList().Count();

            int max = 0;
            max = (from c in db.AcJournalMasters orderby c.AcJournalID descending select c.AcJournalID).FirstOrDefault();

            int MaxId = 0;
            MaxId = (from c in db.AcJournalMasters orderby c.ID descending select c.ID).FirstOrDefault();

            AcJournalMaster ajm = new AcJournalMaster();
            ajm.AcJournalID = max + 1;
            ajm.VoucherNo = (voucherno + 1).ToString();
            ajm.TransDate = v.transdate;
            ajm.TransType = Convert.ToInt16(v.transtype);
            if (v.transtype == 1)
            {
                v.TransactionNo = "RE" + (max + 1).ToString().PadLeft(7, '0');
                //new { ID = "1", trans = "Receipt" },
                // new { ID = "2", trans = "Payment" },
            }
            else if (v.transtype == 2)
            {
                v.TransactionNo = "PA" + (max + 1).ToString().PadLeft(7, '0');
            }
            ajm.TransactionNo = v.TransactionNo;
            ajm.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
            ajm.VoucherType = v.TransactionType;
            ajm.StatusDelete = false;
            ajm.Remarks = v.remarks;
            ajm.UserID = Convert.ToInt32(Session["UserID"].ToString());
            ajm.ShiftID = null;
            ajm.PaymentType = v.paytype;

            //ajm.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            ajm.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            ajm.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            ajm.Reference = v.reference;

            db.AcJournalMasters.Add(ajm);
            db.SaveChanges();

            if (v.TransactionType == "CBR" || v.TransactionType == "BKR")
                StatusTrans = "R";
            else if (v.TransactionType == "CBP" || v.TransactionType == "BKP")
                StatusTrans = "P";

            if (v.chequeno != null)
            {

                var bankdetailid = (from c in db.AcBankDetails orderby c.AcBankDetailID descending select c.AcBankDetailID).FirstOrDefault();


                AcBankDetail acbankDetails = new AcBankDetail();
                acbankDetails.AcBankDetailID = bankdetailid + 1;
                acbankDetails.AcJournalID = ajm.AcJournalID;
                acbankDetails.BankName = v.bankname;
                acbankDetails.ChequeDate = v.chequedate;
                acbankDetails.ChequeNo = v.chequeno;
                acbankDetails.PartyName = v.partyname;
                acbankDetails.StatusTrans = StatusTrans;
                acbankDetails.StatusReconciled = false;
                if (acbankDetails.BankName == null)
                {
                    acbankDetails.BankName = "B";
                }
                if (acbankDetails.PartyName == null)
                {
                    acbankDetails.PartyName = "P";
                }
                AccountsDAO.InsertOrUpdateAcBankDetails(acbankDetails, 0);
            }

            decimal TotalAmt = 0;
            decimal totalTaxAmount = 0;
            for (int i = 0; i < v.AcJDetailVM.Count; i++)
            {
                if (v.AcJDetailVM[i].IsDeleted != true)
                {
                    if (v.AcJDetailVM[i].AmountIncludingTax == true && v.AcJDetailVM[i].TaxAmount > 0)
                    {
                        v.AcJDetailVM[i].Amt = v.AcJDetailVM[i].Amt - v.AcJDetailVM[i].TaxAmount;
                    }
                    TotalAmt = TotalAmt + Convert.ToDecimal(v.AcJDetailVM[i].Amt);
                }   totalTaxAmount = Convert.ToDecimal(totalTaxAmount) + Convert.ToDecimal(v.AcJDetailVM[i].TaxAmount);
            }


            AcJournalDetail ac = new AcJournalDetail();
            int maxAcJDetailID = 0;
            maxAcJDetailID = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();

            ac.AcJournalDetailID = maxAcJDetailID + 1;
            ac.AcJournalID = ajm.AcJournalID;
            ac.AcHeadID = v.SelectedAcHead;
            if (StatusTrans == "P")
            {
                ac.Amount = -(TotalAmt+totalTaxAmount);
            }
            else
            {
                ac.Amount = TotalAmt+totalTaxAmount;
            }
            ac.Remarks = v.AcJDetailVM[0].Rem;
            ac.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            db.AcJournalDetails.Add(ac);
            db.SaveChanges();


            //int maxAcJDetailID = 0;
          
            for (int i = 0; i < v.AcJDetailVM.Count; i++)
            {
                if (v.AcJDetailVM[i].IsDeleted != true)
                {
                    AcJournalDetail acJournalDetail = new AcJournalDetail();

                    maxAcJDetailID = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();

                    acJournalDetail.AcJournalDetailID = maxAcJDetailID + 1;
                    acJournalDetail.AcHeadID = v.AcJDetailVM[i].AcHeadID;

                    acJournalDetail.BranchID = Convert.ToInt32(Session["CurrentBranchID"]);
                    acJournalDetail.AcJournalID = ajm.AcJournalID;
                    acJournalDetail.Remarks = v.AcJDetailVM[i].Rem;
                    acJournalDetail.TaxPercent = v.AcJDetailVM[i].TaxPercent;
                    acJournalDetail.TaxAmount = v.AcJDetailVM[i].TaxAmount;
                 
                    acJournalDetail.SupplierId = v.AcJDetailVM[i].SupplierID;
                    acJournalDetail.AmountIncludingTax = v.AcJDetailVM[i].AmountIncludingTax;

                    //if (v.AcJDetailVM[i].AmountIncludingTax == true && v.AcJDetailVM[i].TaxAmount > 0)
                    //{
                    //    v.AcJDetailVM[i].Amt = v.AcJDetailVM[i].Amt - v.AcJDetailVM[i].TaxAmount;
                    //}
                    if (StatusTrans == "P")
                    {
                        acJournalDetail.Amount = (v.AcJDetailVM[i].Amt);
                    }
                    else
                    {
                        acJournalDetail.Amount = -v.AcJDetailVM[i].Amt;
                    }

                    db.AcJournalDetails.Add(acJournalDetail);
                    //  db.Entry(acJournalDetail).State = EntityState.Added;
                    db.SaveChanges();
                    db.Entry(acJournalDetail).State = EntityState.Detached;

                    if (v.AcJDetailVM[i].AcExpAllocationVM != null)
                    {
                        for (int j = 0; j < v.AcJDetailVM[i].AcExpAllocationVM.Count; j++)
                        {
                            AcAnalysisHeadAllocation objAcAnalysisHeadAllocation = new AcAnalysisHeadAllocation();
                            var maxid = (from c in db.AcAnalysisHeadAllocations orderby c.AcAnalysisHeadAllocationID descending select c.AcAnalysisHeadAllocationID).FirstOrDefault();
                            objAcAnalysisHeadAllocation.AcAnalysisHeadAllocationID = maxid + 1;
                            objAcAnalysisHeadAllocation.AcjournalDetailID = acJournalDetail.AcJournalDetailID;
                            objAcAnalysisHeadAllocation.AnalysisHeadID = v.AcJDetailVM[i].AcExpAllocationVM[j].AcHead;
                            objAcAnalysisHeadAllocation.Amount = v.AcJDetailVM[i].AcExpAllocationVM[j].ExpAllocatedAmount;
                            db.AcAnalysisHeadAllocations.Add(objAcAnalysisHeadAllocation);
                            db.SaveChanges();
                            db.Entry(objAcAnalysisHeadAllocation).State = EntityState.Detached;
                        }



                    }
                }
            }
            //Insert Tax Payable Account Ledger
            int? vataccountid = db.BranchMasters.Find(branchid).VATAccountId;
            if (vataccountid != null && totalTaxAmount > 0)
            {
                ac = new AcJournalDetail();
                maxAcJDetailID = 0;
                maxAcJDetailID = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();

                ac.AcJournalDetailID = maxAcJDetailID + 1;
                ac.AcJournalID = ajm.AcJournalID;
                ac.AcHeadID = vataccountid;
                if (StatusTrans == "P")
                {
                    ac.Amount = (totalTaxAmount);
                }
                else
                {
                    ac.Amount = -totalTaxAmount;
                }
                ac.Remarks = "Tax Payable";
                ac.BranchID = branchid;
                db.AcJournalDetails.Add(ac);
                db.SaveChanges();

            }

            ViewBag.SuccessMsg = "You have successfully added Record";
           // return View("IndexAcBook", db.AcJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())));
            return RedirectToAction("IndexAcBook");

        }

        public JsonResult AcBookDetails(int DetailId)
        {
            var lstAcJournalDetails = AccountsDAO.GetAcJournalDetails(DetailId);
            return Json(lstAcJournalDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditAcBook(AcBookVM v)
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            string cheque = "";
            string StatusTrans = "";

            if (v.paytype > 1)
            {
                cheque = v.chequeno;
            }
            else
            {
                cheque = "";
            }

            AcJournalMaster ajm = new AcJournalMaster();
            ajm.TransactionNo = v.TransactionNo;
            ajm.AcJournalID = v.AcJournalID;
            ajm.VoucherNo = v.VoucherNo;
            ajm.TransDate = v.transdate;
            ajm.TransType = Convert.ToInt16(v.transtype);
            ajm.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
            ajm.VoucherType = v.TransactionType;
            ajm.StatusDelete = false;
            ajm.Remarks = v.remarks;
            ajm.UserID = Convert.ToInt32(Session["UserID"].ToString()); 
            ajm.ShiftID = null;
            ajm.PaymentType = v.paytype;

            ajm.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            ajm.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            ajm.Reference = v.reference;

            db.Entry(ajm).State = EntityState.Modified;
            db.SaveChanges();

            if (v.TransactionType == "CBR" || v.TransactionType == "BKR")
                StatusTrans = "R";
            else if (v.TransactionType == "CBP" || v.TransactionType == "BKP")
                StatusTrans = "P";
            int maxBankDetailID = 0;
            int isexistbankdetail = 0;

            if (v.chequeno != null)
            {

                if (v.AcBankDetailID > 0)
                {
                    maxBankDetailID = v.AcBankDetailID;
                    isexistbankdetail = 1;
                }
                else
                {
                    var bankdetailid = (from c in db.AcBankDetails orderby c.AcBankDetailID descending select c.AcBankDetailID).FirstOrDefault();
                    v.AcBankDetailID = bankdetailid + 1;
                    isexistbankdetail = 0;

                }
                AcBankDetail acbankDetails = new AcBankDetail();
                acbankDetails.AcBankDetailID = v.AcBankDetailID;
                acbankDetails.BankName = v.bankname;
                acbankDetails.ChequeDate = v.chequedate;
                acbankDetails.ChequeNo = v.chequeno;
                acbankDetails.PartyName = v.partyname;
                acbankDetails.AcJournalID = ajm.AcJournalID;
                acbankDetails.StatusTrans = StatusTrans;
                acbankDetails.StatusReconciled = false;
                if (acbankDetails.BankName == null)
                {
                    acbankDetails.BankName = "B";
                }
                if (acbankDetails.PartyName == null)
                {
                    acbankDetails.PartyName = "P";
                }
                AccountsDAO.InsertOrUpdateAcBankDetails(acbankDetails, isexistbankdetail);
            }
            else
            {

            }

            decimal TotalAmt = 0;
            decimal totalTaxAmount = 0;
            for (int i = 0; i < v.AcJDetailVM.Count; i++)
            {
                if (v.AcJDetailVM[i].IsDeleted != true)
                {
                    if (v.AcJDetailVM[i].AmountIncludingTax == true && v.AcJDetailVM[i].TaxAmount > 0)
                    {
                        v.AcJDetailVM[i].Amt = v.AcJDetailVM[i].Amt - v.AcJDetailVM[i].TaxAmount;
                    }
                    TotalAmt = TotalAmt + Convert.ToDecimal(v.AcJDetailVM[i].Amt);
                    totalTaxAmount = Convert.ToDecimal(totalTaxAmount) + Convert.ToDecimal(v.AcJDetailVM[i].TaxAmount);
                }
            }
            var ac = (from c in db.AcJournalDetails where c.AcJournalID == ajm.AcJournalID select c).FirstOrDefault();
            ac.AcJournalDetailID = ac.AcJournalDetailID;
            ac.AcJournalID = ajm.AcJournalID;
            ac.AcHeadID = v.SelectedAcHead;
            if (StatusTrans == "P")
            {
                ac.Amount = -(TotalAmt+totalTaxAmount);
            }
            else
            {
                ac.Amount = TotalAmt+totalTaxAmount;
            }
            ac.Remarks = v.AcJDetailVM[0].Rem;
            ac.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            db.Entry(ac).State = EntityState.Modified;
            db.SaveChanges();

            int maxAcJDetailID = 0;
          
            for (int i = 0; i < v.AcJDetailVM.Count; i++)
            {
                if (v.AcJDetailVM[i].IsDeleted != true)
                {
                    AcJournalDetail acJournalDetail = new AcJournalDetail();
                    int IdExists = 0;
                    if (v.AcJDetailVM[i].AcJournalDetID > 0)
                    {
                        //  IdExists = (from c in db.AcJournalDetails where c.AcJournalDetailID == v.AcJDetailVM[i].AcJournalDetID select c.AcJournalDetailID).FirstOrDefault();
                        IdExists = v.AcJDetailVM[i].AcJournalDetID;
                    }
                    //  AcJournalDetID
                    if (IdExists > 0)
                    {
                        acJournalDetail.AcJournalDetailID = v.AcJDetailVM[i].AcJournalDetID;
                    }
                    else
                    {
                        maxAcJDetailID = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();
                        acJournalDetail.AcJournalDetailID = maxAcJDetailID + 1;
                    }
                    acJournalDetail.AcHeadID = v.AcJDetailVM[i].AcHeadID;
                    acJournalDetail.BranchID = Convert.ToInt32(Session["CurrentBranchID"]);
                    acJournalDetail.AcJournalID = ajm.AcJournalID;
                    if (v.AcJDetailVM[i].Rem == null)
                    {
                        acJournalDetail.Remarks = "";
                    }
                    else
                    {
                        acJournalDetail.Remarks = v.AcJDetailVM[i].Rem;
                    }
                    acJournalDetail.TaxPercent = v.AcJDetailVM[i].TaxPercent;
                    acJournalDetail.TaxAmount = v.AcJDetailVM[i].TaxAmount;                 
                    acJournalDetail.AmountIncludingTax = v.AcJDetailVM[i].AmountIncludingTax;
                    acJournalDetail.SupplierId = v.AcJDetailVM[i].SupplierID;
                    //if (v.AcJDetailVM[i].AmountIncludingTax == true && v.AcJDetailVM[i].TaxAmount > 0)
                    //{
                    //    v.AcJDetailVM[i].Amt = v.AcJDetailVM[i].Amt - v.AcJDetailVM[i].TaxAmount;
                    //}

                    if (StatusTrans == "P")
                    {
                        acJournalDetail.Amount = (v.AcJDetailVM[i].Amt);
                    }
                    else
                    {
                        acJournalDetail.Amount = -v.AcJDetailVM[i].Amt;
                    }
                    if (acJournalDetail.AnalysisHeadID == null)
                    {
                        acJournalDetail.AnalysisHeadID = 0;
                    }
                    if (IdExists > 0)
                    {
                        AccountsDAO.UpdateAcJournalDetail(acJournalDetail);
                    }
                    else
                    {
                        AccountsDAO.InsertAcJournalDetail(acJournalDetail);
                    }

                    if (v.AcJDetailVM[i].AcExpAllocationVM != null)
                    {
                        for (int k = 0; k < v.AcJDetailVM[i].AcExpAllocationVM.Count; k++)
                        {
                            Nullable<int> AllocationIdExists = 0;
                            AcAnalysisHeadAllocation objAcAnalysisHeadAllocations = new AcAnalysisHeadAllocation();
                            if (v.AcJDetailVM[i].AcExpAllocationVM[k].AcAnalysisHeadAllocationID != null && v.AcJDetailVM[i].AcExpAllocationVM[k].AcAnalysisHeadAllocationID > 0)
                            {
                                AllocationIdExists = v.AcJDetailVM[i].AcExpAllocationVM[k].AcAnalysisHeadAllocationID;
                            }
                            if (AllocationIdExists > 0)
                            {
                                objAcAnalysisHeadAllocations.AcAnalysisHeadAllocationID = (int)AllocationIdExists;
                            }
                            else
                            {
                                objAcAnalysisHeadAllocations.AcAnalysisHeadAllocationID = 0;
                            }
                            objAcAnalysisHeadAllocations.AcjournalDetailID = acJournalDetail.AcJournalDetailID;
                            objAcAnalysisHeadAllocations.Amount = v.AcJDetailVM[i].AcExpAllocationVM[k].ExpAllocatedAmount;
                            objAcAnalysisHeadAllocations.AnalysisHeadID = v.AcJDetailVM[i].AcExpAllocationVM[k].AcHead;
                            if (AllocationIdExists > 0)
                            {
                                AccountsDAO.UpdateAcAnalysisHeadAllocation(objAcAnalysisHeadAllocations);
                            }
                            else
                            {
                                AccountsDAO.InsertAcAnalysisHeadAllocation(objAcAnalysisHeadAllocations);
                            }
                            //  AcJournalDetID
                        }
                    }
                }
            }
            //
            //Insert Tax Payable Account Ledger
            try
            {
                int? vataccountid = db.BranchMasters.Find(branchid).VATAccountId;
                if (vataccountid != null && totalTaxAmount > 0)
                {
                    bool newentry = false;
                    ac = new AcJournalDetail();
                    ac = db.AcJournalDetails.Where(cc => cc.AcJournalID == v.AcJournalID && cc.AcHeadID == vataccountid).FirstOrDefault();
                    if (ac == null)
                    {
                        ac = new AcJournalDetail();
                        maxAcJDetailID = 0;
                        maxAcJDetailID = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();
                        ac.AcJournalDetailID = maxAcJDetailID + 1;
                        ac.AcJournalID = ajm.AcJournalID;
                        ac.AcHeadID = vataccountid;
                        newentry = true;
                    }

                    if (StatusTrans == "P")
                    {
                        ac.Amount = (totalTaxAmount);
                    }
                    else
                    {
                        ac.Amount = -totalTaxAmount;
                    }
                    ac.Remarks = "Tax Payable";
                    ac.BranchID = branchid;
                    if (newentry)
                    {
                        db.AcJournalDetails.Add(ac);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(ac).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
            catch (Exception ex2)
            {

            }

            string DeleteJournalDetails = Request["deletedJournalDetails"];
            string[] DeleteJournalDetailsArr = DeleteJournalDetails.Split(',');
            foreach (string JournalDetails in DeleteJournalDetailsArr)
            {
                int iDeleteJournalDetails = 0;
                int.TryParse(JournalDetails, out iDeleteJournalDetails);
                AccountsDAO.DeleteAcJournalDetail(iDeleteJournalDetails);
            }
            string DeleteAcAnalysisHeadAllocation = Request["deletedExpAllocations"];
            string[] DeleteAcAnalysisHeadAllocationArr = DeleteAcAnalysisHeadAllocation.Split(',');
            foreach (string AcAnalysisHeadAllocation in DeleteAcAnalysisHeadAllocationArr)
            {
                int iAcAnalysisHeadAllocation = 0;
                int.TryParse(AcAnalysisHeadAllocation, out iAcAnalysisHeadAllocation);
                AccountsDAO.DeleteAcAnalysisHeadAllocation(iAcAnalysisHeadAllocation);
            }
            ViewBag.SuccessMsg = "You have successfully added Record";
            return RedirectToAction("IndexAcBook");// View("IndexAcBook", db.AcJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())));
            //return View("IndexAcBook", db.AcJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())));
            //string cheque = "";
            //string StatusTrans = "";

            ////if (v.paytype > 1)
            ////{
            ////    cheque = v.chequeno;
            ////}
            ////else
            ////{
            ////    cheque = "";
            ////}


            ////string voucherno = "B123";
            ////int voucherno = 0;
            ////voucherno = (from c in db.AcJournalMasters select c).ToList().Count();



            //AcJournalMaster ajm = new AcJournalMaster();
            //ajm.AcJournalID = v.AcJournalID;
            //ajm.VoucherNo = v.VoucherNo;
            //ajm.TransDate = v.transdate;
            //ajm.TransType = Convert.ToInt16(v.transtype);
            //ajm.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
            ////ajm.VoucherType = v.TransactionType;
            //ajm.VoucherType = v.VoucherType;

            //ajm.StatusDelete = false;
            //ajm.Remarks = v.remarks;
            //ajm.UserID = Convert.ToInt32(Session["UserID"].ToString());
            //ajm.ShiftID = null;

            ////ajm.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            //ajm.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());
            //ajm.Reference = v.reference;

            //db.Entry(ajm).State = EntityState.Modified;
            //db.SaveChanges();

            ////if (v.TransactionType == "CBR" || v.TransactionType == "BKR")
            ////    StatusTrans = "R";
            ////else if (v.TransactionType == "CBP" || v.TransactionType == "BKP")
            ////    StatusTrans = "P";

            //if (v.VoucherType == "CBR" || v.VoucherType == "BKR")
            //{
            //    StatusTrans = "R";
            //}
            //else if (v.VoucherType == "CBP" || v.VoucherType == "BKP")
            //{
            //    StatusTrans = "P";
            //}

            //if (v.chequeno != null)
            //{
            //    AcBankDetail acbankDetails = new AcBankDetail();
            //    acbankDetails.AcBankDetailID = v.AcBankDetailID;
            //    acbankDetails.AcJournalID = ajm.AcJournalID;
            //    acbankDetails.BankName = v.bankname;
            //    acbankDetails.ChequeDate = v.chequedate;
            //    acbankDetails.ChequeNo = v.chequeno;
            //    acbankDetails.PartyName = v.partyname;
            //    acbankDetails.StatusTrans = StatusTrans;
            //    acbankDetails.StatusReconciled = false;
            //    db.Entry(acbankDetails).State = EntityState.Modified;
            //    db.SaveChanges();
            //}



            ////AcJournalDetail ac = new AcJournalDetail();
            ////ac.AcJournalID = ajm.AcJournalID;
            ////ac.AcHeadID = v.AcHead;
            ////if (StatusTrans == "P")
            ////{
            ////    ac.Amount = -(v.TotalAmt);
            ////}
            ////else
            ////{
            ////    ac.Amount = v.TotalAmt;
            ////}
            ////ac.Remarks = v.remarks;
            ////ac.BranchID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());

            ////db.AcJournalDetails.Add(ac);
            ////db.SaveChanges();



            //for (int i = 0; i < v.AcJDetailVM.Count; i++)
            //{
            //    AcJournalDetail acJournalDetail = new AcJournalDetail();
            //    acJournalDetail.AcHeadID = v.AcJDetailVM[i].AcHeadID;
            //    acJournalDetail.AcJournalID = ajm.AcJournalID;
            //    acJournalDetail.BranchID = Convert.ToInt32(Session["CurrentCompanyID"]);
            //    acJournalDetail.AcJournalDetailID = v.AcJDetailVM[i].AcJournalDetID;
            //    acJournalDetail.Remarks = v.AcJDetailVM[i].Rem;

            //    if (StatusTrans == "P")
            //    {
            //        acJournalDetail.Amount = -(v.AcJDetailVM[i].Amt);
            //    }
            //    else
            //    {
            //        acJournalDetail.Amount = v.AcJDetailVM[i].Amt;
            //    }

            //    db.Entry(acJournalDetail).State = EntityState.Modified;
            //    db.SaveChanges();

            //}


            //ViewBag.SuccessMsg = "You have successfully added Record";
            //return View("IndexAcBook", db.AcJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())));
        }

        public ActionResult EditAcBook(int id)
        {
            AcBookVM v = new AcBookVM();

            AcJournalMaster ajm = db.AcJournalMasters.Find(id);
            AcBankDetail abank = (from a in db.AcBankDetails where a.AcJournalID == id select a).FirstOrDefault();
            v.TransactionNo = ajm.TransactionNo;
            v.transdate = ajm.TransDate.Value;
            v.SelectedAcHead = (from c in db.AcJournalDetails where c.AcJournalID == ajm.AcJournalID select c.AcHeadID).FirstOrDefault();
            v.AcHead = (from c in db.AcHeads where c.AcHeadID == v.SelectedAcHead select c.AcHead1).FirstOrDefault();
            v.remarks = ajm.Remarks;
            v.reference = ajm.Reference;
            v.VoucherType = ajm.VoucherType;
            v.AcJournalID = ajm.AcJournalID;
            v.VoucherNo = ajm.VoucherNo;
            v.TransactionType = v.VoucherType;
            v.paytype = Convert.ToInt16(ajm.PaymentType);
            v.transtype = Convert.ToInt32(ajm.TransType);



            if (abank != null)
            {
                v.AcBankDetailID = abank.AcBankDetailID;
                v.bankname = abank.BankName;
                v.partyname = abank.PartyName;
                v.chequedate = abank.ChequeDate.Value;
                v.chequeno = abank.ChequeNo;
            }



            var transtypes = new SelectList(new[]
                                        {
                                            new { ID = "1", trans = "Receipt" },
                                            new { ID = "2", trans = "Payment" },

                                        },
           "ID", "trans", 1);
            var paytypes = new SelectList(new[]{
                                            new { ID = "1", pay = "Cash" },
                                             new { ID = "2", pay = "Cheque" },
                                              new { ID = "3", pay = "Credit Card" },
                                               new { ID = "4", pay = "Bank Transfer" },
                                                new { ID = "5", pay = "Bank Deposit" },
                                        }, "ID", "pay", 1);
            var paymentterms = (from d in db.PaymentTerms select d).ToList();
            var paytypes1 = new SelectList(paymentterms, "PaymentTermID", "PaymentTerm1");
            ViewBag.transtypes = transtypes;
            ViewBag.paytypes = paytypes;
            if (v.VoucherType == "CBR" || v.VoucherType == "CBP")
            {
                var data = db.AcHeadSelectForCash(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
                //ViewBag.heads = db.AcHeadSelectForCash(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
                ViewBag.heads = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            }
            else if (v.VoucherType == "BKP" || v.VoucherType == "BKR" || v.VoucherType == "RP" || v.VoucherType == "BK")
            {
                //ViewBag.heads = db.AcHeadSelectForBank(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
                ViewBag.heads = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            }
            ViewBag.headsreceived = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));


            return View(v);


        }




        public JsonResult GetAcJDetails(Nullable<int> id, int? transtype)
        {
            //var acjlist = (from c in db.AcJournalDetails where c.AcJournalID == id select c).ToList();

            string TransType = "";
            if (transtype == 1)
            {
                TransType = "R";
            }
            else
            {
                TransType = "P";
            }

            //var acjlist = db.AcJournalDetailSelectByAcJournalID(id, TransType).ToList();i


            List<AcJournalDetailVM> AcJDetailVM = new List<AcJournalDetailVM>();
            AcJDetailVM = AccountsDAO.AcJournalDetailSelectByAcJournalID(Convert.ToInt32(id), TransType);


            //foreach (var item in acjlist)
            //{
            //    AcJournalDetailVM v = new AcJournalDetailVM();
            //    string x = (from a in db.AcHeads where a.AcHeadID == item.AcHeadID select a.AcHead1).FirstOrDefault();

            //    v.AcHeadID = item.AcHeadID.Value;
            //    v.AcHead = x;

            //    if (item.Amount < 0)
            //    {
            //        v.Amt = (-item.Amount.Value);
            //    }
            //    else
            //    {
            //        v.Amt = item.Amount.Value;
            //    }
            //    v.Rem = item.Remarks;
            //    v.TaxAmount=item.tax
            //    v.AcJournalDetID = item.AcJournalDetailID;
            //    AcJDetailVM.Add(v);
            //}

            return Json(AcJDetailVM, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAcJDetailsExpenseAllocation(int AcJournalDetailID)
        {
            //  AcAnalysisHeadAllocation objAcAnalysisHeadAllocation = new AcAnalysisHeadAllocation();
            //       objAcAnalysisHeadAllocation.AcjournalDetailID = acJournalDetail.AcJournalDetailID;

            var acjlist = AccountsDAO.GetAcJDetailsExpenseAllocation(AcJournalDetailID);

            //(from a in db.AcAnalysisHeadAllocations where a.AcjournalDetailID == AcJournalDetailID select a).ToList();


            return Json(acjlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadsForCash()
        {

            int AcCompanyID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            //List<AcHeadSelectForCash_Result> x = null;

            //x = db.AcHeadSelectForCash(Convert.ToInt32(Session["CurrentCompanyID"].ToString())).ToList();


            List<AcHeadSelectAll_Result> x = null;
            //x = db.AcHeadSelectAll(AcCompanyID).ToList();
            var x1 = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();



            return Json(x1, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetHeadsForBank()
        {
            int AcCompanyID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            //List<AcHeadSelectForBank_Result> x = null;

            //x = db.AcHeadSelectForBank(Convert.ToInt32(Session["CurrentCompanyID"].ToString())).ToList();


            //List<AcHeadSelectAll_Result> x = null;
            //x = db.AcHeadSelectAll(AcCompanyID).ToList();
            var x1 = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
            return Json(x1, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetBalance(int acheadid)
        {
            var x = db.GetAccountBalanceByHeadID(acheadid, Convert.ToInt32(Session["fyearid"].ToString()));
            return Json(x, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AccountHead(string term)
        {
            int branchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            if (!String.IsNullOrEmpty(term))
            {
                List<AcHeadSelectAllVM> AccountHeadList = new List<AcHeadSelectAllVM>();
                AccountHeadList = AccountsDAO.GetAcHeadSelectAll(branchID).Where(c => c.AcHead.ToLower().Contains(term.ToLower())).OrderBy(x => x.AcHead).ToList(); ;

                //List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
                //AccountHeadList =db.AcHeadSelectAll(branchID).Where(c => c.AcHead.ToLower().Contains(term.ToLower())).OrderBy(x => x.AcHead).ToList();
                return Json(AccountHeadList, JsonRequestBehavior.AllowGet);

                //List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
                //AccountHeadList = MM.AcHeadSelectAll(Common.ParseInt(Session["CurrentBranchID"].ToString()), term);

            }
            else
            {
                List<AcHeadSelectAllVM> AccountHeadList = new List<AcHeadSelectAllVM>();
                AccountHeadList = AccountsDAO.GetAcHeadSelectAll(branchID);
                //List<AcHeadSelectAll_Result> AccountHeadList = new List<AcHeadSelectAll_Result>();
                //AccountHeadList = db.AcHeadSelectAll(branchID).ToList();
                return Json(AccountHeadList, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Supplier(string term)
        {
            int branchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            if (!String.IsNullOrEmpty(term))
            {
                List<SupplierMasterVM> supplierlist = new List<SupplierMasterVM>();
                supplierlist = (from c in db.SupplierMasters where c.SupplierName.ToLower().StartsWith(term.ToLower()) orderby c.SupplierName select new SupplierMasterVM { SupplierID = c.SupplierID, SupplierInfo = c.SupplierName + "( " + c.RegistrationNo + ")" }).ToList();
                
                return Json(supplierlist, JsonRequestBehavior.AllowGet);


            }
            else
            {
                List<SupplierMasterVM> supplierlist = new List<SupplierMasterVM>();
                supplierlist = (from c in db.SupplierMasters  orderby c.SupplierName select new SupplierMasterVM { SupplierID = c.SupplierID, SupplierInfo = c.SupplierName +  "( " + c.RegistrationNo + ")" }).ToList();
                return Json(supplierlist, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckDateValidate(string entrydate)
        {
          int fyearid= Convert.ToInt32(Session["fyearid"].ToString());
            StatusModel result= AccountsDAO.CheckDateValidate(entrydate, fyearid);
            return Json(result, JsonRequestBehavior.AllowGet);

      }


        #region BankReconciliation

        //Bank Reconciliation
        public ActionResult BankReconcilation()
        {

            int AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());

            //ViewBag.Data = db.AcHeadSelectForBank(AcCompanyID);

            ViewBag.Data = db.AcHeadSelectAll(AcCompanyID);

            //List<BankReconcilVM> lst = new List<BankReconcilVM>();

            //var data = (from c in db.AcBankDetails where (c.StatusReconciled == false) select c).ToList();

            //foreach (var item in data)
            //{
            //    BankReconcilVM v = new BankReconcilVM();
            //    v.AcBankDetailID = item.AcBankDetailID;
            //    v.AcJournalID = item.AcJournalID.Value;
            //    v.BankName = item.BankName;
            //    v.ChequeNo = item.ChequeNo;
            //    if (item.ChequeDate.HasValue)
            //        v.ChequeDate = item.ChequeDate.Value;
            //    v.PartyName = item.PartyName;
            //    v.StatusTrans = item.StatusTrans;
            //    if (item.StatusReconciled.HasValue)
            //        v.StatusReconciled = item.StatusReconciled.Value;
            //    v.ValueDate = Convert.ToDateTime(item.ValueDate);
            //    v.IsSelected = false;
            //    lst.Add(v);


            //}

            //return View(lst);

            return View();

        }

        //[HttpPost]
        //public ActionResult GetBankReconciliation(List<CMSV2.Models.BankReconcilVM> lst)
        //{

        //    //Update AcBankDetails Table 
        //    var selectedrecords = lst.Where(item => item.IsSelected == true).ToList();
        //    foreach (var item in selectedrecords)
        //    {
        //        AcBankDetail a = (from c in db.AcBankDetails where c.AcBankDetailID == item.AcBankDetailID select c).FirstOrDefault();
        //        a.ValueDate = item.ValueDate;
        //        a.StatusReconciled = true;
        //        db.Entry(a).State = EntityState.Modified;
        //        db.SaveChanges();

        //    } 
        //    return RedirectToAction("BankReconcilation");

        //}


        public JsonResult ShowBankReconciliation(string acheadid, string from, string to)
        {

            int vacheadid = 0;

            if (acheadid != null)
            {
                vacheadid = Convert.ToInt32(acheadid);
            }
            else
            {
                vacheadid = 0;
            }

            DateTime frm = Convert.ToDateTime(from);
            DateTime dto = Convert.ToDateTime(to);

            var data = db.GetBankReconciliationOutStandings(vacheadid, frm, dto);



            List<BankReconcilVM> lst = new List<BankReconcilVM>();



            foreach (var item in data)
            {
                BankReconcilVM v = new BankReconcilVM();
                v.AcBankDetailID = item.AcBankDetailID;
                v.AcJournalID = item.AcJournalID;
                v.BankName = item.BankName;
                v.ChequeNo = item.ChequeNo;
                if (item.ChequeDate.HasValue)
                    v.ChequeDate = item.ChequeDate.Value;
                v.PartyName = item.PartyName;
                v.StatusReconciled = false;
                v.Remarks = item.Remarks;
                v.AcHead = item.AcHead;
                v.VoucherNo = item.VoucherNo;
                v.VoucherDate = item.TransDate.Value;
                v.Amount = item.Amount.Value;

                v.IsSelected = false;
                lst.Add(v);


            }

            lst = lst.ToList();

            string view = this.RenderPartialView("GetBankReconciliation", lst);
            return new JsonResult
            {
                Data = new
                {
                    success = true,
                    view = view
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }


        [HttpPost]
        public ActionResult ShowBankReconciliation(List<CMSV2.Models.BankReconcilVM> lst)
        {
            var selectedrecords = lst.Where(item => item.IsSelected == true).ToList();
            foreach (var item in selectedrecords)
            {
                AcBankDetail b = (from c in db.AcBankDetails where c.AcBankDetailID == item.AcBankDetailID select c).FirstOrDefault();
                b.ValueDate = item.ValueDate;
                b.StatusReconciled = true;

                db.Entry(b).State = EntityState.Modified;
                db.SaveChanges();

            }
            return RedirectToAction("BankReconcilation");
        }

        //public ActionResult GetBankReconciliation(string acheadid="325", string from="01 Jan 2016", string to="31 Dec 2016")
        //{

        //    int vacheadid = 0;

        //    if (acheadid != null)
        //    {
        //        vacheadid = Convert.ToInt32(acheadid);
        //    }
        //    else
        //    {
        //        vacheadid = 0;
        //    }

        //    DateTime frm = Convert.ToDateTime(from);
        //    DateTime dto = Convert.ToDateTime(to);

        //    var data = db.GetBankReconciliationOutStandings(vacheadid, frm, dto);



        //    List<BankReconcilVM> lst = new List<BankReconcilVM>();



        //    foreach (var item in data)
        //    {
        //        BankReconcilVM v = new BankReconcilVM();

        //        v.AcJournalID = item.AcJournalID;
        //        v.BankName = item.BankName;
        //        v.ChequeNo = item.ChequeNo;
        //        if (item.ChequeDate.HasValue)
        //            v.ChequeDate = item.ChequeDate.Value;
        //        v.PartyName = item.PartyName;
        //        v.StatusReconciled = false;
        //        v.Remarks = item.Remarks;
        //        v.VoucherDate = item.TransDate.Value;
        //        v.Amount = item.Amount.Value;

        //        v.IsSelected = false;
        //        lst.Add(v);


        //    }

        //    return View(lst.ToList());

        //}



        #endregion BankReconciliation



        #region PDCTransaction

        public ActionResult IndexPDCTransaction()
        {

            var data = db.AcMemoJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentBranchID"].ToString())).ToList();

            return View(data);
        }

        public ActionResult CreatePDCTransaction()
        {
            var transtypes = new SelectList(new[]
                                        {
                                            new { ID = "1", trans = "Receipt" },
                                            new { ID = "2", trans = "Payment" },

                                        },
                                   "ID", "trans", 1);

            ViewBag.transtypes = transtypes;

            //ViewBag.heads = db.AcHeadSelectForBank(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            var x1 = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
            ViewBag.heads = x1; // db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString())).OrderBy(cc=>cc.AcHead);
            ViewBag.headsreceived = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString())).OrderBy(cc=>cc.AcHead);


            return View();

        }


        [HttpPost]
        public ActionResult CreatePDCTransaction(PDCVM pdctrans)
        {

            string StatusTrans = "";

            if (pdctrans.transtype == 1)
                StatusTrans = "R";
            else
                StatusTrans = "P";

            //string Vouchern = (from c in db.AcMemoJournalMasters select c).FirstOrDefault();
            //string vno = "";
            //if (Vouchern == "")
            //{
            //    vno = "PD-" + 1;
            //}
            //else
            //{
            //    vno = "PD-" + Convert.ToInt32(Vouchern) + 1;
            //}

            AcMemoJournalMaster acm = new AcMemoJournalMaster();

            acm.VoucherNo = "PD-125";
            acm.TransDate = pdctrans.transdate;
            acm.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
            acm.VoucherType = pdctrans.TransactionType;
            acm.StatusDelete = false;
            acm.Remarks = pdctrans.remarks;
            acm.UserID = Convert.ToInt32(Session["UserID"].ToString());
            acm.AcCompanyID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            //acm.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            db.AcMemoJournalMasters.Add(acm);
            db.SaveChanges();


            if (pdctrans.chequeno.Length > 0)
            {
                AcMemoBankDetail acmbank = new AcMemoBankDetail();
                acmbank.AcMemoBankDetailID = GetMaxAcMemoBankDetailNumber();
                acmbank.AcMemoJournalID = acm.AcMemoJournalID;
                acmbank.BankName = pdctrans.bankname;
                acmbank.ChequeNo = pdctrans.chequeno;
                acmbank.ChequeDate = pdctrans.chequedate;
                acmbank.PartyName = pdctrans.partyname;
                acmbank.StatusTrans = StatusTrans;

                db.AcMemoBankDetails.Add(acmbank);
                db.SaveChanges();

            }

            decimal total = 0;
            for (int i = 0; i < pdctrans.AcJMDetailVM.Count; i++)
            {
                total = total + pdctrans.AcJMDetailVM[i].Amt;
            }



            AcMemoJournalDetail acmd = new AcMemoJournalDetail();
            acmd.AcMemoJournalID = acm.AcMemoJournalID;
            acmd.AcHeadID = pdctrans.AcHead;
            if (StatusTrans == "P")
            {
                acmd.Amount = -total;
            }
            else
            {
                acmd.Amount = total;
            }
            acmd.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            db.AcMemoJournalDetails.Add(acmd);
            db.SaveChanges();

            for (int i = 0; i < pdctrans.AcJMDetailVM.Count; i++)
            {
                if (pdctrans.AcJMDetailVM[i].IsDeleted!=true)
                {
                    AcMemoJournalDetail a = new AcMemoJournalDetail();
                    a.AcMemoJournalID = acm.AcMemoJournalID;
                    a.AcHeadID = pdctrans.AcJMDetailVM[i].AcHeadID;

                    if (StatusTrans == "P")
                    {
                        a.Amount = pdctrans.AcJMDetailVM[i].Amt;
                    }
                    else
                    {
                        a.Amount = -pdctrans.AcJMDetailVM[i].Amt;
                    }

                    a.Remarks = pdctrans.AcJMDetailVM[i].Rem;
                    a.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());

                    db.AcMemoJournalDetails.Add(a);
                    db.SaveChanges();
                }
            }

            ViewBag.SuccessMsg = "You have successfully added Record";
            return RedirectToAction("IndexPDCTransaction");

            //return View("IndexPDCTransaction", db.AcMemoJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())).ToList());


        }


        public JsonResult GetAcMemoJDeatils(int id)
        {
            var acjlist = (from c in db.AcMemoJournalDetails where c.AcMemoJournalID == id orderby c.AcMemoJournalDetailID ascending select c).Skip(1).ToList();
            List<AcMemoJournalDetailVM> AcJDetailVM = new List<AcMemoJournalDetailVM>();
            foreach (var item in acjlist)
            {
                AcMemoJournalDetailVM v = new AcMemoJournalDetailVM();
                string x = (from a in db.AcHeads where a.AcHeadID == item.AcHeadID select a.AcHead1).FirstOrDefault();

                v.AcHeadID = item.AcHeadID.Value;
                v.AcHead = x;

                if (item.Amount < 0)
                {
                    v.Amt = (-item.Amount.Value);
                }
                else
                {
                    v.Amt = item.Amount.Value;
                }
                v.Rem = item.Remarks;
                v.AcMemoDetailID = item.AcMemoJournalDetailID;
                AcJDetailVM.Add(v);
            }

            return Json(AcJDetailVM, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditPDC(int id)
        {
            var transtypes = new SelectList(new[]
                                        {
                                            new { ID = "1", trans = "Receipt" },
                                            new { ID = "2", trans = "Payment" },

                                        },
                                 "ID", "trans", 1);

            ViewBag.transtypes = transtypes;
            var x1 = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
            //ViewBag.heads = db.AcHeadSelectForBank(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            ViewBag.heads = x1; // db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            ViewBag.headsreceived = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));


            PDCVM v = new PDCVM();
            AcMemoJournalMaster ajm = db.AcMemoJournalMasters.Find(id);
            AcMemoBankDetail acb = (from a in db.AcMemoBankDetails where a.AcMemoJournalID == id select a).FirstOrDefault();
            v.AcHead = (from c in db.AcMemoJournalDetails where c.AcMemoJournalID == ajm.AcMemoJournalID select c.AcHeadID).FirstOrDefault().Value;
            v.AcJournalID = ajm.AcMemoJournalID;
            v.AcBankDetailID = acb.AcMemoBankDetailID;
            if (acb.StatusTrans == "P")
            {
                v.transtype = 2;
            }
            else
            {
                v.transtype = 1;
            }
            v.transdate = ajm.TransDate.Value;
            v.remarks = ajm.Remarks;

            v.bankname = acb.BankName;
            v.chequeno = acb.ChequeNo;
            v.chequedate = acb.ChequeDate.Value;
            v.partyname = acb.PartyName;

            v.VoucherNo = ajm.VoucherNo;
            v.TransactionType = ajm.VoucherType;

            return View(v);

        }


        [HttpPost]
        public ActionResult EditPDC(PDCVM pdctrans)
        {
            string StatusTrans = "";

            if (pdctrans.transtype == 1)
                StatusTrans = "R";
            else
                StatusTrans = "P";


            AcMemoJournalMaster acm = new AcMemoJournalMaster();
            acm.AcMemoJournalID = pdctrans.AcJournalID;
            acm.AcJournalID = null;
            acm.VoucherNo = "PD-125";
            acm.TransDate = pdctrans.transdate;
            acm.AcFinancialYearID = Convert.ToInt32(Session["fyearid"].ToString());
            acm.VoucherType = pdctrans.TransactionType;
            acm.StatusDelete = false;
            acm.Remarks = pdctrans.remarks;
            acm.UserID = Convert.ToInt32(Session["UserID"].ToString());
            acm.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());

            db.Entry(acm).State = EntityState.Modified;
            db.SaveChanges();


            if (pdctrans.chequeno.Length > 0)
            {
                AcMemoBankDetail acmbank = new AcMemoBankDetail();
                acmbank.AcMemoBankDetailID = pdctrans.AcBankDetailID;
                acmbank.AcMemoJournalID = acm.AcMemoJournalID;
                acmbank.BankName = pdctrans.bankname;
                acmbank.ChequeNo = pdctrans.chequeno;
                acmbank.ChequeDate = pdctrans.chequedate;
                acmbank.PartyName = pdctrans.partyname;
                acmbank.StatusTrans = StatusTrans;

                db.Entry(acmbank).State = EntityState.Modified;
                db.SaveChanges();

            }

            var x = (from c in db.AcMemoJournalDetails where c.AcMemoJournalID == acm.AcMemoJournalID select c).ToList();

            foreach (var i in x)
            {
                db.AcMemoJournalDetails.Remove(i);
                db.SaveChanges();
            }

            decimal total = 0;
            for (int i = 0; i < pdctrans.AcJMDetailVM.Count; i++)
            {
                total = total + pdctrans.AcJMDetailVM[i].Amt;
            }


            AcMemoJournalDetail acmd = new AcMemoJournalDetail();

            acmd.AcMemoJournalID = acm.AcMemoJournalID;
            acmd.AcHeadID = pdctrans.AcHead;
            acmd.Amount = total * (-1);
            acmd.BranchID = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            acmd.Remarks = acm.Remarks;
            db.AcMemoJournalDetails.Add(acmd);
            db.SaveChanges();



            for (int i = 0; i < pdctrans.AcJMDetailVM.Count; i++)
            {
                if (pdctrans.AcJMDetailVM[i].IsDeleted!=true)
                {
                    AcMemoJournalDetail a = new AcMemoJournalDetail();

                    a.AcMemoJournalID = acm.AcMemoJournalID;
                    a.AcHeadID = pdctrans.AcJMDetailVM[i].AcHeadID;
                    a.Amount = pdctrans.AcJMDetailVM[i].Amt;
                    a.Remarks = pdctrans.AcJMDetailVM[i].Rem;
                    a.BranchID = Convert.ToInt32(Session["CurrentCompanyID"].ToString());

                    db.AcMemoJournalDetails.Add(a);
                    db.SaveChanges();
                }
            }

            ViewBag.SuccessMsg = "You have successfully updated Record";
            return View("IndexPDCTransaction", db.AcMemoJournalMasterSelectAll(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString())).ToList());
        }

        public int GetMaxAcMemoBankDetailNumber()
        {

            var query = db.AcMemoBankDetails.OrderByDescending(item => item.AcMemoBankDetailID).FirstOrDefault();

            if (query == null)
            {
                return 1;
            }
            else
            {
                return query.AcMemoBankDetailID + 1;
            }


        }
        #endregion PDCTransaction


        #region PDCOutstandings

        public ActionResult IndexPDCOutstandings()
        {

            return View();
        }


        public JsonResult GetPDCOutstandings(DateTime iMatureDate)
        {
            List<PDCOutstandingVM> objPDCOutstandingVMList = new List<PDCOutstandingVM>();
            var pdcreminder = db.GetPDCReminder(iMatureDate, 1, Convert.ToInt32(Session["CurrentCompanyID"].ToString())).ToList();
            foreach (var item in pdcreminder)
            {
                PDCOutstandingVM objPDCOutstandingVM = new PDCOutstandingVM();
                objPDCOutstandingVM.AcHead = item.AcHead;
                objPDCOutstandingVM.Amount = item.Amount.Value;
                objPDCOutstandingVM.VoucherNo = item.VoucherNo;
                objPDCOutstandingVM.VoucherDate = item.TransDate.Value;
                objPDCOutstandingVM.ChequeNo = item.ChequeNo;
                objPDCOutstandingVM.ChequeDate = item.ChequeDate.Value;
                objPDCOutstandingVM.AcMemoJournalID = item.AcMemoJournalID;
                objPDCOutstandingVM.IsSelected = false;
                objPDCOutstandingVMList.Add(objPDCOutstandingVM);

            }

            var view = this.RenderPartialView2("ucPDCOutstandings", objPDCOutstandingVMList);
            return new JsonResult
            {
                Data = new
                {
                    success = true,
                    view = view
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public ActionResult IndexPDCOutstandings(List<PDCOutstandingVM> iPDCOutstandingVM)
        {
            foreach (var item in iPDCOutstandingVM)
            {

            }
            return RedirectToAction("IndexPDCOutstandings");
        }


        #endregion PDCOutstandings

        #region "AccountReport"
        public ActionResult AccountsReport()
        {
            return View();
        }
        #endregion

        //public JsonResult YearEndProcess()
        //{
        //    YearEndProcessVM v = new YearEndProcessVM();
        //    v.CurrentFYearFrom = (from c in db.AcFinancialYears where c.AcFinancialYearID == Convert.ToInt32(Session["fyearid"].ToString()) select c.AcFYearFrom).FirstOrDefault().Value;
        //    v.CurrentFYearTo = v.CurrentFYearFrom = (from c in db.AcFinancialYears where c.AcFinancialYearID == Convert.ToInt32(Session["fyearid"].ToString()) select c.AcFYearTo).FirstOrDefault().Value;

        //    v.NewFYearFrom = v.CurrentFYearFrom.AddDays(1);
        //    v.NewFYearTo = v.CurrentFYearTo.AddDays(1);

        //    v.Reference = v.CurrentFYearFrom.AddYears(1).Year.ToString() + "-" + v.CurrentFYearTo.AddYears(1).Year.ToString();

        //    return Json(v, JsonRequestBehavior.AllowGet);

        //}




        public ActionResult YearEndProcess()
        {
            ViewBag.currentFyearFrom = Convert.ToDateTime(Session["FyearFrom"].ToString()).ToString("dd/MM/yyyy");
            ViewBag.currentFyearTo = Convert.ToDateTime(Session["FyearTo"].ToString()).ToString("dd/MM/yyyy");

            return View();
        }


        public JsonResult GetNewFYear(string cFyearFrom, string cFyearTo)
        {
            YearEndProcessVM v = new YearEndProcessVM();
            //using (StreamWriter _logData = new StreamWriter(System.Web.Hosting.HostingEnvironment.MapPath("~/Logyearend.txt"), true))
            //{
            //try
            //{
            //_logData.WriteLine("Fyear :" + cFyearFrom);
            //_logData.WriteLine("toyear :" + cFyearTo);


            v.CurrentFYearFrom = cFyearFrom;
            v.CurrentFYearTo = cFyearTo;

            var fdate = cFyearFrom.Split('/');
            var tdate = cFyearTo.Split('/');
            if (Convert.ToInt32(fdate[0]) > 12)
            {
                cFyearFrom = fdate[1] + "/" + fdate[0] + "/" + fdate[2];

            }
            if (Convert.ToInt32(tdate[0]) > 12)
            {
                cFyearTo = tdate[1] + "/" + tdate[0] + "/" + tdate[2];

            }
            DateTime tnewfyear = Convert.ToDateTime(cFyearFrom).AddYears(1);

            //_logData.WriteLine("tnewfyear :" + tnewfyear);
            v.NewFYearFrom = tnewfyear.ToString("dd/MM/yyyy");
            //_logData.WriteLine("NewFYearFrom :" + v.NewFYearFrom);

            DateTime tnewtyear = Convert.ToDateTime(cFyearTo).AddYears(1);
            //_logData.WriteLine("tnewtyear :" + tnewtyear);

            v.NewFYearTo = tnewtyear.ToString("dd/MM/yyyy");
            //_logData.WriteLine("tnewtyear :" + v.NewFYearTo);

            v.Reference = tnewfyear.Year + "-" + tnewtyear.Year;
            //_logData.WriteLine("Reference :" + v.Reference);

            //}
            //catch(Exception ex)
            //{
            //    _logData.WriteLine("Error :" +ex.Message.ToString());

            //}
            //}
            return Json(v, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindOpenHead(string NewYearFrom, string NewYearTo, string ref1)
        {
            
            int NewFYearID = 0;
            AcFinancialYear a = (from c in db.AcFinancialYears where c.ReferenceName == ref1 select c).FirstOrDefault();

            if (a != null)
            {
                NewFYearID = a.AcFinancialYearID;
            }
            var fdate = NewYearFrom.Split('/');
            var tdate = NewYearTo.Split('/');
            if (Convert.ToInt32(fdate[0]) > 12)
            {
                NewYearFrom = fdate[1] + "/" + fdate[0] + "/" + fdate[2];

            }
            if (Convert.ToInt32(tdate[0]) > 12)
            {
                NewYearTo = tdate[1] + "/" + tdate[0] + "/" + tdate[2];

            }
            //bool result = ESS.SOP.BLL.AcFinancialYear.SaveNewFinancialYear(Convert.ToInt32(Session["fyearid"]), Convert.ToInt32(Session["CurrentCompanyID"]), Convert.ToDateTime(dpNewFyearFrom.SelectedDate), Convert.ToDateTime(dpNewFyearTo.SelectedDate), txtReferenceName.Text, Convert.ToInt32(Session["userid"]), newFinancialYearID);

            int res = db.SaveFinancialYear(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString()), Convert.ToDateTime(NewYearFrom), Convert.ToDateTime(NewYearTo), ref1, Convert.ToInt32(Session["UserID"].ToString()), NewFYearID);
            var Openbal = db.GetOpeningBalanceForYE(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            int res1 = 10;
            return Json(Openbal, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BindPLOpenBalance()
        {
            
            var Openbal = db.GetPLOpeningAmount(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            return Json(Openbal, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BindPLOpenBalanceFinish(string reference)
        {
          
            try
            {
                Yearend(reference);
            }
            catch (Exception ex)
            {
                var Openbal = db.GetPLOpeningAmount(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString()));

                return Json(new { success = false, message = ex.Message.ToString(), bal = Openbal }, JsonRequestBehavior.AllowGet);

            }
            var Openbal1 = db.GetPLOpeningAmount(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString()));

            return Json(new { success = true, message = "Year end process completed successfully.", bal = Openbal1 }, JsonRequestBehavior.AllowGet);
        }
        public void Yearend(string ref1)
        {

            var lstAcHead = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentCompanyID"].ToString()));
            var lstAcJournalMaster = new List<AcJournalMaster>();
            var acJournalMaster = new AcJournalMaster();
            List<AcJournalDetail> lstAcJournalDetail = new List<AcJournalDetail>();
            AcJournalDetail acJournalDetail = new AcJournalDetail(); ;
            var Openbal = db.GetPLOpeningAmount(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString()));

            foreach (var item in Openbal)
            {
                lstAcJournalDetail = new List<AcJournalDetail>();
                decimal Amount = Convert.ToDecimal(item.Balance);
                if (item.Balance == null)
                {
                    Amount = 0;
                }

                if (Amount != 0)
                {
                    //Add YearEnd in AcJournalMaster
                    int maxAcJDetailID = 0;
                    maxAcJDetailID = (from c in db.AcJournalDetails orderby c.AcJournalDetailID descending select c.AcJournalDetailID).FirstOrDefault();

                    acJournalMaster = new AcJournalMaster();

                    acJournalMaster.VoucherNo = "";
                    acJournalMaster.TransDate = DateTime.Now; ;
                    acJournalMaster.AcFinancialYearID = Convert.ToInt32(Session["fyearid"]);
                    acJournalMaster.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"]);
                    acJournalMaster.BranchID = Convert.ToInt32(Session["CurrentBranchID"]);
                    acJournalMaster.VoucherType = "YE";
                    acJournalMaster.TransType = 1;
                    acJournalMaster.StatusDelete = false;
                    acJournalMaster.UserID = Convert.ToInt32(Session["userid"]);

                    //Add Year End in AcJournalDetail
                    acJournalDetail = new AcJournalDetail();
                    acJournalDetail.AcJournalDetailID = maxAcJDetailID + 1;

                    acJournalDetail.AcHeadID = item.AcHeadID;
                    acJournalDetail.Amount = Amount * -1;
                    acJournalDetail.Remarks = "Closing Adjustment";
                    lstAcJournalDetail.Add(acJournalDetail);

                    acJournalDetail = new AcJournalDetail();
                    var achead = (from d in db.AcHeads where d.AcHeadID == 30 select d).FirstOrDefault();
                    acJournalDetail.AcHeadID = achead.AcHeadID;
                    acJournalDetail.Amount = Amount;
                    acJournalDetail.Remarks = "";
                    acJournalDetail.AcJournalDetailID = maxAcJDetailID + 2;
                    lstAcJournalDetail.Add(acJournalDetail);

                    //acJournalMaster.AcJournalDetails = lstAcJournalDetail;
                    lstAcJournalMaster.Add(acJournalMaster);
                }
            }
            foreach (var item in lstAcJournalMaster.ToList())
            {
               
                db.AcJournalMasters.Add(item);
                db.SaveChanges();
            }

            AddInAcOpeningMaster(lstAcHead.ToList(), ref1);

        }
        private void AddInAcOpeningMaster(List<AcHeadSelectAll_Result> lstAcHead, string ref1)
        {
            //AcOpening enter Assets and expenses
            List<AcOpeningMaster> lstAcOpeningMaster = new List<AcOpeningMaster>();
            Int32 acFinancialYearID = (from c in db.AcFinancialYears where c.ReferenceName == ref1 select c.AcFinancialYearID).FirstOrDefault(); ;
            if (acFinancialYearID == 0)
            {
                acFinancialYearID = Convert.ToInt32(Session["fyearid"]);
            }
            var acOpeningMaster = new AcOpeningMaster();
            var Openbal = db.GetPLOpeningAmount(Convert.ToInt32(Session["fyearid"].ToString()), Convert.ToInt32(Session["CurrentCompanyID"].ToString()));

            foreach (var item in Openbal)
            {
                decimal Amount = Convert.ToDecimal(item.Balance);
                if (item.Balance == null)
                {
                    Amount = 0;
                }
                if (Amount != 0)
                {
                    acOpeningMaster = new AcOpeningMaster();
                    acOpeningMaster.AcFinancialYearID = acFinancialYearID;
                    acOpeningMaster.OPDate = DateTime.Now;
                    acOpeningMaster.AcHeadID = item.AcHeadID;
                    acOpeningMaster.Amount = Amount;
                    acOpeningMaster.UserID = Convert.ToInt32(Session["userid"]);
                    lstAcOpeningMaster.Add(acOpeningMaster);
                }

            }
            //Enter PLAccount in AcOpening With New Financial Year ID
            acOpeningMaster = new AcOpeningMaster();
            if (ref1 != string.Empty)
            {
                acOpeningMaster.AcFinancialYearID = acFinancialYearID; //ESS.SOP.BLL.AcFinancialYear.GetNewFinancialYearID(txtReferenceName.Text);
            }
            //acOpeningMaster.OPDate
            var profitlossAccountID = (from d in db.AcHeads where d.AcHeadID == 30 select d).FirstOrDefault();
            acOpeningMaster.AcHeadID = profitlossAccountID.AcHeadID;
            acOpeningMaster.OPDate = DateTime.Now; ;
            var abc = (from p in db.AcJournalDetails
                       join l in db.AcJournalMasters on p.AcJournalID equals l.AcJournalID
                       where l.VoucherType == "YE" && l.TransType == 1 && p.AcHeadID == profitlossAccountID.AcHeadID
                       select p).ToList();
            decimal? plAmount = abc.Sum(i => i.Amount);
            acOpeningMaster.Amount = plAmount;

            acOpeningMaster.UserID = Convert.ToInt32(Session["userid"]);
            lstAcOpeningMaster.Add(acOpeningMaster);
            Int32 ID = -1;
            foreach (var item in lstAcOpeningMaster.ToList())
            {
                item.AcOpeningID = ID;
                db.AcOpeningMasters.Add(item);
                ID = ID - 1;
            }
            var sresult = db.SaveChanges();
            db.Dispose();


        }
        public ActionResult IndexAcHeadAssign()
        {
            var AcheadControl = db.AcHeadControls.ToList();
            var AcheadControlList = new List<AcHeadControlList>();
            foreach (var item in AcheadControl)
            {
                var model = new AcHeadControlList();
                model.AccountName = item.AccountName;
                model.Id = item.Id;
                model.PageControlName = db.PageControlMasters.Where(d => d.Id == item.Pagecontrol).FirstOrDefault().ControlName;
                model.PageControlId = item.Pagecontrol;
                model.PageControlField = item.Remarks;
                model.AcHeadId = item.AccountHeadID;
                var achead = db.AcHeads.Find(item.AccountHeadID);
                if (achead != null)
                {
                    model.AccountHeadName = achead.AcHead1;
                }
                model.Check_Sum = Convert.ToBoolean(item.CheckSum) ? "Page Field Value" : "Sum Value";
                if (item.Remarks == 0)
                {
                    model.PageControlFieldName = "Sum";
                }
                else
                {
                    model.PageControlFieldName = db.PageControlFields.Where(d => d.Id == item.Remarks).FirstOrDefault().FieldName;

                }
                model.AccountNature = (Convert.ToBoolean(item.AccountNature)) ? "Debit" : "Credit";
                AcheadControlList.Add(model);
            }
            return View(AcheadControlList);
        }
        [HttpGet]
        public ActionResult CreateAcHeadControl()
        {

            ViewBag.AccountHeadID = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            var PageControl = db.PageControlMasters.ToList();
            ViewBag.Pagecontrol = new SelectList(PageControl, "Id", "ControlName");
            var PageControlField = db.PageControlFields.ToList();
            ViewBag.Remarks = new SelectList(PageControlField, "Id", "FieldName");
            ViewBag.AccountControl = db.AccountHeadControls.ToList();

            return View();
        }
        [HttpPost]
        public ActionResult CreateAcHeadControl(AcHeadControl acheadcontrol)
        {
            ViewBag.AccountHeadID = db.AcHeadSelectAll(Convert.ToInt32(Session["CurrentBranchID"].ToString())).ToList();
            var PageControl = db.PageControlMasters.ToList();
            ViewBag.Pagecontrol = new SelectList(PageControl, "Id", "ControlName");
            var PageControlField = db.PageControlFields.ToList();
            ViewBag.Remarks = new SelectList(PageControlField, "Id", "FieldName");
            ViewBag.AccountControl = db.AccountHeadControls.ToList();
            var data = new AcHeadControl();
            data.AccountHeadID = acheadcontrol.AccountHeadID;
            data.AccountName = acheadcontrol.AccountName;
            data.AccountNature = acheadcontrol.AccountNature;
            data.Remarks = acheadcontrol.Remarks;
            data.Pagecontrol = acheadcontrol.Pagecontrol;
            if (ModelState.IsValid)
            {
                var duplicate = db.AcHeadControls.Where(cc => cc.Pagecontrol == acheadcontrol.Pagecontrol && cc.AccountName == acheadcontrol.AccountName && cc.AccountNature==acheadcontrol.AccountNature).FirstOrDefault();
                if (duplicate!=null)
                {
                    ViewBag.ErrorMsg = "An Entry Exist to this Page control with same Account Control Name!";
                    return View(acheadcontrol);
                }
                if (acheadcontrol.Remarks == 0)
                {
                    data.CheckSum = false;
                }
                else
                {
                    data.CheckSum = true;
                }
                db.AcHeadControls.Add(data);
                db.SaveChanges();
                ViewBag.SuccessMsg = "You have successfully added Account Assign Head";
                return RedirectToAction("IndexAcHeadAssign");
            }
            return View(acheadcontrol);
        }
        [HttpGet]
        public ActionResult EditAcHeadControl(int Id)
        {
            var data = db.AcHeadControls.Find(Id);

            ViewBag.AccountHeadID = db.AcHeads.ToList();
            var PageControl = db.PageControlMasters.ToList();
            ViewBag.Pagecontrol = PageControl;
            var PageControlField = db.PageControlFields.Where(d => d.PageControlId == data.Pagecontrol).ToList();
            ViewBag.Remarks = PageControlField;
            ViewBag.AccountControl = db.AccountHeadControls.ToList();
            if(db.AcHeads.Find(data.AccountHeadID)!=null)
                @ViewBag.AccountHeadName = db.AcHeads.Find(data.AccountHeadID).AcHead1;

            return View(data);
        }
        [HttpPost]
        public ActionResult EditAcHeadControl(AcHeadControl acheadcontrol)
        {
            var data = db.AcHeadControls.Find(acheadcontrol.Id);

            ViewBag.AccountHeadID = db.AcHeads.ToList();
            var PageControl = db.PageControlMasters.ToList();
            ViewBag.Pagecontrol = PageControl;
            var PageControlField = db.PageControlFields.Where(d => d.PageControlId == data.Pagecontrol).ToList();
            ViewBag.Remarks = PageControlField;
            ViewBag.AccountControl = db.AccountHeadControls.ToList();

            data.AccountHeadID = acheadcontrol.AccountHeadID;
            data.AccountName = acheadcontrol.AccountName;
            data.AccountNature = acheadcontrol.AccountNature;
            data.Remarks = acheadcontrol.Remarks;
            data.Pagecontrol = acheadcontrol.Pagecontrol;
            if (ModelState.IsValid)
            {
                var duplicate = db.AcHeadControls.Where(cc => cc.Pagecontrol == acheadcontrol.Pagecontrol && cc.AccountName == acheadcontrol.AccountName && cc.Id!=data.Id ).FirstOrDefault();
                if (duplicate != null)
                {
                    ViewBag.ErrorMsg = "An Entry Exists to this Page control with same Account Control Name!";
                    return View(acheadcontrol);
                }

                if (acheadcontrol.Remarks == 0)
                {
                    data.CheckSum = false;
                }
                else
                {
                    data.CheckSum = true;
                }
                //db.AcHeadControls.Add(data);
                db.SaveChanges();
                ViewBag.SuccessMsg = "You have successfully added Account Assign Head";
                return RedirectToAction("IndexAcHeadAssign");
            }
            return View(acheadcontrol);
        }
        public JsonResult GetPageControlFields(int id)
        {
            return Json(new SelectList(db.PageControlFields.Where(c => c.PageControlId == id).OrderBy(o => o.Id), "Id", "FieldName"), JsonRequestBehavior.AllowGet);
        }


        ///////////////////////////////
        public ActionResult IndexAcType()
        {

            //var x = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));
            var Accategory = (from d in db.AcCategories select d).ToList();
            var branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            var actype = (from d in db.AcTypes where d.BranchId == branchid select d).ToList();
            var Modellist = new List<AcTypeModel>();
            foreach (var item in actype)
            {
                var model = new AcTypeModel();
                model.Id = item.Id;
                model.AcType = item.AccountType;
                model.AcCategoryID = item.AcCategoryId;
                model.AcCategory = Accategory.Where(d => d.AcCategoryID == item.AcCategoryId).FirstOrDefault().AcCategory1;
                Modellist.Add(model);
            }
            return View(Modellist);
        }

        public ActionResult CreateAcType()
        {
            ViewBag.Category = db.AcCategorySelectAll();
            var model = new AcTypeModel();
            return View(model);
        }


        public bool GetDuplicateType(int AcTypeId, int? CategoryID, string name)
        {
            var branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());

            var data = (from d in db.AcTypes where d.Id != AcTypeId && d.AccountType.ToLower() == name.ToLower() && d.AcCategoryId == CategoryID && d.BranchId == branchid select d).FirstOrDefault();
            if (data == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        [HttpPost]
        public ActionResult CreateAcType(AcTypeModel c)
        {

            var isexist = GetDuplicateType(0, c.AcCategoryID, c.AcType);
            if (isexist == true)
            {
                var actype = new AcType();
                actype.AcCategoryId = c.AcCategoryID;
                actype.AccountType = c.AcType;
                actype.BranchId = Convert.ToInt32(Session["CurrentBranchID"].ToString());
                db.AcTypes.Add(actype);
                db.SaveChanges();
                ViewBag.SuccessMsg = "You have successfully added Account Type";
                return RedirectToAction("IndexAcType", "Accounts", new { id = 0 });


            }
            else
            {
                ViewBag.ErrorMsg = "Account Type already exists !!";
                return View(c);
            }


        }

        public ActionResult EditAcType(int id)
        {
            ViewBag.Category = db.AcCategorySelectAll();

            AcTypeModel v = new AcTypeModel();
            var data = db.AcTypes.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                v.Id = data.Id;
                v.AcCategoryID = data.AcCategoryId;
                v.AcType = data.AccountType;
            }

            return View(v);
        }

        [HttpPost]
        public ActionResult EditAcType(AcTypeModel c)
        {
            var isexist = GetDuplicateType(c.Id, c.AcCategoryID, c.AcType);
            if (isexist == true)
            {
                //var type = new AcType();
                var type = (from d in db.AcTypes where d.Id == c.Id select d).FirstOrDefault();
                //type.Id = c.Id;
                type.AccountType = c.AcType;
                type.AcCategoryId = c.AcCategoryID;
                db.Entry(type).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.SuccessMsg = "You have successfully updated Account Type";
                return RedirectToAction("IndexAcType", "Accounts", new { id = 0 });
            }
            else
            {
                ViewBag.Category = db.AcCategorySelectAll();
                ViewBag.ErrorMsg = "Account Type already exists !!";
                return View(c);
            }
        }


        public ActionResult DeleteAcType(int id)
        {
            AcType c = (from x in db.AcTypes where x.Id == id select x).FirstOrDefault();
            if (c != null)
            {
                try
                {
                    var p = (from a in db.AcGroups where a.AcTypeId == id select a).FirstOrDefault();
                    if (p != null)
                    {
                        ViewBag.ErrorMsg = "Transaction in Use. Can not Delete";
                        throw new Exception();

                    }
                    else
                    {
                        db.AcTypes.Remove(c);
                        db.SaveChanges();


                        ViewBag.SuccessMsg = "You have successfully deleted Account Type";
                        return RedirectToAction("IndexAcType", "Accounts", new { id = 0 });

                    }

                }
                catch (Exception ex)
                {





                }
            }

            return RedirectToAction("IndexAcType", "Accounts", new { id = 0 });
        }

        public ActionResult AnalysisHeadSelectAll(string term)
        {
            //MastersModel MM = new MastersModel();
            int BranchId = CommanFunctions.ParseInt(Session["CurrentBranchID"].ToString());

            if (!String.IsNullOrEmpty(term))
            {
                List<AnalysisHeadSelectAll_Result> AnalysisHeadSelectList = new List<AnalysisHeadSelectAll_Result>();
                AnalysisHeadSelectList = db.AnalysisHeadSelectAll(BranchId).Where(c => c.AnalysisHead.ToLower().Contains(term.ToLower())).OrderBy(x => x.AnalysisGroup).ToList();

                //MM.GetAnalysisHeadSelectList(Common.ParseInt(Session["AcCompanyID"].ToString()), term);
                return Json(AnalysisHeadSelectList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<AnalysisHeadSelectAll_Result> AnalysisHeadSelectList = new List<AnalysisHeadSelectAll_Result>();
                    term = "";
                AnalysisHeadSelectList = db.AnalysisHeadSelectAll(BranchId).Where(c => c.AnalysisHead.ToLower().Contains(term.ToLower())).OrderBy(x => x.AnalysisGroup).ToList();
                //dbM.GetAnalysisHeadSelectList(Common.ParseInt(Session["AcCompanyID"].ToString()), "");
                return Json(AnalysisHeadSelectList, JsonRequestBehavior.AllowGet);
            }
        }
        #region
        public ActionResult Ledger()
        {
            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            ViewBag.AccountType = (from d in db.AcTypes where d.BranchId == branchid select d).ToList();
            ViewBag.groups = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));

            DateTime pFromDate;
            DateTime pToDate;

            if (reportparam == null)
            {
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                pToDate = CommanFunctions.GetLastDayofMonth().Date;
                reportparam = new AccountsReportParam();
                reportparam.FromDate = pFromDate;
                reportparam.ToDate = pToDate;
                reportparam.AcHeadId = 0;
                reportparam.AcHeadName = "";
                reportparam.Output = "PDF";
            }
            else
            {
                if (reportparam.FromDate.Date.ToString() == "01-01-0001 00:00:00" || reportparam.FromDate.Date.ToString() == "01-01-0001")
                {
                    pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                    reportparam.FromDate = pFromDate;
                    reportparam.Output = "PDF";
                }

            }
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("AccLedger"))
                {
                    Session["ReportOutput"] = null;
                }
            }
            ViewBag.ReportName = "Ledger Report";
            return View(reportparam);
        }

        [HttpPost]
        public ActionResult Ledger(AccountsReportParam picker)
        {
            AccountsReportParam model = new AccountsReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                AcHeadId = picker.AcHeadId,
                AcHeadName = picker.AcHeadName,
                Output = picker.Output
            };

            ViewBag.Token = model;
            SessionDataModel.SetAccountsParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            AccountsReportsDAO.GenerateLedgerReport();
            if (model.Output != "PDF")
                return RedirectToAction("Download", "Accounts", new { file = "a" });
            else
                return RedirectToAction("Ledger", "Accounts");



        }
        public ActionResult TrialBalance()
        {
            AccountsReportParam1 reportparam = SessionDataModel.GetAccountsParam1();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            ViewBag.AccountType = (from d in db.AcTypes where d.BranchId == branchid select d).ToList();
            ViewBag.groups = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));

            DateTime pFromDate;
            DateTime pToDate;

            if (reportparam == null)
            {
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                pToDate = CommanFunctions.GetLastDayofMonth().Date;
                reportparam = new AccountsReportParam1();
                reportparam.AsOnDate = pToDate;
                reportparam.Output = "PDF";
            }
            else
            {
                if (reportparam.AsOnDate.Date.ToString() == "01-01-0001 00:00:00" || reportparam.AsOnDate.Date.ToString() == "01-01-0001")
                {
                    pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                    reportparam.AsOnDate = pFromDate;
                    reportparam.Output = "PDF";
                }

            }
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("AccTrialBal"))
                {
                    Session["ReportOutput"] = null;
                }
            }
            ViewBag.ReportName = "Trial Balance Report";
            return View(reportparam);
        }

        [HttpPost]
        public ActionResult TrialBalance(AccountsReportParam1 picker)
        {
            AccountsReportParam1 model = new AccountsReportParam1
            {
                AsOnDate = picker.AsOnDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                Output = picker.Output
            };

            ViewBag.Token = model;
            SessionDataModel.SetAccountsParam1(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            AccountsReportsDAO.GenerateTrialBalanceReport();
            if (model.Output != "PDF")
                return RedirectToAction("Download", "Accounts", new { file = "a" });
            else
                return RedirectToAction("TrialBalance", "Accounts");



        }

        public ActionResult PLTradingReport()
        {
            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam2();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            ViewBag.AccountType = (from d in db.AcTypes where d.BranchId == branchid select d).ToList();
            ViewBag.groups = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));

            DateTime pFromDate;
            DateTime pToDate;

            if (reportparam == null)
            {
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                pToDate = CommanFunctions.GetLastDayofMonth().Date;
                reportparam = new AccountsReportParam();
                reportparam.FromDate = pFromDate;
                reportparam.ToDate = pToDate;
                reportparam.AcHeadId = 0;
                reportparam.AcHeadName = "";
                reportparam.Output = "PDF";
            }
            else
            {
                if (reportparam.FromDate.Date.ToString() == "01-01-0001 00:00:00" || reportparam.FromDate.Date.ToString() == "01-01-0001")
                {
                    pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                    reportparam.FromDate = pFromDate;
                    reportparam.Output = "PDF";
                }

            }
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("AccTrading"))
                {
                    Session["ReportOutput"] = null;
                }
            }
            ViewBag.ReportName = "PL/Trading Report";
            return View(reportparam);
        }

        [HttpPost]
        public ActionResult PLTradingReport(AccountsReportParam picker)
        {
            AccountsReportParam model = new AccountsReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                Output = picker.Output
            };

            ViewBag.Token = model;
            SessionDataModel.SetAccountsParam2(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            AccountsReportsDAO.GenerateTradingAccountReport();
            if (model.Output != "PDF")
                return RedirectToAction("Download", "Accounts", new { file = "a" });
            else
                return RedirectToAction("PLTradingReport", "Accounts");



        }

        public ActionResult ReportFrame()
        {
            if (Session["ReportOutput"] != null)
                ViewBag.ReportOutput = Session["ReportOutput"].ToString();
            else
            {
                string reportpath = AccountsReportsDAO.GenerateDefaultReport();
                ViewBag.ReportOutput = reportpath; // "~/Reports/DefaultReport.pdf";
                //ViewBag.ReportOutput = "~/Reports/DefaultReport.pdf";
            }
            return PartialView();
        }
        public ActionResult ReportParam()
        {
            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            
            ViewBag.AccountType = (from d in db.AcTypes where d.BranchId == branchid select d).ToList();            
            ViewBag.groups = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));

            DateTime pFromDate;
            DateTime pToDate;
            
                if (reportparam == null)
                {
                    pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                pToDate = CommanFunctions.GetLastDayofMonth().Date;
                    reportparam = new AccountsReportParam();
                    reportparam.FromDate = pFromDate;
                    reportparam.ToDate = pToDate;
                    reportparam.AcHeadId = 0;
                    reportparam.AcHeadName = "";
                    reportparam.Output = "PDF";
                }
            else
            {
                
                if (reportparam.FromDate.Date.ToString() =="01-01-0001 00:00:00" || reportparam.FromDate.Date.ToString() == "01-01-0001")
                {
                    pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                    reportparam.FromDate = pFromDate;
                    reportparam.Output = "PDF";
                }
                if (reportparam.ToDate.Date.ToString() == "01-01-0001 00:00:00" || reportparam.FromDate.Date.ToString() == "01-01-0001")
                {
                    pToDate = CommanFunctions.GetLastDayofMonth().Date; //.AddDays(-1);
                    reportparam.ToDate = pToDate;
                    reportparam.Output = "PDF";
                }
            }
                            
            return View(reportparam);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportParam([Bind(Include = "FromDate,ToDate,AcHeadId,AcHeadName,Output,Filters")] AccountsReportParam picker)
        {
            AccountsReportParam model = new AccountsReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),                
                AcHeadId = picker.AcHeadId,
                AcHeadName = picker.AcHeadName,                
                Output =picker.Output
            };

            //model.Output = "EXCEL";
            ViewBag.Token = model;
            
            SessionDataModel.SetAccountsParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //Stream stream= GenerateReport();
            //GenerateDefaultReport();
            var reportid = Session["ReportId"].ToString();

            if (reportid == "1") //ledger
            {
                AccountsReportsDAO.GenerateLedgerReport();
                if (model.Output != "PDF")
                    return RedirectToAction("Download", "Accounts", new { file = "a" });
                else
                    return RedirectToAction("Ledger", "Accounts", new { id = 1 });
            }
            else if (reportid=="3")
            {
                AccountsReportsDAO.GenerateTradingAccountReport();                
                return RedirectToAction("Ledger", "Accounts", new { id = reportid });

            }
            else
            {
                return RedirectToAction("Ledger", "Accounts", new { id = 1 });
            }
            //return File(stream, "application/pdf", "AccLedger.pdf");


            //return PartialView(model);
            //return View(model);

            //return PartialView("InvoiceSearch",model);

        }

        [HttpPost]
        public ActionResult ReportLedger(AccountsReportParam picker)
        {
            picker.AcHeadName = "test";
            AccountsReportParam model = new AccountsReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                AcHeadId = picker.AcHeadId,
                AcHeadName = picker.AcHeadName,
            };

            ViewBag.Token = model;
            SessionDataModel.SetAccountsParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = GenerateReport();
            //MemoryStream outputStream = new MemoryStream();
            //MemoryStream workStream = new MemoryStream();
            //var bytes = System.Text.Encoding.UTF8.GetBytes(id);
            //byte[] byteArray = bytes;
            //outputStream.Write(byteArray, 0, byteArray.Length);
            //outputStream.Position = 0;
            return File(stream, "application/pdf");
            //AccountsReportsDAO.GenerateLedgerReport();
            return File(stream, "application/pdf", "AccLedger.pdf");
            //return RedirectToAction("Ledger", "Accounts",new { id = 1 });

            //return PartialView(model);
            //return View(model);

            //return PartialView("InvoiceSearch",model);

        }
       [HttpGet]
       [DeleteFileAttribute] //Action Filter, it will auto delete the file after download, 
                      //I will explain it later
        public ActionResult Download()
        {
            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            string file = reportparam.ReportFileName;
            string fullPath = "";
            if (Session["ReportOutput"] != null)
                fullPath = Server.MapPath(Session["ReportOutput"].ToString());
            else
                ViewBag.ReportOutput = null;// "~/Reports/DefaultReport.pdf";
            //get the temp folder and file path in server
            
            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            
            if (reportparam.Output=="EXCEL" || reportparam.Output=="WORD")
            {
                return File(fullPath, "application/vnd.ms-excel", file);
            }
            //else if (reportparam.Output=="WORD")
            //{
            //    return File(fullPath, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", file);
            //}
            else
            {
                return File(fullPath, "application/pdf", file);
            }
            
        }

        public ActionResult ReportParamAsonDate()
        {
            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            DateTime pToDate;            

            if (reportparam == null)
            {
                pToDate = CommanFunctions.GetLastDayofMonth().Date;
                reportparam = new AccountsReportParam();
                reportparam.ToDate = pToDate.Date;
            }
            

            return View(reportparam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportParamAsonDate([Bind(Include = "ToDate")] AccountsReportParam picker)
        {
            AccountsReportParam model = new AccountsReportParam
            {
                ToDate = picker.ToDate
            };

            ViewBag.Token = model;
            SessionDataModel.SetAccountsParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            if (Session["ReportID"].ToString() == "2") //trial balance
                AccountsReportsDAO.GenerateTrialBalanceReport();
            else //3 trading account report
                AccountsReportsDAO.GenerateTradingAccountReport();

            int reportid = Convert.ToInt32(Session["ReportID"].ToString());
            return RedirectToAction("Ledger", "Accounts", new {id=reportid});

        }


        public ActionResult CustomerLedger()
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            CustomerLedgerReportParam model = SessionDataModel.GetCustomerLedgerReportParam();
            if (model == null)
            {
                model = new CustomerLedgerReportParam
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date,
                    CustomerId =0,
                    CustomerName ="",
                    Output = "PDF",
                    ReportType="Ledger"
                };
            }
            if (model.FromDate.ToString() == "01-01-0001 00:00:00")
            {
                model.FromDate = CommanFunctions.GetFirstDayofMonth().Date;
            }


            SessionDataModel.SetCustomerLedgerParam(model);

            model.FromDate = AccountsDAO.CheckParamDate(model.FromDate, yearid).Date;
            model.ToDate = AccountsDAO.CheckParamDate(model.ToDate, yearid).Date;

            ViewBag.ReportName = "Customer Ledger";             
                if (Session["ReportOutput"] != null)
                {
                    string currentreport = Session["ReportOutput"].ToString();
                    if (!currentreport.Contains("CustomerLedger") && model.ReportType=="Ledger")
                    {
                        Session["ReportOutput"] = null;
                    }
                    else if (!currentreport.Contains("CustomerOutStanding") && model.ReportType == "OutStanding")
                    {
                        Session["ReportOutput"] = null;
                    }
                }
                
            return View(model);

        }

        [HttpPost]
        public ActionResult CustomerLedger(CustomerLedgerReportParam picker)
        {
            
            CustomerLedgerReportParam model = new CustomerLedgerReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                CustomerId= picker.CustomerId,
                CustomerName = picker.CustomerName,
                Output= "PDF",
                ReportType=picker.ReportType
            };

            ViewBag.Token = model;
            SessionDataModel.SetCustomerLedgerParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            if (model.ReportType=="Ledger")
            {
                //AccountsReportsDAO.GenerateCustomerLedgerReport();
                AccountsReportsDAO.GenerateCustomerLedgerDetailReport();
            }            
            else if (model.ReportType=="OutStanding") 
            {
                AccountsReportsDAO.GenerateCustomerOutStandingReport();
            }
            else if (model.ReportType == "AWBOutStanding")
            {
                AccountsReportsDAO.GenerateAWBOutStandingReport();
            }

            return RedirectToAction("CustomerLedger", "Accounts");
                       

        }
        public Stream GenerateReport()
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int userid= Convert.ToInt32(Session["UserID"].ToString());
            string usertype = Session["UserType"].ToString();

            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "sp_accledger";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate);
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate);
            comd.Parameters.AddWithValue("@AcHeadId", reportparam.AcHeadId);
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@YearId", yearid);
            //comd.CommandText = "up_GetAllCustomer"; comd.Parameters.Add("@Companyname", SqlDbType.VarChar, 50);
            //if (TextBox1.Text.Trim() != "")
            //    comd.Parameters[0].Value = TextBox1.Text;
            //else
            //    comd.Parameters[0].Value = DBNull.Value;
            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AccLedger");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(Server.MapPath("~/Reports"),"AccLedger.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();           

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.rpt"));
            
            rd.SetDataSource(ds);

            
            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["AccountHead"].CurrentValues.AddValue(reportparam.AcHeadName);
            string period = "Period From " + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid,usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmm") + ".pdf";
            string reportpath = Path.Combine(Server.MapPath("~/ReportsPDF"));
            
            //rd.ExportToDisk(ExportFormatType.PortableDocFormat,reportpath );
            Session["ReportOutput"] = "~/ReportsPDF/" + reportname;
                  
            
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));
            //SaveStreamAsFile(reportpath, stream, reportname);
            //reportpath = Path.Combine(Server.MapPath("~/ReportsPDF"),reportname);            
            //return reportpath;
        }

        public void GenerateDefaultReport()
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            int userid = Convert.ToInt32(Session["UserID"].ToString());
            string usertype = Session["UserType"].ToString();

            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConnString);
            SqlCommand comd;
            comd = new SqlCommand();
            comd.Connection = sqlConn;
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "sp_accledger";
            comd.Parameters.AddWithValue("@FromDate", reportparam.FromDate);
            comd.Parameters.AddWithValue("@ToDate", reportparam.ToDate);
            comd.Parameters.AddWithValue("@AcHeadId", reportparam.AcHeadId);
            comd.Parameters.AddWithValue("@BranchId", branchid);
            comd.Parameters.AddWithValue("@YearId", yearid);
            //comd.CommandText = "up_GetAllCustomer"; comd.Parameters.Add("@Companyname", SqlDbType.VarChar, 50);
            //if (TextBox1.Text.Trim() != "")
            //    comd.Parameters[0].Value = TextBox1.Text;
            //else
            //    comd.Parameters[0].Value = DBNull.Value;
            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = comd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, "AccLedger");

            //generate XSD to design report
            //System.IO.StreamWriter writer = new System.IO.StreamWriter(Path.Combine(Server.MapPath("~/Reports"),"AccLedger.xsd"));
            //ds.WriteXmlSchema(writer);
            //writer.Close();           

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "DefaultReport.rpt"));

            //rd.SetDataSource(ds);


            string companyaddress = SourceMastersModel.GetReportHeader2(branchid);
            string companyname = SourceMastersModel.GetReportHeader1(branchid);

            // Assign the params collection to the report viewer
            rd.ParameterFields[0].DefaultValues.AddValue(companyname);
            rd.ParameterFields[0].CurrentValues.AddValue(companyname);
            rd.ParameterFields["CompanyAddress"].CurrentValues.AddValue(companyaddress);
            rd.ParameterFields["AccountHead"].CurrentValues.AddValue("Default Report");
            string period = "Reprot Period as on Date "; // + reportparam.FromDate.Date.ToString("dd-MM-yyyy") + " to " + reportparam.ToDate.Date.ToString("dd-MM-yyyy");
            rd.ParameterFields["ReportPeriod"].CurrentValues.AddValue(period);

            string userdetail = "printed by " + SourceMastersModel.GetUserFullName(userid, usertype) + " on " + DateTime.Now;
            rd.ParameterFields["UserDetail"].CurrentValues.AddValue(userdetail);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //string reportname = "AccLedger_" + DateTime.Now.ToString("ddMMyyHHmm") + ".pdf";
            string reportname = "DefaultReport.pdf";
            string reportpath = Path.Combine(Server.MapPath("~/Reports"),reportname);

            rd.ExportToDisk(ExportFormatType.PortableDocFormat,reportpath );
            //Session["ReportOutput"] = "~/ReportsPDF/" + reportname;

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //return stream;
            //stream.Write(Path.Combine(Server.MapPath("~/Reports"), "AccLedger.pdf"));
            //SaveStreamAsFile(reportpath, stream, reportname);
            //reportpath = Path.Combine(Server.MapPath("~/ReportsPDF"),reportname);            
            //return reportpath;
        }

        public ActionResult ProfitLossReport()
        {
                ViewBag.ReportName = "Profit and Loss Report";
               
               
                if (Session["ReportOutput"] != null)
                {
                    string currentreport = Session["ReportOutput"].ToString();
                    if (!currentreport.Contains("ProfileLossReport"))
                    {
                        Session["ReportOutput"] = null;
                    }
                }
            
            return View();

        }

        public ActionResult DayBook()
        {
            ViewBag.ReportName = "Day Book Report";
            //List<VoucherTypeVM> lsttype = new List<VoucherTypeVM>();
            //lsttype.Add(new VoucherTypeVM { TypeName = "All" });
            //lsttype.Add(new VoucherTypeVM { TypeName = "Cash Receipt & Payments" });
            //lsttype.Add(new VoucherTypeVM { TypeName = "Journal Vouchers" });
            //lsttype.Add(new VoucherTypeVM { TypeName = "AWB Posting" });
            //lsttype.Add(new VoucherTypeVM { TypeName = "Invoice Posting" });
            //lsttype.Add(new VoucherTypeVM { TypeName = "COD Posting" });
            //lsttype.Add(new VoucherTypeVM { TypeName = "Customer Receipt Posting" });
                        
            if (Session["ReportOutput"] != null)
            {
                string currentreport = Session["ReportOutput"].ToString();
                if (!currentreport.Contains("DayBook"))
                {
                    Session["ReportOutput"] = null;
                }
            }
            AccountsReportParam model = SessionDataModel.GetAccountsParam();
            if (model == null)
            {
                model = new AccountsReportParam
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date, //.AddDays(-1);,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date,
                    Output = "PDF",
                    VoucherTypeId = "0"
                };
            }
            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DayBook([Bind(Include = "FromDate,ToDate,Output,Filters,SelectedValues")] AccountsReportParam picker)
        {
            AccountsReportParam model = SessionDataModel.GetAccountsParam();
            if (model == null)
                model = new AccountsReportParam();

            model.FromDate = picker.FromDate;
            model.ToDate = picker.ToDate;
            model.Output = picker.Output;
            model.Filters = picker.Filters;
            model.SelectedValues = picker.SelectedValues;
            model.VoucherTypeId = "";
            if (picker.SelectedValues != null)
            {
                foreach (var item in picker.SelectedValues)
                {
                    if (model.VoucherTypeId == "")
                    {
                        model.VoucherTypeId= item.ToString();
                    }
                    else
                    {
                        model.VoucherTypeId = model.VoucherTypeId + "," + item.ToString();
                    }

                }
            }
            //model.Output = "EXCEL";
            ViewBag.Token = model;
            SessionDataModel.SetAccountsParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
           

            AccountsReportsDAO.GenerateDayBookReport();

            if (model.Output != "PDF")
                return RedirectToAction("Download", "Accounts", new { file = "a" });
            else
                return RedirectToAction("DayBook", "Accounts");// return View(model);            

        }

        public ActionResult ReportParamDate()
        {
            AccountsReportParam reportparam = SessionDataModel.GetAccountsParam();
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());

            //ViewBag.AccountType = (from d in db.AcTypes where d.BranchId == branchid select d).ToList();
            //ViewBag.groups = GetAllAcGroupsByBranch(Convert.ToInt32(Session["CurrentBranchID"].ToString()));

            DateTime pFromDate;
            DateTime pToDate;

            if (reportparam == null)
            {
                pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                pToDate = CommanFunctions.GetLastDayofMonth().Date;
                reportparam = new AccountsReportParam();
                reportparam.FromDate = pFromDate;
                reportparam.ToDate = pToDate;
                reportparam.AcHeadId = 0;
                reportparam.AcHeadName = "";
                reportparam.Output = "PDF";
            }
            else
            {
                if (reportparam.FromDate.Date.ToString() == "01-01-0001 00:00:00")
                {
                    pFromDate = CommanFunctions.GetFirstDayofMonth().Date; //.AddDays(-1);
                    reportparam.FromDate = pFromDate;
                    reportparam.Output = "PDF";
                }

            }

            return View(reportparam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportParamDate([Bind(Include = "FromDate,ToDate,Output,Filters")] AccountsReportParam picker)
        {
            AccountsReportParam model = new AccountsReportParam
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),            
                Output = picker.Output
            };

            //model.Output = "EXCEL";
            ViewBag.Token = model;
            SessionDataModel.SetAccountsParam(model);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //Stream stream= GenerateReport();
            //GenerateDefaultReport();

            AccountsReportsDAO.GenerateProfitLostReport();

            if (model.Output != "PDF")
                return RedirectToAction("Download", "Accounts", new { file = "a" });
            else
                return RedirectToAction("ProfitLossReport", "Accounts", new { id = 1 });

            //return File(stream, "application/pdf", "AccLedger.pdf");


            //return PartialView(model);
            //return View(model);

            //return PartialView("InvoiceSearch",model);

        }

        #endregion
        public static void SaveStreamAsFile(string filePath, Stream inputStream, string fileName)
        {
            DirectoryInfo info = new DirectoryInfo(filePath);
            if (!info.Exists)
            {
                info.Create();
            }

            string path = Path.Combine(filePath, fileName);
            using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
            {
                inputStream.CopyTo(outputFileStream);
            }
        }

        [HttpGet]
        public JsonResult GetSupplierName(string term, string SupplierTypeId)
        {
            var supplierType = new SupplierType();

            if (SupplierTypeId == "H" || SupplierTypeId == "1")
            {
                supplierType = (from d in db.SupplierTypes where d.SupplierType1 == "Hired Drivers" select d).FirstOrDefault();
            }
            else if (SupplierTypeId == "F" || SupplierTypeId == "2")
            {
                supplierType = (from d in db.SupplierTypes where d.SupplierType1 == "Forwarding Agents" select d).FirstOrDefault();

            }
            else if (SupplierTypeId == "S" || SupplierTypeId == "4")
            {
                supplierType = (from d in db.SupplierTypes where d.SupplierType1 == "Sundry Suppliers" select d).FirstOrDefault();

            }
            else if (SupplierTypeId == "3")
            {
                supplierType = (from d in db.SupplierTypes where d.SupplierType1 == "Contract Drivers" select d).FirstOrDefault();

            }

            if (SupplierTypeId == "C") //customer
            {
                if (term.Trim() != "")
                {
                    var customerlist = (from c1 in db.CustomerMasters
                                        where c1.CustomerType == "CR" && c1.CustomerName.ToLower().Contains(term.ToLower())
                                        orderby c1.CustomerName ascending
                                        select new { SupplierID = c1.CustomerID, SupplierName = c1.CustomerName }).ToList();

                    return Json(customerlist, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var customerlist = (from c1 in db.CustomerMasters
                                        where c1.CustomerType == "CR"
                                        orderby c1.CustomerName ascending
                                        select new { SupplierID = c1.CustomerID, SupplierName = c1.CustomerName }).ToList();

                    return Json(customerlist, JsonRequestBehavior.AllowGet);
                }
            }
            else //supplier list
            {
                int supplier;  
                if (SupplierTypeId=="S")
                {
                    supplier = 0;
                }
                else
                {
                    supplier=Convert.ToInt32(SupplierTypeId);
                }
                
                if (term.Trim() != "")
                {
                 
                    var customerlist = (from c1 in db.SupplierMasters
                                        where c1.SupplierName.ToLower().Contains(term.ToLower()) && (c1.SupplierTypeID==supplier || supplier==0)
                                        orderby c1.SupplierName ascending
                                        select new { SupplierID = c1.SupplierID, SupplierName = c1.SupplierName }).ToList();
                    return Json(customerlist, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var customerlist = (from c1 in db.SupplierMasters
                                        where (c1.SupplierTypeID == supplier || supplier == 0)
                                        orderby c1.SupplierName ascending
                                        select new { SupplierID = c1.SupplierID, SupplierName = c1.SupplierName }).ToList();
                    return Json(customerlist, JsonRequestBehavior.AllowGet);
                }
            }

        }

        #region "Account Opening"
        public ActionResult AcOpeningMaster()
        {
            var AcFinancialYearID = Convert.ToInt32(Session["fyearid"]);

            var CurrentStartYear = Convert.ToDateTime(Session["FyearFrom"]);
            var Year = CurrentStartYear.Year;
            DateTime firstDay = new DateTime(Year, 1, 1);
            ViewBag.Opdate = firstDay.ToString("dd-MM-yyyy");
            var OpeningMaster = (from d in db.AcOpeningMasters where d.AcFinancialYearID == AcFinancialYearID select d).ToList();
            return View(OpeningMaster);
        }

        public JsonResult SubmitAcOpeningMasterold(int Id, int AcHeadId, decimal? Amount, string AccNature)
        {
            try
            {
                var AcFinancialYearID = Convert.ToInt32(Session["fyearid"]);
                var isexist = (from d in db.AcOpeningMasters where d.AcOpeningID != Id && d.AcFinancialYearID == AcFinancialYearID && d.AcHeadID == AcHeadId select d).FirstOrDefault();
                if (isexist == null)
                {
                    var data = (from d in db.AcOpeningMasters where d.AcOpeningID == Id select d).FirstOrDefault();
                    if (data == null)
                    {
                        data = new AcOpeningMaster();
                    }
                    data.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"]);
                    data.BranchID = Convert.ToInt32(Session["CurrentBranchID"]);
                    data.AcFinancialYearID = Convert.ToInt32(Session["fyearid"]);
                    data.AcHeadID = AcHeadId;
                    var CurrentStartYear = Convert.ToDateTime(Session["FyearFrom"]);
                    var Year = CurrentStartYear.Year;
                    DateTime firstDay = new DateTime(Year, 1, 1);
                    data.OPDate = firstDay;
                    if (AccNature == "Dr")
                    {
                        data.Amount = Amount;
                    }
                    else
                    {
                        data.Amount = Amount * -1;
                    }
                    data.UserID = Convert.ToInt32(Session["UserID"]);
                    if (Id == 0)
                    {
                        db.AcOpeningMasters.Add(data);
                    }
                    db.SaveChanges();
                    return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Opening Amount already added to this Chart of Account!" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult SubmitAcOpeningMaster(List<AcOpeningMasterVm> list)

        {
            try
            {
                var AcFinancialYearID = Convert.ToInt32(Session["fyearid"]);
                var BranchId = Convert.ToInt32(Session["CurrentBranchID"]);
                int Id = 0;
                int AcHeadId = 0;
                decimal Amount = 0;
                string AccNature;
                bool deleted = false;
                foreach (var item in list)
                {
                    Id = item.AcOpeningID;
                    AcHeadId = item.AcHeadID;
                    Amount = item.Amount;
                    AccNature = item.AcNature;
                    deleted = item.IsDeleted;

                    AcOpeningMaster data = new AcOpeningMaster();
                    if (Id > 0)
                    {
                        data = db.AcOpeningMasters.Find(item.AcOpeningID);
                        if (deleted == true)
                        {
                            db.AcOpeningMasters.Remove(data);
                            db.SaveChanges();
                            continue;
                        }
                    }

                    if (Id == 0 && deleted == true)
                    {
                        continue;
                    }

                    var isexist = (from d in db.AcOpeningMasters where d.AcOpeningID != Id && d.AcFinancialYearID == AcFinancialYearID && d.AcHeadID == AcHeadId select d).FirstOrDefault();
                    if (isexist == null)
                    {

                        data.AcCompanyID = Convert.ToInt32(Session["CurrentCompanyID"]);
                        data.BranchID = Convert.ToInt32(Session["CurrentBranchID"]);
                        data.AcFinancialYearID = Convert.ToInt32(Session["fyearid"]);
                        data.AcHeadID = AcHeadId;
                        var CurrentStartYear = Convert.ToDateTime(Session["FyearFrom"]);
                        var Year = CurrentStartYear.Year;
                        DateTime firstDay = new DateTime(Year, 1, 1);
                        data.OPDate = firstDay;
                        if (AccNature == "Dr")
                        {
                            data.Amount = Amount;
                        }
                        else
                        {
                            data.Amount = Amount * -1;
                        }
                        data.UserID = Convert.ToInt32(Session["UserID"]);
                        if (Id == 0)
                        {
                            db.AcOpeningMasters.Add(data);
                        }
                        db.SaveChanges();
                    }
                }

                AccountsDAO.AccountOpeningPosting(AcFinancialYearID, BranchId);

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Saved Successfully!" }, JsonRequestBehavior.AllowGet);
        }

        //not used
        public ActionResult DeleteAcOpeningMaster(int Id)
        {
            var data = (from d in db.AcOpeningMasters where d.AcOpeningID == Id select d).FirstOrDefault();
            db.AcOpeningMasters.Remove(data);
            db.SaveChanges();
            ViewBag.SuccessMsg = "You have successfully deleted Account Opening Master";
            return RedirectToAction("AcOpeningMaster");

        }
        #endregion

        #region "InvoiceOpening"
        public ActionResult AcOpeningInvoiceIndex(string type)
        {
            if (type == null)
                type = "All";

            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            List<AcInvoiceOpeningVM> vm = new List<AcInvoiceOpeningVM>();
            vm = AccountsDAO.GetInvoiceOpening(yearid, type);
            ViewBag.Type = type;

            decimal debit = vm.Sum(i => i.Debit);
            decimal credit = vm.Sum(i => i.Credit);
            ViewBag.TotalDebit = String.Format("{0:0.000}", debit);
            ViewBag.TotalCredit = String.Format("{0:0.000}", credit);
            ViewBag.Balance = String.Format("{0:0.000}", debit - credit);
            ViewBag.Title = "Invoice Opening";
            return View(vm);
        }

        public ActionResult AcOpeningInvoiceSearch()
        {

            OpeningInvoiceSearch datePicker = SessionDataModel.GetOpeningInvoiceSearch();

            if (datePicker == null)
            {
                datePicker = new OpeningInvoiceSearch();
                datePicker.OpeningDate = Convert.ToDateTime(Session["FyearFrom"]).ToString("dd-MM-yyyy");
                datePicker.InvoiceType = "C";
                datePicker.Remarks = "";
                datePicker.Debit = 0;
                datePicker.Credit = 0;
            }
            if (datePicker != null)
            {

            }

            SessionDataModel.SetOpeningInvoiceSearch(datePicker);
            return View(datePicker);

        }

        [HttpPost]
        public ActionResult AcOpeningInvoiceSearch([Bind(Include = "InvoiceType,PartyId,PartyName")] OpeningInvoiceSearch picker)
        {
            OpeningInvoiceSearch model = SessionDataModel.GetOpeningInvoiceSearch();
            model.PartyId = picker.PartyId;
            model.InvoiceType = picker.InvoiceType;
            model.PartyName = picker.PartyName;
            model.Remarks = picker.Remarks;
            model.Debit = 100;
            model.Credit = 220;
            ViewBag.Token = model;
            SessionDataModel.SetOpeningInvoiceSearch(model);
            return RedirectToAction("AcOpeningInvoice", "Accounts");
            //return PartialView("InvoiceSearch",model);
        }

        public ActionResult AcOpeningInvoice(int id = 0, string invoicetype = "")
        {
            int yearid = Convert.ToInt32(Session["fyearid"].ToString());
            OpeningInvoiceSearch model = SessionDataModel.GetOpeningInvoiceSearch();
            if (model == null)
            {
                model = new OpeningInvoiceSearch();
            }
            string invoicetype1 = "C";
            if (id > 0)
            {

                model.PartyId = id;
                model.InvoiceType = invoicetype;
                invoicetype1 = invoicetype;
                model.OpeningDate = Convert.ToDateTime(Session["FyearFrom"]).ToString("dd-MM-yyyy");
                if (invoicetype == "C")
                    model.PartyName = db.CustomerMasters.Find(model.PartyId).CustomerName;
                else
                    model.PartyName = db.SupplierMasters.Find(model.PartyId).SupplierName;

                SessionDataModel.SetOpeningInvoiceSearch(model);
            }


            var Model = new OpeningBalanceVM();
            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            int partyid = 0;


            if (model != null)
            {
                partyid = model.PartyId;
                invoicetype = model.InvoiceType;
            }

            var opinvoice = db.AcOPInvoiceMasters.Where(cc => cc.AcFinancialYearID == yearid && cc.PartyID == partyid && cc.StatusSDSC == invoicetype).FirstOrDefault();
            AcInvoiceOpeningVM vm = new AcInvoiceOpeningVM();
            List<AcInvoiceOpeningDetailVM> detailvm = new List<AcInvoiceOpeningDetailVM>();
            if (opinvoice != null)
            {
                vm.AcOPInvoiceMasterID = opinvoice.AcOPInvoiceMasterID;
                vm.AcFinancialYearID = opinvoice.AcFinancialYearID;
                vm.PartyID = opinvoice.PartyID;
                vm.Remarks = opinvoice.Remarks;
                vm.StatusSDSC = opinvoice.StatusSDSC;
                var opinvoicedetail = (from c in db.AcOPInvoiceDetails
                                       where c.AcOPInvoiceMasterID == vm.AcOPInvoiceMasterID
                                       select new AcInvoiceOpeningDetailVM { AcOPInvoiceDetailID = c.AcOPInvoiceDetailID, InvoiceNo = c.InvoiceNo, InvoiceDate = c.InvoiceDate, LastTransDate = c.LastTransDate, Amount = c.Amount }).ToList();
                vm.Debit = opinvoicedetail.Where(cc => cc.Amount > 0).Sum(i => i.Amount).Value;
                vm.Credit = -1 * opinvoicedetail.Where(cc => cc.Amount < 0).Sum(i => i.Amount).Value;
                vm.Balance = vm.Debit - vm.Credit;
                vm.InvoiceDetailVM = opinvoicedetail;
            }
            else
            {
                vm.AcOPInvoiceMasterID = 0;
                vm.AcFinancialYearID = yearid;
                vm.PartyID = partyid;
                vm.Remarks = "";
                vm.InvoiceDetailVM = detailvm;
                vm.StatusSDSC = invoicetype;
            }
            if (invoicetype == "C")
                ViewBag.Title = "Customer Invoice Opening";
            else
                ViewBag.Title = "Supplier Invoice Opening";

            return View(vm);
        }

        public JsonResult GetAcOpeningBalanceDetails(string Type, int PartyId)
        {
            var OpeningInvoice = db.AcOPInvoiceMasters.Where(d => d.StatusSDSC == Type && d.PartyID == PartyId).FirstOrDefault();
            var InvoiceDetails = new List<AcOPInvoiceDetail>();

            if (OpeningInvoice != null)
            {
                InvoiceDetails = (from d in db.AcOPInvoiceDetails where d.AcOPInvoiceMasterID == OpeningInvoice.AcOPInvoiceMasterID select d).ToList();
            }
            else
            {
                OpeningInvoice = new AcOPInvoiceMaster();
            }
            InvoiceDetails.ForEach(d => d.AcOPInvoiceMaster = null);
            return Json(new { Remarks = OpeningInvoice.Remarks, InvoiceDetails = InvoiceDetails }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetInvoicesDetails(string InvoiceNo, string Invoicedate, string LastTransdate, decimal Amount, int Drcr)
        {
            var invoice = new AcOPInvoiceDetail();
            invoice.InvoiceNo = InvoiceNo;
            invoice.InvoiceDate = Convert.ToDateTime(Invoicedate);
            invoice.LastTransDate = Convert.ToDateTime(LastTransdate);
            invoice.Amount = Convert.ToDecimal(Amount);
            invoice.StatusClose = Drcr == 1 ? true : false;
            return Json(new { InvoiceDetails = invoice }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveOpeningInvoice(AcInvoiceOpeningVM model)
        {
            try
            {
                int yearid = Convert.ToInt32(Session["fyearid"].ToString());
                int branchid = Convert.ToInt32(Session["CurrentBranchID"]);
                AcOPInvoiceMaster invoice = new AcOPInvoiceMaster();
                if (model.AcOPInvoiceMasterID > 0) // new entry
                {
                    invoice = db.AcOPInvoiceMasters.Find(model.AcOPInvoiceMasterID);
                }

                invoice.Remarks = model.Remarks;
                invoice.PartyID = model.PartyID;
                invoice.AcFinancialYearID = yearid;
                if (model.StatusSDSC == "C")
                { invoice.AcHeadID = 52; }
                else
                { invoice.AcHeadID = 337; }

                invoice.BranchID = Convert.ToInt32(Session["CurrentBranchID"]);
                invoice.StatusSDSC = model.StatusSDSC;

                if (model.AcOPInvoiceMasterID == 0) // new entry
                {
                    db.AcOPInvoiceMasters.Add(invoice);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var IDetails = model.InvoiceDetailVM;
                foreach (var item in IDetails)
                {
                    var InvoiceDetail = new AcOPInvoiceDetail();
                    if (item.AcOPInvoiceDetailID > 0)
                    {
                        InvoiceDetail = db.AcOPInvoiceDetails.Find(item.AcOPInvoiceDetailID);
                        if (item.IsDeleted == true)
                        {
                            db.AcOPInvoiceDetails.Remove(InvoiceDetail);
                            db.SaveChanges();
                            continue;
                        }

                    }
                    else if (item.AcOPInvoiceDetailID == 0 && item.IsDeleted == false)
                    {
                        InvoiceDetail.AcOPInvoiceMasterID = invoice.AcOPInvoiceMasterID;
                    }
                    else
                    {
                        continue;
                    }

                    InvoiceDetail.InvoiceDate = item.InvoiceDate;
                    InvoiceDetail.LastTransDate = item.LastTransDate;
                    InvoiceDetail.Amount = item.Amount;
                    InvoiceDetail.StatusClose = item.StatusClose;
                    InvoiceDetail.InvoiceNo = item.InvoiceNo;
                    if (item.AcOPInvoiceDetailID == 0)
                    {
                        db.AcOPInvoiceDetails.Add(InvoiceDetail);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(InvoiceDetail).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                AccountsDAO.InvoiceOpeningPosting(invoice.AcOPInvoiceMasterID, yearid, branchid);
                return Json(new { status = "ok", message = "Opening Invoice updated Successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = "failed", message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);

            }
        }
        public JsonResult SaveOpInvoice(string Type, int PartyId, string Remarks, string Details)
        {
            try
            {
                var acopeningmaster = (from d in db.AcOPInvoiceMasters where d.StatusSDSC == Type && d.PartyID == PartyId select d).FirstOrDefault();
                if (acopeningmaster == null)
                {
                    acopeningmaster = new AcOPInvoiceMaster();
                }
                else
                {
                    var details = (from d in db.AcOPInvoiceDetails where d.AcOPInvoiceMasterID == acopeningmaster.AcOPInvoiceMasterID select d).ToList();
                    db.AcOPInvoiceDetails.RemoveRange(details);
                    db.SaveChanges();
                }
                acopeningmaster.AcFinancialYearID = Convert.ToInt32(Session["fyearid"]);
                acopeningmaster.AcHeadID = db.AcHeads.FirstOrDefault().AcHeadID;
                acopeningmaster.BranchID = Convert.ToInt32(Session["CurrentBranchID"]);
                acopeningmaster.StatusSDSC = Type;
                acopeningmaster.PartyID = PartyId;
                acopeningmaster.Remarks = Remarks;
                if (acopeningmaster.AcOPInvoiceMasterID == 0)
                {
                    db.AcOPInvoiceMasters.Add(acopeningmaster);
                }
                db.SaveChanges();
                var IDetails = JsonConvert.DeserializeObject<List<AcOPInvoiceDetail>>(Details);
                foreach (var item in IDetails)
                {
                    var InvoiceDetail = new AcOPInvoiceDetail();
                    InvoiceDetail.AcOPInvoiceMasterID = acopeningmaster.AcOPInvoiceMasterID;
                    InvoiceDetail.InvoiceDate = item.InvoiceDate;
                    InvoiceDetail.LastTransDate = item.LastTransDate;
                    InvoiceDetail.Amount = item.Amount;
                    InvoiceDetail.StatusClose = item.StatusClose;
                    InvoiceDetail.InvoiceNo = item.InvoiceNo;
                    db.AcOPInvoiceDetails.Add(InvoiceDetail);
                    db.SaveChanges();

                }
                return Json(new { status = "ok", message = "Opening Invoice updated Successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = "failed", message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);

            }
        }

        #endregion

        #region VoucherPrint
        public JsonResult AcReceipPaymenttVoucher(int id)
        {
            string reportpath = "";
            //int k = 0;
            if (id != 0)
            {
                reportpath = AccountsReportsDAO.GenerateReceiptPaymentVoucherPrint(id);

            }

            return Json(new { path = reportpath, result = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult AcJournalVoucher(int id)
        {
            string reportpath = "";
            //int k = 0;
            if (id != 0)
            {
                reportpath = AccountsReportsDAO.GenerateJournalVoucherPrint(id);

            }

            return Json(new { path = reportpath, result = "ok" }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region "AccountSetupPage"
        public ActionResult AccountSetupMaster()
        {           
            
            List<AccountSetupMasterVM> model = new List<AccountSetupMasterVM>();
            model = AccountsDAO.GetAccountSetupList();
            return View(model);
            
        }

        #endregion
    }



}

public class AcOpeningMasterVm
{
    public int AcOpeningID { get; set; }

    public int AcHeadID { get; set; }
    public decimal Amount { get; set; }
    public string AcHead { get; set; }
    public string AcNature { get; set; }
    public bool IsDeleted { get; set; }
}
public class SessionExpireAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        HttpContext ctx = HttpContext.Current;
        // check  sessions here
        if (HttpContext.Current.Session["UserID"] == null)
        {
            filterContext.Result = new RedirectResult("~/Home/Home");
            return;
        }
        base.OnActionExecuting(filterContext);
    }
}
public static class MvcHelpers
{
    public static string RenderPartialView2(this Controller controller, string viewName, object model)
    {
        if (string.IsNullOrEmpty(viewName))
            viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

        controller.ViewData.Model = model;
        using (var sw = new StringWriter())
        {
            ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
            var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
            viewResult.View.Render(viewContext, sw);

            return sw.GetStringBuilder().ToString();
        }
    }


}
public class DeleteFileAttribute : ActionFilterAttribute
{
    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
        filterContext.HttpContext.Response.Flush();

        //convert the current filter context to file and get the file path
        string filePath = (filterContext.Result as FilePathResult).FileName;

        //delete the file after download
        System.IO.File.Delete(filePath);
    }
}