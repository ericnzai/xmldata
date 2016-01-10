using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Resources;
using System.ComponentModel.DataAnnotations.Schema;
using eXml.Helpers;
namespace eXml.Entities
{
    public enum enPostType
    {
        //[EnumDescription("Invoices - with address")]
        //InvoicesWithAddress = 1,
        // [EnumDescription("Invoices - without address")]
        //InvoiceWithoutAddress = 17,
        [EnumDescription("Invoices (12.5% & 5%) - with address")]
        Invoice_12_5_WithAddress =12,
        //[EnumDescription("Invoices (12.5% & 5%) - without address")]
        //Invoice_12_5_WithoutAddress =18,
        //[EnumDescription("Invoices (12.5% & 5%) - without address - with receipt date")]
        //Invoice_12_5_WithoutAddress_WithRecptDate = 20,
        //[EnumDescription("Invoices - Gujrat (12.5% & 2.5%)")]
        //Invoice_12_5_Gujrat = 14,
        //[EnumDescription("Payment Receipts")]
        //PaymentReceipts = 2,
        //[EnumDescription("Commission (Journal)")]
        //Commision_Journal = 5,
        //[EnumDescription("Commission (Payment)")]
        //Commision_Payment = 6,
        [EnumDescription("Purchase")]
        Purchase = 4,
        //[EnumDescription("Purchase - Gujrat")]
        //Purchase_Gujrat = 15,
        //[EnumDescription("Expenses")]
        //Expenses = 7,
        //[EnumDescription("Invoices (SC/PF)")]
        //Invoices_SC_PF = 8,
        //[EnumDescription("Inventory Sales - with address")]
        //InventorySales_WithAddress = 11,
        //[EnumDescription("Inventory Sales - without address")]
        //Inventory_Sales_WithoutAddress = 23,
        //[EnumDescription("Inventory Sales - Gujrat - without address")]
        //Inventory_Sales_Gujrat_WithoutAddress = 21,
        //[EnumDescription("Inventory Sales - Gujrat - with address")]
        //Inventory_Sales_Gujrat_WithAddress = 22,
        //[EnumDescription("Inventory Purchase")]
        //InventoryPurchase = 10,
        //[EnumDescription("Consultant")]
        //Consultant = 16
    }
    public enum enPaymentStatus
    {
        [EnumDescription("Pending")]
        Pending =1,
         [EnumDescription("Partial")]
        Partial = 2,
         [EnumDescription("Received")]
        Received = 3
    }
    public enum enInventoryStatus
    {
        [EnumDescription("Pending")]
        Pending = 1,
        [EnumDescription("Partial")]
        Partial = 2,
        [EnumDescription("Received")]
        Received = 3
    }
    public enum enPaymentType
    {
        Cash = 1,
        Cheque = 2
    }
    public class PostedTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string  Company { get; set; }
        public string Year { get; set; }
        public string Week { get; set; }
        public DateTime PostDate { get; set; }
        public string AssemblyName{ get; set; }
        public string ConsultantName { get; set; }
        public string ConsultantCode { get; set; }
        public string UnitName { get; set; }
        public decimal? GrossAmount { get; set; }
        public decimal ConsultantPrice { get; set; }
        public decimal NetAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public enPostType PostType { get; set; }
        public enPaymentType? PaymentType { get; set; }
        public string CofNo { get; set; }
        public decimal CofValue { get; set; }
        public enPaymentStatus PayStatus { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ChequeNo { get; set; }
        public string BankName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int OrderId { get; set; }
        public decimal OrderQty { get; set; }
        public string Status { get; set; }
        public enInventoryStatus InventoryStatus { get; set; }
    }
    public class PurchaseRegister
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Company { get; set; }
        public string DbShip { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ItemCode { get; set; }
        public string PaymentInstrument { get; set; }
        public string ItemName { get; set; }
        public string OrdYearWk { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceType { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvYearWk { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceWOTax { get; set; }
        public decimal VAT { get; set; }
        public decimal PriceInclVAT { get; set; }
    }
}