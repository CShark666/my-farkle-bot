using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bot.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentTurnToGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentTurnId",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_CurrentTurnId",
                table: "Games",
                column: "CurrentTurnId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Turns_CurrentTurnId",
                table: "Games",
                column: "CurrentTurnId",
                principalTable: "Turns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Turns_CurrentTurnId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_CurrentTurnId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CurrentTurnId",
                table: "Games");
        }
    }
}
