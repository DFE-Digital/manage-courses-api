using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class EnrichmentServiceTests
    {
        [Test]
        public void GetsEmptyInstitutionEnrichment()
        {
            var contextMock = new Mock<IManageCoursesDbContext>();
            var enrichmentService = new EnrichmentService(contextMock.Object);
            var enrichment = enrichmentService.GetInstitutionEnrichment("INST1", "shirley@example.org");
            enrichment.EnrichmentModel.TrainWithUs.Should().BeNull();
            enrichment.EnrichmentModel.TrainWithDisability.Should().BeNull();
            enrichment.EnrichmentModel.AccreditingProviderEnrichments.Should().BeNull();
            enrichment.EnrichmentModel.Email.Should().BeNull();
            enrichment.EnrichmentModel.Telephone.Should().BeNull();
            enrichment.EnrichmentModel.Website.Should().BeNull();
            enrichment.EnrichmentModel.Address1.Should().BeNull();
            enrichment.EnrichmentModel.Address2.Should().BeNull();
            enrichment.EnrichmentModel.Address3.Should().BeNull();
            enrichment.EnrichmentModel.Address4.Should().BeNull();
            enrichment.EnrichmentModel.Postcode.Should().BeNull();
        }
    }
}
