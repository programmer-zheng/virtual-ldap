using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DingDingSync.EntityFrameworkCore.Migrations
{
    public partial class AddUserUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserName",
                table: "Users");
        }
    }
}
