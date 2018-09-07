using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Exceptions;
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
        /// Call this when a user signs in.
        /// Implementation will handle workflow actions
        /// that need to happen off the back of this.
        /// </summary>
        /// <param name="accessToken">The OAuth AccessToken</param>
        /// <param name="userDetails">Details from DfE Sign-in</param>
        /// <returns>The McUser record for the signed in user</returns>
        /// <exception cref="McUserNotFoundException">Thrown if couldn't find user by subjectId or email address</exception>
        Task<McUser> UserSignedInAsync(string accessToken, JsonUserDetails userDetails);
    }
}
