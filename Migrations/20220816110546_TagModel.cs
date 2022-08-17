using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hydrogen.Migrations
{
    public partial class TagModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "tags",
                newName: "tag_name");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "tags",
                newName: "tag_content");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "tags",
                newName: "tag_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tag_name",
                table: "tags",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "tag_content",
                table: "tags",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "tag_id",
                table: "tags",
                newName: "id");
        }
    }
}
