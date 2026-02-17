using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.Models;

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public User? User { get; set; }

    [Required]
    public int RoomId { get; set; }
    public Room? Room { get; set; }

    [DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime CheckOutDate { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    [Range(0, 1000000)]
    public decimal TotalAmount { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Payment? Payment { get; set; }

    public ICollection<ReservationAddOn> ReservationAddOns { get; set; } = new List<ReservationAddOn>();
}
