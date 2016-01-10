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
    public class ConvertPurchaseRegisterToXmlService : IConvertToXmlService 
    {
       
        public void ProcessExcelSheet(UploadFileModel model, string fileName, string savePath)
        {
            try
            {
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
                {
                    WorkbookPart workBk = doc.WorkbookPart;
                    WorksheetPart workSht = workBk.WorksheetParts.First();
                    SheetData shtData = workSht.Worksheet.Elements<SheetData>().First();

                    Sheet theSheet = workBk.Workbook.Descendants<Sheet>().
                                    Where(s => s.SheetId == 1).FirstOrDefault();

                    var strData = "<ENVELOPE><HEADER><TALLYREQUEST>Import Data</TALLYREQUEST></HEADER><BODY><IMPORTDATA>" +
                            "<REQUESTDESC> <REPORTNAME>All Masters</REPORTNAME><STATICVARIABLES><SVCURRENTCOMPANY>" + model.Company +
                            "</SVCURRENTCOMPANY></STATICVARIABLES></REQUESTDESC><REQUESTDATA>";

                    string cellDbShip; string cellFrmDate; string cellToDate;
                    string cellA; string cellC; string cellD; string cellG;
                    string cellI; string cellJ; string cellL; string cellN;
                    string cellQ; string cellS; string cellU; string cellV;

                    int rows = 0;
                    var itemCode = ""; var itemName = ""; var pmtInstrument = ""; var ordYrWk = "";
                    var invNo = ""; var invType = ""; var invDate = ""; var invYrWk = "";
                    decimal qty; decimal priceWOTax; decimal VAT; decimal priceInclVAT;
                    var item = "";
                    List<StockItem> stockItems = new List<StockItem>();
                    List<Invoice> invoices = new List<Invoice>();
                    List<InvoiceItem> invoiceItems = new List<InvoiceItem>();

                    cellDbShip = XcelServiceHelper.GetCellValue(workBk,theSheet, "B4");
                    cellFrmDate = XcelServiceHelper.GetCellValue(workBk, theSheet, "M4");
                    cellToDate = XcelServiceHelper.GetCellValue(workBk, theSheet, "R4");

                    Invoice inv;
                    InvoiceItem invItem;

                    int rowCount = shtData.Elements<Row>().Count();
                    rowCount = rowCount + 1;

                    for (rows = 1; rows <= rowCount; rows++)
                    {
                        cellD = "D" + rows;
                        if (rows >= 7)
                        {
                            string theCell = XcelServiceHelper.GetCellValue(workBk, theSheet, cellD);
                            if (!string.IsNullOrEmpty(theCell))
                            {
                                cellA = "A" + rows; //itemcode
                                cellC = "C" + rows; // Pmt instrument
                                cellD = "D" + rows; // item name
                                cellG = "G" + rows; // ord year wk
                                cellI = "I" + rows; // inv no
                                cellJ = "J" + rows; //inv type
                                cellL = "L" + rows; //inv date
                                cellN = "N" + rows; //inv yr wk
                                cellQ = "Q" + rows; //qty
                                cellS = "S" + rows; //price w/out tax
                                cellU = "U" + rows; //VAT
                                cellV = "V" + rows; //price incl. of vat
                                string ratePercVal = "";

                                itemCode = XcelServiceHelper.GetCellValue(workBk, theSheet, cellA);
                                itemName = XcelServiceHelper.GetCellValue(workBk, theSheet, cellD);
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

                                    strData = strData + @"<TALLYMESSAGE xmlns:UDF=""TallyUDF"">" +
                                       @"<STOCKITEM NAME="""
                                       + item +
                                       @""" RESERVEDNAME="""">" +
                                       @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
                                       "<PARENT>Tupperware Products</PARENT><CATEGORY/><TAXCLASSIFICATIONNAME/><COSTINGMETHOD>Avg. Cost</COSTINGMETHOD>" +
                                       "<VALUATIONMETHOD>Avg. Price</VALUATIONMETHOD><BASEUNITS>NO</BASEUNITS><ADDITIONALUNITS/><EXCISEITEMCLASSIFICATION/>" +
                                       "<ISCOSTCENTRESON>No</ISCOSTCENTRESON><ISBATCHWISEON>No</ISBATCHWISEON><ISPERISHABLEON>No</ISPERISHABLEON><ISENTRYTAXAPPLICABLE>No</ISENTRYTAXAPPLICABLE>" +
                                       "<ISCOSTTRACKINGON>No</ISCOSTTRACKINGON><IGNOREPHYSICALDIFFERENCE>No</IGNOREPHYSICALDIFFERENCE><IGNORENEGATIVESTOCK>No</IGNORENEGATIVESTOCK>" +
                                       "<TREATSALESASMANUFACTURED>No</TREATSALESASMANUFACTURED><TREATPURCHASESASCONSUMED>No</TREATPURCHASESASCONSUMED><TREATREJECTSASSCRAP>No</TREATREJECTSASSCRAP>" +
                                       "<HASMFGDATE>No</HASMFGDATE><ALLOWUSEOFEXPIREDITEMS>No</ALLOWUSEOFEXPIREDITEMS><IGNOREBATCHES>No</IGNOREBATCHES><IGNOREGODOWNS>No</IGNOREGODOWNS>" +
                                       "<CALCONMRP>No</CALCONMRP><EXCLUDEJRNLFORVALUATION>No</EXCLUDEJRNLFORVALUATION><ISMRPINCLOFTAX>No</ISMRPINCLOFTAX><ISADDLTAXEXEMPT>No</ISADDLTAXEXEMPT>" +
                                       "<ISSUPPLEMENTRYDUTYON>No</ISSUPPLEMENTRYDUTYON><REORDERASHIGHER>No</REORDERASHIGHER><MINORDERASHIGHER>No</MINORDERASHIGHER><DENOMINATOR> 1</DENOMINATOR>" +
                                       @"<RATEOFVAT>" + ratePercVal + "</RATEOFVAT><LANGUAGENAME.LIST>" +
                                       @"<NAME.LIST TYPE=""String""><NAME>" + item + "</NAME></NAME.LIST><LANGUAGEID> 1033</LANGUAGEID>" +
                                       "</LANGUAGENAME.LIST><SCHVIDETAILS.LIST>      </SCHVIDETAILS.LIST><OLDAUDITENTRIES.LIST>      </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>      </ACCOUNTAUDITENTRIES.LIST>" +
                                       "<AUDITENTRIES.LIST>      </AUDITENTRIES.LIST><COMPONENTLIST.LIST>      </COMPONENTLIST.LIST><ADDITIONALLEDGERS.LIST>      </ADDITIONALLEDGERS.LIST>" +
                                       "<SALESLIST.LIST>      </SALESLIST.LIST><PURCHASELIST.LIST>      </PURCHASELIST.LIST><FULLPRICELIST.LIST>      </FULLPRICELIST.LIST>" +
                                       "<BATCHALLOCATIONS.LIST>      </BATCHALLOCATIONS.LIST><TRADEREXCISEDUTIES.LIST>      </TRADEREXCISEDUTIES.LIST><STANDARDCOSTLIST.LIST>      </STANDARDCOSTLIST.LIST>" +
                                       "<STANDARDPRICELIST.LIST>      </STANDARDPRICELIST.LIST><EXCISEITEMGODOWN.LIST>      </EXCISEITEMGODOWN.LIST><MULTICOMPONENTLIST.LIST>      </MULTICOMPONENTLIST.LIST>" +
                                       "<PRICELEVELLIST.LIST>      </PRICELEVELLIST.LIST></STOCKITEM></TALLYMESSAGE>";
                                    s = new StockItem
                                    {
                                        ItemCode = itemCode.Trim().ToString(),
                                        ItemName = itemName.Trim().ToString()
                                    };
                                    stockItems.Add(s);
                                }

                                pmtInstrument = XcelServiceHelper.GetCellValue(workBk, theSheet, cellC);
                                ordYrWk = XcelServiceHelper.GetCellValue(workBk, theSheet, cellG);
                                invYrWk = XcelServiceHelper.GetCellValue(workBk, theSheet, cellN);
                                invNo = XcelServiceHelper.GetCellValue(workBk, theSheet, cellI);
                                invDate = XcelServiceHelper.GetCellValue(workBk, theSheet, cellL);
                                invType = XcelServiceHelper.GetCellValue(workBk, theSheet, cellJ);

                                qty = Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellQ));
                                priceWOTax = Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellS));
                                VAT = Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellU));
                                priceInclVAT= Decimal.Parse(XcelServiceHelper.GetCellValue(workBk, theSheet, cellV));

                                inv = invoices.FirstOrDefault(x => x.InvoiceNo == invNo.Trim());
                               
                                if (inv == null)
                                {
                                    inv = new Invoice
                                    {
                                        InvoiceNo = invNo.Trim(),
                                        InvoiceDate = DateTime.Parse(invDate),
                                        InvoiceType = invType.Trim(),
                                        InvYearWk = invYrWk.Trim(),
                                        OrdYearWk = ordYrWk.Trim(),
                                        VoucherNo = invType.Trim() + "-" + invNo.Trim()
                                    };
                                    invoices.Add(inv);
                                }

                                invItem = inv.Items.FirstOrDefault(x => x.ItemCode == itemCode.Trim() && x.PaymentInstrument == pmtInstrument.Trim());
                                if (invItem == null)
                                {
                                    invItem = new InvoiceItem
                                    {
                                        InvoiceNo = invNo.Trim(),
                                        ItemCode = itemCode.Trim(),
                                        ItemName = itemName.Trim(),
                                        PaymentInstrument = pmtInstrument.Trim(),
                                        Quantity = qty,
                                        PriceWOTax = priceWOTax,
                                        VAT = VAT,
                                        PriceInclVAT = priceInclVAT
                                    };
                                    inv.Items.Add(invItem);
                                }
                            }
                        }
                    }
                    foreach (var invoice in invoices)
                    {
                        decimal invTotalAmt = invoice.Items.Sum(x => x.PriceInclVAT);

                        decimal invTotalAmt_12_5 = invoice.Items.Where(x => !x.ItemName.StartsWith("PPP")).Sum(x => x.PriceWOTax);
                        decimal invTotalVAT_12_5 = invoice.Items.Where(x => !x.ItemName.StartsWith("PPP")).Sum(x => x.VAT);

                        strData = strData + @"<TALLYMESSAGE xmlns:UDF=""TallyUDF"">" +
                        @"<VOUCHER VCHTYPE=""Purchase"" ACTION=""Create"" OBJVIEW=""Invoice Voucher View"">" +
                        @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
                        "<DATE>" + invoice.InvoiceDate + "</DATE>" +
                        "<GUID></GUID><PARTYNAME>Tupperware India Pvt Ltd</PARTYNAME><VOUCHERTYPENAME>Purchase</VOUCHERTYPENAME>" +
                        "<VOUCHERNUMBER>" + invoice.VoucherNo + "</VOUCHERNUMBER><PARTYLEDGERNAME>TUPPERWARE INDIA PVT LTD</PARTYLEDGERNAME>" +
                        "<BASICBASEPARTYNAME>Tupperware India Pvt Ltd</BASICBASEPARTYNAME><CSTFORMISSUETYPE/><CSTFORMRECVTYPE/>" +
                        "<FBTPAYMENTTYPE>Default</FBTPAYMENTTYPE><PERSISTEDVIEW>Invoice Voucher View</PERSISTEDVIEW>" +
                        "<VCHGSTCLASS/><VOUCHERTYPEORIGNAME>Purchase</VOUCHERTYPEORIGNAME><DIFFACTUALQTY>No</DIFFACTUALQTY>" +
                        "<ISMSTFROMSYNC>No</ISMSTFROMSYNC><ASORIGINAL>No</ASORIGINAL><AUDITED>No</AUDITED><FORJOBCOSTING>No</FORJOBCOSTING>" +
                        "<ISOPTIONAL>No</ISOPTIONAL><EFFECTIVEDATE>" + invoice.InvoiceDate + "</EFFECTIVEDATE><USEFOREXCISE>No</USEFOREXCISE>" +
                        "<ISFORJOBWORKIN>No</ISFORJOBWORKIN><ALLOWCONSUMPTION>No</ALLOWCONSUMPTION><USEFORINTEREST>No</USEFORINTEREST>" +
                        "<USEFORGAINLOSS>No</USEFORGAINLOSS><USEFORGODOWNTRANSFER>No</USEFORGODOWNTRANSFER><USEFORCOMPOUND>No</USEFORCOMPOUND>" +
                        "<USEFORSERVICETAX>No</USEFORSERVICETAX><ISEXCISEVOUCHER>No</ISEXCISEVOUCHER><EXCISETAXOVERRIDE>No</EXCISETAXOVERRIDE>" +
                        "<USEFORFINALPRODUCTION>No</USEFORFINALPRODUCTION><ISTDSOVERRIDDEN>No</ISTDSOVERRIDDEN><ISTCSOVERRIDDEN>No</ISTCSOVERRIDDEN>" +
                        "<ISTDSTCSCASHVCH>No</ISTDSTCSCASHVCH><INCLUDEADVPYMTVCH>No</INCLUDEADVPYMTVCH><ISSUBWORKSCONTRACT>No</ISSUBWORKSCONTRACT>" +
                        "<ISVATOVERRIDDEN>No</ISVATOVERRIDDEN><IGNOREORIGVCHDATE>No</IGNOREORIGVCHDATE><ISSERVICETAXOVERRIDDEN>No</ISSERVICETAXOVERRIDDEN>" +
                        "<ISISDVOUCHER>No</ISISDVOUCHER><ISEXCISEOVERRIDDEN>No</ISEXCISEOVERRIDDEN><ISEXCISESUPPLYVCH>No</ISEXCISESUPPLYVCH>" +
                        "<ISCANCELLED>No</ISCANCELLED><HASCASHFLOW>No</HASCASHFLOW><ISPOSTDATED>No</ISPOSTDATED><USETRACKINGNUMBER>No</USETRACKINGNUMBER>" +
                        "<ISINVOICE>Yes</ISINVOICE><MFGJOURNAL>No</MFGJOURNAL><HASDISCOUNTS>No</HASDISCOUNTS><ASPAYSLIP>No</ASPAYSLIP>" +
                        "<ISCOSTCENTRE>No</ISCOSTCENTRE><ISSTXNONREALIZEDVCH>No</ISSTXNONREALIZEDVCH><ISEXCISEMANUFACTURERON>Yes</ISEXCISEMANUFACTURERON>" +
                        "<ISBLANKCHEQUE>No</ISBLANKCHEQUE><ISVOID>No</ISVOID><ISONHOLD>No</ISONHOLD><ORDERLINESTATUS>No</ORDERLINESTATUS>" +
                        "<ISDELETED>No</ISDELETED><CHANGEVCHMODE>No</CHANGEVCHMODE><ALTERID></ALTERID><MASTERID></MASTERID><VOUCHERKEY></VOUCHERKEY>" +
                        "<EXCLUDEDTAXATIONS.LIST>      </EXCLUDEDTAXATIONS.LIST><OLDAUDITENTRIES.LIST>      </OLDAUDITENTRIES.LIST>" +
                        "<ACCOUNTAUDITENTRIES.LIST>      </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>      </AUDITENTRIES.LIST>" +
                        "<DUTYHEADDETAILS.LIST>      </DUTYHEADDETAILS.LIST><SUPPLEMENTARYDUTYHEADDETAILS.LIST>      </SUPPLEMENTARYDUTYHEADDETAILS.LIST>" +
                        "<INVOICEDELNOTES.LIST>      </INVOICEDELNOTES.LIST><INVOICEORDERLIST.LIST>      </INVOICEORDERLIST.LIST><INVOICEINDENTLIST.LIST>      </INVOICEINDENTLIST.LIST>" +
                        "<ATTENDANCEENTRIES.LIST>      </ATTENDANCEENTRIES.LIST><ORIGINVOICEDETAILS.LIST>      </ORIGINVOICEDETAILS.LIST>" +
                        "<INVOICEEXPORTLIST.LIST>      </INVOICEEXPORTLIST.LIST><LEDGERENTRIES.LIST>" +
                        @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
                        "<LEDGERNAME>TUPPERWARE INDIA PVT LTD</LEDGERNAME><GSTCLASS/><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>" +
                        "<LEDGERFROMITEM>No</LEDGERFROMITEM><REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>Yes</ISPARTYLEDGER>" +
                        "<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><AMOUNT>" + invTotalAmt + "</AMOUNT><SERVICETAXDETAILS.LIST>       </SERVICETAXDETAILS.LIST>" +
                        "<BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST><BILLALLOCATIONS.LIST><NAME>" + invoice.VoucherNo + "</NAME><BILLTYPE>New Ref</BILLTYPE>" +
                        "<TDSDEDUCTEEISSPECIALRATE>No</TDSDEDUCTEEISSPECIALRATE><AMOUNT>" + invTotalAmt + "</AMOUNT><INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST>" +
                        "<STBILLCATEGORIES.LIST>        </STBILLCATEGORIES.LIST></BILLALLOCATIONS.LIST><INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST>" +
                        "<OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>" +
                        "<AUDITENTRIES.LIST>       </AUDITENTRIES.LIST><INPUTCRALLOCS.LIST>       </INPUTCRALLOCS.LIST><DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>" +
                        "<EXCISEDUTYHEADDETAILS.LIST>       </EXCISEDUTYHEADDETAILS.LIST><SUMMARYALLOCS.LIST>       </SUMMARYALLOCS.LIST><STPYMTDETAILS.LIST>       </STPYMTDETAILS.LIST>" +
                        "<EXCISEPAYMENTALLOCATIONS.LIST>       </EXCISEPAYMENTALLOCATIONS.LIST><TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>" +
                        "<TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST><VATSTATUTORYDETAILS.LIST>Yes</VATSTATUTORYDETAILS.LIST><COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>" +
                        "<REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST><INVOICEWISEDETAILS.LIST>       </INVOICEWISEDETAILS.LIST></LEDGERENTRIES.LIST>" +
                        @"<LEDGERENTRIES.LIST><OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
                        @"<BASICRATEOFINVOICETAX.LIST TYPE=""Number""><BASICRATEOFINVOICETAX> 12.50</BASICRATEOFINVOICETAX></BASICRATEOFINVOICETAX.LIST>" +
                        "<TAXCLASSIFICATIONNAME>Input VAT @ 12.5%</TAXCLASSIFICATIONNAME><LEDGERNAME>Input Vat @12.5 %</LEDGERNAME><GSTCLASS/>" +
                        "<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM><REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>No</ISPARTYLEDGER>" +
                        "<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE><AMOUNT>-" + invTotalVAT_12_5 + " </AMOUNT><VATASSESSABLEVALUE>-" + invTotalAmt_12_5 + "</VATASSESSABLEVALUE>" +
                        "<SERVICETAXDETAILS.LIST>       </SERVICETAXDETAILS.LIST><BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST><BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>" +
                        "<INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>" +
                        "<AUDITENTRIES.LIST>       </AUDITENTRIES.LIST><INPUTCRALLOCS.LIST>       </INPUTCRALLOCS.LIST><DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>" +
                        "<EXCISEDUTYHEADDETAILS.LIST>       </EXCISEDUTYHEADDETAILS.LIST><SUMMARYALLOCS.LIST>       </SUMMARYALLOCS.LIST><STPYMTDETAILS.LIST>       </STPYMTDETAILS.LIST>" +
                        "<EXCISEPAYMENTALLOCATIONS.LIST>       </EXCISEPAYMENTALLOCATIONS.LIST><TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST>" +
                        "<CATEGORY>Input VAT @ 12.5%</CATEGORY><TAXTYPE>VAT</TAXTYPE><TAXNAME>123456</TAXNAME><PARTYLEDGER>TUPPERWARE INDIA PVT LTD</PARTYLEDGER><REFTYPE>Agst Ref</REFTYPE>" +
                        "<ISOPTIONAL>No</ISOPTIONAL><ISPANVALID>No</ISPANVALID><ZERORATED>No</ZERORATED><EXEMPTED>No</EXEMPTED><ISSPECIALRATE>No</ISSPECIALRATE><ISDEDUCTNOW>No</ISDEDUCTNOW>" +
                        "<ISPANNOTAVAILABLE>No</ISPANNOTAVAILABLE><ISSUPPLEMENTARY>No</ISSUPPLEMENTARY><HASINPUTCREDIT>No</HASINPUTCREDIT><OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>" +
                        "<ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>";
#region 12.5% VAT- Subcategory List
                        var VAT_12_5_Items = invoice.Items.Where(x => !x.ItemName.StartsWith("PPP")).ToList();
                        foreach (var itm in VAT_12_5_Items)
                        {
                            strData = strData + "<SUBCATEGORYALLOCATION.LIST>" +
                            "<STOCKITEMNAME>" + itm.ItemName + "-" + itm.ItemCode + "</STOCKITEMNAME>" +
                            "<SUBCATEGORY>VAT</SUBCATEGORY>" +
                            "<DUTYLEDGER>Input Vat @12.5 %</DUTYLEDGER>" +
                            "<SUBCATZERORATED>No</SUBCATZERORATED>" +
                            "<SUBCATEXEMPTED>No</SUBCATEXEMPTED>" +
                            "<SUBCATISSPECIALRATE>No</SUBCATISSPECIALRATE>" +
                            "<TAXRATE> 12.50</TAXRATE>" +
                            "<ASSESSABLEAMOUNT>-" + itm.PriceWOTax + "</ASSESSABLEAMOUNT>" +
                            "<TAX>-" + itm.PriceWOTax + "</TAX>" +
                            "<BILLEDQTY> " + itm.Quantity + " no</BILLEDQTY>" +
                            "</SUBCATEGORYALLOCATION.LIST>";
                        }

#endregion
                        strData = strData + "</TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>" +
                        "<VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST><COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>" +
                        "<REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST><INVOICEWISEDETAILS.LIST>       </INVOICEWISEDETAILS.LIST>" +
                        "</LEDGERENTRIES.LIST>";
#region 5% VAT- Subcategory List
                        var VAT_5_Items = invoice.Items.Where(x => x.ItemName.StartsWith("PPP")).ToList();
                        if (VAT_5_Items.Count > 0)
                        {
                            decimal invTotalAmt_5 = invoice.Items.Where(x => x.ItemName.StartsWith("PPP")).Sum(x => x.PriceWOTax);
                            decimal invTotalVAT_5 = invoice.Items.Where(x => x.ItemName.StartsWith("PPP")).Sum(x => x.VAT);

                            strData = strData + "<LEDGERENTRIES.LIST>" +
                            @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
                            @"<BASICRATEOFINVOICETAX.LIST TYPE=""Number""><BASICRATEOFINVOICETAX> 5</BASICRATEOFINVOICETAX></BASICRATEOFINVOICETAX.LIST>" +
                            "<TAXCLASSIFICATIONNAME>Input VAT @ 5%</TAXCLASSIFICATIONNAME><LEDGERNAME>Input Vat @5 %</LEDGERNAME><GSTCLASS/>" +
                            "<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM><REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
                            "<ISPARTYLEDGER>No</ISPARTYLEDGER><ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE><AMOUNT>-" + invTotalVAT_5 + "</AMOUNT>" +
                            "<VATASSESSABLEVALUE>-" + invTotalAmt_5 + "</VATASSESSABLEVALUE><SERVICETAXDETAILS.LIST>       </SERVICETAXDETAILS.LIST>" +
                            "<BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST><BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>" +
                            "<INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST>" +
                            "<ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>       </AUDITENTRIES.LIST>" +
                            "<INPUTCRALLOCS.LIST>       </INPUTCRALLOCS.LIST><DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>" +
                            "<EXCISEDUTYHEADDETAILS.LIST>       </EXCISEDUTYHEADDETAILS.LIST><SUMMARYALLOCS.LIST>       </SUMMARYALLOCS.LIST>" +
                            "<STPYMTDETAILS.LIST>       </STPYMTDETAILS.LIST><EXCISEPAYMENTALLOCATIONS.LIST>       </EXCISEPAYMENTALLOCATIONS.LIST>" +
                            "<TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST><CATEGORY>Input VAT @ 5%</CATEGORY>" +
                            "<TAXTYPE>VAT</TAXTYPE><TAXNAME>123456</TAXNAME><PARTYLEDGER>TUPPERWARE INDIA PVT LTD</PARTYLEDGER><REFTYPE>Agst Ref</REFTYPE>" +
                            "<ISOPTIONAL>No</ISOPTIONAL><ISPANVALID>No</ISPANVALID><ZERORATED>No</ZERORATED><EXEMPTED>No</EXEMPTED><ISSPECIALRATE>No</ISSPECIALRATE>" +
                            "<ISDEDUCTNOW>No</ISDEDUCTNOW><ISPANNOTAVAILABLE>No</ISPANNOTAVAILABLE><ISSUPPLEMENTARY>No</ISSUPPLEMENTARY><ISPUREAGENT>No</ISPUREAGENT>" +
                            "<HASINPUTCREDIT>No</HASINPUTCREDIT><OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST>" +
                            "<AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>";

                            foreach (var itm in VAT_5_Items)
                            {
                                strData = strData + "<SUBCATEGORYALLOCATION.LIST>" +
                               "<STOCKITEMNAME>" + itm.ItemName + "-" + itm.ItemCode + "</STOCKITEMNAME>" +
                               "<SUBCATEGORY>VAT</SUBCATEGORY>" +
                               "<DUTYLEDGER>Input Vat @12.5 %</DUTYLEDGER>" +
                               "<SUBCATZERORATED>No</SUBCATZERORATED>" +
                               "<SUBCATEXEMPTED>No</SUBCATEXEMPTED>" +
                               "<SUBCATISSPECIALRATE>No</SUBCATISSPECIALRATE>" +
                               "<TAXRATE> 12.50</TAXRATE>" +
                               "<ASSESSABLEAMOUNT>-" + itm.PriceWOTax + "</ASSESSABLEAMOUNT>" +
                               "<TAX>-" + itm.PriceWOTax + "</TAX>" +
                               "<BILLEDQTY> " + itm.Quantity + " no</BILLEDQTY>" +
                               "</SUBCATEGORYALLOCATION.LIST>";
                            }
                            strData = strData + "</TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>" +
                            "<VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST><COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>" +
                            "<REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST><INVOICEWISEDETAILS.LIST>       </INVOICEWISEDETAILS.LIST>" +
                            "</LEDGERENTRIES.LIST>";

                        }
#endregion

#region 12.5% VAT AllInventoriesList
                        foreach (var itm in VAT_12_5_Items)
                        {
                            decimal itmRate = XcelServiceHelper.TruncateDecimal (itm.PriceWOTax / itm.Quantity,2);
                            strData = strData + "<ALLINVENTORYENTRIES.LIST>" +
                            "<STOCKITEMNAME>" + itm.ItemName + " - " + itm.ItemCode + "</STOCKITEMNAME><ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
                            "<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE><ISAUTONEGATE>No</ISAUTONEGATE><ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE>" +
                            "<ISTRACKCOMPONENT>No</ISTRACKCOMPONENT><ISTRACKPRODUCTION>No</ISTRACKPRODUCTION><ISPRIMARYITEM>No</ISPRIMARYITEM>" +
                            "<ISSCRAP>No</ISSCRAP><RATE>" + itmRate + "/no</RATE><AMOUNT>-" + itm.PriceWOTax + "</AMOUNT><ACTUALQTY> " + itm.Quantity + " no</ACTUALQTY>" +
                            "<BILLEDQTY> " + itm.Quantity + " no</BILLEDQTY><BATCHALLOCATIONS.LIST><GODOWNNAME>Main Location</GODOWNNAME><BATCHNAME>Primary Batch</BATCHNAME>" +
                            "<INDENTNO/><ORDERNO/><TRACKINGNUMBER/><DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED><AMOUNT>-" + itm.PriceWOTax + "</AMOUNT>" +
                            "<ACTUALQTY> " + itm.Quantity + " no</ACTUALQTY><BILLEDQTY> " + itm.Quantity + " no</BILLEDQTY><ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST>" +
                            "<VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST></BATCHALLOCATIONS.LIST><ACCOUNTINGALLOCATIONS.LIST>" +
                            @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST><TAXCLASSIFICATIONNAME>Input VAT @ 12.5%</TAXCLASSIFICATIONNAME>" +
                            "<LEDGERNAME>Purchase @12.5 %</LEDGERNAME><GSTCLASS/><ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM><REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
                            "<ISPARTYLEDGER>No</ISPARTYLEDGER><ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE><AMOUNT>-" + itm.PriceWOTax + "</AMOUNT><SERVICETAXDETAILS.LIST>        </SERVICETAXDETAILS.LIST>" +
                            "<BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST><BILLALLOCATIONS.LIST>        </BILLALLOCATIONS.LIST><INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST>" +
                            "<OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>" +
                            "<INPUTCRALLOCS.LIST>        </INPUTCRALLOCS.LIST><DUTYHEADDETAILS.LIST>        </DUTYHEADDETAILS.LIST><EXCISEDUTYHEADDETAILS.LIST>        </EXCISEDUTYHEADDETAILS.LIST>" +
                            "<SUMMARYALLOCS.LIST>        </SUMMARYALLOCS.LIST><STPYMTDETAILS.LIST>        </STPYMTDETAILS.LIST><EXCISEPAYMENTALLOCATIONS.LIST>        </EXCISEPAYMENTALLOCATIONS.LIST>" +
                            "<TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST>" +
                            "<VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST><COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST><REFVOUCHERDETAILS.LIST>        </REFVOUCHERDETAILS.LIST>" +
                            "<INVOICEWISEDETAILS.LIST>        </INVOICEWISEDETAILS.LIST></ACCOUNTINGALLOCATIONS.LIST><DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>" +
                            "<SUPPLEMENTARYDUTYHEADDETAILS.LIST>       </SUPPLEMENTARYDUTYHEADDETAILS.LIST><TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>" +
                            "<REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST><EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST><EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST>" +
                            "</ALLINVENTORYENTRIES.LIST>";
                        }
#endregion

#region 5% VAT- AllInventoriesList
                        foreach (var itm in VAT_5_Items)
                        {
                            decimal itmRate = XcelServiceHelper.TruncateDecimal(itm.PriceWOTax / itm.Quantity, 2);
                            strData = strData + "<ALLINVENTORYENTRIES.LIST>" +
                            "<STOCKITEMNAME>" + itm.ItemName + " - " + itm.ItemCode + "</STOCKITEMNAME><ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
                            "<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE><ISAUTONEGATE>No</ISAUTONEGATE><ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE>" +
                            "<ISTRACKCOMPONENT>No</ISTRACKCOMPONENT><ISTRACKPRODUCTION>No</ISTRACKPRODUCTION><ISPRIMARYITEM>No</ISPRIMARYITEM>" +
                            "<ISSCRAP>No</ISSCRAP><RATE>" + itmRate + "/no</RATE><AMOUNT>-" + itm.PriceWOTax + "</AMOUNT><ACTUALQTY> " + itm.Quantity + " no</ACTUALQTY>" +
                            "<BILLEDQTY> " + itm.Quantity + " no</BILLEDQTY><BATCHALLOCATIONS.LIST><GODOWNNAME>Main Location</GODOWNNAME><BATCHNAME>Primary Batch</BATCHNAME>" +
                            "<INDENTNO/><ORDERNO/><TRACKINGNUMBER/><DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED><AMOUNT>-" + itm.PriceWOTax + "</AMOUNT>" +
                            "<ACTUALQTY> " + itm.Quantity + " no</ACTUALQTY><BILLEDQTY> " + itm.Quantity + " no</BILLEDQTY><ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST>" +
                            "<VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST></BATCHALLOCATIONS.LIST><ACCOUNTINGALLOCATIONS.LIST>" +
                            @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST><TAXCLASSIFICATIONNAME>Input VAT @ 12.5%</TAXCLASSIFICATIONNAME>" +
                            "<LEDGERNAME>Purchase @12.5 %</LEDGERNAME><GSTCLASS/><ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM><REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
                            "<ISPARTYLEDGER>No</ISPARTYLEDGER><ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE><AMOUNT>-" + itm.PriceWOTax + "</AMOUNT><SERVICETAXDETAILS.LIST>        </SERVICETAXDETAILS.LIST>" +
                            "<BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST><BILLALLOCATIONS.LIST>        </BILLALLOCATIONS.LIST><INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST>" +
                            "<OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>" +
                            "<INPUTCRALLOCS.LIST>        </INPUTCRALLOCS.LIST><DUTYHEADDETAILS.LIST>        </DUTYHEADDETAILS.LIST><EXCISEDUTYHEADDETAILS.LIST>        </EXCISEDUTYHEADDETAILS.LIST>" +
                            "<SUMMARYALLOCS.LIST>        </SUMMARYALLOCS.LIST><STPYMTDETAILS.LIST>        </STPYMTDETAILS.LIST><EXCISEPAYMENTALLOCATIONS.LIST>        </EXCISEPAYMENTALLOCATIONS.LIST>" +
                            "<TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST>" +
                            "<VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST><COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST><REFVOUCHERDETAILS.LIST>        </REFVOUCHERDETAILS.LIST>" +
                            "<INVOICEWISEDETAILS.LIST>        </INVOICEWISEDETAILS.LIST></ACCOUNTINGALLOCATIONS.LIST><DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>" +
                            "<SUPPLEMENTARYDUTYHEADDETAILS.LIST>       </SUPPLEMENTARYDUTYHEADDETAILS.LIST><TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>" +
                            "<REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST><EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST><EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST>" +
                            "</ALLINVENTORYENTRIES.LIST>";
                        }
#endregion
                        strData = strData + "<ATTDRECORDS.LIST>      </ATTDRECORDS.LIST>" +
                                   "</VOUCHER></TALLYMESSAGE>";
                        UpdatePurchaseRegister(invoice, model.Company, cellDbShip, DateTime.Parse(cellFrmDate.ToString()), DateTime.Parse(cellToDate.ToString()));
                    }
                    strData = strData + "</REQUESTDATA></IMPORTDATA></BODY></ENVELOPE>";

                    XmlDocument docum = new XmlDocument();
                    docum.LoadXml(strData);

                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }
                    docum.Save(savePath + "//purchase.xml");
                }
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void UpdatePurchaseRegister(Invoice invoice,string company, string dbShip, 
            DateTime fromDate, DateTime toDate )
        {
            try
            {
                using (var db = new eXmlContext())
                {
                    foreach (var invItem in invoice.Items)
                    {
                        var purRecExists = db.Set<PurchaseRegister>()
                                        .Where(x => x.Company == company && x.DbShip == dbShip)
                                        .Where(x => x.InvoiceNo == invoice.InvoiceNo)
                                        .Where(x => x.InvoiceType == invoice.InvoiceType)
                                        .Where(x => x.ItemCode == invItem.ItemCode)
                                        .Where(x => x.PaymentInstrument == invItem.PaymentInstrument)
                                        .Where(x => x.OrdYearWk == invoice.OrdYearWk && x.InvYearWk == invoice.InvYearWk)
                                        .FirstOrDefault();
                        if (purRecExists == null)
                        {
                            PurchaseRegister reg = new PurchaseRegister
                            {
                                Company = company,
                                DbShip = dbShip,
                                FromDate = fromDate,
                                ToDate = toDate,
                                ItemCode = invItem.ItemCode,
                                PaymentInstrument = invItem.PaymentInstrument,
                                ItemName = invItem.ItemName,
                                OrdYearWk = invoice.OrdYearWk,
                                InvoiceNo = invoice.InvoiceNo,
                                InvoiceType = invoice.InvoiceType,
                                InvoiceDate = invoice.InvoiceDate,
                                InvYearWk = invoice.InvYearWk,
                                Quantity = invItem.Quantity,
                                PriceWOTax = invItem.PriceWOTax,
                                VAT = invItem.VAT,
                                PriceInclVAT = invItem.PriceInclVAT
                            };
                            db.PurchaseRegister.Add(reg);
                        }
                        else
                        {
                            purRecExists.Company = company;
                            purRecExists.DbShip = dbShip;
                            purRecExists.FromDate = fromDate;
                            purRecExists.ToDate = toDate;
                            purRecExists.ItemCode = invItem.ItemCode;
                            purRecExists.PaymentInstrument = invItem.PaymentInstrument;
                            purRecExists.ItemName = invItem.ItemName;
                            purRecExists.OrdYearWk = invoice.OrdYearWk;
                            purRecExists.InvoiceNo = invoice.InvoiceNo;
                            purRecExists.InvoiceType = invoice.InvoiceType;
                            purRecExists.InvoiceDate = invoice.InvoiceDate;
                            purRecExists.InvYearWk = invoice.InvYearWk;
                            purRecExists.Quantity = invItem.Quantity;
                            purRecExists.PriceWOTax = invItem.PriceWOTax;
                            purRecExists.VAT = invItem.VAT;
                            purRecExists.PriceInclVAT = invItem.PriceInclVAT;

                            db.Entry(purRecExists).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}