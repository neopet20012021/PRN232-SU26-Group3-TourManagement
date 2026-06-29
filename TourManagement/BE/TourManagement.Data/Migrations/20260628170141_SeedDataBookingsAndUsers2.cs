using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TourManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataBookingsAndUsers2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "AdultCount", "BookingCode", "BookingDate", "CCCD", "ChildCount", "CreatedBy", "CreatedDate", "CustomerName", "DiscountAmount", "Email", "FinalPrice", "InfantCount", "Notes", "PaymentMethod", "PhoneNumber", "PromoCode", "RoomType", "ScheduleId", "SpecialRequest", "Status", "TotalPrice", "UserId" },
                values: new object[,]
                {
                    { 101, 2, "BK-20230601-101", new DateTime(2026, 6, 24, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8695), null, 0, null, new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8691), "Nguyễn Văn A", 0m, "nguyenvan@gmail.com", 5000000m, 0, null, "BankTransfer", "0901234567", null, null, 1, "Phòng view biển nếu có thể", "Paid", 5000000m, null },
                    { 102, 2, "BK-20230602-102", new DateTime(2026, 6, 27, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8702), null, 1, null, new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8700), "Lê Thị B", 0m, "lethi@gmail.com", 8640000m, 0, null, "CreditCard", "0987654321", null, null, 2, "Có trẻ em đi kèm", "Pending", 8640000m, null },
                    { 103, 1, "BK-20230603-103", new DateTime(2026, 6, 28, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8705), null, 0, null, new DateTime(2026, 6, 29, 0, 1, 41, 456, DateTimeKind.Local).AddTicks(8703), "Trần Trung C", 0m, "tranc@yahoo.com", 2500000m, 0, null, "Cash", "0912333444", null, null, 1, "", "Deposited", 2500000m, null }
                });

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

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "FullName", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 101, "nguyenvan@gmail.com", "Nguyễn Văn A", "nguyenvan123", "Customer", "nguyenvan" },
                    { 102, "lethi@gmail.com", "Lê Thị B", "lethi123", "Customer", "lethi" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 102);

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 6, 28, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6620), new DateTime(2026, 7, 11, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6628), new DateTime(2026, 7, 8, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6623) });

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 6, 28, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6632), new DateTime(2026, 7, 5, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6635), new DateTime(2026, 7, 3, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6634) });

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 6, 28, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6638), new DateTime(2026, 7, 17, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6640), new DateTime(2026, 7, 13, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6639) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 6, 28, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6563));

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 6, 28, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6584));

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 6, 28, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6591));
        }
    }
}
