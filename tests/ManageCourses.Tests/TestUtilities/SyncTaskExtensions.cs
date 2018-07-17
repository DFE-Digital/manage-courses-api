using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Tests.TestUtilities
{
    public static class SyncTaskExtensions
    {
        public static T Await<T>(this Task<T> task) {
            task.Wait();
            return task.Result;
        }

        public static void Await(this Task task) {
            task.Wait();
        }
    }
}