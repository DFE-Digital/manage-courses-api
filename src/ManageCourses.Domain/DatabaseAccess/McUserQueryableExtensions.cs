using System;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Domain.DatabaseAccess
{
    public static class McUserQueryableExtensions
    {
        /// <summary>
        /// Case insensitive filter for users by email address.
        /// This should be used for all lookups by email even for OrganisationUser to keep logic consistent.
        /// Follow this with .SingleOrDefault to get an actuall McUser object (don't forget to check for nulls!).
        /// </summary>
        public static IQueryable<McUser> ByEmail(this IQueryable<McUser> mcUsers, string email)
        {
            return mcUsers.Where(user => string.Equals(user.Email, email, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
