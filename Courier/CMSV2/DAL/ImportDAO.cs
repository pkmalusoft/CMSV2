using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using CMSV2.Models;
using System.Configuration;
namespace CMSV2.DAL
{
    public class ImportDAO
    {
        public static List<ImportManifestVM> GetImportManifestList(int ShipmentTypeId)
        {
            ImportManifestSearch paramobj = (ImportManifestSearch)(HttpContext.Current.Session["ImportManifestSearch"]);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_ImportManifestList";
            cmd.CommandType = CommandType.StoredProcedure;
            if (paramobj.AWBNo==null)
            {
                paramobj.AWBNo = "";
            }
            cmd.Parameters.AddWithValue("@AWBNO", paramobj.AWBNo);

            if (paramobj.FromDate != null)
                cmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(paramobj.FromDate).ToString("MM/dd/yyyy"));
            else
                cmd.Parameters.AddWithValue("@FromDate", "");

            if (paramobj.ToDate != null)
                cmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(paramobj.ToDate).ToString("MM/dd/yyyy"));
            else
                cmd.Parameters.AddWithValue("@ToDate", "");


            cmd.Parameters.AddWithValue("@ShipmentTypeId", ShipmentTypeId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<ImportManifestVM> objList = new List<ImportManifestVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ImportManifestVM     obj = new ImportManifestVM();
                    obj.ID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.ManifestNumber= ds.Tables[0].Rows[i]["ManifestNumber"].ToString();
                    obj.CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"].ToString()); // CommanFunctions.ParseDate(ds.Tables[0].Rows[i]["RecPayDate"].ToString());                    
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static List<ImportManifestVM> GetTranshipmentManifestList(int ShipmentTypeId)
        {
            ImportManifestSearch paramobj = (ImportManifestSearch)(HttpContext.Current.Session["TranshipmentSearch"]);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_ImportManifestList";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@AWBNO", "");
            if (paramobj.FromDate != null)
                cmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(paramobj.FromDate).ToString("MM/dd/yyyy"));

            if (paramobj.ToDate != null)
                cmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(paramobj.ToDate).ToString("MM/dd/yyyy"));

            cmd.Parameters.AddWithValue("@ShipmentTypeId", ShipmentTypeId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            List<ImportManifestVM> objList = new List<ImportManifestVM>();

            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ImportManifestVM obj = new ImportManifestVM();
                    obj.ID = CommanFunctions.ParseInt(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.ManifestNumber = ds.Tables[0].Rows[i]["ManifestNumber"].ToString();
                    obj.CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["CreatedDate"].ToString()); // CommanFunctions.ParseDate(ds.Tables[0].Rows[i]["RecPayDate"].ToString());                    
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static string GetMaxManifestNo(int Companyid, int BranchId,DateTime ManifestDate,string ShipmentType)
        {
            DataTable dt = new DataTable();
            string MaxPickUpNo = "";
            try
            {
                //string json = "";
                string strConnString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "GetMaxManifestNo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompanyId", Companyid);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@ManifestDate",ManifestDate.ToString("yyyyMMdd"));
                        cmd.Parameters.AddWithValue("@ShipmentType", ShipmentType);

                        con.Open();
                        SqlDataAdapter SqlDA = new SqlDataAdapter(cmd);
                        SqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                            MaxPickUpNo = dt.Rows[0][0].ToString();


                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return MaxPickUpNo;

        }


        public static List<TranshipmentModel> GetTranshipmenItems(int ImportShipmentTypeId)
        {
            var awbList = new List<TranshipmentModel>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(CommanFunctions.GetConnectionString);
            cmd.CommandText = "SP_GetTranshipmentItem";
            cmd.CommandType = CommandType.StoredProcedure;            
            cmd.Parameters.AddWithValue("@ImportShipmentId", ImportShipmentTypeId);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

           

            if (ds != null && ds.Tables.Count > 0)
            {
                int i = 1;
                foreach (DataRow objDataRow in ds.Tables[0].Rows)
                {                    
                    awbList.Add(new TranshipmentModel()
                    {
                        SNo = i++,
                        HAWBNo = objDataRow["HAWBNo"].ToString(),
                        AWBDate = objDataRow["AWBDate"].ToString(),
                        Customer = objDataRow["Customer"].ToString(),
                        ConsignorPhone = objDataRow["ConsignorPhone"].ToString(),
                        Consignor = objDataRow["Consignor"].ToString(),
                        ConsignorLocationName = objDataRow["ConsignorLocationName"].ToString(),
                        ConsignorCountryName = objDataRow["ConsignorCountryName"].ToString(),
                        ConsignorCityName = objDataRow["ConsignorCityName"].ToString(),
                        Consignee = objDataRow["Consignee"].ToString(),
                        ConsigneeCountryName = objDataRow["ConsigneeCountryName"].ToString(),
                        ConsigneeCityName = objDataRow["ConsigneeCityName"].ToString(),
                        ConsigneeLocationName = objDataRow["ConsigneeLocationName"].ToString(),
                        ConsignorAddress1_Building = objDataRow["ConsignorAddress1_Building"].ToString(),
                        ConsignorMobile = objDataRow["ConsignorMobile"].ToString(),
                        ConsigneeMobile = objDataRow["ConsigneeMobile"].ToString(),
                        Weight = CommanFunctions.ParseDecimal(objDataRow["Weight"].ToString()),
                        Pieces = objDataRow["Pieces"].ToString(),
                        CourierCharge = CommanFunctions.ParseDecimal(objDataRow["CourierCharge"].ToString()),
                        OtherCharge = CommanFunctions.ParseDecimal(objDataRow["OtherCharge"].ToString()),
                        PaymentMode = objDataRow["PaymentMode"].ToString(),
                        ReceivedBy = objDataRow["ReceivedBy"].ToString(),
                        CollectedBy = objDataRow["CollectedBy"].ToString(),
                        FAWBNo = objDataRow["FAWBNo"].ToString(),
                        FAgentName = objDataRow["FAgentName"].ToString(),
                        CourierType = objDataRow["CourierType"].ToString(),
                        MovementType = objDataRow["MovementType"].ToString(),
                        CourierStatus = objDataRow["CourierStatus"].ToString(),
                        remarks = objDataRow["remarks"].ToString() //Department and Bag no is missing                                                               

                    });
                    //AWBNo AWBDate Bag NO.	Shipper ReceiverName    ReceiverContactName ReceiverPhone   ReceiverAddress DestinationLocation DestinationCountry Pcs Weight CustomsValue    COD Content Reference Status  SynchronisedDateTime

                }
            }
            return awbList;
        }
    }
}