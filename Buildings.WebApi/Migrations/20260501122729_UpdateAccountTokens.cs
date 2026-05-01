using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildings.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountTokens_UserId",
                table: "AccountTokens");

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "AccountTokens",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountTokens_UserId",
                table: "AccountTokens",
                columns: new[] { "UserId", "Hash" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountTokens_UserId",
                table: "AccountTokens");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "AccountTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountTokens_UserId",
                table: "AccountTokens",
                column: "UserId");
        }
    }
}
