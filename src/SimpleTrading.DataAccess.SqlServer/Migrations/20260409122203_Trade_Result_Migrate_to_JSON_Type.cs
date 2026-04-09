using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleTrading.DataAccess.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Trade_Result_Migrate_to_JSON_Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Result",
                table: "Trade",
                type: "json",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
