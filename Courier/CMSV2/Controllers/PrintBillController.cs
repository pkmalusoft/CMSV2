using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace CMSV2.Controllers
{
    public class PrintBillController : Controller
    {
        Entities1 db = new Entities1();


        public ActionResult PrintAWB(string id)
        {

            var data = db.InScans.Where(x => x.AWBNo == id).FirstOrDefault();

            PrintAWBVM v = new PrintAWBVM();

            v.shipper = data.Consignor;
            v.shipperaddres = data.ConsignorAddress;
            v.shippercontact = data.ConsignorContact;
            v.origin = (from c in db.CountryMasters where c.CountryID == data.ConsignorCountryID select c.CountryName).FirstOrDefault();
            v.ConsignorPhone = data.ConsignorPhone;

            v.consignee = data.Consignee;
            v.Consigneeaddress = data.ConsigneeAddress;
            v.consigneecontact = data.ConsigneeContact;

            @ViewBag.shipper = data.Consignor;
            @ViewBag.shipperaddress = data.ConsignorAddress;
            @ViewBag.shippercontact = data.ConsignorContact;
            @ViewBag.origin = v.origin;
            @ViewBag.ConsignorPhone = data.ConsignorPhone;


            @ViewBag.consignee = data.Consignee;
            @ViewBag.Consigneeaddress = data.ConsigneeAddress;
            @ViewBag.Consigneecontact = data.ConsigneeContact;
            @ViewBag.destination = (from c in db.CountryMasters where c.CountryID == data.ConsigneeCountryID select c.CountryName).FirstOrDefault();
            @ViewBag.Consigneephone = data.ConsigneePhone;

            @ViewBag.refno=data.ReferenceAWBNo;
            @ViewBag.date=data.InScanDate;
            @ViewBag.weight = data.StatedWeight;
            @ViewBag.pieces = data.Pieces;

            @ViewBag.cargo = data.CargoDescription;
            @ViewBag.awb = data.AWBNo;

            @ViewBag.hbl = "BH+400000";
            ViewBag.track = "JD01 1936 4089 7001 2922";
            return View();
        }

         

    }
}
