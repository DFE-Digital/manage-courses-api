namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageCoursesApiClientBase
    {
        protected IManageCoursesApiClientConfiguration ApiClientConfiguration {get; private set;}

        public ManageCoursesApiClientBase(IManageCoursesApiClientConfiguration apiClientConfiguration)
        {
            ApiClientConfiguration = apiClientConfiguration;
        }
    }
}
