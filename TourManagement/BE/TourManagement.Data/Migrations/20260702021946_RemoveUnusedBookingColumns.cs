using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedBookingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CCCD",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PromoCode",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "PromoCodeId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    TransactionId = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 101,
                columns: new[] { "BookingDate", "CreatedDate", "PromoCodeId" },
                values: new object[] { new DateTime(2026, 6, 27, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9672), new DateTime(2026, 7, 2, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9664), null });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 102,
                columns: new[] { "BookingDate", "CreatedDate", "PromoCodeId" },
                values: new object[] { new DateTime(2026, 6, 30, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9680), new DateTime(2026, 7, 2, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9676), null });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 103,
                columns: new[] { "BookingDate", "CreatedDate", "PromoCodeId" },
                values: new object[] { new DateTime(2026, 7, 1, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9686), new DateTime(2026, 7, 2, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9683), null });

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "PromoCodeId",
                keyValue: 1,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2027, 7, 2, 9, 19, 46, 53, DateTimeKind.Local).AddTicks(161), new DateTime(2026, 6, 22, 9, 19, 46, 53, DateTimeKind.Local).AddTicks(160) });

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "PromoCodeId",
                keyValue: 2,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2027, 7, 2, 9, 19, 46, 53, DateTimeKind.Local).AddTicks(167), new DateTime(2026, 6, 22, 9, 19, 46, 53, DateTimeKind.Local).AddTicks(166) });

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "PromoCodeId",
                keyValue: 3,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 9, 30, 9, 19, 46, 53, DateTimeKind.Local).AddTicks(171), new DateTime(2026, 6, 22, 9, 19, 46, 53, DateTimeKind.Local).AddTicks(171) });

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 7, 2, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9608), new DateTime(2026, 7, 15, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9622), new DateTime(2026, 7, 12, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9611) });

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 7, 2, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9628), new DateTime(2026, 7, 9, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9631), new DateTime(2026, 7, 7, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9630) });

            migrationBuilder.UpdateData(
                table: "TourSchedules",
                keyColumn: "ScheduleId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 7, 2, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9634), new DateTime(2026, 7, 21, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9636), new DateTime(2026, 7, 17, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9635) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 7, 2, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9543));

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 7, 2, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9571));

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 7, 2, 9, 19, 46, 52, DateTimeKind.Local).AddTicks(9577));

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PromoCodeId",
                table: "Bookings",
                column: "PromoCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_PromoCodes_PromoCodeId",
                table: "Bookings",
                column: "PromoCodeId",
                principalTable: "PromoCodes",
                principalColumn: "PromoCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_PromoCodes_PromoCodeId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PromoCodeId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PromoCodeId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "CCCD",
                table: "Bookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Bookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Bookings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Bookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                table: "Bookings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomType",
                table: "Bookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 101,
                columns: new[] { "BookingDate", "CCCD", "CreatedBy", "CreatedDate", "Notes", "PaymentMethod", "PromoCode", "RoomType" },
                values: new object[] { new DateTime(2026, 6, 27, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3330), null, null, new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3326), null, "BankTransfer", null, null });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 102,
                columns: new[] { "BookingDate", "CCCD", "CreatedBy", "CreatedDate", "Notes", "PaymentMethod", "PromoCode", "RoomType" },
                values: new object[] { new DateTime(2026, 6, 30, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3334), null, null, new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3332), null, "CreditCard", null, null });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "BookingId",
                keyValue: 103,
                columns: new[] { "BookingDate", "CCCD", "CreatedBy", "CreatedDate", "Notes", "PaymentMethod", "PromoCode", "RoomType" },
                values: new object[] { new DateTime(2026, 7, 1, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3338), null, null, new DateTime(2026, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3336), null, "Cash", null, null });

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "PromoCodeId",
                keyValue: 1,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2027, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3593), new DateTime(2026, 6, 22, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3591) });

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "PromoCodeId",
                keyValue: 2,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2027, 7, 2, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3596), new DateTime(2026, 6, 22, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3596) });

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "PromoCodeId",
                keyValue: 3,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 9, 30, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3602), new DateTime(2026, 6, 22, 0, 47, 37, 225, DateTimeKind.Local).AddTicks(3601) });

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
    }
}
