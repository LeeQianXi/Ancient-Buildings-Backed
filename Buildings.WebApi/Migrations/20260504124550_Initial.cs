using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildings.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountTokens",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTokens_UserId", x => new { x.UserId, x.Hash });
                });

            migrationBuilder.CreateTable(
                name: "AccountUser",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    PasswordSaltHash = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DeleteAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountUser_UserId", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountUser_CreatedAt",
                table: "AccountUser",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccountUser_Email",
                table: "AccountUser",
                columns: new[] { "Email", "DeleteAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountUser_UpdateAt",
                table: "AccountUser",
                column: "UpdateAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountTokens");

            migrationBuilder.DropTable(
                name: "AccountUser");
        }
    }
}
