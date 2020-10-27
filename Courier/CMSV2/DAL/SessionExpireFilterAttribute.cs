using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace CMSV2.DAL
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    HttpContext ctx = HttpContext.Current;
        //    // check if session is supported  
        //    if (ctx.Session != null)
        //    {
        //        // check if a new session id was generated  
        //        if (ctx.Session["UserID"] == null) // || ctx.Session.IsNewSession)
        //        {
        //            filterContext.Result = new RedirectResult("~/Login/Login");
        //            return;
        //            ////Check is Ajax request  
        //            //if (filterContext.HttpContext.Request.IsAjaxRequest())
        //            //{
        //            //    filterContext.HttpContext.Response.ClearContent();
        //            //    filterContext.HttpContext.Items["AjaxPermissionDenied"] = true;
        //            //}
        //            //// check if a new session id was generated  
        //            //else
        //            //{
        //            //    filterContext.Result = new RedirectResult("~/Login/Login");
        //            //}
        //        }               
        //    }
        //    base.HandleUnauthorizedRequest(filterContext);
        //}

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            // check  sessions here
            if (HttpContext.Current.Session["UserID"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Home");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}