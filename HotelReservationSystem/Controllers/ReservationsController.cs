using HotelReservationSystem.Data;
using HotelReservationSystem.Models;
using HotelReservationSystem.Services;
using HotelReservationSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Controllers;

[Authorize]
public class ReservationsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IReceiptService _receiptService;

    public ReservationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IReceiptService receiptService)
    {
        _context = context;
        _userManager = userManager;
        _receiptService = receiptService;
    }

    public async Task<IActionResult> Create(int roomId)
    {
        var room = await _context.Rooms.Include(r => r.HotelClass).FirstOrDefaultAsync(r => r.Id == roomId && r.IsAvailable);
        if (room is null) return NotFound();

        var vm = new ReservationCreateViewModel
        {
            RoomId = roomId,
            RoomDisplay = $"{room.HotelClass!.Name} - Room {room.RoomNumber}",
            AddOns = await _context.AddOns.Where(a => a.IsActive).ToListAsync(),
            EstimatedTotal = room.PricePerNight
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReservationCreateViewModel vm)
    {
        var room = await _context.Rooms.Include(r => r.HotelClass).FirstOrDefaultAsync(r => r.Id == vm.RoomId && r.IsAvailable);
        if (room is null)
        {
            ModelState.AddModelError(string.Empty, "Selected room is not available.");
        }

        if (vm.CheckOutDate <= vm.CheckInDate)
        {
            ModelState.AddModelError(string.Empty, "Check-out date must be after check-in date.");
        }

        if (room is not null)
        {
            var hasConflict = await _context.Reservations.AnyAsync(r =>
                r.RoomId == room.Id &&
                r.Status != ReservationStatus.Cancelled &&
                vm.CheckInDate < r.CheckOutDate &&
                vm.CheckOutDate > r.CheckInDate);

            if (hasConflict)
                ModelState.AddModelError(string.Empty, "Room is already booked for the selected dates.");
        }

        if (!ModelState.IsValid || room is null)
        {
            vm.AddOns = await _context.AddOns.Where(a => a.IsActive).ToListAsync();
            return View(vm);
        }

        var user = await _userManager.GetUserAsync(User);
        var selectedAddOns = await _context.AddOns.Where(a => vm.SelectedAddOnIds.Contains(a.Id)).ToListAsync();
        var nights = (vm.CheckOutDate - vm.CheckInDate).Days;
        var total = (room.PricePerNight * nights) + selectedAddOns.Sum(a => a.Price);

        var reservation = new Reservation
        {
            UserId = user!.Id,
            RoomId = room.Id,
            CheckInDate = vm.CheckInDate,
            CheckOutDate = vm.CheckOutDate,
            NumberOfGuests = vm.NumberOfGuests,
            TotalPrice = total,
            Status = ReservationStatus.Confirmed
        };

        foreach (var addOn in selectedAddOns)
            reservation.ReservationAddOns.Add(new ReservationAddOn { AddOnId = addOn.Id });

        reservation.Payment = new Payment
        {
            Method = vm.PaymentMethod,
            Amount = total,
            Status = vm.PaymentMethod == PaymentMethod.CashOnArrival ? PaymentStatus.Pending : PaymentStatus.Paid
        };

        reservation.Receipt = new Receipt { EmailSentTo = user.Email ?? string.Empty };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Receipt), new { id = reservation.Id });
    }

    public async Task<IActionResult> Receipt(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Room)!.ThenInclude(room => room!.HotelClass)
            .Include(r => r.User)
            .Include(r => r.Payment)
            .Include(r => r.ReservationAddOns).ThenInclude(ra => ra.AddOn)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation is null) return NotFound();

        var vm = new ReceiptViewModel
        {
            BookingReference = reservation.BookingReference,
            GuestName = reservation.User?.FullName ?? reservation.User?.UserName ?? "Guest",
            Room = $"{reservation.Room?.HotelClass?.Name} - {reservation.Room?.RoomNumber}",
            CheckIn = reservation.CheckInDate,
            CheckOut = reservation.CheckOutDate,
            Nights = (reservation.CheckOutDate - reservation.CheckInDate).Days,
            AddOns = reservation.ReservationAddOns.Select(a => a.AddOn?.Name ?? string.Empty),
            PaymentMethod = reservation.Payment?.Method.ToString() ?? "N/A",
            Total = reservation.TotalPrice,
            ReservationId = reservation.Id
        };

        return View(vm);
    }

    public async Task<FileResult> DownloadReceiptPdf(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Room)!.ThenInclude(room => room!.HotelClass)
            .Include(r => r.User)
            .Include(r => r.Payment)
            .Include(r => r.ReservationAddOns).ThenInclude(ra => ra.AddOn)
            .FirstAsync(r => r.Id == id);

        var vm = new ReceiptViewModel
        {
            BookingReference = reservation.BookingReference,
            GuestName = reservation.User?.FullName ?? reservation.User?.UserName ?? "Guest",
            Room = $"{reservation.Room?.HotelClass?.Name} - {reservation.Room?.RoomNumber}",
            CheckIn = reservation.CheckInDate,
            CheckOut = reservation.CheckOutDate,
            Nights = (reservation.CheckOutDate - reservation.CheckInDate).Days,
            AddOns = reservation.ReservationAddOns.Select(a => a.AddOn?.Name ?? string.Empty),
            PaymentMethod = reservation.Payment?.Method.ToString() ?? "N/A",
            Total = reservation.TotalPrice,
            ReservationId = reservation.Id
        };

        var pdfBytes = _receiptService.BuildPdf(vm);
        return File(pdfBytes, "application/pdf", $"receipt-{vm.BookingReference}.pdf");
    }
}
