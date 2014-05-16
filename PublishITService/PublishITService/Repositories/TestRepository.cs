using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PublishITService.DTOs;
using PublishITService.Parsers;

namespace PublishITService.Repositories
{
    public class TestRepository : IRepository
    {
        private readonly IPublishITEntities _publishITEntities;

        public TestRepository()
        {
            
        }

        public TestRepository(IPublishITEntities publishITEntities)
        {
            _publishITEntities = publishITEntities;
        }

        public UserDTO FindUserById(int id)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                                 where u.user_id == id
                                 select new UserDTO
                                 {
                                     name = u.name,
                                     username = u.user_name,
                                     birthday = u.birthday,
                                     status = u.status,
                                     email = u.email,
                                     user_id = u.user_id,
                                     password = u.password
                                 }).FirstOrDefault();

                if (foundUser != null)
                {
                    foundUser.roles = new List<RoleDTO>();

                    var roleList = (from r in entities.role
                                    from u in r.user
                                    where u.user_id == foundUser.user_id
                                    select r).ToList();

                    foreach (var r in roleList)
                    {
                        var x = new RoleDTO { Id = r.role_id, Title = r.role1 };
                        foundUser.roles.Add(x);
                    }

                    return foundUser;
                }
                return new UserDTO
                {
                    name = "No user found",
                    status = "Not a user"
                };
            }
        }

        public UserDTO FindUserByUsername(string username)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                                 where u.user_name == username
                                 select new UserDTO
                                 {
                                     name = u.name,
                                     username = u.user_name,
                                     birthday = u.birthday,
                                     status = u.status,
                                     email = u.email,
                                     user_id = u.user_id,
                                     password = u.password
                                 }).FirstOrDefault();


                if (foundUser != null)
                {
                    foundUser.roles = new List<RoleDTO>();

                    var roleList = (from r in entities.role
                                    from u in r.user
                                    where u.user_id == foundUser.user_id
                                    select r).ToList();

                    foreach (var r in roleList)
                    {
                        var x = new RoleDTO { Id = r.role_id, Title = r.role1 };
                        foundUser.roles.Add(x);
                    }

                    return foundUser;
                }

                return new UserDTO
                {
                    username = "No user found",
                    status = "Not a user"
                };
            }
        }

        public UserDTO FindUserByUsernameAndPassword(string username, string password)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                                 where u.user_name == username && u.password == password
                                 select new UserDTO
                                 {
                                     name = u.name,
                                     username = u.user_name,
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
                        var x = new RoleDTO { Id = r.role_id, Title = r.role1 };
                        foundUser.roles.Add(x);
                    }

                    return foundUser;
                }
                return new UserDTO { name = "Sign in failed" };
            }
        }

        public void AddUser(UserDTO newUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
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
                var userRole = (from r in entities.role
                                where r.role_id == 1
                                select r).FirstOrDefault();

                entities.user.Add(new user
                {
                    user_id = id,
                    name = newUser.name,
                    user_name = newUser.username,
                    password = newUser.password,
                    birthday = newUser.birthday,
                    email = newUser.email,
                    organization_id = newUser.organization_id,
                    salt = "salt",
                    status = "Active",
                    role = new Collection<role> { new role { role_id = userRole.role_id, role1 = userRole.role1 } }
                });

                entities.SaveChanges();
            }
        }

        public void DeleteUser(int id)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                                 where u.user_id == id
                                 select u).FirstOrDefault();

                if (foundUser != null) foundUser.status = "Deleted";

                try
                {
                    entities.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void EditUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from user in entities.user
                                 where user.user_id == inputUser.user_id
                                 select user).FirstOrDefault();

                if (foundUser != null)
                {
                    foundUser.name = inputUser.name;
                    foundUser.user_name = inputUser.username;
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
            }
        }

        public void StoreMedia(byte[] fileStream, RemoteFileInfo request, IMediaParser mediaParser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                mediaParser.StoreMedia(fileStream, request, entities);
            }
        }

        public string GetMediaPath(int id)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                return (from med in entities.media
                        where med.media_id == id
                        select med.location).FirstOrDefault();
            }
        }

        public media FindMediaById(int id)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundMedia = (from med in entities.media
                                  where med.media_id == id
                                  select med).FirstOrDefault();

                if (foundMedia != null)
                {
                    var theMedia = new media
                    {
                        media_id = foundMedia.media_id,
                        user_id = foundMedia.user_id,
                        format_id = foundMedia.format_id,
                        title = foundMedia.title,
                        average_rating = foundMedia.average_rating,
                        date = foundMedia.date,
                        description = foundMedia.description,
                        location = foundMedia.location,
                        number_of_downloads = foundMedia.number_of_downloads
                    };
                    return theMedia;
                }
                return new media
                {
                    title = "No media found"
                };
            }
        }

        public List<media> FindMediaByTitle(string title)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var medias = new List<media>();

                var foundMedia = from m in entities.media
                                 where m.title.Contains(title)
                                 select m;

                foreach (var med in foundMedia)
                {
                    medias.Add(new media
                    {
                        media_id = med.media_id,
                        user_id = med.user_id,
                        format_id = med.format_id,
                        title = med.title,
                        average_rating = med.average_rating,
                        date = med.date,
                        description = med.description,
                        location = med.location,
                        number_of_downloads = med.number_of_downloads
                    });
                }

                return medias;
            }
        }

        public IQueryable<genre> FindRelatedGenres(string inputGenre)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                return from genreName in entities.genre
                       where genreName.genre1.Contains(inputGenre)
                       select genreName;
            }
        }

        public List<media> FindMoviesByGenre(string inputGenre)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var medias = new List<media>();

                // Search for genres. Genre contains a collection of medias that represent the specific genre
                var foundgenre = FindRelatedGenres(inputGenre);


                foreach (var gen in foundgenre)
                {
                    // Traversal of the media collection in the specific genre.
                    foreach (var med in gen.media)
                    {
                        medias.Add(new media
                        {
                            media_id = med.media_id,
                            user_id = med.user_id,
                            format_id = med.format_id,
                            title = med.title,
                            average_rating = med.average_rating,
                            date = med.date,
                            description = med.description,
                            location = med.location,
                            number_of_downloads = med.number_of_downloads
                        });
                    }
                }

                return medias;
            }
        }

        public rating FindRating(int movieId, int userId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundRating =
                    entities.rating.SingleOrDefault(rate => rate.media_id == movieId && rate.user_id == userId);

                return foundRating;
            }
        }

        public ResponseMessage PostRating(int rating, int movieId, int userId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundRating = FindRating(movieId, userId);

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

                    return new ResponseMessage { IsExecuted = true, Message = "Rating added" };
                }

                foundRating.rating_id = rating;

                entities.SaveChanges();

                return new ResponseMessage { IsExecuted = true, Message = "Rating changed" };
            }
        }

        public bool CheckingIfRentExists(int userId, int movieId)
        {

            var date = DateTime.Now;
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var q = from r in entities.rent
                        where (r.media_id == movieId && r.user_id == userId && (r.start_date <= date && r.end_date >= date))
                        select r;

                if (q.Any())
                {
                    return true;
                }
            }
            return false;
        }
    }
}