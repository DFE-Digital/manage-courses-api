using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public class ImportMapper<T>
    {
        public IEnumerable<T> Additions { get; set; }
        public IEnumerable<T> Updates { get; set; }
        public IEnumerable<T> Deletes { get; set; }
       }
}
