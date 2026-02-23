using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bot.Migrations
{
    /// <inheritdoc />
    public partial class CreateTotalScoreAndCurrentScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Turns",
                newName: "TotalScore");

            migrationBuilder.AddColumn<int>(
                name: "CurrentScore",
                table: "Turns",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentScore",
                table: "Turns");

            migrationBuilder.RenameColumn(
                name: "TotalScore",
                table: "Turns",
                newName: "Score");
        }
    }
}
