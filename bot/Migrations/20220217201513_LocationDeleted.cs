using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bot.Migrations
{
    public partial class LocationDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Location_LocationLongitude",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Users_LocationLongitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LocationLongitude",
                table: "Users");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Users",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Users",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Users");

            migrationBuilder.AddColumn<double>(
                name: "LocationLongitude",
                table: "Users",
                type: "REAL",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Longitude);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_LocationLongitude",
                table: "Users",
                column: "LocationLongitude");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Location_LocationLongitude",
                table: "Users",
                column: "LocationLongitude",
                principalTable: "Location",
                principalColumn: "Longitude");
        }
    }
}
