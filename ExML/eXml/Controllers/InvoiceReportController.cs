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
    public class InvoiceReportController : Controller
    {
        //
        // GET: /InvoiceReport/
        [ValidateInput(false)]
        public ActionResult InvReport()
        {
            ViewData["PayStatus"] = EnumHelper.ToList(typeof(enPaymentStatus));
            ViewData["Assemblies"] = TransactionProvider.LoadAssemblies();
            return View(TransactionProvider.InvoiceReportsTransactions(null,null,null,null,null,null));
        }
        [ValidateInput(false)]
        public ActionResult _GridListInvoicesReport(string assembly, string unit,string consultant, string status, DateTime? datefrom, DateTime? dateto)
        {
            try
            {
                ViewData["PayStatus"] = EnumHelper.ToList(typeof(enPaymentStatus));
                return PartialView("_GridListInvoicesReport", TransactionProvider.InvoiceReportsTransactions(assembly, unit, consultant,status, datefrom, dateto));
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
