using System;
using System.Linq;
using BeerNotifier.Controllers;
using Quartz;
using Quartz.Impl;
using Raven.Client.Linq;

namespace BeerNotifier.Models
{
    public class Scheduling
    {
        public static void SetupScheduler()
        {
            // construct a scheduler factory
            // get a scheduler
            IScheduler sched = StdSchedulerFactory.GetDefaultScheduler();

            // we need to create a schedule for each location
            // get the locations from the database
            var db = DataDocumentStore.Instance;
            using (var session = db.OpenSession())
            {
                var locations = session.Query<Location>().ToList();
                foreach (var location in locations)
                {
                    AddJobsForLocation(sched,location);
                }
            }
            sched.Start();
        }

        public static void ClearJobs()
        {
            // get a scheduler
            IScheduler sched = StdSchedulerFactory.GetDefaultScheduler();
            sched.Clear();
        }

        private static void AddJobsForLocation(IScheduler sched, Location location)
        {
            AddJob(sched, location.ParticipantNotificationDetails, location.Name, NotifyBeerDay.NotifyPerson);
            AddJob(sched, location.GroupNotificationDetails, location.Name, NotifyBeerDay.NotifyGroup);
        }

        private static void AddJob(IScheduler sched, NotificationDetails details, string location, string notificationIndicator)
        {
            // define the job and tie it to our HelloJob class
            var groupName = Guid.NewGuid();
            IJobDetail job = JobBuilder.Create<NotifyBeerDay>()
                .WithIdentity("myJob-" + Guid.NewGuid(),
                    "group-" + groupName)
                    .UsingJobData("location",location)
                .Build();

            // Trigger the job to run now, and then specified day at the specified time, UTC!
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(notificationIndicator, "group-" + groupName)
                .StartNow()
                .WithSchedule(
                    CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(details.DayOfWeek, details.Hour, details.Minute)
                        .InTimeZone(TimeZoneInfo.Utc))
                .Build();

            sched.ScheduleJob(job, trigger);
        }
    }

    public class NotificationDetails
    {
        public string Name { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        
    }

    public class Location
    {
        public NotificationDetails ParticipantNotificationDetails { get; set; }
        public NotificationDetails GroupNotificationDetails { get; set; }
        public string Name { get; set; }
    }


    public class NotifyBeerDay : IJob
    {
        internal static string NotifyPerson = "notifyperson";
        internal static string NotifyGroup = "emailgroup";

        public void Execute(IJobExecutionContext context)
        {
            var location = context.JobDetail.JobDataMap.GetString("location");
            if (context.Trigger.Key.Name.Equals(NotifyPerson))
            {
                // notify the person per the location
                Broker b = new Broker();
                b.NotifyBuyer(location);
            }
            else if (context.Trigger.Key.Name.Equals(NotifyGroup))
            {
                var b = new Broker();
                b.NotifyGroup(location);
            }

        }
    }
}
