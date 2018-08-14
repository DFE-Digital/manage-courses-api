using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AddedInstitutionEnrichmentrequiredinstCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_institution_enrichment_ucas_institution_inst_code",
                table: "institution_enrichment");

            migrationBuilder.AlterColumn<string>(
                name: "inst_code",
                table: "institution_enrichment",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_institution_enrichment_ucas_institution_inst_code",
                table: "institution_enrichment",
                column: "inst_code",
                principalTable: "ucas_institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_institution_enrichment_ucas_institution_inst_code",
                table: "institution_enrichment");

            migrationBuilder.AlterColumn<string>(
                name: "inst_code",
                table: "institution_enrichment",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_institution_enrichment_ucas_institution_inst_code",
                table: "institution_enrichment",
                column: "inst_code",
                principalTable: "ucas_institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
