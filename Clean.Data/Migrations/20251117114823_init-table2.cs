using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clean.Data.Migrations
{
    /// <inheritdoc />
    public partial class inittable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrFidback",
                table: "Cleaners");

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextOfFeedback = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.AddColumn<string>(
                name: "ArrFidback",
                table: "Cleaners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
