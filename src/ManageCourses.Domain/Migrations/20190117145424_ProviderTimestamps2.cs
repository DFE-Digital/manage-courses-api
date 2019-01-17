using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class ProviderTimestamps2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "site",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'"); // https://stackoverflow.com/questions/16609724/using-current-time-in-utc-as-default-value-in-postgresql/16610360#16610360

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "site",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "provider",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "provider",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "site");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "site");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "provider");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "provider");
        }
    }
}
