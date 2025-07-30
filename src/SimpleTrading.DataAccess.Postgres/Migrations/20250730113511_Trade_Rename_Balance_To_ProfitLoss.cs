using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleTrading.DataAccess.Postgres.Migrations
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
                                 SET "Result"['Source'] = to_jsonb('CalculatedByProfitLoss'::text)
                                 WHERE "Result"->>'Source' = 'CalculatedByBalance'
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
                                 SET "Result"['Source'] = to_jsonb('CalculatedByBalance'::text)
                                 WHERE "Result"->>'Source' = 'CalculatedByProfitLoss'
                                 """);
        }
    }
}
