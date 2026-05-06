using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildings.Migrations
{
    /// <inheritdoc />
    public partial class AddUserMoreInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<JsonDocument>(
                name: "Seo",
                table: "BuildingArticleData",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<JsonDocument>(
                name: "Data",
                table: "BuildingArticleData",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "FriendRelation",
                columns: table => new
                {
                    FromUserId = table.Column<long>(type: "bigint", nullable: false),
                    TargetUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeleteAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRelation_FromUserId_TargetUserId", x => new { x.FromUserId, x.TargetUserId });
                });

            migrationBuilder.CreateTable(
                name: "FriendRequest",
                columns: table => new
                {
                    FromUserId = table.Column<long>(type: "bigint", nullable: false),
                    TargetUserId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequest_FromUserId_TargetUserId", x => new { x.FromUserId, x.TargetUserId });
                });

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Online = table.Column<bool>(type: "boolean", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo_UserId", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRelation_CreatedAt",
                table: "FriendRelation",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRelation_DeleteAt",
                table: "FriendRelation",
                columns: new[] { "FromUserId", "TargetUserId", "DeleteAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_Location",
                table: "UserInfo",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_Tags",
                table: "UserInfo",
                column: "Tags")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendRelation");

            migrationBuilder.DropTable(
                name: "FriendRequest");

            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.AlterColumn<string>(
                name: "Seo",
                table: "BuildingArticleData",
                type: "text",
                nullable: false,
                oldClrType: typeof(JsonDocument),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "BuildingArticleData",
                type: "text",
                nullable: false,
                oldClrType: typeof(JsonDocument),
                oldType: "jsonb");
        }
    }
}
