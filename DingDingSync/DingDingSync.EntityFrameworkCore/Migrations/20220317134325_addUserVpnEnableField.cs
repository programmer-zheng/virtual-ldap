using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DingDingSync.EntityFrameworkCore.Migrations
{
    public partial class addUserVpnEnableField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "VpnAccountEnabled",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VpnAccountEnabled",
                table: "Users");
        }
    }
}
