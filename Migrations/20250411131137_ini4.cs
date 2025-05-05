using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class ini4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_users_UserId",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_UserId",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tasks");

            migrationBuilder.AlterColumn<string>(
                name: "TaskName",
                table: "tasks",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TaskName",
                table: "tasks",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tasks_UserId",
                table: "tasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_users_UserId",
                table: "tasks",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId");
        }
    }
}
