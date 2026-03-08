using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Rename_PurchaseOrders_to_OrderRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellOrderSuggestions_PurchaseOrders_PurchaseOrderId",
                table: "SellOrderSuggestions");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.RenameColumn(
                name: "PurchaseOrderId",
                table: "SellOrderSuggestions",
                newName: "OrderRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_SellOrderSuggestions_PurchaseOrderId",
                table: "SellOrderSuggestions",
                newName: "IX_SellOrderSuggestions_OrderRequestId");

            migrationBuilder.CreateTable(
                name: "OrderRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    MaxPrice = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderRequests_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderRequests_CreatorId",
                table: "OrderRequests",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellOrderSuggestions_OrderRequests_OrderRequestId",
                table: "SellOrderSuggestions",
                column: "OrderRequestId",
                principalTable: "OrderRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellOrderSuggestions_OrderRequests_OrderRequestId",
                table: "SellOrderSuggestions");

            migrationBuilder.DropTable(
                name: "OrderRequests");

            migrationBuilder.RenameColumn(
                name: "OrderRequestId",
                table: "SellOrderSuggestions",
                newName: "PurchaseOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_SellOrderSuggestions_OrderRequestId",
                table: "SellOrderSuggestions",
                newName: "IX_SellOrderSuggestions_PurchaseOrderId");

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    MaxPrice = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CreatorId",
                table: "PurchaseOrders",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellOrderSuggestions_PurchaseOrders_PurchaseOrderId",
                table: "SellOrderSuggestions",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
