using HotelReservationSystem.Models;

namespace HotelReservationSystem.Services;

public class ReservationPricingService : IReservationPricingService
{
    public decimal CalculateTotal(Room room, DateTime checkIn, DateTime checkOut, IEnumerable<AddOn> addOns)
    {
        var nights = Math.Max((checkOut.Date - checkIn.Date).Days, 1);
        var roomRate = room.HotelClass?.BasePricePerNight ?? 0;
        var addOnTotal = addOns.Sum(a => a.Price);

        return (roomRate * nights) + addOnTotal;
    }
}
