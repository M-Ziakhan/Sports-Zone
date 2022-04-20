using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SportsZone
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "account-routes",
                url: "account/{action}/{id}",
                defaults: new { controller = "account", action = "index", id = UrlParameter.Optional }
            );
            //feedback routes
            routes.MapRoute(
                name: "feedback-routes",
                url: "feedback/{action}/{id}",
                defaults: new { controller = "feedback", action = "index", id = UrlParameter.Optional }
            );
            //player activities
            routes.MapRoute(
                name: "player-activities-routes",
                url: "player-activities/{action}/{id}",
                defaults: new { controller = "playeractivities", action = "index", id = UrlParameter.Optional }
            );
            //coach activities
            routes.MapRoute(
                name: "coach-activities-routes",
                url: "coach-activities/{action}/{id}",
                defaults: new { controller = "coachactivities", action = "index", id = UrlParameter.Optional }
            );
            //club routes
            routes.MapRoute(
                name: "club-activities-routes",
                url: "club-activities/{action}/{id}",
                defaults: new { controller = "club", action = "index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "admin-activities-routes",
                url: "admin-activities/{action}/{id}",
                defaults: new { controller = "admin", action = "index", id = UrlParameter.Optional }
            );
            //misc routes
            routes.MapRoute(
                name: "misc-routes",
                url: "misc/{action}",
                defaults: new { controller = "misc", action = "index" }
                );
            //global authorization error route
            routes.MapRoute(
                name: "global-error-routes",
                url: "global/{action}",
                defaults: new { controller = "global", action = "error-401" }
                );
            // membership routes
            routes.MapRoute(
                name: "membership-routes",
                url: "membership/{action}",
                defaults: new { controller = "membership", action = "index"}
            );
            routes.MapRoute(
                name: "Default",
                url: "{action}/{id}",
                defaults: new { controller = "home", action = "index", id = UrlParameter.Optional }
            );

        }
    }
}
