using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementOffice.Migrations
{
    /// <inheritdoc />
    public partial class RankAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "user_id",
                keyValue: new Guid("a5e2f6d1-4b7c-4d8e-9f0a-1b2c3d4e5f6f"));

            migrationBuilder.AddColumn<Guid>(
                name: "rank_id",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    rank_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rank_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    multiplier = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 1m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.rank_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_rank_id",
                table: "Users",
                column: "rank_id");

            migrationBuilder.AddForeignKey(
                name: "fk_rank",
                table: "Users",
                column: "rank_id",
                principalTable: "Ranks",
                principalColumn: "rank_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rank",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropIndex(
                name: "IX_Users_rank_id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "rank_id",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "user_id", "created_at", "deleted_at", "email", "is_active", "last_email", "last_password_hash", "login", "password_hash", "updated_at", "user_role_id" },
                values: new object[] { new Guid("a5e2f6d1-4b7c-4d8e-9f0a-1b2c3d4e5f6f"), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@example.com", true, null, null, "admin_test", "nihjcbweiij12", new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("fb279f32-7235-4306-8968-380f76953e6b") });
        }
    }
}
