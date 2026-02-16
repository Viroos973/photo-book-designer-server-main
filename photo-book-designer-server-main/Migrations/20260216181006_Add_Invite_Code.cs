using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace photo_book_designer_server_main.Migrations
{
    /// <inheritdoc />
    public partial class Add_Invite_Code : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InviteCode",
                table: "Rooms",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InviteCode",
                table: "Rooms");
        }
    }
}
