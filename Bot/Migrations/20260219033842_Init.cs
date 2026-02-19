using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bot.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => new { x.ChatId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Player1ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    Player1UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    Player2ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    Player2UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Users_Player1ChatId_Player1UserId",
                        columns: x => new { x.Player1ChatId, x.Player1UserId },
                        principalTable: "Users",
                        principalColumns: new[] { "ChatId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Users_Player2ChatId_Player2UserId",
                        columns: x => new { x.Player2ChatId, x.Player2UserId },
                        principalTable: "Users",
                        principalColumns: new[] { "ChatId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Turns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    PlayerUserId = table.Column<long>(type: "INTEGER", nullable: false),
                    TurnNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    DiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    DiceValue = table.Column<string>(type: "TEXT", nullable: false),
                    SelectedDice = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turns_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Turns_Users_PlayerChatId_PlayerUserId",
                        columns: x => new { x.PlayerChatId, x.PlayerUserId },
                        principalTable: "Users",
                        principalColumns: new[] { "ChatId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player1ChatId_Player1UserId",
                table: "Games",
                columns: new[] { "Player1ChatId", "Player1UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player2ChatId_Player2UserId",
                table: "Games",
                columns: new[] { "Player2ChatId", "Player2UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Turns_GameId",
                table: "Turns",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Turns_PlayerChatId_PlayerUserId",
                table: "Turns",
                columns: new[] { "PlayerChatId", "PlayerUserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Turns");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
