using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BeerNotifier.Models;
using Quartz;
using Quartz.Impl;

namespace BeerNotifier.Controllers
{
    [BeerAdminAuthorize]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReloadSchedule()
        {
            Scheduling.ClearJobs();
            Scheduling.SetupScheduler();
            return Json(new {Success = true});
        }

        public ActionResult NotifyTest(string location)
        {
            Broker b = new Broker();
            b.NotifyBuyer(location);
            return Json(new { Success = true });
        }
    }

    public class BeerAdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // get the super users
            var db = DataDocumentStore.Instance;
            using (var session = db.OpenSession())
            {
                var adminUser =
                    session.Query<Participant>()
                        .FirstOrDefault(x => x.Username == httpContext.User.Identity.Name && x.IsAdmin);
                return adminUser != null;
            }
        }
    }
}