using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reignite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHobbyIconUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "Hobbies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "Hobbies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 1,
                column: "IconUrl",
                value: "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=100");

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 2,
                column: "IconUrl",
                value: "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=100");

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 3,
                column: "IconUrl",
                value: "https://images.unsplash.com/photo-1504328345606-18bbc8c9d7d1?w=100");

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 4,
                column: "IconUrl",
                value: "https://images.unsplash.com/photo-1493106641515-6b5631de4bb9?w=100");

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 5,
                column: "IconUrl",
                value: "https://images.unsplash.com/photo-1602523961358-f9f03dd557db?w=100");

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 6,
                column: "IconUrl",
                value: "https://images.unsplash.com/photo-1452860606245-08befc0ff44b?w=100");
        }
    }
}
