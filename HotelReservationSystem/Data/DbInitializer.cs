using HotelReservationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        if (!await roleManager.RoleExistsAsync("Manager"))
        {
            await roleManager.CreateAsync(new IdentityRole("Manager"));
        }

        const string managerEmail = "manager@hotel.com";
        var manager = await userManager.FindByEmailAsync(managerEmail);
        if (manager is null)
        {
            manager = new IdentityUser { UserName = managerEmail, Email = managerEmail, EmailConfirmed = true };
            await userManager.CreateAsync(manager, "Manager@123");
            await userManager.AddToRoleAsync(manager, "Manager");
        }

        if (!context.HotelClasses.Any())
        {
            var standard = new HotelClass
            {
                Name = "Standard",
                Description = "Comfortable room for solo or couple stays.",
                BasePricePerNight = 90,
                Capacity = 2,
                Amenities = "Wi-Fi, TV, AC"
            };

            var deluxe = new HotelClass
            {
                Name = "Deluxe",
                Description = "Spacious room with upgraded comfort and city view.",
                BasePricePerNight = 150,
                Capacity = 3,
                Amenities = "Wi-Fi, TV, AC, Mini-bar, Balcony"
            };

            var suite = new HotelClass
            {
                Name = "Suite",
                Description = "Premium suite with living area and luxury services.",
                BasePricePerNight = 260,
                Capacity = 4,
                Amenities = "Wi-Fi, TV, AC, Mini-bar, Balcony, Jacuzzi"
            };

            context.HotelClasses.AddRange(standard, deluxe, suite);
            await context.SaveChangesAsync();

            var rooms = new List<Room>
            {
                new() { RoomNumber = "S101", HotelClassId = standard.Id },
                new() { RoomNumber = "S102", HotelClassId = standard.Id },
                new() { RoomNumber = "D201", HotelClassId = deluxe.Id },
                new() { RoomNumber = "D202", HotelClassId = deluxe.Id },
                new() { RoomNumber = "SU301", HotelClassId = suite.Id }
            };

            context.Rooms.AddRange(rooms);
        }

        if (!context.AddOns.Any())
        {
            context.AddOns.AddRange(
                new AddOn { Name = "Breakfast", Description = "Daily breakfast buffet", Price = 20 },
                new AddOn { Name = "Spa", Description = "Spa access and massage", Price = 60 },
                new AddOn { Name = "Airport Pickup", Description = "Private airport transfer", Price = 35 }
            );
        }

        await context.SaveChangesAsync();
    }
}
