using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RemoveUcasDataTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_institution_ucas_institution_institution_code",
                table: "mc_organisation_institution");

            migrationBuilder.DropTable(
                name: "ucas_course");

            migrationBuilder.DropTable(
                name: "ucas_course_note");

            migrationBuilder.DropTable(
                name: "ucas_course_subject");

            migrationBuilder.DropTable(
                name: "ucas_note_text");

            migrationBuilder.DropTable(
                name: "course_code");

            migrationBuilder.RenameColumn(
                name: "institution_code",
                table: "mc_organisation_institution",
                newName: "inst_code");

            migrationBuilder.RenameIndex(
                name: "IX_mc_organisation_institution_org_id_institution_code",
                table: "mc_organisation_institution",
                newName: "IX_mc_organisation_institution_org_id_inst_code");

            migrationBuilder.RenameIndex(
                name: "IX_mc_organisation_institution_institution_code",
                table: "mc_organisation_institution",
                newName: "IX_mc_organisation_institution_inst_code");

            migrationBuilder.RenameTable("ucas_institution", null, "institution");
            migrationBuilder.RenameColumn("addr1", "institution", "address1");
            migrationBuilder.RenameColumn("addr2", "institution", "address2");
            migrationBuilder.RenameColumn("addr3", "institution", "address3");
            migrationBuilder.RenameColumn("addr4", "institution", "address4");
            migrationBuilder.DropColumn("inst_big","institution");
            migrationBuilder.DropColumn("inst_name","institution");
            migrationBuilder.DropColumn("accrediting_provider","institution");

            migrationBuilder.RenameTable("ucas_subject", null, "subject");
            migrationBuilder.DropColumn("title_match", "subject");
            migrationBuilder.RenameColumn("subject_description", "subject", "subject_name");

            migrationBuilder.RenameTable("ucas_campus", null, "site");
            migrationBuilder.RenameColumn("addr1", "site", "address1");
            migrationBuilder.RenameColumn("addr2", "site", "address2");
            migrationBuilder.RenameColumn("addr3", "site", "address3");
            migrationBuilder.RenameColumn("addr4", "site", "address4");
            migrationBuilder.RenameColumn("campus_code", "site", "code");
            migrationBuilder.RenameColumn("campus_name", "site", "location_name");
            migrationBuilder.DropColumn("email", "site");
            migrationBuilder.DropColumn("region_code", "site");
            migrationBuilder.DropColumn("tel_no", "site");

            migrationBuilder.CreateTable(
                name: "course",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    accrediting_inst_code = table.Column<string>(nullable: true),
                    age_range = table.Column<string>(nullable: true),
                    course_code = table.Column<string>(nullable: true),
                    inst_code = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    profpost_flag = table.Column<string>(nullable: true),
                    program_type = table.Column<string>(nullable: true),
                    qualification = table.Column<int>(nullable: false),
                    start_date = table.Column<DateTime>(nullable: true),
                    study_mode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course", x => x.id);
                    table.ForeignKey(
                        name: "FK_course_institution_accrediting_inst_code",
                        column: x => x.accrediting_inst_code,
                        principalTable: "institution",
                        principalColumn: "inst_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_institution_inst_code",
                        column: x => x.inst_code,
                        principalTable: "institution",
                        principalColumn: "inst_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_subject",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    course_id = table.Column<int>(nullable: true),
                    subject_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_subject", x => x.id);
                    table.ForeignKey(
                        name: "FK_course_subject_course_course_id",
                        column: x => x.course_id,
                        principalTable: "course",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_course_subject_subject_subject_id",
                        column: x => x.subject_id,
                        principalTable: "subject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_site",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    applications_accepted_from = table.Column<string>(nullable: true),
                    course_id = table.Column<int>(nullable: true),
                    publish = table.Column<string>(nullable: true),
                    site_id = table.Column<int>(nullable: true),
                    status = table.Column<string>(nullable: true),
                    vac_status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_site", x => x.id);
                    table.ForeignKey(
                        name: "FK_course_site_course_course_id",
                        column: x => x.course_id,
                        principalTable: "course",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_course_site_site_site_id",
                        column: x => x.site_id,
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_accrediting_inst_code",
                table: "course",
                column: "accrediting_inst_code");

            migrationBuilder.CreateIndex(
                name: "IX_course_inst_code_course_code",
                table: "course",
                columns: new[] { "inst_code", "course_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_site_course_id",
                table: "course_site",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_site_site_id",
                table: "course_site",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_subject_course_id",
                table: "course_subject",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_subject_subject_id",
                table: "course_subject",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_institution_inst_code",
                table: "institution",
                column: "inst_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_inst_code",
                table: "site",
                column: "inst_code");

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_institution_institution_inst_code",
                table: "mc_organisation_institution",
                column: "inst_code",
                principalTable: "institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_institution_institution_inst_code",
                table: "mc_organisation_institution");

            migrationBuilder.DropTable(
                name: "course_site");

            migrationBuilder.DropTable(
                name: "course_subject");

            migrationBuilder.DropTable(
                name: "course");

            migrationBuilder.RenameColumn(
                name: "inst_code",
                table: "mc_organisation_institution",
                newName: "institution_code");

            migrationBuilder.RenameIndex(
                name: "IX_mc_organisation_institution_org_id_inst_code",
                table: "mc_organisation_institution",
                newName: "IX_mc_organisation_institution_org_id_institution_code");

            migrationBuilder.RenameIndex(
                name: "IX_mc_organisation_institution_inst_code",
                table: "mc_organisation_institution",
                newName: "IX_mc_organisation_institution_institution_code");


            migrationBuilder.RenameColumn("address1", "institution", "addr1");
            migrationBuilder.RenameColumn("address2", "institution", "addr2");
            migrationBuilder.RenameColumn("address3", "institution", "addr3");
            migrationBuilder.RenameColumn("address4", "institution", "addr4");
            migrationBuilder.AddColumn<string>("inst_big","institution");
            migrationBuilder.AddColumn<string>("inst_name","institution");
            migrationBuilder.AddColumn<string>("accrediting_provider","institution");
            migrationBuilder.RenameTable("institution", null, "ucas_institution");

            migrationBuilder.AddColumn<string>("title_match", "subject");
            migrationBuilder.RenameColumn("subject_name", "subject", "subject_description");
            migrationBuilder.RenameTable("subject", null, "ucas_subject");

            migrationBuilder.RenameColumn("address1", "site", "addr1");
            migrationBuilder.RenameColumn("address2", "site", "addr2");
            migrationBuilder.RenameColumn("address3", "site", "addr3");
            migrationBuilder.RenameColumn("address4", "site", "addr4");
            migrationBuilder.RenameColumn("code", "site", "campus_code");
            migrationBuilder.RenameColumn("location_name", "site", "campus_name");
            migrationBuilder.AddColumn<string>("email", "site");
            migrationBuilder.AddColumn<string>("region_code", "site");
            migrationBuilder.AddColumn<string>("tel_no", "site");
            migrationBuilder.RenameTable("site", null, "ucas_campus");


            migrationBuilder.CreateTable(
                name: "ucas_course_note",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    crse_code = table.Column<string>(nullable: true),
                    inst_code = table.Column<string>(nullable: true),
                    note_no = table.Column<string>(nullable: true),
                    note_type = table.Column<string>(nullable: true),
                    year_code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ucas_course_note", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ucas_note_text",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    inst_code = table.Column<string>(nullable: true),
                    line_text = table.Column<string>(nullable: true),
                    note_no = table.Column<string>(nullable: true),
                    note_type = table.Column<string>(nullable: true),
                    year_code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ucas_note_text", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "course_code",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    crse_code = table.Column<string>(nullable: false),
                    inst_code = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_code", x => x.id);
                    table.UniqueConstraint("AK_course_code_inst_code_crse_code", x => new { x.inst_code, x.crse_code });
                    table.ForeignKey(
                        name: "FK_course_code_ucas_institution_inst_code",
                        column: x => x.inst_code,
                        principalTable: "ucas_institution",
                        principalColumn: "inst_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ucas_course_subject",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    crse_code = table.Column<string>(nullable: true),
                    inst_code = table.Column<string>(nullable: true),
                    subject_code = table.Column<string>(nullable: true),
                    year_code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ucas_course_subject", x => x.id);
                    table.ForeignKey(
                        name: "FK_ucas_course_subject_ucas_institution_inst_code",
                        column: x => x.inst_code,
                        principalTable: "ucas_institution",
                        principalColumn: "inst_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ucas_course_subject_ucas_subject_subject_code",
                        column: x => x.subject_code,
                        principalTable: "ucas_subject",
                        principalColumn: "subject_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ucas_course_subject_course_code_inst_code_crse_code",
                        columns: x => new { x.inst_code, x.crse_code },
                        principalTable: "course_code",
                        principalColumns: new[] { "inst_code", "crse_code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ucas_course",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    accrediting_provider = table.Column<string>(nullable: true),
                    age = table.Column<string>(nullable: true),
                    campus_code = table.Column<string>(nullable: true),
                    crse_code = table.Column<string>(nullable: true),
                    crse_open_date = table.Column<string>(nullable: true),
                    crse_title = table.Column<string>(nullable: true),
                    has_been_published = table.Column<string>(nullable: true),
                    inst_code = table.Column<string>(nullable: true),
                    profpost_flag = table.Column<string>(nullable: true),
                    program_type = table.Column<string>(nullable: true),
                    publish = table.Column<string>(nullable: true),
                    start_month = table.Column<string>(nullable: true),
                    start_year = table.Column<string>(nullable: true),
                    status = table.Column<string>(nullable: true),
                    studymode = table.Column<string>(nullable: true),
                    vac_status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ucas_course", x => x.id);
                    table.ForeignKey(
                        name: "FK_ucas_course_ucas_institution_accrediting_provider",
                        column: x => x.accrediting_provider,
                        principalTable: "ucas_institution",
                        principalColumn: "inst_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ucas_course_ucas_institution_inst_code",
                        column: x => x.inst_code,
                        principalTable: "ucas_institution",
                        principalColumn: "inst_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ucas_course_ucas_campus_inst_code_campus_code",
                        columns: x => new { x.inst_code, x.campus_code },
                        principalTable: "ucas_campus",
                        principalColumns: new[] { "inst_code", "campus_code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ucas_course_course_code_inst_code_crse_code",
                        columns: x => new { x.inst_code, x.crse_code },
                        principalTable: "course_code",
                        principalColumns: new[] { "inst_code", "crse_code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ucas_course_accrediting_provider",
                table: "ucas_course",
                column: "accrediting_provider");

            migrationBuilder.CreateIndex(
                name: "IX_ucas_course_inst_code_campus_code",
                table: "ucas_course",
                columns: new[] { "inst_code", "campus_code" });

            migrationBuilder.CreateIndex(
                name: "IX_ucas_course_inst_code_crse_code_campus_code",
                table: "ucas_course",
                columns: new[] { "inst_code", "crse_code", "campus_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ucas_course_subject_subject_code",
                table: "ucas_course_subject",
                column: "subject_code");

            migrationBuilder.CreateIndex(
                name: "IX_ucas_course_subject_inst_code_crse_code",
                table: "ucas_course_subject",
                columns: new[] { "inst_code", "crse_code" });

            migrationBuilder.CreateIndex(
                name: "IX_ucas_institution_inst_code",
                table: "ucas_institution",
                column: "inst_code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_institution_ucas_institution_institution_code",
                table: "mc_organisation_institution",
                column: "institution_code",
                principalTable: "ucas_institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
