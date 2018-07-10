using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UserLog
    {
        public int Id { get; set; }
        public DateTime FirstLoginDateUtc { get; set; }
        public McUser User { get; set; }
        public DateTime LastLoginDateUtc { get; set; }
        public string SignInUserId { get; set; }
    }
}
