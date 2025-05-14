using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG7311_PART2_AgriEnergyConnect.Migrations
{
    /// <inheritdoc />
    public partial class AplicationDbConxtcs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Farmers");

            migrationBuilder.AddColumn<int>(
                name: "FarmerId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FarmerId",
                table: "AspNetUsers",
                column: "FarmerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Farmers_FarmerId",
                table: "AspNetUsers",
                column: "FarmerId",
                principalTable: "Farmers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Farmers_FarmerId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FarmerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FarmerId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Farmers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
