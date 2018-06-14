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
    partial class ManageCoursesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ProviderId");

                    b.Property<string>("Title");

                    b.Property<string>("Type");

                    b.Property<string>("UcasCode");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.Provider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("NctlId");

                    b.HasKey("Id");

                    b.ToTable("Providers");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.ProviderMapper", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InstitutionName");

                    b.Property<string>("NctlId");

                    b.Property<string>("Type");

                    b.Property<string>("UcasCode");

                    b.Property<int>("Urn");

                    b.HasKey("Id");

                    b.ToTable("ProviderMapper");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasCampus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address1");

                    b.Property<string>("Address2");

                    b.Property<string>("Address3");

                    b.Property<string>("Address4");

                    b.Property<string>("CampusCode");

                    b.Property<string>("CampusName");

                    b.Property<string>("InstCode");

                    b.Property<string>("PostCode");

                    b.Property<string>("RegionCode");

                    b.Property<string>("TelNo");

                    b.HasKey("Id");

                    b.ToTable("UcasCampuses");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasCourse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AcreditedProvider");

                    b.Property<string>("AgeGroup");

                    b.Property<string>("CampusCode");

                    b.Property<string>("CourseCode");

                    b.Property<string>("InstCode");

                    b.Property<string>("OpenDate");

                    b.Property<string>("ProfPostFlag");

                    b.Property<string>("ProgramType");

                    b.Property<string>("StudyMode");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("UcasCourses");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasCourseNote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CourseCode");

                    b.Property<string>("InstCode");

                    b.Property<string>("NoteNo");

                    b.Property<string>("NoteType");

                    b.HasKey("Id");

                    b.ToTable("UcasCourseNotes");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasCourseSubject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CourseCode");

                    b.Property<string>("InstCode");

                    b.Property<string>("SubjectCode");

                    b.HasKey("Id");

                    b.ToTable("UcasCourseSubjects");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasInstitution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AcreditedProvider");

                    b.Property<string>("Address1");

                    b.Property<string>("Address2");

                    b.Property<string>("Address3");

                    b.Property<string>("Address4");

                    b.Property<string>("ContactName");

                    b.Property<string>("FullName");

                    b.Property<string>("InstCode");

                    b.Property<string>("InstType");

                    b.Property<string>("PostCode");

                    b.Property<string>("Scitt");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("UcasInstitutions");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasNoteText", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InstCode");

                    b.Property<string>("LineText");

                    b.Property<string>("NoteNo");

                    b.Property<string>("NoteType");

                    b.HasKey("Id");

                    b.ToTable("UcasNoteTexts");
                });

            modelBuilder.Entity("GovUk.Education.ManageCourses.Domain.Models.UcasSubject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SubjectCode");

                    b.Property<string>("SubjectDescription");

                    b.Property<string>("TitleMatch");

                    b.HasKey("Id");

                    b.ToTable("UcasSubjects");
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
