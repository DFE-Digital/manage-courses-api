using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AddedSubjecttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InstCode",
                table: "UcasSubjects",
                newName: "TitleMatch");

            migrationBuilder.RenameColumn(
                name: "CourseCode",
                table: "UcasSubjects",
                newName: "SubjectDescription");

            migrationBuilder.CreateTable(
                name: "UcasCourseSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CourseCode = table.Column<string>(nullable: true),
                    InstCode = table.Column<string>(nullable: true),
                    SubjectCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UcasCourseSubjects", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UcasCourseSubjects");

            migrationBuilder.RenameColumn(
                name: "TitleMatch",
                table: "UcasSubjects",
                newName: "InstCode");

            migrationBuilder.RenameColumn(
                name: "SubjectDescription",
                table: "UcasSubjects",
                newName: "CourseCode");
        }
    }
}
