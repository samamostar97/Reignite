using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Reignite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectReviews_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectReviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Umjetnost izrade predmeta od drveta, od namještaja do dekorativnih komada.", "Stolarija" });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Izrada novčanika, remena, torbi i drugih predmeta od kože.", "Obrada kože" });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Oblikovanje i kovanje metala u alate, umjetničke i funkcionalne predmete.", "Obrada metala" });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Oblikovanje gline u keramičke predmete, od posuda do skulptura.", "Keramika" });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Izrada jedinstvenih svijeća sa posebnim mirisima i dizajnom.", "Izrada svijeća" });

            migrationBuilder.InsertData(
                table: "ProjectReviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "IsDeleted", "ProjectId", "Rating", "UserId" },
                values: new object[,]
                {
                    { 1, "Prelijepo urađen novčanik! Šavovi su savršeni, vidi se pažnja u svakom detalju.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, 5, 3 },
                    { 2, "Odličan rad za prvi projekat. Koža je lijepo obrađena.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, 4, 4 },
                    { 3, "Rustični stil je fantastičan! Hrast je prelijepo došao do izražaja.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, 5, 2 },
                    { 4, "Polica izgleda profesionalno. Svaka čast na vještini!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, 5, 4 },
                    { 5, "Kombinacija drveta i metala je odlična ideja. Veoma praktično.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 3, 4, 2 },
                    { 6, "Ovo je pravo majstorstvo! Kovani detalji su nevjerovatni.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 3, 5, 3 },
                    { 7, "Lijep set kutlača, prirodna obrada drveta je super.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 4, 4, 3 },
                    { 8, "Ručni rad u najboljem izdanju. Koristim ih svaki dan!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 4, 5, 4 },
                    { 9, "Elegantna futrola, magnetni zatvarač je odličan dodatak.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 5, 5, 2 },
                    { 10, "Kvalitetna izrada, koža je mekana i ugodna na dodir.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 5, 4, 4 },
                    { 11, "Moderni dizajn koji se uklapa u svaki prostor. Bravo!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 6, 5, 2 },
                    { 12, "Kovano željezo daje poseban šarm. Odlična završna obrada.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 6, 5, 3 }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "admin");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectReviews_ProjectId",
                table: "ProjectReviews",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectReviews_UserId_ProjectId",
                table: "ProjectReviews",
                columns: new[] { "UserId", "ProjectId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectReviews");

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "The art of crafting objects from wood, from furniture to decorative pieces.", "Woodworking" });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Creating wallets, belts, bags and other items from leather.", "Leathercraft" });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Shaping and forging metal into tools, art, and functional objects.", "Metalworking" });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Molding clay into ceramics, from bowls to sculptures.", "Pottery" });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Crafting custom candles with unique scents and designs.", "Candle Making" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "admin@Reignition.com");
        }
    }
}
