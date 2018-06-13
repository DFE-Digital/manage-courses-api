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
    [Migration("20180607153827_InitialCreate")]
    partial class InitialCreate
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
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ProviderId");

                    b.Property<string>("Title");

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