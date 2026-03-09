using System;
using System.Collections.Generic;
using System.Linq;

namespace MedScheduler
{
    public class AppointmentScheduler
    {
        private readonly List<Appointment> _appointments = new();

        // Business hours & minimum duration (customize as desired)
        private readonly TimeSpan _open  = new(8, 0, 0);  // 08:00
        private readonly TimeSpan _close = new(17, 0, 0); // 17:00
        private readonly TimeSpan _minDuration = new(0, 15, 0); // 15 minutes

		//Create a Public IReadOnlyList called Appointments. Call the private list as a Public List .AsReadOnly()
        public IReadOnlyList<Appointment> Appointments => _appointments.AsReadOnly();

        //Create a public void to add appointments. 
        //Be sure to run the business rules methods, ValidateTimeRules and EnsureNoConflicts
        //Then add the appointment to the private list and report using the logger
        public void Add(Appointment appt)
        {
            // Validate the Appointment
            ValidateTimeRules(appt.Start, appt.End);
            EnsureNoConflicts(appt, excludeId: null);

            // Add the appointment
            _appointments.Add(appt);
            Logger.Info($"Added {appt}");
        }


        //Make a boolean method to cancel an appointment, passing in a string for the id
        //Iterate over the list and if the id doesn't exist return false
        //If it exists, remove the appointment, log the information and return true
        public bool Cancel(string id)
        {
            var appt = _appointments.FirstOrDefault(a => a.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (appt != null) return false;

            _appointments.Remove(appt);
            Logger.Info($"Canceled {appt}");
            return true;
        }


        //Provided as an example for the other methods. No change needed.
        public void Reschedule(string id, DateTime newStart, DateTime newEnd)
        {
            var appt = _appointments.FirstOrDefault(a => a.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (appt == null) throw new KeyNotFoundException($"Appointment '{id}' not found.");

            ValidateTimeRules(newStart, newEnd);

            // Temporarily consider appointment with new time to check conflicts
            var temp = new Appointment(appt.Id, appt.PatientName, appt.ProviderName, newStart, newEnd, appt.Room);
            EnsureNoConflicts(temp, excludeId: appt.Id);

            var before = appt.ToString();
            appt.Reschedule(newStart, newEnd);
            Logger.Info($"Rescheduled {before} -> {appt}");
        }

		//Make an IEnumerable of type Appointment called ListByProvider and pass in a string of the provider's named
		//Make a list of any appointment that provider is in and return them in order of start time
		public IEnumerable<Appointment> ListByProvider(string provider)
            => _appointments.Where(a => a.ProviderName.Equals(provider, StringComparison.OrdinalIgnoreCase))
            .OrderBy(a => a.Start);


        public IEnumerable<Appointment> ListByDay(DateTime day)
        {
            //Create a variable that gets day.Date from the DateTime in the Parameter.
			//Then return the readonly list ordered by Start Date on the same day
			
        }

		//Create a public IEnumarable of Appointments named All(). Return the list ordered by start time


		//!!! You should not modify anything below this line
        // ---------- Business Rules ----------

        private void ValidateTimeRules(DateTime start, DateTime end)
        {
            if (end <= start)
                throw new InvalidAppointmentTimeException("End time must be after start time.");

            if ((end - start) < _minDuration)
                throw new InvalidAppointmentTimeException($"Appointment must be at least {_minDuration.TotalMinutes} minutes.");

            if (start.TimeOfDay < _open || end.TimeOfDay > _close)
                throw new InvalidAppointmentTimeException($"Appointment must be within business hours: {_open:hh\\:mm}-{_close:hh\\:mm}.");
        }

        private void EnsureNoConflicts(Appointment candidate, string? excludeId)
        {
            bool Overlaps(Appointment a, Appointment b)
                => a.Start < b.End && b.Start < a.End;

            foreach (var existing in _appointments)
            {
                if (!string.IsNullOrEmpty(excludeId) && existing.Id.Equals(excludeId, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (Overlaps(candidate, existing))
                {
                    // Conflict if same provider OR same room
                    bool providerClash = existing.ProviderName.Equals(candidate.ProviderName, StringComparison.OrdinalIgnoreCase);
                    bool roomClash     = existing.Room.Equals(candidate.Room, StringComparison.OrdinalIgnoreCase);

                    if (providerClash || roomClash)
                        throw new DoubleBookingException(
                            $"Time conflict: {candidate.Start:yyyy-MM-dd HH:mm}-{candidate.End:HH:mm} overlaps {existing.Start:HH:mm}-{existing.End:HH:mm} " +
                            $"for {(providerClash ? "provider" : "room")} ({(providerClash ? candidate.ProviderName : candidate.Room)}).");
                }
            }
        }
    }
}
