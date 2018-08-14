using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AddedInstitutionEnrichmentFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_timestamp_utc",
                table: "institution_enrichment",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "update_timestamp_utc",
                table: "institution_enrichment",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_institution_enrichment_inst_code",
                table: "institution_enrichment",
                column: "inst_code");

            migrationBuilder.AddForeignKey(
                name: "FK_institution_enrichment_ucas_institution_inst_code",
                table: "institution_enrichment",
                column: "inst_code",
                principalTable: "ucas_institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_institution_enrichment_ucas_institution_inst_code",
                table: "institution_enrichment");

            migrationBuilder.DropIndex(
                name: "IX_institution_enrichment_inst_code",
                table: "institution_enrichment");

            migrationBuilder.DropColumn(
                name: "created_timestamp_utc",
                table: "institution_enrichment");

            migrationBuilder.DropColumn(
                name: "update_timestamp_utc",
                table: "institution_enrichment");
        }
    }
}
