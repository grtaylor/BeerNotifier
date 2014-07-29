using System;
using System.Configuration;
using System.Linq;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.UniqueConstraints;

namespace BeerNotifier.Models
{
    public class DataDocumentStore
    {
        private static EmbeddableDocumentStore _instance;

        public static IDocumentStore Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException(
                        "IDocumentStore has not been initialized.");
                return _instance;
            }
        }

        public static IDocumentStore Initialize()
        {
            _instance = new EmbeddableDocumentStore {Configuration={Port=8888}, ConnectionStringName = "RavenDB", UseEmbeddedHttpServer = true};
            _instance.RegisterListener(new UniqueConstraintsStoreListener());
            _instance.Conventions.IdentityPartsSeparator = "-";
            _instance.Initialize();
            
            // configure defaults
            using (var session = _instance.OpenSession())
            {
                BeerConfiguration b = new BeerConfiguration();
                // see if we already have smtp details, if not, initialize to default
                var beerConfiguration = session.Query<BeerConfiguration>().FirstOrDefault();
                if (beerConfiguration == null)
                {
                    b.SmtpDetailsPerson = SmtpDetails.PersonDefault;
                    b.SmtpServerDetails = SmtpDetails.ServerDefault;
                    b.SmtpDetailsGroup = SmtpDetails.GroupDefault;
                    b.TextMessageServerSettings = TextMessageServerSettings.Default;
                    b.TextMessageMessage = "{0}, this a reminder that tomorrow is your beer day.";
                    session.Store(b);
                    
                }
                // now see if we have jobs already, if not, initialize defaults
                var locations = session.Query<Location>().ToList();
                if (locations.Count == 0)
                {
                    // Add 4 default locations
                    var location = new Location()
                    {
                        Name = ConfigurationManager.AppSettings["DefaultName1"],
                        ParticipantNotificationDetails =
                            new NotificationDetails
                            {
                                Name = ConfigurationManager.AppSettings["DefaultName1a"],
                                DayOfWeek = DayOfWeek.Wednesday,
                                Hour = 13,
                                Minute = 0
                            },
                        GroupNotificationDetails = 
                        new NotificationDetails
                        {
                            Name = ConfigurationManager.AppSettings["DefaultName1b"],
                            DayOfWeek = DayOfWeek.Thursday,
                            Hour = 16,
                            Minute = 0
                        }
                    };
                    session.Store(location);

                    var location2 = new Location()
                    {
                        Name = ConfigurationManager.AppSettings["DefaultName2"],
                        ParticipantNotificationDetails =
                            new NotificationDetails
                            {
                                Name = ConfigurationManager.AppSettings["DefaultName2a"],
                                DayOfWeek = DayOfWeek.Wednesday,
                                Hour = 13,
                                Minute = 0
                            },
                        GroupNotificationDetails =
                        new NotificationDetails
                        {
                            Name = ConfigurationManager.AppSettings["DefaultName2b"],
                            DayOfWeek = DayOfWeek.Thursday,
                            Hour = 16,
                            Minute = 0
                        }
                    };
                    session.Store(location2);

                    var location3 = new Location()
                    {
                        Name = ConfigurationManager.AppSettings["DefaultName3"],
                        ParticipantNotificationDetails =
                            new NotificationDetails
                            {
                                Name = ConfigurationManager.AppSettings["DefaultName3a"],
                                DayOfWeek = DayOfWeek.Wednesday,
                                Hour = 13,
                                Minute = 0
                            },
                        GroupNotificationDetails =
                        new NotificationDetails
                        {
                            Name = ConfigurationManager.AppSettings["DefaultName3b"],
                            DayOfWeek = DayOfWeek.Thursday,
                            Hour = 16,
                            Minute = 0
                        }
                    };
                    session.Store(location3);

                    var location4 = new Location()
                    {
                        Name = ConfigurationManager.AppSettings["DefaultName4"],
                        ParticipantNotificationDetails =
                            new NotificationDetails
                            {
                                Name = ConfigurationManager.AppSettings["DefaultName4a"],
                                DayOfWeek = DayOfWeek.Wednesday,
                                Hour = 9,
                                Minute = 0
                            },
                        GroupNotificationDetails =
                        new NotificationDetails
                        {
                            Name = ConfigurationManager.AppSettings["DefaultName4b"],
                            DayOfWeek = DayOfWeek.Thursday,
                            Hour = 13,
                            Minute = 0
                        }
                    };
                    session.Store(location4);
                }
                session.SaveChanges();

                
            }
            return _instance;
        }
    }
}