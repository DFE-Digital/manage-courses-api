using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IDataHelper
    {
        void Load(IManageCoursesDbContext context, IReadOnlyCollection<McUser> dataList);
        UpsertResult Upsert();
    }
}
