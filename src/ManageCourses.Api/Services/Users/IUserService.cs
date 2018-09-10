using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services.Users
{
    /// <summary>
    /// Encapsulate logic for handling user interactions
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Get the user from the claims.
        /// Updates stored user details with updated details in claims.
        /// </summary>
        Task<McUser> GetAndUpdateUserAsync(JsonUserDetails userDetails);

        /// <summary>
        /// Call this when a user signs in.
        /// This updates the login timestamps in the database
        /// and sends welcome email if not already sent.
        /// </summary>
        /// <param name="user">The user to be updated</param>
        Task LoggedInAsync(McUser user);

        /// <summary>
        /// Save a copy of the access token in the cache
        /// </summary>
        Task CacheTokenAsync(string accessToken, McUser mcUser);

        /// <summary>
        /// Figure out who this is based on the cached access token to avoid
        /// unnecessary round-trips to DfE Sign-in claims endpoint.
        /// </summary>
        Task<McUser> GetFromCacheAsync(string accessToken);
    }
}
