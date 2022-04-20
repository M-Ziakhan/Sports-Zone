using SportsZone.Helpers.Authority;
using SportsZone.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsZone.Controllers
{
    [Authorized]
    [OnlyForCoach]
    public class CoachActivitiesController : Controller
    {
        // GET: PlayerActivities
        public ActionResult Index()
        {
            return RedirectToAction("me", "account");
        }
        [ActionName("my-enrollments")]
        public ActionResult Enrollments()
        {
            List<users> ul = (List<users>)Session["Data"];
            int uid = ul[0].userid;
            using (var context = new Entities())
            {
                List<coach_associations> pa = (from p in context.coach_associations
                                               where p.coachs.users.userid == uid
                                                select p)
                                                .Include("coachs")
                                                .Include("clubs")
                                                .Include("teams")
                                                .Include("games_positions")
                                                .ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("Enrollments", pa);
            }
        }
        [HttpPost]
        public JsonResult RemoveEnrollment(string caid, string coachid)
        {
            try
            {
                int cid = int.Parse(coachid);
                int casid = int.Parse(caid);
                List<users> ul = (List<users>)Session["Data"];
                using (var context = new Entities())
                {
                    int? userid = (from u in context.coachs where u.coachid == cid select u.userid).SingleOrDefault();
                    if (userid != null && userid == ul[0].userid)
                    {
                        var delas = context.coach_associations.Find(casid);
                        context.Entry(delas).State = EntityState.Deleted;
                        context.SaveChanges();
                        //TempData["Message"] = "You have been removed from the club!";
                        return Json(new { Msg = "You have been removed from the club and team!" });
                    }
                    else
                    {
                        return Json(new { Msg = "You don't have permissions to do that!" });
                    }
                }
            }
            catch (Exception ex)
            {
                //TempData["Message"] = ex.Message;
                return Json(new { Msg = "You don't have permissions to do that!" + ex.Message });
            }
        }
        [ActionName("enrollment-requests")]
        public ActionResult EnrollmentRequests()
        {
            List<users> ul = (List<users>)Session["Data"];
            int uid = ul[0].userid;
            using (var context = new Entities())
            {
                List<coach_associations_request> ca = (from p in context.coach_associations_request
                                                        where p.coachs.users.userid == uid
                                                        select p)
                                                .Include("coachs")
                                                .Include("clubs.users")
                                                .Include("teams")
                                                .Include("games_positions")
                                                .ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("EnrollmentRequests", ca);
            }
        }
        [HttpPost]
        public JsonResult WithDrawEnrollmentRequest(string carid, string coachid)
        {
            try
            {
                int cid = int.Parse(coachid);
                int casid = int.Parse(carid);
                List<users> ul = (List<users>)Session["Data"];
                using (var context = new Entities())
                {
                    int? userid = (from u in context.coachs where u.coachid == cid select u.userid).SingleOrDefault();
                    if (userid != null && userid == ul[0].userid)
                    {
                        var delas = context.coach_associations_request.Find(casid);
                        if (delas.carstatus == true)
                        {
                            delas.carstatus = false;
                            context.Entry(delas).State = EntityState.Modified;
                            context.SaveChanges();
                            return Json(new { Msg = "Your request has been withdrawn!" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Msg = "This request has already been witdrawn or processed!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Msg = "You don't have permissions to do that!" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Msg = "Error occured! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [ActionName("create-enrollment-request")]
        public ActionResult CreateEnrollmentRequest()
        {
            using (var context = new Entities())
            {
                var clubs = (from c in context.clubs
                             select c).ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("CreateEnrollmentRequest", clubs);
            }
        }
        [HttpPost]
        public ActionResult CreateEnrollmentRequest(string clubid, string teamid)
        {
            try
            {
                List<users> ul = (List<users>)Session["Data"];
                int userid = ul[0].userid;
                using (var context = new Entities())
                {
                    var coach = (from pi in context.coachs
                                  where pi.userid == userid
                                  select pi).ToList();
                    int poid = coach[0].positionid ?? 1;
                    coach_associations_request par = new coach_associations_request
                    {
                        coachid = coach[0].coachid,
                        clubid = int.Parse(clubid),
                        teamid = int.Parse(teamid),
                        positionid = poid,
                        C_date = DateTime.Now,
                        carstatus = true
                    };
                    context.coach_associations_request.Add(par);
                    context.SaveChanges();
                    TempData["Message"] = "New enrollment requests has been made!";
                    return RedirectToAction("enrollment-requests");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Please input valid data! " + ex.Message;
                return RedirectToAction("create-enrollment-request");
            }


        }
        [ActionName("reviews")]
        public ActionResult Reviews()
        {
            using (var context = new Entities())
            {
                List<users> ul = (List<users>)Session["Data"];
                int userid = ul[0].userid;
                var rvs = (from r in context.feedback
                           where r.C_from == userid
                           select r)
                           .Include("users1")
                           .ToList();
                return View("reviews", rvs);
            }
        }
        [ActionName("your-players")]
        public ActionResult YourPlayers()
        {
            List<users> ul = (List<users>)Session["Data"];
            int userid = ul[0].userid;
            using (var context = new Entities())
            {
                var casteam = (from c in context.coach_associations
                              where c.coachs.users.userid == userid
                              select c)
                              .ToList();
                List<CustomPlayers> pl = new List<CustomPlayers>();
                for (int i = 0; i < casteam.Count; i++)
                {
                    int tid = casteam[i].teamid;
                    var ply = (from p in context.player_associations
                               where p.teamid == tid
                               select p)
                               .Include("players")
                               .Include("players.users")
                               .ToList();
                    for (int j = 0; j < ply.Count; j++)
                    {
                        CustomPlayers p = new CustomPlayers
                        {
                            pid = ply[j].playerid,
                            email = ply[j].players.users.email,
                            name=ply[j].players.playername,
                            phone=ply[j].players.users.phone,
                            role=ply[j].games_positions.position
                        };
                        pl.Add(p);
                    }
                }
                return View("YourPlayers", pl);
            }
                
        }
    }
}