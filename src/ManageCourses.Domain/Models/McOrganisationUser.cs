﻿using System.Diagnostics;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class McOrganisationUser
    {
        public int Id { get; set; }

        public McUser McUser { get; set; }

        public McOrganisation McOrganisation { get; set; }

        public string DebuggerDisplay => $"McOrganisationUser: OrgId {McOrganisation?.Id} <--> User {McUser?.Email}";
    }
}
