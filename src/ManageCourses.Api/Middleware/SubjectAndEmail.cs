namespace GovUk.Education.ManageCourses.Api.Middleware
{
    public class SubjectAndEmail
    {
        public string Subject { get; private set; }
        public string Email { get; private set; }

        public SubjectAndEmail(string subject, string email)
        {
            Subject = subject;
            Email = email;
        }
    }
}
