using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PublishITService;

namespace PublishITServiceTests
{
    [TestClass]
    public class PublishITServiceTests
    {
        private PublishITService.PublishITService _publishITService;
        private Mock<IPublishITEntities> _publishITEntitiesMock;

        private Mock<IDbSet<user>> _userMockSet;
        private Mock<IDbSet<role>> _roleMockSet;
        private Mock<IDbSet<rating>> _ratingMockSet;
            
        [TestInitialize]
        public void InitTests()
        {
            _userMockSet = new Mock<IDbSet<user>>();
            SetupUserMockSet(InitUserData());

            _roleMockSet = new Mock<IDbSet<role>>();
            SetupRoleMockSet(InitRoleData());

            _ratingMockSet = new Mock<IDbSet<rating>>();
            SetupRatingMockSet(InitRatingData());

            _publishITEntitiesMock = new Mock<IPublishITEntities>();
            _publishITEntitiesMock.Setup(call => call.user).Returns(_userMockSet.Object);
            _publishITEntitiesMock.Setup(call => call.role).Returns(_roleMockSet.Object);
            _publishITEntitiesMock.Setup(call => call.rating).Returns(_ratingMockSet.Object);

            _publishITService = new PublishITService.PublishITService(_publishITEntitiesMock.Object);
        }

        [TestMethod]
        public void SuccessfullyGettingUserById()
        {
            var user = _publishITService.GetUserById(1);

            Assert.AreEqual(user.user_id, 1);

            Assert.AreEqual(user.username, "userName 1");
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserById()
        {
            var user = _publishITService.GetUserById(1337);

            Assert.AreEqual(user.name, "No user found");

            Assert.AreEqual(user.username, null);

            Assert.AreEqual(user.user_id, 0);
        }

        [TestMethod]
        public void SuccessfullySigningIn()
        {
            var user = _publishITService.SignIn("userName 1", "password 1");

            Assert.AreEqual(user.user_id, 1);
        }

        [TestMethod]
        public void UnsuccessfullySigningIn()
        {
            var user = _publishITService.SignIn("Not existing userName", "Some password");

            Assert.AreEqual(user.name, "Sign in failed");

            Assert.AreEqual(user.user_id, 0);
        }

        [TestMethod]
        public void SuccessfullyGettingUserByName()
        {
            var user = _publishITService.GetUserByUserName("userName 1");

            Assert.AreEqual(user.user_id, 1);

            Assert.AreEqual(user.username, "userName 1");
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserByUserName()
        {
            var user = _publishITService.GetUserByUserName("Not existing userName");

            Assert.AreEqual(user.username, "No user found");

            Assert.AreEqual(user.user_id, 0);
        }

        [TestMethod]
        public void SuccessfullyRegisterUser()
        {
            Assert.AreNotEqual(_publishITService.GetUserByUserName("newUserName").username, "newUserName");
            
            var responseMessage = _publishITService.RegisterUser(new UserDTO
            {
                birthday = DateTime.MinValue,
                email = "newEmail@email.com",
                name = "newName",
                username = "newUserName",
                password = "newPassword",
                organization_id = 1
            });

            _userMockSet.Verify(x => x.Add(It.Is<user>(
                                                        newUser => newUser.birthday == DateTime.MinValue &&
                                                        newUser.email.Equals("newEmail@email.com") && 
                                                        newUser.name.Equals("newName") && 
                                                        newUser.user_name.Equals("newUserName") && 
                                                        newUser.password.Equals("newPassword") && 
                                                        newUser.organization_id == 1)), 
                                                        Times.Once
                                                        );

            //Assert.AreEqual(responseMessage.Message, "User registered");

            //Assert.IsTrue(responseMessage.IsExecuted);
        }

        [TestMethod]
        public void UnsuccessfullyRegisterUserDueToExistingUserName()
        {
            Assert.AreEqual(_publishITService.GetUserByUserName("userName 1").username, "userName 1");

            var responseMessage = _publishITService.RegisterUser(new UserDTO
            {
                birthday = DateTime.MinValue,
                email = "newEmail@email.com",
                name = "newName",
                username = "userName 1",
                password = "newPassword",
                organization_id = 1
            });

            _userMockSet.Verify(x => x.Add(It.Is<user>(
                                                        newUser => newUser.birthday == DateTime.MinValue && 
                                                        newUser.email.Equals("newEmail@email.com") && 
                                                        newUser.name.Equals("newName") && 
                                                        newUser.user_name.Equals("userName 1") && 
                                                        newUser.password.Equals("newPassword") && 
                                                        newUser.organization_id == 1)), 
                                                        Times.Never
                                                        );

            Assert.AreEqual(responseMessage.Message, "Username already exists");

            Assert.IsFalse(responseMessage.IsExecuted);
        }

        [TestMethod]
        public void UnsuccessfullyRegisterUserDueToSomeFailWithRegistration()
        {
            Assert.AreEqual(1,2);
        }

        [TestMethod]
        public void SuccessfullyDeletingUser()
        {
            var responseMessage = _publishITService.DeleteUser(new UserDTO
            {
                user_id = 1
            });

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Once);

            Assert.IsTrue(responseMessage.IsExecuted);

            Assert.AreEqual(responseMessage.Message, "Deletion completed");
        }

        [TestMethod]
        public void UnsuccessfullyDeletingUserDoToUnknownUserId()
        {
            var responseMessage = _publishITService.DeleteUser(new UserDTO
            {
                user_id = 5
            });

            Assert.IsFalse(responseMessage.IsExecuted);

            Assert.AreEqual(responseMessage.Message, "Deletion failed");
        }

        [TestMethod]
        public void SuccessfullyEditingUsersInformation()
        {
            var responseMessage = _publishITService.EditUser(new UserDTO
            {
                    birthday = DateTime.Now,
                    email = "ChangedEmail@email.com",
                    name = "ChangedName",
                    user_id = 1,
                    username = "ChangedUserName 1",
                    password = "Changedpassword 1",
                    status = "Active",
                    organization_id = 1,
                    roles = new List<RoleDTO>
                    {
                        new RoleDTO
                        {
                            Id = 1,
                            Title = "role 1"
                        }
                    }
            });

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Once);

            Assert.IsTrue(responseMessage.IsExecuted);

            Assert.AreEqual(responseMessage.Message, "User edited");
        }

        [TestMethod]
        public void UnsuccessfullyEditingUsersInformationByChangingUserId()
        {
            var responseMessage = _publishITService.EditUser(new UserDTO
            {
                email = "ChangedEmail@email.com",
                name = "ChangedName",
                user_id = 1337,
                username = "ChangedUserName 1",
                password = "Changedpassword 1",
                status = "Active",
                organization_id = 1,
                roles = new List<RoleDTO>
                    {
                        new RoleDTO
                        {
                            Id = 1,
                            Title = "role 1"
                        }
                    }
            });

            Assert.IsFalse(responseMessage.IsExecuted);

            Assert.AreEqual(responseMessage.Message, "Editing failed");
        }

        [TestMethod]
        public void SuccessfullyGettingRating()
        {
            var rating = _publishITService.GetRating(1, 1);

            Assert.AreEqual(rating, 5);
        }


        [TestMethod]
        public void UnsuccessfullyGettingRating()
        {
            var rating = _publishITService.GetRating(80085, 666);

            Assert.AreEqual(rating, -1);
        }

        [TestMethod]
        public void SuccessfullyPostingRating()
        {
            var posted = _publishITService.PostRating(1, 1, 1);

            Assert.IsTrue(posted);
        }

        public void UnsuccessfullyPostingRating()
        {
            Assert.AreEqual(1,2);
        }

        private IQueryable<user> InitUserData()
        {
            return new List<user>
            {
                new user
                {
                    email = "email@email.com",
                    name = "name 1",
                    user_id = 1,
                    user_name = "userName 1",
                    password = "password 1",
                    status = "Active",
                    organization_id = 1,
                    role = new Collection<role>(new List<role>
                    {
                        new role {role_id = 1, role1 = "role 1"}
                    })
                },
                new user
                {
                    email = "email2@email.com",
                    name = "name 2",
                    user_id = 2,
                    user_name = "userName 2",
                    password = "password 2",
                    status = "Active",
                    organization_id = 2,
                    role = new Collection<role>(new List<role>
                    {
                        new role {role_id = 1, role1 = "role 1"},
                        new role {role_id = 2, role1 = "role 2"}
                    })
                },
                new user
                {
                    email = "email3@email.com",
                    name = "name 3",
                    user_id = 3,
                    user_name = "userName 3",
                    password = "password 3",
                    status = "Deleted",
                    organization_id = 1,
                    role = new Collection<role>(new List<role>
                    {
                        new role {role_id = 1, role1 = "role 1"}
                    })
                }
            }.AsQueryable();
        }

        public IQueryable<role> InitRoleData()
        {
            return new List<role>
            {
                new role
                {
                    role_id = 1,
                    role1 = "role 1",
                    user = new Collection<user>(new List<user>
                    {
                        new user
                        {
                            user_id = 1
                        },
                        new user
                        {
                            user_id = 2
                        },
                        new user
                        {
                            user_id = 3
                        }
                    })
                },
                
                new role
                {
                    role_id = 2,
                    role1 = "role 2"
                }
            }.AsQueryable();
        }

        private IQueryable<rating> InitRatingData()
        {
            return new List<rating>
            {
                new rating
                {
                    rating_id = 1,
                    rating1 = 5,
                    media_id = 1,
                    user_id = 1
                },
                new rating
                {
                    rating_id = 2,
                    rating1 = 7,
                    media_id = 2,
                    user_id = 1
                },
                new rating
                {
                    rating_id = 3,
                    rating1 = 1,
                    media_id = 1,
                    user_id = 2
                }
            }.AsQueryable();
        }

        private void SetupUserMockSet(IQueryable<user> data)
        {
            _userMockSet.As<IQueryable<user>>().Setup(m => m.Provider).Returns(data.Provider);
            _userMockSet.As<IQueryable<user>>().Setup(m => m.Expression).Returns(data.Expression);
            _userMockSet.As<IQueryable<user>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _userMockSet.As<IQueryable<user>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupRoleMockSet(IQueryable<role> data)
        {
            _roleMockSet.As<IQueryable<role>>().Setup(m => m.Provider).Returns(data.Provider);
            _roleMockSet.As<IQueryable<role>>().Setup(m => m.Expression).Returns(data.Expression);
            _roleMockSet.As<IQueryable<role>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _roleMockSet.As<IQueryable<role>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupRatingMockSet(IQueryable<rating> data)
        {
            _ratingMockSet.As<IQueryable<rating>>().Setup(m => m.Provider).Returns(data.Provider);
            _ratingMockSet.As<IQueryable<rating>>().Setup(m => m.Expression).Returns(data.Expression);
            _ratingMockSet.As<IQueryable<rating>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _ratingMockSet.As<IQueryable<rating>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

    }
}
