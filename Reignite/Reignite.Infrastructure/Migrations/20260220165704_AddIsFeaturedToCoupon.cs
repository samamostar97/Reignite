using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reignite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFeaturedToCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Coupons",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Coupons");
        }
    }
}
