using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleTrading.DataAccess.SqlServer.Migrations
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
            
            migrationBuilder.Sql("""
                                 UPDATE "Trade"
                                 SET Result = JSON_MODIFY(Result, '$.Source', 'CalculatedByProfitLoss')
                                 WHERE JSON_VALUE(Result, '$.Source') = 'CalculatedByBalance'
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfitLoss",
                table: "Trade",
                newName: "Balance");
            
            migrationBuilder.Sql("""
                                 UPDATE "Trade"
                                 SET Result = JSON_MODIFY(Result, '$.Source', 'CalculatedByBalance')
                                 WHERE JSON_VALUE(Result, '$.Source') = 'CalculatedByProfitLoss'
                                 """);
        }
    }
}
