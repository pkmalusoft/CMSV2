using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMSV2.Models
{
    public class AWBBatchVM : AWBBatch
    {
        public string CODCustomerName { get; set; }
        public string CASHCustomerName { get; set; }
        
        public string FOCCustomerName { get; set; }
        public int FOCCustomerID { get; set; }
        public int CODCustomerID { get; set; }
        public int CASHCustomerId { get; set; }
            public int CustomerID { get; set; }
        public int MovementID { get; set; }
        public int ParcelTypeID { get; set; }
        public int ProductTypeID { get; set; }
        public bool AssignedForCollection { get; set; } //Assigned for Collection
        public int AssignedEmployeeID { get; set; }
        public DateTime AssignedDate { get; set; }  //PickupRequestDate

        public bool CollectedBy { get; set; } //collected By        
        public int PickedUpEmpID { get; set; } //collected        
        public DateTime? PickedupDate { get; set; }

        public bool ReceivedBy { get; set; } //Depot Received by //Inscan Received at origin
        public DateTime? ReceivedDate { get; set; }
        public int DepotReceivedBy { get; set; }

        public bool OutScanDelivery { get; set; } //Out scan Deliver
        public int OutScanDeliveredID { get; set; } //Out scan Deliver
        public DateTime? OutScanDate { get; set; } //Out scan Deliver

        public bool DeliveryAttempted { get; set; } //Delivery Attempt Date
        public int DeliveryAttemptedBy { get; set; } //
        public DateTime? DelieveryAttemptDate { get; set; }

        public bool Delivered { get; set; } //Delivered Date
        public int DeliveredBy { get; set; } //
        public DateTime? DeliveredDate { get; set; }

        public int Pieces { get; set; }
        public decimal Weight { get; set; }
        public int VehiceId { get; set; }
        public string AWBNo { get; set; }
        public DateTime AWBDate { get; set; }
        public string CustomerName { get; set; }
        public string Shipper { get; set; }
        public string ConsignorContact { get; set; }
        public string ConsignorPhone { get; set; }
        public string ConsignorMobileNo { get; set; }
        public string ConsignorAddress1_Building { get; set; }
        public string ConsignorAddress2_Street { get; set; }
        public string ConsignorAddress3_PinCode { get; set; }
        public string ConsignorCityName { get; set; }
        public string ConsignorCountryName { get; set; }

        public string ConsignorLocationName { get; set; }

        public string Consignee { get; set; }
        public string ConsigneeCityName { get; set; }

        public string ConsigneeLocationName { get; set; }
        public string ConsigneeContact { get; set; }
        public string ConsigneePhone { get; set; }
        public string ConsigneeMobileNo { get; set; }
        public string ConsigneeAddress1_Building { get; set; }
        public string ConsigneeAddress2_Street { get; set; }
        public string ConsigneeAddress3_PinCode { get; set; }
        public string ConsigneeCountryName { get; set; }

        
        public decimal CourierCharge { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal OtherCharge { get; set; }
        public decimal NetTotal { get; set; }
        public decimal MaterialCost { get; set; }
        public int PaymentTypeId { get; set; }
        public string PickUpLocation {get;set;}
        public string DeliveryLocation { get; set; }
        public string ItemDescription { get; set; }
        public string PickupSubLocality { get; set; }
        public string DeliverySubLocality { get; set; }
        public string OriginPlaceID { get; set; }
        public string DestinationPlaceID { get; set; }
        public int InscanVehicleId { get; set; }
        public int OutscanVehicleId { get; set; }
        public string Specialnstructions { get; set; }
        public List<AWBBatchDetail> Details { get; set; }
    }


    public class AWBBatchDetail
    {
        AWBBatchDetail()
        {
            ConsignorMobileNo = "";
            ConsigneeMobileNo = "";
            PickupSubLocality = "";
            DeliverySubLocality = "";
            ConsigneePhone = "";
            ConsignorPhone = "";
            ConsignorLocationName = "";
            ConsignorCountryName = "";
            ConsignorPhone = "";
            ConsignorAddress1_Building = "";
            ConsignorAddress2_Street = "";
            ConsignorAddress3_PinCode = "";
            ConsignorMobileNo = "";
            ConsigneeMobileNo = "";
            ConsigneeCountryName = "";
            ConsigneeLocationName = "";
            ConsigneeAddress1_Building = "";
            ConsigneeAddress2_Street = "";
            ConsigneeAddress3_PinCode = "";
        }

        public string AWBNo { get; set; }
           public int CustomerID  { get; set; }
           public string Consignor { get; set; }
        public string ConsignorContact { get; set; }
        public string ConsignorAddress1_Building { get; set; }
        public string ConsignorAddress2_Street { get; set; }
           public string ConsignorAddress3_PinCode { get; set; }
           public string ConsignorPhone { get; set; }
        public string ConsignorCountryName { get; set; }
        public string ConsignorCityName { get; set; }
        public string ConsignorLocationName { get; set; }
        public string Consignee { get; set; }
        public string ConsigneeContact { get; set; }
        public string ConsigneeAddress1_Building { get; set; }
        public string  ConsigneeAddress2_Street { get; set; }
        public string ConsigneeAddress3_PinCode { get; set; }
        public string ConsigneePhone { get; set; }
        public string ConsigneeCountryName { get; set; }
        public string ConsigneeCityName { get; set; }
        public string ConsigneeLocationName { get; set; }
        public string AssignedEmployeeID { get; set; }
        public decimal CourierCharge { get; set; }
        public decimal TaxPercent{ get; set; }
        public decimal TaxAmount { get; set; }
        public decimal OtherCharge { get; set; }
        public decimal NetTotal { get; set; }
        //[DataType(DataType.Date)]
        public string PickupReadyTime { get; set; }
        public int CourierStatusID { get; set; }
        public int PickedUpEmpID { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string PickedupDate { get; set; }
        public string Pieces { get; set; }
        public decimal Weight { get; set; }
        public string CargoDescription { get; set; }
        public int MovementID { get; set; }
        public int ProductTypeID { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        //public DateTime TransactionDate { get; set; }
        public bool IsEnquiry { get; set; }
        public string RequestSource { get; set; }
        public string TransactionDate { get; set; }      
        public int ParcelTypeId { get; set; }
        public decimal MaterialCost { get; set; }
        public int StatusTypeId { get; set; }
        
           public int PaymentModeId { get; set; }
        public int DepotReceivedBy { get; set; }
        public int DocumentTypeId { get; set; }
    public string OriginPlaceID { get; set; }
    public string DestinationPlaceID { get; set; }
        public string PickupLocation { get; set; }
    public string DeliveryLocation { get; set; }
           public string PickupSubLocality { get; set; }
        public string DeliverySubLocality { get; set; }
        public string ConsignorMobileNo { get; set; }
        public string ConsigneeMobileNo { get; set; }
        public bool AssignedForCollection { get; set; } //Assigned for Collection
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public string AssignedDate { get; set; }  //PickupRequestDate
        public bool CollectedBy { get; set; } //collected By        
        public bool ReceivedBy { get; set; } //Depot Received by //Inscan Received at origin
        public bool OutScanDelivery { get; set; } //Out scan Deliver
        public bool DeliveryAttempted { get; set; } //Delivery Attempt Date
        public bool Delivered { get; set; } //Delivered Date
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public string ReceivedDate { get; set; }
        public int OutScanDeliveredID { get; set; } //Out scan Deliver
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public string OutScanDate { get; set; } //Out scan Deliver       
        public int DeliveryAttemptedBy { get; set; } //
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public string DelieveryAttemptDate { get; set; }        
        public int DeliveredBy { get; set; } //
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public string DeliveredDate { get; set; }
        public int InscanVehicleId { get; set; }
		public int OutscanVehicleId { get; set; }
        public string SpecialInstructions { get; set; }

        public string PickupRequestDate { get; set; }

    }

    public class AWBInfo
    {
        public AWBInfo()
        {
            CourierCharge = 0;
        }
        public string AWBNo { get; set; }
        public string Status  { get; set; }
        public string Mode { get; set; }
        public int ReferenceID{ get; set; }
        public int CustomerID{ get; set; }
        public string OriginLocation{ get; set; }
        public string DestinationLocation {get; set; }
        public decimal CourierCharge {get; set;}
        public string CustomerName { get; set; }
        public string LocationName { get; set; }
        public string CountryName{ get; set; }
        public string CityName { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Address1 { get; set; }
        public string Address2{ get; set; }
        public string Address3 { get; set; }
        public string  PickupSubLocality { get; set; }
        public string DeliverySubLocality { get; set; }
        public string OriginPlaceID { get; set; }
        public string DestinationPlaceID { get; set; }



}
}