using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TourManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    TourId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TourCode = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    Nights = table.Column<int>(type: "int", nullable: false),
                    PricePerAdult = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    ChildPrice = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Itinerary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IncludedServices = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExcludedServices = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.TourId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingCode = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AdultCount = table.Column<int>(type: "int", nullable: false),
                    ChildCount = table.Column<int>(type: "int", nullable: false),
                    InfantCount = table.Column<int>(type: "int", nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SpecialRequest = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PromoCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TotalPrice = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Tours",
                columns: new[] { "TourId", "AvailableSeats", "Category", "ChildPrice", "CreatedDate", "Days", "DepartureDate", "Description", "Destination", "ExcludedServices", "Image", "IncludedServices", "IsActive", "Itinerary", "MaxCapacity", "Nights", "PricePerAdult", "TourCode", "TourName", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, 20, "mien-bac", 1750000m, new DateTime(2026, 6, 24, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5371), 3, new DateTime(2026, 7, 4, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5397), "Khám phá vẻ đẹp huyền ảo của sương mù Sa Pa và đỉnh Fansipan hùng vĩ.", "Sa Pa", "Chi phí cá nhân, Thuế VAT", "https://images.unsplash.com/photo-1596423735880-5f2a689b903e?q=80&w=600&auto=format&fit=crop", "Xe đưa đón, Khách sạn 3 sao, Các bữa ăn, Vé tham quan", true, "Ngày 1: Hà Nội - Sa Pa | Ngày 2: Bản Cát Cát | Ngày 3: Fansipan - Hà Nội", 30, 2, 2500000m, "TOUR001", "Hà Nội - Sa Pa - Cát Cát - Fansipan", null },
                    { 2, 15, "mien-bac", 2240000m, new DateTime(2026, 6, 24, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5410), 2, new DateTime(2026, 6, 29, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5414), "Trải nghiệm đẳng cấp 5 sao trên du thuyền khám phá Vịnh Hạ Long và Vịnh Lan Hạ.", "Hạ Long", "Đồ uống trên tàu, Tiền tip", "https://images.unsplash.com/photo-1528127269322-539801943592?q=80&w=600&auto=format&fit=crop", "Phòng du thuyền 5 sao, Các bữa ăn buffet, Chèo thuyền Kayak", true, "Ngày 1: Tuần Châu - Vịnh Hạ Long | Ngày 2: Vịnh Lan Hạ - Hà Nội", 20, 1, 3200000m, "TOUR002", "Hà Nội - Hạ Long - Vịnh Lan Hạ 2 Ngày 1 Đêm Trên Du Thuyền 5 Sao", null },
                    { 3, 25, "mien-trung", 3150000m, new DateTime(2026, 6, 24, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5417), 4, new DateTime(2026, 7, 9, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5421), "Tour miền Trung di sản: Đà Nẵng, Hội An, Bà Nà rực rỡ sắc màu.", "Đà Nẵng", "Chi phí cá nhân", "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?q=80&w=600&auto=format&fit=crop", "Khách sạn 4 sao, Vé máy bay khứ hồi, Xe đưa đón, Vé tham quan", true, "Ngày 1: Đà Nẵng | Ngày 2: Hội An | Ngày 3: Bà Nà Hills | Ngày 4: Sơn Trà - Chợ Hàn", 40, 3, 4500000m, "TOUR003", "Đà Nẵng - Hội An - Bà Nà Hills", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "FullName", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "admin@tourmanagement.com", "Administrator", "admin123", "Admin", "admin" },
                    { 2, "staff@tourmanagement.com", "Staff Member", "staff123", "Staff", "staff" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingCode",
                table: "Bookings",
                column: "BookingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TourId",
                table: "Bookings",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_IsActive",
                table: "Tours",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_TourCode",
                table: "Tours",
                column: "TourCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tours");
        }
    }
}
