using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublishITService;
using PublishITService.Repositories;
using PublishITService.DTOs;

namespace PublishITServiceTests
{
    [TestClass]
    public class ServiceTests
    {
        private IPublishITService _publishITService;
        private TestRepository _repository;

        [TestInitialize]
        public void TestInit()
        {
            _repository = new TestRepository();

            SetupSets();

            _publishITService = new PublishITService.PublishITService(_repository);
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

            Assert.AreEqual(_publishITService.GetUserByUserName("newUserName").username, "newUserName");

            Assert.AreEqual(responseMessage.Message, "User registered");

            Assert.IsTrue(responseMessage.IsExecuted);
        }

        [TestMethod]
        public void UnsuccessfullyRegisterUserDueToExistingUserName()
        {
            Assert.AreEqual(_repository.FindUserByUsername("userName 1").username, "userName 1");

            var responseMessage = _publishITService.RegisterUser(new UserDTO
            {
                birthday = DateTime.MinValue,
                email = "newEmail@email.com",
                name = "newName",
                username = "userName 1",
                password = "newPassword",
                organization_id = 1
            });

            Assert.AreEqual(responseMessage.Message, "Username already exists");

            Assert.IsFalse(responseMessage.IsExecuted);
        }

        [TestMethod]
        public void UnsuccessfullyRegisterUserDueToEmptyUserDTO()
        {
            var responseMessage = _publishITService.RegisterUser(new UserDTO());

            Assert.IsFalse(responseMessage.IsExecuted);

            Assert.AreEqual(responseMessage.Message, "For registration to be performed Name, Username, Password, Email, Birthday and Organization id has to be added");
        }

        [TestMethod]
        public void DeleteUser()
        {
            var responseMessage = _publishITService.DeleteUser(1);

            Assert.IsTrue(responseMessage.IsExecuted);

            Assert.AreEqual(responseMessage.Message, "Deletion completed");
        }

        [TestMethod]
        public void UnsuccessfullyDeletingUserDoToUnknownUserId()
        {
            var responseMessage = _publishITService.DeleteUser(5);

            Assert.IsFalse(responseMessage.IsExecuted);

            Assert.AreEqual(responseMessage.Message, "No user found. Deletion failed");
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
        public void SuccessfullyStreamMedia()
        {
            var movie = _publishITService.StreamMovie(1, 4);

            Assert.AreEqual(movie, "<video width='320' heigth='240' controls>" +
                                        "<source src='location 4' type='video/mp4'>" +
                                        "<source='movie.ogg' type='video/ogg'>" +
                                    "</video>");
        }

        [TestMethod]
        public void UnsuccessfullyStreamMediaDoToNoFoundUserId()
        {
            var movie = _publishITService.StreamMovie(9, 0);

            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
        }

        [TestMethod]
        public void UnsuccessfullyStreamMediaDoToNoFoundMovieId()
        {
            var movie = _publishITService.StreamMovie(1, 9);

            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
        }

        [TestMethod]
        public void UnsuccessfullyStreamMediaDoToDateExpired()
        {
            var movie = _publishITService.StreamMovie(2, 1);

            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
        }

        [TestMethod]
        public void UnsuccessfullyStreamMediaDoToDateNotStarted()
        {
            var movie = _publishITService.StreamMovie(2, 0);

            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
        }



        private void SetupSets()
        {
            _repository._userSet = InitUserData();

            _repository._roleSet = InitRoleData();

            _repository._ratingSet = InitRatingData();

            _repository._mediaSet = InitMediaData();

            _repository._documentSet = InitDocumentData();

            _repository._videoSet = InitVideoData();

            _repository._rentSet = InitRentData();

            _repository._genreSet = InitGenreData();
        }

        private List<user> InitUserData()
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
            };
        }

        public List<role> InitRoleData()
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
            };
        }

        private List<rating> InitRatingData()
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
            };
        }

        private List<media> InitMediaData()
        {
            return new List<media>
            {
                new media
                {
                    media_id = 1,
                    user_id = 1,
                    format_id = 1,
                    title = "title 1",
                    location = "location 1"
                },
                
                new media
                {
                    media_id = 2,
                    user_id = 2,
                    format_id = 2,
                    title = "title 2",
                    location = "location 2"
                },

                new media
                {
                    media_id = 3,
                    user_id = 1,
                    format_id = 1,
                    title = "title 3",
                    location = "location 3"
                },

                new media
                {
                    media_id = 4,
                    user_id = 1,
                    format_id = 2,
                    title = "title 4",
                    location = "location 4"
                }

            };
        }

        private List<document> InitDocumentData()
        {
            return new List<document>
            {
                new document
                {
                    media_id = 1
                },

                new document
                {
                    media_id = 3
                }
            };
        }

        private List<video> InitVideoData()
        {
            return new List<video>
            {
                new video
                {
                    media_id = 2,
                },

                new video
                {
                    media_id = 4
                }

            };
        }

        private List<rent> InitRentData()
        {
            return new List<rent>
            {
                new rent
                {
                    rent_id = 1,
                    media_id = 0,
                    user_id = 1,
                    start_date = DateTime.MinValue,
                    end_date = DateTime.MaxValue
                },

                new rent
                {
                    rent_id = 2,
                    media_id = 1,
                    user_id = 2,
                    start_date = DateTime.MinValue,
                    end_date = DateTime.Parse("10-05-1991 00:00:00")
                },

                new rent
                {
                    rent_id = 3,
                    media_id = 0,
                    user_id = 2,
                    start_date = DateTime.Parse("10-05-2050 00:00:00"),
                    end_date = DateTime.MaxValue
                },

                new rent
                {
                    rent_id = 4,
                    media_id = 4,
                    user_id = 1,
                    start_date = DateTime.MinValue,
                    end_date = DateTime.MaxValue
                },
            };
        }

        private List<genre> InitGenreData()
        {
            return new List<genre>
            {
                new genre
                {
                    genre_id = 1,
                    genre1 = "Comedy",
                    media = new Collection<media>(new List<media>
                    {
                        new media
                        {
                            media_id = 2,
                            user_id = 2,
                            format_id = 2,
                            title = "title 2",
                            location = "location 2"
                        },

                        new media
                        {
                            media_id = 4,
                            user_id = 1,
                            format_id = 2,
                            title = "title 4",
                            location = "location 4"
                        }
                    })
                },

                new genre
                {
                    genre_id = 2,
                    genre1 = "Biography",
                    media = new Collection<media>(new List<media>
                    {
                        new media
                        {
                            media_id = 1,
                            user_id = 1,
                            format_id = 1,
                            title = "title 1",
                            location = "location 1"
                        }
                    })
                }
            };
        }
    }
}
