using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addingBuyerAndSellerTitlesForCompletedOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "CompletedOrders",
                newName: "TitleForSeller");

            migrationBuilder.AddColumn<string>(
                name: "TitleForBuyer",
                table: "CompletedOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleForBuyer",
                table: "CompletedOrders");

            migrationBuilder.RenameColumn(
                name: "TitleForSeller",
                table: "CompletedOrders",
                newName: "Title");
        }
    }
}
