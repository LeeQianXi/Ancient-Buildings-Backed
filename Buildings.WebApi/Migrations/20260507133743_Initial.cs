using System;
using System.Text.Json;
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
                name: "BuildingArticleData",
                columns: table => new
                {
                    ArticleId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    SubTitle = table.Column<string>(type: "text", nullable: false),
                    Seo = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Provinces = table.Column<string>(type: "jsonb", nullable: false),
                    Categories = table.Column<string>(type: "jsonb", nullable: false),
                    Dynasties = table.Column<string>(type: "jsonb", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingArticleData_ArticleId", x => x.ArticleId);
                });

            migrationBuilder.CreateTable(
                name: "UserAccountInfo",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Profile = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Interest = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DeleteAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccountInfo_UserId", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserSecureInfo",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    PasswordSaltHash = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DeleteAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecureInfo_UserId", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserSecureToken",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    RefreshTokenExpiry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastAcquired = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecureToken_UserId", x => new { x.UserId, x.Hash });
                });

            migrationBuilder.CreateTable(
                name: "FriendRelationInfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FriendId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    ActionUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRelationInfo_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendRelationInfo_UserAccountInfo_FriendId",
                        column: x => x.FriendId,
                        principalTable: "UserAccountInfo",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendRelationInfo_UserAccountInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccountInfo",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildingArticleData_Categories",
                table: "BuildingArticleData",
                column: "Categories")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingArticleData_Dynasties",
                table: "BuildingArticleData",
                column: "Dynasties")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingArticleData_Hash",
                table: "BuildingArticleData",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuildingArticleData_Path",
                table: "BuildingArticleData",
                column: "Path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuildingArticleData_Provinces",
                table: "BuildingArticleData",
                column: "Provinces")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRelationInfo_CreatedAt",
                table: "FriendRelationInfo",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRelationInfo_FriendId_Status",
                table: "FriendRelationInfo",
                columns: new[] { "FriendId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRelationInfo_IdPair",
                table: "FriendRelationInfo",
                columns: new[] { "UserId", "FriendId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FriendRelationInfo_UserId_Status",
                table: "FriendRelationInfo",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountInfo_CreatedAt",
                table: "UserAccountInfo",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountInfo_Interest",
                table: "UserAccountInfo",
                column: "Interest")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountInfo_UpdateAt",
                table: "UserAccountInfo",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountInfo_UserName",
                table: "UserAccountInfo",
                columns: new[] { "UserName", "DeleteAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSecureInfo_CreatedAt",
                table: "UserSecureInfo",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecureInfo_Email",
                table: "UserSecureInfo",
                columns: new[] { "Email", "DeleteAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSecureInfo_UpdateAt",
                table: "UserSecureInfo",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildingArticleData");

            migrationBuilder.DropTable(
                name: "FriendRelationInfo");

            migrationBuilder.DropTable(
                name: "UserSecureInfo");

            migrationBuilder.DropTable(
                name: "UserSecureToken");

            migrationBuilder.DropTable(
                name: "UserAccountInfo");
        }
    }
}
