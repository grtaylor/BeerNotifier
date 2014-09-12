using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Web;
using BeerNotifier.Controllers;

namespace BeerNotifier.Models
{
    internal class Notifier
    {
        public void SendEmailToPerson(SmptpServerSettings settings, Participant candidate, SmtpDetails smtpDetailsPerson)
        {
            using (var client = new SmtpClient(settings.Server, settings.Port))
            {
                var newMail = new MailMessage();
                newMail.To.Add(new MailAddress(candidate.Email));
                newMail.Subject = smtpDetailsPerson.Subject;
                newMail.IsBodyHtml = true;
                newMail.From = new MailAddress(smtpDetailsPerson.From);

                var inlineLogo =
                       new LinkedResource(Path.Combine(HttpRuntime.AppDomainAppPath,
                           (@"Content\images\beer_time.png")));
                inlineLogo.ContentId = Guid.NewGuid().ToString();
                var preFabBody = string.Format(smtpDetailsPerson.Body, candidate.Name, DateTime.Now.ToLongDateString());
                var body = string.Format(@"<img src=""cid:{0}"" /><br/>{1}", inlineLogo.ContentId, preFabBody);


                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                view.LinkedResources.Add(inlineLogo);
                newMail.AlternateViews.Add(view);

                client.Send(newMail);
            }
        }

        public void SendText(TextMessageServerSettings textMessageServerSettings, string message, string cellPhone)
        {
            var c = new SmsClient();
            c.SendReminder(cellPhone, message, textMessageServerSettings);
        }

        public void SendEmailToGroup(SmptpServerSettings settings, SmtpDetails smpDetails, List<Participant> people,
            Participant person)
        {
            foreach (var participant in people)
            {
                using (var client = new SmtpClient(settings.Server, settings.Port))
                {
                    var newMail = new MailMessage();
                    newMail.To.Add(new MailAddress(participant.Email));
                    newMail.Subject = smpDetails.Subject;
                    newMail.IsBodyHtml = true;
                    newMail.From = new MailAddress(smpDetails.From);

                    var inlineLogo =
                        new LinkedResource(Path.Combine(HttpRuntime.AppDomainAppPath,
                            (@"Content\images\beer_time.png")));
                    inlineLogo.ContentId = Guid.NewGuid().ToString();

                    var preFabBody = string.Format(smpDetails.Body, person.Name, DateTime.Now.ToLongDateString());
                    var body = string.Format(@"<img src=""cid:{0}"" /><br/>{1}", inlineLogo.ContentId, preFabBody);


                    var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    view.LinkedResources.Add(inlineLogo);
                    newMail.AlternateViews.Add(view);

                    client.Send(newMail);
                }
            }
        }
    }
}