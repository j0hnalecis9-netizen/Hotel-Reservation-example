using HotelReservationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var hotelClasses = await _context.HotelClasses
            .AsNoTracking()
            .ToListAsync();

        ViewBag.Offers = new[]
        {
            "Weekend Deal: 15% off for 2+ nights",
            "Family Package: Kids stay free on Deluxe rooms",
            "Suite Experience: Free spa session with every suite booking"
        };

        ViewBag.Services = new[]
        {
            "24/7 Front Desk",
            "High-speed Wi-Fi",
            "Airport Shuttle",
            "Concierge Service"
        };

        return View(hotelClasses);
    }
}
