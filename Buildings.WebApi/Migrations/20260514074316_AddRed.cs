using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildings.Migrations
{
    /// <inheritdoc />
    public partial class AddRed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRed",
                table: "BuildingArticleData",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_BuildingArticleData_IsRed",
                table: "BuildingArticleData",
                column: "IsRed");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogComment_BlogComment_RootId",
                table: "BlogComment",
                column: "RootId",
                principalTable: "BlogComment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogComment_BlogComment_RootId",
                table: "BlogComment");

            migrationBuilder.DropIndex(
                name: "IX_BuildingArticleData_IsRed",
                table: "BuildingArticleData");

            migrationBuilder.DropColumn(
                name: "IsRed",
                table: "BuildingArticleData");
        }
    }
}
