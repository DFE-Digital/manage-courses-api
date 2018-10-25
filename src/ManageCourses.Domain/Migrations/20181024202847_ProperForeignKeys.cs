using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class ProperForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_institution_accrediting_inst_code",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "FK_course_institution_inst_code",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_institution_institution_inst_code",
                table: "mc_organisation_institution");

            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_institution_mc_organisation_org_id",
                table: "mc_organisation_institution");

            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_user_email",
                table: "mc_organisation_user");

            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_organisation_org_id",
                table: "mc_organisation_user");

            migrationBuilder.DropForeignKey(
                name: "FK_nctl_organisation_mc_organisation_org_id",
                table: "nctl_organisation");

            migrationBuilder.DropForeignKey(
                name: "FK_ucas_campus_ucas_institution_inst_code",
                table: "site");

            migrationBuilder.DropIndex(
                name: "IX_site_inst_code",
                table: "site");

            migrationBuilder.DropIndex(
                name: "IX_nctl_organisation_org_id",
                table: "nctl_organisation");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_mc_user_email",
                table: "mc_user");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_user_org_id",
                table: "mc_organisation_user");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_user_email_org_id",
                table: "mc_organisation_user");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_institution_inst_code",
                table: "mc_organisation_institution");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_institution_org_id_inst_code",
                table: "mc_organisation_institution");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_mc_organisation_org_id",
                table: "mc_organisation");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ucas_institution_inst_code",
                table: "institution");

            migrationBuilder.DropIndex(
                name: "IX_course_accrediting_inst_code",
                table: "course");

            migrationBuilder.DropIndex(
                name: "IX_course_inst_code_course_code",
                table: "course");

            migrationBuilder.AddColumn<int>(
                name: "institution_id",
                table: "site",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE site a SET institution_id = b.id FROM institution b WHERE b.inst_code = a.inst_code");

            migrationBuilder.DropColumn(
                name: "inst_code",
                table: "site");


            migrationBuilder.AddColumn<int>(
                name: "mc_organisation_id",
                table: "nctl_organisation",
                nullable: true);

            migrationBuilder.Sql("UPDATE nctl_organisation a SET mc_organisation_id = b.id FROM mc_organisation b WHERE b.org_id = a.org_id");

            migrationBuilder.DropColumn(
                name: "org_id",
                table: "nctl_organisation");

            
            migrationBuilder.AddColumn<int>(
                name: "mc_organisation_id",
                table: "mc_organisation_user",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "mc_user_id",
                table: "mc_organisation_user",
                nullable: true);

            migrationBuilder.Sql("UPDATE mc_organisation_user a SET mc_organisation_id = b.id, mc_user_id = c.id FROM mc_organisation b, mc_user c WHERE c.email = a.email AND b.org_id = a.org_id ");

            migrationBuilder.DropColumn(
                name: "email",
                table: "mc_organisation_user");

            migrationBuilder.DropColumn(
                name: "org_id",
                table: "mc_organisation_user");

                
            migrationBuilder.AddColumn<int>(
                name: "institution_id",
                table: "mc_organisation_institution",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "mc_organisation_id",
                table: "mc_organisation_institution",
                nullable: true);

            migrationBuilder.Sql("UPDATE mc_organisation_institution a SET mc_organisation_id = b.id, institution_id = c.id FROM mc_organisation b, institution c WHERE c.inst_code = a.inst_code AND b.org_id = a.org_id");

            migrationBuilder.DropColumn(
                name: "inst_code",
                table: "mc_organisation_institution");

            migrationBuilder.DropColumn(
                name: "org_id",
                table: "mc_organisation_institution");

            migrationBuilder.AddColumn<int>(
                name: "accrediting_institution_id",
                table: "course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "institution_id",
                table: "course",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE course a SET accrediting_institution_id = b.id, institution_id = c.id FROM institution b, institution c WHERE c.inst_code = a.inst_code AND b.inst_code = a.accrediting_inst_code");

            migrationBuilder.DropColumn(
                name: "accrediting_inst_code",
                table: "course");

            migrationBuilder.DropColumn(
                name: "inst_code",
                table: "course");


            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "mc_user",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "org_id",
                table: "mc_organisation",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "inst_code",
                table: "institution",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_subject_subject_code",
                table: "subject",
                column: "subject_code",
                unique: true);
                
            migrationBuilder.CreateIndex(
                name: "IX_site_institution_id_code",
                table: "site",
                columns: new[] { "institution_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_nctl_organisation_mc_organisation_id",
                table: "nctl_organisation",
                column: "mc_organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_user_email",
                table: "mc_user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_user_mc_organisation_id",
                table: "mc_organisation_user",
                column: "mc_organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_user_mc_user_id",
                table: "mc_organisation_user",
                column: "mc_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_institution_institution_id",
                table: "mc_organisation_institution",
                column: "institution_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_institution_mc_organisation_id",
                table: "mc_organisation_institution",
                column: "mc_organisation_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_org_id",
                table: "mc_organisation",
                column: "org_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_accrediting_institution_id",
                table: "course",
                column: "accrediting_institution_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_institution_id_id",
                table: "course",
                columns: new[] { "institution_id", "id" },
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
                name: "FK_mc_organisation_institution_institution_institution_id",
                table: "mc_organisation_institution",
                column: "institution_id",
                principalTable: "institution",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_institution_mc_organisation_mc_organisation_id",
                table: "mc_organisation_institution",
                column: "mc_organisation_id",
                principalTable: "mc_organisation",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_user_mc_organisation_mc_organisation_id",
                table: "mc_organisation_user",
                column: "mc_organisation_id",
                principalTable: "mc_organisation",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_user_mc_user_mc_user_id",
                table: "mc_organisation_user",
                column: "mc_user_id",
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
            

            migrationBuilder.AddForeignKey(
                name: "FK_site_institution_institution_id",
                table: "site",
                column: "institution_id",
                principalTable: "institution",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_institution_accrediting_institution_id",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "FK_course_institution_institution_id",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_institution_institution_institution_id",
                table: "mc_organisation_institution");

            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_institution_mc_organisation_mc_organisation_id",
                table: "mc_organisation_institution");

            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_organisation_mc_organisation_id",
                table: "mc_organisation_user");

            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_user_mc_user_id",
                table: "mc_organisation_user");

            migrationBuilder.DropForeignKey(
                name: "FK_nctl_organisation_mc_organisation_mc_organisation_id",
                table: "nctl_organisation");

            migrationBuilder.DropForeignKey(
                name: "FK_site_institution_institution_id",
                table: "site");

            migrationBuilder.DropIndex(
                name: "IX_subject_subject_code",
                table: "subject");

            migrationBuilder.DropIndex(
                name: "IX_site_institution_id_code",
                table: "site");

            migrationBuilder.DropIndex(
                name: "IX_nctl_organisation_mc_organisation_id",
                table: "nctl_organisation");

            migrationBuilder.DropIndex(
                name: "IX_mc_user_email",
                table: "mc_user");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_user_mc_organisation_id",
                table: "mc_organisation_user");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_user_mc_user_id",
                table: "mc_organisation_user");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_institution_institution_id",
                table: "mc_organisation_institution");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_institution_mc_organisation_id",
                table: "mc_organisation_institution");

            migrationBuilder.DropIndex(
                name: "IX_mc_organisation_org_id",
                table: "mc_organisation");

            migrationBuilder.DropIndex(
                name: "IX_course_accrediting_institution_id",
                table: "course");

            migrationBuilder.DropIndex(
                name: "IX_course_institution_id_id",
                table: "course");

            migrationBuilder.DropColumn(
                name: "institution_id",
                table: "site");

            migrationBuilder.DropColumn(
                name: "mc_organisation_id",
                table: "nctl_organisation");

            migrationBuilder.DropColumn(
                name: "mc_organisation_id",
                table: "mc_organisation_user");

            migrationBuilder.DropColumn(
                name: "mc_user_id",
                table: "mc_organisation_user");

            migrationBuilder.DropColumn(
                name: "institution_id",
                table: "mc_organisation_institution");

            migrationBuilder.DropColumn(
                name: "mc_organisation_id",
                table: "mc_organisation_institution");

            migrationBuilder.DropColumn(
                name: "accrediting_institution_id",
                table: "course");

            migrationBuilder.DropColumn(
                name: "institution_id",
                table: "course");

            migrationBuilder.AddColumn<string>(
                name: "inst_code",
                table: "site",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "org_id",
                table: "nctl_organisation",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "mc_user",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "mc_organisation_user",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "org_id",
                table: "mc_organisation_user",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "inst_code",
                table: "mc_organisation_institution",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "org_id",
                table: "mc_organisation_institution",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "org_id",
                table: "mc_organisation",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "inst_code",
                table: "institution",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "accrediting_inst_code",
                table: "course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "inst_code",
                table: "course",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_mc_user_email",
                table: "mc_user",
                column: "email");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_mc_organisation_org_id",
                table: "mc_organisation",
                column: "org_id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ucas_institution_inst_code",
                table: "institution",
                column: "inst_code");

            migrationBuilder.CreateIndex(
                name: "IX_site_inst_code",
                table: "site",
                column: "inst_code");

            migrationBuilder.CreateIndex(
                name: "IX_nctl_organisation_org_id",
                table: "nctl_organisation",
                column: "org_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_user_org_id",
                table: "mc_organisation_user",
                column: "org_id");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_user_email_org_id",
                table: "mc_organisation_user",
                columns: new[] { "email", "org_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_institution_inst_code",
                table: "mc_organisation_institution",
                column: "inst_code");

            migrationBuilder.CreateIndex(
                name: "IX_mc_organisation_institution_org_id_inst_code",
                table: "mc_organisation_institution",
                columns: new[] { "org_id", "inst_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_accrediting_inst_code",
                table: "course",
                column: "accrediting_inst_code");

            migrationBuilder.CreateIndex(
                name: "IX_course_inst_code_course_code",
                table: "course",
                columns: new[] { "inst_code", "course_code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_course_institution_accrediting_inst_code",
                table: "course",
                column: "accrediting_inst_code",
                principalTable: "institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_institution_inst_code",
                table: "course",
                column: "inst_code",
                principalTable: "institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_institution_institution_inst_code",
                table: "mc_organisation_institution",
                column: "inst_code",
                principalTable: "institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_institution_mc_organisation_org_id",
                table: "mc_organisation_institution",
                column: "org_id",
                principalTable: "mc_organisation",
                principalColumn: "org_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_user_mc_user_email",
                table: "mc_organisation_user",
                column: "email",
                principalTable: "mc_user",
                principalColumn: "email",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_user_mc_organisation_org_id",
                table: "mc_organisation_user",
                column: "org_id",
                principalTable: "mc_organisation",
                principalColumn: "org_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_nctl_organisation_mc_organisation_org_id",
                table: "nctl_organisation",
                column: "org_id",
                principalTable: "mc_organisation",
                principalColumn: "org_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ucas_campus_ucas_institution_inst_code",
                table: "site",
                column: "inst_code",
                principalTable: "institution",
                principalColumn: "inst_code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
