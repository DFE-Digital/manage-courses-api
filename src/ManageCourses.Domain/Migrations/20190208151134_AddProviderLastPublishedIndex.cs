using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AddProviderLastPublishedIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_provider_last_published_at",
                table: "provider",
                column: "last_published_at");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_provider_last_published_at",
                table: "provider");
        }
    }
}
