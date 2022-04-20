using SportsZone.Helpers.Authority;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsZone.Controllers
{
    
    public class FeedbackController : Controller
    {
        // GET: Feedback
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public PartialViewResult loadfbform()
        {
            return PartialView("~/views/shared/partial/feedback/fbform.cshtml");
        }
        [Authorized]
        [HttpPost]
        public ActionResult submit(string userid, string comment, string rating, string returnurl)
        {
            try
            {
                int uid = int.Parse(userid);
                int rt = int.Parse(rating);
                List<users> data = (List<users>)Session["Data"];
                if (uid == data[0].userid)
                    return RedirectToAction(returnurl.Split('/')[1], "home", new { id = returnurl.Split('/')[2] });
                else
                {
                    using (var context = new Entities())
                    {
                        feedback fdback = new feedback
                        {
                            C_to = uid,
                            C_from = data[0].userid,
                            msg = comment,
                            C_date = DateTime.Now,
                            rating = rt
                        };
                        context.feedback.Add(fdback);
                        context.SaveChanges();
                        return RedirectToAction(returnurl.Split('/')[1], "home", new { id = returnurl.Split('/')[2] });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return RedirectToAction(returnurl.Split('/')[1], "home", new { id = returnurl.Split('/')[2] });
            }
            
        }
        [HttpPost]
        public PartialViewResult loadfeedbacks(string userid)
        {
            try
            {
                int uid = int.Parse(userid);
                using (var context = new Entities())
                {
                    List<feedback> fds = (from f in context.feedback
                                          where f.C_to == uid
                                          select f)
                                          .Include("users")
                                          .ToList();
                    return PartialView("~/views/shared/partial/feedback/fbcontainer.cshtml", fds);
                }

            }
            catch (Exception ex)
            {
             return PartialView("~/views/shared/partial/feedback/fderr.cshtml", "BC ye kia bakwas he? Kuch mt ker system ke sath!! "+ ex.Message);
            }
        }
    }
}