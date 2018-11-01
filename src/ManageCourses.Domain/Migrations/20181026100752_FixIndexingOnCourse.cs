using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class FixIndexingOnCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_course_institution_id_id",
                table: "course");

            migrationBuilder.CreateIndex(
                name: "IX_course_institution_id_course_code",
                table: "course",
                columns: new[] { "institution_id", "course_code" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_course_institution_id_course_code",
                table: "course");

            migrationBuilder.CreateIndex(
                name: "IX_course_institution_id_id",
                table: "course",
                columns: new[] { "institution_id", "id" },
                unique: true);
        }
    }
}
