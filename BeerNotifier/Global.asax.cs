using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BeerNotifier.Models;
using Twilio;

namespace BeerNotifier
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // initialize db
            DataDocumentStore.Initialize();
            // setup scheduler
            Scheduling.SetupScheduler();
            

        }

        
    }

    public class SmtpDetails
    {
        public static SmtpDetails PersonDefault
        {
            get
            {
                return new SmtpDetails()
                {
                    From = ConfigurationManager.AppSettings["MailFrom"],
                    Subject = "Heads up, your beer day is tomorrow!",
                    Body =
                        "Greetings {0}!<br/><br/>This is your friendly reminder that it is your turn to provide beer tomorrow,{1}.  The consequences for noncompliance are dire."
                };
            }
        }

        public string Body { get; set; }

        public string Subject { get; set; }

        public string From { get; set; }

        public static SmptpServerSettings ServerDefault
        {
            get { return new SmptpServerSettings(ConfigurationManager.AppSettings["SmptServer"], 25); }
        }

        public static SmtpDetails GroupDefault
        {
            get
            {
                return new SmtpDetails
                {
                    From = ConfigurationManager.AppSettings["MailFrom"],
                    Subject = "Beer Time has arrived",
                    Body =
                        "Greeting Beer aficionados!<br/><br/>{0} will be providing beer today,{1}.  look for it in the fridge once the 'beer is here' email has been sent."
                };
            }
        }
    }

    public class SmptpServerSettings
    {
        public SmptpServerSettings()
        {
        }

        public SmptpServerSettings(string server, int port)
        {
            Server = server;
            Port = port;
        }

        public string Server { get; set; }
        public int Port { get; set; }
    }

    public class BeerConfiguration
    {
        public SmtpDetails SmtpDetailsPerson { get; set; }
        public SmtpDetails SmtpDetailsGroup { get; set; }
        public SmptpServerSettings SmtpServerDetails { get; set; }
        public TextMessageServerSettings TextMessageServerSettings { get; set; }
        public string TextMessageMessage { get; set; }
    }

    public class TextMessageServerSettings
    {
        public string SmsId { get; set; }
        public string SmsPassword { get; set; }
        public string SmsFromNumber { get; set; }

        public static TextMessageServerSettings Default
        {
            get
            {
                return new TextMessageServerSettings
                {
                    SmsId = ConfigurationManager.AppSettings["SMSId"],
                    SmsPassword = ConfigurationManager.AppSettings["SMSPassword"],
                    SmsFromNumber = ConfigurationManager.AppSettings["SMSFromNumber"]
                };
            }
        }
    }

    public class SmsClient
    {
        public string SendReminder(string number, string messageText, TextMessageServerSettings settings)
        {
            var accountSid = settings.SmsId;
            var authToken = settings.SmsPassword;
            var fromNumber = settings.SmsFromNumber;

            var twilio = new TwilioRestClient(accountSid, authToken);

            var message = twilio.SendMessage(fromNumber, number, messageText);
            Console.WriteLine(message.Sid);
            if (message.RestException != null)
            {
                var error = message.RestException.Message;
                // handle the error ...
            }
            return message.Status;
        }
    }
}
