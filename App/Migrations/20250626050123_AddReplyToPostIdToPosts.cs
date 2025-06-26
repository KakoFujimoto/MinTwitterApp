using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinTwitterApp.Migrations
{
    /// <inheritdoc />
    public partial class AddReplyToPostIdToPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReplyToPostId",
                table: "Posts",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyToPostId",
                table: "Posts");
        }
    }
}
