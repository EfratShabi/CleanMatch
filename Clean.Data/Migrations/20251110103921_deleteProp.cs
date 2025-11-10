using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clean.Data.Migrations
{
    /// <inheritdoc />
    public partial class deleteProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Cleaners_CleanersId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CleanersId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CleanersId",
                table: "Bookings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CleanersId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CleanersId",
                table: "Bookings",
                column: "CleanersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Cleaners_CleanersId",
                table: "Bookings",
                column: "CleanersId",
                principalTable: "Cleaners",
                principalColumn: "Id");
        }
    }
}
