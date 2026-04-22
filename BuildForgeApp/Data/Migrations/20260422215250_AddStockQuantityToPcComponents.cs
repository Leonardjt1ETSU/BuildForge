using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildForgeApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStockQuantityToPcComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "PcComponents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "PcComponents");
        }
    }
}
