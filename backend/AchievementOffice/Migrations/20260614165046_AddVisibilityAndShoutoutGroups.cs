using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AchievementOffice.Migrations
{
    /// <inheritdoc />
    public partial class AddVisibilityAndShoutoutGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "visibility_id",
                table: "UserDetails",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "visibility_id",
                table: "Shoutouts",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "visibility_id",
                table: "Achievements",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "AchievementGroups",
                columns: table => new
                {
                    achievement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementGroups", x => new { x.achievement_id, x.group_id });
                    table.ForeignKey(
                        name: "FK_AchievementGroups_Achievements_achievement_id",
                        column: x => x.achievement_id,
                        principalTable: "Achievements",
                        principalColumn: "achievement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AchievementGroups_Groups_group_id",
                        column: x => x.group_id,
                        principalTable: "Groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileGroups",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileGroups", x => new { x.user_id, x.group_id });
                    table.ForeignKey(
                        name: "FK_ProfileGroups_Groups_group_id",
                        column: x => x.group_id,
                        principalTable: "Groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileGroups_UserDetails_user_id",
                        column: x => x.user_id,
                        principalTable: "UserDetails",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoutoutGroups",
                columns: table => new
                {
                    shoutout_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoutoutGroups", x => new { x.shoutout_id, x.group_id });
                    table.ForeignKey(
                        name: "FK_ShoutoutGroups_Groups_group_id",
                        column: x => x.group_id,
                        principalTable: "Groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoutoutGroups_Shoutouts_shoutout_id",
                        column: x => x.shoutout_id,
                        principalTable: "Shoutouts",
                        principalColumn: "shoutout_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visibility",
                columns: table => new
                {
                    visibility_mode_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    visibility_mode_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visibility", x => x.visibility_mode_id);
                });

            migrationBuilder.UpdateData(
                table: "UserDetails",
                keyColumn: "user_id",
                keyValue: new Guid("a5e2f6d1-4b7c-4d8e-9f0a-1b2c3d4e5f6f"),
                column: "visibility_id",
                value: 1);

            migrationBuilder.InsertData(
                table: "Visibility",
                columns: new[] { "visibility_mode_id", "visibility_mode_name" },
                values: new object[,]
                {
                    { 1, "Public" },
                    { 2, "Private" },
                    { 3, "Group" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_visibility_id",
                table: "UserDetails",
                column: "visibility_id");

            migrationBuilder.CreateIndex(
                name: "IX_Shoutouts_visibility_id",
                table: "Shoutouts",
                column: "visibility_id");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_visibility_id",
                table: "Achievements",
                column: "visibility_id");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementGroups_group_id",
                table: "AchievementGroups",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileGroups_group_id",
                table: "ProfileGroups",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_ShoutoutGroups_group_id",
                table: "ShoutoutGroups",
                column: "group_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_Visibility_visibility_id",
                table: "Achievements",
                column: "visibility_id",
                principalTable: "Visibility",
                principalColumn: "visibility_mode_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shoutouts_Visibility_visibility_id",
                table: "Shoutouts",
                column: "visibility_id",
                principalTable: "Visibility",
                principalColumn: "visibility_mode_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDetails_Visibility_visibility_id",
                table: "UserDetails",
                column: "visibility_id",
                principalTable: "Visibility",
                principalColumn: "visibility_mode_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievements_Visibility_visibility_id",
                table: "Achievements");

            migrationBuilder.DropForeignKey(
                name: "FK_Shoutouts_Visibility_visibility_id",
                table: "Shoutouts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDetails_Visibility_visibility_id",
                table: "UserDetails");

            migrationBuilder.DropTable(
                name: "AchievementGroups");

            migrationBuilder.DropTable(
                name: "ProfileGroups");

            migrationBuilder.DropTable(
                name: "ShoutoutGroups");

            migrationBuilder.DropTable(
                name: "Visibility");

            migrationBuilder.DropIndex(
                name: "IX_UserDetails_visibility_id",
                table: "UserDetails");

            migrationBuilder.DropIndex(
                name: "IX_Shoutouts_visibility_id",
                table: "Shoutouts");

            migrationBuilder.DropIndex(
                name: "IX_Achievements_visibility_id",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "visibility_id",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "visibility_id",
                table: "Shoutouts");

            migrationBuilder.DropColumn(
                name: "visibility_id",
                table: "Achievements");
        }
    }
}
