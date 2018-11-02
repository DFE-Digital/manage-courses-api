using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Services.Publish
{
    public interface ISearchAndCompareService
    {
        Task<bool> SaveCourse(string providerCode, string courseCode, string email);
        Task<bool> SaveCourses(string providerCode, string email);
    }
}
