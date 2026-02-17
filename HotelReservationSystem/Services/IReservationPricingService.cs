using HotelReservationSystem.Models;

namespace HotelReservationSystem.Services;

public interface IReservationPricingService
{
    decimal CalculateTotal(Room room, DateTime checkIn, DateTime checkOut, IEnumerable<AddOn> addOns);
}
