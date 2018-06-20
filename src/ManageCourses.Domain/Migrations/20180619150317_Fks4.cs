using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class Fks4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_user_email",
                table: "mc_organisation_user");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_user_email_nctl_id",
                table: "mc_organisation_user",
                columns: new[] { "email", "nctl_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_user_email_nctl_id",
                table: "mc_organisation_user");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_user_email",
                table: "mc_organisation_user",
                column: "email");
        }
    }
}
