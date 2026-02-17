using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.Models;

public class Payment
{
    public int Id { get; set; }

    public int ReservationId { get; set; }
    public Reservation? Reservation { get; set; }

    [Required]
    public PaymentMethod Method { get; set; }

    [Range(0, 1000000)]
    public decimal Amount { get; set; }

    public DateTime PaidAtUtc { get; set; } = DateTime.UtcNow;

    public bool ReceiptByEmail { get; set; }
    public bool ReceiptPrinted { get; set; }
}
