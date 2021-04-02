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
    public class PODStatusUpdationController : Controller
    {
       Entities1 db=new Entities1();


        public ActionResult Index()
        {
        //    List<PODStatusVM> lst = (from c in db.PODs join t in db.CourierStatus on c.CourierStatusID equals t.CourierStatusID select new PODStatusVM { PODID = c.PODID, RecevierName = c.ReceiverName, Remarks = c.Remarks, CourierStatus = t.CourierStatus }).ToList();
       
          var data = db.PODs.ToList();
            List<PODStatusVM> lst = new List<PODStatusVM>();
            foreach(var Item in data)
            {
                PODStatusVM obj = new PODStatusVM();
                obj.CourierStatusID = Item.CourierStatusID;
                obj.RecevierName = Item.ReceiverName;
                obj.Remarks = Item.Remarks;
                obj.PODID = Item.PODID;
                lst.Add(obj);
            }

            return View(lst);
        }


           public ActionResult Create()
        {
            ViewBag.delivery = db.CourierStatus.ToList();
            return View();
        }

          [HttpPost]

        public ActionResult Create(PODStatusVM v)
        {

       
                //AWBStatu obj = new AWBStatu();

                //int max = (from c in db.AWBStatus orderby c.AWBStatusID descending select c.AWBStatusID).FirstOrDefault();


                ////List<PODStatusVM> lst = (from c in db.CourierStatus join t in db.AWBStatus on c.CourierStatusID equals t.StatusDescriptionID select new PODStatusVM { CourierStatusID = c.CourierStatusID }).ToList();

                //if (max == null)
                //{
                //    obj.AWBStatusID =  1;
                //    obj.StatusDescriptionID = v.CourierStatusID;
                //}
                //else
                //{
                //    obj.AWBStatusID = max+1;
                //    obj.StatusDescriptionID = v.CourierStatusID;
                //}

                //POD ob = new POD();
                //int data = (from c in db.PODs orderby c.PODID descending select c.PODID).FirstOrDefault();

                //if (data == null)
                //{
                //    ob.PODID =  1;
                //    ob.ReceiverName = v.RecevierName;
                //    ob.Remarks = v.Remarks;

                //}
                //else
                //{
                //    ob.PODID = data+1;
                //    ob.ReceiverName = v.RecevierName;
                //    ob.Remarks = v.Remarks;

                //}


                //db.AWBStatus.Add(obj);
                //db.SaveChanges();


                //db.PODs.Add(ob);
                //db.SaveChanges();


                return RedirectToAction("Index");
            


          
        }




          public ActionResult Edit(int id)
          {
              PODStatusVM ob = new PODStatusVM();
              ViewBag.delivery = db.CourierStatus.ToList();

              POD data = (from c in db.PODs where c.PODID == id select c).FirstOrDefault();
           
              if (data == null)
              {
                  return HttpNotFound();
              }
              else
              {
                  ob.CourierStatusID = data.CourierStatusID;
                  ob.RecevierName = data.ReceiverName;
                  ob.Remarks = data.Remarks;
              }

              return View(ob);
          }
        [HttpPost]
          public ActionResult Edit(PODStatusVM v)
          {
              AWBStatu obj = new AWBStatu();

              obj.AWBStatusID = v.AWBStatusID;
              obj.StatusDescriptionID = v.CourierStatusID;



              POD ob = new POD();
              ob.PODID = v.PODID;
              ob.ReceiverName = v.RecevierName;
              ob.Remarks = v.Remarks;

              db.Entry(obj).State = EntityState.Modified;
              db.SaveChanges();

              db.Entry(ob).State = EntityState.Modified;
              db.SaveChanges();

              ////var insacn = db.AWBStatus.Where(itm => itm.AWBStatusID == v.AWBStatusID).FirstOrDefault();
              ////insacn.AWBStatusID = v.AWBStatusID;
              ////insacn.StatusDescriptionID = v.CourierStatusID;
              ////db.SaveChanges();


              //var insacn1 = db.PODs.Where(itm => itm.PODID == v.PODID).FirstOrDefault();
              //insacn1.PODID = v.PODID;
              //insacn1.ReceiverName = v.RecevierName;
              //insacn1.Remarks = v.Remarks;
              //db.SaveChanges();

              return RedirectToAction("Index");
          }























        public JsonResult getpodshippment(string id)
        {
            var item=(from c in db.InScans where c.AWBNo==id select c).FirstOrDefault();

            PODStatusVM obj=new PODStatusVM();

             if (item != null)
            {
                obj.InscanID = item.InScanID;
                obj.AWBNo = item.AWBNo;
                //obj.Date = DateTime.Now;
                obj.CollectedBy = item.CollectedBy;
                obj.StatedWeight = item.StatedWeight;
                obj.Pieces = item.Pieces;
                obj.CourierCharges = item.CourierCharge;
                obj.Consignor = item.Consignor;
                obj.ConsignorCountryID = item.ConsignorCountryID.Value;
                obj.Consignee = item.Consignee;
                obj.CosigneeCountryID = item.ConsigneeCountryID;
                obj.StatusPaymentMOde = item.StatusPaymentMode;
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteConfirmed(int id = 0)
        {
            AWBStatu a = db.AWBStatus.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.AWBStatus.Remove(a);
                db.SaveChanges();

                List<POD> lst = (from c in db.PODs where c.PODID == id select c).ToList();

                foreach (var item in lst)
                {
                    db.PODs.Remove(item);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
        }


    }
}
