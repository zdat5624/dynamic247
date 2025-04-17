using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsPage.Migrations
{
    /// <inheritdoc />
    public partial class fixTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteTopicss_Topics_TopicId",
                table: "FavoriteTopicss");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteTopicss_UserAccounts_UserId",
                table: "FavoriteTopicss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteTopicss",
                table: "FavoriteTopicss");

            migrationBuilder.RenameTable(
                name: "FavoriteTopicss",
                newName: "FavoriteTopics");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteTopicss_UserId",
                table: "FavoriteTopics",
                newName: "IX_FavoriteTopics_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteTopicss_TopicId",
                table: "FavoriteTopics",
                newName: "IX_FavoriteTopics_TopicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteTopics",
                table: "FavoriteTopics",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteTopics_Topics_TopicId",
                table: "FavoriteTopics",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteTopics_UserAccounts_UserId",
                table: "FavoriteTopics",
                column: "UserId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteTopics_Topics_TopicId",
                table: "FavoriteTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteTopics_UserAccounts_UserId",
                table: "FavoriteTopics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteTopics",
                table: "FavoriteTopics");

            migrationBuilder.RenameTable(
                name: "FavoriteTopics",
                newName: "FavoriteTopicss");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteTopics_UserId",
                table: "FavoriteTopicss",
                newName: "IX_FavoriteTopicss_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteTopics_TopicId",
                table: "FavoriteTopicss",
                newName: "IX_FavoriteTopicss_TopicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteTopicss",
                table: "FavoriteTopicss",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteTopicss_Topics_TopicId",
                table: "FavoriteTopicss",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteTopicss_UserAccounts_UserId",
                table: "FavoriteTopicss",
                column: "UserId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
