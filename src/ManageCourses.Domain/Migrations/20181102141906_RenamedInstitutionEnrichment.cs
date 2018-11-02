using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedInstitutionEnrichment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "institution_enrichment");

            migrationBuilder.CreateTable(
                name: "provider_enrichment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created_by_user_id = table.Column<int>(nullable: true),
                    created_timestamp_utc = table.Column<DateTime>(nullable: false),
                    json_data = table.Column<string>(type: "jsonb", nullable: true),
                    last_published_timestamp_utc = table.Column<DateTime>(nullable: true),
                    provider_code = table.Column<string>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    updated_by_user_id = table.Column<int>(nullable: true),
                    updated_timestamp_utc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provider_enrichment", x => x.id);
                    table.ForeignKey(
                        name: "FK_provider_enrichment_user_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_provider_enrichment_user_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_provider_enrichment_created_by_user_id",
                table: "provider_enrichment",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_provider_enrichment_provider_code",
                table: "provider_enrichment",
                column: "provider_code");

            migrationBuilder.CreateIndex(
                name: "IX_provider_enrichment_updated_by_user_id",
                table: "provider_enrichment",
                column: "updated_by_user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "provider_enrichment");

            migrationBuilder.CreateTable(
                name: "institution_enrichment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created_by_user_id = table.Column<int>(nullable: true),
                    created_timestamp_utc = table.Column<DateTime>(nullable: false),
                    json_data = table.Column<string>(type: "jsonb", nullable: true),
                    last_published_timestamp_utc = table.Column<DateTime>(nullable: true),
                    provider_code = table.Column<string>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    updated_by_user_id = table.Column<int>(nullable: true),
                    updated_timestamp_utc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_institution_enrichment", x => x.id);
                    table.ForeignKey(
                        name: "FK_institution_enrichment_user_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_institution_enrichment_user_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_institution_enrichment_created_by_user_id",
                table: "institution_enrichment",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_institution_enrichment_provider_code",
                table: "institution_enrichment",
                column: "provider_code");

            migrationBuilder.CreateIndex(
                name: "IX_institution_enrichment_updated_by_user_id",
                table: "institution_enrichment",
                column: "updated_by_user_id");
        }
    }
}
