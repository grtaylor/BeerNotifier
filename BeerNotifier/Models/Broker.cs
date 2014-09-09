using System;
using System.Collections.Generic;
using System.Linq;
using BeerNotifier.Controllers;
using Raven.Client;
using Raven.Client.Linq;

namespace BeerNotifier.Models
{
    public class Broker
    {
        public void NotifyBuyer(string location)
        {
            IDocumentStore db = DataDocumentStore.Instance;
            using (var session = db.OpenSession())
            {
                var candidate = GetBuyerByLocation(location, session);
                if (candidate != null)
                {
                    var configuration = session.Query<BeerConfiguration>().FirstOrDefault();
                    NotifyPerson(candidate, configuration);
                }
            }
        }

        private Participant GetBuyerByLocation(string location, IDocumentSession session)
        {
            var candidate =
                session.Query<Participant>().Where(x => x.Location.Contains(location)).FirstOrDefault(x => x.Force);
            if (candidate == null)
            {
                // get the person with the oldest last purchased date
                candidate =
                    session.Query<Participant>()
                        .Where(x => x.Location.Contains(location))
                        .OrderBy(x => x.LastPurchase)
                        .FirstOrDefault();
            }
            return candidate;
        }

        private void NotifyPerson(Participant candidate, BeerConfiguration configuration)
        {
            Notifier notifier = new Notifier();
            try
            {
                
                notifier.SendEmailToPerson(configuration.SmtpServerDetails, candidate, configuration.SmtpDetailsPerson);
                Logger.Log("Email notification successfully sent", candidate.Name + " has been notified via email");
                if (candidate.CellPhone != null)
                {
                    notifier.SendText(configuration.TextMessageServerSettings,
                        string.Format(configuration.TextMessageMessage, candidate.Name),
                        candidate.CellPhone);
                    Logger.Log("Text notification successfully sent",
                        candidate.Name + " has been notified via text message");
                }
                else
                {
                    Logger.Log("Text notification NOT sent",
                       candidate.Name + " does not have a cell phone number on file");
                }
            }
            catch (Exception e)
            {
                var personName = string.Empty;
                if (candidate != null)
                    personName = candidate.Name;

                Logger.Log("An Error occurred notifying beer person: " + personName, e.ToString());
            }
        }

        public void NotifyGroup(string location)
        {
            try
            {
                IDocumentStore db = DataDocumentStore.Instance;
                List<Participant> participants;
                Participant candidate;
                BeerConfiguration configuration;

                using (var session = db.OpenSession())
                {
                    // get people in the location
                    participants = session.Query<Participant>().Where(x => x.Location.Contains(location)).ToList();
                    candidate = GetBuyerByLocation(location, session);
                    configuration = session.Query<BeerConfiguration>().FirstOrDefault();
                    // we assume that the person honors their commitment
                    candidate.DaysChosen += 1;
                    candidate.LastPurchase = DateTime.UtcNow.Date;
                    session.Store(candidate);
                    session.SaveChanges();
                }
                var notifier = new Notifier();
                notifier.SendEmailToGroup(configuration.SmtpServerDetails, configuration.SmtpDetailsGroup, participants,
                    candidate);
                Logger.Log("Group Email notification successfully sent", location + " has been notified via email");
            }
            catch (Exception e)
            {
                Logger.Log("An Error occurred notifying beer group at: " + location, e.ToString());
                
            }
        }
    }
}