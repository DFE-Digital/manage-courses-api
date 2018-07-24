//using System.Collections.Generic;
//using NUnit.Framework;
//using GovUk.Education.ManageCourses.Api.Services;
//using Moq;

//namespace GovUk.Education.ManageCourses.Tests.UnitTesting
//{
//    [TestFixture]
//    public class TemplateEmailServiceTests
//    {
//        private ITemplateEmailService Subject = null;
//        private Mock<INotificationClientWrapper> mock = null;
//        private string templateId = "tempalteId";
//        [OneTimeSetUp]
//        public void OneTimeSetUp()
//        {
//            mock = new Mock<INotificationClientWrapper>();
//            Subject = new TemplateEmailService(mock.Object, templateId);
//        }

//        [Test]
//        public void Send()
//        {
//            var email = "actual email address";

//            var personalisation = new Dictionary<string, dynamic>() {
//                {"first_name", "some name" } };

//            Subject.Send(email, personalisation);

//            mock.Verify(x => x.SendEmail(email, templateId, personalisation, null, null), Times.Once);
//        }
//    }
//}
