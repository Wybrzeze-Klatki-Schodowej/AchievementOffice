using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementOffice.Migrations
{
    /// <inheritdoc />
    public partial class RebuildSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KudosShoutouts",
                columns: table => new
                {
                    shoutout_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reaction = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KudosShoutouts", x => new { x.shoutout_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_KudosShoutouts_Shoutouts_shoutout_id",
                        column: x => x.shoutout_id,
                        principalTable: "Shoutouts",
                        principalColumn: "shoutout_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KudosShoutouts_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KudosShoutouts_user_id",
                table: "KudosShoutouts",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KudosShoutouts");
        }
    }
}
