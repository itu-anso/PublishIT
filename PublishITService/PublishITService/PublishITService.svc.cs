using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PublishITService
{
    public class PublishITService : IPublishITService
    {
        private readonly IPublishITEntities _publishITEntities;

        public PublishITService()
        {
            
        }

        public PublishITService(IPublishITEntities publishITEntities = null)
        {
            _publishITEntities = publishITEntities;
        }

        public UserDTO GetUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                                where u.user_id == inputUser.user_id
                                select new UserDTO()
                                {
                                    name = u.name,
                                    birthday = u.birthday,
                                    status = u.status,
                                    email = u.email,
                                    user_id = u.user_id,
                                }).FirstOrDefault();

                
                if (foundUser != null && foundUser.status.Equals("Active"))
                {
                    foundUser.roles = new List<RoleDTO>();

                    var roleList = (from r in entities.role
                        from u in r.user
                        where u.user_id == foundUser.user_id
                        select r).ToList();

                    foreach (var r in roleList)
                    {
                        var x = new RoleDTO {Id = r.role_id, Title = r.role1};
                        foundUser.roles.Add(x);
                    }

                    return foundUser;
                }

                return new UserDTO() {name = "No user found"};
            }
        }

        public ResponseMessage RegisterUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                if (GetUser(inputUser).name.Equals("No user found"))
                {
                    int id;
                    if (!entities.user.Any())
                    {
                        id = 1;
                    }
                    else
                    {
                        id = entities.user.Max(u => u.user_id) + 1;
                    }
                    entities.user.Add(new user
                    {
                        user_id = id,
                        name = inputUser.name,
                        password = inputUser.password,
                        birthday = inputUser.birthday,
                        email = inputUser.email,
                        organization_id = inputUser.organization_id,
                        salt = "salt",
                        status = "active",

                    });

                    entities.SaveChanges();

                    if (GetUser(inputUser).name.Equals(inputUser.name))
                    {
                        return new ResponseMessage() {IsExecuted = true, Message = "User registered"};
                    }
                    else
                    {
                        return new ResponseMessage() {IsExecuted = false, Message = "Registration failed"};
                    }
                    
                }
                else
                {
                    return new ResponseMessage() {IsExecuted = false, Message = "Username already exists"};
                }
            }
        }

        public ResponseMessage DeleteUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                    where u.user_id == inputUser.user_id
                    select u).FirstOrDefault();

                entities.user.Remove(foundUser);

                try
                {
                    entities.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (GetUser(inputUser).name.Equals("No user found"))
                {
                    return new ResponseMessage() {IsExecuted = true, Message = "Deletion completed"};
                }
                else
                {
                    return new ResponseMessage() {IsExecuted = false, Message = "Deletion failed"};
                }
                
            }
        }

        public ResponseMessage EditUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from user in entities.user
                                where user.user_id == inputUser.user_id
                                select user).FirstOrDefault();

                if (foundUser != null)
                {
                    foundUser.name = inputUser.name;
                    foundUser.password = inputUser.password;
                    foundUser.birthday = inputUser.birthday;
                    foundUser.email = inputUser.email;
                    foundUser.organization_id = inputUser.organization_id;
                }

                try
                    {
                        entities.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw new Exception();
                    }

                    var gottenUser = GetUser(inputUser);

                if (gottenUser.name.Equals(inputUser.name) /*&& gottenUser.password.Equals(inputUser.password)*/ && gottenUser.birthday.Equals(inputUser.birthday) && gottenUser.email.Equals(inputUser.email))
                {
                    return new ResponseMessage() {IsExecuted = true, Message = "User edited"};
                }
                else
                {
                    return new ResponseMessage() {IsExecuted = false, Message = "Editing failed"};
                }
            }
        }

        //public bool UploadMedia(File media)
        //{
        //    bool success = true;
        //    return success;
        //}

        //public File DownloadMedia(int id)
        //{
        //    using (var entities = _publishITEntities = new RentIt09Entities())
        //    {
        //        var foundLocation = from med in entities.media
        //                            where med.media_id == id
        //                            select med.location;
        //        throw new NotImplementedException();

        //    }
        //}

        public string StreamMedia(int id)
        {
            string mediaStreamed = "<video width='320' heigth='240' controls>" +
                                        "<source src='GetMoviesByGenre.mp4' type='video/mp4'>" +
                                        "<source='movie.ogg' type='video/ogg'>" +
                                   "</video>";
            return mediaStreamed;
        }

        //public List<media> SearchMedia(string title)
        //{
        //    using (var entities = _publishITEntities ?? new RentIt09Entities())
        //    {
        //        List<media> mediaTitles = new List<media>();

        //        var foundTitle = from mediaTitle in entities.media
        //                         where mediaTitle.title == title
        //                         select mediaTitle;

        //        foreach (media mediaTitle in foundTitle)
        //            {
        //                mediaTitles.Add(mediaTitle);
        //            }

        //        return mediaTitles;
        //    }
        //}

        public List<string> GetMoviesByGenre(string genre)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                List<string> moviesByGenre = new List<string>();

                var movieByGenre = from mov in entities.video
                                   join med in entities.media on mov.media_id equals med.media_id
                                   select med.title;

                foreach (string mov in movieByGenre)
                {
                    moviesByGenre.Add(mov);
                }

                return moviesByGenre;
            }
        }

        //public media GetMedia(int id)
        //{
        //    using (var entities = _publishITEntities ?? new RentIt09Entities())
        //    {
        //        media foundMovies = (from med in entities.media
        //                            where med.media_id == id
        //                            select med).First();

        //        return foundMovies;
        //    }
        //}

        public int GetRating(int movieId, int userId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundRating =
                    entities.rating.SingleOrDefault(rate => rate.media_id == movieId && rate.user_id == userId);

                if (foundRating != null)
                {
                    return foundRating.rating1;
                }
                return -1;
            }
        }

        public bool PostRating(int rating, int movieId, int userId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundRating =
                    entities.rating.SingleOrDefault(rate => rate.media_id == movieId && rate.user_id == userId);

                if (foundRating == null)
                {
                    int id;

                    if (!entities.rating.Any())
                    {
                        id = 1;
                    }
                    else
                    {
                        id = entities.rating.Max(r => r.rating_id) + 1;
                    }


                    entities.rating.Add(new rating
                    {
                        rating_id = id,
                        rating1 = rating,
                        user_id = userId,
                        media_id = movieId
                    });

                    entities.SaveChanges();
                }
                else
                {
                    foundRating.rating_id = rating;
                    try
                    {
                        entities.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw new Exception();
                    }

                }

                return GetRating(movieId, userId) == rating;
            }
        }
    }
}
