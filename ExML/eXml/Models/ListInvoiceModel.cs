using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eXml.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eXml.Models
{
    public class ListInvoiceModel
    {
        public int Id { get; set;}
        public string Assembly { get; set; }
        public string Unit { get; set; }
        public string YrWk { get; set; }
        public string InvoiceNo { get; set; }
        public string Consultant { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? PendingAmount { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public enPaymentStatus PayStatus { get; set; }
        public enPostType PostType { get; set; }
        public enPaymentType? PayType { get; set; }

        public string Status
        {
            get { return Enum.GetName(typeof(enPaymentStatus), PayStatus).ToString(); }
        }
    }
    public class ListInvoiceTransModel
    {
        [Key]
        public int Id { get; set; }
        public string YrWk { get; set; }
        public string InvoiceNo { get; set; }
        public string AssemblyName { get; set; }
        public string UnitName { get; set; }
        public string Consultant { get; set; }
        public string ConsultantName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }
        [Required]
        public decimal PaymentAmount { get; set; }
        [Required]
        public DateTime? PaymentDate { get; set; }
        public string BankName { get; set; }
        public string ChequeNo { get; set; }
        [Required]
        public enPaymentType? PaymentType { get; set; }
        public enPaymentStatus? PayStatus { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Status
        {
            get
            {
                if (PayStatus != null)
                {
                    return Enum.GetName(typeof(enPaymentStatus), PayStatus).ToString();
                }
                else return null;

            }

        }
        public string Type
        {
            get
            {
                if (PaymentType != null)
                {
                    return Enum.GetName(typeof(enPaymentType), PaymentType).ToString();
                }
                else return null;
            }
        }
    }
    public class ListInvoiceReportModel
    {
        [Key]
        public int Id { get; set; }
        public string YrWk { get; set; }
        //public string InvoiceNo { get; set; }
        public string AssemblyName { get; set; }
        public string UnitName { get; set; }
        public string Consultant { get; set; }
        public string ConsultantName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public enPaymentStatus? PayStatus { get; set; }
        public DateTime PostDate { get; set; }
        public string Status
        {
            get
            {
                if (PayStatus != null)
                {
                    return Enum.GetName(typeof(enPaymentStatus), PayStatus).ToString();
                }
                else return null;

            }

        }
    }
    public class ListPaymentReportModel
    {
        [Key]
        public int Id { get; set; }
        public string AssemblyName { get; set; }
        public string UnitName { get; set; }
        public string Consultant { get; set; }
        public string ConsultantName { get; set; }
        public string YrWk { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public enPaymentType? PayType { get; set; }
        public enPaymentStatus? PayStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime PostDate { get; set; }
        public string Status
        {
            get {
                if (PayStatus != null)
                {
                    return Enum.GetName(typeof(enPaymentStatus), PayStatus).ToString();
                }
                else return null;

                }
                
        }
        public string Type
        {
            get {
                if (PayType != null)
                {
                    return Enum.GetName(typeof(enPaymentType), PayType).ToString();
                }
                else return null;
            }
        }
    }
    public class ListInventoryReportModel
    {
        [Key]
        public long Id { get; set; }
        public string AssemblyName { get; set; }
        public string UnitName { get; set; }
        public string Consultant { get; set; }
        public string ConsultantName { get; set; }
        public string YrWk { get; set; }
        public string Item { get; set; }
        public string ItemName { get; set; }
        public decimal QtyReceived { get; set; }
        public decimal QtySold { get; set; }
        public enInventoryStatus InvStatus { get; set; }
        public DateTime PostDate { get; set; }
        public string Status
        {
            get
            {
                if (InvStatus != null)
                {
                    return Enum.GetName(typeof(enPaymentStatus), InvStatus).ToString();
                }
                else return null;

            }

        }
    }
}