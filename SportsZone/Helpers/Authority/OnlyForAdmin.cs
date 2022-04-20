using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SportsZone.Helpers.Authority
{
    public class OnlyForAdmin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            List<users> u = (List<users>)session["Data"];
            if (session != null && u[0].usertype != "Admin")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                                { "Controller", "global" },
                                { "Action", "error-401" }
                                });
            }
        }
    }
}