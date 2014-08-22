using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web.Mvc;
using BeerNotifier.Models;
using Raven.Client;
using Raven.Client.UniqueConstraints;

namespace BeerNotifier.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new UserViewModel();
            // determine if the user has already registered, if so, show them metrics instead of register
            var db = DataDocumentStore.Instance;
            using (var session = db.OpenSession())
            {
                var username = User.Identity.Name;
                var usernameSplit = User.Identity.Name.Split(new[] { "\\" }, StringSplitOptions.None);
                if (usernameSplit.Length > 0)
                    username = usernameSplit[1];
                var existingUser = session.Query<Participant>().FirstOrDefault(x => x.Username == username);
                if (existingUser != null)
                    model.ParticipantDetails = existingUser;
                model.Locations = new SelectList(session.Query<Location>().ToArray(),"Name","Name");
            }
            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Join(string location)
        {
            var service = new ParticipantService();
            // look up details about the person from AD (name, email, cell phone)
            var username = User.Identity.Name;
            var usernameSplit = User.Identity.Name.Split(new[]{"\\"},StringSplitOptions.None);
            if (usernameSplit.Length > 0)
                username = usernameSplit[1];
            var person = service.GetPersonDetails(username);
            var db = DataDocumentStore.Instance;
            // make sure person isn't already added
            using (var session = db.OpenSession())
            {
                    person.LastPurchase = DateTime.MinValue;
                    person.Location = new string[] {location};
                    var success = service.AddNewMember(person,session);
                    if (success)
                    {

                    }

            }
            // add the person
            return Json(new {Success = true});
        }

        

       

        public ActionResult UpdateLocation(string location)
        {
            var db = DataDocumentStore.Instance;
            using (var session = db.OpenSession())
            {
                var existingUser = session.Query<Participant>().FirstOrDefault(x => x.Username == User.Identity.Name.Trim());
                if (existingUser != null)
                {
                    existingUser.Location = new[] {location};
                    session.Store(existingUser);
                    session.SaveChanges();
                }
            }
            return Json(new {Success = true});
        }

        public ActionResult Force()
        {
            var db = DataDocumentStore.Instance;
            using (var session = db.OpenSession())
            {
                var existingUser = session.Query<Participant>().FirstOrDefault(x => x.Username == User.Identity.Name.Trim());
                if (existingUser != null)
                {
                    existingUser.Force = true;
                    session.Store(existingUser);
                    session.SaveChanges();
                }
            }
            return Json(new { Success = true });
        }
    }

    public class ParticipantService
    {
        public bool AddNewMember(Participant person, IDocumentSession session)
        {
            if (!IsPersonAlreadyAMember(person, session))
            {
                session.Store(person);
                session.SaveChanges();
            }
            return true;
        }

        public bool IsPersonAlreadyAMember(Participant participant, IDocumentSession session)
        {
            var checkResult = session.CheckForUniqueConstraints(participant);
            // returns whether its constraints are available
            return !checkResult.ConstraintsAreFree();
        }

        public Participant GetPersonDetails(string username)
        {
            var context = new PrincipalContext(ContextType.Domain,ConfigurationManager.AppSettings["Domain"],ConfigurationManager.AppSettings["DomainUser"],ConfigurationManager.AppSettings["DomainPassword"]);
            var p = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);
            return new Participant { Name = p.Name, Email = p.EmailAddress, Username = username, CellPhone = p.VoiceTelephoneNumber };
        }
    }

    public class Participant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [UniqueConstraint]
        public string Email { get; set; }
        public int DaysChosen { get; set; }
        public DateTime LastPurchase { get; set; }
        public bool Force { get; set; }
        public string[] Location { get; set; }
        public string Username { get; set; }
        public string CellPhone { get; set; }
        public bool IsAdmin { get; set; }

        public Participant()
        {
        }

        public Participant(string name, string email, int daysChosen, DateTime lastPurchase, bool force)
        {
            Name = name;
            Email = email;
            DaysChosen = daysChosen;
            LastPurchase = lastPurchase;
            Force = force;
        }
    }
}