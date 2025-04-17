using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsPage.Migrations
{
    /// <inheritdoc />
    public partial class create_FavoriteTopics_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteTopicss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteTopicss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteTopicss_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteTopicss_UserAccounts_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteTopicss_TopicId",
                table: "FavoriteTopicss",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteTopicss_UserId",
                table: "FavoriteTopicss",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteTopicss");
        }
    }
}
