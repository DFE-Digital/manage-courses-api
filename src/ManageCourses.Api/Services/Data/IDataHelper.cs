using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services.Data
{
    public interface IDataHelper
    {
        void Load(IManageCoursesDbContext context, IReadOnlyCollection<McUser> dataList);
        UpsertResult Upsert();
    }
}
