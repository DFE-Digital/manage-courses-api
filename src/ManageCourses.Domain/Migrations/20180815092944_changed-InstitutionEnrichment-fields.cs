using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class changedInstitutionEnrichmentfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "saved_by_user_id",
                table: "institution_enrichment");

            migrationBuilder.RenameColumn(
                name: "update_timestamp_utc",
                table: "institution_enrichment",
                newName: "updated_timestamp_utc");

            migrationBuilder.AlterColumn<int>(
                name: "updated_by_user_id",
                table: "institution_enrichment",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "created_by_user_id",
                table: "institution_enrichment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_institution_enrichment_created_by_user_id",
                table: "institution_enrichment",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_institution_enrichment_updated_by_user_id",
                table: "institution_enrichment",
                column: "updated_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_institution_enrichment_mc_user_created_by_user_id",
                table: "institution_enrichment",
                column: "created_by_user_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_institution_enrichment_mc_user_updated_by_user_id",
                table: "institution_enrichment",
                column: "updated_by_user_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_institution_enrichment_mc_user_created_by_user_id",
                table: "institution_enrichment");

            migrationBuilder.DropForeignKey(
                name: "FK_institution_enrichment_mc_user_updated_by_user_id",
                table: "institution_enrichment");

            migrationBuilder.DropIndex(
                name: "IX_institution_enrichment_created_by_user_id",
                table: "institution_enrichment");

            migrationBuilder.DropIndex(
                name: "IX_institution_enrichment_updated_by_user_id",
                table: "institution_enrichment");

            migrationBuilder.DropColumn(
                name: "created_by_user_id",
                table: "institution_enrichment");

            migrationBuilder.RenameColumn(
                name: "updated_timestamp_utc",
                table: "institution_enrichment",
                newName: "update_timestamp_utc");

            migrationBuilder.AlterColumn<int>(
                name: "updated_by_user_id",
                table: "institution_enrichment",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "saved_by_user_id",
                table: "institution_enrichment",
                nullable: false,
                defaultValue: 0);
        }
    }
}
