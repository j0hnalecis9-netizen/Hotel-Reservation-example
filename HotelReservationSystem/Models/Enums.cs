namespace HotelReservationSystem.Models;

public enum PaymentMethod
{
    CreditCard = 1,
    PayPal = 2,
    Cash = 3
}

public enum ReservationStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
    Completed = 4
}
