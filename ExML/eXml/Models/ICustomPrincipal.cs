using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using WebMatrix.WebData;
using System.Security.Principal;

namespace eXml.Models
{
    public interface ICustomPrincipal : IPrincipal
    {
        int Id { get; set; }
        string Email { get; set; }
        //string Role { get; set; }
        bool IsLicensed { get; set; }
        DateTime ExpiryDate { get; set; }
    }
}
