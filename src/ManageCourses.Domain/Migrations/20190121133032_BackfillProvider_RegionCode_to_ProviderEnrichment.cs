using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class BackfillProvider_RegionCode_to_ProviderEnrichment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE provider_enrichment AS pe
SET    json_data = Jsonb_set(
                        json_data :: jsonb,
                        '{RegionCode}',
                        (SELECT To_json(p.region_code)
                        FROM provider AS p
                        WHERE p.provider_code = pe.provider_code) :: jsonb,
                        TRUE
                    )
WHERE  (SELECT p.region_code
        FROM   provider AS p
        WHERE  p.provider_code = pe.provider_code) IS NOT NULL
        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // no need
        }
    }
}
