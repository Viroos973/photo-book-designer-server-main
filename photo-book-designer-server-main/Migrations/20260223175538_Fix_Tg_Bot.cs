using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace photo_book_designer_server_main.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Tg_Bot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "TgBots",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "TgBots");
        }
    }
}
