using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildForgeApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildAndBuildComponentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Builds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuildName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Builds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Builds_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuildId = table.Column<int>(type: "INTEGER", nullable: false),
                    PcComponentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildComponents_Builds_BuildId",
                        column: x => x.BuildId,
                        principalTable: "Builds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildComponents_PcComponents_PcComponentId",
                        column: x => x.PcComponentId,
                        principalTable: "PcComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildComponents_BuildId",
                table: "BuildComponents",
                column: "BuildId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildComponents_PcComponentId",
                table: "BuildComponents",
                column: "PcComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Builds_UserId",
                table: "Builds",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildComponents");

            migrationBuilder.DropTable(
                name: "Builds");
        }
    }
}
