using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsZone.Controllers
{
    public class MembershipController : Controller
    {
        // GET: Membership
        public ActionResult Index() => RedirectToAction("me", "account");
        public ActionResult Dues() => View();
    }
}