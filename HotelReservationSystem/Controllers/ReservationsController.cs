using HotelReservationSystem.Data;
using HotelReservationSystem.Models;
using HotelReservationSystem.Services;
using HotelReservationSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Controllers;

public class ReservationsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IReservationPricingService _pricingService;
    private readonly IEmailService _emailService;

    public ReservationsController(ApplicationDbContext context, IReservationPricingService pricingService, IEmailService emailService)
    {
        _context = context;
        _pricingService = pricingService;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int hotelClassId)
    {
        var vm = new ReservationCreateViewModel
        {
            SelectedHotelClassId = hotelClassId
        };

        await PopulateFormDataAsync(vm, hotelClassId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReservationCreateViewModel vm)
    {
        await PopulateFormDataAsync(vm, vm.SelectedHotelClassId);

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var room = await _context.Rooms.Include(r => r.HotelClass)
            .FirstOrDefaultAsync(r => r.Id == vm.SelectedRoomId && r.IsAvailable);
        if (room is null)
        {
            ModelState.AddModelError(nameof(vm.SelectedRoomId), "Selected room is unavailable.");
            return View(vm);
        }

        var addOns = await _context.AddOns
            .Where(a => vm.SelectedAddOnIds.Contains(a.Id))
            .ToListAsync();

        var user = new User
        {
            FullName = vm.FullName,
            Email = vm.Email,
            PhoneNumber = vm.PhoneNumber
        };

        var total = _pricingService.CalculateTotal(room, vm.CheckInDate!.Value, vm.CheckOutDate!.Value, addOns);

        var reservation = new Reservation
        {
            User = user,
            RoomId = room.Id,
            CheckInDate = vm.CheckInDate.Value,
            CheckOutDate = vm.CheckOutDate.Value,
            TotalAmount = total,
            Status = ReservationStatus.Confirmed,
            Payment = new Payment
            {
                Amount = total,
                Method = vm.PaymentMethod,
                ReceiptByEmail = vm.SendReceiptByEmail,
                ReceiptPrinted = vm.PrintReceipt
            }
        };

        reservation.ReservationAddOns = addOns.Select(a => new ReservationAddOn { AddOnId = a.Id }).ToList();

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        if (vm.SendReceiptByEmail)
        {
            await _emailService.SendReceiptAsync(vm.Email, "Hotel Reservation Receipt",
                $"Thank you {vm.FullName}, your booking #{reservation.Id} is confirmed. Total: {total:C}.");
        }

        return RedirectToAction(nameof(Receipt), new { id = reservation.Id, print = vm.PrintReceipt });
    }

    [HttpGet]
    public async Task<IActionResult> Receipt(int id, bool print = false)
    {
        var reservation = await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Room)!.ThenInclude(room => room.HotelClass)
            .Include(r => r.Payment)
            .Include(r => r.ReservationAddOns).ThenInclude(ra => ra.AddOn)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation is null)
        {
            return NotFound();
        }

        ViewBag.Print = print;
        return View(reservation);
    }

    private async Task PopulateFormDataAsync(ReservationCreateViewModel vm, int hotelClassId)
    {
        vm.AvailableAddOns = await _context.AddOns.AsNoTracking().ToListAsync();

        vm.RoomOptions = await _context.Rooms
            .Where(r => r.HotelClassId == hotelClassId && r.IsAvailable)
            .Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = $"Room {r.RoomNumber}"
            }).ToListAsync();

        vm.PaymentOptions = Enum.GetValues<PaymentMethod>()
            .Select(method => new SelectListItem
            {
                Text = method.ToString(),
                Value = method.ToString()
            });
    }
}
