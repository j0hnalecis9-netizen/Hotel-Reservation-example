using HotelReservationSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<HotelClass> HotelClasses => Set<HotelClass>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<AddOn> AddOns => Set<AddOn>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ReservationAddOn> ReservationAddOns => Set<ReservationAddOn>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Receipt> Receipts => Set<Receipt>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Room>().HasIndex(r => r.RoomNumber).IsUnique();

        builder.Entity<ReservationAddOn>()
            .HasKey(ra => new { ra.ReservationId, ra.AddOnId });

        builder.Entity<Reservation>()
            .HasOne(r => r.Payment)
            .WithOne(p => p.Reservation)
            .HasForeignKey<Payment>(p => p.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Reservation>()
            .HasOne(r => r.Receipt)
            .WithOne(rc => rc.Reservation)
            .HasForeignKey<Receipt>(rc => rc.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Reservation>()
            .HasCheckConstraint("CK_Reservation_Dates", "CheckOutDate > CheckInDate");
    }
}
