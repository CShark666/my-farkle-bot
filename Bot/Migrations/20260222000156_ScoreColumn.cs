using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bot.Migrations
{
    /// <inheritdoc />
    public partial class ScoreColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Users",
                newName: "TotalScore");

            migrationBuilder.RenameColumn(
                name: "DiceId",
                table: "Turns",
                newName: "Score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalScore",
                table: "Users",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Turns",
                newName: "DiceId");
        }
    }
}
