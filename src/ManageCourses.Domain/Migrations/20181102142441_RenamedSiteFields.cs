using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedSiteFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_site_provider_institution_id",
                table: "site");

            migrationBuilder.RenameColumn(
                name: "institution_id",
                table: "site",
                newName: "provider_id");

            migrationBuilder.RenameIndex(
                name: "IX_site_institution_id_code",
                table: "site",
                newName: "IX_site_provider_id_code");

            migrationBuilder.AddForeignKey(
                name: "FK_site_provider_provider_id",
                table: "site",
                column: "provider_id",
                principalTable: "provider",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_site_provider_provider_id",
                table: "site");

            migrationBuilder.RenameColumn(
                name: "provider_id",
                table: "site",
                newName: "institution_id");

            migrationBuilder.RenameIndex(
                name: "IX_site_provider_id_code",
                table: "site",
                newName: "IX_site_institution_id_code");

            migrationBuilder.AddForeignKey(
                name: "FK_site_provider_institution_id",
                table: "site",
                column: "institution_id",
                principalTable: "provider",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
