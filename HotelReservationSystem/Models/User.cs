using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.Models;

public class User
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(40)]
    public string? PhoneNumber { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
