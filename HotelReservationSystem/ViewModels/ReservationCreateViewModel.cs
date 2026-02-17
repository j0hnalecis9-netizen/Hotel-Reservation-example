using System.ComponentModel.DataAnnotations;
using HotelReservationSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelReservationSystem.ViewModels;

public class ReservationCreateViewModel : IValidatableObject
{
    public int SelectedHotelClassId { get; set; }

    [Required(ErrorMessage = "Please select a room.")]
    public int? SelectedRoomId { get; set; }

    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(40)]
    public string? PhoneNumber { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime? CheckInDate { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime? CheckOutDate { get; set; }

    public List<int> SelectedAddOnIds { get; set; } = new();

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    public bool SendReceiptByEmail { get; set; }
    public bool PrintReceipt { get; set; }

    public IEnumerable<SelectListItem> RoomOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> PaymentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<AddOn> AvailableAddOns { get; set; } = Enumerable.Empty<AddOn>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckInDate.HasValue && CheckInDate.Value.Date < DateTime.UtcNow.Date)
        {
            yield return new ValidationResult("Check-in date cannot be in the past.", new[] { nameof(CheckInDate) });
        }

        if (CheckInDate.HasValue && CheckOutDate.HasValue && CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult("Check-out date must be after check-in date.", new[] { nameof(CheckOutDate) });
        }
    }
}
