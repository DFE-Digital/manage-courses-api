using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class UserInstitution
    {
        public McUser McUser { get; set; }
        public UcasInstitution UcasInstitution { get; set; }
    }
}
