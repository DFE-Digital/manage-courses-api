using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class Fks2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mc_user_email",
                table: "mc_user");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "mc_user",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_mc_user_email",
                table: "mc_user",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_user_email",
                table: "mc_organisation_user",
                column: "email");

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_user_mc_user_email",
                table: "mc_organisation_user",
                column: "email",
                principalTable: "mc_user",
                principalColumn: "email",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_user_email",
                table: "mc_organisation_user");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_mc_user_email",
                table: "mc_user");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_user_email",
                table: "mc_organisation_user");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "mc_user",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_mc_user_email",
                table: "mc_user",
                column: "email",
                unique: true);
        }
    }
}
