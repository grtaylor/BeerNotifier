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
            return Json(new {Success = true});
        }

        public ActionResult SetParticipants(string participants)
        {
            var service = new ParticipantService();
            var db = DataDocumentStore.Instance;
            // make sure person isn't already added
            using (var session = db.OpenSession())
            {
                // seperate into new lines
                // seperate by tab
                var persons = participants.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var person in persons)
                {
                    // get each item ()
                    var personItems = person.Split(new string[] {"\t"}, StringSplitOptions.RemoveEmptyEntries);
                    // make sure we have all the data points
                    if (personItems.Length != 4)
                        return
                            Json(
                                new
                                {
                                    Success = false,
                                    Message =
                                        string.Format("This line did not have all of the required fields: {0}",
                                            person)
                                });
                    var name = personItems[0];
                    var email = personItems[1];
                    var date = personItems[2];
                    var location = personItems[3];
                    DateTime lastDate;
                    DateTime.TryParse(date, out lastDate);
                    // see if person already exists, if so just update the date
                    var participant = new Participant() {Email = email};
                    var isPersonAlreadyAMember = service.IsPersonAlreadyAMember(participant, session);
                    if (!isPersonAlreadyAMember)
                    {
                        participant = service.GetPersonDetails(User.Identity.Name);
                        participant.Location = new[] {location};
                        service.AddNewMember(participant, session);
                    }
                    else
                    {
                        // update the date of the person
                        var existingUser = session.Query<Participant>().FirstOrDefault(x => x.Username == User.Identity.Name.Trim());
                        if (existingUser != null)
                        {
                            existingUser.DaysChosen += 1;
                            existingUser.LastPurchase = lastDate;
                            session.Store(existingUser);
                        }
                    }
                    session.SaveChanges();
                }
            }
            return Json(new {Success = true});
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