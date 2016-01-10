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
    public class InvoicesController : Controller
    {
        [ValidateInput(false)]
        public ActionResult Invoice()
        {
            ViewData["PayType"] = EnumHelper.ToList(typeof(enPaymentType));
            ViewData["Assemblies"] =TransactionProvider.LoadAssemblies();
            return View(TransactionProvider.InvoiceEditableTransactions(null,null,null,null));
        }
        [ValidateInput(false)]
        public ActionResult _GridListInvoices(string assembly, string unit, DateTime? datefrom ,DateTime? dateto)
        {
            ViewData["PayType"] = EnumHelper.ToList(typeof(enPaymentType));
            return PartialView("_GridListInvoices", TransactionProvider.InvoiceEditableTransactions(assembly, unit, datefrom, dateto));
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PaymentBulk(string selectedIDsHF)
        {
            char[] array = new Char[1];
            array[0] = Char.Parse(",");
           
            string[] orderIds = selectedIDsHF.Split(array);
            int response = 0;           
            DateTime invDate = DateTime.Now.AddMonths(-1);
            DateTime invDateTo = DateTime.Now;
            foreach (var orderId in orderIds)
            {
                using (var dbContext = new eXmlContext())
                {
                    int id = int.Parse(orderId);
                    IEnumerable<PostedTransaction> pTrans = dbContext.Set<PostedTransaction>()
                                                            .Where(x => x.OrderId == id)
                                                            .ToList();
                    if (pTrans != null)
                    {
                     
                        decimal totalAmtPayable;
                        foreach (var trans in pTrans)
                        {
                            totalAmtPayable = (trans.ConsultantPrice * trans.OrderQty) - trans.PaymentAmount;
                            trans.PaymentAmount = totalAmtPayable + trans.PaymentAmount;
                            trans.PaymentDate = DateTime.Now;
                            trans.PaymentType = enPaymentType.Cash;
                            trans.PayStatus = enPaymentStatus.Received;

                            dbContext.Entry(trans).State = System.Data.Entity.EntityState.Modified;
                        }
                        dbContext.SaveChanges();
                        response = 1;
                    }
                }
            }
            ViewData["PayType"] = EnumHelper.ToList(typeof(enPaymentType));
            ViewData["Assemblies"] = TransactionProvider.LoadAssemblies();
            //return PartialView("_GridListInvoices", TransactionProvider.InvoiceEditableTransactions(null, null, null, null));
            return Json(response);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PaymentUpdate(ListInvoiceTransModel trans)
        {
            string assembly = "";
            string  unit = "";
            DateTime invDate = DateTime.Now;
            if (trans.Id > 0 && trans.PaymentType != null && trans.PaymentDate != null )
            {
                try
                {
                    int Id = trans.Id;
                    using (var db = new eXmlContext())
                    {
                        IEnumerable<PostedTransaction> pTrans = db.Set<PostedTransaction>()
                                    .Where(x => x.OrderId == Id)
                                   .ToList();
                        if (pTrans != null)
                        {
                            assembly = pTrans.First().AssemblyName;
                            unit = pTrans.First().UnitName;
                            invDate = pTrans.First().PostDate;
                            //invDate = pTrans.First().InvoiceDate;
                            //get total net amount payable as per order id
                            decimal netAmtPayable = 0;
                            decimal amtPaid = 0;
                            foreach (var tr in pTrans)
                            {
                                netAmtPayable = netAmtPayable + tr.NetAmount + tr.VatAmount;
                                amtPaid = amtPaid + tr.PaymentAmount;
                            }
                            netAmtPayable = netAmtPayable - amtPaid;
                            if (trans.PaymentAmount <= netAmtPayable)
                            {
                                decimal balAfterDeduct = trans.PaymentAmount;
                                decimal totalAmtPayable;
                                foreach (var transaction in pTrans)
                                {
                                  
                                    totalAmtPayable = (transaction.ConsultantPrice * transaction.OrderQty) - transaction.PaymentAmount;

                                    transaction.PaymentDate = trans.PaymentDate;
                                    transaction.PaymentType = trans.PaymentType;
                                    transaction.BankName = trans.BankName;
                                    transaction.ChequeNo = trans.ChequeNo;

                                    if (balAfterDeduct >= totalAmtPayable && totalAmtPayable > 0)
                                    {
                                        transaction.PaymentAmount = transaction.PaymentAmount + totalAmtPayable;
                                        transaction.PayStatus = enPaymentStatus.Received;
                                        balAfterDeduct = balAfterDeduct - totalAmtPayable;

                                        
                                    }
                                    else if (balAfterDeduct > 0 && balAfterDeduct < totalAmtPayable)
                                    {
                                        transaction.PaymentAmount = transaction.PaymentAmount + balAfterDeduct;
                                        transaction.PayStatus = enPaymentStatus.Partial;

                                        balAfterDeduct = 0 ;
                                    }
                                    else
                                    {
                                        transaction.PaymentAmount = transaction.PaymentAmount;
                                        transaction.PayStatus = enPaymentStatus.Pending;
                                    }
                                    db.Entry(transaction).State = System.Data.Entity.EntityState.Modified;
                                }
                                db.SaveChanges();
                            }
                            else
                            {
                                ViewData["EditError"] = "Payment amount is greater than net amount payable!";
                            }

                        }
                        else
                        {
                            ViewData["EditError"] = "Posted transaction object is null. Unable to obtain transaction from Db";
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewData["EditError"] = ex.Message;
                }
            }
            else
            {
                ViewData["PostedTrans"] = trans;
                ViewData["EditError"] = "Payment type and payment date cannot contain null values! Enter values and update!";
            }
           
            return PartialView("_GridListInvoices", TransactionProvider.InvoiceEditableTransactions(assembly, unit, invDate, invDate));

        }
        public ActionResult _GetAssemblyUnits(string assembly)
        {
            return PartialView("_unitsPartial", TransactionProvider.LoadAssemblyUnits(assembly));
        }
        
    }
}
