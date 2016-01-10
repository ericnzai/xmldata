using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace eXml.Models
{
    public class CustomPrincipal :ICustomPrincipal
    {
        public CustomPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
        }
        public IIdentity Identity { get; set; }
        public bool IsInRole(string role)
        {
            return false;
        }
        public int Id { get; set; }
        public string Email { get; set; }
        //public string Role { get; set; }
        public bool IsLicensed { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}