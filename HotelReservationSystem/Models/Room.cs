using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.Models;

public class Room
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    public int HotelClassId { get; set; }
    public HotelClass? HotelClass { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
