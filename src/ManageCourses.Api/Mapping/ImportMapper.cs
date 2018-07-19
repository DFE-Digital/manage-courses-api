using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public class ImportMapper
    {
        public IEnumerable<object> Additions { get; set; }
        public IEnumerable<object> Updates { get; set; }
        public IEnumerable<object> Deletes { get; set; }
    }
}
