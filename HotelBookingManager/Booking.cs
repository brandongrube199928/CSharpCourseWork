using System;

namespace HotelBookingManager
{
    public sealed class Booking
    {
        
		//Public Variables Required by the assignment
		public string RoomNumber { get; }
        public string GuestName { get; }
        public DateTime CheckIn { get; private set; }
        public DateTime CheckOut { get; private set; }

        public Booking(string roomNumber, string guestName, DateTime checkIn, DateTime checkOut)
        {
            //Check if the roomNumber or guestName are null or whitespace and throw an exception that they are required
            //Check if checkOut is before checkIn with <=. Throw an exception if true
            if (string.IsNullOrWhiteSpace(roomNumber))  throw new ArgumentException("Room number is required.");
            if (string.IsNullOrWhiteSpace(guestName))  throw new ArgumentException("Guest number is required.");
            if (checkOut <= checkIn) throw new ArgumentException("Check out must be after check-in.");


            //If they validate, assign the instance variables here:
            RoomNumber = roomNumber.Trim();
            GuestName = guestName.Trim();
            CheckIn = checkIn;
            CheckOut = checkOut;

        }

        public void Reschedule(DateTime newIn, DateTime newOut)
        {
            //Check if the newOut DateTime is before newIn. Use <= and throw an exception if true
            if (newOut <= newIn) throw new ArgumentException("Check out must be after check-in.");
			//If not, set the Check In and Check Out times to the new ones
            CheckIn = newIn;
            CheckOut = newOut;



        }

        //Create an override for ToString
        //Print according to assignment guidelines: [{RoomNumber}] {CheckIn:MM/dd HH:mm}–{CheckOut:MM/dd HH:mm}  {GuestName}
        public override string ToString()
            => $"[{RoomNumber}] {CheckIn:MM/dd HH:mm}–{CheckOut:MM/dd HH:mm}  {GuestName}";

    }
}
