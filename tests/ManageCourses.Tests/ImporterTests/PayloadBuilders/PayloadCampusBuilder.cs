using GovUk.Education.ManageCourses.Xls.Domain;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests.PayloadBuilders
{
    internal class PayloadCampusBuilder
    {
        private readonly UcasCampus _campus;

        public PayloadCampusBuilder()
        {
            _campus = new UcasCampus();
        }

        public static implicit operator UcasCampus(PayloadCampusBuilder builder)
        {
            return builder._campus;
        }

        public PayloadCampusBuilder WithCampusCode(string campusCode)
        {
            _campus.CampusCode = campusCode;
            return this;
        }

        public PayloadCampusBuilder WithInstCode(string instCode)
        {
            _campus.InstCode = instCode;
            return this;
        }

        public PayloadCampusBuilder WithRegionCode(int regionCode)
        {
            _campus.RegionCode = regionCode;
            return this;
        }
    }
}
