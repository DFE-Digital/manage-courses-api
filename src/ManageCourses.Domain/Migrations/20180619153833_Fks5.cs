using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class Fks5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "inst_code",
                table: "ucas_institution",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ucas_institution_inst_code",
                table: "ucas_institution",
                column: "inst_code");

            migrationBuilder.CreateIndex(
                name: "IX_ucas_institution_inst_code",
                table: "ucas_institution",
                column: "inst_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_institution_institution_code",
                table: "mc_organisation_institution",
                column: "institution_code");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_institution_nctl_id_institution_code",
                table: "mc_organisation_institution",
                columns: new[] { "nctl_id", "institution_code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_institution_ucas_institution_institution_code",
                table: "mc_organisation_institution",
                column: "institution_code",
                principalTable: "ucas_institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_institution_mc_organisation_nctl_id",
                table: "mc_organisation_institution",
                column: "nctl_id",
                principalTable: "mc_organisation",
                principalColumn: "nctl_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_institution_ucas_institution_institution_code",
                table: "mc_organisation_institution");

            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_institution_mc_organisation_nctl_id",
                table: "mc_organisation_institution");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ucas_institution_inst_code",
                table: "ucas_institution");

            migrationBuilder.DropIndex(
                name: "IX_ucas_institution_inst_code",
                table: "ucas_institution");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_institution_institution_code",
                table: "mc_organisation_institution");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_institution_nctl_id_institution_code",
                table: "mc_organisation_institution");

            migrationBuilder.AlterColumn<string>(
                name: "inst_code",
                table: "ucas_institution",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
