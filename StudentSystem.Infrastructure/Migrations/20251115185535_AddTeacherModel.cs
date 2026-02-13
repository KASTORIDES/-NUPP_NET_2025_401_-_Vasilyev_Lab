using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherModelId",
                table: "Courses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    Salary = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherModelId",
                table: "Courses",
                column: "TeacherModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Teachers_TeacherModelId",
                table: "Courses",
                column: "TeacherModelId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Teachers_TeacherModelId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherModelId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "TeacherModelId",
                table: "Courses");
        }
    }
}
