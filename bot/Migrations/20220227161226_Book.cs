using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bot.Migrations
{
    public partial class Book : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    BookedTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Total = table.Column<long>(type: "INTEGER", nullable: false),
                    BookNumber = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
