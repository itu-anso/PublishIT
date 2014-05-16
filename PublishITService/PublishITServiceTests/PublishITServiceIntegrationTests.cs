using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublishITService;
using PublishITService.DTOs;

namespace PublishITServiceTests
{
    [TestClass]
    public class PublishITServiceIntegrationTests
    {
        private IPublishITService _service;
        private IPublishITEntities _entities;

        [TestInitialize]
        public void InitTests()
        {
            _entities = new RentIt09TestEntities();

            _service = new PublishITService.PublishITService(_entities);
        }

        [IntegrationTest]
        [TestMethod]
        public void SuccessfullyRegisterNewUser()
        {
            Assert.AreEqual(_service.GetUserByUserName("username 1").username, "No user found");

            InitTests();

            var responseMessage = _service.RegisterUser(new UserDTO
            {
                email = "email@email.com",
                name = "name 1",
                user_id = 1,
                username = "userName 1",
                password = "password 1",
                status = "Active",
                organization_id = 1,
            });

            Assert.AreEqual(responseMessage.Message, "User registered");

            Assert.IsTrue(responseMessage.IsExecuted);

            CleanUpDatabase();
        }

        public void CleanUpDatabase()
        {
            using(_entities)
            {
                var deleteUsers =
                    from u in _entities.user
                    select u;

                foreach (var u in deleteUsers)
                {
                    _entities.user.Remove(u);
                }

                try
                {
                    _entities.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

//        [IntegrationTest]
//        [TestMethod]
//        public void UnsuccessfullyEditingUsersInformationByChangingUserId()
//        {
//            var responseMessage = _publishITService.EditUser(new UserDTO
//            {
//                email = "ChangedEmail@email.com",
//                name = "ChangedName",
//                user_id = 1337,
//                username = "ChangedUserName 1",
//                password = "Changedpassword 1",
//                status = "Active",
//                organization_id = 1,
//                roles = new List<RoleDTO>
//                    {
//                        new RoleDTO
//                        {
//                            Id = 1,
//                            Title = "role 1"
//                        }
//                    }
//            });

//            Assert.IsFalse(responseMessage.IsExecuted);

//            Assert.AreEqual(responseMessage.Message, "Editing failed");
//        }
    }
}
