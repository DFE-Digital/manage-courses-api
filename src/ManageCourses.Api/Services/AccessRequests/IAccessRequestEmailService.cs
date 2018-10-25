
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IAccessRequestEmailService
    {
        void SendAccessRequestEmailToSupport(AccessRequest accessRequest, User requester, User requestedOrNull);
    }
}
