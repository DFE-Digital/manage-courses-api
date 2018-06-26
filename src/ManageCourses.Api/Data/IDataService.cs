using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IDataService
    {
        void ResetDatabase();
        void ProcessPayload(Payload payload);
        IEnumerable<Model.Course> GetCoursesForUser(string email);
    }
}
