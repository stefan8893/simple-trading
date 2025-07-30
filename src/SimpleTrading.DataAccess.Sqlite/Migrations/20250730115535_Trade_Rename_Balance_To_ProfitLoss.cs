using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleTrading.DataAccess.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class Trade_Rename_Balance_To_ProfitLoss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "Trade",
                newName: "ProfitLoss");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfitLoss",
                table: "Trade",
                newName: "Balance");
        }
    }
}
