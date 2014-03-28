using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PublishITService
{
    public class PublishITService : IPublishITService
    {
        public IPublishITEntities _publishITEntities { get; set; }

        public PublishITService(IPublishITEntities publishITEntities = null)
        {
            _publishITEntities = publishITEntities;
        }

        public UserDTO GetUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from user in entities.user
                                where user.name == inputUser.name
                                select new UserDTO()
                                {
                                    name = user.name,
                                    birthday = user.birthday,
                                    status = user.status,
                                    email = user.email,
                                    user_id = user.user_id,
                                    roles = ;
                                }).FirstOrDefault();
                if (foundUser != null && foundUser.status == )
                {
                    return foundUser;
                }

                return new UserDTO() {name = "No user found"};
            }
        }

        public bool RegisterUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new PublishITEntities())
            {
                int id;

                indsæt eventuelt et GetUser kald og se om den findes i forvejen (og overvej om returtypen skal være andet end bool)
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
                    role_id = ,
                    salt = ,
                    status = ,

                });

                entities.SaveChanges();
            }
            return GetUser(inputUser).name.Equals(inputUser.name);
        }

        public bool DeleteUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new PublishITEntities())
            {
                var foundUser = (from user in entities.user
                    where user.name == inputUser.name
                    select user).FirstOrDefault();

                entities.user.Remove(foundUser);

                try
                {
                    entities.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }

                return GetUser(inputUser).name.Equals("No user found");
            }
        }

        public bool EditUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new PublishITEntities())
            {
                var foundUser = (from user in entities.user
                                where user.name == inputUser.name
                                select user).FirstOrDefault();

                if (foundUser != null)
                {
                    foundUser.name = inputUser.name;
                    foundUser.password = inputUser.password;
                    foundUser.birthday = inputUser.birthday;
                    foundUser.email = inputUser.email;
                    foundUser.organization_id = inputUser.organization_id;
                    foundUser.salt = inputUser.salt;
                    foundUser.status = inputUser.status;
                    try
                    {
                        entities.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw;
                    }

                    return GetUser(inputUser).Equals();
                }
            }
        }

        public bool UploadMedia(File media)
        {
            bool success = true;
            return success;
        }

        public File DownloadMedia(int id)
        {
            throw new NotImplementedException();
        }

        public string StreamMedia(int id)
        {
            string mediaStreamed = "<video width='320' heigth='240' controls>" +
                                        "<source src='GetMoviesByGenre.mp4' type='video/mp4'>" +
                                        "<source='movie.ogg' type='video/ogg'>" +
                                   "</video>";
            return mediaStreamed;
        }

        public List<string> SearchMedia(string title)
        {
            List<string> dummyList = new List<string>();
            dummyList.Add("The movie with the title: " + title + " is found. This is just dummy data for testing");
            return dummyList;
        }

        public List<string> GetMoviesByGenre(string genre)
        {
            List<string> dummyList = new List<string>();
            dummyList.Add("A movie with the genre: " + genre + " is found. This is just dummy data for testing");
            return dummyList;
        }

        public string GetMedia(int id)
        {
            string movie = "this is a test movie with id: " + id;
            return movie;
        }

        public int GetRating(int movieId, int userId)
        {
            using (var entities = _publishITEntities ?? new PublishITEntities())
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
            using (var entities = _publishITEntities ?? new PublishITEntities())
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
                    //?? er det sådan man editer?
                    foundRating.rating_id = rating;
                    try
                    {
                        entities.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }

                return GetRating(movieId, userId) == rating;
            }
        }
    }
}
