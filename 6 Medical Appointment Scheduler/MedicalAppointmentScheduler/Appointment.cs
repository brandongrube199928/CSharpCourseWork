using System;

namespace MedScheduler
{
    public class Appointment
    {
        //Class Variables for this assignment
		public string Id { get; }
        public string PatientName { get; }
        public string ProviderName { get; }
        public DateTime Start { get; private set; }
        public DateTime End   { get; private set; }
        public string Room { get; }

		//Create an overloaded constructor. In the Constructor, pass in all Class Variables
		//For each one except the DateTimes, validate if they are null or Empty and throw an ArgumentException
		//For the DateTimes, Check if end is before start using <= and throw an Argument Exception
		//Then set the public variables

		public Appointment(string id, string patientName, string providerName, DateTime start, DateTime end, string room)
		{
            // Validate the parameters
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id cannot be null or empty.");
            if (string.IsNullOrEmpty(patientName)) throw new ArgumentException("Patient name cannot be null or empty.");
            if (string.IsNullOrEmpty(providerName)) throw new ArgumentException("Provider name cannot be null or empty.");
            if (string.IsNullOrEmpty(room)) throw new ArgumentException("Room cannot be blank.");
            if (end <= start) throw new ArgumentException("End time must be after start time.");

            // Assign the class variables
            Id = id.Trim();
            PatientName = patientName.Trim();
            ProviderName = providerName.Trim();
            Room = room.Trim();
            Start = start;
            End = end;
        }


        //Create a void Reschedule() pass in two new DateTimes.
        //If the new end is before the new start, throw an exception
        //Otherwise set Start and End to the new values.
        public void Reschedule(DateTime newStart, DateTime newEnd)
        {
            if (newEnd <= newStart) throw new ArgumentException("End time must be after start time.");
            Start = newStart;
            End = newEnd;
        }


        //Create a public override for ToString() have it print according to the assignment layout:
        //[2025-11-12 09:30:12] INFO: Added [ A1001 ] 09:00–09:30 Dr. Nguyen Room 201 [2025-11-12 09:32:45]
        public override string ToString()
        => $" [{Id}] {Start:yyyy-MM-dd HH:mm}-{End:HH:mm} | {PatientName} with {ProviderName} (Room{Room})";

    }
}
