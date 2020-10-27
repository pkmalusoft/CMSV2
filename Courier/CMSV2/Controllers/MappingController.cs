using HealthCareApp;

using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSV2.Models;
using CMSV2;
using System.Data;
using System.Data.Entity;
using OfficeOpenXml.Core.ExcelPackage;

namespace CMSV2.Controllers
{
    public class MappingController : Controller
    {
        #region Local Variables

        private HttpPostedFileBase file;
        Entities1 db = new Entities1();
       
        #endregion

        

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: Mapping
        //public ActionResult Mapping()
        //{
            

        //    #region Local Variables
        //    MappingManager manager = new MappingManager();
        //    MappingToDBColumnModel model = new MappingToDBColumnModel();
        //    int noOfCol, noOfRow, totalRecordinExcel = 0;
        //    #endregion

        //    model.ExcelColumns = new List<Models.ExcelColumn>();
        //    model.DBColumns = manager.GetTableColumnNames("");
        //    model.TableList = manager.GetTableList();

        //    file = (HttpPostedFileBase)TempData["SelectedFile"];
        //    TempData["SelectedFile"] = file;

        //    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
        //    {
        //        var package = new ExcelPackage(file.InputStream);
        //        var currentSheet = package.Workbook.Worksheets;
        //        var workSheet = currentSheet;// ..First();
        //        noOfCol = workSheet.Dimension.End.Column;
        //        noOfRow = workSheet.Dimension.End.Row;
        //        totalRecordinExcel = noOfRow;
        //        Models.ExcelColumn column;
        //        for (int i = 1; i <= noOfCol; i++)
        //        {
        //            column = null;
        //            column = new Models.ExcelColumn();
        //            column.CoumnName = Convert.ToString(workSheet.Cells[1, i].Value);
        //            model.ExcelColumns.Add(column);
        //        }
        //    }

        //    return View(model);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Mapping([Bind(Include = "DBColumns, ExcelColumns, TableList, TableName")] MappingToDBColumnModel model)
        {
            #region Local Variable
            string message = "";
            int count = 0;
            int noOfCol, noOfRow, totalRecordinExcel = 0;
            #endregion

            if (null == model.TableList)
            {
                MappingManager manager = new MappingManager();
                model.TableList = manager.GetTableList();
            }


            foreach (DBColumn item in model.DBColumns)
            {
                count = model.DBColumns.Where(t => t.CoumnName == item.CoumnName).ToList().Count();

                if (count > 1 && null != item.CoumnName)
                {
                    message = "Duplicate column selectated. Please select unique column.";
                    break;
                }

                if (count == model.DBColumns.Count())
                {
                    message = "Please map atleast one field.";
                }
            }

            if (string.IsNullOrEmpty(message))
            {
                
                string conStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ToString();
                SqlConnection con = new SqlConnection(conStr);
                SqlCommand comnd;
                con.Open();

                string tableName = "tempTable";
                string query = "";

                file = (HttpPostedFileBase)TempData["SelectedFile"];
                TempData["SelectedFile"] = file;

                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    var package = new ExcelPackage(file.InputStream);
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet; //.First();
                    noOfCol = workSheet.Dimension.End.Column;
                    noOfRow = workSheet.Dimension.End.Row;
                    totalRecordinExcel = noOfRow;

                    string columnsstring = "";
                    string valuestring = "";


                    ImpExp e = new ImpExp();
                    e.Employee = Convert.ToInt32(Session["UserID"].ToString());
                    e.ImportDate = DateTime.Now;
                    e.Status = "Uploaded";
                    e.FName = Session["fname"].ToString();

                    db.ImpExps.Add(e);
                    db.SaveChanges();

                    Session["ImpExpID"] = e.ImpExpID;


                    for (int row = 2; row <= noOfRow; row++)
                    {
                        query = "Insert into " + tableName +" ";
                        columnsstring = "(";
                        valuestring = "(";

                        if (workSheet.Cells[row, 1].Value != null)
                        {
                            for (int col = 1; col <= noOfCol; col++)
                            {


                                if (!string.IsNullOrEmpty(model.DBColumns[col - 1].CoumnName))
                                {
                                    columnsstring += model.DBColumns[col - 1].CoumnName + ",";
                                    valuestring += "'" + workSheet.Cells[row, col].Value + "',";
                                }
                            }
                            columnsstring += "ImportID,";
                            valuestring += "'" + e.ImpExpID + "',";


                            columnsstring = columnsstring.Trim(',');
                            valuestring = valuestring.Trim(',');

                            columnsstring += ")";
                            valuestring += ")";

                            query += columnsstring + " values " + valuestring + ";";

                            comnd = new SqlCommand(query, con);
                            try
                            {
                                comnd.ExecuteNonQuery();
                            }
                            catch (SqlException exc)
                            {
                                TempData["Message"] = "Error : " + exc.Message;
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }



                    //try
                    //{
                    //    DataTable dt = new DataTable();
                    //    SqlDataAdapter da = new SqlDataAdapter();
                    //    SqlCommand cmd1 = new SqlCommand();
                    //    string query1 = "select top " +  noOfRow + " AWBNo,CourierCharge,OtherCharge,Consignee,Consignor,CourierType,DestinationLocation,CBM,Weight,Pieces,Discount,NetTotal,FAgent,FAWBNo,NCND,Department,ReceivedBy,CollectedBy from " + tableName + " order by AWBNo desc";
                    //    con.Close();
                    //    con.Open();
                    //    cmd1.Connection = con;
                    //    cmd1.CommandText = query1;
                    //    da.SelectCommand = cmd1;
                    //    da.Fill(dt);

                    //    for (int i = dt.Rows.Count-1; i >= 0; i--)
                    //    {
                    //        DataRow r=dt.Rows[i];

                    //        int max = (from c in db.InScans orderby c.InScanID descending select c.InScanID).FirstOrDefault();

                    //        max = max + 1;


                    //        string consignor = r[4].ToString();

                    //        int ConsignorID = (from c in db.CustomerMasters where c.CustomerName.Contains(consignor) select c.CustomerID).FirstOrDefault();
                    //        //int ConsigneeID = (from c in db.CustomerMasters where c.CustomerName.Contains(r[3].ToString()) select c.CustomerID).FirstOrDefault();

                    //        if (ConsignorID == null || ConsignorID == 0)
                    //        {
                    //            TempData["Message"] = "The ConsignorID Not Found.";
                    //            throw new Exception();

                    //        }
                            
                            
                    //        string dest = r[6].ToString();
                    //        string cby = r[17].ToString();
                    //        string rcby = r[16].ToString();
                        
                    //        int destinationID = (from c in db.CountryMasters where c.CountryName.Contains(dest) select c.CountryID).FirstOrDefault();
                    //        if (destinationID == null || destinationID == 0)
                    //        {
                    //            throw new Exception();
                    //        }
                    //        int CollectedByemp = (from c in db.EmployeeMasters where c.EmployeeName.Contains(cby) select c.EmployeeID).FirstOrDefault();
                    //        if (CollectedByemp == null || CollectedByemp == 0)
                    //        {
                    //            TempData["Message"] = "The Collected By Not Found.";
                    //            throw new Exception();
                    //        }
                    //        int recbyemp = (from c in db.EmployeeMasters where c.EmployeeName.Contains(rcby) select c.EmployeeID).FirstOrDefault();
                    //        if (recbyemp == null || recbyemp == 0)
                    //        {
                    //            TempData["Message"] = "The Recevied By ID Not Found";
                    //            throw new Exception();
                    //        }
                         
                        

                    //        InScan s = new InScan();
                    //        s.InScanID = max;

                            
                    //        s.AWBNo = r[0].ToString();
                    //        s.CourierCharge = Convert.ToDecimal(r[1].ToString());
                    //        s.OtherCharge = Convert.ToDecimal(r[2].ToString());
                    //        s.Consignee = r[3].ToString();
                    //        s.Consignor = r[4].ToString();
                    //        s.CourierType = r[5].ToString();
                    //        s.DestinationLocation = r[6].ToString();
                    //        s.CBM = Convert.ToDecimal(r[7].ToString());
                    //        s.Weight = Convert.ToDecimal(r[8].ToString());
                    //        s.Pieces = r[9].ToString();
                    //        s.Discount = Convert.ToDecimal(r[10].ToString());
                    //        s.NetTotal = Convert.ToDecimal(r[11].ToString());
                    //        s.FAgent = r[12].ToString();
                    //        s.FAWBNo = r[13].ToString();
                    //        s.NCND = r[14].ToString();
                    //        s.Department = r[15].ToString();
                    //        s.CustomerID = ConsignorID;
                    //        s.DestinationID = destinationID.ToString();
                    //        s.CollectedBy = CollectedByemp;
                    //        s.ReceivedBy = recbyemp;

                    //        //Need To Confirm
                    //        s.UserID = 12;
                    //        s.StatusTransit = false;
                    //        s.InScanDate = DateTime.Now;
                    //        s.CollectedByName = r[17].ToString();
                    //        s.ReceivedByName = r[16].ToString();

                    //        //End

                    //        db.InScans.Add(s);
                    //        db.SaveChanges();

                    //        query1 = "delete from " + tableName + " where AWBNo=" + s.AWBNo;
                    //        cmd1.CommandText = query1;
                    //        cmd1.ExecuteNonQuery();


                    //    }

                       
                    //    con.Close();




                    //}
                    //catch(Exception ex)
                    //{

                  
                    //    return RedirectToAction("Index", "Home");
                    //}
                }
                con.Close();
            }

            if(string.IsNullOrEmpty(message))
            {
                TempData["Message"] = "Data inserted successfully.";
                return RedirectToAction("Verify", "Mapping");

            }
            else
            {
                TempData["Message"] = message;
                MappingManager manager = new MappingManager();
                model.DBColumns = manager.GetTableColumnNames(model.TableName);
                return View("Mapping", model);
            }
        }


        public ActionResult EditImp(int id)
        {
            var data = db.tempTables.ToList().Where(x => x.ImportID == id).ToList();
            if (data.Count > 0)
            {
                Session["ImpExpID"] = id;
                return RedirectToAction("Verify", "Mapping", data);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Verify()
        {
            int impid = Convert.ToInt32(Session["ImpExpID"].ToString());
            var data = db.tempTables.Where(x => x.ImportID == impid).ToList();
            if (data.Count > 0)
            {
                return View(data);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }




        #region Ajax

        //Ajax call for Destination
        public JsonResult GetDestination(string istring)
        {
            var data = db.CountryMasters.Where(x => x.CountryName.StartsWith(istring)).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }




        public class mov
        {
            public string Movement { get; set; }
        }
        public JsonResult GetMovement(string move)
        {
            List<mov> lst = new List<mov>();

            var data = (from c in db.CourierMovements where c.MovementType.StartsWith(move) select c).ToList();
            foreach (var item in data)
            {
                mov e = new mov();
                e.Movement = item.MovementType;
                lst.Add(e);
            }
            return Json(lst,JsonRequestBehavior.AllowGet);
        }

        public class prd
        {
            public string ProductType { get; set; }
        }
        public JsonResult Getproduct(string prdc)
        {
            List<prd> lst = new List<prd>();

            var data = (from c in db.ProductTypes where c.ProductName.StartsWith(prdc) select c).ToList();
            foreach (var item in data)
            {
                prd e = new prd();
                e.ProductType = item.ProductName;
                lst.Add(e);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        public class custmorclass
        {
            public string CustomerRateType { get; set; }
        }

        public JsonResult GetCustmor(string Custom)
        {
            List<custmorclass> lst = new List<custmorclass>();

            var data = (from c in db.CustomerRateTypes where c.CustomerRateType1.StartsWith(Custom) select c).ToList();
            foreach (var item in data)
            {
                custmorclass c = new custmorclass();
                c.CustomerRateType = item.CustomerRateType1;
                lst.Add(c);
            }


            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetEmployee(string ipstr)
        {

            List<Emp> lst = new List<Emp>();
            var data = (from c in db.EmployeeMasters where c.EmployeeName.StartsWith(ipstr) select c).ToList();
            foreach (var item in data)
            {
                Emp e = new Emp();
                e.EmpName = item.EmployeeName;
                lst.Add(e);
            }

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public class Emp
        {
            public string EmpName { get; set; }

        }

        public class Branch
        {
            public string BranchName { get; set; }

        }
        public JsonResult GetAllBranches(string i)
        {
            List<Branch> lst = new List<Branch>();

            var data = db.BranchMasters.Where(x => x.BranchName.StartsWith(i)).ToList();
            foreach (var item in data)
            {
                Branch e = new Branch();
                e.BranchName = item.BranchName;
                lst.Add(e);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }







        public class CourierType
        {
            public string CType { get; set; }
        }

        public JsonResult GetCourierType(string istr)
        {
            List<CourierType> lst = new List<CourierType>();

            var data = db.ParcelTypes.Where(x => x.ParcelType1.StartsWith(istr)).ToList();
            foreach (var item in data)
            {
                CourierType e = new CourierType();
                e.CType = item.ParcelType1;
                lst.Add(e);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);

        }

        public class FAgent
        {
            public string FAName { get; set; }
        }

        public JsonResult GetFAgent(string str)
        {
            List<FAgent> lst = new List<FAgent>();
            var data = db.ForwardingAgentMasters.Where(x => x.FAgentName.StartsWith(str)).ToList();
            foreach (var item in data)
            {
                FAgent e = new FAgent();
                e.FAName = item.FAgentName;
                lst.Add(e);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateData(string awb, string ctype, string consignor, string consignee, string dest, string colby, decimal cbm, decimal weight, decimal pieces, decimal ccharge, decimal ocharge, decimal discount, decimal nettotal, string recby, string fagent, string fawb, string ncnd, string branch, string origincountry, string move)
        {
            var d = db.tempTables.Where(x => x.AWBNo == awb).FirstOrDefault();

            tempTable s = db.tempTables.Where(t => t.AWBNo == awb).FirstOrDefault();
          
            s.AWBNo = awb;
            s.CourierType = ctype;
            s.Consignor = consignor;
            s.Consignee = consignee;
            s.DestinationLocation = dest;
            s.CollectedBy = colby;
            s.CBM = cbm;
            s.Weight = weight;
            s.Pieces = pieces;
            s.CourierCharge = ccharge;
            s.OtherCharge = ocharge;
            s.Discount = discount;
            s.NetTotal = nettotal;
            s.ReceivedBy = recby;
            s.FAgent = fagent;
            s.FAWBNo = fawb;
            s.NCND = ncnd;
            s.Branch = branch;
            s.OriginLocation = origincountry;
            s.CourierMovementType = move;
           
            db.Entry(s).State = EntityState.Modified;
            db.SaveChanges();

            ImpExp e = db.ImpExps.Find(Convert.ToInt32(Session["ImpExpID"].ToString()));
            e.Status = "Verified";
            e.Employee = e.Employee;
            e.ImportDate = e.ImportDate;
            e.ImpExpID = e.ImpExpID;
            e.FName = e.FName;
           
            db.Entry(e).State = EntityState.Modified;
            db.SaveChanges();

            Msg m=new Msg();
            m.message="Data Updated..!";
            return Json(m, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ImportData()
        {
            int id = Convert.ToInt32(Session["ImpExpID"].ToString());
            var data = (from c in db.tempTables where c.ImportID==id orderby c.AWBNo ascending select c).ToList();

            foreach (var item in data)
            {


                string dest = item.DestinationLocation;
                string cby = item.CollectedBy;
                string rcby = item.ReceivedBy;
                string ctype = item.CourierType;
                string branch = item.Branch;
                string fagent = item.FAgent;
                string origin=item.OriginCounty;
                string move=item.CourierMovementType;
               // string prdcd = item.ProductType;
               // string crt = item.CustmorRateType;
                int destinationID = (from c in db.CountryMasters where c.CountryName.Equals(dest) select c.CountryID).FirstOrDefault();
              
                int CollectedByemp = (from c in db.EmployeeMasters where c.EmployeeName.Equals(cby) select c.EmployeeID).FirstOrDefault();
               
                int recbyemp = (from c in db.EmployeeMasters where c.EmployeeName.Equals(rcby) select c.EmployeeID).FirstOrDefault();

                int ctypeid = (from c in db.ParcelTypes where c.ParcelType1.Equals(ctype) select c.ID).FirstOrDefault();

                int fagentid = (from c in db.ForwardingAgentMasters where c.FAgentName.Equals(fagent) select c.FAgentID).FirstOrDefault();

                int branchid = (from c in db.BranchMasters where c.BranchName.Equals(branch) select c.BranchID).FirstOrDefault();

                int origincountryid = (from c in db.CountryMasters where c.CountryName.Equals(origin) select c.CountryID).FirstOrDefault();

               int coumoveid = (from c in db.CourierMovements where c.MovementType.Equals(move) select c.MovementID).FirstOrDefault();
               //int prd = (from c in db.ProductTypes where c.ProductName.Equals(prdcd) select c.ProductTypeID).FirstOrDefault();
               //int crtd = (from c in db.CustomerRateTypes where c.CustomerRateType1.Equals(crt) select c.CustomerRateTypeID).FirstOrDefault();
             
                int max = (from c in db.InScans orderby c.InScanID descending select c.InScanID).FirstOrDefault();

                

                InScan s = new InScan();
                s.InScanID = max + 1;


                s.AWBNo = item.AWBNo;
                s.CourierCharge = item.CourierCharge;
                s.OtherCharge = item.OtherCharge;
                s.Consignee = item.Consignor;
                s.Consignor = item.Consignee;
                s.CourierType = item.CourierType;

                s.DestinationLocation = item.DestinationLocation;
                s.CBM = item.CBM;
                s.Weight = item.Weight;
                s.Pieces = item.Pieces.ToString();
                s.Discount = item.Discount;
                s.NetTotal = item.NetTotal;
                s.FAgent = item.FAgent;
                s.FAWBNo = item.FAWBNo;
                s.NCND = item.NCND;
                s.Department = item.Branch;
                s.DepartmentID = branchid;

                //s.CustomerID = ConsignorID;
                s.ConsigneeCountryID = destinationID;
                s.ConsignorCountryID = origincountryid;
                s.CollectedBy = CollectedByemp;
                s.ReceivedBy = recbyemp;

              
                s.UserID = 12;
                s.StatusTransit = false;
                s.InScanDate = DateTime.Now;
                s.CollectedByName = item.CollectedBy;
                s.ReceivedByName = item.ReceivedBy;
                s.OriginID = origincountryid;
                s.MovementID = coumoveid;
                //s.ProductTypeID = prd;
//s.CustomerRateTypeID = crtd;
               // s.PackingCharge = item.PackingCharge;
                

                db.InScans.Add(s);
                db.SaveChanges();

                var d = (from c in db.tempTables where c.AWBNo == item.AWBNo select c).FirstOrDefault();
                db.tempTables.Remove(d);
                db.SaveChanges();

               



            }

            ImpExp e = db.ImpExps.Find(Convert.ToInt32(Session["ImpExpID"].ToString()));
            e.Status = "Updated";
            e.Employee = e.Employee;
            e.ImportDate = e.ImportDate;
            e.ImpExpID = e.ImpExpID;
            e.FName = e.FName;

            db.Entry(e).State = EntityState.Modified;

            db.SaveChanges();
            TempData["Message"] = "Data Updated successfully.";
            return RedirectToAction("Index", "Home");

        }

        public class Msg
        {
            public string message { get; set; }
        }
        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public ActionResult FillColumn(string TableName)
        {
            MappingManager manager = new MappingManager();
            List<DBColumn> columnlist = manager.GetTableColumnNames(TableName);
            return Json(columnlist, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}