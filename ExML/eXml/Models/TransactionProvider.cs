using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using eXml.Entities;
using eXml.Models;
using System.Collections;
using System.Web.Mvc;

namespace eXml.Models
{
    public static class TransactionProvider
    {
        public static IEnumerable<PostedTransaction> EditableTransactions(string assembly, string unit,DateTime? datefrom, DateTime? dateto)
        {
            IEnumerable<PostedTransaction> lstTrans = new List<PostedTransaction>();
            using (var db = new eXmlContext())
            {
                    lstTrans = db.Set<PostedTransaction>()
                               .Where(x => DbFunctions.TruncateTime (x.PostDate) >= DbFunctions.TruncateTime(datefrom)
                                   && DbFunctions.TruncateTime( x.PostDate) <= DbFunctions.TruncateTime( dateto) )
                                .ToList();
                    if (!string.IsNullOrWhiteSpace(assembly))
                        if (assembly != "ALL") lstTrans = lstTrans.Where(x => x.AssemblyName == assembly).ToList();
                    if (!string.IsNullOrWhiteSpace(unit))
                        if (unit != "ALL") lstTrans = lstTrans.Where(x => x.UnitName == unit).ToList();
            }
            return lstTrans;
        }
        public static List<ListInvoiceTransModel> InvoiceEditableTransactions(string assembly, string unit, DateTime? datefrom, DateTime? dateto)
        {
            var listInvTrans = new List<ListInvoiceTransModel>();
            using (var db = new eXmlContext())
            {
                listInvTrans = db.Set<ListInvoiceTransModel>()
                               .Where(x => DbFunctions.TruncateTime (x.InvoiceDate) >= DbFunctions.TruncateTime (datefrom )
                                   && DbFunctions.TruncateTime(x.InvoiceDate) <= DbFunctions.TruncateTime(dateto))
                                .Distinct()
                               .ToList();

                if (!string.IsNullOrWhiteSpace(assembly))
                    if (assembly != "ALL") listInvTrans = listInvTrans.Where(x => x.AssemblyName == assembly).ToList();
                if (!string.IsNullOrWhiteSpace(unit))
                    if (unit != "ALL") listInvTrans = listInvTrans.Where(x => x.UnitName == unit).ToList();

            }

            return listInvTrans;
        }
        public static List<ListInvoiceReportModel> InvoiceReportsTransactions(string assembly, string unit,
            string consultant, string status, DateTime? datefrom, DateTime? dateto)
        {
            var listInvReprts = new List<ListInvoiceReportModel>();
            using (var db = new eXmlContext())
            {
                 listInvReprts = db.Set<ListInvoiceReportModel>()
                                .Where(x => DbFunctions.TruncateTime(x.PostDate) >= DbFunctions.TruncateTime(datefrom)
                                    && DbFunctions.TruncateTime(x.PostDate) <= DbFunctions.TruncateTime(dateto))
                                .ToList();

                   if (!string.IsNullOrWhiteSpace(assembly)) 
                       if(assembly != "ALL") listInvReprts = listInvReprts.Where(x => x.AssemblyName == assembly).ToList();
                   if (!string.IsNullOrWhiteSpace(unit)) 
                       if( unit != "ALL") listInvReprts = listInvReprts.Where(x => x.UnitName == unit).ToList();
                   if (!string.IsNullOrWhiteSpace(consultant)) 
                       if (consultant != "ALL") listInvReprts = listInvReprts.Where(x => x.ConsultantName == consultant).ToList();
                   if (!string.IsNullOrWhiteSpace(status))
                       if(status != "ALL") listInvReprts = listInvReprts.Where(x => x.Status == status).ToList();
            }
          
            return listInvReprts;
        }
        public static List<ListPaymentReportModel> PaymentReportsTransactions(string assembly, string unit,
            string consultant, string status, string type, DateTime? datefrom, DateTime? dateto, DateTime? datePfrom, DateTime? datePto)
        {
            var listPReprts = new List<ListPaymentReportModel>();
            using (var db = new eXmlContext())
            {
                listPReprts = db.Set<ListPaymentReportModel>()
                            .Where(x => DbFunctions.TruncateTime(x.PostDate) >= DbFunctions.TruncateTime(datefrom)
                                && DbFunctions.TruncateTime(x.PostDate) <= DbFunctions.TruncateTime(dateto))
                            .Where(x => DbFunctions.TruncateTime(x.PaymentDate) >= DbFunctions.TruncateTime(datePfrom)
                                && DbFunctions.TruncateTime(x.PaymentDate) <= DbFunctions.TruncateTime(datePto))
                            .ToList();
                if (!string.IsNullOrWhiteSpace(assembly))
                    if (assembly != "ALL") listPReprts = listPReprts.Where(x => x.AssemblyName == assembly).ToList();
                if (!string.IsNullOrWhiteSpace(unit))
                    if (unit != "ALL") listPReprts = listPReprts.Where(x => x.UnitName == unit).ToList();
                if (!string.IsNullOrWhiteSpace(consultant))
                    if (consultant != "ALL") listPReprts = listPReprts.Where(x => x.ConsultantName == consultant).ToList();
                if (!string.IsNullOrWhiteSpace(status))
                    if (status != "ALL") listPReprts = listPReprts.Where(x => x.Status == status).ToList();
                if (!string.IsNullOrWhiteSpace(type))
                    if (type != "ALL") listPReprts = listPReprts.Where(x => x.Type == type).ToList();
            }
            return listPReprts;
        }
        public static List<ListInventoryReportModel> InventoryReportsTransactions(string assembly, string unit,
            string consultant, string status, string item, DateTime? datefrom, DateTime? dateto)
        {
            var listInvReprts = new List<ListInventoryReportModel>();
            using (var db = new eXmlContext())
            {
                listInvReprts = db.Set<ListInventoryReportModel>()
                                .Where(x => DbFunctions.TruncateTime(x.PostDate) >= DbFunctions.TruncateTime(datefrom)
                                    && DbFunctions.TruncateTime(x.PostDate) <= DbFunctions.TruncateTime(dateto))
                                .ToList();

                if (!string.IsNullOrWhiteSpace(assembly))
                    if (assembly != "ALL") listInvReprts = listInvReprts.Where(x => x.AssemblyName == assembly).ToList();
                if (!string.IsNullOrWhiteSpace(unit))
                    if (unit != "ALL") listInvReprts = listInvReprts.Where(x => x.UnitName == unit).ToList();
                if (!string.IsNullOrWhiteSpace(consultant))
                    if (consultant != "ALL") listInvReprts = listInvReprts.Where(x => x.ConsultantName == consultant).ToList();
                if (!string.IsNullOrWhiteSpace(status))
                    if (status != "ALL") listInvReprts = listInvReprts.Where(x => x.Status == status).ToList();
                if (!string.IsNullOrWhiteSpace(item))
                    if (item != "ALL") listInvReprts = listInvReprts.Where(x => x.ItemName == item).ToList();
            }

            return listInvReprts;
        }
        public static List<SelectListItem> LoadAssemblyUnits(string assembly)
        {
            List<SelectListItem> listAUnits = new List<SelectListItem>();
            IList<string> units;
            using (var db = new eXmlContext())
            {
                units = db.Set<PostedTransaction>()
                    .Where(x => x.AssemblyName == assembly)
                    .Select(x => x.UnitName)
                    .Distinct()
                    .ToList();
            }
            int i = 0;
            foreach (var unit in units)
            {
                i++;
                SelectListItem listAssUnit = new SelectListItem
                {
                    Value = unit.ToString(),
                    Text = unit.ToString(),
                    Selected = (i == 1)
                };
                listAUnits.Add(listAssUnit);
            }
            return listAUnits;
        }
        public static List<SelectListItem> LoadUnitConsultants(string unit)
        {
            List<SelectListItem> listUConsults = new List<SelectListItem>();
            IList<string> consultants;
            using (var db = new eXmlContext())
            {
                consultants = db.Set<PostedTransaction>()
                    .Where(x => x.UnitName == unit)
                    .Select(x => x.ConsultantName)
                    .Distinct()
                    .ToList();
            }
            int i = 0;
            foreach (var consultant in consultants)
            {
                i++;
                SelectListItem listConsltnt = new SelectListItem
                {
                    Value = consultant.ToString(),
                    Text = consultant.ToString(),
                    Selected = (i == 1)
                };
                listUConsults.Add(listConsltnt);
            }
            return listUConsults;
        }
        public static List<SelectListItem> LoadAssemblies()
        {
            List<SelectListItem> listAssemblies = new List<SelectListItem>();
            List<string> assemblies;
            using (var db = new eXmlContext())
            {
                assemblies = db.Set<PostedTransaction>()
                            .Select(x => x.AssemblyName)
                            .Distinct()
                            .ToList();
            }
            foreach (var asmbly in assemblies)
            {
                SelectListItem listAItem = new SelectListItem
                {
                    Value = asmbly.ToString(),
                    Text = asmbly.ToString(),
                    Selected = false
                };
                listAssemblies.Add(listAItem);
            }

            return listAssemblies;
        }
        public static List<SelectListItem> LoadItems()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            List<string> items;
            using (var db = new eXmlContext())
            {
                items = db.Set<PostedTransaction>()
                            .Select(x => x.ItemName)
                            .Distinct()
                            .ToList();
            }
            foreach (var itm in items)
            {
                SelectListItem listAItem = new SelectListItem
                {
                    Value = itm.ToString(),
                    Text = itm.ToString(),
                    Selected = false
                };
                listItems.Add(listAItem);
            }

            return listItems;
        }
    }

    public  class UnitName
    {
        public UnitName()
        {
            UnitConsultants = new HashSet<UnitConsultant>();
        }
        public string Unit { get; set; }
        public bool IsGroupCreated { get; set; }
        public virtual ICollection<UnitConsultant> UnitConsultants { get; set; }
    }
    public  class UnitConsultant
    {
        public UnitConsultant()
        {
            ConsultantOrders = new HashSet<ConsultantOrder>();
        }
        public virtual UnitName UnitName { get; set; }
        public string ConsultantId { get; set; }
        public string Consultant { get; set; }
        public virtual ICollection<ConsultantOrder> ConsultantOrders { get; set; }
    }
    public  class ConsultantOrder
    {
  
        public virtual UnitConsultant Consultant { get; set; }
        public int OrderId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal OrdQty { get; set; }
        public decimal MRP { get; set; }
        public decimal ConsultantPrice { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public int VoucherId { get; set; }
    }
    public class StockItem
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
    }
    public class Invoice
    {
        public Invoice()
        {
            Items = new HashSet<InvoiceItem>();
        }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceType { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvYearWk { get; set; }
        public string OrdYearWk { get; set; }
        public virtual ICollection<InvoiceItem> Items { get; set; }
    }
    public class InvoiceItem
    {
        public string InvoiceNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceWOTax { get; set; }
        public decimal VAT { get; set; }
        public decimal PriceInclVAT { get; set; }
        public virtual Invoice Invoice { get; set; }
        public string PaymentInstrument { get; set; }
    }
}