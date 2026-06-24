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
        public DbSet<User> Users { get; set; }

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

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Seed Data cho Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    PasswordHash = "admin123", // In a real app, this should be hashed
                    Role = "Admin",
                    FullName = "Administrator",
                    Email = "admin@tourmanagement.com"
                },
                new User
                {
                    UserId = 2,
                    Username = "staff",
                    PasswordHash = "staff123",
                    Role = "Staff",
                    FullName = "Staff Member",
                    Email = "staff@tourmanagement.com"
                }
            );

            // Seed Data cho Tours
            modelBuilder.Entity<Tour>().HasData(
                new Tour
                {
                    TourId = 1,
                    TourCode = "TOUR001",
                    TourName = "Hà Nội - Sa Pa - Cát Cát - Fansipan",
                    Description = "Khám phá vẻ đẹp huyền ảo của sương mù Sa Pa và đỉnh Fansipan hùng vĩ.",
                    Days = 3,
                    Nights = 2,
                    PricePerAdult = 2500000,
                    ChildPrice = 1750000,
                    Category = "mien-bac",
                    Destination = "Sa Pa",
                    DepartureDate = DateTime.Now.AddDays(10),
                    AvailableSeats = 20,
                    MaxCapacity = 30,
                    IsActive = true,
                    Itinerary = "Ngày 1: Hà Nội - Sa Pa | Ngày 2: Bản Cát Cát | Ngày 3: Fansipan - Hà Nội",
                    IncludedServices = "Xe đưa đón, Khách sạn 3 sao, Các bữa ăn, Vé tham quan",
                    ExcludedServices = "Chi phí cá nhân, Thuế VAT",
                    Image = "https://images.unsplash.com/photo-1596423735880-5f2a689b903e?q=80&w=600&auto=format&fit=crop"
                },
                new Tour
                {
                    TourId = 2,
                    TourCode = "TOUR002",
                    TourName = "Hà Nội - Hạ Long - Vịnh Lan Hạ 2 Ngày 1 Đêm Trên Du Thuyền 5 Sao",
                    Description = "Trải nghiệm đẳng cấp 5 sao trên du thuyền khám phá Vịnh Hạ Long và Vịnh Lan Hạ.",
                    Days = 2,
                    Nights = 1,
                    PricePerAdult = 3200000,
                    ChildPrice = 2240000,
                    Category = "mien-bac",
                    Destination = "Hạ Long",
                    DepartureDate = DateTime.Now.AddDays(5),
                    AvailableSeats = 15,
                    MaxCapacity = 20,
                    IsActive = true,
                    Itinerary = "Ngày 1: Tuần Châu - Vịnh Hạ Long | Ngày 2: Vịnh Lan Hạ - Hà Nội",
                    IncludedServices = "Phòng du thuyền 5 sao, Các bữa ăn buffet, Chèo thuyền Kayak",
                    ExcludedServices = "Đồ uống trên tàu, Tiền tip",
                    Image = "https://images.unsplash.com/photo-1528127269322-539801943592?q=80&w=600&auto=format&fit=crop"
                },
                new Tour
                {
                    TourId = 3,
                    TourCode = "TOUR003",
                    TourName = "Đà Nẵng - Hội An - Bà Nà Hills",
                    Description = "Tour miền Trung di sản: Đà Nẵng, Hội An, Bà Nà rực rỡ sắc màu.",
                    Days = 4,
                    Nights = 3,
                    PricePerAdult = 4500000,
                    ChildPrice = 3150000,
                    Category = "mien-trung",
                    Destination = "Đà Nẵng",
                    DepartureDate = DateTime.Now.AddDays(15),
                    AvailableSeats = 25,
                    MaxCapacity = 40,
                    IsActive = true,
                    Itinerary = "Ngày 1: Đà Nẵng | Ngày 2: Hội An | Ngày 3: Bà Nà Hills | Ngày 4: Sơn Trà - Chợ Hàn",
                    IncludedServices = "Khách sạn 4 sao, Vé máy bay khứ hồi, Xe đưa đón, Vé tham quan",
                    ExcludedServices = "Chi phí cá nhân",
                    Image = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?q=80&w=600&auto=format&fit=crop"
                }
            );
        }
    }
}