using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RemoveMcPrefix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_access_request_mc_user_requester_id",
                table: "access_request");

            migrationBuilder.DropForeignKey(
                name: "FK_course_enrichment_mc_user_created_by_user_id",
                table: "course_enrichment");

            migrationBuilder.DropForeignKey(
                name: "FK_course_enrichment_mc_user_updated_by_user_id",
                table: "course_enrichment");

            migrationBuilder.DropForeignKey(
                name: "FK_institution_enrichment_mc_user_created_by_user_id",
                table: "institution_enrichment");

            migrationBuilder.DropForeignKey(
                name: "FK_institution_enrichment_mc_user_updated_by_user_id",
                table: "institution_enrichment");

            migrationBuilder.DropForeignKey(
                name: "FK_nctl_organisation_mc_organisation_mc_organisation_id",
                table: "nctl_organisation");

            migrationBuilder.RenameTable("mc_organisation_institution", null, "organisation_institution");
            migrationBuilder.RenameColumn("mc_organisation_id", "organisation_institution", "organisation_id");

            migrationBuilder.RenameTable("mc_organisation_user", null, "organisation_user");
            migrationBuilder.RenameColumn("mc_organisation_id", "organisation_user", "organisation_id");
            migrationBuilder.RenameColumn("mc_user_id", "organisation_user", "user_id");

            migrationBuilder.RenameTable("mc_session", null, "session");
            migrationBuilder.RenameColumn("mc_user_id", "session", "user_id");

            migrationBuilder.RenameTable("mc_organisation", null, "organisation");
            migrationBuilder.RenameTable("mc_user", null, "user");

            migrationBuilder.DropPrimaryKey("PK_mc_organisation_institution", "organisation_institution");
            migrationBuilder.AddPrimaryKey("PK_organisation_institution", "organisation_institution", "id");
            migrationBuilder.AddForeignKey(
                name: "FK_organisation_institution_institution_institution_id",
                table: "organisation_institution",
                column: "institution_id",
                principalTable: "institution",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_organisation_institution_organisation_organisation_id",
                table: "organisation_institution",
                column: "organisation_id",
                principalTable: "organisation",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);


            migrationBuilder.DropPrimaryKey("PK_mc_organisation_user", "organisation_user");
            migrationBuilder.AddPrimaryKey("PK_organisation_user", "organisation_user", "id");
            migrationBuilder.AddForeignKey(
                name: "FK_organisation_user_organisation_organisation_id",
                table: "organisation_user",
                column: "organisation_id",
                principalTable: "organisation",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_organisation_user_user_user_id",
                table: "organisation_user",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropPrimaryKey("PK_mc_session", "session");
            migrationBuilder.AddPrimaryKey("PK_session", "session", "id");
            migrationBuilder.AddForeignKey(
                name: "FK_session_user_user_id",
                table: "session",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
                name: "IX_organisation_org_id",
                table: "organisation",
                column: "org_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_organisation_institution_institution_id",
                table: "organisation_institution",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "IX_organisation_institution_organisation_id",
                table: "organisation_institution",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_organisation_user_organisation_id",
                table: "organisation_user",
                column: "organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_organisation_user_user_id",
                table: "organisation_user",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_user_id",
                table: "session",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_access_token_created_utc",
                table: "session",
                columns: new[] { "access_token", "created_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_access_request_user_requester_id",
                table: "access_request",
                column: "requester_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_course_enrichment_user_created_by_user_id",
                table: "course_enrichment",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_enrichment_user_updated_by_user_id",
                table: "course_enrichment",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_institution_enrichment_user_created_by_user_id",
                table: "institution_enrichment",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_institution_enrichment_user_updated_by_user_id",
                table: "institution_enrichment",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_nctl_organisation_organisation_mc_organisation_id",
                table: "nctl_organisation",
                column: "mc_organisation_id",
                principalTable: "organisation",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_access_request_user_requester_id",
                table: "access_request");

            migrationBuilder.DropForeignKey(
                name: "FK_course_enrichment_user_created_by_user_id",
                table: "course_enrichment");

            migrationBuilder.DropForeignKey(
                name: "FK_course_enrichment_user_updated_by_user_id",
                table: "course_enrichment");

            migrationBuilder.DropForeignKey(
                name: "FK_institution_enrichment_user_created_by_user_id",
                table: "institution_enrichment");

            migrationBuilder.DropForeignKey(
                name: "FK_institution_enrichment_user_updated_by_user_id",
                table: "institution_enrichment");

            migrationBuilder.DropForeignKey(
                name: "FK_nctl_organisation_organisation_mc_organisation_id",
                table: "nctl_organisation");

            migrationBuilder.DropTable(
                name: "organisation_institution");

            migrationBuilder.DropTable(
                name: "organisation_user");

            migrationBuilder.DropTable(
                name: "session");

            migrationBuilder.DropTable(
                name: "organisation");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.CreateTable(
                name: "mc_organisation",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(nullable: true),
                    org_id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mc_organisation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mc_user",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    accept_terms_date_utc = table.Column<DateTime>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    first_login_date_utc = table.Column<DateTime>(nullable: true),
                    first_name = table.Column<string>(nullable: true),
                    invite_date_utc = table.Column<DateTime>(nullable: true),
                    last_login_date_utc = table.Column<DateTime>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    sign_in_user_id = table.Column<string>(nullable: true),
                    welcome_email_date_utc = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mc_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mc_organisation_institution",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    institution_id = table.Column<int>(nullable: true),
                    mc_organisation_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mc_organisation_institution", x => x.id);
                    table.ForeignKey(
                        name: "FK_mc_organisation_institution_institution_institution_id",
                        column: x => x.institution_id,
                        principalTable: "institution",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mc_organisation_institution_mc_organisation_mc_organisation_id",
                        column: x => x.mc_organisation_id,
                        principalTable: "mc_organisation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mc_organisation_user",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    mc_organisation_id = table.Column<int>(nullable: true),
                    mc_user_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mc_organisation_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_mc_organisation_user_mc_organisation_mc_organisation_id",
                        column: x => x.mc_organisation_id,
                        principalTable: "mc_organisation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mc_organisation_user_mc_user_mc_user_id",
                        column: x => x.mc_user_id,
                        principalTable: "mc_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mc_session",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    access_token = table.Column<string>(nullable: true),
                    created_utc = table.Column<DateTime>(nullable: false),
                    mc_user_id = table.Column<int>(nullable: false)
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
                name: "IX_mc_organisation_org_id",
                table: "mc_organisation",
                column: "org_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_institution_institution_id",
                table: "mc_organisation_institution",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_institution_mc_organisation_id",
                table: "mc_organisation_institution",
                column: "mc_organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_user_mc_organisation_id",
                table: "mc_organisation_user",
                column: "mc_organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_user_mc_user_id",
                table: "mc_organisation_user",
                column: "mc_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_session_mc_user_id",
                table: "mc_session",
                column: "mc_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_session_access_token_created_utc",
                table: "mc_session",
                columns: new[] { "access_token", "created_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_mc_user_email",
                table: "mc_user",
                column: "email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_access_request_mc_user_requester_id",
                table: "access_request",
                column: "requester_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_course_enrichment_mc_user_created_by_user_id",
                table: "course_enrichment",
                column: "created_by_user_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_enrichment_mc_user_updated_by_user_id",
                table: "course_enrichment",
                column: "updated_by_user_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_institution_enrichment_mc_user_created_by_user_id",
                table: "institution_enrichment",
                column: "created_by_user_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_institution_enrichment_mc_user_updated_by_user_id",
                table: "institution_enrichment",
                column: "updated_by_user_id",
                principalTable: "mc_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_nctl_organisation_mc_organisation_mc_organisation_id",
                table: "nctl_organisation",
                column: "mc_organisation_id",
                principalTable: "mc_organisation",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
