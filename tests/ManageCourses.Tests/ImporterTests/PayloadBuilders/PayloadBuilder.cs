using System.Collections.Generic;
using GovUk.Education.ManageCourses.UcasCourseImporter;
using GovUk.Education.ManageCourses.Xls.Domain;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests.PayloadBuilders
{
    internal class PayloadBuilder
    {
        private readonly UcasPayload _payload;

        public PayloadBuilder()
        {
            _payload = new UcasPayload();
        }

        public static implicit operator UcasPayload(PayloadBuilder builder)
        {
            return builder._payload;
        }

        public PayloadBuilder WithInstitutions(IEnumerable<UcasInstitution> institutions)
        {
            _payload.Institutions = institutions;
            return this;
        }

        public PayloadBuilder WithCourses(IEnumerable<UcasCourse> courses)
        {
            _payload.Courses = courses;
            return this;
        }

        public PayloadBuilder WithCampuses(IEnumerable<UcasCampus> campuses)
        {
            _payload.Campuses = campuses;
            return this;
        }
    }
}
