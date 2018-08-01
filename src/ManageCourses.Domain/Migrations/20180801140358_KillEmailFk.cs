using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class KillEmailFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_user_email",
                table: "mc_organisation_user");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_mc_user_email",
                table: "mc_user");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "mc_user",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "mc_user_id",
                table: "mc_organisation_user",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_user_mc_user_id",
                table: "mc_organisation_user",
                column: "mc_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_user_mc_user_mc_user_id",
                table: "mc_organisation_user",
                column: "mc_user_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_user_mc_user_id",
                table: "mc_organisation_user");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_user_mc_user_id",
                table: "mc_organisation_user");

            migrationBuilder.DropColumn(
                name: "mc_user_id",
                table: "mc_organisation_user");

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
