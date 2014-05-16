﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PublishITService;
using PublishITService.DTOs;

namespace PublishITServiceTests
{
    [TestClass]
    public class PublishITServiceUnitTests
    {
        private PublishITService.PublishITService _publishITService;
        private Mock<IPublishITEntities> _publishITEntitiesMock;

        private Mock<IDbSet<PublishITService.user>> _userMockSet;
        private Mock<IDbSet<PublishITService.role>> _roleMockSet;
        private Mock<IDbSet<PublishITService.rating>> _ratingMockSet;
        private Mock<IDbSet<PublishITService.media>> _mediaMockSet;
        private Mock<IDbSet<PublishITService.document>> _documentMockSet;
        private Mock<IDbSet<PublishITService.rent>> _rentMockSet;
        private Mock<IDbSet<PublishITService.video>> _videoMockSet;
        private Mock<IDbSet<PublishITService.genre>> _genreMockSet;

        [TestInitialize]
        public void InitTests()
        {
            SetupMockSets();

            SetupEntitiesReturnValue();


            _publishITService = new PublishITService.PublishITService(_publishITEntitiesMock.Object);
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullyGettingUserById()
        {
            var user = _publishITService.GetUserById(1);

            Assert.AreEqual(user.user_id, 1);

            Assert.AreEqual(user.username, "userName 1");
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyGettingUserById()
        {
            var foundUser = _publishITService.GetUserById(1337);

            Assert.AreEqual(foundUser.username, "No user found");

            Assert.AreEqual(foundUser.name, "");

            Assert.AreEqual(foundUser.user_id, 0);
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullySigningIn()
        {
            var user = _publishITService.SignIn("userName 1", "password 1");

            Assert.AreEqual(user.user_id, 1);
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullySigningIn()
        {
            var user = _publishITService.SignIn("Not existing userName", "Some password");

            Assert.AreEqual(user.name, "Sign in failed");

            Assert.AreEqual(user.user_id, 0);
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullyGettingUserByName()
        {
            var user = _publishITService.GetUserByUserName("userName 1");

            Assert.AreEqual(user.user_id, 1);

            Assert.AreEqual(user.username, "userName 1");
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyGettingUserByUserName()
        {
            var user = _publishITService.GetUserByUserName("Not existing userName");

            Assert.AreEqual(user.username, "No user found");

            Assert.AreEqual(user.user_id, 0);
        }

        [UnitTest]
        [TestMethod]
        public void CheckingIfRegisterUserMethodSavesChangesOnceWhenAttemptingToAddNewUser()
        {
            Assert.AreNotEqual(_publishITService.GetUserByUserName("newUserName").username, "newUserName");
            
            _publishITService.RegisterUser(new UserDTO
            {
                birthday = DateTime.MinValue,
                email = "newEmail@email.com",
                name = "newName",
                username = "newUserName",
                password = "newPassword",
                organization_id = 1
            });


            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [UnitTest]
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

            _userMockSet.Verify(x => x.Add(It.Is<PublishITService.user>(
                                                        newUser => 
                                                        newUser.birthday == DateTime.MinValue && 
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

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyRegisterUserDueToEmptyUserDTO()
        {
            var responseMessage = _publishITService.RegisterUser(new UserDTO());

            Assert.IsFalse(responseMessage.IsExecuted);

            Assert.AreEqual(responseMessage.Message, "Registration failed");
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullyDeletingUser()
        {
            var responseMessage = _publishITService.DeleteUser(1);

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Once);

            Assert.IsTrue(responseMessage.IsExecuted);

            Assert.AreEqual(responseMessage.Message, "Deletion completed");
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyDeletingUserDoToUnknownUserId()
        {
            var responseMessage = _publishITService.DeleteUser(5);

            Assert.IsFalse(responseMessage.IsExecuted);

            Assert.AreEqual(responseMessage.Message, "Deletion failed");
        }

        [UnitTest]
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

        [UnitTest]
        [TestMethod]
        public void SuccessfullyUploadingDocument()
        {
            var remoteFileInfo = new RemoteFileInfo
            {
                FileName = "filename.pdf",
                FileStream = new MemoryStream(),
                Title = "title",
                GenreId = 1,
                Length = 1,
                Status = "status",
                UserId = 1
            };

            _publishITService.UploadMedia(remoteFileInfo);

            _mediaMockSet.Verify(x => x.Add(It.Is<PublishITService.media>(
                                            newMedia =>
                                            newMedia.title.Equals("title") && 
                                            newMedia.user_id == 1)),
                                            Times.Once
                                            );

            _documentMockSet.Verify(x => x.Add(It.Is<PublishITService.document>(
                                            newDocument =>
                                            newDocument.media_id == 0 &&
                                            newDocument.status.Equals("status"))), 
                                            Times.Once
                                            );
            
            _videoMockSet.Verify(x => x.Add(It.Is<PublishITService.video>(
                                            newVideo =>
                                            newVideo.media_id == 0)),
                                            Times.Never
                                            );

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Exactly(2));
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullyUploadingVideo()
        {
            var remoteFileInfo = new RemoteFileInfo
            {
                FileName = "filename.mp4",
                FileStream = new MemoryStream(),
                Title = "title",
                GenreId = 1,
                Length = 1,
                Status = "status",
                UserId = 1
            };

            _publishITService.UploadMedia(remoteFileInfo);

            _mediaMockSet.Verify(x => x.Add(It.Is<PublishITService.media>(
                                            newMedia =>
                                            newMedia.title.Equals("title") &&
                                            newMedia.user_id == 1)),
                                            Times.Once
                                            );

            _videoMockSet.Verify(x => x.Add(It.Is<PublishITService.video>(
                                            newVideo =>
                                            newVideo.media_id == 0)),
                                            Times.Once
                                            );

            _documentMockSet.Verify(x => x.Add(It.Is<PublishITService.document>(
                                            newDocument =>
                                            newDocument.media_id == 0 &&
                                            newDocument.status.Equals("status"))),
                                            Times.Never
                                            );

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Exactly(2));
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyUploadingMediaDoToUnknownFileFormat()
        {
            var remoteFileInfo = new RemoteFileInfo
            {
                FileName = "filename.mp3",
                FileStream = new MemoryStream(),
                Title = "title",
                GenreId = 1,
                Length = 1,
                Status = "status",
                UserId = 1
            };

            _publishITService.UploadMedia(remoteFileInfo);

            _mediaMockSet.Verify(x => x.Add(It.Is<PublishITService.media>(
                                            newMedia =>
                                            newMedia.title.Equals("title") &&
                                            newMedia.user_id == 1)),
                                            Times.Never
                                            );

            _videoMockSet.Verify(x => x.Add(It.Is<PublishITService.video>(
                                            newVideo =>
                                            newVideo.media_id == 0)),
                                            Times.Never
                                            );

            _documentMockSet.Verify(x => x.Add(It.Is<PublishITService.document>(
                                            newDocument =>
                                            newDocument.media_id == 0 &&
                                            newDocument.status.Equals("status"))),
                                            Times.Never
                                            );

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Never);
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullyDownloadMedia()
        {
            //var downloadedMedia = _publishITService.DownloadMedia(1);
            Assert.AreEqual(1, 2);
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyDownloadMedia()
        {
            Assert.AreEqual(1, 2);
        }

        [TestMethod]
        public void SuccessfullyStreamMedia()
        {
            var movie = _publishITService.StreamMovie(1,4);

            Assert.AreEqual(movie, "<video width='320' heigth='240' controls>" +
                                        "<source src='location 4' type='video/mp4'>" +
                                        "<source='movie.ogg' type='video/ogg'>" +
                                    "</video>");
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyStreamMediaDoToNoFoundUserId()
        {
            var movie = _publishITService.StreamMovie(9, 0);

            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyStreamMediaDoToNoFoundMovieId()
        {
            var movie = _publishITService.StreamMovie(1, 9);

            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyStreamMediaDoToDateExpired()
        {
            var movie = _publishITService.StreamMovie(2, 1);

            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyStreamMediaDoToDateNotStarted()
        {
            var movie = _publishITService.StreamMovie(2, 0);

            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullySearchMedia()
        {
            var listOfMedia = _publishITService.SearchMedia("title");

            Assert.AreEqual(listOfMedia.Count, 4);

            Assert.AreEqual(listOfMedia[0].title, "title 1");
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullySearchMedia()
        {
            var listOfMedia = _publishITService.SearchMedia("No media");

            Assert.AreEqual(listOfMedia.Count, 0);
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullyGettingMoviesByGenre()
        {
            var movies = _publishITService.GetMediaByGenre("Comedy");

            Assert.AreEqual(movies.Count, 2);

            Assert.AreEqual(movies[0].title, "title 2");
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyGettingMoviesByGenreDoToNoMoviesInTheGenre()
        {
            var movies = _publishITService.GetMediaByGenre("Science Fiction");

            Assert.AreEqual(movies.Count, 0);
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullyGettingMedia()
        {
            var gottenMedia = _publishITService.GetMedia(1);

            Assert.AreEqual(gottenMedia.title, "title 1");
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyGettingMedia()
        {
            var gottenMedia = _publishITService.GetMedia(5);

            Assert.AreEqual(gottenMedia.title, "No media found");
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullyGettingRating()
        {
            var rating = _publishITService.GetRating(1, 1);

            Assert.AreEqual(rating, 5);
        }

        [UnitTest]
        [TestMethod]
        public void UnsuccessfullyGettingRating()
        {
            var rating = _publishITService.GetRating(80085, 666);

            Assert.AreEqual(rating, -1);
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullyPostingNewRating()
        {
            var posted = _publishITService.PostRating(3, 4, 1);

            _ratingMockSet.Verify(x =>x.Add(It.Is<PublishITService.rating>(newRating => 
                                                            newRating.media_id == 4 && 
                                                            newRating.rating1 == 3 && 
                                                            newRating.user_id == 1 &&
                                                            newRating.rating_id == 4)),
                                                            Times.Once);

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Once);

            Assert.IsTrue(posted.IsExecuted);

            Assert.AreEqual(posted.Message, "Rating added");
        }

        [UnitTest]
        [TestMethod]
        public void SuccessfullyChangingRating()
        {
            var posted = _publishITService.PostRating(1, 2, 1);

            _ratingMockSet.Verify(x => x.Add(It.Is<PublishITService.rating>(newRating =>
                                                            newRating.media_id == 2 &&
                                                            newRating.rating1 == 1 &&
                                                            newRating.user_id == 1 &&
                                                            newRating.rating_id == 4)),
                                                            Times.Never);

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Once);

            Assert.IsTrue(posted.IsExecuted);

            Assert.AreEqual(posted.Message, "Rating changed");
        }


        private IQueryable<PublishITService.user> InitUserData()
        {
            return new List<PublishITService.user>
            {
                new PublishITService.user
                {
                    email = "email@email.com",
                    name = "name 1",
                    user_id = 1,
                    user_name = "userName 1",
                    password = "password 1",
                    status = "Active",
                    organization_id = 1,
                    role = new Collection<PublishITService.role>(new List<PublishITService.role>
                    {
                        new PublishITService.role {role_id = 1, role1 = "role 1"}
                    })
                },
                new PublishITService.user
                {
                    email = "email2@email.com",
                    name = "name 2",
                    user_id = 2,
                    user_name = "userName 2",
                    password = "password 2",
                    status = "Active",
                    organization_id = 2,
                    role = new Collection<PublishITService.role>(new List<PublishITService.role>
                    {
                        new PublishITService.role {role_id = 1, role1 = "role 1"},
                        new PublishITService.role {role_id = 2, role1 = "role 2"}
                    })
                },
                new PublishITService.user
                {
                    email = "email3@email.com",
                    name = "name 3",
                    user_id = 3,
                    user_name = "userName 3",
                    password = "password 3",
                    status = "Deleted",
                    organization_id = 1,
                    role = new Collection<PublishITService.role>(new List<PublishITService.role>
                    {
                        new PublishITService.role {role_id = 1, role1 = "role 1"}
                    })
                }
            }.AsQueryable();
        }

        public IQueryable<PublishITService.role> InitRoleData()
        {
            return new List<PublishITService.role>
            {
                new PublishITService.role
                {
                    role_id = 1,
                    role1 = "role 1",
                    user = new Collection<PublishITService.user>(new List<PublishITService.user>
                    {
                        new PublishITService.user
                        {
                            user_id = 1
                        },
                        new PublishITService.user
                        {
                            user_id = 2
                        },
                        new PublishITService.user
                        {
                            user_id = 3
                        }
                    })
                },
                
                new PublishITService.role
                {
                    role_id = 2,
                    role1 = "role 2"
                }
            }.AsQueryable();
        }

        private IQueryable<PublishITService.rating> InitRatingData()
        {
            return new List<PublishITService.rating>
            {
                new PublishITService.rating
                {
                    rating_id = 1,
                    rating1 = 5,
                    media_id = 1,
                    user_id = 1
                },
                new PublishITService.rating
                {
                    rating_id = 2,
                    rating1 = 7,
                    media_id = 2,
                    user_id = 1
                },
                new PublishITService.rating
                {
                    rating_id = 3,
                    rating1 = 1,
                    media_id = 1,
                    user_id = 2
                }
            }.AsQueryable();
        }

        private IQueryable<PublishITService.media> InitMediaData()
        {
            return new List<PublishITService.media>
            {
                new PublishITService.media
                {
                    media_id = 1,
                    user_id = 1,
                    format_id = 1,
                    title = "title 1",
                    location = "location 1"
                },
                
                new PublishITService.media
                {
                    media_id = 2,
                    user_id = 2,
                    format_id = 2,
                    title = "title 2",
                    location = "location 2"
                },

                new PublishITService.media
                {
                    media_id = 3,
                    user_id = 1,
                    format_id = 1,
                    title = "title 3",
                    location = "location 3"
                },

                new PublishITService.media
                {
                    media_id = 4,
                    user_id = 1,
                    format_id = 2,
                    title = "title 4",
                    location = "location 4"
                }

            }.AsQueryable();
        }

        private IQueryable<PublishITService.genre> InitGenreData()
        {
            return new List<PublishITService.genre>
            {
                new PublishITService.genre
                {
                    genre_id = 1,
                    genre1 = "Comedy",
                    media = new Collection<PublishITService.media>(new List<PublishITService.media>
                    {
                        new PublishITService.media
                        {
                            media_id = 2,
                            user_id = 2,
                            format_id = 2,
                            title = "title 2",
                            location = "location 2"
                        },

                        new PublishITService.media
                        {
                            media_id = 4,
                            user_id = 1,
                            format_id = 2,
                            title = "title 4",
                            location = "location 4"
                        }
                    })
                },

                new PublishITService.genre
                {
                    genre_id = 2,
                    genre1 = "Biography",
                    media = new Collection<PublishITService.media>(new List<PublishITService.media>
                    {
                        new PublishITService.media
                        {
                            media_id = 1,
                            user_id = 1,
                            format_id = 1,
                            title = "title 1",
                            location = "location 1"
                        }
                    })
                }
            }.AsQueryable();
        }

        private IQueryable<PublishITService.document> InitDocumentData()
        {
            return new List<PublishITService.document>
            {
                new PublishITService.document
                {
                    media_id = 1
                },

                new PublishITService.document
                {
                    media_id = 3
                }
            }.AsQueryable();
        }

        private IQueryable<PublishITService.video> InitVideoData()
        {
            return new List<PublishITService.video>
            {
                new PublishITService.video
                {
                    media_id = 2,
                },

                new PublishITService.video
                {
                    media_id = 4
                }

            }.AsQueryable();
        }

        private IQueryable<PublishITService.rent> InitRentData()
        {
            return new List<PublishITService.rent>
            {
                new PublishITService.rent
                {
                    rent_id = 1,
                    media_id = 0,
                    user_id = 1,
                    start_date = DateTime.MinValue,
                    end_date = DateTime.MaxValue
                },

                new PublishITService.rent
                {
                    rent_id = 2,
                    media_id = 1,
                    user_id = 2,
                    start_date = DateTime.MinValue,
                    end_date = DateTime.Parse("10-05-1991 00:00:00")
                },

                new PublishITService.rent
                {
                    rent_id = 3,
                    media_id = 0,
                    user_id = 2,
                    start_date = DateTime.Parse("10-05-2050 00:00:00"),
                    end_date = DateTime.MaxValue
                },

                new PublishITService.rent
                {
                    rent_id = 4,
                    media_id = 4,
                    user_id = 1,
                    start_date = DateTime.MinValue,
                    end_date = DateTime.MaxValue
                },
            }.AsQueryable();
        }

        private void SetupUserMockSet(IQueryable<PublishITService.user> data)
        {
            _userMockSet.As<IQueryable<PublishITService.user>>().Setup(m => m.Provider).Returns(data.Provider);
            _userMockSet.As<IQueryable<PublishITService.user>>().Setup(m => m.Expression).Returns(data.Expression);
            _userMockSet.As<IQueryable<PublishITService.user>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _userMockSet.As<IQueryable<PublishITService.user>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupRoleMockSet(IQueryable<PublishITService.role> data)
        {
            _roleMockSet.As<IQueryable<PublishITService.role>>().Setup(m => m.Provider).Returns(data.Provider);
            _roleMockSet.As<IQueryable<PublishITService.role>>().Setup(m => m.Expression).Returns(data.Expression);
            _roleMockSet.As<IQueryable<PublishITService.role>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _roleMockSet.As<IQueryable<PublishITService.role>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupRatingMockSet(IQueryable<PublishITService.rating> data)
        {
            _ratingMockSet.As<IQueryable<PublishITService.rating>>().Setup(m => m.Provider).Returns(data.Provider);
            _ratingMockSet.As<IQueryable<PublishITService.rating>>().Setup(m => m.Expression).Returns(data.Expression);
            _ratingMockSet.As<IQueryable<PublishITService.rating>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _ratingMockSet.As<IQueryable<PublishITService.rating>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupMediaMockSet(IQueryable<PublishITService.media> data)
        {
            _mediaMockSet.As<IQueryable<PublishITService.media>>().Setup(m => m.Provider).Returns(data.Provider);
            _mediaMockSet.As<IQueryable<PublishITService.media>>().Setup(m => m.Expression).Returns(data.Expression);
            _mediaMockSet.As<IQueryable<PublishITService.media>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mediaMockSet.As<IQueryable<PublishITService.media>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupDocumentMockSet(IQueryable<PublishITService.document> data)
        {
            _documentMockSet.As<IQueryable<PublishITService.document>>().Setup(m => m.Provider).Returns(data.Provider);
            _documentMockSet.As<IQueryable<PublishITService.document>>().Setup(m => m.Expression).Returns(data.Expression);
            _documentMockSet.As<IQueryable<PublishITService.document>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _documentMockSet.As<IQueryable<PublishITService.document>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupVideoMockSet(IQueryable<PublishITService.video> data)
        {
            _videoMockSet.As<IQueryable<PublishITService.video>>().Setup(m => m.Provider).Returns(data.Provider);
            _videoMockSet.As<IQueryable<PublishITService.video>>().Setup(m => m.Expression).Returns(data.Expression);
            _videoMockSet.As<IQueryable<PublishITService.video>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _videoMockSet.As<IQueryable<PublishITService.video>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupGenreMockSet(IQueryable<PublishITService.genre> data)
        {
            _genreMockSet.As<IQueryable<PublishITService.genre>>().Setup(m => m.Provider).Returns(data.Provider);
            _genreMockSet.As<IQueryable<PublishITService.genre>>().Setup(m => m.Expression).Returns(data.Expression);
            _genreMockSet.As<IQueryable<PublishITService.genre>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _genreMockSet.As<IQueryable<PublishITService.genre>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupRentMockSet(IQueryable<PublishITService.rent> data)
        {
            _rentMockSet.As<IQueryable<PublishITService.rent>>().Setup(m => m.Provider).Returns(data.Provider);
            _rentMockSet.As<IQueryable<PublishITService.rent>>().Setup(m => m.Expression).Returns(data.Expression);
            _rentMockSet.As<IQueryable<PublishITService.rent>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _rentMockSet.As<IQueryable<PublishITService.rent>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }



        private void SetupMockSets()
        {
            _userMockSet = new Mock<IDbSet<PublishITService.user>>();
            SetupUserMockSet(InitUserData());

            _roleMockSet = new Mock<IDbSet<PublishITService.role>>();
            SetupRoleMockSet(InitRoleData());

            _ratingMockSet = new Mock<IDbSet<PublishITService.rating>>();
            SetupRatingMockSet(InitRatingData());

            _mediaMockSet = new Mock<IDbSet<PublishITService.media>>();
            SetupMediaMockSet(InitMediaData());

            _documentMockSet = new Mock<IDbSet<PublishITService.document>>();
            SetupDocumentMockSet(InitDocumentData());

            _videoMockSet = new Mock<IDbSet<PublishITService.video>>();
            SetupVideoMockSet(InitVideoData());

            _rentMockSet = new Mock<IDbSet<PublishITService.rent>>();
            SetupRentMockSet(InitRentData());

            _genreMockSet = new Mock<IDbSet<PublishITService.genre>>();
            SetupGenreMockSet(InitGenreData());
        }

        private void SetupEntitiesReturnValue()
        {
            _publishITEntitiesMock = new Mock<IPublishITEntities>();

            _publishITEntitiesMock.Setup(call => call.user).Returns(_userMockSet.Object);
            _publishITEntitiesMock.Setup(call => call.role).Returns(_roleMockSet.Object);
            _publishITEntitiesMock.Setup(call => call.rating).Returns(_ratingMockSet.Object);
            _publishITEntitiesMock.Setup(call => call.media).Returns(_mediaMockSet.Object);
            _publishITEntitiesMock.Setup(call => call.document).Returns(_documentMockSet.Object);
            _publishITEntitiesMock.Setup(call => call.video).Returns(_videoMockSet.Object);
            _publishITEntitiesMock.Setup(call => call.rent).Returns(_rentMockSet.Object);
            _publishITEntitiesMock.Setup(call => call.genre).Returns(_genreMockSet.Object);
        }
    }

    public class UnitTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories
        {
            get { return new List<string> { "UnitTest" }; }
        }
    }

    public class IntegrationTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories
        {
            get { return new List<string> { "IntegrationTest" }; }
        }
    }
}
