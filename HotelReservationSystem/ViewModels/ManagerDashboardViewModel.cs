using HotelReservationSystem.Models;

namespace HotelReservationSystem.ViewModels;

public class ManagerDashboardViewModel
{
    public int TotalReservations { get; set; }
    public int DailyBookings { get; set; }
    public int MonthlyBookings { get; set; }
    public decimal RevenueSummary { get; set; }
    public List<Reservation> RecentReservations { get; set; } = new();
}
