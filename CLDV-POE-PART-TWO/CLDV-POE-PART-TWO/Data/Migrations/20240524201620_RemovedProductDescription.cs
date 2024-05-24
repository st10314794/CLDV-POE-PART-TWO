using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CLDV_POE_PART_TWO.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedProductDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductDescription",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
