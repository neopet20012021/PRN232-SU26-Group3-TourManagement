using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTourBookingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tours",
                type: "NVARCHAR(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "DepartureDate" },
                values: new object[] { new DateTime(2026, 6, 24, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3074), new DateTime(2026, 7, 4, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3100) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "DepartureDate" },
                values: new object[] { new DateTime(2026, 6, 24, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3118), new DateTime(2026, 6, 29, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3123) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "DepartureDate" },
                values: new object[] { new DateTime(2026, 6, 24, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3126), new DateTime(2026, 7, 9, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3129) });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tours",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)");

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "DepartureDate" },
                values: new object[] { new DateTime(2026, 6, 24, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5371), new DateTime(2026, 7, 4, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5397) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "DepartureDate" },
                values: new object[] { new DateTime(2026, 6, 24, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5410), new DateTime(2026, 6, 29, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5414) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "DepartureDate" },
                values: new object[] { new DateTime(2026, 6, 24, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5417), new DateTime(2026, 7, 9, 20, 22, 33, 481, DateTimeKind.Local).AddTicks(5421) });
        }
    }
}
