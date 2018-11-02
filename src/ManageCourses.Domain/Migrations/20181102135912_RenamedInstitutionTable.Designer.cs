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

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    [DbContext(typeof(ManageCoursesDbContext))]
    [Migration("20181102135912_RenamedInstitutionTable")]
    partial class RenamedInstitutionTable
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

                    b.Property<string>("RequesterEmail")
                        .HasColumnName("requester_email");

                    b.Property<int?>("RequesterId")
                        .HasColumnName("requester_id");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.HasIndex("RequesterId");

                    b.ToTable("access_request");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int?>("AccreditingInstitutionId")
                        .HasColumnName("accrediting_institution_id");

                    b.Property<string>("AgeRange")
                        .HasColumnName("age_range");

                    b.Property<string>("CourseCode")
                        .HasColumnName("course_code");

                    b.Property<int>("InstitutionId")
                        .HasColumnName("institution_id");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<string>("ProfpostFlag")
                        .HasColumnName("profpost_flag");

                    b.Property<string>("ProgramType")
                        .HasColumnName("program_type");

                    b.Property<int>("Qualification")
                        .HasColumnName("qualification");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnName("start_date");

                    b.Property<string>("StudyMode")
                        .HasColumnName("study_mode");

                    b.HasKey("Id");

                    b.HasIndex("AccreditingInstitutionId");

                    b.HasIndex("InstitutionId", "CourseCode")
                        .IsUnique();

                    b.ToTable("course");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.CourseEnrichment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnName("created_by_user_id");

                    b.Property<DateTime>("CreatedTimestampUtc")
                        .HasColumnName("created_timestamp_utc");

                    b.Property<string>("InstCode")
                        .IsRequired()
                        .HasColumnName("inst_code");

                    b.Property<string>("JsonData")
                        .HasColumnName("json_data")
                        .HasColumnType("jsonb");

                    b.Property<DateTime?>("LastPublishedTimestampUtc")
                        .HasColumnName("last_published_timestamp_utc");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.Property<string>("UcasCourseCode")
                        .IsRequired()
                        .HasColumnName("ucas_course_code");

                    b.Property<int?>("UpdatedByUserId")
                        .HasColumnName("updated_by_user_id");

                    b.Property<DateTime>("UpdatedTimestampUtc")
                        .HasColumnName("updated_timestamp_utc");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("UpdatedByUserId");

                    b.ToTable("course_enrichment");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.CourseSite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("ApplicationsAcceptedFrom")
                        .HasColumnName("applications_accepted_from");

                    b.Property<int?>("CourseId")
                        .HasColumnName("course_id");

                    b.Property<string>("Publish")
                        .HasColumnName("publish");

                    b.Property<int?>("SiteId")
                        .HasColumnName("site_id");

                    b.Property<string>("Status")
                        .HasColumnName("status");

                    b.Property<string>("VacStatus")
                        .HasColumnName("vac_status");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("SiteId");

                    b.ToTable("course_site");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.CourseSubject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int?>("CourseId")
                        .HasColumnName("course_id");

                    b.Property<int?>("SubjectId")
                        .HasColumnName("subject_id");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("SubjectId");

                    b.ToTable("course_subject");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.InstitutionEnrichment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnName("created_by_user_id");

                    b.Property<DateTime>("CreatedTimestampUtc")
                        .HasColumnName("created_timestamp_utc");

                    b.Property<string>("InstCode")
                        .IsRequired()
                        .HasColumnName("inst_code");

                    b.Property<string>("JsonData")
                        .HasColumnName("json_data")
                        .HasColumnType("jsonb");

                    b.Property<DateTime?>("LastPublishedTimestampUtc")
                        .HasColumnName("last_published_timestamp_utc");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.Property<int?>("UpdatedByUserId")
                        .HasColumnName("updated_by_user_id");

                    b.Property<DateTime>("UpdatedTimestampUtc")
                        .HasColumnName("updated_timestamp_utc");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("InstCode");

                    b.HasIndex("UpdatedByUserId");

                    b.ToTable("institution_enrichment");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.NctlOrganisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<string>("NctlId")
                        .IsRequired()
                        .HasColumnName("nctl_id");

                    b.Property<int?>("OrganisationId")
                        .HasColumnName("organisation_id");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.ToTable("nctl_organisation");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Organisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<string>("OrgId")
                        .HasColumnName("org_id");

                    b.HasKey("Id");

                    b.HasIndex("OrgId")
                        .IsUnique();

                    b.ToTable("organisation");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.OrganisationProvider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int?>("OrganisationId")
                        .HasColumnName("organisation_id");

                    b.Property<int?>("ProviderId")
                        .HasColumnName("provider_id");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.HasIndex("ProviderId");

                    b.ToTable("organisation_provider");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.OrganisationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int?>("OrganisationId")
                        .HasColumnName("organisation_id");

                    b.Property<int?>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.HasIndex("UserId");

                    b.ToTable("organisation_user");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.PgdeCourse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("CourseCode")
                        .IsRequired()
                        .HasColumnName("course_code");

                    b.Property<string>("InstCode")
                        .IsRequired()
                        .HasColumnName("inst_code");

                    b.HasKey("Id");

                    b.ToTable("pgde_course");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Provider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Address1")
                        .HasColumnName("address1");

                    b.Property<string>("Address2")
                        .HasColumnName("address2");

                    b.Property<string>("Address3")
                        .HasColumnName("address3");

                    b.Property<string>("Address4")
                        .HasColumnName("address4");

                    b.Property<string>("ContactName")
                        .HasColumnName("contact_name");

                    b.Property<string>("Email")
                        .HasColumnName("email");

                    b.Property<string>("InstType")
                        .HasColumnName("inst_type");

                    b.Property<string>("Postcode")
                        .HasColumnName("postcode");

                    b.Property<string>("ProviderCode")
                        .HasColumnName("provider_code");

                    b.Property<string>("ProviderName")
                        .HasColumnName("provider_name");

                    b.Property<string>("SchemeMember")
                        .HasColumnName("scheme_member");

                    b.Property<string>("Scitt")
                        .HasColumnName("scitt");

                    b.Property<string>("Telephone")
                        .HasColumnName("telephone");

                    b.Property<string>("Url")
                        .HasColumnName("url");

                    b.Property<string>("YearCode")
                        .HasColumnName("year_code");

                    b.HasKey("Id");

                    b.HasIndex("ProviderCode")
                        .IsUnique();

                    b.ToTable("provider");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Session", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("AccessToken")
                        .HasColumnName("access_token");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnName("created_utc");

                    b.Property<int>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("AccessToken", "CreatedUtc");

                    b.ToTable("session");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Site", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Address1")
                        .HasColumnName("address1");

                    b.Property<string>("Address2")
                        .HasColumnName("address2");

                    b.Property<string>("Address3")
                        .HasColumnName("address3");

                    b.Property<string>("Address4")
                        .HasColumnName("address4");

                    b.Property<string>("Code")
                        .HasColumnName("code");

                    b.Property<int>("InstitutionId")
                        .HasColumnName("institution_id");

                    b.Property<string>("LocationName")
                        .HasColumnName("location_name");

                    b.Property<string>("Postcode")
                        .HasColumnName("postcode");

                    b.HasKey("Id");

                    b.HasIndex("InstitutionId", "Code")
                        .IsUnique();

                    b.ToTable("site");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Subject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("SubjectCode")
                        .HasColumnName("subject_code");

                    b.Property<string>("SubjectName")
                        .HasColumnName("subject_name");

                    b.HasKey("Id");

                    b.HasIndex("SubjectCode")
                        .IsUnique();

                    b.ToTable("subject");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime?>("AcceptTermsDateUtc")
                        .HasColumnName("accept_terms_date_utc");

                    b.Property<string>("Email")
                        .HasColumnName("email");

                    b.Property<DateTime?>("FirstLoginDateUtc")
                        .HasColumnName("first_login_date_utc");

                    b.Property<string>("FirstName")
                        .HasColumnName("first_name");

                    b.Property<DateTime?>("InviteDateUtc")
                        .HasColumnName("invite_date_utc");

                    b.Property<DateTime?>("LastLoginDateUtc")
                        .HasColumnName("last_login_date_utc");

                    b.Property<string>("LastName")
                        .HasColumnName("last_name");

                    b.Property<string>("SignInUserId")
                        .HasColumnName("sign_in_user_id");

                    b.Property<DateTime?>("WelcomeEmailDateUtc")
                        .HasColumnName("welcome_email_date_utc");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("user");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.AccessRequest", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.User", "Requester")
                        .WithMany("AccessRequests")
                        .HasForeignKey("RequesterId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Course", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Provider", "AccreditingInstitution")
                        .WithMany("AccreditedCourses")
                        .HasForeignKey("AccreditingInstitutionId");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Provider", "Institution")
                        .WithMany("Courses")
                        .HasForeignKey("InstitutionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.CourseEnrichment", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.User", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.User", "UpdatedByUser")
                        .WithMany()
                        .HasForeignKey("UpdatedByUserId");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.CourseSite", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Course", "Course")
                        .WithMany("CourseSites")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Site", "Site")
                        .WithMany("CourseSites")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.CourseSubject", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Course", "Course")
                        .WithMany("CourseSubjects")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Subject", "Subject")
                        .WithMany("CourseSubjects")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.InstitutionEnrichment", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.User", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.User", "UpdatedByUser")
                        .WithMany()
                        .HasForeignKey("UpdatedByUserId");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.NctlOrganisation", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Organisation", "Organisation")
                        .WithMany("NctlOrganisations")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.OrganisationProvider", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Organisation", "Organisation")
                        .WithMany("OrganisationInstitutions")
                        .HasForeignKey("OrganisationId");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Provider", "Provider")
                        .WithMany("OrganisationProviders")
                        .HasForeignKey("ProviderId");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.OrganisationUser", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Organisation", "Organisation")
                        .WithMany("OrganisationUsers")
                        .HasForeignKey("OrganisationId");

                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.User", "User")
                        .WithMany("OrganisationUsers")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Session", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Site", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Provider", "Institution")
                        .WithMany("Sites")
                        .HasForeignKey("InstitutionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
