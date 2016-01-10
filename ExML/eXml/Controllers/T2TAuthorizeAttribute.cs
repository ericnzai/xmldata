using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
namespace eXml.Controllers
{
    [System.AttributeUsage(AttributeTargets.Class)]
    public class T2TAuthorizeAttribute :FilterAttribute, IAuthorizationFilter
    {
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext != null)
            {

                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    HttpContextBase context = filterContext.RequestContext.HttpContext;
                    if (!context.User.Identity.IsAuthenticated)
                    {
                        string url = "~/Home/Index";
                        context.Response.Redirect(url);
                    }
                   
                }
            }
        }
       
    }
}