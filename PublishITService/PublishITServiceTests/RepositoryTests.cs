using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PublishITService;
using PublishITService.Parsers;
using PublishITService.Repositories;
using PublishITService.DTOs;

namespace PublishITServiceTests
{
    [TestClass]
    public class RepositoryTests
    {
        private IRepository _repository;
        private Mock<IPublishITEntities> _publishITEntitiesMock;

        private Mock<IDbSet<user>> _userMockSet;
        private Mock<IDbSet<role>> _roleMockSet;
        private Mock<IDbSet<rating>> _ratingMockSet;
        private Mock<IDbSet<media>> _mediaMockSet;
        private Mock<IDbSet<document>> _documentMockSet;
        private Mock<IDbSet<rent>> _rentMockSet;
        private Mock<IDbSet<video>> _videoMockSet;
        private Mock<IDbSet<genre>> _genreMockSet;
            
        [TestInitialize]
        public void InitTests()
        {
            SetupMockSets();

            SetupEntitiesReturnValue();


            _repository = new Repository(_publishITEntitiesMock.Object);
        }

        //FindUserById test

        [TestMethod]
        public void SuccessfullyGettingUserByIdVerifyByEmail()
        {
            var user = _repository.FindUserById(1);

            Assert.AreEqual(user.email, "email@email.com");
        }

        [TestMethod]
        public void SuccessfullyGettingUserByIdVerifyByName()
        {
            var user = _repository.FindUserById(1);

            Assert.AreEqual(user.name, "name 1");
        }

        [TestMethod]
        public void SuccessfullyGettingUserByIdVerifyByUserId()
        {
            var user = _repository.FindUserById(1);

            Assert.AreEqual(user.user_id, 1);
        }

       [TestMethod]
        public void SuccessfullyGettingUserByIdVerifyByUsername()
        {
            var user = _repository.FindUserById(1);

            Assert.AreEqual(user.username, "userName 1");
        }

       [TestMethod]
       public void SuccessfullyGettingUserByIdVerifyByPassword()
       {
           var user = _repository.FindUserById(1);

           Assert.AreEqual(user.password, "password 1");
       }

       [TestMethod]
       public void SuccessfullyGettingUserByIdVerifyByStatus()
       {
           var user = _repository.FindUserById(1);

           Assert.AreEqual(user.status, "Active");
       }

       [TestMethod] 
       public void SuccessfullyGettingUserByIdVerifyByRoleCount()
       {
           var user = _repository.FindUserById(1);

           Assert.AreEqual(user.roles.Count, 1);
       }

       [TestMethod]
       public void SuccessfullyGettingUserByIdVerifyByRoleId()
       {
           var user = _repository.FindUserById(1);

           Assert.AreEqual(user.roles[0].Id, 1);
       }
        
        [TestMethod]
        public void UnsuccessfullyGettingUserByIdVerifyByName()
        {
            var user = _repository.FindUserById(1337);

            Assert.AreEqual(user.name, "No user found");
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserByIdVerifyByUserId()
        {
            var user = _repository.FindUserById(1337);

            Assert.AreEqual(user.user_id, 0);
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserByIdVerifyByStatus()
        {
            var user = _repository.FindUserById(1337);

            Assert.AreEqual(user.status, "Not a user");
        }

        //FindUserByUsername test

        [TestMethod]
        public void SuccessfullyGettingUserByUserNameVerifyByUserEmail()
        {
            var user = _repository.FindUserByUsername("userName 1");

            Assert.AreEqual(user.email, "email@email.com");
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUserNameVerifyByUserName()
        {
            var user = _repository.FindUserByUsername("userName 1");

            Assert.AreEqual(user.name, "name 1");
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUserNameVerifyByUserId()
        {
            var user = _repository.FindUserByUsername("userName 1");

            Assert.AreEqual(user.user_id, 1);
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUserNameVerifyByUsername()
        {
            var user = _repository.FindUserByUsername("userName 1");

            Assert.AreEqual(user.username, "userName 1");
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUserNameVerifyByUserPassword()
        {
            var user = _repository.FindUserByUsername("userName 1");

            Assert.AreEqual(user.password, "password 1");
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUserNameVerifyByUserStatus()
        {
            var user = _repository.FindUserByUsername("userName 1");

            Assert.AreEqual(user.status, "Active");
        }

        [TestMethod] 
        public void SuccessfullyGettingUserByUserNameVerifyByRoleCount()
        {
            var user = _repository.FindUserByUsername("userName 1");

            Assert.AreEqual(user.roles.Count, 1);
        }

        [TestMethod] 
        public void SuccessfullyGettingUserByUserNameVerifyByRoleId()
        {
            var user = _repository.FindUserByUsername("userName 1");

            Assert.AreEqual(user.roles[0].Id, 1);
        }

        [TestMethod] 
        public void UnsuccessfullyGettingUserByUserNameWithCapitalVerifyByUserId()
        {
            var user = _repository.FindUserByUsername("USERNAME 1");

            Assert.AreNotEqual(user.user_id, 1);
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserByUsernameVerifyByUserName()
        {
            var user = _repository.FindUserByUsername("Not existing userName");

            Assert.AreEqual(user.username, "No user found");
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserByUsernameVerifyByUserId()
        {
            var user = _repository.FindUserByUsername("Not existing userName");

            Assert.AreEqual(user.user_id, 0);
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserByUsernameVerifyByStatus()
        {
            var user = _repository.FindUserByUsername("Not existing userName");

            Assert.AreEqual(user.status, "Not a user");
        }

        //FindUserByUsernameAndPassword test

        [TestMethod]
        public void SuccessfullyGettingUserByUsernameAndPasswordVerifyByName()
        {
            var user = _repository.FindUserByUsernameAndPassword("userName 1", "password 1", 1);

            Assert.AreEqual(user.name, "name 1");
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUsernameAndPasswordVerifyByUsername()
        {
            var user = _repository.FindUserByUsernameAndPassword("userName 1", "password 1", 1);

            Assert.AreEqual(user.username, "userName 1");
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUsernameAndPasswordVerifyByBirthday()
        {
            var user = _repository.FindUserByUsernameAndPassword("userName 1", "password 1",1);

            Assert.AreEqual(user.birthday, null);
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUsernameAndPasswordVerifyByStatus()
        {
            var user = _repository.FindUserByUsernameAndPassword("userName 1", "password 1",1);

            Assert.AreEqual(user.status, "Active");
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUsernameAndPasswordVerifyByEmail()
        {
            var user = _repository.FindUserByUsernameAndPassword("userName 1", "password 1",1);

            Assert.AreEqual(user.email, "email@email.com");
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUsernameAndPasswordVerifyByUserId()
        {
            var user = _repository.FindUserByUsernameAndPassword("userName 1", "password 1",1);

            Assert.AreEqual(user.user_id, 1);
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUsernameAndPasswordVerifyByRoleCount()
        {
            var user = _repository.FindUserByUsernameAndPassword("userName 1", "password 1",1);

            Assert.AreEqual(user.roles.Count, 1);
        }

        [TestMethod]
        public void SuccessfullyGettingUserByUsernameAndPasswordVerifyByRoleId()
        {
            var user = _repository.FindUserByUsernameAndPassword("userName 1", "password 1",1);

            Assert.AreEqual(user.roles[0].Id, 1);
        }

        [TestMethod]
        public void UnsuccessfullyGetingUserByUsernameAndPasswordDueToWrongOrganizationId()
        {
            var user = _repository.FindUserByUsernameAndPassword("userName 1", "password 1", 2);

            Assert.AreEqual(user.name, "Sign in failed");
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserByUsernameAndPasswordVerifyByName()
        {
            var user = _repository.FindUserByUsernameAndPassword("Not existing userName", "Some password",1);

            Assert.AreEqual(user.name, "Sign in failed");
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserByUsernameAndPasswordVerifyByUserId()
        {
            var user = _repository.FindUserByUsernameAndPassword("Not existing userName", "Some password",1);

            Assert.AreEqual(user.user_id, 0);
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserByUserAndPasswordWithOnlyCapitalLettersVerifyByUserId()
        {
            var user = _repository.FindUserByUsernameAndPassword("USERNAME 1", "SOME PASSWORD",1);

            Assert.AreNotEqual(user.user_id, 1);
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserByUserAndPasswordWithCapitalUsernameVerifyByUserId()
        {
            var user = _repository.FindUserByUsernameAndPassword("USERNAME 1", "some password",1);

            Assert.AreNotEqual(user.user_id, 1);
        }

        [TestMethod]
        public void UnsuccessfullyGettingUserByUserAndPasswordWithCapitalPasswordVerifyByUserId()
        {
            var user = _repository.FindUserByUsernameAndPassword("username 1", "SOME PASSWORD",1);

            Assert.AreNotEqual(user.user_id, 1);
        }

        //AddUser test

        [TestMethod]
        public void CheckingIfSaveChangesIsCalledOnceWhenAddingNewUser()
        {
            _repository.AddUser(new UserDTO
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

        //RegisterUser test

        [TestMethod]
        public void UnsuccessfullyRegisterUserDueToEmptyUserDTO()
        {
            _repository.AddUser(new UserDTO());
        }

        //DeleteUser test

        [TestMethod]
        public void CheckingIfSaveChangesIsCalledOnceWhenRemovingUserById()
        {
            _repository.DeleteUser(1);

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        //EditingUser test

        [TestMethod]
        public void CheckingIfSaveChangesIsCalledOnceWhenEditingUser()
        {
            _repository.EditUser(new UserDTO
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
        }

        [TestMethod]
        public void CheckingThatSaveChangesIsNeverCalledWhenEditingUser()
        {
            _repository.EditUser(new UserDTO
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

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Never);
        }

        //StoreMedia test

        [TestMethod]
        public void CheckingIfAddIsCalledCorrectlyWhenUploadingDocumentVerifyByMedia()
        {
            var mediaParser = new DocumentParser();

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

            _repository.StoreMedia(remoteFileInfo, mediaParser);

            _mediaMockSet.Verify(x => x.Add(It.Is<media>(
                                            newMedia =>
                                            newMedia.title.Equals("title") && 
                                            newMedia.user_id == 1)),
                                            Times.Once
                                            );


            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Exactly(2));
        }

        [TestMethod]
        public void CheckingIfAddIsCalledCorrectlyWhenUploadingDocumentVerifyByDocument()
        {
            var mediaParser = new DocumentParser();

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

			_repository.StoreMedia(remoteFileInfo, mediaParser);

            _documentMockSet.Verify(x => x.Add(It.Is<document>(
                                            newDocument =>
                                            newDocument.media_id == 0 &&
                                            newDocument.status.Equals("status"))),
                                            Times.Once
                                            );

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Exactly(2));
        }

        [TestMethod]
        public void CheckingIfAddIsCalledCorrectlyWhenUploadingDocumentVerifyByVideo()
        {
            var mediaParser = new DocumentParser();

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

			_repository.StoreMedia(remoteFileInfo, mediaParser);
          
            _videoMockSet.Verify(x => x.Add(It.Is<video>(
                                            newVideo =>
                                            newVideo.media_id == 0)),
                                            Times.Never
                                            );

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Exactly(2));
        }

        [TestMethod]
        public void CheckingIfAddIsCalledCorrectlyWhenUploadingVideoVerifyByMedia()
        {
            var mediaParser = new VideoParser();

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

			_repository.StoreMedia(remoteFileInfo, mediaParser);

            _mediaMockSet.Verify(x => x.Add(It.Is<media>(
                                            newMedia =>
                                            newMedia.title.Equals("title") &&
                                            newMedia.user_id == 1)),
                                            Times.Once
                                            );

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Exactly(2));
        }

        [TestMethod]
        public void CheckingIfAddIsCalledCorrectlyWhenUploadingVideoVerifyByVideo()
        {
            var mediaParser = new VideoParser();

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

			_repository.StoreMedia(remoteFileInfo, mediaParser);

            _videoMockSet.Verify(x => x.Add(It.Is<video>(
                                            newVideo =>
                                            newVideo.media_id == 0)),
                                            Times.Once
                                            );

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Exactly(2));
        }

        [TestMethod]
        public void CheckingIfAddIsCalledCorrectlyWhenUploadingVideoVerifyByDocument()
        {
            var mediaParser = new VideoParser();

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

			_repository.StoreMedia(remoteFileInfo, mediaParser);

            _documentMockSet.Verify(x => x.Add(It.Is<document>(
                                            newDocument =>
                                            newDocument.media_id == 0 &&
                                            newDocument.status.Equals("status"))),
                                            Times.Never
                                            );

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Exactly(2));
        }


        //GetMediaPath test

        [TestMethod]
        public void SuccessfullyGettingTheRightPath()
        {
            var path = _repository.GetMediaPath(4);

            Assert.AreEqual(path, "location 4");
        }
        
        [TestMethod]
        public void UnsuccessfullyGettingPathDoToNoFoundMovieId()
        {
            var movie = _repository.GetMediaPath(9);

            Assert.IsNull(movie);
        }

        //FindMediaByTitle test

        [TestMethod]
        public void SuccessfullySearchMediaByTitleVerifyByCount()
        {
            var listOfMedia = _repository.FindMediaByTitle("title", 1);

            Assert.AreEqual(listOfMedia.Count, 3);
        }

        [TestMethod]
        public void SuccessfullySearchMediaByTitleVerifyByMediaId()
        {
            var listOfMedia = _repository.FindMediaByTitle("title", 1);

            Assert.AreEqual(listOfMedia[0].media_id, 1);
        }

        [TestMethod]
        public void SuccessfullySearchMediaByTitleVerifyByUserId()
        {
            var listOfMedia = _repository.FindMediaByTitle("title", 1);

            Assert.AreEqual(listOfMedia[0].user_id, 1);
        }

        [TestMethod]
        public void SuccessfullySearchMediaByTitleVerifyByFormatId()
        {
            var listOfMedia = _repository.FindMediaByTitle("title", 1);

            Assert.AreEqual(listOfMedia[0].format_id, 1);
        }

        [TestMethod]
        public void SuccessfullySearchMediaByTitleVerifyByTitle()
        {
            var listOfMedia = _repository.FindMediaByTitle("title", 1);

            Assert.AreEqual(listOfMedia[0].title, "title 1");
        }

        [TestMethod]
        public void SuccessfullySearchMediaByTitleVerifyByLocation()
        {
            var listOfMedia = _repository.FindMediaByTitle("title", 1);

            Assert.AreEqual(listOfMedia[0].location, "location 1");
        }

        [TestMethod]
        public void UnsuccessfullySearchMediaByTitleVerifyByCount()
        {
            var listOfMedia = _repository.FindMediaByTitle("No media", 1);

            Assert.AreEqual(listOfMedia.Count, 0);
        }

        [TestMethod]
        public void UnsuccessfullySearchMediaByTitleCapitalLetterVerifyByCount()
        {
            var listOfMedia = _repository.FindMediaByTitle("TITLE", 1);

            Assert.AreEqual(listOfMedia.Count, 0);
        }

        //FindMediaById test

        [TestMethod]
        public void SuccessfullyGettingMediaByIdVerifyByMediaId()
        {
            var gottenMedia = _repository.FindMediaById(1);

            Assert.AreEqual(gottenMedia.media_id, 1);
        }

        [TestMethod]
        public void SuccessfullyGettingMediaByIdVerifyByUserId()
        {
            var gottenMedia = _repository.FindMediaById(1);

            Assert.AreEqual(gottenMedia.user_id, 1);
        }

        [TestMethod]
        public void SuccessfullyGettingMediaByIdVerifyByFormatId()
        {
            var gottenMedia = _repository.FindMediaById(1);

            Assert.AreEqual(gottenMedia.format_id, 1);
        }

        [TestMethod]
        public void SuccessfullyGettingMediaByIdVerifyByTitle()
        {
            var gottenMedia = _repository.FindMediaById(1);

            Assert.AreEqual(gottenMedia.title, "title 1");
        }

        [TestMethod]
        public void SuccessfullyGettingMediaByIdVerifyByLocation()
        {
            var gottenMedia = _repository.FindMediaById(1);

            Assert.AreEqual(gottenMedia.location, "location 1");
        }

        [TestMethod]
        public void UnsuccessfullyGettingMediaByIdVerifyByTitle()
        {
            var gottenMedia = _repository.FindMediaById(5);

            Assert.AreEqual(gottenMedia.title, "No media found");
        }

        //FindMoviesByGenre test

        [TestMethod]
        public void SuccessfullyGettingMoviesByGenreVerifyByCount()
        {
            var movies = _repository.FindMoviesByGenre("Comedy", 1);

            Assert.AreEqual(movies.Count, 1);
        }

        [TestMethod]
        public void SuccessfullyGettingMoviesByGenreVerifyByMediaId()
        {
            var movies = _repository.FindMoviesByGenre("Comedy", 1);

            Assert.AreEqual(movies[0].media_id, 4);
        }

        [TestMethod]
        public void SuccessfullyGettingMoviesByGenreVerifyByUserId()
        {
            var movies = _repository.FindMoviesByGenre("Comedy", 1);

            Assert.AreEqual(movies[0].user_id, 1);
        }

        [TestMethod]
        public void SuccessfullyGettingMoviesByGenreVerifyByFormatId()
        {
            var movies = _repository.FindMoviesByGenre("Comedy", 1);

            Assert.AreEqual(movies[0].format_id, 2);
        }

        [TestMethod]
        public void SuccessfullyGettingMoviesByGenreVerifyByTitle()
        {
            var movies = _repository.FindMoviesByGenre("Comedy", 1);

            Assert.AreEqual(movies[0].title, "title 4");
        }

        [TestMethod]
        public void SuccessfullyGettingMoviesByGenreVerifyByLocation()
        {
            var movies = _repository.FindMoviesByGenre("Comedy", 1);

            Assert.AreEqual(movies[0].location, "location 4");
        }

        [TestMethod]
        public void UnsuccessfullyGettingMoviesByGenreDueToNoMoviesInTheGenreVerifyByCount()
        {
            var movies = _repository.FindMoviesByGenre("Science Fiction", 1);

            Assert.AreEqual(movies.Count, 0);
        }

        [TestMethod]
        public void UnsuccessfullyGettingMoviesByGenreDueToCapitalLettesVerifyByCount()
        {
            var movies = _repository.FindMoviesByGenre("COMEDY", 1);

            Assert.AreEqual(movies.Count, 0);
        }

        //FindRating test

        [TestMethod]
        public void SuccessfullyGettingRating()
        {
            var rating = _repository.FindRating(1, 1);

            Assert.AreEqual(rating.rating1, 5);
        }

        [TestMethod]
        public void UnsuccessfullyGettingRating()
        {
            var rating = _repository.FindRating(80085, 666);

            Assert.IsNull(rating);
        }

        //PostRating test

        [TestMethod]
        public void SuccessfullyPostingNewRating()
        {
            var posted = _repository.PostRating(3, 4, 1);

            _ratingMockSet.Verify(x =>x.Add(It.Is<rating>(newRating => 
                                                            newRating.media_id == 4 && 
                                                            newRating.rating1 == 3 && 
                                                            newRating.user_id == 1 &&
                                                            newRating.rating_id == 4)),
                                                            Times.Once);

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Once);

            Assert.IsTrue(posted.IsExecuted);

            Assert.AreEqual(posted.Message, "Rating added");
        }

        [TestMethod]
        public void SuccessfullyChangingRating()
        {
            var posted = _repository.PostRating(1, 2, 1);

            _ratingMockSet.Verify(x => x.Add(It.Is<rating>(newRating =>
                                                            newRating.media_id == 2 &&
                                                            newRating.rating1 == 1 &&
                                                            newRating.user_id == 1 &&
                                                            newRating.rating_id == 4)),
                                                            Times.Never);

            _publishITEntitiesMock.Verify(x => x.SaveChanges(), Times.Once);

            Assert.IsTrue(posted.IsExecuted);

            Assert.AreEqual(posted.Message, "Rating changed");
        }

        //CheckIfRentExists test

        [TestMethod]
        public void SuccessfullyCheckRentExist()
        {
            var temp = _repository.CheckingIfRentExists(1, 4);

            Assert.IsTrue(temp);
        }

        [TestMethod]
        public void UnsuccessfullyCheckRentExist()
        {
            var temp = _repository.CheckingIfRentExists(6, 7);

            Assert.IsFalse(temp);
        }

        //FindMediaByAuthorId test

        [TestMethod]
        public void SuccessfullyGetMediaByAuthorIdVerifyByMediaId()
        {
            var movie = _repository.FindMediasByAuthorId(1);

            Assert.AreEqual(movie[0].media_id, 1);
        }

        [TestMethod]
        public void SuccessfullyGetMediaByAuthorIdVerifyByUserId()
        {
            var movie = _repository.FindMediasByAuthorId(1);

            Assert.AreEqual(movie[0].user_id, 1);
        }

        [TestMethod]
        public void SuccessfullyGetMediaByAuthorIdVerifyByFormatId()
        {
            var movie = _repository.FindMediasByAuthorId(1);

            Assert.AreEqual(movie[0].format_id, 1);
        }

        [TestMethod]
        public void SuccessfullyGetMediaByAuthorIdVerifyByTitle()
        {
            var movie = _repository.FindMediasByAuthorId(1);

            Assert.AreEqual(movie[0].title, "title 1");
        }

        [TestMethod]
        public void SuccessfullyGetMediaByAuthorIdVerifyByLocation()
        {
            var movie = _repository.FindMediasByAuthorId(1);

            Assert.AreEqual(movie[0].location, "location 1");
        }

        [TestMethod]
        public void UnsuccessfullyGetMediaByAuthorIdVerifyByCount()
        {
            var movie = _repository.FindMediasByAuthorId(10);

            Assert.AreEqual(movie.Count, 0);
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

        private IQueryable<media> InitMediaData()
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

            }.AsQueryable();
        }

        private IQueryable<document> InitDocumentData()
        {
            return new List<document>
            {
                new document
                {
                    media_id = 1,
                    status = "status"
                },

                new document
                {
                    media_id = 3,
                    status = "status"
                }
            }.AsQueryable();
        }

        private IQueryable<video> InitVideoData()
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

            }.AsQueryable();
        }

        private IQueryable<rent> InitRentData()
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
            }.AsQueryable();
        }

        private IQueryable<genre> InitGenreData()
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

        private void SetupMediaMockSet(IQueryable<media> data)
        {
            _mediaMockSet.As<IQueryable<media>>().Setup(m => m.Provider).Returns(data.Provider);
            _mediaMockSet.As<IQueryable<media>>().Setup(m => m.Expression).Returns(data.Expression);
            _mediaMockSet.As<IQueryable<media>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mediaMockSet.As<IQueryable<media>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupDocumentMockSet(IQueryable<document> data)
        {
            _documentMockSet.As<IQueryable<document>>().Setup(m => m.Provider).Returns(data.Provider);
            _documentMockSet.As<IQueryable<document>>().Setup(m => m.Expression).Returns(data.Expression);
            _documentMockSet.As<IQueryable<document>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _documentMockSet.As<IQueryable<document>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupVideoMockSet(IQueryable<video> data)
        {
            _videoMockSet.As<IQueryable<video>>().Setup(m => m.Provider).Returns(data.Provider);
            _videoMockSet.As<IQueryable<video>>().Setup(m => m.Expression).Returns(data.Expression);
            _videoMockSet.As<IQueryable<video>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _videoMockSet.As<IQueryable<video>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupRentMockSet(IQueryable<rent> data)
        {
            _rentMockSet.As<IQueryable<rent>>().Setup(m => m.Provider).Returns(data.Provider);
            _rentMockSet.As<IQueryable<rent>>().Setup(m => m.Expression).Returns(data.Expression);
            _rentMockSet.As<IQueryable<rent>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _rentMockSet.As<IQueryable<rent>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupGenreMockSet(IQueryable<genre> data)
        {
            _genreMockSet.As<IQueryable<genre>>().Setup(m => m.Provider).Returns(data.Provider);
            _genreMockSet.As<IQueryable<genre>>().Setup(m => m.Expression).Returns(data.Expression);
            _genreMockSet.As<IQueryable<genre>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _genreMockSet.As<IQueryable<genre>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private void SetupMockSets()
        {
            _userMockSet = new Mock<IDbSet<user>>();
            SetupUserMockSet(InitUserData());

            _roleMockSet = new Mock<IDbSet<role>>();
            SetupRoleMockSet(InitRoleData());

            _ratingMockSet = new Mock<IDbSet<rating>>();
            SetupRatingMockSet(InitRatingData());

            _mediaMockSet = new Mock<IDbSet<media>>();
            SetupMediaMockSet(InitMediaData());

            _documentMockSet = new Mock<IDbSet<document>>();
            SetupDocumentMockSet(InitDocumentData());

            _videoMockSet = new Mock<IDbSet<video>>();
            SetupVideoMockSet(InitVideoData());

            _rentMockSet = new Mock<IDbSet<rent>>();
            SetupRentMockSet(InitRentData());

            _genreMockSet = new Mock<IDbSet<genre>>();
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
}
