using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clean.Data.Migrations
{
    /// <inheritdoc />
    public partial class relationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IdOfCustomer",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Cleaners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IdOfCleaner",
                table: "Cleaners",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Cleaners",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Cleaners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CleanersId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomersId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "cleanerId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "customerId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cleaners_ScheduleId",
                table: "Cleaners",
                column: "ScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CleanersId",
                table: "Bookings",
                column: "CleanersId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CustomersId",
                table: "Bookings",
                column: "CustomersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Cleaners_CleanersId",
                table: "Bookings",
                column: "CleanersId",
                principalTable: "Cleaners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Customers_CustomersId",
                table: "Bookings",
                column: "CustomersId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cleaners_Schedule_ScheduleId",
                table: "Cleaners",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Cleaners_CleanersId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Customers_CustomersId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Cleaners_Schedule_ScheduleId",
                table: "Cleaners");

            migrationBuilder.DropIndex(
                name: "IX_Cleaners_ScheduleId",
                table: "Cleaners");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CleanersId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CustomersId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IdOfCustomer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Cleaners");

            migrationBuilder.DropColumn(
                name: "IdOfCleaner",
                table: "Cleaners");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Cleaners");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Cleaners");

            migrationBuilder.DropColumn(
                name: "CleanersId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomersId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "cleanerId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "customerId",
                table: "Bookings");
        }
    }
}
