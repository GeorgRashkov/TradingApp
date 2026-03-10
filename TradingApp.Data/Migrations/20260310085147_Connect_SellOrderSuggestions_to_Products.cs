using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Connect_SellOrderSuggestions_to_Products : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellOrderSuggestions_SellOrders_SellOrderId",
                table: "SellOrderSuggestions");

            migrationBuilder.RenameColumn(
                name: "SellOrderId",
                table: "SellOrderSuggestions",
                newName: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellOrderSuggestions_Products_ProductId",
                table: "SellOrderSuggestions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellOrderSuggestions_Products_ProductId",
                table: "SellOrderSuggestions");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "SellOrderSuggestions",
                newName: "SellOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellOrderSuggestions_SellOrders_SellOrderId",
                table: "SellOrderSuggestions",
                column: "SellOrderId",
                principalTable: "SellOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
