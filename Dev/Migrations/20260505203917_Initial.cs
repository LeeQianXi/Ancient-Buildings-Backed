using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dev.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "_content_buildings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "VARCHAR", nullable: true),
                    Body = table.Column<string>(type: "TEXT", nullable: true),
                    Categories = table.Column<string>(type: "TEXT", nullable: true),
                    Desc = table.Column<string>(type: "VARCHAR", nullable: true),
                    Description = table.Column<string>(type: "VARCHAR", nullable: true),
                    Dynasties = table.Column<string>(type: "TEXT", nullable: true),
                    Extension = table.Column<string>(type: "VARCHAR", nullable: true),
                    Img = table.Column<string>(type: "VARCHAR", nullable: true),
                    Meta = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "VARCHAR", nullable: true),
                    Navigation = table.Column<string>(type: "TEXT", nullable: true, defaultValue: "true"),
                    Path = table.Column<string>(type: "VARCHAR", nullable: true),
                    Provinces = table.Column<string>(type: "TEXT", nullable: true),
                    Seo = table.Column<string>(type: "TEXT", nullable: true, defaultValue: "{}"),
                    Stem = table.Column<string>(type: "VARCHAR", nullable: true),
                    Subtitle = table.Column<string>(type: "VARCHAR", nullable: true),
                    Hash = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__content_buildings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX__content_buildings_Hash",
                table: "_content_buildings",
                column: "Hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "_content_buildings");
        }
    }
}
