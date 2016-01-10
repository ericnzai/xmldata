using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Excel;
using System.Xml.Serialization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Xml;
using System.IO;
using eXml.Models;
using eXml.Entities;
using System.Data.Entity;
using System.Configuration;
using eXml.Abstractions;

namespace eXml.Services
{
    public static class XcelServiceHelper
    {
        public static decimal TruncateDecimal(decimal value, int precision)
        {
            decimal step = (decimal)Math.Pow(10, precision);
            int tmp = (int)Math.Truncate(step * value);
            return tmp / step;
        }
        public static string GetCellValue(WorkbookPart wbPart, Sheet theSheet, string address)
        {
            string value = null;
            if (theSheet != null)
            {
                WorksheetPart wsPart =
                   (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                Cell theCell = wsPart.Worksheet.Descendants<Cell>().
                 Where(c => c.CellReference == address).FirstOrDefault();

                if (theCell != null)
                {
                    value = theCell.InnerText;

                    // If the cell represents an integer number, you are done. 
                    // For dates, this code returns the serialized value that 
                    // represents the date. The code handles strings and 
                    // Booleans individually. For shared strings, the code 
                    // looks up the corresponding value in the shared string 
                    // table. For Booleans, the code converts the value into 
                    // the words TRUE or FALSE.
                    if (theCell.DataType != null)
                    {
                        switch (theCell.DataType.Value)
                        {
                            case CellValues.SharedString:

                                // For shared strings, look up the value in the
                                // shared strings table.
                                var stringTable =
                                    wbPart.GetPartsOfType<SharedStringTablePart>()
                                    .FirstOrDefault();

                                // If the shared string table is missing, something 
                                // is wrong. Return the index that is in
                                // the cell. Otherwise, look up the correct text in 
                                // the table.
                                if (stringTable != null)
                                {
                                    value =
                                        stringTable.SharedStringTable
                                        .ElementAt(int.Parse(value)).InnerText;
                                }
                                break;

                            case CellValues.Boolean:
                                switch (value)
                                {
                                    case "0":
                                        value = "FALSE";
                                        break;
                                    default:
                                        value = "TRUE";
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
            return value;
        }
    }
    public class ConvertInvoiceVoucherToXmlService : IConvertToXmlService
    {
        IUnitOfWork unitOfWork= null;
        //public  void ProcessExcelSheet(UploadFileModel model, string fileName, string savePath)
        //{
        //    ProcessExcelSheet(model, fileName, savePath, true);
        //    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
        //    {
        //        WorkbookPart workBk = doc.WorkbookPart;
        //        WorksheetPart workSht = workBk.WorksheetParts.First();
        //        SheetData shtData = workSht.Worksheet.Elements<SheetData>().First();

        //        Sheet theSheet = workBk.Workbook.Descendants<Sheet>().
        //          Where(s => s.SheetId == 1).FirstOrDefault();

        //        var strData = "<ENVELOPE><HEADER><TALLYREQUEST>Import Data</TALLYREQUEST></HEADER><BODY><IMPORTDATA>" +
        //                    "<REQUESTDESC> <REPORTNAME>All Masters</REPORTNAME><STATICVARIABLES><SVCURRENTCOMPANY>" + model.Company +
        //                    "</SVCURRENTCOMPANY></STATICVARIABLES></REQUESTDESC><REQUESTDATA>";

        //        int rows = 0;
        //        string cellA;   string cellD; 
        //        string cellF; string cellH;  string cellJ; string cellQ;
        //        string cellU; string cellW; string cellY; string cellAB; string cellAD; string cellAssembly;


        //        var year = ""; var week = ""; var assembly = "";  var unitname = "";
        //        var consutantCode = ""; var consultantName = "";  
        //         decimal checkedAmt; var item = ""; var itemCode = ""; var itemName = ""; int orderId;
        //        decimal MRP; decimal ordQty; decimal consPrice; decimal amount; var status = "";

        //            List<UnitName> units = new List<UnitName>();
        //            List<UnitConsultant> unitConsultants = new List<UnitConsultant>();
        //            List<ConsultantOrder> consOrders = new List<ConsultantOrder>();
        //            List<StockItem> stockItems = new List<StockItem>();

        //            cellAssembly = "L8";
        //            assembly = XcelServiceHelper.GetCellValue(workBk, theSheet, cellAssembly);
        //            week = XcelServiceHelper.GetCellValue(workBk, theSheet, "U9");
        //            year = XcelServiceHelper.GetCellValue(workBk, theSheet, "U8");
        //            UnitName u;
        //            ConsultantOrder co;
        //            UnitConsultant uc;
        //            int j = 0;

        //            int rowCount = shtData.Elements<Row>().Count();
        //            rowCount = rowCount + 4;
        //            //foreach (Row r in shtData.Elements<Row>())
        //            for (rows = 1; rows <= rowCount; rows++)
        //            {
        //                //rows += 1;
        //                cellQ = "Q" + rows;

        //                if (rows >= 12)
        //                {
        //                    //Cell theCell = r.Descendants<Cell>().Where(x => x.CellReference == cellQ).FirstOrDefault();
        //                    string theCell = XcelServiceHelper.GetCellValue(workBk, theSheet, cellQ);
        //                    if (!string.IsNullOrEmpty(theCell))
        //                    {
        //                        //
        //                        string ratePercVal = "";
        //                        cellQ = "Q" + rows;
        //                        cellJ = "J" + rows;
        //                        itemCode = XcelServiceHelper.GetCellValue(workBk, theSheet, cellJ);
        //                        itemName = XcelServiceHelper.GetCellValue(workBk, theSheet, cellQ);
        //                        itemName = itemName.Replace("(", " ");
        //                        itemName = itemName.Replace(")", " ");
        //                        itemName = itemName.Replace("&", "_");
        //                        itemName = itemName.Replace("'", " ");
        //                        item = itemName + " - " + itemCode;

        //                        StockItem s = stockItems.FirstOrDefault(x => x.ItemCode == itemCode.Trim());
        //                        if (s == null)
        //                        {
        //                            if (itemName.StartsWith("PPP")) ratePercVal = " 5";
        //                            else ratePercVal = " 12.5";

        //                            strData = strData + @"<TALLYMESSAGE xmlns:UDF=""TallyUDF"">" +
        //                               @"<STOCKITEM NAME="""
        //                               + item +
        //                               @""" RESERVEDNAME="""">" +
        //                               @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
        //                               "<PARENT>Tupperware Products</PARENT><CATEGORY/><TAXCLASSIFICATIONNAME/><COSTINGMETHOD>Avg. Cost</COSTINGMETHOD>" +
        //                               "<VALUATIONMETHOD>Avg. Price</VALUATIONMETHOD><BASEUNITS>NO</BASEUNITS><ADDITIONALUNITS/><EXCISEITEMCLASSIFICATION/>" +
        //                               "<ISCOSTCENTRESON>No</ISCOSTCENTRESON><ISBATCHWISEON>No</ISBATCHWISEON><ISPERISHABLEON>No</ISPERISHABLEON><ISENTRYTAXAPPLICABLE>No</ISENTRYTAXAPPLICABLE>" +
        //                               "<ISCOSTTRACKINGON>No</ISCOSTTRACKINGON><IGNOREPHYSICALDIFFERENCE>No</IGNOREPHYSICALDIFFERENCE><IGNORENEGATIVESTOCK>No</IGNORENEGATIVESTOCK>" +
        //                               "<TREATSALESASMANUFACTURED>No</TREATSALESASMANUFACTURED><TREATPURCHASESASCONSUMED>No</TREATPURCHASESASCONSUMED><TREATREJECTSASSCRAP>No</TREATREJECTSASSCRAP>" +
        //                               "<HASMFGDATE>No</HASMFGDATE><ALLOWUSEOFEXPIREDITEMS>No</ALLOWUSEOFEXPIREDITEMS><IGNOREBATCHES>No</IGNOREBATCHES><IGNOREGODOWNS>No</IGNOREGODOWNS>" +
        //                               "<CALCONMRP>No</CALCONMRP><EXCLUDEJRNLFORVALUATION>No</EXCLUDEJRNLFORVALUATION><ISMRPINCLOFTAX>No</ISMRPINCLOFTAX><ISADDLTAXEXEMPT>No</ISADDLTAXEXEMPT>" +
        //                               "<ISSUPPLEMENTRYDUTYON>No</ISSUPPLEMENTRYDUTYON><REORDERASHIGHER>No</REORDERASHIGHER><MINORDERASHIGHER>No</MINORDERASHIGHER><DENOMINATOR> 1</DENOMINATOR>" +
        //                               @"<RATEOFVAT>" + ratePercVal + "</RATEOFVAT><LANGUAGENAME.LIST>" +
        //                               @"<NAME.LIST TYPE=""String""><NAME>" + item + "</NAME></NAME.LIST><LANGUAGEID> 1033</LANGUAGEID>" +
        //                               "</LANGUAGENAME.LIST><SCHVIDETAILS.LIST>      </SCHVIDETAILS.LIST><OLDAUDITENTRIES.LIST>      </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>      </ACCOUNTAUDITENTRIES.LIST>" +
        //                               "<AUDITENTRIES.LIST>      </AUDITENTRIES.LIST><COMPONENTLIST.LIST>      </COMPONENTLIST.LIST><ADDITIONALLEDGERS.LIST>      </ADDITIONALLEDGERS.LIST>" +
        //                               "<SALESLIST.LIST>      </SALESLIST.LIST><PURCHASELIST.LIST>      </PURCHASELIST.LIST><FULLPRICELIST.LIST>      </FULLPRICELIST.LIST>" +
        //                               "<BATCHALLOCATIONS.LIST>      </BATCHALLOCATIONS.LIST><TRADEREXCISEDUTIES.LIST>      </TRADEREXCISEDUTIES.LIST><STANDARDCOSTLIST.LIST>      </STANDARDCOSTLIST.LIST>" +
        //                               "<STANDARDPRICELIST.LIST>      </STANDARDPRICELIST.LIST><EXCISEITEMGODOWN.LIST>      </EXCISEITEMGODOWN.LIST><MULTICOMPONENTLIST.LIST>      </MULTICOMPONENTLIST.LIST>" +
        //                               "<PRICELEVELLIST.LIST>      </PRICELEVELLIST.LIST></STOCKITEM></TALLYMESSAGE>";
        //                            s = new StockItem
        //                            {
        //                                ItemCode = itemCode.Trim().ToString(),
        //                                ItemName = itemName.Trim().ToString()
        //                            };
        //                            stockItems.Add(s);
        //                        }
        //                        cellA = "A" + rows;
        //                        unitname = XcelServiceHelper.GetCellValue(workBk, theSheet, cellA);

        //                        u = units.FirstOrDefault(x => x.Unit == unitname.Trim());
        //                        if (u == null)
        //                        {
        //                            u = new UnitName
        //                            {
        //                                Unit = unitname,
        //                                IsGroupCreated = false
        //                            };
        //                            units.Add(u);
        //                            units.OrderBy(x => x.Unit);
        //                        }
        //                        cellD = "D" + rows;
        //                        consutantCode = XcelServiceHelper.GetCellValue(workBk, theSheet, cellD);
        //                        cellF = "F" + rows;
        //                        consultantName = XcelServiceHelper.GetCellValue(workBk, theSheet, cellF);

        //                        uc = u.UnitConsultants.FirstOrDefault(x => x.Consultant == consultantName.Trim());
        //                        if (uc == null)
        //                        {
        //                            uc = new UnitConsultant
        //                            {
        //                                Consultant = consultantName.Trim(),
        //                                ConsultantId = consutantCode.Trim(),
        //                                UnitName = u
        //                            };
        //                            if (u != null) u.UnitConsultants.Add(uc);
        //                        }
        //                        cellH = "H" + rows;
        //                        orderId = int.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellH));
        //                        cellU = "U" + rows;
        //                        ordQty = Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellU));

        //                        cellW = "W" + rows;
        //                        MRP = Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellW));

        //                        cellY = "Y" + rows;
        //                        consPrice = Math.Round(Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellY)));

        //                        cellAB = "AB" + rows;
        //                        amount = Math.Round(Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellAB)));

        //                        cellAD = "AD" + rows;
        //                        status = XcelServiceHelper.GetCellValue(workBk, theSheet, cellAD);

        //                        co = uc.ConsultantOrders.FirstOrDefault(x => x.ItemCode == itemCode.Trim());


        //                        if (co == null)
        //                        {
        //                            co = new ConsultantOrder
        //                            {
        //                                OrderId = orderId,
        //                                Amount = amount,
        //                                ItemCode = itemCode.Trim(),
        //                                ItemName = itemName.Trim(),
        //                                MRP = MRP,
        //                                OrdQty = ordQty,
        //                                ConsultantPrice = consPrice,
        //                                Status = status,
        //                                Consultant = uc

        //                            };

        //                            co.VoucherId = j + 1;
        //                            j++;

        //                            if (uc != null)
        //                            {
        //                                //prevId = co.OrderId;
        //                                //prevVoucherId = co.VoucherId;
        //                                uc.ConsultantOrders.Add(co);
        //                                uc.ConsultantOrders.OrderBy(x => x.OrderId).ThenBy(x => x.ItemName);
        //                            }
        //                        }

        //                    }
        //                }

        //            }

        //            foreach (var unit in units)
        //            {
        //                unitOfWork = new UnitOfWork();
        //                foreach (var uCon in unit.UnitConsultants)
        //                {
        //                    string consultant = uCon.Consultant + " - " + uCon.ConsultantId;

        //                    //newVoucherNo = newVoucherNo + 1;

        //                    strData = strData + @"<TALLYMESSAGE xmlns:UDF=""TallyUDF"">" +
        //                    @"<LEDGER NAME="""
        //                    + consultant +
        //                    @""" RESERVEDNAME="""">" +
        //                    @"<ADDRESS.LIST TYPE=""String""><ADDRESS>" + unit.Unit + "</ADDRESS><ADDRESS>" + assembly + "</ADDRESS></ADDRESS.LIST>" +
        //                    @"<MAILINGNAME.LIST TYPE=""String""><MAILINGNAME>" + consultant + "</MAILINGNAME></MAILINGNAME.LIST>" +
        //                    @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
        //                    "<ALTEREDON>" + model.Date + "</ALTEREDON><STATENAME>Maharashtra</STATENAME><PARENT>" + unit.Unit + "</PARENT><TAXCLASSIFICATIONNAME/>" +
        //                    "<TAXTYPE>Others</TAXTYPE><BUSINESSTYPE/><BASICTYPEOFDUTY>Excise Surcharge</BASICTYPEOFDUTY><GSTTYPE/><APPROPRIATEFOR/>" +
        //                    "<SERVICECATEGORY/><EXCISELEDGERCLASSIFICATION/><EXCISEDUTYTYPE/><EXCISENATUREOFPURCHASE/><LEDGERFBTCATEGORY/>" +
        //                    "<ISBILLWISEON>No</ISBILLWISEON><ISCOSTCENTRESON>Yes</ISCOSTCENTRESON><ISINTERESTON>No</ISINTERESTON><ALLOWINMOBILE>No</ALLOWINMOBILE>" +
        //                    "<ISCOSTTRACKINGON>No</ISCOSTTRACKINGON><ISCONDENSED>No</ISCONDENSED><AFFECTSSTOCK>No</AFFECTSSTOCK><FORPAYROLL>No</FORPAYROLL>" +
        //                    "<ISABCENABLED>No</ISABCENABLED><INTERESTONBILLWISE>No</INTERESTONBILLWISE><OVERRIDEINTEREST>No</OVERRIDEINTEREST><OVERRIDEADVINTEREST>No</OVERRIDEADVINTEREST>" +
        //                    "<USEFORVAT>No</USEFORVAT><IGNORETDSEXEMPT>No</IGNORETDSEXEMPT><ISTCSAPPLICABLE>No</ISTCSAPPLICABLE><ISTDSAPPLICABLE>No</ISTDSAPPLICABLE><ISFBTAPPLICABLE>No</ISFBTAPPLICABLE>" +
        //                    "<ISGSTAPPLICABLE>No</ISGSTAPPLICABLE><ISEXCISEAPPLICABLE>No</ISEXCISEAPPLICABLE><ISTDSEXPENSE>No</ISTDSEXPENSE><ISEDLIAPPLICABLE>No</ISEDLIAPPLICABLE>" +
        //                    "<ISRELATEDPARTY>No</ISRELATEDPARTY><USEFORESIELIGIBILITY>No</USEFORESIELIGIBILITY><SHOWINPAYSLIP>No</SHOWINPAYSLIP><USEFORGRATUITY>No</USEFORGRATUITY>" +
        //                    "<ISTDSPROJECTED>No</ISTDSPROJECTED><FORSERVICETAX>No</FORSERVICETAX><ISINPUTCREDIT>No</ISINPUTCREDIT><ISEXEMPTED>No</ISEXEMPTED><ISABATEMENTAPPLICABLE>No</ISABATEMENTAPPLICABLE>" +
        //                    "<ISSTXPARTY>No</ISSTXPARTY><ISSTXNONREALIZEDTYPE>No</ISSTXNONREALIZEDTYPE><TDSDEDUCTEEISSPECIALRATE>No</TDSDEDUCTEEISSPECIALRATE><AUDITED>No</AUDITED><SORTPOSITION> 1000</SORTPOSITION>" +
        //                    @"<RATEOFTAXCALCULATION> 12.50</RATEOFTAXCALCULATION><LANGUAGENAME.LIST><NAME.LIST TYPE=""String""><NAME>" + consultant + "</NAME></NAME.LIST><LANGUAGEID> 1033</LANGUAGEID>" +
        //                    "</LANGUAGENAME.LIST><XBRLDETAIL.LIST>      </XBRLDETAIL.LIST><AUDITDETAILS.LIST>      </AUDITDETAILS.LIST><SCHVIDETAILS.LIST>      </SCHVIDETAILS.LIST><SLABPERIOD.LIST>      </SLABPERIOD.LIST>" +
        //                    "<GRATUITYPERIOD.LIST>      </GRATUITYPERIOD.LIST><ADDITIONALCOMPUTATIONS.LIST>      </ADDITIONALCOMPUTATIONS.LIST><BANKALLOCATIONS.LIST>      </BANKALLOCATIONS.LIST><PAYMENTDETAILS.LIST>      </PAYMENTDETAILS.LIST>" +
        //                    "<BANKEXPORTFORMATS.LIST>      </BANKEXPORTFORMATS.LIST><BILLALLOCATIONS.LIST>      </BILLALLOCATIONS.LIST><INTERESTCOLLECTION.LIST>      </INTERESTCOLLECTION.LIST><LEDGERCLOSINGVALUES.LIST>      </LEDGERCLOSINGVALUES.LIST>" +
        //                    "<LEDGERAUDITCLASS.LIST>      </LEDGERAUDITCLASS.LIST><OLDAUDITENTRIES.LIST>      </OLDAUDITENTRIES.LIST><TDSEXEMPTIONRULES.LIST>      </TDSEXEMPTIONRULES.LIST><DEDUCTINSAMEVCHRULES.LIST>      </DEDUCTINSAMEVCHRULES.LIST>" +
        //                    "<LOWERDEDUCTION.LIST>      </LOWERDEDUCTION.LIST><STXABATEMENTDETAILS.LIST>      </STXABATEMENTDETAILS.LIST><LEDMULTIADDRESSLIST.LIST>      </LEDMULTIADDRESSLIST.LIST><STXTAXDETAILS.LIST>      </STXTAXDETAILS.LIST>" +
        //                    "<CHEQUERANGE.LIST>      </CHEQUERANGE.LIST><DEFAULTVCHCHEQUEDETAILS.LIST>      </DEFAULTVCHCHEQUEDETAILS.LIST><ACCOUNTAUDITENTRIES.LIST>      </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>      </AUDITENTRIES.LIST>" +
        //                    "<BRSIMPORTEDINFO.LIST>      </BRSIMPORTEDINFO.LIST><AUTOBRSCONFIGS.LIST>      </AUTOBRSCONFIGS.LIST><BANKURENTRIES.LIST>      </BANKURENTRIES.LIST><DEFAULTCHEQUEDETAILS.LIST>      </DEFAULTCHEQUEDETAILS.LIST>" +
        //                    "<DEFAULTOPENINGCHEQUEDETAILS.LIST>      </DEFAULTOPENINGCHEQUEDETAILS.LIST></LEDGER></TALLYMESSAGE>";


        //                    if (unit != null && unit.IsGroupCreated == false)
        //                    {
        //                        strData = strData + @"<TALLYMESSAGE xmlns:UDF=""TallyUDF"">" +
        //                        @"<GROUP NAME="""
        //                        + unit.Unit +
        //                        @""" ACTION = ""CREATE"">" +
        //                        "<NAME.LIST><NAME>" + unit.Unit + "</NAME></NAME.LIST><PARENT>Sundry Debtors</PARENT><ISSUBLEDGER>No</ISSUBLEDGER><ISBILLWISEON>No</ISBILLWISEON>" +
        //                        "<ISCOSTCENTRESON>No</ISCOSTCENTRESON></GROUP></TALLYMESSAGE>";
        //                        unit.IsGroupCreated = true;
        //                    }
        //                    decimal totalConsultantAmt = uCon.ConsultantOrders.Sum(x => x.ConsultantPrice * x.OrdQty);
        //                    decimal consultantAmount = uCon.ConsultantOrders.Where(x => !x.ItemName.StartsWith("PPP")).Sum(x => x.ConsultantPrice * x.OrdQty);
        //                    decimal netAmount = ((consultantAmount / Decimal.Parse("112.5") * Decimal.Parse("100")));
        //                    //decimal tax = ((netAmount * Decimal.Parse("12.5")) / Decimal.Parse("100"));


        //                    var ordId = uCon.ConsultantOrders.Select(x => x.OrderId).First();

        //                    var voucherId = uCon.ConsultantOrders.Where(x => x.OrderId == ordId).Select(x => x.VoucherId).First();

        //                    netAmount = Math.Round(netAmount, 2);
        //                    decimal tax = consultantAmount - netAmount;

        //                    //tax = TruncateDecimal(tax, 2);
        //                    decimal chkAmt = (consultantAmount - (netAmount + tax));
        //                    //if (chkAmt <= 1 || chkAmt >= -1)
        //                    //{
        //                    //    tax = tax + chkAmt;
        //                    //}
        //                    string VoucherNo = week + "-" + ordId + "/" + voucherId;

        //                    strData = strData + @"<TALLYMESSAGE xmlns:UDF=""TallyUDF"">" +
        //                    @"<VOUCHER VCHTYPE=""Sales"" ACTION=""Create"" OBJVIEW=""Invoice Voucher View"">" +
        //                    @"<ADDRESS.LIST TYPE=""String""><ADDRESS>" + unit.Unit + "</ADDRESS><ADDRESS>" + assembly + "</ADDRESS>" +
        //                    @"</ADDRESS.LIST><BASICBUYERADDRESS.LIST TYPE=""String""><BASICBUYERADDRESS>" + unit.Unit + "</BASICBUYERADDRESS>" +
        //                    "<BASICBUYERADDRESS>" + assembly + "</BASICBUYERADDRESS></BASICBUYERADDRESS.LIST>" +
        //                    @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST><DATE>" + model.Date + "</DATE>" +
        //                    "<PARTYNAME>" + consultant + "</PARTYNAME><VOUCHERTYPENAME>Sales</VOUCHERTYPENAME><VOUCHERNUMBER>" + VoucherNo + "</VOUCHERNUMBER>" +
        //                    "<PARTYLEDGERNAME>" + consultant + "</PARTYLEDGERNAME><BASICBASEPARTYNAME>" + consultant + "</BASICBASEPARTYNAME>" +
        //                    "<CSTFORMISSUETYPE/><CSTFORMRECVTYPE/><FBTPAYMENTTYPE>Default</FBTPAYMENTTYPE><PERSISTEDVIEW>Invoice Voucher View</PERSISTEDVIEW>" +
        //                    "<BASICBUYERNAME>" + consultant + "</BASICBUYERNAME><BASICFINALDESTINATION>" + assembly + "</BASICFINALDESTINATION>" +
        //                    "<VCHGSTCLASS/><DIFFACTUALQTY>No</DIFFACTUALQTY><AUDITED>No</AUDITED><FORJOBCOSTING>No</FORJOBCOSTING><ISOPTIONAL>No</ISOPTIONAL>" +
        //                    "<EFFECTIVEDATE>" + model.Date + "</EFFECTIVEDATE><ISFORJOBWORKIN>No</ISFORJOBWORKIN><ALLOWCONSUMPTION>No</ALLOWCONSUMPTION>" +
        //                    "<USEFORINTEREST>No</USEFORINTEREST><USEFORGAINLOSS>No</USEFORGAINLOSS><USEFORGODOWNTRANSFER>No</USEFORGODOWNTRANSFER>" +
        //                    "<USEFORCOMPOUND>No</USEFORCOMPOUND><EXCISEOPENING>No</EXCISEOPENING><USEFORFINALPRODUCTION>No</USEFORFINALPRODUCTION>" +
        //                    "<ISCANCELLED>No</ISCANCELLED><HASCASHFLOW>No</HASCASHFLOW><ISPOSTDATED>No</ISPOSTDATED><USETRACKINGNUMBER>No</USETRACKINGNUMBER>" +
        //                    "<ISINVOICE>Yes</ISINVOICE><MFGJOURNAL>No</MFGJOURNAL><HASDISCOUNTS>No</HASDISCOUNTS><ASPAYSLIP>No</ASPAYSLIP><ISCOSTCENTRE>No</ISCOSTCENTRE>" +
        //                    "<ISSTXNONREALIZEDVCH>No</ISSTXNONREALIZEDVCH><ISEXCISEMANUFACTURERON>Yes</ISEXCISEMANUFACTURERON><ISBLANKCHEQUE>No</ISBLANKCHEQUE>" +
        //                    "<ISDELETED>No</ISDELETED><ASORIGINAL>No</ASORIGINAL><VCHISFROMSYNC>No</VCHISFROMSYNC><OLDAUDITENTRIES.LIST>      </OLDAUDITENTRIES.LIST>" +
        //                    "<ACCOUNTAUDITENTRIES.LIST>      </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>      </AUDITENTRIES.LIST><INVOICEDELNOTES.LIST>      </INVOICEDELNOTES.LIST>" +
        //                    "<INVOICEORDERLIST.LIST>      </INVOICEORDERLIST.LIST><INVOICEINDENTLIST.LIST>      </INVOICEINDENTLIST.LIST><ATTENDANCEENTRIES.LIST>      </ATTENDANCEENTRIES.LIST>" +
        //                    "<ORIGINVOICEDETAILS.LIST>      </ORIGINVOICEDETAILS.LIST><INVOICEEXPORTLIST.LIST>      </INVOICEEXPORTLIST.LIST><LEDGERENTRIES.LIST>" +
        //                    @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST><LEDGERNAME>" + consultant + "</LEDGERNAME>" +
        //                    "<GSTCLASS/><ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM><REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>Yes</ISPARTYLEDGER>" +
        //                    "<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE><AMOUNT>-" + totalConsultantAmt + "</AMOUNT><CATEGORYALLOCATIONS.LIST><CATEGORY>Primary Cost Category</CATEGORY>" +
        //                    "<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE><COSTCENTREALLOCATIONS.LIST><NAME>" + assembly + "</NAME><AMOUNT>-" + totalConsultantAmt + "</AMOUNT>" +
        //                    "</COSTCENTREALLOCATIONS.LIST></CATEGORYALLOCATIONS.LIST><BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST><BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>" +
        //                    "<INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>" +
        //                    "<AUDITENTRIES.LIST>       </AUDITENTRIES.LIST><TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>" +
        //                    "<TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST><VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST><COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>" +
        //                    @"</LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
        //                    @"<BASICRATEOFINVOICETAX.LIST TYPE=""Number""><BASICRATEOFINVOICETAX> 12.50</BASICRATEOFINVOICETAX></BASICRATEOFINVOICETAX.LIST><TAXCLASSIFICATIONNAME>Output VAT @ 12.5%</TAXCLASSIFICATIONNAME>" +
        //                    "<ROUNDTYPE>Normal Rounding</ROUNDTYPE><LEDGERNAME>12.5% Vat on Sales</LEDGERNAME><GSTCLASS/><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM>" +
        //                    "<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>No</ISPARTYLEDGER><ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><AMOUNT>" + tax + "</AMOUNT>" +
        //                    "<VATASSESSABLEVALUE>" + netAmount + "</VATASSESSABLEVALUE><BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST><BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>" +
        //                    "<INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>" +
        //                    "<AUDITENTRIES.LIST>       </AUDITENTRIES.LIST><TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST><CATEGORY>Output VAT @ 12.5%</CATEGORY>" +
        //                    "<TAXTYPE>VAT</TAXTYPE><TAXNAME>" + VoucherNo + "</TAXNAME><PARTYLEDGER>" + consultant + "</PARTYLEDGER><REFTYPE>New Ref</REFTYPE><ISOPTIONAL>No</ISOPTIONAL>" +
        //                    "<ISPANVALID>No</ISPANVALID><ZERORATED>No</ZERORATED><EXEMPTED>No</EXEMPTED><ISSPECIALRATE>No</ISSPECIALRATE><ISDEDUCTNOW>No</ISDEDUCTNOW><ISPANNOTAVAILABLE>No</ISPANNOTAVAILABLE>" +
        //                    "<ISSUPPLEMENTARY>No</ISSUPPLEMENTARY><OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>";

        //                    decimal trackNetAmtTotal = 0;
        //                    int count = uCon.ConsultantOrders.Where(x => !x.ItemName.StartsWith("PPP")).Count();
        //                    int q = 0;
        //                    foreach (var con in uCon.ConsultantOrders)
        //                    {
        //                        if (!con.ItemName.StartsWith("PPP"))
        //                        {
        //                            decimal itemTotalVal = con.ConsultantPrice * con.OrdQty;
        //                            decimal itemRate = itemTotalVal / con.OrdQty;
        //                            decimal itemNetAmt = (con.ConsultantPrice / Decimal.Parse("112.5") * Decimal.Parse("100"));
        //                            itemNetAmt = Math.Round(itemNetAmt, 2);
        //                            //decimal itemTax = (itemNetAmt * Decimal.Parse("12.5") / Decimal.Parse("100"));
        //                            decimal itemTax = con.ConsultantPrice - itemNetAmt;
        //                            itemRate = itemRate - itemTax;

        //                            decimal itemsTotalAmt = itemRate * con.OrdQty;
        //                            decimal itemTotalTax = itemTax * con.OrdQty;
        //                            decimal itemTotalNetAmt = itemNetAmt * con.OrdQty;

        //                            //----------------------------------------------------
        //                            //INCLUDED TO TRACK NET AMT TOTALS AFFECTING ROUNDING
        //                            //-----------------------------------------------------
        //                            trackNetAmtTotal += itemTotalNetAmt;


        //                            if (q == count - 1) // last item in consultant orders
        //                            {
                                        
        //                                if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
        //                                {
        //                                    checkedAmt = netAmount - trackNetAmtTotal;

        //                                    itemsTotalAmt = itemsTotalAmt + checkedAmt;
        //                                    itemTotalTax = itemTotalTax - checkedAmt;

        //                                }

        //                            }

        //                            strData = strData + "<SUBCATEGORYALLOCATION.LIST>" +
        //                            "<STOCKITEMNAME>" + con.ItemName + "-" + con.ItemCode + "</STOCKITEMNAME>" +
        //                            "<SUBCATEGORY>VAT</SUBCATEGORY>" +
        //                            "<DUTYLEDGER>12.5% Vat on Sales</DUTYLEDGER>" +
        //                            "<SUBCATZERORATED>No</SUBCATZERORATED>" +
        //                            "<SUBCATEXEMPTED>No</SUBCATEXEMPTED>" +
        //                            "<SUBCATISSPECIALRATE>No</SUBCATISSPECIALRATE>" +
        //                            "<TAXRATE> 12.50</TAXRATE>" +
        //                            "<ASSESSABLEAMOUNT>" + itemsTotalAmt + "</ASSESSABLEAMOUNT>" +
        //                            "<TAX>" + itemTotalTax + "</TAX>" +
        //                            "<BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY>" +
        //                            "</SUBCATEGORYALLOCATION.LIST>";
        //                            q++;
        //                        }

        //                    }
        //                    strData = strData + "</TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>" +
        //                    "<VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST><COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>" +
        //                    "</LEDGERENTRIES.LIST>";
        //                    q = 0;
        //                    trackNetAmtTotal = 0;
        //                    foreach (var con in uCon.ConsultantOrders)
        //                    {
                               
        //                        if (!con.ItemName.StartsWith("PPP"))
        //                        {
        //                            decimal itemTotalVal = con.ConsultantPrice * con.OrdQty;
        //                            decimal itemRate = itemTotalVal / con.OrdQty;
        //                            decimal itemNetAmt = (con.ConsultantPrice / Decimal.Parse("112.5") * Decimal.Parse("100"));
        //                            itemNetAmt = Math.Round(itemNetAmt, 2);
        //                            //decimal itemTax = (itemNetAmt * Decimal.Parse("12.5") / Decimal.Parse("100"));
        //                            decimal itemTax = con.ConsultantPrice - itemNetAmt;
        //                            itemRate = itemRate - itemTax;

        //                            decimal itemsTotalAmt = itemRate * con.OrdQty;
        //                            decimal itemTotalTax = itemTax * con.OrdQty;
        //                            decimal itemTotalNetAmt = itemNetAmt * con.OrdQty;

        //                            //----------------------------------------------------
        //                            //INCLUDED TO TRACK NET AMT TOTALS AFFECTING ROUNDING
        //                            //-----------------------------------------------------
        //                            trackNetAmtTotal += itemTotalNetAmt;


        //                            if (q == count - 1) // last item in consultant orders
        //                            {
                                       
        //                                if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
        //                                {
        //                                    checkedAmt = netAmount - trackNetAmtTotal;

        //                                    itemsTotalAmt = itemsTotalAmt + checkedAmt;
        //                                    itemTotalTax = itemTotalTax - checkedAmt;
        //                                    itemRate = itemsTotalAmt / con.OrdQty;
        //                                }

        //                            }
        //                            //-----------------------------------------------------------------------

        //                            strData = strData + "<ALLINVENTORYENTRIES.LIST><STOCKITEMNAME>" + con.ItemName + "-" + con.ItemCode + "</STOCKITEMNAME>" +
        //                            "<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><ISAUTONEGATE>No</ISAUTONEGATE>" +
        //                            "<ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE><ISTRACKCOMPONENT>No</ISTRACKCOMPONENT><ISTRACKPRODUCTION>No</ISTRACKPRODUCTION>" +
        //                            "<ISPRIMARYITEM>No</ISPRIMARYITEM><ISSCRAP>No</ISSCRAP><RATE>" + itemRate + "/NO</RATE><AMOUNT>" + itemsTotalAmt + "</AMOUNT>" + //<AMOUNT>" + itemNetAmt + "</AMOUNT>
        //                            "<ACTUALQTY> " + con.OrdQty + " NO</ACTUALQTY><BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY><BATCHALLOCATIONS.LIST>" +
        //                            "<GODOWNNAME>Main Location</GODOWNNAME><BATCHNAME>Primary Batch</BATCHNAME><INDENTNO/><ORDERNO/><TRACKINGNUMBER/>" +
        //                            "<DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED><AMOUNT>" + itemsTotalAmt + "</AMOUNT><ACTUALQTY> " + con.OrdQty + " NO</ACTUALQTY>" +
        //                            "<BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY><ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST><VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST>" +
        //                            @"</BATCHALLOCATIONS.LIST><ACCOUNTINGALLOCATIONS.LIST><OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
        //                            "</OLDAUDITENTRYIDS.LIST><TAXCLASSIFICATIONNAME>Output VAT @ 12.5%</TAXCLASSIFICATIONNAME><LEDGERNAME>Sales @12.5%</LEDGERNAME><GSTCLASS/>" +
        //                            "<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM><REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>No</ISPARTYLEDGER>" +
        //                            "<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><AMOUNT>" + itemsTotalAmt + "</AMOUNT><BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST>" +
        //                            "<BILLALLOCATIONS.LIST>        </BILLALLOCATIONS.LIST><INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>" +
        //                            "<ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>        </AUDITENTRIES.LIST><TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST>" +
        //                            "<TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST><VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST>" +
        //                            "<COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST></ACCOUNTINGALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>" +
        //                            "<EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST><EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST></ALLINVENTORYENTRIES.LIST>";

        //                            q++;
        //                            itemNetAmt = XcelServiceHelper.TruncateDecimal(itemNetAmt, 2);
        //                            decimal chkItemTax = con.ConsultantPrice - (itemNetAmt + itemTax);
        //                            if (chkItemTax <= 1 || chkItemTax >= -1)
        //                            {
        //                                itemTax = itemTax + chkItemTax;
        //                            }
        //                            PostedTransaction trans;
        //                            //using (var db = new eXmlContext())
        //                            //{
        //                                trans =   unitOfWork.PostedTransactions.All()
        //                                         .Where(x => x.ItemCode == con.ItemCode)
        //                                         .Where(x => x.OrderId == con.OrderId)
        //                                         .Where(x => x.ConsultantCode == con.Consultant.ConsultantId)
        //                                         .FirstOrDefault();

        //                                if (trans == null)
        //                                {
        //                                    trans = new PostedTransaction
        //                                    {
        //                                        Company = model.Company,
        //                                        UnitName = unit.Unit,
        //                                        AssemblyName = assembly,
        //                                        ConsultantName = uCon.Consultant,
        //                                        ConsultantCode = uCon.ConsultantId,
        //                                        PostDate = DateTime.Now,
        //                                        Year = year,
        //                                        Week = week,
        //                                        InvoiceNo = VoucherNo,
        //                                        GrossAmount = con.Amount,
        //                                        ConsultantPrice = con.ConsultantPrice,
        //                                        NetAmount = itemNetAmt,
        //                                        VatAmount = itemTax,
        //                                        ItemCode = con.ItemCode,
        //                                        ItemName = con.ItemName,
        //                                        OrderId = con.OrderId,
        //                                        OrderQty = con.OrdQty,
        //                                        Status = con.Status,
        //                                        PayStatus = enPaymentStatus.Pending,
        //                                        InventoryStatus = enInventoryStatus.Pending,
        //                                        PaymentAmount = 0,
        //                                        PostType = enPostType.Invoice_12_5_WithAddress

        //                                    };
        //                                    unitOfWork.PostedTransactions.Insert(trans);
        //                                }
        //                                else
        //                                {
        //                                    trans.ItemCode = con.ItemCode;
        //                                    trans.GrossAmount = con.Amount;
        //                                    trans.ConsultantPrice = con.ConsultantPrice;
        //                                    trans.NetAmount = itemNetAmt;
        //                                    trans.VatAmount = itemTax + (con.ConsultantPrice - (itemNetAmt + itemTax));
        //                                    trans.ItemCode = con.ItemCode;
        //                                    trans.ItemName = con.ItemName;
        //                                    trans.OrderId = con.OrderId;
        //                                    trans.OrderQty = con.OrdQty;
        //                                    trans.Status = con.Status;

        //                                    unitOfWork.PostedTransactions.Update(trans);

        //                                }
                                        
        //                            //}
        //                        }

        //                    }
        //                    #region VAT_5%
        //                    if (uCon.ConsultantOrders.Count(x => x.ItemName.StartsWith("PPP")) > 0) // 5 % VAT Items
        //                    {
        //                        consultantAmount = uCon.ConsultantOrders.Where(x => x.ItemName.StartsWith("PPP")).Sum(x => x.ConsultantPrice * x.OrdQty);
        //                        netAmount = ((consultantAmount / Decimal.Parse("105") * Decimal.Parse("100")));
        //                        //tax = ((netAmount * Decimal.Parse("5")) / Decimal.Parse("100"));
        //                        netAmount = Math.Round(netAmount, 2);
        //                        tax = consultantAmount - netAmount;

        //                        //tax = TruncateDecimal(tax, 2);
        //                        chkAmt = (consultantAmount - (netAmount + tax));
        //                        //if (chkAmt <= 1 || chkAmt >= -1)
        //                        //{
        //                        //    tax = tax + chkAmt;
        //                        //}

        //                        strData = strData + @"<LEDGERENTRIES.LIST><OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
        //                                @"<BASICRATEOFINVOICETAX.LIST TYPE=""Number""><BASICRATEOFINVOICETAX> 5</BASICRATEOFINVOICETAX></BASICRATEOFINVOICETAX.LIST><TAXCLASSIFICATIONNAME>Output VAT @ 5%</TAXCLASSIFICATIONNAME>" +
        //                                "<ROUNDTYPE>Normal Rounding</ROUNDTYPE><LEDGERNAME>Output Vat @5%</LEDGERNAME><GSTCLASS/><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM>" +
        //                                "<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>No</ISPARTYLEDGER><ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><AMOUNT>" + tax + "</AMOUNT>" +
        //                                "<VATASSESSABLEVALUE>" + netAmount + "</VATASSESSABLEVALUE><BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST><BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>" +
        //                                "<INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>" +
        //                                "<AUDITENTRIES.LIST>       </AUDITENTRIES.LIST><TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST><CATEGORY>Output VAT @ 5%</CATEGORY>" +
        //                                "<TAXTYPE>VAT</TAXTYPE><TAXNAME>" + VoucherNo + "</TAXNAME><PARTYLEDGER>" + consultant + "</PARTYLEDGER><REFTYPE>New Ref</REFTYPE><ISOPTIONAL>No</ISOPTIONAL>" +
        //                                "<ISPANVALID>No</ISPANVALID><ZERORATED>No</ZERORATED><EXEMPTED>No</EXEMPTED><ISSPECIALRATE>No</ISSPECIALRATE><ISDEDUCTNOW>No</ISDEDUCTNOW><ISPANNOTAVAILABLE>No</ISPANNOTAVAILABLE>" +
        //                                "<ISSUPPLEMENTARY>No</ISSUPPLEMENTARY><OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>";

        //                        var PercentItems = uCon.ConsultantOrders.Where(x => x.ItemName.StartsWith("PPP")).ToList();
        //                        trackNetAmtTotal = 0;
        //                        count = PercentItems.Count();
        //                        q = 0;
        //                        foreach (var con in PercentItems)
        //                        {
        //                            decimal itemTotalVal = con.ConsultantPrice * con.OrdQty;
        //                            decimal itemRate = itemTotalVal / con.OrdQty;
        //                            decimal itemNetAmt = (con.ConsultantPrice / Decimal.Parse("105") * Decimal.Parse("100"));
        //                            itemNetAmt = Math.Round(itemNetAmt, 2);
        //                            //decimal itemTax = (itemNetAmt * Decimal.Parse("5") / Decimal.Parse("100"));
        //                            decimal itemTax = con.ConsultantPrice - itemNetAmt;
        //                            itemRate = itemRate - itemTax;

        //                            decimal itemsTotalAmt = itemRate * con.OrdQty;
        //                            decimal itemTotalTax = itemTax * con.OrdQty;
        //                            decimal itemTotalNetAmt = itemNetAmt * con.OrdQty;

        //                            //----------------------------------------------------
        //                            //INCLUDED TO TRACK NET AMT TOTALS AFFECTING ROUNDING
        //                            //-----------------------------------------------------
        //                            trackNetAmtTotal += itemTotalNetAmt;


        //                            if (q == count - 1) // last item in consultant orders
        //                            {
                                       
        //                                if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
        //                                {
        //                                    checkedAmt = netAmount - trackNetAmtTotal;

        //                                    itemsTotalAmt = itemsTotalAmt + checkedAmt;
        //                                    itemTotalTax = itemTotalTax - checkedAmt;

        //                                }

        //                            }
        //                            strData = strData + "<SUBCATEGORYALLOCATION.LIST>" +
        //                           "<STOCKITEMNAME>" + con.ItemName + "-" + con.ItemCode + "</STOCKITEMNAME>" +
        //                           "<SUBCATEGORY>VAT</SUBCATEGORY>" +
        //                           "<DUTYLEDGER>12.5% Vat on Sales</DUTYLEDGER>" +
        //                           "<SUBCATZERORATED>No</SUBCATZERORATED>" +
        //                           "<SUBCATEXEMPTED>No</SUBCATEXEMPTED>" +
        //                           "<SUBCATISSPECIALRATE>No</SUBCATISSPECIALRATE>" +
        //                           "<TAXRATE> 12.50</TAXRATE>" +
        //                           "<ASSESSABLEAMOUNT>" + itemsTotalAmt + "</ASSESSABLEAMOUNT>" +
        //                           "<TAX>" + itemTotalTax + "</TAX>" +
        //                           "<BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY>" +
        //                           "</SUBCATEGORYALLOCATION.LIST>";
        //                            q++;
        //                        }
        //                        strData = strData + "</TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>" +
        //                                   "<VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST><COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>" +
        //                                   "</LEDGERENTRIES.LIST>";
        //                        q = 0;
        //                        trackNetAmtTotal = 0;
        //                        foreach (var con in PercentItems)
        //                        {
        //                            decimal itemTotalVal = con.ConsultantPrice * con.OrdQty;
        //                            decimal itemRate = itemTotalVal / con.OrdQty;
        //                            decimal itemNetAmt = (con.ConsultantPrice / Decimal.Parse("105") * Decimal.Parse("100"));
        //                            itemNetAmt = Math.Round(itemNetAmt, 2);
        //                            //decimal itemTax = (itemNetAmt * Decimal.Parse("5") / Decimal.Parse("100"));
        //                            decimal itemTax = con.ConsultantPrice - itemNetAmt;
        //                            itemRate = itemRate - itemTax;

        //                            decimal itemsTotalAmt = itemRate * con.OrdQty;
        //                            decimal itemTotalTax = itemTax * con.OrdQty;
        //                            decimal itemTotalNetAmt = itemNetAmt * con.OrdQty;

        //                            //itemNetAmt = Math.Round(itemNetAmt, 0);

        //                            //----------------------------------------------------
        //                            //INCLUDED TO TRACK NET AMT TOTALS AFFECTING ROUNDING
        //                            //-----------------------------------------------------
        //                            trackNetAmtTotal += itemTotalNetAmt;


        //                            if (q == count - 1) // last item in consultant orders
        //                            {
                                        
        //                                if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
        //                                {
        //                                    checkedAmt = netAmount - trackNetAmtTotal;

        //                                    itemsTotalAmt = itemsTotalAmt + checkedAmt;
        //                                    itemTotalTax = itemTotalTax - checkedAmt;
        //                                    itemRate = itemsTotalAmt / con.OrdQty;
        //                                }

        //                            }

        //                            strData = strData + "<ALLINVENTORYENTRIES.LIST><STOCKITEMNAME>" + con.ItemName + "-" + con.ItemCode + "</STOCKITEMNAME>" +
        //                            "<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><ISAUTONEGATE>No</ISAUTONEGATE>" +
        //                            "<ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE><ISTRACKCOMPONENT>No</ISTRACKCOMPONENT><ISTRACKPRODUCTION>No</ISTRACKPRODUCTION>" +
        //                            "<ISPRIMARYITEM>No</ISPRIMARYITEM><ISSCRAP>No</ISSCRAP><RATE>" + itemRate + "/NO</RATE><AMOUNT>" + itemsTotalAmt + "</AMOUNT>" + //<AMOUNT>" + itemNetAmt + "</AMOUNT>
        //                            "<ACTUALQTY> " + con.OrdQty + " NO</ACTUALQTY><BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY><BATCHALLOCATIONS.LIST>" +
        //                            "<GODOWNNAME>Main Location</GODOWNNAME><BATCHNAME>Primary Batch</BATCHNAME><INDENTNO/><ORDERNO/><TRACKINGNUMBER/>" +
        //                            "<DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED><AMOUNT>" + itemsTotalAmt + "</AMOUNT><ACTUALQTY> " + con.OrdQty + " NO</ACTUALQTY>" +
        //                            "<BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY><ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST><VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST>" +
        //                            @"</BATCHALLOCATIONS.LIST><ACCOUNTINGALLOCATIONS.LIST><OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
        //                            "</OLDAUDITENTRYIDS.LIST><TAXCLASSIFICATIONNAME>Output VAT @ 5%</TAXCLASSIFICATIONNAME><LEDGERNAME>Sales @5%</LEDGERNAME><GSTCLASS/>" +
        //                            "<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM><REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>No</ISPARTYLEDGER>" +
        //                            "<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><AMOUNT>" + itemsTotalAmt + "</AMOUNT><BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST>" +
        //                            "<BILLALLOCATIONS.LIST>        </BILLALLOCATIONS.LIST><INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>" +
        //                            "<ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>        </AUDITENTRIES.LIST><TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST>" +
        //                            "<TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST><VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST>" +
        //                            "<COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST></ACCOUNTINGALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>" +
        //                            "<EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST><EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST></ALLINVENTORYENTRIES.LIST>";

        //                            q++;
        //                            itemNetAmt = XcelServiceHelper.TruncateDecimal(itemNetAmt, 2);
        //                            decimal chkItemTax = con.ConsultantPrice - (itemNetAmt + itemTax);
        //                            if (chkItemTax <= 1 || chkItemTax >= -1)
        //                            {
        //                                itemTax = itemTax + chkItemTax;
        //                            }
        //                            PostedTransaction trans;
        //                            trans = unitOfWork.PostedTransactions.All()
        //                                         .Where(x => x.ItemCode == con.ItemCode)
        //                                         .Where(x => x.OrderId == con.OrderId)
        //                                         .Where(x => x.ConsultantCode == con.Consultant.ConsultantId)
        //                                         .FirstOrDefault();

        //                            if (trans == null)
        //                            {
        //                                trans = new PostedTransaction
        //                                {
        //                                    Company = model.Company,
        //                                    UnitName = unit.Unit,
        //                                    AssemblyName = assembly,
        //                                    ConsultantName = uCon.Consultant,
        //                                    ConsultantCode = uCon.ConsultantId,
        //                                    PostDate = DateTime.Now,
        //                                    Year = year,
        //                                    Week = week,
        //                                    InvoiceNo = VoucherNo,
        //                                    GrossAmount = con.Amount,
        //                                    ConsultantPrice = con.ConsultantPrice,
        //                                    NetAmount = itemNetAmt,
        //                                    VatAmount = itemTax,
        //                                    ItemCode = con.ItemCode,
        //                                    ItemName = con.ItemName,
        //                                    OrderId = con.OrderId,
        //                                    OrderQty = con.OrdQty,
        //                                    Status = con.Status,
        //                                    PayStatus = enPaymentStatus.Pending,
        //                                    InventoryStatus = enInventoryStatus.Pending,
        //                                    PaymentAmount = 0,
        //                                    PostType = enPostType.Invoice_12_5_WithAddress

        //                                };
        //                                unitOfWork.PostedTransactions.Insert(trans);
        //                            }
        //                            else
        //                            {
        //                                trans.ItemCode = con.ItemCode;
        //                                trans.GrossAmount = con.Amount;
        //                                trans.ConsultantPrice = con.ConsultantPrice;
        //                                trans.NetAmount = itemNetAmt;
        //                                trans.VatAmount = itemTax + (con.ConsultantPrice - (itemNetAmt + itemTax));
        //                                trans.ItemCode = con.ItemCode;
        //                                trans.ItemName = con.ItemName;
        //                                trans.OrderId = con.OrderId;
        //                                trans.OrderQty = con.OrdQty;
        //                                trans.Status = con.Status;

        //                                unitOfWork.PostedTransactions.Update(trans);
        //                            }
        //                        }

        //                    }
        //                    #endregion VAT_5%
        //                    strData = strData + "<ATTDRECORDS.LIST>      </ATTDRECORDS.LIST>" +
        //                    "</VOUCHER></TALLYMESSAGE>";
        //                }
        //                unitOfWork.Commit();
        //                unitOfWork.Dispose();
        //            }
                   
        //            strData = strData + "</REQUESTDATA></IMPORTDATA></BODY></ENVELOPE>";
        //            //strData = strData + @"<TALLYMESSAGE xmlns:UDF=""TallyUDF""><COMPANY><REMOTECMPINFO.LIST MERGE=""Yes""><NAME>1992944e-597f-4d18-bdde-35856a4fbddc</NAME>" +
        //            //"<REMOTECMPNAME>" + model.Company + " </REMOTECMPNAME><REMOTECMPSTATE>Maharashtra</REMOTECMPSTATE></REMOTECMPINFO.LIST>" +
        //            //    "</COMPANY></TALLYMESSAGE></REQUESTDATA></IMPORTDATA></BODY></ENVELOPE>";
        //            XmlDocument docum = new XmlDocument();

        //            docum.LoadXml(strData);

        //            if (!Directory.Exists(savePath))
        //            {
        //                Directory.CreateDirectory(savePath);
        //            }
        //            docum.Save(savePath + "//payment.xml");
        //    }
        //}
        public void ProcessExcelSheet(UploadFileModel model, string fileName, string savePath)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart workBk = doc.WorkbookPart;
                WorksheetPart workSht = workBk.WorksheetParts.First();
                SheetData shtData = workSht.Worksheet.Elements<SheetData>().First();

                Sheet theSheet = workBk.Workbook.Descendants<Sheet>().
                  Where(s => s.SheetId == 1).FirstOrDefault();


                //var strData = "<ENVELOPE><HEADER><TALLYREQUEST>Import Data</TALLYREQUEST></HEADER><BODY><IMPORTDATA>" +
                //            "<REQUESTDESC> <REPORTNAME>All Masters</REPORTNAME><STATICVARIABLES><SVCURRENTCOMPANY>" + model.Company +
                //            "</SVCURRENTCOMPANY></STATICVARIABLES></REQUESTDESC><REQUESTDATA>";

                XmlWriter objXmlWriter = XmlWriter.Create(new BufferedStream
                    (new FileStream(savePath + "//payment.xml", FileMode.Create, System.Security.AccessControl.FileSystemRights.Write, 
                        FileShare.None, 1024, FileOptions.SequentialScan)), 
                        new XmlWriterSettings { Indent = true, CloseOutput = true, OmitXmlDeclaration = true , WriteEndDocumentOnClose = false});
                using (objXmlWriter)
                {
                    //writing xml contents here
                    objXmlWriter.WriteStartElement("ENVELOPE");
                    objXmlWriter.WriteStartElement("HEADER");
                    objXmlWriter.WriteElementString("TALLYREQUEST","Import Data");
                    objXmlWriter.WriteEndElement();
                    objXmlWriter.WriteStartElement("BODY");
                    objXmlWriter.WriteStartElement("IMPORTDATA");
                    objXmlWriter.WriteStartElement("REQUESTDESC");
                    objXmlWriter.WriteElementString("REPORTNAME", "All Masters");
                    objXmlWriter.WriteStartElement("STATICVARIABLES");
                    objXmlWriter.WriteElementString("SVCURRENTCOMPANY", model.Company);
                    objXmlWriter.WriteEndElement();
                    objXmlWriter.WriteEndElement();
                    objXmlWriter.WriteStartElement("REQUESTDATA");
                    objXmlWriter.Flush();
                }
                int rows = 0;
                string cellA; string cellD;
                string cellF; string cellH; string cellJ; string cellQ;
                string cellU; string cellW; string cellY; string cellAB; string cellAD; string cellAssembly;


                var year = ""; var week = ""; var assembly = ""; var unitname = "";
                var consutantCode = ""; var consultantName = "";
                decimal checkedAmt; var item = ""; var itemCode = ""; var itemName = ""; int orderId;
                decimal MRP; decimal ordQty; decimal consPrice; decimal amount; var status = "";

                List<UnitName> units = new List<UnitName>();
                List<UnitConsultant> unitConsultants = new List<UnitConsultant>();
                List<ConsultantOrder> consOrders = new List<ConsultantOrder>();
                List<StockItem> stockItems = new List<StockItem>();

                cellAssembly = "L8";
                assembly = XcelServiceHelper.GetCellValue(workBk, theSheet, cellAssembly);
                week = XcelServiceHelper.GetCellValue(workBk, theSheet, "U9");
                year = XcelServiceHelper.GetCellValue(workBk, theSheet, "U8");
                UnitName u;
                ConsultantOrder co;
                UnitConsultant uc;
                int j = 0;

                int rowCount = shtData.Elements<Row>().Count();
                rowCount = rowCount + 4;
                //foreach (Row r in shtData.Elements<Row>())
                XmlWriter objXmlWriter2 = XmlWriter.Create(new BufferedStream
                                               (new FileStream(savePath + "//payment.xml", FileMode.Append, System.Security.AccessControl.FileSystemRights.Write,
                                               FileShare.None, 16384, FileOptions.SequentialScan)),
                                               new XmlWriterSettings { Indent = true, CloseOutput = true, OmitXmlDeclaration = true, 
                                                   WriteEndDocumentOnClose = false, ConformanceLevel = ConformanceLevel.Fragment });
                using (objXmlWriter2)
                {
                    for (rows = 1; rows <= rowCount; rows++)
                    {
                        //rows += 1;
                        cellQ = "Q" + rows;

                        if (rows >= 12)
                        {
                            //Cell theCell = r.Descendants<Cell>().Where(x => x.CellReference == cellQ).FirstOrDefault();
                            string theCell = XcelServiceHelper.GetCellValue(workBk, theSheet, cellQ);
                            if (!string.IsNullOrEmpty(theCell))
                            {
                                //
                                string ratePercVal = "";
                                cellQ = "Q" + rows;
                                cellJ = "J" + rows;
                                itemCode = XcelServiceHelper.GetCellValue(workBk, theSheet, cellJ);
                                itemName = XcelServiceHelper.GetCellValue(workBk, theSheet, cellQ);
                                itemName = itemName.Replace("(", " ");
                                itemName = itemName.Replace(")", " ");
                                itemName = itemName.Replace("&", "_");
                                itemName = itemName.Replace("'", " ");
                                item = itemName + " - " + itemCode;

                                StockItem s = stockItems.FirstOrDefault(x => x.ItemCode == itemCode.Trim());
                                if (s == null)
                                {
                                    if (itemName.StartsWith("PPP")) ratePercVal = " 5";
                                    else ratePercVal = " 12.5";

                     

                                    objXmlWriter2.WriteStartElement("TALLYMESSAGE"); objXmlWriter2.WriteAttributeString("xmlns", "UDF", null, "TallyUDF");
                                    objXmlWriter2.WriteStartElement("STOCKITEM"); objXmlWriter2.WriteAttributeString("NAME", item);
                                    objXmlWriter2.WriteAttributeString("RESERVEDNAME", ""); objXmlWriter2.WriteStartElement("OLDAUDITENTRYIDS.LIST");
                                    objXmlWriter2.WriteAttributeString("TYPE", "Number"); objXmlWriter2.WriteElementString("OLDAUDITENTRYIDS", "-1");
                                    objXmlWriter2.WriteEndElement(); objXmlWriter2.WriteElementString("PARENT", "Tupperware Products");
                                    objXmlWriter2.WriteStartElement("CATEGORY"); objXmlWriter2.WriteEndElement(); objXmlWriter2.WriteStartElement("TAXCLASSIFICATIONNAME"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteElementString("COSTINGMETHOD", "Avg. Cost"); objXmlWriter2.WriteElementString("VALUATIONMETHOD", "Avg. Price");
                                    objXmlWriter2.WriteElementString("BASEUNITS", "NO"); objXmlWriter2.WriteStartElement("ADDITIONALUNITS"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteStartElement("EXCISEITEMCLASSIFICATION"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteElementString("ISCOSTCENTRESON", "NO"); objXmlWriter2.WriteElementString("ISBATCHWISEON", "NO");
                                    objXmlWriter2.WriteElementString("ISPERISHABLEON", "NO"); objXmlWriter2.WriteElementString("ISENTRYTAXAPPLICABLE", "NO");
                                    objXmlWriter2.WriteElementString("ISCOSTTRACKINGON", "NO"); objXmlWriter2.WriteElementString("IGNOREPHYSICALDIFFERENCE", "NO");
                                    objXmlWriter2.WriteElementString("IGNORENEGATIVESTOCK", "NO"); objXmlWriter2.WriteElementString("TREATSALESASMANUFACTURED", "NO");
                                    objXmlWriter2.WriteElementString("TREATPURCHASESASCONSUMED", "NO"); objXmlWriter2.WriteElementString("TREATREJECTSASSCRAP", "NO");
                                    objXmlWriter2.WriteElementString("HASMFGDATE", "NO"); objXmlWriter2.WriteElementString("ALLOWUSEOFEXPIREDITEMS", "NO");
                                    objXmlWriter2.WriteElementString("IGNOREBATCHES", "NO"); objXmlWriter2.WriteElementString("IGNOREGODOWNS", "NO");
                                    objXmlWriter2.WriteElementString("CALCONMRP", "NO"); objXmlWriter2.WriteElementString("EXCLUDEJRNLFORVALUATION", "NO");
                                    objXmlWriter2.WriteElementString("ISMRPINCLOFTAX", "NO"); objXmlWriter2.WriteElementString("ISADDLTAXEXEMPT", "NO");
                                    objXmlWriter2.WriteElementString("ISSUPPLEMENTRYDUTYON", "NO"); objXmlWriter2.WriteElementString("REORDERASHIGHER", "NO");
                                    objXmlWriter2.WriteElementString("MINORDERASHIGHER", "NO"); objXmlWriter2.WriteElementString("DENOMINATOR", "1");
                                    objXmlWriter2.WriteElementString("RATEOFVAT", ratePercVal); objXmlWriter2.WriteStartElement("LANGUAGENAME.LIST");
                                    objXmlWriter2.WriteStartElement("NAME.LIST"); objXmlWriter2.WriteAttributeString("TYPE", "String");
                                    objXmlWriter2.WriteElementString("NAME", item); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteElementString("LANGUAGEID", "1033"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteStartElement("SCHVIDETAILS.LIST"); objXmlWriter2.WriteEndElement(); objXmlWriter2.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter2.WriteEndElement(); objXmlWriter2.WriteStartElement("AUDITENTRIES.LIST"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteStartElement("COMPONENTLIST.LIST"); objXmlWriter2.WriteEndElement(); objXmlWriter2.WriteStartElement("ADDITIONALLEDGERS.LIST"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteStartElement("SALESLIST.LIST"); objXmlWriter2.WriteEndElement(); objXmlWriter2.WriteStartElement("PURCHASELIST.LIST"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteStartElement("FULLPRICELIST.LIST"); objXmlWriter2.WriteEndElement(); objXmlWriter2.WriteStartElement("BATCHALLOCATIONS.LIST"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteStartElement("TRADEREXCISEDUTIES.LIST"); objXmlWriter2.WriteEndElement(); objXmlWriter2.WriteStartElement("STANDARDCOSTLIST.LIST"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteStartElement("STANDARDPRICELIST.LIST"); objXmlWriter2.WriteEndElement(); objXmlWriter2.WriteStartElement("EXCISEITEMGODOWN.LIST"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteStartElement("MULTICOMPONENTLIST.LIST"); objXmlWriter2.WriteEndElement(); objXmlWriter2.WriteStartElement("PRICELEVELLIST.LIST"); objXmlWriter2.WriteEndElement();
                                    objXmlWriter2.WriteEndElement(); objXmlWriter2.WriteEndElement();
                                    
                                    s = new StockItem
                                    {
                                        ItemCode = itemCode.Trim().ToString(),
                                        ItemName = itemName.Trim().ToString()
                                    };
                                    stockItems.Add(s);
                                }
                                cellA = "A" + rows;
                                unitname = XcelServiceHelper.GetCellValue(workBk, theSheet, cellA);

                                u = units.FirstOrDefault(x => x.Unit == unitname.Trim());
                                if (u == null)
                                {
                                    u = new UnitName
                                    {
                                        Unit = unitname,
                                        IsGroupCreated = false
                                    };
                                    units.Add(u);
                                    units.OrderBy(x => x.Unit);
                                }
                                cellD = "D" + rows;
                                consutantCode = XcelServiceHelper.GetCellValue(workBk, theSheet, cellD);
                                cellF = "F" + rows;
                                consultantName = XcelServiceHelper.GetCellValue(workBk, theSheet, cellF);

                                uc = u.UnitConsultants.FirstOrDefault(x => x.Consultant == consultantName.Trim());
                                if (uc == null)
                                {
                                    uc = new UnitConsultant
                                    {
                                        Consultant = consultantName.Trim(),
                                        ConsultantId = consutantCode.Trim(),
                                        UnitName = u
                                    };
                                    if (u != null) u.UnitConsultants.Add(uc);
                                }
                                cellH = "H" + rows;
                                orderId = int.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellH));
                                cellU = "U" + rows;
                                ordQty = Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellU));

                                cellW = "W" + rows;
                                MRP = Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellW));

                                cellY = "Y" + rows;
                                consPrice = Math.Round(Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellY)));

                                cellAB = "AB" + rows;
                                amount = Math.Round(Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellAB)));

                                cellAD = "AD" + rows;
                                status = XcelServiceHelper.GetCellValue(workBk, theSheet, cellAD);

                                co = uc.ConsultantOrders.FirstOrDefault(x => x.ItemCode == itemCode.Trim());


                                if (co == null)
                                {
                                    co = new ConsultantOrder
                                    {
                                        OrderId = orderId,
                                        Amount = amount,
                                        ItemCode = itemCode.Trim(),
                                        ItemName = itemName.Trim(),
                                        MRP = MRP,
                                        OrdQty = ordQty,
                                        ConsultantPrice = consPrice,
                                        Status = status,
                                        Consultant = uc

                                    };

                                    co.VoucherId = j + 1;
                                    j++;

                                    if (uc != null)
                                    {
                                        //prevId = co.OrderId;
                                        //prevVoucherId = co.VoucherId;
                                        uc.ConsultantOrders.Add(co);
                                        uc.ConsultantOrders.OrderBy(x => x.OrderId).ThenBy(x => x.ItemName);
                                    }
                                }

                            }
                        }

                    }
                    objXmlWriter2.Flush();
                    objXmlWriter2.Dispose();
                }

                
                foreach (var unit in units)
                
                {
                    unitOfWork = new UnitOfWork();
                    XmlWriter objXmlWriter3 = XmlWriter.Create(new BufferedStream
                                               (new FileStream(savePath + "//payment.xml", FileMode.Append, System.Security.AccessControl.FileSystemRights.Write,
                                               FileShare.None, 16384, FileOptions.SequentialScan)),
                                               new XmlWriterSettings
                                               {
                                                   Indent = true,
                                                   CloseOutput = true,
                                                   OmitXmlDeclaration = true,
                                                   WriteEndDocumentOnClose = false,
                                                   ConformanceLevel = ConformanceLevel.Fragment
                                               });
                    using (objXmlWriter3)
                    {
                        foreach (var uCon in unit.UnitConsultants)
                        {
                            string consultant = uCon.Consultant + " - " + uCon.ConsultantId;

                            //newVoucherNo = newVoucherNo + 1;
                            objXmlWriter3.WriteStartElement("TALLYMESSAGE"); objXmlWriter3.WriteAttributeString("xmlns", "UDF", null, "TallyUDF");
                            objXmlWriter3.WriteStartElement("LEDGER"); objXmlWriter3.WriteAttributeString("NAME", consultant); objXmlWriter3.WriteAttributeString("RESERVEDNAME", "");
                            objXmlWriter3.WriteStartElement("ADDRESS.LIST"); objXmlWriter3.WriteAttributeString("TYPE", "String"); objXmlWriter3.WriteElementString("ADDRESS", unit.Unit);
                            objXmlWriter3.WriteElementString("ADDRESS", assembly); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("MAILINGNAME.LIST");
                            objXmlWriter3.WriteAttributeString("TYPE", "String"); objXmlWriter3.WriteElementString("MAILINGNAME", consultant); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("OLDAUDITENTRYIDS.LIST"); objXmlWriter3.WriteAttributeString("TYPE", "Number"); objXmlWriter3.WriteElementString("OLDAUDITENTRYIDS", "-1");
                            objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteElementString("ALTEREDON", model.Date); objXmlWriter3.WriteElementString("STATENAME", "Maharashtra");
                            objXmlWriter3.WriteElementString("PARENT", unit.Unit); objXmlWriter3.WriteStartElement("TAXCLASSIFICATIONNAME"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteElementString("TAXTYPE", "Others"); objXmlWriter3.WriteStartElement("BUSINESSTYPE"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteElementString("BASICTYPEOFDUTY", "Excise Surcharge"); objXmlWriter3.WriteStartElement("GSTTYPE"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("APPROPRIATEFOR"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("SERVICECATEGORY"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("EXCISELEDGERCLASSIFICATION"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("EXCISEDUTYTYPE"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("EXCISENATUREOFPURCHASE"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("LEDGERFBTCATEGORY"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteElementString("ISBILLWISEON", "No"); objXmlWriter3.WriteElementString("ISCOSTCENTRESON", "Yes"); objXmlWriter3.WriteElementString("ISINTERESTON", "No");
                            objXmlWriter3.WriteElementString("ALLOWINMOBILE", "No"); objXmlWriter3.WriteElementString("ISCOSTTRACKINGON", "No"); objXmlWriter3.WriteElementString("ISCONDENSED", "No");
                            objXmlWriter3.WriteElementString("AFFECTSSTOCK", "No"); objXmlWriter3.WriteElementString("FORPAYROLL", "No"); objXmlWriter3.WriteElementString("ISABCENABLED", "No");
                            objXmlWriter3.WriteElementString("INTERESTONBILLWISE", "No"); objXmlWriter3.WriteElementString("OVERRIDEINTEREST", "No"); objXmlWriter3.WriteElementString("OVERRIDEADVINTEREST", "No");
                            objXmlWriter3.WriteElementString("USEFORVAT", "No"); objXmlWriter3.WriteElementString("IGNORETDSEXEMPT", "No"); objXmlWriter3.WriteElementString("ISTCSAPPLICABLE", "No");
                            objXmlWriter3.WriteElementString("ISTDSAPPLICABLE", "No"); objXmlWriter3.WriteElementString("ISFBTAPPLICABLE", "No"); objXmlWriter3.WriteElementString("ISGSTAPPLICABLE", "No");
                            objXmlWriter3.WriteElementString("ISEXCISEAPPLICABLE", "No"); objXmlWriter3.WriteElementString("ISTDSEXPENSE", "No"); objXmlWriter3.WriteElementString("ISEDLIAPPLICABLE", "No");
                            objXmlWriter3.WriteElementString("ISRELATEDPARTY", "No"); objXmlWriter3.WriteElementString("USEFORESIELIGIBILITY", "No"); objXmlWriter3.WriteElementString("SHOWINPAYSLIP", "No");
                            objXmlWriter3.WriteElementString("USEFORGRATUITY", "No"); objXmlWriter3.WriteElementString("ISTDSPROJECTED", "No"); objXmlWriter3.WriteElementString("FORSERVICETAX", "No");
                            objXmlWriter3.WriteElementString("ISINPUTCREDIT", "No"); objXmlWriter3.WriteElementString("ISEXEMPTED", "No"); objXmlWriter3.WriteElementString("ISABATEMENTAPPLICABLE", "No");
                            objXmlWriter3.WriteElementString("ISSTXPARTY", "No"); objXmlWriter3.WriteElementString("ISSTXNONREALIZEDTYPE", "No"); objXmlWriter3.WriteElementString("TDSDEDUCTEEISSPECIALRATE", "No");
                            objXmlWriter3.WriteElementString("AUDITED", "No"); objXmlWriter3.WriteElementString("SORTPOSITION", "1000"); objXmlWriter3.WriteElementString("RATEOFTAXCALCULATION", "12.50");
                            objXmlWriter3.WriteStartElement("LANGUAGENAME.LIST"); objXmlWriter3.WriteStartElement("NAME.LIST"); objXmlWriter3.WriteAttributeString("TYPE", "String");
                            objXmlWriter3.WriteElementString("NAME", consultant); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteElementString("LANGUAGEID", "1033"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("XBRLDETAIL.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("AUDITDETAILS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("SCHVIDETAILS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("SLABPERIOD.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("GRATUITYPERIOD.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ADDITIONALCOMPUTATIONS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("BANKALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("PAYMENTDETAILS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("BANKEXPORTFORMATS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("BILLALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("INTERESTCOLLECTION.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("LEDGERCLOSINGVALUES.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("LEDGERAUDITCLASS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("TDSEXEMPTIONRULES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("DEDUCTINSAMEVCHRULES.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("LOWERDEDUCTION.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("STXABATEMENTDETAILS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("LEDMULTIADDRESSLIST.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("STXTAXDETAILS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("CHEQUERANGE.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("DEFAULTVCHCHEQUEDETAILS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("AUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("BRSIMPORTEDINFO.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("AUTOBRSCONFIGS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("BANKURENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("DEFAULTCHEQUEDETAILS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("DEFAULTOPENINGCHEQUEDETAILS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteEndElement();

                            if (unit != null && unit.IsGroupCreated == false)
                            {
                                objXmlWriter3.WriteStartElement("TALLYMESSAGE"); objXmlWriter3.WriteAttributeString("xmlns", "UDF", null, "TallyUDF");
                                objXmlWriter3.WriteStartElement("GROUP"); objXmlWriter3.WriteAttributeString("NAME", unit.Unit); objXmlWriter3.WriteAttributeString("ACTION", "CREATE");
                                objXmlWriter3.WriteStartElement("NAME.LIST"); objXmlWriter3.WriteElementString("NAME", unit.Unit); objXmlWriter3.WriteEndElement();
                                objXmlWriter3.WriteElementString("PARENT", "Sundry Debtors"); objXmlWriter3.WriteElementString("ISSUBLEDGER", "No"); objXmlWriter3.WriteElementString("ISBILLWISEON", "No");
                                objXmlWriter3.WriteElementString("ISCOSTCENTRESON", "No"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteEndElement();
                            }
                            decimal totalConsultantAmt = uCon.ConsultantOrders.Sum(x => x.ConsultantPrice * x.OrdQty);
                            decimal consultantAmount = uCon.ConsultantOrders.Where(x => !x.ItemName.StartsWith("PPP")).Sum(x => x.ConsultantPrice * x.OrdQty);
                            decimal netAmount = ((consultantAmount / Decimal.Parse("112.5") * Decimal.Parse("100")));
                            //decimal tax = ((netAmount * Decimal.Parse("12.5")) / Decimal.Parse("100"));


                            var ordId = uCon.ConsultantOrders.Select(x => x.OrderId).First();

                            var voucherId = uCon.ConsultantOrders.Where(x => x.OrderId == ordId).Select(x => x.VoucherId).First();

                            netAmount = Math.Round(netAmount, 2);
                            decimal tax = consultantAmount - netAmount;

                            //tax = TruncateDecimal(tax, 2);
                            decimal chkAmt = (consultantAmount - (netAmount + tax));
                            //if (chkAmt <= 1 || chkAmt >= -1)
                            //{
                            //    tax = tax + chkAmt;
                            //}
                            string VoucherNo = week + "-" + ordId + "/" + voucherId;

                            objXmlWriter3.WriteStartElement("TALLYMESSAGE"); objXmlWriter3.WriteAttributeString("xmlns", "UDF", null, "TallyUDF");
                            objXmlWriter3.WriteStartElement("VOUCHER"); objXmlWriter3.WriteAttributeString("VCHTYPE", "Sales"); objXmlWriter3.WriteAttributeString("ACTION", "Create");
                            objXmlWriter3.WriteAttributeString("OBJVIEW", "Invoice Voucher View"); objXmlWriter3.WriteStartElement("ADDRESS.LIST"); objXmlWriter3.WriteAttributeString("TYPE", "String");
                            objXmlWriter3.WriteElementString("ADDRESS", unit.Unit); objXmlWriter3.WriteElementString("ADDRESS", assembly); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("BASICBUYERADDRESS.LIST"); objXmlWriter3.WriteAttributeString("TYPE", "String");
                            objXmlWriter3.WriteElementString("BASICBUYERADDRESS", unit.Unit); objXmlWriter3.WriteElementString("BASICBUYERADDRESS", assembly); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("OLDAUDITENTRYIDS.LIST"); objXmlWriter3.WriteAttributeString("TYPE", "Number"); objXmlWriter3.WriteElementString("OLDAUDITENTRYIDS", "-1");
                            objXmlWriter3.WriteElementString("DATE", model.Date); objXmlWriter3.WriteElementString("PARTYNAME", consultant); objXmlWriter3.WriteElementString("VOUCHERTYPENAME", "Sales");
                            objXmlWriter3.WriteElementString("VOUCHERNUMBER", VoucherNo); objXmlWriter3.WriteElementString("PARTYLEDGERNAME", consultant); objXmlWriter3.WriteElementString("BASICBASEPARTYNAME", consultant);
                            objXmlWriter3.WriteStartElement("CSTFORMISSUETYPE"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("CSTFORMRECVTYPE"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteElementString("FBTPAYMENTTYPE", "Default"); objXmlWriter3.WriteElementString("PERSISTEDVIEW", "Invoice Voucher View");objXmlWriter3.WriteElementString("BASICBUYERNAME", consultant); 
                            objXmlWriter3.WriteElementString("BASICFINALDESTINATION", assembly); objXmlWriter3.WriteStartElement("VCHGSTCLASS"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteElementString("DIFFACTUALQTY", "No");
                            objXmlWriter3.WriteElementString("AUDITED", "No"); objXmlWriter3.WriteElementString("FORJOBCOSTING", "No"); objXmlWriter3.WriteElementString("ISOPTIONAL", "No");
                            objXmlWriter3.WriteElementString("EFFECTIVEDATE", model.Date); objXmlWriter3.WriteElementString("ISFORJOBWORKIN", "No"); objXmlWriter3.WriteElementString("ALLOWCONSUMPTION", "No");
                            objXmlWriter3.WriteElementString("USEFORINTEREST", "No"); objXmlWriter3.WriteElementString("USEFORGAINLOSS", "No"); objXmlWriter3.WriteElementString("USEFORGODOWNTRANSFER", "No");
                            objXmlWriter3.WriteElementString("USEFORCOMPOUND", "No"); objXmlWriter3.WriteElementString("EXCISEOPENING", "No"); objXmlWriter3.WriteElementString("USEFORFINALPRODUCTION", "No");
                            objXmlWriter3.WriteElementString("ISCANCELLED", "No"); objXmlWriter3.WriteElementString("HASCASHFLOW", "No"); objXmlWriter3.WriteElementString("ISPOSTDATED", "No");
                            objXmlWriter3.WriteElementString("USETRACKINGNUMBER", "No"); objXmlWriter3.WriteElementString("ISINVOICE", "Yes"); objXmlWriter3.WriteElementString("MFGJOURNAL", "No");
                            objXmlWriter3.WriteElementString("HASDISCOUNTS", "No"); objXmlWriter3.WriteElementString("ASPAYSLIP", "No"); objXmlWriter3.WriteElementString("ISCOSTCENTRE", "No");
                            objXmlWriter3.WriteElementString("ISSTXNONREALIZEDVCH", "No"); objXmlWriter3.WriteElementString("ISEXCISEMANUFACTURERON", "Yes"); objXmlWriter3.WriteElementString("ISBLANKCHEQUE", "No");
                            objXmlWriter3.WriteElementString("ISDELETED", "No"); objXmlWriter3.WriteElementString("ASORIGINAL", "No"); objXmlWriter3.WriteElementString("VCHISFROMSYNC", "No");
                            objXmlWriter3.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("AUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("INVOICEDELNOTES.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("INVOICEORDERLIST.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("INVOICEINDENTLIST.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("ATTENDANCEENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ORIGINVOICEDETAILS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("INVOICEEXPORTLIST.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("LEDGERENTRIES.LIST");
                            objXmlWriter3.WriteStartElement("OLDAUDITENTRYIDS.LIST"); objXmlWriter3.WriteAttributeString("TYPE", "Number"); objXmlWriter3.WriteElementString("OLDAUDITENTRYIDS", "-1");
                            objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteElementString("LEDGERNAME", consultant); objXmlWriter3.WriteStartElement("GSTCLASS"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteElementString("ISDEEMEDPOSITIVE", "Yes"); objXmlWriter3.WriteElementString("LEDGERFROMITEM", "No"); objXmlWriter3.WriteElementString("REMOVEZEROENTRIES", "No");
                            objXmlWriter3.WriteElementString("ISPARTYLEDGER", "Yes"); objXmlWriter3.WriteElementString("ISLASTDEEMEDPOSITIVE", "Yes"); objXmlWriter3.WriteElementString("AMOUNT", "-" + totalConsultantAmt);
                            objXmlWriter3.WriteStartElement("CATEGORYALLOCATIONS.LIST"); objXmlWriter3.WriteElementString("CATEGORY", "Primary Cost Category"); objXmlWriter3.WriteElementString("ISDEEMEDPOSITIVE", "Yes");
                            objXmlWriter3.WriteStartElement("COSTCENTREALLOCATIONS.LIST"); objXmlWriter3.WriteElementString("NAME", assembly); objXmlWriter3.WriteElementString("AMOUNT", "-" + totalConsultantAmt);
                            objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("BANKALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("BILLALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();objXmlWriter3.WriteStartElement("INTERESTCOLLECTION.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("AUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TAXBILLALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("TAXOBJECTALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TDSEXPENSEALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("VATSTATUTORYDETAILS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("COSTTRACKALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("LEDGERENTRIES.LIST");
                            objXmlWriter3.WriteStartElement("OLDAUDITENTRYIDS.LIST"); objXmlWriter3.WriteAttributeString("TYPE", "Number"); objXmlWriter3.WriteElementString("OLDAUDITENTRYIDS", "-1");
                            objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("BASICRATEOFINVOICETAX.LIST"); objXmlWriter3.WriteAttributeString("TYPE", "Number");
                            objXmlWriter3.WriteElementString("BASICRATEOFINVOICETAX", "12.50"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteElementString("TAXCLASSIFICATIONNAME", "Output VAT @ 12.5%");
                            objXmlWriter3.WriteElementString("ROUNDTYPE", "Normal Rounding"); objXmlWriter3.WriteElementString("LEDGERNAME", "12.5% Vat on Sales"); objXmlWriter3.WriteStartElement("GSTCLASS"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteElementString("ISDEEMEDPOSITIVE", "No"); objXmlWriter3.WriteElementString("LEDGERFROMITEM", "No"); objXmlWriter3.WriteElementString("REMOVEZEROENTRIES", "No");
                            objXmlWriter3.WriteElementString("ISPARTYLEDGER", "No"); objXmlWriter3.WriteElementString("ISLASTDEEMEDPOSITIVE", "No"); objXmlWriter3.WriteElementString("AMOUNT", tax.ToString());
                            objXmlWriter3.WriteElementString("VATASSESSABLEVALUE", netAmount.ToString()); objXmlWriter3.WriteStartElement("BANKALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("BILLALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("INTERESTCOLLECTION.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("AUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TAXBILLALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("TAXOBJECTALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteElementString("CATEGORY", "Output VAT @ 12.5%");
                            objXmlWriter3.WriteElementString("TAXTYPE", "VAT"); objXmlWriter3.WriteElementString("TAXNAME", VoucherNo); objXmlWriter3.WriteElementString("PARTYLEDGER", consultant);
                            objXmlWriter3.WriteElementString("REFTYPE", "New Ref"); objXmlWriter3.WriteElementString("ISOPTIONAL", "No"); objXmlWriter3.WriteElementString("ISPANVALID", "No");
                            objXmlWriter3.WriteElementString("ZERORATED", "No"); objXmlWriter3.WriteElementString("EXEMPTED", "No"); objXmlWriter3.WriteElementString("ISSPECIALRATE", "No");
                            objXmlWriter3.WriteElementString("ISDEDUCTNOW", "No"); objXmlWriter3.WriteElementString("ISPANNOTAVAILABLE", "No"); objXmlWriter3.WriteElementString("ISSUPPLEMENTARY", "No");
                            objXmlWriter3.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("AUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();

                            decimal trackNetAmtTotal = 0;
                            int count = uCon.ConsultantOrders.Where(x => !x.ItemName.StartsWith("PPP")).Count();
                            int q = 0;
                            foreach (var con in uCon.ConsultantOrders)
                            {
                                if (!con.ItemName.StartsWith("PPP"))
                                {
                                    decimal itemTotalVal = con.ConsultantPrice * con.OrdQty;
                                    decimal itemRate = itemTotalVal / con.OrdQty;
                                    decimal itemNetAmt = (con.ConsultantPrice / Decimal.Parse("112.5") * Decimal.Parse("100"));
                                    itemNetAmt = Math.Round(itemNetAmt, 2);
                                    //decimal itemTax = (itemNetAmt * Decimal.Parse("12.5") / Decimal.Parse("100"));
                                    decimal itemTax = con.ConsultantPrice - itemNetAmt;
                                    itemRate = itemRate - itemTax;

                                    decimal itemsTotalAmt = itemRate * con.OrdQty;
                                    decimal itemTotalTax = itemTax * con.OrdQty;
                                    decimal itemTotalNetAmt = itemNetAmt * con.OrdQty;

                                    //----------------------------------------------------
                                    //INCLUDED TO TRACK NET AMT TOTALS AFFECTING ROUNDING
                                    //-----------------------------------------------------
                                    trackNetAmtTotal += itemTotalNetAmt;


                                    if (q == count - 1) // last item in consultant orders
                                    {

                                        if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
                                        {
                                            checkedAmt = netAmount - trackNetAmtTotal;

                                            itemsTotalAmt = itemsTotalAmt + checkedAmt;
                                            itemTotalTax = itemTotalTax - checkedAmt;

                                        }

                                    }
                                    objXmlWriter3.WriteStartElement("SUBCATEGORYALLOCATION.LIST"); objXmlWriter3.WriteElementString("STOCKITEMNAME", con.ItemName + "-" + con.ItemCode);
                                    objXmlWriter3.WriteElementString("SUBCATEGORY", "VAT"); objXmlWriter3.WriteElementString("DUTYLEDGER", "12.5% Vat on Sales");
                                    objXmlWriter3.WriteElementString("SUBCATZERORATED", "No"); objXmlWriter3.WriteElementString("SUBCATEXEMPTED", "No");
                                    objXmlWriter3.WriteElementString("SUBCATISSPECIALRATE", "No"); objXmlWriter3.WriteElementString("TAXRATE", " 12.50");
                                    objXmlWriter3.WriteElementString("ASSESSABLEAMOUNT", itemsTotalAmt.ToString()); objXmlWriter3.WriteElementString("TAX", itemTotalTax.ToString());
                                    objXmlWriter3.WriteElementString("BILLEDQTY", con.OrdQty.ToString()); objXmlWriter3.WriteEndElement();
                                    q++;
                                }

                            }
                            objXmlWriter3.WriteStartElement("TAXOBJECTALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("TDSEXPENSEALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("VATSTATUTORYDETAILS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteStartElement("COSTTRACKALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteEndElement();

                            q = 0;
                            trackNetAmtTotal = 0;
                            foreach (var con in uCon.ConsultantOrders)
                            {

                                if (!con.ItemName.StartsWith("PPP"))
                                {
                                    decimal itemTotalVal = con.ConsultantPrice * con.OrdQty;
                                    decimal itemRate = itemTotalVal / con.OrdQty;
                                    decimal itemNetAmt = (con.ConsultantPrice / Decimal.Parse("112.5") * Decimal.Parse("100"));
                                    itemNetAmt = Math.Round(itemNetAmt, 2);
                                    //decimal itemTax = (itemNetAmt * Decimal.Parse("12.5") / Decimal.Parse("100"));
                                    decimal itemTax = con.ConsultantPrice - itemNetAmt;
                                    itemRate = itemRate - itemTax;

                                    decimal itemsTotalAmt = itemRate * con.OrdQty;
                                    decimal itemTotalTax = itemTax * con.OrdQty;
                                    decimal itemTotalNetAmt = itemNetAmt * con.OrdQty;

                                    //----------------------------------------------------
                                    //INCLUDED TO TRACK NET AMT TOTALS AFFECTING ROUNDING
                                    //-----------------------------------------------------
                                    trackNetAmtTotal += itemTotalNetAmt;


                                    if (q == count - 1) // last item in consultant orders
                                    {

                                        if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
                                        {
                                            checkedAmt = netAmount - trackNetAmtTotal;

                                            itemsTotalAmt = itemsTotalAmt + checkedAmt;
                                            itemTotalTax = itemTotalTax - checkedAmt;
                                            itemRate = itemsTotalAmt / con.OrdQty;
                                        }

                                    }
                                    //-----------------------------------------------------------------------
                                    objXmlWriter3.WriteStartElement("ALLINVENTORYENTRIES.LIST"); objXmlWriter3.WriteElementString("STOCKITEMNAME", con.ItemName + "-" + con.ItemCode);
                                    objXmlWriter3.WriteElementString("ISDEEMEDPOSITIVE", "No"); objXmlWriter3.WriteElementString("ISLASTDEEMEDPOSITIVE", "No");
                                    objXmlWriter3.WriteElementString("ISAUTONEGATE", "No"); objXmlWriter3.WriteElementString("ISCUSTOMSCLEARANCE", "No");
                                    objXmlWriter3.WriteElementString("ISTRACKCOMPONENT", "No"); objXmlWriter3.WriteElementString("ISTRACKPRODUCTION", "No");
                                    objXmlWriter3.WriteElementString("ISPRIMARYITEM", "No"); objXmlWriter3.WriteElementString("ISSCRAP", "No");
                                    objXmlWriter3.WriteElementString("RATE", itemRate.ToString() + "/NO"); objXmlWriter3.WriteElementString("AMOUNT", itemsTotalAmt.ToString());
                                    objXmlWriter3.WriteElementString("ACTUALQTY", con.OrdQty.ToString()); objXmlWriter3.WriteElementString("BILLEDQTY", con.OrdQty.ToString());
                                    objXmlWriter3.WriteStartElement("BATCHALLOCATIONS.LIST"); objXmlWriter3.WriteElementString("GODOWNNAME", "Main Location");
                                    objXmlWriter3.WriteElementString("BATCHNAME", "Primary Batch"); objXmlWriter3.WriteStartElement("INDENTNO"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("ORDERNO"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TRACKINGNUMBER"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteElementString("DYNAMICCSTISCLEARED", "No"); objXmlWriter3.WriteElementString("AMOUNT", itemsTotalAmt.ToString());
                                    objXmlWriter3.WriteElementString("ACTUALQTY", con.OrdQty.ToString() + " NO"); objXmlWriter3.WriteElementString("BILLEDQTY", con.OrdQty.ToString() + " NO");
                                    objXmlWriter3.WriteStartElement("ADDITIONALDETAILS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("VOUCHERCOMPONENTLIST.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTINGALLOCATIONS.LIST"); objXmlWriter3.WriteStartElement("OLDAUDITENTRYIDS.LIST");
                                    objXmlWriter3.WriteAttributeString("TYPE", "Number"); objXmlWriter3.WriteElementString("OLDAUDITENTRYIDS", "-1"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteElementString("TAXCLASSIFICATIONNAME", "Output VAT @ 12.5%"); objXmlWriter3.WriteElementString("LEDGERNAME", "Sales @12.5%");
                                    objXmlWriter3.WriteStartElement("GSTCLASS"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteElementString("ISDEEMEDPOSITIVE", "No");
                                    objXmlWriter3.WriteElementString("LEDGERFROMITEM", "No"); objXmlWriter3.WriteElementString("REMOVEZEROENTRIES", "No");
                                    objXmlWriter3.WriteElementString("ISPARTYLEDGER", "No"); objXmlWriter3.WriteElementString("ISLASTDEEMEDPOSITIVE", "No");
                                    objXmlWriter3.WriteElementString("AMOUNT", itemsTotalAmt.ToString()); objXmlWriter3.WriteStartElement("BANKALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("BILLALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("INTERESTCOLLECTION.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("AUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TAXBILLALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("TAXOBJECTALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TDSEXPENSEALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("VATSTATUTORYDETAILS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("COSTTRACKALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TAXOBJECTALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("EXCISEALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("EXPENSEALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteEndElement();

                                    q++;
                                    itemNetAmt = XcelServiceHelper.TruncateDecimal(itemNetAmt, 2);
                                    decimal chkItemTax = con.ConsultantPrice - (itemNetAmt + itemTax);
                                    if (chkItemTax <= 1 || chkItemTax >= -1)
                                    {
                                        itemTax = itemTax + chkItemTax;
                                    }
                                    PostedTransaction trans;
                                    //using (var db = new eXmlContext())
                                    //{
                                    trans = unitOfWork.PostedTransactions.All()
                                             .Where(x => x.ItemCode == con.ItemCode)
                                             .Where(x => x.OrderId == con.OrderId)
                                             .Where(x => x.ConsultantCode == con.Consultant.ConsultantId)
                                             .FirstOrDefault();

                                    if (trans == null)
                                    {
                                        trans = new PostedTransaction
                                        {
                                            Company = model.Company,
                                            UnitName = unit.Unit,
                                            AssemblyName = assembly,
                                            ConsultantName = uCon.Consultant,
                                            ConsultantCode = uCon.ConsultantId,
                                            PostDate = DateTime.Now,
                                            Year = year,
                                            Week = week,
                                            InvoiceNo = VoucherNo,
                                            GrossAmount = con.Amount,
                                            ConsultantPrice = con.ConsultantPrice,
                                            NetAmount = itemNetAmt,
                                            VatAmount = itemTax,
                                            ItemCode = con.ItemCode,
                                            ItemName = con.ItemName,
                                            OrderId = con.OrderId,
                                            OrderQty = con.OrdQty,
                                            Status = con.Status,
                                            PayStatus = enPaymentStatus.Pending,
                                            InventoryStatus = enInventoryStatus.Pending,
                                            PaymentAmount = 0,
                                            PostType = enPostType.Invoice_12_5_WithAddress

                                        };
                                        unitOfWork.PostedTransactions.Insert(trans);
                                    }
                                    else
                                    {
                                        trans.ItemCode = con.ItemCode;
                                        trans.GrossAmount = con.Amount;
                                        trans.ConsultantPrice = con.ConsultantPrice;
                                        trans.NetAmount = itemNetAmt;
                                        trans.VatAmount = itemTax + (con.ConsultantPrice - (itemNetAmt + itemTax));
                                        trans.ItemCode = con.ItemCode;
                                        trans.ItemName = con.ItemName;
                                        trans.OrderId = con.OrderId;
                                        trans.OrderQty = con.OrdQty;
                                        trans.Status = con.Status;

                                        unitOfWork.PostedTransactions.Update(trans);

                                    }

                                    //}
                                }

                            }
                            #region VAT_5%
                            if (uCon.ConsultantOrders.Count(x => x.ItemName.StartsWith("PPP")) > 0) // 5 % VAT Items
                            {
                                consultantAmount = uCon.ConsultantOrders.Where(x => x.ItemName.StartsWith("PPP")).Sum(x => x.ConsultantPrice * x.OrdQty);
                                netAmount = ((consultantAmount / Decimal.Parse("105") * Decimal.Parse("100")));
                                //tax = ((netAmount * Decimal.Parse("5")) / Decimal.Parse("100"));
                                netAmount = Math.Round(netAmount, 2);
                                tax = consultantAmount - netAmount;

                                //tax = TruncateDecimal(tax, 2);
                                chkAmt = (consultantAmount - (netAmount + tax));
                                //if (chkAmt <= 1 || chkAmt >= -1)
                                //{
                                //    tax = tax + chkAmt;
                                //}
                                objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("LEDGERENTRIES.LIST");
                                objXmlWriter3.WriteStartElement("OLDAUDITENTRYIDS.LIST"); objXmlWriter3.WriteAttributeString("TYPE", "Number"); objXmlWriter3.WriteElementString("OLDAUDITENTRYIDS", "-1");
                                objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("BASICRATEOFINVOICETAX.LIST"); objXmlWriter3.WriteAttributeString("TYPE", "Number");
                                objXmlWriter3.WriteElementString("BASICRATEOFINVOICETAX", "5"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteElementString("TAXCLASSIFICATIONNAME", "Output VAT @ 5%");
                                objXmlWriter3.WriteElementString("ROUNDTYPE", "Normal Rounding"); objXmlWriter3.WriteElementString("LEDGERNAME", "Output Vat @5%"); objXmlWriter3.WriteStartElement("GSTCLASS"); objXmlWriter3.WriteEndElement();
                                objXmlWriter3.WriteElementString("ISDEEMEDPOSITIVE", "No"); objXmlWriter3.WriteElementString("LEDGERFROMITEM", "No"); objXmlWriter3.WriteElementString("REMOVEZEROENTRIES", "No");
                                objXmlWriter3.WriteElementString("ISPARTYLEDGER", "No"); objXmlWriter3.WriteElementString("ISLASTDEEMEDPOSITIVE", "No"); objXmlWriter3.WriteElementString("AMOUNT", tax.ToString());
                                objXmlWriter3.WriteElementString("VATASSESSABLEVALUE", netAmount.ToString()); objXmlWriter3.WriteStartElement("BANKALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                objXmlWriter3.WriteStartElement("BILLALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("INTERESTCOLLECTION.LIST"); objXmlWriter3.WriteEndElement();
                                objXmlWriter3.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                                objXmlWriter3.WriteStartElement("AUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TAXBILLALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                objXmlWriter3.WriteStartElement("TAXOBJECTALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteElementString("CATEGORY", "Output VAT @ 5%");
                                objXmlWriter3.WriteElementString("TAXTYPE", "VAT"); objXmlWriter3.WriteElementString("TAXNAME", VoucherNo); objXmlWriter3.WriteElementString("PARTYLEDGER", consultant);
                                objXmlWriter3.WriteElementString("REFTYPE", "New Ref"); objXmlWriter3.WriteElementString("ISOPTIONAL", "No"); objXmlWriter3.WriteElementString("ISPANVALID", "No");
                                objXmlWriter3.WriteElementString("ZERORATED", "No"); objXmlWriter3.WriteElementString("EXEMPTED", "No"); objXmlWriter3.WriteElementString("ISSPECIALRATE", "No");
                                objXmlWriter3.WriteElementString("ISDEDUCTNOW", "No"); objXmlWriter3.WriteElementString("ISPANNOTAVAILABLE", "No"); objXmlWriter3.WriteElementString("ISSUPPLEMENTARY", "No");
                                objXmlWriter3.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                                objXmlWriter3.WriteStartElement("AUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                               

                                var PercentItems = uCon.ConsultantOrders.Where(x => x.ItemName.StartsWith("PPP")).ToList();
                                trackNetAmtTotal = 0;
                                count = PercentItems.Count();
                                q = 0;
                                foreach (var con in PercentItems)
                                {
                                    decimal itemTotalVal = con.ConsultantPrice * con.OrdQty;
                                    decimal itemRate = itemTotalVal / con.OrdQty;
                                    decimal itemNetAmt = (con.ConsultantPrice / Decimal.Parse("105") * Decimal.Parse("100"));
                                    itemNetAmt = Math.Round(itemNetAmt, 2);
                                    //decimal itemTax = (itemNetAmt * Decimal.Parse("5") / Decimal.Parse("100"));
                                    decimal itemTax = con.ConsultantPrice - itemNetAmt;
                                    itemRate = itemRate - itemTax;

                                    decimal itemsTotalAmt = itemRate * con.OrdQty;
                                    decimal itemTotalTax = itemTax * con.OrdQty;
                                    decimal itemTotalNetAmt = itemNetAmt * con.OrdQty;

                                    //----------------------------------------------------
                                    //INCLUDED TO TRACK NET AMT TOTALS AFFECTING ROUNDING
                                    //-----------------------------------------------------
                                    trackNetAmtTotal += itemTotalNetAmt;


                                    if (q == count - 1) // last item in consultant orders
                                    {

                                        if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
                                        {
                                            checkedAmt = netAmount - trackNetAmtTotal;

                                            itemsTotalAmt = itemsTotalAmt + checkedAmt;
                                            itemTotalTax = itemTotalTax - checkedAmt;

                                        }

                                    }
                                    objXmlWriter3.WriteStartElement("SUBCATEGORYALLOCATION.LIST"); objXmlWriter3.WriteElementString("STOCKITEMNAME", con.ItemName + "-" + con.ItemCode);
                                    objXmlWriter3.WriteElementString("SUBCATEGORY", "VAT"); objXmlWriter3.WriteElementString("DUTYLEDGER", "5% Vat on Sales");
                                    objXmlWriter3.WriteElementString("SUBCATZERORATED", "No"); objXmlWriter3.WriteElementString("SUBCATEXEMPTED", "No");
                                    objXmlWriter3.WriteElementString("SUBCATISSPECIALRATE", "No"); objXmlWriter3.WriteElementString("TAXRATE", "5");
                                    objXmlWriter3.WriteElementString("ASSESSABLEAMOUNT", itemsTotalAmt.ToString()); objXmlWriter3.WriteElementString("TAX", itemTotalTax.ToString());
                                    objXmlWriter3.WriteElementString("BILLEDQTY", con.OrdQty.ToString()); objXmlWriter3.WriteEndElement();
                                   
                                    q++;
                                }
                                objXmlWriter3.WriteStartElement("TAXOBJECTALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                objXmlWriter3.WriteStartElement("TDSEXPENSEALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                objXmlWriter3.WriteStartElement("VATSTATUTORYDETAILS.LIST"); objXmlWriter3.WriteEndElement();
                                objXmlWriter3.WriteStartElement("COSTTRACKALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                objXmlWriter3.WriteEndElement();
                                
                                q = 0;
                                trackNetAmtTotal = 0;
                                foreach (var con in PercentItems)
                                {
                                    decimal itemTotalVal = con.ConsultantPrice * con.OrdQty;
                                    decimal itemRate = itemTotalVal / con.OrdQty;
                                    decimal itemNetAmt = (con.ConsultantPrice / Decimal.Parse("105") * Decimal.Parse("100"));
                                    itemNetAmt = Math.Round(itemNetAmt, 2);
                                    //decimal itemTax = (itemNetAmt * Decimal.Parse("5") / Decimal.Parse("100"));
                                    decimal itemTax = con.ConsultantPrice - itemNetAmt;
                                    itemRate = itemRate - itemTax;

                                    decimal itemsTotalAmt = itemRate * con.OrdQty;
                                    decimal itemTotalTax = itemTax * con.OrdQty;
                                    decimal itemTotalNetAmt = itemNetAmt * con.OrdQty;

                                    //itemNetAmt = Math.Round(itemNetAmt, 0);

                                    //----------------------------------------------------
                                    //INCLUDED TO TRACK NET AMT TOTALS AFFECTING ROUNDING
                                    //-----------------------------------------------------
                                    trackNetAmtTotal += itemTotalNetAmt;


                                    if (q == count - 1) // last item in consultant orders
                                    {

                                        if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
                                        {
                                            checkedAmt = netAmount - trackNetAmtTotal;

                                            itemsTotalAmt = itemsTotalAmt + checkedAmt;
                                            itemTotalTax = itemTotalTax - checkedAmt;
                                            itemRate = itemsTotalAmt / con.OrdQty;
                                        }

                                    }

                                    objXmlWriter3.WriteStartElement("ALLINVENTORYENTRIES.LIST"); objXmlWriter3.WriteElementString("STOCKITEMNAME", con.ItemName + "-" + con.ItemCode);
                                    objXmlWriter3.WriteElementString("ISDEEMEDPOSITIVE", "No"); objXmlWriter3.WriteElementString("ISLASTDEEMEDPOSITIVE", "No");
                                    objXmlWriter3.WriteElementString("ISAUTONEGATE", "No"); objXmlWriter3.WriteElementString("ISCUSTOMSCLEARANCE", "No");
                                    objXmlWriter3.WriteElementString("ISTRACKCOMPONENT", "No"); objXmlWriter3.WriteElementString("ISTRACKPRODUCTION", "No");
                                    objXmlWriter3.WriteElementString("ISPRIMARYITEM", "No"); objXmlWriter3.WriteElementString("ISSCRAP", "No");
                                    objXmlWriter3.WriteElementString("RATE", itemRate.ToString() + "/NO"); objXmlWriter3.WriteElementString("AMOUNT", itemsTotalAmt.ToString());
                                    objXmlWriter3.WriteElementString("ACTUALQTY", con.OrdQty.ToString()); objXmlWriter3.WriteElementString("BILLEDQTY", con.OrdQty.ToString());
                                    objXmlWriter3.WriteStartElement("BATCHALLOCATIONS.LIST"); objXmlWriter3.WriteElementString("GODOWNNAME", "Main Location");
                                    objXmlWriter3.WriteElementString("BATCHNAME", "Primary Batch"); objXmlWriter3.WriteStartElement("INDENTNO"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("ORDERNO"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TRACKINGNUMBER"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteElementString("DYNAMICCSTISCLEARED", "No"); objXmlWriter3.WriteElementString("AMOUNT", itemsTotalAmt.ToString());
                                    objXmlWriter3.WriteElementString("ACTUALQTY", con.OrdQty.ToString() + " NO"); objXmlWriter3.WriteElementString("BILLEDQTY", con.OrdQty.ToString() + " NO");
                                    objXmlWriter3.WriteStartElement("ADDITIONALDETAILS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("VOUCHERCOMPONENTLIST.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTINGALLOCATIONS.LIST"); objXmlWriter3.WriteStartElement("OLDAUDITENTRYIDS.LIST");
                                    objXmlWriter3.WriteAttributeString("TYPE", "Number"); objXmlWriter3.WriteElementString("OLDAUDITENTRYIDS", "-1"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteElementString("TAXCLASSIFICATIONNAME", "Output VAT @ 5%"); objXmlWriter3.WriteElementString("LEDGERNAME", "Sales @ 5%");
                                    objXmlWriter3.WriteStartElement("GSTCLASS"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteElementString("ISDEEMEDPOSITIVE", "No");
                                    objXmlWriter3.WriteElementString("LEDGERFROMITEM", "No"); objXmlWriter3.WriteElementString("REMOVEZEROENTRIES", "No");
                                    objXmlWriter3.WriteElementString("ISPARTYLEDGER", "No"); objXmlWriter3.WriteElementString("ISLASTDEEMEDPOSITIVE", "No");
                                    objXmlWriter3.WriteElementString("AMOUNT", itemsTotalAmt.ToString()); objXmlWriter3.WriteStartElement("BANKALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("BILLALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("INTERESTCOLLECTION.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("AUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TAXBILLALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("TAXOBJECTALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TDSEXPENSEALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("VATSTATUTORYDETAILS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("COSTTRACKALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("OLDAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("ACCOUNTAUDITENTRIES.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("TAXOBJECTALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteStartElement("EXCISEALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteStartElement("EXPENSEALLOCATIONS.LIST"); objXmlWriter3.WriteEndElement();
                                    objXmlWriter3.WriteEndElement();

                                    q++;
                                    itemNetAmt = XcelServiceHelper.TruncateDecimal(itemNetAmt, 2);
                                    decimal chkItemTax = con.ConsultantPrice - (itemNetAmt + itemTax);
                                    if (chkItemTax <= 1 || chkItemTax >= -1)
                                    {
                                        itemTax = itemTax + chkItemTax;
                                    }
                                    PostedTransaction trans;
                                    trans = unitOfWork.PostedTransactions.All()
                                                 .Where(x => x.ItemCode == con.ItemCode)
                                                 .Where(x => x.OrderId == con.OrderId)
                                                 .Where(x => x.ConsultantCode == con.Consultant.ConsultantId)
                                                 .FirstOrDefault();

                                    if (trans == null)
                                    {
                                        trans = new PostedTransaction
                                        {
                                            Company = model.Company,
                                            UnitName = unit.Unit,
                                            AssemblyName = assembly,
                                            ConsultantName = uCon.Consultant,
                                            ConsultantCode = uCon.ConsultantId,
                                            PostDate = DateTime.Now,
                                            Year = year,
                                            Week = week,
                                            InvoiceNo = VoucherNo,
                                            GrossAmount = con.Amount,
                                            ConsultantPrice = con.ConsultantPrice,
                                            NetAmount = itemNetAmt,
                                            VatAmount = itemTax,
                                            ItemCode = con.ItemCode,
                                            ItemName = con.ItemName,
                                            OrderId = con.OrderId,
                                            OrderQty = con.OrdQty,
                                            Status = con.Status,
                                            PayStatus = enPaymentStatus.Pending,
                                            InventoryStatus = enInventoryStatus.Pending,
                                            PaymentAmount = 0,
                                            PostType = enPostType.Invoice_12_5_WithAddress

                                        };
                                        unitOfWork.PostedTransactions.Insert(trans);
                                    }
                                    else
                                    {
                                        trans.ItemCode = con.ItemCode;
                                        trans.GrossAmount = con.Amount;
                                        trans.ConsultantPrice = con.ConsultantPrice;
                                        trans.NetAmount = itemNetAmt;
                                        trans.VatAmount = itemTax + (con.ConsultantPrice - (itemNetAmt + itemTax));
                                        trans.ItemCode = con.ItemCode;
                                        trans.ItemName = con.ItemName;
                                        trans.OrderId = con.OrderId;
                                        trans.OrderQty = con.OrdQty;
                                        trans.Status = con.Status;

                                        unitOfWork.PostedTransactions.Update(trans);
                                    }
                                }

                            }
                            #endregion VAT_5%
                            objXmlWriter3.WriteStartElement("ATTDRECORDS.LIST"); objXmlWriter3.WriteEndElement();
                            objXmlWriter3.WriteEndElement(); objXmlWriter3.WriteEndElement();
                            //strData = strData + "<ATTDRECORDS.LIST>      </ATTDRECORDS.LIST>" +
                            //"</VOUCHER></TALLYMESSAGE>";
                            objXmlWriter3.Flush();
                        }
                        //objXmlWriter3.Dispose();
                    }
                    unitOfWork.Commit();
                    unitOfWork.Dispose();
                }

                
                string closeTag = "</REQUESTDATA></IMPORTDATA></BODY></ENVELOPE>";
                var fs = System.IO.File.Open(savePath + "//payment.xml", System.IO.FileMode.Open,System.IO.FileAccess.ReadWrite);
                fs.Position = fs.Length; 
                var writer = new StreamWriter(fs);  // after the Position is set
                writer.Write(closeTag);  // NOT WriteLine !!
                writer.Close();
                fs.Close();
               
            }
        }
    }
}