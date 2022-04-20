using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsZone.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [ActionName("pricing")]
        public ActionResult Pricing()
        {
            return View();
        }
        [ActionName("players")]
        public ActionResult Players(string id, string name)
        {
            if (id == null)
            {
                try
                {
                    using (var context = new Entities())
                    {
                        List<players> player = (from p in context.players
                                                where p.playername == null || DbFunctions.Like(p.playername, "%" + name + "%")
                                                select p)
                                                .Include("users")
                                                .Include("games_positions")
                                                .ToList();
                        return View("players", player);
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View("error404");
                }
            }
            else
            {
                try
                {
                    using (var context = new Entities())
                    {
                        List<players> player = (from p in context.players
                                                where p.users.username == id
                                                select p)
                                                .Include("users")
                                                .Include("games_positions")
                                                .ToList();
                        if (player.Count == 1)
                        {
                            List<player_associations> pa = (from pl in context.player_associations
                                                            where pl.players.users.username == id
                                                            select pl)
                                                            .Include("teams")
                                                            .Include("clubs")
                                                            .Include("clubs.users")
                                                            .ToList();
                            if (pa.Count == 0) ViewBag.PlayerAssociations = null;
                            else ViewBag.PlayerAssociations = pa;
                            return View("playerdetail", player[0]);
                        }
                        else
                        {
                            ViewBag.Message = "Player not found!";
                            return View("error404");
                        }

                        
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View("error404");
                }
            }
        }
        [ActionName("coachs")]
        public ActionResult Coachs(string id, string name)
        {
            if (id == null)
            {
                try
                {
                    using (var context = new Entities())
                    {
                        List<coachs> coachs = (from p in context.coachs
                                                where p.name == null || DbFunctions.Like(p.name, "%" + name + "%")
                                                select p)
                                                .Include("users")
                                                .Include("games_positions")
                                                .ToList();
                        return View("coachs", coachs);
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View("error404");
                }
            }
            else
            {
                try
                {
                    using (var context = new Entities())
                    {
                        List<coachs> coach = (from p in context.coachs
                                                where p.users.username == id
                                                select p)
                                                .Include("users")
                                                .Include("games_positions")
                                                .ToList();
                        if (coach.Count == 1)
                        {
                            List<coach_associations> ca = (from pl in context.coach_associations
                                                            where pl.coachs.users.username == id
                                                            select pl)
                                                            .Include("teams")
                                                            .Include("teams.games")
                                                            .Include("clubs")
                                                            .ToList();
                            if (ca.Count == 0) ViewBag.CoachAssociations = null;
                            else ViewBag.CoachAssociations = ca;
                            return View("coachdetail", coach[0]);
                        }
                        else
                        {
                            ViewBag.Message = "Coach not found!";
                            return View("error404");
                        }


                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View("error404");
                }
            }
        }
        [ActionName("clubs")]
        public ActionResult Clubs(string id, string city, int? game)
        {
            try
            {
                if (city == "all") city = null;
                if (game == 0) game = null;
                if (id == null)
                {
                    using (var context = new Entities())
                    {
                        // if (city == null) city = "";
                        // if (name == null) name = "";
                        List<games> gms = (from gm in context.games select gm).ToList();
                        List<clubs> clubd = new List<clubs>();
                        List<clubs> cls = (from c in context.clubs
                                           where 
                                                 (c.city == null || DbFunctions.Like(c.city, "%" + city + "%")) && 
                                                 (c.cg == null || DbFunctions.Like(c.cg, "%" + game +","+ "%"))
                                           select c).ToList();
                                                 

                        for (int i = 0; i < cls.Count; i++)
                        {
                            { 
                            int uid = cls[i].userid;
                            int cid = cls[i].clubid;
                                string username = (from u in context.users where u.userid == uid select u.username).SingleOrDefault();
                            int tcount = (from c in context.teams where c.clubid == cid select c).Count();
                            int ccount = (from cc in context.coach_associations where cc.coachid == cid select cc).Count();
                           
                            clubs clbd = new clubs
                            {
                                clubid = cls[i].clubid,
                                userid = cls[i].userid,
                                clubname = cls[i].clubname + "♦" + username + "♦" + tcount + "♦" + ccount,
                                city = cls[i].city,
                                C_address = cls[i].C_address,
                                C_state= cls[i].C_state,
                                lat = cls[i].lat,
                                @long = cls[i].@long,
                                logo = cls[i].logo,
                                cover = cls[i].cover
                            };
                            clubd.Add(clbd);
                            }
                        }
                        ViewBag.Games = gms;
                        return View(clubd);
                    }
                }
                else
                {
                    using (var context = new Entities()) {
                        int? uid = (from u in context.users where u.username == id select u.userid).SingleOrDefault();
                        if (uid != null)
                        {
                            List<clubs> clb = (from c in context.clubs where c.userid == uid select c).ToList();
                            int cid = clb[0].clubid; //club id
                            // Get Teams Data
                            List<teams> tms = (from t in context.teams where t.clubid == cid select t).ToList();
                            List<teams> teams = new List<teams>();
                            for (int i = 0; i < tms.Count; i++)
                            {
                                int teamid = tms[i].teamid;
                                int gameid = tms[i].gameid;
                                int totalplayerinteam = (from tpt in context.player_associations where tpt.teamid == teamid select tpt).Count();
                                int totalcoachsinteam = (from tct in context.coach_associations where tct.teamid == teamid select tct).Count();
                                string gamename = (from gn in context.games where gn.gameid == gameid select gn.gamename).SingleOrDefault();
                                teams tm = new teams()
                                {
                                    teamid = teamid,
                                    name = tms[i].name + "♦" + gamename + "♦" + totalplayerinteam + "♦" + totalcoachsinteam,
                                    logo = tms[i].logo
                                };
                                teams.Add(tm);
                            }
                            //Get Coachs Data
                            List<int> chs = (from ch in context.coach_associations where ch.clubid == cid select ch.coachid).ToList();

                            //Get Coachs - Complete data ------------------
                            List<coachs> ourcoachs = new List<coachs>();
                            for (int i = 0; i < chs.Count; i++)
                            {
                                int chid = chs[i];
                                List<coachs> coachs = (from c_c in context.coachs where c_c.coachid==chid select c_c).ToList();
                                int? positionid = coachs[0].positionid;
                                string position = (from pos in context.games_positions where pos.positionid == positionid select pos.position).SingleOrDefault();
                                //get user data
                                int userid = coachs[0].userid;
                                List<users> users = (from usr in context.users where usr.userid == userid select usr).ToList();
                                coachs _coach = new coachs
                                {
                                    coachid = coachs[0].coachid,
                                    userid = coachs[0].userid,
                                    name = coachs[0].name + "♦" + users[0].username + "♦" + position,
                                    age = coachs[0].age,
                                    picture= coachs[0].picture,
                                };
                                ourcoachs.Add(_coach);
                            }
                            // get top 3 players
                            List<player_associations> pa = (from plas in context.player_associations where plas.clubid == cid select plas).ToList();
                            List<players> asplayers = new List<players>();
                            int j = 0;
                            if (pa.Count >= 3) j = 3; else j = pa.Count;
                            for (int i = 0; i < j; i++)
                            {
                                int roleid = pa[i].roleid;
                                int playerid = pa[i].playerid;
                                string position = (from rnam in context.games_positions where rnam.positionid == roleid select rnam.position).SingleOrDefault();
                                List<players> player = (from ply in context.players where ply.playerid == playerid select ply).ToList();
                                players plyr = new players
                                {
                                    playerid = playerid,
                                    playername = player[0].playername + "♦" + position,
                                    age= player[0].age,
                                    photo= player[0].photo,
                                    height= player[0].height,
                                };
                                asplayers.Add(plyr);
                            }
                            ViewBag.Players = asplayers;
                            ViewBag.Teams = teams;
                            ViewBag.Coachs = ourcoachs;
                            return View("clubdetail", clb);
                        }
                        else
                        {
                            ViewBag.Message = "Club not found!";
                            return View("error404");
                        }   
                    }

                }
                
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View("error404");
            }
        }
        [ActionName("teams")]
        public ActionResult Teams(int? id)
        {

            if (id == null)
                return RedirectToAction("index");
            else
            {
                using (var context = new Entities())
                {
                    List<teams> team = (from t in context.teams
                                         where t.teamid == id
                                         select t)
                                         .Include("games")
                                         .ToList();
                    if (team.Count > 0)
                    {
                        List<player_associations> pl = (from p in context.player_associations
                                            where p.teamid == id
                                            select p)
                                            
                                            .Include("players.users")
                                            .Include("players.games_positions")
                                                        .ToList();
                        if (pl.Count > 0) ViewBag.Players = pl;
                        else ViewBag.Players = null;
                        return View("teamdetail", team[0]);
                    }
                    else
                    {
                        ViewBag.Message = "Team Not Found!";
                        return View("error404");
                    }
                }
                    
            }
        }
        [ActionName("matchs")]
        public ActionResult Matchs()
        {
            using (var context = new Entities())
            {
                List<matches> mtchs = (from m in context.matches
                                       where m.ov == "Accepted"
                                       select m)
                                       .Include("clubs")
                                       .Include("clubs1")
                                       .Include("games")
                                       .ToList();
                return View(mtchs);
            }
                
        }
        [ActionName("match-results")]
        public ActionResult MatchrResults()
        {
            using (var context = new Entities())
            {
                List<match_result> mtchs = (from m in context.match_result
                                       select m)
                                       .Include("matches")
                                       .Include("matches.games")
                                       .Include("clubs")
                                       .ToList();
                return View("MatchrResults", mtchs);
            }
                
        }
    }
}