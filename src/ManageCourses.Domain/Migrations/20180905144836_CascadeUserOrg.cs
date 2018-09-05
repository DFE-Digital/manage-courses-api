using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class CascadeUserOrg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_user_email",
                table: "mc_organisation_user");

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_user_mc_user_email",
                table: "mc_organisation_user",
                column: "email",
                principalTable: "mc_user",
                principalColumn: "email",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_user_email",
                table: "mc_organisation_user");

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_user_mc_user_email",
                table: "mc_organisation_user",
                column: "email",
                principalTable: "mc_user",
                principalColumn: "email",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
