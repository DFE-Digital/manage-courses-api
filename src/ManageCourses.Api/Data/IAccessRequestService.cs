using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IAccessRequestService
    {
        void LogAccessRequest(AccessRequest request, string requesterEmail);
    }
}
