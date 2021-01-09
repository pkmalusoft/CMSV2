using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Data;
using CrystalDecisions.CrystalReports.ViewerObjectModel;
using CMSV2.DAL;
using System.Data.Entity;
//using System.IO;
//using Newtonsoft.Json;
//using System.Text.RegularExpressions;
//using System.Net.Mail;
//using System.Configuration;
//using System.Collections.Specialized;
//using System.Net;
//using System.Text;
//using CMSV2.DAL;
//using System.Data.Entity;

namespace CMSV2.Controllers
{
    [SessionExpire]
    public class CODReceiptController : Controller
    {
        SourceMastersModel MM = new SourceMastersModel();
        RecieptPaymentModel RP = new RecieptPaymentModel();
        CustomerRcieptVM cust = new CustomerRcieptVM();
        Entities1 db = new Entities1();

        EditCommanFu editfu = new EditCommanFu();
        // GET: CODReceipt
        public ActionResult Index()
        {
            DatePicker datePicker = SessionDataModel.GetTableVariable();
            DatePicker model = new DatePicker();
            //string tz = "Arabian Standard Time";
            //DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            if (datePicker == null)
            {
                model = new DatePicker
                {
                    FromDate = CommanFunctions.GetFirstDayofMonth().Date,
                    ToDate = CommanFunctions.GetLastDayofMonth().Date //DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddHours(8)

                    //      Delete = (bool)Token.Permissions.Deletion,
                    //    Update = (bool)Token.Permissions.Updation,
                    //  Create = (bool)Token.Permissions.Creation
                };
            }
            else
            {
                model.FromDate = datePicker.FromDate;
                model.ToDate = datePicker.ToDate;

            }
            ViewBag.Token = model;
            SessionDataModel.SetTableVariable(model);
            return View(model);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "FromDate,ToDate")] DatePicker picker)
        {

            DatePicker model = new DatePicker
            {
                FromDate = picker.FromDate,
                ToDate = picker.ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                Delete = true, // (bool)Token.Permissions.Deletion,
                Update = true, //(bool)Token.Permissions.Updation,
                Create = true //.ToStrin//(bool)Token.Permissions.Creation
            };
            ViewBag.Token = model;
            SessionDataModel.SetTableVariable(model);
            return View(model);

        }

        public ActionResult Table()
        {
            int branchid = Convert.ToInt32(Session["CurrentBranchID"].ToString());
            int depotId = Convert.ToInt32(Session["CurrentDepotID"].ToString());

            DatePicker datePicker = SessionDataModel.GetTableVariable();
            ViewBag.Token = datePicker;

            List<CODReceiptVM> _Invoices = (from c in db.CODReceipts
                                                 join cust in db.AgentMasters on c.AgentID equals cust.AgentID
                                                 where (c.ReceiptDate >= datePicker.FromDate && c.ReceiptDate < datePicker.ToDate)
                                                 && c.Deleted == false
                                                 orderby c.ReceiptDate descending
                                                 select new CODReceiptVM
                                                 {
                                                     ReceiptID = c.ReceiptID,
                                                     ReceiptNo= c.ReceiptNo,
                                                     ReceiptDate = c.ReceiptDate,
                                                     AgentID = c.AgentID,
                                                     AgentName = cust.Name,
                                                     allocatedtotalamount=c.Amount

                                                 }).ToList();

            return View("Table", _Invoices);

        }

        public ActionResult Create(int id=0)
        {
            var CODReceiptSession= Session["CODReceipt"] as CODReceiptVM;

            CODReceiptVM vm = new CODReceiptVM();
            var branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            var companyid = Convert.ToInt32(Session["CurrentCompanyID"]);
            
            var acheadforcash = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();            
            var acheadforbank = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

            ViewBag.achead = acheadforcash;
            ViewBag.acheadbank = acheadforbank;
            List<CurrencyMaster> Currencys = new List<CurrencyMaster>();
            Currencys = MM.GetCurrency();
            ViewBag.Currency = new SelectList(Currencys, "CurrencyID", "CurrencyName");
            
            ViewBag.Agents = db.AgentMasters.ToList();            
            vm.ReceiptDetails = new List<CODReceiptDetailVM>();
            vm.allocatedtotalamount = 0;

            if (id>0)
            {
                ViewBag.Title = "COD Receipt - Modify";
                var receipt = db.CODReceipts.Find(id);
                vm.ReceiptID = receipt.ReceiptID;
                vm.ReceiptDate = receipt.ReceiptDate;
                vm.ReceiptNo = receipt.ReceiptNo;
                vm.Remarks = receipt.Remarks;
                vm.ManifestID = receipt.ManifestID;
                vm.CurrencyID = receipt.CurrencyID;
                vm.EXRate = receipt.EXRate;
                vm.AchHeadID = receipt.AchHeadID;                
                vm.Amount = receipt.Amount;
                vm.AgentID = receipt.AgentID;                                

                List<CODReceiptDetailVM> receiptdetails = (from c in db.CODReceiptDetails                                                           
                                                           join ins in db.InScanMasters on c.InScanId equals ins.InScanID
                                                      join i in db.ExportShipments on c.ManifestID  equals i.ID

                                                      where c.ReceiptID == vm.ReceiptID                                                       
                                                      select new CODReceiptDetailVM
                                                      {
                                                          InScanId = c.InScanId,
                                                          ManifestID = c.ManifestID,
                                                          ManifestNumber = i.ManifestNumber,
                                                          AWBNo = c.AWBNo,
                                                          AWBDate =Convert.ToDateTime(ins.TransactionDate),
                                                          Consignee = c.Consignee,
                                                          ConsigneePhone = c.ConsigneePhone,
                                                          CourierCharge = c.CourierCharge, 
                                                          OtherCharge = c.OtherCharge,
                                                          TotalCharge = c.TotalCharge,
                                                          AmountAllocate = c.AmountAllocate,
                                                          Discount =c.Discount                                                          
                                                      }).ToList();

                vm.ReceiptDetails = receiptdetails;
                vm.allocatedtotalamount = vm.Amount;
                Session["CODReceiptCreate"] = vm;
            }
            else if (CODReceiptSession!=null)
            {
                vm = CODReceiptSession;
            }
            else
            {
                ViewBag.Title = "COD Receipt - Create";
                PickupRequestDAO _dao = new PickupRequestDAO();
                vm.CurrencyID = SourceMastersModel.GetCompanyCurrencyID(companyid);
                vm.ReceiptNo = _dao.GetMaxCODReceiptNo(companyid, branchid);
                CODReceiptDetailVM detail = new CODReceiptDetailVM();                                               
            }
            
            return View(vm);
        }

        [HttpPost]
        public JsonResult GetManifest(int id)
        {
            var manifests = (from c in db.ExportShipments where c.AgentID == id select new { ID = c.ID, c.ManifestNumber }).ToList();
            return Json(new { data = manifests }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(CODReceiptVM model)
        {
            var branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            var companyid = Convert.ToInt32(Session["CurrentCompanyID"]);
            var fyearid = Convert.ToInt32(Session["fyearid"]);
            string savemessage = "";
            CODReceipt codreceipt = new CODReceipt();
            try
            {


                if (model.ReceiptID == 0)
                {
                    codreceipt.ReceiptNo = model.ReceiptNo;
                    codreceipt.ReceiptDate = model.ReceiptDate;
                    codreceipt.FYearID = fyearid;
                    codreceipt.Deleted = false;
                    codreceipt.BranchID = branchid;
                    codreceipt.AcCompanyID = companyid;
                    codreceipt.AgentID = model.AgentID;
                    codreceipt.AcJournalID = 0;
                    codreceipt.FMoney = 0;
                }
                else
                {
                    codreceipt = db.CODReceipts.Find(model.ReceiptID);
                }

                codreceipt.CurrencyID = model.CurrencyID;
                codreceipt.EXRate = model.EXRate;
                codreceipt.ManifestID = model.ManifestID;
                codreceipt.Amount = model.Amount;
                codreceipt.Remarks = model.Remarks;
                codreceipt.AchHeadID = model.AchHeadID;
                codreceipt.ChequeNo = model.ChequeNo;
                codreceipt.ChequeDate = model.ChequeDate;

                if (model.ReceiptID == 0)
                {
                    db.CODReceipts.Add(codreceipt);
                    db.SaveChanges();
                    savemessage = "You have successfully Saved the COD Receipt";
                }
                else
                {
                    db.Entry(codreceipt).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    savemessage = "You have successfully Updated the COD Receipt";
                }

                //Detail table save and udpate
                foreach (var item in model.ReceiptDetails)
                {
                    if (item.ReceiptDetailID == 0)
                    {
                        if (item.AmountAllocate > 0 || item.Discount > 0)
                        {
                            CODReceiptDetail detail = new CODReceiptDetail();
                            detail.ReceiptID = codreceipt.ReceiptID;
                            detail.InScanId = item.InScanId;
                            detail.ManifestID = item.ManifestID;
                            detail.AWBNo = item.AWBNo;
                            detail.Consignee = item.Consignee;
                            detail.ConsigneePhone = item.ConsigneePhone;
                            detail.CourierCharge = item.CourierCharge;
                            detail.OtherCharge = item.OtherCharge;
                            detail.TotalCharge = item.TotalCharge;
                            detail.AmountAllocate = item.AmountAllocate;
                            detail.Discount = item.Discount;
                            db.CODReceiptDetails.Add(detail);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        CODReceiptDetail detail = db.CODReceiptDetails.Find(item.ReceiptDetailID);
                        if (item.AmountAllocate > 0 || item.Discount > 0)
                        {
                            detail.ReceiptID = codreceipt.ReceiptID;
                            detail.InScanId = item.InScanId;
                            detail.ManifestID = item.ManifestID;
                            detail.AWBNo = item.AWBNo;
                            detail.Consignee = item.Consignee;
                            detail.ConsigneePhone = item.ConsigneePhone;
                            detail.CourierCharge = item.CourierCharge;
                            detail.OtherCharge = item.OtherCharge;
                            detail.TotalCharge = item.TotalCharge;
                            detail.AmountAllocate = item.AmountAllocate;
                            detail.Discount = item.Discount;
                            db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Entry(detail).State = System.Data.Entity.EntityState.Deleted;
                            db.SaveChanges();
                        }
                    }

                }

                PickupRequestDAO _dao = new PickupRequestDAO();
                _dao.GenerateCODPosting(codreceipt.ReceiptID);

                TempData["SuccessMsg"] = savemessage;
            }

            catch(Exception ex)
            {
                savemessage = ex.Message;
                TempData["WarningMsg"] = savemessage;
                

                var acheadforcash = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Cash" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();
                var acheadforbank = (from c in db.AcHeads join g in db.AcGroups on c.AcGroupID equals g.AcGroupID where g.AcGroup1 == "Bank" select new { AcHeadID = c.AcHeadID, AcHead = c.AcHead1 }).ToList();

                ViewBag.achead = acheadforcash;
                ViewBag.acheadbank = acheadforbank;
                List<CurrencyMaster> Currencys = new List<CurrencyMaster>();
                Currencys = MM.GetCurrency();
                ViewBag.Currency = new SelectList(Currencys, "CurrencyID", "CurrencyName");
                ViewBag.Agents = db.AgentMasters.ToList();
                return View(model);

            }

            return RedirectToAction("Index", "CODReceipt");
        }

        [HttpPost]
        public JsonResult GetManifestID(CODReceiptVM ship)
        {
            ship.ManifestID = "";
            if (ship.SelectedValues != null)
            {
                foreach (var item in ship.SelectedValues)
                {
                    if (ship.ManifestID == "")
                    {
                        ship.ManifestID = item.ToString();
                    }
                    else
                    {
                        ship.ManifestID = ship.ManifestID + "," + item.ToString();
                    }

                }
            }
            return Json(new { manifestids = ship.ManifestID }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]        
        public ActionResult GetManifestAWB(CODReceiptVM ship)
        {
            ship.ManifestID = "";
            if (ship.SelectedValues != null)
            {
                foreach (var item in ship.SelectedValues)
                {
                    if (ship.ManifestID == "")
                    {
                        ship.ManifestID = item.ToString();
                    }
                    else
                    {
                        ship.ManifestID = ship.ManifestID + "," + item.ToString();
                    }

                }
            }


            List<CODReceiptDetailVM> manifests = (from c in db.ExportShipments  join d in db.ExportShipmentDetails on c.ID equals d.ExportID
                             join i in db.InScanMasters on d.InscanId equals i.InScanID
                             where c.AgentID == ship.AgentID &&  ship.ManifestID.Contains(c.ID.ToString()) 
                             &&  i.PaymentModeId==2
                             && i.IsDeleted==false
                             && i.NetTotal>0 //COd
                               select new CODReceiptDetailVM { InScanId  = i.InScanID,ManifestID=c.ID,ManifestNumber=c.ManifestNumber,
                                   AWBNo=d.AWB ,AWBDate= i.TransactionDate,Consignee=i.Consignee,ConsigneePhone=i.ConsigneePhone,CourierCharge=((decimal)(i.CourierCharge != null ? i.CourierCharge : 0)),OtherCharge=i.OtherCharge,TotalCharge=(decimal)(i.NetTotal !=null ? i.NetTotal : 0)}).ToList();

            

            if  (ship.Amount >0)
            {
                decimal totalamount = ship.Amount;
                int i = 0;
                while(totalamount>0 && i<manifests.Count)
                {
                    if (manifests[i].TotalCharge<=totalamount)
                    {
                        manifests[i].AmountAllocate = manifests[i].TotalCharge;
                        manifests[i].Discount = 0;
                        totalamount = totalamount - manifests[i].AmountAllocate;
                        i++;
                    }
                    else
                    {
                        manifests[i].AmountAllocate = totalamount;
                        manifests[i].Discount = 0;
                        totalamount = totalamount - manifests[i].AmountAllocate;
                        i++;

                    }
                }
                if (totalamount>0)
                {
                    ship.allocatedtotalamount = totalamount;
                }
                else
                {
                    ship.allocatedtotalamount = ship.Amount;
                }

            }
            ship.ReceiptDetails = manifests;
            Session["CODReceiptCreate"] = ship;
            
            return PartialView("ReceiptDetail", ship);            


        }

        public ActionResult DeleteConfirmed(int id)
        {
            CODReceipt a = db.CODReceipts.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {

                //var _inscans = db.InScanMasters.Where(cc => cc.InvoiceID == id).ToList();
                //foreach (InScanMaster _inscan in _inscans)
                //{
                //    _inscan.InvoiceID = null;
                //    db.Entry(_inscan).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
                a.Deleted = true;
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMsg"] = "You have successfully deleted COD Receipt.";


                return RedirectToAction("Index");
            }
        }

        public ActionResult Details(int id)
        {
        
            CODReceiptVM vm = new CODReceiptVM();
            var branchid = Convert.ToInt32(Session["CurrentBranchID"]);
            var companyid = Convert.ToInt32(Session["CurrentCompanyID"]);                    
                        
            vm.ReceiptDetails = new List<CODReceiptDetailVM>();
            if (id > 0)
            {
                var receipt = db.CODReceipts.Find(id);
                vm.ReceiptID = receipt.ReceiptID;
                vm.ReceiptDate = receipt.ReceiptDate;
                vm.ReceiptNo = receipt.ReceiptNo;
                vm.Remarks = receipt.Remarks;
                vm.ManifestID = receipt.ManifestID;
                vm.CurrencyID = receipt.CurrencyID;
                vm.EXRate = receipt.EXRate;
                vm.AchHeadID = receipt.AchHeadID;
                vm.AcHeadName = db.AcHeads.Find(receipt.AchHeadID).AcHead1;
                vm.CurrencyName = db.CurrencyMasters.Find(receipt.CurrencyID).CurrencyName;
                vm.Amount = receipt.Amount;
                vm.AgentID = receipt.AgentID;
                vm.AgentName = db.AgentMasters.Find(vm.AgentID).Name;
                List<CODReceiptDetailVM> receiptdetails = (from c in db.CODReceiptDetails
                                                           join ins in db.InScanMasters on c.InScanId equals ins.InScanID
                                                           join i in db.ExportShipments on c.ManifestID equals i.ID

                                                           where c.ReceiptID == vm.ReceiptID
                                                           select new CODReceiptDetailVM
                                                           {
                                                               InScanId = c.InScanId,
                                                               ManifestID = c.ManifestID,
                                                               ManifestNumber = i.ManifestNumber,
                                                               AWBNo = c.AWBNo,
                                                               AWBDate = ins.TransactionDate,
                                                               Consignee = c.Consignee,
                                                               ConsigneePhone = c.ConsigneePhone,
                                                               CourierCharge = c.CourierCharge,
                                                               OtherCharge = c.OtherCharge,
                                                               TotalCharge = c.TotalCharge,
                                                               AmountAllocate = c.AmountAllocate,
                                                               Discount = c.Discount
                                                           }).ToList();

                vm.ReceiptDetails = receiptdetails;            
            }
            

            return View(vm);
        }
    }
}