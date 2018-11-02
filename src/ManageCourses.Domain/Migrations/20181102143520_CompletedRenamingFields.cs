using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class CompletedRenamingFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_provider_accrediting_institution_id",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "FK_course_provider_institution_id",
                table: "course");

            migrationBuilder.RenameColumn(
                name: "inst_code",
                table: "pgde_course",
                newName: "provider_code");

            migrationBuilder.RenameColumn(
                name: "inst_code",
                table: "course_enrichment",
                newName: "provider_code");

            migrationBuilder.RenameColumn(
                name: "institution_id",
                table: "course",
                newName: "provider_id");

            migrationBuilder.RenameColumn(
                name: "accrediting_institution_id",
                table: "course",
                newName: "accrediting_provider_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_institution_id_course_code",
                table: "course",
                newName: "IX_course_provider_id_course_code");

            migrationBuilder.RenameIndex(
                name: "IX_course_accrediting_institution_id",
                table: "course",
                newName: "IX_course_accrediting_provider_id");

            migrationBuilder.AddForeignKey(
                name: "FK_course_provider_accrediting_provider_id",
                table: "course",
                column: "accrediting_provider_id",
                principalTable: "provider",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_provider_provider_id",
                table: "course",
                column: "provider_id",
                principalTable: "provider",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_provider_accrediting_provider_id",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "FK_course_provider_provider_id",
                table: "course");

            migrationBuilder.RenameColumn(
                name: "provider_code",
                table: "pgde_course",
                newName: "inst_code");

            migrationBuilder.RenameColumn(
                name: "provider_code",
                table: "course_enrichment",
                newName: "inst_code");

            migrationBuilder.RenameColumn(
                name: "provider_id",
                table: "course",
                newName: "institution_id");

            migrationBuilder.RenameColumn(
                name: "accrediting_provider_id",
                table: "course",
                newName: "accrediting_institution_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_provider_id_course_code",
                table: "course",
                newName: "IX_course_institution_id_course_code");

            migrationBuilder.RenameIndex(
                name: "IX_course_accrediting_provider_id",
                table: "course",
                newName: "IX_course_accrediting_institution_id");

            migrationBuilder.AddForeignKey(
                name: "FK_course_provider_accrediting_institution_id",
                table: "course",
                column: "accrediting_institution_id",
                principalTable: "provider",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_provider_institution_id",
                table: "course",
                column: "institution_id",
                principalTable: "provider",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
