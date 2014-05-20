using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        public UserDTO FindUserByUsernameAndPassword(string username, string password, int organizationId)
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

        public ResponseMessage AddUser(UserDTO newUser)
        {
            var foundUser = (from u in _userSet
                             where u.user_name == newUser.username
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

            if (foundUser == null)
            {
                int id = 4;
                
                var userRole = (from r in _roleSet
                                where r.role_id == 1
                                select r).FirstOrDefault();

                if (userRole != null)
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
                        status = "Active"
                    });
                
                var addedUser = (from u in _userSet
                                 where u.user_name == newUser.username
                                 select u).FirstOrDefault();

                if (addedUser != null && addedUser.user_name.Equals(newUser.username))
                {
                    return new ResponseMessage { IsExecuted = true, Message = "User registered" };
                }
                return new ResponseMessage { IsExecuted = false, Message = "Registration failed" };
            }
            return new ResponseMessage { IsExecuted = false, Message = "Username already exists" };
        }

        public ResponseMessage DeleteUser(int id)
        {
            var userToBeDeleted = (from u in _userSet
                    where u.user_id == id
                    select u).FirstOrDefault();

                if (userToBeDeleted != null && userToBeDeleted.status.Equals("Active"))
                {
                    userToBeDeleted.status = "Deleted";

                    
                }
                else
                {
                    return new ResponseMessage { IsExecuted = false, Message = "No user found. Deletion failed" };
                }

                var userThatShouldBeDeletedNow = (from u in _userSet
                    where u.user_id == id
                    select u).FirstOrDefault();

                if (userThatShouldBeDeletedNow != null && userThatShouldBeDeletedNow.status.Equals("Deleted"))
                {
                    return new ResponseMessage { IsExecuted = true, Message = "Deletion completed" };
                }
                return new ResponseMessage { IsExecuted = false, Message = "Deletion failed" };
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

        public void StoreMedia(Stream mediaStream, RemoteFileInfo mediaInfo, IMediaParser mediaParser)
        {
            string success = "let's say it has been stored";
        }

        public string GetMediaPath(int id)
        {
            return (from med in _mediaSet
                    where med.media_id == id
                    select med.location).FirstOrDefault();
        }

       

        public List<MediaDTO> FindMediaByTitle(string title, int organizationId)
        {
            var medias = new List<MediaDTO>();

            var foundMediaDoc = from med in _mediaSet
                                join u in _userSet on med.user_id equals u.user_id
                                join doc in _documentSet on med.media_id equals doc.media_id
                                where med.title.Contains(title) && u.organization_id == organizationId
                                select new { med, doc };

            var foundMediaVid = from med in _mediaSet
                                join u in _userSet on med.user_id equals u.user_id
                                join vid in _videoSet on med.media_id equals vid.media_id
                                where med.title.Contains(title) && u.organization_id == organizationId
                                select new { med, vid };

            if (foundMediaDoc.Any())
            {
                foreach (var med in foundMediaDoc)
                {
                    medias.Add(new MediaDTO
                    {
                        MediaId = med.med.media_id,
                        UserId = med.med.user_id,
                        FormatId = med.med.format_id,
                        Title = med.med.title,
                        AvgRating = med.med.average_rating,
                        Date = med.med.date,
                        Description = med.med.description,
                        Location = med.med.location,
                        NumberOfDownloads = med.med.number_of_downloads,
                        Status = med.doc.status
                    });
                }
            }
            if (foundMediaVid.Any())
            {
                foreach (var med in foundMediaVid)
                {
                    medias.Add(new MediaDTO
                    {
                        MediaId = med.med.media_id,
                        UserId = med.med.user_id,
                        FormatId = med.med.format_id,
                        Title = med.med.title,
                        AvgRating = med.med.average_rating,
                        Date = med.med.date,
                        Description = med.med.description,
                        Location = med.med.location,
                        NumberOfDownloads = med.med.number_of_downloads,
                        Length = med.vid.length,
                        NumberOfRents = med.vid.number_of_rents,
                        NumberOfTrailerViews = med.vid.number_of_trailer_views
                    });
                }
            }


            return medias;
        } 

        public MediaDTO FindMediaById(int id)
        {
            var foundMedia = (from med in _mediaSet
                              where med.media_id == id
                              join vid in _videoSet on med.media_id equals vid.media_id
                              join doc in _documentSet on med.media_id equals doc.media_id
                              select new MediaDTO
                              {
                                  MediaId = med.media_id,
                                  UserId = med.user_id,
                                  FormatId = med.format_id,
                                  Title = med.title,
                                  AvgRating = med.average_rating,
                                  Date = med.date,
                                  Description = med.description,
                                  Location = med.location,
                                  NumberOfDownloads = med.number_of_downloads,
                                  Length = vid.length,
                                  NumberOfRents = vid.number_of_rents,
                                  NumberOfTrailerViews = vid.number_of_trailer_views
                              }).FirstOrDefault();

            if (foundMedia != null)
            {
                return foundMedia;
            }
            return new MediaDTO
            {
                Title = "No media found"
            };
        } 

        public List<MediaDTO> FindMoviesByGenre(string inputGenre, int organizationId)
        {
                var gen = (from genreName in _genreSet
                    where genreName.genre1.Equals(inputGenre)
                    select genreName).FirstOrDefault();
                
                    // Traversal of the media collection in the specific genre.
                if (gen != null)
                {
                    var medias = (from med in gen.media
                        join vid in _videoSet on med.media_id equals vid.media_id
                        join doc in _documentSet on med.media_id equals doc.media_id
                        select new MediaDTO
                        {
                            MediaId = med.media_id,
                            UserId = med.user_id,
                            FormatId = med.format_id,
                            Title = med.title,
                            AvgRating = med.average_rating,
                            Date = med.date,
                            Description = med.description,
                            Location = med.location,
                            NumberOfDownloads = med.number_of_downloads,
                            Length = vid.length,
                            NumberOfRents = vid.number_of_rents,
                            NumberOfTrailerViews = vid.number_of_trailer_views,
                            Status = doc.status
                        }).ToList();
                    return medias;
                }

                return new List<MediaDTO>();
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

        public List<MediaDTO> FindMediasByAuthorId(int userId)
        {
            List<MediaDTO> medias = new List<MediaDTO>();


            var userMedia = from med in _mediaSet
                            where med.user_id == userId
                            select med;

            if (userMedia.Any())
            {
            var mediaIds = userMedia.Select(med => med.media_id)
                            .Union(_documentSet.Select(doc => doc.media_id))
                            .Union(_videoSet.Select(vid => vid.media_id));

            var foundMedia = from id in mediaIds
                             join med in _mediaSet on id equals med.media_id into mMed
                             from med in mMed.DefaultIfEmpty()
                             join doc in _documentSet on id equals doc.media_id into mDocs
                             from doc in mDocs.DefaultIfEmpty()
                             join vid in _videoSet on id equals vid.media_id into mVids
                             from vid in mVids.DefaultIfEmpty()
                             where doc == null ^ vid == null ^ med == null
                             select new { med, doc, vid };

            foreach (var med in foundMedia)
            {
                if (med.med != null)
                {
                    if (med.vid == null)
                    {
                        medias.Add(new MediaDTO
                        {
                            MediaId = med.med.media_id,
                            UserId = med.med.user_id,
                            FormatId = med.med.format_id,
                            Title = med.med.title,
                            AvgRating = med.med.average_rating,
                            Date = med.med.date,
                            Description = med.med.description,
                            Location = med.med.location,
                            NumberOfDownloads = med.med.number_of_downloads,
                            Status = med.doc.status
                        });
                    }
                    if (med.doc == null)
                    {
                        medias.Add(new MediaDTO
                        {
                            MediaId = med.med.media_id,
                            UserId = med.med.user_id,
                            FormatId = med.med.format_id,
                            Title = med.med.title,
                            AvgRating = med.med.average_rating,
                            Date = med.med.date,
                            Description = med.med.description,
                            Location = med.med.location,
                            NumberOfDownloads = med.med.number_of_downloads,
                            Length = med.vid.length,
                            NumberOfRents = med.vid.number_of_rents,
                            NumberOfTrailerViews = med.vid.number_of_trailer_views
                        });
                    }
                }
            }
        }

            return medias;
        }

        public List<MediaDTO> FindMediasByAuthorName(string username, int organizationId)
        {
            List<MediaDTO> medias = new List<MediaDTO>();

            var userMedia = from med in _mediaSet
                            join u in _userSet on med.user_id equals u.user_id
                            where u.user_name.Contains(username) && u.organization_id == organizationId
                            select med;

            if (userMedia.Count() > 0)
            {
                var mediaIds = userMedia.Select(med => med.media_id)
                    .Union(_documentSet.Select(doc => doc.media_id))
                    .Union(_videoSet.Select(vid => vid.media_id));

                var foundMedia = from id in mediaIds
                                 join med in _mediaSet on id equals med.media_id into mMed
                                 from med in mMed.DefaultIfEmpty()
                                 join doc in _documentSet on id equals doc.media_id into mDocs
                                 from doc in mDocs.DefaultIfEmpty()
                                 join vid in _videoSet on id equals vid.media_id into mVids
                                 from vid in mVids.DefaultIfEmpty()
                                 where doc == null ^ vid == null ^ med == null
                                 select new { med, doc, vid };


                foreach (var med in foundMedia)
                {
                    if (med.med != null)
                    {
                        if (med.vid == null)
                        {
                            medias.Add(new MediaDTO
                            {
                                MediaId = med.med.media_id,
                                UserId = med.med.user_id,
                                FormatId = med.med.format_id,
                                Title = med.med.title,
                                AvgRating = med.med.average_rating,
                                Date = med.med.date,
                                Description = med.med.description,
                                Location = med.med.location,
                                NumberOfDownloads = med.med.number_of_downloads,
                                Status = med.doc.status
                            });
                        }
                        if (med.doc == null)
                        {
                            medias.Add(new MediaDTO
                            {
                                MediaId = med.med.media_id,
                                UserId = med.med.user_id,
                                FormatId = med.med.format_id,
                                Title = med.med.title,
                                AvgRating = med.med.average_rating,
                                Date = med.med.date,
                                Description = med.med.description,
                                Location = med.med.location,
                                NumberOfDownloads = med.med.number_of_downloads,
                                Length = med.vid.length,
                                NumberOfRents = med.vid.number_of_rents,
                                NumberOfTrailerViews = med.vid.number_of_trailer_views
                            });
                        }
                    }
                }
            }

            return medias;
        }

        public void AddAdminAsRole(int userId)
        {
            string added = "Admin role added";
        }
    }
}