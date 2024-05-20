using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Bislerium.Migrations
{
    /// <inheritdoc />
    public partial class v232 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalPopularity",
                table: "BlogPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPopularity",
                table: "BlogPosts");
        }
    }
}
