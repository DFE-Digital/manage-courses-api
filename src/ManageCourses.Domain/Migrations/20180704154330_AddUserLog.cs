using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AddUserLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_log_sign_in_user_id",
                table: "user_log");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_user_log_sign_in_user_id",
                table: "user_log",
                column: "sign_in_user_id",
                unique: true);
        }
    }
}
