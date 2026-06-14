using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementOffice.Migrations
{
    /// <inheritdoc />
    public partial class SyncWithDev : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shoutouts_Users_ReceiverId",
                table: "Shoutouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Shoutouts_Users_SenderId",
                table: "Shoutouts");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Shoutouts",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Shoutouts",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Shoutouts",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Shoutouts",
                newName: "sender_id");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Shoutouts",
                newName: "receiver_id");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Shoutouts",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Shoutouts",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ShoutoutId",
                table: "Shoutouts",
                newName: "shoutout_id");

            migrationBuilder.RenameIndex(
                name: "IX_Shoutouts_SenderId",
                table: "Shoutouts",
                newName: "IX_Shoutouts_sender_id");

            migrationBuilder.RenameIndex(
                name: "IX_Shoutouts_ReceiverId",
                table: "Shoutouts",
                newName: "IX_Shoutouts_receiver_id");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "Shoutouts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "Shoutouts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Shoutouts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddForeignKey(
                name: "FK_Shoutouts_Users_receiver_id",
                table: "Shoutouts",
                column: "receiver_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shoutouts_Users_sender_id",
                table: "Shoutouts",
                column: "sender_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shoutouts_Users_receiver_id",
                table: "Shoutouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Shoutouts_Users_sender_id",
                table: "Shoutouts");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Shoutouts",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Shoutouts",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Shoutouts",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "sender_id",
                table: "Shoutouts",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "receiver_id",
                table: "Shoutouts",
                newName: "ReceiverId");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "Shoutouts",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Shoutouts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "shoutout_id",
                table: "Shoutouts",
                newName: "ShoutoutId");

            migrationBuilder.RenameIndex(
                name: "IX_Shoutouts_sender_id",
                table: "Shoutouts",
                newName: "IX_Shoutouts_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Shoutouts_receiver_id",
                table: "Shoutouts",
                newName: "IX_Shoutouts_ReceiverId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Shoutouts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Shoutouts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Shoutouts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddForeignKey(
                name: "FK_Shoutouts_Users_ReceiverId",
                table: "Shoutouts",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shoutouts_Users_SenderId",
                table: "Shoutouts",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
