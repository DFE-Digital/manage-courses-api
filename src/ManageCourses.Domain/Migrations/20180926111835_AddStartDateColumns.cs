using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AddStartDateColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "start_month",
                table: "ucas_course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "start_year",
                table: "ucas_course",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "start_month",
                table: "ucas_course");

            migrationBuilder.DropColumn(
                name: "start_year",
                table: "ucas_course");
        }
    }
}
