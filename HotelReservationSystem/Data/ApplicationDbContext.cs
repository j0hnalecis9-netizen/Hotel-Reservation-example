using HotelReservationSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<HotelClass> HotelClasses => Set<HotelClass>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AddOn> AddOns => Set<AddOn>();
    public DbSet<ReservationAddOn> ReservationAddOns => Set<ReservationAddOn>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Room>()
            .HasOne(r => r.HotelClass)
            .WithMany(h => h.Rooms)
            .HasForeignKey(r => r.HotelClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Reservation>()
            .HasOne(r => r.Room)
            .WithMany(rm => rm.Reservations)
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Reservation>()
            .HasOne(r => r.Payment)
            .WithOne(p => p.Reservation)
            .HasForeignKey<Payment>(p => p.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ReservationAddOn>()
            .HasKey(ra => new { ra.ReservationId, ra.AddOnId });

        builder.Entity<ReservationAddOn>()
            .HasOne(ra => ra.Reservation)
            .WithMany(r => r.ReservationAddOns)
            .HasForeignKey(ra => ra.ReservationId);

        builder.Entity<ReservationAddOn>()
            .HasOne(ra => ra.AddOn)
            .WithMany(a => a.ReservationAddOns)
            .HasForeignKey(ra => ra.AddOnId);
    }
}
