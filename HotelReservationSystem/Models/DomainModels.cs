using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.Models;

public enum ReservationStatus
{
    Pending,
    Confirmed,
    Cancelled,
    CheckedIn,
    CheckedOut
}

public enum PaymentMethod
{
    Card,
    GCash,
    PayPal,
    CashOnArrival
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed
}

public class HotelClass
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;
    [Range(0, 100000)]
    public decimal BasePricePerNight { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string AvailableServices { get; set; } = string.Empty;
    public string SpecialOffers { get; set; } = string.Empty;
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}

public class Room
{
    public int Id { get; set; }
    [Required, StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;
    [Range(1, 10)]
    public int MaxGuests { get; set; }
    public bool IsAvailable { get; set; } = true;
    [Range(0, 100000)]
    public decimal PricePerNight { get; set; }

    public int HotelClassId { get; set; }
    public HotelClass? HotelClass { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

public class Service
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AddOn
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [Range(0, 50000)]
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Reservation
{
    public int Id { get; set; }
    [Required]
    public string BookingReference { get; set; } = Guid.NewGuid().ToString("N")[..10].ToUpper();
    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public int RoomId { get; set; }
    public Room? Room { get; set; }

    [DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; }
    [DataType(DataType.Date)]
    public DateTime CheckOutDate { get; set; }
    [Range(1, 10)]
    public int NumberOfGuests { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    [Range(0, 500000)]
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ReservationAddOn> ReservationAddOns { get; set; } = new List<ReservationAddOn>();
    public Payment? Payment { get; set; }
    public Receipt? Receipt { get; set; }
}

public class ReservationAddOn
{
    public int ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
    public int AddOnId { get; set; }
    public AddOn? AddOn { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    [Range(0, 500000)]
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}

public class Receipt
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
    public string ReceiptNumber { get; set; } = $"RCPT-{DateTime.UtcNow:yyyyMMddHHmmss}";
    public string EmailSentTo { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
}
