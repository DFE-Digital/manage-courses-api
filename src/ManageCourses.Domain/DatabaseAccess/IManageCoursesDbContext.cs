using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Domain.DatabaseAccess
{
    public interface IManageCoursesDbContext
    {
        DbSet<Course> Courses { get; set; }
        IList<Course> GetAll();
        void AddCourse(Course course);
        void Save();
    }
}
