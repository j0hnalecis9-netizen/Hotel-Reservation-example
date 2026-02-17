using ClosedXML.Excel;
using HotelReservationSystem.Data;
using HotelReservationSystem.Models;
using HotelReservationSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Controllers;

[Authorize(Roles = "Manager")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Dashboard()
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var vm = new DashboardViewModel
        {
            TotalReservations = await _context.Reservations.CountAsync(),
            DailyBookings = await _context.Reservations.CountAsync(r => r.CreatedAt >= today),
            MonthlyBookings = await _context.Reservations.CountAsync(r => r.CreatedAt >= monthStart),
            TotalRevenue = await _context.Payments.Where(p => p.Status == PaymentStatus.Paid).SumAsync(p => p.Amount)
        };

        return View(vm);
    }

    public async Task<IActionResult> Reservations(DateTime? from, DateTime? to, int? hotelClassId)
    {
        var query = _context.Reservations
            .Include(r => r.Room)!.ThenInclude(room => room!.HotelClass)
            .Include(r => r.User)
            .AsQueryable();

        if (from.HasValue) query = query.Where(r => r.CheckInDate >= from.Value);
        if (to.HasValue) query = query.Where(r => r.CheckOutDate <= to.Value);
        if (hotelClassId.HasValue) query = query.Where(r => r.Room!.HotelClassId == hotelClassId);

        ViewBag.HotelClasses = await _context.HotelClasses.ToListAsync();
        return View(await query.OrderByDescending(r => r.CreatedAt).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, ReservationStatus status)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation is null) return NotFound();
        reservation.Status = status;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Reservations));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation is null) return NotFound();
        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Reservations));
    }

    public async Task<FileResult> ExportBookingsExcel()
    {
        var reservations = await _context.Reservations
            .Include(r => r.Room)!.ThenInclude(room => room!.HotelClass)
            .Include(r => r.User)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Bookings");
        worksheet.Cell(1, 1).Value = "BookingRef";
        worksheet.Cell(1, 2).Value = "Guest";
        worksheet.Cell(1, 3).Value = "Class";
        worksheet.Cell(1, 4).Value = "CheckIn";
        worksheet.Cell(1, 5).Value = "CheckOut";
        worksheet.Cell(1, 6).Value = "Status";
        worksheet.Cell(1, 7).Value = "Total";

        for (var i = 0; i < reservations.Count; i++)
        {
            var row = i + 2;
            var reservation = reservations[i];
            worksheet.Cell(row, 1).Value = reservation.BookingReference;
            worksheet.Cell(row, 2).Value = reservation.User?.FullName ?? reservation.User?.Email;
            worksheet.Cell(row, 3).Value = reservation.Room?.HotelClass?.Name;
            worksheet.Cell(row, 4).Value = reservation.CheckInDate;
            worksheet.Cell(row, 5).Value = reservation.CheckOutDate;
            worksheet.Cell(row, 6).Value = reservation.Status.ToString();
            worksheet.Cell(row, 7).Value = reservation.TotalPrice;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "bookings.xlsx");
    }
}
