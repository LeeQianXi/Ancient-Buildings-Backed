using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildings.Migrations
{
    /// <inheritdoc />
    public partial class MigrationData : Migration
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
                    Seo = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildingArticleData");
        }
    }
}
