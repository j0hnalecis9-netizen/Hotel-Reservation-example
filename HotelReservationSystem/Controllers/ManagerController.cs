using ClosedXML.Excel;
using HotelReservationSystem.Data;
using HotelReservationSystem.Models;
using HotelReservationSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Controllers;

public class ManagerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<IdentityUser> _signInManager;

    public ManagerController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager)
    {
        _context = context;
        _signInManager = signInManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() => View();

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid manager credentials.");
            return View();
        }

        return RedirectToAction(nameof(Dashboard));
    }

    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Dashboard()
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var reservations = _context.Reservations.AsNoTracking();

        var vm = new ManagerDashboardViewModel
        {
            TotalReservations = await reservations.CountAsync(),
            DailyBookings = await reservations.CountAsync(r => r.CreatedAtUtc >= today),
            MonthlyBookings = await reservations.CountAsync(r => r.CreatedAtUtc >= monthStart),
            RevenueSummary = await _context.Payments.SumAsync(p => (decimal?)p.Amount) ?? 0,
            RecentReservations = await _context.Reservations
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAtUtc)
                .Take(10)
                .AsNoTracking()
                .ToListAsync()
        };

        return View(vm);
    }

    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Reservations()
    {
        var reservations = await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Room)
            .OrderByDescending(r => r.CreatedAtUtc)
            .AsNoTracking()
            .ToListAsync();

        return View(reservations);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, ReservationStatus status)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation is null)
        {
            return NotFound();
        }

        reservation.Status = status;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Reservations));
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation is null)
        {
            return NotFound();
        }

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Reservations));
    }

    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ExportToExcel()
    {
        var reservations = await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Room)
            .Include(r => r.Payment)
            .AsNoTracking()
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reservations");

        worksheet.Cell(1, 1).Value = "Reservation Id";
        worksheet.Cell(1, 2).Value = "Customer";
        worksheet.Cell(1, 3).Value = "Room";
        worksheet.Cell(1, 4).Value = "Check-in";
        worksheet.Cell(1, 5).Value = "Check-out";
        worksheet.Cell(1, 6).Value = "Status";
        worksheet.Cell(1, 7).Value = "Payment";
        worksheet.Cell(1, 8).Value = "Total";

        for (var i = 0; i < reservations.Count; i++)
        {
            var row = i + 2;
            var item = reservations[i];
            worksheet.Cell(row, 1).Value = item.Id;
            worksheet.Cell(row, 2).Value = item.User?.FullName;
            worksheet.Cell(row, 3).Value = item.Room?.RoomNumber;
            worksheet.Cell(row, 4).Value = item.CheckInDate;
            worksheet.Cell(row, 5).Value = item.CheckOutDate;
            worksheet.Cell(row, 6).Value = item.Status.ToString();
            worksheet.Cell(row, 7).Value = item.Payment?.Method.ToString() ?? "N/A";
            worksheet.Cell(row, 8).Value = item.TotalAmount;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"reservations-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    [Authorize]
    public IActionResult AccessDenied() => View();
}
