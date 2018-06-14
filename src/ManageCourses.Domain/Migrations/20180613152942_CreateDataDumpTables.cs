using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class CreateDataDumpTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UcasCampuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    Address3 = table.Column<string>(nullable: true),
                    Address4 = table.Column<string>(nullable: true),
                    CampusCode = table.Column<string>(nullable: true),
                    CampusName = table.Column<string>(nullable: true),
                    InstCode = table.Column<string>(nullable: true),
                    PostCode = table.Column<string>(nullable: true),
                    RegionCode = table.Column<string>(nullable: true),
                    TelNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UcasCampuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UcasCourseNotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CourseCode = table.Column<string>(nullable: true),
                    InstCode = table.Column<string>(nullable: true),
                    NoteNo = table.Column<string>(nullable: true),
                    NoteType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UcasCourseNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UcasCourses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AcreditedProvider = table.Column<string>(nullable: true),
                    AgeGroup = table.Column<string>(nullable: true),
                    CampusCode = table.Column<string>(nullable: true),
                    CourseCode = table.Column<string>(nullable: true),
                    InstCode = table.Column<string>(nullable: true),
                    OpenDate = table.Column<string>(nullable: true),
                    ProfPostFlag = table.Column<string>(nullable: true),
                    ProgramType = table.Column<string>(nullable: true),
                    StudyMode = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UcasCourses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UcasInstitutions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AcreditedProvider = table.Column<string>(nullable: true),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    Address3 = table.Column<string>(nullable: true),
                    Address4 = table.Column<string>(nullable: true),
                    ContactName = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    InstCode = table.Column<string>(nullable: true),
                    InstType = table.Column<string>(nullable: true),
                    PostCode = table.Column<string>(nullable: true),
                    Scitt = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UcasInstitutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UcasNoteTexts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    InstCode = table.Column<string>(nullable: true),
                    LineText = table.Column<string>(nullable: true),
                    NoteNo = table.Column<string>(nullable: true),
                    NoteType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UcasNoteTexts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UcasSubjects",
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
                    table.PrimaryKey("PK_UcasSubjects", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UcasCampuses");

            migrationBuilder.DropTable(
                name: "UcasCourseNotes");

            migrationBuilder.DropTable(
                name: "UcasCourses");

            migrationBuilder.DropTable(
                name: "UcasInstitutions");

            migrationBuilder.DropTable(
                name: "UcasNoteTexts");

            migrationBuilder.DropTable(
                name: "UcasSubjects");
        }
    }
}
