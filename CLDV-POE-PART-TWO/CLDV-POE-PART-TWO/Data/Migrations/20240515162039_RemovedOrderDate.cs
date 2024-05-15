using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CLDV_POE_PART_TWO.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedOrderDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //    migrationBuilder.CreateTable(
            //        name: "Order",
            //        columns: table => new
            //        {
            //            OrderID = table.Column<int>(type: "int", nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //            Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            Address = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Order", x => x.OrderID);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "OrderItem",
            //        columns: table => new
            //        {
            //            OrderItemID = table.Column<int>(type: "int", nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            OrderID = table.Column<int>(type: "int", nullable: false),
            //            ProductID = table.Column<int>(type: "int", nullable: false),
            //            Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_OrderItem", x => x.OrderItemID);
            //            table.ForeignKey(
            //                name: "FK_OrderItem_Order_OrderID",
            //                column: x => x.OrderID,
            //                principalTable: "Order",
            //                principalColumn: "OrderID",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_OrderItem_Products_ProductID",
            //                column: x => x.ProductID,
            //                principalTable: "Products",
            //                principalColumn: "ProductID",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateIndex(
            //        name: "IX_OrderItem_OrderID",
            //        table: "OrderItem",
            //        column: "OrderID");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_OrderItem_ProductID",
            //        table: "OrderItem",
            //        column: "ProductID");
            //

            // Drop the OrderDate column from the Order table
            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "Order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
               name: "OrderDate",
               table: "Order",
               type: "datetime",
               nullable: true);
        }
    }
}
