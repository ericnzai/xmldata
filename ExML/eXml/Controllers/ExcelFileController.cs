using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eXml.Controllers
{
    public class ExcelFileController : Controller
    {
        //
        // GET: /ExcelFile/

        public ActionResult ViewExcel(string fileName)
        {
            return View();
        }
        public ActionResult ViewDocPartial()
        {
            return PartialView("ViewDocPartial");
        }
    }
}
