﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services.Publish
{
    public interface ICourseMapper
    {
        SearchAndCompare.Domain.Models.Course MapToSearchAndCompareCourse(UcasInstitution ucasInstData, Course ucasCourseData, InstitutionEnrichmentModel orgEnrichmentModel, CourseEnrichmentModel courseEnrichmentModel);
    }
}