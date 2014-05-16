////using System;
////using System.Collections.Generic;
////using System.IO;
////using Microsoft.VisualStudio.TestTools.UnitTesting;
////using Moq;
////using PublishITService;
////using PublishITService.Repositories;
////using PublishITService.DTOs;

////namespace PublishITServiceTests
////{
////    [TestClass]
////    public class ServiceTests
////    {
////        private IPublishITService _publishITService;

////        [TestInitialize]
////        public void TestInit()
////        {
////            _publishITService = new PublishITService.PublishITService(new TestRepository());
////        }

////        [TestMethod]
////        public void SuccessfullyRegisterUser()
////        {
////            Assert.AreNotEqual(_publishITService.GetUserByUserName("newUserName").username, "newUserName");

////            var responseMessage = _publishITService.RegisterUser(new UserDTO
////            {
////                birthday = DateTime.MinValue,
////                email = "newEmail@email.com",
////                name = "newName",
////                username = "newUserName",
////                password = "newPassword",
////                organization_id = 1
////            });

////            Assert.AreEqual(responseMessage.Message, "User registered");

////            Assert.IsTrue(responseMessage.IsExecuted);
////        }

////        [TestMethod]
////        public void UnsuccessfullyRegisterUserDueToExistingUserName()
////        {
////            Assert.AreEqual(_repository.GetUserByUserName("userName 1").username, "userName 1");

////            var responseMessage = _repository.RegisterUser(new UserDTO
////            {
////                birthday = DateTime.MinValue,
////                email = "newEmail@email.com",
////                name = "newName",
////                username = "userName 1",
////                password = "newPassword",
////                organization_id = 1
////            });

////            _userMockSet.Verify(x => x.Add(It.Is<user>(
////                                                        newUser =>
////                                                        newUser.birthday == DateTime.MinValue &&
////                                                        newUser.email.Equals("newEmail@email.com") &&
////                                                        newUser.name.Equals("newName") &&
////                                                        newUser.user_name.Equals("userName 1") &&
////                                                        newUser.password.Equals("newPassword") &&
////                                                        newUser.organization_id == 1)),
////                                                        Times.Never
////                                                        );

////            Assert.AreEqual(responseMessage.Message, "Username already exists");

////            Assert.IsFalse(responseMessage.IsExecuted);
////        }

////        [TestMethod]
////        public void UnsuccessfullyRegisterUserDueToEmptyUserDTO()
////        {
////            var responseMessage = _repository.RegisterUser(new UserDTO());

////            Assert.IsFalse(responseMessage.IsExecuted);

////            Assert.AreEqual(responseMessage.Message, "Registration failed");
////        }

////        [TestMethod]
////        public void DeleteUser()
////        {
////            Assert.IsTrue(responseMessage.IsExecuted);

////            Assert.AreEqual(responseMessage.Message, "Deletion completed");
////        }

////        [TestMethod]
////        public void UnsuccessfullyDeletingUserDoToUnknownUserId()
////        {
////            var responseMessage = _repository.DeleteUser(5);

////            Assert.IsFalse(responseMessage.IsExecuted);

////            Assert.AreEqual(responseMessage.Message, "Deletion failed");
////        }

////        public void SuccessfullyEditingUsersInformation()
////        {
////            var responseMessage = _repository.EditUser(new UserDTO
////            {
////                birthday = DateTime.Now,
////                email = "ChangedEmail@email.com",
////                name = "ChangedName",
////                user_id = 1,
////                username = "ChangedUserName 1",
////                password = "Changedpassword 1",
////                status = "Active",
////                organization_id = 1,
////                roles = new List<RoleDTO>
////                    {
////                        new RoleDTO
////                        {
////                            Id = 1,
////                            Title = "role 1"
////                        }
////                    }
////            });

////            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Once);

////            Assert.IsTrue(responseMessage.IsExecuted);

////            Assert.AreEqual(responseMessage.Message, "User edited");
////        }

////        [TestMethod]
////        public void UnsuccessfullyEditingUsersInformationByChangingUserId()
////        {
////            var responseMessage = _repository.EditUser(new UserDTO
////            {
////                email = "ChangedEmail@email.com",
////                name = "ChangedName",
////                user_id = 1337,
////                username = "ChangedUserName 1",
////                password = "Changedpassword 1",
////                status = "Active",
////                organization_id = 1,
////                roles = new List<RoleDTO>
////                    {
////                        new RoleDTO
////                        {
////                            Id = 1,
////                            Title = "role 1"
////                        }
////                    }
////            });

////            Assert.IsFalse(responseMessage.IsExecuted);

////            Assert.AreEqual(responseMessage.Message, "Editing failed");
////        }

////        [TestMethod]
////        public void SuccessfullyUploadingDocument()
////        {
////            var remoteFileInfo = new RemoteFileInfo
////            {
////                FileName = "filename.pdf",
////                FileStream = new MemoryStream(),
////                Title = "title",
////                GenreId = 1,
////                Length = 1,
////                Status = "status",
////                UserId = 1
////            };

////            var responseMessage = _repository.UploadMedia(remoteFileInfo);

////            _mediaMockSet.Verify(x => x.Add(It.Is<media>(
////                                            newMedia =>
////                                            newMedia.title.Equals("title") &&
////                                            newMedia.user_id == 1)),
////                                            Times.Once
////                                            );

////            _documentMockSet.Verify(x => x.Add(It.Is<document>(
////                                            newDocument =>
////                                            newDocument.media_id == 0 &&
////                                            newDocument.status.Equals("status"))),
////                                            Times.Once
////                                            );

////            _videoMockSet.Verify(x => x.Add(It.Is<video>(
////                                            newVideo =>
////                                            newVideo.media_id == 0)),
////                                            Times.Never
////                                            );

////            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Exactly(2));

////            Assert.IsTrue(responseMessage.IsExecuted);

////            Assert.AreEqual(responseMessage.Message, "File successfully uploaded");
////        }

////        [TestMethod]
////        public void SuccessfullyUploadingVideo()
////        {
////            var remoteFileInfo = new RemoteFileInfo
////            {
////                FileName = "filename.mp4",
////                FileStream = new MemoryStream(),
////                Title = "title",
////                GenreId = 1,
////                Length = 1,
////                Status = "status",
////                UserId = 1
////            };

////            var responseMessage = _repository.UploadMedia(remoteFileInfo);

////            _mediaMockSet.Verify(x => x.Add(It.Is<media>(
////                                            newMedia =>
////                                            newMedia.title.Equals("title") &&
////                                            newMedia.user_id == 1)),
////                                            Times.Once
////                                            );

////            _videoMockSet.Verify(x => x.Add(It.Is<video>(
////                                            newVideo =>
////                                            newVideo.media_id == 0)),
////                                            Times.Once
////                                            );

////            _documentMockSet.Verify(x => x.Add(It.Is<document>(
////                                            newDocument =>
////                                            newDocument.media_id == 0 &&
////                                            newDocument.status.Equals("status"))),
////                                            Times.Never
////                                            );

////            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Exactly(2));

////            Assert.IsTrue(responseMessage.IsExecuted);

////            Assert.AreEqual(responseMessage.Message, "File successfully uploaded");
////        }

////        [TestMethod]
////        public void UnsuccessfullyUploadingMediaDoToUnknownFileFormat()
////        {
////            var remoteFileInfo = new RemoteFileInfo
////            {
////                FileName = "filename.mp3",
////                FileStream = new MemoryStream(),
////                Title = "title",
////                GenreId = 1,
////                Length = 1,
////                Status = "status",
////                UserId = 1
////            };

////            var responseMessage = _repository.UploadMedia(remoteFileInfo);

////            _mediaMockSet.Verify(x => x.Add(It.Is<media>(
////                                            newMedia =>
////                                            newMedia.title.Equals("title") &&
////                                            newMedia.user_id == 1)),
////                                            Times.Never
////                                            );

////            _videoMockSet.Verify(x => x.Add(It.Is<video>(
////                                            newVideo =>
////                                            newVideo.media_id == 0)),
////                                            Times.Never
////                                            );

////            _documentMockSet.Verify(x => x.Add(It.Is<document>(
////                                            newDocument =>
////                                            newDocument.media_id == 0 &&
////                                            newDocument.status.Equals("status"))),
////                                            Times.Never
////                                            );

////            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Never);

////            Assert.IsFalse(responseMessage.IsExecuted);

////            Assert.AreEqual(responseMessage.Message, "Unknown file format");
////        }

////        [TestMethod]
////        public void SuccessfullyStreamMedia()
////        {
////            var movie = _repository.StreamMovie(1, 4);

////            Assert.AreEqual(movie, "<video width='320' heigth='240' controls>" +
////                                        "<source src='location 4' type='video/mp4'>" +
////                                        "<source='movie.ogg' type='video/ogg'>" +
////                                    "</video>");
////        }

////        [TestMethod]
////        public void UnsuccessfullyStreamMediaDoToNoFoundUserId()
////        {
////            var movie = _repository.StreamMovie(9, 0);

////            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
////        }

////        [TestMethod]
////        public void UnsuccessfullyStreamMediaDoToNoFoundMovieId()
////        {
////            var movie = _repository.StreamMovie(1, 9);

////            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
////        }

////        [TestMethod]
////        public void UnsuccessfullyStreamMediaDoToDateExpired()
////        {
////            var movie = _repository.StreamMovie(2, 1);

////            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
////        }

////        [TestMethod]
////        public void UnsuccessfullyStreamMediaDoToDateNotStarted()
////        {
////            var movie = _repository.StreamMovie(2, 0);

////            Assert.AreEqual(movie, "" + "<div>" + "<span>Sorry.. It appears you did not rent this title. </span>" + "</div>");
////        }
//[TestMethod]
//        public void CheckingIfAddIsNeverCalledWhenAddingNewUserWithExistingName()
//        {
//            _repository.AddUser(new UserDTO
//            {
//                birthday = DateTime.MinValue,
//                email = "newEmail@email.com",
//                name = "newName",
//                username = "userName 1",
//                password = "newPassword",
//                organization_id = 1
//            });

//            _userMockSet.Verify(x => x.Add(It.Is<user>(
//                                                        newUser => 
//                                                        newUser.birthday == DateTime.MinValue && 
//                                                        newUser.email.Equals("newEmail@email.com") && 
//                                                        newUser.name.Equals("newName") && 
//                                                        newUser.user_name.Equals("userName 1") && 
//                                                        newUser.password.Equals("newPassword") && 
//                                                        newUser.organization_id == 1)), 
//                                                        Times.Never
//                                                        );
//        }
////    }
////}
