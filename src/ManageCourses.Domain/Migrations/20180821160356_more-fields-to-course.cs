using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class morefieldstocourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "has_been_published",
                table: "ucas_course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "publish",
                table: "ucas_course",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "has_been_published",
                table: "ucas_course");

            migrationBuilder.DropColumn(
                name: "publish",
                table: "ucas_course");
        }
    }
}
