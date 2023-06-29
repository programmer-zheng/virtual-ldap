using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualLdap.EntityFrameworkCore.Migrations
{
    public partial class changeRelaEntityKeyAndIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDepartmentsRelations",
                table: "UserDepartmentsRelations");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserDepartmentsRelations",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDepartmentsRelations",
                table: "UserDepartmentsRelations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartmentsRelations_Id_DeptId",
                table: "UserDepartmentsRelations",
                columns: new[] { "Id", "DeptId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDepartmentsRelations",
                table: "UserDepartmentsRelations");

            migrationBuilder.DropIndex(
                name: "IX_UserDepartmentsRelations_Id_DeptId",
                table: "UserDepartmentsRelations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserDepartmentsRelations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDepartmentsRelations",
                table: "UserDepartmentsRelations",
                columns: new[] { "Id", "DeptId" });
        }
    }
}
