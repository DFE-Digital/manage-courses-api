using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public interface IManageCoursesApiClientConfiguration
    {
        string GetAccessToken();
    }
}
