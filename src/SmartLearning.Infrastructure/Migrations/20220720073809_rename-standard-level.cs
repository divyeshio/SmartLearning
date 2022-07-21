using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLearning.Infrastructure.Migrations
{
    public partial class renamestandardlevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Standards_Name",
                table: "Standards");

            migrationBuilder.DropIndex(
                name: "IX_Classes_Name",
                table: "Classes");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Standards",
                newName: "Level");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Standards",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Standards",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Standards",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Standards_Name",
                table: "Standards",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_Name",
                table: "Classes",
                column: "Name",
                unique: true);
        }
    }
}
