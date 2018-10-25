using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RedefineSubjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_subject_subject_code",
                table: "subject");

            migrationBuilder.DropColumn(
                name: "subject_code",
                table: "subject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "subject_code",
                table: "subject",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_subject_subject_code",
                table: "subject",
                column: "subject_code",
                unique: true);
        }
    }
}
