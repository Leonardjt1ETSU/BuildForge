using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildForgeApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPcComponentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PcComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Brand = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ComponentType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    SocketType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    FormFactor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Wattage = table.Column<int>(type: "INTEGER", nullable: true),
                    CapacityGB = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcComponents", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PcComponents");
        }
    }
}
