using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Data
{
    /// <summary>
    /// This helper class generates the diff between the existing and new dataset.
    /// Then implements the upsert
    /// </summary>
    public class UserDataHelper
    {
        private readonly IManageCoursesDbContext _context;
        private readonly IReadOnlyCollection<McUser> _dataList;
        private ImportMapper<McUser> _mapper;
        /// <summary>
        /// constructor for the class. Initialises the dataset and context.
        /// Generates the diff (additions, updates, deletions)
        /// </summary>
        /// <param name="context">context for the db</param>
        /// <param name="dataList">new dataset coming from the importer</param>
        public UserDataHelper(IManageCoursesDbContext context, IReadOnlyCollection<McUser> dataList)
        {
            _context = context;
            _dataList = dataList;
            GenerateDiff();
        }
        /// <summary>
        /// Actions the additions, deletions, updates
        /// </summary>
        /// <returns></returns>
        public bool Upsert()
        {
            var returnBool = false;
            try
            {
                _context.McUsers.AddRange(_mapper.Additions);
                _context.McUsers.RemoveRange(_mapper.Deletes);
                DoUpdates();
                _context.Save();
                returnBool = true;
            }
            catch (Exception e)
            {
                //write to error log?
            }
            return returnBool;
        }
        private McUser GetDbRecord(McUser record)
        {
            return _context.McUsers.FirstOrDefault(u => u.Email == record.Email);
        }

        private bool IsUpdated(McUser dbRecord, McUser importRecord)
        {
            return (dbRecord.FirstName != importRecord.FirstName || dbRecord.LastName != importRecord.LastName);
        }

        private void DoUpdates()
        {
            foreach (var user in _mapper.Updates)
            {
                var dbRecord = GetDbRecord(user);
                if (dbRecord == null) continue;
                dbRecord.Email = user.Email;
                dbRecord.FirstName = user.FirstName;
                dbRecord.LastName = user.LastName;
            }
        }
        private void GenerateDiff()
        {
            _mapper = new ImportMapper<McUser>();
            var additions = new List<McUser>();
            var updates = new List<McUser>();

            foreach (var record in _dataList)
            {
                var dbRecord = GetDbRecord(record);
                if (dbRecord == null)
                {
                    additions.Add(record);
                }
                else
                {
                    if (IsUpdated(dbRecord, record))
                    {
                        updates.Add(record);
                    }

                }
            }

            var deletes = _context.McUsers.Where(dbRecord => _dataList.All(c => c.Email.ToLower().Trim() != dbRecord.Email.ToLower().Trim())).ToList();

            _mapper.Additions = additions;
            _mapper.Updates = updates;
            _mapper.Deletes = deletes;
        }
    }
}
