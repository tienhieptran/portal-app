using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http.Filters;
using System.Web.Mvc;
using ActionFilterAttribute = System.Web.Mvc.ActionFilterAttribute;

namespace bicen.Helpers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CheckSessionTimeOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;
            if (context.Session != null)
            {
                if (context.Session.IsNewSession)
                {
                    string sessionCookie = context.Request.Headers["Cookie"];
                    string id = WebConfigurationManager.AppSettings["SESSION_NAME_USERID"];
                    if ((sessionCookie != null) && (sessionCookie.IndexOf(id) >= 0))
                    {
                        System.Web.Security.FormsAuthentication.SignOut();
                        string redirectTo = "~/Home/Login";
                        if (!string.IsNullOrEmpty(context.Request.RawUrl))
                        {
                            redirectTo = string.Format("~/Home/Login?ReturnUrl={0}", HttpUtility.UrlEncode(context.Request.RawUrl));
                        }
                        filterContext.HttpContext.Response.Redirect(redirectTo, true);
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}