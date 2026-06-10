using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementOffice.Migrations
{
    /// <inheritdoc />
    public partial class NotificationsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "achievement_verification_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    achievement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requester_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievement_verification_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_achievement_verification_requests_Achievements_achievement_~",
                        column: x => x.achievement_id,
                        principalTable: "Achievements",
                        principalColumn: "achievement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_achievement_verification_requests_Users_requester_user_id",
                        column: x => x.requester_user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_achievement_verification_requests_Users_target_user_id",
                        column: x => x.target_user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    group_description = table.Column<string>(type: "text", nullable: false),
                    max_user_count = table.Column<int>(type: "integer", nullable: false),
                    group_avatar_url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.group_id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    achievement_verification_request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notifications_achievement_verification_requests_achievement~",
                        column: x => x.achievement_verification_request_id,
                        principalTable: "achievement_verification_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupUserRole",
                columns: table => new
                {
                    group_user_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_user_role_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUserRole", x => x.group_user_role_id);
                    table.ForeignKey(
                        name: "FK_GroupUserRole_Groups_group_id",
                        column: x => x.group_id,
                        principalTable: "Groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupUser",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_user_role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUser", x => new { x.group_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_GroupUser_GroupUserRole_group_user_role_id",
                        column: x => x.group_user_role_id,
                        principalTable: "GroupUserRole",
                        principalColumn: "group_user_role_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupUser_Groups_group_id",
                        column: x => x.group_id,
                        principalTable: "Groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupUser_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_achievement_verification_requests_achievement_id",
                table: "achievement_verification_requests",
                column: "achievement_id");

            migrationBuilder.CreateIndex(
                name: "IX_achievement_verification_requests_requester_user_id",
                table: "achievement_verification_requests",
                column: "requester_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_achievement_verification_requests_target_user_id",
                table: "achievement_verification_requests",
                column: "target_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_group_user_role_id",
                table: "GroupUser",
                column: "group_user_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_user_id",
                table: "GroupUser",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUserRole_group_id",
                table: "GroupUserRole",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_achievement_verification_request_id",
                table: "notifications",
                column: "achievement_verification_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id",
                table: "notifications",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupUser");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "GroupUserRole");

            migrationBuilder.DropTable(
                name: "achievement_verification_requests");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
