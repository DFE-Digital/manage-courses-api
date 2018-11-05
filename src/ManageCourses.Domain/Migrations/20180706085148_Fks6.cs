using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class Fks6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "subject_code",
                table: "ucas_subject",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "inst_code",
                table: "ucas_campus",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "campus_code",
                table: "ucas_campus",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ucas_subject_subject_code",
                table: "ucas_subject",
                column: "subject_code");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ucas_campus_inst_code_campus_code",
                table: "ucas_campus",
                columns: new[] { "inst_code", "campus_code" });

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

            migrationBuilder.CreateIndex(
                name: "IX_ucas_course_subject_subject_code",
                table: "ucas_course_subject",
                column: "subject_code");

            migrationBuilder.CreateIndex(
                name: "IX_ucas_course_subject_inst_code_crse_code",
                table: "ucas_course_subject",
                columns: new[] { "inst_code", "crse_code" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_ucas_campus_ucas_institution_inst_code",
                table: "ucas_campus",
                column: "inst_code",
                principalTable: "ucas_institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ucas_course_ucas_institution_accrediting_provider",
                table: "ucas_course",
                column: "accrediting_provider",
                principalTable: "ucas_institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ucas_course_ucas_institution_inst_code",
                table: "ucas_course",
                column: "inst_code",
                principalTable: "ucas_institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ucas_course_ucas_campus_inst_code_campus_code",
                table: "ucas_course",
                columns: new[] { "inst_code", "campus_code" },
                principalTable: "ucas_campus",
                principalColumns: new[] { "inst_code", "campus_code" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ucas_course_course_code_inst_code_crse_code",
                table: "ucas_course",
                columns: new[] { "inst_code", "crse_code" },
                principalTable: "course_code",
                principalColumns: new[] { "inst_code", "crse_code" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ucas_course_subject_ucas_institution_inst_code",
                table: "ucas_course_subject",
                column: "inst_code",
                principalTable: "ucas_institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ucas_course_subject_ucas_subject_subject_code",
                table: "ucas_course_subject",
                column: "subject_code",
                principalTable: "ucas_subject",
                principalColumn: "subject_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ucas_course_subject_course_code_inst_code_crse_code",
                table: "ucas_course_subject",
                columns: new[] { "inst_code", "crse_code" },
                principalTable: "course_code",
                principalColumns: new[] { "inst_code", "crse_code" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ucas_campus_ucas_institution_inst_code",
                table: "ucas_campus");

            migrationBuilder.DropForeignKey(
                name: "FK_ucas_course_ucas_institution_accrediting_provider",
                table: "ucas_course");

            migrationBuilder.DropForeignKey(
                name: "FK_ucas_course_ucas_institution_inst_code",
                table: "ucas_course");

            migrationBuilder.DropForeignKey(
                name: "FK_ucas_course_ucas_campus_inst_code_campus_code",
                table: "ucas_course");

            migrationBuilder.DropForeignKey(
                name: "FK_ucas_course_course_code_inst_code_crse_code",
                table: "ucas_course");

            migrationBuilder.DropForeignKey(
                name: "FK_ucas_course_subject_ucas_institution_inst_code",
                table: "ucas_course_subject");

            migrationBuilder.DropForeignKey(
                name: "FK_ucas_course_subject_ucas_subject_subject_code",
                table: "ucas_course_subject");

            migrationBuilder.DropForeignKey(
                name: "FK_ucas_course_subject_course_code_inst_code_crse_code",
                table: "ucas_course_subject");

            migrationBuilder.DropTable(
                name: "course_code");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ucas_subject_subject_code",
                table: "ucas_subject");

            migrationBuilder.DropIndex(
                name: "IX_ucas_course_subject_subject_code",
                table: "ucas_course_subject");

            migrationBuilder.DropIndex(
                name: "IX_ucas_course_subject_inst_code_crse_code",
                table: "ucas_course_subject");

            migrationBuilder.DropIndex(
                name: "IX_ucas_course_accrediting_provider",
                table: "ucas_course");

            migrationBuilder.DropIndex(
                name: "IX_ucas_course_inst_code_campus_code",
                table: "ucas_course");

            migrationBuilder.DropIndex(
                name: "IX_ucas_course_inst_code_crse_code_campus_code",
                table: "ucas_course");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ucas_campus_inst_code_campus_code",
                table: "ucas_campus");

            migrationBuilder.AlterColumn<string>(
                name: "subject_code",
                table: "ucas_subject",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "inst_code",
                table: "ucas_campus",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "campus_code",
                table: "ucas_campus",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
