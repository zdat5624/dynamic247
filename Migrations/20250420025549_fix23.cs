using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsPage.Migrations
{
    /// <inheritdoc />
    public partial class fix23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageVisitors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccessCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageVisitors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageVisitors_UserAccounts_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageVisitors_UserId",
                table: "PageVisitors",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageVisitors");
        }
    }
}
