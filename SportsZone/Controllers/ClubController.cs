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
    [OnlyForClub]
    public class ClubController : Controller
    {
        // GET: Club
        public ActionResult Index() => RedirectToAction("me", "account");
        //get teams which are already associated with club
        [ActionName("teams")]
        public ActionResult Teams()
        {
            List<users> ul = (List<users>)Session["Data"];
            int userid = ul[0].userid;
            using (var context = new Entities())
            {
                var teams = (from t in context.teams
                             where t.clubs.users.userid == userid
                             select t)
                             .ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("teams", teams);
            }
        }
        //create  new team in portal
        [ActionName("create-team")]
        public ActionResult CreateTeam()
        {
            using (var context = new Entities())
            {
                var games = (from g in context.games
                             select g).ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("createteam", games);
            }
        }
        [HttpPost]
        public ActionResult CreateTeam(string gameid, string name, HttpPostedFileBase photo1, HttpPostedFileBase photo2)
        {
            if (gameid == null || name == null || photo1 == null || photo2 == null)
            {
                TempData["Message"] = "Values shouldn't be empty!";
                return RedirectToAction("create-team");
            }
            else
            {
                List<users> ul = (List<users>)Session["Data"];
                int userid = ul[0].userid;
                string photo = "";
                string cover = "";
                if (photo1 != null)
                {
                    string pic = System.IO.Path.GetFileName(photo1.FileName);
                    string newname = "spz-" + pic.Split('.')[0] + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + "." + pic.Split('.')[1];
                    string path = System.IO.Path.Combine(
                    Server.MapPath("~/uploads/media"), newname);
                    // file is uploaded
                    photo1.SaveAs(path);
                    photo = newname;
                }
                if (photo2 != null)
                {
                    string pic = System.IO.Path.GetFileName(photo2.FileName);
                    string newname = "spz-" + pic.Split('.')[0] + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + "." + pic.Split('.')[1];
                    string path = System.IO.Path.Combine(
                    Server.MapPath("~/uploads/media"), newname);
                    // file is uploaded
                    photo2.SaveAs(path);
                    cover = newname;
                }
                using (var context = new Entities())
                {
                    int clubid = (from c in context.clubs where c.userid == userid select c.clubid).SingleOrDefault();
                    teams tm = new teams
                    {
                        clubid = clubid,
                        gameid = int.Parse(gameid),
                        name = name,
                        logo = photo,
                        cover = cover
                    };
                    context.teams.Add(tm);
                    //adding games to clubs
                    var update = context.clubs.Find(clubid);
                    update.cg = update.cg + gameid + ",";
                    context.Entry(update).State = EntityState.Modified;
                    //save changes to db
                    context.SaveChanges();
                    TempData["Message"] = "New team has been created!";
                    return RedirectToAction("teams");
                };
            }
        }
        //your schedual matchs
        [ActionName("match-scheduals")]
        public ActionResult MatchScheduals()
        {
            List<users> ul = (List<users>)Session["Data"];
            int userid = ul[0].userid;
            using (var context = new Entities())
            {
                int clubid = (from c in context.clubs where c.userid == userid select c.clubid).SingleOrDefault();
                var matchs = (from m in context.matches
                              where m.club1 == clubid
                              select m)
                              .Include("clubs1")
                              .Include("games")
                              .ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("MatchScheduals", matchs);
            }
        }
        //match schedual requests made by user
        [ActionName("match-schedual-requests")]
        public ActionResult MatchSchedualRequests()
        {
            List<users> ul = (List<users>)Session["Data"];
            int userid = ul[0].userid;
            using (var context = new Entities())
            {
                int clubid = (from c in context.clubs where c.userid == userid select c.clubid).SingleOrDefault();
                var matchs = (from m in context.matches
                              where m.club2 == clubid
                              select m)
                              .Include("clubs")
                              .Include("games")
                              .ToList();
                return View("MatchSchedualRequests", matchs);
            }
        }
        //request new match schedaul
        [ActionName("schedual-match")]
        public ActionResult SchedualMatch()
        {
            using (var context = new Entities())
            {
                var clubs = (from c in context.clubs
                             select c).ToList();
                var games = (from g in context.games
                             select g).ToList();
                ViewBag.Clubs = clubs;
                ViewBag.Games = games;
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("schedualmatch");
            }
        }
        [HttpPost]
        public ActionResult SchedualMatch(string club2id, string gameid, string loca, DateTime dateof)
        {
            List<users> ul = (List<users>)Session["Data"];
            int userid = ul[0].userid;
            using (var context = new Entities())
            {
                int clubid = (from c in context.clubs where c.userid == userid select c.clubid).SingleOrDefault();
                matches mch = new matches
                {
                    club1 = clubid,
                    club2 = int.Parse(club2id),
                    gameid = int.Parse(gameid),
                    loc = loca,
                    C_date = dateof,
                    C_status = true
                };
                context.matches.Add(mch);
                context.SaveChanges();
                TempData["Message"] = "Match scdual request has been made!";
                return RedirectToAction("match-scheduals");
            }
        }
        //accept/reject requests
        [HttpPost]
        public JsonResult WhatToDoWithRequest(string matchid, string yesno)
        {
            using (var context = new Entities())
            {
                string rspns = "";
                if (yesno == "yes") rspns = "Accepted";
                else rspns = "Rejected";
                var update = context.matches.Find(int.Parse(matchid));
                update.C_status = false;
                update.ov = rspns;
                context.Entry(update).State = EntityState.Modified;
                context.SaveChanges();
                return Json(new { Msg = "Match request has been successfully updated!" }, JsonRequestBehavior.AllowGet);
            }
        }

        // input match results
        [ActionName("match-result")]
        public ActionResult MatchResult()
        {
            List<users> ul = (List<users>)Session["Data"];
            int userid = ul[0].userid;
            using (var context = new Entities())
            {
                int clubid = (from c in context.clubs where c.userid == userid select c.clubid).SingleOrDefault();
                var mchs = (from m in context.matches
                            where m.club1 == clubid && m.ov == "Accepted"
                            select m)
                            .Include("clubs1")
                            .Include("games")
                            .ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("MatchResult", mchs);
            }
        }
        [HttpPost]
        public ActionResult MatchResult(string matchid, string status)
        {
            List<users> ul = (List<users>)Session["Data"];
            int userid = ul[0].userid;
            int mid = int.Parse(matchid);
            using (var context = new Entities())
            {
                if ((from cc in context.match_result where cc.mid == mid select cc).Count() == 2)
                {
                    TempData["Message"] = "This match has already been added to result table!";
                    return RedirectToAction("match-result");
                }
                else
                {
                    int clubid = (from c in context.clubs where c.userid == userid select c.clubid).SingleOrDefault();
                    int club2id = (from c2 in context.matches where c2.mid == mid select c2.club2).SingleOrDefault();
                    if (status == "win")
                    {
                        match_result mr = new match_result
                        {
                            mid = mid,
                            clubid = clubid,
                            points = 100,
                            C_status = "W"
                        };
                        match_result mr1 = new match_result
                        {
                            mid = mid,
                            clubid = club2id,
                            points = 10,
                            C_status = "L"
                        };
                        context.match_result.Add(mr);
                        context.match_result.Add(mr1);
                        context.SaveChanges();
                    }
                    else if (status == "lose")
                    {
                        match_result mr = new match_result
                        {
                            mid = mid,
                            clubid = club2id,
                            points = 100,
                            C_status = "W"
                        };
                        match_result mr1 = new match_result
                        {
                            mid = mid,
                            clubid = clubid,
                            points = 10,
                            C_status = "L"
                        };
                        context.match_result.Add(mr);
                        context.match_result.Add(mr1);
                        context.SaveChanges();
                    }
                    else if (status == "draw")
                    {
                        match_result mr = new match_result
                        {
                            mid = mid,
                            clubid = clubid,
                            points = 50,
                            C_status = "D"
                        };
                        match_result mr1 = new match_result
                        {
                            mid = mid,
                            clubid = club2id,
                            points = 50,
                            C_status = "D"

                        };
                        context.match_result.Add(mr);
                        context.match_result.Add(mr1);
                        context.SaveChanges();
                    }

                }
                TempData["Message"] = "Results have been saved to result table!";
                return RedirectToAction("match-result");
            }
        }
        //player association requests with clubs
        [ActionName("player-requests")]
        public ActionResult PlayerAssociationRequest()
        {
            List<users> ul = (List<users>)Session["Data"];
            int userid = ul[0].userid;
            using (var context = new Entities())
            {
                int clubid = (from c in context.clubs where c.userid == userid select c.clubid).SingleOrDefault();
                List<player_associations_request> pas = (from pa in context.player_associations_request
                                                         where pa.clubid == clubid
                                                         select pa)
                                                 .Include("players")
                                                 .Include("teams")
                                                 .Include("games_positions")
                                                 .ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("playerassociationrequest", pas);
            }
        }
        //players that are enrolled with clubs
        [ActionName("enrolled-players")]
        public ActionResult EnrolledPlayers()
        {
            List<users> ul = (List<users>)Session["Data"];
            int userid = ul[0].userid;
            using (var context = new Entities())
            {
                int clubid = (from c in context.clubs where c.userid == userid select c.clubid).SingleOrDefault();
                List<player_associations> pas = (from pa in context.player_associations
                                                 where pa.clubid == clubid
                                                 select pa)
                                                 .Include("players")
                                                 .Include("teams")
                                                 .Include("games_positions")
                                                 .ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("enrolledplayers", pas);
            }
        }
        //remove player from club
        [HttpPost]
        public JsonResult RemovePlayer(string paid)
        {
            int cid = int.Parse(paid);
            using (var context = new Entities())
            {
                var update = context.player_associations.Find(cid);
                context.Entry(update).State = EntityState.Deleted;
                context.SaveChanges();
            }
            return Json(new { Msg = "Player is removed from specific team!" }, JsonRequestBehavior.AllowGet);
        }
        //accept / reject player's request
        [HttpPost]
        public JsonResult WhatToDoWithPlayer(string parid, string yesno)
        {
            int cid = int.Parse(parid);
            using (var context = new Entities())
            {
                if (yesno == "no")
                {
                    var update = context.player_associations_request.Find(cid);
                    update.parstatus = false;
                    context.Entry(update).State = EntityState.Modified;
                    context.SaveChanges();
                    return Json(new { Msg = "Player's request has been rejected and closed!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<player_associations_request> par = (from pa in context.player_associations_request
                                                             where pa.parid == cid
                                                             select pa
                                                             ).ToList();
                    player_associations plyas = new player_associations
                    {
                        playerid=par[0].playerid,
                        clubid=par[0].clubid,
                        teamid=par[0].teamid,
                        roleid=par[0].roleid,
                        C_date=DateTime.Now
                    };
                    context.player_associations.Add(plyas);
                    context.SaveChanges();
                    //closing request
                    var update = context.player_associations_request.Find(cid);
                    update.parstatus = false;
                    context.Entry(update).State = EntityState.Modified;
                    context.SaveChanges();
                    return Json(new { Msg = "Player's request has been accepted and closed!" }, JsonRequestBehavior.AllowGet);
                }
            }
            
        }
        //coach association requests with clubs
        [ActionName("coach-requests")]
        public ActionResult CoachAssociationRequest()
        {
            List<users> ul = (List<users>)Session["Data"];
            int userid = ul[0].userid;
            using (var context = new Entities())
            {
                int clubid = (from c in context.clubs where c.userid == userid select c.clubid).SingleOrDefault();
                List<coach_associations_request> pas = (from pa in context.coach_associations_request
                                                        where pa.clubid == clubid
                                                        select pa)
                                                 .Include("coachs")
                                                 .Include("teams")
                                                 .Include("games_positions")
                                                 .ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("coachassociationrequest", pas);
            }
        }
        //coachs that are enrolled with clubs
        [ActionName("enrolled-coachs")]
        public ActionResult EnrolledCoachs()
        {
            List<users> ul = (List<users>)Session["Data"];
            int userid = ul[0].userid;
            using (var context = new Entities())
            {
                int clubid = (from c in context.clubs where c.userid == userid select c.clubid).SingleOrDefault();
                List<coach_associations> pas = (from pa in context.coach_associations
                                                        where pa.clubid == clubid
                                                        select pa)
                                                 .Include("coachs")
                                                 .Include("teams")
                                                 .Include("games_positions")
                                                 .ToList();
                if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
                return View("enrolledcoachs", pas);
            }
        }
        [HttpPost]
        public JsonResult RemoveCoach(string caid)
        {
            int cid = int.Parse(caid);
            using (var context = new Entities())
            {
                var update = context.coach_associations.Find(cid);
                context.Entry(update).State = EntityState.Deleted;
                context.SaveChanges();
            }
            return Json(new { Msg = "Coach is removed from specific team!" }, JsonRequestBehavior.AllowGet);
        }
        //accept / reject player's request
        [HttpPost]
        public JsonResult WhatToDoWithCoach(string carid, string yesno)
        {
            int cid = int.Parse(carid);
            using (var context = new Entities())
            {
                if (yesno == "no")
                {
                    var update = context.coach_associations_request.Find(cid);
                    update.carstatus = false;
                    context.Entry(update).State = EntityState.Modified;
                    context.SaveChanges();
                    return Json(new { Msg = "Coach's request has been rejected and closed!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<coach_associations_request> par = (from pa in context.coach_associations_request
                                                             where pa.carid == cid
                                                             select pa
                                                             ).ToList();
                    coach_associations plyas = new coach_associations
                    {
                        coachid = par[0].coachid,
                        clubid = par[0].clubid,
                        teamid = par[0].teamid,
                        positionid = par[0].positionid,
                        C_date = DateTime.Now
                    };
                    context.coach_associations.Add(plyas);
                    context.SaveChanges();
                    //closing request
                    var update = context.coach_associations_request.Find(cid);
                    update.carstatus = false;
                    context.Entry(update).State = EntityState.Modified;
                    context.SaveChanges();
                    return Json(new { Msg = "Coach's request has been accepted and closed!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}