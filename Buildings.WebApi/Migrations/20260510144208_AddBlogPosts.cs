using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildings.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogPost",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsAi = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Tag = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DeleteAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Views = table.Column<int>(type: "integer", nullable: false),
                    Likes = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPost_PostId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPost_UserAccountInfo_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "UserAccountInfo",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogComment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    PostId = table.Column<long>(type: "bigint", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsAi = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DeleteAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogComment_PostId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogComment_BlogPost_PostId",
                        column: x => x.PostId,
                        principalTable: "BlogPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogComment_UserAccountInfo_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "UserAccountInfo",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogComment_AuthorId",
                table: "BlogComment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogComment_CreatedAt",
                table: "BlogComment",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BlogComment_IsAi",
                table: "BlogComment",
                column: "IsAi");

            migrationBuilder.CreateIndex(
                name: "IX_BlogComment_PostId",
                table: "BlogComment",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPost_AuthorId",
                table: "BlogPost",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPost_CreatedAt",
                table: "BlogPost",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPost_IsAi",
                table: "BlogPost",
                column: "IsAi");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPost_Tag",
                table: "BlogPost",
                column: "Tag");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPost_UpdateAt",
                table: "BlogPost",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogComment");

            migrationBuilder.DropTable(
                name: "BlogPost");
        }
    }
}
