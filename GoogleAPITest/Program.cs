using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace GoogleAPITest
{
    class Program
    {
        static string[] Scopes = { "https://www.googleapis.com/auth/calendar","https://www.googleapis.com/auth/userinfo.profile" };

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    null).Result;
                // Console.WriteLine("Credential file saved to: " + credPath);
            }

            CalendarService calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                // ApiKey = "AIzaSyBDi7lgN343WnLralgfIdZWwh6_4t_ll4M",
                ApplicationName = "Travel Reminder"
            });

            IList<CalendarListEntry> calList = calendarService.CalendarList.List().Execute().Items;
            IList<Event> _lstCal = calendarService.Events.List("primary").Execute().Items;

            if (calList.Count > 0)
            {

                #region Create Event

                EventDateTime eveStTime = new EventDateTime()
                {
                    DateTime = System.DateTime.Now.AddMinutes(50)
                };
                EventDateTime eveEdTime = new EventDateTime()
                {
                    DateTime = System.DateTime.Now.AddMinutes(80)
                };

                Event eve = new Event()
                {
                    Summary = "Event Ceated",
                    Location = "Pune",
                    Description = "Alerted",
                    Start = eveStTime,
                    End = eveEdTime
                };


                EventAttendee ea1 = new EventAttendee();
                ea1.DisplayName = "Shefali";
                ea1.Email = "vashishthashefali15@gmail.com";
                ea1.Organizer = false;
                ea1.Resource = false;
                IList<EventAttendee> ealist = new List<EventAttendee>();
                ealist.Add(ea1);
                eve.Attendees = ealist;

                // This will create reminder a day prior to the event as email
                EventReminder er1 =
                    new EventReminder()
                    {
                        Method = "email",
                        Minutes = 24 * 60
                    };

                // This will create a reminder 10 minutes before the event as popup
                EventReminder er2 = new EventReminder()
                {
                    Method = "popup",
                    Minutes = 10
                };

                Event.RemindersData erdata = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = new[]{
                        er1,er2
                    }
                };

                eve.Reminders = erdata;
                eve = calendarService.Events.Insert(eve, "primary").Execute();
                }

                calList = calendarService.CalendarList.List().Execute().Items;
               
            #endregion


            foreach (Event calenda in _lstCal)
            {
                if (calenda.Location != null)
                {
                    Console.WriteLine("Event Summary : " + calenda.Summary);
                    Console.WriteLine("Location : " + calenda.Location);
                    Console.WriteLine("Description : " + calenda.Description);
                }
            }
        }


    }
}
