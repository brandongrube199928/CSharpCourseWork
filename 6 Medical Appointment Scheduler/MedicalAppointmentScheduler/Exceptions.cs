using System;

namespace MedScheduler
{
    // Create a Business-rule exception for overlapping appointments (same provider or same room)
    public class DoubleBookingException : Exception
    {
        public DoubleBookingException(string message) : base(message) { }
    }

    // Create a Business-rule exception for outside hours, short duration, etc.
    public class InvalidAppointmentTimeException : Exception
    {
        public InvalidAppointmentTimeException(string message) : base(message){}
    }

}
