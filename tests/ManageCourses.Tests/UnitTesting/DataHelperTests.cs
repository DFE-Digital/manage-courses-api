using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.Enums;
using GovUk.Education.ManageCourses.Tests.UnitTesting.Helpers;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    class DataHelperTests
    {
        private ManageCoursesDbContext _dbContext;
        private IDataHelper _dataHelper;
        [OneTimeSetUp]
        public void Setup()
        {
            _dbContext = TestHelper.GetFakeData(EnumTestType.DataHelper);            
        }

        [Test]
        public void TestUserAdded()
        {
            var targetEmail = "addeduser@test.com";
            _dataHelper = new UserDataHelper();
            var fakeDataList = GetDataList();
            //now add one
            fakeDataList.Add(new McUser{Email = targetEmail, FirstName = "qwe", LastName = "qwe"});
            _dataHelper.Load(_dbContext, fakeDataList);
            var result = _dataHelper.Upsert();
            Assert.True(result.Success && result.NumberInserted == 1 && result.NumberDeleted == 0 && result.NumberUpdated == 0);
            Assert.True(DoesUserExist(targetEmail));
        }
        [Test]
        public void TestUserUpdate()
        {
            const string firstNameUpdate = "FirstNameUpdated";
            const string lastNameUpdate = "LastNameUpdated";
            _dataHelper = new UserDataHelper();
            var fakeDataList = GetDataList();
            //now get one to update
            var updatedUserEmail = fakeDataList[1].Email;
            fakeDataList[1].FirstName = firstNameUpdate;
            fakeDataList[1].LastName = lastNameUpdate;
            
            _dataHelper.Load(_dbContext, fakeDataList);
            var result = _dataHelper.Upsert();
            Assert.True(result.Success && result.NumberInserted == 0 && result.NumberDeleted == 0 && result.NumberUpdated == 1);

            var updatedUser = _dbContext.McUsers.FirstOrDefault(u => u.Email == updatedUserEmail);
            Assert.IsTrue(updatedUser != null && (updatedUser.FirstName == firstNameUpdate && updatedUser.LastName == lastNameUpdate));
        }
        [Test]
        public void TestUserRemoved()
        {
            _dataHelper = new UserDataHelper();
            var fakeDataList = GetDataList();
            //now remove one
            var userToRemove = fakeDataList.First();

            fakeDataList.Remove(userToRemove);
            _dataHelper.Load(_dbContext, fakeDataList);
            var result = _dataHelper.Upsert();
            Assert.True(result.Success && result.NumberInserted == 0 && result.NumberDeleted == 1 && result.NumberUpdated == 0);

            Assert.False(DoesUserExist(userToRemove.Email));
        }
        [Test]
        public void TestNoChange()
        {
            _dataHelper = new UserDataHelper();
            var fakeDataList = GetDataList();
            _dataHelper.Load(_dbContext, fakeDataList);
            var result = _dataHelper.Upsert();
            Assert.True(result.Success && result.NumberInserted == 0 && result.NumberDeleted == 0 && result.NumberUpdated == 0);

            Assert.True(fakeDataList.All(u => DoesUserExist(u.Email)));
        }

        private bool DoesUserExist(string email)
        {
            return (_dbContext.McUsers.FirstOrDefault(u => u.Email == email) != null);
        }
        /// <summary>
        /// Generates a test data list of users based on the fake users in the context
        /// </summary>
        /// <returns></returns>
        private List<McUser> GetDataList()
        {
            var returnList = _dbContext.McUsers.Select( u => new McUser{Email = u.Email, FirstName = u.FirstName, LastName = u.LastName}).ToList();
            return returnList;
        }
    }
}
