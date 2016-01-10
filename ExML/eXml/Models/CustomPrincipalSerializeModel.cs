using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eXml.Models
{
    public class CustomPrincipalSerializeModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        //public string Role { get; set; }
        public bool IsLicensed { get; set; }
        public DateTime ExpiryDate { get; set; }

    }
}