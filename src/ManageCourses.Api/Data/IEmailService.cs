
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IEmailService
    {
        bool ShouldBeAbleToSend();
        void SendAccessRequestEmailToSupport(AccessRequest accessRequest, McUser requester, McUser requestedOrNull);
    }
}
