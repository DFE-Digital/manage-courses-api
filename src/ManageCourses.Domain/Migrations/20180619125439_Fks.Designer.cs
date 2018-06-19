﻿// <auto-generated />
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
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
    [Migration("20180619125439_Fks")]
    partial class Fks
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int?>("ProviderId")
                        .HasColumnName("provider_id");

                    b.Property<string>("Title")
                        .HasColumnName("title");

                    b.Property<string>("Type")
                        .HasColumnName("type");

                    b.Property<string>("UcasCode")
                        .HasColumnName("ucas_code");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId");

                    b.ToTable("course");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.McOrganisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<string>("NctlId")
                        .HasColumnName("nctl_id");

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

                    b.Property<string>("NctlId")
                        .HasColumnName("nctl_id");

                    b.HasKey("Id");

                    b.ToTable("mc_organisation_institution");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.McOrganisationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .HasColumnName("email");

                    b.Property<string>("NctlId")
                        .HasColumnName("nctl_id");

                    b.HasKey("Id");

                    b.ToTable("mc_organisation_user");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.McUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .HasColumnName("last_name");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("mc_user");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Provider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("NctlId")
                        .HasColumnName("nctl_id");

                    b.HasKey("Id");

                    b.ToTable("provider");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.ProviderMapper", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("InstitutionName")
                        .HasColumnName("institution_name");

                    b.Property<string>("NctlId")
                        .HasColumnName("nctl_id");

                    b.Property<string>("Type")
                        .HasColumnName("type");

                    b.Property<string>("UcasCode")
                        .HasColumnName("ucas_code");

                    b.Property<int>("Urn")
                        .HasColumnName("urn");

                    b.HasKey("Id");

                    b.ToTable("provider_mapper");
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
                        .HasColumnName("campus_code");

                    b.Property<string>("CampusName")
                        .HasColumnName("campus_name");

                    b.Property<string>("Email")
                        .HasColumnName("email");

                    b.Property<string>("InstCode")
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
                        .HasColumnName("subject_code");

                    b.Property<string>("SubjectDescription")
                        .HasColumnName("subject_description");

                    b.Property<string>("TitleMatch")
                        .HasColumnName("title_match");

                    b.HasKey("Id");

                    b.ToTable("ucas_subject");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Course", b =>
                {
                    b.HasOne("GovUk.Education.ManageCourses.Domain.Models.Provider")
                        .WithMany("Courses")
                        .HasForeignKey("ProviderId");
                });
#pragma warning restore 612, 618
        }
    }
}
