using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG7311_PART2_AgriEnergyConnect.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
