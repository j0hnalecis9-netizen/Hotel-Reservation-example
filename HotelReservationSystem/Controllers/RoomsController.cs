using HotelReservationSystem.Data;
using HotelReservationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Controllers;

[Authorize(Roles = "Manager")]
public class RoomsController : Controller
{
    private readonly ApplicationDbContext _context;
    public RoomsController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index() => View(await _context.Rooms.Include(r => r.HotelClass).ToListAsync());

    public async Task<IActionResult> Create()
    {
        ViewBag.HotelClassId = new SelectList(await _context.HotelClasses.ToListAsync(), "Id", "Name");
        return View(new Room());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Room room)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.HotelClassId = new SelectList(await _context.HotelClasses.ToListAsync(), "Id", "Name");
            return View(room);
        }

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room is null) return NotFound();
        ViewBag.HotelClassId = new SelectList(await _context.HotelClasses.ToListAsync(), "Id", "Name", room.HotelClassId);
        return View(room);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Room room)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.HotelClassId = new SelectList(await _context.HotelClasses.ToListAsync(), "Id", "Name", room.HotelClassId);
            return View(room);
        }
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
