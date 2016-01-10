using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eXml.Models;
using eXml.Entities;

using System.Web.Script.Serialization;
using System.Web.Security;

using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace eXml.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();    
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                user = AdminServiceProvider.Login(model);

                if (user != null)
                {
                    if (user.IsLicensed)
                    {
                        if (user.ExpiryDate >= DateTime.Now)
                        {
                            CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                            serializeModel.Id = user.UserId;
                            serializeModel.Email = user.Email;
                            //serializeModel.Role = user.Roles;
                            serializeModel.IsLicensed = user.IsLicensed;
                            serializeModel.ExpiryDate = user.ExpiryDate;

                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            string userData = serializer.Serialize(serializeModel);

                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, model.Email,
                                DateTime.Now, DateTime.Now.AddMinutes(15), false, userData);
                            string encTicket = FormsAuthentication.Encrypt(authTicket);

                            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                            Response.Cookies.Add(faCookie);

                            ViewData["userFile"] = UpdateUserMainMenuXml(user.UserId);
                            return RedirectToAction("UploadFile", "UploadControl");
                        }else
                        ViewData["ErrorMsg"] = "Kindly, Be notified that your license has expired ! Expiry Date: " + user.ExpiryDate;
                    } else
                    ViewData["ErrorMsg"] = "Your license is not activated! Expiry Date is " + user.ExpiryDate;
                }else
                ViewData["ErrorMsg"] = "Wrong Email and/or password! ";
            }
            return View(model);
        }
        public ActionResult LogOff()
        {
            Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        public string UpdateUserMainMenuXml(int userId)
        {
            bool isAdmin = false;
             string userFile;
            using (var db = new eXmlContext())
            {
                User user = db.Set<User>().Where(x => x.UserId == userId).First();

                foreach (var role in user.Roles)
                {
                    //check if user has admin role
                    if (role.RoleType == enRoleType.Admin)
                    {
                        isAdmin = true;
                    }
                }
                string fileName = Path.Combine(Server.MapPath("~/App_Data"), "main.xml");
                XmlTextReader reader = new XmlTextReader(fileName);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                reader.Close();

                XmlNode adminNode;
                XmlElement root = doc.DocumentElement;
                userFile = "newmenu.xml";
                string userFilePath = Path.Combine(Server.MapPath("~/App_Data"), userFile);
                if (isAdmin == false) // remove admin child node
                {
                    adminNode = root.SelectSingleNode("/mainmenu/item[@Role='Admin']");
                    root.RemoveChild(adminNode);
                    doc.Save(userFilePath);
                }
                else doc.Save(userFilePath);
            }
            return userFile;
        }
    }
}