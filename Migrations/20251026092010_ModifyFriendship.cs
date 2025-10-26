using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatAppApi.Migrations
{
    /// <inheritdoc />
    public partial class ModifyFriendship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PrivateConversationId",
                table: "Friendship",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_PrivateConversationId",
                table: "Friendship",
                column: "PrivateConversationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_Conversation_PrivateConversationId",
                table: "Friendship",
                column: "PrivateConversationId",
                principalTable: "Conversation",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_Conversation_PrivateConversationId",
                table: "Friendship");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_PrivateConversationId",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "PrivateConversationId",
                table: "Friendship");
        }
    }
}
