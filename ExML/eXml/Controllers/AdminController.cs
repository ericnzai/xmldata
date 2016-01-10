using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eXml.Models;
using eXml.Entities;
using System.Configuration;
using iPay.Security.Encryption;
namespace eXml.Controllers
{
    [T2TAuthorize]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Users()
        {
            return View(AdminServiceProvider.Users());
        }
        [ValidateInput(false)]
        public ActionResult _GridListUsers()
        {
            return PartialView("_GridListUsers", AdminServiceProvider.Users());
        }
        public ActionResult AddUser(UserModel model)
        {
            if (ModelState.IsValid)
            {
                string cryptoKey = ConfigurationManager.AppSettings["CryptoKey"].ToString();
                Crypto.Key = cryptoKey;
                Crypto.EncryptionAlgorithm = Crypto.Algorithm.DES;
                
                try
                {
                    string encryptPwd ="" ;
                    if (Crypto.EncryptString(model.Password))
                    {
                       
                        encryptPwd = Crypto.Content;
                    }
                    User u = new User
                    {
                        Email = model.Email,
                        Password = encryptPwd,
                        IsLicensed = model.IsLicensed,
                        ExpiryDate = model.ExpiryDate
                    };

                    using (var db = new eXmlContext())
                    {
                        db.Users.Add(u);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            else
            {
                ViewData["EditError"] = "Please correct all errors";
                ViewData["User"] = model;
            }
            return PartialView("_GridListUsers",AdminServiceProvider.Users());
        }
        public ActionResult UpdateUser(UserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new eXmlContext())
                    {
                        var user = db.Set<User>()
                            .SingleOrDefault(x => x.UserId == model.UserId);

                        user.Email = model.Email;
                        user.Password = model.Password;
                        user.IsLicensed = model.IsLicensed;
                        user.ExpiryDate = model.ExpiryDate;

                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }

            }
            else
            {
                ViewData["EditError"] = "Please correct all errors";
                ViewData["User"] = model;
            }
            return PartialView("_GridListUsers", AdminServiceProvider.Users());
        }
        public ActionResult Roles()
        {
            return View(AdminServiceProvider.Roles());
        }
        public ActionResult _GridListRoles()
        {
            return PartialView("_GridListRoles", AdminServiceProvider.Roles());
        }
        public ActionResult AddRole(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Role r = new Role
                    {
                        RoleName = model.RoleName,
                        RoleType = model.RoleType
                    };
                    using (var db = new eXmlContext())
                    {
                        db.Roles.Add(r);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Please correct all errors";
                ViewData["Role"] = model;
            }
            return PartialView("_GridListRoles", AdminServiceProvider.Roles());
        }
        public ActionResult UpdateRole(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new eXmlContext())
                    {
                        var role = db.Set<Role>()
                            .SingleOrDefault(x => x.RoleId == model.RoleId);

                        role.RoleName = model.RoleName;
                        role.RoleType = model.RoleType;

                        db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }

            }
            else
            {
                ViewData["EditError"] = "Please correct all errors";
                ViewData["Role"] = model;
            }
            return PartialView("_GridListRoles", AdminServiceProvider.Roles());
        }
        [HttpGet]
        public ActionResult SetRole(int userId)
        {
            var  model = new RolesCheckBoxListEditModel();
            using (var db = new eXmlContext())
            {
                User user;
                user = db.Set<User>().FirstOrDefault(x => x.UserId == userId);
                var allRoles = db.Set<Role>().ToList();
                if (user != null)
                {
                    IEnumerable<SelectListItem> listRoles = user.Roles.ToCheckBoxRolesListSource(allRoles);
                    for (var i = 0; i < allRoles.Count(); i++)
                    {
                        listRoles.ElementAt(i).Text = allRoles.ElementAt<Role>(i).RoleName;
                    }
                    model.Roles = listRoles;
                    model.Id = user.UserId;
                }
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult SetRole(RolesCheckBoxListEditModel model)
        {
            using (var db = new eXmlContext())
            {
                User user = db.Users.Find(model.Id);
                user.Roles.UpdateRoleCollectionFromModel(db.Roles, model.RoleIds);
                db.SaveChanges();
            }
            ViewData["Message"] = "Roles for this user have been set successfully!";
            return RedirectToAction("SetRole", new { userId = model.Id });
        }
    }

}
