using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourManagement.Data
{
    public class TourManagementDbContext : DbContext
    {
        // Hàm khởi tạo nhận Options từ ngoài truyền vào (từ file Program.cs của API)
        public TourManagementDbContext(DbContextOptions<TourManagementDbContext> options) : base(options)
        {
        }



        // Khai báo các bảng dữ liệu
        public DbSet<User> Users { get; set; }
        public DbSet<Tour> Tours { get; set; }

        // Hàm này dùng để chèn sẵn dữ liệu mẫu (Seed Data) khi tạo DB
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tạo sẵn 2 tài khoản test
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "123", FullName = "Quản trị viên", Role = "Admin" },
                new User { Id = 2, Username = "user", Password = "123", FullName = "Khách hàng thân thiết", Role = "User" }
            );

            // Tạo sẵn vài Tour du lịch mẫu để test Search
            modelBuilder.Entity<Tour>().HasData(
        new Tour
        {
            Id = 1,
            TourName = "Tour du lịch Hạ Long",
            Description = "Khám phá Vịnh biển",
            Price = 3000000,
            ImageUrl = "halong.jpg" // <-- Thêm dòng này
        },
        new Tour
        {
            Id = 2,
            TourName = "Tour khám phá Phú Quốc",
            Description = "Biển xanh cát trắng",
            Price = 5000000,
            ImageUrl = "phuquoc.jpg" // <-- Thêm dòng này
        }
    );
        }
    }
}
