using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace eXml.Models
{
    public class RolesCheckBoxListEditModel
    {
            public int Id { get; set; }
            public IEnumerable<SelectListItem> Roles { get; set; }
            public int[] RoleIds { get; set; }
    }    
}