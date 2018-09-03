using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Middleware;

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
        /// <returns></returns>
        Task UserSignedInAsync(string accessToken, JsonUserDetails userDetails);
    }
}
