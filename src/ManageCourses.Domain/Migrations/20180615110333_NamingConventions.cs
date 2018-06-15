using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class NamingConventions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Providers_ProviderId",
                table: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UcasSubjects",
                table: "UcasSubjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UcasNoteTexts",
                table: "UcasNoteTexts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UcasInstitutions",
                table: "UcasInstitutions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UcasCourseSubjects",
                table: "UcasCourseSubjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UcasCourses",
                table: "UcasCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UcasCourseNotes",
                table: "UcasCourseNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UcasCampuses",
                table: "UcasCampuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Providers",
                table: "Providers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProviderMapper",
                table: "ProviderMapper");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "UcasSubjects",
                newName: "ucas_subject");

            migrationBuilder.RenameTable(
                name: "UcasNoteTexts",
                newName: "ucas_note_text");

            migrationBuilder.RenameTable(
                name: "UcasInstitutions",
                newName: "ucas_institution");

            migrationBuilder.RenameTable(
                name: "UcasCourseSubjects",
                newName: "ucas_course_subject");

            migrationBuilder.RenameTable(
                name: "UcasCourses",
                newName: "ucas_course");

            migrationBuilder.RenameTable(
                name: "UcasCourseNotes",
                newName: "ucas_course_note");

            migrationBuilder.RenameTable(
                name: "UcasCampuses",
                newName: "ucas_campus");

            migrationBuilder.RenameTable(
                name: "Providers",
                newName: "provider");

            migrationBuilder.RenameTable(
                name: "ProviderMapper",
                newName: "provider_mapper");

            migrationBuilder.RenameTable(
                name: "Courses",
                newName: "course");

            migrationBuilder.RenameColumn(
                name: "TitleMatch",
                table: "ucas_subject",
                newName: "title_match");

            migrationBuilder.RenameColumn(
                name: "SubjectDescription",
                table: "ucas_subject",
                newName: "subject_description");

            migrationBuilder.RenameColumn(
                name: "SubjectCode",
                table: "ucas_subject",
                newName: "subject_code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ucas_subject",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "YearCode",
                table: "ucas_note_text",
                newName: "year_code");

            migrationBuilder.RenameColumn(
                name: "NoteType",
                table: "ucas_note_text",
                newName: "note_type");

            migrationBuilder.RenameColumn(
                name: "NoteNo",
                table: "ucas_note_text",
                newName: "note_no");

            migrationBuilder.RenameColumn(
                name: "LineText",
                table: "ucas_note_text",
                newName: "line_text");

            migrationBuilder.RenameColumn(
                name: "InstCode",
                table: "ucas_note_text",
                newName: "inst_code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ucas_note_text",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "YearCode",
                table: "ucas_institution",
                newName: "year_code");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "ucas_institution",
                newName: "url");

            migrationBuilder.RenameColumn(
                name: "Scitt",
                table: "ucas_institution",
                newName: "scitt");

            migrationBuilder.RenameColumn(
                name: "SchemeMember",
                table: "ucas_institution",
                newName: "scheme_member");

            migrationBuilder.RenameColumn(
                name: "Postcode",
                table: "ucas_institution",
                newName: "postcode");

            migrationBuilder.RenameColumn(
                name: "InstType",
                table: "ucas_institution",
                newName: "inst_type");

            migrationBuilder.RenameColumn(
                name: "InstName",
                table: "ucas_institution",
                newName: "inst_name");

            migrationBuilder.RenameColumn(
                name: "InstFull",
                table: "ucas_institution",
                newName: "inst_full");

            migrationBuilder.RenameColumn(
                name: "InstCode",
                table: "ucas_institution",
                newName: "inst_code");

            migrationBuilder.RenameColumn(
                name: "InstBig",
                table: "ucas_institution",
                newName: "inst_big");

            migrationBuilder.RenameColumn(
                name: "ContactName",
                table: "ucas_institution",
                newName: "contact_name");

            migrationBuilder.RenameColumn(
                name: "Addr4",
                table: "ucas_institution",
                newName: "addr4");

            migrationBuilder.RenameColumn(
                name: "Addr3",
                table: "ucas_institution",
                newName: "addr3");

            migrationBuilder.RenameColumn(
                name: "Addr2",
                table: "ucas_institution",
                newName: "addr2");

            migrationBuilder.RenameColumn(
                name: "Addr1",
                table: "ucas_institution",
                newName: "addr1");

            migrationBuilder.RenameColumn(
                name: "AccreditingProvider",
                table: "ucas_institution",
                newName: "accrediting_provider");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ucas_institution",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "YearCode",
                table: "ucas_course_subject",
                newName: "year_code");

            migrationBuilder.RenameColumn(
                name: "SubjectCode",
                table: "ucas_course_subject",
                newName: "subject_code");

            migrationBuilder.RenameColumn(
                name: "InstCode",
                table: "ucas_course_subject",
                newName: "inst_code");

            migrationBuilder.RenameColumn(
                name: "CrseCode",
                table: "ucas_course_subject",
                newName: "crse_code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ucas_course_subject",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Studymode",
                table: "ucas_course",
                newName: "studymode");

            migrationBuilder.RenameColumn(
                name: "ProgramType",
                table: "ucas_course",
                newName: "program_type");

            migrationBuilder.RenameColumn(
                name: "ProfpostFlag",
                table: "ucas_course",
                newName: "profpost_flag");

            migrationBuilder.RenameColumn(
                name: "InstCode",
                table: "ucas_course",
                newName: "inst_code");

            migrationBuilder.RenameColumn(
                name: "CrseTitle",
                table: "ucas_course",
                newName: "crse_title");

            migrationBuilder.RenameColumn(
                name: "CrseOpenDate",
                table: "ucas_course",
                newName: "crse_open_date");

            migrationBuilder.RenameColumn(
                name: "CrseCode",
                table: "ucas_course",
                newName: "crse_code");

            migrationBuilder.RenameColumn(
                name: "CampusCode",
                table: "ucas_course",
                newName: "campus_code");

            migrationBuilder.RenameColumn(
                name: "Age",
                table: "ucas_course",
                newName: "age");

            migrationBuilder.RenameColumn(
                name: "AccreditingProvider",
                table: "ucas_course",
                newName: "accrediting_provider");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ucas_course",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "YearCode",
                table: "ucas_course_note",
                newName: "year_code");

            migrationBuilder.RenameColumn(
                name: "NoteType",
                table: "ucas_course_note",
                newName: "note_type");

            migrationBuilder.RenameColumn(
                name: "NoteNo",
                table: "ucas_course_note",
                newName: "note_no");

            migrationBuilder.RenameColumn(
                name: "InstCode",
                table: "ucas_course_note",
                newName: "inst_code");

            migrationBuilder.RenameColumn(
                name: "CrseCode",
                table: "ucas_course_note",
                newName: "crse_code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ucas_course_note",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TelNo",
                table: "ucas_campus",
                newName: "tel_no");

            migrationBuilder.RenameColumn(
                name: "RegionCode",
                table: "ucas_campus",
                newName: "region_code");

            migrationBuilder.RenameColumn(
                name: "Postcode",
                table: "ucas_campus",
                newName: "postcode");

            migrationBuilder.RenameColumn(
                name: "InstCode",
                table: "ucas_campus",
                newName: "inst_code");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "ucas_campus",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "CampusName",
                table: "ucas_campus",
                newName: "campus_name");

            migrationBuilder.RenameColumn(
                name: "CampusCode",
                table: "ucas_campus",
                newName: "campus_code");

            migrationBuilder.RenameColumn(
                name: "Addr4",
                table: "ucas_campus",
                newName: "addr4");

            migrationBuilder.RenameColumn(
                name: "Addr3",
                table: "ucas_campus",
                newName: "addr3");

            migrationBuilder.RenameColumn(
                name: "Addr2",
                table: "ucas_campus",
                newName: "addr2");

            migrationBuilder.RenameColumn(
                name: "Addr1",
                table: "ucas_campus",
                newName: "addr1");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ucas_campus",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "NctlId",
                table: "provider",
                newName: "nctl_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "provider",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Urn",
                table: "provider_mapper",
                newName: "urn");

            migrationBuilder.RenameColumn(
                name: "UcasCode",
                table: "provider_mapper",
                newName: "ucas_code");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "provider_mapper",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "NctlId",
                table: "provider_mapper",
                newName: "nctl_id");

            migrationBuilder.RenameColumn(
                name: "InstitutionName",
                table: "provider_mapper",
                newName: "institution_name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "provider_mapper",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UcasCode",
                table: "course",
                newName: "ucas_code");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "course",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "course",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "course",
                newName: "provider_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "course",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_ProviderId",
                table: "course",
                newName: "IX_course_provider_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ucas_subject",
                table: "ucas_subject",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ucas_note_text",
                table: "ucas_note_text",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ucas_institution",
                table: "ucas_institution",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ucas_course_subject",
                table: "ucas_course_subject",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ucas_course",
                table: "ucas_course",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ucas_course_note",
                table: "ucas_course_note",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ucas_campus",
                table: "ucas_campus",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_provider",
                table: "provider",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_provider_mapper",
                table: "provider_mapper",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_course",
                table: "course",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_course_provider_provider_id",
                table: "course",
                column: "provider_id",
                principalTable: "provider",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_provider_provider_id",
                table: "course");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ucas_subject",
                table: "ucas_subject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ucas_note_text",
                table: "ucas_note_text");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ucas_institution",
                table: "ucas_institution");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ucas_course_subject",
                table: "ucas_course_subject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ucas_course_note",
                table: "ucas_course_note");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ucas_course",
                table: "ucas_course");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ucas_campus",
                table: "ucas_campus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_provider_mapper",
                table: "provider_mapper");

            migrationBuilder.DropPrimaryKey(
                name: "PK_provider",
                table: "provider");

            migrationBuilder.DropPrimaryKey(
                name: "PK_course",
                table: "course");

            migrationBuilder.RenameTable(
                name: "ucas_subject",
                newName: "UcasSubjects");

            migrationBuilder.RenameTable(
                name: "ucas_note_text",
                newName: "UcasNoteTexts");

            migrationBuilder.RenameTable(
                name: "ucas_institution",
                newName: "UcasInstitutions");

            migrationBuilder.RenameTable(
                name: "ucas_course_subject",
                newName: "UcasCourseSubjects");

            migrationBuilder.RenameTable(
                name: "ucas_course_note",
                newName: "UcasCourseNotes");

            migrationBuilder.RenameTable(
                name: "ucas_course",
                newName: "UcasCourses");

            migrationBuilder.RenameTable(
                name: "ucas_campus",
                newName: "UcasCampuses");

            migrationBuilder.RenameTable(
                name: "provider_mapper",
                newName: "ProviderMapper");

            migrationBuilder.RenameTable(
                name: "provider",
                newName: "Providers");

            migrationBuilder.RenameTable(
                name: "course",
                newName: "Courses");

            migrationBuilder.RenameColumn(
                name: "title_match",
                table: "UcasSubjects",
                newName: "TitleMatch");

            migrationBuilder.RenameColumn(
                name: "subject_description",
                table: "UcasSubjects",
                newName: "SubjectDescription");

            migrationBuilder.RenameColumn(
                name: "subject_code",
                table: "UcasSubjects",
                newName: "SubjectCode");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UcasSubjects",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "year_code",
                table: "UcasNoteTexts",
                newName: "YearCode");

            migrationBuilder.RenameColumn(
                name: "note_type",
                table: "UcasNoteTexts",
                newName: "NoteType");

            migrationBuilder.RenameColumn(
                name: "note_no",
                table: "UcasNoteTexts",
                newName: "NoteNo");

            migrationBuilder.RenameColumn(
                name: "line_text",
                table: "UcasNoteTexts",
                newName: "LineText");

            migrationBuilder.RenameColumn(
                name: "inst_code",
                table: "UcasNoteTexts",
                newName: "InstCode");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UcasNoteTexts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "year_code",
                table: "UcasInstitutions",
                newName: "YearCode");

            migrationBuilder.RenameColumn(
                name: "url",
                table: "UcasInstitutions",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "scitt",
                table: "UcasInstitutions",
                newName: "Scitt");

            migrationBuilder.RenameColumn(
                name: "scheme_member",
                table: "UcasInstitutions",
                newName: "SchemeMember");

            migrationBuilder.RenameColumn(
                name: "postcode",
                table: "UcasInstitutions",
                newName: "Postcode");

            migrationBuilder.RenameColumn(
                name: "inst_type",
                table: "UcasInstitutions",
                newName: "InstType");

            migrationBuilder.RenameColumn(
                name: "inst_name",
                table: "UcasInstitutions",
                newName: "InstName");

            migrationBuilder.RenameColumn(
                name: "inst_full",
                table: "UcasInstitutions",
                newName: "InstFull");

            migrationBuilder.RenameColumn(
                name: "inst_code",
                table: "UcasInstitutions",
                newName: "InstCode");

            migrationBuilder.RenameColumn(
                name: "inst_big",
                table: "UcasInstitutions",
                newName: "InstBig");

            migrationBuilder.RenameColumn(
                name: "contact_name",
                table: "UcasInstitutions",
                newName: "ContactName");

            migrationBuilder.RenameColumn(
                name: "addr4",
                table: "UcasInstitutions",
                newName: "Addr4");

            migrationBuilder.RenameColumn(
                name: "addr3",
                table: "UcasInstitutions",
                newName: "Addr3");

            migrationBuilder.RenameColumn(
                name: "addr2",
                table: "UcasInstitutions",
                newName: "Addr2");

            migrationBuilder.RenameColumn(
                name: "addr1",
                table: "UcasInstitutions",
                newName: "Addr1");

            migrationBuilder.RenameColumn(
                name: "accrediting_provider",
                table: "UcasInstitutions",
                newName: "AccreditingProvider");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UcasInstitutions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "year_code",
                table: "UcasCourseSubjects",
                newName: "YearCode");

            migrationBuilder.RenameColumn(
                name: "subject_code",
                table: "UcasCourseSubjects",
                newName: "SubjectCode");

            migrationBuilder.RenameColumn(
                name: "inst_code",
                table: "UcasCourseSubjects",
                newName: "InstCode");

            migrationBuilder.RenameColumn(
                name: "crse_code",
                table: "UcasCourseSubjects",
                newName: "CrseCode");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UcasCourseSubjects",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "year_code",
                table: "UcasCourseNotes",
                newName: "YearCode");

            migrationBuilder.RenameColumn(
                name: "note_type",
                table: "UcasCourseNotes",
                newName: "NoteType");

            migrationBuilder.RenameColumn(
                name: "note_no",
                table: "UcasCourseNotes",
                newName: "NoteNo");

            migrationBuilder.RenameColumn(
                name: "inst_code",
                table: "UcasCourseNotes",
                newName: "InstCode");

            migrationBuilder.RenameColumn(
                name: "crse_code",
                table: "UcasCourseNotes",
                newName: "CrseCode");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UcasCourseNotes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "studymode",
                table: "UcasCourses",
                newName: "Studymode");

            migrationBuilder.RenameColumn(
                name: "program_type",
                table: "UcasCourses",
                newName: "ProgramType");

            migrationBuilder.RenameColumn(
                name: "profpost_flag",
                table: "UcasCourses",
                newName: "ProfpostFlag");

            migrationBuilder.RenameColumn(
                name: "inst_code",
                table: "UcasCourses",
                newName: "InstCode");

            migrationBuilder.RenameColumn(
                name: "crse_title",
                table: "UcasCourses",
                newName: "CrseTitle");

            migrationBuilder.RenameColumn(
                name: "crse_open_date",
                table: "UcasCourses",
                newName: "CrseOpenDate");

            migrationBuilder.RenameColumn(
                name: "crse_code",
                table: "UcasCourses",
                newName: "CrseCode");

            migrationBuilder.RenameColumn(
                name: "campus_code",
                table: "UcasCourses",
                newName: "CampusCode");

            migrationBuilder.RenameColumn(
                name: "age",
                table: "UcasCourses",
                newName: "Age");

            migrationBuilder.RenameColumn(
                name: "accrediting_provider",
                table: "UcasCourses",
                newName: "AccreditingProvider");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UcasCourses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tel_no",
                table: "UcasCampuses",
                newName: "TelNo");

            migrationBuilder.RenameColumn(
                name: "region_code",
                table: "UcasCampuses",
                newName: "RegionCode");

            migrationBuilder.RenameColumn(
                name: "postcode",
                table: "UcasCampuses",
                newName: "Postcode");

            migrationBuilder.RenameColumn(
                name: "inst_code",
                table: "UcasCampuses",
                newName: "InstCode");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "UcasCampuses",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "campus_name",
                table: "UcasCampuses",
                newName: "CampusName");

            migrationBuilder.RenameColumn(
                name: "campus_code",
                table: "UcasCampuses",
                newName: "CampusCode");

            migrationBuilder.RenameColumn(
                name: "addr4",
                table: "UcasCampuses",
                newName: "Addr4");

            migrationBuilder.RenameColumn(
                name: "addr3",
                table: "UcasCampuses",
                newName: "Addr3");

            migrationBuilder.RenameColumn(
                name: "addr2",
                table: "UcasCampuses",
                newName: "Addr2");

            migrationBuilder.RenameColumn(
                name: "addr1",
                table: "UcasCampuses",
                newName: "Addr1");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UcasCampuses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "urn",
                table: "ProviderMapper",
                newName: "Urn");

            migrationBuilder.RenameColumn(
                name: "ucas_code",
                table: "ProviderMapper",
                newName: "UcasCode");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "ProviderMapper",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "nctl_id",
                table: "ProviderMapper",
                newName: "NctlId");

            migrationBuilder.RenameColumn(
                name: "institution_name",
                table: "ProviderMapper",
                newName: "InstitutionName");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ProviderMapper",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nctl_id",
                table: "Providers",
                newName: "NctlId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Providers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ucas_code",
                table: "Courses",
                newName: "UcasCode");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Courses",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Courses",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "provider_id",
                table: "Courses",
                newName: "ProviderId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Courses",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_course_provider_id",
                table: "Courses",
                newName: "IX_Courses_ProviderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UcasSubjects",
                table: "UcasSubjects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UcasNoteTexts",
                table: "UcasNoteTexts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UcasInstitutions",
                table: "UcasInstitutions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UcasCourseSubjects",
                table: "UcasCourseSubjects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UcasCourseNotes",
                table: "UcasCourseNotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UcasCourses",
                table: "UcasCourses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UcasCampuses",
                table: "UcasCampuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProviderMapper",
                table: "ProviderMapper",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Providers",
                table: "Providers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Providers_ProviderId",
                table: "Courses",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
