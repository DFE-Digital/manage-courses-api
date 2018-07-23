using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class UserLogExtendMcUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "first_login_date_utc",
                table: "mc_user",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_login_date_utc",
                table: "mc_user",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sign_in_user_id",
                table: "mc_user",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "welcome_email_date_utc",
                table: "mc_user",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "first_login_date_utc",
                table: "mc_user");

            migrationBuilder.DropColumn(
                name: "last_login_date_utc",
                table: "mc_user");

            migrationBuilder.DropColumn(
                name: "sign_in_user_id",
                table: "mc_user");

            migrationBuilder.DropColumn(
                name: "welcome_email_date_utc",
                table: "mc_user");
        }
    }
}
