using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryProject.DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class nullable_Logo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "logo",
                schema: "directory_service",
                table: "departments",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "logo",
                schema: "directory_service",
                table: "departments",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
