using SportsZone.Helpers.Authority;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace SportsZone.Controllers
{
    [Authorized]
    //[OnlyForAdmin]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index() => RedirectToAction("me", "account");
        [ActionName("users")]// users list 
        public ActionResult Users()
        {
            using (var context = new Entities())
            {
                List <users> usrs = (from u in context.users
                                  select u).ToList();
                return View(usrs);
            }
        }
        public ActionResult Clubs()
        {
            using (var context = new Entities())
            {
                List<clubs> clbs = (from cb in context.clubs
                                    select cb)
                                    .ToList();
                return View(clbs);
            }
        }
        public ActionResult Players()
        {
            using (var context = new Entities())
            {
                List<players> plyrs = (from pl in context.players
                                       select pl)
                                       .Include("games_positions")
                                       .ToList();
                return View(plyrs);
            }
        }
        public ActionResult Coachs()
        {
            using (var context = new Entities())
            {
                List<coachs> cchs = (from cb in context.coachs
                                     select cb)
                                     .Include("games_positions")
                                     .ToList();
                return View(cchs);
            }
        }
        [HttpPost]
        public JsonResult BanUnbanUser(string userid, string banunban)
        {
            int uid = int.Parse(userid);
            using (var context = new Entities())
            {
                bool sts = false;
                var update = context.users.Find(uid);
                if (banunban == "ban")
                {
                    sts = false;
                }
                else if (banunban == "unban")
                {
                    sts = true;
                }
                update.C_status = sts;
                context.Entry(update).State = EntityState.Modified;
                context.SaveChanges();
                return Json(new { Msg = "Required action has been successfully performed!" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult MarkPayment(string userid)
        {
            int uid = int.Parse(userid);
            using (var context = new Entities())
            {
                var update = context.users.Find(uid);
                update.ispayment = true;
                context.Entry(update).State = EntityState.Modified;
                context.SaveChanges();
                return Json(new { Msg = "Payment marked as cleared!!" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}