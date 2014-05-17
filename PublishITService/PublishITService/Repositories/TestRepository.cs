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
        public List<user> _userSet;
        public List<rating> _ratingSet;
        public List<role> _roleSet;
        public List<document> _documentSet;
        public List<media> _mediaSet;
        public List<video> _videoSet;
        public List<rent> _rentSet;
        public List<genre> _genreSet;

        public UserDTO FindUserById(int id)
        {
            var foundUser = (from u in _userSet
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

                var roleList = (from r in _roleSet
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

        public UserDTO FindUserByUsername(string username)
        {
            var foundUser = (from u in _userSet
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

                var roleList = (from r in _roleSet
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

        public UserDTO FindUserByUsernameAndPassword(string username, string password)
        {
            var foundUser = (from u in _userSet
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

                var roleList = (from r in _roleSet
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

        public void AddUser(UserDTO newUser)
        {
            int id = 4;

            _userSet.Add(new user
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
                role = new Collection<role> { new role { role_id = 1, role1 = "Role 1" } }
            });
        }

        public void DeleteUser(int id)
        {
            var foundUser = (from u in _userSet
                             where u.user_id == id
                             select u).FirstOrDefault();

            if (foundUser != null) foundUser.status = "Deleted";
        }

        public void EditUser(UserDTO inputUser)
        {
            var foundUser = (from user in _userSet
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
        }

        public void StoreMedia(byte[] mediaStream, RemoteFileInfo mediaInfo, IMediaParser mediaParser)
        {
            string success = "let's say it has been stored";
        }

        public string GetMediaPath(int id)
        {
            return (from med in _mediaSet
                    where med.media_id == id
                    select med.location).FirstOrDefault();
        }

        public media FindMediaById(int id)
        {
            var foundMedia = (from med in _mediaSet
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

        public List<media> FindMediaByTitle(string title)
        {
            var medias = new List<media>();

            var foundMedia = from m in _mediaSet
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

        public List<media> FindMoviesByGenre(string inputGenre)
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

        public IQueryable<genre> FindRelatedGenres(string inputGenre)
        {
            return (from genreName in _genreSet
                   where genreName.genre1.Contains(inputGenre)
                   select genreName).AsQueryable();
        }

        public rating FindRating(int movieId, int userId)
        {
            var foundRating =
                    _ratingSet.SingleOrDefault(rate => rate.media_id == movieId && rate.user_id == userId);

            return foundRating;
        }

        public ResponseMessage PostRating(int rating, int movieId, int userId)
        {
            var foundRating = FindRating(movieId, userId);

            if (foundRating == null)
            {
                int id = 4;


                _ratingSet.Add(new rating
                {
                    rating_id = id,
                    rating1 = rating,
                    user_id = userId,
                    media_id = movieId
                });

                return new ResponseMessage { IsExecuted = true, Message = "Rating added" };
            }

            foundRating.rating_id = rating;

            return new ResponseMessage { IsExecuted = true, Message = "Rating changed" };
        }

        public bool CheckingIfRentExists(int userId, int movieId)
        {
            var date = DateTime.Now;
            
                var q = from r in _rentSet
                        where (r.media_id == movieId && r.user_id == userId && (r.start_date <= date && r.end_date >= date))
                        select r;

                if (q.Any())
                {
                    return true;
                }
            
            return false;
        }

        public List<media> FindMediasByAuthorId(int id)
        {
            List<media> medias = new List<media>();

            var foundMedia = from med in _mediaSet
                             where med.user_id == id
                             select med;

            foreach (media med in foundMedia)
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
}