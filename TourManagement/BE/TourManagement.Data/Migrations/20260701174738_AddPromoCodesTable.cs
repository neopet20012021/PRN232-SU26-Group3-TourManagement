using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TourManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPromoCodesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    PromoCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    MinBookingValue = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxUsage = table.Column<int>(type: "int", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.PromoCodeId);
                });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 101,
                columns: new[] { "BookingDate", "CreatedDate" },
                values: new object[] { new DateTime(2026, 6, 27, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3330), new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3326) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 102,
                columns: new[] { "BookingDate", "CreatedDate" },
                values: new object[] { new DateTime(2026, 6, 30, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3334), new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3332) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 103,
                columns: new[] { "BookingDate", "CreatedDate" },
                values: new object[] { new DateTime(2026, 7, 1, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3338), new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3336) });

            migrationBuilder.InsertData(
                table: "PromoCodes",
                columns: new[] { "PromoCodeId", "Code", "DiscountPercent", "EndDate", "IsActive", "MaxUsage", "MinBookingValue", "StartDate", "UsageCount" },
                values: new object[,]
                {
                    { 1, "TOUR2025", 0.10m, new DateTime(2027, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3593), true, 100, 0m, new DateTime(2026, 6, 22, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3591), 0 },
                    { 2, "VIP50", 0.50m, new DateTime(2027, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3596), true, 10, 5000000m, new DateTime(2026, 6, 22, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3596), 0 },
                    { 3, "SUMMER100", 0.15m, new DateTime(2026, 9, 30, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3602), true, 200, 2000000m, new DateTime(2026, 6, 22, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3601), 0 }
                });

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3294), new DateTime(2026, 7, 15, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3302), new DateTime(2026, 7, 12, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3296) });

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3306), new DateTime(2026, 7, 9, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3307), new DateTime(2026, 7, 7, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3307) });

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3309), new DateTime(2026, 7, 21, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3310), new DateTime(2026, 7, 17, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3310) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3254));

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3274));

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3278));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoCodes");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 101,
                columns: new[] { "BookingDate", "CreatedDate" },
                values: new object[] { new DateTime(2026, 6, 24, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8695), new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8691) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 102,
                columns: new[] { "BookingDate", "CreatedDate" },
                values: new object[] { new DateTime(2026, 6, 27, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8702), new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8700) });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 103,
                columns: new[] { "BookingDate", "CreatedDate" },
                values: new object[] { new DateTime(2026, 6, 28, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8705), new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8703) });

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8661), new DateTime(2026, 7, 12, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8668), new DateTime(2026, 7, 9, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8664) });

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8670), new DateTime(2026, 7, 6, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8672), new DateTime(2026, 7, 4, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8671) });

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8674), new DateTime(2026, 7, 18, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8675), new DateTime(2026, 7, 14, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8675) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8621));

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8639));

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8643));
        }
    }
}
