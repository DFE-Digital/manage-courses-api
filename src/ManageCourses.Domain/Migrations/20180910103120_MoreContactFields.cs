using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class MoreContactFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "ucas_institution",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "telephone",
                table: "ucas_institution",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "ucas_institution");

            migrationBuilder.DropColumn(
                name: "telephone",
                table: "ucas_institution");
        }
    }
}
