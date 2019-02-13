using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenameSequences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER SEQUENCE mc_organisation_id_seq RENAME TO organisation_id_seq;
ALTER TABLE organisation ALTER id SET DEFAULT nextval('organisation_id_seq'::regclass);

ALTER SEQUENCE mc_organisation_institution_id_seq RENAME TO organisation_provider_id_seq;
ALTER TABLE organisation_provider ALTER id SET DEFAULT nextval('organisation_provider_id_seq'::regclass);

ALTER SEQUENCE mc_organisation_user_id_seq RENAME TO organisation_user_id_seq;
ALTER TABLE organisation_user ALTER id SET DEFAULT nextval('organisation_user_id_seq'::regclass);

ALTER SEQUENCE ""UcasInstitutions_Id_seq"" RENAME TO provider_id_seq;
ALTER TABLE provider ALTER id SET DEFAULT nextval('provider_id_seq'::regclass);

ALTER SEQUENCE institution_enrichment_id_seq RENAME TO provider_enrichment_id_seq;
ALTER TABLE provider_enrichment ALTER id SET DEFAULT nextval('provider_enrichment_id_seq'::regclass);

ALTER SEQUENCE mc_session_id_seq RENAME TO session_id_seq;
ALTER TABLE session ALTER id SET DEFAULT nextval('session_id_seq'::regclass);

ALTER SEQUENCE ""UcasCampuses_Id_seq"" RENAME TO site_id_seq;
ALTER TABLE site ALTER id SET DEFAULT nextval('site_id_seq'::regclass);

ALTER SEQUENCE ""UcasSubjects_Id_seq"" RENAME TO subject_id_seq;
ALTER TABLE subject ALTER id SET DEFAULT nextval('subject_id_seq'::regclass);

ALTER SEQUENCE mc_user_id_seq RENAME TO user_id_seq;
ALTER TABLE ""user"" ALTER id SET DEFAULT nextval('user_id_seq'::regclass);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER SEQUENCE organisation_id_seq RENAME TO mc_organisation_id_seq;

ALTER TABLE organisation ALTER id SET DEFAULT nextval('mc_organisation_id_seq'::regclass);

ALTER SEQUENCE organisation_provider_id_seq RENAME TO mc_organisation_institution_id_seq;
ALTER TABLE organisation_provider ALTER id SET DEFAULT nextval('mc_organisation_institution_id_seq'::regclass);

ALTER SEQUENCE organisation_user_id_seq RENAME TO mc_organisation_user_id_seq;
ALTER TABLE organisation_user ALTER id SET DEFAULT nextval('mc_organisation_user_id_seq'::regclass);

ALTER SEQUENCE provider_id_seq RENAME TO ""UcasInstitutions_Id_seq"";
ALTER TABLE provider ALTER id SET DEFAULT nextval('""UcasInstitutions_Id_seq""'::regclass);

ALTER SEQUENCE provider_enrichment_id_seq RENAME TO institution_enrichment_id_seq;
ALTER TABLE provider_enrichment ALTER id SET DEFAULT nextval('institution_enrichment_id_seq'::regclass);

ALTER SEQUENCE session_id_seq RENAME TO mc_session_id_seq;
ALTER TABLE session ALTER id SET DEFAULT nextval('mc_session_id_seq'::regclass);

ALTER SEQUENCE site_id_seq RENAME TO ""UcasCampuses_Id_seq"";
ALTER TABLE site ALTER id SET DEFAULT nextval('""UcasCampuses_Id_seq""'::regclass);

ALTER SEQUENCE subject_id_seq RENAME TO ""UcasSubjects_Id_seq"";
ALTER TABLE subject ALTER id SET DEFAULT nextval('""UcasSubjects_Id_seq""'::regclass);

ALTER SEQUENCE user_id_seq RENAME TO mc_user_id_seq;
ALTER TABLE ""user"" ALTER id SET DEFAULT nextval('mc_user_id_seq'::regclass);
            ");
        }
    }
}
