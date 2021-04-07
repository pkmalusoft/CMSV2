//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CMSV2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class InScanMaster
    {
        public int InScanID { get; set; }
        public string EnquiryNo { get; set; }
        public string AWBNo { get; set; }
        public Nullable<System.DateTime> PickupRequestDate { get; set; }
        public string RequestSource { get; set; }
        public Nullable<int> AcCompanyID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> DepotID { get; set; }
        public string DeviceID { get; set; }
        public Nullable<int> CustomerID { get; set; }
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
        public string ConsigneeAddress2_Street { get; set; }
        public string ConsigneeAddress3_PinCode { get; set; }
        public string ConsigneePhone { get; set; }
        public string ConsigneeCountryName { get; set; }
        public string ConsigneeCityName { get; set; }
        public string ConsigneeLocationName { get; set; }
        public Nullable<int> AssignedEmployeeID { get; set; }
        public Nullable<decimal> CourierCharge { get; set; }
        public string PickupRemarks { get; set; }
        public Nullable<System.DateTime> PickupReadyTime { get; set; }
        public Nullable<int> EnteredByID { get; set; }
        public Nullable<bool> IsEnquiry { get; set; }
        public Nullable<int> CourierStatusID { get; set; }
        public Nullable<int> PickupRequestStatusId { get; set; }
        public Nullable<int> PickedUpEmpID { get; set; }
        public Nullable<System.DateTime> PickedupDate { get; set; }
        public string Pieces { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public string CargoDescription { get; set; }
        public Nullable<double> StatedWeight { get; set; }
        public Nullable<decimal> CBM_length { get; set; }
        public Nullable<decimal> CBM_width { get; set; }
        public Nullable<decimal> CBM_height { get; set; }
        public Nullable<decimal> CBM { get; set; }
        public string BagNo { get; set; }
        public string PalletNo { get; set; }
        public string HandlingInstruction { get; set; }
        public string SpecialInstructions { get; set; }
        public Nullable<int> TypeOfGoodID { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> MovementID { get; set; }
        public Nullable<int> ProductTypeID { get; set; }
        public Nullable<int> CustomerRateID { get; set; }
        public Nullable<int> QuickInscanID { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public Nullable<decimal> OtherCharge { get; set; }
        public Nullable<decimal> NetTotal { get; set; }
        public Nullable<int> ParcelTypeId { get; set; }
        public Nullable<decimal> MaterialCost { get; set; }
        public Nullable<decimal> CustomsValue { get; set; }
        public Nullable<int> SubReasonId { get; set; }
        public Nullable<int> StatusTypeId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> PaymentModeId { get; set; }
        public Nullable<int> DepotReceivedBy { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public Nullable<int> InvoiceID { get; set; }
        public bool CustomerShipperSame { get; set; }
        public Nullable<int> DRSID { get; set; }
        public Nullable<int> AcFinancialYearID { get; set; }
        public Nullable<int> AcJournalID { get; set; }
        public Nullable<int> ManifestID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string InvoiceTo { get; set; }
        public Nullable<int> AcHeadID { get; set; }
        public string OriginPlaceID { get; set; }
        public string DestinationPlaceID { get; set; }
        public string PickupLocation { get; set; }
        public string DeliveryLocation { get; set; }
        public bool IsNCND { get; set; }
        public bool IsCashOnly { get; set; }
        public bool IsChequeOnly { get; set; }
        public bool IsCollectMaterial { get; set; }
        public bool IsDOCopyBack { get; set; }
        public Nullable<int> VehicleTypeId { get; set; }
        public string PickupSubLocality { get; set; }
        public string DeliverySubLocality { get; set; }
        public Nullable<int> DRRId { get; set; }
        public Nullable<int> CODReceiptId { get; set; }
        public string ConsignorMobileNo { get; set; }
        public string ConsigneeMobileNo { get; set; }
        public bool AWBProcessed { get; set; }
        public Nullable<int> PrepaidAWBID { get; set; }
        public Nullable<int> AWBBookIssueID { get; set; }
        public Nullable<int> BATCHID { get; set; }
        public Nullable<decimal> TaxPercent { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
    }
}
