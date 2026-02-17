using HotelReservationSystem.Data;
using HotelReservationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Controllers;

[Authorize(Roles = "Manager")]
public class ServicesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServicesController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index() => View(await _context.AddOns.ToListAsync());

    public IActionResult Create() => View(new AddOn());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddOn addOn)
    {
        if (!ModelState.IsValid) return View(addOn);
        _context.AddOns.Add(addOn);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var addOn = await _context.AddOns.FindAsync(id);
        if (addOn is null) return NotFound();
        return View(addOn);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AddOn addOn)
    {
        if (!ModelState.IsValid) return View(addOn);
        _context.AddOns.Update(addOn);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var addOn = await _context.AddOns.FindAsync(id);
        if (addOn is not null)
        {
            _context.AddOns.Remove(addOn);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
