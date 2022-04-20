using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SportsZone.Controllers
{
    public class MiscController : Controller
    {
        [HttpGet]
        public JsonResult GetAllClubs()
        {
            using (var context = new Entities())
            {
                var clbs = (from c in context.clubs select c).ToList();
                return Json(clbs, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetTeams(string clubid)
        {
            int cid = int.Parse(clubid);
            Entities context = new Entities();
            context.Configuration.ProxyCreationEnabled = false;
            var team = (from t in context.teams
                        where t.clubid==cid
                        select t).ToList();
            return new JsonResult { Data = team, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}