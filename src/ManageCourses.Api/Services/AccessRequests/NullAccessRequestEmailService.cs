﻿using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public class NullAccessRequestEmailService : IAccessRequestEmailService
    {
        public void SendAccessRequestEmailToSupport(AccessRequest accessRequest, User requester, User requestedOrNull)
        {
            return;
        }
    }
}