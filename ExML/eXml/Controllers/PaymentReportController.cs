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
    public class PaymentReportController : Controller
    {
        //
        // GET: /PaymentReport/
        [ValidateInput(false)]
        public ActionResult Report()
        {
            ViewData["PayStatus"] = EnumHelper.ToList(typeof(enPaymentStatus));
            ViewData["PayType"] = EnumHelper.ToList(typeof(enPaymentType));
            ViewData["Assemblies"] = TransactionProvider.LoadAssemblies();

            return View(TransactionProvider.PaymentReportsTransactions(null,null,null,null,null,null,null,null,null));
        }
        [ValidateInput(false)]
        public ActionResult _GridListPaymentsReport(string assembly, string unit, string consultant, string status,
            string type, DateTime? datefrom, DateTime? dateto, DateTime? datePfrom, DateTime? datePto)
            {
                try
                {
                    ViewData["PayStatus"] = EnumHelper.ToList(typeof(enPaymentStatus));
                    ViewData["PayType"] = EnumHelper.ToList(typeof(enPaymentType));
                    return PartialView("_GridListPaymentsReport", TransactionProvider.PaymentReportsTransactions(assembly, unit,
                        consultant, status, type , datefrom, dateto,datePfrom,datePto));
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
