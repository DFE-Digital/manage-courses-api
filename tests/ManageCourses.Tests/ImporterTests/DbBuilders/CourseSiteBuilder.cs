using System.Collections.Generic;
using System;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders
{
    internal class CourseSiteBuilder
    {
        private readonly CourseSite _courseSite;

        public CourseSiteBuilder()
        {
            _courseSite = new CourseSite();
            _courseSite.Publish = "N";
        }

        public static CourseSiteBuilder Build(CourseType courseType)
        {
            var result = new CourseSiteBuilder();

            switch (courseType)
            {
                case CourseType.NewUnpublished:
                {
                    result.Unpublished().WithNewStatus();
                    break;
                }
                case CourseType.NewPublished:
                {
                    result.Published().WithNewStatus();
                    break;
                }
                case CourseType.SuspensedUnpublished:
                {
                    result.Unpublished().WithSuspensedStatus();
                    break;
                }
                case CourseType.SuspensedPublished:
                {
                    result.Published().WithSuspensedStatus();
                    break;
                }
                case CourseType.RunningUnpublished:
                {
                    result.Unpublished().WithRunningStatus();
                    break;
                }
                case CourseType.RunningPublished:
                {
                    result.Published().WithRunningStatus();
                    break;
                }
                case CourseType.DiscontinuedUnpublished:
                {
                    result.Unpublished().WithDiscontinuedStatus();
                    break;
                }
                case CourseType.DiscontinuedPublished:
                {
                    result.Published().WithDiscontinuedStatus();
                    break;
                }
            }
            return result;
        }

        public CourseSiteBuilder Unpublished()
        {
            return Publish("N");

        }
        public CourseSiteBuilder Published()
        {
            return Publish();
        }
        private CourseSiteBuilder Publish(string publish = "Y")
        {
            _courseSite.Publish = publish;
            return this;
        }

        public CourseSiteBuilder Status(string status)
        {
            _courseSite.Status = status;
            return this;
        }

        public CourseSiteBuilder WithDiscontinuedStatus()
        {
            return this.Status("D");
        }
        public CourseSiteBuilder WithNewStatus()
        {
            return this.Status("N");
        }

        public CourseSiteBuilder WithSuspensedStatus()
        {
            return this.Status("S");
        }

        public CourseSiteBuilder WithRunningStatus()
        {
            return this.Status("R");
        }

        public static implicit operator CourseSite(CourseSiteBuilder builder)
        {
            return builder._courseSite;
        }
    }
}
