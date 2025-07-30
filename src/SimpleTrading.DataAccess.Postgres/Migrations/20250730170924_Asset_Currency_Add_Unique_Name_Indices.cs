using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleTrading.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Asset_Currency_Add_Unique_Name_Indices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Currency_Name",
                table: "Currency",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Name",
                table: "Asset",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Currency_Name",
                table: "Currency");

            migrationBuilder.DropIndex(
                name: "IX_Asset_Name",
                table: "Asset");
        }
    }
}
