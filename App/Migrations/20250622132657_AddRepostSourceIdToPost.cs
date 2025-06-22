using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinTwitterApp.Migrations
{
    /// <inheritdoc />
    public partial class AddRepostSourceIdToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RepostSourceId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepostSourceId",
                table: "Posts");
        }
    }
}
