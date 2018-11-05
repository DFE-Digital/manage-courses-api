using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class CourseEnrichment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_enrichment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created_by_user_id = table.Column<int>(nullable: true),
                    created_timestamp_utc = table.Column<DateTime>(nullable: false),
                    inst_code = table.Column<string>(nullable: false),
                    json_data = table.Column<string>(type: "jsonb", nullable: true),
                    last_published_timestamp_utc = table.Column<DateTime>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    ucas_course_code = table.Column<string>(nullable: false),
                    updated_by_user_id = table.Column<int>(nullable: true),
                    updated_timestamp_utc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_enrichment", x => x.id);
                    table.ForeignKey(
                        name: "FK_course_enrichment_mc_user_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "mc_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_enrichment_mc_user_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalTable: "mc_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_enrichment_created_by_user_id",
                table: "course_enrichment",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_enrichment_updated_by_user_id",
                table: "course_enrichment",
                column: "updated_by_user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_enrichment");
        }
    }
}
