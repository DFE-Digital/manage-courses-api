using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class Fks3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "nctl_id",
                table: "mc_organisation",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_mc_organisation_nctl_id",
                table: "mc_organisation",
                column: "nctl_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_user_nctl_id",
                table: "mc_organisation_user",
                column: "nctl_id");

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_user_mc_organisation_nctl_id",
                table: "mc_organisation_user",
                column: "nctl_id",
                principalTable: "mc_organisation",
                principalColumn: "nctl_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_organisation_nctl_id",
                table: "mc_organisation_user");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_user_nctl_id",
                table: "mc_organisation_user");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_mc_organisation_nctl_id",
                table: "mc_organisation");

            migrationBuilder.AlterColumn<string>(
                name: "nctl_id",
                table: "mc_organisation",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
