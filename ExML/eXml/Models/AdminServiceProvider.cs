using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using eXml.Entities;
using eXml.Models;
using System.Collections;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using System.Security;
using iPay.Security.Encryption;

namespace eXml.Models
{
    public static class AdminServiceProvider
    {
        public static List<UserModel> Users ()
        {
             var users = new List<UserModel>();
            using (var db = new eXmlContext())
            {
                users = db.Set<User>()
                        .Select(x => new UserModel
                        {
                            UserId = x.UserId,
                            Email = x.Email,
                            Password = x.Password,
                            IsLicensed = x.IsLicensed,
                            ExpiryDate = x.ExpiryDate
                        }).ToList();
            }
            return users;
        }
        public static List<RoleModel> Roles()
        {
            var roles = new List<RoleModel>();
            using (var db = new eXmlContext())
            {
                roles = db.Set<Role>()
                        .Select(x => new RoleModel
                        {
                            RoleId = x.RoleId,
                            RoleType = x.RoleType,
                            RoleName = x.RoleName
                        }).ToList();
            }
            return roles;
        }
        public static IEnumerable<Role> ListRoles()
        {
            IEnumerable<Role> roles;
            using (var db = new eXmlContext())
            {
                roles = db.Set<Role>()
                        .ToList();
            }
            return roles;
        }
        public static List<PermissionModel> Permissions()
        {
            var perms = new List<PermissionModel>();
            using (var db = new eXmlContext())
            {
                perms = db.Set<Permission>()
                        .Select(x => new PermissionModel
                        {
                            PermissionId = x.PermissionId,
                            PermissionName = x.PermissionName
                        }).ToList();
            }
            return perms;
        }
        public static User GetUser(int userId)
        {
            User user;
            using (var db = new eXmlContext())
            {
                user = db.Set<User>
                    ().FirstOrDefault(u => u.UserId == userId);
            }
            return user;
        }
        public static IEnumerable<SelectListItem> ToCheckBoxRolesListSource<T>(this IEnumerable<T> checkedCollection,
        IEnumerable<T> allCollection) where T : Role
        {
            var result = new List<SelectListItem>();

            foreach (var allItem in allCollection)
            {
                var selectItem = new SelectListItem();
                //selectItem.Text = allItem.;
                selectItem.Value = allItem.RoleId.ToString();
                selectItem.Selected = (checkedCollection.Count(c => c.RoleId == allItem.RoleId) > 0);
                result.Add(selectItem);
            }

            return result;
        }
        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name,
          IEnumerable<SelectListItem> items, string legendName)
        {

            var output = new StringBuilder();
            output.Append(@"<div class=""controls-group"">");
            //<legend class="legendform">Material Category Details</legend>
            output.Append(@"<fieldset><legend class=""legendform"">");
            output.Append(legendName);
            output.Append("</legend>");
            output.Append(@"<div class=""checkboxList"">");

            foreach (var item in items)
            {
                output.Append(@"<input type=""checkbox"" name=""");
                output.Append(name);
                output.Append("\" value=\"");
                output.Append(item.Value);
                output.Append("\"");

                if (item.Selected)
                    output.Append(@" checked=""checked""");

                output.Append(" />");
                output.Append(item.Text);
                output.Append("<br />");

            }
            output.Append("</div>");
            output.Append("</fieldset>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());

        }
        public static void UpdateRoleCollectionFromModel<T>(this ICollection<T> domainCollection,
           IQueryable<T> objects, int[] newValues) where T : Role
        {
            if (newValues == null)// nothing selected
            {
                domainCollection.Clear();
                return;
            }
            for (var i = domainCollection.Count - 1; i >= 0; i--)
            {
                var domainObject = domainCollection.ElementAt(i);
                if (!newValues.Contains((int)domainObject.RoleId))
                    domainCollection.Remove(domainObject);
            }
            foreach (var newId in newValues)
            {
                var domainObject = domainCollection.FirstOrDefault(t => t.RoleId == newId);
                if (domainObject != null)
                    continue;
                domainObject = objects.FirstOrDefault(t => t.RoleId == newId);
                if (domainObject == null)
                {
                    continue;
                }
                domainCollection.Add(domainObject);
            }
        }
        public static User Login(LoginModel model)
        {
            //bool IsSuccessful = false;
            User user;
            using (var db = new eXmlContext())
            {
                user = db.Set<User>().Where(x => x.Email == model.Email).FirstOrDefault();

                if (user != null)
                {
                    string strPwd = "";
                    string cryptoKey = ConfigurationManager.AppSettings["CryptoKey"].ToString();
                    Crypto.Key = cryptoKey;
                    Crypto.EncryptionAlgorithm = Crypto.Algorithm.DES;
                    Crypto.Content = user.Password;

                    if (Crypto.DecryptString())
                    {
                        strPwd = Crypto.Content;
                    }
                    if (strPwd == model.Password)
                    {
                        return user;
                    }
                    else
                    {
                        user = null;

                    }
                }
            }
            return user;
        }
      
    }
}