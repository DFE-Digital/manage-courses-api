using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class ChangedFieldNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostCode",
                table: "UcasInstitutions",
                newName: "Postcode");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "UcasInstitutions",
                newName: "YearCode");

            migrationBuilder.RenameColumn(
                name: "Address4",
                table: "UcasInstitutions",
                newName: "SchemeMember");

            migrationBuilder.RenameColumn(
                name: "Address3",
                table: "UcasInstitutions",
                newName: "InstName");

            migrationBuilder.RenameColumn(
                name: "Address2",
                table: "UcasInstitutions",
                newName: "InstFull");

            migrationBuilder.RenameColumn(
                name: "Address1",
                table: "UcasInstitutions",
                newName: "InstBig");

            migrationBuilder.RenameColumn(
                name: "AcreditedProvider",
                table: "UcasInstitutions",
                newName: "Addr4");

            migrationBuilder.RenameColumn(
                name: "CourseCode",
                table: "UcasCourseSubjects",
                newName: "YearCode");

            migrationBuilder.RenameColumn(
                name: "StudyMode",
                table: "UcasCourses",
                newName: "Studymode");

            migrationBuilder.RenameColumn(
                name: "ProfPostFlag",
                table: "UcasCourses",
                newName: "ProfpostFlag");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "UcasCourses",
                newName: "CrseTitle");

            migrationBuilder.RenameColumn(
                name: "OpenDate",
                table: "UcasCourses",
                newName: "CrseOpenDate");

            migrationBuilder.RenameColumn(
                name: "CourseCode",
                table: "UcasCourses",
                newName: "CrseCode");

            migrationBuilder.RenameColumn(
                name: "AgeGroup",
                table: "UcasCourses",
                newName: "Age");

            migrationBuilder.RenameColumn(
                name: "AcreditedProvider",
                table: "UcasCourses",
                newName: "AccreditingProvider");

            migrationBuilder.RenameColumn(
                name: "CourseCode",
                table: "UcasCourseNotes",
                newName: "YearCode");

            migrationBuilder.RenameColumn(
                name: "PostCode",
                table: "UcasCampuses",
                newName: "Postcode");

            migrationBuilder.RenameColumn(
                name: "Address4",
                table: "UcasCampuses",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "Address3",
                table: "UcasCampuses",
                newName: "Addr4");

            migrationBuilder.RenameColumn(
                name: "Address2",
                table: "UcasCampuses",
                newName: "Addr3");

            migrationBuilder.RenameColumn(
                name: "Address1",
                table: "UcasCampuses",
                newName: "Addr2");

            migrationBuilder.AddColumn<string>(
                name: "YearCode",
                table: "UcasNoteTexts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccreditingProvider",
                table: "UcasInstitutions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Addr1",
                table: "UcasInstitutions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Addr2",
                table: "UcasInstitutions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Addr3",
                table: "UcasInstitutions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrseCode",
                table: "UcasCourseSubjects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrseCode",
                table: "UcasCourseNotes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Addr1",
                table: "UcasCampuses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearCode",
                table: "UcasNoteTexts");

            migrationBuilder.DropColumn(
                name: "AccreditingProvider",
                table: "UcasInstitutions");

            migrationBuilder.DropColumn(
                name: "Addr1",
                table: "UcasInstitutions");

            migrationBuilder.DropColumn(
                name: "Addr2",
                table: "UcasInstitutions");

            migrationBuilder.DropColumn(
                name: "Addr3",
                table: "UcasInstitutions");

            migrationBuilder.DropColumn(
                name: "CrseCode",
                table: "UcasCourseSubjects");

            migrationBuilder.DropColumn(
                name: "CrseCode",
                table: "UcasCourseNotes");

            migrationBuilder.DropColumn(
                name: "Addr1",
                table: "UcasCampuses");

            migrationBuilder.RenameColumn(
                name: "Postcode",
                table: "UcasInstitutions",
                newName: "PostCode");

            migrationBuilder.RenameColumn(
                name: "YearCode",
                table: "UcasInstitutions",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "SchemeMember",
                table: "UcasInstitutions",
                newName: "Address4");

            migrationBuilder.RenameColumn(
                name: "InstName",
                table: "UcasInstitutions",
                newName: "Address3");

            migrationBuilder.RenameColumn(
                name: "InstFull",
                table: "UcasInstitutions",
                newName: "Address2");

            migrationBuilder.RenameColumn(
                name: "InstBig",
                table: "UcasInstitutions",
                newName: "Address1");

            migrationBuilder.RenameColumn(
                name: "Addr4",
                table: "UcasInstitutions",
                newName: "AcreditedProvider");

            migrationBuilder.RenameColumn(
                name: "YearCode",
                table: "UcasCourseSubjects",
                newName: "CourseCode");

            migrationBuilder.RenameColumn(
                name: "Studymode",
                table: "UcasCourses",
                newName: "StudyMode");

            migrationBuilder.RenameColumn(
                name: "ProfpostFlag",
                table: "UcasCourses",
                newName: "ProfPostFlag");

            migrationBuilder.RenameColumn(
                name: "CrseTitle",
                table: "UcasCourses",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "CrseOpenDate",
                table: "UcasCourses",
                newName: "OpenDate");

            migrationBuilder.RenameColumn(
                name: "CrseCode",
                table: "UcasCourses",
                newName: "CourseCode");

            migrationBuilder.RenameColumn(
                name: "Age",
                table: "UcasCourses",
                newName: "AgeGroup");

            migrationBuilder.RenameColumn(
                name: "AccreditingProvider",
                table: "UcasCourses",
                newName: "AcreditedProvider");

            migrationBuilder.RenameColumn(
                name: "YearCode",
                table: "UcasCourseNotes",
                newName: "CourseCode");

            migrationBuilder.RenameColumn(
                name: "Postcode",
                table: "UcasCampuses",
                newName: "PostCode");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "UcasCampuses",
                newName: "Address4");

            migrationBuilder.RenameColumn(
                name: "Addr4",
                table: "UcasCampuses",
                newName: "Address3");

            migrationBuilder.RenameColumn(
                name: "Addr3",
                table: "UcasCampuses",
                newName: "Address2");

            migrationBuilder.RenameColumn(
                name: "Addr2",
                table: "UcasCampuses",
                newName: "Address1");
        }
    }
}
