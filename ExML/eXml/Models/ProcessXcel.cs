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
namespace eXml.Models
{
    public class ProcessXcel
    {
        public static void processExcelSheet(UploadFileModel model, string fileName, string savePath)
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

                int rows = 0;
                string cellA; string cellB; string cellC; string cellD; string cellE; string cellG;
                string cellF; string cellH; string cellL; string cellO; string cellJ; string cellQ;
                string cellU; string cellW; string cellY; string cellAB; string cellAD; string cellAssembly;
                

                var yearwk = ""; var year = ""; var week = ""; var assembly = ""; var date1 = ""; var unitname = "";
                var cofNo = ""; var consutantCode = ""; var consultantName = ""; decimal grossAmt; decimal netAmt;
                decimal vatAmt; decimal checkedAmt; string partyName; var panNo = ""; int mnth; var narration = "";
                var month = ""; var day = ""; var item = ""; var itemCode =""; var itemName = ""; int orderId;
                decimal MRP; decimal ordQty; decimal consPrice; decimal amount; var status ="";

                 
              
                //if (model.Type == enPostType.Invoice_12_5_WithAddress)
                //{
                    List<UnitName> units = new List<UnitName>();
                    List<UnitConsultant> unitConsultants = new List<UnitConsultant>();
                    List<ConsultantOrder> consOrders = new List<ConsultantOrder>();
                    List<StockItem> stockItems = new List<StockItem>();

                    cellAssembly = "L8";
                    assembly = GetCellValue(workBk, theSheet, cellAssembly);
                    week = GetCellValue(workBk,theSheet,"U9");
                    year = GetCellValue(workBk, theSheet, "U8");
                    UnitName u;
                    ConsultantOrder co;
                    UnitConsultant uc;
                    int j =0 ;
                   
                    int rowCount = shtData.Elements<Row>().Count();
                    rowCount = rowCount + 4;
                    //foreach (Row r in shtData.Elements<Row>())
                    for (rows = 1; rows <= rowCount; rows++ )
                    {
                        //rows += 1;
                        cellQ = "Q" + rows;

                        if (rows >= 12)
                        {
                            //Cell theCell = r.Descendants<Cell>().Where(x => x.CellReference == cellQ).FirstOrDefault();
                            string theCell = GetCellValue(workBk, theSheet, cellQ);
                            if (!string.IsNullOrEmpty(theCell))
                            {
                                //
                                cellQ = "Q" + rows;
                                cellJ = "J" + rows;
                                itemCode = GetCellValue(workBk, theSheet, cellJ);
                                itemName = GetCellValue(workBk, theSheet, cellQ);
                                itemName = itemName.Replace("(", " ");
                                itemName = itemName.Replace(")", " ");
                                itemName = itemName.Replace("&", "_");
                                item = itemName + " - " + itemCode;

                                StockItem s = stockItems.FirstOrDefault(x => x.ItemCode == itemCode.Trim());
                                if (s == null)
                                {
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
                                       @"<RATEOFVAT> 12.5</RATEOFVAT><LANGUAGENAME.LIST><NAME.LIST TYPE=""String""><NAME>" + item + "</NAME></NAME.LIST><LANGUAGEID> 1033</LANGUAGEID>" +
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
                                cellA = "A" + rows;
                                unitname = GetCellValue(workBk, theSheet, cellA);

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
                                consutantCode = GetCellValue(workBk, theSheet, cellD);
                                cellF = "F" + rows;
                                consultantName = GetCellValue(workBk, theSheet, cellF);

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
                                orderId = int.Parse(GetCellValue(workBk, theSheet, cellH));
                                cellU = "U" + rows;
                                ordQty = Decimal.Parse(GetCellValue(workBk, theSheet, cellU));

                                cellW = "W" + rows;
                                MRP = Decimal.Parse(GetCellValue(workBk, theSheet, cellW));

                                cellY = "Y" + rows;
                                consPrice = Math.Round(Decimal.Parse(GetCellValue(workBk, theSheet, cellY)));

                                cellAB = "AB" + rows;
                                amount = Math.Round(Decimal.Parse(GetCellValue(workBk, theSheet, cellAB)));

                                cellAD = "AD" + rows;
                                status = GetCellValue(workBk, theSheet, cellAD);

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
                   
                    foreach (var unit in units)
                    {
                       
                        foreach (var uCon in unit.UnitConsultants )
                        {
                            string consultant = uCon.Consultant + " - " + uCon.ConsultantId;

                            //newVoucherNo = newVoucherNo + 1;

                            strData = strData + @"<TALLYMESSAGE xmlns:UDF=""TallyUDF"">" +
                            @"<LEDGER NAME="""
                            + consultant +
                            @""" RESERVEDNAME="""">" +
                            @"<ADDRESS.LIST TYPE=""String""><ADDRESS>" + unit.Unit + "</ADDRESS><ADDRESS>" + assembly + "</ADDRESS></ADDRESS.LIST>" +
                            @"<MAILINGNAME.LIST TYPE=""String""><MAILINGNAME>" + consultant + "</MAILINGNAME></MAILINGNAME.LIST>" +
                            @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
                            "<ALTEREDON>" + model.Date + "</ALTEREDON><STATENAME>Maharashtra</STATENAME><PARENT>" + unit.Unit + "</PARENT><TAXCLASSIFICATIONNAME/>" +
                            "<TAXTYPE>Others</TAXTYPE><BUSINESSTYPE/><BASICTYPEOFDUTY>Excise Surcharge</BASICTYPEOFDUTY><GSTTYPE/><APPROPRIATEFOR/>" +
                            "<SERVICECATEGORY/><EXCISELEDGERCLASSIFICATION/><EXCISEDUTYTYPE/><EXCISENATUREOFPURCHASE/><LEDGERFBTCATEGORY/>" +
                            "<ISBILLWISEON>No</ISBILLWISEON><ISCOSTCENTRESON>Yes</ISCOSTCENTRESON><ISINTERESTON>No</ISINTERESTON><ALLOWINMOBILE>No</ALLOWINMOBILE>" +
                            "<ISCOSTTRACKINGON>No</ISCOSTTRACKINGON><ISCONDENSED>No</ISCONDENSED><AFFECTSSTOCK>No</AFFECTSSTOCK><FORPAYROLL>No</FORPAYROLL>" +
                            "<ISABCENABLED>No</ISABCENABLED><INTERESTONBILLWISE>No</INTERESTONBILLWISE><OVERRIDEINTEREST>No</OVERRIDEINTEREST><OVERRIDEADVINTEREST>No</OVERRIDEADVINTEREST>" +
                            "<USEFORVAT>No</USEFORVAT><IGNORETDSEXEMPT>No</IGNORETDSEXEMPT><ISTCSAPPLICABLE>No</ISTCSAPPLICABLE><ISTDSAPPLICABLE>No</ISTDSAPPLICABLE><ISFBTAPPLICABLE>No</ISFBTAPPLICABLE>" +
                            "<ISGSTAPPLICABLE>No</ISGSTAPPLICABLE><ISEXCISEAPPLICABLE>No</ISEXCISEAPPLICABLE><ISTDSEXPENSE>No</ISTDSEXPENSE><ISEDLIAPPLICABLE>No</ISEDLIAPPLICABLE>" +
                            "<ISRELATEDPARTY>No</ISRELATEDPARTY><USEFORESIELIGIBILITY>No</USEFORESIELIGIBILITY><SHOWINPAYSLIP>No</SHOWINPAYSLIP><USEFORGRATUITY>No</USEFORGRATUITY>" +
                            "<ISTDSPROJECTED>No</ISTDSPROJECTED><FORSERVICETAX>No</FORSERVICETAX><ISINPUTCREDIT>No</ISINPUTCREDIT><ISEXEMPTED>No</ISEXEMPTED><ISABATEMENTAPPLICABLE>No</ISABATEMENTAPPLICABLE>" +
                            "<ISSTXPARTY>No</ISSTXPARTY><ISSTXNONREALIZEDTYPE>No</ISSTXNONREALIZEDTYPE><TDSDEDUCTEEISSPECIALRATE>No</TDSDEDUCTEEISSPECIALRATE><AUDITED>No</AUDITED><SORTPOSITION> 1000</SORTPOSITION>" +
                            @"<RATEOFTAXCALCULATION> 12.50</RATEOFTAXCALCULATION><LANGUAGENAME.LIST><NAME.LIST TYPE=""String""><NAME>" + consultant + "</NAME></NAME.LIST><LANGUAGEID> 1033</LANGUAGEID>" +
                            "</LANGUAGENAME.LIST><XBRLDETAIL.LIST>      </XBRLDETAIL.LIST><AUDITDETAILS.LIST>      </AUDITDETAILS.LIST><SCHVIDETAILS.LIST>      </SCHVIDETAILS.LIST><SLABPERIOD.LIST>      </SLABPERIOD.LIST>" +
                            "<GRATUITYPERIOD.LIST>      </GRATUITYPERIOD.LIST><ADDITIONALCOMPUTATIONS.LIST>      </ADDITIONALCOMPUTATIONS.LIST><BANKALLOCATIONS.LIST>      </BANKALLOCATIONS.LIST><PAYMENTDETAILS.LIST>      </PAYMENTDETAILS.LIST>" +
                            "<BANKEXPORTFORMATS.LIST>      </BANKEXPORTFORMATS.LIST><BILLALLOCATIONS.LIST>      </BILLALLOCATIONS.LIST><INTERESTCOLLECTION.LIST>      </INTERESTCOLLECTION.LIST><LEDGERCLOSINGVALUES.LIST>      </LEDGERCLOSINGVALUES.LIST>" +
                            "<LEDGERAUDITCLASS.LIST>      </LEDGERAUDITCLASS.LIST><OLDAUDITENTRIES.LIST>      </OLDAUDITENTRIES.LIST><TDSEXEMPTIONRULES.LIST>      </TDSEXEMPTIONRULES.LIST><DEDUCTINSAMEVCHRULES.LIST>      </DEDUCTINSAMEVCHRULES.LIST>" +
                            "<LOWERDEDUCTION.LIST>      </LOWERDEDUCTION.LIST><STXABATEMENTDETAILS.LIST>      </STXABATEMENTDETAILS.LIST><LEDMULTIADDRESSLIST.LIST>      </LEDMULTIADDRESSLIST.LIST><STXTAXDETAILS.LIST>      </STXTAXDETAILS.LIST>" +
                            "<CHEQUERANGE.LIST>      </CHEQUERANGE.LIST><DEFAULTVCHCHEQUEDETAILS.LIST>      </DEFAULTVCHCHEQUEDETAILS.LIST><ACCOUNTAUDITENTRIES.LIST>      </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>      </AUDITENTRIES.LIST>" +
                            "<BRSIMPORTEDINFO.LIST>      </BRSIMPORTEDINFO.LIST><AUTOBRSCONFIGS.LIST>      </AUTOBRSCONFIGS.LIST><BANKURENTRIES.LIST>      </BANKURENTRIES.LIST><DEFAULTCHEQUEDETAILS.LIST>      </DEFAULTCHEQUEDETAILS.LIST>" +
                            "<DEFAULTOPENINGCHEQUEDETAILS.LIST>      </DEFAULTOPENINGCHEQUEDETAILS.LIST></LEDGER></TALLYMESSAGE>";

                          
                            if (unit != null && unit.IsGroupCreated == false)
                            {
                                strData = strData + @"<TALLYMESSAGE xmlns:UDF=""TallyUDF"">" +
                                @"<GROUP NAME="""
                                + unit.Unit +
                                @""" ACTION = ""CREATE"">" +
                                "<NAME.LIST><NAME>" + unit.Unit + "</NAME></NAME.LIST><PARENT>Sundry Debtors</PARENT><ISSUBLEDGER>No</ISSUBLEDGER><ISBILLWISEON>No</ISBILLWISEON>" +
                                "<ISCOSTCENTRESON>No</ISCOSTCENTRESON></GROUP></TALLYMESSAGE>";
                                unit.IsGroupCreated = true;
                            }
                            decimal totalConsultantAmt = uCon.ConsultantOrders.Sum(x => x.ConsultantPrice * x.OrdQty);
                            decimal consultantAmount = uCon.ConsultantOrders.Where(x => !x.ItemName.StartsWith("PPP")).Sum(x => x.ConsultantPrice * x.OrdQty);
                            decimal netAmount =((consultantAmount /Decimal.Parse("112.5") * Decimal.Parse("100")));
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
                          
                            strData = strData + @"<TALLYMESSAGE xmlns:UDF=""TallyUDF"">" +
                            @"<VOUCHER VCHTYPE=""Sales"" ACTION=""Create"" OBJVIEW=""Invoice Voucher View"">" +
                            @"<ADDRESS.LIST TYPE=""String""><ADDRESS>" + unit.Unit + "</ADDRESS><ADDRESS>" + assembly + "</ADDRESS>" +
                            @"</ADDRESS.LIST><BASICBUYERADDRESS.LIST TYPE=""String""><BASICBUYERADDRESS>" + unit.Unit + "</BASICBUYERADDRESS>" +
                            "<BASICBUYERADDRESS>" + assembly + "</BASICBUYERADDRESS></BASICBUYERADDRESS.LIST>" +
                            @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST><DATE>" + model.Date + "</DATE>" +
                            "<PARTYNAME>" + consultant + "</PARTYNAME><VOUCHERTYPENAME>Sales</VOUCHERTYPENAME><VOUCHERNUMBER>" + VoucherNo + "</VOUCHERNUMBER>" +
                            "<PARTYLEDGERNAME>" + consultant + "</PARTYLEDGERNAME><BASICBASEPARTYNAME>" + consultant + "</BASICBASEPARTYNAME>" +
                            "<CSTFORMISSUETYPE/><CSTFORMRECVTYPE/><FBTPAYMENTTYPE>Default</FBTPAYMENTTYPE><PERSISTEDVIEW>Invoice Voucher View</PERSISTEDVIEW>" +
                            "<BASICBUYERNAME>" + consultant + "</BASICBUYERNAME><BASICFINALDESTINATION>" + assembly + "</BASICFINALDESTINATION>" +
                            "<VCHGSTCLASS/><DIFFACTUALQTY>No</DIFFACTUALQTY><AUDITED>No</AUDITED><FORJOBCOSTING>No</FORJOBCOSTING><ISOPTIONAL>No</ISOPTIONAL>" +
                            "<EFFECTIVEDATE>" + model.Date + "</EFFECTIVEDATE><ISFORJOBWORKIN>No</ISFORJOBWORKIN><ALLOWCONSUMPTION>No</ALLOWCONSUMPTION>" +
                            "<USEFORINTEREST>No</USEFORINTEREST><USEFORGAINLOSS>No</USEFORGAINLOSS><USEFORGODOWNTRANSFER>No</USEFORGODOWNTRANSFER>" +
                            "<USEFORCOMPOUND>No</USEFORCOMPOUND><EXCISEOPENING>No</EXCISEOPENING><USEFORFINALPRODUCTION>No</USEFORFINALPRODUCTION>" +
                            "<ISCANCELLED>No</ISCANCELLED><HASCASHFLOW>No</HASCASHFLOW><ISPOSTDATED>No</ISPOSTDATED><USETRACKINGNUMBER>No</USETRACKINGNUMBER>" +
                            "<ISINVOICE>Yes</ISINVOICE><MFGJOURNAL>No</MFGJOURNAL><HASDISCOUNTS>No</HASDISCOUNTS><ASPAYSLIP>No</ASPAYSLIP><ISCOSTCENTRE>No</ISCOSTCENTRE>" +
                            "<ISSTXNONREALIZEDVCH>No</ISSTXNONREALIZEDVCH><ISEXCISEMANUFACTURERON>Yes</ISEXCISEMANUFACTURERON><ISBLANKCHEQUE>No</ISBLANKCHEQUE>" +
                            "<ISDELETED>No</ISDELETED><ASORIGINAL>No</ASORIGINAL><VCHISFROMSYNC>No</VCHISFROMSYNC><OLDAUDITENTRIES.LIST>      </OLDAUDITENTRIES.LIST>" +
                            "<ACCOUNTAUDITENTRIES.LIST>      </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>      </AUDITENTRIES.LIST><INVOICEDELNOTES.LIST>      </INVOICEDELNOTES.LIST>" +
                            "<INVOICEORDERLIST.LIST>      </INVOICEORDERLIST.LIST><INVOICEINDENTLIST.LIST>      </INVOICEINDENTLIST.LIST><ATTENDANCEENTRIES.LIST>      </ATTENDANCEENTRIES.LIST>" +
                            "<ORIGINVOICEDETAILS.LIST>      </ORIGINVOICEDETAILS.LIST><INVOICEEXPORTLIST.LIST>      </INVOICEEXPORTLIST.LIST><LEDGERENTRIES.LIST>" +
                            @"<OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST><LEDGERNAME>" + consultant + "</LEDGERNAME>" +
                            "<GSTCLASS/><ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM><REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>Yes</ISPARTYLEDGER>" +
                            "<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE><AMOUNT>-" + totalConsultantAmt + "</AMOUNT><CATEGORYALLOCATIONS.LIST><CATEGORY>Primary Cost Category</CATEGORY>" +
                            "<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE><COSTCENTREALLOCATIONS.LIST><NAME>" + assembly + "</NAME><AMOUNT>-" + totalConsultantAmt + "</AMOUNT>" +
                            "</COSTCENTREALLOCATIONS.LIST></CATEGORYALLOCATIONS.LIST><BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST><BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>" +
                            "<INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>" +
                            "<AUDITENTRIES.LIST>       </AUDITENTRIES.LIST><TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>" +
                            "<TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST><VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST><COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>" +
                            @"</LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
                            @"<BASICRATEOFINVOICETAX.LIST TYPE=""Number""><BASICRATEOFINVOICETAX> 12.50</BASICRATEOFINVOICETAX></BASICRATEOFINVOICETAX.LIST><TAXCLASSIFICATIONNAME>Output VAT @ 12.5%</TAXCLASSIFICATIONNAME>" +
                            "<ROUNDTYPE>Normal Rounding</ROUNDTYPE><LEDGERNAME>12.5% Vat on Sales</LEDGERNAME><GSTCLASS/><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM>" +
                            "<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>No</ISPARTYLEDGER><ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><AMOUNT>" + tax + "</AMOUNT>" +
                            "<VATASSESSABLEVALUE>" + netAmount + "</VATASSESSABLEVALUE><BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST><BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>" +
                            "<INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>" +
                            "<AUDITENTRIES.LIST>       </AUDITENTRIES.LIST><TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST><CATEGORY>Output VAT @ 12.5%</CATEGORY>" +
                            "<TAXTYPE>VAT</TAXTYPE><TAXNAME>" + VoucherNo + "</TAXNAME><PARTYLEDGER>" + consultant + "</PARTYLEDGER><REFTYPE>New Ref</REFTYPE><ISOPTIONAL>No</ISOPTIONAL>" +
                            "<ISPANVALID>No</ISPANVALID><ZERORATED>No</ZERORATED><EXEMPTED>No</EXEMPTED><ISSPECIALRATE>No</ISSPECIALRATE><ISDEDUCTNOW>No</ISDEDUCTNOW><ISPANNOTAVAILABLE>No</ISPANNOTAVAILABLE>" +
                            "<ISSUPPLEMENTARY>No</ISSUPPLEMENTARY><OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>";

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
                                        //check if net amount == tracknetamttotal
                                        if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
                                        {
                                            checkedAmt = netAmount - trackNetAmtTotal;
                                          
                                            itemsTotalAmt = itemsTotalAmt + checkedAmt;
                                            itemTotalTax = itemTotalTax - checkedAmt;
                                           
                                        }

                                    }

                                    strData = strData + "<SUBCATEGORYALLOCATION.LIST>" +
                                    "<STOCKITEMNAME>" + con.ItemName + "-" + con.ItemCode + "</STOCKITEMNAME>" +
                                    "<SUBCATEGORY>VAT</SUBCATEGORY>" +
                                    "<DUTYLEDGER>12.5% Vat on Sales</DUTYLEDGER>" +
                                    "<SUBCATZERORATED>No</SUBCATZERORATED>" +
                                    "<SUBCATEXEMPTED>No</SUBCATEXEMPTED>" +
                                    "<SUBCATISSPECIALRATE>No</SUBCATISSPECIALRATE>" +
                                    "<TAXRATE> 12.50</TAXRATE>" +
                                    "<ASSESSABLEAMOUNT>" + itemsTotalAmt + "</ASSESSABLEAMOUNT>" +
                                    "<TAX>" + itemTotalTax + "</TAX>" +
                                    "<BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY>" +
                                    "</SUBCATEGORYALLOCATION.LIST>";
                                    q++;
                                }
                              
                            }
                            strData = strData + "</TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>" +
                            "<VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST><COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>" +
                            "</LEDGERENTRIES.LIST>";
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
                                        //check if net amount == tracknetamttotal
                                        if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
                                        {
                                            checkedAmt = netAmount - trackNetAmtTotal;

                                            itemsTotalAmt = itemsTotalAmt + checkedAmt;
                                            itemTotalTax = itemTotalTax - checkedAmt;
                                            itemRate = itemsTotalAmt / con.OrdQty;
                                        }

                                    }
                                    //-----------------------------------------------------------------------

                                    strData = strData + "<ALLINVENTORYENTRIES.LIST><STOCKITEMNAME>" + con.ItemName + "-" + con.ItemCode + "</STOCKITEMNAME>" +
                                    "<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><ISAUTONEGATE>No</ISAUTONEGATE>" +
                                    "<ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE><ISTRACKCOMPONENT>No</ISTRACKCOMPONENT><ISTRACKPRODUCTION>No</ISTRACKPRODUCTION>" +
                                    "<ISPRIMARYITEM>No</ISPRIMARYITEM><ISSCRAP>No</ISSCRAP><RATE>" + itemRate + "/NO</RATE><AMOUNT>" + itemsTotalAmt + "</AMOUNT>" + //<AMOUNT>" + itemNetAmt + "</AMOUNT>
                                    "<ACTUALQTY> " + con.OrdQty + " NO</ACTUALQTY><BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY><BATCHALLOCATIONS.LIST>" +
                                    "<GODOWNNAME>Main Location</GODOWNNAME><BATCHNAME>Primary Batch</BATCHNAME><INDENTNO/><ORDERNO/><TRACKINGNUMBER/>" +
                                    "<DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED><AMOUNT>" + itemsTotalAmt + "</AMOUNT><ACTUALQTY> " + con.OrdQty + " NO</ACTUALQTY>" +
                                    "<BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY><ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST><VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST>" +
                                    @"</BATCHALLOCATIONS.LIST><ACCOUNTINGALLOCATIONS.LIST><OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
                                    "</OLDAUDITENTRYIDS.LIST><TAXCLASSIFICATIONNAME>Output VAT @ 12.5%</TAXCLASSIFICATIONNAME><LEDGERNAME>Sales @12.5%</LEDGERNAME><GSTCLASS/>" +
                                    "<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM><REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>No</ISPARTYLEDGER>" +
                                    "<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><AMOUNT>" + itemsTotalAmt + "</AMOUNT><BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST>" +
                                    "<BILLALLOCATIONS.LIST>        </BILLALLOCATIONS.LIST><INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>" +
                                    "<ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>        </AUDITENTRIES.LIST><TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST>" +
                                    "<TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST><VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST>" +
                                    "<COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST></ACCOUNTINGALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>" +
                                    "<EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST><EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST></ALLINVENTORYENTRIES.LIST>";

                                    q++;
                                    itemNetAmt = TruncateDecimal(itemNetAmt, 2);
                                    decimal chkItemTax = con.ConsultantPrice - (itemNetAmt + itemTax);
                                    if (chkItemTax <= 1 || chkItemTax >= -1)
                                    {
                                        itemTax = itemTax + chkItemTax;
                                    }
                                    PostedTransaction trans;
                                    using (var db = new eXmlContext())
                                    {
                                        trans = db.Set<PostedTransaction>()
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
                                                VatAmount = itemTax ,
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
                                            db.PostedTransaction.Add(trans);

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

                                            db.Entry(trans).State = System.Data.Entity.EntityState.Modified;

                                        }
                                        db.SaveChanges();
                                    }
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

                                strData = strData + @"<LEDGERENTRIES.LIST><OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST>" +
                                        @"<BASICRATEOFINVOICETAX.LIST TYPE=""Number""><BASICRATEOFINVOICETAX> 5</BASICRATEOFINVOICETAX></BASICRATEOFINVOICETAX.LIST><TAXCLASSIFICATIONNAME>Output VAT @ 5%</TAXCLASSIFICATIONNAME>" +
                                        "<ROUNDTYPE>Normal Rounding</ROUNDTYPE><LEDGERNAME>Output Vat @5%</LEDGERNAME><GSTCLASS/><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM>" +
                                        "<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>No</ISPARTYLEDGER><ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><AMOUNT>" + tax + "</AMOUNT>" +
                                        "<VATASSESSABLEVALUE>" + netAmount + "</VATASSESSABLEVALUE><BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST><BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>" +
                                        "<INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>" +
                                        "<AUDITENTRIES.LIST>       </AUDITENTRIES.LIST><TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST><CATEGORY>Output VAT @ 5%</CATEGORY>" +
                                        "<TAXTYPE>VAT</TAXTYPE><TAXNAME>" + VoucherNo + "</TAXNAME><PARTYLEDGER>" + consultant + "</PARTYLEDGER><REFTYPE>New Ref</REFTYPE><ISOPTIONAL>No</ISOPTIONAL>" +
                                        "<ISPANVALID>No</ISPANVALID><ZERORATED>No</ZERORATED><EXEMPTED>No</EXEMPTED><ISSPECIALRATE>No</ISSPECIALRATE><ISDEDUCTNOW>No</ISDEDUCTNOW><ISPANNOTAVAILABLE>No</ISPANNOTAVAILABLE>" +
                                        "<ISSUPPLEMENTARY>No</ISSUPPLEMENTARY><OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST><ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>";

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
                                        //check if net amount == tracknetamttotal
                                        if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
                                        {
                                            checkedAmt = netAmount - trackNetAmtTotal;

                                            itemsTotalAmt = itemsTotalAmt + checkedAmt;
                                            itemTotalTax = itemTotalTax - checkedAmt;

                                        }

                                    }
                                    strData = strData + "<SUBCATEGORYALLOCATION.LIST>" +
                                   "<STOCKITEMNAME>" + con.ItemName + "-" + con.ItemCode + "</STOCKITEMNAME>" +
                                   "<SUBCATEGORY>VAT</SUBCATEGORY>" +
                                   "<DUTYLEDGER>12.5% Vat on Sales</DUTYLEDGER>" +
                                   "<SUBCATZERORATED>No</SUBCATZERORATED>" +
                                   "<SUBCATEXEMPTED>No</SUBCATEXEMPTED>" +
                                   "<SUBCATISSPECIALRATE>No</SUBCATISSPECIALRATE>" +
                                   "<TAXRATE> 12.50</TAXRATE>" +
                                   "<ASSESSABLEAMOUNT>" + itemsTotalAmt + "</ASSESSABLEAMOUNT>" +
                                   "<TAX>" + itemTotalTax + "</TAX>" +
                                   "<BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY>" +
                                   "</SUBCATEGORYALLOCATION.LIST>";
                                    q++;
                                }
                                strData = strData + "</TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>" +
                                           "<VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST><COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>" +
                                           "</LEDGERENTRIES.LIST>";
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
                                        //check if net amount == tracknetamttotal
                                        if (netAmount > trackNetAmtTotal || netAmount < trackNetAmtTotal)
                                        {
                                            checkedAmt = netAmount - trackNetAmtTotal;

                                            itemsTotalAmt = itemsTotalAmt + checkedAmt;
                                            itemTotalTax = itemTotalTax - checkedAmt;
                                            itemRate = itemsTotalAmt / con.OrdQty;
                                        }

                                    }

                                    strData = strData + "<ALLINVENTORYENTRIES.LIST><STOCKITEMNAME>" + con.ItemName + "-" + con.ItemCode + "</STOCKITEMNAME>" +
                                    "<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><ISAUTONEGATE>No</ISAUTONEGATE>" +
                                    "<ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE><ISTRACKCOMPONENT>No</ISTRACKCOMPONENT><ISTRACKPRODUCTION>No</ISTRACKPRODUCTION>" +
                                    "<ISPRIMARYITEM>No</ISPRIMARYITEM><ISSCRAP>No</ISSCRAP><RATE>" + itemRate + "/NO</RATE><AMOUNT>" + itemsTotalAmt + "</AMOUNT>" + //<AMOUNT>" + itemNetAmt + "</AMOUNT>
                                    "<ACTUALQTY> " + con.OrdQty + " NO</ACTUALQTY><BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY><BATCHALLOCATIONS.LIST>" +
                                    "<GODOWNNAME>Main Location</GODOWNNAME><BATCHNAME>Primary Batch</BATCHNAME><INDENTNO/><ORDERNO/><TRACKINGNUMBER/>" +
                                    "<DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED><AMOUNT>" + itemsTotalAmt + "</AMOUNT><ACTUALQTY> " + con.OrdQty + " NO</ACTUALQTY>" +
                                    "<BILLEDQTY> " + con.OrdQty + " NO</BILLEDQTY><ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST><VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST>" +
                                    @"</BATCHALLOCATIONS.LIST><ACCOUNTINGALLOCATIONS.LIST><OLDAUDITENTRYIDS.LIST TYPE=""Number""><OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
                                    "</OLDAUDITENTRYIDS.LIST><TAXCLASSIFICATIONNAME>Output VAT @ 5%</TAXCLASSIFICATIONNAME><LEDGERNAME>Sales @5%</LEDGERNAME><GSTCLASS/>" +
                                    "<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE><LEDGERFROMITEM>No</LEDGERFROMITEM><REMOVEZEROENTRIES>No</REMOVEZEROENTRIES><ISPARTYLEDGER>No</ISPARTYLEDGER>" +
                                    "<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE><AMOUNT>" + itemsTotalAmt + "</AMOUNT><BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST>" +
                                    "<BILLALLOCATIONS.LIST>        </BILLALLOCATIONS.LIST><INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST><OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>" +
                                    "<ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST><AUDITENTRIES.LIST>        </AUDITENTRIES.LIST><TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST>" +
                                    "<TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST><TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST><VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST>" +
                                    "<COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST></ACCOUNTINGALLOCATIONS.LIST><TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>" +
                                    "<EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST><EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST></ALLINVENTORYENTRIES.LIST>";

                                    q++;
                                    itemNetAmt = TruncateDecimal(itemNetAmt, 2);
                                    decimal chkItemTax = con.ConsultantPrice - (itemNetAmt + itemTax);
                                    if (chkItemTax <= 1 || chkItemTax >= -1)
                                    {
                                        itemTax = itemTax + chkItemTax;
                                    }
                                    PostedTransaction trans;
                                    using (var db = new eXmlContext())
                                    {
                                        trans = db.Set<PostedTransaction>()
                                                //.Where(x => x.Company == model.Company)
                                                // .Where(x => x.AssemblyName == assembly)
                                                // .Where(x => x.UnitName == unit.Unit)
                                                // .Where(x => x.Year == year)
                                                // .Where(x => x.Week == week)
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
                                            db.PostedTransaction.Add(trans);

                                        }
                                        else
                                        {
                                            trans.ItemCode = con.ItemCode;
                                            trans.GrossAmount = con.Amount;
                                            trans.NetAmount = itemNetAmt;
                                            trans.VatAmount = itemTax;
                                            trans.ItemCode = con.ItemCode;
                                            trans.ItemName = con.ItemName;
                                            trans.OrderId = con.OrderId;
                                            trans.OrderQty = con.OrdQty;
                                            trans.Status = con.Status;

                                            db.Entry(trans).State = System.Data.Entity.EntityState.Modified;

                                        }
                                        db.SaveChanges();
                                    }
                                }

                            }
#endregion VAT_5%
                            strData = strData + "<ATTDRECORDS.LIST>      </ATTDRECORDS.LIST>" +
                            "</VOUCHER></TALLYMESSAGE>";
                        }
                    }
                    strData = strData + "</REQUESTDATA></IMPORTDATA></BODY></ENVELOPE>";
                    //strData = strData + @"<TALLYMESSAGE xmlns:UDF=""TallyUDF""><COMPANY><REMOTECMPINFO.LIST MERGE=""Yes""><NAME>1992944e-597f-4d18-bdde-35856a4fbddc</NAME>" +
                    //"<REMOTECMPNAME>" + model.Company + " </REMOTECMPNAME><REMOTECMPSTATE>Maharashtra</REMOTECMPSTATE></REMOTECMPINFO.LIST>" +
                    //    "</COMPANY></TALLYMESSAGE></REQUESTDATA></IMPORTDATA></BODY></ENVELOPE>";
                    XmlDocument docum = new XmlDocument();

                    docum.LoadXml(strData);

                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }
                    docum.Save(savePath + "//payment.xml");
                }
            //}
        }

        public static decimal TruncateDecimal(decimal value, int precision)
        {
            decimal step = (decimal)Math.Pow(10, precision);
            int tmp = (int)Math.Truncate(step * value);
            return tmp / step;
        }
        public static void processExcelSAX(string fileName)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart workBk = doc.WorkbookPart;
                WorksheetPart workSht = workBk.WorksheetParts.First();
                OpenXmlReader rdr = OpenXmlReader.Create(workSht);
                string text = "";
                while (rdr.Read())
                {
                    if (rdr.ElementType == typeof(CellValue))
                    {
                        if (!String.IsNullOrEmpty(rdr.GetText()))
                        {
                             text = rdr.GetText();
                             Console.WriteLine(text);
                        }
                    }
                }

            }
        }
        public static string GetCellValue(string fileName,string sheetName,string addressName)
        {
            string value = null;

            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(fileName, false))
            {
                // Retrieve a reference to the workbook part.
                WorkbookPart wbPart = document.WorkbookPart;

                // Find the sheet with the supplied name, and then use that 
                // Sheet object to retrieve a reference to the first worksheet.
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().
                  Where(s => s.Name == sheetName).FirstOrDefault();

                // Throw an exception if there is no sheet.
                if (theSheet == null)
                {
                    return "";
                }

                // Retrieve a reference to the worksheet part.
                WorksheetPart wsPart =
                    (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                // Use its Worksheet property to get a reference to the cell 
                // whose address matches the address you supplied.
                Cell theCell = wsPart.Worksheet.Descendants<Cell>().
                  Where(c => c.CellReference== addressName).FirstOrDefault();

                // If the cell does not exist, return an empty string.
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
        public static string  GetCellValue(WorkbookPart wbPart,  Sheet theSheet , string address)
        {
            string value = null;
            if (theSheet != null)
            {
                 WorksheetPart wsPart =
                    (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                 Cell theCell = wsPart.Worksheet.Descendants<Cell>().
                  Where(c => c.CellReference== address).FirstOrDefault();

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
}