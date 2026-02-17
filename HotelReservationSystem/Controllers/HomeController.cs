using HotelReservationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var hotelClasses = await _context.HotelClasses.Include(h => h.Rooms).AsNoTracking().ToListAsync();
        return View(hotelClasses);
    }
}
