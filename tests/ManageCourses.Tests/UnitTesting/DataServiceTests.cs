using System.Collections.Generic;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Data;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class DataServiceTests
    {
        private Mock<IManageCoursesDbContext> _contextMock;
        private Mock<IEnrichmentService> _enrichmentServiceMock;
        private IDataService _dataService;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IManageCoursesDbContext>();
            _enrichmentServiceMock = new Mock<IEnrichmentService>();
            _dataService = new DataService(_contextMock.Object, _enrichmentServiceMock.Object, new Mock<ILogger<DataService>>().Object);
        }

        /// <summary>
        /// Test status generation variations
        /// </summary>
        [Test]
        public void Test_OrgStatus()
        {
            const string email = "roger@example.org";
            const string instCode = "BAT4";
            _contextMock.Setup(c => c.GetUserOrganisation(email, instCode)).Returns(new McOrganisationInstitution
            {
                Institution = new Institution(),
            });

            // test blank
            var org = _dataService.GetOrganisationForUser(email, instCode);
            org.Should().NotBeNull();
            org.EnrichmentWorkflowStatus.Should().BeNull("there's no enrichment present");

            // test draft
            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel { Status = EnumStatus.Draft };
            _enrichmentServiceMock.Setup(e => e.GetInstitutionEnrichment(instCode, email))
                .Returns(ucasInstitutionEnrichmentGetModel);
            var enrichedOrg = _dataService.GetOrganisationForUser(email, instCode);
            enrichedOrg.Should().NotBeNull();
            enrichedOrg.EnrichmentWorkflowStatus.Should().Be(ucasInstitutionEnrichmentGetModel.Status, $"there's a {ucasInstitutionEnrichmentGetModel.Status} enrichment present");

            // test published
            ucasInstitutionEnrichmentGetModel.Status = EnumStatus.Published;
            var publishedOrg = _dataService.GetOrganisationForUser(email, instCode);
            publishedOrg.Should().NotBeNull();
            publishedOrg.EnrichmentWorkflowStatus.Should().Be(ucasInstitutionEnrichmentGetModel.Status, $"there's a {ucasInstitutionEnrichmentGetModel.Status} enrichment present");
        }
    }
}

