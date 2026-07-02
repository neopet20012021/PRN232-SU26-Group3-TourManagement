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
        public DbSet<TourSchedule> TourSchedules { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<Payment> Payments { get; set; }

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

                entity.HasOne(d => d.Schedule)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ScheduleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure TourSchedule entity
            modelBuilder.Entity<TourSchedule>(entity =>
            {
                entity.HasKey(e => e.ScheduleId);

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.TourId)
                    .OnDelete(DeleteBehavior.Cascade);
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
                },
                new User
                {
                    UserId = 101,
                    Username = "nguyenvan",
                    PasswordHash = "nguyenvan123", // Password hash placeholder
                    Role = "Customer",
                    FullName = "Nguyễn Văn A",
                    Email = "nguyenvan@gmail.com"
                },
                new User
                {
                    UserId = 102,
                    Username = "lethi",
                    PasswordHash = "lethi123",
                    Role = "Customer",
                    FullName = "Lê Thị B",
                    Email = "lethi@gmail.com"
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
                    MaxCapacity = 40,
                    IsActive = true,
                    Itinerary = "Ngày 1: Đà Nẵng | Ngày 2: Hội An | Ngày 3: Bà Nà Hills | Ngày 4: Sơn Trà - Chợ Hàn",
                    IncludedServices = "Khách sạn 4 sao, Vé máy bay khứ hồi, Xe đưa đón, Vé tham quan",
                    ExcludedServices = "Chi phí cá nhân",
                    Image = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?q=80&w=600&auto=format&fit=crop"
                }
            );

            // Seed Data cho TourSchedules
            modelBuilder.Entity<TourSchedule>().HasData(
                new TourSchedule
                {
                    ScheduleId = 1,
                    TourId = 1,
                    StartDate = DateTime.Now.AddDays(10),
                    EndDate = DateTime.Now.AddDays(13),
                    MaxParticipants = 30,
                    AvailableSeats = 20,
                    ActualAdultPrice = 2500000,
                    ActualChildPrice = 1750000,
                    Status = "Active"
                },
                new TourSchedule
                {
                    ScheduleId = 2,
                    TourId = 2,
                    StartDate = DateTime.Now.AddDays(5),
                    EndDate = DateTime.Now.AddDays(7),
                    MaxParticipants = 20,
                    AvailableSeats = 15,
                    ActualAdultPrice = 3200000,
                    ActualChildPrice = 2240000,
                    Status = "Active"
                },
                new TourSchedule
                {
                    ScheduleId = 3,
                    TourId = 3,
                    StartDate = DateTime.Now.AddDays(15),
                    EndDate = DateTime.Now.AddDays(19),
                    MaxParticipants = 40,
                    AvailableSeats = 25,
                    ActualAdultPrice = 4500000,
                    ActualChildPrice = 3150000,
                    Status = "Active"
                }
            );

            // Seed Data cho Bookings
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    BookingId = 101,
                    BookingCode = "BK-20230601-101",
                    CustomerName = "Nguyễn Văn A",
                    Email = "nguyenvan@gmail.com",
                    PhoneNumber = "0901234567",
                    ScheduleId = 1,
                    AdultCount = 2,
                    ChildCount = 0,
                    InfantCount = 0,
                    TotalPrice = 5000000, // 2 * 2500000
                    BookingDate = DateTime.Now.AddDays(-5),
                    Status = "Paid",
                    PaymentMethod = "BankTransfer",
                    SpecialRequest = "Phòng view biển nếu có thể",
                    FinalPrice = 5000000
                },
                new Booking
                {
                    BookingId = 102,
                    BookingCode = "BK-20230602-102",
                    CustomerName = "Lê Thị B",
                    Email = "lethi@gmail.com",
                    PhoneNumber = "0987654321",
                    ScheduleId = 2,
                    AdultCount = 2,
                    ChildCount = 1,
                    InfantCount = 0,
                    TotalPrice = 8640000, // 2 * 3200000 + 1 * 2240000
                    BookingDate = DateTime.Now.AddDays(-2),
                    Status = "Pending",
                    PaymentMethod = "CreditCard",
                    SpecialRequest = "Có trẻ em đi kèm",
                    FinalPrice = 8640000
                },
                new Booking
                {
                    BookingId = 103,
                    BookingCode = "BK-20230603-103",
                    CustomerName = "Trần Trung C",
                    Email = "tranc@yahoo.com",
                    PhoneNumber = "0912333444",
                    ScheduleId = 1,
                    AdultCount = 1,
                    ChildCount = 0,
                    InfantCount = 0,
                    TotalPrice = 2500000,
                    BookingDate = DateTime.Now.AddDays(-1),
                    Status = "Deposited",
                    PaymentMethod = "Cash",
                    SpecialRequest = "",
                    FinalPrice = 2500000
                }
            );

            // Configure PromoCode entity
            modelBuilder.Entity<PromoCode>(entity =>
            {
                entity.HasKey(e => e.PromoCodeId);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DiscountPercent).HasPrecision(18, 2);
                entity.Property(e => e.MinBookingValue).HasPrecision(18, 2);
            });

            // Seed Data cho PromoCodes
            modelBuilder.Entity<PromoCode>().HasData(
                new PromoCode
                {
                    PromoCodeId = 1,
                    Code = "TOUR2025",
                    DiscountPercent = 0.10m,
                    MinBookingValue = 0,
                    StartDate = DateTime.Now.AddDays(-10),
                    EndDate = DateTime.Now.AddDays(365),
                    MaxUsage = 100,
                    UsageCount = 0,
                    IsActive = true
                },
                new PromoCode
                {
                    PromoCodeId = 2,
                    Code = "VIP50",
                    DiscountPercent = 0.50m,
                    MinBookingValue = 5000000,
                    StartDate = DateTime.Now.AddDays(-10),
                    EndDate = DateTime.Now.AddDays(365),
                    MaxUsage = 10,
                    UsageCount = 0,
                    IsActive = true
                },
                new PromoCode
                {
                    PromoCodeId = 3,
                    Code = "SUMMER100",
                    DiscountPercent = 0.15m,
                    MinBookingValue = 2000000,
                    StartDate = DateTime.Now.AddDays(-10),
                    EndDate = DateTime.Now.AddDays(90),
                    MaxUsage = 200,
                    UsageCount = 0,
                    IsActive = true
                }
            );
        }
    }
}