using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eXml.Entities;
using DevExpress.Web;
using System.ComponentModel.DataAnnotations;
namespace eXml.Models
{
    public class UploadFileModel
    {
        public string Company { get; set; }
       // public HttpPostedFile File { get; set; }
        public string Date { get; set; }
        //[Required]
        //public enPostType Type { get; set; }
    }
}