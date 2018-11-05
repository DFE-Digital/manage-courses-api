﻿// <auto-generated />
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    [DbContext(typeof(ManageCoursesDbContext))]
    [Migration("20180710162624_AddUserEmailColumn")]
    partial class AddUserEmailColumn
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.AccessRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("EmailAddress")
                        .HasColumnName("email_address");

                    b.Property<string>("FirstName")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .HasColumnName("last_name");

                    b.Property<string>("Organisation")
                        .HasColumnName("organisation");

                    b.Property<string>("Reason")
                        .HasColumnName("reason");

                    b.Property<DateTime>("RequestDateUtc")
                        .HasColumnName("request_date_utc");

                    b.Property<int?>("RequesterId")
                        .HasColumnName("requester_id");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.HasIndex("RequesterId");

                    b.ToTable("access_request");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.CourseCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("CrseCode")
                        .IsRequired()
                        .HasColumnName("crse_code");

                    b.Property<string>("InstCode")
                        .IsRequired()
                        .HasColumnName("inst_code");

                    b.HasKey("Id");

                    b.ToTable("course_code");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.McOrganisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<string>("OrgId")
                        .IsRequired()
                        .HasColumnName("org_id");

                    b.HasKey("Id");

                    b.ToTable("mc_organisation");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.McOrganisationInstitution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("InstitutionCode")
                        .HasColumnName("institution_code");

                    b.Property<string>("OrgId")
                        .HasColumnName("org_id");

                    b.HasKey("Id");

                    b.HasIndex("InstitutionCode");

                    b.HasIndex("OrgId", "InstitutionCode")
                        .IsUnique();

                    b.ToTable("mc_organisation_institution");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.McOrganisationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .HasColumnName("email");

                    b.Property<string>("OrgId")
                        .HasColumnName("org_id");

                    b.HasKey("Id");

                    b.HasIndex("OrgId");

                    b.HasIndex("Email", "OrgId")
                        .IsUnique();

                    b.ToTable("mc_organisation_user");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.McUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .HasColumnName("last_name");

                    b.HasKey("Id");

                    b.ToTable("mc_user");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasCampus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Addr1")
                        .HasColumnName("addr1");

                    b.Property<string>("Addr2")
                        .HasColumnName("addr2");

                    b.Property<string>("Addr3")
                        .HasColumnName("addr3");

                    b.Property<string>("Addr4")
                        .HasColumnName("addr4");

                    b.Property<string>("CampusCode")
                        .IsRequired()
                        .HasColumnName("campus_code");

                    b.Property<string>("CampusName")
                        .HasColumnName("campus_name");

                    b.Property<string>("Email")
                        .HasColumnName("email");

                    b.Property<string>("InstCode")
                        .IsRequired()
                        .HasColumnName("inst_code");

                    b.Property<string>("Postcode")
                        .HasColumnName("postcode");

                    b.Property<string>("RegionCode")
                        .HasColumnName("region_code");

                    b.Property<string>("TelNo")
                        .HasColumnName("tel_no");

                    b.HasKey("Id");

                    b.ToTable("ucas_campus");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasCourse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("AccreditingProvider")
                        .HasColumnName("accrediting_provider");

                    b.Property<string>("Age")
                        .HasColumnName("age");

                    b.Property<string>("CampusCode")
                        .HasColumnName("campus_code");

                    b.Property<string>("CrseCode")
                        .HasColumnName("crse_code");

                    b.Property<string>("CrseOpenDate")
                        .HasColumnName("crse_open_date");

                    b.Property<string>("CrseTitle")
                        .HasColumnName("crse_title");

                    b.Property<string>("InstCode")
                        .HasColumnName("inst_code");

                    b.Property<string>("ProfpostFlag")
                        .HasColumnName("profpost_flag");

                    b.Property<string>("ProgramType")
                        .HasColumnName("program_type");

                    b.Property<string>("Studymode")
                        .HasColumnName("studymode");

                    b.HasKey("Id");

                    b.HasIndex("AccreditingProvider");

                    b.HasIndex("InstCode", "CampusCode");

                    b.HasIndex("InstCode", "CrseCode", "CampusCode")
                        .IsUnique();

                    b.ToTable("ucas_course");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasCourseNote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("CrseCode")
                        .HasColumnName("crse_code");

                    b.Property<string>("InstCode")
                        .HasColumnName("inst_code");

                    b.Property<string>("NoteNo")
                        .HasColumnName("note_no");

                    b.Property<string>("NoteType")
                        .HasColumnName("note_type");

                    b.Property<string>("YearCode")
                        .HasColumnName("year_code");

                    b.HasKey("Id");

                    b.ToTable("ucas_course_note");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasCourseSubject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("CrseCode")
                        .HasColumnName("crse_code");

                    b.Property<string>("InstCode")
                        .HasColumnName("inst_code");

                    b.Property<string>("SubjectCode")
                        .HasColumnName("subject_code");

                    b.Property<string>("YearCode")
                        .HasColumnName("year_code");

                    b.HasKey("Id");

                    b.HasIndex("SubjectCode");

                    b.HasIndex("InstCode", "CrseCode");

                    b.ToTable("ucas_course_subject");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasInstitution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("AccreditingProvider")
                        .HasColumnName("accrediting_provider");

                    b.Property<string>("Addr1")
                        .HasColumnName("addr1");

                    b.Property<string>("Addr2")
                        .HasColumnName("addr2");

                    b.Property<string>("Addr3")
                        .HasColumnName("addr3");

                    b.Property<string>("Addr4")
                        .HasColumnName("addr4");

                    b.Property<string>("ContactName")
                        .HasColumnName("contact_name");

                    b.Property<string>("InstBig")
                        .HasColumnName("inst_big");

                    b.Property<string>("InstCode")
                        .IsRequired()
                        .HasColumnName("inst_code");

                    b.Property<string>("InstFull")
                        .HasColumnName("inst_full");

                    b.Property<string>("InstName")
                        .HasColumnName("inst_name");

                    b.Property<string>("InstType")
                        .HasColumnName("inst_type");

                    b.Property<string>("Postcode")
                        .HasColumnName("postcode");

                    b.Property<string>("SchemeMember")
                        .HasColumnName("scheme_member");

                    b.Property<string>("Scitt")
                        .HasColumnName("scitt");

                    b.Property<string>("Url")
                        .HasColumnName("url");

                    b.Property<string>("YearCode")
                        .HasColumnName("year_code");

                    b.HasKey("Id");

                    b.HasIndex("InstCode")
                        .IsUnique();

                    b.ToTable("ucas_institution");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasNoteText", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("InstCode")
                        .HasColumnName("inst_code");

                    b.Property<string>("LineText")
                        .HasColumnName("line_text");

                    b.Property<string>("NoteNo")
                        .HasColumnName("note_no");

                    b.Property<string>("NoteType")
                        .HasColumnName("note_type");

                    b.Property<string>("YearCode")
                        .HasColumnName("year_code");

                    b.HasKey("Id");

                    b.ToTable("ucas_note_text");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasSubject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("SubjectCode")
                        .IsRequired()
                        .HasColumnName("subject_code");

                    b.Property<string>("SubjectDescription")
                        .HasColumnName("subject_description");

                    b.Property<string>("TitleMatch")
                        .HasColumnName("title_match");

                    b.HasKey("Id");

                    b.ToTable("ucas_subject");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UserLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("FirstLoginDateUtc")
                        .HasColumnName("first_login_date_utc");

                    b.Property<DateTime>("LastLoginDateUtc")
                        .HasColumnName("last_login_date_utc");

                    b.Property<string>("SignInUserId")
                        .HasColumnName("sign_in_user_id");

                    b.Property<string>("UserEmail")
                        .HasColumnName("user_email");

                    b.Property<int?>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("user_log");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.AccessRequest", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.McUser", "Requester")
                        .WithMany()
                        .HasForeignKey("RequesterId");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.CourseCode", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.UcasInstitution", "UcasInstitution")
                        .WithMany("CourseCodes")
                        .HasForeignKey("InstCode")
                        .HasPrincipalKey("InstCode")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.McOrganisationInstitution", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.UcasInstitution", "UcasInstitution")
                        .WithMany("McOrganisationInstitutions")
                        .HasForeignKey("InstitutionCode")
                        .HasPrincipalKey("InstCode");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.McOrganisation", "McOrganisation")
                        .WithMany("McOrganisationInstitutions")
                        .HasForeignKey("OrgId")
                        .HasPrincipalKey("OrgId");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.McOrganisationUser", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.McUser", "McUser")
                        .WithMany("McOrganisationUsers")
                        .HasForeignKey("Email")
                        .HasPrincipalKey("Email");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.McOrganisation", "McOrganisation")
                        .WithMany("McOrganisationUsers")
                        .HasForeignKey("OrgId")
                        .HasPrincipalKey("OrgId");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasCampus", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.UcasInstitution", "UcasInstitution")
                        .WithMany("UcasCampuses")
                        .HasForeignKey("InstCode")
                        .HasPrincipalKey("InstCode")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasCourse", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.UcasInstitution", "AccreditingProviderInstitution")
                        .WithMany("AccreditedUcasCourses")
                        .HasForeignKey("AccreditingProvider")
                        .HasPrincipalKey("InstCode");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.UcasInstitution", "UcasInstitution")
                        .WithMany("UcasCourses")
                        .HasForeignKey("InstCode")
                        .HasPrincipalKey("InstCode");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.UcasCampus", "UcasCampus")
                        .WithMany("UcasCourses")
                        .HasForeignKey("InstCode", "CampusCode")
                        .HasPrincipalKey("InstCode", "CampusCode");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.CourseCode", "CourseCode")
                        .WithMany("UcasCourses")
                        .HasForeignKey("InstCode", "CrseCode")
                        .HasPrincipalKey("InstCode", "CrseCode");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasCourseSubject", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.UcasInstitution", "UcasInstitution")
                        .WithMany("UcasCourseSubjects")
                        .HasForeignKey("InstCode")
                        .HasPrincipalKey("InstCode");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.UcasSubject", "UcasSubject")
                        .WithMany("UcasCourseSubjects")
                        .HasForeignKey("SubjectCode")
                        .HasPrincipalKey("SubjectCode");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.CourseCode", "CourseCode")
                        .WithMany("UcasCourseSubjects")
                        .HasForeignKey("InstCode", "CrseCode")
                        .HasPrincipalKey("InstCode", "CrseCode");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UserLog", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.McUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
