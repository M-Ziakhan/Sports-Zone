using Newtonsoft.Json;
using SportsZone.Helpers.Authority;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsZone.Controllers
{
    [Authorized]
    [OnlyForPlayer]
    public class PlayerActivitiesController : Controller
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
                List<player_associations> pa = (from p in context.player_associations
                                                where p.players.users.userid == uid
                                                select p)
                                                .Include("players")
                                                .Include("clubs")
                                                .Include("clubs.users")
                                                .Include("teams")
                                                .Include("games_positions")
                                                .ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("Enrollments", pa);
            }
        }
        [HttpPost]
        public JsonResult RemoveEnrollment(string paid, string playerid)
        {
            try
            {
                int pid = int.Parse(playerid);
                int pasid = int.Parse(paid);
                List<users> ul = (List<users>)Session["Data"];
                using (var context = new Entities())
                {
                    int? userid = (from u in context.players where u.playerid == pid select u.userid).SingleOrDefault();
                    if (userid != null && userid == ul[0].userid)
                    {
                        var delas = context.player_associations.Find(pasid);
                        context.Entry(delas).State = EntityState.Deleted;
                        context.SaveChanges();
                        //TempData["Message"] = "You have been removed from the club!";
                        return Json(new { Msg = "You have been removed from the club!" });
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
                List<player_associations_request> pa = (from p in context.player_associations_request
                                                        where p.players.users.userid == uid
                                                        select p)
                                                .Include("players")
                                                .Include("clubs.users")
                                                .Include("teams")
                                                .Include("games_positions")
                                                .ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("EnrollmentRequests", pa);
            }
        }
        [HttpPost]
        public JsonResult WithDrawEnrollmentRequest(string parid, string playerid)
        {
            try
            {
                int pid = int.Parse(playerid);
                int pasid = int.Parse(parid);
                List<users> ul = (List<users>)Session["Data"];
                using (var context = new Entities())
                {
                    int? userid = (from u in context.players where u.playerid == pid select u.userid).SingleOrDefault();
                    if (userid != null && userid == ul[0].userid)
                    {
                        var delas = context.player_associations_request.Find(pasid);
                        if (delas.parstatus == true)
                        {
                            delas.parstatus = false;
                            context.Entry(delas).State = EntityState.Modified;
                            context.SaveChanges();
                            return Json(new { Msg= "Your request has been withdrawn!"}, JsonRequestBehavior.AllowGet);
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
                return Json(new { Msg = "Error occured! "+ ex.Message }, JsonRequestBehavior.AllowGet);
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
                    var player = (from pi in context.players
                                  where pi.userid == userid
                                  select pi).ToList();
                    player_associations_request par = new player_associations_request
                    {
                        playerid = player[0].playerid,
                        clubid = int.Parse(clubid),
                        teamid = int.Parse(teamid),
                        roleid = player[0].roleid,
                        C_date = DateTime.Now,
                        parstatus = true
                    };
                    context.player_associations_request.Add(par);
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
                           where r.C_from==userid
                           select r)
                           .Include("users1")
                           .ToList();
                return View("reviews", rvs);
            }
        }
    }
}