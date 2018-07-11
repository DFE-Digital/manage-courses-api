using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class MakeRequesterAndLogUserNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_access_request_mc_user_requester_id",
                table: "access_request");

            migrationBuilder.DropForeignKey(
                name: "FK_user_log_mc_user_user_id",
                table: "user_log");

            migrationBuilder.AddForeignKey(
                name: "FK_access_request_mc_user_requester_id",
                table: "access_request",
                column: "requester_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_user_log_mc_user_user_id",
                table: "user_log",
                column: "user_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_access_request_mc_user_requester_id",
                table: "access_request");

            migrationBuilder.DropForeignKey(
                name: "FK_user_log_mc_user_user_id",
                table: "user_log");

            migrationBuilder.AddForeignKey(
                name: "FK_access_request_mc_user_requester_id",
                table: "access_request",
                column: "requester_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_log_mc_user_user_id",
                table: "user_log",
                column: "user_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
