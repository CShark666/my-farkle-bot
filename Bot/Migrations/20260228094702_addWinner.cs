using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bot.Migrations
{
    /// <inheritdoc />
    public partial class addWinner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WinnerChatId",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WinnerUserId",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_WinnerChatId_WinnerUserId",
                table: "Games",
                columns: new[] { "WinnerChatId", "WinnerUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_WinnerChatId_WinnerUserId",
                table: "Games",
                columns: new[] { "WinnerChatId", "WinnerUserId" },
                principalTable: "Users",
                principalColumns: new[] { "ChatId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_WinnerChatId_WinnerUserId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_WinnerChatId_WinnerUserId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "WinnerChatId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "WinnerUserId",
                table: "Games");
        }
    }
}
