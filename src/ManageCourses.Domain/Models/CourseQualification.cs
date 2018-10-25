namespace GovUk.Education.ManageCourses.Domain.Models
{
    public enum CourseQualification
    {
        /// <summary>
        /// Qualified teacher status
        /// </summary>
        Qts,

        /// <summary>
        /// Qualified teacher status with postgraduate certificate of education
        /// </summary>
        QtsWithPgce,

        /// <summary>
        /// Qualified teacher status with postgraduate diploma in education
        /// </summary>
        QtsWithPgde,

        /// <summary>
        /// Postgraduate certificate of education only
        /// </summary>
        QtlsWithPgce,

        /// <summary>
        /// Postgraduate diploma in education only
        /// </summary>        
        QtlsWithPgde
    }
}
