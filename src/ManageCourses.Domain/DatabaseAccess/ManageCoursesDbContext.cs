using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GovUk.Education.ManageCourses.Domain.DatabaseAccess {
    public class ManageCoursesDbContext : DbContext, IManageCoursesDbContext {
        public ManageCoursesDbContext (DbContextOptions<ManageCoursesDbContext> options) : base (options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Provider> Providers { get; set; }

        public IList<Course> GetAll () {

            return Courses.ToList();
        }
    }
}
