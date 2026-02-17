namespace HotelReservationSystem.Models;

public class ReservationAddOn
{
    public int ReservationId { get; set; }
    public Reservation? Reservation { get; set; }

    public int AddOnId { get; set; }
    public AddOn? AddOn { get; set; }
}
