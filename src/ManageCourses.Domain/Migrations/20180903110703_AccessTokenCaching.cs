using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AccessTokenCaching : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mc_session",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    access_token = table.Column<string>(nullable: true),
                    created_utc = table.Column<DateTime>(nullable: false),
                    mc_user_id = table.Column<int>(nullable: true),
                    subject = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mc_session", x => x.id);
                    table.ForeignKey(
                        name: "FK_mc_session_mc_user_mc_user_id",
                        column: x => x.mc_user_id,
                        principalTable: "mc_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mc_session_mc_user_id",
                table: "mc_session",
                column: "mc_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_session_access_token_created_utc",
                table: "mc_session",
                columns: new[] { "access_token", "created_utc" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mc_session");
        }
    }
}
