using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class AccessRequest
    {
        public int Id { get; set; }
        public DateTime RequestDateUtc { get; set; }
        public McUser Requester { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Organisation { get; set; }
        public string Reason { get; set; }
        public RequestStatus Status { get; set; }

        public enum RequestStatus 
        {
            Requested,
            Approved,
            Completed,
            Declined
        }
    }
}
