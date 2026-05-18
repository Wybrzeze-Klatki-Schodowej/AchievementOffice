using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementOffice.Migrations
{
    /// <inheritdoc />
    public partial class AddAchievementApprove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementApprove",
                columns: table => new
                {
                    achievement_approve_id = table.Column<Guid>(type: "uuid", nullable: false),
                    achievement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementApprove", x => x.achievement_approve_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AchievementApprove_achievement_id_user_id",
                table: "AchievementApprove",
                columns: new[] { "achievement_id", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AchievementApprove");
        }
    }
}
