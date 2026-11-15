using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clean.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReviewsLinkedToBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Cleaners_CleanersId",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Feedback_CleanersId",
                table: "Feedback");

            migrationBuilder.RenameColumn(
                name: "CleanersId",
                table: "Feedback",
                newName: "Rating");

            migrationBuilder.AlterColumn<string>(
                name: "TextOfFeedback",
                table: "Feedback",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "BookingCode",
                table: "Feedback",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Feedback",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_BookingCode",
                table: "Feedback",
                column: "BookingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_CleanerId",
                table: "Feedback",
                column: "CleanerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Bookings_BookingCode",
                table: "Feedback",
                column: "BookingCode",
                principalTable: "Bookings",
                principalColumn: "BookingCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Cleaners_CleanerId",
                table: "Feedback",
                column: "CleanerId",
                principalTable: "Cleaners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Bookings_BookingCode",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Cleaners_CleanerId",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Feedback_BookingCode",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Feedback_CleanerId",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "BookingCode",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Feedback");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Feedback",
                newName: "CleanersId");

            migrationBuilder.AlterColumn<string>(
                name: "TextOfFeedback",
                table: "Feedback",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_CleanersId",
                table: "Feedback",
                column: "CleanersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Cleaners_CleanersId",
                table: "Feedback",
                column: "CleanersId",
                principalTable: "Cleaners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
