using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reignite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameReviewToProductReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename the table
            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "ProductReviews");

            // Rename the primary key constraint
            migrationBuilder.RenameIndex(
                name: "PK_Reviews",
                table: "ProductReviews",
                newName: "PK_ProductReviews");

            // Rename foreign key constraints
            migrationBuilder.RenameIndex(
                name: "IX_Reviews_ProductId",
                table: "ProductReviews",
                newName: "IX_ProductReviews_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_UserId_ProductId",
                table: "ProductReviews",
                newName: "IX_ProductReviews_UserId_ProductId");

            // Drop and recreate foreign keys with new names
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Products_ProductId",
                table: "ProductReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "ProductReviews");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_Products_ProductId",
                table: "ProductReviews",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_Users_UserId",
                table: "ProductReviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_Products_ProductId",
                table: "ProductReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_Users_UserId",
                table: "ProductReviews");

            // Rename indexes back
            migrationBuilder.RenameIndex(
                name: "IX_ProductReviews_ProductId",
                table: "ProductReviews",
                newName: "IX_Reviews_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductReviews_UserId_ProductId",
                table: "ProductReviews",
                newName: "IX_Reviews_UserId_ProductId");

            // Rename primary key back
            migrationBuilder.RenameIndex(
                name: "PK_ProductReviews",
                table: "ProductReviews",
                newName: "PK_Reviews");

            // Rename table back
            migrationBuilder.RenameTable(
                name: "ProductReviews",
                newName: "Reviews");

            // Recreate original foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Products_ProductId",
                table: "Reviews",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
