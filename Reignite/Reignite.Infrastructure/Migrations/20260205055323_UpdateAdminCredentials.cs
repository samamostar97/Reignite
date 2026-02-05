using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reignite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "PasswordHash" },
                values: new object[] { "admin@reignite.com", "$2a$11$u7rBIoSp8ouEtSRDsEUwCe3PXUpc3EoPZTYhp6D0abbXyOEOk.gjS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "PasswordHash" },
                values: new object[] { "admin", "$2a$11$r2qfyc5ic4xn12pabxQnqutUdAnwvk.pZk9868MOTFC4Jo6dQOCPK" });
        }
    }
}
