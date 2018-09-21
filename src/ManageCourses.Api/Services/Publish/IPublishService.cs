using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Services.Publish
{
    public interface IPublishService
    {
        Task<bool> SaveSingleCourseToSearchAndCompare(string instCode, string courseCode, string email);
    }
}
