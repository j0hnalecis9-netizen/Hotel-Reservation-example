using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.Models;

public class HotelClass
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(800)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 100000)]
    public decimal BasePricePerNight { get; set; }

    [Range(1, 20)]
    public int Capacity { get; set; }

    [Required, StringLength(400)]
    public string Amenities { get; set; } = string.Empty;

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
