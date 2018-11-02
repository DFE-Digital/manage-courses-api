using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedOrganisationInstitution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organisation_institution");

            migrationBuilder.CreateTable(
                name: "organisation_provider",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    institution_id = table.Column<int>(nullable: true),
                    organisation_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organisation_provider", x => x.id);
                    table.ForeignKey(
                        name: "FK_organisation_provider_institution_institution_id",
                        column: x => x.institution_id,
                        principalTable: "institution",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_organisation_provider_organisation_organisation_id",
                        column: x => x.organisation_id,
                        principalTable: "organisation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_organisation_provider_institution_id",
                table: "organisation_provider",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "IX_organisation_provider_organisation_id",
                table: "organisation_provider",
                column: "organisation_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organisation_provider");

            migrationBuilder.CreateTable(
                name: "organisation_institution",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    institution_id = table.Column<int>(nullable: true),
                    organisation_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organisation_institution", x => x.id);
                    table.ForeignKey(
                        name: "FK_organisation_institution_institution_institution_id",
                        column: x => x.institution_id,
                        principalTable: "institution",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_organisation_institution_organisation_organisation_id",
                        column: x => x.organisation_id,
                        principalTable: "organisation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_organisation_institution_institution_id",
                table: "organisation_institution",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "IX_organisation_institution_organisation_id",
                table: "organisation_institution",
                column: "organisation_id");
        }
    }
}
