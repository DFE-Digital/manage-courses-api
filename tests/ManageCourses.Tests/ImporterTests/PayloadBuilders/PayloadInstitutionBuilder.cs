using GovUk.Education.ManageCourses.Xls.Domain;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests.PayloadBuilders
{
    internal class PayloadInstitutionBuilder
    {
        private readonly UcasInstitution _institution;

        public PayloadInstitutionBuilder()
        {
            _institution = new UcasInstitution();
        }

        public static implicit operator UcasInstitution(PayloadInstitutionBuilder builder)
        {
            return builder._institution;
        }

        public PayloadInstitutionBuilder WithInstCode(string instCode)
        {
            _institution.InstCode = instCode;
            return this;
        }

        public PayloadInstitutionBuilder WithRegionCode(int regionCode)
        {
            _institution.RegionCode = regionCode;
            return this;
        }

        public PayloadInstitutionBuilder WithPostcode(string postCode)
        {
            _institution.Postcode = postCode;
            return this;
        }
    }
}
