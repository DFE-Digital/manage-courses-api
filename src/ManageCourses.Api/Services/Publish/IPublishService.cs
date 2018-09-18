using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Services.Publish
{
    public interface IPublishService
    {
        Task<bool> PublishCourse(string instCode, string courseCode);
        Task<bool> PublishCourse(string instCode, string courseCode, string email);
    }
}
