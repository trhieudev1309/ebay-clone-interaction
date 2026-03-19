using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbayChat.Migrations
{
    /// <inheritdoc />
    public partial class AddSeenFieldToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "seen",
                table: "Message",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "seen",
                table: "Message");
        }
    }
}
