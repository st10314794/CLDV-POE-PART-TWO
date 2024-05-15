using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CLDV_POE_PART_TWO.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserOrderNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Order",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserOrderNumber",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserID",
                table: "Order",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_UserID",
                table: "Order",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_UserID",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_UserID",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "UserOrderNumber",
                table: "Order");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
