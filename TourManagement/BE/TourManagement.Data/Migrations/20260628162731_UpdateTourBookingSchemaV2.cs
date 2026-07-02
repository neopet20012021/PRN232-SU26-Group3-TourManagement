using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TourManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTourBookingSchemaV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Tours_TourId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AvailableSeats",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "DepartureDate",
                table: "Tours");

            migrationBuilder.RenameColumn(
                name: "TourId",
                table: "Bookings",
                newName: "ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_TourId",
                table: "Bookings",
                newName: "IX_Bookings_ScheduleId");

            migrationBuilder.CreateTable(
                name: "TourSchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    ActualAdultPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    ActualChildPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false),
                    GuideName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourSchedules", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_TourSchedules_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TourSchedules",
                columns: new[] { "ScheduleId", "ActualAdultPrice", "ActualChildPrice", "AvailableSeats", "CreatedDate", "EndDate", "GuideName", "MaxParticipants", "StartDate", "Status", "TourId" },
                values: new object[,]
                {
                    { 1, 2500000m, 1750000m, 20, new DateTime(2026, 6, 28, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6620), new DateTime(2026, 7, 11, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6628), null, 30, new DateTime(2026, 7, 8, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6623), "Active", 1 },
                    { 2, 3200000m, 2240000m, 15, new DateTime(2026, 6, 28, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6632), new DateTime(2026, 7, 5, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6635), null, 20, new DateTime(2026, 7, 3, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6634), "Active", 2 },
                    { 3, 4500000m, 3150000m, 25, new DateTime(2026, 6, 28, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6638), new DateTime(2026, 7, 17, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6640), null, 40, new DateTime(2026, 7, 13, 23, 27, 27, 462, DateTimeKind.Local).AddTicks(6639), "Active", 3 }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_TourSchedules_TourId",
                table: "TourSchedules",
                column: "TourId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TourSchedules_ScheduleId",
                table: "Bookings",
                column: "ScheduleId",
                principalTable: "TourSchedules",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TourSchedules_ScheduleId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "TourSchedules");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "Bookings",
                newName: "TourId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_ScheduleId",
                table: "Bookings",
                newName: "IX_Bookings_TourId");

            migrationBuilder.AddColumn<int>(
                name: "AvailableSeats",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DepartureDate",
                table: "Tours",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 1,
                columns: new[] { "AvailableSeats", "CreatedDate", "DepartureDate" },
                values: new object[] { 20, new DateTime(2026, 6, 24, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3074), new DateTime(2026, 7, 4, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3100) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 2,
                columns: new[] { "AvailableSeats", "CreatedDate", "DepartureDate" },
                values: new object[] { 15, new DateTime(2026, 6, 24, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3118), new DateTime(2026, 6, 29, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3123) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "TourId",
                keyValue: 3,
                columns: new[] { "AvailableSeats", "CreatedDate", "DepartureDate" },
                values: new object[] { 25, new DateTime(2026, 6, 24, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3126), new DateTime(2026, 7, 9, 21, 23, 3, 32, DateTimeKind.Local).AddTicks(3129) });

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Tours_TourId",
                table: "Bookings",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "TourId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
