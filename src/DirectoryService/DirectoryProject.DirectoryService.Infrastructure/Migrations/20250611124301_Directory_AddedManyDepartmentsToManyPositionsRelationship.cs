﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryProject.DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Directory_AddedManyDepartmentsToManyPositionsRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "department_positions",
                schema: "diretory_service",
                columns: table => new
                {
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_department_positions", x => new { x.department_id, x.position_id });
                    table.ForeignKey(
                        name: "fk_department_positions_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "diretory_service",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_department_positions_positions_position_id",
                        column: x => x.position_id,
                        principalSchema: "diretory_service",
                        principalTable: "positions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_department_positions_position_id",
                schema: "diretory_service",
                table: "department_positions",
                column: "position_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "department_positions",
                schema: "diretory_service");
        }
    }
}
