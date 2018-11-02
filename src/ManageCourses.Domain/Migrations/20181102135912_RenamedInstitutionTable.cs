using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedInstitutionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_institution_accrediting_institution_id",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "FK_course_institution_institution_id",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "FK_organisation_provider_institution_provider_id",
                table: "organisation_provider");

            migrationBuilder.DropForeignKey(
                name: "FK_site_institution_institution_id",
                table: "site");

            migrationBuilder.DropTable(
                name: "institution");

            migrationBuilder.CreateTable(
                name: "provider",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    address1 = table.Column<string>(nullable: true),
                    address2 = table.Column<string>(nullable: true),
                    address3 = table.Column<string>(nullable: true),
                    address4 = table.Column<string>(nullable: true),
                    contact_name = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    inst_type = table.Column<string>(nullable: true),
                    postcode = table.Column<string>(nullable: true),
                    provider_code = table.Column<string>(nullable: true),
                    provider_name = table.Column<string>(nullable: true),
                    scheme_member = table.Column<string>(nullable: true),
                    scitt = table.Column<string>(nullable: true),
                    telephone = table.Column<string>(nullable: true),
                    url = table.Column<string>(nullable: true),
                    year_code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provider", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_provider_provider_code",
                table: "provider",
                column: "provider_code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_course_provider_accrediting_institution_id",
                table: "course",
                column: "accrediting_institution_id",
                principalTable: "provider",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_provider_institution_id",
                table: "course",
                column: "institution_id",
                principalTable: "provider",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_organisation_provider_provider_provider_id",
                table: "organisation_provider",
                column: "provider_id",
                principalTable: "provider",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_site_provider_institution_id",
                table: "site",
                column: "institution_id",
                principalTable: "provider",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_provider_accrediting_institution_id",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "FK_course_provider_institution_id",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "FK_organisation_provider_provider_provider_id",
                table: "organisation_provider");

            migrationBuilder.DropForeignKey(
                name: "FK_site_provider_institution_id",
                table: "site");

            migrationBuilder.DropTable(
                name: "provider");

            migrationBuilder.CreateTable(
                name: "institution",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    address1 = table.Column<string>(nullable: true),
                    address2 = table.Column<string>(nullable: true),
                    address3 = table.Column<string>(nullable: true),
                    address4 = table.Column<string>(nullable: true),
                    contact_name = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    inst_type = table.Column<string>(nullable: true),
                    postcode = table.Column<string>(nullable: true),
                    provider_code = table.Column<string>(nullable: true),
                    provider_name = table.Column<string>(nullable: true),
                    scheme_member = table.Column<string>(nullable: true),
                    scitt = table.Column<string>(nullable: true),
                    telephone = table.Column<string>(nullable: true),
                    url = table.Column<string>(nullable: true),
                    year_code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_institution", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_institution_provider_code",
                table: "institution",
                column: "provider_code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_course_institution_accrediting_institution_id",
                table: "course",
                column: "accrediting_institution_id",
                principalTable: "institution",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_institution_institution_id",
                table: "course",
                column: "institution_id",
                principalTable: "institution",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_organisation_provider_institution_provider_id",
                table: "organisation_provider",
                column: "provider_id",
                principalTable: "institution",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_site_institution_institution_id",
                table: "site",
                column: "institution_id",
                principalTable: "institution",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
