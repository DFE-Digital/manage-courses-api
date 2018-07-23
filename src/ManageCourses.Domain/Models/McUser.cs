using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    /// <summary>
    /// Name/Email are set initially by us when we give a user access,
    /// then they are updated from DfE SignIn every time they
    /// access the service to keep it fresh.
    /// </summary>
    public class McUser
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        /// <summary>
        /// We use this to match a newly added user to their first sign-in,
        /// thereafter this is just a secondary cache of information updated
        /// from the DfE Sign-in system and <see cref="SignInUserId"/> becomes
        /// the key for matching signed in users to their data.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// When the user was sent the invite email
        /// </summary>
        public DateTime? InviteDateUtc { get; set; }

        /// GUID from DfE Sign-in uniquely identifying this user.
        /// We capture this the first time a user signs and then use
        /// it to match up the currently signed in user from then on.
        /// This allows us to cope with users changing their email address.
        /// </summary>
        public string SignInUserId { get; set; }

        public DateTime? FirstLoginDateUtc { get; set; }
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// When the user was sent the welcome email
        /// </summary>
        public DateTime? WelcomeEmailDateUtc { get; set; }

        public ICollection<McOrganisationUser> McOrganisationUsers { get; set; }
        public ICollection<AccessRequest> AccessRequests { get; set; }
    }
}
