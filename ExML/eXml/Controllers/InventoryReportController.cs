using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eXml.Models;
using eXml.Entities;
using eXml.Helpers;

namespace eXml.Controllers
{
    [T2TAuthorize]
    public class InventoryReportController : Controller
    {
        //
        // GET: /InventoryReport/
        [ValidateInput(false)]
        public ActionResult Report()
        {
            ViewData["InvStatus"] = EnumHelper.ToList(typeof(enPaymentStatus));
            ViewData["Assemblies"] = TransactionProvider.LoadAssemblies();
            ViewData["Items"] = TransactionProvider.LoadItems();
            return View(TransactionProvider.InventoryReportsTransactions(null, null, null,null,null, null,null));
        }
        [ValidateInput(false)]
        public ActionResult _GridListInventoryReport(string assembly, string unit, string consultant, string status,
            string item, DateTime? datefrom, DateTime? dateto)
        {
            try
            {
                ViewData["PayStatus"] = EnumHelper.ToList(typeof(enPaymentStatus));
                ViewData["Items"] = TransactionProvider.LoadItems();
                return PartialView("_GridListInventoryReport", TransactionProvider.InventoryReportsTransactions(assembly, unit, consultant, status, item, datefrom, dateto));
            }
            catch (Exception ex)
            {
                //HandleErrorInfo info = new HandleErrorInfo(ex.InnerException, "InvoiceReport", "_GridListInvoiceReports");
                throw new Exception(ex.Message, ex.InnerException);

            }

        }
        public ActionResult _GetAssemblyUnits(string assembly)
        {
            return PartialView("_unitsPartial", TransactionProvider.LoadAssemblyUnits(assembly));
        }
        public ActionResult _GetUnitConsultants(string unit)
        {

            return PartialView("_consultantPartial", TransactionProvider.LoadUnitConsultants(unit));
        }

    }
}
