using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementOffice.Migrations
{
    /// <inheritdoc />
    public partial class AddRankingConfigurationAndAwardedPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Older records do not contain the historical multiplier or award.
            // Refuse to invent amounts that could corrupt later reversals.
            migrationBuilder.Sql("""
                DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM "AchievementApprove")
                       OR EXISTS (SELECT 1 FROM "Shoutouts")
                       OR EXISTS (SELECT 1 FROM "KudosShoutouts") THEN
                        RAISE EXCEPTION 'Historical scoring records need an explicit awarded_points backfill before applying this migration.';
                    END IF;
                END $$;
                """);

            migrationBuilder.AddColumn<decimal>(
                name: "awarded_points",
                table: "Shoutouts",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "awarded_points",
                table: "KudosShoutouts",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "awarded_points",
                table: "AchievementApprove",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "RankingConfiguration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ShoutoutBase = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ReactionShoutOutPoints = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AchievementApprovedBasePoints = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DisapprovalPoints = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingConfiguration", x => x.Id);
                    table.CheckConstraint("CK_RankingConfiguration_Points", "\"ShoutoutBase\" >= 0 AND \"ReactionShoutOutPoints\" >= 0 AND \"AchievementApprovedBasePoints\" >= 0 AND \"DisapprovalPoints\" <= 0");
                    table.CheckConstraint("CK_RankingConfiguration_Singleton", "\"Id\" = 1");
                });

            migrationBuilder.InsertData(
                table: "RankingConfiguration",
                columns: new[] { "Id", "AchievementApprovedBasePoints", "DisapprovalPoints", "ReactionShoutOutPoints", "ShoutoutBase" },
                values: new object[] { 1, 50m, -20m, 2m, 10m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RankingConfiguration");

            migrationBuilder.DropColumn(
                name: "awarded_points",
                table: "Shoutouts");

            migrationBuilder.DropColumn(
                name: "awarded_points",
                table: "KudosShoutouts");

            migrationBuilder.DropColumn(
                name: "awarded_points",
                table: "AchievementApprove");
        }
    }
}
