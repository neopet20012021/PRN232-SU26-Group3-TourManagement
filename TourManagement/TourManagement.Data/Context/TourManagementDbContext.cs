using Microsoft.EntityFrameworkCore;
using TourManagement.Data.Models;

namespace TourManagement.Data.Context
{
    public class TourManagementDbContext : DbContext
    {
        public TourManagementDbContext(DbContextOptions<TourManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Tour entity
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasKey(e => e.TourId);

                entity.Property(e => e.TourCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.TourName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.PricePerAdult)
                    .HasPrecision(18, 2);

                entity.Property(e => e.ChildPrice)
                    .HasPrecision(18, 2);

                entity.HasIndex(e => e.TourCode)
                    .IsUnique();

                entity.HasIndex(e => e.IsActive);
            });

            // Configure Booking entity
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.BookingId);

                entity.Property(e => e.BookingCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TotalPrice)
                    .HasPrecision(18, 2);

                entity.HasIndex(e => e.BookingCode)
                    .IsUnique();

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}