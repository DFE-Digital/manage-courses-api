using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class addedstatustocourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "ucas_course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "vac_status",
                table: "ucas_course",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "ucas_course");

            migrationBuilder.DropColumn(
                name: "vac_status",
                table: "ucas_course");
        }
    }
}
