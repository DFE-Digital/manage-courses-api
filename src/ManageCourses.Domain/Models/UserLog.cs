using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UserLog
    {
        public int Id { get; set; }
        public DateTime FirstLoginDateUtc { get; set; }
        public int? UserId { get; set; }
        public McUser User { get; set; }
        public string UserEmail { get; set; }
        public DateTime LastLoginDateUtc { get; set; }
        public string SignInUserId { get; set; }
        public DateTime? WelcomeEmailDateUtc { get; set; }
    }
}
