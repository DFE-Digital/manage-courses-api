using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class UserLogByeBye : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_log");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_log",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    first_login_date_utc = table.Column<DateTime>(nullable: false),
                    last_login_date_utc = table.Column<DateTime>(nullable: false),
                    sign_in_user_id = table.Column<string>(nullable: true),
                    user_email = table.Column<string>(nullable: true),
                    user_id = table.Column<int>(nullable: true),
                    welcome_email_date_utc = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_log_mc_user_user_id",
                        column: x => x.user_id,
                        principalTable: "mc_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_log_user_id",
                table: "user_log",
                column: "user_id");
        }
    }
}
