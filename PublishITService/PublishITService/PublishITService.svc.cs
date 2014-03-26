using System;
using System.Collections.Generic;
using System.IO;

namespace PublishITService
{
    public class PublishITService : IPublishITService
    {
        public string GetUser(string userName, string password)
        {
            using (var entities = _publishITEntities ?? new PublishITEntities())
            {
                var foundUser = from user in entities.User
                                where user.Name == userName
                                select new
                                {
                                    user.Name,
                                    user.Password,
                                    user.birthday,
                                    user.gender
                                };

                var userString = foundUser.Name + "; " + foundUser.Password + "; " + foundUser.birthday + "; " + foundUser.gender;

                return userString;
            }
        }

        public bool RegisterUser(string userName, string password, string birthday, int gender)
        {
            using (var entities = _publishITEntities ?? new PublishITEntities())
            {
                int id;

                if (!entities.User.Any())
                {
                    id = 1;
                }
                else
                {
                    id = entities.User.Max(u => u.Id) + 1;
                }
                entities.User.Add(new User
                {
                    Id = id,
                    name = name,
                    password = password,
                    birthday = birthday,
                    gender = gender
                });

                entities.SaveChanges();
            }
            return GetUser(userName, password).Equals(userName + "; " + password + "; " + birthday + "; " + gender);
        }

        public bool DeleteUser(string userName, string password)
        {
            using (var entities = _publishITEntities ?? new PublishITEntities())
            {
                var foundUser = from user in entities.User
                    where user.Name == userName
                    select user;

                entities.User.DeleteOnSubmit(foundUser);

                try
                {
                    entities.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }

                return something
            }
        }

        public bool EditUser(string userName, string password, string birthday, int gender)
        {
            using (var entities = _publishITEntities ?? new PublishITEntities())
            {
                var foundUser = from user in entities.User
                                where user.Name == userName
                                select user;

                foundUser.name = userName;
                foundUser.password = password;
                foundUser.birthday = birthday;
                foundUser.gernder = gender;

                try
                {
                    entities.SubmitChanges();
                }
                catch (Exception e)
                {
                    throw;
                }

                return GetUser(userName, password).Equals(foundUser.Name + "; " + foundUser.Password + "; " + foundUser.birthday + "; " + foundUser.gender);
            }
        }

        public bool UploadMedia(File media)
        {
            throw new NotImplementedException();
        }

        public File DownloadMedia(int id)
        {
            throw new NotImplementedException();
        }

        public Stream StreamMedia(int id)
        {
            throw new NotImplementedException();
        }

        public List<string> SearchMedia(string title)
        {
            throw new NotImplementedException();
        }

        public List<string> GetMoviesByGenre(string genre)
        {
            throw new NotImplementedException();
        }

        public string GetMedia(int id)
        {
            throw new NotImplementedException();
        }

        public int? GetRating(int movieId, int userId)
        {
            using (var entities = _publishITEntities ?? new PublishITEntities())
            {
                var foundRating =
                    entities.Rating.SingleOrDefault(rate => rate.movie_id == movieId && rate.user_Id == userId);

                return foundRating;
            }
        }

        public bool PostRating(int rating, int movieId, int userId)
        {
            using (var entities = _publishITEntities ?? new PublishITEntities())
            {
                var foundRating =
                    entities.Rating.SingleOrDefault(rate => rate.movie_id == movieId && rate.user_Id == foundUserId);

                if (foundRating == null)
                {
                    int id;

                    if (!entities.Rating.Any())
                    {
                        id = 1;
                    }
                    else
                    {
                        id = entities.Rating.Max(u => u.id) + 1;
                    }


                    entities.Rating.Add(new Rating
                    {
                        id = id,
                        rating1 = rating,
                        // FindUserIdFromUsername ville kun være relevant hvis de ikke kan sende id (som det ser ud nu sender vi ikke et userId til dem)
                        user_Id = FindUserIdFromUsername(userId),
                        movie_id = data.MovieId
                    });

                    entities.SaveChanges();
                }
                else
                {
                    // FindUserIdFromUsername ville kun være relevant hvis de ikke kan sende id (som det ser ud nu sender vi ikke et userId til dem)
                    int foundUserId = FindUserIdFromUsername(userId);

                    foundRating.RatingId = rating;
                    try
                    {
                        entities.SubmitChanges();
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
