namespace GovUk.Education.ManageCourses.Api.Services.Data
{
    public class UpsertResult
    {
        public int NumberInserted { get; set; }
        public int NumberDeleted { get; set; }
        public int NumberUpdated { get; set; }
        public bool Success { get; set; }
        public string errorMessage { get; set; }
    }
}
