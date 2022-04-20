using SportsZone.Helpers.Authority;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsZone.Controllers
{
    public class GlobalController : Controller
    {
        // GET: Global
        [ActionName("error-401")]
        [Authorized]
        public ActionResult Error401()
        {
            return View("Error401");
        }
    }
}