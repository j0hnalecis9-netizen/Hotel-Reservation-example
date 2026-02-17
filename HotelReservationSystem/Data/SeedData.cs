using HotelReservationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        foreach (var role in new[] { "Customer", "Manager" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var managerEmail = "manager@hotel.com";
        var manager = await userManager.FindByEmailAsync(managerEmail);
        if (manager is null)
        {
            manager = new ApplicationUser { UserName = managerEmail, Email = managerEmail, FullName = "Hotel Manager", EmailConfirmed = true };
            await userManager.CreateAsync(manager, "Manager123!");
            await userManager.AddToRoleAsync(manager, "Manager");
        }

        if (!await context.HotelClasses.AnyAsync())
        {
            var classes = new List<HotelClass>
            {
                new() { Name = "Economy", Description = "Budget-friendly room for practical travelers.", BasePricePerNight = 1800, ImageUrl = "https://images.unsplash.com/photo-1631049552240-59c37f38802b", AvailableServices = "WiFi, Aircon", SpecialOffers = "Stay 3 nights, get 10% off" },
                new() { Name = "Standard", Description = "Comfortable stay with city view.", BasePricePerNight = 2800, ImageUrl = "https://images.unsplash.com/photo-1568495248636-6432b97bd949", AvailableServices = "WiFi, Breakfast", SpecialOffers = "Free welcome drink" },
                new() { Name = "Deluxe", Description = "Spacious room with premium amenities.", BasePricePerNight = 4200, ImageUrl = "https://images.unsplash.com/photo-1618773928121-c32242e63f39", AvailableServices = "WiFi, Breakfast, Gym", SpecialOffers = "15% off weekday bookings" },
                new() { Name = "Suite", Description = "Elegant suite with living area.", BasePricePerNight = 6500, ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b", AvailableServices = "WiFi, Breakfast, Lounge access", SpecialOffers = "Complimentary airport pickup" },
                new() { Name = "Presidential Suite", Description = "Luxury top-tier suite for VIP guests.", BasePricePerNight = 12000, ImageUrl = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461", AvailableServices = "All-inclusive services", SpecialOffers = "Private butler service" }
            };

            context.HotelClasses.AddRange(classes);
            await context.SaveChangesAsync();

            var rooms = new List<Room>();
            foreach (var hotelClass in classes)
            {
                for (var i = 1; i <= 5; i++)
                {
                    rooms.Add(new Room
                    {
                        RoomNumber = $"{hotelClass.Name[..1].ToUpper()}{i:000}",
                        MaxGuests = hotelClass.Name is "Suite" or "Presidential Suite" ? 4 : 2,
                        IsAvailable = true,
                        PricePerNight = hotelClass.BasePricePerNight,
                        HotelClassId = hotelClass.Id
                    });
                }
            }

            context.Rooms.AddRange(rooms);
            context.AddOns.AddRange(
                new AddOn { Name = "Extra Bed", Price = 500 },
                new AddOn { Name = "Breakfast", Price = 350 },
                new AddOn { Name = "Spa", Price = 1200 },
                new AddOn { Name = "Tour Package", Price = 2000 }
            );
            await context.SaveChangesAsync();
        }
    }
}
