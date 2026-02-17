using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.Models;

public class AddOn
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 100000)]
    public decimal Price { get; set; }

    public ICollection<ReservationAddOn> ReservationAddOns { get; set; } = new List<ReservationAddOn>();
}
