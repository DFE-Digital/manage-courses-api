using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Middleware;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public interface IUserService
    {
        Task UserSignedInAsync(JsonUserDetails userDetails);
    }
}
