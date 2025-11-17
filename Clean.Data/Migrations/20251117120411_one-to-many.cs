using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clean.Data.Migrations
{
    /// <inheritdoc />
    public partial class onetomany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CleanerId",
                table: "Feedback",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CleanersId",
                table: "Feedback",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Cleaners_CleanersId",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Feedback_CleanersId",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "CleanerId",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "CleanersId",
                table: "Feedback");
        }
    }
}
