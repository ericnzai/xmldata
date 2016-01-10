using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using eXml.Helpers;
using eXml.Models;
using eXml.Entities;
using System.Data.OleDb;
using System.Data;
using System.Text;
using OfficeOpenXml;
using eXml.Services;
using eXml.Abstractions;

namespace eXml.Controllers
{
    [T2TAuthorize]
    public class UploadControlController : Controller
    {
        IConvertToXmlService _converter = null;
        XmlProvider _provider = null;

        public ActionResult UploadFile()
        {
            UploadFileModel model = new UploadFileModel();
            //{
            //    Type = enPostType.Invoice_12_5_WithAddress
            //};
            ViewData["Msg"] = "";
            ViewData["type"] = EnumHelper.ToList(typeof(enPostType));//GetSelectListItems().ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult UploadFile(UploadFileModel model, int Type, HttpPostedFileBase file)
        {
            //if (DevExpressHelper.IsCallback)
            //{
            if (ModelState.IsValid && Type > 0)
            {
                byte[] fileBytes = new byte[1];
                string outputFile = " //payment.xml";
                if (Request.Files.Count > 0)
                {
                    var f = Request.Files[0];

                    if (f != null && file.ContentLength > 0)
                    {
                        if (!Directory.Exists(Server.MapPath("~/Content/Uploads")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Content/Uploads"));
                        }
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Uploads"), fileName);
                        var savePath = Server.MapPath("~/Content/Output");
                        file.SaveAs(path);
                        var ext = Path.GetExtension(path);
                        if (ext == ".xls")
                        {
                            path = Convert(path, true);
                            if ((enPostType)Type == enPostType.Invoice_12_5_WithAddress)
                            {
                                outputFile = " //payment.xml";
                                _converter = new ConvertInvoiceVoucherToXmlService();
                            }
                            else if ((enPostType)Type == enPostType.Purchase)
                            {
                                outputFile = " //purchase.xml";
                                _converter = new ConvertPurchaseRegisterToXmlService();
                            }
                            _provider = new XmlProvider(_converter, model, path, savePath);
                            _provider.ConvertToXml();

                            ViewData["Msg"] = "File uploaded succesfully. Xml file generated";

                            fileBytes = System.IO.File.ReadAllBytes(savePath + outputFile);
                        }
                        else
                        {
                            ViewData["MsgError"] = "File conversion failed. File type not excel. Check file extension!!";
                        }

                    }
                }

                //}
                if (fileBytes.Length > 10)
                {
                    return File(fileBytes, "application/xml", outputFile.Substring(3));
                }
                else
                {
                    ViewData["type"] = EnumHelper.ToList(typeof(enPostType));
                    return View(model);
                }
            }

            else
            {
                ModelState.AddModelError("", "Upload Type not specified!");
                ViewData["type"] = EnumHelper.ToList(typeof(enPostType));
                return View(model);
            }
        }
        public string Convert(String file)
        {
            var app = new Microsoft.Office.Interop.Excel.Application();
            var wb = app.Workbooks.Open(file);
            file = file + "x";
            wb.SaveAs(Filename: file , FileFormat: Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
            wb.Close();
            app.Quit();
            return file;
        }
        public string Convert(string path, bool is2003xls)
        {

            string newPath = "";
            DataSet ds = ReadXlsFile(path);
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet excel = pck.Workbook.Worksheets.Add(ds.Tables[0].TableName);
                DataTable dt = ds.Tables[0];
                excel.Cells["A1"].LoadFromDataTable(dt, true);
                 newPath = path + "x";
                System.IO.FileInfo file = new System.IO.FileInfo(newPath);
                pck.SaveAs(file);
                ds.Dispose();
            }
            return newPath;
        }
        private static DataSet ReadXlsFile(string path)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            // XLS - Excel 2003 and Older
            props["Provider"] = "Microsoft.Jet.OLEDB.4.0";
            props["Extended Properties"] = "'Excel 8.0;IMEX=1'";
            //props["IMEX"] = "1";
            props["Data Source"] = path;
            //props["TypeGuessRows"] = "0";
            //props["ImportMixedTypes"] = "Text";

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }
            DataSet ds = new DataSet();

            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1;TypeGuessRows=0;ImportMixedTypes=Text\"";
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                // Loop through all Sheets to get data
                foreach (DataRow dr in dtSheet.Rows)
                {
                    string sheetName = dr["TABLE_NAME"].ToString();

                    if (!sheetName.EndsWith("$"))
                        continue;

                    // Get all rows from the Sheet
                    cmd.CommandText = "SELECT * FROM [" + sheetName + "]";

                    DataTable dt = new DataTable();
                    dt.TableName = sheetName;

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);

                    ds.Tables.Add(dt);
                }
                    cmd = null;
                   
                    conn.Close();
        }
                    return ds;
   }


        private IEnumerable<SelectListItem> GetSelectListItems()
        {
            var selectList = new List<SelectListItem>();
            var enumValues = Enum.GetValues(typeof(enPostType)) as enPostType[];

            if (enumValues == null) return null;
            foreach(var enumVal in enumValues)
            {
                selectList.Add(new SelectListItem
                            {
                                Value = enumVal.ToString(),
                                Text = Enum.GetName(typeof(enPostType),enumVal).ToString()
                            });
            }
            return selectList;
        }
        private string GetPostType(enPostType value)
        {
            var memberInfo = value.GetType().GetMember(value.ToString());
            if (memberInfo.Length != 1) return null;

            var displayAttr = memberInfo[0].GetCustomAttributes(typeof(IODescriptionAttribute),false).ToString();

            return displayAttr;
        }
       
    }
   
}
