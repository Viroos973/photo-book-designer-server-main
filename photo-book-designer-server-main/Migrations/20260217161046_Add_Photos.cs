using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace photo_book_designer_server_main.Migrations
{
    /// <inheritdoc />
    public partial class Add_Photos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomPhotos",
                columns: table => new
                {
                    ImageId = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPhotos", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_RoomPhotos_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomPhotos_RoomId",
                table: "RoomPhotos",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomPhotos");
        }
    }
}
