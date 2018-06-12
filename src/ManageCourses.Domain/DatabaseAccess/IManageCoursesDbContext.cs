using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Domain.DatabaseAccess {
    public interface IManageCoursesDbContext {
        IList<Course> GetAll ();
        void AddCourse(Course course);
        void Save();
    }
}
