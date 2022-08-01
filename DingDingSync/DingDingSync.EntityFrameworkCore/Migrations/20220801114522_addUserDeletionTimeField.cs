using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DingDingSync.EntityFrameworkCore.Migrations
{
    public partial class addUserDeletionTimeField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Users",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Users");
        }
    }
}
