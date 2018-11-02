using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "inst_code",
                table: "institution",
                newName: "provider_code");

            migrationBuilder.RenameIndex(
                name: "IX_institution_inst_code",
                table: "institution",
                newName: "IX_institution_provider_code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "provider_code",
                table: "institution",
                newName: "inst_code");

            migrationBuilder.RenameIndex(
                name: "IX_institution_provider_code",
                table: "institution",
                newName: "IX_institution_inst_code");
        }
    }
}
