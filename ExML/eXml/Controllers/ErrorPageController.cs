using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eXml.Controllers
{
    public class ErrorPageController : Controller
    {
        //
        // GET: /ErrorPage/
        [HandleError]
        public ActionResult Error(string aspxerrorpath)
        {
            //Char[] fslash = new Char[1];
            //fslash[0] =Char.Parse("/");
            //string[] errorPath = aspxerrorpath.Split(fslash);
            //string controller = errorPath[1].ToString();
            //string action = errorPath[2].ToString();
           
           
            return View("errorPage");
        }

    }
}
