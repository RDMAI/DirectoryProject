using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryProject.DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Directory_RemovedUniqueIndexForPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_departments_path",
                schema: "diretory_service",
                table: "departments");

            migrationBuilder.CreateIndex(
                name: "ix_departments_path",
                schema: "diretory_service",
                table: "departments",
                column: "path")
                .Annotation("Npgsql:IndexMethod", "gist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_departments_path",
                schema: "diretory_service",
                table: "departments");

            migrationBuilder.CreateIndex(
                name: "ix_departments_path",
                schema: "diretory_service",
                table: "departments",
                column: "path",
                unique: true)
                .Annotation("Npgsql:IndexMethod", "gist");
        }
    }
}
