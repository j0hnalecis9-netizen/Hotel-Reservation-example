using HotelReservationSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.ViewModels;

public class ReservationCreateViewModel
{
    [Required]
    public int RoomId { get; set; }
    public string RoomDisplay { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; } = DateTime.Today.AddDays(1);
    [Required, DataType(DataType.Date)]
    public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(2);

    [Range(1, 10)]
    public int NumberOfGuests { get; set; } = 1;

    public List<int> SelectedAddOnIds { get; set; } = new();
    public List<AddOn> AddOns { get; set; } = new();

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    public decimal EstimatedTotal { get; set; }
}

public class ReceiptViewModel
{
    public string BookingReference { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Nights { get; set; }
    public IEnumerable<string> AddOns { get; set; } = Array.Empty<string>();
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int ReservationId { get; set; }
}

public class DashboardViewModel
{
    public int TotalReservations { get; set; }
    public int DailyBookings { get; set; }
    public int MonthlyBookings { get; set; }
    public decimal TotalRevenue { get; set; }
}
