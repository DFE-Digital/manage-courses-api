using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
namespace GovUk.Education.ManageCourses.Api.Services.Publish
{
    public interface IPublishService
    {
        Task<bool> PublishCourses(IList<Course> courses);
        Task<bool> PublishCourse(string instCode, string courseCode);
        Task<bool> PublishCourse(string instCode, string courseCode, string email);
    }
}
