using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AddProviderChangedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "changed_at",
                table: "provider",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'"); // https://stackoverflow.com/questions/16609724/using-current-time-in-utc-as-default-value-in-postgresql/16610360#16610360
            migrationBuilder.CreateIndex(
                name: "IX_provider_changed_at",
                table: "provider",
                column: "changed_at");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_provider_changed_at",
                table: "provider");
            migrationBuilder.DropColumn(
                name: "changed_at",
                table: "provider");
        }
    }
}
