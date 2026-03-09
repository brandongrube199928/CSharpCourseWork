using System;
using System.Globalization;
using System.Linq;

namespace MedScheduler
{
    internal static class Program
    {
        private static void Main()
        {
            //Create a var of your AppointmentScheduler class and initialize as new()
            var scheduler = new AppointmentScheduler();

            //Boilerplate to welcome the end user
            Console.WriteLine("=== Medical Appointment Scheduler ===");

            //User Input handled in a while loop
            bool running = true;
            while (running)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Add Appointment");
                Console.WriteLine("2. Cancel Appointment");
                Console.WriteLine("3. Reschedule Appointment");
                Console.WriteLine("4. List All Appointments");
                Console.WriteLine("5. List by Provider");
                Console.WriteLine("6. List by Day");
                Console.WriteLine("7. Exit");
                Console.Write("Choose: ");

                switch ((Console.ReadLine() ?? "").Trim())
                {
                    //For Cases 1-6, call the appropriate method
                    //Every case needs a break; statement
                    case "1": AddAppointmentMenu(scheduler); break;
                    case "2": CancelAppointmentMenu(scheduler); break;
                    case "3": RescheduleAppointmentMenu(scheduler); break;
                    case "4": ListAllMenu(scheduler); break;
                    case "5": ListByProviderMenu(scheduler); break;
                    case "6": ListByDayMenu(scheduler); break;
                    case "7": running = false; break;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
            //Leave the user a Goodbye Message
            Console.WriteLine("Goodbye!");
        }

        // ---------- Menus ----------

        private static void AddAppointmentMenu(AppointmentScheduler scheduler)
        {
            //Make a try catch block that uses Prompt() to get the user for all appointment information
            //In the try block, create the appointment and report if succesful
            //If it fails, create a catch exception for: 
            //DoubleBookingException
            //InvalidAppointmentTimeException
            //ArgumentException
            //Exception (the catch all for anything else)
            try
            {
                string id = Prompt("Appointment ID (e.g. A1001): ");
                string patient = Prompt("Patient Name: ");
                string provider = Prompt("Provider Name: ");
                string room = Prompt("Room (e.g., 201A): ");

                var start = PromptDateTime("Start (yyyy-MM-dd HH:mm): ");
                var end = PromptDateTime("End (yyyy-MM-dd HH:mm): ");


                var appt = new Appointment(id, patient, provider, start, end, room);
                scheduler.Add(appt);

                Console.WriteLine("Appointment added successfully.");
            }
            catch (DoubleBookingException ex)
            {
                Logger.Warn("Double booking attempt: " + ex.Message);
                Console.WriteLine("Conflict: " + ex.Message);
            }
            catch (InvalidAppointmentTimeException ex)
            {
                Logger.Warn("Invalid Time: " + ex.Message);
                Console.WriteLine("Invalid time: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Logger.Warn("Validation error: " + ex.Message);
                Console.WriteLine("Validation error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error adding appointment: " + ex);
                Console.WriteLine("An unexpected error occurred. See scheduler.log for information.");
            }

        }

        private static void CancelAppointmentMenu(AppointmentScheduler scheduler)
        {
            //Create a Try Catch block that prompts a user for an ID
            //Inside the try block, check if the appointment exists and remove it
            //If it doesn't exist, report that to the user and log it
            //In the catch block, log an exception and write to the console that there was an error
            try
            {
                string id = Prompt("Appointment ID to cancel: ");
                if (scheduler.Cancel(id))
                {
                    Console.WriteLine("Appointment canceled successfully.");
                }
                else
                {
                    Console.WriteLine("Not found.");
                    Logger.Warn($"Cancel failed: Appointment '{id}' not found.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error canceling appointment: " + ex);
                Console.WriteLine("Unexpected error occurred. See scheduler.log for information.");
            }
        }

        private static void RescheduleAppointmentMenu(AppointmentScheduler scheduler)
        {
            //Make a try catch block that uses Prompt() to get the user for an appointment ID
            //Get the new start and the new end, then attempt to use reschedule. Report if succesful
            //If it fails, create a catch exception for: 
            //KeyNotFound
            //DoubleBookingException
            //InvalidAppointmentTimeException
            //ArgumentException
            //Exception (the catch all for anything else)
            try
            {
                string id = Prompt("Appointment ID to reschedule: ");
                var newStart = PromptDateTime("New Start (yyyy-MM-dd HH:mm): ");
                var newEnd = PromptDateTime("New End (yyyy-MM-dd HH:mm): ");

                scheduler.Reschedule(id, newStart, newEnd);
                Console.WriteLine("Appointment rescheduled successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                Logger.Warn(ex.Message);
                Console.WriteLine(ex.Message);
            }
            catch (DoubleBookingException ex)
            {
                Logger.Warn("Double booking attempt: " + ex.Message);
                Console.WriteLine("Conflict: " + ex.Message);
            }
            catch (InvalidAppointmentTimeException ex)
            {
                Logger.Warn("Invalid Time: " + ex.Message);
                Console.WriteLine("Time Error: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Logger.Warn("Validation Error: " + ex.Message);
                Console.WriteLine("Validation error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error rescheduling: " + ex);
                Console.WriteLine("An unexpected error occurred. See scheduler.log for information.");
            }
        }

        private static void ListAllMenu(AppointmentScheduler scheduler)
        {
            //convert all of the appointments into the scheduler into a list and store this in a var
            //If the list is empty, report this to the user and return
            var list = scheduler.All().ToList();
            if (!list.Any()) { Console.WriteLine("No appointments scheduled."); return; }
            //If not, use a foreach to write the contents of the list below this line-
            Console.WriteLine("\n--- All Appointments ---");
            foreach (var a in list) Console.WriteLine(a);

        }

        private static void ListByProviderMenu(AppointmentScheduler scheduler)
        {
            //Use Prompt() to get the provider's name from the end user
            string provider = Prompt("Provider Name: ");
            //Then store the list by provider in a var
            var list = scheduler.ListByProvider(provider).ToList();
            //If the var is empty, report this to the end user
            if (!list.Any()) { Console.WriteLine("No appointments found for this provider."); return; }
            //If not, use a foreach to write all appointments below this line-
            Console.WriteLine($"\n--- Appointments for {provider} ---");
            foreach (var a in list) Console.WriteLine(a);

        }

        private static void ListByDayMenu(AppointmentScheduler scheduler)
        {
            //Use the PromptDateTime method and store the value in a var. Use .Date to get the day
            var day = PromptDateTime("Day (yyyy-MM-dd 00:00): ").Date;
            //Store the value of ListByDay(day).ToList() in a var
            var list = scheduler.ListByDay(day).ToList();
            //Report if there are no dates that day and return
            if (!list.Any()) { Console.WriteLine("No appointments found for this date."); return; }
            //If not, use a foreach to write the list of all appointments below this line-
            Console.WriteLine($"\n--- Appointments on {day:yyyy-MM-dd} ---");
            foreach (var a in list) Console.WriteLine(a);

        }

        //!!! No need to modify these methods. They are there to help you
        // ---------- Helpers ----------

        private static string Prompt(string label)
        {
            Console.Write(label);
            var s = Console.ReadLine() ?? "";
            return s.Trim();
        }

        private static DateTime PromptDateTime(string label)
        {
            Console.Write(label);
            var s = Console.ReadLine();
            if (!DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out var dt))
            {
                throw new ArgumentException("Invalid date/time format. Use yyyy-MM-dd HH:mm (e.g., 2025-11-15 09:30).");
            }
            return dt;
        }
    }
}
